using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintWorkshop.Mint
{
    [Flags]
    public enum Action
    {
        None,
        Jump,
        Return
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
        public short V() { return BitConverter.ToInt16(new byte[] { X, Y }, 0); }
    }
}
