using System.IO;
using IO.Shared;
using UnityEngine;

namespace Util
{
    public static class Extensions
    {
        #region Binary Reader Extensions
        public static M2Array ReadM2Array(this BinaryReader reader)
        {
            return new M2Array
            {
                Size = reader.ReadUInt32(),
                Offset = reader.ReadUInt32()
            };
        }
        
        public static void Skip(this BinaryReader reader, uint size)
        {
            reader.BaseStream.Position += size;
        }

        public static uint Flip(this uint n) => (n << 24) | (((n >> 16) << 24) >> 16) | (((n << 16) >> 24) << 16) | (n >> 24);
        #endregion
        
        #region Unity Editor Extensions
        public static GameObject FindObject(this GameObject parent, string name)
        {
            var children = parent.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                if (child.name == name)
                    return child.gameObject;
            }

            return default;
        }
        #endregion
    }
}