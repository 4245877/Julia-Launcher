using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.IO;
using static Julia_Launcher.SettingsManager;
using System.Diagnostics;
using System.Threading.Tasks;
using Assimp;

namespace Julia_Launcher
{
    public partial class UserControl2 : UserControl
    {
        // Константы
        private const float MODEL_ROTATION_SPEED = 0.5f;
        private const float CAMERA_MOVEMENT_SPEED = 0.1f;
        private const float CAMERA_ZOOM_SPEED = 120.0f;


        private const float BACKGROUND_COLOR_R = 0.11764706f;  
        private const float BACKGROUND_COLOR_G = 0.23529412f;  
        private const float BACKGROUND_COLOR_B = 0.44705882f;  
        private const float BACKGROUND_COLOR_A = 1.0f;

        private float modelScale = Model.INITIAL_MODEL_SCALE;

        private bool isAnimating = false;
        private DateTime lastFrameTime;
        private System.Windows.Forms.Timer animationTimer;
        private bool loaded = false;
        private Model model;
        private Camera camera;
        private Shader shader;
        private float rotation = 0.0f;
        private Vector3 modelPosition = Vector3.Zero;
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
            if (model == null || camera == null)
            {
                LogError("Модель или камера не инициализированы в AutoPositionCamera.");
                return;
            }

            var (min, max) = model.CalculateBoundingBox();
            Vector3 center = (min + max) / 2f;
            Vector3 size = max - min;

            if (size.LengthSquared < 0.0001f)
            {
                LogError("Размер модели слишком мал для AutoPositionCamera.");
                camera.Position = new Vector3(0, 1, 3);
                camera.LookAt(Vector3.Zero);
                glControl1.Invalidate();
                return;
            }

            float fovYDeg = 45f;
            float fovYRad = MathHelper.DegreesToRadians(fovYDeg);
            float tanFovYHalf = (float)Math.Tan(fovYRad / 2f);

            float baseDistance = (size.Y / 2f) / tanFovYHalf;
            float cameraViewDistance = baseDistance * 0.3f; 

            // Камера и фокус на уровне центра модели (50% высоты)
            float focusHeight = min.Y + (size.Y * 0.3f);
            float cameraActualHeight = focusHeight;

            // Угол 0 градусов — камера прямо перед персонажем
            float horizontalAngleDegrees = 0.0f;
            float horizontalAngleRadians = MathHelper.DegreesToRadians(horizontalAngleDegrees);

            float cameraX = center.X + cameraViewDistance * (float)Math.Sin(horizontalAngleRadians); // Sin(0) = 0
            float cameraZ = center.Z + cameraViewDistance * (float)Math.Cos(horizontalAngleRadians); // Cos(0) = 1

            camera.Position = new Vector3(cameraX, cameraActualHeight, cameraZ);
            Vector3 targetFocusPoint = new Vector3(center.X, focusHeight, center.Z);
            camera.LookAt(targetFocusPoint);

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

                if (!CheckShaderFiles(vertexPath, fragmentPath))
                {
                    return;
                }

                shader = new Shader(vertexPath, fragmentPath);
                camera = new Camera(new Vector3(0, 0, 3), glControl1.Width / (float)glControl1.Height);
                camera.LookAt(new Vector3(0, 0, 0));

                string modelsDirectory = Path.Combine(appDirectory, "Model");
                string modelPath = Path.Combine(modelsDirectory, "sketch2.fbx");

                if (!File.Exists(modelPath))
                {
                    MessageBox.Show($"Файл модели не найден!\nПуть к sketch2.fbx: {modelPath}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LoadModel(modelPath);
                loaded = true;

                // Запуск асинхронного цикла
                StartAnimationLoop();
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
                modelScale = Model.INITIAL_MODEL_SCALE;
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

        



        private bool CheckShaderFiles(string vertexPath, string fragmentPath)
        {
            if (!File.Exists(vertexPath) || !File.Exists(fragmentPath))
            {
                MessageBox.Show($"Файлы шейдеров не найдены!\nПуть к vertex.glsl: {vertexPath}\nПуть к fragment.glsl: {fragmentPath}",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        private async void StartAnimationLoop()
        {
            while (true)
            {
                if (model != null && model.HasAnimations && isAnimating)
                {
                    DateTime now = DateTime.Now;
                    float deltaTime = (float)(now - lastFrameTime).TotalSeconds;
                    lastFrameTime = now;

                    model.Update(deltaTime);

                    glControl1.Invalidate();
                }

                await Task.Delay(16); // ~60 FPS
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

            public void Use()
            {
                GL.UseProgram(Handle);
            }

            public void SetMatrix3(string name, Matrix3 value)
            {
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
                GL.Uniform1(GetUniformLocation(name), value);
            }

            public void SetFloat(string name, float value)
            {
                GL.Uniform1(GetUniformLocation(name), value);
            }

            public void SetVector3(string name, Vector3 value)
            {
                GL.Uniform3(GetUniformLocation(name), value);
            }

            public void SetVector2(string name, Vector2 value)
            {
                GL.Uniform2(GetUniformLocation(name), value);
            }

            public void SetVector4(string name, Vector4 value)
            {
                GL.Uniform4(GetUniformLocation(name), value);
            }

            public void SetBool(string name, bool value)
            {
                GL.Uniform1(GetUniformLocation(name), value ? 1 : 0);
            }

            public void SetTexture(string name, int textureUnit, int textureId)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
                GL.BindTexture(TextureTarget.Texture2D, textureId);
                GL.Uniform1(GetUniformLocation(name), textureUnit);
            }

            public void SetPBRMaterial(string materialName, int diffuseMap, int normalMap, int specularMap, int roughnessMap, int aoMap, float metallic = 0.0f)
            {
                SetTexture($"{materialName}_diffuse1", 0, diffuseMap);
                SetTexture($"{materialName}_normal1", 1, normalMap);
                SetTexture($"{materialName}_specular1", 2, specularMap);
                SetTexture($"{materialName}_roughness1", 3, roughnessMap);
                SetTexture($"{materialName}_ao1", 4, aoMap);
                SetFloat("metallic", metallic);
            }

            public void SetLightProperties(Vector3 position, Vector3 color, float intensity, Vector3 ambientLight, float gamma = 2.2f)
            {
                SetVector3("lightPosition", position);
                SetVector3("lightColor", color);
                SetFloat("lightIntensity", intensity);
                SetVector3("ambientLight", ambientLight);
                SetFloat("gamma", gamma);
            }

            public void SetFogProperties(bool enable, Vector3 color, float near, float far)
            {
                SetBool("enableFog", enable);
                SetVector3("fogColor", color);
                SetFloat("fogNear", near);
                SetFloat("fogFar", far);
            }

            public void SetShadowMap(int shadowMapTextureId, Matrix4 lightSpaceMatrix)
            {
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
                SetFloat("saturation", saturation);
                SetFloat("brightness", brightness);
                SetFloat("contrast", contrast);
            }

            public void SetOutlineProperties(float thickness = 0.005f, Vector3? color = null)
            {
                color ??= new Vector3(0.0f, 0.0f, 0.0f);

                SetFloat("outlineThickness", thickness);
                SetVector3("outlineColor", color.Value);
            }

            public void SetEnvironmentProperties(Vector3? environmentColor = null, float environmentStrength = 0.3f, float fogDensity = 0.02f, Vector3? fogColor = null)
            {
                environmentColor ??= new Vector3(0.1f, 0.1f, 0.2f);
                fogColor ??= new Vector3(0.8f, 0.9f, 1.0f);

                SetVector3("environmentColor", environmentColor.Value);
                SetFloat("environmentStrength", environmentStrength);
                SetFloat("fogDensity", fogDensity);
                SetVector3("fogColor", fogColor.Value);
            }

            public void SetSketchProperties(float threshold = 0.1f, float strength = 0.08f)
            {
                SetFloat("sketchThreshold", threshold);
                SetFloat("sketchStrength", strength);
            }

            public void UpdateTime(float time)
            {
                SetFloat("time", time);
            }

            public void SetShadowProperties(float shadowIntensity = 0.7f, float shadowSoftness = 0.05f)
            {
                SetFloat("shadowIntensity", shadowIntensity);
                SetFloat("shadowSoftness", shadowSoftness);
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
            PlayHelloWorldAsync();
        }

        private async void PlayHelloWorldAsync()
        {
            string filePath = "output.wav";

            Process process = new Process();
            process.StartInfo.FileName = "python";
            process.StartInfo.Arguments = "script.py";
            process.EnableRaisingEvents = true;
            process.Exited += (s, e) =>
            {
                // Выполняется в UI-потоке после завершения процесса
                if (File.Exists(filePath))
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(filePath);
                    player.Play();
                }
                else
                {
                    Console.WriteLine($"Ошибка: файл {filePath} не найден.");
                }
            };
            process.Start();

            // UI остаётся отзывчивым
        }
    }
}