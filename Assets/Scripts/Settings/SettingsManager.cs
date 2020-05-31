using System.IO;
using Newtonsoft.Json;

namespace Settings
{
    public static class SettingsManager<T>
    {
        public static bool IsInitialized { get; private set; } = false;
        public static T Config { get; private set; }

        public static void Initialize(string filename)
        {
            var fileContent = File.ReadAllText(filename);
            Config = JsonConvert.DeserializeObject<T>(fileContent);

            IsInitialized = true;
        }
    }
}