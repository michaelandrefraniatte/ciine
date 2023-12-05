using System;
using System.Windows.Forms;
using controllers;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Vector3 = System.Numerics.Vector3;
using SwitchProControllerAPI;

namespace ciine_gp_spc
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
        private static bool running;
        private double statex = 0f, statey = 0f, mousex = 0f, mousey = 0f, mousestatex = 0f, mousestatey = 0f, viewpower1x = 1f, viewpower2x = 0f, viewpower3x = 0f, viewpower1y = 1f, viewpower2y = 0f, viewpower3y = 0f, dzx = 20.0f, dzy = 0f;
        private int sleeptime = 1;
        private ScpBus scp = new ScpBus();
        private SwitchProController spc = new SwitchProController();
        public static bool ProControllerButtonACC, ProControllerRollLeft, ProControllerRollRight;
        private static double ProControllerLeftStickX, ProControllerLeftStickY, ProControllerRightStickX, ProControllerRightStickY;
        public static System.Collections.Generic.List<double> ProValListX = new System.Collections.Generic.List<double>(), ProValListY = new System.Collections.Generic.List<double>();
        public static bool ProControllerAccelCenter, ProControllerStickCenter;
        public static double ProControllerAccelX, ProControllerAccelY, ProControllerGyroX, ProControllerGyroY;
        private static double[] stickleftPro = { 0, 0 };
        private static double[] stickCenterleftPro = { 0, 0 };
        private static byte[] stick_rawleftPro = { 0, 0, 0 };
        private static double[] stickrightPro = { 0, 0 };
        private static double[] stickCenterrightPro = { 0, 0 };
        private static byte[] stick_rawrightPro = { 0, 0, 0 };
        public static Vector3 acc_gPro = new Vector3();
        public static Vector3 gyr_gPro = new Vector3();
        public static Vector3 InitDirectAnglesPro, DirectAnglesPro;
        public static bool ProControllerButtonSHOULDER_Left_1, ProControllerButtonSHOULDER_Left_2, ProControllerButtonSHOULDER_Right_1, ProControllerButtonSHOULDER_Right_2, ProControllerButtonDPAD_DOWN, ProControllerButtonDPAD_RIGHT, ProControllerButtonDPAD_UP, ProControllerButtonDPAD_LEFT, ProControllerButtonA, ProControllerButtonB, ProControllerButtonX, ProControllerButtonY, ProControllerButtonMINUS, ProControllerButtonPLUS, ProControllerButtonSTICK_Left, ProControllerButtonSTICK_Right, ProControllerButtonCAPTURE, ProControllerButtonHOME;
        public static bool ISPRO = true;
        public static float acc_gcalibrationProX, acc_gcalibrationProY, acc_gcalibrationProZ;
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
                spc.Close();
            }
            catch { }
        }
        private void Start()
        {
            running = true;
            spc.ScanPro();
            spc.BeginAsyncPolling();
            Thread.Sleep(1000);
            InitProController();
            scp.LoadController();
            Task.Run(() => taskX());
        }
        private void taskX()
        {
            while (running)
            {
                ProcessButtonsAndSticksPro();
                ProControllerAccelCenter = ProControllerButtonPLUS;
                mousex = ProControllerAccelY * 31.25f;
                mousey = ProControllerLeftStickY * 32767f;
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
                mousex = mousestatex + ProControllerLeftStickX * 32767f;
                mousey = mousestatey;
                statex = Math.Abs(mousex) <= 32767f ? mousex : Math.Sign(mousex) * 32767f;
                statey = Math.Abs(mousey) <= 32767f ? mousey : Math.Sign(mousey) * 32767f;
                controller1_send_leftstickx = statex;
                controller1_send_leftsticky = statey;
                mousex = ProControllerRightStickX * 1024f;
                mousey = ProControllerRightStickY * 1024f;
                controller1_send_rightstickx = Math.Abs(-mousex * 32767f / 1024f) <= 32767f ? -mousex * 32767f / 1024f : Math.Sign(-mousex) * 32767f;
                controller1_send_rightsticky = Math.Abs(-mousey * 32767f / 1024f) <= 32767f ? -mousey * 32767f / 1024f : Math.Sign(-mousey) * 32767f;
                controller1_send_down = ProControllerButtonDPAD_DOWN;
                controller1_send_left = ProControllerButtonDPAD_LEFT;
                controller1_send_right = ProControllerButtonDPAD_RIGHT;
                controller1_send_up = ProControllerButtonDPAD_UP;
                controller1_send_leftstick = ProControllerButtonSTICK_Left;
                controller1_send_rightstick = ProControllerButtonSTICK_Right;
                controller1_send_B = ProControllerButtonA;
                controller1_send_A = ProControllerButtonB;
                controller1_send_Y = ProControllerButtonX;
                controller1_send_X = ProControllerButtonY;
                controller1_send_lefttriggerposition = ProControllerButtonSHOULDER_Left_2 ? 255 : 0;
                controller1_send_righttriggerposition = ProControllerButtonSHOULDER_Right_2 ? 255 : 0;
                controller1_send_leftbumper = ProControllerButtonSHOULDER_Left_1;
                controller1_send_rightbumper = ProControllerButtonSHOULDER_Right_1;
                controller1_send_back = ProControllerButtonCAPTURE | ProControllerButtonMINUS;
                controller1_send_start = ProControllerButtonHOME;
                scp.SetController(controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition, controller1_send_xbox);
                Thread.Sleep(sleeptime);
            }
        }
        private double Scale(double value, double min, double max, double minScale, double maxScale)
        {
            double scaled = minScale + (double)(value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }
        public void InitProController()
        {
            try
            {
                stick_rawleftPro[0] = spc.report_bufPro[6 + (ISPRO ? 0 : 3)];
                stick_rawleftPro[1] = spc.report_bufPro[7 + (ISPRO ? 0 : 3)];
                stick_rawleftPro[2] = spc.report_bufPro[8 + (ISPRO ? 0 : 3)];
                stickCenterleftPro[0] = (UInt16)(stick_rawleftPro[0] | ((stick_rawleftPro[1] & 0xf) << 8));
                stickCenterleftPro[1] = (UInt16)((stick_rawleftPro[1] >> 4) | (stick_rawleftPro[2] << 4));
                stick_rawrightPro[0] = spc.report_bufPro[6 + (!ISPRO ? 0 : 3)];
                stick_rawrightPro[1] = spc.report_bufPro[7 + (!ISPRO ? 0 : 3)];
                stick_rawrightPro[2] = spc.report_bufPro[8 + (!ISPRO ? 0 : 3)];
                stickCenterrightPro[0] = (UInt16)(stick_rawrightPro[0] | ((stick_rawrightPro[1] & 0xf) << 8));
                stickCenterrightPro[1] = (UInt16)((stick_rawrightPro[1] >> 4) | (stick_rawrightPro[2] << 4));
                acc_gcalibrationProX = (Int16)(spc.report_bufPro[13 + 0 * 12] | ((spc.report_bufPro[14 + 0 * 12] << 8) & 0xff00)) + (Int16)(spc.report_bufPro[13 + 1 * 12] | ((spc.report_bufPro[14 + 1 * 12] << 8) & 0xff00)) + (Int16)(spc.report_bufPro[13 + 2 * 12] | ((spc.report_bufPro[14 + 2 * 12] << 8) & 0xff00));
                acc_gcalibrationProY = (Int16)(spc.report_bufPro[15 + 0 * 12] | ((spc.report_bufPro[16 + 0 * 12] << 8) & 0xff00)) + (Int16)(spc.report_bufPro[15 + 1 * 12] | ((spc.report_bufPro[16 + 1 * 12] << 8) & 0xff00)) + (Int16)(spc.report_bufPro[15 + 2 * 12] | ((spc.report_bufPro[16 + 2 * 12] << 8) & 0xff00));
                acc_gcalibrationProZ = (Int16)(spc.report_bufPro[17 + 0 * 12] | ((spc.report_bufPro[18 + 0 * 12] << 8) & 0xff00)) + (Int16)(spc.report_bufPro[17 + 1 * 12] | ((spc.report_bufPro[18 + 1 * 12] << 8) & 0xff00)) + (Int16)(spc.report_bufPro[17 + 2 * 12] | ((spc.report_bufPro[18 + 2 * 12] << 8) & 0xff00));
            }
            catch { }
        }
        public void ProcessButtonsAndSticksPro()
        {
            try
            {
                if (ProControllerStickCenter)
                {
                    stick_rawleftPro[0] = spc.report_bufPro[6 + (ISPRO ? 0 : 3)];
                    stick_rawleftPro[1] = spc.report_bufPro[7 + (ISPRO ? 0 : 3)];
                    stick_rawleftPro[2] = spc.report_bufPro[8 + (ISPRO ? 0 : 3)];
                    stickCenterleftPro[0] = (UInt16)(stick_rawleftPro[0] | ((stick_rawleftPro[1] & 0xf) << 8));
                    stickCenterleftPro[1] = (UInt16)((stick_rawleftPro[1] >> 4) | (stick_rawleftPro[2] << 4));
                    stick_rawrightPro[0] = spc.report_bufPro[6 + (!ISPRO ? 0 : 3)];
                    stick_rawrightPro[1] = spc.report_bufPro[7 + (!ISPRO ? 0 : 3)];
                    stick_rawrightPro[2] = spc.report_bufPro[8 + (!ISPRO ? 0 : 3)];
                    stickCenterrightPro[0] = (UInt16)(stick_rawrightPro[0] | ((stick_rawrightPro[1] & 0xf) << 8));
                    stickCenterrightPro[1] = (UInt16)((stick_rawrightPro[1] >> 4) | (stick_rawrightPro[2] << 4));
                }
                stick_rawleftPro[0] = spc.report_bufPro[6 + (ISPRO ? 0 : 3)];
                stick_rawleftPro[1] = spc.report_bufPro[7 + (ISPRO ? 0 : 3)];
                stick_rawleftPro[2] = spc.report_bufPro[8 + (ISPRO ? 0 : 3)];
                stickleftPro[0] = ((UInt16)(stick_rawleftPro[0] | ((stick_rawleftPro[1] & 0xf) << 8)) - stickCenterleftPro[0]) / 1440f;
                stickleftPro[1] = ((UInt16)((stick_rawleftPro[1] >> 4) | (stick_rawleftPro[2] << 4)) - stickCenterleftPro[1]) / 1440f;
                ProControllerLeftStickX = stickleftPro[0];
                ProControllerLeftStickY = stickleftPro[1];
                stick_rawrightPro[0] = spc.report_bufPro[6 + (!ISPRO ? 0 : 3)];
                stick_rawrightPro[1] = spc.report_bufPro[7 + (!ISPRO ? 0 : 3)];
                stick_rawrightPro[2] = spc.report_bufPro[8 + (!ISPRO ? 0 : 3)];
                stickrightPro[0] = ((UInt16)(stick_rawrightPro[0] | ((stick_rawrightPro[1] & 0xf) << 8)) - stickCenterrightPro[0]) / 1440f;
                stickrightPro[1] = ((UInt16)((stick_rawrightPro[1] >> 4) | (stick_rawrightPro[2] << 4)) - stickCenterrightPro[1]) / 1440f;
                ProControllerRightStickX = -stickrightPro[0];
                ProControllerRightStickY = -stickrightPro[1];
                acc_gPro.X = ((Int16)(spc.report_bufPro[13 + 0 * 12] | ((spc.report_bufPro[14 + 0 * 12] << 8) & 0xff00)) + (Int16)(spc.report_bufPro[13 + 1 * 12] | ((spc.report_bufPro[14 + 1 * 12] << 8) & 0xff00)) + (Int16)(spc.report_bufPro[13 + 2 * 12] | ((spc.report_bufPro[14 + 2 * 12] << 8) & 0xff00)) - acc_gcalibrationProX) * (1.0f / 12000f);
                acc_gPro.Y = -((Int16)(spc.report_bufPro[15 + 0 * 12] | ((spc.report_bufPro[16 + 0 * 12] << 8) & 0xff00)) + (Int16)(spc.report_bufPro[15 + 1 * 12] | ((spc.report_bufPro[16 + 1 * 12] << 8) & 0xff00)) + (Int16)(spc.report_bufPro[15 + 2 * 12] | ((spc.report_bufPro[16 + 2 * 12] << 8) & 0xff00)) - acc_gcalibrationProY) * (1.0f / 12000f);
                acc_gPro.Z = -((Int16)(spc.report_bufPro[17 + 0 * 12] | ((spc.report_bufPro[18 + 0 * 12] << 8) & 0xff00)) + (Int16)(spc.report_bufPro[17 + 1 * 12] | ((spc.report_bufPro[18 + 1 * 12] << 8) & 0xff00)) + (Int16)(spc.report_bufPro[17 + 2 * 12] | ((spc.report_bufPro[18 + 2 * 12] << 8) & 0xff00)) - acc_gcalibrationProZ) * (1.0f / 12000f);
                ProControllerButtonSHOULDER_Left_1 = (spc.report_bufPro[3 + (ISPRO ? 2 : 0)] & 0x40) != 0;
                ProControllerButtonSHOULDER_Left_2 = (spc.report_bufPro[3 + (ISPRO ? 2 : 0)] & 0x80) != 0;
                ProControllerButtonDPAD_DOWN = (spc.report_bufPro[3 + (ISPRO ? 2 : 0)] & (ISPRO ? 0x01 : 0x04)) != 0;
                ProControllerButtonDPAD_RIGHT = (spc.report_bufPro[3 + (ISPRO ? 2 : 0)] & (ISPRO ? 0x04 : 0x08)) != 0;
                ProControllerButtonDPAD_UP = (spc.report_bufPro[3 + (ISPRO ? 2 : 0)] & (ISPRO ? 0x02 : 0x02)) != 0;
                ProControllerButtonDPAD_LEFT = (spc.report_bufPro[3 + (ISPRO ? 2 : 0)] & (ISPRO ? 0x08 : 0x01)) != 0;
                ProControllerButtonMINUS = (spc.report_bufPro[4] & 0x01) != 0;
                ProControllerButtonCAPTURE = (spc.report_bufPro[4] & 0x20) != 0;
                ProControllerButtonSTICK_Left = (spc.report_bufPro[4] & (ISPRO ? 0x08 : 0x04)) != 0;
                ProControllerButtonACC = acc_gPro.X <= -1.13;
                ProControllerButtonSHOULDER_Right_1 = (spc.report_bufPro[3 + (!ISPRO ? 2 : 0)] & 0x40) != 0;
                ProControllerButtonSHOULDER_Right_2 = (spc.report_bufPro[3 + (!ISPRO ? 2 : 0)] & 0x80) != 0;
                ProControllerButtonA = (spc.report_bufPro[3 + (!ISPRO ? 2 : 0)] & (!ISPRO ? 0x04 : 0x08)) != 0;
                ProControllerButtonB = (spc.report_bufPro[3 + (!ISPRO ? 2 : 0)] & (!ISPRO ? 0x01 : 0x04)) != 0;
                ProControllerButtonX = (spc.report_bufPro[3 + (!ISPRO ? 2 : 0)] & (!ISPRO ? 0x02 : 0x02)) != 0;
                ProControllerButtonY = (spc.report_bufPro[3 + (!ISPRO ? 2 : 0)] & (!ISPRO ? 0x08 : 0x01)) != 0;
                ProControllerButtonPLUS = (spc.report_bufPro[4] & 0x02) != 0;
                ProControllerButtonHOME = (spc.report_bufPro[4] & 0x10) != 0;
                ProControllerButtonSTICK_Right = ((spc.report_bufPro[4] & (!ISPRO ? 0x08 : 0x04)) != 0);
                if (ProValListY.Count >= 50)
                {
                    ProValListY.RemoveAt(0);
                    ProValListY.Add(acc_gPro.Y);
                }
                else
                    ProValListY.Add(acc_gPro.Y);
                if (ProControllerAccelCenter)
                    InitDirectAnglesPro = acc_gPro;
                DirectAnglesPro = acc_gPro - InitDirectAnglesPro;
                ProControllerAccelX = DirectAnglesPro.X * 1350f;
                ProControllerAccelY = -DirectAnglesPro.Y * 1350f;
                gyr_gPro.X = ((Int16)(spc.report_bufPro[19 + 0 * 12] | ((spc.report_bufPro[20 + 0 * 12] << 8) & 0xff00)) + (Int16)(spc.report_bufPro[19 + 1 * 12] | ((spc.report_bufPro[20 + 1 * 12] << 8) & 0xff00)) + (Int16)(spc.report_bufPro[19 + 2 * 12] | ((spc.report_bufPro[20 + 2 * 12] << 8) & 0xff00)));
                gyr_gPro.Y = ((Int16)(spc.report_bufPro[21 + 0 * 12] | ((spc.report_bufPro[22 + 0 * 12] << 8) & 0xff00)) + (Int16)(spc.report_bufPro[21 + 1 * 12] | ((spc.report_bufPro[22 + 1 * 12] << 8) & 0xff00)) + (Int16)(spc.report_bufPro[21 + 2 * 12] | ((spc.report_bufPro[22 + 2 * 12] << 8) & 0xff00)));
                gyr_gPro.Z = ((Int16)(spc.report_bufPro[23 + 0 * 12] | ((spc.report_bufPro[24 + 0 * 12] << 8) & 0xff00)) + (Int16)(spc.report_bufPro[23 + 1 * 12] | ((spc.report_bufPro[24 + 1 * 12] << 8) & 0xff00)) + (Int16)(spc.report_bufPro[23 + 2 * 12] | ((spc.report_bufPro[24 + 2 * 12] << 8) & 0xff00)));
                ProControllerGyroX = gyr_gPro.Z;
                ProControllerGyroY = gyr_gPro.Y;
            }
            catch { }
        }
    }
}