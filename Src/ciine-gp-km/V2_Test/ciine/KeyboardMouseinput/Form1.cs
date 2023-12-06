using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeyboardMouseInputAPI;

namespace KeyboardMouseinput
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public void SetLabel1(string str)
        {
            this.label1.Text = str;
        }
        public void SetVisible()
        {
            this.ShowDialog();
        }
        public void SetUnvisible()
        {
            this.Hide();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}