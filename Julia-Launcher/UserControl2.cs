using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.IO;
using Assimp;
using Assimp.Configs;
using OpenTK.GLControl;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection;

namespace Julia_Launcher
{
    public partial class UserControl2 : UserControl
    {
        private bool loaded = false;
        private Model model;
        private Camera camera;
        private Shader shader;
        private float rotation = 0.0f;
        private Vector3 modelPosition = Vector3.Zero;
        private float modelScale = 1.0f;
        private bool isDragging = false;
        private Point lastMousePos;

        private Vector3 lightPos = new Vector3(1.2f, 1.0f, 2.0f);
        private Vector3 lightColor = new Vector3(1.0f, 1.0f, 1.0f);
        private float ambientStrength = 0.1f;
        private float specularStrength = 0.5f;

        public UserControl2()
        {
            InitializeComponent();

            // Use existing glControl1 instead of creating a new one 
            this.glControl1.Load += GlControl_Load;
            this.glControl1.Paint += GlControl_Paint;
            this.glControl1.Resize += GlControl_Resize;
            this.glControl1.MouseDown += GlControl_MouseDown;
            this.glControl1.MouseMove += GlControl_MouseMove;
            this.glControl1.MouseUp += GlControl_MouseUp;
            this.glControl1.MouseWheel += GlControl_MouseWheel;
        }

        // Remove the InitializeOpenGL method entirely

        private void GlControl_Load(object sender, EventArgs e)
        {
            try
            {
                GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
                GL.Enable(EnableCap.DepthTest);
                GL.Viewport(0, 0, glControl1.Width, glControl1.Height); // Добавлен Viewport

                string appDirectory = "F:\\Work\\C#\\Julia-Launcher\\Julia-Launcher\\Julia-Launcher";

                // Исправлены пути на относительные
                string shadersDirectory = Path.Combine(appDirectory, "Shaders");
                string vertexPath = Path.Combine(shadersDirectory, "vertex.glsl");
                string fragmentPath = Path.Combine(shadersDirectory, "fragment.glsl");

                if (!File.Exists(vertexPath) || !File.Exists(fragmentPath))
                {
                    MessageBox.Show("Файлы шейдеров не найдены!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                shader = new Shader(vertexPath, fragmentPath);

                camera = new Camera(new Vector3(0, 0, 3), glControl1.Width / (float)glControl1.Height);
                camera.LookAt(new Vector3(0, 0, 0));

                // Исправлен путь к модели

                string modelPath = "F:\\Work\\C#\\Julia-Launcher\\Julia-Launcher\\Julia-Launcher\\Model\\Raphtalia\\hero_spy_orange_body_0001\\hero_spy_orange_body_0001.fbx";

                LoadModel(modelPath);
                loaded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ResetCamera()
        {
            // Сбросить положение камеры к исходному
            camera.Position = new Vector3(0, 0, 3);
            camera.LookAt(new Vector3(0, 2, 0));
            rotation = 0.0f;
            glControl1.Invalidate();
        }
        private void GlControl_Paint(object sender, PaintEventArgs e)
        {

            if (!loaded) return;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (model != null)
            {
                shader.Use();

                Matrix4 view = camera.GetViewMatrix();
                Matrix4 projection = camera.GetProjectionMatrix();
                shader.SetMatrix4("view", view);
                shader.SetMatrix4("projection", projection);

                // Освещение
                shader.SetVector3("lightPos", lightPos);
                shader.SetVector3("lightColor", lightColor);
                shader.SetFloat("ambientStrength", ambientStrength);
                shader.SetFloat("specularStrength", specularStrength);
                shader.SetVector3("viewPos", camera.Position);

                // Матрица модели
                Matrix4 modelMatrix = Matrix4.CreateScale(modelScale) *
                                     Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation)) *
                                     Matrix4.CreateTranslation(modelPosition);
                shader.SetMatrix4("model", modelMatrix);

                // Матрица нормалей
                Matrix4 rotationMatrix = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation));
                Matrix3 normalMatrix = new Matrix3(
                    rotationMatrix.Row0.Xyz,
                    rotationMatrix.Row1.Xyz,
                    rotationMatrix.Row2.Xyz
                );
                shader.SetMatrix3("normalMatrix", normalMatrix);

                model.Draw(shader);
            }

            glControl1.SwapBuffers();
        }

        private void GlControl_Resize(object sender, EventArgs e)
        {
            if (!loaded) return;

            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            camera.AspectRatio = glControl1.Width / (float)glControl1.Height;
            glControl1.Invalidate();
        }

        private void GlControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastMousePos = e.Location;
            }
        }

        private void GlControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        private void GlControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                float xOffset = e.X - lastMousePos.X;
                float yOffset = lastMousePos.Y - e.Y;
                lastMousePos = e.Location;

                // При удерживании Shift или Ctrl - вращаем модель
                if (ModifierKeys.HasFlag(Keys.Shift) || ModifierKeys.HasFlag(Keys.Control))
                {
                    rotation += xOffset * 0.5f;
                }
                else
                {
                    // Иначе перемещаем камеру вокруг модели
                    camera.ProcessMouseMovement(xOffset * 0.1f, yOffset * 0.1f);
                }

                glControl1.Invalidate();
            }
        }

        private void GlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            // Adjust zoom based on mouse wheel
            camera.ProcessMouseScroll(e.Delta / 120.0f);
            glControl1.Invalidate();
        }

        public void LoadModel(string path)
        {
            try
            {
                // Dispose previous model if exists
                model?.Dispose();

                model = new Model(path);

                // Center and scale model to fit view
                modelScale = 1.0f;
                modelPosition = Vector3.Zero;
                rotation = 0.0f;

                // Reset camera position when loading a new model
                ResetCamera();

                glControl1.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load model: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GlControl_Click(object sender, EventArgs e)
        {
            // Empty implementation or different functionality if needed
        }

        private void trackBar7_Scroll(object sender, EventArgs e)
        {
            if (loaded && model != null)
            {
                // Assume trackBar7 controls model scale
                modelScale = trackBar7.Value / 100.0f;
                glControl1.Invalidate();
            }
        }

        // Camera class to handle camera transformations
        public class Camera
        {
            public Vector3 Position { get; set; }
            public Vector3 Front { get; private set; }
            public Vector3 Up { get; private set; }
            public Vector3 Right { get; private set; }
            public float AspectRatio { get; set; }

            private Vector3 worldUp = new Vector3(0, 1, 0);
            private float yaw = -90.0f;
            private float pitch = 0.0f;
            private float zoom = 45.0f;

            public Camera(Vector3 position, float aspectRatio)
            {
                Position = position;
                AspectRatio = aspectRatio;
                UpdateCameraVectors();
            }

            public void LookAt(Vector3 target)
            {
                Vector3 direction = Vector3.Normalize(target - Position);
                if (direction.LengthSquared > 0)
                {
                    pitch = MathHelper.RadiansToDegrees((float)Math.Asin(direction.Y));
                    yaw = MathHelper.RadiansToDegrees((float)Math.Atan2(direction.Z, direction.X));
                    UpdateCameraVectors();
                }
            }

            public Matrix4 GetViewMatrix()
            {
                return Matrix4.LookAt(Position, Position + Front, Up);
            }

            public Matrix4 GetProjectionMatrix()
            {
                return Matrix4.CreatePerspectiveFieldOfView(
                    MathHelper.DegreesToRadians(zoom),
                    AspectRatio,
                    0.1f,
                    100.0f);
            }

            public void ProcessMouseMovement(float xOffset, float yOffset, bool constrainPitch = true)
            {
                yaw += xOffset;
                pitch += yOffset;

                if (constrainPitch)
                {
                    pitch = Math.Clamp(pitch, -89.0f, 89.0f);
                }

                UpdateCameraVectors();
            }

            public void ProcessMouseScroll(float yOffset)
            {
                zoom -= yOffset;
                zoom = Math.Clamp(zoom, 1.0f, 90.0f);
            }

            private void UpdateCameraVectors()
            {
                // Calculate new Front vector
                Vector3 front;
                front.X = (float)(Math.Cos(MathHelper.DegreesToRadians(yaw)) * Math.Cos(MathHelper.DegreesToRadians(pitch)));
                front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
                front.Z = (float)(Math.Sin(MathHelper.DegreesToRadians(yaw)) * Math.Cos(MathHelper.DegreesToRadians(pitch)));
                Front = Vector3.Normalize(front);

                // Recalculate Right and Up vectors
                Right = Vector3.Normalize(Vector3.Cross(Front, worldUp));
                Up = Vector3.Normalize(Vector3.Cross(Right, Front));
            }
        }

        // Shader class to handle GLSL shaders
        public class Shader
        {
            public int Handle { get; private set; }

            private Dictionary<string, int> uniformLocations;

            public Shader(string vertexPath, string fragmentPath)
            {
                string vertexShaderSource = File.ReadAllText(vertexPath);
                string fragmentShaderSource = File.ReadAllText(fragmentPath);

                int vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderSource);
                int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);

                // Create program, attach shaders, and link
                Handle = GL.CreateProgram();
                GL.AttachShader(Handle, vertexShader);
                GL.AttachShader(Handle, fragmentShader);
                GL.LinkProgram(Handle);

                // Check for linking errors
                GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
                if (success == 0)
                {
                    string infoLog = GL.GetProgramInfoLog(Handle);
                    Console.WriteLine($"ERROR::PROGRAM::LINKING_FAILED\n{infoLog}");
                }

                // Delete the shaders as they're now linked into the program
                GL.DetachShader(Handle, vertexShader);
                GL.DetachShader(Handle, fragmentShader);
                GL.DeleteShader(vertexShader);
                GL.DeleteShader(fragmentShader);

                // Cache all uniform locations
                GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out int uniformCount);
                uniformLocations = new Dictionary<string, int>();

                for (int i = 0; i < uniformCount; i++)
                {
                    string name = GL.GetActiveUniform(Handle, i, out _, out _);
                    int location = GL.GetUniformLocation(Handle, name);
                    uniformLocations[name] = location;
                }
            }
            public void SetMatrix3(string name, Matrix3 value)
            {
                GL.UseProgram(Handle);
                int location = GetUniformLocation(name);
                GL.UniformMatrix3(location, false, ref value);
            }
            private int CompileShader(ShaderType type, string source)
            {
                int shader = GL.CreateShader(type);
                GL.ShaderSource(shader, source);
                GL.CompileShader(shader);

                GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
                if (success == 0)
                {
                    string infoLog = GL.GetShaderInfoLog(shader);
                    Console.WriteLine($"ERROR::SHADER::{type}::COMPILATION_FAILED\n{infoLog}");
                }

                return shader;
            }

            public void Use()
            {
                GL.UseProgram(Handle);
            }

            public void SetFloat(string name, float value)
            {
                GL.UseProgram(Handle);
                GL.Uniform1(GetUniformLocation(name), value);
            }

            public void SetVector3(string name, Vector3 value)
            {
                GL.UseProgram(Handle);
                GL.Uniform3(GetUniformLocation(name), value);
            }

            public void SetMatrix4(string name, Matrix4 value)
            {
                GL.UseProgram(Handle);
                GL.UniformMatrix4(GetUniformLocation(name), false, ref value);
            }

            private int GetUniformLocation(string name)
            {
                if (uniformLocations.TryGetValue(name, out int location))
                {
                    return location;
                }

                location = GL.GetUniformLocation(Handle, name);
                uniformLocations[name] = location;

                if (location == -1)
                {
                    Console.WriteLine($"Warning: uniform '{name}' doesn't exist!");
                }

                return location;
            }
        }

        // Mesh class to store mesh data
        public class Mesh
        {
            private int VAO, VBO, EBO;
            private int indexCount;
            public List<Texture> Textures { get; private set; }

            public Mesh(float[] vertices, uint[] indices, List<Texture> textures)
            {
                Textures = textures;
                indexCount = indices.Length;

                // Create buffers/arrays
                GL.GenVertexArrays(1, out VAO);
                GL.GenBuffers(1, out VBO);
                GL.GenBuffers(1, out EBO);

                GL.BindVertexArray(VAO);

                // Load data into vertex buffer
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

                // Load data into element buffer
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

                // Set vertex attribute pointers
                // Position attribute
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                // Normal attribute
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);

                // Texture coords attribute
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
                GL.EnableVertexAttribArray(2);

                GL.BindVertexArray(0);
            }

            public void Draw(Shader shader)
            {
                // Bind appropriate textures
                uint diffuseNr = 1;
                uint specularNr = 1;

                for (int i = 0; i < Textures.Count; i++)
                {
                    // Activate texture unit
                    GL.ActiveTexture(TextureUnit.Texture0 + i);

                    // Generate texture name based on type
                    string number = "";
                    string name = Textures[i].Type;

                    if (name == "texture_diffuse")
                        number = diffuseNr++.ToString();
                    else if (name == "texture_specular")
                        number = specularNr++.ToString();

                    // Set the sampler to the correct texture unit
                    shader.SetFloat($"{name}{number}", i);

                    // Bind the texture
                    GL.BindTexture(TextureTarget.Texture2D, Textures[i].Id);
                }

                // Draw mesh
                GL.BindVertexArray(VAO);
                GL.DrawElements(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);

                // Reset active texture
                GL.ActiveTexture(TextureUnit.Texture0);
            }

            public void Dispose()
            {
                GL.DeleteVertexArray(VAO);
                GL.DeleteBuffer(VBO);
                GL.DeleteBuffer(EBO);
            }
        }

        // Texture class to hold texture data
        public class Texture
        {
            public int Id { get; private set; }
            public string Type { get; private set; }
            public string Path { get; private set; }

            public Texture(int id, string type, string path)
            {
                Id = id;
                Type = type;
                Path = path;
            }

            public static int LoadTextureFromFile(string path)
            {
                GL.GenTextures(1, out int textureId);

                using (var image = new Bitmap(path))
                {
                    // Flip the image to correct orientation for OpenGL
                    image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                    System.Drawing.Imaging.BitmapData data = image.LockBits(
                        new Rectangle(0, 0, image.Width, image.Height),
                        System.Drawing.Imaging.ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.BindTexture(TextureTarget.Texture2D, textureId);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                        image.Width, image.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                    image.UnlockBits(data);

                    // Set texture parameters
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)OpenTK.Graphics.OpenGL4.TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)OpenTK.Graphics.OpenGL4.TextureMagFilter.Linear);

                    // Generate mipmaps
                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                }

                return textureId;
            }
        }


        // Model class to load and render 3D models
        public class Model
        {
            private List<Mesh> meshes = new List<Mesh>();
            private string directory;
            private Dictionary<string, int> loadedTextures = new Dictionary<string, int>();
            private int defaultDiffuseTexture;
            private int defaultSpecularTexture;

            public Model(string path)
            {
                // Инициализация стандартных текстур
                defaultDiffuseTexture = CreateDefaultTexture(255, 255, 255, 255); // Белая
                defaultSpecularTexture = CreateDefaultTexture(0, 0, 0, 255);      // Чёрная
                LoadModel(path);
            }
            private int CreateDefaultTexture(byte r, byte g, byte b, byte a)
            {
                GL.GenTextures(1, out int textureId);
                GL.BindTexture(TextureTarget.Texture2D, textureId);
                byte[] data = new byte[4] { r, g, b, a };
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 1, 1, 0,
                    PixelFormat.Rgba, PixelType.UnsignedByte, data);

                // Используем полное имя типа для устранения неоднозначности
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)OpenTK.Graphics.OpenGL4.TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)OpenTK.Graphics.OpenGL4.TextureMagFilter.Linear);

                return textureId;
            }
            private void LoadModel(string path)
            {
                var importer = new AssimpContext();
                importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
                Scene scene = importer.ImportFile(path,
                    PostProcessSteps.Triangulate |
                    PostProcessSteps.GenerateSmoothNormals |
                    PostProcessSteps.FlipUVs |
                    PostProcessSteps.CalculateTangentSpace);

                if (scene == null || scene.RootNode == null || (scene.SceneFlags & SceneFlags.Incomplete) == SceneFlags.Incomplete)
                {
                    throw new Exception("Не удалось загрузить модель с помощью Assimp.");
                }

                directory = Path.GetDirectoryName(path);
                ProcessNode(scene.RootNode, scene);
            }

            private void ProcessNode(Node node, Scene scene)
            {
                // Обработка всех мешей в текущем узле
                for (int i = 0; i < node.MeshCount; i++)
                {
                    Assimp.Mesh mesh = scene.Meshes[node.MeshIndices[i]];
                    meshes.Add(ProcessMesh(mesh, scene));
                }

                // Рекурсивная обработка дочерних узлов
                for (int i = 0; i < node.ChildCount; i++)
                {
                    ProcessNode(node.Children[i], scene);
                }
            }

            private Mesh ProcessMesh(Assimp.Mesh mesh, Scene scene)
            {
                List<float> vertices = new List<float>();
                List<uint> indices = new List<uint>();
                List<Texture> textures = new List<Texture>();

                // Обработка вершин
                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    vertices.Add(mesh.Vertices[i].X);
                    vertices.Add(mesh.Vertices[i].Y);
                    vertices.Add(mesh.Vertices[i].Z);

                    if (mesh.HasNormals)
                    {
                        vertices.Add(mesh.Normals[i].X);
                        vertices.Add(mesh.Normals[i].Y);
                        vertices.Add(mesh.Normals[i].Z);
                    }
                    else
                    {
                        vertices.Add(0.0f);
                        vertices.Add(0.0f);
                        vertices.Add(1.0f);
                    }

                    if (mesh.HasTextureCoords(0))
                    {
                        vertices.Add(mesh.TextureCoordinateChannels[0][i].X);
                        vertices.Add(mesh.TextureCoordinateChannels[0][i].Y);
                    }
                    else
                    {
                        vertices.Add(0.0f);
                        vertices.Add(0.0f);
                    }
                }

                // Обработка индексов
                for (int i = 0; i < mesh.FaceCount; i++)
                {
                    Face face = mesh.Faces[i];
                    for (int j = 0; j < face.IndexCount; j++)
                    {
                        indices.Add((uint)face.Indices[j]);
                    }
                }

                // Загрузка материалов
                if (mesh.MaterialIndex >= 0)
                {
                    Material material = scene.Materials[mesh.MaterialIndex];
                    List<Texture> diffuseMaps = LoadMaterialTextures(material, TextureType.Diffuse, "texture_diffuse");
                    List<Texture> specularMaps = LoadMaterialTextures(material, TextureType.Specular, "texture_specular");
                    textures.AddRange(diffuseMaps);
                    textures.AddRange(specularMaps);
                }

                // Добавление стандартных текстур, если их нет
                if (!textures.Any(t => t.Type == "texture_diffuse"))
                {
                    textures.Add(new Texture(defaultDiffuseTexture, "texture_diffuse", "default_diffuse"));
                }
                if (!textures.Any(t => t.Type == "texture_specular"))
                {
                    textures.Add(new Texture(defaultSpecularTexture, "texture_specular", "default_specular"));
                }

                return new Mesh(vertices.ToArray(), indices.ToArray(), textures);
            }

            private List<Texture> LoadMaterialTextures(Material material, TextureType type, string typeName)
            {
                List<Texture> textures = new List<Texture>();

                for (int i = 0; i < material.GetMaterialTextureCount(type); i++)
                {
                    material.GetMaterialTexture(type, i, out TextureSlot textureSlot);
                    string path = textureSlot.FilePath;

                    if (!loadedTextures.ContainsKey(path))
                    {
                        string fullPath = Path.Combine(directory, path);
                        int id;

                        if (File.Exists(fullPath))
                        {
                            id = Texture.LoadTextureFromFile(fullPath);
                            loadedTextures[path] = id;
                        }
                        else
                        {
                            id = typeName == "texture_diffuse" ? defaultDiffuseTexture : defaultSpecularTexture;
                            loadedTextures[path] = id;
                        }

                        textures.Add(new Texture(id, typeName, path));
                    }
                    else
                    {
                        textures.Add(new Texture(loadedTextures[path], typeName, path));
                    }
                }

                return textures;
            }

            public void Draw(Shader shader)
            {
                foreach (var mesh in meshes)
                {
                    mesh.Draw(shader);
                }
            }

            public void Dispose()
            {
                foreach (var mesh in meshes)
                {
                    mesh.Dispose();
                }
                foreach (var textureId in loadedTextures.Values)
                {
                    GL.DeleteTexture(textureId);
                }
                GL.DeleteTexture(defaultDiffuseTexture);
                GL.DeleteTexture(defaultSpecularTexture);
            }
        }

        // Empty event handlers - implementing them with basic functionality camera 
        private void glControl1_Click(object sender, EventArgs e)
        {
            // Forward to main click handler
            GlControl_Click(sender, e);
        }

        private void glControl1_Click_1(object sender, EventArgs e)
        {
            // Forward to main click handler
            GlControl_Click(sender, e);
        }

        private void glControl1_Click_2(object sender, EventArgs e)
        {
            // Forward to main click handler
            GlControl_Click(sender, e);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            // Implement if needed or remove
            // Currently empty as trackBar7 is handling scale
        }

        private void UserControl2_Load(object sender, EventArgs e)
        {

        }

        private void btnModel_Click(object sender, EventArgs e)
        {
            // Use open file dialog to select 3D model file
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "3D Models|*.fbx;*.obj;*.3ds;*.dae|FBX files (*.fbx)|*.fbx|OBJ files (*.obj)|*.obj|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadModel(openFileDialog.FileName);
                }
            }
        }
    }
}