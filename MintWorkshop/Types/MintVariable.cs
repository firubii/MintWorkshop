using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MintWorkshop.Mint;
using MintWorkshop.Util;

namespace MintWorkshop.Types
{
    public class MintVariable
    {
        public MintClass ParentClass { get; private set; }

        public string Name { get; private set; }
        public byte[] Hash { get; private set; }
        public string Type { get; set; }
        public uint Flags { get; set; }

        public MintVariable(string name, string type, uint flags, MintClass parentClass)
        {
            ParentClass = parentClass;
            Name = name;
            Type = type;
            Flags = flags;
        }

        public MintVariable(EndianBinaryReader reader, MintClass parentClass)
        {
            ParentClass = parentClass;

            uint nameOffs = reader.ReadUInt32();
            byte[] hash = reader.ReadBytes(4);
            uint typeOffs = reader.ReadUInt32();
            uint flags = reader.ReadUInt32();

            reader.BaseStream.Seek(nameOffs, SeekOrigin.Begin);
            string name = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));

            reader.BaseStream.Seek(typeOffs, SeekOrigin.Begin);
            string type = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));

            Name = name;
            Hash = hash;
            Type = type;
            Flags = flags;
        }

        public void SetName(string name)
        {
            Name = name;
            Hash = HashCalculator.Calculate(Name);
        }

        public string FullName()
        {
            return ParentClass.Name + "." + Name;
        }
    }
}
