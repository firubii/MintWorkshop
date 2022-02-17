using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            byte[] v = ParentClass.ParentScript.Version;
            Instructions = new List<Instruction>() { 
                new Instruction(new byte[] { (byte)MintVersions.Versions[v].ToList().FindIndex(x => x.Name == "fenter"),1,0,0 }),
                new Instruction(new byte[] { (byte)MintVersions.Versions[v].ToList().FindIndex(x => x.Name == "fleave"),0xff,0xff,0 })
            };
            Flags = flags;
        }

        public MintFunction(EndianBinaryReader reader, MintClass parent)
        {
            ParentClass = parent;

            uint nameOffs = reader.ReadUInt32();
            Hash = reader.ReadBytes(4);
            uint dataOffs = reader.ReadUInt32();
            if (ParentClass.ParentScript.Version[0] >= 2)
                Flags = reader.ReadUInt32();

            reader.BaseStream.Seek(nameOffs, SeekOrigin.Begin);
            Name = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));

            reader.BaseStream.Seek(dataOffs, SeekOrigin.Begin);
            Instructions = new List<Instruction>();
            Opcode[] opcodes = MintVersions.Versions[ParentClass.ParentScript.Version];
            bool hasReturn = opcodes.Select(x => x.Action.HasFlag(Mint.Action.Return)).Count() > 0;
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                Instruction i = new Instruction(reader.ReadBytes(4));
                Instructions.Add(i);

                if (hasReturn)
                {
                    if (opcodes[i.Opcode].Action.HasFlag(Mint.Action.Return))
                        break;
                }
                else
                {
                    // Detect fleave and fret instructions for stopping reading the function
                    // Mainly for future-proofing, all fleave and fret instructions use only argument Y
                    if (i.Z == 0xFF && i.X == 0xFF && i.Y != 0xFF)
                        break;
                }
            }
        }

        public string Disassemble(ref Dictionary<byte[], string> hashes, bool uppercase = false)
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
                int loc = i + inst.V(ParentClass.ParentScript.XData.Endianness);
                if (op.Action.HasFlag(Mint.Action.Jump) && !jmpLoc.ContainsKey(loc))
                {
                    if (opcodes[Instructions[loc].Opcode].Action.HasFlag(Mint.Action.Return))
                        jmpLoc.Add(loc, "return");
                    else
                        jmpLoc.Add(loc, $"loc_{loc:x8}");
                }
            }

            for (int i = 0; i < Instructions.Count; i++)
            {
                Instruction inst = Instructions[i];
                if (opcodes.Length - 1 < inst.Opcode)
                {
                    disasm += $"{inst.Opcode:X2}{inst.Z:X2}{inst.X:X2}{inst.Y:X2} Unimplemented opcode!";
                    disasm += "\n";
                    continue;
                }

                if (jmpLoc.ContainsKey(i))
                {
                    disasm += "\n";
                    disasm += $"{jmpLoc[i]}:";
                    disasm += "\n";
                }

                Opcode op = opcodes[inst.Opcode];

                if (uppercase)
                    disasm += op.Name.ToUpper();
                else
                    disasm += op.Name;

                for (int s = op.Name.Length; s < 7; s++)
                    disasm += " ";
                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    switch (op.Arguments[a])
                    {
                        case InstructionArg.Z:
                            {
                                disasm += inst.Z;
                                break;
                            }
                        case InstructionArg.X:
                            {
                                disasm += inst.X;
                                break;
                            }
                        case InstructionArg.Y:
                            {
                                disasm += inst.Y;
                                break;
                            }
                        case InstructionArg.RegZ:
                            {
                                disasm += $"r{inst.Z}";
                                break;
                            }
                        case InstructionArg.RegX:
                            {
                                disasm += $"r{inst.X}";
                                break;
                            }
                        case InstructionArg.RegY:
                            {
                                disasm += $"r{inst.Y}";
                                break;
                            }
                        case InstructionArg.VSigned:
                            {
                                if (op.Action.HasFlag(Mint.Action.Jump))
                                    disasm += jmpLoc[i + inst.V(ParentClass.ParentScript.XData.Endianness)];
                                else
                                    disasm += inst.V(ParentClass.ParentScript.XData.Endianness);
                                break;
                            }
                        case InstructionArg.IntRegZ:
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
                        case InstructionArg.IntRegX:
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
                        case InstructionArg.IntRegY:
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
                        case InstructionArg.IntV:
                            {
                                disasm += $"0x{BitConverter.ToUInt32(Sdata, (ushort)inst.V(ParentClass.ParentScript.XData.Endianness)):X}";
                                break;
                            }
                        case InstructionArg.ArrRegZ:
                            {
                                if ((inst.Z & 0x80) != 0)
                                {
                                    List<byte> b = new List<byte>();
                                    bool utf16 = false;
                                    for (int s = (inst.Z ^ 0x80) * 4; s < Sdata.Length; s++)
                                    {
                                        if (Sdata[s] == 0x00 && ((s & 0x1) == 0x1))
                                        {
                                            if (Sdata[s - 1] == 0x00)
                                            {
                                                b.RemoveAt(b.Count - 1);
                                                utf16 = true;
                                            }
                                            break;
                                        }
                                        else if (Sdata[s] == 0xFF)
                                        {
                                            if (Sdata[s - 1] == 0x00)
                                            {
                                                b.RemoveAt(b.Count - 1);
                                                if (Sdata[s - 2] == 0 && b.Count - 1 >= 0)
                                                {
                                                    b.RemoveAt(b.Count - 1);
                                                    utf16 = true;
                                                }
                                                break;
                                            }
                                        }

                                        b.Add(Sdata[s]);
                                    }
                                    if (!utf16)
                                        disasm += $"\"{Encoding.UTF8.GetString(b.ToArray()).TrimEnd('\0')}\"";
                                    else
                                        disasm += $"u\"{Encoding.Unicode.GetString(b.ToArray()).TrimEnd('\0')}\"";
                                }
                                else
                                {
                                    disasm += $"r{inst.Z}";
                                }
                                break;
                            }
                        case InstructionArg.ArrRegX:
                            {
                                if ((inst.X & 0x80) != 0)
                                {
                                    List<byte> b = new List<byte>();
                                    bool utf16 = false;
                                    for (int s = (inst.X ^ 0x80) * 4; s < Sdata.Length; s++)
                                    {
                                        if (Sdata[s] == 0x00 && ((s & 0x1) == 0x1))
                                        {
                                            if (Sdata[s - 1] == 0x00)
                                            {
                                                b.RemoveAt(b.Count - 1);
                                                utf16 = true;
                                            }
                                            break;
                                        }
                                        else if (Sdata[s] == 0xFF)
                                        {
                                            if (Sdata[s - 1] == 0x00)
                                            {
                                                b.RemoveAt(b.Count - 1);
                                                if (Sdata[s - 2] == 0 && b.Count - 1 >= 0)
                                                {
                                                    b.RemoveAt(b.Count - 1);
                                                    utf16 = true;
                                                }
                                                break;
                                            }
                                        }

                                        b.Add(Sdata[s]);
                                    }
                                    if (!utf16)
                                        disasm += $"\"{Encoding.UTF8.GetString(b.ToArray()).TrimEnd('\0')}\"";
                                    else
                                        disasm += $"u\"{Encoding.Unicode.GetString(b.ToArray()).TrimEnd('\0')}\"";
                                }
                                else
                                {
                                    disasm += $"r{inst.X}";
                                }
                                break;
                            }
                        case InstructionArg.ArrRegY:
                            {
                                if ((inst.Y & 0x80) != 0)
                                {
                                    List<byte> b = new List<byte>();
                                    bool utf16 = false;
                                    for (int s = (inst.Y ^ 0x80) * 4; s < Sdata.Length; s++)
                                    {
                                        if (Sdata[s] == 0x00 && ((s & 0x1) == 0x1))
                                        {
                                            if (Sdata[s - 1] == 0x00)
                                            {
                                                b.RemoveAt(b.Count - 1);
                                                utf16 = true;
                                            }
                                            break;
                                        }
                                        else if (Sdata[s] == 0xFF)
                                        {
                                            if (Sdata[s - 1] == 0x00)
                                            {
                                                b.RemoveAt(b.Count - 1);
                                                if (Sdata[s - 2] == 0 && b.Count - 1 >= 0)
                                                {
                                                    b.RemoveAt(b.Count - 1);
                                                    utf16 = true;
                                                }
                                                break;
                                            }
                                        }

                                        b.Add(Sdata[s]);
                                    }
                                    if (!utf16)
                                        disasm += $"\"{Encoding.UTF8.GetString(b.ToArray()).TrimEnd('\0')}\"";
                                    else
                                        disasm += $"u\"{Encoding.Unicode.GetString(b.ToArray()).TrimEnd('\0')}\"";
                                }
                                else
                                {
                                    disasm += $"r{inst.Y}";
                                }
                                break;
                            }
                        case InstructionArg.ArrV:
                            {
                                List<byte> b = new List<byte>();
                                bool utf16 = false;
                                for (uint s = (ushort)inst.V(ParentClass.ParentScript.XData.Endianness); s < Sdata.Length; s++)
                                {
                                    if (Sdata[s] == 0x00 && ((s & 0x1) == 0x1))
                                    {
                                        if (Sdata[s - 1] == 0x00)
                                        {
                                            b.RemoveAt(b.Count - 1);
                                            utf16 = true;
                                        }
                                        break;
                                    }
                                    else if (Sdata[s] == 0xFF)
                                    {
                                        if (Sdata[s - 1] == 0x00)
                                        {
                                            b.RemoveAt(b.Count - 1);
                                            if (Sdata[s - 2] == 0 && b.Count - 1 >= 0)
                                            {
                                                b.RemoveAt(b.Count - 1);
                                                utf16 = true;
                                            }
                                            break;
                                        }
                                    }

                                    b.Add(Sdata[s]);
                                }
                                if (!utf16)
                                    disasm += $"\"{Encoding.UTF8.GetString(b.ToArray()).TrimEnd('\0')}\"";
                                else
                                    disasm += $"u\"{Encoding.Unicode.GetString(b.ToArray()).TrimEnd('\0')}\"";
                                break;
                            }
                        case InstructionArg.XRefV:
                            {
                                byte[] h = Xref[(ushort)inst.V(ParentClass.ParentScript.XData.Endianness)];
                                if (hashes.ContainsKey(h))
                                {
                                    string x = hashes[h];
                                    if (x.StartsWith(ParentClass.Name + ".") || x == ParentClass.Name)
                                        x = "this" + x.Remove(0, ParentClass.Name.Length);

                                    disasm += x;
                                }
                                else
                                    disasm += $"{h[0]:X2}{h[1]:X2}{h[2]:X2}{h[3]:X2}";
                                break;
                            }
                        case InstructionArg.XRefZ:
                            {
                                byte[] h = Xref[inst.Z];
                                if (hashes.ContainsKey(h))
                                {
                                    string x = hashes[h];
                                    if (x.StartsWith(ParentClass.Name + ".") || x == ParentClass.Name)
                                        x = "this" + x.Remove(0, ParentClass.Name.Length);

                                    disasm += x;
                                }
                                else
                                    disasm += $"{h[0]:X2}{h[1]:X2}{h[2]:X2}{h[3]:X2}";
                                break;
                            }
                        case InstructionArg.XRefX:
                            {
                                byte[] h = Xref[inst.X];
                                if (hashes.ContainsKey(h))
                                {
                                    string x = hashes[h];
                                    if (x.StartsWith(ParentClass.Name + ".") || x == ParentClass.Name)
                                        x = "this" + x.Remove(0, ParentClass.Name.Length);

                                    disasm += x;
                                }
                                else
                                    disasm += $"{h[0]:X2}{h[1]:X2}{h[2]:X2}{h[3]:X2}";
                                break;
                            }
                        case InstructionArg.XRefY:
                            {
                                byte[] h = Xref[inst.Y];
                                if (hashes.ContainsKey(h))
                                {
                                    string x = hashes[h];
                                    if (x.StartsWith(ParentClass.Name + ".") || x == ParentClass.Name)
                                        x = "this" + x.Remove(0, ParentClass.Name.Length);

                                    disasm += x;
                                }
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

        public void Disassemble(ref Dictionary<byte[], string> hashes, ref RichTextBox textBox, bool uppercase = false)
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
                int loc = i + inst.V(ParentClass.ParentScript.XData.Endianness);
                if (op.Action.HasFlag(Mint.Action.Jump) && !jmpLoc.ContainsKey(loc))
                {
                    if (opcodes[Instructions[loc].Opcode].Action.HasFlag(Mint.Action.Return))
                        jmpLoc.Add(loc, "return");
                    else
                        jmpLoc.Add(loc, $"loc_{loc:x8}");
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

                if (uppercase)
                    textBox.AppendText(op.Name.ToUpper(), TextColors.MneumonicColor);
                else
                    textBox.AppendText(op.Name, TextColors.MneumonicColor);

                for (int s = op.Name.Length; s < 7; s++)
                    textBox.AppendText(" ");
                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    switch (op.Arguments[a])
                    {
                        case InstructionArg.Z:
                            {
                                textBox.AppendText($"{inst.Z}", TextColors.ConstantColor);
                                break;
                            }
                        case InstructionArg.X:
                            {
                                textBox.AppendText($"{inst.X}", TextColors.ConstantColor);
                                break;
                            }
                        case InstructionArg.Y:
                            {
                                textBox.AppendText($"{inst.Y}", TextColors.ConstantColor);
                                break;
                            }
                        case InstructionArg.RegZ:
                            {
                                textBox.AppendText($"r{inst.Z}", TextColors.RegisterColor);
                                break;
                            }
                        case InstructionArg.RegX:
                            {
                                textBox.AppendText($"r{inst.X}", TextColors.RegisterColor);
                                break;
                            }
                        case InstructionArg.RegY:
                            {
                                textBox.AppendText($"r{inst.Y}", TextColors.RegisterColor);
                                break;
                            }
                        case InstructionArg.VSigned:
                            {
                                if (op.Action.HasFlag(Mint.Action.Jump))
                                    textBox.AppendText(jmpLoc[i + inst.V(ParentClass.ParentScript.XData.Endianness)], TextColors.JumpLocColor);
                                else
                                    textBox.AppendText($"{inst.V(ParentClass.ParentScript.XData.Endianness)}", TextColors.ConstantColor);
                                break;
                            }
                        case InstructionArg.IntRegZ:
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
                        case InstructionArg.IntRegX:
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
                        case InstructionArg.IntRegY:
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
                        case InstructionArg.IntV:
                            {
                                textBox.AppendText($"0x{BitConverter.ToUInt32(Sdata, (ushort)inst.V(ParentClass.ParentScript.XData.Endianness)):X}", TextColors.ConstantColor);
                                break;
                            }
                        case InstructionArg.ArrRegZ:
                            {
                                if ((inst.Z & 0x80) != 0)
                                {
                                    List<byte> b = new List<byte>();
                                    bool utf16 = false;
                                    for (int s = (inst.Z ^ 0x80) * 4; s < Sdata.Length; s++)
                                    {
                                        if (Sdata[s] == 0x00 && ((s & 0x1) == 0x1))
                                        {
                                            if (Sdata[s - 1] == 0x00)
                                            {
                                                b.RemoveAt(b.Count - 1);
                                                utf16 = true;
                                            }
                                            break;
                                        }
                                        else if (Sdata[s] == 0xFF)
                                        {
                                            if (Sdata[s - 1] == 0x00)
                                            {
                                                b.RemoveAt(b.Count - 1);
                                                if (Sdata[s - 2] == 0 && b.Count - 1 >= 0)
                                                {
                                                    b.RemoveAt(b.Count - 1);
                                                    utf16 = true;
                                                }
                                                break;
                                            }
                                        }

                                        b.Add(Sdata[s]);
                                    }

                                    if (!utf16)
                                        textBox.AppendText($"\"{Encoding.UTF8.GetString(b.ToArray()).TrimEnd('\0')}\"", TextColors.StringColor);
                                    else
                                        textBox.AppendText($"u\"{Encoding.Unicode.GetString(b.ToArray()).TrimEnd('\0')}\"", TextColors.StringColor);
                                }
                                else
                                {
                                    textBox.AppendText($"r{inst.Z}", TextColors.RegisterColor);
                                }
                                break;
                            }
                        case InstructionArg.ArrRegX:
                            {
                                if ((inst.X & 0x80) != 0)
                                {
                                    List<byte> b = new List<byte>();
                                    bool utf16 = false;
                                    for (int s = (inst.X ^ 0x80) * 4; s < Sdata.Length; s++)
                                    {
                                        if (Sdata[s] == 0x00 && ((s & 0x1) == 0x1))
                                        {
                                            if (Sdata[s - 1] == 0x00)
                                            {
                                                b.RemoveAt(b.Count - 1);
                                                utf16 = true;
                                            }
                                            break;
                                        }
                                        else if (Sdata[s] == 0xFF)
                                        {
                                            if (Sdata[s - 1] == 0x00)
                                            {
                                                b.RemoveAt(b.Count - 1);
                                                if (Sdata[s - 2] == 0 && b.Count - 1 >= 0)
                                                {
                                                    b.RemoveAt(b.Count - 1);
                                                    utf16 = true;
                                                }
                                                break;
                                            }
                                        }

                                        b.Add(Sdata[s]);
                                    }

                                    if (!utf16)
                                        textBox.AppendText($"\"{Encoding.UTF8.GetString(b.ToArray()).TrimEnd('\0')}\"", TextColors.StringColor);
                                    else
                                        textBox.AppendText($"u\"{Encoding.Unicode.GetString(b.ToArray()).TrimEnd('\0')}\"", TextColors.StringColor);
                                }
                                else
                                {
                                    textBox.AppendText($"r{inst.X}", TextColors.RegisterColor);
                                }
                                break;
                            }
                        case InstructionArg.ArrRegY:
                            {
                                if ((inst.Y & 0x80) != 0)
                                {
                                    List<byte> b = new List<byte>();
                                    bool utf16 = false;
                                    for (int s = (inst.Y ^ 0x80) * 4; s < Sdata.Length; s++)
                                    {
                                        if (Sdata[s] == 0x00 && ((s & 0x1) == 0x1))
                                        {
                                            if (Sdata[s - 1] == 0x00)
                                            {
                                                b.RemoveAt(b.Count - 1);
                                                utf16 = true;
                                            }
                                            break;
                                        }
                                        else if (Sdata[s] == 0xFF)
                                        {
                                            if (Sdata[s - 1] == 0x00)
                                            {
                                                b.RemoveAt(b.Count - 1);
                                                if (Sdata[s - 2] == 0 && b.Count - 1 >= 0)
                                                {
                                                    b.RemoveAt(b.Count - 1);
                                                    utf16 = true;
                                                }
                                                break;
                                            }
                                        }

                                        b.Add(Sdata[s]);
                                    }

                                    if (!utf16)
                                        textBox.AppendText($"\"{Encoding.UTF8.GetString(b.ToArray()).TrimEnd('\0')}\"", TextColors.StringColor);
                                    else
                                        textBox.AppendText($"u\"{Encoding.Unicode.GetString(b.ToArray()).TrimEnd('\0')}\"", TextColors.StringColor);
                                }
                                else
                                {
                                    textBox.AppendText($"r{inst.Y}", TextColors.RegisterColor);
                                }
                                break;
                            }
                        case InstructionArg.ArrV:
                            {
                                List<byte> b = new List<byte>();
                                bool utf16 = false;
                                for (uint s = (ushort)inst.V(ParentClass.ParentScript.XData.Endianness); s < Sdata.Length; s++)
                                {
                                    if (Sdata[s] == 0x00 && ((s & 0x1) == 0x1))
                                    {
                                        if (Sdata[s - 1] == 0x00)
                                        {
                                            b.RemoveAt(b.Count - 1);
                                            utf16 = true;
                                        }
                                        break;
                                    }
                                    else if (Sdata[s] == 0xFF)
                                    {
                                        if (Sdata[s - 1] == 0x00)
                                        {
                                            b.RemoveAt(b.Count - 1);
                                            if (Sdata[s - 2] == 0 && b.Count - 1 >= 0) 
                                            {
                                                b.RemoveAt(b.Count - 1);
                                                utf16 = true;
                                            }
                                            break;
                                        }
                                    }

                                    b.Add(Sdata[s]);
                                }

                                if (!utf16)
                                    textBox.AppendText($"\"{Encoding.UTF8.GetString(b.ToArray()).TrimEnd('\0')}\"", TextColors.StringColor);
                                else
                                    textBox.AppendText($"u\"{Encoding.Unicode.GetString(b.ToArray()).TrimEnd('\0')}\"", TextColors.StringColor);
                                break;
                            }
                        case InstructionArg.XRefV:
                            {
                                byte[] h = Xref[(ushort)inst.V(ParentClass.ParentScript.XData.Endianness)];
                                if (hashes.ContainsKey(h))
                                {
                                    string x = hashes[h];
                                    if (x.StartsWith(ParentClass.Name + ".") || x == ParentClass.Name)
                                        x = "this" + x.Remove(0, ParentClass.Name.Length);

                                    textBox.AppendText(x, TextColors.XRefColor);
                                }
                                else
                                    textBox.AppendText($"{h[0]:X2}{h[1]:X2}{h[2]:X2}{h[3]:X2}", TextColors.XRefColor);
                                break;
                            }
                        case InstructionArg.XRefZ:
                            {
                                byte[] h = Xref[inst.Z];
                                if (hashes.ContainsKey(h))
                                {
                                    string x = hashes[h];
                                    if (x.StartsWith(ParentClass.Name + ".") || x == ParentClass.Name)
                                        x = "this" + x.Remove(0, ParentClass.Name.Length);

                                    textBox.AppendText(x, TextColors.XRefColor);
                                }
                                else
                                    textBox.AppendText($"{h[0]:X2}{h[1]:X2}{h[2]:X2}{h[3]:X2}", TextColors.XRefColor);
                                break;
                            }
                        case InstructionArg.XRefX:
                            {
                                byte[] h = Xref[inst.X];
                                if (hashes.ContainsKey(h))
                                {
                                    string x = hashes[h];
                                    if (x.StartsWith(ParentClass.Name + ".") || x == ParentClass.Name)
                                        x = "this" + x.Remove(0, ParentClass.Name.Length);

                                    textBox.AppendText(x, TextColors.XRefColor);
                                }
                                else
                                    textBox.AppendText($"{h[0]:X2}{h[1]:X2}{h[2]:X2}{h[3]:X2}", TextColors.XRefColor);
                                break;
                            }
                        case InstructionArg.XRefY:
                            {
                                byte[] h = Xref[inst.Y];
                                if (hashes.ContainsKey(h))
                                {
                                    string x = hashes[h];
                                    if (x.StartsWith(ParentClass.Name + ".") || x == ParentClass.Name)
                                        x = "this" + x.Remove(0, ParentClass.Name.Length);

                                    textBox.AppendText(x, TextColors.XRefColor);
                                }
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

            if (new Regex("(\\(.*\\))").IsMatch(lines[0]))
            {
                if (ParentClass.ParentScript.Version[0] >= 2)
                {
                    string[] funcDeclaration = lines[0].Split(' ');
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
                    Flags = funcFlags;
                    SetName(name);
                }
                else
                {
                    SetName(lines[0]);
                }

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

            //Verify each opcode name exists
            for (int i = 0; i < lines.Count; i++)
            {
                string opName = lines[i].Split(' ')[0].ToLower();
                if (opcodes.Where(x => x.Name == opName).Count() == 0)
                {
                    MessageBox.Show($"Error: Unknown opcode name!\nOpcode: {opName}\nLine: {lines[i]}", "Mint Assembler", MessageBoxButtons.OK);
                    return;
                }
            }

            //Sdata Integers
            for (int i = 0; i < lines.Count; i++)
            {
                string[] line = lines[i].Split(' ');
                Opcode op = opcodes.Where(x => x.Name == line[0].ToLower()).FirstOrDefault();
                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    if (op.Arguments[a] == InstructionArg.IntV || op.Arguments[a].HasFlag(InstructionArg.SDataRegInt))
                    {
                        string arg = line[a + 1].TrimEnd(',');
                        byte[] b;

                        if (arg.StartsWith("r"))
                        {
                            if (op.Arguments[a].HasFlag(InstructionArg.SDataRegInt)) continue;
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
                        else if (arg == "true")
                        {
                            b = BitConverter.GetBytes((int)1);
                        }
                        else if (arg == "false")
                        {
                            b = BitConverter.GetBytes((int)0);
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

                        //Make absolutely sure that SData is 4-aligned before running through SData
                        while ((sdata.Count & 0xF) != 0x0
                            && (sdata.Count & 0xF) != 0x4
                            && (sdata.Count & 0xF) != 0x8
                            && (sdata.Count & 0xF) != 0xC)
                            sdata.Add(0xFF);

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
                        if (op.Arguments[a].HasFlag(InstructionArg.SDataRegInt))
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
                Opcode op = opcodes.Where(x => x.Name == line[0].ToLower()).FirstOrDefault();
                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    if (op.Arguments[a] == InstructionArg.ArrV || op.Arguments[a].HasFlag(InstructionArg.SDataRegArr))
                    {
                        string arg = line[a + 1];
                        List<byte> b = new List<byte>();

                        if (arg.StartsWith("r"))
                        {
                            if (op.Arguments[a].HasFlag(InstructionArg.SDataRegArr)) continue;
                            else
                            {
                                MessageBox.Show($"Error: Expected string, got register.\nArgument: {arg}\nLine: {lines[i]}", "Mint Assembler", MessageBoxButtons.OK);
                                return;
                            }
                        }

                        int strEndIndex = a;
                        string str = "";
                        for (int s = a + 1; s < line.Length; s++)
                        {
                            str += line[s];
                            if (line[s].EndsWith("\"") || line[s].EndsWith("\","))
                                break;
                        }
                        if (!str.EndsWith("\"") && !str.EndsWith("\","))
                        {
                            MessageBox.Show($"Error: String has no ending:\nArgument: {str}\nLine: {lines[i]}", "Mint Assembler", MessageBoxButtons.OK);
                            return;
                        }

                        if (str.StartsWith("u\""))
                        {
                            b = Encoding.Unicode.GetBytes(str.Substring(1).TrimStart('\"').TrimEnd('\"')).ToList();
                            b.Add(0);
                        }
                        else
                            b = Encoding.UTF8.GetBytes(str.TrimStart('\"').TrimEnd('\"')).ToList();
                        b.Add(0);

                        //Make absolutely sure that SData is 4-aligned before running through SData
                        while ((sdata.Count & 0xF) != 0x0
                            && (sdata.Count & 0xF) != 0x4
                            && (sdata.Count & 0xF) != 0x8
                            && (sdata.Count & 0xF) != 0xC)
                            sdata.Add(0xFF);

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

                        //4-align SData if necessary
                        //Step isn't done for integers since those are always 32-bit
                        while ((sdata.Count & 0xF) != 0x0
                            && (sdata.Count & 0xF) != 0x4
                            && (sdata.Count & 0xF) != 0x8
                            && (sdata.Count & 0xF) != 0xC)
                            sdata.Add(0xFF);

                        int buildVal = sIndex;
                        if (op.Arguments[a].HasFlag(InstructionArg.SDataRegArr))
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
                Opcode op = opcodes.Where(x => x.Name == line[0].ToLower()).FirstOrDefault();
                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    if (op.Arguments[a].HasFlag(InstructionArg.XRef))
                    {
                        string arg = line[a + 1];
                        byte[] b = new byte[4];

                        if (arg.StartsWith("r"))
                        {
                            if (op.Arguments[a] != InstructionArg.XRefV) continue;
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
                            if (xstring.StartsWith("this.") || xstring == "this")
                            {
                                xstring = ParentClass.Name + xstring.Remove(0, 4);
                            }
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
                Opcode op = opcodes.Where(x => x.Name == line[0].ToLower()).FirstOrDefault();
                if (op.Action.HasFlag(Mint.Action.Jump))
                {
                    for (int a = 0; a < op.Arguments.Length; a++)
                    {
                        if (op.Arguments[a] == InstructionArg.VSigned)
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
                var o = opcodes.Where(x => x.Name == line[0].ToLower());

                Opcode op = o.FirstOrDefault();
                Instruction inst = new Instruction(new byte[] { (byte)opcodes.ToList().IndexOf(op), 0xFF, 0xFF, 0xFF });
                //Console.WriteLine(lines[i]);
                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    switch (op.Arguments[a] & InstructionArg.AllData)
                    {
                        case InstructionArg.Z:
                            {
                                inst.Z = byte.Parse(line[a + 1].TrimStart('r'));
                                break;
                            }
                        case InstructionArg.X:
                            {
                                inst.X = byte.Parse(line[a + 1].TrimStart('r'));
                                break;
                            }
                        case InstructionArg.Y:
                            {
                                inst.Y = byte.Parse(line[a + 1].TrimStart('r'));
                                break;
                            }
                        case InstructionArg.V:
                            {
                                byte[] v;
                                if (op.Arguments[a].HasFlag(InstructionArg.Signed))
                                    v = BitConverter.GetBytes(short.Parse(line[a + 1]));
                                else
                                    v = BitConverter.GetBytes(ushort.Parse(line[a + 1]));
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
