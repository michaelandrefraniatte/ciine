﻿using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Web.WebView2.Core;
using WebView2 = Microsoft.Web.WebView2.WinForms.WebView2;
using System.Collections.Generic;
using System.Linq;
using SharpDX.XInput;
using CSCore;
using WinformsVisualization.Visualization;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing;
using System.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using Bitmap = System.Drawing.Bitmap;
using Point = System.Drawing.Point;
using System.Drawing.Imaging;

namespace ciine
{
    public partial class Form2 : Form
    {
        public Form2()
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
        public bool closed = false;
        public int width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
        public WebView2 webView21 = new WebView2();
        public List<double> List_A = new List<double>(), List_B = new List<double>(), List_X = new List<double>(), List_Y = new List<double>(), List_LB = new List<double>(), List_RB = new List<double>(), List_LT = new List<double>(), List_RT = new List<double>(), List_MAP = new List<double>(), List_MENU = new List<double>(), List_LSTICK = new List<double>(), List_RSTICK = new List<double>(), List_DU = new List<double>(), List_DD = new List<double>(), List_DL = new List<double>(), List_DR = new List<double>(), List_XBOX = new List<double>();
        public bool Controller_A, Controller_B, Controller_X, Controller_Y, Controller_LB, Controller_RB, Controller_MAP, Controller_MENU, Controller_LSTICK, Controller_RSTICK, Controller_DU, Controller_DD, Controller_DL, Controller_DR, Controller_XBOX;
        public double Controller_LT, Controller_RT, Controller_LX, Controller_LY, Controller_RX, Controller_RY;
        private Controller[] controller = new Controller[] { null };
        public int xnum;
        private State state;
        public bool Controller1ButtonAPressed;
        public bool Controller1ButtonBPressed;
        public bool Controller1ButtonXPressed;
        public bool Controller1ButtonYPressed;
        public bool Controller1ButtonStartPressed;
        public bool Controller1ButtonBackPressed;
        public bool Controller1ButtonDownPressed;
        public bool Controller1ButtonUpPressed;
        public bool Controller1ButtonLeftPressed;
        public bool Controller1ButtonRightPressed;
        public bool Controller1ButtonShoulderLeftPressed;
        public bool Controller1ButtonShoulderRightPressed;
        public bool Controller1ThumbpadLeftPressed;
        public bool Controller1ThumbpadRightPressed;
        public double Controller1TriggerLeftPosition;
        public double Controller1TriggerRightPosition;
        public double Controller1ThumbLeftX;
        public double Controller1ThumbLeftY;
        public double Controller1ThumbRightX;
        public double Controller1ThumbRightY;
        public static double ir1x, ir1y, ir2x, ir2y;
        public int numBars = 100;
        public float[] barDataLeft = new float[100];
        public float[] barDataRight = new float[100];
        public int minFreq = 0;
        public int maxFreq = 23000;
        public int barSpacing = 0;
        public bool logScale = true;
        public bool isAverage = false;
        public float highScaleAverage = 1000f;
        public float highScaleNotAverage = 1000f;
        public LineSpectrum lineSpectrumLeft;
        public LineSpectrum lineSpectrumRight;
        public CSCore.SoundIn.WasapiCapture capture;
        public CSCore.DSP.FftSize fftSize;
        public float[] fftBuffer;
        public BasicSpectrumProvider spectrumProviderLeft;
        public BasicSpectrumProvider spectrumProviderRight;
        public CSCore.IWaveSource finalSource;
        private Bitmap bmp;
        public static int minimapstart, minimapend;
        private FilterInfoCollection CaptureDevice;
        private VideoCaptureDevice FinalFrame;
        private VideoCapabilities[] videoCapabilities;
        private double initratio;
        private List<Control> shadowControls = new List<Control>();
        private Bitmap shadowBmp = null;
        private GraphicsPath gp;
        private void Form2_Load(object sender, EventArgs e)
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
        }
        private async void Form2_Shown(object sender, EventArgs e)
        {
            width = Screen.PrimaryScreen.Bounds.Width;
            height = Screen.PrimaryScreen.Bounds.Height;
            CoreWebView2EnvironmentOptions options = new CoreWebView2EnvironmentOptions("--disable-web-security --allow-file-access-from-files --allow-file-access", "en");
            CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, null, options);
            await webView21.EnsureCoreWebView2Async(environment);
            webView21.CoreWebView2.SetVirtualHostNameToFolderMapping("appassets", "assets", CoreWebView2HostResourceAccessKind.DenyCors);
            webView21.CoreWebView2.Settings.AreDevToolsEnabled = true;
            webView21.KeyDown += WebView21_KeyDown;
            webView21.Source = new Uri("https://appassets/index.html");
            webView21.Dock = DockStyle.Fill;
            webView21.DefaultBackgroundColor = System.Drawing.Color.Transparent;
            this.Controls.Add(webView21);
            this.Location = new System.Drawing.Point(0, 0);
            this.Size = new System.Drawing.Size(width, height);
            try
            {
                var controllers = new[] { new Controller(UserIndex.One) };
                xnum = 0;
                foreach (var selectControler in controllers)
                {
                    if (selectControler.IsConnected)
                    {
                        controller[xnum] = selectControler;
                        xnum++;
                        if (xnum > 0)
                        {
                            break;
                        }
                    }
                }
            }
            catch { }
            Task.Run(() => GetAudioByteArray());
            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (CaptureDevice.Count > 0)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                this.pictureBox1.Size = new Size(minimapend - minimapstart, minimapend - minimapstart);
                this.pictureBox1.Location = new Point((width - this.pictureBox1.Width) / 2 - (minimapend - minimapstart) / 2 - 5, 5);
                this.pictureBox2.Size = new Size(minimapend - minimapstart, minimapend - minimapstart);
                this.pictureBox2.Location = new Point((width - this.pictureBox2.Width) / 2 + (minimapend - minimapstart) / 2 + 5, 5);
                gp = new GraphicsPath();
                gp.AddEllipse(pictureBox1.DisplayRectangle);
                pictureBox1.Region = new Region(gp);
                gp = new GraphicsPath();
                gp.AddEllipse(pictureBox2.DisplayRectangle);
                pictureBox2.Region = new Region(gp);
                shadowControls.Add(pictureBox1);
                shadowControls.Add(pictureBox2);
                this.Refresh();
                FinalFrame = new VideoCaptureDevice(CaptureDevice[0].MonikerString);
                videoCapabilities = FinalFrame.VideoCapabilities;
                FinalFrame.VideoResolution = videoCapabilities[1];
                initratio = Convert.ToDouble(FinalFrame.VideoResolution.FrameSize.Width) / Convert.ToDouble(FinalFrame.VideoResolution.FrameSize.Height);
                FinalFrame.NewFrame += FinalFrame_NewFrame;
                FinalFrame.Start();
            }
            else
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                this.pictureBox1.Size = new Size(minimapend - minimapstart, minimapend - minimapstart);
                this.pictureBox1.Location = new Point((width - this.pictureBox1.Width) / 2, 0);
            }
        }
        private void Form2_KeyDown(object sender, KeyEventArgs e)
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
        private void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap capture = eventArgs.Frame.Clone() as Bitmap;
                this.pictureBox2.Image = cropImage(capture);
                capture.Dispose();
            }
            catch { }
        }
        private Bitmap cropImage(Bitmap bmp)
        {
            int oldwidth = (int)(initratio * bmp.Height);
            int oldheight = bmp.Height;
            int newWidth = oldheight;
            int newHeight = oldheight;
            Rectangle CropArea = new Rectangle((oldwidth - oldheight) / 2, 0, newWidth, newHeight);
            Bitmap bmpCrop = bmp.Clone(CropArea, bmp.PixelFormat);
            return bmpCrop;
        }
        public async void SetDots(double ir1x, double ir1y, double ir2x, double ir2y)
        {
            try
            {
                await execScriptHelper($"setDots('{ir1x.ToString()}', '{ir1y.ToString()}', '{ir2x.ToString()}', '{ir2y.ToString()}');");
            }
            catch { }
        }
        public async void SetController(bool ControllerButtonAPressed, bool ControllerButtonBPressed, bool ControllerButtonXPressed, bool ControllerButtonYPressed, bool ControllerButtonStartPressed, bool ControllerButtonBackPressed, bool ControllerButtonDownPressed, bool ControllerButtonUpPressed, bool ControllerButtonLeftPressed, bool ControllerButtonRightPressed, bool ControllerButtonShoulderLeftPressed, bool ControllerButtonShoulderRightPressed, bool ControllerThumbpadLeftPressed, bool ControllerThumbpadRightPressed, double ControllerTriggerLeftPosition, double ControllerTriggerRightPosition, double ControllerThumbLeftX, double ControllerThumbLeftY, double ControllerThumbRightX, double ControllerThumbRightY)
        {
            try
            {
                Controller_RX = ControllerThumbRightX;
                Controller_RY = ControllerThumbRightY;
                Controller_LX = ControllerThumbLeftX;
                Controller_LY = ControllerThumbLeftY;
                Controller_RT = ControllerTriggerRightPosition;
                Controller_LT = ControllerTriggerLeftPosition;
                if (List_A.Count >= 5)
                {
                    List_A.RemoveAt(0);
                    List_A.Add(ControllerButtonAPressed ? 1 : 0);
                    Controller_A = List_A.Average() > 0 ? true : false;
                }
                else
                {
                    List_A.Add(0);
                }
                if (List_B.Count >= 5)
                {
                    List_B.RemoveAt(0);
                    List_B.Add(ControllerButtonBPressed ? 1 : 0);
                    Controller_B = List_B.Average() > 0 ? true : false;
                }
                else
                {
                    List_B.Add(0);
                }
                if (List_X.Count >= 5)
                {
                    List_X.RemoveAt(0);
                    List_X.Add(ControllerButtonXPressed ? 1 : 0);
                    Controller_X = List_X.Average() > 0 ? true : false;
                }
                else
                {
                    List_X.Add(0);
                }
                if (List_Y.Count >= 5)
                {
                    List_Y.RemoveAt(0);
                    List_Y.Add(ControllerButtonYPressed ? 1 : 0);
                    Controller_Y = List_Y.Average() > 0 ? true : false;
                }
                else
                {
                    List_Y.Add(0);
                }
                if (List_LB.Count >= 5)
                {
                    List_LB.RemoveAt(0);
                    List_LB.Add(ControllerButtonShoulderLeftPressed ? 1 : 0);
                    Controller_LB = List_LB.Average() > 0 ? true : false;
                }
                else
                {
                    List_LB.Add(0);
                }
                if (List_RB.Count >= 5)
                {
                    List_RB.RemoveAt(0);
                    List_RB.Add(ControllerButtonShoulderRightPressed ? 1 : 0);
                    Controller_RB = List_RB.Average() > 0 ? true : false;
                }
                else
                {
                    List_RB.Add(0);
                }
                if (List_MAP.Count >= 5)
                {
                    List_MAP.RemoveAt(0);
                    List_MAP.Add(ControllerButtonBackPressed ? 1 : 0);
                    Controller_MAP = List_MAP.Average() > 0 ? true : false;
                }
                else
                {
                    List_MAP.Add(0);
                }
                if (List_MENU.Count >= 5)
                {
                    List_MENU.RemoveAt(0);
                    List_MENU.Add(ControllerButtonStartPressed ? 1 : 0);
                    Controller_MENU = List_MENU.Average() > 0 ? true : false;
                }
                else
                {
                    List_MENU.Add(0);
                }
                if (List_LSTICK.Count >= 5)
                {
                    List_LSTICK.RemoveAt(0);
                    List_LSTICK.Add(ControllerThumbpadLeftPressed ? 1 : 0);
                    Controller_LSTICK = List_LSTICK.Average() > 0 ? true : false;
                }
                else
                {
                    List_LSTICK.Add(0);
                }
                if (List_RSTICK.Count >= 5)
                {
                    List_RSTICK.RemoveAt(0);
                    List_RSTICK.Add(ControllerThumbpadRightPressed ? 1 : 0);
                    Controller_RSTICK = List_RSTICK.Average() > 0 ? true : false;
                }
                else
                {
                    List_RSTICK.Add(0);
                }
                if (List_DU.Count >= 5)
                {
                    List_DU.RemoveAt(0);
                    List_DU.Add(ControllerButtonUpPressed ? 1 : 0);
                    Controller_DU = List_DU.Average() > 0 ? true : false;
                }
                else
                {
                    List_DU.Add(0);
                }
                if (List_DD.Count >= 5)
                {
                    List_DD.RemoveAt(0);
                    List_DD.Add(ControllerButtonDownPressed ? 1 : 0);
                    Controller_DD = List_DD.Average() > 0 ? true : false;
                }
                else
                {
                    List_DD.Add(0);
                }
                if (List_DL.Count >= 5)
                {
                    List_DL.RemoveAt(0);
                    List_DL.Add(ControllerButtonLeftPressed ? 1 : 0);
                    Controller_DL = List_DL.Average() > 0 ? true : false;
                }
                else
                {
                    List_DL.Add(0);
                }
                if (List_DR.Count >= 5)
                {
                    List_DR.RemoveAt(0);
                    List_DR.Add(ControllerButtonRightPressed ? 1 : 0);
                    Controller_DR = List_DR.Average() > 0 ? true : false;
                }
                else
                {
                    List_DR.Add(0);
                }
                if (List_XBOX.Count >= 5)
                {
                    List_XBOX.RemoveAt(0);
                    List_XBOX.Add(false ? 1 : 0);
                    Controller_XBOX = List_XBOX.Average() > 0 ? true : false;
                }
                else
                {
                    List_XBOX.Add(0);
                }
                await execScriptHelper($"setController('{Controller_A.ToString()}', '{Controller_B.ToString()}', '{Controller_X.ToString()}', '{Controller_Y.ToString()}', '{Controller_MAP.ToString()}', '{Controller_MENU.ToString()}', '{Controller_DD.ToString()}', '{Controller_DU.ToString()}', '{Controller_DL.ToString()}', '{Controller_DR.ToString()}', '{Controller_LB.ToString()}', '{Controller_RB.ToString()}', '{Controller_LSTICK.ToString()}', '{Controller_RSTICK.ToString()}', '{Controller_LT.ToString()}', '{Controller_RT.ToString()}', '{Controller_XBOX.ToString()}', '{Controller_LX.ToString()}', '{Controller_LY.ToString()}', '{Controller_RX.ToString()}', '{Controller_RY.ToString()}');");
            }
            catch { }
        }
        private async Task<String> execScriptHelper(String script)
        {
            var x = await webView21.ExecuteScriptAsync(script).ConfigureAwait(false);
            return x;
        }
        private async void timer1_Tick(object sender, EventArgs e)
        {
            if (!closed)
            {
                try
                {
                    bmp = new Bitmap(minimapend - minimapstart, minimapend - minimapstart);
                    Graphics graphics = Graphics.FromImage(bmp as Image);
                    graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    graphics.SmoothingMode = SmoothingMode.HighSpeed;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighSpeed;
                    graphics.Clear(Color.Transparent);
                    graphics.CopyFromScreen(minimapstart, minimapstart, 0, 0, bmp.Size);
                    this.pictureBox1.Image = bmp;
                    graphics.Dispose();
                }
                catch { }
                SetDots(ir1x, ir1y, ir2x, ir2y);
                taskEmulate();
                try
                {
                    ComputeData();
                    string stringinject = @"
                        try {
                            var h = window.innerHeight;
                            var width = h * 9 / 16 + 'px';
                            var left = h * 9 / 16 / 2 + 'px';
                            var parentcanvas = document.getElementById('parentcanvas');
                            if (parentcanvas == null) {
                                parentcanvas = document.createElement('div');
                                document.body.insertBefore(parentcanvas, document.body.firstChild);
                                parentcanvas.id = 'parentcanvas';
                            }
                            parentcanvas.style.position = 'absolute';
                            parentcanvas.style.display = 'inline-block';
                            parentcanvas.style.width = width;
                            parentcanvas.style.height = '1000px';
                            parentcanvas.style.left = 'calc(50% - left)';
                            parentcanvas.style.bottom = '0px';
                            parentcanvas.style.backgroundColor = 'transparent';
                            var canvas = document.getElementsByTagName('canvas');
                            if (canvas.length == 0) {
                                canvas = document.createElement('canvas');
                                parentcanvas.append(canvas);
                                canvas.id = 'canvas';
                            }
                            canvas = document.getElementById('canvas');
                            canvas.width = parentcanvas.clientWidth;
                            canvas.height = parentcanvas.clientHeight;
                            var WIDTH = canvas.width;
                            var HEIGHT = canvas.height;
                            var ctx = canvas.getContext('2d');
                            ctx.fillStyle = 'transparent';
                            ctx.fillRect(0, 0, WIDTH, HEIGHT);
                            var audiorawdata = [rawdata100];
                            var barWidth = WIDTH / 199;
                            var barHeight = HEIGHT;
                            var x = 0;
                            for (var i = 1; i < 200; i += 1) {
                                barHeight = audiorawdata[i] / 2;
                                ctx.fillStyle = 'white';
                                ctx.strokeStyle = 'white';
                                ctx.fillRect(x, HEIGHT - barHeight, barWidth, barHeight);
                                x += barWidth;
                            }
                            ctx.stroke();
                        }
                        catch {}
                    ";
                    await execScriptHelper(stringinject.Replace("rawdata100", (int)barDataLeft[0] + ", " + (int)barDataLeft[1] + ", " + (int)barDataLeft[2] + ", " + (int)barDataLeft[3] + ", " + (int)barDataLeft[4] + ", " + (int)barDataLeft[5] + ", " + (int)barDataLeft[6] + ", " + (int)barDataLeft[7] + ", " + (int)barDataLeft[8] + ", " + (int)barDataLeft[9] + ", " + (int)barDataLeft[10] + ", " + (int)barDataLeft[11] + ", " + (int)barDataLeft[12] + ", " + (int)barDataLeft[13] + ", " + (int)barDataLeft[14] + ", " + (int)barDataLeft[15] + ", " + (int)barDataLeft[16] + ", " + (int)barDataLeft[17] + ", " + (int)barDataLeft[18] + ", " + (int)barDataLeft[19] + ", " + (int)barDataLeft[20] + ", " + (int)barDataLeft[21] + ", " + (int)barDataLeft[22] + ", " + (int)barDataLeft[23] + ", " + (int)barDataLeft[24] + ", " + (int)barDataLeft[25] + ", " + (int)barDataLeft[26] + ", " + (int)barDataLeft[27] + ", " + (int)barDataLeft[28] + ", " + (int)barDataLeft[29] + ", " + (int)barDataLeft[30] + ", " + (int)barDataLeft[31] + ", " + (int)barDataLeft[32] + ", " + (int)barDataLeft[33] + ", " + (int)barDataLeft[34] + ", " + (int)barDataLeft[35] + ", " + (int)barDataLeft[36] + ", " + (int)barDataLeft[37] + ", " + (int)barDataLeft[38] + ", " + (int)barDataLeft[39] + ", " + (int)barDataLeft[40] + ", " + (int)barDataLeft[41] + ", " + (int)barDataLeft[42] + ", " + (int)barDataLeft[43] + ", " + (int)barDataLeft[44] + ", " + (int)barDataLeft[45] + ", " + (int)barDataLeft[46] + ", " + (int)barDataLeft[47] + ", " + (int)barDataLeft[48] + ", " + (int)barDataLeft[49] + ", " + (int)barDataLeft[50] + ", " + (int)barDataLeft[51] + ", " + (int)barDataLeft[52] + ", " + (int)barDataLeft[53] + ", " + (int)barDataLeft[54] + ", " + (int)barDataLeft[55] + ", " + (int)barDataLeft[56] + ", " + (int)barDataLeft[57] + ", " + (int)barDataLeft[58] + ", " + (int)barDataLeft[59] + ", " + (int)barDataLeft[60] + ", " + (int)barDataLeft[61] + ", " + (int)barDataLeft[62] + ", " + (int)barDataLeft[63] + ", " + (int)barDataLeft[64] + ", " + (int)barDataLeft[65] + ", " + (int)barDataLeft[66] + ", " + (int)barDataLeft[67] + ", " + (int)barDataLeft[68] + ", " + (int)barDataLeft[69] + ", " + (int)barDataLeft[70] + ", " + (int)barDataLeft[71] + ", " + (int)barDataLeft[72] + ", " + (int)barDataLeft[73] + ", " + (int)barDataLeft[74] + ", " + (int)barDataLeft[75] + ", " + (int)barDataLeft[76] + ", " + (int)barDataLeft[77] + ", " + (int)barDataLeft[78] + ", " + (int)barDataLeft[79] + ", " + (int)barDataLeft[80] + ", " + (int)barDataLeft[81] + ", " + (int)barDataLeft[82] + ", " + (int)barDataLeft[83] + ", " + (int)barDataLeft[84] + ", " + (int)barDataLeft[85] + ", " + (int)barDataLeft[86] + ", " + (int)barDataLeft[87] + ", " + (int)barDataLeft[88] + ", " + (int)barDataLeft[89] + ", " + (int)barDataLeft[90] + ", " + (int)barDataLeft[91] + ", " + (int)barDataLeft[92] + ", " + (int)barDataLeft[93] + ", " + (int)barDataLeft[94] + ", " + (int)barDataLeft[95] + ", " + (int)barDataLeft[96] + ", " + (int)barDataLeft[97] + ", " + (int)barDataLeft[98] + ", " + (int)barDataLeft[99] + ", " + (int)barDataRight[0] + ", " + (int)barDataRight[1] + ", " + (int)barDataRight[2] + ", " + (int)barDataRight[3] + ", " + (int)barDataRight[4] + ", " + (int)barDataRight[5] + ", " + (int)barDataRight[6] + ", " + (int)barDataRight[7] + ", " + (int)barDataRight[8] + ", " + (int)barDataRight[9] + ", " + (int)barDataRight[10] + ", " + (int)barDataRight[11] + ", " + (int)barDataRight[12] + ", " + (int)barDataRight[13] + ", " + (int)barDataRight[14] + ", " + (int)barDataRight[15] + ", " + (int)barDataRight[16] + ", " + (int)barDataRight[17] + ", " + (int)barDataRight[18] + ", " + (int)barDataRight[19] + ", " + (int)barDataRight[20] + ", " + (int)barDataRight[21] + ", " + (int)barDataRight[22] + ", " + (int)barDataRight[23] + ", " + (int)barDataRight[24] + ", " + (int)barDataRight[25] + ", " + (int)barDataRight[26] + ", " + (int)barDataRight[27] + ", " + (int)barDataRight[28] + ", " + (int)barDataRight[29] + ", " + (int)barDataRight[30] + ", " + (int)barDataRight[31] + ", " + (int)barDataRight[32] + ", " + (int)barDataRight[33] + ", " + (int)barDataRight[34] + ", " + (int)barDataRight[35] + ", " + (int)barDataRight[36] + ", " + (int)barDataRight[37] + ", " + (int)barDataRight[38] + ", " + (int)barDataRight[39] + ", " + (int)barDataRight[40] + ", " + (int)barDataRight[41] + ", " + (int)barDataRight[42] + ", " + (int)barDataRight[43] + ", " + (int)barDataRight[44] + ", " + (int)barDataRight[45] + ", " + (int)barDataRight[46] + ", " + (int)barDataRight[47] + ", " + (int)barDataRight[48] + ", " + (int)barDataRight[49] + ", " + (int)barDataRight[50] + ", " + (int)barDataRight[51] + ", " + (int)barDataRight[52] + ", " + (int)barDataRight[53] + ", " + (int)barDataRight[54] + ", " + (int)barDataRight[55] + ", " + (int)barDataRight[56] + ", " + (int)barDataRight[57] + ", " + (int)barDataRight[58] + ", " + (int)barDataRight[59] + ", " + (int)barDataRight[60] + ", " + (int)barDataRight[61] + ", " + (int)barDataRight[62] + ", " + (int)barDataRight[63] + ", " + (int)barDataRight[64] + ", " + (int)barDataRight[65] + ", " + (int)barDataRight[66] + ", " + (int)barDataRight[67] + ", " + (int)barDataRight[68] + ", " + (int)barDataRight[69] + ", " + (int)barDataRight[70] + ", " + (int)barDataRight[71] + ", " + (int)barDataRight[72] + ", " + (int)barDataRight[73] + ", " + (int)barDataRight[74] + ", " + (int)barDataRight[75] + ", " + (int)barDataRight[76] + ", " + (int)barDataRight[77] + ", " + (int)barDataRight[78] + ", " + (int)barDataRight[79] + ", " + (int)barDataRight[80] + ", " + (int)barDataRight[81] + ", " + (int)barDataRight[82] + ", " + (int)barDataRight[83] + ", " + (int)barDataRight[84] + ", " + (int)barDataRight[85] + ", " + (int)barDataRight[86] + ", " + (int)barDataRight[87] + ", " + (int)barDataRight[88] + ", " + (int)barDataRight[89] + ", " + (int)barDataRight[90] + ", " + (int)barDataRight[91] + ", " + (int)barDataRight[92] + ", " + (int)barDataRight[93] + ", " + (int)barDataRight[94] + ", " + (int)barDataRight[95] + ", " + (int)barDataRight[96] + ", " + (int)barDataRight[97] + ", " + (int)barDataRight[98] + ", " + (int)barDataRight[99]));
                }
                catch { }
            }
        }
        private async void taskEmulate()
        {
            try
            {
                for (int inc = 0; inc < xnum; inc++)
                {
                    state = controller[inc].GetState();
                    if (inc == 0)
                    {
                        if (state.Gamepad.Buttons.ToString().Contains("A"))
                            Controller1ButtonAPressed = true;
                        else
                            Controller1ButtonAPressed = false;
                        if (state.Gamepad.Buttons.ToString().EndsWith("B") | state.Gamepad.Buttons.ToString().Contains("B, "))
                            Controller1ButtonBPressed = true;
                        else
                            Controller1ButtonBPressed = false;
                        if (state.Gamepad.Buttons.ToString().Contains("X"))
                            Controller1ButtonXPressed = true;
                        else
                            Controller1ButtonXPressed = false;
                        if (state.Gamepad.Buttons.ToString().Contains("Y"))
                            Controller1ButtonYPressed = true;
                        else
                            Controller1ButtonYPressed = false;
                        if (state.Gamepad.Buttons.ToString().Contains("Start"))
                            Controller1ButtonStartPressed = true;
                        else
                            Controller1ButtonStartPressed = false;
                        if (state.Gamepad.Buttons.ToString().Contains("Back"))
                            Controller1ButtonBackPressed = true;
                        else
                            Controller1ButtonBackPressed = false;
                        if (state.Gamepad.Buttons.ToString().Contains("DPadDown"))
                            Controller1ButtonDownPressed = true;
                        else
                            Controller1ButtonDownPressed = false;
                        if (state.Gamepad.Buttons.ToString().Contains("DPadUp"))
                            Controller1ButtonUpPressed = true;
                        else
                            Controller1ButtonUpPressed = false;
                        if (state.Gamepad.Buttons.ToString().Contains("DPadLeft"))
                            Controller1ButtonLeftPressed = true;
                        else
                            Controller1ButtonLeftPressed = false;
                        if (state.Gamepad.Buttons.ToString().Contains("DPadRight"))
                            Controller1ButtonRightPressed = true;
                        else
                            Controller1ButtonRightPressed = false;
                        if (state.Gamepad.Buttons.ToString().Contains("LeftShoulder"))
                            Controller1ButtonShoulderLeftPressed = true;
                        else
                            Controller1ButtonShoulderLeftPressed = false;
                        if (state.Gamepad.Buttons.ToString().Contains("RightShoulder"))
                            Controller1ButtonShoulderRightPressed = true;
                        else
                            Controller1ButtonShoulderRightPressed = false;
                        if (state.Gamepad.Buttons.ToString().Contains("LeftThumb"))
                            Controller1ThumbpadLeftPressed = true;
                        else
                            Controller1ThumbpadLeftPressed = false;
                        if (state.Gamepad.Buttons.ToString().Contains("RightThumb"))
                            Controller1ThumbpadRightPressed = true;
                        else
                            Controller1ThumbpadRightPressed = false;
                        Controller1TriggerLeftPosition = state.Gamepad.LeftTrigger;
                        Controller1TriggerRightPosition = state.Gamepad.RightTrigger;
                        Controller1ThumbLeftX = state.Gamepad.LeftThumbX;
                        Controller1ThumbLeftY = state.Gamepad.LeftThumbY;
                        Controller1ThumbRightX = state.Gamepad.RightThumbX;
                        Controller1ThumbRightY = state.Gamepad.RightThumbY;
                        SetController(Controller1ButtonAPressed, Controller1ButtonBPressed, Controller1ButtonXPressed, Controller1ButtonYPressed, Controller1ButtonStartPressed, Controller1ButtonBackPressed, Controller1ButtonDownPressed, Controller1ButtonUpPressed, Controller1ButtonLeftPressed, Controller1ButtonRightPressed, Controller1ButtonShoulderLeftPressed, Controller1ButtonShoulderRightPressed, Controller1ThumbpadLeftPressed, Controller1ThumbpadRightPressed, Controller1TriggerLeftPosition, Controller1TriggerRightPosition, Controller1ThumbLeftX, Controller1ThumbLeftY, Controller1ThumbRightX, Controller1ThumbRightY);
                    }
                }
            }
            catch { }
        }
        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            if (shadowBmp == null || shadowBmp.Size != this.Size)
            {
                shadowBmp?.Dispose();
                shadowBmp = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
            }
            foreach (Control control in shadowControls)
            {
                using (GraphicsPath gp = new GraphicsPath())
                {
                    gp.AddRectangle(new Rectangle(control.Location.X, control.Location.Y, control.Size.Width, control.Size.Height));
                    DrawShadowSmooth(gp, 100, 60, shadowBmp);
                }
                e.Graphics.DrawImage(shadowBmp, new Point(0, 0));
            }
        }
        private static void DrawShadowSmooth(GraphicsPath gp, int intensity, int radius, Bitmap dest)
        {
            using (Graphics g = Graphics.FromImage(dest))
            {
                g.Clear(Color.Transparent);
                g.CompositingMode = CompositingMode.SourceCopy;
                double alpha = 0;
                double astep = 0;
                double astepstep = (double)intensity / radius / (radius / 2D);
                for (int thickness = radius; thickness > 0; thickness--)
                {
                    using (Pen p = new Pen(Color.FromArgb((int)alpha, 0, 0, 0), thickness))
                    {
                        p.LineJoin = LineJoin.Round;
                        g.DrawPath(p, gp);
                    }
                    alpha += astep;
                    astep += astepstep;
                }
            }
        }
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                FinalFrame.NewFrame -= FinalFrame_NewFrame;
                if (FinalFrame.IsRunning)
                    FinalFrame.Stop();
            }
            catch { }
        }
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            closed = true;
            Thread.Sleep(100);
            capture.Stop();
        }
        public void GetAudioByteArray()
        {
            capture = new CSCore.SoundIn.WasapiLoopbackCapture();
            capture.Initialize();
            CSCore.IWaveSource source = new CSCore.Streams.SoundInSource(capture);
            fftBuffer = new float[(int)CSCore.DSP.FftSize.Fft4096];
            spectrumProviderLeft = new BasicSpectrumProvider(capture.WaveFormat.Channels, capture.WaveFormat.SampleRate, CSCore.DSP.FftSize.Fft4096);
            lineSpectrumLeft = new LineSpectrum(CSCore.DSP.FftSize.Fft4096)
            {
                SpectrumProvider = spectrumProviderLeft,
                UseAverage = true,
                BarCount = numBars,
                BarSpacing = 2,
                IsXLogScale = false,
                ScalingStrategy = ScalingStrategy.Sqrt
            };
            spectrumProviderRight = new BasicSpectrumProvider(capture.WaveFormat.Channels, capture.WaveFormat.SampleRate, CSCore.DSP.FftSize.Fft4096);
            lineSpectrumRight = new LineSpectrum(CSCore.DSP.FftSize.Fft4096)
            {
                SpectrumProvider = spectrumProviderRight,
                UseAverage = true,
                BarCount = numBars,
                BarSpacing = 2,
                IsXLogScale = false,
                ScalingStrategy = ScalingStrategy.Sqrt
            };
            var notificationSource = new CSCore.Streams.SingleBlockNotificationStream(source.ToSampleSource());
            notificationSource.SingleBlockRead += NotificationSource_SingleBlockRead;
            finalSource = notificationSource.ToWaveSource();
            capture.DataAvailable += Capture_DataAvailable;
            capture.Start();
        }
        public void Capture_DataAvailable(object sender, CSCore.SoundIn.DataAvailableEventArgs e)
        {
            finalSource.Read(e.Data, e.Offset, e.ByteCount);
        }
        public void NotificationSource_SingleBlockRead(object sender, CSCore.Streams.SingleBlockReadEventArgs e)
        {
            spectrumProviderLeft.Add(e.Left, 0);
            spectrumProviderRight.Add(0, e.Right);
        }
        public float[] GetFFtDataLeft()
        {
            lock (barDataLeft)
            {
                lineSpectrumLeft.BarCount = numBars;
                if (numBars != barDataLeft.Length)
                {
                    barDataLeft = new float[numBars];
                }
            }
            if (spectrumProviderLeft.IsNewDataAvailable)
            {
                lineSpectrumLeft.MinimumFrequency = minFreq;
                lineSpectrumLeft.MaximumFrequency = maxFreq;
                lineSpectrumLeft.IsXLogScale = logScale;
                lineSpectrumLeft.BarSpacing = barSpacing;
                lineSpectrumLeft.SpectrumProvider.GetFftData(fftBuffer, this);
                return lineSpectrumLeft.GetSpectrumPoints(100.0f, fftBuffer);
            }
            else
            {
                return null;
            }
        }
        public float[] GetFFtDataRight()
        {
            lock (barDataRight)
            {
                lineSpectrumRight.BarCount = numBars;
                if (numBars != barDataRight.Length)
                {
                    barDataRight = new float[numBars];
                }
            }
            if (spectrumProviderRight.IsNewDataAvailable)
            {
                lineSpectrumRight.MinimumFrequency = minFreq;
                lineSpectrumRight.MaximumFrequency = maxFreq;
                lineSpectrumRight.IsXLogScale = logScale;
                lineSpectrumRight.BarSpacing = barSpacing;
                lineSpectrumRight.SpectrumProvider.GetFftData(fftBuffer, this);
                return lineSpectrumRight.GetSpectrumPoints(100.0f, fftBuffer);
            }
            else
            {
                return null;
            }
        }
        public void ComputeData()
        {
            try
            {
                float[] resData = GetFFtDataLeft();
                int numBars = barDataLeft.Length;
                if (resData == null)
                {
                    return;
                }
                lock (barDataLeft)
                {
                    for (int i = 0; i < numBars && i < resData.Length; i++)
                    {
                        barDataLeft[i] = resData[i] / 100.0f;
                        if (lineSpectrumLeft.UseAverage)
                        {
                            barDataLeft[i] = barDataLeft[i] + highScaleAverage * (float)Math.Sqrt(i / (numBars + 0.0f)) * barDataLeft[i];
                        }
                        else
                        {
                            barDataLeft[i] = barDataLeft[i] + highScaleNotAverage * (float)Math.Sqrt(i / (numBars + 0.0f)) * barDataLeft[i];
                        }
                    }
                }
            }
            catch { }
            try
            {
                float[] resData = GetFFtDataRight();
                int numBars = barDataRight.Length;
                if (resData == null)
                {
                    return;
                }
                lock (barDataRight)
                {
                    for (int i = 0; i < numBars && i < resData.Length; i++)
                    {
                        barDataRight[i] = resData[i] / 100.0f;
                        if (lineSpectrumRight.UseAverage)
                        {
                            barDataRight[i] = barDataRight[i] + highScaleAverage * (float)Math.Sqrt(i / (numBars + 0.0f)) * barDataRight[i];
                        }
                        else
                        {
                            barDataRight[i] = barDataRight[i] + highScaleNotAverage * (float)Math.Sqrt(i / (numBars + 0.0f)) * barDataRight[i];
                        }
                    }
                }
            }
            catch { }
        }
    }
}
namespace WinformsVisualization.Visualization
{
    /// <summary>
    ///     BasicSpectrumProvider
    /// </summary>
    public class BasicSpectrumProvider : CSCore.DSP.FftProvider, ISpectrumProvider
    {
        public readonly int _sampleRate;
        public readonly List<object> _contexts = new List<object>();

        public BasicSpectrumProvider(int channels, int sampleRate, CSCore.DSP.FftSize fftSize)
            : base(channels, fftSize)
        {
            if (sampleRate <= 0)
                throw new ArgumentOutOfRangeException("sampleRate");
            _sampleRate = sampleRate;
        }

        public int GetFftBandIndex(float frequency)
        {
            int fftSize = (int)FftSize;
            double f = _sampleRate / 2.0;
            // ReSharper disable once PossibleLossOfFraction
            return (int)((frequency / f) * (fftSize / 2));
        }

        public bool GetFftData(float[] fftResultBuffer, object context)
        {
            if (_contexts.Contains(context))
                return false;

            _contexts.Add(context);
            GetFftData(fftResultBuffer);
            return true;
        }

        public override void Add(float[] samples, int count)
        {
            base.Add(samples, count);
            if (count > 0)
                _contexts.Clear();
        }

        public override void Add(float left, float right)
        {
            base.Add(left, right);
            _contexts.Clear();
        }
    }
}
namespace WinformsVisualization.Visualization
{
    public interface ISpectrumProvider
    {
        bool GetFftData(float[] fftBuffer, object context);
        int GetFftBandIndex(float frequency);
    }
}
namespace WinformsVisualization.Visualization
{
    internal class GradientCalculator
    {
        public Color[] _colors;

        public GradientCalculator()
        {
        }

        public GradientCalculator(params Color[] colors)
        {
            _colors = colors;
        }

        public Color[] Colors
        {
            get { return _colors ?? (_colors = new Color[] { }); }
            set { _colors = value; }
        }

        public Color GetColor(float perc)
        {
            if (_colors.Length > 1)
            {
                int index = Convert.ToInt32((_colors.Length - 1) * perc - 0.5f);
                float upperIntensity = (perc % (1f / (_colors.Length - 1))) * (_colors.Length - 1);
                if (index + 1 >= Colors.Length)
                    index = Colors.Length - 2;

                return Color.FromArgb(
                    255,
                    (byte)(_colors[index + 1].R * upperIntensity + _colors[index].R * (1f - upperIntensity)),
                    (byte)(_colors[index + 1].G * upperIntensity + _colors[index].G * (1f - upperIntensity)),
                    (byte)(_colors[index + 1].B * upperIntensity + _colors[index].B * (1f - upperIntensity)));
            }
            return _colors.FirstOrDefault();
        }
    }
}
namespace WinformsVisualization.Visualization
{
    public class LineSpectrum : SpectrumBase
    {
        public int _barCount;
        public double _barSpacing;
        public double _barWidth;
        public Size _currentSize;

        public LineSpectrum(CSCore.DSP.FftSize fftSize)
        {
            FftSize = fftSize;
        }

        [Browsable(false)]
        public double BarWidth
        {
            get { return _barWidth; }
        }

        public double BarSpacing
        {
            get { return _barSpacing; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");
                _barSpacing = value;
                UpdateFrequencyMapping();

                RaisePropertyChanged("BarSpacing");
                RaisePropertyChanged("BarWidth");
            }
        }

        public int BarCount
        {
            get { return _barCount; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                _barCount = value;
                SpectrumResolution = value;
                UpdateFrequencyMapping();

                RaisePropertyChanged("BarCount");
                RaisePropertyChanged("BarWidth");
            }
        }

        [BrowsableAttribute(false)]
        public Size CurrentSize
        {
            get { return _currentSize; }
            set
            {
                _currentSize = value;
                RaisePropertyChanged("CurrentSize");
            }
        }

        public Bitmap CreateSpectrumLine(Size size, Brush brush, Color background, bool highQuality)
        {
            if (!UpdateFrequencyMappingIfNessesary(size))
                return null;

            var fftBuffer = new float[(int)FftSize];

            //get the fft result from the spectrum provider
            if (SpectrumProvider.GetFftData(fftBuffer, this))
            {
                using (var pen = new Pen(brush, (float)_barWidth))
                {
                    var bitmap = new Bitmap(size.Width, size.Height);

                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        PrepareGraphics(graphics, highQuality);
                        graphics.Clear(background);

                        CreateSpectrumLineInternal(graphics, pen, fftBuffer, size);
                    }

                    return bitmap;
                }
            }
            return null;
        }

        public Bitmap CreateSpectrumLine(Size size, Color color1, Color color2, Color background, bool highQuality)
        {
            if (!UpdateFrequencyMappingIfNessesary(size))
                return null;

            using (
                Brush brush = new LinearGradientBrush(new RectangleF(0, 0, (float)_barWidth, size.Height), color2,
                    color1, LinearGradientMode.Vertical))
            {
                return CreateSpectrumLine(size, brush, background, highQuality);
            }
        }

        public void CreateSpectrumLineInternal(Graphics graphics, Pen pen, float[] fftBuffer, Size size)
        {
            int height = size.Height;
            //prepare the fft result for rendering 
            SpectrumPointData[] spectrumPoints = CalculateSpectrumPoints(height, fftBuffer);

            //connect the calculated points with lines
            for (int i = 0; i < spectrumPoints.Length; i++)
            {
                SpectrumPointData p = spectrumPoints[i];
                int barIndex = p.SpectrumPointIndex;
                double xCoord = BarSpacing * (barIndex + 1) + (_barWidth * barIndex) + _barWidth / 2;

                var p1 = new PointF((float)xCoord, height);
                var p2 = new PointF((float)xCoord, height - (float)p.Value - 1);

                graphics.DrawLine(pen, p1, p2);
            }
        }

        public override void UpdateFrequencyMapping()
        {
            _barWidth = Math.Max(((_currentSize.Width - (BarSpacing * (BarCount + 1))) / BarCount), 0.00001);
            base.UpdateFrequencyMapping();
        }

        public bool UpdateFrequencyMappingIfNessesary(Size newSize)
        {
            if (newSize != CurrentSize)
            {
                CurrentSize = newSize;
                UpdateFrequencyMapping();
            }

            return newSize.Width > 0 && newSize.Height > 0;
        }

        public void PrepareGraphics(Graphics graphics, bool highQuality)
        {
            if (highQuality)
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.CompositingQuality = CompositingQuality.AssumeLinear;
                graphics.PixelOffsetMode = PixelOffsetMode.Default;
                graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            }
            else
            {
                graphics.SmoothingMode = SmoothingMode.HighSpeed;
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.PixelOffsetMode = PixelOffsetMode.None;
                graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
            }
        }
        public float[] GetSpectrumPoints(float height, float[] fftBuffer)
        {
            SpectrumPointData[] dats = CalculateSpectrumPoints(height, fftBuffer);
            float[] res = new float[dats.Length];
            for (int i = 0; i < dats.Length; i++)
            {
                res[i] = (float)dats[i].Value;
            }

            return res;
        }
    }
}
namespace WinformsVisualization.Visualization
{
    public class SpectrumBase : INotifyPropertyChanged
    {
        public const int ScaleFactorLinear = 9;
        public const int ScaleFactorSqr = 2;
        public const double MinDbValue = -90;
        public const double MaxDbValue = 0;
        public const double DbScale = (MaxDbValue - MinDbValue);

        public int _fftSize;
        public bool _isXLogScale;
        public int _maxFftIndex;
        public int _maximumFrequency = 20000;
        public int _maximumFrequencyIndex;
        public int _minimumFrequency = 20; //Default spectrum from 20Hz to 20kHz
        public int _minimumFrequencyIndex;
        public ScalingStrategy _scalingStrategy;
        public int[] _spectrumIndexMax;
        public int[] _spectrumLogScaleIndexMax;
        public ISpectrumProvider _spectrumProvider;

        public int SpectrumResolution;
        public bool _useAverage;

        public int MaximumFrequency
        {
            get { return _maximumFrequency; }
            set
            {
                if (value <= MinimumFrequency)
                {
                    throw new ArgumentOutOfRangeException("value",
                        "Value must not be less or equal the MinimumFrequency.");
                }
                _maximumFrequency = value;
                UpdateFrequencyMapping();

                RaisePropertyChanged("MaximumFrequency");
            }
        }

        public int MinimumFrequency
        {
            get { return _minimumFrequency; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");
                _minimumFrequency = value;
                UpdateFrequencyMapping();

                RaisePropertyChanged("MinimumFrequency");
            }
        }

        [BrowsableAttribute(false)]
        public ISpectrumProvider SpectrumProvider
        {
            get { return _spectrumProvider; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _spectrumProvider = value;

                RaisePropertyChanged("SpectrumProvider");
            }
        }

        public bool IsXLogScale
        {
            get { return _isXLogScale; }
            set
            {
                _isXLogScale = value;
                UpdateFrequencyMapping();
                RaisePropertyChanged("IsXLogScale");
            }
        }

        public ScalingStrategy ScalingStrategy
        {
            get { return _scalingStrategy; }
            set
            {
                _scalingStrategy = value;
                RaisePropertyChanged("ScalingStrategy");
            }
        }

        public bool UseAverage
        {
            get { return _useAverage; }
            set
            {
                _useAverage = value;
                RaisePropertyChanged("UseAverage");
            }
        }

        [BrowsableAttribute(false)]
        public CSCore.DSP.FftSize FftSize
        {
            get { return (CSCore.DSP.FftSize)_fftSize; }
            set
            {
                if ((int)Math.Log((int)value, 2) % 1 != 0)
                    throw new ArgumentOutOfRangeException("value");

                _fftSize = (int)value;
                _maxFftIndex = _fftSize / 2 - 1;

                RaisePropertyChanged("FFTSize");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void UpdateFrequencyMapping()
        {
            _maximumFrequencyIndex = Math.Min(_spectrumProvider.GetFftBandIndex(MaximumFrequency) + 1, _maxFftIndex);
            _minimumFrequencyIndex = Math.Min(_spectrumProvider.GetFftBandIndex(MinimumFrequency), _maxFftIndex);

            int actualResolution = SpectrumResolution;

            int indexCount = _maximumFrequencyIndex - _minimumFrequencyIndex;
            double linearIndexBucketSize = Math.Round(indexCount / (double)actualResolution, 3);

            _spectrumIndexMax = _spectrumIndexMax.CheckBuffer(actualResolution, true);
            _spectrumLogScaleIndexMax = _spectrumLogScaleIndexMax.CheckBuffer(actualResolution, true);

            double maxLog = Math.Log(actualResolution, actualResolution);
            for (int i = 1; i < actualResolution; i++)
            {
                int logIndex =
                    (int)((maxLog - Math.Log((actualResolution + 1) - i, (actualResolution + 1))) * indexCount) +
                    _minimumFrequencyIndex;

                _spectrumIndexMax[i - 1] = _minimumFrequencyIndex + (int)(i * linearIndexBucketSize);
                _spectrumLogScaleIndexMax[i - 1] = logIndex;
            }

            if (actualResolution > 0)
            {
                _spectrumIndexMax[_spectrumIndexMax.Length - 1] =
                    _spectrumLogScaleIndexMax[_spectrumLogScaleIndexMax.Length - 1] = _maximumFrequencyIndex;
            }
        }

        public virtual SpectrumPointData[] CalculateSpectrumPoints(double maxValue, float[] fftBuffer)
        {
            var dataPoints = new List<SpectrumPointData>();

            double value0 = 0, value = 0;
            double lastValue = 0;
            double actualMaxValue = maxValue;
            int spectrumPointIndex = 0;

            for (int i = _minimumFrequencyIndex; i <= _maximumFrequencyIndex; i++)
            {
                switch (ScalingStrategy)
                {
                    case ScalingStrategy.Decibel:
                        value0 = (((20 * Math.Log10(fftBuffer[i])) - MinDbValue) / DbScale) * actualMaxValue;
                        break;
                    case ScalingStrategy.Linear:
                        value0 = (fftBuffer[i] * ScaleFactorLinear) * actualMaxValue;
                        break;
                    case ScalingStrategy.Sqrt:
                        value0 = ((Math.Sqrt(fftBuffer[i])) * ScaleFactorSqr) * actualMaxValue;
                        break;
                }

                bool recalc = true;

                value = Math.Max(0, Math.Max(value0, value));

                while (spectrumPointIndex <= _spectrumIndexMax.Length - 1 &&
                       i ==
                       (IsXLogScale
                           ? _spectrumLogScaleIndexMax[spectrumPointIndex]
                           : _spectrumIndexMax[spectrumPointIndex]))
                {
                    if (!recalc)
                        value = lastValue;

                    if (value > maxValue)
                        value = maxValue;

                    if (_useAverage && spectrumPointIndex > 0)
                        value = (lastValue + value) / 2.0;

                    dataPoints.Add(new SpectrumPointData { SpectrumPointIndex = spectrumPointIndex, Value = value });

                    lastValue = value;
                    value = 0.0;
                    spectrumPointIndex++;
                    recalc = false;
                }

                //value = 0;
            }

            return dataPoints.ToArray();
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null && !String.IsNullOrEmpty(propertyName))
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        [DebuggerDisplay("{Value}")]
        public struct SpectrumPointData
        {
            public int SpectrumPointIndex;
            public double Value;
        }
    }
}
namespace WinformsVisualization.Visualization
{
    public enum ScalingStrategy
    {
        Decibel,
        Linear,
        Sqrt
    }
}