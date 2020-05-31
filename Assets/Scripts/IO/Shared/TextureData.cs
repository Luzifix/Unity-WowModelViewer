using UnityEngine;

namespace IO.Shared
{
    public class TextureData
    {
        public byte[] RawData;
        public int Width;
        public int Height;
        public bool HasMipmaps;
        public TextureFormat TextureFormat;
    }
}