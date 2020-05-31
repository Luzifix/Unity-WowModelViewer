using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Casc;
using Constants;
using IO.M2;
using UnityEngine;
using Util;

namespace IO.SKIN
{
    public static class SkinReader
    {
        public static void ReadSkin(uint fileDataId, M2Model model)
        {
            var stream = CASC.OpenFile(fileDataId);
            if (stream == null)
                return;

            using (var reader = new BinaryReader(stream))
            {
                var magic = reader.ReadUInt32().Flip();
                if (magic != (uint) Chunks.SKIN)
                    throw new Exception($"{fileDataId} is not a SKIN! {magic:X}");

                var indices         = reader.ReadM2Array();
                var triangles       = reader.ReadM2Array();
                var bones           = reader.ReadM2Array();
                var submeshes       = reader.ReadM2Array();
                var batches         = reader.ReadM2Array();
                var boneCountMax       = reader.ReadUInt32();
                var shadowBatches   = reader.ReadM2Array();
                
                // Read Triangles
                reader.BaseStream.Position = triangles.Offset;
                var triangleList = new int[triangles.Size];
                for (var i = 0; i < triangles.Size; ++i)
                    triangleList[i] = reader.ReadUInt16();
                
                // Read Batches
                reader.BaseStream.Position = batches.Offset;
                for (var i = 0; i < batches.Size; ++i)
                {
                    var skinBatch = new M2Batch
                    {
                        Flag1                      = reader.ReadByte(),
                        Flag2                      = reader.ReadByte(),
                        ShaderId                   = reader.ReadUInt16(),
                        SkinSectionId              = reader.ReadUInt16(),
                        GeosetIndex                = reader.ReadUInt16(),
                        ColorIndex                 = reader.ReadUInt16(),
                        MaterialIndex              = reader.ReadUInt16(),
                        MaterialLayer              = reader.ReadUInt16(),
                        TextureCount               = reader.ReadUInt16(),
                        TextureComboIndex          = reader.ReadUInt16(),
                        TextureCoordComboIndex     = reader.ReadUInt16(),
                        TextureTransformComboIndex = reader.ReadUInt16(),
                        TextureWeightComboIndex    = reader.ReadUInt16()
                    };
                    
                    model.BatchIndices.Add(skinBatch);
                }

                // Read Submeshes
                reader.BaseStream.Position = submeshes.Offset;
                for (var i = 0; i < submeshes.Size; ++i)
                {
                    var submesh = new M2Submesh();
                    submesh.SkinSectionId = reader.ReadUInt16();
                    var submeshLevel = reader.ReadUInt16();
                    submesh.VertexStart = reader.ReadUInt16() + (submeshLevel << 16);
                    submesh.VertexCount = reader.ReadUInt16();
                    submesh.TriangleStart = reader.ReadUInt16() + (submeshLevel << 16);
                    submesh.TriangleCount = reader.ReadUInt16();
                    submesh.BoneCount = reader.ReadUInt16();
                    submesh.BoneComboIndex = reader.ReadUInt16();
                    submesh.BoneInfluence = reader.ReadUInt16();
                    submesh.CenterBoneIndex = reader.ReadUInt16();
                    
                    var centerPosition = new Vector3(reader.ReadSingle() / WorldConstants.WorldScale, 
                        reader.ReadSingle() / WorldConstants.WorldScale,
                        reader.ReadSingle() / WorldConstants.WorldScale);
                    submesh.CenterPosition = new Vector3(-centerPosition.x, centerPosition.z, -centerPosition.y);
                    
                    var sortCenterPosition = new Vector3(reader.ReadSingle() / WorldConstants.WorldScale, 
                        reader.ReadSingle() / WorldConstants.WorldScale,
                        reader.ReadSingle() / WorldConstants.WorldScale);
                    submesh.SortCenterPosition = new Vector3(-sortCenterPosition.x, sortCenterPosition.z, -sortCenterPosition.y);
                    submesh.SortRadius = reader.ReadSingle();

                    submesh.Vertices = new Vector3[submesh.VertexCount];
                    submesh.Normals = new Vector3[submesh.VertexCount];
                    submesh.UVs = new Vector2[submesh.VertexCount];
                    submesh.UV2s = new Vector2[submesh.VertexCount];
                    
                    Parallel.For(0, submesh.VertexCount, j =>
                    {
                        submesh.Vertices[j]    = model.MeshData.Vertices[j + submesh.VertexStart];
                        submesh.Normals[j]     = model.MeshData.Normals[j + submesh.VertexStart];
                        submesh.UVs[j]         = model.MeshData.TexCoords[j + submesh.VertexStart];
                        submesh.UV2s[j]        = model.MeshData.TexCoords2[j + submesh.VertexStart];
                    });
                    
                    submesh.Triangles = new int[submesh.TriangleCount];
                    Parallel.For(0, submesh.TriangleCount, j =>
                    { 
                        submesh.Triangles[j] = triangleList[j + submesh.TriangleStart];
                    });
                    
                    model.Submeshes.Add(submesh);
                }
            }
        }
    }
}