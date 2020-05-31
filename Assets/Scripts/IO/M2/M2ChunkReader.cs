using System;
using System.Collections.Generic;
using System.IO;
using Casc;
using Constants;
using IO.Shared;
using UnityEngine;
using Util;

namespace IO.M2
{
    public static partial class M2Reader
    {
        public static List<uint> SkinFileIds = new List<uint>();
        public static List<uint> TextureFileIds = new List<uint>();
        
        public static void ReadMD20(BinaryReader reader, M2Model model)
        {
            var magic = reader.ReadUInt32().Flip();
            if (magic != (uint) Chunks.MD20)
                throw new Exception($"{model.FileDataId} has invalid MD20 magic! {magic:X}");

            var version = reader.ReadUInt32();
            if (version < 272)
                throw new Exception($"{model.FileDataId} has invalid version! {version}");

            // Get the model name
            var name = reader.ReadM2Array();
            var oldPos = reader.BaseStream.Position;
            reader.BaseStream.Position = name.Offset + 8;
            model.ModelName = new string(reader.ReadChars((int)name.Size)).Replace("\0", "");
            reader.BaseStream.Position = oldPos;

            var flags = reader.ReadUInt32();
            var globalLoops = reader.ReadM2Array();
            var sequences = reader.ReadM2Array();
            var sequencesLookups = reader.ReadM2Array();
            var bones = reader.ReadM2Array();
            var keyBoneLookup = reader.ReadM2Array();
            var vertices = reader.ReadM2Array();
            var skinCount = reader.ReadInt32();
            var colors = reader.ReadM2Array();
            var textures = reader.ReadM2Array();
            var textureWeights = reader.ReadM2Array();
            var textureTransforms = reader.ReadM2Array();
            var replacableTextureLookup = reader.ReadM2Array();
            var materials = reader.ReadM2Array();
            var boneLookupTable = reader.ReadM2Array();
            var textureLookupTable = reader.ReadM2Array();

            reader.BaseStream.Position = 8 + vertices.Offset;
            for (var i = 0; i < vertices.Size; ++i)
            {
                var position = new Vector3
                {
                    x = reader.ReadSingle() / WorldConstants.WorldScale,
                    y = reader.ReadSingle() / WorldConstants.WorldScale,
                    z = reader.ReadSingle() / WorldConstants.WorldScale,
                };
                
                model.MeshData.Vertices.Add(new Vector3(-position.x, position.z, -position.y));
                model.MeshData.BoneWeights.Add(new []
                {
                    reader.ReadByte() / 255.0f,
                    reader.ReadByte() / 255.0f,
                    reader.ReadByte() / 255.0f,
                    reader.ReadByte() / 255.0f
                });
                model.MeshData.BoneIndices.Add(new int[] { reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte() });

                var normalPosition = new Vector3
                {
                    x = reader.ReadSingle() * WorldConstants.WorldScale,
                    y = reader.ReadSingle() * WorldConstants.WorldScale,
                    z = reader.ReadSingle() * WorldConstants.WorldScale
                };
                model.MeshData.Normals.Add(new Vector3(-normalPosition.x, normalPosition.z, -normalPosition.y));
                model.MeshData.TexCoords.Add(new Vector2(reader.ReadSingle(), reader.ReadSingle()));
                model.MeshData.TexCoords2.Add(new Vector2(reader.ReadSingle(), reader.ReadSingle()));
            }

            reader.BaseStream.Position = 8 + textureLookupTable.Offset;
            for (var i = 0; i < textureLookupTable.Size; ++i)
                model.TextureLookupTable.Add(reader.ReadUInt16());
        }

        public static void ReadSFID(BinaryReader reader, uint chunkSize)
        {
            var sfidSize = chunkSize / 4;
            for (var i = 0; i < sfidSize; ++i)
            {
                SkinFileIds.Add(reader.ReadUInt32());
            }
        }

        public static void ReadTXID(BinaryReader reader, M2Model model, uint chunkSize)
        {
            var txidSize = chunkSize / 4;
            for (var i = 0; i < txidSize; ++i)
            {
                var fileDataId = reader.ReadUInt32();
                
                if (!TextureFileIds.Contains(fileDataId))
                {
                    var m2Texture = new M2Texture();
                    var textureData = new TextureData();
                    
                    using (var blpStream = CASC.OpenFile(fileDataId))
                    {
                        var blp = new BLP();
                        var blpData = blp.GetUncompressed(blpStream);
                        var blpInfo = blp.GetInfo();

                        textureData.HasMipmaps = blpInfo.hasMipmaps;
                        textureData.Width = blpInfo.width;
                        textureData.Height = blpInfo.height;
                        textureData.RawData = blpData;
                        textureData.TextureFormat = blpInfo.textureFormat;

                        m2Texture.TextureData = textureData;
                        m2Texture.FileDataId = fileDataId;
                        
                        TextureFileIds.Add(fileDataId);
                    }
                    
                    model.Textures.Add(m2Texture);
                }
            }
        }
    }
}