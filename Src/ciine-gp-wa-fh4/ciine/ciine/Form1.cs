﻿using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using controllers;
using WiiMoteAPI;

namespace ciine
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
        private static bool controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_xbox;
        private static double controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition;
        private static double mousex = 0f, mousey = 0f, dzx = 15.0f, dzy = 0f;
        private bool running;
        private WiiMote wm = new WiiMote();
        private XBoxController scp = new XBoxController();
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
            if (keyData == Keys.F2)
            {
                const string message = "• Accelerometer: left stick.\n\r\n\r• Left: down.\n\r\n\r• Up: left.\n\r\n\r• Down: right.\n\r\n\r• Right: up.\n\r\n\r• B: a.\n\r\n\r• One and Two: b.\n\r\n\r• A: y.\n\r\n\r• Home: x.\n\r\n\r• Plus: right bumper.\n\r\n\r• Minus: left bumper.\n\r\n\r• One: left trigger.\n\r\n\r• Two: right trigger.";
                const string caption = "Help";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (keyData == Keys.Escape)
            {
                this.Close();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
            SetProcessPriority();
            Task.Run(() => Start());
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                running = false;
                Thread.Sleep(100);
                scp.Disconnect();
                wm.Close();
            }
            catch { }
        }
        private void SetProcessPriority()
        {
            using (Process p = Process.GetCurrentProcess())
            {
                p.PriorityClass = ProcessPriorityClass.RealTime;
            }
        }
        private void Start()
        {
            running = true;
            wm.ScanWiimote();
            wm.BeginPolling();
            Thread.Sleep(1000);
            wm.Init();
            scp.Connect();
            Task.Run(() => taskX());
        }
        private void taskX()
        {
            for (; ; )
            {
                if (!running)
                    break;
                if (wm.WiimoteRawValuesY > 0f)
                    mousex = Scale(wm.WiimoteRawValuesY * 45f, 0f, 1024f, (dzx / 100f) * 1024f, 1024f);
                if (wm.WiimoteRawValuesY < 0f)
                    mousex = Scale(wm.WiimoteRawValuesY * 45f, -1024f, 0f, -1024f, -(dzx / 100f) * 1024f);
                if (wm.WiimoteRawValuesX > 0f)
                    mousey = Scale(wm.WiimoteRawValuesX * 90f, 0f, 1024f, (dzy / 100f) * 1024f, 1024f);
                if (wm.WiimoteRawValuesX < 0f)
                    mousey = Scale(wm.WiimoteRawValuesX * 90f, -1024f, 0f, -1024f, -(dzy / 100f) * 1024f);
                controller1_send_leftstickx = Math.Abs(-mousex * 32767f / 1024f) <= 32767f ? -mousex * 32767f / 1024f : Math.Sign(-mousex) * 32767f;
                controller1_send_leftsticky = Math.Abs(-mousey * 32767f / 1024f) <= 32767f ? -mousey * 32767f / 1024f : Math.Sign(-mousey) * 32767f;
                controller1_send_down = wm.WiimoteButtonStateLeft;
                controller1_send_left = wm.WiimoteButtonStateUp;
                controller1_send_right = wm.WiimoteButtonStateDown;
                controller1_send_up = wm.WiimoteButtonStateRight;
                controller1_send_A = wm.WiimoteButtonStateB;
                controller1_send_B = wm.WiimoteButtonStateOne & wm.WiimoteButtonStateTwo;
                controller1_send_Y = wm.WiimoteButtonStateA;
                controller1_send_X = wm.WiimoteButtonStateHome;
                controller1_send_rightbumper = wm.WiimoteButtonStatePlus;
                controller1_send_leftbumper = wm.WiimoteButtonStateMinus;
                controller1_send_lefttriggerposition = wm.WiimoteButtonStateOne ? 255 : 0;
                controller1_send_righttriggerposition = wm.WiimoteButtonStateTwo ? 255 : 0;
                scp.Set(controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition, controller1_send_xbox);
                Thread.Sleep(1);
            }
        }
        private double Scale(double value, double min, double max, double minScale, double maxScale)
        {
            double scaled = minScale + (double)(value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }
    }
}