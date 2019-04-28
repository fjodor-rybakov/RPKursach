using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Config
{
    public class Config
    {
        private const string ConfigPath = @"../../../config.json";

        public static ConfigModel GetConfig()
        {
            using (var reader = new StreamReader(ConfigPath))
            {
                var json = reader.ReadToEnd();
                var config = JsonConvert.DeserializeObject<ConfigModel>(json);

                return config;
            }
        }
    }
}
