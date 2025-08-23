using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MintWorkshop.Util;
using KirbyLib.IO;

namespace MintWorkshop.Mint
{
    [Flags]
    public enum Action
    {
        None = 0,
        Jump = 1,
        Return = 2
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
        SDataArray = 0x800,
        SDataRegInt = 0x1000,
        SDataRegFloat = 0x2000,
        SDataRegArray = 0x4000,
        XRef = 0x8000,

        AllTypes = Register | SDataInt | SDataFloat | SDataArray | SDataRegInt | SDataRegFloat | SDataRegArray | XRef,

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

        ArrZ = Z | SDataArray,
        ArrX = X | SDataArray,
        ArrY = Y | SDataArray,
        ArrV = V | SDataArray,
        ArrA = A | SDataArray,
        ArrB = B | SDataArray,
        ArrC = C | SDataArray,
        ArrE = E | SDataArray,

        ArrRegZ = Z | SDataRegArray,
        ArrRegX = X | SDataRegArray,
        ArrRegY = Y | SDataRegArray,
        ArrRegA = A | SDataRegArray,
        ArrRegB = B | SDataRegArray,
        ArrRegC = C | SDataRegArray,

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
}
