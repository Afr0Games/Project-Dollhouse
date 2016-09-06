using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Files.Vitaboy;
using Files.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Vitaboy
{
    public enum SkinType
    {
        Light = 0,
        Medium = 1,
        Dark = 2
    }

    internal enum MeshType
    {
        Body = 0,
        Head = 1,
        LHand = 2,
        RHand = 3
    }

    /// <summary>
    /// Class for drawing and updating the meshes that make up an avatar.
    /// Also responsible for animating an avatar.
    /// </summary>
    public class AvatarBase
    {
        protected GraphicsDevice m_Devc;
        public Skeleton Skel;
        protected BasicEffect m_BodyEffect, m_HeadEffect, m_LeftHandEffect, RightHandEffect;
        public Mesh HeadMesh, BodyMesh, LeftHandMesh, RightHandMesh;
        public Texture2D LeftHandTexture, RightHandTexture, BodyTexture, HeadTexture;
        public Anim Animation;
        private float m_AnimationTime = 0.0f;

        public float RotationStartAngle = 0.0f;
        public float RotationSpeed = new TimeSpan(0, 0, 10).Ticks;
        public float RotationRange = 40.0f;

        /// <summary>
        /// Should this avatar be rotated when rendered?
        /// Used for rendering in UI.
        /// </summary>
        public bool ShouldRotate = false;

        private bool m_WorldIsDirty = false;
        private Matrix m_World;
        private float m_RotateX = 0.0f, m_RotateY = 0.0f, m_RotateZ = 0.0f;
        private float m_Scale = 1.0f;
        private Vector3 m_Position = new Vector3(0.0f, 0.0f, 0.0f);

        /// <summary>
        /// This avatar's world matrix. Used for rendering.
        /// </summary>
        public Matrix WorldMatrix
        {
            get
            {
                if (m_WorldIsDirty)
                {
                    m_World = Matrix.CreateRotationX(m_RotateX) * Matrix.CreateRotationY(m_RotateY) * Matrix.CreateRotationZ(m_RotateZ) * Matrix.CreateScale(m_Scale) * Matrix.CreateTranslation(m_Position);
                    m_WorldIsDirty = false;
                }

                return m_World;
            }
        }

        /// <summary>
        /// Gets or sets the angle (in degrees) of the rotation axis for this Avatar.
        /// </summary>
        public float RotationY
        {
            get { return m_RotateY; }
            set
            {
                m_RotateY = value;
                m_WorldIsDirty = true;
            }
        }

        /// <summary>
        /// Changes this avatar's outfit.
        /// </summary>
        public void ChangeOutfit(Outfit Oft, SkinType Type)
        {
            Binding[] Bindings;

            if (Oft.HandgroupID.FileID != 0)
            {
                HandGroup Hag = FileManager.GetHandgroup(Oft.HandgroupID.UniqueID);
                Appearance LeftHandApr, RightHandApr;

                switch (Type)
                {
                    case SkinType.Light:
                        LeftHandApr = FileManager.GetAppearance(Hag.Light.Left.Idle.AppearanceID.UniqueID);
                        RightHandApr = FileManager.GetAppearance(Hag.Light.Right.Idle.AppearanceID.UniqueID);
                        break;
                    case SkinType.Medium:
                        LeftHandApr = FileManager.GetAppearance(Hag.Medium.Left.Idle.AppearanceID.UniqueID);
                        RightHandApr = FileManager.GetAppearance(Hag.Medium.Right.Idle.AppearanceID.UniqueID);
                        break;
                    case SkinType.Dark:
                        LeftHandApr = FileManager.GetAppearance(Hag.Medium.Left.Idle.AppearanceID.UniqueID);
                        RightHandApr = FileManager.GetAppearance(Hag.Dark.Right.Idle.AppearanceID.UniqueID);
                        break;
                    default:
                        LeftHandApr = FileManager.GetAppearance(Hag.Light.Left.Idle.AppearanceID.UniqueID);
                        RightHandApr = FileManager.GetAppearance(Hag.Light.Right.Idle.AppearanceID.UniqueID);
                        break;
                }

                Bindings = FileManager.GetBindings(LeftHandApr.BindingIDs);

                foreach (Binding Bnd in Bindings)
                {
                    switch (Bnd.Bone)
                    {
                        case "L_HAND":
                            LeftHandMesh = FileManager.GetMesh(Bnd.MeshID.UniqueID);
                            LeftHandTexture = FileManager.GetTexture(Bnd.TextureID.UniqueID);
                            break;
                        case "R_HAND":
                            RightHandMesh = FileManager.GetMesh(Bnd.MeshID.UniqueID);
                            RightHandTexture = FileManager.GetTexture(Bnd.TextureID.UniqueID);
                            break;
                    }
                }

                Bindings = FileManager.GetBindings(RightHandApr.BindingIDs);

                foreach (Binding Bnd in Bindings)
                {
                    switch (Bnd.Bone)
                    {
                        case "L_HAND":
                            LeftHandMesh = FileManager.GetMesh(Bnd.MeshID.UniqueID);
                            LeftHandTexture = FileManager.GetTexture(Bnd.TextureID.UniqueID);
                            break;
                        case "R_HAND":
                            RightHandMesh = FileManager.GetMesh(Bnd.MeshID.UniqueID);
                            RightHandTexture = FileManager.GetTexture(Bnd.TextureID.UniqueID);
                            break;
                    }
                }
            }

            Appearance Apr;

            switch (Type)
            {
                case SkinType.Light:
                    Apr = FileManager.GetAppearance(Oft.LightAppearance.UniqueID);
                    break;
                case SkinType.Medium:
                    Apr = FileManager.GetAppearance(Oft.MediumAppearance.UniqueID);
                    break;
                case SkinType.Dark:
                    Apr = FileManager.GetAppearance(Oft.DarkAppearance.UniqueID);
                    break;
                default:
                    Apr = FileManager.GetAppearance(Oft.LightAppearance.UniqueID);
                    break;
            }

            Bindings = FileManager.GetBindings(Apr.BindingIDs);

            foreach (Binding Bnd in Bindings)
            {
                switch (Bnd.Bone)
                {
                    case "PELVIS":
                        BodyMesh = FileManager.GetMesh(Bnd.MeshID.UniqueID);
                        BodyTexture = FileManager.GetTexture(Bnd.TextureID.UniqueID);
                        break;
                }
            }
        }

        /// <summary>
        /// Sets this avatar's head appearance.
        /// </summary>
        public Appearance Head
        {
            set
            {
                Binding Bnd = FileManager.GetBinding(value.BindingIDs[0].UniqueID);

                HeadMesh = FileManager.GetMesh(Bnd.MeshID.UniqueID);
                HeadTexture = FileManager.GetTexture(Bnd.TextureID.UniqueID);
            }
        }

        /// <summary>
        /// Creates a new instance of AvatarBase.
        /// </summary>
        /// <param name="Devc">A GraphicsDevice instance.</param>
        /// <param name="Skel">A Skeleton instance.</param>
        public AvatarBase(GraphicsDevice Devc, Skeleton Skel)
        {
            m_Devc = Devc;
            this.Skel = Skel;

            m_HeadEffect = new BasicEffect(Devc);
            m_BodyEffect = new BasicEffect(Devc);
            m_LeftHandEffect = new BasicEffect(Devc);
        }

        /// <summary>
        /// Converts an angle given in degrees to radians.
        /// </summary>
        /// <param name="Angle">The angle to convert.</param>
        /// <returns>The angle in radians.</returns>
        private double DegreesToRadians(double Angle)
        {
            return (Math.PI / 180) * Angle;
        }

        /// <summary>
        /// Updates this avatar's rotation.
        /// </summary>
        /// <param name="GTime">A GameTime instance.</param>
        public void Update(GameTime GTime)
        {
            if (ShouldRotate)
            {
                float Time = GTime.TotalGameTime.Ticks;
                float Phase = (Time % RotationSpeed) / RotationSpeed;
                double Multiplier = Math.Sin((Math.PI * 2) * Phase);
                double NewAngle = RotationStartAngle + (RotationRange * Multiplier);

                RotationY = (float)DegreesToRadians(NewAngle);
            }
        }

        /// <summary>
        /// Renders the different meshes making up this avatar.
        /// </summary>
        /// <param name="ViewMatrix">A view matrix.</param>
        /// <param name="WorldMatrix">A world matrix.</param>
        /// <param name="ProjectionMatrix">A projection matrix.</param>
        public void Render(Matrix ViewMatrix, Matrix WorldMatrix, Matrix ProjectionMatrix)
        {
            //This sets DepthBufferEnable and DepthBufferWriteEnable.
            m_Devc.DepthStencilState = DepthStencilState.Default;
            m_Devc.BlendState = BlendState.AlphaBlend;
            m_Devc.RasterizerState = RasterizerState.CullNone;

            // Configure effects
            m_HeadEffect.World = WorldMatrix;
            m_HeadEffect.View = ViewMatrix;
            m_HeadEffect.Projection = ProjectionMatrix;
            m_HeadEffect.EnableDefaultLighting();

            if (HeadTexture != null)
            {
                m_HeadEffect.Texture = HeadTexture;
                m_HeadEffect.TextureEnabled = true;
            }

            m_BodyEffect.World = WorldMatrix;
            m_BodyEffect.View = ViewMatrix;
            m_BodyEffect.Projection = ProjectionMatrix;
            m_BodyEffect.EnableDefaultLighting();

            if (m_BodyEffect != null)
            {
                m_BodyEffect.Texture = BodyTexture;
                m_BodyEffect.TextureEnabled = true;
            }

            // Configure effects
            m_LeftHandEffect.World = WorldMatrix;
            m_LeftHandEffect.View = ViewMatrix;
            m_LeftHandEffect.Projection = ProjectionMatrix;
            m_LeftHandEffect.EnableDefaultLighting();

            if (LeftHandTexture != null)
            {
                m_LeftHandEffect.Texture = LeftHandTexture;
                m_LeftHandEffect.TextureEnabled = true;
            }

            if (HeadMesh != null)
            {
                foreach (EffectPass Pass in m_HeadEffect.CurrentTechnique.Passes)
                {
                    Pass.Apply();

                    foreach (Vector3 Fce in HeadMesh.Faces)
                    {
                        // Draw
                        VertexPositionNormalTexture[] Vertex = new VertexPositionNormalTexture[3];
                        Vertex[0] = HeadMesh.TransformedVertices[(int)Fce.X];
                        Vertex[1] = HeadMesh.TransformedVertices[(int)Fce.Y];
                        Vertex[2] = HeadMesh.TransformedVertices[(int)Fce.Z];

                        Vertex[0].TextureCoordinate = HeadMesh.TransformedVertices[(int)Fce.X].TextureCoordinate;
                        Vertex[1].TextureCoordinate = HeadMesh.TransformedVertices[(int)Fce.Y].TextureCoordinate;
                        Vertex[2].TextureCoordinate = HeadMesh.TransformedVertices[(int)Fce.Z].TextureCoordinate;

                        Vertex[0].Normal = HeadMesh.TransformedVertices[(int)Fce.X].Normal;
                        Vertex[1].Normal = HeadMesh.TransformedVertices[(int)Fce.Y].Normal;
                        Vertex[2].Normal = HeadMesh.TransformedVertices[(int)Fce.Z].Normal;

                        m_Devc.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, Vertex, 0, 1);
                    }

                    TransformVertices(HeadMesh, null, MeshType.Head);
                }
            }

            if (BodyMesh != null)
            {
                foreach (EffectPass Pass in m_BodyEffect.CurrentTechnique.Passes)
                {
                    Pass.Apply();

                    foreach (Vector3 Fce in BodyMesh.Faces)
                    {
                        // Draw
                        VertexPositionNormalTexture[] Vertex = new VertexPositionNormalTexture[3];
                        Vertex[0] = BodyMesh.TransformedVertices[(int)Fce.X];
                        Vertex[1] = BodyMesh.TransformedVertices[(int)Fce.Y];
                        Vertex[2] = BodyMesh.TransformedVertices[(int)Fce.Z];

                        Vertex[0].TextureCoordinate = BodyMesh.TransformedVertices[(int)Fce.X].TextureCoordinate;
                        Vertex[1].TextureCoordinate = BodyMesh.TransformedVertices[(int)Fce.Y].TextureCoordinate;
                        Vertex[2].TextureCoordinate = BodyMesh.TransformedVertices[(int)Fce.Z].TextureCoordinate;

                        Vertex[0].Normal = BodyMesh.TransformedVertices[(int)Fce.X].Normal;
                        Vertex[1].Normal = BodyMesh.TransformedVertices[(int)Fce.Y].Normal;
                        Vertex[2].Normal = BodyMesh.TransformedVertices[(int)Fce.Z].Normal;

                        m_Devc.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, Vertex, 0, 1);
                    }

                    TransformVertices(BodyMesh, Skel.Bones[0], MeshType.Body);
                }
            }

            if (LeftHandMesh != null)
            {
                foreach (EffectPass Pass in m_LeftHandEffect.CurrentTechnique.Passes)
                {
                    Pass.Apply();

                    foreach (Vector3 Fce in LeftHandMesh.Faces)
                    {
                        // Draw
                        VertexPositionNormalTexture[] Vertex = new VertexPositionNormalTexture[3];
                        Vertex[0] = LeftHandMesh.TransformedVertices[(int)Fce.X];
                        Vertex[1] = LeftHandMesh.TransformedVertices[(int)Fce.Y];
                        Vertex[2] = LeftHandMesh.TransformedVertices[(int)Fce.Z];

                        m_Devc.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, Vertex, 0, 1);
                    }

                    TransformVertices(LeftHandMesh, null, MeshType.LHand);
                }
            }

            if (RightHandMesh != null)
            {
                foreach (EffectPass Pass in m_LeftHandEffect.CurrentTechnique.Passes)
                {
                    Pass.Apply();

                    foreach (Vector3 Fce in RightHandMesh.Faces)
                    {
                        // Draw
                        VertexPositionNormalTexture[] Vertex = new VertexPositionNormalTexture[3];
                        Vertex[0] = RightHandMesh.TransformedVertices[(int)Fce.X];
                        Vertex[1] = RightHandMesh.TransformedVertices[(int)Fce.Y];
                        Vertex[2] = RightHandMesh.TransformedVertices[(int)Fce.Z];

                        m_Devc.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, Vertex, 0, 1);
                    }

                    TransformVertices(RightHandMesh, null, MeshType.RHand);
                }
            }
        }

        /// <summary>
        /// Transforms all vertices in a given mesh to their correct positions.
        /// </summary>
        /// <param name="Msh">The mesh to transform.</param>
        /// <param name="bone">The bone to transform to.</param>
        private void TransformVertices(Mesh Msh, Bone bone, MeshType MshType)
        {
            switch (MshType)
            {
                case MeshType.Head:
                    for (int i = 0; i < Msh.TotalVertexCount; i++)
                    {
                        //Transform the head vertices' position by the absolute transform
                        //for the headbone (which is always bone 17) to render the head in place.
                        Msh.TransformedVertices[i].Position = Vector3.Transform(Msh.RealVertices[i].Position,
                            Skel.Bones[16].AbsoluteMatrix);

                        Msh.TransformedVertices[i].TextureCoordinate = Msh.RealVertices[i].TextureCoordinate;

                        //Transform the head normals' position by the absolute transform
                        //for the headbone (which is always bone 17) to render the head in place.
                        Msh.TransformedVertices[i].Normal = Vector3.Transform(Msh.RealVertices[i].Normal,
                            Skel.Bones[16].AbsoluteMatrix);
                    }

                    return;

                case MeshType.Body:
                    BoneBinding boneBinding = Msh.BoneBindings.FirstOrDefault(x => Msh.Bones[(int)x.BoneIndex] == bone.Name);

                    if (boneBinding != null)
                    {
                        for (int i = 0; i < boneBinding.RealVertexCount; i++)
                        {
                            int vertexIndex = (int)boneBinding.FirstRealVertexIndex + i;
                            VertexPositionNormalTexture relativeVertex = Msh.RealVertices[vertexIndex];

                            Matrix translatedMatrix = Matrix.CreateTranslation(new Vector3(relativeVertex.Position.X, relativeVertex.Position.Y, relativeVertex.Position.Z)) * bone.AbsoluteMatrix;
                            Msh.TransformedVertices[vertexIndex].Position = Vector3.Transform(Vector3.Zero, translatedMatrix);

                            Msh.TransformedVertices[vertexIndex].TextureCoordinate = relativeVertex.TextureCoordinate;

                            //Normals...
                            translatedMatrix = Matrix.CreateTranslation(new Vector3(relativeVertex.Normal.X, relativeVertex.Normal.Y, relativeVertex.Normal.Z)) * bone.AbsoluteMatrix;
                            Msh.TransformedVertices[vertexIndex].Normal = Vector3.Transform(Vector3.Zero, translatedMatrix);
                        }
                    }

                    foreach (var child in bone.Children)
                        TransformVertices(Msh, child, MshType);

                    break;

                case MeshType.LHand:
                    for (int i = 0; i < Msh.TotalVertexCount; i++)
                    {
                        //Transform the left hand vertices' position by the absolute transform
                        //for the left handbone (which is always bone 10) to render the left hand in place.
                        Msh.TransformedVertices[i].Position = Vector3.Transform(Msh.RealVertices[i].Position,
                            Skel.Bones[9].AbsoluteMatrix);

                        //Transform the left hand normals' position by the absolute transform
                        //for the left handbone (which is always bone 10) to render the left hand in place.
                        Msh.TransformedVertices[i].Normal = Vector3.Transform(Msh.RealVertices[i].Normal,
                            Skel.Bones[9].AbsoluteMatrix);
                    }

                    return;

                case MeshType.RHand:
                    for (int i = 0; i < Msh.TotalVertexCount; i++)
                    {
                        //Transform the right hand vertices' position by the absolute transform
                        //for the right handbone (which is always bone 15) to render the right hand in place.
                        Msh.TransformedVertices[i].Position = Vector3.Transform(Msh.RealVertices[i].Position,
                            Skel.Bones[14].AbsoluteMatrix);

                        //Transform the right hand normals' position by the absolute transform
                        //for the right handbone (which is always bone 15) to render the right hand in place.
                        Msh.TransformedVertices[i].Normal = Vector3.Transform(Msh.RealVertices[i].Normal,
                            Skel.Bones[14].AbsoluteMatrix);
                    }

                    return;
            }
        }

        /// <summary>
        /// Advances the frame of an animation for a skeleton used on this mesh.
        /// </summary>
        /// <param name="Animation">The animation to advance.</param>
        /// <param name="TimeDelta">The timedelta of the rendering loop.</param>
        public void AdvanceFrame(Anim Animation, float TimeDelta)
        {
            float Duration = (float)Animation.Motions[0].FrameCount / 30;
            m_AnimationTime += TimeDelta;
            m_AnimationTime = m_AnimationTime % Duration; //Loop the animation

            for (int i = 0; i < Animation.Motions.Count; i++)
            {
                int BoneIndex = Skel.FindBone(Animation.Motions[i].BoneName);

                if (BoneIndex == -1)
                    continue;

                int Frame = (int)(m_AnimationTime * 30);
                float FractionShown = m_AnimationTime * 30 - Frame;
                int NextFrame = (Frame + 1 != Animation.Motions[0].FrameCount) ? Frame + 1 : 0;

                if (Animation.Motions[i].HasTranslation)
                {
                    Vector3 Translation = new Vector3(
                        Animation.Translations[Frame + Animation.Motions[i].FirstTranslationIndex, 0],
                        Animation.Translations[Frame + Animation.Motions[i].FirstTranslationIndex, 1],
                        Animation.Translations[Frame + Animation.Motions[i].FirstTranslationIndex, 2]);
                    Vector3 NextTranslation = new Vector3(
                        Animation.Translations[NextFrame + Animation.Motions[i].FirstTranslationIndex, 0],
                        Animation.Translations[NextFrame + Animation.Motions[i].FirstTranslationIndex, 1],
                        Animation.Translations[NextFrame + Animation.Motions[i].FirstTranslationIndex, 2]);

                    Vector3 UpdatedTranslation = new Vector3();
                    UpdatedTranslation.X = (1 - FractionShown) * Translation.X + FractionShown * NextTranslation.X;
                    UpdatedTranslation.Y = (1 - FractionShown) * Translation.Y + FractionShown * NextTranslation.Y;
                    UpdatedTranslation.Z = (1 - FractionShown) * Translation.Z + FractionShown * NextTranslation.Z;

                    Skel.Bones[BoneIndex].Translation = UpdatedTranslation;
                }

                if (Animation.Motions[i].HasRotation)
                {
                    Quaternion Rotation = new Quaternion(
                        Animation.Rotations[Frame + Animation.Motions[i].FirstRotationIndex, 0],
                        Animation.Rotations[Frame + Animation.Motions[i].FirstRotationIndex, 1],
                        Animation.Rotations[Frame + Animation.Motions[i].FirstRotationIndex, 2],
                        Animation.Rotations[Frame + Animation.Motions[i].FirstRotationIndex, 3]);
                    Quaternion NextRotation = new Quaternion(
                        Animation.Rotations[NextFrame + Animation.Motions[i].FirstRotationIndex, 0],
                        Animation.Rotations[NextFrame + Animation.Motions[i].FirstRotationIndex, 1],
                        Animation.Rotations[NextFrame + Animation.Motions[i].FirstRotationIndex, 2],
                        Animation.Rotations[NextFrame + Animation.Motions[i].FirstRotationIndex, 3]);

                    //Use Slerp to interpolate
                    float W1, W2 = 1.0f;
                    float CosTheta = Helpers.DotProduct(Rotation, NextRotation);

                    if (CosTheta < 0)
                    {
                        CosTheta *= -1;
                        W2 *= -1;
                    }

                    float Theta = (float)Math.Acos(CosTheta);
                    float SinTheta = (float)Math.Sin(Theta);

                    if (SinTheta > 0.001f)
                    {
                        W1 = (float)Math.Sin((1.0f - FractionShown) * Theta) / SinTheta;
                        W2 *= (float)Math.Sin(FractionShown * Theta) / SinTheta;
                    }
                    else
                    {
                        W1 = 1.0f - FractionShown;
                        W2 = FractionShown;
                    }

                    Quaternion UpdatedRotation = new Quaternion();
                    UpdatedRotation.X = W1 * Rotation.X + W2 * NextRotation.X;
                    UpdatedRotation.Y = W1 * Rotation.Y + W2 * NextRotation.Y;
                    UpdatedRotation.Z = W1 * Rotation.Z + W2 * NextRotation.Z;
                    UpdatedRotation.W = W1 * Rotation.W + W2 * NextRotation.W;

                    Skel.Bones[BoneIndex].Rotation.X = UpdatedRotation.X;
                    Skel.Bones[BoneIndex].Rotation.Y = UpdatedRotation.Y;
                    Skel.Bones[BoneIndex].Rotation.Z = UpdatedRotation.Z;
                    Skel.Bones[BoneIndex].Rotation.W = UpdatedRotation.W;
                }
            }
        }

        /// <summary>
        /// Computes the absolute positions for all bones in a Skeleton.
        /// </summary>
        /// <param name="bone">The first bone in the skeleton.</param>
        /// <param name="world">World matrix.</param>
        public void ComputeBonePositions(Bone bone, Matrix world)
        {
            var translateMatrix = Matrix.CreateTranslation(bone.Translation);
            var rotationMatrix = Helpers.FindQuaternionMatrix(bone.Rotation);

            var myWorld = (rotationMatrix * translateMatrix) * world;
            bone.AbsolutePosition = Vector3.Transform(Vector3.Zero, myWorld);
            bone.AbsoluteMatrix = myWorld;

            foreach (var child in bone.Children)
            {
                ComputeBonePositions(child, myWorld);
            }
        }
    }
}
