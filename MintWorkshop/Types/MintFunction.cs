﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MintWorkshop.Mint;
using MintWorkshop.Util;

namespace MintWorkshop.Types
{
    public class MintFunction
    {
        public MintClass ParentClass { get; private set; }

        public string Name { get; private set; }
        public byte[] Hash { get; private set; }
        public List<Instruction> Instructions { get; private set; }
        public uint Flags { get; set; }

        public MintFunction(string name, uint flags, MintClass parent)
        {
            ParentClass = parent;
            SetName(name);
            Instructions = new List<Instruction>();
            Flags = flags;
        }

        public MintFunction(EndianBinaryReader reader, MintClass parent)
        {
            ParentClass = parent;

            uint nameOffs = reader.ReadUInt32();
            Hash = reader.ReadBytes(4);
            uint dataOffs = reader.ReadUInt32();
            Flags = reader.ReadUInt32();

            reader.BaseStream.Seek(nameOffs, SeekOrigin.Begin);
            Name = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));

            reader.BaseStream.Seek(dataOffs, SeekOrigin.Begin);
            Instructions = new List<Instruction>();
            Opcode[] opcodes = MintVersions.Versions[ParentClass.ParentScript.Version];
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                Instruction i = new Instruction(reader.ReadBytes(4));
                Instructions.Add(i);

                // Detect fleave and fret instructions for stopping execution
                // Mainly for future-proofing, all fleave and fret instructions use only argument Y
                if ((i.Z == 0xFF && i.X == 0xFF && i.Y != 0xFF) || opcodes[i.Opcode].Action.HasFlag(Mint.Action.Return))
                {
                    //Console.WriteLine($"Finished reading function {ParentClass.Name}.{NameWithoutType()} at instruction {i.Opcode:X2} {i.Z:X2} {i.X:X2} {i.Y:X2}");
                    break;
                }
            }
        }

        //This is completely unused but I'm keeping it here just because it's nice to have
        public string Disassemble(ref Dictionary<byte[], string> hashes)
        {
            byte[] version = ParentClass.ParentScript.Version;
            if (!MintVersions.Versions.ContainsKey(version))
                return "Unimplemented Mint version";

            byte[] Sdata = ParentClass.ParentScript.SData.ToArray();
            List<byte[]> Xref = ParentClass.ParentScript.XRef;
            Opcode[] opcodes = MintVersions.Versions[version];
            string disasm = "";

            Dictionary<int, string> jmpLoc = new Dictionary<int, string>();
            for (int i = 0; i < Instructions.Count; i++)
            {
                Instruction inst = Instructions[i];
                if (opcodes.Length - 1 < inst.Opcode) continue;
                Opcode op = opcodes[inst.Opcode];
                if (op.Action.HasFlag(Mint.Action.Jump) && !jmpLoc.ContainsKey(i + inst.V()))
                    jmpLoc.Add(i + inst.V(), $"loc_{i + inst.V():x8}");
            }

            for (int i = 0; i < Instructions.Count; i++)
            {
                Instruction inst = Instructions[i];
                if (opcodes.Length - 1 < inst.Opcode)
                {
                    disasm += $"{inst.Opcode:X2}{inst.Z:X2}{inst.X:X2}{inst.Y:X2} Unknown opcode!\n";
                    continue;
                }
                
                if (jmpLoc.ContainsKey(i))
                    disasm += $"\n{jmpLoc[i]}:\n";

                Opcode op = opcodes[inst.Opcode];
                disasm += op.Name + " ";
                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    switch (op.Arguments[a])
                    {
                        case "z":
                            {
                                disasm += inst.Z;
                                break;
                            }
                        case "x":
                            {
                                disasm += inst.X;
                                break;
                            }
                        case "y":
                            {
                                disasm += inst.Y;
                                break;
                            }
                        case "rz":
                            {
                                disasm += $"r{inst.Z}";
                                break;
                            }
                        case "rx":
                            {
                                disasm += $"r{inst.X}";
                                break;
                            }
                        case "ry":
                            {
                                disasm += $"r{inst.Y}";
                                break;
                            }
                        case "v":
                            {
                                if (op.Action.HasFlag(Mint.Action.Jump))
                                    disasm += jmpLoc[i + inst.V()];
                                else
                                    disasm += inst.V();
                                break;
                            }
                        case "sz":
                            {
                                if ((inst.Z & 0x80) != 0)
                                {
                                    disasm += $"0x{BitConverter.ToUInt32(Sdata, (inst.Z ^ 0x80) * 4):X}";
                                }
                                else
                                {
                                    disasm += $"r{inst.Z}";
                                }
                                break;
                            }
                        case "sx":
                            {
                                if ((inst.X & 0x80) != 0)
                                {
                                    disasm += $"0x{BitConverter.ToUInt32(Sdata, (inst.X ^ 0x80) * 4):X}";
                                }
                                else
                                {
                                    disasm += $"r{inst.X}";
                                }
                                break;
                            }
                        case "sy":
                            {
                                if ((inst.Y & 0x80) != 0)
                                {
                                    disasm += $"0x{BitConverter.ToUInt32(Sdata, (inst.Y ^ 0x80) * 4):X}";
                                }
                                else
                                {
                                    disasm += $"r{inst.Y}";
                                }
                                break;
                            }
                        case "vint":
                            {
                                disasm += $"0x{BitConverter.ToUInt32(Sdata, (ushort)inst.V()):X}";
                                break;
                            }
                        case "strz":
                            {
                                if ((inst.Z & 0x80) != 0)
                                {
                                    List<byte> b = new List<byte>();
                                    for (int s = (inst.Z ^ 0x80) * 4; s < Sdata.Length; s++)
                                    {
                                        if (Sdata[s] == 0) break;
                                        b.Add(Sdata[s]);
                                    }
                                    disasm += $"\"{Encoding.UTF8.GetString(b.ToArray())}\"";
                                }
                                else
                                {
                                    disasm += $"r{inst.Z}";
                                }
                                break;
                            }
                        case "strx":
                            {
                                if ((inst.X & 0x80) != 0)
                                {
                                    List<byte> b = new List<byte>();
                                    for (int s = (inst.X ^ 0x80) * 4; s < Sdata.Length; s++)
                                    {
                                        if (Sdata[s] == 0) break;
                                        b.Add(Sdata[s]);
                                    }
                                    disasm += $"\"{Encoding.UTF8.GetString(b.ToArray())}\"";
                                }
                                else
                                {
                                    disasm += $"r{inst.X}";
                                }
                                break;
                            }
                        case "stry":
                            {
                                if ((inst.Y & 0x80) != 0)
                                {
                                    List<byte> b = new List<byte>();
                                    for (int s = (inst.Y ^ 0x80) * 4; s < Sdata.Length; s++)
                                    {
                                        if (Sdata[s] == 0) break;
                                        b.Add(Sdata[s]);
                                    }
                                    disasm += $"\"{Encoding.UTF8.GetString(b.ToArray())}\"";
                                }
                                else
                                {
                                    disasm += $"r{inst.Y}";
                                }
                                break;
                            }
                        case "vstr":
                            {
                                List<byte> b = new List<byte>();
                                for (uint s = (ushort)inst.V(); s < Sdata.Length; s++)
                                {
                                    if (Sdata[s] == 0) break;
                                    b.Add(Sdata[s]);
                                }
                                disasm += $"\"{Encoding.UTF8.GetString(b.ToArray())}\"";
                                break;
                            }
                        case "vxref":
                            {
                                byte[] h = Xref[(ushort)inst.V()];
                                if (hashes.ContainsKey(h))
                                    disasm += hashes[h];
                                else
                                    disasm += $"{h[0]:X2}{h[1]:X2}{h[2]:X2}{h[3]:X2}";
                                break;
                            }
                        case "zxref":
                            {
                                byte[] h = Xref[inst.Z];
                                if (hashes.ContainsKey(h))
                                    disasm += hashes[h];
                                else
                                    disasm += $"{h[0]:X2}{h[1]:X2}{h[2]:X2}{h[3]:X2}";
                                break;
                            }
                        case "xxref":
                            {
                                byte[] h = Xref[inst.X];
                                if (hashes.ContainsKey(h))
                                    disasm += hashes[h];
                                else
                                    disasm += $"{h[0]:X2}{h[1]:X2}{h[2]:X2}{h[3]:X2}";
                                break;
                            }
                        case "yxref":
                            {
                                byte[] h = Xref[inst.Y];
                                if (hashes.ContainsKey(h))
                                    disasm += hashes[h];
                                else
                                    disasm += $"{h[0]:X2}{h[1]:X2}{h[2]:X2}{h[3]:X2}";
                                break;
                            }
                    }

                    if (a < op.Arguments.Length - 1)
                        disasm += ", ";
                }

                disasm += "\n";
            }

            return disasm;
        }

        public void Disassemble(ref Dictionary<byte[], string> hashes, ref RichTextBox textBox, bool writeBytes = false)
        {
            byte[] version = ParentClass.ParentScript.Version;
            if (!MintVersions.Versions.ContainsKey(version))
            {
                textBox.AppendText("Unknown Mint version!", Color.Red);
                return;
            }

            byte[] Sdata = ParentClass.ParentScript.SData.ToArray();
            List<byte[]> Xref = ParentClass.ParentScript.XRef;
            Opcode[] opcodes = MintVersions.Versions[version];

            Dictionary<int, string> jmpLoc = new Dictionary<int, string>();
            for (int i = 0; i < Instructions.Count; i++)
            {
                Instruction inst = Instructions[i];
                if (opcodes.Length - 1 < inst.Opcode) continue;
                Opcode op = opcodes[inst.Opcode];
                if (op.Action.HasFlag(Mint.Action.Jump) && !jmpLoc.ContainsKey(i + inst.V()))
                {
                    if (opcodes[Instructions[i + inst.V()].Opcode].Action.HasFlag(Mint.Action.Return))
                        jmpLoc.Add(i + inst.V(), "return");
                    else
                        jmpLoc.Add(i + inst.V(), $"loc_{i + inst.V():x8}");
                }
            }

            for (int i = 0; i < Instructions.Count; i++)
            {
                Instruction inst = Instructions[i];
                if (opcodes.Length - 1 < inst.Opcode)
                {
                    textBox.AppendText($"{inst.Opcode:X2}{inst.Z:X2}{inst.X:X2}{inst.Y:X2} Unimplemented opcode!", Color.White, Color.Red);
                    textBox.AppendText("\n");
                    continue;
                }

                if (jmpLoc.ContainsKey(i))
                {
                    textBox.AppendText("\n");
                    textBox.AppendText($"{jmpLoc[i]}:", TextColors.JumpLocColor);
                    textBox.AppendText("\n");
                }

                Opcode op = opcodes[inst.Opcode];

                if (writeBytes)
                {
                    textBox.AppendText($" {inst.Opcode} {inst.Z} {inst.X} {inst.Y} ", TextColors.ByteColor);
                }

                textBox.AppendText(op.Name, TextColors.MneumonicColor);
                for (int s = op.Name.Length; s < 7; s++)
                    textBox.AppendText(" ");
                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    switch (op.Arguments[a])
                    {
                        case "z":
                            {
                                textBox.AppendText($"{inst.Z}", TextColors.ConstantColor);
                                break;
                            }
                        case "x":
                            {
                                textBox.AppendText($"{inst.X}", TextColors.ConstantColor);
                                break;
                            }
                        case "y":
                            {
                                textBox.AppendText($"{inst.Y}", TextColors.ConstantColor);
                                break;
                            }
                        case "rz":
                            {
                                textBox.AppendText($"r{inst.Z}", TextColors.RegisterColor);
                                break;
                            }
                        case "rx":
                            {
                                textBox.AppendText($"r{inst.X}", TextColors.RegisterColor);
                                break;
                            }
                        case "ry":
                            {
                                textBox.AppendText($"r{inst.Y}", TextColors.RegisterColor);
                                break;
                            }
                        case "v":
                            {
                                if (op.Action.HasFlag(Mint.Action.Jump))
                                    textBox.AppendText(jmpLoc[i + inst.V()], TextColors.JumpLocColor);
                                else
                                    textBox.AppendText($"{inst.V()}", TextColors.ConstantColor);
                                break;
                            }
                        case "sz":
                            {
                                if ((inst.Z & 0x80) != 0)
                                {
                                    textBox.AppendText($"0x{BitConverter.ToUInt32(Sdata, (inst.Z ^ 0x80) * 4):X}", TextColors.ConstantColor);
                                }
                                else
                                {
                                    textBox.AppendText($"r{inst.Z}", TextColors.RegisterColor);
                                }
                                break;
                            }
                        case "sx":
                            {
                                if ((inst.X & 0x80) != 0)
                                {
                                    textBox.AppendText($"0x{BitConverter.ToUInt32(Sdata, (inst.X ^ 0x80) * 4):X}", TextColors.ConstantColor);
                                }
                                else
                                {
                                    textBox.AppendText($"r{inst.X}", TextColors.RegisterColor);
                                }
                                break;
                            }
                        case "sy":
                            {
                                if ((inst.Y & 0x80) != 0)
                                {
                                    textBox.AppendText($"0x{BitConverter.ToUInt32(Sdata, (inst.Y ^ 0x80) * 4):X}", TextColors.ConstantColor);
                                }
                                else
                                {
                                    textBox.AppendText($"r{inst.Y}", TextColors.RegisterColor);
                                }
                                break;
                            }
                        case "vint":
                            {
                                textBox.AppendText($"0x{BitConverter.ToUInt32(Sdata, (ushort)inst.V()):X}", TextColors.ConstantColor);
                                break;
                            }
                        case "strz":
                            {
                                if ((inst.Z & 0x80) != 0)
                                {
                                    List<byte> b = new List<byte>();
                                    for (int s = (inst.Z ^ 0x80) * 4; s < Sdata.Length; s++)
                                    {
                                        if (Sdata[s] == 0) break;
                                        b.Add(Sdata[s]);
                                    }
                                    textBox.AppendText($"\"{Encoding.UTF8.GetString(b.ToArray())}\"", TextColors.StringColor);
                                }
                                else
                                {
                                    textBox.AppendText($"r{inst.Z}", TextColors.RegisterColor);
                                }
                                break;
                            }
                        case "strx":
                            {
                                if ((inst.X & 0x80) != 0)
                                {
                                    List<byte> b = new List<byte>();
                                    for (int s = (inst.X ^ 0x80) * 4; s < Sdata.Length; s++)
                                    {
                                        if (Sdata[s] == 0) break;
                                        b.Add(Sdata[s]);
                                    }
                                    textBox.AppendText($"\"{Encoding.UTF8.GetString(b.ToArray())}\"", TextColors.StringColor);
                                }
                                else
                                {
                                    textBox.AppendText($"r{inst.X}", TextColors.RegisterColor);
                                }
                                break;
                            }
                        case "stry":
                            {
                                if ((inst.Y & 0x80) != 0)
                                {
                                    List<byte> b = new List<byte>();
                                    for (int s = (inst.Y ^ 0x80) * 4; s < Sdata.Length; s++)
                                    {
                                        if (Sdata[s] == 0) break;
                                        b.Add(Sdata[s]);
                                    }
                                    textBox.AppendText($"\"{Encoding.UTF8.GetString(b.ToArray())}\"", TextColors.StringColor);
                                }
                                else
                                {
                                    textBox.AppendText($"r{inst.Y}", TextColors.RegisterColor);
                                }
                                break;
                            }
                        case "vstr":
                            {
                                List<byte> b = new List<byte>();
                                for (uint s = (ushort)inst.V(); s < Sdata.Length; s++)
                                {
                                    if (Sdata[s] == 0) break;
                                    b.Add(Sdata[s]);
                                }
                                textBox.AppendText($"\"{Encoding.UTF8.GetString(b.ToArray())}\"", TextColors.StringColor);
                                break;
                            }
                        case "vxref":
                            {
                                byte[] h = Xref[(ushort)inst.V()];
                                if (hashes.ContainsKey(h))
                                    textBox.AppendText(hashes[h], TextColors.XRefColor);
                                else
                                    textBox.AppendText($"{h[0]:X2}{h[1]:X2}{h[2]:X2}{h[3]:X2}", TextColors.XRefColor);
                                break;
                            }
                        case "zxref":
                            {
                                byte[] h = Xref[inst.Z];
                                if (hashes.ContainsKey(h))
                                    textBox.AppendText(hashes[h], TextColors.XRefColor);
                                else
                                    textBox.AppendText($"{h[0]:X2}{h[1]:X2}{h[2]:X2}{h[3]:X2}", TextColors.XRefColor);
                                break;
                            }
                        case "xxref":
                            {
                                byte[] h = Xref[inst.X];
                                if (hashes.ContainsKey(h))
                                    textBox.AppendText(hashes[h], TextColors.XRefColor);
                                else
                                    textBox.AppendText($"{h[0]:X2}{h[1]:X2}{h[2]:X2}{h[3]:X2}", TextColors.XRefColor);
                                break;
                            }
                        case "yxref":
                            {
                                byte[] h = Xref[inst.Y];
                                if (hashes.ContainsKey(h))
                                    textBox.AppendText(hashes[h], TextColors.XRefColor);
                                else
                                    textBox.AppendText($"{h[0]:X2}{h[1]:X2}{h[2]:X2}{h[3]:X2}", TextColors.XRefColor);
                                break;
                            }
                        default:
                            {
                                textBox.AppendText($"Unknown argument \"{op.Arguments[a]}\"", Color.White, Color.Red);
                                break;
                            }
                    }

                    if (a < op.Arguments.Length - 1)
                        textBox.AppendText(", ");
                }

                textBox.AppendText("\n");
            }
        }

        public void Assemble(string[] text)
        {
            byte[] version = ParentClass.ParentScript.Version;
            List<string> lines = text.ToList();
            List<Instruction> instructions = new List<Instruction>();
            Opcode[] opcodes = MintVersions.Versions[version];

            List<byte> sdata = ParentClass.ParentScript.SData;
            List<byte[]> xref = ParentClass.ParentScript.XRef;

            if (lines[0].StartsWith("[Flags:"))
            {
                string flag = string.Join("", lines[0].Skip(7).TakeWhile(x => x != ']'));
                if (uint.TryParse(flag, out uint f))
                    Flags = f;
                else
                {
                    MessageBox.Show($"Error: Could not parse flags.", "Mint Assembler", MessageBoxButtons.OK);
                    return;
                }
                Name = string.Join("", lines[0].Skip(8 + flag.Length).SkipWhile(x => x != ' ').Skip(1));

                lines.RemoveAt(0);
            }

            //Remove any excess spaces and empty lines
            for (int i = 0; i < lines.Count; i++)
            {
                for (int c = 0; c < lines[i].Length - 1; c++)
                {
                    if (lines[i][c] == ' ' && lines[i][c+1] == ' ')
                    {
                        lines[i] = lines[i].Remove(c, 1);
                        c--;
                    }
                }
                if (lines[i] == "")
                {
                    lines.RemoveAt(i);
                    i--;
                }
            }

            //Get jump locations
            Dictionary<string, int> locs = new Dictionary<string, int>();
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].EndsWith(":"))
                {
                    locs.Add(lines[i].Remove(lines[i].Length - 1, 1), i);
                    lines.RemoveAt(i);
                    i--;
                }
            }

            //Sdata Integers
            for (int i = 0; i < lines.Count; i++)
            {
                string[] line = lines[i].Split(' ');
                Opcode op = opcodes.Where(x => x.Name == line[0]).FirstOrDefault();
                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    if (op.Arguments[a] == "vint" || op.Arguments[a].StartsWith("s"))
                    {
                        string arg = line[a + 1].TrimEnd(',');
                        byte[] b;

                        if (arg.StartsWith("r"))
                        {
                            if (op.Arguments[a].StartsWith("s")) continue;
                            else
                            {
                                MessageBox.Show($"Error: Expected integer, got register.\nArgument: {arg}\nLine: {lines[i]}", "Mint Assembler", MessageBoxButtons.OK);
                                return;
                            }
                        }

                        if (arg.StartsWith("0x"))
                        {
                            if (int.TryParse(arg.Remove(0,2), NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out int val))
                                b = BitConverter.GetBytes(val);
                            else
                            {
                                MessageBox.Show($"Error: Could not parse integer.\nArgument: {arg}\nLine: {lines[i]}", "Mint Assembler", MessageBoxButtons.OK);
                                return;
                            }
                        }
                        else if (arg.Contains('.') || arg.EndsWith("f"))
                        {
                            if (float.TryParse(arg.Replace("f",""), out float val))
                                b = BitConverter.GetBytes(val);
                            else
                            {
                                MessageBox.Show($"Error: Could not parse float.\nArgument: {arg}\nLine: {lines[i]}", "Mint Assembler", MessageBoxButtons.OK);
                                return;
                            }
                        }
                        else
                        {
                            if (int.TryParse(arg, out int val))
                                b = BitConverter.GetBytes(val);
                            else
                            {
                                MessageBox.Show($"Error: Could not parse integer.\nArgument: {arg}\nLine: {lines[i]}", "Mint Assembler", MessageBoxButtons.OK);
                                return;
                            }
                        }

                        int sIndex = sdata.Count;
                        for (int s = 0; s < sdata.Count; s += 4)
                        {
                            if (ByteArrayComparer.Equal(sdata.Skip(s).Take(4).ToArray(), b))
                            {
                                sIndex = s;
                                break;
                            }
                        }

                        if (sIndex == sdata.Count)
                            sdata.AddRange(b);

                        int buildVal = sIndex;
                        if (op.Arguments[a].StartsWith("s"))
                        {
                            buildVal /= 4;
                            buildVal ^= 0x80;
                        }

                        line[a + 1] = buildVal.ToString();
                        if (a < op.Arguments.Length - 1) line[a + 1] = line[a + 1] + ",";

                        lines[i] = string.Join(" ", line);
                    }
                }
            }

            //Sdata Strings
            for (int i = 0; i < lines.Count; i++)
            {
                string[] line = lines[i].Split(' ');
                Opcode op = opcodes.Where(x => x.Name == line[0]).FirstOrDefault();
                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    if (op.Arguments[a] == "vstr" || op.Arguments[a].StartsWith("str"))
                    {
                        string arg = line[a + 1];
                        List<byte> b = new List<byte>();

                        if (arg.StartsWith("r"))
                        {
                            if (op.Arguments[a].StartsWith("str")) continue;
                            else
                            {
                                MessageBox.Show($"Error: Expected string, got register.\nArgument: {arg}\nLine: {lines[i]}", "Mint Assembler", MessageBoxButtons.OK);
                                return;
                            }
                        }

                        int strEndIndex = a;
                        string str = string.Join(" ", line.Skip(a).TakeWhile(x => !x.EndsWith("\"") && !x.EndsWith("\",")));
                        if (!str.EndsWith("\"") && !str.EndsWith("\","))
                        {
                            MessageBox.Show($"Error: String has no ending:\nArgument: {str}\nLine: {lines[i]}", "Mint Assembler", MessageBoxButtons.OK);
                            return;
                        }
                        b = Encoding.UTF8.GetBytes(str.TrimStart('\"').TrimEnd('\"')).ToList();
                        b.Add(0);
                        while ((b.Count & 0xF) != 0x0
                            && (b.Count & 0xF) != 0x4
                            && (b.Count & 0xF) != 0x8
                            && (b.Count & 0xF) != 0xC)
                            b.Add(0xFF);

                        int sIndex = sdata.Count;
                        for (int s = 0; s < sdata.Count; s += 4)
                        {
                            if (ByteArrayComparer.Equal(sdata.Skip(s).Take(b.Count).ToArray(), b.ToArray()))
                            {
                                sIndex = s;
                                break;
                            }
                        }

                        if (sIndex == sdata.Count)
                            sdata.AddRange(b);

                        int buildVal = sIndex;
                        if (op.Arguments[a].StartsWith("str"))
                        {
                            buildVal /= 4;
                            buildVal ^= 0x80;
                        }

                        line[a + 1] = buildVal.ToString();
                        for (int s = a + 1; s < strEndIndex + 1; s++)
                            line[s] = "";

                        if (a < op.Arguments.Length - 1) line[a + 1] = line[a + 1] + ",";

                        lines[i] = string.Join(" ", line);
                    }
                }
            }

            //Xref
            for (int i = 0; i < lines.Count; i++)
            {
                string[] line = lines[i].Split(' ');
                Opcode op = opcodes.Where(x => x.Name == line[0]).FirstOrDefault();
                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    if (op.Arguments[a].EndsWith("xref"))
                    {
                        string arg = line[a + 1];
                        byte[] b = new byte[4];

                        if (arg.StartsWith("r"))
                        {
                            if (!op.Arguments[a].StartsWith("v")) continue;
                            else
                            {
                                MessageBox.Show($"Error: Expected XRef, got register.\nArgument: {arg}\nLine: {lines[i]}", "Mint Assembler", MessageBoxButtons.OK);
                                return;
                            }
                        }

                        if (arg.Length == 8)
                        {
                            if (!uint.TryParse(arg, NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out uint h))
                            {
                                MessageBox.Show($"Error: Could not parse XRef hash.\nArgument: {arg}\nLine: {lines[i]}", "Mint Assembler", MessageBoxButtons.OK);
                                return;
                            }
                            else
                            {
                                b = new byte[] { byte.Parse(string.Join("", arg.Take(2)), NumberStyles.HexNumber),
                                                 byte.Parse(string.Join("", arg.Skip(2).Take(2)), NumberStyles.HexNumber),
                                                 byte.Parse(string.Join("", arg.Skip(4).Take(2)), NumberStyles.HexNumber),
                                                 byte.Parse(string.Join("", arg.Skip(6).Take(2)), NumberStyles.HexNumber)
                                };
                            }
                        }
                        else
                        {
                            string xstring = string.Join(" ", line.Skip(a + 1).TakeWhile(x => x.IndexOf(x) < line.Length));
                            b = HashCalculator.Calculate(xstring);
                        }

                        int xIndex = xref.Count;
                        for (int s = 0; s < xref.Count; s++)
                        {
                            if (ByteArrayComparer.Equal(xref[s], b))
                            {
                                //Console.WriteLine($"xref[{s}]: {xref[s][0]:X2}{xref[s][1]:X2}{xref[s][2]:X2}{xref[s][3]:X2}, b: {b[0]:X2}{b[1]:X2}{b[2]:X2}{b[3]:X2}");
                                xIndex = s;
                                break;
                            }
                        }

                        if (xIndex == xref.Count)
                            xref.Add(b);

                        int buildVal = xIndex;

                        line[a + 1] = buildVal.ToString();
                        if (a < op.Arguments.Length - 1) line[a + 1] = line[a + 1] + ",";

                        lines[i] = string.Join(" ", line);
                    }
                }
            }

            //Jumps
            for (int i = 0; i < lines.Count; i++)
            {
                string[] line = lines[i].Split(' ');
                Opcode op = opcodes.Where(x => x.Name == line[0]).FirstOrDefault();
                if (op.Action.HasFlag(Mint.Action.Jump))
                {
                    for (int a = 0; a < op.Arguments.Length; a++)
                    {
                        if (op.Arguments[a] == "v")
                        {
                            string arg = line[a + 1];

                            if (!locs.ContainsKey(arg))
                            {
                                MessageBox.Show($"Error: Jump label does not exist.\nArgument: {arg}\nLine: {lines[i]}", "Mint Assembler", MessageBoxButtons.OK);
                                return;
                            }

                            line[a + 1] = (locs[arg] - i).ToString();
                            if (a < op.Arguments.Length - 1) line[a + 1] = line[a + 1] + ",";
                            lines[i] = string.Join(" ", line);
                        }
                    }
                }
            }

            //Assembling instructions
            for (int i = 0; i < lines.Count; i++)
            {
                string[] line = lines[i].Replace(",", "").Split(' ');
                var o = opcodes.Where(x => x.Name == line[0]);
                if (o.Count() == 0) { Console.WriteLine($"oops: {line[0]}"); continue; }

                Opcode op = o.FirstOrDefault();
                Instruction inst = new Instruction(new byte[] { (byte)opcodes.ToList().IndexOf(op), 0xFF, 0xFF, 0xFF });
                //Console.WriteLine(lines[i]);
                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    switch (op.Arguments[a])
                    {
                        case "z":
                        case "rz":
                        case "sz":
                        case "strz":
                        case "zxref":
                            {
                                inst.Z = byte.Parse(line[a + 1].TrimStart('r'));
                                break;
                            }
                        case "x":
                        case "rx":
                        case "sx":
                        case "strx":
                        case "xxref":
                            {
                                inst.X = byte.Parse(line[a + 1].TrimStart('r'));
                                break;
                            }
                        case "y":
                        case "ry":
                        case "sy":
                        case "stry":
                        case "yxref":
                            {
                                inst.Y = byte.Parse(line[a + 1].TrimStart('r'));
                                break;
                            }
                        case "v":
                            {
                                byte[] v = BitConverter.GetBytes(short.Parse(line[a + 1]));
                                inst.X = v[0];
                                inst.Y = v[1];
                                break;
                            }
                        case "vint":
                        case "vstr":
                        case "vxref":
                            {
                                byte[] v = BitConverter.GetBytes(ushort.Parse(line[a + 1]));
                                inst.X = v[0];
                                inst.Y = v[1];
                                break;
                            }
                    }
                }
                instructions.Add(inst);
            }

            ParentClass.ParentScript.SData = sdata;
            ParentClass.ParentScript.XRef = xref;
            Instructions = instructions;
        }

        public void SetName(string name)
        {
            Name = name;
            Hash = HashCalculator.Calculate(FullName());
        }

        public string FullName()
        {
            return ParentClass.Name + "." + NameWithoutType();
        }

        public string FullNameWithoutSignature()
        {
            return ParentClass.Name + "." + NameWithoutSignature();
        }

        public string NameWithoutType()
        {
            int i = 0;
            while (Name[i] != ' ') i++;
            return Name.Remove(0, i + 1);
        }

        public string NameWithoutSignature()
        {
            int i = 0;
            while (Name[i] != ' ') i++;
            int p = 0;
            while (Name[p] != '(') p++;
            return Name.Remove(p, Name.Length - p).Remove(0, i + 1);
        }
    }
}
