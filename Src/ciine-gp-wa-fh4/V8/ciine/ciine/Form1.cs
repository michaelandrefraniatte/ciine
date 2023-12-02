using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
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
        [DllImport("MotionInputPairing.dll", EntryPoint = "wiimoteconnect")]
        public static extern bool wiimoteconnect();
        [DllImport("MotionInputPairing.dll", EntryPoint = "wiimotedisconnect")]
        public static extern bool wiimotedisconnect();
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern uint TimeBeginPeriod(uint ms);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        private static extern uint TimeEndPeriod(uint ms);
        [DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
        private static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
        private static bool controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_xbox;
        private static double controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition;
        private static double WiimoteRawValuesX, WiimoteRawValuesY, WiimoteRawValuesZ, calibrationinit;
        private static bool WiimoteButtonStateA, WiimoteButtonStateB, WiimoteButtonStateMinus, WiimoteButtonStateHome, WiimoteButtonStatePlus, WiimoteButtonStateOne, WiimoteButtonStateTwo, WiimoteButtonStateUp, WiimoteButtonStateDown, WiimoteButtonStateLeft, WiimoteButtonStateRight, running;
        private static double mousex = 0f, mousey = 0f, dzx = 15.0f, dzy = 0f;
        private static uint CurrentResolution = 0;
        private WiiMote wm = new WiiMote();
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
                ScpBus.UnLoadController();
                wm.Close();
                wiimotedisconnect();
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
            do
                Thread.Sleep(1);
            while (!wiimoteconnect());
            wm.ScanWiimote();
            Task.Run(() => taskD());
            Thread.Sleep(1000);
            calibrationinit = -wm.aBuffer[4] + 135f;
            ScpBus.LoadController();
            Task.Run(() => taskX());
        }
        private void taskX()
        {
            for (; ; )
            {
                if (!running)
                    break;
                wm.Reconnection();
                WiimoteButtonStateA = (wm.aBuffer[2] & 0x08) != 0;
                WiimoteButtonStateB = (wm.aBuffer[2] & 0x04) != 0;
                WiimoteButtonStateMinus = (wm.aBuffer[2] & 0x10) != 0;
                WiimoteButtonStateHome = (wm.aBuffer[2] & 0x80) != 0;
                WiimoteButtonStatePlus = (wm.aBuffer[1] & 0x10) != 0;
                WiimoteButtonStateOne = (wm.aBuffer[2] & 0x02) != 0;
                WiimoteButtonStateTwo = (wm.aBuffer[2] & 0x01) != 0;
                WiimoteButtonStateUp = (wm.aBuffer[1] & 0x08) != 0;
                WiimoteButtonStateDown = (wm.aBuffer[1] & 0x04) != 0;
                WiimoteButtonStateLeft = (wm.aBuffer[1] & 0x01) != 0;
                WiimoteButtonStateRight = (wm.aBuffer[1] & 0x02) != 0;
                WiimoteRawValuesX = wm.aBuffer[3] - 135f + calibrationinit;
                WiimoteRawValuesY = wm.aBuffer[4] - 135f + calibrationinit;
                WiimoteRawValuesZ = wm.aBuffer[5] - 135f + calibrationinit;
                if (WiimoteRawValuesY > 0f)
                    mousex = Scale(WiimoteRawValuesY * 45f, 0f, 1024f, (dzx / 100f) * 1024f, 1024f);
                if (WiimoteRawValuesY < 0f)
                    mousex = Scale(WiimoteRawValuesY * 45f, -1024f, 0f, -1024f, -(dzx / 100f) * 1024f);
                if (WiimoteRawValuesX > 0f)
                    mousey = Scale(WiimoteRawValuesX * 90f, 0f, 1024f, (dzy / 100f) * 1024f, 1024f);
                if (WiimoteRawValuesX < 0f)
                    mousey = Scale(WiimoteRawValuesX * 90f, -1024f, 0f, -1024f, -(dzy / 100f) * 1024f);
                controller1_send_leftstickx = Math.Abs(-mousex * 32767f / 1024f) <= 32767f ? -mousex * 32767f / 1024f : Math.Sign(-mousex) * 32767f;
                controller1_send_leftsticky = Math.Abs(-mousey * 32767f / 1024f) <= 32767f ? -mousey * 32767f / 1024f : Math.Sign(-mousey) * 32767f;
                controller1_send_down = WiimoteButtonStateLeft;
                controller1_send_left = WiimoteButtonStateUp;
                controller1_send_right = WiimoteButtonStateDown;
                controller1_send_up = WiimoteButtonStateRight;
                controller1_send_A = WiimoteButtonStateB;
                controller1_send_B = WiimoteButtonStateOne & WiimoteButtonStateTwo;
                controller1_send_Y = WiimoteButtonStateA;
                controller1_send_X = WiimoteButtonStateHome;
                controller1_send_rightbumper = WiimoteButtonStatePlus;
                controller1_send_leftbumper = WiimoteButtonStateMinus;
                controller1_send_lefttriggerposition = WiimoteButtonStateOne ? 255 : 0;
                controller1_send_righttriggerposition = WiimoteButtonStateTwo ? 255 : 0;
                ScpBus.SetController(controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition, controller1_send_xbox);
                Thread.Sleep(1);
            }
        }
        private double Scale(double value, double min, double max, double minScale, double maxScale)
        {
            double scaled = minScale + (double)(value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }
        private void taskD()
        {
            for (; ; )
            {
                if (!running)
                    break;
                wm.BeginPolling();
            }
        }
    }
}