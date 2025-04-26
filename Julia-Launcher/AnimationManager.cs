using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Julia_Launcher.UserControl2;

namespace Julia_Launcher
{
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

        public void LookAtCamera(Vector3 cameraPos, string headBoneName)
        {
            if (currentAnimation == null || !currentAnimation.Bones.TryGetValue(headBoneName, out Bone headBone)) return;

            Vector3 headPos = Vector3.TransformPosition(Vector3.Zero, headBone.FinalTransformation);
            Vector3 direction = Vector3.Normalize(cameraPos - headPos);
            Quaternion rotation = Quaternion.FromAxisAngle(Vector3.UnitY, (float)Math.Atan2(direction.X, direction.Z));
            headBone.FinalTransformation = Matrix4.CreateFromQuaternion(rotation) * headBone.InterpolateTransform(currentTime);
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

    // Класс для управления анимациями
    public class AnimationManager
    {
        private readonly Model model;
        private readonly Queue<string> animationQueue = new Queue<string>();
        private float switchInterval = 5.0f; // Секунды между переключениями
        private float elapsedTime = 0.0f;

        public AnimationManager(Model model)
        {
            this.model = model;
        }

        public void EnqueueAnimation(string animationName) => animationQueue.Enqueue(animationName);
        public void SetSwitchInterval(float interval) => switchInterval = interval;

        public void Update(float deltaTime)
        {
            if (!model.HasAnimations || animationQueue.Count == 0) return;

            elapsedTime += deltaTime;
            if (elapsedTime >= switchInterval || !model.Animator.IsPlaying)
            {
                elapsedTime = 0.0f;
                string nextAnim = animationQueue.Dequeue();
                model.PlayAnimation(nextAnim);
                animationQueue.Enqueue(nextAnim); // Цикл анимации
            }
        }
    }
}
