using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Extensions
{
    public static class StringSplitExtensions
    {
        /// <summary>
        /// Разбивает текст на строки, затем компонует их в блоки указанного максимального размера
        /// </summary>
        /// <param name="text">Исходный текст</param>
        /// <param name="maxLength">Максимальная длина блока (по умолчанию 4096 для Telegram)</param>
        /// <returns>Список блоков текста, не превышающих maxLength</returns>
        public static List<string> SplitTextByLines(this string text, int maxLength = 4096)
        {
            if (string.IsNullOrEmpty(text))
                return new List<string>();

            // Если весь текст помещается в лимит - возвращаем как есть
            if (text.Length <= maxLength)
                return new List<string> { text };

            var result = new List<string>();

            // Разбиваем исходный текст на строки (сохраняем символы перевода строки)
            string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            var currentBlock = new StringBuilder();

            foreach (string line in lines)
            {
                // Проверяем, поместится ли текущая строка в текущий блок
                // +1 для учета символа новой строки, который мы добавим
                int newLineLength = currentBlock.Length > 0 ? Environment.NewLine.Length : 0;

                if (currentBlock.Length + newLineLength + line.Length <= maxLength)
                {
                    // Строка помещается - добавляем её в текущий блок
                    if (currentBlock.Length > 0)
                        currentBlock.Append(Environment.NewLine);

                    currentBlock.Append(line);
                }
                else
                {
                    // Строка не помещается в текущий блок
                    if (currentBlock.Length > 0)
                    {
                        // Сохраняем текущий блок, если он не пуст
                        result.Add(currentBlock.ToString());
                        currentBlock.Clear();
                    }

                    // Проверяем, не превышает ли сама строка максимальную длину
                    if (line.Length > maxLength)
                    {
                        // Если строка слишком длинная - разбиваем её принудительно
                        string remainingLine = line;
                        while (remainingLine.Length > maxLength)
                        {
                            result.Add(remainingLine.Substring(0, maxLength));
                            remainingLine = remainingLine.Substring(maxLength);
                        }

                        // Остаток строки становится началом нового блока
                        if (remainingLine.Length > 0)
                        {
                            currentBlock.Append(remainingLine);
                        }
                    }
                    else
                    {
                        // Строка нормальной длины - начинаем с неё новый блок
                        currentBlock.Append(line);
                    }
                }
            }

            // Добавляем последний блок, если он не пуст
            if (currentBlock.Length > 0)
                result.Add(currentBlock.ToString());

            return result;
        }
    }
}