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
            Directory.CreateDirectory(Folder);
        }

        public void Save()
        {
            File.WriteAllText(DataFileName, Data.ToJsonStr());
        }

        public ClientData Data => _clientState ?? ( _clientState = GetClientState());

        private ClientData GetClientState() 
        {
            if (!File.Exists(DataFileName)) 
                return new ClientData();

            var dataText = File.ReadAllText(DataFileName);
            var data = dataText.FromJsonStr<ClientData>();

            return data; 
        }
    }
}
