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
        public struct MintConstant
        {
            public MintConstant(string name, uint value)
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
        public List<MintConstant> Constants { get; private set; }
        public List<uint> UnknownList { get; set; }
        public List<uint> Unknown2List { get; set; }
        public uint Flags { get; set; }

        public MintClass(string name, uint flags, MintScript parent)
        {
            ParentScript = parent;
            SetName(name);
            Variables = new List<MintVariable>();
            Functions = new List<MintFunction>();
            Constants = new List<MintConstant>();
            UnknownList = new List<uint>();
            Unknown2List = new List<uint>();
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
            uint unkOffs = 0;
            uint unk2Offs = 0;
            if (ParentScript.Version[0] >= 2 || ParentScript.Version[1] >= 1)
                unkOffs = reader.ReadUInt32();
            if (ParentScript.Version[0] >= 7)
                unk2Offs = reader.ReadUInt32();
            Flags = reader.ReadUInt32();

            reader.BaseStream.Seek(nameOffs, SeekOrigin.Begin);
            Name = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));

            Variables = new List<MintVariable>();
            if (varOffs > 0)
            {
                reader.BaseStream.Seek(varOffs, SeekOrigin.Begin);
                uint varCount = reader.ReadUInt32();
                for (int i = 0; i < varCount; i++)
                {
                    reader.BaseStream.Seek(varOffs + 4 + (i * 4), SeekOrigin.Begin);
                    reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
                    Variables.Add(new MintVariable(reader, this));
                }
            }

            Functions = new List<MintFunction>();
            if (funcOffs > 0)
            {
                reader.BaseStream.Seek(funcOffs, SeekOrigin.Begin);
                uint funcCount = reader.ReadUInt32();
                for (int i = 0; i < funcCount; i++)
                {
                    reader.BaseStream.Seek(funcOffs + 4 + (i * 4), SeekOrigin.Begin);
                    reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
                    Functions.Add(new MintFunction(reader, this));
                }
            }

            Constants = new List<MintConstant>();
            if (constOffs > 0)
            {
                reader.BaseStream.Seek(constOffs, SeekOrigin.Begin);
                uint constCount = reader.ReadUInt32();
                for (int i = 0; i < constCount; i++)
                {
                    reader.BaseStream.Seek(constOffs + 4 + (i * 4), SeekOrigin.Begin);
                    reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
                    uint cnameOffs = reader.ReadUInt32();
                    uint cval = reader.ReadUInt32();

                    reader.BaseStream.Seek(cnameOffs, SeekOrigin.Begin);
                    string cname = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));

                    Constants.Add(new MintConstant(cname, cval));
                }
            }

            UnknownList = new List<uint>();
            if (unkOffs > 0)
            {
                reader.BaseStream.Seek(unkOffs, SeekOrigin.Begin);
                uint unkCount = reader.ReadUInt32();
                for (int i = 0; i < unkCount; i++)
                    UnknownList.Add(reader.ReadUInt32());
                if (UnknownList.Count > 0)
                    Console.WriteLine($"UnkData found at {Name} - {UnknownList.Count} Unknowns: {string.Join(",", UnknownList)}");
            }

            Unknown2List = new List<uint>();
            if (unk2Offs > 0)
            {
                reader.BaseStream.Seek(unk2Offs, SeekOrigin.Begin);
                uint unk2Count = reader.ReadUInt32();
                for (int i = 0; i < unk2Count; i++)
                    Unknown2List.Add(reader.ReadUInt32());
                if (Unknown2List.Count > 0)
                    Console.WriteLine($"Unk2Data found at {Name} - {Unknown2List.Count} Unknowns: {string.Join(",", Unknown2List)}");
            }
        }

        public void SetName(string name)
        {
            Name = name;
            Hash = HashCalculator.Calculate(Name);
        }
    }
}
