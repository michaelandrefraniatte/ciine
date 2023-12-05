using System;
using System.Windows.Forms;
using controllers;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
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
            spc.InitProController();
            scp.LoadController();
            Task.Run(() => taskX());
        }
        private void taskX()
        {
            while (running)
            {
                spc.ProcessButtonsAndSticksPro();
                if (spc.ProControllerButtonPLUS)
                    spc.InitProControllerAccel();
                mousex = spc.ProControllerAccelY * 31.25f;
                mousey = spc.ProControllerLeftStickY * 32767f;
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
                mousex = mousestatex + spc.ProControllerLeftStickX * 32767f;
                mousey = mousestatey;
                statex = Math.Abs(mousex) <= 32767f ? mousex : Math.Sign(mousex) * 32767f;
                statey = Math.Abs(mousey) <= 32767f ? mousey : Math.Sign(mousey) * 32767f;
                controller1_send_leftstickx = statex;
                controller1_send_leftsticky = statey;
                mousex = spc.ProControllerRightStickX * 1024f;
                mousey = spc.ProControllerRightStickY * 1024f;
                controller1_send_rightstickx = Math.Abs(-mousex * 32767f / 1024f) <= 32767f ? -mousex * 32767f / 1024f : Math.Sign(-mousex) * 32767f;
                controller1_send_rightsticky = Math.Abs(-mousey * 32767f / 1024f) <= 32767f ? -mousey * 32767f / 1024f : Math.Sign(-mousey) * 32767f;
                controller1_send_down = spc.ProControllerButtonDPAD_DOWN;
                controller1_send_left = spc.ProControllerButtonDPAD_LEFT;
                controller1_send_right = spc.ProControllerButtonDPAD_RIGHT;
                controller1_send_up = spc.ProControllerButtonDPAD_UP;
                controller1_send_leftstick = spc.ProControllerButtonSTICK_Left;
                controller1_send_rightstick = spc.ProControllerButtonSTICK_Right;
                controller1_send_B = spc.ProControllerButtonA;
                controller1_send_A = spc.ProControllerButtonB;
                controller1_send_Y = spc.ProControllerButtonX;
                controller1_send_X = spc.ProControllerButtonY;
                controller1_send_lefttriggerposition = spc.ProControllerButtonSHOULDER_Left_2 ? 255 : 0;
                controller1_send_righttriggerposition = spc.ProControllerButtonSHOULDER_Right_2 ? 255 : 0;
                controller1_send_leftbumper = spc.ProControllerButtonSHOULDER_Left_1;
                controller1_send_rightbumper = spc.ProControllerButtonSHOULDER_Right_1;
                controller1_send_back = spc.ProControllerButtonCAPTURE | spc.ProControllerButtonMINUS;
                controller1_send_start = spc.ProControllerButtonHOME;
                scp.SetController(controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition, controller1_send_xbox);
                Thread.Sleep(sleeptime);
            }
        }
        private double Scale(double value, double min, double max, double minScale, double maxScale)
        {
            double scaled = minScale + (double)(value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }
    }
}