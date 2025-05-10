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
using Quaternion = OpenTK.Mathematics.Quaternion;
using System.IO;
using Assimp;
using Assimp.Configs;
using OpenTK.GLControl;
using System.Reflection;
using static Julia_Launcher.SettingsManager;
using System.Diagnostics;

namespace Julia_Launcher
{
    public partial class UserControl2 : UserControl
    {
        // Константы
        private const float NORMAL_SMOOTHING_ANGLE = 66.0f;
        private const float MODEL_ROTATION_SPEED = 0.5f;
        private const float CAMERA_MOVEMENT_SPEED = 0.1f;
        private const float CAMERA_ZOOM_SPEED = 120.0f;
        private const float INITIAL_MODEL_SCALE = 0.5f;
        private const int MAX_BONE_COUNT = 100;
        private const float BACKGROUND_COLOR_R = 0.7f;
        private const float BACKGROUND_COLOR_G = 0.6f;
        private const float BACKGROUND_COLOR_B = 0.5f;
        private const float BACKGROUND_COLOR_A = 1.0f;



        private bool isAnimating = false;
        private DateTime lastFrameTime;
        private System.Windows.Forms.Timer animationTimer;
        private System.Windows.Forms.ComboBox comboAnimations;
        private System.Windows.Forms.Button btnPlayPause;
        private bool loaded = false;
        private Model model;
        private Camera camera;
        private Shader shader;
        private float rotation = 0.0f;
        private Vector3 modelPosition = Vector3.Zero;
        private float modelScale = INITIAL_MODEL_SCALE;
        private bool isDragging = false;
        private Point lastMousePos;

        private Vector3 lightPos = new Vector3(5.0f, 5.0f, 5.0f);
        private Vector3 lightColor = new Vector3(1.0f, 1.0f, 1.0f);
        private float ambientStrength = 0.1f;
        private float specularStrength = 0.5f;
        private float diffuseStrength = 0.8f;
        private float shininess = 16.0f;
        private float time = 0.0f;

        private void AutoPositionCamera()
        {
            if (model == null) return;

            var (min, max) = model.CalculateBoundingBox();
            Vector3 center = (min + max) / 2;
            Vector3 size = max - min;

            float fovYRad = MathHelper.DegreesToRadians(30);
            float tanFovYHalf = (float)Math.Tan(fovYRad / 2);
            float fovXRad = 2 * (float)Math.Atan(camera.AspectRatio * tanFovYHalf);
            float tanFovXHalf = (float)Math.Tan(fovXRad / 2);

            float width = size.X;
            float height = size.Y;
            float dX = (width / 2) / tanFovXHalf;
            float dY = (height / 2) / tanFovYHalf;
            float distance = Math.Max(dX, dY);

            distance *= 0.8f;

            float cameraHeightOffset = size.Y * 0.25f;
            camera.Position = center + new Vector3(0, cameraHeightOffset, distance);

            float focusHeightOffset = size.Y * 0.25f;
            Vector3 focusPoint = center + new Vector3(0, focusHeightOffset, 0);

            camera.LookAt(focusPoint);

            glControl1.Invalidate();
        }

        public UserControl2()
        {
            InitializeComponent();

            this.glControl1.Load += GlControl_Load;
            this.glControl1.Paint += GlControl_Paint;
            this.glControl1.Resize += GlControl_Resize;
            this.glControl1.MouseDown += GlControl_MouseDown;
            this.glControl1.MouseMove += GlControl_MouseMove;
            this.glControl1.MouseUp += GlControl_MouseUp;
            this.glControl1.MouseWheel += GlControl_MouseWheel;

            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = ReadSettings();

            trkHeight.Value = settings.Height;
            trkWeight.Value = settings.Weight;
            trkAge.Value = settings.Age;
            trkTone.Value = settings.Tone;
            trkSpeechRate.Value = settings.SpeechRate;
            trkVolume.Value = settings.Volume;
            trkTimbre.Value = settings.Timbre;
        }

        private void UserControl2_Load(object sender, EventArgs e)
        {
            string exePath = Application.StartupPath;
            string projectDir = Directory.GetParent(Directory.GetParent(exePath).FullName).FullName;
            string shadersDirectory = Path.Combine(projectDir, "Shaders");
            string vertexPath = Path.Combine(shadersDirectory, "vertex.glsl");
            string fragmentPath = Path.Combine(shadersDirectory, "fragment.glsl");

            if (!File.Exists(vertexPath) || !File.Exists(fragmentPath))
            {
                MessageBox.Show("Файлы шейдеров не найдены!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void GlControl_Load(object sender, EventArgs e)
        {
            try
            {
                GL.ClearColor(BACKGROUND_COLOR_R, BACKGROUND_COLOR_G, BACKGROUND_COLOR_B, BACKGROUND_COLOR_A);
                GL.Enable(EnableCap.DepthTest);
                GL.Viewport(0, 0, glControl1.Width, glControl1.Height);

                string appDirectory = Application.StartupPath;
                string shadersDirectory = Path.Combine(appDirectory, "Shaders");
                string vertexPath = Path.Combine(shadersDirectory, "vertex.glsl");
                string fragmentPath = Path.Combine(shadersDirectory, "fragment.glsl");

                if (!File.Exists(vertexPath) || !File.Exists(fragmentPath))
                {
                    MessageBox.Show($"Файлы шейдеров не найдены!\nПуть к vertex.glsl: {vertexPath}\nПуть к fragment.glsl: {fragmentPath}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                shader = new Shader(vertexPath, fragmentPath);
                camera = new Camera(new Vector3(0, 0, 3), glControl1.Width / (float)glControl1.Height);
                camera.LookAt(new Vector3(0, 0, 0));

                string modelDirectory = Path.Combine(appDirectory, "..", "..", "..", "Model");
                string modelPath = Path.Combine(modelDirectory, "sketch2.fbx");

                if (!File.Exists(modelPath))
                {
                    MessageBox.Show($"Файл модели не найден!\nПуть к sketch2.fbx: {modelPath}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

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

                Matrix4 modelMatrix = Matrix4.CreateScale(modelScale) *
                                     Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation)) *
                                     Matrix4.CreateTranslation(modelPosition);

                Matrix3 normalMatrix = Matrix3.Transpose(Matrix3.Invert(new Matrix3(modelMatrix)));
                shader.SetMatrix4("model", modelMatrix);
                shader.SetMatrix3("normalMatrix", normalMatrix);

                shader.SetVector3("lightPosition", lightPos);
                shader.SetVector3("lightColor", lightColor);
                shader.SetVector3("viewPosition", camera.Position);
                shader.SetFloat("ambientStrength", ambientStrength);
                shader.SetFloat("diffuseStrength", diffuseStrength);
                shader.SetFloat("specularStrength", specularStrength);
                shader.SetFloat("shininess", shininess);

                model.Draw(shader);
            }

            glControl1.SwapBuffers();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (model != null && model.HasAnimations && isAnimating)
            {
                DateTime now = DateTime.Now;
                float deltaTime = (float)(now - lastFrameTime).TotalSeconds;
                lastFrameTime = now;

                model.Update(deltaTime);

                glControl1.Invalidate();
            }
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

                if (ModifierKeys.HasFlag(Keys.Shift) || ModifierKeys.HasFlag(Keys.Control))
                {
                    rotation += xOffset * MODEL_ROTATION_SPEED;
                }
                else
                {
                    camera.ProcessMouseMovement(xOffset * CAMERA_MOVEMENT_SPEED, yOffset * CAMERA_MOVEMENT_SPEED);
                }

                glControl1.Invalidate();
            }
        }

        private void GlControl_MouseWheel(object sender, MouseEventArgs e)
        {
            camera.ProcessMouseScroll(e.Delta / CAMERA_ZOOM_SPEED);
            glControl1.Invalidate();
        }

        private void LoadModel(string path)
        {
            try
            {
                model?.Dispose();
                model = new Model(path);
                modelScale = INITIAL_MODEL_SCALE;
                modelPosition = Vector3.Zero;
                rotation = 0.0f;

                AutoPositionCamera();

                glControl1.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load model: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LogError(string message)
        {
            string logPath = Path.Combine(Application.StartupPath, "error_log.txt");
            try
            {
                File.AppendAllText(logPath, $"{DateTime.Now}: {message}\n");
            }
            catch (Exception logEx)
            {
                Console.WriteLine($"Failed to log error: {logEx.Message}");
            }
        }

        private void GlControl_Click(object sender, EventArgs e) { }

        private void trackBar7_Scroll(object sender, EventArgs e)
        {
            if (loaded && model != null)
            {
                modelScale = trkSpeechRate.Value / 100.0f;
                glControl1.Invalidate();
            }
        }

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

                Handle = GL.CreateProgram();
                GL.AttachShader(Handle, vertexShader);
                GL.AttachShader(Handle, fragmentShader);
                GL.LinkProgram(Handle);

                GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
                if (success == 0)
                {
                    string infoLog = GL.GetProgramInfoLog(Handle);
                    Console.WriteLine($"ERROR::PROGRAM::LINKING_FAILED\n{infoLog}");
                }

                GL.DetachShader(Handle, vertexShader);
                GL.DetachShader(Handle, fragmentShader);
                GL.DeleteShader(vertexShader);
                GL.DeleteShader(fragmentShader);

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

            public void SetInt(string name, int value)
            {
                GL.UseProgram(Handle);
                GL.Uniform1(GetUniformLocation(name), value);
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

            public void SetVector2(string name, Vector2 value)
            {
                GL.UseProgram(Handle);
                GL.Uniform2(GetUniformLocation(name), value);
            }

            public void SetVector4(string name, Vector4 value)
            {
                GL.UseProgram(Handle);
                GL.Uniform4(GetUniformLocation(name), value);
            }

            public void SetBool(string name, bool value)
            {
                GL.UseProgram(Handle);
                GL.Uniform1(GetUniformLocation(name), value ? 1 : 0);
            }

            public void SetTexture(string name, int textureUnit, int textureId)
            {
                GL.UseProgram(Handle);
                GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
                GL.BindTexture(TextureTarget.Texture2D, textureId);
                GL.Uniform1(GetUniformLocation(name), textureUnit);
            }

            public void SetPBRMaterial(string materialName, int diffuseMap, int normalMap, int specularMap, int roughnessMap, int aoMap, float metallic = 0.0f)
            {
                Use();
                SetTexture($"{materialName}_diffuse1", 0, diffuseMap);
                SetTexture($"{materialName}_normal1", 1, normalMap);
                SetTexture($"{materialName}_specular1", 2, specularMap);
                SetTexture($"{materialName}_roughness1", 3, roughnessMap);
                SetTexture($"{materialName}_ao1", 4, aoMap);
                SetFloat("metallic", metallic);
            }

            public void SetLightProperties(Vector3 position, Vector3 color, float intensity, Vector3 ambientLight, float gamma = 2.2f)
            {
                Use();
                SetVector3("lightPosition", position);
                SetVector3("lightColor", color);
                SetFloat("lightIntensity", intensity);
                SetVector3("ambientLight", ambientLight);
                SetFloat("gamma", gamma);
            }

            public void SetFogProperties(bool enable, Vector3 color, float near, float far)
            {
                Use();
                SetBool("enableFog", enable);
                SetVector3("fogColor", color);
                SetFloat("fogNear", near);
                SetFloat("fogFar", far);
            }

            public void SetShadowMap(int shadowMapTextureId, Matrix4 lightSpaceMatrix)
            {
                Use();
                SetTexture("shadowMap", 5, shadowMapTextureId);
                SetMatrix4("lightSpaceMatrix", lightSpaceMatrix);
            }

            public static Matrix3 CalculateNormalMatrix(Matrix4 modelMatrix)
            {
                Matrix4 normalMatrix = Matrix4.Transpose(Matrix4.Invert(modelMatrix));
                return new Matrix3(
                    normalMatrix.M11, normalMatrix.M12, normalMatrix.M13,
                    normalMatrix.M21, normalMatrix.M22, normalMatrix.M23,
                    normalMatrix.M31, normalMatrix.M32, normalMatrix.M33
                );
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

            public void SetCelShadingProperties(float[] diffuseThresholds = null, float[] diffuseFactors = null, float specularThreshold = 0.6f, Vector3? rimColor = null, float rimPower = 3.0f, float rimWidth = 0.3f)
            {
                Use();

                diffuseThresholds ??= new float[] { 0.8f, 0.6f, 0.3f };
                diffuseFactors ??= new float[] { 1.0f, 0.8f, 0.5f, 0.2f };
                rimColor ??= new Vector3(0.8f, 0.9f, 1.0f);

                for (int i = 0; i < diffuseThresholds.Length; i++)
                {
                    SetFloat($"diffuseThresholds[{i}]", diffuseThresholds[i]);
                }

                for (int i = 0; i < diffuseFactors.Length; i++)
                {
                    SetFloat($"diffuseFactors[{i}]", diffuseFactors[i]);
                }

                SetFloat("specularThreshold", specularThreshold);
                SetVector3("rimColor", rimColor.Value);
                SetFloat("rimPower", rimPower);
                SetFloat("rimWidth", rimWidth);
            }

            public void SetColorGradingProperties(float saturation = 1.2f, float brightness = 1.1f, float contrast = 1.15f)
            {
                Use();
                SetFloat("saturation", saturation);
                SetFloat("brightness", brightness);
                SetFloat("contrast", contrast);
            }

            public void SetOutlineProperties(float thickness = 0.005f, Vector3? color = null)
            {
                Use();
                color ??= new Vector3(0.0f, 0.0f, 0.0f);

                SetFloat("outlineThickness", thickness);
                SetVector3("outlineColor", color.Value);
            }

            public void SetEnvironmentProperties(Vector3? environmentColor = null, float environmentStrength = 0.3f, float fogDensity = 0.02f, Vector3? fogColor = null)
            {
                Use();
                environmentColor ??= new Vector3(0.1f, 0.1f, 0.2f);
                fogColor ??= new Vector3(0.8f, 0.9f, 1.0f);

                SetVector3("environmentColor", environmentColor.Value);
                SetFloat("environmentStrength", environmentStrength);
                SetFloat("fogDensity", fogDensity);
                SetVector3("fogColor", fogColor.Value);
            }

            public void SetSketchProperties(float threshold = 0.1f, float strength = 0.08f)
            {
                Use();
                SetFloat("sketchThreshold", threshold);
                SetFloat("sketchStrength", strength);
            }

            public void UpdateTime(float time)
            {
                Use();
                SetFloat("time", time);
            }

            public void SetShadowProperties(float shadowIntensity = 0.7f, float shadowSoftness = 0.05f)
            {
                Use();
                SetFloat("shadowIntensity", shadowIntensity);
                SetFloat("shadowSoftness", shadowSoftness);
            }
        }

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
                    image.RotateFlip(RotateFlipType.RotateNoneFlipY);

                    System.Drawing.Imaging.BitmapData data = image.LockBits(
                        new Rectangle(0, 0, image.Width, image.Height),
                        System.Drawing.Imaging.ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    GL.BindTexture(TextureTarget.Texture2D, textureId);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                        image.Width, image.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                    image.UnlockBits(data);

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)OpenTK.Graphics.OpenGL4.TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)OpenTK.Graphics.OpenGL4.TextureMagFilter.Linear);

                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                }

                return textureId;
            }
        }

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

            private void LoadModel(string path)
            {
                var importer = new AssimpContext();
                importer.SetConfig(new NormalSmoothingAngleConfig(NORMAL_SMOOTHING_ANGLE));
                Scene scene = importer.ImportFile(path,
                    PostProcessSteps.Triangulate |
                    PostProcessSteps.GenerateSmoothNormals |
                    PostProcessSteps.FlipUVs |
                    PostProcessSteps.CalculateTangentSpace |
                    PostProcessSteps.LimitBoneWeights);

                if (scene == null || scene.RootNode == null || (scene.SceneFlags & SceneFlags.Incomplete) == SceneFlags.Incomplete)
                {
                    throw new Exception("Не удалось загрузить модель с помощью Assimp.");
                }

                directory = Path.GetDirectoryName(path);
                LoadAnimations(scene);

                Matrix4 scaleTransform = Matrix4.CreateScale(INITIAL_MODEL_SCALE, INITIAL_MODEL_SCALE, INITIAL_MODEL_SCALE);
                ProcessNode(scene.RootNode, scene, scaleTransform);

                if (animations.Count > 0)
                {
                    hasAnimations = true;
                    animator.SetAnimation(animations.Values.First());
                }
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

            private void BuildSkeleton(Scene scene)
            {
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
                                Quaternion rot = frameIdx < nodeAnim.RotationKeyCount ?
                                    new Quaternion(nodeAnim.RotationKeys[frameIdx].Value.X, nodeAnim.RotationKeys[frameIdx].Value.Y,
                                                   nodeAnim.RotationKeys[frameIdx].Value.Z, nodeAnim.RotationKeys[frameIdx].Value.W) :
                                    Quaternion.Identity;
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

        private void glControl1_Click_2(object sender, EventArgs e)
        {
            GlControl_Click(sender, e);
        }



        private void btnModel_Click(object sender, EventArgs e)
        {
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

        private void trkHeight_Scroll(object sender, EventArgs e)
        {
            SaveSettings("Height", trkHeight.Value);
        }

        private void trkWeight_Scroll(object sender, EventArgs e)
        {
            SaveSettings("Weight", trkWeight.Value);
        }

        private void trkAge_Scroll(object sender, EventArgs e)
        {
            SaveSettings("Age", trkAge.Value);
        }

        private void trkTone_Scroll(object sender, EventArgs e)
        {
            SaveSettings("Tone", trkTone.Value);
        }

        private void trkSpeechRate_Scroll(object sender, EventArgs e)
        {
            SaveSettings("SpeechRate", trkSpeechRate.Value);
        }

        private void trkVolume_Scroll(object sender, EventArgs e)
        {
            SaveSettings("Volume", trkVolume.Value);
        }

        private void trkTimbre_Scroll(object sender, EventArgs e)
        {
            SaveSettings("Timbre", trkTimbre.Value);
        }

        private void btnAddClothing_Click(object sender, EventArgs e)
        {
            PlayHelloWorld();
        }

        private void label2_Click(object sender, EventArgs e) { }

        public void PlayHelloWorld()
        {
            string filePath = "output.wav";

            Process process = new Process();
            process.StartInfo.FileName = "python";
            process.StartInfo.Arguments = "script.py";
            process.Start();
            process.WaitForExit();

            if (File.Exists(filePath))
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(filePath);
                player.Play();
            }
            else
            {
                Console.WriteLine($"Ошибка: файл {filePath} не найден.");
            }
        }
    }
}