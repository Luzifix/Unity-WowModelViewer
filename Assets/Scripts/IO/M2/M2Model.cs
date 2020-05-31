using System.Collections.Concurrent;
using System.Collections.Generic;
using IO.Shared;
using UnityEngine;

namespace IO.M2
{
    public static class M2Data
    {
        public static ConcurrentQueue<M2Model> EnqueuedModels = new ConcurrentQueue<M2Model>();
    }
    
    public class M2Model
    {
        public uint FileDataId;
        public string ModelName;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public M2MeshData MeshData = new M2MeshData();
        public List<M2Submesh> Submeshes = new List<M2Submesh>();
        public List<M2Batch> BatchIndices = new List<M2Batch>();
        public List<M2Texture> Textures = new List<M2Texture>();

        public List<ushort> TextureLookupTable = new List<ushort>();
    }

    public struct M2Submesh
    {
        public ushort SkinSectionId;
        public int VertexStart;
        public ushort VertexCount;
        public int TriangleStart;
        public ushort TriangleCount;
        public ushort BoneCount;
        public ushort BoneComboIndex;
        public ushort BoneInfluence;
        public ushort CenterBoneIndex;
        public Vector3 CenterPosition;
        public Vector3 SortCenterPosition;
        public float SortRadius;
        
        public Vector3[] Vertices;
        public Vector3[] Normals;
        public Vector2[] UVs;
        public Vector2[] UV2s;
        public int[] Triangles;
    }

    public class M2MeshData
    {
        public List<Vector3> Vertices = new List<Vector3>();
        public List<float[]> BoneWeights = new List<float[]>();
        public List<int[]> BoneIndices = new List<int[]>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<Vector2> TexCoords = new List<Vector2>();
        public List<Vector2> TexCoords2 = new List<Vector2>();
    }

    public struct M2Batch
    {
        public byte Flag1;
        public byte Flag2;
        public ushort ShaderId;
        public ushort SkinSectionId;
        public ushort GeosetIndex;
        public ushort ColorIndex;
        public ushort MaterialIndex;
        public ushort MaterialLayer;
        public ushort TextureCount;
        public ushort TextureComboIndex;
        public ushort TextureCoordComboIndex;
        public ushort TextureWeightComboIndex;
        public ushort TextureTransformComboIndex;
    }

    public struct M2Texture
    {
        public int Type;
        public int Flags;
        public string Filename;
        public uint FileDataId;
        public TextureData TextureData;
    }
}