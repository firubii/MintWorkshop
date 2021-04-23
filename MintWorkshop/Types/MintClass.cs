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
    public class MintClass
    {
        public struct Constant
        {
            public Constant(string name, uint value)
            {
                this.Name = name;
                this.Value = value;
            }
            public string Name;
            public uint Value;
        }

        public MintScript ParentScript { get; private set; }

        public string Name { get; private set; }
        public byte[] Hash { get; private set; }
        public List<MintVariable> Variables { get; private set; }
        public List<MintFunction> Functions { get; private set; }
        public List<Constant> Constants { get; private set; }
        public List<uint> UnknownList { get; set; }
        public uint Flags { get; set; }

        public MintClass(string name, uint flags, MintScript parent)
        {
            ParentScript = parent;
            Name = name;
            Hash = HashCalculator.Calculate(Name);
            Variables = new List<MintVariable>();
            Functions = new List<MintFunction>();
            Constants = new List<Constant>();
            UnknownList = new List<uint>();
            Flags = flags;
        }

        public MintClass(EndianBinaryReader reader, MintScript parent)
        {
            ParentScript = parent;

            uint nameOffs = reader.ReadUInt32();
            Hash = reader.ReadBytes(4);
            uint varOffs = reader.ReadUInt32();
            uint funcOffs = reader.ReadUInt32();
            uint constOffs = reader.ReadUInt32();
            uint unkOffs = reader.ReadUInt32();
            Flags = reader.ReadUInt32();

            reader.BaseStream.Seek(nameOffs, SeekOrigin.Begin);
            Name = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));

            reader.BaseStream.Seek(varOffs, SeekOrigin.Begin);
            uint varCount = reader.ReadUInt32();
            Variables = new List<MintVariable>();
            for (int i = 0; i < varCount; i++)
            {
                reader.BaseStream.Seek(varOffs + 4 + (i * 4), SeekOrigin.Begin);
                reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
                Variables.Add(new MintVariable(reader, this));
            }

            reader.BaseStream.Seek(funcOffs, SeekOrigin.Begin);
            uint funcCount = reader.ReadUInt32();
            Functions = new List<MintFunction>();
            for (int i = 0; i < funcCount; i++)
            {
                reader.BaseStream.Seek(funcOffs + 4 + (i * 4), SeekOrigin.Begin);
                reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
                Functions.Add(new MintFunction(reader, this));
            }

            reader.BaseStream.Seek(constOffs, SeekOrigin.Begin);
            uint constCount = reader.ReadUInt32();
            Constants = new List<Constant>();
            for (int i = 0; i < constCount; i++)
            {
                reader.BaseStream.Seek(constOffs + 4 + (i * 4), SeekOrigin.Begin);
                reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
                uint cnameOffs = reader.ReadUInt32();
                uint cval = reader.ReadUInt32();

                reader.BaseStream.Seek(cnameOffs, SeekOrigin.Begin);
                string cname = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));

                Constants.Add(new Constant(cname, cval));
            }

            UnknownList = new List<uint>();
            if (!ByteArrayComparer.Equal(ParentScript.Version, new byte[] { 1, 0, 5, 0 }))
            {
                reader.BaseStream.Seek(unkOffs, SeekOrigin.Begin);
                uint unkCount = reader.ReadUInt32();
                UnknownList = new List<uint>();
                for (int i = 0; i < unkCount; i++)
                    UnknownList.Add(reader.ReadUInt32());
            }
            /*if (unkCount > 0)
                Console.WriteLine($"{Name} - {unkCount} Unknowns: {string.Join(",", UnknownList)}");*/
        }

        public void SetName(string name)
        {
            Name = name;
            Hash = HashCalculator.Calculate(Name);
        }
    }
}
