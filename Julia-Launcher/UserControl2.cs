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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static Julia_Launcher.SettingsManager;
using System.Reflection;

namespace Julia_Launcher
{
    public static class Matrix4Extensions
    {
        public static Vector3 ExtractTranslation(this Matrix4 matrix)
        {
            return new Vector3(matrix.M41, matrix.M42, matrix.M43);
        }

        public static Quaternion ExtractRotation(this Matrix4 matrix)
        {
            // Remove scaling
            Vector3 scale = matrix.ExtractScale();
            Matrix4 rotMat = matrix;

            if (scale.X != 0)
            {
                rotMat.M11 /= scale.X;
                rotMat.M12 /= scale.X;
                rotMat.M13 /= scale.X;
            }

            if (scale.Y != 0)
            {
                rotMat.M21 /= scale.Y;
                rotMat.M22 /= scale.Y;
                rotMat.M23 /= scale.Y;
            }

            if (scale.Z != 0)
            {
                rotMat.M31 /= scale.Z;
                rotMat.M32 /= scale.Z;
                rotMat.M33 /= scale.Z;
            }
            // Создание Matrix3 из верхних левых 3x3 элементов
            Matrix3 rotationMatrix = new Matrix3(
                rotMat.M11, rotMat.M12, rotMat.M13,
                rotMat.M21, rotMat.M22, rotMat.M23,
                rotMat.M31, rotMat.M32, rotMat.M33
            );

            // Извлечение кватерниона из Matrix3
            return Quaternion.FromMatrix(rotationMatrix);
        }

        public static Vector3 ExtractScale(this Matrix4 matrix)
        {
            return new Vector3(
                new Vector3(matrix.M11, matrix.M12, matrix.M13).Length,
                new Vector3(matrix.M21, matrix.M22, matrix.M23).Length,
                new Vector3(matrix.M31, matrix.M32, matrix.M33).Length
            );
        }

        public static Matrix4 ClearTranslation(this Matrix4 matrix)
        {
            Matrix4 result = matrix;
            result.M41 = 0;
            result.M42 = 0;
            result.M43 = 0;
            return result;
        }
    }




    public partial class UserControl2 : UserControl
    {
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
        private float modelScale = 1.0f;
        private bool isDragging = false;
        private Point lastMousePos;

        private Vector3 lightPos = new Vector3(3.2f, 5.0f, 4.0f);
        private Vector3 lightColor = new Vector3(1.0f, 1.0f, 1.0f);
        private float ambientStrength = 0.1f;
        private float specularStrength = 0.5f;
        private float diffuseStrength = 0.8f; // Сила диффузного освещения
        private float shininess = 16.0f;      // Блеск (степень зеркальности)

        private void AutoPositionCamera()
        {
            if (model == null) return;

            var (min, max) = model.CalculateBoundingBox();
            Vector3 center = (min + max) / 2;
            Vector3 size = max - min;

            float fovYRad = MathHelper.DegreesToRadians(30); // Вертикальный FOV
            float tanFovYHalf = (float)Math.Tan(fovYRad / 2);
            float fovXRad = 2 * (float)Math.Atan(camera.AspectRatio * tanFovYHalf); // Горизонтальный FOV
            float tanFovXHalf = (float)Math.Tan(fovXRad / 2);

            float width = size.X;
            float height = size.Y;
            float dX = (width / 2) / tanFovXHalf;
            float dY = (height / 2) / tanFovYHalf;
            float distance = Math.Max(dX, dY);

            distance *= 1.2f; // Запас 20%

            camera.Position = center + new Vector3(0, 0, distance);
            camera.LookAt(center);

            glControl1.Invalidate();
        }

        public UserControl2()
        {
            InitializeComponent();


            // Использовать существующий glControl1 вместо создания нового
            this.glControl1.Load += GlControl_Load;
            this.glControl1.Paint += GlControl_Paint;
            this.glControl1.Resize += GlControl_Resize;
            this.glControl1.MouseDown += GlControl_MouseDown;
            this.glControl1.MouseMove += GlControl_MouseMove;
            this.glControl1.MouseUp += GlControl_MouseUp;
            this.glControl1.MouseWheel += GlControl_MouseWheel;


            // Загружаем настройки при запуске
            LoadSettings();
        }
        private void LoadSettings()
        {
            var settings = ReadSettings();

            // TrackBar controls
            trkHeight.Value = settings.Height;
            trkWeight.Value = settings.Weight;
            trkAge.Value = settings.Age;
            trkTone.Value = settings.Tone;
            trkSpeechRate.Value = settings.SpeechRate;
            trkVolume.Value = settings.Volume;
            trkTimbre.Value = settings.Timbre;
        }

        // Remove the InitializeOpenGL method entirely

        private void GlControl_Load(object sender, EventArgs e)
        {
            try
            {
                GL.ClearColor(0.7f, 0.9f, 0.5f, 1.0f);
                GL.Enable(EnableCap.DepthTest);
                GL.Viewport(0, 0, glControl1.Width, glControl1.Height);

                string appDirectory = "F:\\Work\\C#\\Julia-Launcher\\Julia-Launcher\\Julia-Launcher";
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

                string modelPath = "F:\\Work\\C#\\Julia-Launcher\\Julia-Launcher\\Julia-Launcher\\Model\\sketch2.fbx";
                LoadModel(modelPath); // Здесь теперь вызывается AutoPositionCamera
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

                Matrix4 modelMatrix = Matrix4.CreateScale(modelScale) *
                                     Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation)) *
                                     Matrix4.CreateTranslation(modelPosition);

                // Вычислить нормальную матрицу
                Matrix3 normalMatrix = Matrix3.Transpose(Matrix3.Invert(new Matrix3(modelMatrix)));
                shader.SetMatrix4("model", modelMatrix);
                shader.SetMatrix3("normalMatrix", normalMatrix);

                // Установить униформу освещения
                shader.SetVector3("lightPosition", lightPos);
                shader.SetVector3("lightColor", lightColor);
                shader.SetVector3("viewPosition", camera.Position);
                shader.SetFloat("ambientStrength", ambientStrength);
                shader.SetFloat("diffuseStrength", diffuseStrength);
                shader.SetFloat("specularStrength", specularStrength);
                shader.SetFloat("shininess", shininess);

                // Рисуем модель (которая установит преобразования костей, если анимирована)
                model.Draw(shader);
            }

            glControl1.SwapBuffers();
        }

        // Обратный вызов таймера анимации
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (model != null && model.HasAnimations && isAnimating)
            {
                // Рассчитать дельта-время для плавной анимации
                DateTime now = DateTime.Now;
                float deltaTime = (float)(now - lastFrameTime).TotalSeconds;
                lastFrameTime = now;

                // Обновляем анимацию модели
                model.Update(deltaTime);

                // Запрос на перерисовку
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

        private void LoadModel(string path)
        {
            try
            {
                model?.Dispose();
                model = new Model(path);
                modelScale = 1.0f;
                modelPosition = Vector3.Zero;
                rotation = 0.0f;

                // Попробуем загрузить камеру из файла, но всё равно применим автопозиционирование
                LoadCameraFromFile(path);
                AutoPositionCamera(); // Всегда вызываем после загрузки модели

                glControl1.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load model: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCameraFromFile(string path)
        {
            try
            {
                var importer = new AssimpContext();
                Scene scene = importer.ImportFile(path,
                    PostProcessSteps.Triangulate |
                    PostProcessSteps.GenerateSmoothNormals |
                    PostProcessSteps.FlipUVs |
                    PostProcessSteps.CalculateTangentSpace);

                if (scene != null && scene.CameraCount > 0)
                {
                    // Загрузка камеры из файла
                    Assimp.Camera assimpCamera = scene.Cameras[0];
                    if (camera != null)
                    {
                        camera.SetFromAssimpCamera(assimpCamera);
                        Console.WriteLine($"Camera loaded from file: {assimpCamera.Name}");
                        glControl1.Invalidate();
                    }
                    else
                    {
                        camera = new Camera(
                            new Vector3(0, 0, 3),
                            glControl1.Width / (float)glControl1.Height);
                        camera.SetFromAssimpCamera(assimpCamera);
                    }
                }
                else
                {
                    Console.WriteLine("No camera found in the file, auto positioning camera");
                    AutoPositionCamera(); // Автоматическое позиционирование камеры
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading camera: {ex.Message}. Auto positioning camera.");
                AutoPositionCamera(); // В случае ошибки тоже используем автоматическое позиционирование
            }
        }

        private void GlControl_Click(object sender, EventArgs e){ }

        private void trackBar7_Scroll(object sender, EventArgs e)
        {
            if (loaded && model != null)
            {
                // Assume trackBar7 controls model scale
                modelScale = trkSpeechRate.Value / 100.0f;
                glControl1.Invalidate();
            }
        }

        // Класс камеры для обработки преобразований камеры
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


            public void SetFromAssimpCamera(Assimp.Camera assimpCamera, bool convertCoordinateSystem = true)
            {
                // Store the original position and target
                Vector3 originalPosition = new Vector3(assimpCamera.Position.X, assimpCamera.Position.Y, assimpCamera.Position.Z);
                Vector3 originalTarget = new Vector3(
                    assimpCamera.Position.X + assimpCamera.Direction.X,
                    assimpCamera.Position.Y + assimpCamera.Direction.Y,
                    assimpCamera.Position.Z + assimpCamera.Direction.Z);
                Vector3 originalUp = new Vector3(assimpCamera.Up.X, assimpCamera.Up.Y, assimpCamera.Up.Z);

                // Convert from Assimp to OpenTK coordinate system if needed
                if (convertCoordinateSystem)
                {
                    // Convert from Y-up (Assimp) to Y-up (OpenTK) - may need adjustment based on your specific case
                    // For typical FBX files we might need to flip Z or invert other axes
                    Position = new Vector3(originalPosition.X, originalPosition.Y, -originalPosition.Z);
                    Vector3 target = new Vector3(originalTarget.X, originalTarget.Y, -originalTarget.Z);
                    Vector3 up = new Vector3(originalUp.X, originalUp.Y, -originalUp.Z);

                    // Calculate front direction from position to target
                    Front = Vector3.Normalize(target - Position);

                    // Calculate the right vector from front and up
                    Right = Vector3.Normalize(Vector3.Cross(up, Front));

                    // Recalculate the up vector to ensure orthogonality
                    Up = Vector3.Normalize(Vector3.Cross(Front, Right));
                }
                else
                {
                    // Use directly without conversion
                    Position = originalPosition;
                    Front = Vector3.Normalize(new Vector3(assimpCamera.Direction.X, assimpCamera.Direction.Y, assimpCamera.Direction.Z));
                    Up = Vector3.Normalize(originalUp);
                    Right = Vector3.Normalize(Vector3.Cross(Up, Front));
                }

                // Calculate yaw and pitch from the Front vector
                yaw = MathHelper.RadiansToDegrees((float)Math.Atan2(Front.Z, Front.X));
                pitch = MathHelper.RadiansToDegrees((float)Math.Asin(Front.Y));

                // Set field of view (zoom)
                zoom = MathHelper.RadiansToDegrees(assimpCamera.FieldOfview);
                if (zoom <= 0) zoom = 45.0f;

                // Set aspect ratio
                AspectRatio = assimpCamera.AspectRatio > 0 ? assimpCamera.AspectRatio : 1.0f;

                // Make sure all vectors are updated
                UpdateCameraVectors();

                // Log camera import details for debugging
                Console.WriteLine($"Imported camera - Position: {Position}, Front: {Front}, Up: {Up}");
                Console.WriteLine($"FOV: {zoom}, Yaw: {yaw}, Pitch: {pitch}");
            }


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
                    1000.0f);
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

        // Класс шейдера для обработки шейдеров GLSL
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

            // Texture binding helper methods
            public void SetTexture(string name, int textureUnit, int textureId)
            {
                GL.UseProgram(Handle);
                GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
                GL.BindTexture(TextureTarget.Texture2D, textureId);
                GL.Uniform1(GetUniformLocation(name), textureUnit);
            }

            // Methods to load common PBR material settings
            public void SetPBRMaterial(
                string materialName,
                int diffuseMap,
                int normalMap,
                int specularMap,
                int roughnessMap,
                int aoMap,
                float metallic = 0.0f)
            {
                Use();
                SetTexture($"{materialName}_diffuse1", 0, diffuseMap);
                SetTexture($"{materialName}_normal1", 1, normalMap);
                SetTexture($"{materialName}_specular1", 2, specularMap);
                SetTexture($"{materialName}_roughness1", 3, roughnessMap);
                SetTexture($"{materialName}_ao1", 4, aoMap);
                SetFloat("metallic", metallic);
            }

            // Method to set standard lighting properties
            public void SetLightProperties(
                Vector3 position,
                Vector3 color,
                float intensity,
                Vector3 ambientLight,
                float gamma = 2.2f)
            {
                Use();
                SetVector3("lightPosition", position);
                SetVector3("lightColor", color);
                SetFloat("lightIntensity", intensity);
                SetVector3("ambientLight", ambientLight);
                SetFloat("gamma", gamma);
            }

            // Method to configure fog
            public void SetFogProperties(bool enable, Vector3 color, float near, float far)
            {
                Use();
                SetBool("enableFog", enable);
                SetVector3("fogColor", color);
                SetFloat("fogNear", near);
                SetFloat("fogFar", far);
            }

            // Shadow mapping configuration
            public void SetShadowMap(int shadowMapTextureId, Matrix4 lightSpaceMatrix)
            {
                Use();
                SetTexture("shadowMap", 5, shadowMapTextureId);
                SetMatrix4("lightSpaceMatrix", lightSpaceMatrix);
            }

            // Helper method to calculate normal matrix from model matrix
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
        }

        // Класс сетки для хранения данных сетки
        // Модифицированный класс сетки для обработки данных костей
        public class Mesh
        {
            private int VAO, VBO, EBO;
            private int indexCount;
            private bool hasBones;
            public List<Texture> Textures { get; private set; }
            private float[] vertices;

            // Публичное свойство для доступа к hasBones
            public bool HasBones => hasBones;

            public Mesh(float[] vertices, uint[] indices, List<Texture> textures, bool hasBones = false)
            {
                this.vertices = vertices; // Сохраняем копию вершин
                Textures = textures;
                indexCount = indices.Length;
                this.hasBones = hasBones;
 

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
                int stride = hasBones ? 16 * sizeof(float) : 8 * sizeof(float);

                // Position attribute
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
                GL.EnableVertexAttribArray(0);

                // Normal attribute
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);

                // Texture coords attribute
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));
                GL.EnableVertexAttribArray(2);


                // If we have bones, set up bone attributes
                if (hasBones)
                {
                    // Bone IDs (as vec4 of floats)
                    GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, stride, 8 * sizeof(float));
                    GL.EnableVertexAttribArray(3);

                    // Bone weights (as vec4 of floats)
                    GL.VertexAttribPointer(4, 4, VertexAttribPointerType.Float, false, stride, 12 * sizeof(float));
                    GL.EnableVertexAttribArray(4);
                }

                GL.BindVertexArray(0);
            }


            // Новый метод для получения вершин
            public float[] GetVertices()
            {
                return vertices;
            }

            public void Draw(Shader shader)
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
                GL.DrawElements(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);
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

        // Класс текстуры для хранения данных текстуры
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

        // Добавьте эти новые классы для обработки анимации

        // Хранить информацию о костях и иерархию
        public class Bone
        {
            public int ID { get; private set; }
            public string Name { get; private set; }
            public Matrix4 OffsetMatrix { get; private set; }
            public List<KeyFrame> KeyFrames { get; private set; }
            public Matrix4 FinalTransformation { get; set; }

            public Bone(int id, string name, Matrix4 offsetMatrix)
            {
                ID = id;
                Name = name;
                OffsetMatrix = offsetMatrix;
                KeyFrames = new List<KeyFrame>();
                FinalTransformation = Matrix4.Identity;
            }

            public void AddKeyFrame(KeyFrame keyFrame)
            {
                KeyFrames.Add(keyFrame);
            }

            // Интерполяция между ключевыми кадрами на основе времени анимации
            public Matrix4 InterpolateTransform(float animationTime)
            {
                if (KeyFrames.Count == 0) return Matrix4.Identity;
                if (KeyFrames.Count == 1) return KeyFrames[0].Transform;

                // Найти, между какими ключевыми кадрами следует выполнить интерполяцию
                int frameIndex = FindFrameIndex(animationTime);
                int nextFrameIndex = (frameIndex + 1) % KeyFrames.Count;

                KeyFrame currentFrame = KeyFrames[frameIndex];
                KeyFrame nextFrame = KeyFrames[nextFrameIndex];

                float delta = CalculateDelta(animationTime, currentFrame, nextFrame);

                // Интерполяция между текущим кадром и следующим кадром
                return InterpolateMatrices(currentFrame.Transform, nextFrame.Transform, delta);
            }

            private int FindFrameIndex(float animationTime)
            {
                for (int i = 0; i < KeyFrames.Count - 1; i++)
                {
                    if (animationTime < KeyFrames[i + 1].Time)
                        return i;
                }
                return KeyFrames.Count - 1;
            }

            private float CalculateDelta(float animationTime, KeyFrame currentFrame, KeyFrame nextFrame)
            {
                float framesDiff = nextFrame.Time - currentFrame.Time;
                if (framesDiff < 0.0001f) return 0;

                float delta = (animationTime - currentFrame.Time) / framesDiff;
                return Math.Clamp(delta, 0.0f, 1.0f);
            }

            private Matrix4 InterpolateMatrices(Matrix4 start, Matrix4 end, float factor)
            {
                // Извлечь положение, поворот и масштаб из матриц
                Vector3 startPos = start.ExtractTranslation();
                Vector3 endPos = end.ExtractTranslation();

                Quaternion startRot = start.ExtractRotation();
                Quaternion endRot = end.ExtractRotation();

                Vector3 startScale = start.ExtractScale();
                Vector3 endScale = end.ExtractScale();

                // Интерполировать компоненты
                Vector3 pos = Vector3.Lerp(startPos, endPos, factor);
                Quaternion rot = Quaternion.Slerp(startRot, endRot, factor);
                Vector3 scale = Vector3.Lerp(startScale, endScale, factor);

                // Объединить в новую трансформацию
                Matrix4 result = Matrix4.CreateScale(scale) *
                                  Matrix4.CreateFromQuaternion(rot) *
                                  Matrix4.CreateTranslation(pos);

                return result;
            }
        }

        // Сохраняем данные ключевых кадров для анимации
        public class KeyFrame
        {
            public float Time { get; private set; }
            public Matrix4 Transform { get; private set; }

            public KeyFrame(float time, Matrix4 transform)
            {
                Time = time;
                Transform = transform;
            }
        }

        // Класс анимации для хранения и управления одной анимацией
        public class Animation
        {
            public string Name { get; private set; }
            public float Duration { get; private set; } // в секундах
            public float TicksPerSecond { get; private set; }
            public Dictionary<string, Bone> Bones { get; private set; }

            public Animation(string name, float duration, float ticksPerSecond)
            {
                Name = name;
                Duration = duration;
                TicksPerSecond = ticksPerSecond > 0 ? ticksPerSecond : 25.0f; // Резервный вариант по умолчанию
                Bones = new Dictionary<string, Bone>();
            }

            public void AddBone(Bone bone)
            {
                Bones[bone.Name] = bone;
            }
        }

        // Класс аниматора для обработки воспроизводимых анимаций
        public class Animator
        {
            private Animation currentAnimation;
            private float currentTime = 0.0f;
            private bool isPlaying = false;
            private Dictionary<string, Matrix4> boneTransforms = new Dictionary<string, Matrix4>();
            private Matrix4[] finalBoneMatrices;
            private int bonesCount;

            // Mapping from bone name to index in final matrices array
            private Dictionary<string, int> boneMapping = new Dictionary<string, int>();

            public Animator(int maxBones = 100)
            {
                finalBoneMatrices = new Matrix4[maxBones];
                for (int i = 0; i < maxBones; i++)
                {
                    finalBoneMatrices[i] = Matrix4.Identity;
                }
                bonesCount = 0;
            }

            public void SetAnimation(Animation animation)
            {
                currentAnimation = animation;
                currentTime = 0.0f;
                isPlaying = true;

                // Build the bone mapping if needed
                foreach (var bone in animation.Bones.Values)
                {
                    if (!boneMapping.ContainsKey(bone.Name))
                    {
                        boneMapping[bone.Name] = bonesCount++;
                    }
                }
            }

            public void Update(float deltaTime)
            {
                if (currentAnimation == null || !isPlaying) return;

                currentTime += deltaTime * currentAnimation.TicksPerSecond;

                // Loop the animation
                if (currentTime > currentAnimation.Duration)
                {
                    currentTime = currentTime % currentAnimation.Duration;
                }

                CalculateBoneTransforms();
            }

            private void CalculateBoneTransforms()
            {
                foreach (var bone in currentAnimation.Bones.Values)
                {
                    Matrix4 boneTransform = bone.InterpolateTransform(currentTime);

                    int boneIndex = boneMapping[bone.Name];
                    finalBoneMatrices[boneIndex] = bone.OffsetMatrix * boneTransform;
                }
            }

            public Matrix4[] GetFinalBoneMatrices()
            {
                return finalBoneMatrices;
            }

            public void Play()
            {
                isPlaying = true;
            }

            public void Pause()
            {
                isPlaying = false;
            }

            public void Stop()
            {
                isPlaying = false;
                currentTime = 0.0f;
            }

            public float CurrentTime
            {
                get { return currentTime; }
                set { currentTime = value; }
            }

            public bool IsPlaying
            {
                get { return isPlaying; }
            }

            public Animation CurrentAnimation
            {
                get { return currentAnimation; }
            }
        }


        // Класс модели для загрузки и рендеринга 3D-моделей
        // Измените класс модели для поддержки анимации
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

            public bool HasAnimations => hasAnimations;
            public Animator Animator => animator;
            public IReadOnlyDictionary<string, Animation> Animations => animations;

            public Model(string path)
            {
                defaultDiffuseTexture = CreateDefaultTexture(255, 255, 255, 255);
                defaultSpecularTexture = CreateDefaultTexture(0, 0, 0, 255);
                animator = new Animator(100); // Поддержка до 100 костей
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
                importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));
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
                ProcessNode(scene.RootNode, scene, Matrix4.Identity);

                if (animations.Count > 0)
                {
                    hasAnimations = true;
                    animator.SetAnimation(animations.Values.First());
                }
            }
            public (Vector3 Min, Vector3 Max) CalculateBoundingBox()
            {
                if (meshes.Count == 0) return (Vector3.Zero, Vector3.Zero);

                Vector3 min = new Vector3(float.MaxValue);
                Vector3 max = new Vector3(float.MinValue);

                foreach (var mesh in meshes)
                {
                    var vertices = mesh.GetVertices();
                    int stride = mesh.HasBones ? 16 : 8; // Шаг зависит от наличия костей
                    for (int i = 0; i < vertices.Length; i += stride)
                    {
                        Vector3 pos = new Vector3(vertices[i], vertices[i + 1], vertices[i + 2]);
                        min = Vector3.ComponentMin(min, pos);
                        max = Vector3.ComponentMax(max, pos);
                    }
                }

                return (min, max);
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
                    var vertices = mesh.GetVertices(); // Предполагается, что у Mesh есть метод для получения вершин
                    for (int i = 0; i < vertices.Length; i += 8) // Шаг 8, если вершина содержит позицию, нормали и текстурные координаты
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

        // Пустые обработчики событий — реализуем их с базовой функциональностью camera
        private void glControl1_Click(object sender, EventArgs e)
        {
            // Переслать главному обработчику кликов
            GlControl_Click(sender, e);
        }

        private void glControl1_Click_1(object sender, EventArgs e)
        {
            // Переслать главному обработчику кликов
            GlControl_Click(sender, e);
        }

        private void glControl1_Click_2(object sender, EventArgs e)
        {
            // Переслать главному обработчику кликов
            GlControl_Click(sender, e);
        }



        private void UserControl2_Load(object sender, EventArgs e) { }

        private void btnModel_Click(object sender, EventArgs e)
        {
            // Используйте диалог открытия файла для выбора файла 3D-модели
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


        // Trackbars
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
    }
}