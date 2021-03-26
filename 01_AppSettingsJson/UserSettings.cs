using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System;
using System.IO;

namespace _01_AppSettingsJson
{
    public class UserSettings<T> where T : IUserSettingsModel, new()
    {
        private string _configFilePath;

        public string ConfigFilePath
        {
            get { return _configFilePath; }
            set
            {
                if (File.Exists(_configFilePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(value));
                    File.Move(_configFilePath, value, overwrite: true);
                }
                _configFilePath = value;
            }
        }

        public bool IsHidden { get; set; } = true;

        public T Settings { get; set; } = new T();

        public UserSettings(string configFilePath = @".\.config")
        {
            if (string.IsNullOrWhiteSpace(configFilePath))
                throw new ArgumentException();
            ConfigFilePath = configFilePath;
            Read();
        }
        public string GetVersion()
        {
            using (FileStream fs = new FileStream(ConfigFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader file = new StreamReader(fs))
                {
                    var fileContent = file.ReadToEnd();
                    var data = (JObject)JsonConvert.DeserializeObject(fileContent);
                    return data["Version"].Value<string>();
                }
            }
        }

        public bool Read()
        {
            if (!File.Exists(ConfigFilePath))
                return false;
            using (FileStream fs = new FileStream(ConfigFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader file = new StreamReader(fs))
                {
                    var fileContent = file.ReadToEnd();
                    if (IsValidJson(fileContent))
                    {
                        Settings = JsonConvert.DeserializeObject<T>(fileContent);
                        return true;
                    }
                    return false;
                }
            }
        }

        public bool Read(string configFilePath)
        {
            ConfigFilePath = configFilePath;
            return Read();
        }

        public void Save()
        {
            if (!IsHidden && File.Exists(ConfigFilePath))
                File.SetAttributes(ConfigFilePath, FileAttributes.Normal);


            //open file stream
            using (FileStream fs = File.Exists(ConfigFilePath) ?
                new FileStream(ConfigFilePath, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite) :
                new FileStream(ConfigFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                using (StreamWriter file = new StreamWriter(fs))
                {
                    if (IsHidden)
                    {
                        File.SetAttributes(ConfigFilePath, FileAttributes.Hidden);
                    }
                    //serialize object directly into file stream
                    var jsonString = JsonConvert.SerializeObject(Settings, Formatting.Indented);
                    file.Write(jsonString);
                }
            }
        }

        private static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Debug.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Debug.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}