﻿/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the Vitaboy library.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Files.Vitaboy;
using Files.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using log4net;

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
    public class AvatarBase : IDisposable
    {
        protected GraphicsDevice m_Devc;
        public Skeleton Skel;
        protected Effect m_VitaboyShader;
        protected BasicEffect m_BodyEffect, m_HeadEffect, m_AccessoryEffect, m_LeftHandEffect, m_RightHandEffect;
        public Mesh HeadMesh, AccessoryMesh, BodyMesh, LeftHandMesh, RightHandMesh;
        public Texture2D LeftHandTexture, RightHandTexture, BodyTexture, HeadTexture, AccessoryTexture;
        public Anim Animation;
        private float m_AnimationTime = 0.0f;

        private bool m_GPURender = false;

        public float RotationStartAngle = 0.0f;
        public float RotationSpeed = new TimeSpan(0, 0, 10).Ticks;
        public float RotationRange = 40.0f;

        private static readonly ILog m_Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

            AccessoryMesh = null;
            AccessoryTexture = null;

            if (Oft.HandgroupID.FileID != 0)
            {
                HandGroup Hag = FileManager.Instance.GetHandgroup(Oft.HandgroupID.UniqueID);
                Appearance LeftHandApr, RightHandApr;

                switch (Type)
                {
                    case SkinType.Light:
                        LeftHandApr = FileManager.Instance.GetAppearance(Hag.Light.Left.Idle.AppearanceID.UniqueID);
                        RightHandApr = FileManager.Instance.GetAppearance(Hag.Light.Right.Idle.AppearanceID.UniqueID);
                        break;
                    case SkinType.Medium:
                        LeftHandApr = FileManager.Instance.GetAppearance(Hag.Medium.Left.Idle.AppearanceID.UniqueID);
                        RightHandApr = FileManager.Instance.GetAppearance(Hag.Medium.Right.Idle.AppearanceID.UniqueID);
                        break;
                    case SkinType.Dark:
                        LeftHandApr = FileManager.Instance.GetAppearance(Hag.Dark.Left.Idle.AppearanceID.UniqueID);
                        RightHandApr = FileManager.Instance.GetAppearance(Hag.Dark.Right.Idle.AppearanceID.UniqueID);
                        break;
                    default:
                        LeftHandApr = FileManager.Instance.GetAppearance(Hag.Light.Left.Idle.AppearanceID.UniqueID);
                        RightHandApr = FileManager.Instance.GetAppearance(Hag.Light.Right.Idle.AppearanceID.UniqueID);
                        break;
                }

                Bindings = FileManager.Instance.GetBindings(LeftHandApr.BindingIDs);

                foreach (Binding Bnd in Bindings)
                {
                    switch (Bnd.Bone)
                    {
                        case "L_HAND":
                            LeftHandMesh = FileManager.Instance.GetMesh(Bnd.MeshID.UniqueID);
                            LeftHandTexture = FileManager.Instance.GetTexture(Bnd.TextureID.UniqueID);
                            break;
                    }
                }

                Bindings = FileManager.Instance.GetBindings(RightHandApr.BindingIDs);

                foreach (Binding Bnd in Bindings)
                {
                    switch (Bnd.Bone)
                    {
                        case "R_HAND":
                            RightHandMesh = FileManager.Instance.GetMesh(Bnd.MeshID.UniqueID);
                            RightHandTexture = FileManager.Instance.GetTexture(Bnd.TextureID.UniqueID);
                            break;
                    }
                }
            }

            Appearance Apr;

            switch (Type)
            {
                case SkinType.Light:
                    Apr = FileManager.Instance.GetAppearance(Oft.LightAppearance.UniqueID);
                    break;
                case SkinType.Medium:
                    Apr = FileManager.Instance.GetAppearance(Oft.MediumAppearance.UniqueID);
                    break;
                case SkinType.Dark:
                    Apr = FileManager.Instance.GetAppearance(Oft.DarkAppearance.UniqueID);
                    break;
                default:
                    Apr = FileManager.Instance.GetAppearance(Oft.LightAppearance.UniqueID);
                    break;
            }

            Bindings = FileManager.Instance.GetBindings(Apr.BindingIDs);

            if(Oft.Region == OutfitRegion.Head)
                HeadMesh = null; //IMPORTANT: Reset the head mesh before loading a new one.

            foreach (Binding Bnd in Bindings)
            {
                switch (Bnd.Bone)
                {
                    case "PELVIS":
                        BodyMesh = FileManager.Instance.GetMesh(Bnd.MeshID.UniqueID);
                        BodyTexture = FileManager.Instance.GetTexture(Bnd.TextureID.UniqueID);
                        break;
                    case "HEAD":
                        if (HeadMesh == null)
                        {
                            HeadMesh = FileManager.Instance.GetMesh(Bnd.MeshID.UniqueID);
                            HeadTexture = FileManager.Instance.GetTexture(Bnd.TextureID.UniqueID);
                        }
                        else
                        {
                            AccessoryMesh = FileManager.Instance.GetMesh(Bnd.MeshID.UniqueID);
                            AccessoryTexture = FileManager.Instance.GetTexture(Bnd.TextureID.UniqueID);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Sets this avatar's head outfit.
        /// </summary>
        public void SetHead(Outfit HeadOutfit, SkinType Type)
        {
            Appearance Apr;

            switch (Type)
            {
                case SkinType.Light:
                    Apr = FileManager.Instance.GetAppearance(HeadOutfit.LightAppearance.UniqueID);
                    break;
                case SkinType.Medium:
                    Apr = FileManager.Instance.GetAppearance(HeadOutfit.MediumAppearance.UniqueID);
                    break;
                case SkinType.Dark:
                    Apr = FileManager.Instance.GetAppearance(HeadOutfit.DarkAppearance.UniqueID);
                    break;
                default:
                    Apr = FileManager.Instance.GetAppearance(HeadOutfit.LightAppearance.UniqueID);
                    break;
            }

            AccessoryMesh = null;
            AccessoryTexture = null;

            //I think that heads are always 0...
            Binding Bnd = FileManager.Instance.GetBinding(Apr.BindingIDs[0].UniqueID);

            HeadMesh = FileManager.Instance.GetMesh(Bnd.MeshID.UniqueID);
            HeadTexture = FileManager.Instance.GetTexture(Bnd.TextureID.UniqueID);

            if(Apr.BindingIDs.Count > 1) //This head has accessories.
                Bnd = FileManager.Instance.GetBinding(Apr.BindingIDs[1].UniqueID);
        }

        /// <summary>
        /// Creates a new instance of AvatarBase.
        /// </summary>
        /// <param name="Devc">A GraphicsDevice instance.</param>
        /// <param name="Skel">A Skeleton instance.</param>
        /// <param name="Shader">A shader (optional) used to render the avatar on the GPU.</param>
        public AvatarBase(GraphicsDevice Devc, Skeleton Skel, Effect Shader = null)
        {
            m_Devc = Devc;
            this.Skel = Skel;

            m_HeadEffect = new BasicEffect(Devc);
            m_AccessoryEffect = new BasicEffect(Devc);
            m_BodyEffect = new BasicEffect(Devc);
            m_LeftHandEffect = new BasicEffect(Devc);
            m_RightHandEffect = new BasicEffect(Devc);

            if (Shader != null)
            {
                m_GPURender = true;
                m_VitaboyShader = Shader;
            }
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
            ComputeBonePositions(Skel.RootBone, Matrix.Identity);

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
            ComputeBonePositions(Skel.RootBone, Matrix.Identity);

            //Rotate the root bone by 180 degrees around the X-axis
            Matrix correction = Matrix.CreateRotationX(MathHelper.ToRadians(180));

            //Apply this to the root bone's matrix in ComputeBonePositions
            ComputeBonePositions(Skel.RootBone, correction);

            //Rotate the root bone 180 degrees around the Y-axis
            correction = Matrix.CreateRotationZ(MathHelper.ToRadians(180));
            ComputeBonePositions(Skel.RootBone, correction);


            //This sets DepthBufferEnable and DepthBufferWriteEnable.
            m_Devc.DepthStencilState = DepthStencilState.Default;
            m_Devc.BlendState = BlendState.AlphaBlend;
            m_Devc.RasterizerState = RasterizerState.CullNone;

            if (m_GPURender == false)
            {
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

                m_AccessoryEffect.World = WorldMatrix;
                m_AccessoryEffect.View = ViewMatrix;
                m_AccessoryEffect.Projection = ProjectionMatrix;
                m_AccessoryEffect.EnableDefaultLighting();

                if (AccessoryTexture != null)
                {
                    m_AccessoryEffect.Texture = AccessoryTexture;
                    m_AccessoryEffect.TextureEnabled = true;
                }

                m_LeftHandEffect.World = WorldMatrix;
                m_LeftHandEffect.View = ViewMatrix;
                m_LeftHandEffect.Projection = ProjectionMatrix;
                m_LeftHandEffect.EnableDefaultLighting();

                if (LeftHandTexture != null)
                {
                    m_LeftHandEffect.Texture = LeftHandTexture;
                    m_LeftHandEffect.TextureEnabled = true;
                }

                m_RightHandEffect.World = WorldMatrix;
                m_RightHandEffect.View = ViewMatrix;
                m_RightHandEffect.Projection = ProjectionMatrix;
                m_RightHandEffect.EnableDefaultLighting();

                if (RightHandTexture != null)
                {
                    m_RightHandEffect.Texture = RightHandTexture;
                    m_RightHandEffect.TextureEnabled = true;
                }

                m_BodyEffect.World = WorldMatrix;
                m_BodyEffect.View = ViewMatrix;
                m_BodyEffect.Projection = ProjectionMatrix;
                m_BodyEffect.EnableDefaultLighting();

                if (BodyTexture != null)
                {
                    m_BodyEffect.Texture = BodyTexture;
                    m_BodyEffect.TextureEnabled = true;
                }
            }
            else
            {
                m_VitaboyShader.Parameters["View"].SetValue(ViewMatrix);
                m_VitaboyShader.Parameters["Projection"].SetValue(ProjectionMatrix);
            }

            if (HeadMesh != null)
            {
                if (m_GPURender == false)
                {
                    foreach (EffectPass Pass in m_HeadEffect.CurrentTechnique.Passes)
                    {
                        TransformVertices(HeadMesh, null, MeshType.Head);

                        Pass.Apply();

                        foreach (Vector3 Fce in HeadMesh.Faces)
                        {
                            // Draw
                            VertexPositionNormalTexture[] Vertex = new VertexPositionNormalTexture[3];
                            Vertex[0].Position = HeadMesh.TransformedVertices[(int)Fce.X].Position;
                            Vertex[1].Position = HeadMesh.TransformedVertices[(int)Fce.Y].Position;
                            Vertex[2].Position = HeadMesh.TransformedVertices[(int)Fce.Z].Position;

                            Vertex[0].TextureCoordinate = HeadMesh.TransformedVertices[(int)Fce.X].TextureCoordinate;
                            Vertex[1].TextureCoordinate = HeadMesh.TransformedVertices[(int)Fce.Y].TextureCoordinate;
                            Vertex[2].TextureCoordinate = HeadMesh.TransformedVertices[(int)Fce.Z].TextureCoordinate;

                            Vertex[0].Normal = HeadMesh.TransformedVertices[(int)Fce.X].Normal;
                            Vertex[1].Normal = HeadMesh.TransformedVertices[(int)Fce.Y].Normal;
                            Vertex[2].Normal = HeadMesh.TransformedVertices[(int)Fce.Z].Normal;

                            m_Devc.DrawUserPrimitives(PrimitiveType.TriangleList, Vertex, 0, 1);
                        }
                    }

                }
                else
                {
                    m_VitaboyShader.Parameters["VitaboyTexture"].SetValue(HeadTexture);
                    m_VitaboyShader.Parameters["World"].SetValue(WorldMatrix);
                    m_VitaboyShader.Parameters["ChildBones"].SetValue(GetAllBones(HeadMesh, Skel));

                    //foreach (EffectPass Pass in m_VitaboyShader.Techniques["TransformHeadTechnique"].Passes)
                    foreach (EffectPass Pass in m_VitaboyShader.Techniques["TransformVerticesTechnique"].Passes)
                    {
                        Pass.Apply();

                        foreach (Vector3 Fce in HeadMesh.Faces)
                        {
                            VitaboyVertex[] Vertex = new VitaboyVertex[3];
                            Vertex[0].Position = HeadMesh.RealVertices[(int)Fce.X].Position;
                            Vertex[1].Position = HeadMesh.RealVertices[(int)Fce.Y].Position;
                            Vertex[2].Position = HeadMesh.RealVertices[(int)Fce.Z].Position;

                            Vertex[0].TextureCoordinate = HeadMesh.RealVertices[(int)Fce.X].TextureCoordinate;
                            Vertex[1].TextureCoordinate = HeadMesh.RealVertices[(int)Fce.Y].TextureCoordinate;
                            Vertex[2].TextureCoordinate = HeadMesh.RealVertices[(int)Fce.Z].TextureCoordinate;

                            Vertex[0].Normal = HeadMesh.RealVertices[(int)Fce.X].Normal;
                            Vertex[1].Normal = HeadMesh.RealVertices[(int)Fce.Y].Normal;
                            Vertex[2].Normal = HeadMesh.RealVertices[(int)Fce.Z].Normal;

                            //All the meshes except the body mesh only references one bone.
                            Vertex[0].BoneBinding = 0;
                            Vertex[1].BoneBinding = 0;
                            Vertex[2].BoneBinding = 0;

                            m_Devc.DrawUserPrimitives(PrimitiveType.TriangleList, Vertex, 0, 1);
                        }
                    }
                }
            }

            if (AccessoryMesh != null)
            {
                if (m_GPURender == false)
                {
                    foreach (EffectPass Pass in m_AccessoryEffect.CurrentTechnique.Passes)
                    {
                        TransformVertices(AccessoryMesh, null, MeshType.Head);

                        Pass.Apply();

                        foreach (Vector3 Fce in AccessoryMesh.Faces)
                        {
                            // Draw
                            VertexPositionNormalTexture[] Vertex = new VertexPositionNormalTexture[3];
                            Vertex[0].Position = AccessoryMesh.TransformedVertices[(int)Fce.X].Position;
                            Vertex[1].Position = AccessoryMesh.TransformedVertices[(int)Fce.Y].Position;
                            Vertex[2].Position = AccessoryMesh.TransformedVertices[(int)Fce.Z].Position;

                            Vertex[0].TextureCoordinate = AccessoryMesh.TransformedVertices[(int)Fce.X].TextureCoordinate;
                            Vertex[1].TextureCoordinate = AccessoryMesh.TransformedVertices[(int)Fce.Y].TextureCoordinate;
                            Vertex[2].TextureCoordinate = AccessoryMesh.TransformedVertices[(int)Fce.Z].TextureCoordinate;

                            Vertex[0].Normal = AccessoryMesh.TransformedVertices[(int)Fce.X].Normal;
                            Vertex[1].Normal = AccessoryMesh.TransformedVertices[(int)Fce.Y].Normal;
                            Vertex[2].Normal = AccessoryMesh.TransformedVertices[(int)Fce.Z].Normal;

                            m_Devc.DrawUserPrimitives(PrimitiveType.TriangleList, Vertex, 0, 1);
                        }
                    }
                }
                else
                {
                    m_VitaboyShader.Parameters["VitaboyTexture"].SetValue(AccessoryTexture);
                    m_VitaboyShader.Parameters["World"].SetValue(WorldMatrix);
                    m_VitaboyShader.Parameters["ChildBones"].SetValue(GetAllBones(HeadMesh, Skel));

                    foreach (EffectPass Pass in m_VitaboyShader.Techniques["TransformVerticesTechnique"].Passes)
                    {
                        Pass.Apply();

                        foreach (Vector3 Fce in AccessoryMesh.Faces)
                        {
                            VitaboyVertex[] Vertex = new VitaboyVertex[3];
                            Vertex[0].Position = AccessoryMesh.RealVertices[(int)Fce.X].Position;
                            Vertex[1].Position = AccessoryMesh.RealVertices[(int)Fce.Y].Position;
                            Vertex[2].Position = AccessoryMesh.RealVertices[(int)Fce.Z].Position;

                            Vertex[0].TextureCoordinate = AccessoryMesh.RealVertices[(int)Fce.X].TextureCoordinate;
                            Vertex[1].TextureCoordinate = AccessoryMesh.RealVertices[(int)Fce.Y].TextureCoordinate;
                            Vertex[2].TextureCoordinate = AccessoryMesh.RealVertices[(int)Fce.Z].TextureCoordinate;

                            Vertex[0].Normal = AccessoryMesh.RealVertices[(int)Fce.X].Normal;
                            Vertex[1].Normal = AccessoryMesh.RealVertices[(int)Fce.Y].Normal;
                            Vertex[2].Normal = AccessoryMesh.RealVertices[(int)Fce.Z].Normal;

                            Vertex[0].BoneBinding = 0;
                            Vertex[1].BoneBinding = 0;
                            Vertex[2].BoneBinding = 0;

                            m_Devc.DrawUserPrimitives(PrimitiveType.TriangleList, Vertex, 0, 1);
                        }
                    }
                }
            }

            if (BodyMesh != null)
            {
                if (m_GPURender == false)
                {
                    foreach (EffectPass Pass in m_BodyEffect.CurrentTechnique.Passes)
                    {
                        TransformVertices(BodyMesh, Skel.Bones[Skel.FindBone("PELVIS")], MeshType.Body);

                        Pass.Apply();

                        foreach (Vector3 Fce in BodyMesh.Faces)
                        {
                            // Draw
                            VertexPositionNormalTexture[] Vertex = new VertexPositionNormalTexture[3];
                            Vertex[0].Position = BodyMesh.TransformedVertices[(int)Fce.X].Position;
                            Vertex[1].Position = BodyMesh.TransformedVertices[(int)Fce.Y].Position;
                            Vertex[2].Position = BodyMesh.TransformedVertices[(int)Fce.Z].Position;

                            Vertex[0].TextureCoordinate = BodyMesh.TransformedVertices[(int)Fce.X].TextureCoordinate;
                            Vertex[1].TextureCoordinate = BodyMesh.TransformedVertices[(int)Fce.Y].TextureCoordinate;
                            Vertex[2].TextureCoordinate = BodyMesh.TransformedVertices[(int)Fce.Z].TextureCoordinate;

                            Vertex[0].Normal = BodyMesh.TransformedVertices[(int)Fce.X].Normal;
                            Vertex[1].Normal = BodyMesh.TransformedVertices[(int)Fce.Y].Normal;
                            Vertex[2].Normal = BodyMesh.TransformedVertices[(int)Fce.Z].Normal;

                            m_Devc.DrawUserPrimitives(PrimitiveType.TriangleList, Vertex, 0, 1);
                        }
                    }
                }
                else
                {
                    CopyBodyVertices(Skel.Bones[Skel.FindBone("PELVIS")]);

                    m_VitaboyShader.Parameters["VitaboyTexture"].SetValue(BodyTexture);
                    m_VitaboyShader.Parameters["World"].SetValue(WorldMatrix);
                    m_VitaboyShader.Parameters["ChildBones"].SetValue(GetAllBones(BodyMesh, Skel));

                    foreach (EffectPass Pass in m_VitaboyShader.Techniques["TransformVerticesTechnique"].Passes)
                    {
                        foreach (Vector3 Fce in BodyMesh.Faces)
                        {
                            Pass.Apply();

                            // Draw
                            VitaboyVertex[] Vertex = new VitaboyVertex[3];

                            Vertex[0].Position = BodyMesh.TransformedVertices[(int)Fce.X].Position;
                            Vertex[1].Position = BodyMesh.TransformedVertices[(int)Fce.Y].Position;
                            Vertex[2].Position = BodyMesh.TransformedVertices[(int)Fce.Z].Position;

                            Vertex[0].TextureCoordinate = BodyMesh.TransformedVertices[(int)Fce.X].TextureCoordinate;
                            Vertex[1].TextureCoordinate = BodyMesh.TransformedVertices[(int)Fce.Y].TextureCoordinate;
                            Vertex[2].TextureCoordinate = BodyMesh.TransformedVertices[(int)Fce.Z].TextureCoordinate;

                            Vertex[0].Normal = BodyMesh.TransformedVertices[(int)Fce.X].Normal;
                            Vertex[1].Normal = BodyMesh.TransformedVertices[(int)Fce.Y].Normal;
                            Vertex[2].Normal = BodyMesh.TransformedVertices[(int)Fce.Z].Normal;

                            Vertex[0].BoneBinding = BodyMesh.TransformedVertices[(int)Fce.X].BoneBinding;
                            Vertex[1].BoneBinding = BodyMesh.TransformedVertices[(int)Fce.Y].BoneBinding;
                            Vertex[2].BoneBinding = BodyMesh.TransformedVertices[(int)Fce.Z].BoneBinding;

                            m_Devc.DrawUserPrimitives(PrimitiveType.TriangleList, Vertex, 0, 1);
                        }
                    }
                }
            }

            if (LeftHandMesh != null)
            {
                if (m_GPURender == false)
                {
                    foreach (EffectPass Pass in m_LeftHandEffect.CurrentTechnique.Passes)
                    {
                        TransformVertices(LeftHandMesh, null, MeshType.LHand);

                        Pass.Apply();

                        foreach (Vector3 Fce in LeftHandMesh.Faces)
                        {
                            // Draw
                            VertexPositionNormalTexture[] Vertex = new VertexPositionNormalTexture[3];
                            Vertex[0].Position = LeftHandMesh.TransformedVertices[(int)Fce.X].Position;
                            Vertex[1].Position = LeftHandMesh.TransformedVertices[(int)Fce.Y].Position;
                            Vertex[2].Position = LeftHandMesh.TransformedVertices[(int)Fce.Z].Position;

                            m_Devc.DrawUserPrimitives(PrimitiveType.TriangleList, Vertex, 0, 1);
                        }
                    }
                }
                else
                {
                    m_VitaboyShader.Parameters["VitaboyTexture"].SetValue(LeftHandTexture);
                    m_VitaboyShader.Parameters["World"].SetValue(WorldMatrix);
                    m_VitaboyShader.Parameters["ChildBones"].SetValue(GetAllBones(LeftHandMesh, Skel));

                    foreach (EffectPass Pass in m_VitaboyShader.Techniques["TransformVerticesTechnique"].Passes)
                    {
                        Pass.Apply();

                        foreach (Vector3 Fce in LeftHandMesh.Faces)
                        {
                            VitaboyVertex[] Vertex = new VitaboyVertex[3];
                            Vertex[0].Position = LeftHandMesh.RealVertices[(int)Fce.X].Position;
                            Vertex[1].Position = LeftHandMesh.RealVertices[(int)Fce.Y].Position;
                            Vertex[2].Position = LeftHandMesh.RealVertices[(int)Fce.Z].Position;

                            Vertex[0].TextureCoordinate = LeftHandMesh.RealVertices[(int)Fce.X].TextureCoordinate;
                            Vertex[1].TextureCoordinate = LeftHandMesh.RealVertices[(int)Fce.Y].TextureCoordinate;
                            Vertex[2].TextureCoordinate = LeftHandMesh.RealVertices[(int)Fce.Z].TextureCoordinate;

                            Vertex[0].Normal = LeftHandMesh.RealVertices[(int)Fce.X].Normal;
                            Vertex[1].Normal = LeftHandMesh.RealVertices[(int)Fce.Y].Normal;
                            Vertex[2].Normal = LeftHandMesh.RealVertices[(int)Fce.Z].Normal;

                            //All the meshes except the body mesh only references one bone.
                            Vertex[0].BoneBinding = 0;
                            Vertex[1].BoneBinding = 0;
                            Vertex[2].BoneBinding = 0;

                            m_Devc.DrawUserPrimitives(PrimitiveType.TriangleList, Vertex, 0, 1);
                        }
                    }
                }
            }

            if (RightHandMesh != null)
            {
                if (m_GPURender == false)
                {
                    foreach (EffectPass Pass in m_RightHandEffect.CurrentTechnique.Passes)
                    {
                        Pass.Apply();

                        foreach (Vector3 Fce in RightHandMesh.Faces)
                        {
                            // Draw
                            VertexPositionNormalTexture[] Vertex = new VertexPositionNormalTexture[3];
                            Vertex[0].Position = RightHandMesh.TransformedVertices[(int)Fce.X].Position;
                            Vertex[1].Position = RightHandMesh.TransformedVertices[(int)Fce.Y].Position;
                            Vertex[2].Position = RightHandMesh.TransformedVertices[(int)Fce.Z].Position;

                            m_Devc.DrawUserPrimitives(PrimitiveType.TriangleList, Vertex, 0, 1);
                        }

                        TransformVertices(RightHandMesh, null, MeshType.RHand);
                    }
                }
                else
                {
                    m_VitaboyShader.Parameters["VitaboyTexture"].SetValue(RightHandTexture);
                    m_VitaboyShader.Parameters["World"].SetValue(WorldMatrix);
                    m_VitaboyShader.Parameters["ChildBones"].SetValue(GetAllBones(RightHandMesh, Skel));

                    foreach (EffectPass Pass in m_VitaboyShader.Techniques["TransformVerticesTechnique"].Passes)
                    {
                        Pass.Apply();

                        foreach (Vector3 Fce in RightHandMesh.Faces)
                        {
                            VitaboyVertex[] Vertex = new VitaboyVertex[3];
                            Vertex[0].Position = RightHandMesh.RealVertices[(int)Fce.X].Position;
                            Vertex[1].Position = RightHandMesh.RealVertices[(int)Fce.Y].Position;
                            Vertex[2].Position = RightHandMesh.RealVertices[(int)Fce.Z].Position;

                            Vertex[0].TextureCoordinate = RightHandMesh.RealVertices[(int)Fce.X].TextureCoordinate;
                            Vertex[1].TextureCoordinate = RightHandMesh.RealVertices[(int)Fce.Y].TextureCoordinate;
                            Vertex[2].TextureCoordinate = RightHandMesh.RealVertices[(int)Fce.Z].TextureCoordinate;

                            Vertex[0].Normal = RightHandMesh.RealVertices[(int)Fce.X].Normal;
                            Vertex[1].Normal = RightHandMesh.RealVertices[(int)Fce.Y].Normal;
                            Vertex[2].Normal = RightHandMesh.RealVertices[(int)Fce.Z].Normal;

                            //All the meshes except the body mesh only references one bone.
                            Vertex[0].BoneBinding = 0;
                            Vertex[1].BoneBinding = 0;
                            Vertex[2].BoneBinding = 0;

                            m_Devc.DrawUserPrimitives(PrimitiveType.TriangleList, Vertex, 0, 1);
                        }
                    }
                }
            }
        }

        private void CopyBodyVertices(Bone Bne)
        {
            BoneBinding boneBinding = BodyMesh.BoneBindings.FirstOrDefault(x => BodyMesh.Bones[(int)x.BoneIndex] == Bne.Name);

            if (boneBinding != null)
            {
                for (int i = 0; i < (boneBinding.RealVertexCount); i++)
                {
                    int VertexIndex = (int)boneBinding.FirstRealVertexIndex + i;
                    
                    BodyMesh.TransformedVertices[VertexIndex].BoneBinding = boneBinding.BoneIndex;
                    BodyMesh.TransformedVertices[VertexIndex].Position = BodyMesh.RealVertices[VertexIndex].Position;

                    BodyMesh.TransformedVertices[VertexIndex].TextureCoordinate = BodyMesh.RealVertices[VertexIndex].TextureCoordinate;

                    BodyMesh.TransformedVertices[VertexIndex].Normal = BodyMesh.RealVertices[VertexIndex].Normal;
                }
            }

            foreach (Bone Child in Bne.Children)
                CopyBodyVertices(Child);
        }

        /// <summary>
        /// Gets all the bones' absolute matrices from a mesh.
        /// </summary>
        /// <param name="Msh">The mesh containing a list of bone (names).</param>
        /// <param name="Skel">The skeleton with the bones that the names refer to.</param>
        /// <returns></returns>
        public Matrix[] GetAllBones(Mesh Msh, Skeleton Skel)
        {
            List<Matrix> AllMatrices = new List<Matrix>();

            foreach (string Bne in Msh.Bones)
                AllMatrices.Add(Skel.Bones[Skel.FindBone(Bne)].AbsoluteMatrix);

            return AllMatrices.ToArray();
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
                            Skel.Bones[Skel.FindBone("HEAD")].AbsoluteMatrix);

                        Msh.TransformedVertices[i].TextureCoordinate = Msh.RealVertices[i].TextureCoordinate;

                        //Transform the head normals' position by the absolute transform
                        //for the headbone (which is always bone 17) to render the head in place.
                        Msh.TransformedVertices[i].Normal = Vector3.TransformNormal(Msh.RealVertices[i].Normal,
                            Skel.Bones[Skel.FindBone("HEAD")].AbsoluteMatrix);
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
                            Msh.TransformedVertices[vertexIndex].Normal = Vector3.TransformNormal(Vector3.Zero, translatedMatrix);
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
                            Skel.Bones[Skel.FindBone("L_HAND")].AbsoluteMatrix);

                        //Transform the left hand normals' position by the absolute transform
                        //for the left handbone (which is always bone 10) to render the left hand in place.
                        Msh.TransformedVertices[i].Normal = Vector3.TransformNormal(Msh.RealVertices[i].Normal,
                            Skel.Bones[Skel.FindBone("L_HAND")].AbsoluteMatrix);
                    }

                    return;

                case MeshType.RHand:
                    for (int i = 0; i < Msh.TotalVertexCount; i++)
                    {
                        //Transform the right hand vertices' position by the absolute transform
                        //for the right handbone (which is always bone 15) to render the right hand in place.
                        Msh.TransformedVertices[i].Position = Vector3.Transform(Msh.RealVertices[i].Position,
                            Skel.Bones[Skel.FindBone("R_HAND")].AbsoluteMatrix);

                        //Transform the right hand normals' position by the absolute transform
                        //for the right handbone (which is always bone 15) to render the right hand in place.
                        Msh.TransformedVertices[i].Normal = Vector3.TransformNormal(Msh.RealVertices[i].Normal,
                            Skel.Bones[Skel.FindBone("R_HAND")].AbsoluteMatrix);
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
                ComputeBonePositions(child, myWorld);
        }

        ~AvatarBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of the resources used by this AvatarBase instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes of the resources used by this AvatarBase instance.
        /// <param name="Disposed">Was this resource disposed explicitly?</param>
        /// </summary>
        protected virtual void Dispose(bool Disposed)
        {
            if (Disposed)
            {
                if (m_AccessoryEffect != null)
                    m_AccessoryEffect.Dispose();
                if(m_BodyEffect != null)
                    m_BodyEffect.Dispose();
                if(m_HeadEffect != null)
                    m_HeadEffect.Dispose();
                if(m_LeftHandEffect != null)
                    m_LeftHandEffect.Dispose();
                if (m_RightHandEffect != null)
                    m_RightHandEffect.Dispose();

                // Prevent the finalizer from calling ~AvatarBase, since the object is already disposed at this point.
                GC.SuppressFinalize(this);
            }
            else
                m_Logger.Error("AvatarBase not explicitly disposed!");
        }
    }
}
