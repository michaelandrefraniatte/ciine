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
        private static double mousex = 0f, mousey = 0f, viewpower1x = 0f, viewpower2x = 1f, viewpower3x = 0f, viewpower1y = 0.25f, viewpower2y = 0.75f, viewpower3y = 0f, dzx = 2.0f, dzy = 2.0f, countup = 0, countupup = 0, countxy = 0, county = 0;
        private static bool getstate;
        private bool running;
        private static Valuechange ValueChange = new Valuechange();
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
                const string message = "• B: right trigger.\n\r\n\r• A: left trigger toggle.\n\r\n\r• Accelerometers: right stick button or x.\n\r\n\r• Z: left stick button.\n\r\n\r• C: a.\n\r\n\r• One: back.\n\r\n\r• Two: start.\n\r\n\r• Right: y or x for long press.\n\r\n\r• Minus or Up: left bumper.\n\r\n\r• Plus or Up: right bumper.\n\r\n\r• Down: b.\n\r\n\r• IR: right stick.\n\r\n\r• Home: up.\n\r\n\r• Stick: left stick.\n\r\n\r• Left and Stick: up, down, left, and right.";
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
                controller1_send_righttriggerposition = wm.WiimoteButtonStateB ? 255 : 0;
                ValueChange[0] = wm.WiimoteButtonStateA ? 1 : 0;
                if (ValueChange._ValueChange[0] > 0f & !getstate)
                {
                    getstate = true;
                }
                else
                {
                    if (ValueChange._ValueChange[0] > 0f & getstate)
                    {
                        getstate = false;
                    }
                }
                if (controller1_send_X | controller1_send_Y | controller1_send_rightbumper | controller1_send_leftbumper | controller1_send_rightstick | controller1_send_leftstick | controller1_send_back | controller1_send_start)
                {
                    getstate = false;
                }
                controller1_send_lefttriggerposition = getstate ? 255 : 0;
                controller1_send_rightstick = wm.WiimoteNunchuckStateRawValuesY >= 90f;
                controller1_send_leftstick = wm.WiimoteNunchuckStateZ;
                controller1_send_A = wm.WiimoteNunchuckStateC;
                controller1_send_back = wm.WiimoteButtonStateOne;
                controller1_send_start = wm.WiimoteButtonStateTwo;
                ValueChange[1] = wm.WiimoteButtonStateRight ? 1 : 0;
                if ((countxy > 0 & countxy < 300 & ValueChange._ValueChange[1] < 0f) | county > 0)
                    county++;
                if (county > 100)
                    county = 0;
                countxy = wm.WiimoteButtonStateRight ? countxy + 1 : 0;
                controller1_send_Y = county > 0;
                controller1_send_X = countxy > 300 | ((wm.WiimoteRawValuesZ > 0 ? wm.WiimoteRawValuesZ : -wm.WiimoteRawValuesZ) >= 30f & (wm.WiimoteRawValuesY > 0 ? wm.WiimoteRawValuesY : -wm.WiimoteRawValuesY) >= 30f & (wm.WiimoteRawValuesX > 0 ? wm.WiimoteRawValuesX : -wm.WiimoteRawValuesX) >= 30f);
                controller1_send_leftbumper = wm.WiimoteButtonStateMinus | wm.WiimoteButtonStateUp;
                controller1_send_rightbumper = wm.WiimoteButtonStatePlus | wm.WiimoteButtonStateUp;
                controller1_send_B = wm.WiimoteButtonStateDown;
                if (wm.irx >= 0f & wm.irx <= 1024f)
                    mousex = Scale(wm.irx * wm.irx * wm.irx / 1024f / 1024f * viewpower3x + wm.irx * wm.irx / 1024f * viewpower2x + wm.irx * viewpower1x, 0f, 1024f, dzx / 100f * 1024f, 1024f);
                if (wm.irx <= 0f & wm.irx >= -1024f)
                    mousex = Scale(-(-wm.irx * -wm.irx * -wm.irx) / 1024f / 1024f * viewpower3x - (-wm.irx * -wm.irx) / 1024f * viewpower2x - (-wm.irx) * viewpower1x, -1024f, 0f, -1024f, -(dzx / 100f) * 1024f);
                if (wm.iry >= 0f & wm.iry <= 1024f)
                    mousey = Scale(wm.iry * wm.iry * wm.iry / 1024f / 1024f * viewpower3y + wm.iry * wm.iry / 1024f * viewpower2y + wm.iry * viewpower1y, 0f, 1024f, dzy / 100f * 1024f, 1024f);
                if (wm.iry <= 0f & wm.iry >= -1024f)
                    mousey = Scale(-(-wm.iry * -wm.iry * -wm.iry) / 1024f / 1024f * viewpower3y - (-wm.iry * -wm.iry) / 1024f * viewpower2y - (-wm.iry) * viewpower1y, -1024f, 0f, -1024f, -(dzy / 100f) * 1024f);
                controller1_send_rightstickx = (short)(-mousex / 1024f * 32767f);
                controller1_send_rightsticky = (short)(-mousey / 1024f * 32767f);
                countup = wm.WiimoteButtonStateHome ? countup + 1 : 0;
                if ((countup > 0 & countup < 300) | countupup > 0)
                    countupup++;
                if (countupup > 300)
                    countupup = 0;
                if (!wm.WiimoteButtonStateOne)
                {
                    if (!wm.WiimoteButtonStateLeft)
                    {
                        if (wm.WiimoteNunchuckStateRawJoystickX > 42f)
                            controller1_send_leftstickx = 32767;
                        if (wm.WiimoteNunchuckStateRawJoystickX < -42f)
                            controller1_send_leftstickx = -32767;
                        if (wm.WiimoteNunchuckStateRawJoystickX <= 42f & wm.WiimoteNunchuckStateRawJoystickX >= -42f)
                            controller1_send_leftstickx = 0;
                        if (wm.WiimoteNunchuckStateRawJoystickY > 42f)
                            controller1_send_leftsticky = 32767;
                        if (wm.WiimoteNunchuckStateRawJoystickY < -42f)
                            controller1_send_leftsticky = -32767;
                        if (wm.WiimoteNunchuckStateRawJoystickY <= 42f & wm.WiimoteNunchuckStateRawJoystickY >= -42f)
                            controller1_send_leftsticky = 0;
                        controller1_send_right = false;
                        controller1_send_left = false;
                        controller1_send_up = (countupup > 0 & countupup < 100) | (countupup > 200 & countupup < 300) | countup > 300;
                        controller1_send_down = false;
                    }
                    else
                    {
                        controller1_send_leftstickx = 0;
                        controller1_send_leftsticky = 0;
                        controller1_send_right = wm.WiimoteNunchuckStateRawJoystickX >= 42f;
                        controller1_send_left = wm.WiimoteNunchuckStateRawJoystickX <= -42f;
                        controller1_send_up = wm.WiimoteNunchuckStateRawJoystickY >= 42f;
                        controller1_send_down = wm.WiimoteNunchuckStateRawJoystickY <= -42f;
                    }
                }
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
    public class Valuechange
    {
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern uint TimeBeginPeriod(uint ms);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        private static extern uint TimeEndPeriod(uint ms);
        [DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
        private static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
        private static uint CurrentResolution = 0;
        public double[] _valuechange = { 0, 0 };
        public double[] _ValueChange = { 0, 0 };
        public Valuechange()
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
        }
        public double this[int index]
        {
            get { return _ValueChange[index]; }
            set
            {
                if (_valuechange[index] != value)
                    _ValueChange[index] = value - _valuechange[index];
                else
                    _ValueChange[index] = 0;
                _valuechange[index] = value;
            }
        }
    }
}