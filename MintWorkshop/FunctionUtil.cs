using BrawlLib.Internal;
using KirbyLib.Crypto;
using KirbyLib.IO;
using KirbyLib.Mint;
using MintWorkshop.Mint;
using MintWorkshop.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MintWorkshop
{
    internal static partial class MintRegex
    {
        [GeneratedRegex(@"(//.*)")]
        internal static partial Regex Comment();

        [GeneratedRegex(@"(/\*[\s\S]*?\*/)")]
        internal static partial Regex MultilineComment();

        [GeneratedRegex(@"module ([.0-9A-Z_a-z<>\[\]^]+)")]
        internal static partial Regex Module();

        // world's worst regex??
        [GeneratedRegex(@"\.sdata ((?:(?:u)?"".*?"")|(?:(?:0x[0-9A-Fa-f]+)|(?:-?[0-9]+(?:\.[0-9]+)?f?)))")]
        internal static partial Regex SData();

        // world's second worst regex??
        [GeneratedRegex(@"\.xref ((?:[.0-9A-Z_a-z<>\[\]^]+)|(?:#[0-9A-Fa-f]{8}\b))")]
        internal static partial Regex XRef();

        // world's questionably worst regex??
        [GeneratedRegex(@"\.raw ([0-9a-fA-F]{2}),? ([0-9a-fA-F]{2}),? ([0-9a-fA-F]{2}),? ([0-9a-fA-F]{2})")]
        internal static partial Regex Raw();

        [GeneratedRegex(@"class ([.0-9A-Z_a-z<>\[\]^]+)")]
        internal static partial Regex Object();

        // return of the world's second worst regex
        [GeneratedRegex(@"extend ((?:[.0-9A-Z_a-z<>\[\]^]+)|(?:#[0-9A-Fa-f]{8}\b))")]
        internal static partial Regex Extend();

        [GeneratedRegex(@"([.0-9A-Z_a-z<>\[\]^]+) ([.-9A-Z_a-z]+);")]
        internal static partial Regex Variable();

        [GeneratedRegex(@"([.0-9A-Z_a-z<>\[\]^]+ [.-9A-Z_a-z]+\(.*?\))")]
        internal static partial Regex Function();

        [GeneratedRegex(@"const ([.0-9A-Z_a-z]+) = ((?:(?:0x[0-9A-Fa-f]+)|(?:-?[0-9]+)));")]
        internal static partial Regex Enum();

        [GeneratedRegex(@"^r([0-9]+)$")]
        internal static partial Regex Register();

        [GeneratedRegex(@"^((?:0x[0-9A-Fa-f]+)|(?:-?[0-9]+))$")]
        internal static partial Regex Integer();

        [GeneratedRegex(@"^(-?[0-9]+(?:\.[0-9]+)?f?)$")]
        internal static partial Regex Float();

        [GeneratedRegex(@"^((?:(?:0x[0-9A-Fa-f]+)|(?:-?[0-9]+(?:\.[0-9]+)?f?)))$")]
        internal static partial Regex Value();

        [GeneratedRegex(@"^((?:u)?"".*?"")$")]
        internal static partial Regex String();

        [GeneratedRegex(@"^#([0-9A-Fa-f]{8})\b$")]
        internal static partial Regex Hash();
    }

    public static class FunctionUtil
    {
        public static string Disassemble(Module module, MintObject obj, MintFunction func, byte[] version, ref Dictionary<uint, string> hashes)
        {
            string text = "";

            byte[] sdata = module.SData.ToArray();

            Dictionary<int, string> jmpLoc = new Dictionary<int, string>();

            var opcodes = MintVersions.Versions[version];

            // Find jump instructions to create labels
            for (int i = 0; i < func.Data.Length; i += 4)
            {
                byte o = func.Data[i];
                if (o >= opcodes.Length || string.IsNullOrEmpty(opcodes[o].Name))
                    continue;

                var op = opcodes[o];
                if (op.Action.HasFlag(Mint.Action.Jump))
                {
                    byte[] bLoc = op.Arguments.Contains(InstructionArg.ESigned)
                        ? func.Data[(i + 6)..(i + 8)]
                        : func.Data[(i + 2)..(i + 4)];

                    if (module.XData.Endianness == Endianness.Big)
                        bLoc = bLoc.Reverse().ToArray();

                    short loc = BitConverter.ToInt16(bLoc);
                    if (op.Arguments.Contains(InstructionArg.ESigned))
                        loc++;

                    int target = i + (loc * 4);

                    if (!jmpLoc.ContainsKey(target))
                    {
                        byte oT = func.Data[target];
                        string label = oT < opcodes.Length && opcodes[oT].Action.HasFlag(Mint.Action.Return)
                            ? $"return_{target:x8}"
                            : $"loc_{target:x8}";

                        jmpLoc.Add(target, label);
                    }
                }

                i += op.Size - 4;
            }

            for (int i = 0; i < func.Data.Length; i += 4)
            {
                if (jmpLoc.ContainsKey(i))
                    text += "\n" + jmpLoc[i] + ":\n";

                byte o = func.Data[i];
                if (o >= opcodes.Length || string.IsNullOrEmpty(opcodes[o].Name))
                {
                    text += $".raw {o:X2}, {func.Data[i + 1]:X2}, {func.Data[i + 2]:X2}, {func.Data[i + 3]:X2}\n";
                    continue;
                }

                var op = opcodes[o];
                text += op.Name + new string(' ', 6 - op.Name.Length + 1);
                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    var arg = op.Arguments[a];
                    var argType = arg & InstructionArg.AllTypes;
                    var argLoc = arg & InstructionArg.AllData;
                    switch (argType)
                    {
                        default:
                            {
                                int value = 0;
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        value = func.Data[i + 1];
                                        break;
                                    case InstructionArg.X:
                                        value = func.Data[i + 2];
                                        break;
                                    case InstructionArg.Y:
                                        value = func.Data[i + 3];
                                        break;
                                    case InstructionArg.V:
                                        byte[] v = func.Data[(i + 2)..(i + 4)];
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = v.Reverse().ToArray();
                                        value = arg.HasFlag(InstructionArg.Signed)
                                            ? BitConverter.ToInt16(v)
                                            : BitConverter.ToUInt16(v);
                                        break;
                                    case InstructionArg.A:
                                        value = func.Data[i + 5];
                                        break;
                                    case InstructionArg.B:
                                        value = func.Data[i + 6];
                                        break;
                                    case InstructionArg.C:
                                        value = func.Data[i + 7];
                                        break;
                                    case InstructionArg.E:
                                        byte[] e = func.Data[(i + 6)..(i + 8)];
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = e.Reverse().ToArray();
                                        value = arg.HasFlag(InstructionArg.Signed)
                                            ? BitConverter.ToInt16(e) + 1
                                            : BitConverter.ToUInt16(e);
                                        break;
                                }

                                if (op.Action.HasFlag(Mint.Action.Jump))
                                    text += jmpLoc[i + (value * 4)];
                                else
                                    text += value;

                                break;
                            }
                        case InstructionArg.Register:
                            {
                                byte reg = 0;
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        reg = func.Data[i + 1];
                                        break;
                                    case InstructionArg.X:
                                        reg = func.Data[i + 2];
                                        break;
                                    case InstructionArg.Y:
                                        reg = func.Data[i + 3];
                                        break;
                                    case InstructionArg.A:
                                        reg = func.Data[i + 5];
                                        break;
                                    case InstructionArg.B:
                                        reg = func.Data[i + 6];
                                        break;
                                    case InstructionArg.C:
                                        reg = func.Data[i + 7];
                                        break;
                                }

                                text += "r" + reg;
                                break;
                            }
                        case InstructionArg.SDataInt:
                            {
                                uint data = 0;
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        data = BitConverter.ToUInt32(sdata, func.Data[i + 1] * 4);
                                        break;
                                    case InstructionArg.X:
                                        data = BitConverter.ToUInt32(sdata, func.Data[i + 2] * 4);
                                        break;
                                    case InstructionArg.Y:
                                        data = BitConverter.ToUInt32(sdata, func.Data[i + 3] * 4);
                                        break;
                                    case InstructionArg.V:
                                        int v = BitConverter.ToUInt16(func.Data, i + 2);
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = ((v & 0xFF00) >> 8) | ((v & 0xFF) << 8);
                                        data = BitConverter.ToUInt32(sdata, v);
                                        break;
                                    case InstructionArg.A:
                                        data = BitConverter.ToUInt32(sdata, func.Data[i + 5] * 4);
                                        break;
                                    case InstructionArg.B:
                                        data = BitConverter.ToUInt32(sdata, func.Data[i + 6] * 4);
                                        break;
                                    case InstructionArg.C:
                                        data = BitConverter.ToUInt32(sdata, func.Data[i + 7] * 4);
                                        break;
                                    case InstructionArg.E:
                                        int e = BitConverter.ToUInt16(func.Data, i + 6);
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = ((e & 0xFF00) >> 8) | ((e & 0xFF) << 8);
                                        data = BitConverter.ToUInt32(sdata, e);
                                        break;
                                }

                                if (module.XData.Endianness == Endianness.Big)
                                    data = ((data & 0xFF000000) >> 24)
                                        | ((data & 0x00FF0000) >> 8)
                                        | ((data & 0x0000FF00) << 8)
                                        | ((data & 0x000000FF) << 24);

                                text += "0x" + data.ToString("X");
                                break;
                            }
                        case InstructionArg.SDataFloat:
                            {
                                int start = 0;
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        start = func.Data[i + 1] * 4;
                                        break;
                                    case InstructionArg.X:
                                        start = func.Data[i + 2] * 4;
                                        break;
                                    case InstructionArg.Y:
                                        start = func.Data[i + 3] * 4;
                                        break;
                                    case InstructionArg.V:
                                        int v = BitConverter.ToUInt16(func.Data, i + 2);
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = ((v & 0xFF00) >> 8) | ((v & 0xFF) << 8);
                                        start = v;
                                        break;
                                    case InstructionArg.A:
                                        start = func.Data[i + 5] * 4;
                                        break;
                                    case InstructionArg.B:
                                        start = func.Data[i + 6] * 4;
                                        break;
                                    case InstructionArg.C:
                                        start = func.Data[i + 7] * 4;
                                        break;
                                    case InstructionArg.E:
                                        int e = BitConverter.ToUInt16(func.Data, i + 6);
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = ((e & 0xFF00) >> 8) | ((e & 0xFF) << 8);
                                        start = e;
                                        break;
                                }

                                byte[] data = sdata[start..(start + 4)];

                                if (module.XData.Endianness == Endianness.Big)
                                    data = data.Reverse().ToArray();

                                text += BitConverter.ToSingle(data) + "f";
                                break;
                            }
                        case InstructionArg.SDataArray:
                            {
                                int index = 0;
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        index = func.Data[i + 1] * 4;
                                        break;
                                    case InstructionArg.X:
                                        index = func.Data[i + 2] * 4;
                                        break;
                                    case InstructionArg.Y:
                                        index = func.Data[i + 3] * 4;
                                        break;
                                    case InstructionArg.V:
                                        int v = BitConverter.ToUInt16(func.Data, i + 2);
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = ((v & 0xFF00) >> 8) | ((v & 0xFF) << 8);
                                        index = v;
                                        break;
                                    case InstructionArg.A:
                                        index = func.Data[i + 5] * 4;
                                        break;
                                    case InstructionArg.B:
                                        index = func.Data[i + 6] * 4;
                                        break;
                                    case InstructionArg.C:
                                        index = func.Data[i + 7] * 4;
                                        break;
                                    case InstructionArg.E:
                                        int e = BitConverter.ToUInt16(func.Data, i + 6);
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = ((e & 0xFF00) >> 8) | ((e & 0xFF) << 8);
                                        index = e;
                                        break;
                                }

                                List<byte> bytes = new List<byte>();
                                bool utf16 = false;
                                for (int s = index; s < sdata.Length; s++)
                                {
                                    if (sdata[s] == 0x00 && ((s & 0x1) == 0x1))
                                    {
                                        if (s > 0 && bytes.Count > 0)
                                        {
                                            if (sdata[s - 1] == 0x00)
                                            {
                                                bytes.RemoveAt(bytes.Count - 1);
                                                utf16 = true;
                                            }
                                        }
                                        break;
                                    }
                                    else if (sdata[s] == 0xFF)
                                    {
                                        if (sdata[s - 1] == 0x00)
                                        {
                                            bytes.RemoveAt(bytes.Count - 1);
                                            if (sdata[s - 2] == 0 && bytes.Count - 1 >= 0)
                                            {
                                                bytes.RemoveAt(bytes.Count - 1);
                                                utf16 = true;
                                            }
                                            break;
                                        }
                                    }
                                    else if (sdata[s] == 0x00)
                                        break;

                                    bytes.Add(sdata[s]);
                                }

                                Encoding enc = utf16 ? Encoding.Unicode : Encoding.UTF8;
                                if (utf16)
                                    text += "u";
                                text += "\"" + enc.GetString(bytes.ToArray()) + "\"";

                                break;
                            }
                        case InstructionArg.SDataRegInt:
                            {
                                byte reg = 0;
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        reg = func.Data[i + 1];
                                        break;
                                    case InstructionArg.X:
                                        reg = func.Data[i + 2];
                                        break;
                                    case InstructionArg.Y:
                                        reg = func.Data[i + 3];
                                        break;
                                    case InstructionArg.A:
                                        reg = func.Data[i + 5];
                                        break;
                                    case InstructionArg.B:
                                        reg = func.Data[i + 6];
                                        break;
                                    case InstructionArg.C:
                                        reg = func.Data[i + 7];
                                        break;
                                }

                                if ((reg & 0x80) != 0)
                                {
                                    uint data = BitConverter.ToUInt32(sdata, (reg & 0x7F) * 4);
                                    if (module.XData.Endianness == Endianness.Big)
                                        data = ((data & 0xFF000000) >> 24)
                                            | ((data & 0x00FF0000) >> 8)
                                            | ((data & 0x0000FF00) << 8)
                                            | ((data & 0x000000FF) << 24);

                                    text += "0x" + data.ToString("X");
                                }
                                else
                                    text += "r" + reg;
                                break;
                            }
                        case InstructionArg.SDataRegFloat:
                            {
                                byte reg = 0;
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        reg = func.Data[i + 1];
                                        break;
                                    case InstructionArg.X:
                                        reg = func.Data[i + 2];
                                        break;
                                    case InstructionArg.Y:
                                        reg = func.Data[i + 3];
                                        break;
                                    case InstructionArg.A:
                                        reg = func.Data[i + 5];
                                        break;
                                    case InstructionArg.B:
                                        reg = func.Data[i + 6];
                                        break;
                                    case InstructionArg.C:
                                        reg = func.Data[i + 7];
                                        break;
                                }

                                if ((reg & 0x80) != 0)
                                {
                                    int start = (reg & 0x7F) * 4;
                                    byte[] data = sdata[start..(start + 4)];
                                    if (module.XData.Endianness == Endianness.Big)
                                        data = data.Reverse().ToArray();

                                    text += BitConverter.ToSingle(data) + "f";
                                }
                                else
                                    text += "r" + reg;
                                break;
                            }
                        case InstructionArg.SDataRegArray:
                            {
                                byte reg = 0;
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        reg = func.Data[i + 1];
                                        break;
                                    case InstructionArg.X:
                                        reg = func.Data[i + 2];
                                        break;
                                    case InstructionArg.Y:
                                        reg = func.Data[i + 3];
                                        break;
                                    case InstructionArg.A:
                                        reg = func.Data[i + 5];
                                        break;
                                    case InstructionArg.B:
                                        reg = func.Data[i + 6];
                                        break;
                                    case InstructionArg.C:
                                        reg = func.Data[i + 7];
                                        break;
                                }

                                if ((reg & 0x80) != 0)
                                {
                                    List<byte> bytes = new List<byte>();
                                    bool utf16 = false;
                                    for (int s = reg * 4; s < sdata.Length; s++)
                                    {
                                        if (sdata[s] == 0x00 && ((s & 0x1) == 0x1))
                                        {
                                            if (s > 0 && bytes.Count > 0)
                                            {
                                                if (sdata[s - 1] == 0x00)
                                                {
                                                    bytes.RemoveAt(bytes.Count - 1);
                                                    utf16 = true;
                                                }
                                            }
                                            break;
                                        }
                                        else if (sdata[s] == 0xFF)
                                        {
                                            if (sdata[s - 1] == 0x00)
                                            {
                                                bytes.RemoveAt(bytes.Count - 1);
                                                if (sdata[s - 2] == 0 && bytes.Count - 1 >= 0)
                                                {
                                                    bytes.RemoveAt(bytes.Count - 1);
                                                    utf16 = true;
                                                }
                                                break;
                                            }
                                        }
                                        else if (sdata[s] == 0x00)
                                            break;

                                        bytes.Add(sdata[s]);
                                    }

                                    Encoding enc = utf16 ? Encoding.Unicode : Encoding.UTF8;
                                    if (utf16)
                                        text += "u";
                                    text += "\"" + enc.GetString(bytes.ToArray()) + "\"";
                                }
                                else
                                    text += "r" + reg;
                                break;
                            }
                        case InstructionArg.XRef:
                            {
                                int index = 0;
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        index = func.Data[i + 1];
                                        break;
                                    case InstructionArg.X:
                                        index = func.Data[i + 2];
                                        break;
                                    case InstructionArg.Y:
                                        index = func.Data[i + 3];
                                        break;
                                    case InstructionArg.V:
                                        int v = BitConverter.ToUInt16(func.Data, i + 2);
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = ((v & 0xFF00) >> 8) | ((v & 0xFF) << 8);
                                        index = v;
                                        break;
                                    case InstructionArg.A:
                                        index = func.Data[i + 5];
                                        break;
                                    case InstructionArg.B:
                                        index = func.Data[i + 6];
                                        break;
                                    case InstructionArg.C:
                                        index = func.Data[i + 7];
                                        break;
                                    case InstructionArg.E:
                                        int e = BitConverter.ToUInt16(func.Data, i + 6);
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = ((e & 0xFF00) >> 8) | ((e & 0xFF) << 8);
                                        index = e;
                                        break;
                                }

                                uint hash = module.XRef[index];
                                if (hashes.ContainsKey(hash))
                                {
                                    string xref = hashes[hash];
                                    if (xref.StartsWith(obj.Name + ".") || xref.StartsWith(obj.Name + "/") || xref == obj.Name)
                                        xref = "this" + xref[obj.Name.Length..];
                                    text += xref;
                                }
                                else
                                    text += $"#{hash:X8}";

                                break;
                            }
                    }

                    if (a < op.Arguments.Length - 1)
                        text += ", ";
                }

                text += "\n";

                i += op.Size - 4;
            }

            return text;
        }

        public static string Disassemble(ModuleRtDL module, MintFunction func, byte[] version, ref Dictionary<uint, string> hashes)
        {
            string text = "";

            byte[] sdata = module.SData.ToArray();

            Dictionary<int, string> jmpLoc = new Dictionary<int, string>();

            var opcodes = MintVersions.Versions[version];

            // Find jump instructions to create labels
            for (int i = 0; i < func.Data.Length; i += 4)
            {
                byte o = func.Data[i];
                if (o >= opcodes.Length || string.IsNullOrEmpty(opcodes[o].Name))
                    continue;

                var op = opcodes[o];
                if (op.Action.HasFlag(Mint.Action.Jump))
                {
                    byte[] bLoc = op.Arguments.Contains(InstructionArg.ESigned)
                        ? func.Data[(i + 6)..(i + 8)]
                        : func.Data[(i + 2)..(i + 4)];

                    if (module.XData.Endianness == Endianness.Big)
                        bLoc = bLoc.Reverse().ToArray();

                    short loc = BitConverter.ToInt16(bLoc);
                    if (op.Arguments.Contains(InstructionArg.ESigned))
                        loc++;

                    int target = i + (loc * 4);

                    if (!jmpLoc.ContainsKey(target))
                    {
                        byte oT = func.Data[target];
                        string label = oT < opcodes.Length && opcodes[oT].Action.HasFlag(Mint.Action.Return)
                            ? $"return_{target:x8}"
                            : $"loc_{target:x8}";

                        jmpLoc.Add(target, label);
                    }
                }

                i += op.Size - 4;
            }

            for (int i = 0; i < func.Data.Length; i += 4)
            {
                if (jmpLoc.ContainsKey(i))
                    text += "\n" + jmpLoc[i] + ":\n";

                byte o = func.Data[i];
                if (o >= opcodes.Length || string.IsNullOrEmpty(opcodes[o].Name))
                {
                    text += $".raw {o:X2}, {func.Data[i + 1]:X2}, {func.Data[i + 2]:X2}, {func.Data[i + 3]:X2}\n";
                    continue;
                }

                var op = opcodes[o];
                text += op.Name + new string(' ', 6 - op.Name.Length + 1);
                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    var arg = op.Arguments[a];
                    var argType = arg & InstructionArg.AllTypes;
                    var argLoc = arg & InstructionArg.AllData;
                    switch (argType)
                    {
                        default:
                            {
                                int value = 0;
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        value = func.Data[i + 1];
                                        break;
                                    case InstructionArg.X:
                                        value = func.Data[i + 2];
                                        break;
                                    case InstructionArg.Y:
                                        value = func.Data[i + 3];
                                        break;
                                    case InstructionArg.V:
                                        byte[] v = func.Data[(i + 2)..(i + 4)];
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = v.Reverse().ToArray();
                                        value = arg.HasFlag(InstructionArg.Signed)
                                            ? BitConverter.ToInt16(v)
                                            : BitConverter.ToUInt16(v);
                                        break;
                                    case InstructionArg.A:
                                        value = func.Data[i + 5];
                                        break;
                                    case InstructionArg.B:
                                        value = func.Data[i + 6];
                                        break;
                                    case InstructionArg.C:
                                        value = func.Data[i + 7];
                                        break;
                                    case InstructionArg.E:
                                        byte[] e = func.Data[(i + 6)..(i + 8)];
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = e.Reverse().ToArray();
                                        value = arg.HasFlag(InstructionArg.Signed)
                                            ? BitConverter.ToInt16(e) + 1
                                            : BitConverter.ToUInt16(e);
                                        break;
                                }

                                if (op.Action.HasFlag(Mint.Action.Jump))
                                    text += jmpLoc[i + (value * 4)];
                                else
                                    text += value;

                                break;
                            }
                        case InstructionArg.Register:
                            {
                                byte reg = 0;
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        reg = func.Data[i + 1];
                                        break;
                                    case InstructionArg.X:
                                        reg = func.Data[i + 2];
                                        break;
                                    case InstructionArg.Y:
                                        reg = func.Data[i + 3];
                                        break;
                                    case InstructionArg.A:
                                        reg = func.Data[i + 5];
                                        break;
                                    case InstructionArg.B:
                                        reg = func.Data[i + 6];
                                        break;
                                    case InstructionArg.C:
                                        reg = func.Data[i + 7];
                                        break;
                                }

                                text += "r" + reg;
                                break;
                            }
                        case InstructionArg.SDataInt:
                            {
                                uint data = 0;
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        data = BitConverter.ToUInt32(sdata, func.Data[i + 1] * 4);
                                        break;
                                    case InstructionArg.X:
                                        data = BitConverter.ToUInt32(sdata, func.Data[i + 2] * 4);
                                        break;
                                    case InstructionArg.Y:
                                        data = BitConverter.ToUInt32(sdata, func.Data[i + 3] * 4);
                                        break;
                                    case InstructionArg.V:
                                        int v = BitConverter.ToUInt16(func.Data, i + 2);
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = ((v & 0xFF00) >> 8) | ((v & 0xFF) << 8);
                                        data = BitConverter.ToUInt32(sdata, v);
                                        break;
                                    case InstructionArg.A:
                                        data = BitConverter.ToUInt32(sdata, func.Data[i + 5] * 4);
                                        break;
                                    case InstructionArg.B:
                                        data = BitConverter.ToUInt32(sdata, func.Data[i + 6] * 4);
                                        break;
                                    case InstructionArg.C:
                                        data = BitConverter.ToUInt32(sdata, func.Data[i + 7] * 4);
                                        break;
                                    case InstructionArg.E:
                                        int e = BitConverter.ToUInt16(func.Data, i + 6);
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = ((e & 0xFF00) >> 8) | ((e & 0xFF) << 8);
                                        data = BitConverter.ToUInt32(sdata, e);
                                        break;
                                }

                                if (module.XData.Endianness == Endianness.Big)
                                    data = ((data & 0xFF000000) >> 24)
                                        | ((data & 0x00FF0000) >> 8)
                                        | ((data & 0x0000FF00) << 8)
                                        | ((data & 0x000000FF) << 24);

                                text += "0x" + data.ToString("X");
                                break;
                            }
                        case InstructionArg.SDataFloat:
                            {
                                int start = 0;
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        start = func.Data[i + 1] * 4;
                                        break;
                                    case InstructionArg.X:
                                        start = func.Data[i + 2] * 4;
                                        break;
                                    case InstructionArg.Y:
                                        start = func.Data[i + 3] * 4;
                                        break;
                                    case InstructionArg.V:
                                        int v = BitConverter.ToUInt16(func.Data, i + 2);
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = ((v & 0xFF00) >> 8) | ((v & 0xFF) << 8);
                                        start = v;
                                        break;
                                    case InstructionArg.A:
                                        start = func.Data[i + 5] * 4;
                                        break;
                                    case InstructionArg.B:
                                        start = func.Data[i + 6] * 4;
                                        break;
                                    case InstructionArg.C:
                                        start = func.Data[i + 7] * 4;
                                        break;
                                    case InstructionArg.E:
                                        int e = BitConverter.ToUInt16(func.Data, i + 6);
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = ((e & 0xFF00) >> 8) | ((e & 0xFF) << 8);
                                        start = e;
                                        break;
                                }

                                byte[] data = sdata[start..(start + 4)];

                                if (module.XData.Endianness == Endianness.Big)
                                    data = data.Reverse().ToArray();

                                text += BitConverter.ToSingle(data) + "f";
                                break;
                            }
                        case InstructionArg.SDataArray:
                            {
                                int index = 0;
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        index = func.Data[i + 1] * 4;
                                        break;
                                    case InstructionArg.X:
                                        index = func.Data[i + 2] * 4;
                                        break;
                                    case InstructionArg.Y:
                                        index = func.Data[i + 3] * 4;
                                        break;
                                    case InstructionArg.V:
                                        int v = BitConverter.ToUInt16(func.Data, i + 2);
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = ((v & 0xFF00) >> 8) | ((v & 0xFF) << 8);
                                        index = v;
                                        break;
                                    case InstructionArg.A:
                                        index = func.Data[i + 5] * 4;
                                        break;
                                    case InstructionArg.B:
                                        index = func.Data[i + 6] * 4;
                                        break;
                                    case InstructionArg.C:
                                        index = func.Data[i + 7] * 4;
                                        break;
                                    case InstructionArg.E:
                                        int e = BitConverter.ToUInt16(func.Data, i + 6);
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = ((e & 0xFF00) >> 8) | ((e & 0xFF) << 8);
                                        index = e;
                                        break;
                                }

                                List<byte> bytes = new List<byte>();
                                bool utf16 = false;
                                for (int s = index; s < sdata.Length; s++)
                                {
                                    if (sdata[s] == 0x00 && ((s & 0x1) == 0x1))
                                    {
                                        if (s > 0 && bytes.Count > 0)
                                        {
                                            if (sdata[s - 1] == 0x00)
                                            {
                                                bytes.RemoveAt(bytes.Count - 1);
                                                utf16 = true;
                                            }
                                        }
                                        break;
                                    }
                                    else if (sdata[s] == 0xFF)
                                    {
                                        if (sdata[s - 1] == 0x00)
                                        {
                                            bytes.RemoveAt(bytes.Count - 1);
                                            if (sdata[s - 2] == 0 && bytes.Count - 1 >= 0)
                                            {
                                                bytes.RemoveAt(bytes.Count - 1);
                                                utf16 = true;
                                            }
                                            break;
                                        }
                                    }
                                    else if (sdata[s] == 0x00)
                                        break;

                                    bytes.Add(sdata[s]);
                                }

                                Encoding enc = utf16 ? Encoding.Unicode : Encoding.UTF8;
                                if (utf16)
                                    text += "u";
                                text += "\"" + enc.GetString(bytes.ToArray()) + "\"";

                                break;
                            }
                        case InstructionArg.SDataRegInt:
                            {
                                byte reg = 0;
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        reg = func.Data[i + 1];
                                        break;
                                    case InstructionArg.X:
                                        reg = func.Data[i + 2];
                                        break;
                                    case InstructionArg.Y:
                                        reg = func.Data[i + 3];
                                        break;
                                    case InstructionArg.A:
                                        reg = func.Data[i + 5];
                                        break;
                                    case InstructionArg.B:
                                        reg = func.Data[i + 6];
                                        break;
                                    case InstructionArg.C:
                                        reg = func.Data[i + 7];
                                        break;
                                }

                                if ((reg & 0x80) != 0)
                                {
                                    uint data = BitConverter.ToUInt32(sdata, (reg & 0x7F) * 4);
                                    if (module.XData.Endianness == Endianness.Big)
                                        data = ((data & 0xFF000000) >> 24)
                                            | ((data & 0x00FF0000) >> 8)
                                            | ((data & 0x0000FF00) << 8)
                                            | ((data & 0x000000FF) << 24);

                                    text += "0x" + data.ToString("X");
                                }
                                else
                                    text += "r" + reg;
                                break;
                            }
                        case InstructionArg.SDataRegFloat:
                            {
                                byte reg = 0;
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        reg = func.Data[i + 1];
                                        break;
                                    case InstructionArg.X:
                                        reg = func.Data[i + 2];
                                        break;
                                    case InstructionArg.Y:
                                        reg = func.Data[i + 3];
                                        break;
                                    case InstructionArg.A:
                                        reg = func.Data[i + 5];
                                        break;
                                    case InstructionArg.B:
                                        reg = func.Data[i + 6];
                                        break;
                                    case InstructionArg.C:
                                        reg = func.Data[i + 7];
                                        break;
                                }

                                if ((reg & 0x80) != 0)
                                {
                                    int start = (reg & 0x7F) * 4;
                                    byte[] data = sdata[start..(start + 4)];
                                    if (module.XData.Endianness == Endianness.Big)
                                        data = data.Reverse().ToArray();

                                    text += BitConverter.ToSingle(data) + "f";
                                }
                                else
                                    text += "r" + reg;
                                break;
                            }
                        case InstructionArg.SDataRegArray:
                            {
                                byte reg = 0;
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        reg = func.Data[i + 1];
                                        break;
                                    case InstructionArg.X:
                                        reg = func.Data[i + 2];
                                        break;
                                    case InstructionArg.Y:
                                        reg = func.Data[i + 3];
                                        break;
                                    case InstructionArg.A:
                                        reg = func.Data[i + 5];
                                        break;
                                    case InstructionArg.B:
                                        reg = func.Data[i + 6];
                                        break;
                                    case InstructionArg.C:
                                        reg = func.Data[i + 7];
                                        break;
                                }

                                if ((reg & 0x80) != 0)
                                {
                                    List<byte> bytes = new List<byte>();
                                    bool utf16 = false;
                                    for (int s = reg * 4; s < sdata.Length; s++)
                                    {
                                        if (sdata[s] == 0x00 && ((s & 0x1) == 0x1))
                                        {
                                            if (s > 0 && bytes.Count > 0)
                                            {
                                                if (sdata[s - 1] == 0x00)
                                                {
                                                    bytes.RemoveAt(bytes.Count - 1);
                                                    utf16 = true;
                                                }
                                            }
                                            break;
                                        }
                                        else if (sdata[s] == 0xFF)
                                        {
                                            if (sdata[s - 1] == 0x00)
                                            {
                                                bytes.RemoveAt(bytes.Count - 1);
                                                if (sdata[s - 2] == 0 && bytes.Count - 1 >= 0)
                                                {
                                                    bytes.RemoveAt(bytes.Count - 1);
                                                    utf16 = true;
                                                }
                                                break;
                                            }
                                        }
                                        else if (sdata[s] == 0x00)
                                            break;

                                        bytes.Add(sdata[s]);
                                    }

                                    Encoding enc = utf16 ? Encoding.Unicode : Encoding.UTF8;
                                    if (utf16)
                                        text += "u";
                                    text += "\"" + enc.GetString(bytes.ToArray()) + "\"";
                                }
                                else
                                    text += "r" + reg;
                                break;
                            }
                        case InstructionArg.XRef:
                            {
                                int index = 0;
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        index = func.Data[i + 1];
                                        break;
                                    case InstructionArg.X:
                                        index = func.Data[i + 2];
                                        break;
                                    case InstructionArg.Y:
                                        index = func.Data[i + 3];
                                        break;
                                    case InstructionArg.V:
                                        int v = BitConverter.ToUInt16(func.Data, i + 2);
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = ((v & 0xFF00) >> 8) | ((v & 0xFF) << 8);
                                        index = v;
                                        break;
                                    case InstructionArg.A:
                                        index = func.Data[i + 5];
                                        break;
                                    case InstructionArg.B:
                                        index = func.Data[i + 6];
                                        break;
                                    case InstructionArg.C:
                                        index = func.Data[i + 7];
                                        break;
                                    case InstructionArg.E:
                                        int e = BitConverter.ToUInt16(func.Data, i + 6);
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = ((e & 0xFF00) >> 8) | ((e & 0xFF) << 8);
                                        index = e;
                                        break;
                                }

                                text += module.XRef[index];

                                break;
                            }
                    }

                    if (a < op.Arguments.Length - 1)
                        text += ", ";
                }

                text += "\n";

                i += op.Size - 4;
            }

            return text;
        }

        public static string[] GetFlagLabels(uint flags, ref Dictionary<uint, string> labels)
        {
            List<string> label = new List<string>();
            for (uint f = 1; true; f <<= 1)
            {
                if ((flags & f) != 0)
                {
                    if (labels.ContainsKey(f))
                        label.Add(labels[f]);
                    else
                        label.Add("flag" + f.ToString("X"));
                }

                if (f == 0x80000000)
                    break;
            }

            return label.ToArray();
        }

        public static string Disassemble(Module module, byte[] version, ref Dictionary<uint, string> hashes)
        {
            var opcodes = MintVersions.Versions[version];

            string text = $"module {module.Name}\n{{\n";

            if (module.UnkHash != 0xFFFFFFFF)
                text += "\tUnkHash = " + (hashes.ContainsKey(module.UnkHash) ? hashes[module.UnkHash] : $"#{module.UnkHash:X8}") + "\n\t\n";

            for (int i = 0; i < module.Objects.Count; i++)
            {
                var obj = module[i];

                {
                    string flags = string.Join(' ', GetFlagLabels(obj.Flags, ref FlagLabels.ClassFlags));
                    if (!string.IsNullOrWhiteSpace(flags))
                        flags += " ";

                    text += $"\t{flags}class {obj.Name}";
                }

                List<string> impl = new List<string>();
                for (int o = 0; o < obj.Implements.Count; o++)
                {
                    uint hash = obj.Implements[o];
                    impl.Add(hashes.ContainsKey(hash) ? hashes[hash] : $"#{hash:X8}");
                }

                if (impl.Count > 0)
                    text += " : " + string.Join(", ", impl);

                text += "\n\t{\n";

                for (int o = 0; o < obj.Extends.Count; o++)
                {
                    byte[] ext = obj.Extends[o];
                    string str = "";
                    if (opcodes[ext[0]].Name == "_xref")
                    {
                        uint hash = module.XRef[BitConverter.ToUInt16(ext, 2)];
                        str = hashes.ContainsKey(hash) ? hashes[hash] : $"#{hash:X8}";
                    }
                    else if (opcodes[ext[0]].Name == "_short")
                    {
                        ushort type = BitConverter.ToUInt16(ext, 2);
                        str = FlagLabels.StdTypes.ContainsKey(type) ? FlagLabels.StdTypes[type] : type.ToString();
                    }
                    else
                        str = $"{ext[0]:X2}{ext[1]:X2}{ext[2]:X2}{ext[3]:X2}";

                    text += $"\t\textend {str}\n";
                }

                if (obj.Extends.Count > 0 && (obj.Variables.Count > 0 || obj.Functions.Count > 0 || obj.Enums.Count > 0))
                    text += "\t\t\n";

                for (int o = 0; o < obj.Variables.Count; o++)
                {
                    var mVar = obj.Variables[o];
                    string flags = string.Join(' ', GetFlagLabels(mVar.Flags, ref FlagLabels.VariableFlags));
                    if (!string.IsNullOrWhiteSpace(flags))
                        flags += " ";

                    text += $"\t\t{flags}{mVar.Type} {mVar.Name};\n";
                }

                if (obj.Variables.Count > 0 && (obj.Functions.Count > 0 || obj.Enums.Count > 0))
                    text += "\t\t\n";

                for (int o = 0; o < obj.Functions.Count; o++)
                {
                    var mFunc = obj.Functions[o];
                    string flags = string.Join(' ', GetFlagLabels(mFunc.Flags, ref FlagLabels.FunctionFlags));
                    if (!string.IsNullOrWhiteSpace(flags))
                        flags += " ";

                    text += $"\t\t{flags}{mFunc.Name}\n\t\t{{\n";

                    text += "\t\t\t" + Disassemble(module, obj, mFunc, version, ref hashes).Trim().Replace("\n", "\n\t\t\t");

                    text += "\n\t\t}\n\t\t\n";
                }

                if (obj.Functions.Count > 0 && obj.Enums.Count > 0)
                    text += "\t\t\n";

                for (int o = 0; o < obj.Enums.Count; o++)
                {
                    var mEnum = obj.Enums[o];
                    string flags = string.Join(' ', GetFlagLabels(mEnum.Flags, ref FlagLabels.EnumFlags));
                    if (!string.IsNullOrWhiteSpace(flags))
                        flags += " ";

                    text += $"\t\t{flags}const {mEnum.Name} = 0x{mEnum.Value:X};\n";
                }

                text += "\t}\n";
            }

            text += "}";

            return text;
        }

        public static string Disassemble(ModuleRtDL module, byte[] version, ref Dictionary<uint, string> hashes)
        {
            var opcodes = MintVersions.Versions[version];

            string text = $"module {module.Name}\n{{\n";

            for (int i = 0; i < module.Objects.Count; i++)
            {
                var obj = module[i];

                text += $"\tclass {obj.Name}\n\t{{\n";

                for (int o = 0; o < obj.Variables.Count; o++)
                {
                    var mVar = obj.Variables[o];
                    text += $"\t\t{mVar.Type} {mVar.Name};\n";
                }

                for (int o = 0; o < obj.Functions.Count; o++)
                {
                    var mFunc = obj.Functions[o];
                    text += $"\t\t{mFunc.Name}\n\t\t{{\n";

                    text += "\t\t\t" + Disassemble(module, mFunc, version, ref hashes).Trim().Replace("\n", "\n\t\t\t");

                    text += "\n\t\t}\n\t\t\n";
                }

                text += "\t}\n";
            }

            text += "}";

            return text;
        }

        static readonly char[] token_delim = { ',' };
        static readonly char[] token_open = { '(', '\"', '[' };
        static readonly char[] token_close = { ')', '\"', ']' };

        // Really basic tokenize function
        public static string[] Tokenize(string text)
        {
            List<string> tokens = new List<string>();
            string token = "";
            int nest = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if ((char.IsWhiteSpace(text[i]) || token_delim.Contains(text[i])) && nest == 0)
                {
                    if (!string.IsNullOrWhiteSpace(token))
                        tokens.Add(token);
                    token = "";
                }
                else
                {
                    token += text[i];
                    if (token_open.Contains(text[i]) && text[i - 1] != '\\')
                        nest++;
                    else if (token_close.Contains(text[i]) && text[i - 1] != '\\')
                        nest--;
                }

                if (nest < 0)
                    throw new FormatException("Tokenize error: Malformatted token! Missing opening or closing character in parentheses or quotation mark pair.");

                if (token == "//")
                {
                    for (int c = i + 1; c < text.Length; c++)
                    {
                        if (c == text.Length - 1 || text[c] == '\n')
                        {
                            i = c;
                            break;
                        }
                    }
                    token = "";
                }
                else if (token == "/*")
                {
                    for (int c = i + 1; c < text.Length; c++)
                    {
                        if (c == text.Length - 1 || (text[c] == '*' && text[c + 1] == '/'))
                        {
                            i = c;
                            break;
                        }
                    }
                    token = "";
                }
            }

            if (!string.IsNullOrWhiteSpace(token))
                tokens.Add(token);

            return tokens.ToArray();
        }

        public static uint ParseFlags(string[] flags, Dictionary<uint, string> labels)
        {
            uint f = 0;

            for (int i = 0; i < flags.Length; i++)
            {
                if (labels.ContainsValue(flags[i]))
                    f |= labels.Keys.First(x => labels[x] == flags[i]);
                else if (flags[i].StartsWith("flag"))
                    f |= uint.Parse(flags[i].Substring(4), System.Globalization.NumberStyles.HexNumber);
            }

            return f;
        }

        public static byte[] ParseStringToken(string token)
        {
            bool utf16 = token.StartsWith("u");

            Encoding encoding = utf16 ? Encoding.Unicode : Encoding.UTF8;
            List<byte> raw = encoding.GetBytes(
                utf16 ? token[2..^1] : token[1..^1]
            ).ToList();
            raw.Add(0);
            if (utf16)
                raw.Add(0);

            while ((raw.Count % 4) != 0)
                raw.Add(0xFF);

            return raw.ToArray();
        }

        public static byte[] ParseValueToken(string token)
        {
            if (token.StartsWith("0x"))
                return BitConverter.GetBytes(uint.Parse(token[2..], System.Globalization.NumberStyles.HexNumber));
            else if (token.EndsWith('f'))
                return BitConverter.GetBytes(float.Parse(token[..^1]));
            else if (token.Contains('.'))
                return BitConverter.GetBytes(float.Parse(token));
            else
                return BitConverter.GetBytes(int.Parse(token));
        }

        public static byte[] ParseFloatToken(string token)
        {
            if (token.EndsWith("f"))
                return BitConverter.GetBytes(float.Parse(token.Substring(0, token.Length - 1)));
            else
                return BitConverter.GetBytes(float.Parse(token));
        }

        public static int SearchSData(Module module, byte[] value)
        {
            if (module.XData.Endianness == Endianness.Big)
                value = value.Reverse().ToArray();

            int index = -1;
            for (int s = 0; s < module.SData.Count; s += 4)
            {
                if (new ByteArrayComparer().Equals(value, module.SData.GetRange(s, 4).ToArray()))
                {
                    index = s;
                    break;
                }
            }

            if (index < 0)
            {
                index = module.SData.Count;
                module.SData.AddRange(value);
            }

            return index;
        }

        public static int SearchSData(ModuleRtDL module, byte[] value, bool ignoreEndianness = false)
        {
            if (!ignoreEndianness && module.XData.Endianness == Endianness.Big)
                value = value.Reverse().ToArray();

            int index = -1;
            for (int s = 0; s < module.SData.Count - value.Length; s += 4)
            {
                if (new ByteArrayComparer().Equals(value, module.SData.GetRange(s, 4).ToArray()))
                {
                    index = s;
                    break;
                }
            }

            if (index < 0)
            {
                index = module.SData.Count;
                module.SData.AddRange(value);
            }

            return index;
        }

        public static int SearchXRef(Module module, uint hash)
        {
            int index = -1;
            for (int x = 0; x < module.XRef.Count; x++)
            {
                if (module.XRef[x] == hash)
                {
                    index = x;
                    break;
                }
            }

            if (index < 0)
            {
                index = module.XRef.Count;
                module.XRef.Add(hash);
            }

            return index;
        }

        public static int SearchXRef(ModuleRtDL module, string xref)
        {
            int index = -1;
            for (int x = 0; x < module.XRef.Count; x++)
            {
                if (module.XRef[x] == xref)
                {
                    index = x;
                    break;
                }
            }

            if (index < 0)
            {
                index = module.XRef.Count;
                module.XRef.Add(xref);
            }

            return index;
        }

        public static void AssembleSData(Module module, List<string> text, byte[] version)
        {
            var opcodes = MintVersions.Versions[version];

            // Integer pass
            for (int i = 0; i < text.Count; i++)
            {
                string line = text[i];
                if (string.IsNullOrWhiteSpace(line) || line.EndsWith(':'))
                    continue;

                if (MintRegex.Raw().IsMatch(line))
                    continue;

                string[] tokens = Tokenize(line);

                Opcode op;
                {
                    var ow = opcodes.Where(x => x.Name == tokens[0].ToLower());
                    if (ow.Count() == 0)
                        throw new FormatException($"Unknown opcode {tokens[0]}!");
                    op = ow.First();
                }

                byte[] bytes = new byte[op.Size];
                Array.Copy(op.BaseData, bytes, op.Size);

                bytes[0] = (byte)opcodes.IndexOf(op);

                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    string token = tokens[a + 1];

                    switch (op.Arguments[a] & InstructionArg.AllTypes)
                    {
                        case InstructionArg.SDataInt:
                            {
                                if (!MintRegex.Value().IsMatch(token))
                                    throw new FormatException($"Operand {a} is not a value.\nLine: {line}");

                                SearchSData(module, ParseValueToken(token));

                                break;
                            }
                        case InstructionArg.SDataFloat:
                            {
                                if (!MintRegex.Float().IsMatch(token))
                                    throw new FormatException($"Operand {a} is not a float value.\nLine: {line}");

                                SearchSData(module, ParseFloatToken(token));

                                break;
                            }
                        case InstructionArg.SDataRegInt:
                            {
                                bool isValue = MintRegex.Value().IsMatch(token);
                                if (!isValue && !MintRegex.Register().IsMatch(token))
                                    throw new FormatException($"Operand {a} is neither a value or register.\nLine: {line}");

                                if (isValue)
                                    SearchSData(module, ParseValueToken(token));

                                break;
                            }
                        case InstructionArg.SDataRegFloat:
                            {
                                bool isValue = MintRegex.Float().IsMatch(token);
                                if (!isValue && !MintRegex.Register().IsMatch(token))
                                    throw new FormatException($"Operand {a} is neither a float value or register.\nLine: {line}");

                                if (isValue)
                                    SearchSData(module, ParseFloatToken(token));

                                break;
                            }
                    }
                }
            }

            // String/Array pass
            for (int i = 0; i < text.Count; i++)
            {
                string line = text[i];
                if (string.IsNullOrWhiteSpace(line) || line.EndsWith(':'))
                    continue;

                if (MintRegex.Raw().IsMatch(line))
                    continue;

                string[] tokens = Tokenize(line);

                Opcode op;
                {
                    var ow = opcodes.Where(x => x.Name == tokens[0].ToLower());
                    if (ow.Count() == 0)
                        throw new FormatException($"Unknown opcode {tokens[0]}!");
                    op = ow.First();
                }

                byte[] bytes = new byte[op.Size];
                Array.Copy(op.BaseData, bytes, op.Size);

                bytes[0] = (byte)opcodes.IndexOf(op);

                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    string token = tokens[a + 1];
                    switch (op.Arguments[a] & InstructionArg.AllTypes)
                    {
                        case InstructionArg.SDataArray:
                            {
                                if (!MintRegex.String().IsMatch(token))
                                    throw new FormatException($"Operand {a} is not a string.\nLine: {line}");

                                SearchSData(module, ParseStringToken(token));

                                break;
                            }
                        case InstructionArg.SDataRegArray:
                            {
                                bool isValue = MintRegex.String().IsMatch(token);
                                if (!isValue && !MintRegex.Register().IsMatch(token))
                                    throw new FormatException($"Operand {a} is neither a string or register.\nLine: {line}");

                                if (isValue)
                                    SearchSData(module, ParseStringToken(token));

                                break;
                            }
                    }
                }
            }
        }

        public static byte[] AssembleFunction(Module module, MintObject obj, List<string> text, byte[] version)
        {
            // Prep function
            for (int i = 0; i < text.Count; i++)
            {
                text[i] = text[i].Trim();

                if (string.IsNullOrWhiteSpace(text[i]))
                {
                    text.RemoveAt(i);
                    i--;
                }
            }

            List<byte> data = new List<byte>();

            var opcodes = MintVersions.Versions[version];

            // Get code labels and populate instruction offsets
            Dictionary<string, int> labels = new Dictionary<string, int>();
            List<int> instOffsets = new List<int>();
            int offset = 0;
            for (int i = 0; i < text.Count; i++)
            {
                string line = text[i];
                if (line.EndsWith(":"))
                {
                    labels.Add(line.Substring(0, line.Length - 1), i);
                    text.RemoveAt(i);
                    i--;
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    if (MintRegex.Raw().IsMatch(line))
                    {
                        instOffsets.Add(offset);
                        offset += 4;
                        continue;
                    }

                    string[] tokens = Tokenize(line);
                    Opcode op;
                    {
                        var ow = opcodes.Where(x => x.Name == tokens[0].ToLower());
                        if (ow.Count() == 0)
                            throw new FormatException($"Unknown opcode {tokens[0]}!");
                        op = ow.First();
                    }

                    instOffsets.Add(offset);
                    offset += op.Size;
                }
            }

            // Fix up jump instructions
            for (int i = 0; i < text.Count; i++)
            {
                string line = text[i];

                if (MintRegex.Raw().IsMatch(line))
                    continue;

                string[] tokens = Tokenize(line);

                Opcode op;
                {
                    var ow = opcodes.Where(x => x.Name == tokens[0].ToLower());
                    if (ow.Count() == 0)
                        continue;
                    op = ow.First();
                }

                if (!op.Action.HasFlag(Mint.Action.Jump))
                    continue;

                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    string token = tokens[a + 1];

                    if (op.Arguments[a] == InstructionArg.VSigned
                        || op.Arguments[a] == InstructionArg.ESigned)
                    {
                        if (!labels.ContainsKey(token))
                            throw new FormatException($"Label {token} does not exist.\nLine: {line}");

                        tokens[a + 1] =
                            op.Arguments[a] == InstructionArg.ESigned
                            ? ((instOffsets[labels[token]] - (instOffsets[i] + 4)) / 4).ToString()
                            : ((instOffsets[labels[token]] - (instOffsets[i])) / 4).ToString();
                    }
                }

                text[i] = string.Join(' ', tokens);
            }

            // Actually assemble
            for (int i = 0; i < text.Count; i++)
            {
                string line = text[i];

                var raw = MintRegex.Raw().Match(line);
                if (raw.Success)
                {
                    data.Add(byte.Parse(raw.Groups[1].Value, System.Globalization.NumberStyles.HexNumber));
                    data.Add(byte.Parse(raw.Groups[2].Value, System.Globalization.NumberStyles.HexNumber));
                    data.Add(byte.Parse(raw.Groups[3].Value, System.Globalization.NumberStyles.HexNumber));
                    data.Add(byte.Parse(raw.Groups[4].Value, System.Globalization.NumberStyles.HexNumber));
                    continue;
                }

                string[] tokens = Tokenize(line);

                Opcode op;
                {
                    var ow = opcodes.Where(x => x.Name == tokens[0].ToLower());
                    if (ow.Count() == 0)
                        throw new FormatException($"Unknown opcode {tokens[0]}!");
                    op = ow.First();
                }

                byte[] bytes = new byte[op.Size];
                Array.Copy(op.BaseData, bytes, op.Size);

                bytes[0] = (byte)opcodes.IndexOf(op);

                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    string token = tokens[a + 1];

                    var arg = op.Arguments[a];
                    var argType = arg & InstructionArg.AllTypes;
                    var argLoc = arg & InstructionArg.AllData;
                    switch (argType)
                    {
                        default:
                            {
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        bytes[1] = byte.Parse(token);
                                        break;
                                    case InstructionArg.X:
                                        bytes[2] = byte.Parse(token);
                                        break;
                                    case InstructionArg.Y:
                                        bytes[3] = byte.Parse(token);
                                        break;
                                    case InstructionArg.V:
                                        byte[] v = arg.HasFlag(InstructionArg.Signed)
                                            ? BitConverter.GetBytes(short.Parse(token))
                                            : BitConverter.GetBytes(ushort.Parse(token));
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = v.Reverse().ToArray();
                                        Array.Copy(v, 0, bytes, 2, 2);
                                        break;
                                    case InstructionArg.A:
                                        bytes[5] = byte.Parse(token);
                                        break;
                                    case InstructionArg.B:
                                        bytes[6] = byte.Parse(token);
                                        break;
                                    case InstructionArg.C:
                                        bytes[7] = byte.Parse(token);
                                        break;
                                    case InstructionArg.E:
                                        byte[] e = arg.HasFlag(InstructionArg.Signed)
                                            ? BitConverter.GetBytes(short.Parse(token))
                                            : BitConverter.GetBytes(ushort.Parse(token));
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = e.Reverse().ToArray();
                                        Array.Copy(e, 0, bytes, 6, 2);
                                        break;
                                }
                                break;
                            }
                        case InstructionArg.Register:
                            {
                                if (!MintRegex.Register().IsMatch(token))
                                    throw new FormatException($"Operand {a} is not a register.\nLine: {line}");

                                byte reg = byte.Parse(token.Substring(1));
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        bytes[1] = reg;
                                        break;
                                    case InstructionArg.X:
                                        bytes[2] = reg;
                                        break;
                                    case InstructionArg.Y:
                                        bytes[3] = reg;
                                        break;
                                    case InstructionArg.A:
                                        bytes[5] = reg;
                                        break;
                                    case InstructionArg.B:
                                        bytes[6] = reg;
                                        break;
                                    case InstructionArg.C:
                                        bytes[7] = reg;
                                        break;
                                }
                                break;
                            }
                        case InstructionArg.SDataInt:
                            {
                                if (!MintRegex.Value().IsMatch(token))
                                    throw new FormatException($"Operand {a} is not a value.\nLine: {line}");

                                int index = SearchSData(module, ParseValueToken(token));

                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        bytes[1] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.X:
                                        bytes[2] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.Y:
                                        bytes[3] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.V:
                                        byte[] v = BitConverter.GetBytes((ushort)index);
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = v.Reverse().ToArray();
                                        Array.Copy(v, 0, bytes, 2, 2);
                                        break;
                                    case InstructionArg.A:
                                        bytes[5] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.B:
                                        bytes[6] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.C:
                                        bytes[7] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.E:
                                        byte[] e = BitConverter.GetBytes((ushort)index);
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = e.Reverse().ToArray();
                                        Array.Copy(e, 0, bytes, 6, 2);
                                        break;
                                }

                                break;
                            }
                        case InstructionArg.SDataFloat:
                            {
                                if (!MintRegex.Float().IsMatch(token))
                                    throw new FormatException($"Operand {a} is not a float value.\nLine: {line}");

                                int index = SearchSData(module, ParseFloatToken(token));

                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        bytes[1] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.X:
                                        bytes[2] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.Y:
                                        bytes[3] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.V:
                                        byte[] v = BitConverter.GetBytes((ushort)index);
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = v.Reverse().ToArray();
                                        Array.Copy(v, 0, bytes, 2, 2);
                                        break;
                                    case InstructionArg.A:
                                        bytes[5] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.B:
                                        bytes[6] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.C:
                                        bytes[7] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.E:
                                        byte[] e = BitConverter.GetBytes((ushort)index);
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = e.Reverse().ToArray();
                                        Array.Copy(e, 0, bytes, 6, 2);
                                        break;
                                }

                                break;
                            }
                        case InstructionArg.SDataArray:
                            {
                                if (!MintRegex.String().IsMatch(token))
                                    throw new FormatException($"Operand {a} is not a string.\nLine: {line}");

                                int index = SearchSData(module, ParseStringToken(token));

                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        bytes[1] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.X:
                                        bytes[2] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.Y:
                                        bytes[3] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.V:
                                        byte[] v = BitConverter.GetBytes((ushort)index);
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = v.Reverse().ToArray();
                                        Array.Copy(v, 0, bytes, 2, 2);
                                        break;
                                    case InstructionArg.A:
                                        bytes[5] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.B:
                                        bytes[6] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.C:
                                        bytes[7] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.E:
                                        byte[] e = BitConverter.GetBytes((ushort)index);
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = e.Reverse().ToArray();
                                        Array.Copy(e, 0, bytes, 6, 2);
                                        break;
                                }

                                break;
                            }
                        case InstructionArg.SDataRegInt:
                            {
                                bool isValue = MintRegex.Value().IsMatch(token);
                                if (!isValue && !MintRegex.Register().IsMatch(token))
                                    throw new FormatException($"Operand {a} is neither a value or register.\nLine: {line}");

                                byte val;
                                if (isValue)
                                    val = (byte)((SearchSData(module, ParseValueToken(token)) / 4) | 0x80);
                                else
                                    val = byte.Parse(token.Substring(1));

                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        bytes[1] = val;
                                        break;
                                    case InstructionArg.X:
                                        bytes[2] = val;
                                        break;
                                    case InstructionArg.Y:
                                        bytes[3] = val;
                                        break;
                                    case InstructionArg.A:
                                        bytes[5] = val;
                                        break;
                                    case InstructionArg.B:
                                        bytes[6] = val;
                                        break;
                                    case InstructionArg.C:
                                        bytes[7] = val;
                                        break;
                                }

                                break;
                            }
                        case InstructionArg.SDataRegFloat:
                            {
                                bool isValue = MintRegex.Float().IsMatch(token);
                                if (!isValue && !MintRegex.Register().IsMatch(token))
                                    throw new FormatException($"Operand {a} is neither a float value or register.\nLine: {line}");

                                byte val;
                                if (isValue)
                                    val = (byte)((SearchSData(module, ParseFloatToken(token)) / 4) | 0x80);
                                else
                                    val = byte.Parse(token.Substring(1));

                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        bytes[1] = val;
                                        break;
                                    case InstructionArg.X:
                                        bytes[2] = val;
                                        break;
                                    case InstructionArg.Y:
                                        bytes[3] = val;
                                        break;
                                    case InstructionArg.A:
                                        bytes[5] = val;
                                        break;
                                    case InstructionArg.B:
                                        bytes[6] = val;
                                        break;
                                    case InstructionArg.C:
                                        bytes[7] = val;
                                        break;
                                }

                                break;
                            }
                        case InstructionArg.SDataRegArray:
                            {
                                bool isValue = MintRegex.String().IsMatch(token);
                                if (!isValue && !MintRegex.Register().IsMatch(token))
                                    throw new FormatException($"Operand {a} is neither a string or register.\nLine: {line}");

                                byte val;
                                if (isValue)
                                    val = (byte)((SearchSData(module, ParseStringToken(token)) / 4) | 0x80);
                                else
                                    val = byte.Parse(token.Substring(1));

                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        bytes[1] = val;
                                        break;
                                    case InstructionArg.X:
                                        bytes[2] = val;
                                        break;
                                    case InstructionArg.Y:
                                        bytes[3] = val;
                                        break;
                                    case InstructionArg.A:
                                        bytes[5] = val;
                                        break;
                                    case InstructionArg.B:
                                        bytes[6] = val;
                                        break;
                                    case InstructionArg.C:
                                        bytes[7] = val;
                                        break;
                                }

                                break;
                            }
                        case InstructionArg.XRef:
                            {
                                if (token.StartsWith("this.") || token.StartsWith("this/") || token == "this")
                                    token = obj.Name + token[4..];

                                var match = MintRegex.Hash().Match(token);

                                uint hash = match.Success
                                    ? uint.Parse(match.Groups[1].Value, System.Globalization.NumberStyles.HexNumber)
                                    : Crc32C.CalculateInv(token);

                                int index = SearchXRef(module, hash);

                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        bytes[1] = (byte)index;
                                        break;
                                    case InstructionArg.X:
                                        bytes[2] = (byte)index;
                                        break;
                                    case InstructionArg.Y:
                                        bytes[3] = (byte)index;
                                        break;
                                    case InstructionArg.V:
                                        byte[] v = BitConverter.GetBytes((ushort)index);
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = v.Reverse().ToArray();
                                        Array.Copy(v, 0, bytes, 2, 2);
                                        break;
                                    case InstructionArg.A:
                                        bytes[5] = (byte)index;
                                        break;
                                    case InstructionArg.B:
                                        bytes[6] = (byte)index;
                                        break;
                                    case InstructionArg.C:
                                        bytes[7] = (byte)index;
                                        break;
                                    case InstructionArg.E:
                                        byte[] e = BitConverter.GetBytes((ushort)index);
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = e.Reverse().ToArray();
                                        Array.Copy(e, 0, bytes, 6, 2);
                                        break;
                                }

                                break;
                            }
                    }
                }

                data.AddRange(bytes);
            }

            return data.ToArray();
        }

        public static void AssembleSData(ModuleRtDL module, List<string> text, byte[] version)
        {
            var opcodes = MintVersions.Versions[version];

            // Integer pass
            for (int i = 0; i < text.Count; i++)
            {
                string line = text[i];

                if (MintRegex.Raw().IsMatch(line))
                    continue;

                string[] tokens = Tokenize(line);

                Opcode op;
                {
                    var ow = opcodes.Where(x => x.Name == tokens[0].ToLower());
                    if (ow.Count() == 0)
                        throw new FormatException($"Unknown opcode {tokens[0]}!");
                    op = ow.First();
                }

                byte[] bytes = new byte[op.Size];
                Array.Copy(op.BaseData, bytes, op.Size);

                bytes[0] = (byte)opcodes.IndexOf(op);

                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    string token = tokens[a + 1];

                    switch (op.Arguments[a] & InstructionArg.AllTypes)
                    {
                        case InstructionArg.SDataInt:
                            {
                                if (!MintRegex.Value().IsMatch(token))
                                    throw new FormatException($"Operand {a} is not a value.\nLine: {line}");

                                SearchSData(module, ParseValueToken(token));

                                break;
                            }
                        case InstructionArg.SDataFloat:
                            {
                                if (!MintRegex.Float().IsMatch(token))
                                    throw new FormatException($"Operand {a} is not a float value.\nLine: {line}");

                                SearchSData(module, ParseFloatToken(token));

                                break;
                            }
                        case InstructionArg.SDataRegInt:
                            {
                                bool isValue = MintRegex.Value().IsMatch(token);
                                if (!isValue && !MintRegex.Register().IsMatch(token))
                                    throw new FormatException($"Operand {a} is neither a value or register.\nLine: {line}");

                                if (isValue)
                                    SearchSData(module, ParseValueToken(token));

                                break;
                            }
                        case InstructionArg.SDataRegFloat:
                            {
                                bool isValue = MintRegex.Float().IsMatch(token);
                                if (!isValue && !MintRegex.Register().IsMatch(token))
                                    throw new FormatException($"Operand {a} is neither a float value or register.\nLine: {line}");

                                if (isValue)
                                    SearchSData(module, ParseFloatToken(token));

                                break;
                            }
                    }
                }
            }

            // String/Array pass
            for (int i = 0; i < text.Count; i++)
            {
                string line = text[i];

                if (MintRegex.Raw().IsMatch(line))
                    continue;

                string[] tokens = Tokenize(line);

                Opcode op;
                {
                    var ow = opcodes.Where(x => x.Name == tokens[0].ToLower());
                    if (ow.Count() == 0)
                        throw new FormatException($"Unknown opcode {tokens[0]}!");
                    op = ow.First();
                }

                byte[] bytes = new byte[op.Size];
                Array.Copy(op.BaseData, bytes, op.Size);

                bytes[0] = (byte)opcodes.IndexOf(op);

                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    string token = tokens[a + 1];
                    switch (op.Arguments[a] & InstructionArg.AllTypes)
                    {
                        case InstructionArg.SDataArray:
                            {
                                if (!MintRegex.String().IsMatch(token))
                                    throw new FormatException($"Operand {a} is not a string.\nLine: {line}");

                                SearchSData(module, ParseStringToken(token), true);

                                break;
                            }
                        case InstructionArg.SDataRegArray:
                            {
                                bool isValue = MintRegex.String().IsMatch(token);
                                if (!isValue && !MintRegex.Register().IsMatch(token))
                                    throw new FormatException($"Operand {a} is neither a string or register.\nLine: {line}");

                                if (isValue)
                                    SearchSData(module, ParseStringToken(token), true);

                                break;
                            }
                    }
                }
            }
        }

        public static byte[] AssembleFunction(ModuleRtDL module, List<string> text, byte[] version)
        {
            // Prep function
            for (int i = 0; i < text.Count; i++)
            {
                text[i] = text[i].Trim();

                if (string.IsNullOrWhiteSpace(text[i]))
                {
                    text.RemoveAt(i);
                    i--;
                }
            }

            List<byte> data = new List<byte>();

            var opcodes = MintVersions.Versions[version];

            // Get code labels and populate instruction offsets
            Dictionary<string, int> labels = new Dictionary<string, int>();
            List<int> instOffsets = new List<int>();
            int offset = 0;
            for (int i = 0; i < text.Count; i++)
            {
                string line = text[i];
                if (line.EndsWith(":"))
                {
                    labels.Add(line.Substring(0, line.Length - 1), i);
                    text.RemoveAt(i);
                    i--;
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    if (MintRegex.Raw().IsMatch(line))
                    {
                        instOffsets.Add(offset);
                        offset += 4;
                        continue;
                    }

                    string[] tokens = Tokenize(line);
                    Opcode op;
                    {
                        var ow = opcodes.Where(x => x.Name == tokens[0].ToLower());
                        if (ow.Count() == 0)
                            throw new FormatException($"Unknown opcode {tokens[0]}!");
                        op = ow.First();
                    }

                    instOffsets.Add(offset);
                    offset += op.Size;
                }
            }

            // Fix up jump instructions
            for (int i = 0; i < text.Count; i++)
            {
                string line = text[i];

                if (MintRegex.Raw().IsMatch(line))
                    continue;

                string[] tokens = Tokenize(line);

                Opcode op;
                {
                    var ow = opcodes.Where(x => x.Name == tokens[0].ToLower());
                    if (ow.Count() == 0)
                        continue;
                    op = ow.First();
                }

                if (!op.Action.HasFlag(Mint.Action.Jump))
                    continue;

                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    string token = tokens[a + 1];

                    if (op.Arguments[a] == InstructionArg.VSigned
                        || op.Arguments[a] == InstructionArg.ESigned)
                    {
                        if (!labels.ContainsKey(token))
                            throw new FormatException($"Label {token} does not exist.\nLine: {line}");

                        tokens[a + 1] =
                            op.Arguments[a] == InstructionArg.ESigned
                            ? ((instOffsets[labels[token]] - (instOffsets[i] + 4)) / 4).ToString()
                            : ((instOffsets[labels[token]] - (instOffsets[i])) / 4).ToString();
                    }
                }

                text[i] = string.Join(' ', tokens);
            }

            // Actually assemble
            for (int i = 0; i < text.Count; i++)
            {
                string line = text[i];

                var raw = MintRegex.Raw().Match(line);
                if (raw.Success)
                {
                    data.Add(byte.Parse(raw.Groups[1].Value, System.Globalization.NumberStyles.HexNumber));
                    data.Add(byte.Parse(raw.Groups[2].Value, System.Globalization.NumberStyles.HexNumber));
                    data.Add(byte.Parse(raw.Groups[3].Value, System.Globalization.NumberStyles.HexNumber));
                    data.Add(byte.Parse(raw.Groups[4].Value, System.Globalization.NumberStyles.HexNumber));
                    continue;
                }

                string[] tokens = Tokenize(line);

                Opcode op;
                {
                    var ow = opcodes.Where(x => x.Name == tokens[0].ToLower());
                    if (ow.Count() == 0)
                        throw new FormatException($"Unknown opcode {tokens[0]}!");
                    op = ow.First();
                }

                byte[] bytes = new byte[op.Size];
                Array.Copy(op.BaseData, bytes, op.Size);

                bytes[0] = (byte)opcodes.IndexOf(op);

                for (int a = 0; a < op.Arguments.Length; a++)
                {
                    string token = tokens[a + 1];

                    var arg = op.Arguments[a];
                    var argType = arg & InstructionArg.AllTypes;
                    var argLoc = arg & InstructionArg.AllData;
                    switch (argType)
                    {
                        default:
                            {
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        bytes[1] = byte.Parse(token);
                                        break;
                                    case InstructionArg.X:
                                        bytes[2] = byte.Parse(token);
                                        break;
                                    case InstructionArg.Y:
                                        bytes[3] = byte.Parse(token);
                                        break;
                                    case InstructionArg.V:
                                        byte[] v = arg.HasFlag(InstructionArg.Signed)
                                            ? BitConverter.GetBytes(short.Parse(token))
                                            : BitConverter.GetBytes(ushort.Parse(token));
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = v.Reverse().ToArray();
                                        Array.Copy(v, 0, bytes, 2, 2);
                                        break;
                                    case InstructionArg.A:
                                        bytes[5] = byte.Parse(token);
                                        break;
                                    case InstructionArg.B:
                                        bytes[6] = byte.Parse(token);
                                        break;
                                    case InstructionArg.C:
                                        bytes[7] = byte.Parse(token);
                                        break;
                                    case InstructionArg.E:
                                        byte[] e = arg.HasFlag(InstructionArg.Signed)
                                            ? BitConverter.GetBytes(short.Parse(token))
                                            : BitConverter.GetBytes(ushort.Parse(token));
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = e.Reverse().ToArray();
                                        Array.Copy(e, 0, bytes, 6, 2);
                                        break;
                                }
                                break;
                            }
                        case InstructionArg.Register:
                            {
                                if (!MintRegex.Register().IsMatch(token))
                                    throw new FormatException($"Operand {a} is not a register.\nLine: {line}");

                                byte reg = byte.Parse(token.Substring(1));
                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        bytes[1] = reg;
                                        break;
                                    case InstructionArg.X:
                                        bytes[2] = reg;
                                        break;
                                    case InstructionArg.Y:
                                        bytes[3] = reg;
                                        break;
                                    case InstructionArg.A:
                                        bytes[5] = reg;
                                        break;
                                    case InstructionArg.B:
                                        bytes[6] = reg;
                                        break;
                                    case InstructionArg.C:
                                        bytes[7] = reg;
                                        break;
                                }
                                break;
                            }
                        case InstructionArg.SDataInt:
                            {
                                if (!MintRegex.Value().IsMatch(token))
                                    throw new FormatException($"Operand {a} is not a value.\nLine: {line}");

                                int index = SearchSData(module, ParseValueToken(token));

                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        bytes[1] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.X:
                                        bytes[2] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.Y:
                                        bytes[3] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.V:
                                        byte[] v = BitConverter.GetBytes((ushort)index);
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = v.Reverse().ToArray();
                                        Array.Copy(v, 0, bytes, 2, 2);
                                        break;
                                    case InstructionArg.A:
                                        bytes[5] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.B:
                                        bytes[6] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.C:
                                        bytes[7] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.E:
                                        byte[] e = BitConverter.GetBytes((ushort)index);
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = e.Reverse().ToArray();
                                        Array.Copy(e, 0, bytes, 6, 2);
                                        break;
                                }

                                break;
                            }
                        case InstructionArg.SDataFloat:
                            {
                                if (!MintRegex.Float().IsMatch(token))
                                    throw new FormatException($"Operand {a} is not a float value.\nLine: {line}");

                                int index = SearchSData(module, ParseFloatToken(token));

                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        bytes[1] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.X:
                                        bytes[2] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.Y:
                                        bytes[3] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.V:
                                        byte[] v = BitConverter.GetBytes((ushort)index);
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = v.Reverse().ToArray();
                                        Array.Copy(v, 0, bytes, 2, 2);
                                        break;
                                    case InstructionArg.A:
                                        bytes[5] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.B:
                                        bytes[6] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.C:
                                        bytes[7] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.E:
                                        byte[] e = BitConverter.GetBytes((ushort)index);
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = e.Reverse().ToArray();
                                        Array.Copy(e, 0, bytes, 6, 2);
                                        break;
                                }

                                break;
                            }
                        case InstructionArg.SDataArray:
                            {
                                if (!MintRegex.String().IsMatch(token))
                                    throw new FormatException($"Operand {a} is not a string.\nLine: {line}");

                                int index = SearchSData(module, ParseStringToken(token), true);

                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        bytes[1] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.X:
                                        bytes[2] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.Y:
                                        bytes[3] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.V:
                                        byte[] v = BitConverter.GetBytes((ushort)index);
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = v.Reverse().ToArray();
                                        Array.Copy(v, 0, bytes, 2, 2);
                                        break;
                                    case InstructionArg.A:
                                        bytes[5] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.B:
                                        bytes[6] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.C:
                                        bytes[7] = (byte)(index / 4);
                                        break;
                                    case InstructionArg.E:
                                        byte[] e = BitConverter.GetBytes((ushort)index);
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = e.Reverse().ToArray();
                                        Array.Copy(e, 0, bytes, 6, 2);
                                        break;
                                }

                                break;
                            }
                        case InstructionArg.SDataRegInt:
                            {
                                bool isValue = MintRegex.Value().IsMatch(token);
                                if (!isValue && !MintRegex.Register().IsMatch(token))
                                    throw new FormatException($"Operand {a} is neither a value or register.\nLine: {line}");

                                byte val;
                                if (isValue)
                                    val = (byte)((SearchSData(module, ParseValueToken(token)) / 4) | 0x80);
                                else
                                    val = byte.Parse(token.Substring(1));

                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        bytes[1] = val;
                                        break;
                                    case InstructionArg.X:
                                        bytes[2] = val;
                                        break;
                                    case InstructionArg.Y:
                                        bytes[3] = val;
                                        break;
                                    case InstructionArg.A:
                                        bytes[5] = val;
                                        break;
                                    case InstructionArg.B:
                                        bytes[6] = val;
                                        break;
                                    case InstructionArg.C:
                                        bytes[7] = val;
                                        break;
                                }

                                break;
                            }
                        case InstructionArg.SDataRegFloat:
                            {
                                bool isValue = MintRegex.Float().IsMatch(token);
                                if (!isValue && !MintRegex.Register().IsMatch(token))
                                    throw new FormatException($"Operand {a} is neither a float value or register.\nLine: {line}");

                                byte val;
                                if (isValue)
                                    val = (byte)((SearchSData(module, ParseFloatToken(token)) / 4) | 0x80);
                                else
                                    val = byte.Parse(token.Substring(1));

                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        bytes[1] = val;
                                        break;
                                    case InstructionArg.X:
                                        bytes[2] = val;
                                        break;
                                    case InstructionArg.Y:
                                        bytes[3] = val;
                                        break;
                                    case InstructionArg.A:
                                        bytes[5] = val;
                                        break;
                                    case InstructionArg.B:
                                        bytes[6] = val;
                                        break;
                                    case InstructionArg.C:
                                        bytes[7] = val;
                                        break;
                                }

                                break;
                            }
                        case InstructionArg.SDataRegArray:
                            {
                                bool isValue = MintRegex.String().IsMatch(token);
                                if (!isValue && !MintRegex.Register().IsMatch(token))
                                    throw new FormatException($"Operand {a} is neither a string or register.\nLine: {line}");

                                byte val;
                                if (isValue)
                                    val = (byte)((SearchSData(module, ParseStringToken(token), true) / 4) | 0x80);
                                else
                                    val = byte.Parse(token.Substring(1));

                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        bytes[1] = val;
                                        break;
                                    case InstructionArg.X:
                                        bytes[2] = val;
                                        break;
                                    case InstructionArg.Y:
                                        bytes[3] = val;
                                        break;
                                    case InstructionArg.A:
                                        bytes[5] = val;
                                        break;
                                    case InstructionArg.B:
                                        bytes[6] = val;
                                        break;
                                    case InstructionArg.C:
                                        bytes[7] = val;
                                        break;
                                }

                                break;
                            }
                        case InstructionArg.XRef:
                            {
                                int index = SearchXRef(module, token);

                                switch (argLoc)
                                {
                                    case InstructionArg.Z:
                                        bytes[1] = (byte)index;
                                        break;
                                    case InstructionArg.X:
                                        bytes[2] = (byte)index;
                                        break;
                                    case InstructionArg.Y:
                                        bytes[3] = (byte)index;
                                        break;
                                    case InstructionArg.V:
                                        byte[] v = BitConverter.GetBytes((ushort)index);
                                        if (module.XData.Endianness == Endianness.Big)
                                            v = v.Reverse().ToArray();
                                        Array.Copy(v, 0, bytes, 2, 2);
                                        break;
                                    case InstructionArg.A:
                                        bytes[5] = (byte)index;
                                        break;
                                    case InstructionArg.B:
                                        bytes[6] = (byte)index;
                                        break;
                                    case InstructionArg.C:
                                        bytes[7] = (byte)index;
                                        break;
                                    case InstructionArg.E:
                                        byte[] e = BitConverter.GetBytes((ushort)index);
                                        if (module.XData.Endianness == Endianness.Big)
                                            e = e.Reverse().ToArray();
                                        Array.Copy(e, 0, bytes, 6, 2);
                                        break;
                                }

                                break;
                            }
                    }
                }

                data.AddRange(bytes);
            }

            return data.ToArray();
        }

        public static string StripComments(string text)
        {
            var lineComments = MintRegex.Comment().Matches(text);
            for (int i = 0; i < lineComments.Count; i++)
                text = text.Replace(lineComments[i].Value, "");

            var multiComments = MintRegex.MultilineComment().Matches(text);
            for (int i = 0; i < multiComments.Count; i++)
                text = text.Replace(multiComments[i].Value, "");

            return text;
        }

        public static Module Assemble(string[] text, byte[] version)
        {
            Module module = new Module();

            var opcodes = MintVersions.Versions[version].ToList();

            // Find module declaration
            for (int i = 0; i < text.Length; i++)
            {
                string line = text[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var match = MintRegex.Module().Match(line);
                if (match.Success)
                {
                    module.Name = match.Groups[1].Value;
                    break;
                }
                else if (i == text.Length - 1)
                    throw new FormatException("Could not find module declaration!");
            }

            // Find function declarations to fill SData and other preliminary stuff
            List<string> inst = new List<string>();
            for (int i = 0; i < text.Length; i++)
            {
                string line = text[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var match = MintRegex.SData().Match(line);
                if (match.Success)
                {
                    string str = match.Groups[1].Value;
                    if (MintRegex.String().IsMatch(str))
                        SearchSData(module, ParseStringToken(str));
                    else
                        SearchSData(module, ParseValueToken(str));
                    continue;
                }

                match = MintRegex.XRef().Match(line);
                if (match.Success)
                {
                    string str = match.Groups[1].Value;
                    if (MintRegex.Hash().IsMatch(str))
                        SearchXRef(module, uint.Parse(str.Substring(1), System.Globalization.NumberStyles.HexNumber));
                    else
                        SearchXRef(module, Crc32C.CalculateInv(str));
                    continue;
                }

                match = MintRegex.Function().Match(line);
                if (match.Success)
                {
                    for (; i < text.Length; i++)
                    {
                        if (text[i].Trim().EndsWith("{"))
                            break;
                    }

                    i++;
                    for (; i < text.Length; i++)
                    {
                        if (text[i].Trim().EndsWith("}"))
                            break;
                        else
                            inst.Add(text[i].Trim());
                    }
                }
            }

            AssembleSData(module, inst, version);

            for (int i = 0; i < text.Length; i++)
            {
                string line = text[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("UnkHash = "))
                {
                    string unkHash = line[10..];
                    var hashMatch = MintRegex.Hash().Match(unkHash);

                    uint hash = hashMatch.Success
                        ? uint.Parse(hashMatch.Groups[1].Value, System.Globalization.NumberStyles.HexNumber)
                        : Crc32C.CalculateInv(unkHash);

                    module.UnkHash = hash;
                }

                var match = MintRegex.Object().Match(line);
                if (match.Success)
                {
                    MintObject obj = new MintObject();
                    obj.Name = match.Groups[1].Value;

                    string[] objFlags = line.Substring(0, match.Groups[1].Index).Split(' ').ToArray();
                    obj.Flags = ParseFlags(objFlags, FlagLabels.ClassFlags);

                    if (line.Contains(':'))
                    {
                        string[] inheritance = Tokenize(line[(line.IndexOf(':') + 1)..]);
                        for (int h = 0; h < inheritance.Length; h++)
                        {
                            var hashMatch = MintRegex.Hash().Match(inheritance[h]);

                            uint hash = hashMatch.Success
                                ? uint.Parse(hashMatch.Groups[1].Value, System.Globalization.NumberStyles.HexNumber)
                                : Crc32C.CalculateInv(inheritance[h]);

                            obj.Implements.Add(hash);
                        }
                    }

                    while (!line.EndsWith("{"))
                        line = text[i++].Trim();

                    for (; i < text.Length; i++)
                    {
                        line = text[i].Trim();
                        if (line.EndsWith("}"))
                            break;

                        if (line.StartsWith("extend "))
                        {
                            string[] tokens = Tokenize(line);
                            byte[] b = { 0xFF, 0xFF, 0x00, 0x00 };
                            if (ushort.TryParse(tokens[1], out ushort stdRaw))
                            {
                                b[0] = (byte)opcodes.FindIndex(x => x.Name == "_short");
                                Array.Copy(BitConverter.GetBytes(stdRaw), 0, b, 2, 2);
                            }
                            else if (FlagLabels.StdTypes.ContainsValue(tokens[1]))
                            {
                                b[0] = (byte)opcodes.FindIndex(x => x.Name == "_short");
                                Array.Copy(BitConverter.GetBytes(FlagLabels.StdTypes.FirstOrDefault(x => x.Value == tokens[1]).Key), 0, b, 2, 2);
                            }
                            else
                            {
                                b[0] = (byte)opcodes.FindIndex(x => x.Name == "_xref");
                                Array.Copy(BitConverter.GetBytes((ushort)SearchXRef(module, Crc32C.CalculateInv(tokens[1]))), 0, b, 2, 2);
                            }

                            obj.Extends.Add(b);
                        }

                        var objMatch = MintRegex.Variable().Match(line);
                        if (objMatch.Success)
                        {
                            string type = objMatch.Groups[1].Value;
                            string name = objMatch.Groups[2].Value;
                            MintVariable mVar = new MintVariable(type, name);

                            string[] flags = line.Substring(0, objMatch.Groups[1].Index).Split(' ').ToArray();
                            mVar.Flags = ParseFlags(flags, FlagLabels.VariableFlags);

                            obj.Variables.Add(mVar);

                            continue;
                        }

                        objMatch = MintRegex.Function().Match(line);
                        if (objMatch.Success)
                        {
                            string name = objMatch.Groups[0].Value;
                            MintFunction mFunc = new MintFunction(name);

                            string[] flags = line.Substring(0, objMatch.Groups[0].Index).Split(' ').ToArray();
                            mFunc.Flags = ParseFlags(flags, FlagLabels.FunctionFlags);

                            while (!line.EndsWith("{"))
                                line = text[i++].Trim();

                            List<string> funcInst = new List<string>();
                            for (; i < text.Length; i++)
                            {
                                line = text[i].Trim();
                                if (line.EndsWith("}"))
                                    break;
                                funcInst.Add(line);
                            }

                            mFunc.Data = AssembleFunction(module, obj, funcInst, version);

                            obj.Functions.Add(mFunc);

                            continue;
                        }

                        objMatch = MintRegex.Enum().Match(line);
                        if (objMatch.Success)
                        {
                            string name = objMatch.Groups[1].Value;
                            string value = objMatch.Groups[2].Value;

                            int val = value.StartsWith("0x")
                                ? int.Parse(value.Substring(2), System.Globalization.NumberStyles.HexNumber)
                                : int.Parse(value);

                            MintEnum mEnum = new MintEnum(name, val);

                            string[] flags = line.Substring(0, objMatch.Groups[1].Index - 6).Split(' ').ToArray();
                            mEnum.Flags = ParseFlags(flags, FlagLabels.EnumFlags);

                            obj.Enums.Add(mEnum);

                            continue;
                        }
                    }

                    module.Objects.Add(obj);
                }
            }

            return module;
        }

        public static ModuleRtDL AssembleRtDL(string[] text)
        {
            ModuleRtDL module = new ModuleRtDL();
            module.XData.Endianness = Endianness.Big;

            // Find module declaration
            for (int i = 0; i < text.Length; i++)
            {
                string line = text[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var match = MintRegex.Module().Match(line);
                if (match.Success)
                {
                    module.Name = match.Groups[1].Value;
                    break;
                }
                else if (i == text.Length - 1)
                    throw new FormatException("Could not find module declaration!");
            }

            // Find function declarations to fill SData and other preliminary stuff
            List<string> inst = new List<string>();
            for (int i = 0; i < text.Length; i++)
            {
                string line = text[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var match = MintRegex.SData().Match(line);
                if (match.Success)
                {
                    string str = match.Groups[1].Value;
                    if (MintRegex.String().IsMatch(str))
                        SearchSData(module, ParseStringToken(str), true);
                    else
                        SearchSData(module, ParseValueToken(str));
                    continue;
                }

                match = MintRegex.XRef().Match(line);
                if (match.Success)
                {
                    SearchXRef(module, match.Groups[1].Value);
                    continue;
                }

                match = MintRegex.Function().Match(line);
                if (match.Success)
                {
                    for (; i < text.Length; i++)
                    {
                        if (text[i].Trim().EndsWith("{"))
                            break;
                    }

                    i++;
                    for (; i < text.Length; i++)
                    {
                        if (text[i].Trim().EndsWith("}"))
                            break;
                        else
                            inst.Add(text[i].Trim());
                    }
                }
            }

            AssembleSData(module, inst, MainForm.RTDL_VERSION);

            for (int i = 0; i < text.Length; i++)
            {
                string line = text[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var match = MintRegex.Object().Match(line);
                if (match.Success)
                {
                    MintObject obj = new MintObject();
                    obj.Name = match.Groups[1].Value;

                    while (!line.EndsWith("{"))
                        line = text[i++].Trim();

                    for (; i < text.Length; i++)
                    {
                        line = text[i].Trim();
                        if (line.EndsWith("}"))
                            break;

                        var objMatch = MintRegex.Variable().Match(line);
                        if (objMatch.Success)
                        {
                            string type = objMatch.Groups[1].Value;
                            string name = objMatch.Groups[2].Value;
                            MintVariable mVar = new MintVariable(type, name);

                            string[] flags = line.Substring(0, objMatch.Groups[1].Index).Split(' ').ToArray();
                            mVar.Flags = ParseFlags(flags, FlagLabels.VariableFlags);

                            obj.Variables.Add(mVar);

                            continue;
                        }

                        objMatch = MintRegex.Function().Match(line);
                        if (objMatch.Success)
                        {
                            string name = objMatch.Groups[0].Value;
                            MintFunction mFunc = new MintFunction(name);

                            string[] flags = line.Substring(0, objMatch.Groups[0].Index).Split(' ').ToArray();
                            mFunc.Flags = ParseFlags(flags, FlagLabels.FunctionFlags);

                            while (!line.EndsWith("{"))
                                line = text[i++].Trim();

                            List<string> funcInst = new List<string>();
                            for (; i < text.Length; i++)
                            {
                                line = text[i].Trim();
                                if (line.EndsWith("}"))
                                    break;
                                funcInst.Add(line);
                            }

                            mFunc.Data = AssembleFunction(module, funcInst, MainForm.RTDL_VERSION);

                            obj.Functions.Add(mFunc);

                            continue;
                        }
                    }

                    module.Objects.Add(obj);
                }
            }

            return module;
        }
    }
}
