using UnityEngine;
using UnityEngine.UI;
using World.Model;

namespace GUI
{
    public class DebugGui : MonoBehaviour
    {
        public Button LoadM2;
        
        public void Start()
        {
            LoadM2.onClick.AddListener(delegate { LoadModel(LoadType.M2, 3605056); });
        }
        
        public void LoadModel(LoadType type, uint fileDataId)
        {
            switch (type)
            {
                case LoadType.M2:
                    M2Loader.Load(fileDataId, new Vector3(0, 0, 1), new Quaternion(0, -90, 0, 0), Vector3.one);
                    break;
            }
        }
    }

    public enum LoadType
    {
        WMO = 0,
        M2 = 1,
        ADT = 3
    }
}