using System.IO;
using Newtonsoft.Json;
using Settings;
using UnityEngine;
using World.Model;

namespace World
{
    public class WorldLoader : MonoBehaviour
    {
        private M2Handler m2Handler;
        
        public void Start()
        {
            // Initialize settings.
            InitializeSettings();

            var m2Parent = GameObject.Find("M2");
            m2Handler = new M2Handler(m2Parent);
        }

        public void Update()
        {
            m2Handler?.Update();
        }

        private void InitializeSettings()
        {
            if (!File.Exists("settings.json"))
            {
                var config = new ModelViewerConfig();
                File.WriteAllText("settings.json", JsonConvert.SerializeObject(config, Formatting.Indented));
            }
            
            SettingsManager<ModelViewerConfig>.Initialize("settings.json");
        }
    }
}