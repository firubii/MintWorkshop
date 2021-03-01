using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MintWorkshop.Mint;
using MintWorkshop.Util;

namespace MintWorkshop.Types
{
    public class MintScript
    {
        public XData XData { get; private set; }
        public byte[] Version { get; private set; }

        public string Name { get; set; }
        public List<byte> SData { get; set; }
        public List<byte[]> XRef { get; set; }
        public List<MintClass> Classes { get; private set; }

        public MintScript(string name, byte[] version)
        {
            XData = new XData(Endianness.Little);
            Version = version;
            Name = name;
            SData = new List<byte>();
            XRef = new List<byte[]>();
            Classes = new List<MintClass>();
        }

        public MintScript(EndianBinaryReader reader)
        {
            XData = new XData(reader);

            reader.BaseStream.Seek(0x10, SeekOrigin.Begin);
            reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
            Name = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));

            reader.BaseStream.Seek(0x14, SeekOrigin.Begin);
            reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
            SData = new List<byte>();
            SData.AddRange(reader.ReadBytes(reader.ReadInt32()));

            reader.BaseStream.Seek(0x18, SeekOrigin.Begin);
            reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
            uint xrefCount = reader.ReadUInt32();
            XRef = new List<byte[]>();
            for (int i = 0; i < xrefCount; i++)
                XRef.Add(reader.ReadBytes(4));

            reader.BaseStream.Seek(0x1C, SeekOrigin.Begin);
            uint classOffs = reader.ReadUInt32();
            reader.BaseStream.Seek(classOffs, SeekOrigin.Begin);
            uint classCount = reader.ReadUInt32();
            Classes = new List<MintClass>();
            for (int i = 0; i < classCount; i++)
            {
                reader.BaseStream.Seek(classOffs + 4 + (i * 4), SeekOrigin.Begin);
                reader.BaseStream.Seek(reader.ReadUInt32(), SeekOrigin.Begin);
                Classes.Add(new MintClass(reader, this));
            }
        }

        public byte[] Write()
        {
            using (MemoryStream stream = new MemoryStream())
            using (EndianBinaryWriter writer = new EndianBinaryWriter(stream, XData.Endianness))
            {
                XData.Write(writer);

                writer.Write(-1);
                writer.Write(0x20);
                writer.Write(0x24 + SData.Count);
                uint classListOffs = (uint)(0x24 + SData.Count + 4 + (XRef.Count * 4));
                writer.Write(classListOffs);
                
                writer.Write(SData.Count);
                writer.Write(SData.ToArray());

                writer.Write(XRef.Count);
                for (int i = 0; i < XRef.Count; i++)
                    writer.Write(XRef[i]);

                writer.Write(Classes.Count);
                for (int i = 0; i < Classes.Count; i++)
                    writer.Write(-1);

                List<uint> classNameOffs = new List<uint>();
                List<uint[]> varNameOffs = new List<uint[]>();
                List<uint[]> funcNameOffs = new List<uint[]>();
                List<uint[]> constNameOffs = new List<uint[]>();
                for (int i = 0; i < Classes.Count; i++)
                {
                    writer.BaseStream.Seek(classListOffs + 4 + (i * 4), SeekOrigin.Begin);
                    writer.Write((uint)writer.BaseStream.Length);
                    writer.BaseStream.Seek(0, SeekOrigin.End);

                    uint cl = (uint)writer.BaseStream.Position;
                    classNameOffs.Add(cl);

                    uint varListOffs = cl + 0x1C;
                    uint funcListOffs = (uint)(varListOffs + 4 + (Classes[i].Variables.Count * 4) + (Classes[i].Variables.Count * 0x10));
                    List<uint> funcOffsList = new List<uint>();

                    writer.Write(-1);
                    writer.Write(Classes[i].Hash);
                    writer.Write(-1);
                    writer.Write(-1);
                    writer.Write(-1);
                    writer.Write(-1);
                    writer.Write(Classes[i].Flags);

                    writer.BaseStream.Seek(cl + 0x8, SeekOrigin.Begin);
                    writer.Write((uint)writer.BaseStream.Length);
                    writer.BaseStream.Seek(0, SeekOrigin.End);

                    writer.Write(Classes[i].Variables.Count);
                    List<uint> vOffs = new List<uint>();
                    for (int v = 0; v < Classes[i].Variables.Count; v++)
                        writer.Write((uint)(varListOffs + 4 + (Classes[i].Variables.Count * 4) + (v * 0x10)));
                    for (int v = 0; v < Classes[i].Variables.Count; v++)
                    {
                        vOffs.Add((uint)writer.BaseStream.Position);
                        writer.Write(-1);
                        writer.Write(Classes[i].Variables[v].Hash);
                        writer.Write(-1);
                        writer.Write(Classes[i].Variables[v].Flags);
                    }

                    writer.BaseStream.Seek(cl + 0xC, SeekOrigin.Begin);
                    writer.Write((uint)writer.BaseStream.Length);
                    writer.BaseStream.Seek(0, SeekOrigin.End);

                    writer.Write(Classes[i].Functions.Count);
                    for (int v = 0; v < Classes[i].Functions.Count; v++)
                        writer.Write(-1);
                    for (int v = 0; v < Classes[i].Functions.Count; v++)
                    {
                        writer.BaseStream.Seek(funcListOffs + 4 + (v * 4), SeekOrigin.Begin);
                        writer.Write((uint)writer.BaseStream.Length);
                        writer.BaseStream.Seek(0, SeekOrigin.End);

                        funcOffsList.Add((uint)writer.BaseStream.Length);
                        writer.Write(-1);
                        writer.Write(Classes[i].Functions[v].Hash);
                        writer.Write((uint)writer.BaseStream.Position + 8);
                        writer.Write(Classes[i].Functions[v].Flags);
                        for (int f = 0; f < Classes[i].Functions[v].Instructions.Count; f++)
                        {
                            Instruction inst = Classes[i].Functions[v].Instructions[f];
                            writer.Write(inst.Opcode);
                            writer.Write(inst.Z);
                            writer.Write(inst.X);
                            writer.Write(inst.Y);
                        }
                    }

                    writer.BaseStream.Seek(cl + 0x10, SeekOrigin.Begin);
                    writer.Write((uint)writer.BaseStream.Length);
                    writer.BaseStream.Seek(0, SeekOrigin.End);

                    writer.Write(Classes[i].Constants.Count);
                    uint constListOffs = (uint)writer.BaseStream.Position;
                    List<uint> cOffs = new List<uint>();
                    for (int v = 0; v < Classes[i].Constants.Count; v++)
                        writer.Write((uint)(constListOffs + (Classes[i].Constants.Count * 4) + (v * 8)));
                    for (int v = 0; v < Classes[i].Constants.Count; v++)
                    {
                        cOffs.Add((uint)writer.BaseStream.Position);
                        writer.Write(-1);
                        writer.Write(Classes[i].Constants[v].Value);
                    }

                    writer.BaseStream.Seek(cl + 0x14, SeekOrigin.Begin);
                    writer.Write((uint)writer.BaseStream.Length);
                    writer.BaseStream.Seek(0, SeekOrigin.End);

                    writer.Write(Classes[i].UnknownList.Count);
                    for (int v = 0; v < Classes[i].UnknownList.Count; v++)
                        writer.Write(Classes[i].UnknownList[v]);

                    varNameOffs.Add(vOffs.ToArray());
                    funcNameOffs.Add(funcOffsList.ToArray());
                    constNameOffs.Add(cOffs.ToArray());
                }

                //String writing
                writer.BaseStream.Seek(0x10, SeekOrigin.Begin);
                writer.Write((uint)writer.BaseStream.Length);
                writer.BaseStream.Seek(0, SeekOrigin.End);
                WriteUtil.WriteString(writer, Name);

                for (int i = 0; i < Classes.Count; i++)
                {
                    writer.BaseStream.Seek(classNameOffs[i], SeekOrigin.Begin);
                    writer.Write((uint)writer.BaseStream.Length);
                    writer.BaseStream.Seek(0, SeekOrigin.End);
                    WriteUtil.WriteString(writer, Classes[i].Name);

                    for (int v = 0; v < Classes[i].Variables.Count; v++)
                    {
                        uint vo = varNameOffs[i][v];
                        writer.BaseStream.Seek(vo, SeekOrigin.Begin);
                        writer.Write((uint)writer.BaseStream.Length);
                        writer.BaseStream.Seek(0, SeekOrigin.End);
                        WriteUtil.WriteString(writer, Classes[i].Variables[v].Name);

                        writer.BaseStream.Seek(vo + 0x8, SeekOrigin.Begin);
                        writer.Write((uint)writer.BaseStream.Length);
                        writer.BaseStream.Seek(0, SeekOrigin.End);
                        WriteUtil.WriteString(writer, Classes[i].Variables[v].Type);
                    }

                    for (int v = 0; v < Classes[i].Functions.Count; v++)
                    {
                        writer.BaseStream.Seek(funcNameOffs[i][v], SeekOrigin.Begin);
                        writer.Write((uint)writer.BaseStream.Length);
                        writer.BaseStream.Seek(0, SeekOrigin.End);
                        WriteUtil.WriteString(writer, Classes[i].Functions[v].Name);
                    }

                    for (int v = 0; v < Classes[i].Constants.Count; v++)
                    {
                        writer.BaseStream.Seek(constNameOffs[i][v], SeekOrigin.Begin);
                        writer.Write((uint)writer.BaseStream.Length);
                        writer.BaseStream.Seek(0, SeekOrigin.End);
                        WriteUtil.WriteString(writer, Classes[i].Constants[v].Name);
                    }
                }

                XData.UpdateFilesize(writer);
                return stream.GetBuffer().Take((int)XData.Filesize).ToArray();
            }
        }
    }
}
