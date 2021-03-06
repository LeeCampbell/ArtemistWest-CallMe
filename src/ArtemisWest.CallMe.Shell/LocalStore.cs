using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using ArtemisWest.CallMe.Contract;
using Newtonsoft.Json;

namespace ArtemisWest.CallMe.Shell
{
    //TODO: make thread safe.
    public class LocalStore : ILocalStore
    {
        private const string FileKey = "LocalStoreSettings";
        private readonly Dictionary<string, string> _data = new Dictionary<string, string>();

        public LocalStore()
        {
            var isolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly();
            using (var srReader = new StreamReader(new IsolatedStorageFileStream(FileKey, FileMode.OpenOrCreate, isolatedStorage)))
            {
                string payload = srReader.ReadToEnd();
                _data = JsonConvert.DeserializeObject<Dictionary<string, string>>(payload)
                    ?? new Dictionary<string, string>();
                srReader.Close();
            }
        }

        public string Get(string key)
        {
            string value;
            _data.TryGetValue(key, out value);
            return value;
        }

        public void Put(string key, string value)
        {
            _data[key] = value;
            SaveData();
        }

        public void Remove(string key)
        {
            _data.Remove(key);
            SaveData();
        }

        private void SaveData()
        {
            var isolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly();

            //create a stream writer object to write content in the location
            using (var srWriter = new StreamWriter(new IsolatedStorageFileStream(FileKey, FileMode.Create, isolatedStorage)))
            {
                var payload = JsonConvert.SerializeObject(_data, Formatting.Indented);
                srWriter.Write(payload);
                srWriter.Flush();
                srWriter.Close();
            }
        }
    }
}