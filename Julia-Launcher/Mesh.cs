using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;
using Julia_Launcher;

namespace Julia_Launcher
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector3 Position;     // Позиция вершины (x, y, z)
        public Vector3 Normal;       // Нормаль (x, y, z)
        public Vector2 TexCoord;     // Текстурные координаты (u, v)
        public Vector3 Tangent;      // Касательная (x, y, z)
        public Vector3 Bitangent;    // Бинормаль (x, y, z)
        public Vector4i BoneIds;     // Индексы костей (4 int)
        public Vector4 BoneWeights;  // Веса костей (4 float)

        // Конструктор для удобного создания экземпляра
        public Vertex(Vector3 position, Vector3 normal, Vector2 texCoord, Vector3 tangent, Vector3 bitangent, Vector4i boneIds, Vector4 boneWeights)
        {
            Position = position;
            Normal = normal;
            TexCoord = texCoord;
            Tangent = tangent;
            Bitangent = bitangent;
            BoneIds = boneIds;
            BoneWeights = boneWeights;
        }
    }




    public class Mesh
    {
        private int VAO, VBO, EBO;
        private int indexCount;
        private bool hasBones;
        public List<UserControl2.Texture> Textures { get; private set; }
        private float[] vertices;

        public bool HasBones => hasBones;

        public Mesh(float[] vertices, uint[] indices, List<UserControl2.Texture> textures, bool hasBones = false)
        {
            this.vertices = vertices;
            Textures = textures;
            indexCount = indices.Length;
            this.hasBones = hasBones;

            GL.GenVertexArrays(1, out VAO);
            GL.GenBuffers(1, out VBO);
            GL.GenBuffers(1, out EBO);

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            int stride = hasBones ? 16 * sizeof(float) : 8 * sizeof(float);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            if (hasBones)
            {
                GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, stride, 8 * sizeof(float));
                GL.EnableVertexAttribArray(3);

                GL.VertexAttribPointer(4, 4, VertexAttribPointerType.Float, false, stride, 12 * sizeof(float));
                GL.EnableVertexAttribArray(4);
            }

            GL.BindVertexArray(0);
        }

        public float[] GetVertices()
        {
            return vertices;
        }

        public void Draw(UserControl2.Shader shader)
        {
            uint diffuseNr = 1;
            uint specularNr = 1;

            for (int i = 0; i < Textures.Count; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i);
                string number = "";
                string name = Textures[i].Type;

                if (name == "texture_diffuse")
                    number = diffuseNr++.ToString();
                else if (name == "texture_specular")
                    number = specularNr++.ToString();

                shader.SetInt($"{name}{number}", i);
                GL.BindTexture(TextureTarget.Texture2D, Textures[i].Id);
            }

            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            GL.ActiveTexture(TextureUnit.Texture0);
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(VAO);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);
        }
    }
}