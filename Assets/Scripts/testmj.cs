using System.Xml;

namespace Mujoco
{
    public class testmj: MjComponent
    {
        public override MujocoLib.mjtObj ObjectType { get; }
        protected override void OnParseMjcf(XmlElement mjcf)
        {
            
        }

        protected override XmlElement OnGenerateMjcf(XmlDocument doc)
        {
            return null;
        }
    }
}