using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using controllers;
using System.Diagnostics;
using KeyboardMouseInputAPI;

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
        private static bool running;
        private static bool controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_lefttrigger, controller1_send_righttrigger, controller1_send_xbox;
        private static double controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition;
        private static double statex = 0f, statey = 0f, mousex = 0f, mousey = 0f, mousestatex = 0f, mousestatey = 0f, dzx = 0.0f, dzy = 0.0f, viewpower1x = 0f, viewpower2x = 1f, viewpower3x = 0f, viewpower1y = 0.25f, viewpower2y = 0.75f, viewpower3y = 0f;
        private static bool[] getstate = new bool[12];
        private static int sleeptime = 1;
        private ScpBus scp = new ScpBus();
        private KeyboardMouseInput kmi =new KeyboardMouseInput();
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
            SetProcessPriority();
            Task.Run(() => Start());
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                running = false;
                Thread.Sleep(100);
                kmi.Close();
                scp.UnLoadController();
            }
            catch { }
        }
        private static void SetProcessPriority()
        {
            using (Process p = Process.GetCurrentProcess())
            {
                p.PriorityClass = ProcessPriorityClass.RealTime;
            }
        }
        private void Start()
        {
            running = true;
            kmi.MouseInputHookConnect();
            kmi.KeyboardInputHookConnect();
            kmi.BeginPollingMouse();
            kmi.BeginPollingKeyboard();
            scp.LoadController();
            Task.Run(() => taskX());
        }
        private void taskX()
        {
            for (; ; )
            {
                if (!running)
                    break;
                double viewpower05x = 0f, viewpower05y = 0f;
                valchanged(0, kmi.Keyboard1KeyAdd);
                if (wd[0] == 1 & !getstate[0])
                {
                    getstate[0] = true;
                }
                else
                {
                    if (wd[0] == 1 & getstate[0])
                    {
                        getstate[0] = false;
                    }
                }
                if (getstate[0])
                {
                    statex = -kmi.Mouse1AxisX * 50f;
                    statey = kmi.Mouse1AxisY * 50f;
                    if (statex >= 1024f)
                        statex = 1024f;
                    if (statex <= -1024f)
                        statex = -1024f;
                    if (statey >= 1024f)
                        statey = 1024f;
                    if (statey <= -1024f)
                        statey = -1024f;
                    if (statex >= 0f)
                        mousex = Scale(Math.Pow(statex, 3f) / Math.Pow(1024f, 2f) * viewpower3x + Math.Pow(statex, 2f) / Math.Pow(1024f, 1f) * viewpower2x + Math.Pow(statex, 1f) / Math.Pow(1024f, 0f) * viewpower1x + Math.Pow(statex, 0.5f) * Math.Pow(1024f, 0.5f) * viewpower05x, 0f, 1024f, (dzx / 100f) * 1024f, 1024f);
                    if (statex <= 0f)
                        mousex = Scale(-Math.Pow(-statex, 3f) / Math.Pow(1024f, 2f) * viewpower3x - Math.Pow(-statex, 2f) / Math.Pow(1024f, 1f) * viewpower2x - Math.Pow(-statex, 1f) / Math.Pow(1024f, 0f) * viewpower1x - Math.Pow(-statex, 0.5f) * Math.Pow(1024f, 0.5f) * viewpower05x, -1024f, 0f, -1024f, -(dzx / 100f) * 1024f);
                    if (statey >= 0f)
                        mousey = Scale(Math.Pow(statey, 3f) / Math.Pow(1024f, 2f) * viewpower3y + Math.Pow(statey, 2f) / Math.Pow(1024f, 1f) * viewpower2y + Math.Pow(statey, 1f) / Math.Pow(1024f, 0f) * viewpower1y + Math.Pow(statey, 0.5f) * Math.Pow(1024f, 0.5f) * viewpower05y, 0f, 1024f, (dzy / 100f) * 1024f, 1024f);
                    if (statey <= 0f)
                        mousey = Scale(-Math.Pow(-statey, 3f) / Math.Pow(1024f, 2f) * viewpower3y - Math.Pow(-statey, 2f) / Math.Pow(1024f, 1f) * viewpower2y - Math.Pow(-statey, 1f) / Math.Pow(1024f, 0f) * viewpower1y - Math.Pow(-statey, 0.5f) * Math.Pow(1024f, 0.5f) * viewpower05y, -1024f, 0f, -1024f, -(dzy / 100f) * 1024f);
                    controller1_send_rightstickx = Math.Abs(-mousex * 32767f / 1024f) <= 32767f ? -mousex * 32767f / 1024f : Math.Sign(-mousex) * 32767f;
                    controller1_send_rightsticky = Math.Abs(-mousey * 32767f / 1024f) <= 32767f ? -mousey * 32767f / 1024f : Math.Sign(-mousey) * 32767f;
                    controller1_send_left = kmi.Keyboard1KeyZ;
                    controller1_send_right = kmi.Keyboard1KeyV;
                    controller1_send_down = kmi.Keyboard1KeyC;
                    controller1_send_up = kmi.Keyboard1KeyX;
                    controller1_send_rightstick = kmi.Keyboard1KeyE;
                    controller1_send_leftstick = kmi.Keyboard1KeyLeftShift;
                    controller1_send_A = kmi.Keyboard1KeySpace;
                    controller1_send_back = kmi.Keyboard1KeyTab;
                    controller1_send_start = kmi.Keyboard1KeyEscape;
                    controller1_send_X = kmi.Mouse1Buttons2 | kmi.Keyboard1KeyR;
                    controller1_send_rightbumper = kmi.Keyboard1KeyG | kmi.Mouse1Buttons4;
                    controller1_send_leftbumper = kmi.Keyboard1KeyT | kmi.Mouse1Buttons3;
                    controller1_send_B = kmi.Keyboard1KeyLeftControl | kmi.Keyboard1KeyQ;
                    controller1_send_Y = kmi.Mouse1AxisZ > 0 | kmi.Mouse1AxisZ < 0;
                    controller1_send_righttriggerposition = kmi.Mouse1Buttons0 ? 255 : 0;
                    if (kmi.Keyboard1KeyW)
                        controller1_send_leftsticky = 32767;
                    if (kmi.Keyboard1KeyS)
                        controller1_send_leftsticky = -32767;
                    if ((!kmi.Keyboard1KeyW & !kmi.Keyboard1KeyS) | (kmi.Keyboard1KeyW & kmi.Keyboard1KeyS))
                        controller1_send_leftsticky = 0;
                    if (kmi.Keyboard1KeyD)
                        controller1_send_leftstickx = 32767;
                    if (kmi.Keyboard1KeyA)
                        controller1_send_leftstickx = -32767;
                    if ((!kmi.Keyboard1KeyD & !kmi.Keyboard1KeyA) | (kmi.Keyboard1KeyD & kmi.Keyboard1KeyA))
                        controller1_send_leftstickx = 0;
                    valchanged(1, kmi.Mouse1Buttons1);
                    if (wd[1] == 1 & !getstate[1])
                    {
                        getstate[1] = true;
                    }
                    else
                    {
                        if (wd[1] == 1 & getstate[1])
                        {
                            getstate[1] = false;
                        }
                    }
                    if (controller1_send_X | controller1_send_Y | controller1_send_rightbumper | controller1_send_leftbumper | controller1_send_rightstick | controller1_send_leftstick | controller1_send_back | controller1_send_start)
                    {
                        getstate[1] = false;
                    }
                    controller1_send_lefttriggerposition = getstate[1] ? 255 : 0;
                }
                else
                {
                    controller1_send_rightstickx = 0;
                    controller1_send_rightsticky = 0;
                    controller1_send_leftstickx = 0;
                    controller1_send_leftsticky = 0;
                    controller1_send_left = false;
                    controller1_send_right = false;
                    controller1_send_down = false;
                    controller1_send_up = false;
                    controller1_send_rightstick = false;
                    controller1_send_leftstick = false;
                    controller1_send_A = false;
                    controller1_send_back = false;
                    controller1_send_start = false;
                    controller1_send_X = false;
                    controller1_send_rightbumper = false;
                    controller1_send_leftbumper = false;
                    controller1_send_B = false;
                    controller1_send_Y = false;
                    controller1_send_lefttriggerposition = 0;
                    controller1_send_righttriggerposition = 0;
                }
                scp.SetController(controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition, controller1_send_xbox);
                Thread.Sleep(sleeptime);
                kmi.Mouse1AxisZ = 0;
            }
        }
        private static double Scale(double value, double min, double max, double minScale, double maxScale)
        {
            double scaled = minScale + (double)(value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }
    }
}