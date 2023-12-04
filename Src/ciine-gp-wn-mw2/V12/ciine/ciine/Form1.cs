using System;
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
        private static bool controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_xbox;
        private static double controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition;
        private static double irx2, iry2, irx3, iry3, irx, iry, tempirx, tempiry, WiimoteIRSensors0X, WiimoteIRSensors0Y, WiimoteIRSensors1X, WiimoteIRSensors1Y, WiimoteRawValuesX, WiimoteRawValuesY, WiimoteRawValuesZ, calibrationinit, WiimoteIRSensors0Xcam, WiimoteIRSensors0Ycam, WiimoteIRSensors1Xcam, WiimoteIRSensors1Ycam, WiimoteIRSensorsXcam, WiimoteIRSensorsYcam;
        private static bool WiimoteIR0foundcam, WiimoteIR1foundcam, WiimoteIRswitch, WiimoteIR1found, WiimoteIR0found, WiimoteButtonStateA, WiimoteButtonStateB, WiimoteButtonStateMinus, WiimoteButtonStateHome, WiimoteButtonStatePlus, WiimoteButtonStateOne, WiimoteButtonStateTwo, WiimoteButtonStateUp, WiimoteButtonStateDown, WiimoteButtonStateLeft, WiimoteButtonStateRight, running, WiimoteNunchuckStateC, WiimoteNunchuckStateZ;
        private static double WiimoteIR0notfound, stickviewxinit, stickviewyinit, WiimoteNunchuckStateRawValuesX, WiimoteNunchuckStateRawValuesY, WiimoteNunchuckStateRawValuesZ, WiimoteNunchuckStateRawJoystickX, WiimoteNunchuckStateRawJoystickY;
        private static double mousex = 0f, mousey = 0f, viewpower1x = 0f, viewpower2x = 1f, viewpower3x = 0f, viewpower1y = 0.25f, viewpower2y = 0.75f, viewpower3y = 0f, dzx = 2.0f, dzy = 2.0f, centery = 80f;
        private static bool getstate;
        private static uint CurrentResolution = 0;
        public static Valuechange ValueChange = new Valuechange();
        private WiiMote wm = new WiiMote();
        private ScpBus scp = new ScpBus();
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
            Task.Run(() => taskD());
            Thread.Sleep(1000);
            calibrationinit = -wm.aBuffer[4] + 135f;
            stickviewxinit = -wm.aBuffer[16] + 125f;
            stickviewyinit = -wm.aBuffer[17] + 125f;
            scp.LoadController();
            Task.Run(() => taskX());
        }
        private void taskX()
        {
            for (; ; )
            {
                if (!running)
                    break;
                wm.Reconnection();
                WiimoteIR0found = (wm.aBuffer[6] | ((wm.aBuffer[8] >> 4) & 0x03) << 8) > 1 & (wm.aBuffer[6] | ((wm.aBuffer[8] >> 4) & 0x03) << 8) < 1023;
                WiimoteIR1found = (wm.aBuffer[9] | ((wm.aBuffer[8] >> 0) & 0x03) << 8) > 1 & (wm.aBuffer[9] | ((wm.aBuffer[8] >> 0) & 0x03) << 8) < 1023;
                if (WiimoteIR0notfound == 0 & WiimoteIR1found)
                    WiimoteIR0notfound = 1;
                if (WiimoteIR0notfound == 1 & !WiimoteIR0found & !WiimoteIR1found)
                    WiimoteIR0notfound = 2;
                if (WiimoteIR0notfound == 2 & WiimoteIR0found)
                {
                    WiimoteIR0notfound = 0;
                    if (!WiimoteIRswitch)
                        WiimoteIRswitch = true;
                    else
                        WiimoteIRswitch = false;
                }
                if (WiimoteIR0notfound == 0 & WiimoteIR0found)
                    WiimoteIR0notfound = 0;
                if (WiimoteIR0notfound == 0 & !WiimoteIR0found & !WiimoteIR1found)
                    WiimoteIR0notfound = 0;
                if (WiimoteIR0notfound == 1 & WiimoteIR0found)
                    WiimoteIR0notfound = 0;
                if (WiimoteIR0found)
                {
                    WiimoteIRSensors0X = wm.aBuffer[6] | ((wm.aBuffer[8] >> 4) & 0x03) << 8;
                    WiimoteIRSensors0Y = wm.aBuffer[7] | ((wm.aBuffer[8] >> 6) & 0x03) << 8;
                }
                if (WiimoteIR1found)
                {
                    WiimoteIRSensors1X = wm.aBuffer[9] | ((wm.aBuffer[8] >> 0) & 0x03) << 8;
                    WiimoteIRSensors1Y = wm.aBuffer[10] | ((wm.aBuffer[8] >> 2) & 0x03) << 8;
                }
                if (WiimoteIRswitch)
                {
                    WiimoteIR0foundcam = WiimoteIR0found;
                    WiimoteIR1foundcam = WiimoteIR1found;
                    WiimoteIRSensors0Xcam = WiimoteIRSensors0X - 512f;
                    WiimoteIRSensors0Ycam = WiimoteIRSensors0Y - 384f;
                    WiimoteIRSensors1Xcam = WiimoteIRSensors1X - 512f;
                    WiimoteIRSensors1Ycam = WiimoteIRSensors1Y - 384f;
                }
                else
                {
                    WiimoteIR1foundcam = WiimoteIR0found;
                    WiimoteIR0foundcam = WiimoteIR1found;
                    WiimoteIRSensors1Xcam = WiimoteIRSensors0X - 512f;
                    WiimoteIRSensors1Ycam = WiimoteIRSensors0Y - 384f;
                    WiimoteIRSensors0Xcam = WiimoteIRSensors1X - 512f;
                    WiimoteIRSensors0Ycam = WiimoteIRSensors1Y - 384f;
                }
                if (WiimoteIR0foundcam & WiimoteIR1foundcam)
                {
                    irx2 = WiimoteIRSensors0Xcam;
                    iry2 = WiimoteIRSensors0Ycam;
                    irx3 = WiimoteIRSensors1Xcam;
                    iry3 = WiimoteIRSensors1Ycam;
                    WiimoteIRSensorsXcam = WiimoteIRSensors0Xcam - WiimoteIRSensors1Xcam;
                    WiimoteIRSensorsYcam = WiimoteIRSensors0Ycam - WiimoteIRSensors1Ycam;
                }
                if (WiimoteIR0foundcam & !WiimoteIR1foundcam)
                {
                    irx2 = WiimoteIRSensors0Xcam;
                    iry2 = WiimoteIRSensors0Ycam;
                    irx3 = WiimoteIRSensors0Xcam - WiimoteIRSensorsXcam;
                    iry3 = WiimoteIRSensors0Ycam - WiimoteIRSensorsYcam;
                }
                if (WiimoteIR1foundcam & !WiimoteIR0foundcam)
                {
                    irx3 = WiimoteIRSensors1Xcam;
                    iry3 = WiimoteIRSensors1Ycam;
                    irx2 = WiimoteIRSensors1Xcam + WiimoteIRSensorsXcam;
                    iry2 = WiimoteIRSensors1Ycam + WiimoteIRSensorsYcam;
                }
                if (WiimoteIR0foundcam | WiimoteIR1foundcam)
                {
                    tempirx = irx;
                    tempiry = iry;
                    irx = (irx2 + irx3) * (1024f / 1346f);
                    iry = iry2 + iry3 + centery >= 0 ? Scale(iry2 + iry3 + centery, 0f, 782f + centery, 0f, 1024f) : Scale(iry2 + iry3 + centery, -782f + centery, 0f, -1024f, 0f);
                }
                else
                {
                    if (irx - tempirx >= 1f)
                        irx = 1024f;
                    if (irx - tempirx <= -1f)
                        irx = -1024f;
                    if (iry - tempiry >= 1f)
                        iry = 1024f;
                    if (iry - tempiry <= -1f)
                        iry = -1024f;
                }
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
                WiimoteNunchuckStateRawJoystickX = wm.aBuffer[16] - 125f + stickviewxinit;
                WiimoteNunchuckStateRawJoystickY = wm.aBuffer[17] - 125f + stickviewyinit;
                WiimoteNunchuckStateRawValuesX = wm.aBuffer[18] - 125f;
                WiimoteNunchuckStateRawValuesY = wm.aBuffer[19] - 125f;
                WiimoteNunchuckStateRawValuesZ = wm.aBuffer[20] - 125f;
                WiimoteNunchuckStateC = (wm.aBuffer[21] & 0x02) == 0;
                WiimoteNunchuckStateZ = (wm.aBuffer[21] & 0x01) == 0;
                controller1_send_rightstick = WiimoteNunchuckStateRawValuesY >= 90f;
                controller1_send_leftstick = WiimoteNunchuckStateZ;
                controller1_send_A = WiimoteNunchuckStateC;
                controller1_send_back = WiimoteButtonStateOne;
                controller1_send_start = WiimoteButtonStateTwo;
                controller1_send_X = WiimoteButtonStateHome | ((WiimoteRawValuesZ > 0 ? WiimoteRawValuesZ : -WiimoteRawValuesZ) >= 30f & (WiimoteRawValuesY > 0 ? WiimoteRawValuesY : -WiimoteRawValuesY) >= 30f & (WiimoteRawValuesX > 0 ? WiimoteRawValuesX : -WiimoteRawValuesX) >= 30f);
                controller1_send_leftbumper = WiimoteButtonStateMinus | WiimoteButtonStateUp;
                controller1_send_rightbumper = WiimoteButtonStatePlus | WiimoteButtonStateUp;
                controller1_send_B = WiimoteButtonStateDown;
                controller1_send_Y = WiimoteButtonStateRight;
                controller1_send_righttriggerposition = WiimoteButtonStateB ? 255 : 0;
                ValueChange[0] = WiimoteButtonStateA ? 1 : 0;
                if (Valuechange._ValueChange[0] > 0f & !getstate)
                {
                    getstate = true;
                }
                else
                {
                    if (Valuechange._ValueChange[0] > 0f & getstate)
                    {
                        getstate = false;
                    }
                }
                if (controller1_send_X | controller1_send_Y | controller1_send_rightbumper | controller1_send_leftbumper | controller1_send_rightstick | controller1_send_leftstick | controller1_send_back | controller1_send_start)
                {
                    getstate = false;
                }
                controller1_send_lefttriggerposition = getstate ? 255 : 0;
                if (irx >= 0f & irx <= 1024f)
                    mousex = Scale(irx * irx * irx / 1024f / 1024f * viewpower3x + irx * irx / 1024f * viewpower2x + irx * viewpower1x, 0f, 1024f, dzx / 100f * 1024f, 1024f);
                if (irx <= 0f & irx >= -1024f)
                    mousex = Scale(-(-irx * -irx * -irx) / 1024f / 1024f * viewpower3x - (-irx * -irx) / 1024f * viewpower2x - (-irx) * viewpower1x, -1024f, 0f, -1024f, -(dzx / 100f) * 1024f);
                if (iry >= 0f & iry <= 1024f)
                    mousey = Scale(iry * iry * iry / 1024f / 1024f * viewpower3y + iry * iry / 1024f * viewpower2y + iry * viewpower1y, 0f, 1024f, dzy / 100f * 1024f, 1024f);
                if (iry <= 0f & iry >= -1024f)
                    mousey = Scale(-(-iry * -iry * -iry) / 1024f / 1024f * viewpower3y - (-iry * -iry) / 1024f * viewpower2y - (-iry) * viewpower1y, -1024f, 0f, -1024f, -(dzy / 100f) * 1024f);
                controller1_send_rightstickx = (short)(-mousex / 1024f * 32767f);
                controller1_send_rightsticky = (short)(-mousey / 1024f * 32767f);
                if (!WiimoteButtonStateOne)
                {
                    if (!WiimoteButtonStateLeft)
                    {
                        if (WiimoteNunchuckStateRawJoystickX > 42f)
                            controller1_send_leftstickx = 32767;
                        if (WiimoteNunchuckStateRawJoystickX < -42f)
                            controller1_send_leftstickx = -32767;
                        if (WiimoteNunchuckStateRawJoystickX <= 42f & WiimoteNunchuckStateRawJoystickX >= -42f)
                            controller1_send_leftstickx = 0;
                        if (WiimoteNunchuckStateRawJoystickY > 42f)
                            controller1_send_leftsticky = 32767;
                        if (WiimoteNunchuckStateRawJoystickY < -42f)
                            controller1_send_leftsticky = -32767;
                        if (WiimoteNunchuckStateRawJoystickY <= 42f & WiimoteNunchuckStateRawJoystickY >= -42f)
                            controller1_send_leftsticky = 0;
                        controller1_send_right = false;
                        controller1_send_left = false;
                        controller1_send_up = false;
                        controller1_send_down = false;
                    }
                    else
                    {
                        controller1_send_leftstickx = 0;
                        controller1_send_leftsticky = 0;
                        controller1_send_right = WiimoteNunchuckStateRawJoystickX >= 42f;
                        controller1_send_left = WiimoteNunchuckStateRawJoystickX <= -42f;
                        controller1_send_up = WiimoteNunchuckStateRawJoystickY >= 42f;
                        controller1_send_down = WiimoteNunchuckStateRawJoystickY <= -42f;
                    }
                }
                scp.SetController(controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition, controller1_send_xbox);
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
    public class Valuechange
    {
        public static double[] _valuechange = { 0 };
        public static double[] _ValueChange = { 0 };
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