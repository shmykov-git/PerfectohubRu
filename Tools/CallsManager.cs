using Calls.HttpClients.Abstractions;
using Calls.Model.Ats;
using Calls.Model.Libraries;
using Microsoft.Extensions.Options;
using Perfecto.Deploy.Extensions;
using PerfectohubRu.Model;
using Shared.Extensions;
using Shared.Libraries;
using Shared.Model.Enums;
using Shared.Model.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PerfectohubRu.Tools
{

    public class CallsManager
    {
        private readonly IAtsHttpClient httpClient;
        private readonly ClientDataProvider dataProvider;
        private readonly ClientOptions clientOptions;
        private ClientData data;

        public CallsManager(IAtsHttpClient atsHttpClient, ClientDataProvider dataProvider, IOptions<ClientOptions> clientOptions)
        {
            this.httpClient = atsHttpClient;
            this.dataProvider = dataProvider;
            this.clientOptions = clientOptions.Value;
            data = dataProvider.Data;
        }

        public async Task<string[]> GetUniqueCallsMessage(bool split, bool hasHtml, bool splitLine, CancellationToken token)
        {
            var utcNow = DateTime.UtcNow;
            var marks = clientOptions.Marks;

            var nowClient = clientOptions.GetClientTime(utcNow).RoundToNearestSecond();
            var rightnowClient = clientOptions.GetClientTime().RoundToNearestSecond();
            var todayClient = nowClient.Date;
            var today = nowClient.Date;
            var todayFromClientUtc = today.ToUniversalTime().RoundToNearestSecond();

            var nDay = 1;
            var isOneDay = nDay == 1;
            var isToday = today == rightnowClient.Date;

            var calls = await GetUniqueCalls(utcNow, token);
            var uniqueCallsCount = calls.Length;
            var skippedCount = calls.Where(c => c.IsBotMessage).Count();

            var msgPeriod = hasHtml 
                ?
                    (isOneDay
                        ? $"На <b>{todayClient.ToString("dd.MM.yyyy")}</b> <b>{(isToday ? nowClient.ToString("HH:mm") : "")}</b>"
                        : $"На период <b>с {todayClient.AddDays(1 - nDay).ToString("dd.MM.yyyy")} по {todayClient.ToString("dd.MM.yyyy")}</b>")
                :
                    (isOneDay
                        ? $"На {todayClient.ToString("dd.MM.yyyy")} {(isToday ? nowClient.ToString("HH:mm") : "")}"
                        : $"На период с {todayClient.AddDays(1 - nDay).ToString("dd.MM.yyyy")} по {todayClient.ToString("dd.MM.yyyy")}")
                ;

            string GetRepeat(string mark, int count) => Enumerable.Range(0, count).Select(_ => mark).SJoin("");

            string GetCallTryMessage(UniqueCall c)
            {
                var phone = c.Phone;
                var triesCount = c.Count > 1 ? $" ({c.Count})" : "";
                var time = isOneDay
                    ? c.LastTime.ToString("HH:mm ")
                    : c.LastTime.ToString("dd.MM HH:mm ");
                var successTime = c.SuccessLastTime.HasValue
                    ? (isOneDay || c.LastTime.Date == c.SuccessLastTime.Value.Date
                        ? c.SuccessLastTime.Value.ToString("|HH:mm ")
                        : c.SuccessLastTime.Value.ToString("|dd.MM HH:mm "))
                    : null;
                var tries = c.RecallTryCount > 0 ? $" {GetRepeat(marks[MarkType.Repeat], c.RecallTryCount)}" : "";
                var inbound = c.HasInbound ? $"{marks[MarkType.Inbound]}" : "";

                return hasHtml
                    ? $"{phone}<b>{triesCount}{inbound} {time}{successTime}</b>➤\n   {c.Whom.SJoin($"\n   ")}{tries}"
                    : $"{phone}{triesCount}{inbound} {time}{successTime}➤\n   {c.Whom.SJoin($"\n   ")}{tries}";
            }

            // show not excepted and skipped and not replied calls
            var msgSkippedCalls = calls
                    .Where(c => c.IsBotMessage)
                    .Select(GetCallTryMessage)
                    .SJoin(splitLine ? "\n\n" : "\n");

            var msg = hasHtml
                ? 
(skippedCount > 0
    ? $@"{msgPeriod}
Количество уникальных звонков <b>{uniqueCallsCount}</b>
Из них без ответа <b>{skippedCount}</b> (<b>{100.0 * skippedCount / uniqueCallsCount:F0}%</b>):{(splitLine ? "\n" : "")}
{msgSkippedCalls}
" : (uniqueCallsCount > 0 ? $@"
{msgPeriod}
Количество уникальных звонков <b>{uniqueCallsCount}</b>
Все звонки были обработаны {marks[MarkType.Check]}
" : $@"
{msgPeriod}
Звонков не было {marks[MarkType.Check]}
"))
                :
(skippedCount > 0
    ? $@"{msgPeriod}
Количество уникальных звонков {uniqueCallsCount}
Из них без ответа {skippedCount} ({100.0 * skippedCount / uniqueCallsCount:F0}%):{(splitLine ? "\n" : "")}
{msgSkippedCalls}
" : (uniqueCallsCount > 0 ? $@"
{msgPeriod}
Количество уникальных звонков {uniqueCallsCount}
Все звонки были обработаны {marks[MarkType.Check]}
" : $@"
{msgPeriod}
Звонков не было {marks[MarkType.Check]}
"))
;

            if (msg.Length <= clientOptions.MaxMessageSize || !split)
            {
                return new string[] { msg };
            }
            else
            {
                return msg.SplitTextByLines(clientOptions.MaxMessageSize).ToArray();
            }
        }

        public async Task<UniqueCall[]> GetUniqueCalls(DateTime nowUtc, CancellationToken token)
        {
            var todayUtc = nowUtc.ToLocalTime().Date.ToUniversalTime();
            var view = await httpClient.GetCalls(todayUtc, todayUtc.AddDays(1));
            var calls = await GetUniqueSkippedCalls(view);

            return calls;
        }

        private async Task<UniqueCall[]> GetUniqueSkippedCalls(AtsCall[] atsCalls)
        {
            var actives = data.Actives.Select(x => x.Phone).Concat(data.Commons).Concat(new string[] { Values.UnknownPhone }).ToHashSet();
            var allKnowns = data.Knowns.Concat(actives).ToHashSet();
            data.AllKnowns = allKnowns;

            int GetRecallTriesCount(string phone, DateTime timeUtc) =>
                atsCalls
                    .Where(Filters.Out)
                    .Where(c => c.Duration == 0)
                    .Where(c => c.Time > timeUtc)
                    .Where(c => c.ClientPhone == phone)
                    .Count();

            var recalls = atsCalls
                .Where(Filters.Out)
                .Where(c => c.Duration > 0)
                .GroupBy(c => c.ClientPhone)
                .ToDictionary(gc => gc.Key, gc => gc.Max(c => c.Time));

            // Фильтр входящих звонков от не сервисных телефонов
            // Сервисные телефоны - это полный список телефонов в паспорте
            var externalInFn = Filters.ExternalIn(allKnowns, actives);

            // Входящие телефоны, которых нет в паспорте (звонка водителя, тут нет)
            var calls = atsCalls
                .Where(externalInFn)
                .GroupBy(c => c.ClientPhone)
                .Select(gc => new
                {
                    Phone = gc.Key,
                    Duration = gc.Sum(c => c.Duration),
                    Count = gc.Count(),
                    SuccessCallTimeUtc = (gc.Where(Filters.Received).Select(c => c.Time).MaxOrMinValue(),
                                          recalls.TryGetValue(gc.Key, out var recallTime) ? recallTime : DateTime.MinValue.KindOfUtc()).Max(),
                    HasInbound = gc.Any(c => HasServiceInbound(c)),
                    Whom = gc.Select(c => GetServiceName(c)).Distinct().ToArray(),
                    LastTimeUtc = gc.Select(c => c.Time).Max(),
                    Calls = gc.ToArray()
                })
                .Select(c => new
                {
                    c.Phone,
                    c.Duration,
                    c.Count,
                    c.SuccessCallTimeUtc,
                    c.HasInbound,
                    IsMissed = c.Calls.Where(cc => c.SuccessCallTimeUtc < cc.Time).Count(Filters.Missed) > 0,   // не дозвонился (после успешного созвона)
                    IsRecalled = c.LastTimeUtc < c.SuccessCallTimeUtc,
                    RecallTryCount = GetRecallTriesCount(c.Phone, c.LastTimeUtc),
                    c.Whom,
                    LastTime = c.LastTimeUtc.ToLocalTime(),
                })
                .Select(c => new UniqueCall
                {
                    Phone = c.Phone,
                    Duration = c.Duration,
                    Count = c.Count,
                    RecallTryCount = c.RecallTryCount,  // количество перезвонов (после успешного созвона)
                    HasInbound = c.HasInbound,
                    Whom = c.Whom,                      // список сотрудников, которым звонил клиент
                    LastTime = c.LastTime,              // локальное время последнего звонка клиента
                    SuccessLastTime = c.SuccessCallTimeUtc == DateTime.MinValue.KindOfUtc()
                        ? null
                        : (DateTime?)c.SuccessCallTimeUtc.ToLocalTime(),    // локальное время последнего звонка
                                                                                // Бот покажет сообщение, если
                    IsBotMessage = 
                                   c.IsMissed &&        // сервис не поднял трубку после (после успешного созвона)
                                   !c.IsRecalled &&     // сервис не дозвонился или не перезвонил (после успешного созвона)
                                                        // сервис пытался перезвонить недостаточное число раз (после успешного созвона)
                                   IsNotReplied(c.RecallTryCount)
                })
                .OrderByDescending(v => v.LastTime)
                .ToArray();

            return calls;
        }

        public bool IsReplied(int recallTryCount) => !IsNotReplied(recallTryCount);
        public bool IsNotReplied(int recallTryCount) => recallTryCount < clientOptions.RecallClientLimit;

        private bool HasServiceInbound(AtsCall call) =>
            call.CompanyInboundPhone != null &&
            data.AllKnowns.Contains(call.CompanyInboundPhone);

        private string GetServicePhone(AtsCall call)
        {
            return call.CompanyInboundPhone != null && data.AllKnowns.Contains(call.CompanyInboundPhone)
                ? call.CompanyInboundPhone
                : (data.AllKnowns.Contains(call.CompanyPhone) ? call.CompanyPhone : Values.UnknownPhone);
        }

        private string GetServiceName(AtsCall call)
        {
            var phone = GetServicePhone(call);

            var active = data.Actives.FirstOrDefault(v=>v.Phone == phone);

            if (active != null)
                return active.Name;

            var common = data.Commons.FirstOrDefault(v => v == phone);

            if (common != null)
                return $"Общий номер {common}";

            return phone;
        }
    }
}
