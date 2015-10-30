using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Files.Vitaboy
{
    /// <summary>
    /// Bindings (known as skins in The Sims 1) specify a mesh and the bone to which it is applied. 
    /// They may optionally specify a texture to apply to the mesh.
    /// </summary>
    public class Binding
    {
        /// <summary>
        /// A Pascal string specifying the bone to which the mesh applies.
        /// </summary>
        public string Bone;

        /// <summary>
        /// The mesh this binding points to. May be null.
        /// </summary>
        public UniqueFileID MeshID;

        /// <summary>
        /// The texture this binding points to. May be null.
        /// </summary>
        public UniqueFileID TextureID;

        public Binding(Stream Data)
        {
            FileReader Reader = new FileReader(Data, true);

            Reader.ReadUInt32(); //Version

            Bone = Reader.ReadPascalString();

            uint AssetType = Reader.ReadUInt32();
            uint FileID = 0, TypeID = 0;

            if (AssetType == 8)
            {
                //A 4-byte unsigned integer specifying the type of data that follows; should be 0xA96F6D42 for cAssetKey
                Reader.ReadUInt32();
                FileID = Reader.ReadUInt32();
                TypeID = Reader.ReadUInt32();
                MeshID = new UniqueFileID(TypeID, FileID);
            }

            AssetType = Reader.ReadUInt32();

            if (AssetType == 8)
            {
                //A 4-byte unsigned integer specifying the type of data that follows; should be 0xA96F6D42 for cAssetKey
                Reader.ReadUInt32();
                FileID = Reader.ReadUInt32();
                TypeID = Reader.ReadUInt32();
                TextureID = new UniqueFileID(TypeID, FileID);
            }
        }
    }
}
