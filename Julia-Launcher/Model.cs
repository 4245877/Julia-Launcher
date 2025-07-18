﻿using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Assimp;
using System.IO;
using System.Linq;
using Assimp.Configs;
using static Julia_Launcher.UserControl2;

namespace Julia_Launcher
{
    public class Model : IDisposable
    {
        private List<Mesh> meshes = new List<Mesh>();
        private string directory;
        private Dictionary<string, int> loadedTextures = new Dictionary<string, int>();
        private int defaultDiffuseTexture;
        private int defaultSpecularTexture;
        private Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
        private Animator animator;
        private bool hasAnimations = false;

        private Vector3 boundingBoxMin;
        private Vector3 boundingBoxMax;
        private bool isBoundingBoxCalculated = false;

        private const float NORMAL_SMOOTHING_ANGLE = 60.0f;
        private const int MAX_BONE_COUNT = 100;
        public const float INITIAL_MODEL_SCALE = 0.5f;

        public bool HasAnimations => hasAnimations;
        public Animator Animator => animator;
        public IReadOnlyDictionary<string, Animation> Animations => animations;

        private List<CharacterComponent> components = new List<CharacterComponent>();
        public void AddComponent(CharacterComponent component) => components.Add(component);
        private List<Equipment> equipment = new List<Equipment>();
        public void AddEquipment(Equipment eq) => equipment.Add(eq);

        public Model(string path)
        {
            defaultDiffuseTexture = CreateDefaultTexture(255, 255, 255, 255);
            defaultSpecularTexture = CreateDefaultTexture(0, 0, 0, 255);
            animator = new Animator(MAX_BONE_COUNT);
            LoadModel(path);
        }

        private int CreateDefaultTexture(byte r, byte g, byte b, byte a)
        {
            GL.GenTextures(1, out int textureId);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            byte[] pixels = new byte[] { r, g, b, a };
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 1, 1, 0,
                          PixelFormat.Rgba, PixelType.UnsignedByte, pixels);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            return textureId;
        }

        private AssimpContext InitializeImporter()
        {
            var importer = new AssimpContext();
            importer.SetConfig(new NormalSmoothingAngleConfig(NORMAL_SMOOTHING_ANGLE));
            return importer;
        }

        private Scene ImportSceneFromFile(AssimpContext importer, string path, PostProcessSteps steps)
        {
            return importer.ImportFile(path, steps);
        }

        private void ValidateScene(Scene scene)
        {
            if (scene == null || scene.RootNode == null || (scene.SceneFlags & SceneFlags.Incomplete) == SceneFlags.Incomplete)
            {
                throw new Exception("Не удалось загрузить модель с помощью Assimp.");
            }
        }

        private void ProcessSceneContent(Scene scene, Matrix4 initialTransform)
        {
            ProcessNode(scene.RootNode, scene, initialTransform);
        }

        private void SetupAnimations(Scene scene)
        {
            LoadAnimations(scene);
            if (animations.Count > 0)
            {
                hasAnimations = true;
                animator.SetAnimation(animations.Values.First());
            }
        }

        private void LoadModel(string path)
        {
            AssimpContext importer = InitializeImporter();
            Scene scene = ImportSceneFromFile(importer, path,
                PostProcessSteps.Triangulate |
                PostProcessSteps.GenerateSmoothNormals |
                PostProcessSteps.FlipUVs |
                PostProcessSteps.CalculateTangentSpace |
                PostProcessSteps.LimitBoneWeights);
            ValidateScene(scene);
            directory = Path.GetDirectoryName(path);
            Matrix4 scaleTransform = Matrix4.CreateScale(INITIAL_MODEL_SCALE, INITIAL_MODEL_SCALE, INITIAL_MODEL_SCALE);
            ProcessSceneContent(scene, scaleTransform);
            SetupAnimations(scene);
        }

        public (Vector3 Min, Vector3 Max) CalculateBoundingBox()
        {
            if (isBoundingBoxCalculated)
            {
                return (boundingBoxMin, boundingBoxMax);
            }

            if (meshes.Count == 0)
            {
                boundingBoxMin = Vector3.Zero;
                boundingBoxMax = Vector3.Zero;
            }
            else
            {
                boundingBoxMin = new Vector3(float.MaxValue);
                boundingBoxMax = new Vector3(float.MinValue);

                foreach (var mesh in meshes)
                {
                    var vertices = mesh.GetVertices();
                    int stride = mesh.HasBones ? 16 : 8;
                    for (int i = 0; i < vertices.Length; i += stride)
                    {
                        Vector3 pos = new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]);
                        boundingBoxMin = Vector3.ComponentMin(boundingBoxMin, pos);
                        boundingBoxMax = Vector3.ComponentMax(boundingBoxMax, pos);
                    }
                }
            }

            isBoundingBoxCalculated = true;
            return (boundingBoxMin, boundingBoxMax);
        }

        private void ProcessNode(Node node, Scene scene, Matrix4 parentTransform)
        {
            Matrix4 nodeTransform = ConvertMatrix(node.Transform);
            Matrix4 globalTransform = nodeTransform * parentTransform;

            for (int i = 0; i < node.MeshCount; i++)
            {
                Assimp.Mesh mesh = scene.Meshes[node.MeshIndices[i]];
                meshes.Add(ProcessMesh(mesh, scene, globalTransform));
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessNode(node.Children[i], scene, globalTransform);
            }
        }

        private Matrix4 ConvertMatrix(Matrix4x4 assimpMatrix)
        {
            return new Matrix4(
                assimpMatrix.A1, assimpMatrix.B1, assimpMatrix.C1, assimpMatrix.D1,
                assimpMatrix.A2, assimpMatrix.B2, assimpMatrix.C2, assimpMatrix.D2,
                assimpMatrix.A3, assimpMatrix.B3, assimpMatrix.C3, assimpMatrix.D3,
                assimpMatrix.A4, assimpMatrix.B4, assimpMatrix.C4, assimpMatrix.D4
            );
        }

        public Vector3 CalculateModelCenter()
        {
            Vector3 sum = Vector3.Zero;
            int vertexCount = 0;

            foreach (var mesh in meshes)
            {
                var vertices = mesh.GetVertices();
                for (int i = 0; i < vertices.Length; i += 8)
                {
                    sum += new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]);
                    vertexCount++;
                }
            }

            return vertexCount > 0 ? sum / vertexCount : Vector3.Zero;
        }

        private List<Texture> LoadMaterialTextures(Material material, TextureType type, string typeName)
        {
            List<Texture> textures = new List<Texture>();

            for (int i = 0; i < material.GetMaterialTextureCount(type); i++)
            {
                if (material.GetMaterialTexture(type, i, out TextureSlot slot))
                {
                    string texturePath = Path.Combine(directory, slot.FilePath);

                    if (!loadedTextures.TryGetValue(texturePath, out int textureId))
                    {
                        textureId = Texture.LoadTextureFromFile(texturePath);
                        loadedTextures[texturePath] = textureId;
                    }

                    textures.Add(new Texture(textureId, typeName, texturePath));
                }
            }

            return textures;
        }

        private void LoadAnimations(Scene scene)
        {
            if (scene.AnimationCount > 0)
            {
                Console.WriteLine($"Found {scene.AnimationCount} animations in the scene.");
                for (int i = 0; i < scene.AnimationCount; i++)
                {
                    Assimp.Animation assimpAnim = scene.Animations[i];
                    string animName = string.IsNullOrEmpty(assimpAnim.Name) ? $"Animation_{i}" : assimpAnim.Name;
                    float ticksPerSecond = (float)assimpAnim.TicksPerSecond > 0 ? (float)assimpAnim.TicksPerSecond : 25.0f;
                    float durationInTicks = (float)assimpAnim.DurationInTicks;
                    Animation animation = new Animation(animName, durationInTicks, ticksPerSecond);

                    foreach (var nodeAnim in assimpAnim.NodeAnimationChannels)
                    {
                        string boneName = nodeAnim.NodeName;
                        Bone bone = new Bone(animations.Count, boneName, Matrix4.Identity);

                        for (int frameIdx = 0; frameIdx < nodeAnim.PositionKeyCount; frameIdx++)
                        {
                            float timeInTicks = (float)nodeAnim.PositionKeys[frameIdx].Time;
                            var position = nodeAnim.PositionKeys[frameIdx].Value;
                            Vector3 pos = new Vector3(position.X, position.Y, position.Z);
                            OpenTK.Mathematics.Quaternion rot = frameIdx < nodeAnim.RotationKeyCount ?
                                new OpenTK.Mathematics.Quaternion(
                                    nodeAnim.RotationKeys[frameIdx].Value.X,
                                    nodeAnim.RotationKeys[frameIdx].Value.Y,
                                    nodeAnim.RotationKeys[frameIdx].Value.Z,
                                    nodeAnim.RotationKeys[frameIdx].Value.W) :
                                OpenTK.Mathematics.Quaternion.Identity;
                            Vector3 scale = frameIdx < nodeAnim.ScalingKeyCount ?
                                new Vector3(nodeAnim.ScalingKeys[frameIdx].Value.X, nodeAnim.ScalingKeys[frameIdx].Value.Y,
                                            nodeAnim.ScalingKeys[frameIdx].Value.Z) : Vector3.One;

                            Matrix4 transform = Matrix4.CreateScale(scale) * Matrix4.CreateFromQuaternion(rot) * Matrix4.CreateTranslation(pos);
                            bone.AddKeyFrame(new KeyFrame(timeInTicks, transform));
                        }

                        animation.AddBone(bone);
                    }

                    animations[animName] = animation;
                }
            }
        }

        private Mesh ProcessMesh(Assimp.Mesh mesh, Scene scene, Matrix4 globalTransform)
        {
            List<float> vertices = new List<float>();
            List<uint> indices = new List<uint>();
            List<Texture> textures = new List<Texture>();
            bool hasBones = mesh.HasBones;
            List<int> boneIds = new List<int>();
            List<float> boneWeights = new List<float>();

            if (hasBones)
            {
                int[] vertexBoneIds = new int[mesh.VertexCount * 4];
                float[] vertexBoneWeights = new float[mesh.VertexCount * 4];
                int[] boneCountPerVertex = new int[mesh.VertexCount];

                for (int i = 0; i < vertexBoneIds.Length; i++) vertexBoneIds[i] = 0;
                for (int i = 0; i < vertexBoneWeights.Length; i++) vertexBoneWeights[i] = 0.0f;

                for (int boneIndex = 0; boneIndex < mesh.BoneCount; boneIndex++)
                {
                    Assimp.Bone bone = mesh.Bones[boneIndex];
                    foreach (var weight in bone.VertexWeights)
                    {
                        int vertexId = weight.VertexID;
                        if (boneCountPerVertex[vertexId] < 4)
                        {
                            int idx = vertexId * 4 + boneCountPerVertex[vertexId];
                            vertexBoneIds[idx] = boneIndex;
                            vertexBoneWeights[idx] = weight.Weight;
                            boneCountPerVertex[vertexId]++;
                        }
                    }
                }

                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        boneIds.Add(vertexBoneIds[i * 4 + j]);
                        boneWeights.Add(vertexBoneWeights[i * 4 + j]);
                    }
                }
            }

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vector3 pos = new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z);
                pos = Vector3.TransformPosition(pos, globalTransform);
                vertices.Add(pos.X); vertices.Add(pos.Y); vertices.Add(pos.Z);

                if (mesh.HasNormals)
                {
                    Vector3 normal = new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z);
                    normal = Vector3.TransformNormal(normal, globalTransform);
                    vertices.Add(normal.X); vertices.Add(normal.Y); vertices.Add(normal.Z);
                }
                else
                {
                    vertices.Add(0.0f); vertices.Add(0.0f); vertices.Add(1.0f);
                }

                if (mesh.HasTextureCoords(0))
                {
                    vertices.Add(mesh.TextureCoordinateChannels[0][i].X);
                    vertices.Add(mesh.TextureCoordinateChannels[0][i].Y);
                }
                else
                {
                    vertices.Add(0.0f); vertices.Add(0.0f);
                }

                if (hasBones)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        vertices.Add(boneIds[i * 4 + j]);
                        vertices.Add(boneWeights[i * 4 + j]);
                    }
                }
            }

            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];
                for (int j = 0; j < face.IndexCount; j++)
                {
                    indices.Add((uint)face.Indices[j]);
                }
            }

            if (mesh.MaterialIndex >= 0)
            {
                Material material = scene.Materials[mesh.MaterialIndex];
                List<Texture> diffuseMaps = LoadMaterialTextures(material, TextureType.Diffuse, "texture_diffuse");
                List<Texture> specularMaps = LoadMaterialTextures(material, TextureType.Specular, "texture_specular");
                textures.AddRange(diffuseMaps);
                textures.AddRange(specularMaps);
            }

            if (!textures.Any(t => t.Type == "texture_diffuse"))
            {
                textures.Add(new Texture(defaultDiffuseTexture, "texture_diffuse", "default_diffuse"));
            }
            if (!textures.Any(t => t.Type == "texture_specular"))
            {
                textures.Add(new Texture(defaultSpecularTexture, "texture_specular", "default_specular"));
            }

            return new Mesh(vertices.ToArray(), indices.ToArray(), textures, hasBones);
        }

        public void Update(float deltaTime)
        {
            if (hasAnimations)
            {
                animator.Update(deltaTime);
            }
        }

        public void Draw(Shader shader)
        {
            if (hasAnimations)
            {
                Matrix4[] boneTransforms = animator.GetFinalBoneMatrices();
                for (int i = 0; i < boneTransforms.Length; i++)
                {
                    shader.SetMatrix4($"boneTransforms[{i}]", boneTransforms[i]);
                }
                shader.SetBool("hasAnimation", true);
            }
            else
            {
                shader.SetBool("hasAnimation", false);
            }

            foreach (var mesh in meshes)
            {
                mesh.Draw(shader);
            }
        }

        public void PlayAnimation(string name)
        {
            if (animations.TryGetValue(name, out Animation animation))
            {
                animator.SetAnimation(animation);
                animator.Play();
            }
        }

        public void Dispose()
        {
            GL.DeleteTexture(defaultDiffuseTexture);
            GL.DeleteTexture(defaultSpecularTexture);
            foreach (var textureId in loadedTextures.Values)
            {
                GL.DeleteTexture(textureId);
            }
            foreach (var mesh in meshes)
            {
                mesh.Dispose();
            }
        }
    }
}