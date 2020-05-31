using Constants;

namespace Settings
{
    public class ModelViewerConfig
    {
        public CascLoadType LoadType { get; set; }
        public string LocalStorage { get; set; }
        public string LocalBranch { get; set; }
        public string OnlineBranch { get; set; }
    }
}