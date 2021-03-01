using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MintWorkshop.Util
{
    public class EndianBinaryReader : BinaryReader
    {
        public Endianness Endianness = Endianness.Little;

        public EndianBinaryReader(Stream stream) : base(stream) { Endianness = Endianness.Little; }
        public EndianBinaryReader(Stream stream, Endianness endian) : base(stream) { Endianness = endian; }

        public override short ReadInt16()
        {
            if (Endianness == Endianness.Little)
            {
                return base.ReadInt16();
            }
            else
            {
                var data = base.ReadBytes(2);
                return BitConverter.ToInt16(data.Reverse().ToArray(), 0);
            }
        }

        public override ushort ReadUInt16()
        {
            if (Endianness == Endianness.Little)
            {
                return base.ReadUInt16();
            }
            else
            {
                var data = base.ReadBytes(2);
                return BitConverter.ToUInt16(data.Reverse().ToArray(), 0);
            }
        }

        public override int ReadInt32()
        {
            if (Endianness == Endianness.Little)
            {
                return base.ReadInt32();
            }
            else
            {
                var data = base.ReadBytes(4);
                return BitConverter.ToInt32(data.Reverse().ToArray(), 0);
            }
        }

        public override uint ReadUInt32()
        {
            if (Endianness == Endianness.Little)
            {
                return base.ReadUInt32();
            }
            else
            {
                var data = base.ReadBytes(4);
                return BitConverter.ToUInt32(data.Reverse().ToArray(), 0);
            }
        }

        public override long ReadInt64()
        {
            if (Endianness == Endianness.Little)
            {
                return base.ReadInt64();
            }
            else
            {
                var data = base.ReadBytes(8);
                return BitConverter.ToInt64(data.Reverse().ToArray(), 0);
            }
        }

        public override ulong ReadUInt64()
        {
            if (Endianness == Endianness.Little)
            {
                return base.ReadUInt64();
            }
            else
            {
                var data = base.ReadBytes(8);
                return BitConverter.ToUInt64(data.Reverse().ToArray(), 0);
            }
        }

        public override float ReadSingle()
        {
            if (Endianness == Endianness.Little)
            {
                return base.ReadSingle();
            }
            else
            {
                var data = base.ReadBytes(4);
                return BitConverter.ToSingle(data.Reverse().ToArray(), 0);
            }
        }
    }
}
