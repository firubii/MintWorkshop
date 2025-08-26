using Force.Crc32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KirbyLib.IO;

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
        public static Color LabelColor = Color.FromArgb(180, 0, 240);
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

    // From https://stackoverflow.com/a/487757
    public static class DrawingControl
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
        }

        public static void ResumeDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
            parent.Refresh();
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
}
