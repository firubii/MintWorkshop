using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Crc32C;

namespace MintWorkshop.Util
{
    public static class TextColors
    {
        public static Color ByteColor = Color.FromArgb(0, 0, 255);
        public static Color MneumonicColor = Color.FromArgb(0, 0, 128);
        public static Color MneumonicExtColor = Color.FromArgb(0, 128, 255);
        public static Color RegisterColor = Color.FromArgb(155, 150, 50);
        public static Color ConstantColor = Color.FromArgb(0, 128, 54);
        public static Color StringColor = Color.FromArgb(100, 100, 100);
        public static Color XRefColor = Color.FromArgb(0, 0, 255);
        public static Color HashColor = Color.FromArgb(0, 128, 0);
        public static Color SDataColor = Color.FromArgb(155, 50, 155);
        public static Color JumpLocColor = Color.FromArgb(180, 0, 240);
    }

    public enum Endianness
    {
        Big,
        Little
    }

    // From https://stackoverflow.com/questions/1440392/use-byte-as-key-in-dictionary
    public class ByteArrayComparer : EqualityComparer<byte[]>
    {
        public static bool Equal(byte[] left, byte[] right)
        {
            return new ByteArrayComparer().Equals(left, right);
        }

        public override bool Equals(byte[] left, byte[] right)
        {
            if (left == null || right == null)
            {
                return left == right;
            }
            if (left.Length != right.Length)
            {
                return false;
            }
            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                {
                    return false;
                }
            }
            return true;
        }
        public override int GetHashCode(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            int sum = 0;
            foreach (byte cur in key)
            {
                sum += cur;
            }
            return sum;
        }
    }

    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            AppendText(box, text, color, box.BackColor);
        }

        public static void AppendText(this RichTextBox box, string text, Color color, Color backColor)
        {
            if (string.IsNullOrEmpty(text))
                return;

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionBackColor = backColor;
            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
            box.SelectionBackColor = box.BackColor;
        }
    }

    public static class HashCalculator
    {
        public static byte[] Calculate(string str)
        {
            byte[] hash;
            Crc32CAlgorithm crc = new Crc32CAlgorithm();
            hash = crc.ComputeHash(Encoding.UTF8.GetBytes(str));
            //Invert the hash, since Hal does that for some reason
            for (int i = 0; i < hash.Length; i++)
                hash[i] = (byte)(255 - hash[i]);
            return hash;
        }
    }

    public static class WriteUtil
    {
        public static void WriteString(EndianBinaryWriter writer, string str)
        {
            writer.Write(str.Length);
            writer.Write(Encoding.UTF8.GetBytes(str));
            writer.Write(0);
            while ((writer.BaseStream.Length & 0xF) != 0x0
                && (writer.BaseStream.Length & 0xF) != 0x4
                && (writer.BaseStream.Length & 0xF) != 0x8
                && (writer.BaseStream.Length & 0xF) != 0xC)
                writer.Write((byte)0);
        }
    }
}
