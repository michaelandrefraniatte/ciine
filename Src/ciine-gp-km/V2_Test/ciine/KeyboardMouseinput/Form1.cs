using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            KeyboardMouseInput.viewdatashow = false;
            this.Hide();
            e.Cancel = true;
        }
    }
}