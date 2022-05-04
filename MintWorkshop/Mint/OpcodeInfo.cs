using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MintWorkshop.Util;

namespace MintWorkshop.Mint
{
    [Flags]
    public enum Action
    {
        None = 0,
        Jump = 1,
        Skip = 2,
        Return = 4
    }

    [Flags]
    public enum InstructionArg : uint
    {
        None = 0,

        //Argument Locations
        Z = 0x1, //Byte 2
        X = 0x2, //Byte 3
        Y = 0x4, //Byte 4
        V = 0x8, //Byte 3, Byte 4 as short
        AllBytes = Z | X | Y,
        VSigned = V | Signed,

        //Extended Argument Locations
        A = 0x10,
        B = 0x20,
        C = 0x40,
        E = 0x80,
        AllExtBytes = A | B | C,
        AllData = AllBytes | AllExtBytes | V | E,
        ESigned = E | Signed,

        //Argument Type
        Register = 0x100,
        SDataInt = 0x200,
        SDataFloat = 0x400,
        SDataArr = 0x800,
        SDataRegInt = 0x1000,
        SDataRegFloat = 0x2000,
        SDataRegArr = 0x4000,
        XRef = 0x8000,

        Signed = 0x80000000,

        //Presets
        RegZ = Z | Register,
        RegX = X | Register,
        RegY = Y | Register,
        RegA = A | Register,
        RegB = B | Register,
        RegC = C | Register,

        IntZ = Z | SDataInt,
        IntX = X | SDataInt,
        IntY = Y | SDataInt,
        IntV = V | SDataInt,
        IntA = A | SDataInt,
        IntB = B | SDataInt,
        IntC = C | SDataInt,
        IntE = E | SDataInt,

        IntRegZ = Z | SDataRegInt,
        IntRegX = X | SDataRegInt,
        IntRegY = Y | SDataRegInt,
        IntRegA = A | SDataRegInt,
        IntRegB = B | SDataRegInt,
        IntRegC = C | SDataRegInt,

        FloatZ = Z | SDataFloat,
        FloatX = X | SDataFloat,
        FloatY = Y | SDataFloat,
        FloatV = V | SDataFloat,
        FloatA = A | SDataFloat,
        FloatB = B | SDataFloat,
        FloatC = C | SDataFloat,
        FloatE = E | SDataFloat,

        FloatRegZ = Z | SDataRegFloat,
        FloatRegX = X | SDataRegFloat,
        FloatRegY = Y | SDataRegFloat,
        FloatRegA = A | SDataRegFloat,
        FloatRegB = B | SDataRegFloat,
        FloatRegC = C | SDataRegFloat,

        ArrZ = Z | SDataArr,
        ArrX = X | SDataArr,
        ArrY = Y | SDataArr,
        ArrV = V | SDataArr,
        ArrA = A | SDataArr,
        ArrB = B | SDataArr,
        ArrC = C | SDataArr,
        ArrE = E | SDataArr,

        ArrRegZ = Z | SDataRegArr,
        ArrRegX = X | SDataRegArr,
        ArrRegY = Y | SDataRegArr,
        ArrRegA = A | SDataRegArr,
        ArrRegB = B | SDataRegArr,
        ArrRegC = C | SDataRegArr,

        XRefZ = Z | XRef,
        XRefX = X | XRef,
        XRefY = Y | XRef,
        XRefV = V | XRef,
        XRefA = A | XRef,
        XRefB = B | XRef,
        XRefC = C | XRef,
        XRefE = E | XRef
    }

    public struct Opcode
    {
        public Opcode(string name, InstructionArg[] args, Action action = Action.None, bool extended = false, byte[] baseData = null)
        {
            this.Name = name;
            this.Action = action;
            this.Arguments = args;
            this.Size = extended ? 8 : 4;
            this.BaseData = baseData != null ? baseData : new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
        }
        public static Opcode None() => new Opcode(null, null);

        public string Name;
        public Action Action;
        public InstructionArg[] Arguments;
        public int Size;
        public byte[] BaseData;
    }

    public struct Instruction
    {
        public Instruction(byte[] instruction)
        {
            this.instData = instruction;
        }
        private byte[] instData;
        public byte Opcode { get { return instData[0]; } set { instData[0] = value; } }
        public byte Z { get { return instData[1]; } set { instData[1] = value; } }
        public byte X { get { return instData[2]; } set { instData[2] = value; } }
        public byte Y { get { return instData[3]; } set { instData[3] = value; } }
        public short V(Endianness endianness)
        {
            if (endianness == Endianness.Big)
                return BitConverter.ToInt16(new byte[] { Y, X }, 0);
            return BitConverter.ToInt16(new byte[] { X, Y }, 0);
        }

        //Extended-size instruction data for Basil
        public byte D { get { return instData[4]; } set { instData[4] = value; } }
        public byte A { get { return instData[5]; } set { instData[5] = value; } }
        public byte B { get { return instData[6]; } set { instData[6] = value; } }
        public byte C { get { return instData[7]; } set { instData[7] = value; } }
        public short E(Endianness endianness)
        {
            if (endianness == Endianness.Big)
                return BitConverter.ToInt16(new byte[] { C, B }, 0);
            return BitConverter.ToInt16(new byte[] { B, C }, 0);
        }

        public int Length => instData.Length;

        public void Write(EndianBinaryWriter writer)
        {
            writer.Write(instData);
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < instData.Length; i++)
                s += instData[i].ToString("X2");
            return s;
        }
    }
}
