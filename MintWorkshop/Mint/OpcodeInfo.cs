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
        None,
        Jump,
        Return
    }

    [Flags]
    public enum InstructionArg
    {
        None = 0,

        //Argument Locations
        Z = 1, //Byte 2
        X = 2, //Byte 3
        Y = 4, //Byte 4
        V = 8, //Byte 3, Byte 4 as short

        //Argument Type
        Register = 12,
        SDataInt = 16,
        SDataArr = 20,
        SDataRegInt = 24,
        SDataRegArr = 28,
        XRef = 32,
        Raw = 64,

        //Presets
        RegZ = Z | Register,
        RegX = X | Register,
        RegY = Y | Register,

        IntZ = Z | SDataInt,
        IntX = X | SDataInt,
        IntY = Y | SDataInt,
        IntV = V | SDataInt,

        IntRegZ = Z | SDataRegInt,
        IntRegX = X | SDataRegInt,
        IntRegY = Y | SDataRegInt,

        ArrZ = Z | SDataArr,
        ArrX = X | SDataArr,
        ArrY = Y | SDataArr,
        ArrV = V | SDataArr,

        ArrRegZ = Z | SDataRegArr,
        ArrRegX = X | SDataRegArr,
        ArrRegY = Y | SDataRegArr,

        XRefZ = Z | XRef,
        XRefX = X | XRef,
        XRefY = Y | XRef,
        XRefV = V | XRef,

        Short = V | Raw
    }

    public struct Opcode
    {
        public Opcode(string name, string[] args, Action action = Action.None)
        {
            this.Name = name;
            this.Action = action;
            this.Arguments = args;
        }
        public string Name;
        public Action Action;
        public string[] Arguments;
    }

    public struct Instruction
    {
        public Instruction(byte[] instruction)
        {
            this.Opcode = instruction[0];
            this.Z = instruction[1];
            this.X = instruction[2];
            this.Y = instruction[3];
        }
        public byte Opcode;
        public byte Z;
        public byte X;
        public byte Y;
        public short V(Endianness endianness)
        { 
            if (endianness == Endianness.Big)
                return BitConverter.ToInt16(new byte[] { Y, X }, 0);
            return BitConverter.ToInt16(new byte[] { X, Y }, 0);
        }
    }
}
