using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using static Julia_Launcher.UserControl2;

namespace Julia_Launcher
{
    //Класс оборудования
    public class Equipment
    {
        public string Name { get; }
        public Model Model { get; }
        public string AttachmentBone { get; }
        public Matrix4 Offset { get; }

        public Equipment(string name, string modelPath, string bone, Matrix4 offset)
        {
            Name = name;
            Model = new Model(modelPath);
            AttachmentBone = bone;
            Offset = offset;
        }
    }
    public class CharacterComponent
    {
        public string Name { get; }
        public Mesh Mesh { get; set; }
        public Dictionary<string, float> MorphTargets { get; } = new Dictionary<string, float>(); // e.g., "EyeSize": 0.5

        public CharacterComponent(string name, Mesh mesh)
        {
            Name = name;
            Mesh = mesh;
        }

        public void ApplyMorph(string target, float weight)
        {
            MorphTargets[target] = Math.Clamp(weight, 0.0f, 1.0f);
        }
    }
}
