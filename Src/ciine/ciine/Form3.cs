using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Web.WebView2.Core;
using WebView2 = Microsoft.Web.WebView2.WinForms.WebView2;
using System.Linq;
using System.Drawing;

namespace ciine
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        public static extern uint TimeBeginPeriod(uint ms);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        public static extern uint TimeEndPeriod(uint ms);
        [DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
        public static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
        public static uint CurrentResolution = 0;
        public static bool closed = false;
        public static int Width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, Height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
        public static double ir1x, ir1y, ir2x, ir2y;
        public WebView2 webView21 = new WebView2();
        private async void Form3_Load(object sender, EventArgs e)
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
            Width = Screen.PrimaryScreen.Bounds.Width;
            Height = Screen.PrimaryScreen.Bounds.Height;
            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions("--disable-web-security --allow-file-access-from-files --allow-file-access", "en");
            CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, null, options);
            await webView21.EnsureCoreWebView2Async(environment);
            webView21.CoreWebView2.SetVirtualHostNameToFolderMapping("appassets", "assets", CoreWebView2HostResourceAccessKind.DenyCors);
            webView21.CoreWebView2.Settings.AreDevToolsEnabled = true;
            webView21.KeyDown += WebView21_KeyDown;
            webView21.Source = new Uri("https://appassets/tracker/index.html");
            webView21.Dock = DockStyle.Fill;
            webView21.DefaultBackgroundColor = System.Drawing.Color.Transparent;
            this.Controls.Add(webView21);
            this.Location = new System.Drawing.Point(0, 0);
            this.Size = new System.Drawing.Size(Width, Height);
        }
        private void Form3_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e.KeyData);
        }
        private void WebView21_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e.KeyData);
        }
        private void OnKeyDown(System.Windows.Forms.Keys keyData)
        {
            if (keyData == System.Windows.Forms.Keys.F1)
            {
                const string message = "• Author: Michaël André Franiatte.\n\r\n\r• Contact: michael.franiatte@gmail.com.\n\r\n\r• Publisher: https://github.com/michaelandrefraniatte.\n\r\n\r• Copyrights: All rights reserved, no permissions granted.\n\r\n\r• License: Not open source, not free of charge to use.";
                const string caption = "About";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public async void SetDots(double ir1x, double ir1y, double ir2x, double ir2y)
        {
            try
            {
                await execScriptHelper($"setDots('{ir1x.ToString()}', '{ir1y.ToString()}', '{ir2x.ToString()}', '{ir2y.ToString()}');");
            }
            catch { }
        }
        private async Task<String> execScriptHelper(String script)
        {
            var x = await webView21.ExecuteScriptAsync(script).ConfigureAwait(false);
            return x;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            SetDots(ir1x, ir1y, ir2x, ir2y);
        }
        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            closed = true;
        }
    }
}