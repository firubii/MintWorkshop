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
        public byte[] Hash { get; private set; } //Only for Mint 7.0.2
        public List<byte> SData { get; set; }
        public List<byte[]> XRef { get; set; }
        public List<MintClass> Classes { get; private set; }

        public MintScript(string name, byte[] version)
        {
            XData = new XData(Endianness.Little, new byte[] { 2, 0 });
            Version = version;
            Name = name;
            if (Version[0] >= 7)
                Hash = HashCalculator.Calculate(Name);
            SData = new List<byte>();
            XRef = new List<byte[]>();
            Classes = new List<MintClass>();
        }

        public MintScript(EndianBinaryReader reader, byte[] version)
        {
            XData = new XData(reader);
            Version = version;

            uint nameOffs = reader.ReadUInt32();
            if (version[0] >= 7)
                Hash = reader.ReadBytes(4);
            else
                Hash = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
            uint sdataOffs = reader.ReadUInt32();
            uint xrefOffs = reader.ReadUInt32();
            uint classOffs = reader.ReadUInt32();

            reader.BaseStream.Seek(nameOffs, SeekOrigin.Begin);
            Name = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));

            reader.BaseStream.Seek(sdataOffs, SeekOrigin.Begin);
            SData = new List<byte>();
            SData.AddRange(reader.ReadBytes(reader.ReadInt32()));

            reader.BaseStream.Seek(xrefOffs, SeekOrigin.Begin);
            uint xrefCount = reader.ReadUInt32();
            XRef = new List<byte[]>();
            for (int i = 0; i < xrefCount; i++)
                XRef.Add(reader.ReadBytes(4));

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
            if (Version[0] >= 7)
                Hash = HashCalculator.Calculate(Name);
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
                            newClass.Flags |= uint.Parse(classDeclaration[i].Substring(4), NumberStyles.HexNumber);
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
                                    newVar.Flags |= uint.Parse(varDeclaration[i].Substring(4), NumberStyles.HexNumber);
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
                                    funcFlags |= uint.Parse(funcDeclaration[i].Substring(4), NumberStyles.HexNumber);
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
                            newFunc.DetectArguments();
                            newFunc.DetectRegisters();

                            newClass.Functions.Add(newFunc);
                        }
                        else if (classLine.StartsWith("implements "))
                        {
                            string implement = classLine.Substring(11);
                            newClass.ClassImpl.Add((ushort)AddXRef(implement));
                        }
                        else if (classLine.StartsWith("extends "))
                        {
                            string extend = classLine.Substring(8);
                            newClass.Extends.Add(new MintClass.ClassExtend((ushort)AddXRef(extend), false));
                        }
                        else if (classLine.StartsWith("extendstd "))
                        {
                            string extend = classLine.Substring(10);
                            bool valid = false;
                            foreach (KeyValuePair<ushort, string> pair in MintClass.StdTypes)
                            {
                                if (pair.Value == extend)
                                {
                                    valid = true;
                                    newClass.Extends.Add(new MintClass.ClassExtend(pair.Key, true));
                                }
                            }
                            if (!valid)
                                newClass.Extends.Add(new MintClass.ClassExtend(ushort.Parse(extend), true));
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

                int padding = Version[0] >= 7 ? 0 : -1;
                uint fileStart = (uint)writer.BaseStream.Position;
                writer.Write(padding);
                if (Version[0] >= 7)
                {
                    if (Hash == null)
                        Hash = HashCalculator.Calculate(Name);
                    writer.Write(Hash);
                }
                uint hSdataOffset = (uint)writer.BaseStream.Position;
                writer.Write(Version[0] >= 7 ? 0x28 : 0x20);
                writer.Write(padding);
                writer.Write(padding);
                if (Version[0] >= 7)
                    writer.Write(padding);

                writer.Write(SData.Count);
                writer.Write(SData.ToArray());
                writer.Write((uint)0);
                WriteUtil.WritePadding(writer);

                writer.BaseStream.Seek(hSdataOffset + 0x4, SeekOrigin.Begin);
                writer.Write((uint)writer.BaseStream.Length);
                writer.BaseStream.Seek(0, SeekOrigin.End);
                writer.Write(XRef.Count);
                for (int i = 0; i < XRef.Count; i++)
                    writer.Write(XRef[i]);

                writer.BaseStream.Seek(hSdataOffset + 0x8, SeekOrigin.Begin);
                writer.Write((uint)writer.BaseStream.Length);
                writer.BaseStream.Seek(0, SeekOrigin.End);
                uint classListOffs = (uint)writer.BaseStream.Position;
                writer.Write(Classes.Count);
                for (int i = 0; i < Classes.Count; i++)
                    writer.Write(padding);

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

                    writer.Write(padding);
                    writer.Write(Classes[i].Hash);
                    writer.Write(padding);
                    writer.Write(padding);
                    writer.Write(padding);
                    if (Version[0] >= 2 || Version[1] >= 1)
                        writer.Write(padding);
                    if (Version[0] >= 7)
                        writer.Write(padding);
                    writer.Write(Classes[i].Flags);

                    bool writeVariables = true;
                    //if (Version[0] >= 7)
                    //    writeVariables = Classes[i].Variables.Count > 0;

                    List<uint> vOffs = new List<uint>();
                    if (writeVariables)
                    {
                        writer.BaseStream.Seek(cl + 0x8, SeekOrigin.Begin);
                        writer.Write((uint)writer.BaseStream.Length);
                        writer.BaseStream.Seek(0, SeekOrigin.End);

                        uint varListOffs = (uint)writer.BaseStream.Position;
                        writer.Write(Classes[i].Variables.Count);
                        for (int v = 0; v < Classes[i].Variables.Count; v++)
                            writer.Write((uint)(varListOffs + 4 + (Classes[i].Variables.Count * 4) + (v * 0x10)));
                        for (int v = 0; v < Classes[i].Variables.Count; v++)
                        {
                            vOffs.Add((uint)writer.BaseStream.Position);
                            writer.Write(padding);
                            writer.Write(Classes[i].Variables[v].Hash);
                            writer.Write(padding);
                            writer.Write(Classes[i].Variables[v].Flags);
                        }
                    }

                    bool writeFunctions = true;
                    //if (Version[0] >= 7)
                    //    writeFunctions = Classes[i].Functions.Count > 0;

                    List<uint> funcOffsList = new List<uint>();
                    if (writeFunctions)
                    {
                        writer.BaseStream.Seek(cl + 0xC, SeekOrigin.Begin);
                        writer.Write((uint)writer.BaseStream.Length);
                        writer.BaseStream.Seek(0, SeekOrigin.End);

                        uint funcListOffs = (uint)writer.BaseStream.Position;
                        writer.Write(Classes[i].Functions.Count);
                        for (int v = 0; v < Classes[i].Functions.Count; v++)
                            writer.Write(padding);
                        for (int v = 0; v < Classes[i].Functions.Count; v++)
                        {
                            writer.BaseStream.Seek(funcListOffs + 4 + (v * 4), SeekOrigin.Begin);
                            writer.Write((uint)writer.BaseStream.Length);
                            writer.BaseStream.Seek(0, SeekOrigin.End);

                            funcOffsList.Add((uint)writer.BaseStream.Length);
                            writer.Write(padding);
                            writer.Write(Classes[i].Functions[v].Hash);
                            if (Version[0] >= 7)
                            {
                                writer.Write(Classes[i].Functions[v].Arguments);
                                writer.Write(Classes[i].Functions[v].Registers);
                            }
                            if (Version[0] >= 2 || Version[1] >= 1) //Only 2.x and 1.1.x use function flags
                            {
                                writer.Write((uint)writer.BaseStream.Position + 8);
                                writer.Write(Classes[i].Functions[v].Flags);
                            }
                            else
                                writer.Write((uint)writer.BaseStream.Position + 4);
                            for (int f = 0; f < Classes[i].Functions[v].Instructions.Count; f++)
                                Classes[i].Functions[v].Instructions[f].Write(writer);
                        }
                    }

                    bool writeConstants = true;
                    if (Version[0] >= 7)
                        writeConstants = Classes[i].Constants.Count > 0;

                    List<uint> cOffs = new List<uint>();
                    if (writeConstants)
                    {
                        writer.BaseStream.Seek(cl + 0x10, SeekOrigin.Begin);
                        writer.Write((uint)writer.BaseStream.Length);
                        writer.BaseStream.Seek(0, SeekOrigin.End);

                        writer.Write(Classes[i].Constants.Count);
                        uint constListOffs = (uint)writer.BaseStream.Position;
                        for (int v = 0; v < Classes[i].Constants.Count; v++)
                            writer.Write((uint)(constListOffs + (Classes[i].Constants.Count * 4) + (v * 8)));
                        for (int v = 0; v < Classes[i].Constants.Count; v++)
                        {
                            cOffs.Add((uint)writer.BaseStream.Position);
                            writer.Write(padding);
                            writer.Write(Classes[i].Constants[v].Value);
                        }
                    }

                    bool writeClassImpl = false;
                    if (Version[0] >= 2 || Version[1] >= 1)
                        writeClassImpl = true;
                    else if (Version[0] >= 7)
                        writeClassImpl = Classes[i].ClassImpl.Count > 0;

                    if (writeClassImpl)
                    {
                        writer.BaseStream.Seek(cl + 0x14, SeekOrigin.Begin);
                        writer.Write((uint)writer.BaseStream.Length);
                        writer.BaseStream.Seek(0, SeekOrigin.End);

                        writer.Write(Classes[i].ClassImpl.Count);
                        for (int v = 0; v < Classes[i].ClassImpl.Count; v++)
                            writer.Write(Classes[i].ClassImpl[v]);

                        WriteUtil.WritePadding(writer);
                    }

                    bool writeExtends = false;
                    if (Version[0] >= 7)
                        writeExtends = Classes[i].Extends.Count > 0;

                    if (writeExtends)
                    {
                        writer.BaseStream.Seek(cl + 0x18, SeekOrigin.Begin);
                        writer.Write((uint)writer.BaseStream.Length);
                        writer.BaseStream.Seek(0, SeekOrigin.End);

                        writer.Write(Classes[i].Extends.Count);
                        for (int v = 0; v < Classes[i].Extends.Count; v++)
                        {
                            writer.Write(Classes[i].Extends[v].StdType ? (byte)0x6B : (byte)0x6A);
                            writer.Write((byte)0xFF);
                            writer.Write(Classes[i].Extends[v].Index);
                        }
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

                    if (Version[0] < 7 || Classes[i].Variables.Count > 0)
                    {
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
                    }

                    if (Version[0] < 7 || Classes[i].Functions.Count > 0)
                    {
                        for (int v = 0; v < Classes[i].Functions.Count; v++)
                        {
                            writer.BaseStream.Seek(funcNameOffs[i][v], SeekOrigin.Begin);
                            writer.Write((uint)writer.BaseStream.Length);
                            writer.BaseStream.Seek(0, SeekOrigin.End);
                            WriteUtil.WriteString(writer, Classes[i].Functions[v].Name);
                        }
                    }

                    if (Version[0] < 7 || Classes[i].Constants.Count > 0)
                    {
                        for (int v = 0; v < Classes[i].Constants.Count; v++)
                        {
                            writer.BaseStream.Seek(constNameOffs[i][v], SeekOrigin.Begin);
                            writer.Write((uint)writer.BaseStream.Length);
                            writer.BaseStream.Seek(0, SeekOrigin.End);
                            WriteUtil.WriteString(writer, Classes[i].Constants[v].Name);
                        }
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

                for (int i = 0; i < Classes[c].ClassImpl.Count; i++)
                {
                    byte[] h = XRef[Classes[c].ClassImpl[i]];
                    if (hashes.ContainsKey(h))
                        text.Add($"\t\timplements {hashes[h]}");
                    else
                        text.Add($"\t\timplements {h[0]:X2}{h[1]:X2}{h[2]:X2}{h[3]:X2}");
                }

                if (Classes[c].ClassImpl.Count > 0)
                    text.Add("\t\t");

                for (int i = 0; i < Classes[c].Extends.Count; i++)
                {
                    ushort idx = Classes[c].Extends[i].Index;
                    if (Classes[c].Extends[i].StdType)
                    {
                        if (MintClass.StdTypes.ContainsKey(idx))
                            text.Add($"\t\textendstd {MintClass.StdTypes[idx]}");
                        else
                            text.Add($"\t\textendstd {idx}");
                    }
                    else
                    {
                        byte[] h = XRef[idx];
                        if (hashes.ContainsKey(h))
                            text.Add($"\t\textends {hashes[h]}");
                        else
                            text.Add($"\t\textends {h[0]:X2}{h[1]:X2}{h[2]:X2}{h[3]:X2}");
                    }
                }

                if (Classes[c].Extends.Count > 0)
                    text.Add("\t\t");

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

        public int AddXRef(string xref)
        {
            byte[] hash = new byte[4];

            if (xref.Length == 8)
            {
                if (uint.TryParse(xref, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out uint h))
                {
                    hash = new byte[]
                    {
                        byte.Parse(xref.Substring(0, 2), NumberStyles.HexNumber),
                        byte.Parse(xref.Substring(2, 2), NumberStyles.HexNumber),
                        byte.Parse(xref.Substring(4, 2), NumberStyles.HexNumber),
                        byte.Parse(xref.Substring(6, 2), NumberStyles.HexNumber)
                    };
                }
            }
            else
            {
                if (xref.StartsWith("this.") || xref == "this")
                    xref = Name + xref.Remove(0, 4);
                hash = HashCalculator.Calculate(xref);
            }

            return AddXRef(hash);
        }

        public int AddXRef(byte[] xref)
        {
            for (int i = 0; i < XRef.Count; i++)
                if (XRef[i].SequenceEqual(xref))
                    return i;

            XRef.Add(xref);
            return XRef.Count - 1;
        }
    }
}
