using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MintWorkshop.Util
{
    public class EndianBinaryWriter : BinaryWriter
    {
        public Endianness Endianness = Endianness.Little;

        public EndianBinaryWriter(Stream stream) : base(stream) { Endianness = Endianness.Little; }
        public EndianBinaryWriter(Stream stream, Endianness endian) : base(stream) { Endianness = endian; }

        public override void Write(short value)
        {
            if (Endianness == Endianness.Big)
            {
                value = InvertEndianness(value);
            }
            base.Write(value);
        }

        public override void Write(ushort value)
        {
            if (Endianness == Endianness.Big)
            {
                value = InvertEndianness(value);
            }
            base.Write(value);
        }

        public override void Write(int value)
        {
            if (Endianness == Endianness.Big)
            {
                value = InvertEndianness(value);
            }
            base.Write(value);
        }

        public override void Write(uint value)
        {
            if (Endianness == Endianness.Big)
            {
                value = InvertEndianness(value);
            }
            base.Write(value);
        }

        public override void Write(float value)
        {
            if (Endianness == Endianness.Big)
            {
                value = InvertEndianness(value);
            }
            base.Write(value);
        }

        private short InvertEndianness(short val)
        {
            return (short)(((val & 0x00ff) << 8) +
                           ((val & 0xff00) >> 8));
        }

        private ushort InvertEndianness(ushort val)
        {
            return (ushort)(((val & 0x00ff) << 8) +
                            ((val & 0xff00) >> 8));
        }

        private int InvertEndianness(int val)
        {
            return (int)(((val & 0x000000ff) << 24) +
                         ((val & 0x0000ff00) << 8) +
                         ((val & 0x00ff0000) >> 8) +
                         ((val & 0xff000000) >> 24));
        }

        private uint InvertEndianness(uint val)
        {
            return ((val & 0x000000ff) << 24) +
                   ((val & 0x0000ff00) << 8) +
                   ((val & 0x00ff0000) >> 8) +
                   ((val & 0xff000000) >> 24);
        }

        private float InvertEndianness(float val)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(val).Reverse().ToArray(), 0);
        }
    }
}
