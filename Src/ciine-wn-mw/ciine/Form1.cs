﻿using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using controllers;
using WiiMoteAPI;
using ValueStateChanged;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.ServiceProcess;
using Microsoft.Win32.SafeHandles;
using System.Globalization;
using ciine;
using static System.Windows.Forms.DataFormats;

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
        private bool controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_xbox;
        private double controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition;
        private double mousex = 0f, mousey = 0f, viewpower1x = 0f, viewpower2x = 1f, viewpower3x = 0f, viewpower1y = 0.25f, viewpower2y = 0.75f, viewpower3y = 0f, dzx = 2.0f, dzy = 2.0f, countup = 0, countupup = 0, countxy = 0, county = 0;
        private WiiMote wm = new WiiMote();
        private XBoxController scp = new XBoxController();
        private bool running, getstate, closed;
        private List<string> servBLs = new List<string>();
        private string procnamesbl = "", servNames = "";
        private ServiceController[] services;
        private TimeSpan timeout = new TimeSpan(0, 0, 1);
        private valuechanged ValueChanged = new valuechanged();
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            OnKeyDown(e.KeyData);
        }
        private void OnKeyDown(Keys keyData)
        {
            if (keyData == Keys.F1)
            {
                const string message = "• Author: Michaël André Franiatte.\n\r\n\r• Contact: michael.franiatte@gmail.com.\n\r\n\r• Publisher: https://github.com/michaelandrefraniatte.\n\r\n\r• Copyrights: All rights reserved, no permissions granted.\n\r\n\r• License: Not open source, not free of charge to use.";
                const string caption = "About";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (keyData == Keys.F2)
            {
                const string message = "• B: right trigger.\n\r\n\r• A: left trigger toggle.\n\r\n\r• Accelerometers: right stick button or x.\n\r\n\r• Z: left stick button.\n\r\n\r• C: a.\n\r\n\r• One: back.\n\r\n\r• Two: start.\n\r\n\r• Right: y or x for long press.\n\r\n\r• Minus or Up: left bumper.\n\r\n\r• Plus or Up: right bumper.\n\r\n\r• Down: b.\n\r\n\r• IR: right stick.\n\r\n\r• Home: up.\n\r\n\r• Stick: left stick.\n\r\n\r• Left and Stick: up, down, left, and right.\n\r\n\r• Page Down: borderless.\n\r\n\r• Page Up: borderfull.\n\r\n\r• Decimal: start capture.\n\r\n\r• Numpad 0: stop capture.";
                const string caption = "Help";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (keyData == Keys.Escape)
            {
                this.Close();
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
            SetProcessPriority();
            Task.Run(() => StartCiine());
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                closed = true;
                running = false;
                Thread.Sleep(100);
                scp.Disconnect();
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
        private void StartCiine()
        {
            running = true;
            wm.ScanWiimote();
            wm.BeginPolling();
            Thread.Sleep(1000);
            wm.Init();
            scp.Connect();
            Task.Run(() => taskX());
        }
        private void taskX()
        {
            for (; ; )
            {
                if (!running)
                    break;
                controller1_send_righttriggerposition = wm.WiimoteButtonStateB ? 255 : 0;
                ValueChanged[15] = wm.WiimoteButtonStateA;
                if (valuechanged._ValueChanged[15] & valuechanged._valuechanged[15] & !getstate)
                {
                    getstate = true;
                }
                else if (valuechanged._ValueChanged[15] & valuechanged._valuechanged[15] & getstate)
                {
                    getstate = false;
                }
                controller1_send_lefttriggerposition = getstate ? 255 : 0;
                controller1_send_rightstick = wm.WiimoteNunchuckStateRawValuesY >= 90f;
                controller1_send_leftstick = wm.WiimoteNunchuckStateZ;
                controller1_send_A = wm.WiimoteNunchuckStateC;
                controller1_send_back = wm.WiimoteButtonStateOne;
                controller1_send_start = wm.WiimoteButtonStateTwo;
                ValueChanged[16] = wm.WiimoteButtonStateRight;
                if ((countxy > 0 & countxy < 300 & valuechanged._ValueChanged[16] & !valuechanged._valuechanged[16]) | county > 0)
                    county++;
                if (county > 100)
                    county = 0;
                countxy = wm.WiimoteButtonStateRight ? countxy + 1 : 0;
                controller1_send_Y = county > 0;
                controller1_send_X = countxy > 300 | ((wm.WiimoteRawValuesZ > 0 ? wm.WiimoteRawValuesZ : -wm.WiimoteRawValuesZ) >= 30f & (wm.WiimoteRawValuesY > 0 ? wm.WiimoteRawValuesY : -wm.WiimoteRawValuesY) >= 30f & (wm.WiimoteRawValuesX > 0 ? wm.WiimoteRawValuesX : -wm.WiimoteRawValuesX) >= 30f);
                controller1_send_leftbumper = wm.WiimoteButtonStateMinus | wm.WiimoteButtonStateUp;
                controller1_send_rightbumper = wm.WiimoteButtonStatePlus | wm.WiimoteButtonStateUp;
                controller1_send_B = wm.WiimoteButtonStateDown;
                if (wm.irx >= 0f & wm.irx <= 1024f)
                    mousex = Scale(wm.irx * wm.irx * wm.irx / 1024f / 1024f * viewpower3x + wm.irx * wm.irx / 1024f * viewpower2x + wm.irx * viewpower1x, 0f, 1024f, dzx / 100f * 1024f, 1024f);
                if (wm.irx <= 0f & wm.irx >= -1024f)
                    mousex = Scale(-(-wm.irx * -wm.irx * -wm.irx) / 1024f / 1024f * viewpower3x - (-wm.irx * -wm.irx) / 1024f * viewpower2x - (-wm.irx) * viewpower1x, -1024f, 0f, -1024f, -(dzx / 100f) * 1024f);
                if (wm.iry >= 0f & wm.iry <= 1024f)
                    mousey = Scale(wm.iry * wm.iry * wm.iry / 1024f / 1024f * viewpower3y + wm.iry * wm.iry / 1024f * viewpower2y + wm.iry * viewpower1y, 0f, 1024f, dzy / 100f * 1024f, 1024f);
                if (wm.iry <= 0f & wm.iry >= -1024f)
                    mousey = Scale(-(-wm.iry * -wm.iry * -wm.iry) / 1024f / 1024f * viewpower3y - (-wm.iry * -wm.iry) / 1024f * viewpower2y - (-wm.iry) * viewpower1y, -1024f, 0f, -1024f, -(dzy / 100f) * 1024f);
                controller1_send_rightstickx = (short)(-mousex / 1024f * 32767f);
                controller1_send_rightsticky = (short)(-mousey / 1024f * 32767f);
                countup = wm.WiimoteButtonStateHome ? countup + 1 : 0;
                if ((countup > 0 & countup < 300) | countupup > 0)
                    countupup++;
                if (countupup > 300)
                    countupup = 0;
                if (!wm.WiimoteButtonStateOne)
                {
                    if (!wm.WiimoteButtonStateLeft)
                    {
                        if (wm.WiimoteNunchuckStateRawJoystickX > 42f)
                            controller1_send_leftstickx = 32767;
                        if (wm.WiimoteNunchuckStateRawJoystickX < -42f)
                            controller1_send_leftstickx = -32767;
                        if (wm.WiimoteNunchuckStateRawJoystickX <= 42f & wm.WiimoteNunchuckStateRawJoystickX >= -42f)
                            controller1_send_leftstickx = 0;
                        if (wm.WiimoteNunchuckStateRawJoystickY > 42f)
                            controller1_send_leftsticky = 32767;
                        if (wm.WiimoteNunchuckStateRawJoystickY < -42f)
                            controller1_send_leftsticky = -32767;
                        if (wm.WiimoteNunchuckStateRawJoystickY <= 42f & wm.WiimoteNunchuckStateRawJoystickY >= -42f)
                            controller1_send_leftsticky = 0;
                        controller1_send_right = false;
                        controller1_send_left = false;
                        controller1_send_up = (countupup > 0 & countupup < 100) | (countupup > 200 & countupup < 300) | countup > 300;
                        controller1_send_down = false;
                    }
                    else
                    {
                        controller1_send_leftstickx = 0;
                        controller1_send_leftsticky = 0;
                        controller1_send_right = wm.WiimoteNunchuckStateRawJoystickX >= 42f;
                        controller1_send_left = wm.WiimoteNunchuckStateRawJoystickX <= -42f;
                        controller1_send_up = wm.WiimoteNunchuckStateRawJoystickY >= 42f;
                        controller1_send_down = wm.WiimoteNunchuckStateRawJoystickY <= -42f;
                    }
                }
                if (controller1_send_X | controller1_send_Y | controller1_send_rightbumper | controller1_send_leftbumper | controller1_send_rightstick | controller1_send_leftstick | controller1_send_back | controller1_send_start)
                {
                    getstate = false;
                }
                scp.Set(controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition, controller1_send_xbox);
                Thread.Sleep(1);
            }
        }
        private double Scale(double value, double min, double max, double minScale, double maxScale)
        {
            double scaled = minScale + (double)(value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }
    }
}
namespace WiiMoteAPI
{
    public class WiiMote
    {
        [DllImport("MotionInputPairing.dll", EntryPoint = "wiimoteconnect")]
        private static extern bool wiimoteconnect();
        [DllImport("MotionInputPairing.dll", EntryPoint = "wiimotedisconnect")]
        private static extern bool wiimotedisconnect();
        [DllImport("Kernel32.dll")]
        public static extern bool ReadFile(SafeFileHandle hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped); 
        [DllImport("hid.dll")]
        private static extern void HidD_GetHidGuid(out Guid gHid);
        [DllImport("hid.dll")]
        private extern static bool HidD_SetOutputReport(IntPtr HidDeviceObject, byte[] lpReportBuffer, uint ReportBufferLength);
        [DllImport("setupapi.dll")]
        private static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, string Enumerator, IntPtr hwndParent, UInt32 Flags);
        [DllImport("setupapi.dll")]
        private static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInvo, ref Guid interfaceClassGuid, Int32 memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);
        [DllImport("setupapi.dll")]
        private static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, UInt32 deviceInterfaceDetailDataSize, out UInt32 requiredSize, IntPtr deviceInfoData);
        [DllImport("setupapi.dll")]
        private static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, UInt32 deviceInterfaceDetailDataSize, out UInt32 requiredSize, IntPtr deviceInfoData);
        [DllImport("Kernel32.dll")]
        private static extern SafeFileHandle CreateFile(string fileName, [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess, [MarshalAs(UnmanagedType.U4)] FileShare fileShare, IntPtr securityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition, [MarshalAs(UnmanagedType.U4)] uint flags, IntPtr template);
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern uint TimeBeginPeriod(uint ms);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        private static extern uint TimeEndPeriod(uint ms);
        [DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
        private static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
        private static uint CurrentResolution = 0;
        private const double REGISTER_IR = 0x04b00030, REGISTER_EXTENSION_INIT_1 = 0x04a400f0, REGISTER_EXTENSION_INIT_2 = 0x04a400fb, REGISTER_EXTENSION_TYPE = 0x04a400fa, REGISTER_EXTENSION_CALIBRATION = 0x04a40020, REGISTER_MOTIONPLUS_INIT = 0x04a600fe;
        private string path;
        private byte[] mBuff = new byte[22];
        private const byte Type = 0x12, IR = 0x13, WriteMemory = 0x16, ReadMemory = 0x16, IRExtensionAccel = 0x37;
        private uint report_len = 22;
        private byte[] report_buf = new byte[22];
        private static SafeFileHandle handle = null, handleunshared = null;
        private static uint BytesRead = 0;
        private bool reconnectingwiimotebool;
        private double reconnectingwiimotecount;
        private bool isvalidhandle = false;
        private bool running;
        private List<double> vallistirx = new List<double>(), vallistiry = new List<double>();
        public double ir1x, ir1y, ir2x, ir2y;
        private double irxc, iryc, irx2, iry2, irx3, iry3, WiimoteIRSensors0X, WiimoteIRSensors0Y, WiimoteIRSensors1X, WiimoteIRSensors1Y, calibrationinit, WiimoteIRSensors0Xcam, WiimoteIRSensors0Ycam, WiimoteIRSensors1Xcam, WiimoteIRSensors1Ycam, WiimoteIRSensorsXcam, WiimoteIRSensorsYcam;
        public double irx, iry, WiimoteRawValuesX, WiimoteRawValuesY, WiimoteRawValuesZ;
        public bool WiimoteButtonStateA, WiimoteButtonStateB, WiimoteButtonStateMinus, WiimoteButtonStateHome, WiimoteButtonStatePlus, WiimoteButtonStateOne, WiimoteButtonStateTwo, WiimoteButtonStateUp, WiimoteButtonStateDown, WiimoteButtonStateLeft, WiimoteButtonStateRight, WiimoteNunchuckStateC, WiimoteNunchuckStateZ;
        private bool WiimoteIR0foundcam, WiimoteIR1foundcam, WiimoteIRswitch, WiimoteIR1found, WiimoteIR0found;
        public double WiimoteNunchuckStateRawValuesX, WiimoteNunchuckStateRawValuesY, WiimoteNunchuckStateRawValuesZ, WiimoteNunchuckStateRawJoystickX, WiimoteNunchuckStateRawJoystickY;
        private double WiimoteIR0notfound, stickviewxinit, stickviewyinit, centery = 80f;
        public WiiMote()
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
            running = true;
            while (vallistirx.Count <= 20)
            {
                vallistirx.Add(0);
            }
            while (vallistiry.Count <= 20)
            {
                vallistiry.Add(0);
            }
        }
        public void Close()
        {
            running = false;
            Thread.Sleep(100);
            wiimotedisconnect();
        }
        public void BeginPolling()
        {
            Task.Run(() => taskD());
            Task.Run(() => taskP());
        }
        private void taskD()
        {
            for (; ; )
            {
                if (!running)
                    break;
                try
                {
                    ReadFile(handle, report_buf, report_len, out BytesRead, IntPtr.Zero);
                    reconnectingwiimotebool = false;
                }
                catch { Thread.Sleep(10); }
            }
        }
        private void taskP()
        {
            for (; ; )
            {
                if (!running)
                    break;
                Reconnection();
                ProcessStateLogic();
                Thread.Sleep(1);
            }
        }
        public void Init()
        {
            calibrationinit = -report_buf[4] + 135f;
            stickviewxinit = -report_buf[16] + 125f;
            stickviewyinit = -report_buf[17] + 125f;
        }
        private void ProcessStateLogic()
        {
            WiimoteIR0found = (report_buf[6] | ((report_buf[8] >> 4) & 0x03) << 8) > 1 & (report_buf[6] | ((report_buf[8] >> 4) & 0x03) << 8) < 1023;
            WiimoteIR1found = (report_buf[9] | ((report_buf[8] >> 0) & 0x03) << 8) > 1 & (report_buf[9] | ((report_buf[8] >> 0) & 0x03) << 8) < 1023;
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
                WiimoteIRSensors0X = report_buf[6] | ((report_buf[8] >> 4) & 0x03) << 8;
                WiimoteIRSensors0Y = report_buf[7] | ((report_buf[8] >> 6) & 0x03) << 8;
            }
            if (WiimoteIR1found)
            {
                WiimoteIRSensors1X = report_buf[9] | ((report_buf[8] >> 0) & 0x03) << 8;
                WiimoteIRSensors1Y = report_buf[10] | ((report_buf[8] >> 2) & 0x03) << 8;
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
            ir1x = irx2;
            ir1y = iry2;
            ir2x = irx3;
            ir2y = iry3;
            irxc = irx2 + irx3;
            iryc = iry2 + iry3;
            if (WiimoteIR0found | WiimoteIR1found)
            {
                vallistirx.Add(irx);
                vallistirx.RemoveAt(0);
                vallistiry.Add(iry);
                vallistiry.RemoveAt(0);
                irx = irxc * (1024f / 1346f);
                iry = iryc + centery >= 0 ? Scale(iryc + centery, 0f, 782f + centery, 0f, 1024f) : Scale(iryc + centery, -782f + centery, 0f, -1024f, 0f);
            }
            else
            {
                if (irx - vallistirx.Average() >= 600f)
                    irx = 1024f;
                if (irx - vallistirx.Average() <= -600f)
                    irx = -1024f;
                if (iry - vallistiry.Average() >= 200f)
                    iry = 1024f;
                if (iry - vallistiry.Average() <= -200f)
                    iry = -1024f;
            }
            WiimoteButtonStateA = (report_buf[2] & 0x08) != 0;
            WiimoteButtonStateB = (report_buf[2] & 0x04) != 0;
            WiimoteButtonStateMinus = (report_buf[2] & 0x10) != 0;
            WiimoteButtonStateHome = (report_buf[2] & 0x80) != 0;
            WiimoteButtonStatePlus = (report_buf[1] & 0x10) != 0;
            WiimoteButtonStateOne = (report_buf[2] & 0x02) != 0;
            WiimoteButtonStateTwo = (report_buf[2] & 0x01) != 0;
            WiimoteButtonStateUp = (report_buf[1] & 0x08) != 0;
            WiimoteButtonStateDown = (report_buf[1] & 0x04) != 0;
            WiimoteButtonStateLeft = (report_buf[1] & 0x01) != 0;
            WiimoteButtonStateRight = (report_buf[1] & 0x02) != 0;
            WiimoteRawValuesX = report_buf[3] - 135f + calibrationinit;
            WiimoteRawValuesY = report_buf[4] - 135f + calibrationinit;
            WiimoteRawValuesZ = report_buf[5] - 135f + calibrationinit;
            WiimoteNunchuckStateRawJoystickX = report_buf[16] - 125f + stickviewxinit;
            WiimoteNunchuckStateRawJoystickY = report_buf[17] - 125f + stickviewyinit;
            WiimoteNunchuckStateRawValuesX = report_buf[18] - 125f;
            WiimoteNunchuckStateRawValuesY = report_buf[19] - 125f;
            WiimoteNunchuckStateRawValuesZ = report_buf[20] - 125f;
            WiimoteNunchuckStateC = (report_buf[21] & 0x02) == 0;
            WiimoteNunchuckStateZ = (report_buf[21] & 0x01) == 0;
        }
        private double Scale(double value, double min, double max, double minScale, double maxScale)
        {
            double scaled = minScale + (double)(value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }
        private void Reconnection()
        {
            if (reconnectingwiimotecount == 0)
                reconnectingwiimotebool = true;
            reconnectingwiimotecount++;
            if (reconnectingwiimotecount >= 150f)
            {
                if (reconnectingwiimotebool)
                {
                    ReconnectionInit();
                    WiimoteFound(path);
                    reconnectingwiimotecount = -150f;
                }
                else
                    reconnectingwiimotecount = 0;
            }
        }
        private void ReconnectionInit()
        {
            WiimoteButtonStateA = false;
            WiimoteButtonStateB = false;
            WiimoteButtonStateMinus = false;
            WiimoteButtonStateHome = false;
            WiimoteButtonStatePlus = false;
            WiimoteButtonStateOne = false;
            WiimoteButtonStateTwo = false;
            WiimoteButtonStateUp = false;
            WiimoteButtonStateDown = false;
            WiimoteButtonStateLeft = false;
            WiimoteButtonStateRight = false;
            WiimoteRawValuesX = 0f;
            WiimoteRawValuesY = 0f;
            WiimoteRawValuesZ = 0f;
            WiimoteNunchuckStateRawJoystickX = 0f;
            WiimoteNunchuckStateRawJoystickY = 0f;
            WiimoteNunchuckStateRawValuesX = 0f;
            WiimoteNunchuckStateRawValuesY = 0f;
            WiimoteNunchuckStateRawValuesZ = 0f;
            WiimoteNunchuckStateC = false;
            WiimoteNunchuckStateZ = false;
        }
        private const string vendor_id = "57e", vendor_id_ = "057e", product_id = "0330", product_id_ = "0306";
        private enum EFileAttributes : uint
        {
            Overlapped = 0x40000000,
            Normal = 0x80
        };
        private struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid InterfaceClassGuid;
            public int Flags;
            public IntPtr RESERVED;
        }
        private struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public UInt32 cbSize;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }
        public void ScanWiimote()
        {
            do
                Thread.Sleep(1);
            while (!wiimoteconnect());
            int index = 0;
            Guid guid;
            HidD_GetHidGuid(out guid);
            IntPtr hDevInfo = SetupDiGetClassDevs(ref guid, null, new IntPtr(), 0x00000010);
            SP_DEVICE_INTERFACE_DATA diData = new SP_DEVICE_INTERFACE_DATA();
            diData.cbSize = Marshal.SizeOf(diData);
            while (SetupDiEnumDeviceInterfaces(hDevInfo, new IntPtr(), ref guid, index, ref diData))
            {
                UInt32 size;
                SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, new IntPtr(), 0, out size, new IntPtr());
                SP_DEVICE_INTERFACE_DETAIL_DATA diDetail = new SP_DEVICE_INTERFACE_DETAIL_DATA();
                diDetail.cbSize = 5;
                if (SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, ref diDetail, size, out size, new IntPtr()))
                {
                    if ((diDetail.DevicePath.Contains(vendor_id) | diDetail.DevicePath.Contains(vendor_id_)) & (diDetail.DevicePath.Contains(product_id) | diDetail.DevicePath.Contains(product_id_)))
                    {
                        path = diDetail.DevicePath;
                        isvalidhandle = WiimoteFound(path);
                        isvalidhandle = WiimoteFound(path);
                        isvalidhandle = WiimoteFound(path);
                        handleunshared = CreateFile(path, FileAccess.ReadWrite, FileShare.None, IntPtr.Zero, FileMode.Open, (uint)EFileAttributes.Normal, IntPtr.Zero);
                        if (isvalidhandle)
                        {
                            break;
                        }
                    }
                }
                index++;
            }
        }
        private bool WiimoteFound(string path)
        {
            try
            {
                handle = CreateFile(path, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, (uint)EFileAttributes.Normal, IntPtr.Zero);
                WriteData(handle, IR, (int)REGISTER_IR, new byte[] { 0x08 }, 1);
                WriteData(handle, Type, (int)REGISTER_EXTENSION_INIT_1, new byte[] { 0x55 }, 1);
                WriteData(handle, Type, (int)REGISTER_EXTENSION_INIT_2, new byte[] { 0x00 }, 1);
                WriteData(handle, Type, (int)REGISTER_MOTIONPLUS_INIT, new byte[] { 0x04 }, 1);
                ReadData(handle, 0x0016, 7);
                ReadData(handle, (int)REGISTER_EXTENSION_TYPE, 6);
                ReadData(handle, (int)REGISTER_EXTENSION_CALIBRATION, 16);
                ReadData(handle, (int)REGISTER_EXTENSION_CALIBRATION, 32);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void ReadData(SafeFileHandle _hFile, int address, short size)
        {
            mBuff[0] = (byte)ReadMemory;
            mBuff[1] = (byte)((address & 0xff000000) >> 24);
            mBuff[2] = (byte)((address & 0x00ff0000) >> 16);
            mBuff[3] = (byte)((address & 0x0000ff00) >> 8);
            mBuff[4] = (byte)(address & 0x000000ff);
            mBuff[5] = (byte)((size & 0xff00) >> 8);
            mBuff[6] = (byte)(size & 0xff);
            HidD_SetOutputReport(_hFile.DangerousGetHandle(), mBuff, 22);
        }
        private void WriteData(SafeFileHandle _hFile, byte mbuff, int address, byte[] buff, short size)
        {
            mBuff[0] = (byte)mbuff;
            mBuff[1] = (byte)(0x04);
            mBuff[2] = (byte)IRExtensionAccel;
            Array.Copy(buff, 0, mBuff, 3, 1);
            HidD_SetOutputReport(_hFile.DangerousGetHandle(), mBuff, 22);
            mBuff[0] = (byte)WriteMemory;
            mBuff[1] = (byte)(((address & 0xff000000) >> 24));
            mBuff[2] = (byte)((address & 0x00ff0000) >> 16);
            mBuff[3] = (byte)((address & 0x0000ff00) >> 8);
            mBuff[4] = (byte)((address & 0x000000ff) >> 0);
            mBuff[5] = (byte)size;
            Array.Copy(buff, 0, mBuff, 6, 1);
            HidD_SetOutputReport(_hFile.DangerousGetHandle(), mBuff, 22);
        }
    }
}
namespace controllers
{
    public class XBoxController
    {
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern uint TimeBeginPeriod(uint ms);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        private static extern uint TimeEndPeriod(uint ms);
        [DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
        private static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
        private static uint CurrentResolution = 0;
        private int number;
        private const string SCP_BUS_CLASS_GUID = "{F679F562-3164-42CE-A4DB-E7DDBE723909}";
        private SafeFileHandle _deviceHandle;
        private int transferred = 0;
        private byte[] outputBuffer = null;
        private valuechanged ValueChanged = new valuechanged();
        public void Connect(int number = 0)
        {
            this.number = number;
            string devicePath = "";
            if (Find(new Guid(SCP_BUS_CLASS_GUID), ref devicePath, 0))
            {
                _deviceHandle = GetHandle(devicePath);
            }
            if (number == 0 | number == 1)
                PlugIn(1);
            else if (number == 2)
                PlugIn(2);
        }
        public void Disconnect()
        {
            Set(false, false, false, false, false, false, false, false, false, false, false, false, false, false, 0, 0, 0, 0, 0, 0, false);
            if (number == 0 | number == 1)
                Unplug(1);
            else if (number == 2)
                Unplug(2);
        }
        public void Set(bool back, bool start, bool A, bool B, bool X, bool Y, bool up, bool left, bool down, bool right, bool leftstick, bool rightstick, bool leftbumper, bool rightbumper, double leftstickx, double leftsticky, double rightstickx, double rightsticky, double lefttriggerposition, double righttriggerposition, bool xbox)
        {
            ValueChanged[0] = back;
            if (valuechanged._ValueChanged[0] & valuechanged._valuechanged[0])
                Buttons ^= X360Buttons.Back;
            if (!back)
                Buttons &= ~X360Buttons.Back;
            ValueChanged[1] = start;
            if (valuechanged._ValueChanged[1] & valuechanged._valuechanged[1])
                Buttons ^= X360Buttons.Start;
            if (!start)
                Buttons &= ~X360Buttons.Start;
            ValueChanged[2] = A;
            if (valuechanged._ValueChanged[2] & valuechanged._valuechanged[2])
                Buttons ^= X360Buttons.A;
            if (!A)
                Buttons &= ~X360Buttons.A;
            ValueChanged[3] = B;
            if (valuechanged._ValueChanged[3] & valuechanged._valuechanged[3])
                Buttons ^= X360Buttons.B;
            if (!B)
                Buttons &= ~X360Buttons.B;
            ValueChanged[4] = X;
            if (valuechanged._ValueChanged[4] & valuechanged._valuechanged[4])
                Buttons ^= X360Buttons.X;
            if (!X)
                Buttons &= ~X360Buttons.X;
            ValueChanged[5] = Y;
            if (valuechanged._ValueChanged[5] & valuechanged._valuechanged[5])
                Buttons ^= X360Buttons.Y;
            if (!Y)
                Buttons &= ~X360Buttons.Y;
            ValueChanged[6] = up;
            if (valuechanged._ValueChanged[6] & valuechanged._valuechanged[6])
                Buttons ^= X360Buttons.Up;
            if (!up)
                Buttons &= ~X360Buttons.Up;
            ValueChanged[7] = left;
            if (valuechanged._ValueChanged[7] & valuechanged._valuechanged[7])
                Buttons ^= X360Buttons.Left;
            if (!left)
                Buttons &= ~X360Buttons.Left;
            ValueChanged[8] = down;
            if (valuechanged._ValueChanged[8] & valuechanged._valuechanged[8])
                Buttons ^= X360Buttons.Down;
            if (!down)
                Buttons &= ~X360Buttons.Down;
            ValueChanged[9] = right;
            if (valuechanged._ValueChanged[9] & valuechanged._valuechanged[9])
                Buttons ^= X360Buttons.Right;
            if (!right)
                Buttons &= ~X360Buttons.Right;
            ValueChanged[10] = leftstick;
            if (valuechanged._ValueChanged[10] & valuechanged._valuechanged[10])
                Buttons ^= X360Buttons.LeftStick;
            if (!leftstick)
                Buttons &= ~X360Buttons.LeftStick;
            ValueChanged[11] = rightstick;
            if (valuechanged._ValueChanged[11] & valuechanged._valuechanged[11])
                Buttons ^= X360Buttons.RightStick;
            if (!rightstick)
                Buttons &= ~X360Buttons.RightStick;
            ValueChanged[12] = leftbumper;
            if (valuechanged._ValueChanged[12] & valuechanged._valuechanged[12])
                Buttons ^= X360Buttons.LeftBumper;
            if (!leftbumper)
                Buttons &= ~X360Buttons.LeftBumper;
            ValueChanged[13] = rightbumper;
            if (valuechanged._ValueChanged[13] & valuechanged._valuechanged[13])
                Buttons ^= X360Buttons.RightBumper;
            if (!rightbumper)
                Buttons &= ~X360Buttons.RightBumper;
            ValueChanged[14] = xbox;
            if (valuechanged._ValueChanged[14] & valuechanged._valuechanged[14])
                Buttons ^= X360Buttons.Logo;
            if (!xbox)
                Buttons &= ~X360Buttons.Logo;
            LeftStickX = (short)leftstickx;
            LeftStickY = (short)leftsticky;
            RightStickX = (short)rightstickx;
            RightStickY = (short)rightsticky;
            LeftTrigger = (byte)lefttriggerposition;
            RightTrigger = (byte)righttriggerposition;
            Report(GetReport());
        }
        public XBoxController()
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
        }
        private bool PlugIn(int controllerNumber)
        {
            int transfered = 0;
            byte[] buffer = new byte[16];
            buffer[0] = 0x10;
            buffer[1] = 0x00;
            buffer[2] = 0x00;
            buffer[3] = 0x00;
            buffer[4] = (byte)((controllerNumber) & 0xFF);
            buffer[5] = (byte)((controllerNumber >> 8) & 0xFF);
            buffer[6] = (byte)((controllerNumber >> 16) & 0xFF);
            buffer[7] = (byte)((controllerNumber >> 24) & 0xFF);
            return DeviceIoControl(_deviceHandle, 0x2A4000, buffer, buffer.Length, null, 0, ref transfered, IntPtr.Zero);
        }
        private bool Unplug(int controllerNumber)
        {
            int transfered = 0;
            byte[] buffer = new byte[16];
            buffer[0] = 0x10;
            buffer[1] = 0x00;
            buffer[2] = 0x00;
            buffer[3] = 0x00;
            buffer[4] = (byte)((controllerNumber) & 0xFF);
            buffer[5] = (byte)((controllerNumber >> 8) & 0xFF);
            buffer[6] = (byte)((controllerNumber >> 16) & 0xFF);
            buffer[7] = (byte)((controllerNumber >> 24) & 0xFF);
            return DeviceIoControl(_deviceHandle, 0x2A4004, buffer, buffer.Length, null, 0, ref transfered, IntPtr.Zero);
        }
        private bool Report(byte[] controllerReport)
        {
            return DeviceIoControl(_deviceHandle, 0x2A400C, controllerReport, controllerReport.Length, outputBuffer, outputBuffer?.Length ?? 0, ref transferred, IntPtr.Zero) && transferred > 0;
        }
        private bool Find(Guid target, ref string path, int instance = 0)
        {
            IntPtr detailDataBuffer = IntPtr.Zero;
            IntPtr deviceInfoSet = IntPtr.Zero;
            try
            {
                SP_DEVICE_INTERFACE_DATA DeviceInterfaceData = new SP_DEVICE_INTERFACE_DATA(), da = new SP_DEVICE_INTERFACE_DATA();
                int bufferSize = 0, memberIndex = 0;
                deviceInfoSet = SetupDiGetClassDevs(ref target, IntPtr.Zero, IntPtr.Zero, DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);
                DeviceInterfaceData.cbSize = da.cbSize = Marshal.SizeOf(DeviceInterfaceData);
                while (SetupDiEnumDeviceInterfaces(deviceInfoSet, IntPtr.Zero, ref target, memberIndex, ref DeviceInterfaceData))
                {
                    SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref DeviceInterfaceData, IntPtr.Zero, 0, ref bufferSize, ref da);
                    detailDataBuffer = Marshal.AllocHGlobal(bufferSize);
                    Marshal.WriteInt32(detailDataBuffer, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);
                    if (SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref DeviceInterfaceData, detailDataBuffer, bufferSize, ref bufferSize, ref da))
                    {
                        IntPtr pDevicePathName = detailDataBuffer + 4;
                        path = Marshal.PtrToStringAuto(pDevicePathName).ToUpper(CultureInfo.InvariantCulture);
                        Marshal.FreeHGlobal(detailDataBuffer);
                        if (memberIndex == instance)
                            return true;
                    }
                    else
                        Marshal.FreeHGlobal(detailDataBuffer);
                    memberIndex++;
                }
            }
            finally
            {
                if (deviceInfoSet != IntPtr.Zero)
                {
                    SetupDiDestroyDeviceInfoList(deviceInfoSet);
                }
            }
            return false;
        }
        private SafeFileHandle GetHandle(string devicePath)
        {
            devicePath = devicePath.ToUpper(CultureInfo.InvariantCulture);
            SafeFileHandle handle = CreateFile(devicePath, GENERIC_WRITE | GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED, UIntPtr.Zero);
            return handle;
        }
        private X360Buttons Buttons { get; set; }
        private byte LeftTrigger { get; set; }
        private byte RightTrigger { get; set; }
        private short LeftStickX { get; set; }
        private short LeftStickY { get; set; }
        private short RightStickX { get; set; }
        private short RightStickY { get; set; }
        private byte[] fullReport = { 0x1C, 0, 0, 0, (1) & 0xFF, (1 >> 8) & 0xFF, (1 >> 16) & 0xFF, (1 >> 24) & 0xFF, 0x00, 0x14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        private byte[] GetReport()
        {
            fullReport[10] = (byte)((ushort)Buttons & 0xFF);
            fullReport[11] = (byte)((ushort)Buttons >> 8 & 0xFF);
            fullReport[12] = LeftTrigger;
            fullReport[13] = RightTrigger;
            fullReport[14] = (byte)(LeftStickX & 0xFF);
            fullReport[15] = (byte)(LeftStickX >> 8 & 0xFF);
            fullReport[16] = (byte)(LeftStickY & 0xFF);
            fullReport[17] = (byte)(LeftStickY >> 8 & 0xFF);
            fullReport[18] = (byte)(RightStickX & 0xFF);
            fullReport[19] = (byte)(RightStickX >> 8 & 0xFF);
            fullReport[20] = (byte)(RightStickY & 0xFF);
            fullReport[21] = (byte)(RightStickY >> 8 & 0xFF);
            return fullReport;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct SP_DEVICE_INTERFACE_DATA
        {
            internal int cbSize;
            internal Guid InterfaceClassGuid;
            internal int Flags;
            internal IntPtr Reserved;
        }
        internal const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        internal const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        internal const uint FILE_SHARE_READ = 1;
        internal const uint FILE_SHARE_WRITE = 2;
        internal const uint GENERIC_READ = 0x80000000;
        internal const uint GENERIC_WRITE = 0x40000000;
        internal const uint OPEN_EXISTING = 3;
        internal const int DIGCF_PRESENT = 0x0002;
        internal const int DIGCF_DEVICEINTERFACE = 0x0010;
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, UIntPtr hTemplateFile);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeviceIoControl(SafeFileHandle hDevice, int dwIoControlCode, byte[] lpInBuffer, int nInBufferSize, byte[] lpOutBuffer, int nOutBufferSize, ref int lpBytesReturned, IntPtr lpOverlapped);
        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern int SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);
        [DllImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInfo, ref Guid interfaceClassGuid, int memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr SetupDiGetClassDevs(ref Guid classGuid, IntPtr enumerator, IntPtr hwndParent, int flags);
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, ref SP_DEVICE_INTERFACE_DATA deviceInfoData);
        [Flags]
        private enum X360Buttons
        {
            None = 0,
            Up = 1 << 0,
            Down = 1 << 1,
            Left = 1 << 2,
            Right = 1 << 3,
            Start = 1 << 4,
            Back = 1 << 5,
            LeftStick = 1 << 6,
            RightStick = 1 << 7,
            LeftBumper = 1 << 8,
            RightBumper = 1 << 9,
            Logo = 1 << 10,
            A = 1 << 12,
            B = 1 << 13,
            X = 1 << 14,
            Y = 1 << 15,
        }
    }
}
namespace ValueStateChanged
{
    public class valuechanged
    {
        public static bool[] _valuechanged = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
        public static bool[] _ValueChanged = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
        public bool this[int index]
        {
            get { return _ValueChanged[index]; }
            set
            {
                if (_valuechanged[index] != value)
                    _ValueChanged[index] = true;
                else
                    _ValueChanged[index] = false;
                _valuechanged[index] = value;
            }
        }
    }
}