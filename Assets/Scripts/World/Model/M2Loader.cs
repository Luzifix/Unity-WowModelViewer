using System.Collections.Generic;
using IO.M2;
using IO.SKIN;
using UnityEngine;

namespace World.Model
{
    public static class M2Loader
    {
        public static void Load(uint fileDataId, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            var model = new M2Model
            {
                FileDataId = fileDataId,
                Position = position,
                Rotation = rotation,
                Scale = scale
            };
            
            // Parse the M2 format
            M2Reader.ReadM2(fileDataId, model);
            
            // Parse the SKIN format.
            foreach (var skinFile in M2Reader.SkinFileIds)
                SkinReader.ReadSkin(skinFile, model);
            
            M2Data.EnqueuedModels.Enqueue(model);
        }
    }
}