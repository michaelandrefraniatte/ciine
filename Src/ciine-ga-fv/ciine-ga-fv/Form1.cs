using Microsoft.Win32.SafeHandles;
using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Reflection;
using controllers;
using DirectInputAPI;

namespace ciine_ga_fv
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern uint TimeBeginPeriod(uint ms);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        private static extern uint TimeEndPeriod(uint ms);
        [DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
        private static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
        private static uint CurrentResolution = 0;
        private static bool running;
        private static bool back, start, A, B, X, Y, up, left, down, right, leftstick, rightstick, leftbumper, rightbumper, lefttrigger, righttrigger, xbox;
        private static double leftstickx, leftsticky, rightstickx, rightsticky, lefttriggerposition, righttriggerposition;
        private double statex = 0f, statey = 0f, mousex = 0f, mousey = 0f, mousestatex = 0f, mousestatey = 0f, dzx = 0.0f, dzy = 0.0f, viewpower1x = 0f, viewpower2x = 1f, viewpower3x = 0f, viewpower1y = 0.25f, viewpower2y = 0.75f, viewpower3y = 0f;
        private double[] mousexp = new double[12];
        private double[] mouseyp = new double[12];
        private int sleeptime = 1;
        private ScpBus scp = new ScpBus();
        private DirectInput di = new DirectInput();
        private static int[] wd = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
        private static int[] wu = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
        public static void valchanged(int n, bool val)
        {
            if (val)
            {
                if (wd[n] <= 1)
                {
                    wd[n] = wd[n] + 1;
                }
                wu[n] = 0;
            }
            else
            {
                if (wu[n] <= 1)
                {
                    wu[n] = wu[n] + 1;
                }
                wd[n] = 0;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
            Task.Run(() => Start());
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e.KeyData);
        }
        private void OnKeyDown(Keys keyData)
        {
            if (keyData == Keys.F1)
            {
                const string message = "• Author: Michaël André Franiatte.\n\r\n\r• Contact: michael.franiatte@gmail.com.\n\r\n\r• Publisher: https://github.com/michaelandrefraniatte.\n\r\n\r• Copyrights: All rights reserved, no permissions granted.\n\r\n\r• License: Not open source, not free of charge to use.";
                const string caption = "About";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (keyData == Keys.Escape)
            {
                this.Close();
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                running = false;
                Thread.Sleep(100);
                di.Close();
                scp.UnLoadController();
            }
            catch { }
        }
        private void Start()
        {
            running = true;
            di.DirectInputHookConnect();
            di.BeginPolling();
            scp.LoadController();
            Task.Run(() => taskX());
        }
        private void taskX()
        {
            while (running)
            {
                mousex = -(di.Joystick1AxisX - 32767f) / 20f;
                mousey = (di.Joystick1AxisY - 32767f) / 20f;
                leftstickx = Math.Abs(-mousex * 32767f / 1024f) <= 32767f ? -mousex * 32767f / 1024f : Math.Sign(-mousex) * 32767f;
                leftsticky = Math.Abs(-mousey * 32767f / 1024f) <= 32767f ? -mousey * 32767f / 1024f : Math.Sign(-mousey) * 32767f;
                if (di.Joystick1Buttons12)
                {
                    mousexp[0] = 0f;
                    mouseyp[0] = 0f;
                }
                mousexp[0] += (di.Joystick1AxisZ - 65535f / 2f) / 5000f;
                mouseyp[0] += (di.Joystick1RotationZ - 65535f / 2f) / 5000f;
                if (mousexp[0] >= 1024f)
                    mousexp[0] = 1024f;
                if (mousexp[0] <= -1024f)
                    mousexp[0] = -1024f;
                if (mouseyp[0] >= 1024f)
                    mouseyp[0] = 1024f;
                if (mouseyp[0] <= -1024f)
                    mouseyp[0] = -1024f;
                mousex = -mousexp[0] - (di.Joystick1AxisZ - 65535f / 2f) / 5000f;
                mousey = mouseyp[0] + (di.Joystick1RotationZ - 65535f / 2f) / 5000f;
                rightstickx = Math.Abs(-mousex * 32767f / 1024f) <= 32767f ? -mousex * 32767f / 1024f : Math.Sign(-mousex) * 32767f;
                rightsticky = Math.Abs(-mousey * 32767f / 1024f) <= 32767f ? -mousey * 32767f / 1024f : Math.Sign(-mousey) * 32767f;
                up = di.Joystick1PointOfViewControllers0 == 4500 | di.Joystick1PointOfViewControllers0 == 0 | di.Joystick1PointOfViewControllers0 == 31500;
                left = di.Joystick1PointOfViewControllers0 == 22500 | di.Joystick1PointOfViewControllers0 == 27000 | di.Joystick1PointOfViewControllers0 == 31500;
                down = di.Joystick1PointOfViewControllers0 == 22500 | di.Joystick1PointOfViewControllers0 == 18000 | di.Joystick1PointOfViewControllers0 == 13500;
                right = di.Joystick1PointOfViewControllers0 == 4500 | di.Joystick1PointOfViewControllers0 == 9000 | di.Joystick1PointOfViewControllers0 == 13500;
                back = di.Joystick1Buttons8;
                start = di.Joystick1Buttons9;
                leftstick = di.Joystick1Buttons10;
                rightstick = di.Joystick1Buttons11;
                leftbumper = di.Joystick1Buttons4;
                rightbumper = di.Joystick1Buttons5;
                A = di.Joystick1Buttons1;
                B = di.Joystick1Buttons2;
                X = di.Joystick1Buttons0;
                Y = di.Joystick1Buttons3;
                lefttriggerposition = di.Joystick1Buttons9 ? 255 : 0;
                righttriggerposition = di.Joystick1Buttons10 ? 255 : 0;
                scp.SetController(back, start, A, B, X, Y, up, left, down, right, leftstick, rightstick, leftbumper, rightbumper, leftstickx, leftsticky, rightstickx, rightsticky, lefttriggerposition, righttriggerposition, xbox);
                Thread.Sleep(sleeptime);
            }
        }
        private static double Scale(double value, double min, double max, double minScale, double maxScale)
        {
            double scaled = minScale + (double)(value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }
    }
}