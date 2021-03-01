using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MintWorkshop.Util;

namespace MintWorkshop.Types
{
    public class XData
    {
        public const string XDATA_MAGIC = "XBIN";

        public string Magic { get; private set; }
        public Endianness Endianness { get; private set; }
        public byte[] Version { get; private set; }
        public uint Filesize { get; private set; }
        public uint Unknown0C { get; private set; }

        public XData(Endianness endianness)
        {
            Magic = XDATA_MAGIC;
            Endianness = endianness;
            Version = new byte[] { 2, 0 };
            Filesize = 0;
            Unknown0C = 65001;
        }

        public XData(EndianBinaryReader reader)
        {
            Magic = Encoding.UTF8.GetString(reader.ReadBytes(4));
            ushort bom = reader.ReadUInt16();
            //Invert reader endianness if the byte order mark doesn't match 0x1234
            if (bom == 0x3412)
            {
                if (reader.Endianness == Endianness.Big)
                    reader.Endianness = Endianness.Little;
                else if (reader.Endianness == Endianness.Little)
                    reader.Endianness = Endianness.Big;
            }
            Endianness = reader.Endianness;
            Version = reader.ReadBytes(2);

            if (!isValid()) { throw new Exception("Invalid XData."); }

            Filesize = reader.ReadUInt32();
            Unknown0C = reader.ReadUInt32();
        }

        public void Write(EndianBinaryWriter writer)
        {
            if (!isValid()) { throw new Exception("Invalid XData."); }

            writer.Write(new byte[] { 0x58, 0x42, 0x49, 0x4E });
            writer.Endianness = Endianness;
            writer.Write((ushort)0x1234);
            writer.Write(Version);
            writer.Write(-1);
            writer.Write(Unknown0C);
        }

        public void UpdateFilesize(EndianBinaryWriter writer)
        {
            Filesize = (uint)writer.BaseStream.Length;

            writer.BaseStream.Seek(0x8, SeekOrigin.Begin);
            writer.Write(Filesize);
        }

        public bool isValid()
        {
            return Magic == XDATA_MAGIC && Version.SequenceEqual(new byte[] { 2, 0 });
        }
    }
}
