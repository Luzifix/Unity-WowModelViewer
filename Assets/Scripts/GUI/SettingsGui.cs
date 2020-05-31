using System.Collections.Generic;
using System.IO;
using Constants;
using Newtonsoft.Json;
using Settings;
using SFB;
using UnityEngine;
using UnityEngine.UI;
using Util;
using Button = UnityEngine.UI.Button;

namespace GUI
{
    public class SettingsGui : MonoBehaviour
    {
        public Dropdown LoadType;
        public Dropdown OnlineBranch;
        public Dropdown LocalBranch;
        public InputField LocalPath;

        public Button OkayButton;
        public Button CancelButton;
        public Button OpenStoragebutton;

        private List<string> onlineBranches = new List<string> { "wow_beta", "wowt", "wow" };

        public void Start()
        {
            if (!SettingsManager<ModelViewerConfig>.IsInitialized)
                SettingsManager<ModelViewerConfig>.Initialize("settings.json");

            var config = SettingsManager<ModelViewerConfig>.Config;
            
            // Initialize all settings.
            LoadType.value     = (int)config.LoadType;
            OnlineBranch.value = onlineBranches.IndexOf(config.OnlineBranch);
            LocalPath.text     = config.LocalStorage;

            if (config.LocalStorage != string.Empty && File.Exists($"{config.LocalStorage}/.build.info"))
            {
                var localBranches = Utilities.GetLocalBranch($"{config.LocalStorage}/.build.info");
                foreach (var branch in localBranches)
                    LocalBranch.options.Add(new Dropdown.OptionData(branch));

                LocalBranch.value = localBranches.IndexOf(config.LocalBranch);
            }

            OkayButton.onClick.AddListener(OnSaveSettings);
            CancelButton.onClick.AddListener(OnSettingsClose);
            OpenStoragebutton.onClick.AddListener(OnOpenStorage);
        }

        private void OnSaveSettings()
        {
            var config = new ModelViewerConfig
            {
                LoadType     = (CascLoadType) LoadType.value,
                LocalBranch  = LocalBranch.options[LocalBranch.value].text,
                LocalStorage = LocalPath.text,
                OnlineBranch = onlineBranches[OnlineBranch.value]
            };

            // Check if the config is the same as the loaded config.
            if (config != SettingsManager<ModelViewerConfig>.Config)
            {
                // Replace all the contents of settings with the new settings
                // and reload the SettingsManager with the new settings.
                File.WriteAllText("settings.json", JsonConvert.SerializeObject(config, Formatting.Indented));
                SettingsManager<ModelViewerConfig>.Initialize("settings.json");
            }

            // Close the settings window.
            OnSettingsClose();
        }

        private void OnOpenStorage()
        {
            var paths = StandaloneFileBrowser.OpenFolderPanel("Select WoW Folder", "", false);
            foreach (var path in paths)
            {
                if (!Directory.Exists($"{path}/Data"))
                {
                    Debug.Log("Invalid WoW Folder!");
                }

                var localBranches = Utilities.GetLocalBranch($"{path}/.build.info");
                foreach (var branch in localBranches)
                    LocalBranch.options.Add(new Dropdown.OptionData(branch));

                LocalPath.text = path;
                LocalBranch.value = 0;
            }
        }

        private void OnSettingsClose() => gameObject.SetActive(false);
    }
}