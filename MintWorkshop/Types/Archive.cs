using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BrawlLib.Internal;
using BrawlLib.Internal.IO;
using BrawlLib.SSBB.ResourceNodes;
using BrawlLib.Wii;
using BrawlLib.Wii.Compression;
using MintWorkshop.Util;

namespace MintWorkshop.Types
{
    public class Archive : IDisposable
    {
        public struct Namespace
        {
            public Namespace(int index, string name, int scripts, int totalScripts, int childNamespaces)
            {
                this.Index = index;
                this.Name = name;
                this.Scripts = scripts;
                this.TotalScripts = totalScripts;
                this.ChildNamespaces = childNamespaces;
            }
            public int Index;
            public string Name;
            public int Scripts;
            public int TotalScripts;
            public int ChildNamespaces;
        }

        public bool LZ77Compressed { get; set; } = false;

        public XData XData { get; private set; }
        public byte[] Version { get; private set; }
        public uint RootNamespaces { get; private set; }

        public List<Namespace> Namespaces { get; set; }
        public Dictionary<string, MintScript> Scripts { get; private set; }
        public List<int> IndexTable { get; private set; }

        public Archive(string filePath)
        {
            EndianBinaryReader reader = new EndianBinaryReader(new FileStream(filePath, FileMode.Open, FileAccess.Read));
            if (reader.ReadByte() == 0x11)
            {
                Console.WriteLine("LZ77 Extended compression detected. Decompressing...");
                LZ77Compressed = true;
                DataSource dataSrc = new DataSource(new MemoryStream(File.ReadAllBytes(filePath)), CompressionType.ExtendedLZ77);
                FileStream stream = Compressor.TryExpand(ref dataSrc, false).BaseStream;
                stream.Lock(0, stream.Length);
                reader = new EndianBinaryReader(stream);
            }

            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            Read(reader);
            if (LZ77Compressed)
                (reader.BaseStream as FileStream).Unlock(0, reader.BaseStream.Length);
            reader.Dispose();
        }

        public Archive(EndianBinaryReader reader)
        {
            Read(reader);
        }

        public void Read(EndianBinaryReader reader)
        {
            XData = new XData(reader);
            if (!XData.isValid()) return;

            Version = reader.ReadBytes(4);
            int namespaceCount = reader.ReadInt32() - 1;
            reader.BaseStream.Seek(4, SeekOrigin.Current);
            int scriptCount = reader.ReadInt32();
            uint scriptList = reader.ReadUInt32();
            uint unk = reader.ReadUInt32();
            reader.BaseStream.Seek(0x30, SeekOrigin.Begin);
            RootNamespaces = reader.ReadUInt32();

            Console.WriteLine($"Archive Header Data:" +
                            $"\n-XData-" +
                            $"\nMagic: {XData.Magic}" +
                            $"\nEndianness: {XData.Endianness}" +
                            $"\nFilesize: 0x{XData.Filesize:X8}" +
                            $"\n-Mint Archive-" +
                            $"\nVersion: {string.Join(".", Version)}" +
                            $"\nNamespaces: {namespaceCount}" +
                            $"\nRoot Namespaces: {RootNamespaces}" +
                            $"\nScripts: {scriptCount}" +
                            $"\nScript List Offset: 0x{scriptList:X8}" +
                            $"\nIndex Table Offset: 0x{0x34 + (namespaceCount * 0x14):X8}");

            Console.WriteLine("Reading Namespaces...");
            Namespaces = new List<Namespace>();
            for (int i = 0; i < namespaceCount; i++)
            {
                reader.BaseStream.Seek(0x34 + (i * 0x14), SeekOrigin.Begin);
                uint indexOffset = reader.ReadUInt32();
                uint nameOffset = reader.ReadUInt32();
                int scrCount = reader.ReadInt32();
                int totalScripts = reader.ReadInt32();
                int childrenCount = reader.ReadInt32();

                reader.BaseStream.Seek(indexOffset, SeekOrigin.Begin);
                int index = reader.ReadInt32();
                reader.BaseStream.Seek(nameOffset, SeekOrigin.Begin);
                string name = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));

                Namespaces.Add(new Namespace(index, name, scrCount, totalScripts, childrenCount));
            }

            Console.WriteLine("Reading index table...");
            IndexTable = new List<int>();
            reader.BaseStream.Seek(0x34 + (namespaceCount * 0x14), SeekOrigin.Begin);
            while (reader.BaseStream.Position < scriptList)
                IndexTable.Add(reader.ReadInt32());
            //Console.WriteLine(string.Join(",", IndexTable));

            Console.WriteLine("Reading scripts...");
            Scripts = new Dictionary<string, MintScript>();
            reader.BaseStream.Seek(scriptList, SeekOrigin.Begin);
            for (int i = 0; i < scriptCount; i++)
            {
                reader.BaseStream.Seek(scriptList + (i * 8), SeekOrigin.Begin);
                uint nameOffset = reader.ReadUInt32();
                uint scriptOffset = reader.ReadUInt32();

                reader.BaseStream.Seek(nameOffset, SeekOrigin.Begin);
                string name = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()));

                reader.BaseStream.Seek(scriptOffset, SeekOrigin.Begin);
                XData scrX = new XData(reader);
                reader.BaseStream.Seek(scriptOffset, SeekOrigin.Begin);

                using (MemoryStream scr = new MemoryStream(reader.ReadBytes((int)scrX.Filesize)))
                using (EndianBinaryReader scrReader = new EndianBinaryReader(scr))
                    Scripts.Add(name, new MintScript(new EndianBinaryReader(scr), Version));
            }

            reader.BaseStream.Seek(unk, SeekOrigin.Begin);
            //Console.WriteLine(reader.ReadUInt32());
        }

        public void Write(string path)
        {
            if (LZ77Compressed)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (EndianBinaryWriter writer = new EndianBinaryWriter(stream))
                        Write(writer);
                    byte[] buffer = stream.GetBuffer().Take((int)XData.Filesize).ToArray();
                    unsafe
                    {
                        fixed (byte* b = &buffer[0])
                        {
                            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
                                Compressor.Compact(CompressionType.ExtendedLZ77, new VoidPtr { address = b }, buffer.Length, file, new RawDataNode { _mainForm = Program.MainForm, Name = "Mint Archive" });
                        }
                    }
                }
            }
            else
            {
                using (EndianBinaryWriter writer = new EndianBinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
                    Write(writer);
            }
        }

        public void Write(EndianBinaryWriter writer)
        {
            XData.Write(writer);

            writer.Write(Version);
            writer.Write(Namespaces.Count + 1);
            writer.Write(0x24);
            writer.Write(Scripts.Count);

            uint scrListOffs = (uint)(0x34 + (Namespaces.Count * 0x14) + (IndexTable.Count * 4));
            writer.Write(scrListOffs);
            //Script data end bound offset, -1 for now until we get there
            writer.Write(-1);
            writer.Write(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });

            //Namespace information
            writer.Write(RootNamespaces);
            for (int i = 0; i < Namespaces.Count; i++)
            {
                //Console.WriteLine($"{Namespaces[i].Name} - Index {Namespaces[i].Index}, In table: {IndexTable.Contains(Namespaces[i].Index)}, Index: {IndexTable.IndexOf(Namespaces[i].Index)}");
                if (!IndexTable.Contains(Namespaces[i].Index))
                    writer.Write(scrListOffs);
                else
                    writer.Write((uint)(0x34 + (Namespaces.Count * 0x14) + (IndexTable.IndexOf(Namespaces[i].Index) * 4)));

                writer.Write(-1); //Come back to this later for string writing
                writer.Write(Namespaces[i].Scripts);
                writer.Write(Namespaces[i].TotalScripts);
                writer.Write(Namespaces[i].ChildNamespaces);
            }
            //writer.Write(scrListOffs);
            for (int i = 0; i < IndexTable.Count; i++)
                writer.Write(IndexTable[i]);

            writer.BaseStream.Seek(scrListOffs, SeekOrigin.Begin);
            for (int i = 0; i < Scripts.Count; i++)
            {
                writer.Write(-1); //Come back to this later for string writing
                writer.Write(-1);
            }
            for (int i = 0; i < Scripts.Count; i++)
            {
                writer.BaseStream.Seek(scrListOffs + (i * 8) + 4, SeekOrigin.Begin);
                writer.Write((uint)writer.BaseStream.Length);
                writer.BaseStream.Seek(0, SeekOrigin.End);
                writer.Write(Scripts.Values.ToArray()[i].Write());

                while ((writer.BaseStream.Length & 0xF) != 0x0
                    && (writer.BaseStream.Length & 0xF) != 0x4
                    && (writer.BaseStream.Length & 0xF) != 0x8
                    && (writer.BaseStream.Length & 0xF) != 0xC)
                        writer.Write((byte)0);
            }
            writer.BaseStream.Seek(0x24, SeekOrigin.Begin);
            writer.Write((uint)writer.BaseStream.Length);
            writer.BaseStream.Seek(0, SeekOrigin.End);
            writer.Write((long)0);

            while ((writer.BaseStream.Length & 0xF) != 0x0
                && (writer.BaseStream.Length & 0xF) != 0x4
                && (writer.BaseStream.Length & 0xF) != 0x8
                && (writer.BaseStream.Length & 0xF) != 0xC)
                    writer.Write((byte)0);

            for (int i = 0; i < Namespaces.Count; i++)
            {
                writer.BaseStream.Seek(0x34 + (i * 20) + 4, SeekOrigin.Begin);
                writer.Write((uint)writer.BaseStream.Length);
                writer.BaseStream.Seek(0, SeekOrigin.End);
                WriteUtil.WriteString(writer, Namespaces[i].Name);
            }
            for (int i = 0; i < Scripts.Count; i++)
            {
                writer.BaseStream.Seek(scrListOffs + (i * 8), SeekOrigin.Begin);
                writer.Write((uint)writer.BaseStream.Length);
                writer.BaseStream.Seek(0, SeekOrigin.End);
                WriteUtil.WriteString(writer, Scripts.Keys.ToArray()[i]);
            }

            XData.UpdateFilesize(writer);
        }

        public string GetSemanticVersion()
        {
            return $"{Version[0]}.{Version[1]}.{Version[2]}.{Version[3]}";
        }

        public Dictionary<byte[], string> GetHashes()
        {
            Dictionary<byte[], string> hashes = new Dictionary<byte[], string>(new ByteArrayComparer());
            foreach (KeyValuePair<string, MintScript> pair in Scripts)
            {
                for (int i = 0; i < pair.Value.Classes.Count; i++)
                {
                    hashes.Add(pair.Value.Classes[i].Hash, pair.Value.Classes[i].Name);
                    for (int c = 0; c < pair.Value.Classes[i].Variables.Count; c++)
                        hashes.Add(pair.Value.Classes[i].Variables[c].Hash, pair.Value.Classes[i].Name + "." + pair.Value.Classes[i].Variables[c].Name);
                    for (int c = 0; c < pair.Value.Classes[i].Functions.Count; c++)
                        hashes.Add(pair.Value.Classes[i].Functions[c].Hash, pair.Value.Classes[i].Functions[c].FullName());
                }
            }
            return hashes;
        }

        public void Dispose()
        {
            Namespaces.Clear();
            Scripts.Clear();
            IndexTable.Clear();
        }
    }
}
