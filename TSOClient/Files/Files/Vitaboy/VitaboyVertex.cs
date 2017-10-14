using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Files.Vitaboy
{
    public struct VitaboyVertex : IVertexType
    {
        public Vector3 Position;
        public Vector2 TextureCoordinate;
        public Vector3 Normal;
        public float BoneBinding; //The index of the bone associated with this vertex.

        public VitaboyVertex(Vector3 position, Vector3 normal, Vector2 textureCoordinate, float boneBinding)
        {
            Position = position;
            TextureCoordinate = textureCoordinate;
            Normal = normal;
            BoneBinding = boneBinding;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            //A Vector3 is sizeof(float) * 3
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 5, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(sizeof(float) * 8, VertexElementFormat.Single, VertexElementUsage.BlendWeight, 0)
        );

        /// <summary>
        /// Returns the vertex declaration for this VitaboyVertex.
        /// </summary>
        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }

        public static bool operator ==(VitaboyVertex left, VitaboyVertex right)
        {
            if (left.BoneBinding != right.BoneBinding)
                return false;

            return (((left.Position == right.Position) && (left.Normal == right.Normal)) && (left.TextureCoordinate == right.TextureCoordinate));
        }

        /// <summary>
        /// Is this VitaboyVertex equal to another object?
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>Returns true if equal, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() != base.GetType())
            {
                return false;
            }
            return (this == ((VitaboyVertex)obj));
        }

        public override string ToString()
        {
            return string.Format("{{Position:{0} Normal:{1} TextureCoordinate:{2} BoneBindings:{3} }}", new object[] { this.Position, this.Normal, this.TextureCoordinate, this.BoneBinding });
        }

        public static bool operator !=(VitaboyVertex left, VitaboyVertex right)
        {
            if (left.BoneBinding != right.BoneBinding)
                return true;

            return !(left == right);
        }
    }
}
