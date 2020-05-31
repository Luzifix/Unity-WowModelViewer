using System.Collections.Generic;
using System.Threading.Tasks;
using Constants;
using IO.M2;
using UnityEngine;

namespace World.Model
{
    public class M2Handler
    {
        private GameObject m2Parent;
        
        private Dictionary<uint, Texture2D> activeTextures = new Dictionary<uint, Texture2D>();

        public M2Handler(GameObject parent) => m2Parent = parent;
        
        public void Update()
        {
            if (M2Data.EnqueuedModels.Count > 0)
            {
                if (M2Data.EnqueuedModels.TryDequeue(out var model))
                    CreateM2Object(model);
            }
        }

        public void CreateM2Object(M2Model model)
        {
            var m2Object = new GameObject();
            m2Object.name = GuiConstants.IsInModelPreview ? "activeModel" : model.ModelName;
            m2Object.transform.SetParent(m2Parent.transform);

            // Bones
            var m2Bone = new GameObject();
            m2Bone.name = "bones";
            m2Bone.transform.SetParent(m2Object.transform);

            // Mesh
            var m2MeshObject = new GameObject();
            m2MeshObject.name = "mesh";
            m2MeshObject.transform.position = Vector3.zero;
            m2MeshObject.transform.rotation = Quaternion.identity;
            m2MeshObject.transform.SetParent(m2Object.transform);

            var mesh = new Mesh();
            mesh.vertices = model.MeshData.Vertices.ToArray();
            mesh.normals = model.MeshData.Normals.ToArray();
            mesh.uv = model.MeshData.TexCoords.ToArray();
            mesh.uv2 = model.MeshData.TexCoords2.ToArray();
            mesh.subMeshCount = model.Submeshes.Count;

            var meshRenderer = m2MeshObject.AddComponent<SkinnedMeshRenderer>();
            meshRenderer.sharedMesh = mesh;
            
            var materials = new Material[model.Submeshes.Count];
            Parallel.For(0, model.Submeshes.Count, i =>
            {
                mesh.SetTriangles(model.Submeshes[i].Triangles, i, true);
                materials[i] = new Material(Shader.Find("WowModelViewer/WMO/S_Diffuse"));

                var skinSectionIndex = model.BatchIndices[i].SkinSectionId;
                var textureFileDataId = model.Textures[model.TextureLookupTable[model.BatchIndices[skinSectionIndex].TextureComboIndex]].FileDataId;
                var textureData = model.Textures[model.TextureLookupTable[model.BatchIndices[skinSectionIndex].TextureComboIndex]].TextureData;

                if (textureFileDataId != 0 && textureData != null)
                {
                    if (!activeTextures.ContainsKey(textureFileDataId))
                    {
                        var texture = new Texture2D(textureData.Width, textureData.Height, textureData.TextureFormat, textureData.HasMipmaps);
                        texture.LoadRawTextureData(textureData.RawData);
                        texture.Apply();

                        activeTextures[textureFileDataId] = texture;
                    }
                    
                    materials[i].SetTexture("_MainTex", activeTextures[textureFileDataId]);
                }
            });
            meshRenderer.materials = materials;

            if (GuiConstants.IsInModelPreview)
            {
                var camera = GameObject.Find("Main Camera").GetComponent<Camera>();
                camera.transform.LookAt(m2Object.transform);
            }
        }
    }
}