using System;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;
using System.IO;
using System.Runtime.InteropServices;
using controllers;
using System.Threading;
using System.Threading.Tasks;
using Vector3 = System.Numerics.Vector3;
using JoyconLeftAPI;
using JoyconRightAPI;

namespace ciine_gp_joys
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
        private static bool controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_xbox;
        private static double controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition;
        private static uint CurrentResolution = 0;
        private static bool running;
        private double statex = 0f, statey = 0f, mousex = 0f, mousey = 0f, mousestatex = 0f, mousestatey = 0f, viewpower1x = 1f, viewpower2x = 0f, viewpower3x = 0f, viewpower1y = 1f, viewpower2y = 0f, viewpower3y = 0f, dzx = 20.0f, dzy = 0f;
        private int sleeptime = 1;
        public static bool JoyconLeftButtonSMA, JoyconLeftButtonACC, JoyconLeftRollLeft, JoyconLeftRollRight;
        private static double JoyconLeftStickX, JoyconLeftStickY;
        public static System.Collections.Generic.List<double> LeftValListX = new System.Collections.Generic.List<double>(), LeftValListY = new System.Collections.Generic.List<double>();
        public static bool JoyconLeftAccelCenter, JoyconLeftStickCenter;
        public static double JoyconLeftAccelX, JoyconLeftAccelY, JoyconLeftGyroX, JoyconLeftGyroY;
        private static double[] stickLeft = { 0, 0 };
        private static double[] stickCenterLeft = { 0, 0 };
        private static byte[] stick_rawLeft = { 0, 0, 0 };
        public static Vector3 acc_gLeft = new Vector3();
        public static Vector3 gyr_gLeft = new Vector3();
        public static Vector3 InitDirectAnglesLeft, DirectAnglesLeft;
        public static bool JoyconLeftButtonSHOULDER_1, JoyconLeftButtonSHOULDER_2, JoyconLeftButtonSR, JoyconLeftButtonSL, JoyconLeftButtonDPAD_DOWN, JoyconLeftButtonDPAD_RIGHT, JoyconLeftButtonDPAD_UP, JoyconLeftButtonDPAD_LEFT, JoyconLeftButtonMINUS, JoyconLeftButtonSTICK, JoyconLeftButtonCAPTURE;
        public static float acc_gcalibrationLeftX, acc_gcalibrationLeftY, acc_gcalibrationLeftZ;
        public static bool JoyconRightButtonSPA, JoyconRightButtonACC, JoyconRightRollLeft, JoyconRightRollRight;
        private static double JoyconRightStickX, JoyconRightStickY;
        public static System.Collections.Generic.List<double> RightValListX = new System.Collections.Generic.List<double>(), RightValListY = new System.Collections.Generic.List<double>();
        public static bool JoyconRightAccelCenter, JoyconRightStickCenter;
        public static double JoyconRightAccelX, JoyconRightAccelY, JoyconRightGyroX, JoyconRightGyroY;
        private static double[] stickRight = { 0, 0 };
        private static double[] stickCenterRight = { 0, 0 };
        private static byte[] stick_rawRight = { 0, 0, 0 };
        public static Vector3 acc_gRight = new Vector3();
        public static Vector3 gyr_gRight = new Vector3();
        public static Vector3 InitDirectAnglesRight, DirectAnglesRight;
        public static bool JoyconRightButtonSHOULDER_1, JoyconRightButtonSHOULDER_2, JoyconRightButtonSR, JoyconRightButtonSL, JoyconRightButtonDPAD_DOWN, JoyconRightButtonDPAD_RIGHT, JoyconRightButtonDPAD_UP, JoyconRightButtonDPAD_LEFT, JoyconRightButtonPLUS, JoyconRightButtonSTICK, JoyconRightButtonHOME;
        public static float acc_gcalibrationRightX, acc_gcalibrationRightY, acc_gcalibrationRightZ;
        public static bool ISLEFT = true, ISRIGHT = true;
        private ScpBus scp = new ScpBus();
        private JoyconLeft jl = new JoyconLeft();
        private JoyconRight jr = new JoyconRight();
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
                jl.Close();
                jr.Close();
            }
            catch { }
        }
        private void Start()
        {
            running = true;
            jl.ScanLeft();
            jr.ScanRight();
            Task.Run(() => taskDLeft());
            Task.Run(() => taskDRight());
            Thread.Sleep(1000);
            InitLeftJoycon();
            InitRightJoycon();
            scp.LoadController();
            Task.Run(() => taskX());
        }
        private void taskX()
        {
            while (running)
            {
                ProcessButtonsLeftJoycon();
                ProcessButtonsRightJoycon();
                JoyconLeftAccelCenter = JoyconRightButtonPLUS;
                JoyconRightAccelCenter = JoyconRightButtonPLUS;
                mousex = (JoyconLeftAccelY - JoyconRightAccelY) * 13.5f;
                mousey = JoyconLeftStickY * 32767f * 1.2f;
                statex = Math.Abs(mousex) <= 32767f ? mousex : Math.Sign(mousex) * 32767f;
                statey = Math.Abs(mousey) <= 32767f ? mousey : Math.Sign(mousey) * 32767f;
                if (statex > 0f)
                    mousestatex = Scale(statex, 0f, 32767f, dzx / 100f * 32767f, 32767f);
                if (statex < 0f)
                    mousestatex = Scale(statex, -32767f, 0f, -32767f, -(dzx / 100f) * 32767f);
                if (statey > 0f)
                    mousestatey = Scale(statey, 0f, 32767f, dzy / 100f * 32767f, 32767f);
                if (statey < 0f)
                    mousestatey = Scale(statey, -32767f, 0f, -32767f, -(dzy / 100f) * 32767f);
                mousex = mousestatex + JoyconLeftStickX * 32767f * 1.2f;
                mousey = mousestatey;
                statex = Math.Abs(mousex) <= 32767f ? mousex : Math.Sign(mousex) * 32767f;
                statey = Math.Abs(mousey) <= 32767f ? mousey : Math.Sign(mousey) * 32767f;
                controller1_send_leftstickx = statex;
                controller1_send_leftsticky = statey;
                mousex = JoyconRightStickX * 1400f;
                mousey = JoyconRightStickY * 1400f;
                controller1_send_rightstickx = Math.Abs(-mousex * 32767f / 1024f) <= 32767f ? -mousex * 32767f / 1024f : Math.Sign(-mousex) * 32767f;
                controller1_send_rightsticky = Math.Abs(-mousey * 32767f / 1024f) <= 32767f ? -mousey * 32767f / 1024f : Math.Sign(-mousey) * 32767f;
                controller1_send_up = JoyconLeftButtonDPAD_UP;
                controller1_send_left = JoyconLeftButtonDPAD_LEFT;
                controller1_send_down = JoyconLeftButtonDPAD_DOWN;
                controller1_send_right = JoyconLeftButtonDPAD_RIGHT;
                controller1_send_back = JoyconLeftButtonMINUS | JoyconRightButtonHOME;
                controller1_send_start = JoyconLeftButtonCAPTURE | JoyconRightButtonPLUS;
                controller1_send_leftstick = JoyconLeftButtonSTICK;
                controller1_send_leftbumper = JoyconLeftButtonSL | JoyconLeftButtonSHOULDER_1 | JoyconRightButtonSL;
                controller1_send_rightbumper = JoyconLeftButtonSR | JoyconRightButtonSHOULDER_1 | JoyconRightButtonSR;
                controller1_send_A = JoyconRightButtonDPAD_DOWN;
                controller1_send_B = JoyconRightButtonDPAD_RIGHT;
                controller1_send_X = JoyconRightButtonDPAD_LEFT;
                controller1_send_Y = JoyconRightButtonDPAD_UP;
                controller1_send_rightstick = JoyconRightButtonSTICK;
                controller1_send_lefttriggerposition = JoyconLeftButtonSHOULDER_2 ? 255 : 0;
                controller1_send_righttriggerposition = JoyconRightButtonSHOULDER_2 ? 255 : 0;
                scp.SetController(controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition, controller1_send_xbox);
                Thread.Sleep(sleeptime);
            }
        }
        private static double Scale(double value, double min, double max, double minScale, double maxScale)
        {
            double scaled = minScale + (double)(value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }
        private void taskDLeft()
        {
            while (running)
            {
                try
                {
                    jl.BeginAsyncPolling();
                }
                catch { }
            }
        }
        private void taskDRight()
        {
            while (running)
            {
                try
                {
                    jr.BeginAsyncPolling();
                }
                catch { }
            }
        }
        public void InitLeftJoycon()
        {
            try
            {
                stick_rawLeft[0] = jl.report_bufLeft[6 + (ISLEFT ? 0 : 3)];
                stick_rawLeft[1] = jl.report_bufLeft[7 + (ISLEFT ? 0 : 3)];
                stick_rawLeft[2] = jl.report_bufLeft[8 + (ISLEFT ? 0 : 3)];
                stickCenterLeft[0] = (UInt16)(stick_rawLeft[0] | ((stick_rawLeft[1] & 0xf) << 8));
                stickCenterLeft[1] = (UInt16)((stick_rawLeft[1] >> 4) | (stick_rawLeft[2] << 4));
                acc_gcalibrationLeftX = (Int16)(jl.report_bufLeft[13 + 0 * 12] | ((jl.report_bufLeft[14 + 0 * 12] << 8) & 0xff00)) + (Int16)(jl.report_bufLeft[13 + 1 * 12] | ((jl.report_bufLeft[14 + 1 * 12] << 8) & 0xff00)) + (Int16)(jl.report_bufLeft[13 + 2 * 12] | ((jl.report_bufLeft[14 + 2 * 12] << 8) & 0xff00));
                acc_gcalibrationLeftY = (Int16)(jl.report_bufLeft[15 + 0 * 12] | ((jl.report_bufLeft[16 + 0 * 12] << 8) & 0xff00)) + (Int16)(jl.report_bufLeft[15 + 1 * 12] | ((jl.report_bufLeft[16 + 1 * 12] << 8) & 0xff00)) + (Int16)(jl.report_bufLeft[15 + 2 * 12] | ((jl.report_bufLeft[16 + 2 * 12] << 8) & 0xff00));
                acc_gcalibrationLeftZ = (Int16)(jl.report_bufLeft[17 + 0 * 12] | ((jl.report_bufLeft[18 + 0 * 12] << 8) & 0xff00)) + (Int16)(jl.report_bufLeft[17 + 1 * 12] | ((jl.report_bufLeft[18 + 1 * 12] << 8) & 0xff00)) + (Int16)(jl.report_bufLeft[17 + 2 * 12] | ((jl.report_bufLeft[18 + 2 * 12] << 8) & 0xff00));
            }
            catch { }
        }
        public void ProcessButtonsLeftJoycon()
        {
            try
            {
                if (JoyconLeftStickCenter)
                {
                    stick_rawLeft[0] = jl.report_bufLeft[6 + (ISLEFT ? 0 : 3)];
                    stick_rawLeft[1] = jl.report_bufLeft[7 + (ISLEFT ? 0 : 3)];
                    stick_rawLeft[2] = jl.report_bufLeft[8 + (ISLEFT ? 0 : 3)];
                    stickCenterLeft[0] = (UInt16)(stick_rawLeft[0] | ((stick_rawLeft[1] & 0xf) << 8));
                    stickCenterLeft[1] = (UInt16)((stick_rawLeft[1] >> 4) | (stick_rawLeft[2] << 4));
                }
                stick_rawLeft[0] = jl.report_bufLeft[6 + (ISLEFT ? 0 : 3)];
                stick_rawLeft[1] = jl.report_bufLeft[7 + (ISLEFT ? 0 : 3)];
                stick_rawLeft[2] = jl.report_bufLeft[8 + (ISLEFT ? 0 : 3)];
                stickLeft[0] = ((UInt16)(stick_rawLeft[0] | ((stick_rawLeft[1] & 0xf) << 8)) - stickCenterLeft[0]) / 1440f;
                stickLeft[1] = ((UInt16)((stick_rawLeft[1] >> 4) | (stick_rawLeft[2] << 4)) - stickCenterLeft[1]) / 1440f;
                JoyconLeftStickX = stickLeft[0];
                JoyconLeftStickY = stickLeft[1];
                acc_gLeft.X = ((Int16)(jl.report_bufLeft[13 + 0 * 12] | ((jl.report_bufLeft[14 + 0 * 12] << 8) & 0xff00)) + (Int16)(jl.report_bufLeft[13 + 1 * 12] | ((jl.report_bufLeft[14 + 1 * 12] << 8) & 0xff00)) + (Int16)(jl.report_bufLeft[13 + 2 * 12] | ((jl.report_bufLeft[14 + 2 * 12] << 8) & 0xff00)) - acc_gcalibrationLeftX) * (1.0f / 12000f);
                acc_gLeft.Y = -((Int16)(jl.report_bufLeft[15 + 0 * 12] | ((jl.report_bufLeft[16 + 0 * 12] << 8) & 0xff00)) + (Int16)(jl.report_bufLeft[15 + 1 * 12] | ((jl.report_bufLeft[16 + 1 * 12] << 8) & 0xff00)) + (Int16)(jl.report_bufLeft[15 + 2 * 12] | ((jl.report_bufLeft[16 + 2 * 12] << 8) & 0xff00)) - acc_gcalibrationLeftY) * (1.0f / 12000f);
                acc_gLeft.Z = -((Int16)(jl.report_bufLeft[17 + 0 * 12] | ((jl.report_bufLeft[18 + 0 * 12] << 8) & 0xff00)) + (Int16)(jl.report_bufLeft[17 + 1 * 12] | ((jl.report_bufLeft[18 + 1 * 12] << 8) & 0xff00)) + (Int16)(jl.report_bufLeft[17 + 2 * 12] | ((jl.report_bufLeft[18 + 2 * 12] << 8) & 0xff00)) - acc_gcalibrationLeftZ) * (1.0f / 12000f);
                JoyconLeftButtonSHOULDER_1 = (jl.report_bufLeft[3 + (ISLEFT ? 2 : 0)] & 0x40) != 0;
                JoyconLeftButtonSHOULDER_2 = (jl.report_bufLeft[3 + (ISLEFT ? 2 : 0)] & 0x80) != 0;
                JoyconLeftButtonSR = (jl.report_bufLeft[3 + (ISLEFT ? 2 : 0)] & 0x10) != 0;
                JoyconLeftButtonSL = (jl.report_bufLeft[3 + (ISLEFT ? 2 : 0)] & 0x20) != 0;
                JoyconLeftButtonDPAD_DOWN = (jl.report_bufLeft[3 + (ISLEFT ? 2 : 0)] & (ISLEFT ? 0x01 : 0x04)) != 0;
                JoyconLeftButtonDPAD_RIGHT = (jl.report_bufLeft[3 + (ISLEFT ? 2 : 0)] & (ISLEFT ? 0x04 : 0x08)) != 0;
                JoyconLeftButtonDPAD_UP = (jl.report_bufLeft[3 + (ISLEFT ? 2 : 0)] & (ISLEFT ? 0x02 : 0x02)) != 0;
                JoyconLeftButtonDPAD_LEFT = (jl.report_bufLeft[3 + (ISLEFT ? 2 : 0)] & (ISLEFT ? 0x08 : 0x01)) != 0;
                JoyconLeftButtonMINUS = (jl.report_bufLeft[4] & 0x01) != 0;
                JoyconLeftButtonCAPTURE = (jl.report_bufLeft[4] & 0x20) != 0;
                JoyconLeftButtonSTICK = (jl.report_bufLeft[4] & (ISLEFT ? 0x08 : 0x04)) != 0;
                JoyconLeftButtonACC = acc_gLeft.X <= -1.13;
                JoyconLeftButtonSMA = JoyconLeftButtonSL | JoyconLeftButtonSR | JoyconLeftButtonMINUS | JoyconLeftButtonACC;
                if (JoyconLeftAccelCenter)
                    InitDirectAnglesLeft = acc_gLeft;
                DirectAnglesLeft = acc_gLeft - InitDirectAnglesLeft;
                JoyconLeftAccelX = DirectAnglesLeft.X * 1350f;
                JoyconLeftAccelY = -DirectAnglesLeft.Y * 1350f;
                gyr_gLeft.X = ((Int16)(jl.report_bufLeft[19 + 0 * 12] | ((jl.report_bufLeft[20 + 0 * 12] << 8) & 0xff00)) + (Int16)(jl.report_bufLeft[19 + 1 * 12] | ((jl.report_bufLeft[20 + 1 * 12] << 8) & 0xff00)) + (Int16)(jl.report_bufLeft[19 + 2 * 12] | ((jl.report_bufLeft[20 + 2 * 12] << 8) & 0xff00)));
                gyr_gLeft.Y = ((Int16)(jl.report_bufLeft[21 + 0 * 12] | ((jl.report_bufLeft[22 + 0 * 12] << 8) & 0xff00)) + (Int16)(jl.report_bufLeft[21 + 1 * 12] | ((jl.report_bufLeft[22 + 1 * 12] << 8) & 0xff00)) + (Int16)(jl.report_bufLeft[21 + 2 * 12] | ((jl.report_bufLeft[22 + 2 * 12] << 8) & 0xff00)));
                gyr_gLeft.Z = ((Int16)(jl.report_bufLeft[23 + 0 * 12] | ((jl.report_bufLeft[24 + 0 * 12] << 8) & 0xff00)) + (Int16)(jl.report_bufLeft[23 + 1 * 12] | ((jl.report_bufLeft[24 + 1 * 12] << 8) & 0xff00)) + (Int16)(jl.report_bufLeft[23 + 2 * 12] | ((jl.report_bufLeft[24 + 2 * 12] << 8) & 0xff00)));
                JoyconLeftGyroX = gyr_gLeft.Z;
                JoyconLeftGyroY = gyr_gLeft.Y;
            }
            catch { }
        }
        public void InitRightJoycon()
        {
            try
            {
                stick_rawRight[0] = jr.report_bufRight[6 + (!ISRIGHT ? 0 : 3)];
                stick_rawRight[1] = jr.report_bufRight[7 + (!ISRIGHT ? 0 : 3)];
                stick_rawRight[2] = jr.report_bufRight[8 + (!ISRIGHT ? 0 : 3)];
                stickCenterRight[0] = (UInt16)(stick_rawRight[0] | ((stick_rawRight[1] & 0xf) << 8));
                stickCenterRight[1] = (UInt16)((stick_rawRight[1] >> 4) | (stick_rawRight[2] << 4));
                acc_gcalibrationRightX = (Int16)(jr.report_bufRight[13 + 0 * 12] | ((jr.report_bufRight[14 + 0 * 12] << 8) & 0xff00)) + (Int16)(jr.report_bufRight[13 + 1 * 12] | ((jr.report_bufRight[14 + 1 * 12] << 8) & 0xff00)) + (Int16)(jr.report_bufRight[13 + 2 * 12] | ((jr.report_bufRight[14 + 2 * 12] << 8) & 0xff00));
                acc_gcalibrationRightY = (Int16)(jr.report_bufRight[15 + 0 * 12] | ((jr.report_bufRight[16 + 0 * 12] << 8) & 0xff00)) + (Int16)(jr.report_bufRight[15 + 1 * 12] | ((jr.report_bufRight[16 + 1 * 12] << 8) & 0xff00)) + (Int16)(jr.report_bufRight[15 + 2 * 12] | ((jr.report_bufRight[16 + 2 * 12] << 8) & 0xff00));
                acc_gcalibrationRightZ = (Int16)(jr.report_bufRight[17 + 0 * 12] | ((jr.report_bufRight[18 + 0 * 12] << 8) & 0xff00)) + (Int16)(jr.report_bufRight[17 + 1 * 12] | ((jr.report_bufRight[18 + 1 * 12] << 8) & 0xff00)) + (Int16)(jr.report_bufRight[17 + 2 * 12] | ((jr.report_bufRight[18 + 2 * 12] << 8) & 0xff00));
            }
            catch { }
        }
        public void ProcessButtonsRightJoycon()
        {
            try
            {
                if (JoyconRightStickCenter)
                {
                    stick_rawRight[0] = jr.report_bufRight[6 + (!ISRIGHT ? 0 : 3)];
                    stick_rawRight[1] = jr.report_bufRight[7 + (!ISRIGHT ? 0 : 3)];
                    stick_rawRight[2] = jr.report_bufRight[8 + (!ISRIGHT ? 0 : 3)];
                    stickCenterRight[0] = (UInt16)(stick_rawRight[0] | ((stick_rawRight[1] & 0xf) << 8));
                    stickCenterRight[1] = (UInt16)((stick_rawRight[1] >> 4) | (stick_rawRight[2] << 4));
                }
                stick_rawRight[0] = jr.report_bufRight[6 + (!ISRIGHT ? 0 : 3)];
                stick_rawRight[1] = jr.report_bufRight[7 + (!ISRIGHT ? 0 : 3)];
                stick_rawRight[2] = jr.report_bufRight[8 + (!ISRIGHT ? 0 : 3)];
                stickRight[0] = ((UInt16)(stick_rawRight[0] | ((stick_rawRight[1] & 0xf) << 8)) - stickCenterRight[0]) / 1440f;
                stickRight[1] = ((UInt16)((stick_rawRight[1] >> 4) | (stick_rawRight[2] << 4)) - stickCenterRight[1]) / 1440f;
                JoyconRightStickX = -stickRight[0];
                JoyconRightStickY = -stickRight[1];
                acc_gRight.X = ((Int16)(jr.report_bufRight[13 + 0 * 12] | ((jr.report_bufRight[14 + 0 * 12] << 8) & 0xff00)) + (Int16)(jr.report_bufRight[13 + 1 * 12] | ((jr.report_bufRight[14 + 1 * 12] << 8) & 0xff00)) + (Int16)(jr.report_bufRight[13 + 2 * 12] | ((jr.report_bufRight[14 + 2 * 12] << 8) & 0xff00)) - acc_gcalibrationRightX) * (1.0f / 12000f);
                acc_gRight.Y = -((Int16)(jr.report_bufRight[15 + 0 * 12] | ((jr.report_bufRight[16 + 0 * 12] << 8) & 0xff00)) + (Int16)(jr.report_bufRight[15 + 1 * 12] | ((jr.report_bufRight[16 + 1 * 12] << 8) & 0xff00)) + (Int16)(jr.report_bufRight[15 + 2 * 12] | ((jr.report_bufRight[16 + 2 * 12] << 8) & 0xff00)) - acc_gcalibrationRightY) * (1.0f / 12000f);
                acc_gRight.Z = -((Int16)(jr.report_bufRight[17 + 0 * 12] | ((jr.report_bufRight[18 + 0 * 12] << 8) & 0xff00)) + (Int16)(jr.report_bufRight[17 + 1 * 12] | ((jr.report_bufRight[18 + 1 * 12] << 8) & 0xff00)) + (Int16)(jr.report_bufRight[17 + 2 * 12] | ((jr.report_bufRight[18 + 2 * 12] << 8) & 0xff00)) - acc_gcalibrationRightZ) * (1.0f / 12000f);
                JoyconRightButtonSHOULDER_1 = (jr.report_bufRight[3 + (!ISRIGHT ? 2 : 0)] & 0x40) != 0;
                JoyconRightButtonSHOULDER_2 = (jr.report_bufRight[3 + (!ISRIGHT ? 2 : 0)] & 0x80) != 0;
                JoyconRightButtonSR = (jr.report_bufRight[3 + (!ISRIGHT ? 2 : 0)] & 0x10) != 0;
                JoyconRightButtonSL = (jr.report_bufRight[3 + (!ISRIGHT ? 2 : 0)] & 0x20) != 0;
                JoyconRightButtonDPAD_DOWN = (jr.report_bufRight[3 + (!ISRIGHT ? 2 : 0)] & (!ISRIGHT ? 0x01 : 0x04)) != 0;
                JoyconRightButtonDPAD_RIGHT = (jr.report_bufRight[3 + (!ISRIGHT ? 2 : 0)] & (!ISRIGHT ? 0x04 : 0x08)) != 0;
                JoyconRightButtonDPAD_UP = (jr.report_bufRight[3 + (!ISRIGHT ? 2 : 0)] & (!ISRIGHT ? 0x02 : 0x02)) != 0;
                JoyconRightButtonDPAD_LEFT = (jr.report_bufRight[3 + (!ISRIGHT ? 2 : 0)] & (!ISRIGHT ? 0x08 : 0x01)) != 0;
                JoyconRightButtonPLUS = ((jr.report_bufRight[4] & 0x02) != 0);
                JoyconRightButtonHOME = ((jr.report_bufRight[4] & 0x10) != 0);
                JoyconRightButtonSTICK = ((jr.report_bufRight[4] & (!ISRIGHT ? 0x08 : 0x04)) != 0);
                JoyconRightButtonACC = acc_gRight.X <= -1.13;
                JoyconRightButtonSPA = JoyconRightButtonSL | JoyconRightButtonSR | JoyconRightButtonPLUS | JoyconRightButtonACC;
                if (JoyconRightAccelCenter)
                    InitDirectAnglesRight = acc_gRight;
                DirectAnglesRight = acc_gRight - InitDirectAnglesRight;
                JoyconRightAccelX = DirectAnglesRight.X * 1350f;
                JoyconRightAccelY = -DirectAnglesRight.Y * 1350f;
                gyr_gRight.X = ((Int16)(jr.report_bufRight[19 + 0 * 12] | ((jr.report_bufRight[20 + 0 * 12] << 8) & 0xff00)) + (Int16)(jr.report_bufRight[19 + 1 * 12] | ((jr.report_bufRight[20 + 1 * 12] << 8) & 0xff00)) + (Int16)(jr.report_bufRight[19 + 2 * 12] | ((jr.report_bufRight[20 + 2 * 12] << 8) & 0xff00)));
                gyr_gRight.Y = ((Int16)(jr.report_bufRight[21 + 0 * 12] | ((jr.report_bufRight[22 + 0 * 12] << 8) & 0xff00)) + (Int16)(jr.report_bufRight[21 + 1 * 12] | ((jr.report_bufRight[22 + 1 * 12] << 8) & 0xff00)) + (Int16)(jr.report_bufRight[21 + 2 * 12] | ((jr.report_bufRight[22 + 2 * 12] << 8) & 0xff00)));
                gyr_gRight.Z = ((Int16)(jr.report_bufRight[23 + 0 * 12] | ((jr.report_bufRight[24 + 0 * 12] << 8) & 0xff00)) + (Int16)(jr.report_bufRight[23 + 1 * 12] | ((jr.report_bufRight[24 + 1 * 12] << 8) & 0xff00)) + (Int16)(jr.report_bufRight[23 + 2 * 12] | ((jr.report_bufRight[24 + 2 * 12] << 8) & 0xff00)));
                JoyconRightGyroX = gyr_gRight.Z;
                JoyconRightGyroY = gyr_gRight.Y;
            }
            catch { }
        }
    }
}