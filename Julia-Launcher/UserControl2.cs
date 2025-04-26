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

using static Julia_Launcher.UserControl2;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

using static Julia_Launcher.SettingsManager;
using static Julia_Launcher.Camera;
using static Julia_Launcher.Mesh;
using static Julia_Launcher.Equipment;
using static Julia_Launcher.Bone;
using static Julia_Launcher.KeyFrame;
using static Julia_Launcher.Animation;
using static Julia_Launcher.AnimationManager;
using static Julia_Launcher.Animator;
using static Julia_Launcher.Texture;
using static Julia_Launcher.Model;

namespace Julia_Launcher
{
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

        private Vector3 lightPos = new Vector3 (5.0f, 5.0f, 5.0f);
        private Vector3 lightColor = new Vector3(1.0f, 1.0f, 1.0f);
        private float ambientStrength = 0.1f;
        private float specularStrength = 0.5f;
        private float diffuseStrength = 0.8f; // Сила диффузного освещения
        private float shininess = 16.0f;      // Блеск (степень зеркальности)
        private float time = 0.0f;
        private AnimationManager animationManager;


        private void AutoPositionCamera()
        {
            if (model == null) return;

            // Вычисляем центр и размеры модели
            var (min, max) = model.CalculateBoundingBox();
            Vector3 center = (min + max) / 2; // Центр модели
            Vector3 size = max - min;         // Размеры модели

            // Расчёт расстояния камеры на основе поля зрения (FOV)
            float fovYRad = MathHelper.DegreesToRadians(30); // Вертикальный FOV
            float tanFovYHalf = (float)Math.Tan(fovYRad / 2);
            float fovXRad = 2 * (float)Math.Atan(camera.AspectRatio * tanFovYHalf); // Горизонтальный FOV
            float tanFovXHalf = (float)Math.Tan(fovXRad / 2);

            float width = size.X;
            float height = size.Y;
            float dX = (width / 2) / tanFovXHalf;
            float dY = (height / 2) / tanFovYHalf;
            float distance = Math.Max(dX, dY);

            // Уменьшаем расстояние для приближения камеры
            distance *= 0.8f; // 80% от расчётного расстояния

            // Позиция камеры: немного выше центра
            float cameraHeightOffset = size.Y * 0.25f; // 1/4 высоты модели
            camera.Position = center + new Vector3(0, cameraHeightOffset, distance);

            // Новая точка фокуса: выше центра модели
            float focusHeightOffset = size.Y * 0.25f; // Четверть высоты модели (настраиваемо)
            Vector3 focusPoint = center + new Vector3(0, focusHeightOffset, 0);

            // Направляем камеру на точку выше центра
            camera.LookAt(focusPoint);

            // Обновляем вид
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

        private void GlControl_Load(object sender, EventArgs e)
        {
            try
            {
                GL.ClearColor(0.7f, 0.9f, 0.5f, 1.0f);
                GL.Enable(EnableCap.DepthTest);
                GL.Viewport(0, 0, glControl1.Width, glControl1.Height);

                // Получаем путь к директории исполняемого файла
                string appDirectory = Application.StartupPath;

                // Строим путь к папке Shaders в выходной директории
                string shadersDirectory = Path.Combine(appDirectory, "Shaders");
                string vertexPath = Path.Combine(shadersDirectory, "vertex.glsl");
                string fragmentPath = Path.Combine(shadersDirectory, "fragment.glsl");

                // Проверяем существование файлов шейдеров
                if (!File.Exists(vertexPath) || !File.Exists(fragmentPath))
                {
                    MessageBox.Show($"Файлы шейдеров не найдены!\nПуть к vertex.glsl: {vertexPath}\nПуть к fragment.glsl: {fragmentPath}",
                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                shader = new Shader(vertexPath, fragmentPath);
                camera = new Camera(new Vector3(0, 0, 3), glControl1.Width / (float)glControl1.Height);
                camera.LookAt(new Vector3(0, 0, 0));

                // Строим относительный путь к модели
                string modelDirectory = Path.Combine(appDirectory, "..", "..", "..", "Model");
                string modelPath = Path.Combine(modelDirectory, "sketch2.fbx");

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
 
                AutoPositionCamera(); // Всегда вызываем после загрузки модели

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
                // Assume trackBar7 controls model scale
                modelScale = trkSpeechRate.Value / 100.0f;
                glControl1.Invalidate();
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
            // Add these methods to your Shader class

            // Method to set Genshin Impact cel-shading properties
            public void SetCelShadingProperties(
                float[] diffuseThresholds = null,
                float[] diffuseFactors = null,
                float specularThreshold = 0.6f,
                Vector3? rimColor = null,
                float rimPower = 3.0f,
                float rimWidth = 0.3f)
            {
                Use();

                // Set defaults if no values provided
                diffuseThresholds ??= new float[] { 0.8f, 0.6f, 0.3f };
                diffuseFactors ??= new float[] { 1.0f, 0.8f, 0.5f, 0.2f };
                rimColor ??= new Vector3(0.8f, 0.9f, 1.0f);

                // Set uniform values
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

            // Method to set color grading properties
            public void SetColorGradingProperties(
                float saturation = 1.2f,
                float brightness = 1.1f,
                float contrast = 1.15f)
            {
                Use();
                SetFloat("saturation", saturation);
                SetFloat("brightness", brightness);
                SetFloat("contrast", contrast);
            }

            // Method to set outline properties
            public void SetOutlineProperties(
                float thickness = 0.005f,
                Vector3? color = null)
            {
                Use();
                color ??= new Vector3(0.0f, 0.0f, 0.0f); // Default black

                SetFloat("outlineThickness", thickness);
                SetVector3("outlineColor", color.Value);
            }

            // Method to set environment properties
            public void SetEnvironmentProperties(
                Vector3? environmentColor = null,
                float environmentStrength = 0.3f,
                float fogDensity = 0.02f,
                Vector3? fogColor = null)
            {
                Use();
                environmentColor ??= new Vector3(0.1f, 0.1f, 0.2f);
                fogColor ??= new Vector3(0.8f, 0.9f, 1.0f);

                SetVector3("environmentColor", environmentColor.Value);
                SetFloat("environmentStrength", environmentStrength);
                SetFloat("fogDensity", fogDensity);
                SetVector3("fogColor", fogColor.Value);
            }

            // Method to set sketch effect properties
            public void SetSketchProperties(
                float threshold = 0.1f,
                float strength = 0.08f)
            {
                Use();
                SetFloat("sketchThreshold", threshold);
                SetFloat("sketchStrength", strength);
            }

            // Method to update time value for animations
            public void UpdateTime(float time)
            {
                Use();
                SetFloat("time", time);
            }
            // Метод для установки параметров теней
            public void SetShadowProperties(
                float shadowIntensity = 0.7f,
                float shadowSoftness = 0.05f)
            {
                Use();
                SetFloat("shadowIntensity", shadowIntensity);
                SetFloat("shadowSoftness", shadowSoftness);
            }
        }






        private void AnimationTimerTick(object sender, EventArgs e)
        {
            if (model != null && model.HasAnimations && isAnimating)
            {
                float deltaTime = (float)(DateTime.Now - lastFrameTime).TotalSeconds;
                lastFrameTime = DateTime.Now;
                model.Update(deltaTime);
                animationManager.Update(deltaTime);
                glControl1.Invalidate();
            }
        }


        // Класс модели для загрузки и рендеринга 3D-моделей


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



        private void UserControl2_Load(object sender, EventArgs e)
        {
            // Получаем путь к корню проекта
            string exePath = Application.StartupPath;
            string projectDir = Directory.GetParent(Directory.GetParent(exePath).FullName).FullName;
            string shadersDirectory = Path.Combine(projectDir, "Shaders");
            string vertexPath = Path.Combine(shadersDirectory, "vertex.glsl");
            string fragmentPath = Path.Combine(shadersDirectory, "fragment.glsl");

            // Проверка наличия файлов
            if (!File.Exists(vertexPath) || !File.Exists(fragmentPath))
            {
                MessageBox.Show("Файлы шейдеров не найдены!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Здесь продолжение логики, например, загрузка шейдеров
            // Shader shader = new Shader(vertexPath, fragmentPath);
        }


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

        private void btnAddClothing_Click(object sender, EventArgs e)
        {
            var shirt = new Equipment("Shirt", "path/to/shirt.fbx", "Spine", Matrix4.Identity);
            model.AddEquipment(shirt);
            glControl1.Invalidate();
        }
    }

    public static class Matrix4Extensions
    {
        public static Vector3 ExtractTranslation(this Matrix4 matrix)
        {
            return new Vector3(matrix.M41, matrix.M42, matrix.M43);
        }

        public static Quaternion ExtractRotation(this Matrix4 matrix)
        {
            // Удалить масштабирование
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
}