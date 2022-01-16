using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MintWorkshop
{
    public partial class ProgressBar : Form
    {
        string statusText = "";

        public ProgressBar()
        {
            InitializeComponent();
        }

        public void SetMax(int max)
        {
            progress.Maximum = max;
            UpdateDisplay();
        }

        public void SetValue(int value)
        {
            progress.Value = value;
            UpdateDisplay();
        }

        public void IncrementValue()
        {
            progress.Value++;
            UpdateDisplay();
        }

        public void SetTitle(string text)
        {
            this.Text = text;
            UpdateDisplay();
        }

        public void SetStatus(string text)
        {
            statusText = text;
            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            status.Text = $"{statusText} {progress.Value}/{progress.Maximum} - {progress.Value/progress.Maximum * 100f}%";
        }

    }
}
