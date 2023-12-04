using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Numerics;
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
        public static bool PS4ControllerButtonCrossPressed;
        public static bool PS4ControllerButtonCirclePressed;
        public static bool PS4ControllerButtonSquarePressed;
        public static bool PS4ControllerButtonTrianglePressed;
        public static bool PS4ControllerButtonDPadUpPressed;
        public static bool PS4ControllerButtonDPadRightPressed;
        public static bool PS4ControllerButtonDPadDownPressed;
        public static bool PS4ControllerButtonDPadLeftPressed;
        public static bool PS4ControllerButtonL1Pressed;
        public static bool PS4ControllerButtonR1Pressed;
        public static bool PS4ControllerButtonL2Pressed;
        public static bool PS4ControllerButtonR2Pressed;
        public static bool PS4ControllerButtonL3Pressed;
        public static bool PS4ControllerButtonR3Pressed;
        public static bool PS4ControllerButtonCreatePressed;
        public static bool PS4ControllerButtonMenuPressed;
        public static bool PS4ControllerButtonLogoPressed;
        public static bool PS4ControllerButtonTouchpadPressed;
        public static bool PS4ControllerButtonMicPressed;
        public static bool PS4ControllerTouchOn;
        private static double PS4ControllerLeftStickX, PS4ControllerLeftStickY, PS4ControllerRightStickX, PS4ControllerRightStickY, PS4ControllerRightTriggerPosition, PS4ControllerLeftTriggerPosition, PS4ControllerTouchX, PS4ControllerTouchY;
        public static bool PS4ControllerAccelCenter;
        public static double PS4ControllerAccelX, PS4ControllerAccelY, PS4ControllerGyroX, PS4ControllerGyroY;
        public static Vector3 gyr_gPS4 = new Vector3();
        public static Vector3 acc_gPS4 = new Vector3();
        public static Vector3 InitDirectAnglesPS4, DirectAnglesPS4;
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
            }
            catch { }
        }
        private void Start()
        {
            running = true;
            ds4.EnumerateControllers("54C", "9CC", "Wireless Controller");
            Thread.Sleep(2000);
            Task.Run(() => taskD());
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
                PS4ControllerLeftStickX = DualShock4.LeftAnalogStick.X;
                PS4ControllerLeftStickY = DualShock4.LeftAnalogStick.Y;
                PS4ControllerRightStickX = -DualShock4.RightAnalogStick.X;
                PS4ControllerRightStickY = -DualShock4.RightAnalogStick.Y;
                PS4ControllerLeftTriggerPosition = DualShock4.L2;
                PS4ControllerRightTriggerPosition = DualShock4.R2;
                PS4ControllerTouchX = DualShock4.Touchpad1.X;
                PS4ControllerTouchY = DualShock4.Touchpad1.Y;
                PS4ControllerTouchOn = DualShock4.Touchpad1.IsDown;
                gyr_gPS4.X = DualShock4.Gyro.Z;
                gyr_gPS4.Y = -DualShock4.Gyro.X;
                gyr_gPS4.Z = -DualShock4.Gyro.Y;
                PS4ControllerGyroX = gyr_gPS4.Z;
                PS4ControllerGyroY = gyr_gPS4.Y;
                acc_gPS4 = new Vector3(DualShock4.Accelerometer.X, DualShock4.Accelerometer.Z, DualShock4.Accelerometer.Y);
                PS4ControllerAccelCenter = DualShock4.MenuButton;
                if (PS4ControllerAccelCenter)
                    InitDirectAnglesPS4 = acc_gPS4;
                DirectAnglesPS4 = acc_gPS4 - InitDirectAnglesPS4;
                PS4ControllerAccelX = -(DirectAnglesPS4.Y + DirectAnglesPS4.Z) / 6f;
                PS4ControllerAccelY = DirectAnglesPS4.X / 6f;
                PS4ControllerButtonCrossPressed = DualShock4.CrossButton;
                PS4ControllerButtonCirclePressed = DualShock4.CircleButton;
                PS4ControllerButtonSquarePressed = DualShock4.SquareButton;
                PS4ControllerButtonTrianglePressed = DualShock4.TriangleButton;
                PS4ControllerButtonDPadUpPressed = DualShock4.DPadUpButton;
                PS4ControllerButtonDPadRightPressed = DualShock4.DPadRightButton;
                PS4ControllerButtonDPadDownPressed = DualShock4.DPadDownButton;
                PS4ControllerButtonDPadLeftPressed = DualShock4.DPadLeftButton;
                PS4ControllerButtonL1Pressed = DualShock4.L1Button;
                PS4ControllerButtonR1Pressed = DualShock4.R1Button;
                PS4ControllerButtonL2Pressed = DualShock4.L2Button;
                PS4ControllerButtonR2Pressed = DualShock4.R2Button;
                PS4ControllerButtonL3Pressed = DualShock4.L3Button;
                PS4ControllerButtonR3Pressed = DualShock4.R3Button;
                PS4ControllerButtonCreatePressed = DualShock4.CreateButton;
                PS4ControllerButtonMenuPressed = DualShock4.MenuButton;
                PS4ControllerButtonLogoPressed = DualShock4.LogoButton;
                PS4ControllerButtonTouchpadPressed = DualShock4.TouchpadButton;
                PS4ControllerButtonMicPressed = DualShock4.MicButton;
                statex = PS4ControllerGyroX * 15f;
                statey = PS4ControllerGyroY * 15f;
                if (statex > 0f)
                    mousestatex = Scale(statex, 0f, 32767f, dzx / 100f * 32767f, 32767f);
                if (statex < 0f)
                    mousestatex = Scale(statex, -32767f, 0f, -32767f, -(dzx / 100f) * 32767f);
                if (statey > 0f)
                    mousestatey = Scale(statey, 0f, 32767f, dzy / 100f * 32767f, 32767f);
                if (statey < 0f)
                    mousestatey = Scale(statey, -32767f, 0f, -32767f, -(dzy / 100f) * 32767f);
                mousex = mousestatex + PS4ControllerRightStickX * 32767f;
                mousey = mousestatey + PS4ControllerRightStickY * 32767f;
                statex = Math.Abs(mousex) <= 32767f ? mousex : Math.Sign(mousex) * 32767f;
                statey = Math.Abs(mousey) <= 32767f ? mousey : Math.Sign(mousey) * 32767f;
                controller1_send_rightstickx = -statex;
                controller1_send_rightsticky = -statey;
                mousex = -PS4ControllerLeftStickX * 1024f;
                mousey = -PS4ControllerLeftStickY * 1024f;
                controller1_send_leftstickx = Math.Abs(-mousex * 32767f / 1024f) <= 32767f ? -mousex * 32767f / 1024f : Math.Sign(-mousex) * 32767f;
                controller1_send_leftsticky = Math.Abs(-mousey * 32767f / 1024f) <= 32767f ? -mousey * 32767f / 1024f : Math.Sign(-mousey) * 32767f;
                controller1_send_down = PS4ControllerButtonDPadDownPressed;
                controller1_send_left = PS4ControllerButtonDPadLeftPressed;
                controller1_send_right = PS4ControllerButtonDPadRightPressed;
                controller1_send_up = PS4ControllerButtonDPadUpPressed;
                controller1_send_leftstick = PS4ControllerButtonL3Pressed;
                controller1_send_rightstick = PS4ControllerButtonR3Pressed;
                controller1_send_B = PS4ControllerButtonCrossPressed;
                controller1_send_A = PS4ControllerButtonCirclePressed;
                controller1_send_Y = PS4ControllerButtonSquarePressed;
                controller1_send_X = PS4ControllerButtonTrianglePressed;
                controller1_send_lefttriggerposition = PS4ControllerLeftTriggerPosition * 255;
                controller1_send_righttriggerposition = PS4ControllerRightTriggerPosition * 255;
                controller1_send_leftbumper = PS4ControllerButtonL1Pressed;
                controller1_send_rightbumper = PS4ControllerButtonR1Pressed;
                controller1_send_back = PS4ControllerButtonLogoPressed;
                controller1_send_start = PS4ControllerButtonTouchpadPressed;
                scp.SetController(controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition, controller1_send_xbox);
                Thread.Sleep(sleeptime);
            }
        }
        private static double Scale(double value, double min, double max, double minScale, double maxScale)
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
                ds4.BeginPolling();
            }
        }
    }
}