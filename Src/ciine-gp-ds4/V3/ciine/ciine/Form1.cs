using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using controllers;
using DualShock4API;

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
        private double statex = 0f, statey = 0f, mousex = 0f, mousey = 0f, mousestatex = 0f, mousestatey = 0f, dzx = 0.0f, dzy = 0.0f, viewpower1x = 0f, viewpower2x = 1f, viewpower3x = 0f, viewpower1y = 0.25f, viewpower2y = 0.75f, viewpower3y = 0f;
        public DualShock4 ds4 = new DualShock4();
        public ScpBus scp = new ScpBus();
        private static bool running;
        private int sleeptime = 1;
        private void Form1_Load(object sender, EventArgs e)
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
            Task.Run(() => Start());
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                running = false;
                Thread.Sleep(100);
                scp.UnLoadController();
                ds4.Close();
            }
            catch { }
        }
        private void Start()
        {
            running = true;
            ds4.EnumerateControllers("54C", "9CC", "Wireless Controller");
            Thread.Sleep(2000);
            ds4.BeginPolling();
            scp.LoadController();
            Thread.Sleep(2000);
            Task.Run(() => taskX());
        }
        private void taskX()
        {
            for (; ; )
            {
                if (!running)
                    break;
                ds4.ProcessStateLogic();
                statex = ds4.PS4ControllerGyroX * 15f;
                statey = ds4.PS4ControllerGyroY * 15f;
                if (statex > 0f)
                    mousestatex = Scale(statex, 0f, 32767f, dzx / 100f * 32767f, 32767f);
                if (statex < 0f)
                    mousestatex = Scale(statex, -32767f, 0f, -32767f, -(dzx / 100f) * 32767f);
                if (statey > 0f)
                    mousestatey = Scale(statey, 0f, 32767f, dzy / 100f * 32767f, 32767f);
                if (statey < 0f)
                    mousestatey = Scale(statey, -32767f, 0f, -32767f, -(dzy / 100f) * 32767f);
                mousex = mousestatex + ds4.PS4ControllerRightStickX * 32767f;
                mousey = mousestatey + ds4.PS4ControllerRightStickY * 32767f;
                statex = Math.Abs(mousex) <= 32767f ? mousex : Math.Sign(mousex) * 32767f;
                statey = Math.Abs(mousey) <= 32767f ? mousey : Math.Sign(mousey) * 32767f;
                controller1_send_rightstickx = -statex;
                controller1_send_rightsticky = -statey;
                mousex = -ds4.PS4ControllerLeftStickX * 1024f;
                mousey = -ds4.PS4ControllerLeftStickY * 1024f;
                controller1_send_leftstickx = Math.Abs(-mousex * 32767f / 1024f) <= 32767f ? -mousex * 32767f / 1024f : Math.Sign(-mousex) * 32767f;
                controller1_send_leftsticky = Math.Abs(-mousey * 32767f / 1024f) <= 32767f ? -mousey * 32767f / 1024f : Math.Sign(-mousey) * 32767f;
                controller1_send_down = ds4.PS4ControllerButtonDPadDownPressed;
                controller1_send_left = ds4.PS4ControllerButtonDPadLeftPressed;
                controller1_send_right = ds4.PS4ControllerButtonDPadRightPressed;
                controller1_send_up = ds4.PS4ControllerButtonDPadUpPressed;
                controller1_send_leftstick = ds4.PS4ControllerButtonL3Pressed;
                controller1_send_rightstick = ds4.PS4ControllerButtonR3Pressed;
                controller1_send_B = ds4.PS4ControllerButtonCrossPressed;
                controller1_send_A = ds4.PS4ControllerButtonCirclePressed;
                controller1_send_Y = ds4.PS4ControllerButtonSquarePressed;
                controller1_send_X = ds4.PS4ControllerButtonTrianglePressed;
                controller1_send_lefttriggerposition = ds4.PS4ControllerLeftTriggerPosition * 255;
                controller1_send_righttriggerposition = ds4.PS4ControllerRightTriggerPosition * 255;
                controller1_send_leftbumper = ds4.PS4ControllerButtonL1Pressed;
                controller1_send_rightbumper = ds4.PS4ControllerButtonR1Pressed;
                controller1_send_back = ds4.PS4ControllerButtonLogoPressed;
                controller1_send_start = ds4.PS4ControllerButtonTouchpadPressed;
                scp.SetController(controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition, controller1_send_xbox);
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