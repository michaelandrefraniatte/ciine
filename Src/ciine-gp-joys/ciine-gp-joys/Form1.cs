using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using controllers;
using System.Threading;
using System.Threading.Tasks;
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
        private static uint CurrentResolution = 0;
        private static bool controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_xbox;
        private static double controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition;
        private static bool running;
        private double statex = 0f, statey = 0f, mousex = 0f, mousey = 0f, mousestatex = 0f, mousestatey = 0f, viewpower1x = 1f, viewpower2x = 0f, viewpower3x = 0f, viewpower1y = 1f, viewpower2y = 0f, viewpower3y = 0f, dzx = 20.0f, dzy = 0f;
        private int sleeptime = 1;
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
            jl.BeginAsyncPolling();
            jr.BeginAsyncPolling();
            Thread.Sleep(1000);
            jl.InitLeftJoycon();
            jr.InitRightJoycon();
            scp.LoadController();
            Task.Run(() => taskX());
        }
        private void taskX()
        {
            while (running)
            {
                jl.ProcessButtonsLeftJoycon();
                jr.ProcessButtonsRightJoycon();
                if (jr.JoyconRightButtonPLUS)
                {
                    jl.InitLeftJoyconAccel();
                    jr.InitRightJoyconAccel();
                }
                mousex = (jl.JoyconLeftAccelY - jr.JoyconRightAccelY) * 13.5f;
                mousey = jl.JoyconLeftStickY * 32767f * 1.2f;
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
                mousex = mousestatex + jl.JoyconLeftStickX * 32767f * 1.2f;
                mousey = mousestatey;
                statex = Math.Abs(mousex) <= 32767f ? mousex : Math.Sign(mousex) * 32767f;
                statey = Math.Abs(mousey) <= 32767f ? mousey : Math.Sign(mousey) * 32767f;
                controller1_send_leftstickx = statex;
                controller1_send_leftsticky = statey;
                mousex = jr.JoyconRightStickX * 1400f;
                mousey = jr.JoyconRightStickY * 1400f;
                controller1_send_rightstickx = Math.Abs(-mousex * 32767f / 1024f) <= 32767f ? -mousex * 32767f / 1024f : Math.Sign(-mousex) * 32767f;
                controller1_send_rightsticky = Math.Abs(-mousey * 32767f / 1024f) <= 32767f ? -mousey * 32767f / 1024f : Math.Sign(-mousey) * 32767f;
                controller1_send_up = jl.JoyconLeftButtonDPAD_UP;
                controller1_send_left = jl.JoyconLeftButtonDPAD_LEFT;
                controller1_send_down = jl.JoyconLeftButtonDPAD_DOWN;
                controller1_send_right = jl.JoyconLeftButtonDPAD_RIGHT;
                controller1_send_back = jl.JoyconLeftButtonMINUS | jr.JoyconRightButtonHOME;
                controller1_send_start = jl.JoyconLeftButtonCAPTURE | jr.JoyconRightButtonPLUS;
                controller1_send_leftstick = jl.JoyconLeftButtonSTICK;
                controller1_send_leftbumper = jl.JoyconLeftButtonSL | jl.JoyconLeftButtonSHOULDER_1 | jr.JoyconRightButtonSL;
                controller1_send_rightbumper = jl.JoyconLeftButtonSR | jr.JoyconRightButtonSHOULDER_1 | jr.JoyconRightButtonSR;
                controller1_send_A = jr.JoyconRightButtonDPAD_DOWN;
                controller1_send_B = jr.JoyconRightButtonDPAD_RIGHT;
                controller1_send_X = jr.JoyconRightButtonDPAD_LEFT;
                controller1_send_Y = jr.JoyconRightButtonDPAD_UP;
                controller1_send_rightstick = jr.JoyconRightButtonSTICK;
                controller1_send_lefttriggerposition = jl.JoyconLeftButtonSHOULDER_2 ? 255 : 0;
                controller1_send_righttriggerposition = jr.JoyconRightButtonSHOULDER_2 ? 255 : 0;
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