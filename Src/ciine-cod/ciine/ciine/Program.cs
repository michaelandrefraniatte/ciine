using System;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Management;
using System.Windows.Forms;

namespace StringHandle
{
    internal class Program
    {
        static void OnKeyDown(Keys keyData)
        {
            if (keyData == Keys.F1)
            {
                const string message = "• Author: Michaël André Franiatte.\n\r\n\r• Contact: michael.franiatte@gmail.com.\n\r\n\r• Publisher: https://github.com/michaelandrefraniatte.\n\r\n\r• Copyrights: All rights reserved, no permissions granted.\n\r\n\r• License: Not open source, not free of charge to use.";
                const string caption = "About";
                MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow([In] IntPtr hWnd, [In] Int32 nCmdShow);
        const Int32 SW_MINIMIZE = 6;
        static ConsoleEventDelegate handler;
        private delegate bool ConsoleEventDelegate(int eventType);
        private static Type program;
        private static object obj;
        private static Assembly assembly;
        private static System.CodeDom.Compiler.CompilerResults results;
        private static Microsoft.CSharp.CSharpCodeProvider provider;
        private static System.CodeDom.Compiler.CompilerParameters parameters;
        private static string code = @"
                using System;
                using System.Globalization;
                using System.IO;
                using System.Numerics;
                using System.Runtime.InteropServices;
                using System.Threading;
                using System.Threading.Tasks;
                using System.Windows;
                using System.Windows.Forms;
                using System.Reflection;
                using Microsoft.Win32.SafeHandles;
                using System.Linq;
                using System.Collections;
                using System.Runtime;
                using System.Collections.Generic;
                using controllers;
                namespace StringToCode
                {
                    public class FooClass 
                    { 
                        [DllImport(""winmm.dll"", EntryPoint = ""timeBeginPeriod"")]
                        private static extern uint TimeBeginPeriod(uint ms);
                        [DllImport(""winmm.dll"", EntryPoint = ""timeEndPeriod"")]
                        private static extern uint TimeEndPeriod(uint ms);
                        [DllImport(""ntdll.dll"", EntryPoint = ""NtSetTimerResolution"")]
                        private static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
                        private static uint CurrentResolution = 0;
                        private static bool controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_xbox;
                        private static double controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition;
                        private static double mousex = 0f, mousey = 0f, viewpower1x = 0f, viewpower2x = 1f, viewpower3x = 0f, viewpower1y = 0.25f, viewpower2y = 0.75f, viewpower3y = 0f, dzx = 2.0f, dzy = 2.0f, countup = 0, countupup = 0, countxy = 0, county = 0;
                        private static bool getstate;
                        private bool running;
                        private static Valuechange ValueChange = new Valuechange();
                        private WiiMote wm = new WiiMote();
                        private XBoxController scp = new XBoxController();
                        private ThreadStart threadstart;
                        private Thread thread;
                        public void Close()
                        {
                            threadstart = new ThreadStart(FormClose);
                            thread = new Thread(threadstart);
                            thread.Start();
                        }
                        public static void Main() {}
                        private void FormClose()
                        {
                            try
                            {
                                running = false;
                                Thread.Sleep(100);
                                scp.Disconnect();
                                wm.Close();
                            }
                            catch { }
                        }
                        public void Load()
                        {
                            TimeBeginPeriod(1);
                            NtSetTimerResolution(1, true, ref CurrentResolution);
                            Task.Run(() => Start());
                        }
                        private void Start()
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
                                ValueChange[0] = wm.WiimoteButtonStateA ? 1 : 0;
                                if (ValueChange._ValueChange[0] > 0f & !getstate)
                                {
                                    getstate = true;
                                }
                                else
                                {
                                    if (ValueChange._ValueChange[0] > 0f & getstate)
                                    {
                                        getstate = false;
                                    }
                                }
                                if (controller1_send_X | controller1_send_Y | controller1_send_rightbumper | controller1_send_leftbumper | controller1_send_rightstick | controller1_send_leftstick | controller1_send_back | controller1_send_start)
                                {
                                    getstate = false;
                                }
                                controller1_send_lefttriggerposition = getstate ? 255 : 0;
                                controller1_send_rightstick = wm.WiimoteNunchuckStateRawValuesY >= 90f;
                                controller1_send_leftstick = wm.WiimoteNunchuckStateZ;
                                controller1_send_A = wm.WiimoteNunchuckStateC;
                                controller1_send_back = wm.WiimoteButtonStateOne;
                                controller1_send_start = wm.WiimoteButtonStateTwo;
                                ValueChange[1] = wm.WiimoteButtonStateRight ? 1 : 0;
                                if ((countxy > 0 & countxy < 150 & ValueChange._ValueChange[1] < 0f) | county > 0)
                                    county++;
                                if (county > 50)
                                    county = 0;
                                countxy = wm.WiimoteButtonStateRight ? countxy + 1 : 0;
                                controller1_send_Y = county > 0;
                                controller1_send_X = countxy > 150 | ((wm.WiimoteRawValuesZ > 0 ? wm.WiimoteRawValuesZ : -wm.WiimoteRawValuesZ) >= 30f & (wm.WiimoteRawValuesY > 0 ? wm.WiimoteRawValuesY : -wm.WiimoteRawValuesY) >= 30f & (wm.WiimoteRawValuesX > 0 ? wm.WiimoteRawValuesX : -wm.WiimoteRawValuesX) >= 30f);
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
                                if ((countup > 0 & countup < 150) | countupup > 0)
                                    countupup++;
                                if (countupup > 150)
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
                                        controller1_send_up = (countupup > 0 & countupup < 50) | (countupup > 100 & countupup < 150) | countup > 150;
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
                    public class Valuechange
                    {
                        [DllImport(""winmm.dll"", EntryPoint = ""timeBeginPeriod"")]
                        private static extern uint TimeBeginPeriod(uint ms);
                        [DllImport(""winmm.dll"", EntryPoint = ""timeEndPeriod"")]
                        private static extern uint TimeEndPeriod(uint ms);
                        [DllImport(""ntdll.dll"", EntryPoint = ""NtSetTimerResolution"")]
                        private static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
                        private static uint CurrentResolution = 0;
                        public double[] _valuechange = { 0, 0 };
                        public double[] _ValueChange = { 0, 0 };
                        public Valuechange()
                        {
                            TimeBeginPeriod(1);
                            NtSetTimerResolution(1, true, ref CurrentResolution);
                        }
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
                    public class WiiMote
                    {
                        [DllImport(""MotionInputPairing.dll"", EntryPoint = ""wiimoteconnect"")]
                        private static extern bool wiimoteconnect();
                        [DllImport(""MotionInputPairing.dll"", EntryPoint = ""wiimotedisconnect"")]
                        private static extern bool wiimotedisconnect();
                        [DllImport(""hid.dll"")]
                        private static extern void HidD_GetHidGuid(out Guid gHid);
                        [DllImport(""hid.dll"")]
                        private extern static bool HidD_SetOutputReport(IntPtr HidDeviceObject, byte[] lpReportBuffer, uint ReportBufferLength);
                        [DllImport(""setupapi.dll"")]
                        private static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, string Enumerator, IntPtr hwndParent, UInt32 Flags);
                        [DllImport(""setupapi.dll"")]
                        private static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInvo, ref Guid interfaceClassGuid, Int32 memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);
                        [DllImport(""setupapi.dll"")]
                        private static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, UInt32 deviceInterfaceDetailDataSize, out UInt32 requiredSize, IntPtr deviceInfoData);
                        [DllImport(""setupapi.dll"")]
                        private static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, UInt32 deviceInterfaceDetailDataSize, out UInt32 requiredSize, IntPtr deviceInfoData);
                        [DllImport(""Kernel32.dll"")]
                        private static extern SafeFileHandle CreateFile(string fileName, [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess, [MarshalAs(UnmanagedType.U4)] FileShare fileShare, IntPtr securityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition, [MarshalAs(UnmanagedType.U4)] uint flags, IntPtr template);
                        [DllImport(""Kernel32.dll"")]
                        private static extern IntPtr CreateFile(string fileName, System.IO.FileAccess fileAccess, System.IO.FileShare fileShare, IntPtr securityAttributes, System.IO.FileMode creationDisposition, EFileAttributes flags, IntPtr template);
                        [DllImport(""winmm.dll"", EntryPoint = ""timeBeginPeriod"")]
                        private static extern uint TimeBeginPeriod(uint ms);
                        [DllImport(""winmm.dll"", EntryPoint = ""timeEndPeriod"")]
                        private static extern uint TimeEndPeriod(uint ms);
                        [DllImport(""ntdll.dll"", EntryPoint = ""NtSetTimerResolution"")]
                        private static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
                        private static uint CurrentResolution = 0;
                        private const double REGISTER_IR = 0x04b00030, REGISTER_EXTENSION_INIT_1 = 0x04a400f0, REGISTER_EXTENSION_INIT_2 = 0x04a400fb, REGISTER_EXTENSION_TYPE = 0x04a400fa, REGISTER_EXTENSION_CALIBRATION = 0x04a40020, REGISTER_MOTIONPLUS_INIT = 0x04a600fe;
                        private string path;
                        private byte[] mBuff = new byte[22], aBuffer = new byte[22];
                        private const byte Type = 0x12, IR = 0x13, WriteMemory = 0x16, ReadMemory = 0x16, IRExtensionAccel = 0x37;
                        private static FileStream mStream;
                        private static SafeFileHandle handle = null, handleunshared = null;
                        private bool reconnectingwiimotebool;
                        private double reconnectingwiimotecount;
                        private bool isvalidhandle = false;
                        private bool running;
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
                        }
                        public void Close()
                        {
                            running = false;
                            Thread.Sleep(100);
                            handleunshared.Close();
                            handleunshared.Dispose();
                            mStream.Close();
                            mStream.Dispose();
                            handle.Close();
                            handle.Dispose();
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
                                    mStream.Read(aBuffer, 0, 22);
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
                            calibrationinit = -aBuffer[4] + 135f;
                            stickviewxinit = -aBuffer[16] + 125f;
                            stickviewyinit = -aBuffer[17] + 125f;
                        }
                        private void ProcessStateLogic()
                        {
                            WiimoteIR0found = (aBuffer[6] | ((aBuffer[8] >> 4) & 0x03) << 8) > 1 & (aBuffer[6] | ((aBuffer[8] >> 4) & 0x03) << 8) < 1023;
                            WiimoteIR1found = (aBuffer[9] | ((aBuffer[8] >> 0) & 0x03) << 8) > 1 & (aBuffer[9] | ((aBuffer[8] >> 0) & 0x03) << 8) < 1023;
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
                                WiimoteIRSensors0X = aBuffer[6] | ((aBuffer[8] >> 4) & 0x03) << 8;
                                WiimoteIRSensors0Y = aBuffer[7] | ((aBuffer[8] >> 6) & 0x03) << 8;
                            }
                            if (WiimoteIR1found)
                            {
                                WiimoteIRSensors1X = aBuffer[9] | ((aBuffer[8] >> 0) & 0x03) << 8;
                                WiimoteIRSensors1Y = aBuffer[10] | ((aBuffer[8] >> 2) & 0x03) << 8;
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
                            irxc = irx2 + irx3;
                            iryc = iry2 + iry3;
                            if (WiimoteIR0found | WiimoteIR1found)
                            {
                                irx = irxc * (1024f / 1346f);
                                iry = iryc + centery >= 0 ? Scale(iryc + centery, 0f, 782f + centery, 0f, 1024f) : Scale(iryc + centery, -782f + centery, 0f, -1024f, 0f);
                            }
                            WiimoteButtonStateA = (aBuffer[2] & 0x08) != 0;
                            WiimoteButtonStateB = (aBuffer[2] & 0x04) != 0;
                            WiimoteButtonStateMinus = (aBuffer[2] & 0x10) != 0;
                            WiimoteButtonStateHome = (aBuffer[2] & 0x80) != 0;
                            WiimoteButtonStatePlus = (aBuffer[1] & 0x10) != 0;
                            WiimoteButtonStateOne = (aBuffer[2] & 0x02) != 0;
                            WiimoteButtonStateTwo = (aBuffer[2] & 0x01) != 0;
                            WiimoteButtonStateUp = (aBuffer[1] & 0x08) != 0;
                            WiimoteButtonStateDown = (aBuffer[1] & 0x04) != 0;
                            WiimoteButtonStateLeft = (aBuffer[1] & 0x01) != 0;
                            WiimoteButtonStateRight = (aBuffer[1] & 0x02) != 0;
                            WiimoteRawValuesX = aBuffer[3] - 135f + calibrationinit;
                            WiimoteRawValuesY = aBuffer[4] - 135f + calibrationinit;
                            WiimoteRawValuesZ = aBuffer[5] - 135f + calibrationinit;
                            WiimoteNunchuckStateRawJoystickX = aBuffer[16] - 125f + stickviewxinit;
                            WiimoteNunchuckStateRawJoystickY = aBuffer[17] - 125f + stickviewyinit;
                            WiimoteNunchuckStateRawValuesX = aBuffer[18] - 125f;
                            WiimoteNunchuckStateRawValuesY = aBuffer[19] - 125f;
                            WiimoteNunchuckStateRawValuesZ = aBuffer[20] - 125f;
                            WiimoteNunchuckStateC = (aBuffer[21] & 0x02) == 0;
                            WiimoteNunchuckStateZ = (aBuffer[21] & 0x01) == 0;
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
                        private const string vendor_id = ""57e"", vendor_id_ = ""057e"", product_id = ""0330"", product_id_ = ""0306"";
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
                                        if (handleunshared != null)
                                        {
                                            handleunshared.Close();
                                            handleunshared.Dispose();
                                            handleunshared = null;
                                        }
                                        path = diDetail.DevicePath;
                                        isvalidhandle = WiimoteFound(path);
                                        isvalidhandle = WiimoteFound(path);
                                        isvalidhandle = WiimoteFound(path);
                                        handleunshared = CreateFile(path, FileAccess.ReadWrite, FileShare.None, IntPtr.Zero, FileMode.Open, (uint)EFileAttributes.Overlapped, IntPtr.Zero);
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
                                handle = CreateFile(path, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, (uint)EFileAttributes.Overlapped, IntPtr.Zero);
                                WriteData(handle, IR, (int)REGISTER_IR, new byte[] { 0x08 }, 1);
                                WriteData(handle, Type, (int)REGISTER_EXTENSION_INIT_1, new byte[] { 0x55 }, 1);
                                WriteData(handle, Type, (int)REGISTER_EXTENSION_INIT_2, new byte[] { 0x00 }, 1);
                                WriteData(handle, Type, (int)REGISTER_MOTIONPLUS_INIT, new byte[] { 0x04 }, 1);
                                ReadData(handle, 0x0016, 7);
                                ReadData(handle, (int)REGISTER_EXTENSION_TYPE, 6);
                                ReadData(handle, (int)REGISTER_EXTENSION_CALIBRATION, 16);
                                ReadData(handle, (int)REGISTER_EXTENSION_CALIBRATION, 32);
                                mStream = new FileStream(handle, FileAccess.Read, 22, true);
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
                }";
        static void Main(string[] args)
        {
            MinimizeConsoleWindow();
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);
            parameters = new System.CodeDom.Compiler.CompilerParameters();
            parameters.GenerateExecutable = true;
            parameters.GenerateInMemory = false;
            parameters.IncludeDebugInformation = true;
            parameters.TreatWarningsAsErrors = false;
            parameters.WarningLevel = 0;
            parameters.CompilerOptions = "/optimize+ /platform:x86 /target:exe /unsafe";
            parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            parameters.ReferencedAssemblies.Add("System.Drawing.dll");
            parameters.ReferencedAssemblies.Add(Application.StartupPath + @"\System.Windows.Forms.dll");
            parameters.ReferencedAssemblies.Add(Application.StartupPath + @"\System.Drawing.dll");
            parameters.ReferencedAssemblies.Add(Application.StartupPath + @"\System.Runtime.dll");
            parameters.ReferencedAssemblies.Add(Application.StartupPath + @"\System.Collections.dll");
            parameters.ReferencedAssemblies.Add(Application.StartupPath + @"\System.Linq.dll");
            parameters.ReferencedAssemblies.Add(Application.StartupPath + @"\System.Numerics.dll");
            parameters.ReferencedAssemblies.Add(Application.StartupPath + @"\controllers.dll");
            provider = new Microsoft.CSharp.CSharpCodeProvider();
            results = provider.CompileAssemblyFromSource(parameters, code);
            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();
                foreach (System.CodeDom.Compiler.CompilerError error in results.Errors)
                {
                    sb.AppendLine(String.Format("Error ({0}) : {1}", error.ErrorNumber, error.ErrorText));
                }
                MessageBox.Show("Script Error :\n\r" + sb.ToString());
                return;
            }
            assembly = results.CompiledAssembly;
            program = assembly.GetType("StringToCode.FooClass");
            obj = Activator.CreateInstance(program);
            program.InvokeMember("Load", BindingFlags.Default | BindingFlags.InvokeMethod, null, obj, new object[] { });
            Console.WriteLine("cod");
            Console.ReadKey();
        }
        private static void MinimizeConsoleWindow()
        {
            IntPtr hWndConsole = GetConsoleWindow();
            ShowWindow(hWndConsole, SW_MINIMIZE);
        }
        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                program.InvokeMember("Close", BindingFlags.Default | BindingFlags.InvokeMethod, null, obj, new object[] { });
                Thread.Sleep(5000);
            }
            return false;
        }
    }
}