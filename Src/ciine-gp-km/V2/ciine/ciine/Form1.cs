using System;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using Microsoft.Win32.SafeHandles;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SharpDX.DirectInput;
using SharpDX;
using controllers;
using System.Diagnostics;

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
        static DirectInput directInput = new DirectInput();
        private static bool controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_lefttrigger, controller1_send_righttrigger, controller1_send_xbox;
        private static double controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition;
        private static double statex = 0f, statey = 0f, mousex = 0f, mousey = 0f, mousestatex = 0f, mousestatey = 0f, dzx = 0.0f, dzy = 0.0f, viewpower1x = 0f, viewpower2x = 1f, viewpower3x = 0f, viewpower1y = 0.25f, viewpower2y = 0.75f, viewpower3y = 0f;
        private static bool[] getstate = new bool[12];
        private static int sleeptime = 1;
        private ScpBus scp = new ScpBus();
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
            MouseInputHookConnect();
            KeyboardInputHookConnect();
            scp.LoadController();
            Task.Run(() => taskX());
        }
        private void taskX()
        {
            for (; ; )
            {
                if (!running)
                    break;
                MouseInputProcess();
                KeyboardInputProcess();
                double viewpower05x = 0f, viewpower05y = 0f;
                valchanged(0, Keyboard1KeyAdd);
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
                    statex = -Mouse1AxisX * 50f;
                    statey = Mouse1AxisY * 50f;
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
                    controller1_send_left = Keyboard1KeyZ;
                    controller1_send_right = Keyboard1KeyV;
                    controller1_send_down = Keyboard1KeyC;
                    controller1_send_up = Keyboard1KeyX;
                    controller1_send_rightstick = Keyboard1KeyE;
                    controller1_send_leftstick = Keyboard1KeyLeftShift;
                    controller1_send_A = Keyboard1KeySpace;
                    controller1_send_back = Keyboard1KeyTab;
                    controller1_send_start = Keyboard1KeyEscape;
                    controller1_send_X = Mouse1Buttons2 | Keyboard1KeyR;
                    controller1_send_rightbumper = Keyboard1KeyG | Mouse1Buttons4;
                    controller1_send_leftbumper = Keyboard1KeyT | Mouse1Buttons3;
                    controller1_send_B = Keyboard1KeyLeftControl | Keyboard1KeyQ;
                    controller1_send_Y = Mouse1AxisZ > 0 | Mouse1AxisZ < 0;
                    controller1_send_righttriggerposition = Mouse1Buttons0 ? 255 : 0;
                    if (Keyboard1KeyW)
                        controller1_send_leftsticky = 32767;
                    if (Keyboard1KeyS)
                        controller1_send_leftsticky = -32767;
                    if ((!Keyboard1KeyW & !Keyboard1KeyS) | (Keyboard1KeyW & Keyboard1KeyS))
                        controller1_send_leftsticky = 0;
                    if (Keyboard1KeyD)
                        controller1_send_leftstickx = 32767;
                    if (Keyboard1KeyA)
                        controller1_send_leftstickx = -32767;
                    if ((!Keyboard1KeyD & !Keyboard1KeyA) | (Keyboard1KeyD & Keyboard1KeyA))
                        controller1_send_leftstickx = 0;
                    valchanged(1, Mouse1Buttons1);
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
                Mouse1AxisZ = 0;
            }
        }
        private static double Scale(double value, double min, double max, double minScale, double maxScale)
        {
            double scaled = minScale + (double)(value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }
        private static Mouse[] mouse = new Mouse[] { null };
        private static Guid[] mouseGuid = new Guid[] { Guid.Empty };
        private static int mnum = 0;
        public static bool Mouse1Buttons0;
        public static bool Mouse1Buttons1;
        public static bool Mouse1Buttons2;
        public static bool Mouse1Buttons3;
        public static bool Mouse1Buttons4;
        public static bool Mouse1Buttons5;
        public static bool Mouse1Buttons6;
        public static bool Mouse1Buttons7;
        public static int Mouse1AxisX;
        public static int Mouse1AxisY;
        public static int Mouse1AxisZ;
        public static bool MouseInputHookConnect()
        {
            try
            {
                directInput = new DirectInput();
                mouse = new Mouse[] { null };
                mouseGuid = new Guid[] { Guid.Empty };
                mnum = 0;
                foreach (var deviceInstance in directInput.GetDevices(SharpDX.DirectInput.DeviceType.Mouse, DeviceEnumerationFlags.AllDevices))
                {
                    mouseGuid[mnum] = deviceInstance.InstanceGuid;
                    mnum++;
                    if (mnum >= 1)
                        break;
                }
            }
            catch { }
            if (mouseGuid[0] == Guid.Empty)
            {
                return false;
            }
            else
            {
                for (int inc = 0; inc < mnum; inc++)
                {
                    mouse[inc] = new Mouse(directInput);
                    mouse[inc].Properties.BufferSize = 128;
                    mouse[inc].Acquire();
                }
                return true;
            }
        }
        public static void MouseInputProcess()
        {
            for (int inc = 0; inc < mnum; inc++)
            {
                mouse[inc].Poll();
                var datas = mouse[inc].GetBufferedData();
                foreach (var state in datas)
                {
                    if (inc == 0 & state.Offset == MouseOffset.X)
                        Mouse1AxisX = state.Value;
                    if (inc == 0 & state.Offset == MouseOffset.Y)
                        Mouse1AxisY = state.Value;
                    if (inc == 0 & state.Offset == MouseOffset.Z)
                        Mouse1AxisZ = state.Value;
                    if (inc == 0 & state.Offset == MouseOffset.Buttons0 & state.Value == 128)
                        Mouse1Buttons0 = true;
                    if (inc == 0 & state.Offset == MouseOffset.Buttons0 & state.Value == 0)
                        Mouse1Buttons0 = false;
                    if (inc == 0 & state.Offset == MouseOffset.Buttons1 & state.Value == 128)
                        Mouse1Buttons1 = true;
                    if (inc == 0 & state.Offset == MouseOffset.Buttons1 & state.Value == 0)
                        Mouse1Buttons1 = false;
                    if (inc == 0 & state.Offset == MouseOffset.Buttons2 & state.Value == 128)
                        Mouse1Buttons2 = true;
                    if (inc == 0 & state.Offset == MouseOffset.Buttons2 & state.Value == 0)
                        Mouse1Buttons2 = false;
                    if (inc == 0 & state.Offset == MouseOffset.Buttons3 & state.Value == 128)
                        Mouse1Buttons3 = true;
                    if (inc == 0 & state.Offset == MouseOffset.Buttons3 & state.Value == 0)
                        Mouse1Buttons3 = false;
                    if (inc == 0 & state.Offset == MouseOffset.Buttons4 & state.Value == 128)
                        Mouse1Buttons4 = true;
                    if (inc == 0 & state.Offset == MouseOffset.Buttons4 & state.Value == 0)
                        Mouse1Buttons4 = false;
                    if (inc == 0 & state.Offset == MouseOffset.Buttons5 & state.Value == 128)
                        Mouse1Buttons5 = true;
                    if (inc == 0 & state.Offset == MouseOffset.Buttons5 & state.Value == 0)
                        Mouse1Buttons5 = false;
                    if (inc == 0 & state.Offset == MouseOffset.Buttons6 & state.Value == 128)
                        Mouse1Buttons6 = true;
                    if (inc == 0 & state.Offset == MouseOffset.Buttons6 & state.Value == 0)
                        Mouse1Buttons6 = false;
                    if (inc == 0 & state.Offset == MouseOffset.Buttons7 & state.Value == 128)
                        Mouse1Buttons7 = true;
                    if (inc == 0 & state.Offset == MouseOffset.Buttons7 & state.Value == 0)
                        Mouse1Buttons7 = false;
                }
            }
        }
        private static Keyboard[] keyboard = new Keyboard[] { null };
        private static Guid[] keyboardGuid = new Guid[] { Guid.Empty };
        private static int knum = 0;
        public static bool Keyboard1KeyEscape;
        public static bool Keyboard1KeyD1;
        public static bool Keyboard1KeyD2;
        public static bool Keyboard1KeyD3;
        public static bool Keyboard1KeyD4;
        public static bool Keyboard1KeyD5;
        public static bool Keyboard1KeyD6;
        public static bool Keyboard1KeyD7;
        public static bool Keyboard1KeyD8;
        public static bool Keyboard1KeyD9;
        public static bool Keyboard1KeyD0;
        public static bool Keyboard1KeyMinus;
        public static bool Keyboard1KeyEquals;
        public static bool Keyboard1KeyBack;
        public static bool Keyboard1KeyTab;
        public static bool Keyboard1KeyQ;
        public static bool Keyboard1KeyW;
        public static bool Keyboard1KeyE;
        public static bool Keyboard1KeyR;
        public static bool Keyboard1KeyT;
        public static bool Keyboard1KeyY;
        public static bool Keyboard1KeyU;
        public static bool Keyboard1KeyI;
        public static bool Keyboard1KeyO;
        public static bool Keyboard1KeyP;
        public static bool Keyboard1KeyLeftBracket;
        public static bool Keyboard1KeyRightBracket;
        public static bool Keyboard1KeyReturn;
        public static bool Keyboard1KeyLeftControl;
        public static bool Keyboard1KeyA;
        public static bool Keyboard1KeyS;
        public static bool Keyboard1KeyD;
        public static bool Keyboard1KeyF;
        public static bool Keyboard1KeyG;
        public static bool Keyboard1KeyH;
        public static bool Keyboard1KeyJ;
        public static bool Keyboard1KeyK;
        public static bool Keyboard1KeyL;
        public static bool Keyboard1KeySemicolon;
        public static bool Keyboard1KeyApostrophe;
        public static bool Keyboard1KeyGrave;
        public static bool Keyboard1KeyLeftShift;
        public static bool Keyboard1KeyBackslash;
        public static bool Keyboard1KeyZ;
        public static bool Keyboard1KeyX;
        public static bool Keyboard1KeyC;
        public static bool Keyboard1KeyV;
        public static bool Keyboard1KeyB;
        public static bool Keyboard1KeyN;
        public static bool Keyboard1KeyM;
        public static bool Keyboard1KeyComma;
        public static bool Keyboard1KeyPeriod;
        public static bool Keyboard1KeySlash;
        public static bool Keyboard1KeyRightShift;
        public static bool Keyboard1KeyMultiply;
        public static bool Keyboard1KeyLeftAlt;
        public static bool Keyboard1KeySpace;
        public static bool Keyboard1KeyCapital;
        public static bool Keyboard1KeyF1;
        public static bool Keyboard1KeyF2;
        public static bool Keyboard1KeyF3;
        public static bool Keyboard1KeyF4;
        public static bool Keyboard1KeyF5;
        public static bool Keyboard1KeyF6;
        public static bool Keyboard1KeyF7;
        public static bool Keyboard1KeyF8;
        public static bool Keyboard1KeyF9;
        public static bool Keyboard1KeyF10;
        public static bool Keyboard1KeyNumberLock;
        public static bool Keyboard1KeyScrollLock;
        public static bool Keyboard1KeyNumberPad7;
        public static bool Keyboard1KeyNumberPad8;
        public static bool Keyboard1KeyNumberPad9;
        public static bool Keyboard1KeySubtract;
        public static bool Keyboard1KeyNumberPad4;
        public static bool Keyboard1KeyNumberPad5;
        public static bool Keyboard1KeyNumberPad6;
        public static bool Keyboard1KeyAdd;
        public static bool Keyboard1KeyNumberPad1;
        public static bool Keyboard1KeyNumberPad2;
        public static bool Keyboard1KeyNumberPad3;
        public static bool Keyboard1KeyNumberPad0;
        public static bool Keyboard1KeyDecimal;
        public static bool Keyboard1KeyOem102;
        public static bool Keyboard1KeyF11;
        public static bool Keyboard1KeyF12;
        public static bool Keyboard1KeyF13;
        public static bool Keyboard1KeyF14;
        public static bool Keyboard1KeyF15;
        public static bool Keyboard1KeyKana;
        public static bool Keyboard1KeyAbntC1;
        public static bool Keyboard1KeyConvert;
        public static bool Keyboard1KeyNoConvert;
        public static bool Keyboard1KeyYen;
        public static bool Keyboard1KeyAbntC2;
        public static bool Keyboard1KeyNumberPadEquals;
        public static bool Keyboard1KeyPreviousTrack;
        public static bool Keyboard1KeyAT;
        public static bool Keyboard1KeyColon;
        public static bool Keyboard1KeyUnderline;
        public static bool Keyboard1KeyKanji;
        public static bool Keyboard1KeyStop;
        public static bool Keyboard1KeyAX;
        public static bool Keyboard1KeyUnlabeled;
        public static bool Keyboard1KeyNextTrack;
        public static bool Keyboard1KeyNumberPadEnter;
        public static bool Keyboard1KeyRightControl;
        public static bool Keyboard1KeyMute;
        public static bool Keyboard1KeyCalculator;
        public static bool Keyboard1KeyPlayPause;
        public static bool Keyboard1KeyMediaStop;
        public static bool Keyboard1KeyVolumeDown;
        public static bool Keyboard1KeyVolumeUp;
        public static bool Keyboard1KeyWebHome;
        public static bool Keyboard1KeyNumberPadComma;
        public static bool Keyboard1KeyDivide;
        public static bool Keyboard1KeyPrintScreen;
        public static bool Keyboard1KeyRightAlt;
        public static bool Keyboard1KeyPause;
        public static bool Keyboard1KeyHome;
        public static bool Keyboard1KeyUp;
        public static bool Keyboard1KeyPageUp;
        public static bool Keyboard1KeyLeft;
        public static bool Keyboard1KeyRight;
        public static bool Keyboard1KeyEnd;
        public static bool Keyboard1KeyDown;
        public static bool Keyboard1KeyPageDown;
        public static bool Keyboard1KeyInsert;
        public static bool Keyboard1KeyDelete;
        public static bool Keyboard1KeyLeftWindowsKey;
        public static bool Keyboard1KeyRightWindowsKey;
        public static bool Keyboard1KeyApplications;
        public static bool Keyboard1KeyPower;
        public static bool Keyboard1KeySleep;
        public static bool Keyboard1KeyWake;
        public static bool Keyboard1KeyWebSearch;
        public static bool Keyboard1KeyWebFavorites;
        public static bool Keyboard1KeyWebRefresh;
        public static bool Keyboard1KeyWebStop;
        public static bool Keyboard1KeyWebForward;
        public static bool Keyboard1KeyWebBack;
        public static bool Keyboard1KeyMyComputer;
        public static bool Keyboard1KeyMail;
        public static bool Keyboard1KeyMediaSelect;
        public static bool Keyboard1KeyUnknown;
        public static bool KeyboardInputHookConnect()
        {
            try
            {
                directInput = new DirectInput();
                keyboard = new Keyboard[] { null };
                keyboardGuid = new Guid[] { Guid.Empty };
                knum = 0;
                foreach (var deviceInstance in directInput.GetDevices(SharpDX.DirectInput.DeviceType.Keyboard, DeviceEnumerationFlags.AllDevices))
                {
                    keyboardGuid[knum] = deviceInstance.InstanceGuid;
                    knum++;
                    if (knum >= 1)
                        break;
                }
            }
            catch { }
            if (keyboardGuid[0] == Guid.Empty)
            {
                return false;
            }
            else
            {
                for (int inc = 0; inc < knum; inc++)
                {
                    keyboard[inc] = new Keyboard(directInput);
                    keyboard[inc].Properties.BufferSize = 128;
                    keyboard[inc].Acquire();
                }
                return true;
            }
        }
        public static void KeyboardInputProcess()
        {
            for (int inc = 0; inc < knum; inc++)
            {
                keyboard[inc].Poll();
                var datas = keyboard[inc].GetBufferedData();
                foreach (var state in datas)
                {
                    if (inc == 0 & state.IsPressed & state.Key == Key.Escape)
                        Keyboard1KeyEscape = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Escape)
                        Keyboard1KeyEscape = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.D1)
                        Keyboard1KeyD1 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.D1)
                        Keyboard1KeyD1 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.D2)
                        Keyboard1KeyD2 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.D2)
                        Keyboard1KeyD2 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.D3)
                        Keyboard1KeyD3 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.D3)
                        Keyboard1KeyD3 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.D4)
                        Keyboard1KeyD4 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.D4)
                        Keyboard1KeyD4 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.D5)
                        Keyboard1KeyD5 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.D5)
                        Keyboard1KeyD5 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.D6)
                        Keyboard1KeyD6 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.D6)
                        Keyboard1KeyD6 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.D7)
                        Keyboard1KeyD7 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.D7)
                        Keyboard1KeyD7 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.D8)
                        Keyboard1KeyD8 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.D8)
                        Keyboard1KeyD8 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.D9)
                        Keyboard1KeyD9 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.D9)
                        Keyboard1KeyD9 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.D0)
                        Keyboard1KeyD0 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.D0)
                        Keyboard1KeyD0 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Minus)
                        Keyboard1KeyMinus = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Minus)
                        Keyboard1KeyMinus = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Equals)
                        Keyboard1KeyEquals = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Equals)
                        Keyboard1KeyEquals = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Back)
                        Keyboard1KeyBack = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Back)
                        Keyboard1KeyBack = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Tab)
                        Keyboard1KeyTab = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Tab)
                        Keyboard1KeyTab = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Q)
                        Keyboard1KeyQ = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Q)
                        Keyboard1KeyQ = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.W)
                        Keyboard1KeyW = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.W)
                        Keyboard1KeyW = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.E)
                        Keyboard1KeyE = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.E)
                        Keyboard1KeyE = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.R)
                        Keyboard1KeyR = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.R)
                        Keyboard1KeyR = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.T)
                        Keyboard1KeyT = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.T)
                        Keyboard1KeyT = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Y)
                        Keyboard1KeyY = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Y)
                        Keyboard1KeyY = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.U)
                        Keyboard1KeyU = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.U)
                        Keyboard1KeyU = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.I)
                        Keyboard1KeyI = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.I)
                        Keyboard1KeyI = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.O)
                        Keyboard1KeyO = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.O)
                        Keyboard1KeyO = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.P)
                        Keyboard1KeyP = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.P)
                        Keyboard1KeyP = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.LeftBracket)
                        Keyboard1KeyLeftBracket = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.LeftBracket)
                        Keyboard1KeyLeftBracket = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.RightBracket)
                        Keyboard1KeyRightBracket = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.RightBracket)
                        Keyboard1KeyRightBracket = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Return)
                        Keyboard1KeyReturn = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Return)
                        Keyboard1KeyReturn = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.LeftControl)
                        Keyboard1KeyLeftControl = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.LeftControl)
                        Keyboard1KeyLeftControl = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.A)
                        Keyboard1KeyA = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.A)
                        Keyboard1KeyA = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.S)
                        Keyboard1KeyS = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.S)
                        Keyboard1KeyS = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.D)
                        Keyboard1KeyD = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.D)
                        Keyboard1KeyD = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.F)
                        Keyboard1KeyF = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.F)
                        Keyboard1KeyF = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.G)
                        Keyboard1KeyG = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.G)
                        Keyboard1KeyG = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.H)
                        Keyboard1KeyH = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.H)
                        Keyboard1KeyH = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.J)
                        Keyboard1KeyJ = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.J)
                        Keyboard1KeyJ = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.K)
                        Keyboard1KeyK = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.K)
                        Keyboard1KeyK = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.L)
                        Keyboard1KeyL = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.L)
                        Keyboard1KeyL = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Semicolon)
                        Keyboard1KeySemicolon = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Semicolon)
                        Keyboard1KeySemicolon = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Apostrophe)
                        Keyboard1KeyApostrophe = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Apostrophe)
                        Keyboard1KeyApostrophe = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Grave)
                        Keyboard1KeyGrave = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Grave)
                        Keyboard1KeyGrave = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.LeftShift)
                        Keyboard1KeyLeftShift = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.LeftShift)
                        Keyboard1KeyLeftShift = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Backslash)
                        Keyboard1KeyBackslash = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Backslash)
                        Keyboard1KeyBackslash = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Z)
                        Keyboard1KeyZ = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Z)
                        Keyboard1KeyZ = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.X)
                        Keyboard1KeyX = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.X)
                        Keyboard1KeyX = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.C)
                        Keyboard1KeyC = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.C)
                        Keyboard1KeyC = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.V)
                        Keyboard1KeyV = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.V)
                        Keyboard1KeyV = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.B)
                        Keyboard1KeyB = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.B)
                        Keyboard1KeyB = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.N)
                        Keyboard1KeyN = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.N)
                        Keyboard1KeyN = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.M)
                        Keyboard1KeyM = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.M)
                        Keyboard1KeyM = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Comma)
                        Keyboard1KeyComma = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Comma)
                        Keyboard1KeyComma = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Period)
                        Keyboard1KeyPeriod = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Period)
                        Keyboard1KeyPeriod = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Slash)
                        Keyboard1KeySlash = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Slash)
                        Keyboard1KeySlash = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.RightShift)
                        Keyboard1KeyRightShift = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.RightShift)
                        Keyboard1KeyRightShift = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Multiply)
                        Keyboard1KeyMultiply = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Multiply)
                        Keyboard1KeyMultiply = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.LeftAlt)
                        Keyboard1KeyLeftAlt = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.LeftAlt)
                        Keyboard1KeyLeftAlt = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Space)
                        Keyboard1KeySpace = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Space)
                        Keyboard1KeySpace = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Capital)
                        Keyboard1KeyCapital = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Capital)
                        Keyboard1KeyCapital = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.F1)
                        Keyboard1KeyF1 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.F1)
                        Keyboard1KeyF1 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.F2)
                        Keyboard1KeyF2 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.F2)
                        Keyboard1KeyF2 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.F3)
                        Keyboard1KeyF3 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.F3)
                        Keyboard1KeyF3 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.F4)
                        Keyboard1KeyF4 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.F4)
                        Keyboard1KeyF4 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.F5)
                        Keyboard1KeyF5 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.F5)
                        Keyboard1KeyF5 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.F6)
                        Keyboard1KeyF6 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.F6)
                        Keyboard1KeyF6 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.F7)
                        Keyboard1KeyF7 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.F7)
                        Keyboard1KeyF7 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.F8)
                        Keyboard1KeyF8 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.F8)
                        Keyboard1KeyF8 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.F9)
                        Keyboard1KeyF9 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.F9)
                        Keyboard1KeyF9 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.F10)
                        Keyboard1KeyF10 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.F10)
                        Keyboard1KeyF10 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.NumberLock)
                        Keyboard1KeyNumberLock = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.NumberLock)
                        Keyboard1KeyNumberLock = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.ScrollLock)
                        Keyboard1KeyScrollLock = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.ScrollLock)
                        Keyboard1KeyScrollLock = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.NumberPad7)
                        Keyboard1KeyNumberPad7 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.NumberPad7)
                        Keyboard1KeyNumberPad7 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.NumberPad8)
                        Keyboard1KeyNumberPad8 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.NumberPad8)
                        Keyboard1KeyNumberPad8 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.NumberPad9)
                        Keyboard1KeyNumberPad9 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.NumberPad9)
                        Keyboard1KeyNumberPad9 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Subtract)
                        Keyboard1KeySubtract = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Subtract)
                        Keyboard1KeySubtract = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.NumberPad4)
                        Keyboard1KeyNumberPad4 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.NumberPad4)
                        Keyboard1KeyNumberPad4 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.NumberPad5)
                        Keyboard1KeyNumberPad5 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.NumberPad5)
                        Keyboard1KeyNumberPad5 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.NumberPad6)
                        Keyboard1KeyNumberPad6 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.NumberPad6)
                        Keyboard1KeyNumberPad6 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Add)
                        Keyboard1KeyAdd = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Add)
                        Keyboard1KeyAdd = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.NumberPad1)
                        Keyboard1KeyNumberPad1 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.NumberPad1)
                        Keyboard1KeyNumberPad1 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.NumberPad2)
                        Keyboard1KeyNumberPad2 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.NumberPad2)
                        Keyboard1KeyNumberPad2 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.NumberPad3)
                        Keyboard1KeyNumberPad3 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.NumberPad3)
                        Keyboard1KeyNumberPad3 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.NumberPad0)
                        Keyboard1KeyNumberPad0 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.NumberPad0)
                        Keyboard1KeyNumberPad0 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Decimal)
                        Keyboard1KeyDecimal = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Decimal)
                        Keyboard1KeyDecimal = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Oem102)
                        Keyboard1KeyOem102 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Oem102)
                        Keyboard1KeyOem102 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.F11)
                        Keyboard1KeyF11 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.F11)
                        Keyboard1KeyF11 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.F12)
                        Keyboard1KeyF12 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.F12)
                        Keyboard1KeyF12 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.F13)
                        Keyboard1KeyF13 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.F13)
                        Keyboard1KeyF13 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.F14)
                        Keyboard1KeyF14 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.F14)
                        Keyboard1KeyF14 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.F15)
                        Keyboard1KeyF15 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.F15)
                        Keyboard1KeyF15 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Kana)
                        Keyboard1KeyKana = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Kana)
                        Keyboard1KeyKana = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.AbntC1)
                        Keyboard1KeyAbntC1 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.AbntC1)
                        Keyboard1KeyAbntC1 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Convert)
                        Keyboard1KeyConvert = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Convert)
                        Keyboard1KeyConvert = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.NoConvert)
                        Keyboard1KeyNoConvert = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.NoConvert)
                        Keyboard1KeyNoConvert = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Yen)
                        Keyboard1KeyYen = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Yen)
                        Keyboard1KeyYen = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.AbntC2)
                        Keyboard1KeyAbntC2 = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.AbntC2)
                        Keyboard1KeyAbntC2 = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.NumberPadEquals)
                        Keyboard1KeyNumberPadEquals = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.NumberPadEquals)
                        Keyboard1KeyNumberPadEquals = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.PreviousTrack)
                        Keyboard1KeyPreviousTrack = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.PreviousTrack)
                        Keyboard1KeyPreviousTrack = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.AT)
                        Keyboard1KeyAT = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.AT)
                        Keyboard1KeyAT = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Colon)
                        Keyboard1KeyColon = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Colon)
                        Keyboard1KeyColon = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Underline)
                        Keyboard1KeyUnderline = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Underline)
                        Keyboard1KeyUnderline = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Kanji)
                        Keyboard1KeyKanji = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Kanji)
                        Keyboard1KeyKanji = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Stop)
                        Keyboard1KeyStop = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Stop)
                        Keyboard1KeyStop = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.AX)
                        Keyboard1KeyAX = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.AX)
                        Keyboard1KeyAX = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Unlabeled)
                        Keyboard1KeyUnlabeled = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Unlabeled)
                        Keyboard1KeyUnlabeled = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.NextTrack)
                        Keyboard1KeyNextTrack = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.NextTrack)
                        Keyboard1KeyNextTrack = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.NumberPadEnter)
                        Keyboard1KeyNumberPadEnter = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.NumberPadEnter)
                        Keyboard1KeyNumberPadEnter = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.RightControl)
                        Keyboard1KeyRightControl = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.RightControl)
                        Keyboard1KeyRightControl = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Mute)
                        Keyboard1KeyMute = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Mute)
                        Keyboard1KeyMute = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Calculator)
                        Keyboard1KeyCalculator = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Calculator)
                        Keyboard1KeyCalculator = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.PlayPause)
                        Keyboard1KeyPlayPause = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.PlayPause)
                        Keyboard1KeyPlayPause = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.MediaStop)
                        Keyboard1KeyMediaStop = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.MediaStop)
                        Keyboard1KeyMediaStop = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.VolumeDown)
                        Keyboard1KeyVolumeDown = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.VolumeDown)
                        Keyboard1KeyVolumeDown = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.VolumeUp)
                        Keyboard1KeyVolumeUp = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.VolumeUp)
                        Keyboard1KeyVolumeUp = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.WebHome)
                        Keyboard1KeyWebHome = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.WebHome)
                        Keyboard1KeyWebHome = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.NumberPadComma)
                        Keyboard1KeyNumberPadComma = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.NumberPadComma)
                        Keyboard1KeyNumberPadComma = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Divide)
                        Keyboard1KeyDivide = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Divide)
                        Keyboard1KeyDivide = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.PrintScreen)
                        Keyboard1KeyPrintScreen = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.PrintScreen)
                        Keyboard1KeyPrintScreen = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.RightAlt)
                        Keyboard1KeyRightAlt = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.RightAlt)
                        Keyboard1KeyRightAlt = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Pause)
                        Keyboard1KeyPause = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Pause)
                        Keyboard1KeyPause = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Home)
                        Keyboard1KeyHome = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Home)
                        Keyboard1KeyHome = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Up)
                        Keyboard1KeyUp = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Up)
                        Keyboard1KeyUp = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.PageUp)
                        Keyboard1KeyPageUp = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.PageUp)
                        Keyboard1KeyPageUp = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Left)
                        Keyboard1KeyLeft = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Left)
                        Keyboard1KeyLeft = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Right)
                        Keyboard1KeyRight = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Right)
                        Keyboard1KeyRight = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.End)
                        Keyboard1KeyEnd = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.End)
                        Keyboard1KeyEnd = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Down)
                        Keyboard1KeyDown = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Down)
                        Keyboard1KeyDown = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.PageDown)
                        Keyboard1KeyPageDown = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.PageDown)
                        Keyboard1KeyPageDown = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Insert)
                        Keyboard1KeyInsert = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Insert)
                        Keyboard1KeyInsert = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Delete)
                        Keyboard1KeyDelete = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Delete)
                        Keyboard1KeyDelete = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.LeftWindowsKey)
                        Keyboard1KeyLeftWindowsKey = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.LeftWindowsKey)
                        Keyboard1KeyLeftWindowsKey = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.RightWindowsKey)
                        Keyboard1KeyRightWindowsKey = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.RightWindowsKey)
                        Keyboard1KeyRightWindowsKey = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Applications)
                        Keyboard1KeyApplications = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Applications)
                        Keyboard1KeyApplications = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Power)
                        Keyboard1KeyPower = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Power)
                        Keyboard1KeyPower = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Sleep)
                        Keyboard1KeySleep = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Sleep)
                        Keyboard1KeySleep = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Wake)
                        Keyboard1KeyWake = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Wake)
                        Keyboard1KeyWake = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.WebSearch)
                        Keyboard1KeyWebSearch = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.WebSearch)
                        Keyboard1KeyWebSearch = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.WebFavorites)
                        Keyboard1KeyWebFavorites = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.WebFavorites)
                        Keyboard1KeyWebFavorites = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.WebRefresh)
                        Keyboard1KeyWebRefresh = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.WebRefresh)
                        Keyboard1KeyWebRefresh = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.WebStop)
                        Keyboard1KeyWebStop = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.WebStop)
                        Keyboard1KeyWebStop = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.WebForward)
                        Keyboard1KeyWebForward = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.WebForward)
                        Keyboard1KeyWebForward = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.WebBack)
                        Keyboard1KeyWebBack = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.WebBack)
                        Keyboard1KeyWebBack = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.MyComputer)
                        Keyboard1KeyMyComputer = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.MyComputer)
                        Keyboard1KeyMyComputer = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Mail)
                        Keyboard1KeyMail = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Mail)
                        Keyboard1KeyMail = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.MediaSelect)
                        Keyboard1KeyMediaSelect = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.MediaSelect)
                        Keyboard1KeyMediaSelect = false;
                    if (inc == 0 & state.IsPressed & state.Key == Key.Unknown)
                        Keyboard1KeyUnknown = true;
                    if (inc == 0 & state.IsReleased & state.Key == Key.Unknown)
                        Keyboard1KeyUnknown = false;
                }
            }
        }
    }
}