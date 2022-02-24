using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            XData = new XData(Endianness.Little, new byte[] { 2, 0 });
            Version = version;
            Name = name;
            SData = new List<byte>();
            XRef = new List<byte[]>();
            Classes = new List<MintClass>();
        }

        public MintScript(EndianBinaryReader reader, byte[] version)
        {
            XData = new XData(reader);
            Version = version;

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

        static char[] trimChars = new char[] { '\t', ' ' };

        public MintScript(string[] text, byte[] version)
        {
            string scriptDeclare = text[0];
            if (!scriptDeclare.StartsWith("script "))
            {
                MessageBox.Show("Error: Invalid Mint Script file.", "Mint Workshop", MessageBoxButtons.OK);
                return;
            }

            XData = new XData(Endianness.Little, new byte[] { 2, 0 });
            Version = version;
            Name = scriptDeclare.Substring(7);
            SData = new List<byte>();
            XRef = new List<byte[]>();
            Classes = new List<MintClass>();

            Regex classRegex = new Regex("\\b(class)\\b");
            Regex varRegex = new Regex("\\b(var)\\b");
            Regex funcRegex = new Regex("(\\(.*\\))");

            for (int l = 0; l < text.Length; l++)
            {
                string line = text[l].TrimStart(trimChars);
                if (line.StartsWith("//"))
                    continue;
                if (classRegex.IsMatch(line))
                {
                    string[] classDeclaration = line.Split(' ');
                    int classWord = classDeclaration.ToList().IndexOf("class");
                    MintClass newClass = new MintClass(classDeclaration[classWord + 1], 0, this);
                    for (int i = 0; i < classWord; i++)
                    {
                        if (classDeclaration[i].StartsWith("flag"))
                            newClass.Flags |= uint.Parse(classDeclaration[i].Remove(0, 4));
                        else if (FlagLabels.ClassFlags.ContainsValue(classDeclaration[i]))
                            newClass.Flags |= FlagLabels.ClassFlags.Keys.ToArray()[FlagLabels.ClassFlags.Values.ToList().IndexOf(classDeclaration[i])];
                        else
                            throw new Exception($"Unknown Class flag name \"{classDeclaration[i]}\"");
                    }

                    for (int cl = l; cl < text.Length; cl++)
                    {
                        string classLine = text[cl].TrimStart(trimChars);
                        if (classLine.StartsWith("//"))
                            continue;
                        if (varRegex.IsMatch(classLine))
                        {
                            string[] varDeclaration = classLine.Split(' ');
                            int varWord = varDeclaration.ToList().IndexOf("var");
                            MintVariable newVar = new MintVariable(varDeclaration[varWord + 2], varDeclaration[varWord + 1], 0, newClass);
                            for (int i = 0; i < varWord; i++)
                            {
                                if (varDeclaration[i].StartsWith("flag"))
                                    newVar.Flags |= uint.Parse(varDeclaration[i].Remove(0, 4));
                                else if (FlagLabels.VariableFlags.ContainsValue(varDeclaration[i]))
                                    newVar.Flags |= FlagLabels.VariableFlags.Keys.ToArray()[FlagLabels.VariableFlags.Values.ToList().IndexOf(varDeclaration[i])];
                                else
                                    throw new Exception($"Unknown Variable flag name \"{varDeclaration[i]}\"");
                            }
                            newClass.Variables.Add(newVar);
                        }
                        else if (funcRegex.IsMatch(classLine))
                        {
                            string[] funcDeclaration = classLine.Split(' ');
                            string name = "";
                            uint funcFlags = 0;
                            for (int i = 0; i < funcDeclaration.Length; i++)
                            {
                                if (funcDeclaration[i].StartsWith("flag"))
                                    funcFlags |= uint.Parse(funcDeclaration[i].Remove(0, 4));
                                else if (FlagLabels.FunctionFlags.ContainsValue(funcDeclaration[i]))
                                    funcFlags |= FlagLabels.FunctionFlags.Keys.ToArray()[FlagLabels.FunctionFlags.Values.ToList().IndexOf(funcDeclaration[i])];
                                else
                                {
                                    name = string.Join(" ", funcDeclaration.Skip(i));
                                    break;
                                }
                            }

                            MintFunction newFunc = new MintFunction(name, funcFlags, newClass);
                            List<string> instructions = new List<string>();
                            for (int fl = cl + 2; fl < text.Length; fl++)
                            {
                                string funcLine = text[fl].TrimStart(trimChars);
                                if (funcLine.StartsWith("//"))
                                    continue;
                                if (funcLine.StartsWith("}"))
                                {
                                    cl = fl;
                                    break;
                                }
                                instructions.Add(funcLine);
                            }
                            newFunc.Assemble(instructions.ToArray());
                            newClass.Functions.Add(newFunc);
                        }
                        else if (classLine.StartsWith("const "))
                        {
                            string[] constDeclaration = classLine.Split(' ');
                            uint value;
                            if (constDeclaration[3].StartsWith("0x"))
                                value = uint.Parse(constDeclaration[3].Substring(2), NumberStyles.HexNumber);
                            else
                                value = uint.Parse(constDeclaration[3]);

                            newClass.Constants.Add(new MintClass.MintConstant(constDeclaration[1], value));
                        }
                        else if (classLine.StartsWith("unkvalue "))
                        {
                            string[] unkDeclaration = classLine.Split(' ');
                            newClass.UnknownList.Add(uint.Parse(unkDeclaration[1]));
                        }
                        else if (classLine.StartsWith("}"))
                        {
                            l = cl;
                            break;
                        }
                    }

                    Classes.Add(newClass);
                }
            }
        }

        public void Optimize()
        {
            Dictionary<byte[], string> blankHashes = new Dictionary<byte[], string>(new ByteArrayComparer());

            MintScript optimized = new MintScript(WriteText(ref blankHashes), Version);
            SData = optimized.SData;
            XRef = optimized.XRef;
            Classes = optimized.Classes;
        }

        public byte[] Write()
        {
            using (MemoryStream stream = new MemoryStream())
            using (EndianBinaryWriter writer = new EndianBinaryWriter(stream, XData.Endianness))
            {
                XData.Write(writer);

                writer.Write(-1);
                writer.Write(0x20);
                writer.Write(0x24 + SData.Count + 4);
                uint classListOffs = (uint)(0x24 + SData.Count + 8 + (XRef.Count * 4));
                writer.Write(classListOffs);
                
                writer.Write(SData.Count);
                writer.Write(SData.ToArray());
                writer.Write((uint)0);

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
                    if (!ByteArrayComparer.Equal(Version, new byte[] { 1, 0, 5, 0 }))
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
                        if (Version[0] >= 2 || Version[1] >= 1) //Only 2.x and 1.1.x use function flags
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

                    if (Version[0] >= 2 || Version[1] >= 1)
                    {
                        writer.BaseStream.Seek(cl + 0x14, SeekOrigin.Begin);
                        writer.Write((uint)writer.BaseStream.Length);
                        writer.BaseStream.Seek(0, SeekOrigin.End);

                        writer.Write(Classes[i].UnknownList.Count);
                        for (int v = 0; v < Classes[i].UnknownList.Count; v++)
                            writer.Write(Classes[i].UnknownList[v]);
                    }

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
        
        public string[] WriteText(ref Dictionary<byte[], string> hashes)
        {
            List<string> text = new List<string>();

            text.Add($"script {Name}");
            text.Add("{");

            for (int c = 0; c < Classes.Count; c++)
            {
                uint cFlags = Classes[c].Flags;
                string classFlags = "";
                for (uint i = 1; i <= cFlags; i <<= 1)
                {
                    if ((cFlags & i) != 0)
                    {
                        if (FlagLabels.ClassFlags.ContainsKey(cFlags & i))
                            classFlags += $"{FlagLabels.ClassFlags[cFlags & i]} ";
                        else
                            classFlags += $"flag{cFlags & i:X} ";
                    }
                }

                text.Add($"\t{classFlags}class {Classes[c].Name}");
                text.Add("\t{");

                for (int i = 0; i < Classes[c].Variables.Count; i++)
                {
                    uint vFlags = Classes[c].Variables[i].Flags;
                    string varFlags = "";
                    for (uint f = 1; f <= vFlags; f <<= 1)
                    {
                        if ((vFlags & f) != 0)
                        {
                            if (FlagLabels.VariableFlags.ContainsKey(vFlags & f))
                                varFlags += $"{FlagLabels.VariableFlags[vFlags & f]} ";
                            else
                                varFlags += $"flag{vFlags & f:X} ";
                        }
                    }

                    text.Add($"\t\t{varFlags}var {Classes[c].Variables[i].Type} {Classes[c].Variables[i].Name}");
                }
                if (Classes[c].Variables.Count > 0)
                    text.Add("\t\t");

                for (int i = 0; i < Classes[c].Functions.Count; i++)
                {
                    uint fFlags = Classes[c].Functions[i].Flags;
                    string funcFlags = "";
                    if (Version[0] >= 2 || Version[1] >= 1) //Only 2.x and 1.1.x use function flags
                    {
                        for (uint f = 1; f <= fFlags; f <<= 1)
                        {
                            if ((fFlags & f) != 0)
                            {
                                if (FlagLabels.FunctionFlags.ContainsKey(fFlags & f))
                                    funcFlags += $"{FlagLabels.FunctionFlags[fFlags & f]} ";
                                else
                                    funcFlags += $"flag{fFlags & f:X} ";
                            }
                        }
                    }

                    text.Add($"\t\t{funcFlags}{Classes[c].Functions[i].Name}");
                    text.Add("\t\t{");

                    string[] disasm = Classes[c].Functions[i].Disassemble(ref hashes).TrimEnd(new char[] { ' ', '\n', '\r' }).Split('\n');
                    for (int d = 0; d < disasm.Length; d++)
                        text.Add("\t\t\t" + disasm[d]);

                    text.Add("\t\t}");
                    text.Add("\t\t");
                }
                if (Classes[c].Functions.Count > 0)
                    text.Add("\t\t");

                for (int i = 0; i < Classes[c].Constants.Count; i++)
                {
                    text.Add($"\t\tconst {Classes[c].Constants[i].Name} = 0x{Classes[c].Constants[i].Value:X}");
                }
                if (Classes[c].Constants.Count > 0)
                    text.Add("\t\t");

                for (int i = 0; i < Classes[c].UnknownList.Count; i++)
                {
                    text.Add($"\t\tunkvalue {Classes[c].UnknownList[i]}");
                }

                while (text.Last().TrimStart(new char[] { '\t' }) == "")
                {
                    text.RemoveAt(text.Count - 1);
                }

                text.Add("\t}");
                if (c < Classes.Count - 1)
                    text.Add("\t");
            }

            text.Add("}");

            return text.ToArray();
        }
    }
}
