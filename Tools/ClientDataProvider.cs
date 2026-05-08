using Perfecto.Deploy.Extensions;
using PerfectohubRu.Model;
using System;
using System.IO;

namespace PerfectohubRu.Tools
{
    public class ClientDataProvider
    {
        private ClientData _clientState;

        private string Folder => Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Perfecto",
                "PerfectoRu"
            );

        private string DataFileName => Path.Combine(Folder, "data.json");

        public ClientDataProvider()
        {
            try
            {
                Directory.CreateDirectory(Folder);
            }
            catch (Exception ex) 
            {
                Data.CriticalError = ex.Message;
            }
        }

        public void Save()
        {
            try
            {
                File.WriteAllText(DataFileName, Data.ToJsonStr());
            }
            catch (Exception ex)
            {
                Data.CriticalError = ex.Message;
            }
        }

        public ClientData Data => _clientState ?? ( _clientState = GetClientState());

        private ClientData GetClientState() 
        {
            try
            {
                if (!File.Exists(DataFileName))
                    return new ClientData();

                var dataText = File.ReadAllText(DataFileName);
                var data = dataText.FromJsonStr<ClientData>();

                return data;
            }
            catch (Exception ex)
            {
                var data = new ClientData() { CriticalError = ex.Message };

                return data;
            }
        }
    }
}
