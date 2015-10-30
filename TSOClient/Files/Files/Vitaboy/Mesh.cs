/*This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
If a copy of the MPL was not distributed with this file, You can obtain one at
http://mozilla.org/MPL/2.0/.

The Original Code is the SimsLib.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s):
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Files.Vitaboy
{
    public class Mesh
    {
        private FileReader m_Reader;
        public uint BoneCount;
        public List<string> Bones = new List<string>();
        public uint FaceCount;
        public List<Vector3> Faces = new List<Vector3>();
        public uint BindingCount;
        public List<BoneBinding> BoneBindings = new List<BoneBinding>();
        public uint RealVertexCount;
        public uint BlendVertexCount;
        public List<BlendVertexProperty> BlendVertexProps = new List<BlendVertexProperty>();
        public uint TotalVertexCount;
        public List<VertexPositionNormalTexture> RealVertices = new List<VertexPositionNormalTexture>();
        public VertexPositionNormalTexture[] TransformedVertices;
        public List<VertexPositionNormalTexture> BlendedVertices = new List<VertexPositionNormalTexture>();

        public Mesh(Stream Data)
        {
            m_Reader = new FileReader(Data, true);

            m_Reader.ReadUInt32(); //Version
            BoneCount = m_Reader.ReadUInt32();

            for (int i = 0; i < BoneCount; i++)
                Bones.Add(m_Reader.ReadPascalString());

            FaceCount = m_Reader.ReadUInt32();

            for (int i = 0; i < FaceCount; i++)
                Faces.Add(new Vector3(m_Reader.ReadUInt32(), m_Reader.ReadUInt32(), m_Reader.ReadUInt32()));

            BindingCount = m_Reader.ReadUInt32();

            for (int i = 0; i < BindingCount; i++)
                BoneBindings.Add(new BoneBinding(m_Reader));

            RealVertexCount = m_Reader.ReadUInt32();
            List<Vector2> TexVertices = new List<Vector2>();

            for (int i = 0; i < RealVertexCount; i++)
                TexVertices.Add(new Vector2(m_Reader.ReadFloat(), m_Reader.ReadFloat()));

            BlendVertexCount = m_Reader.ReadUInt32();

            for (int i = 0; i < BlendVertexCount; i++)
                BlendVertexProps.Add(new BlendVertexProperty(m_Reader));

            TotalVertexCount = m_Reader.ReadUInt32();
            TransformedVertices = new VertexPositionNormalTexture[TotalVertexCount];

            for (int i = 0; i < RealVertexCount; i++)
            {
                RealVertices.Add(new VertexPositionNormalTexture(new Vector3(m_Reader.ReadFloat(), m_Reader.ReadFloat(), m_Reader.ReadFloat()),
                    new Vector3(m_Reader.ReadFloat(), m_Reader.ReadFloat(), m_Reader.ReadFloat()), TexVertices[i]));
            }

            for(int i = 0; i < BlendVertexCount; i++)
            {
                BlendedVertices.Add(new VertexPositionNormalTexture(new Vector3(m_Reader.ReadFloat(), m_Reader.ReadFloat(), m_Reader.ReadFloat()), 
                    new Vector3(m_Reader.ReadFloat(), m_Reader.ReadFloat(), m_Reader.ReadFloat()), TexVertices[i]));
            }

            m_Reader.Close();
        }
    }

    public class BlendData
    {
        public float Weight;
        public int OtherVertex;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MeshVertex
    {
        public Vector3 Coord;
        /** UV Mapping **/
        public Vector2 TextureCoord;
        public Vector3 NormalCoord;

        public static int SizeInBytes = sizeof(float) * 8;

        public static VertexElement[] VertexElements = new VertexElement[]
        {
             new VertexElement( 0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0 ),
             new VertexElement( 0, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0 ),
             new VertexElement( 0, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0 )
        };
    }

    public class MeshVertexData
    {
        public MeshVertex Vertex;

        public uint BoneIndex;
        public BlendData BlendData;
    }
}
