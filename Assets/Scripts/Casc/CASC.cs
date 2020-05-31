using System.ComponentModel;
using System.IO;
using CASCLib;
using Constants;
using GUI.Components;
using Settings;
using UnityEngine;

namespace Casc
{
    public static class CASC
    {
        private static CASCHandler cascHandler;
        private static BackgroundWorker cascWorker;
        
        private static GameObject loadingScreen;
        
        public static void Initialize(ModelViewerConfig config, GameObject loadScreen)
        {
            loadingScreen = loadScreen;
            
            var progressBar = loadingScreen.GetComponentInChildren<ProgressBar>();
            progressBar.SetStatus("Initializing CASC Storage", 0);
            
            cascWorker = new BackgroundWorker { WorkerReportsProgress = true };
            cascWorker.DoWork += CascWorkerOnDoWork;
            cascWorker.ProgressChanged += CascWorkerOnProgressChanged;
            cascWorker.RunWorkerCompleted += CascWorkerOnRunWorkerCompleted;
            cascWorker.RunWorkerAsync(config);
        }

        private static void CascWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loadingScreen.SetActive(false);
        }

        private static void CascWorkerOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var progressBar = loadingScreen.GetComponentInChildren<ProgressBar>();
            progressBar.SetStatus(e.UserState.ToString(), (uint)e.ProgressPercentage);
        }

        private static void CascWorkerOnDoWork(object sender, DoWorkEventArgs e)
        {
            var config = (ModelViewerConfig) e.Argument;
            
            cascHandler = config.LoadType == CascLoadType.Online
                ? CASCHandler.OpenOnlineStorage(cascWorker, config.OnlineBranch)
                : CASCHandler.OpenLocalStorage(cascWorker, config.LocalStorage, config.LocalBranch);
            
            cascWorker.ReportProgress(0, "Setting flags...");
            cascHandler.Root.SetFlags(LocaleFlags.enUS);
        }

        public static Stream OpenFile(uint fileDataId)
        {
            if (!Directory.Exists("Cache"))
                Directory.CreateDirectory("Cache");
            
            if (File.Exists($"Cache/{fileDataId}"))
            {
                var stream = File.Open($"Cache/{fileDataId}", FileMode.Open);
                return stream;
            }
            else if (cascHandler.FileExists((int) fileDataId))
            {
                var stream = cascHandler.OpenFile((int) fileDataId);
                var buffer = new byte[stream.Length];
                
                // Read the data and save the file.
                stream.Read(buffer, 0, buffer.Length);                
                File.WriteAllBytes($"Cache/{fileDataId}", buffer);
                
                stream.Position = 0;
                return stream;
            }

            return null;
        }
    }
}