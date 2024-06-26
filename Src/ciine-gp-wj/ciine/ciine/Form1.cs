﻿using System;
using System.Windows.Forms;
using System.Reflection;
using System.Text;

namespace ciine
{
    public partial class Form1 : Form
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
        public Form1()
        {
            InitializeComponent();
        }
        private Type program;
        private object obj;
        private Assembly assembly;
        private System.CodeDom.Compiler.CompilerResults results;
        private Microsoft.CSharp.CSharpCodeProvider provider;
        private System.CodeDom.Compiler.CompilerParameters parameters;
        private string code = @"
                using Microsoft.Win32.SafeHandles;
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
                using controllers;
                namespace StringToCode
                {
                    public class FooClass 
                    { 
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
                        [DllImport(""lhidread.dll"", CallingConvention = CallingConvention.Cdecl, EntryPoint = ""Lhid_read_timeout"")]
                        private static extern int Lhid_read_timeout(SafeFileHandle dev, byte[] data, UIntPtr length);
                        [DllImport(""lhidread.dll"", CallingConvention = CallingConvention.Cdecl, EntryPoint = ""Lhid_write"")]
                        private static extern int Lhid_write(SafeFileHandle device, byte[] data, UIntPtr length);
                        [DllImport(""lhidread.dll"", CallingConvention = CallingConvention.Cdecl, EntryPoint = ""Lhid_open_path"")]
                        private static extern SafeFileHandle Lhid_open_path(IntPtr handle);
                        [DllImport(""lhidread.dll"", CallingConvention = CallingConvention.Cdecl, EntryPoint = ""Lhid_close"")]
                        private static extern void Lhid_close(SafeFileHandle device);
                        [DllImport(""winmm.dll"", EntryPoint = ""timeBeginPeriod"")]
                        private static extern uint TimeBeginPeriod(uint ms);
                        [DllImport(""winmm.dll"", EntryPoint = ""timeEndPeriod"")]
                        private static extern uint TimeEndPeriod(uint ms);
                        [DllImport(""ntdll.dll"", EntryPoint = ""NtSetTimerResolution"")]
                        private static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
                        private static bool back, start, A, B, X, Y, up, left, down, right, leftstick, rightstick, leftbumper, rightbumper, lefttrigger, righttrigger;
                        private static double leftstickx, leftsticky, rightstickx, rightsticky;
                        private const double REGISTER_IR = 0x04b00030, REGISTER_EXTENSION_INIT_1 = 0x04a400f0, REGISTER_EXTENSION_INIT_2 = 0x04a400fb, REGISTER_EXTENSION_TYPE = 0x04a400fa, REGISTER_EXTENSION_CALIBRATION = 0x04a40020, REGISTER_MOTIONPLUS_INIT = 0x04a600fe;
                        private static double irx0, iry0, irx1, iry1, irx, iry, WiimoteIRSensors0X, WiimoteIRSensors0Y, WiimoteIRSensors1X, WiimoteIRSensors1Y, WiimoteRawValuesX, WiimoteRawValuesY, WiimoteRawValuesZ, calibrationinit, WiimoteIRSensors0Xcam, WiimoteIRSensors0Ycam, WiimoteIRSensors1Xcam, WiimoteIRSensors1Ycam, WiimoteIRSensorsXcam, WiimoteIRSensorsYcam;
                        private static bool WiimoteIR1found, WiimoteIR0found, WiimoteButtonStateA, WiimoteButtonStateB, WiimoteButtonStateMinus, WiimoteButtonStateHome, WiimoteButtonStatePlus, WiimoteButtonStateOne, WiimoteButtonStateTwo, WiimoteButtonStateUp, WiimoteButtonStateDown, WiimoteButtonStateLeft, WiimoteButtonStateRight, ISWIIMOTE, running;
                        private static byte[] buff = new byte[] { 0x55 }, mBuff = new byte[22], aBuffer = new byte[22];
                        private const byte Type = 0x12, IR = 0x13, WriteMemory = 0x16, ReadMemory = 0x16, IRExtensionAccel = 0x37;
                        private static uint CurrentResolution = 0;
                        private static FileStream mStream;
                        private static SafeFileHandle handle = null;
                        private double statex = 0f, statey = 0f, mousex = 0f, mousey = 0f, mousestatex = 0f, mousestatey = 0f, viewpower1x = 0f, viewpower2x = 1f, viewpower3x = 0f, viewpower1y = 0.25f, viewpower2y = 0.75f, viewpower3y = 0f, dzx = 2.0f, dzy = 2.0f, centery = 80f;
                        private bool[] getstate = new bool[12];
                        private int[] pollcount = new int[12];
                        private int[] keys12345 = new int[12];
                        private int[] keys54321 = new int[12];
                        private double[] mousexp = new double[12];
                        private double[] mouseyp = new double[12];
                        private int sleeptime = 1;
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
                        public void Load()
                        {
                            TimeBeginPeriod(1);
                            NtSetTimerResolution(1, true, ref CurrentResolution);
                            Task.Run(() => Start());
                        }
                        private void Start()
                        {
                            running = true;
                            do
                                Thread.Sleep(1);
                            while (!wiimotejoyconleftconnect());
                            ScanScanLeft();
                            Task.Run(() => taskD());
                            Task.Run(() => taskDLeft());
                            Thread.Sleep(1000);
                            calibrationinit = -aBuffer[4] + 135f;
                            stick_rawLeft[0] = report_bufLeft[6 + (ISLEFT ? 0 : 3)];
                            stick_rawLeft[1] = report_bufLeft[7 + (ISLEFT ? 0 : 3)];
                            stick_rawLeft[2] = report_bufLeft[8 + (ISLEFT ? 0 : 3)];
                            stickCenterLeft[0] = (UInt16)(stick_rawLeft[0] | ((stick_rawLeft[1] & 0xf) << 8));
                            stickCenterLeft[1] = (UInt16)((stick_rawLeft[1] >> 4) | (stick_rawLeft[2] << 4));
                            acc_gcalibrationLeftY = (Int16)(report_bufLeft[15] | ((report_bufLeft[16] << 8) & 0xff00));
                            JoyconLeftAccelY = ((Int16)(report_bufLeft[15] | ((report_bufLeft[16] << 8) & 0xff00)) - acc_gcalibrationLeftY) * (1.0f / 4000f);
                            ScpBus.LoadController();
                            Task.Run(() => taskX());
                        }
                        private void taskX()
                        {
                            while (running)
                            {
                                WiimoteIRSensors0X = aBuffer[6] | ((aBuffer[8] >> 4) & 0x03) << 8;
                                WiimoteIRSensors0Y = aBuffer[7] | ((aBuffer[8] >> 6) & 0x03) << 8;
                                WiimoteIRSensors1X = aBuffer[9] | ((aBuffer[8] >> 0) & 0x03) << 8;
                                WiimoteIRSensors1Y = aBuffer[10] | ((aBuffer[8] >> 2) & 0x03) << 8;
                                WiimoteIR0found = WiimoteIRSensors0X > 0f & WiimoteIRSensors0X <= 1024f & WiimoteIRSensors0Y > 0f & WiimoteIRSensors0Y <= 768f;
                                WiimoteIR1found = WiimoteIRSensors1X > 0f & WiimoteIRSensors1X <= 1024f & WiimoteIRSensors1Y > 0f & WiimoteIRSensors1Y <= 768f;
                                if (WiimoteIR0found)
                                {
                                    WiimoteIRSensors0Xcam = WiimoteIRSensors0X - 512f;
                                    WiimoteIRSensors0Ycam = WiimoteIRSensors0Y - 384f;
                                }
                                if (WiimoteIR1found)
                                {
                                    WiimoteIRSensors1Xcam = WiimoteIRSensors1X - 512f;
                                    WiimoteIRSensors1Ycam = WiimoteIRSensors1Y - 384f;
                                }
                                if (WiimoteIR0found & WiimoteIR1found)
                                {
                                    WiimoteIRSensorsXcam = (WiimoteIRSensors0Xcam + WiimoteIRSensors1Xcam) / 2f;
                                    WiimoteIRSensorsYcam = (WiimoteIRSensors0Ycam + WiimoteIRSensors1Ycam) / 2f;
                                }
                                if (WiimoteIR0found)
                                {
                                    irx0 = 2 * WiimoteIRSensors0Xcam - WiimoteIRSensorsXcam;
                                    iry0 = 2 * WiimoteIRSensors0Ycam - WiimoteIRSensorsYcam;
                                }
                                if (WiimoteIR1found)
                                {
                                    irx1 = 2 * WiimoteIRSensors1Xcam - WiimoteIRSensorsXcam;
                                    iry1 = 2 * WiimoteIRSensors1Ycam - WiimoteIRSensorsYcam;
                                }
                                irx = (irx0 + irx1) * (1024f / 1346f);
                                iry = iry0 + iry1 + centery >= 0 ? Scale(iry0 + iry1 + centery, 0f, 782f + centery, 0f, 1024f) : Scale(iry0 + iry1 + centery, -782f + centery, 0f, -1024f, 0f);
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
                                JoyconLeftAccelY = ((Int16)(report_bufLeft[15] | ((report_bufLeft[16] << 8) & 0xff00)) - acc_gcalibrationLeftY) * (1.0f / 4000f);
                                JoyconLeftButtonSHOULDER_1 = (report_bufLeft[3 + (ISLEFT ? 2 : 0)] & 0x40) != 0;
                                JoyconLeftButtonSHOULDER_2 = (report_bufLeft[3 + (ISLEFT ? 2 : 0)] & 0x80) != 0;
                                JoyconLeftButtonSR = (report_bufLeft[3 + (ISLEFT ? 2 : 0)] & 0x10) != 0;
                                JoyconLeftButtonSL = (report_bufLeft[3 + (ISLEFT ? 2 : 0)] & 0x20) != 0;
                                JoyconLeftButtonDPAD_DOWN = (report_bufLeft[3 + (ISLEFT ? 2 : 0)] & (ISLEFT ? 0x01 : 0x04)) != 0;
                                JoyconLeftButtonDPAD_RIGHT = (report_bufLeft[3 + (ISLEFT ? 2 : 0)] & (ISLEFT ? 0x04 : 0x08)) != 0;
                                JoyconLeftButtonDPAD_UP = (report_bufLeft[3 + (ISLEFT ? 2 : 0)] & (ISLEFT ? 0x02 : 0x02)) != 0;
                                JoyconLeftButtonDPAD_LEFT = (report_bufLeft[3 + (ISLEFT ? 2 : 0)] & (ISLEFT ? 0x08 : 0x01)) != 0;
                                JoyconLeftButtonMINUS = ((report_bufLeft[4] & 0x01) != 0);
                                JoyconLeftButtonCAPTURE = ((report_bufLeft[4] & 0x20) != 0);
                                JoyconLeftButtonSTICK = ((report_bufLeft[4] & (ISLEFT ? 0x08 : 0x04)) != 0);
                                stick_rawLeft[0] = report_bufLeft[6 + (ISLEFT ? 0 : 3)];
                                stick_rawLeft[1] = report_bufLeft[7 + (ISLEFT ? 0 : 3)];
                                stick_rawLeft[2] = report_bufLeft[8 + (ISLEFT ? 0 : 3)];
                                stickLeft[0] = ((UInt16)(stick_rawLeft[0] | ((stick_rawLeft[1] & 0xf) << 8)) - stickCenterLeft[0]) / 1100f;
                                stickLeft[1] = ((UInt16)((stick_rawLeft[1] >> 4) | (stick_rawLeft[2] << 4)) - stickCenterLeft[1]) / 1100f;
                                JoyconLeftStickX = stickLeft[0];
                                JoyconLeftStickY = stickLeft[1];
                                down = JoyconLeftButtonDPAD_DOWN;
                                left = JoyconLeftButtonDPAD_LEFT;
                                right = JoyconLeftButtonDPAD_RIGHT;
                                up = JoyconLeftButtonDPAD_UP;
                                rightstick = JoyconLeftAccelY <= -1.13;
                                leftstick = JoyconLeftButtonSHOULDER_2;
                                A = JoyconLeftButtonSHOULDER_1;
                                back = WiimoteButtonStateOne;
                                start = WiimoteButtonStateTwo;
                                X = WiimoteButtonStateHome | ((WiimoteRawValuesZ > 0 ? WiimoteRawValuesZ : -WiimoteRawValuesZ) >= 30f & (WiimoteRawValuesY > 0 ? WiimoteRawValuesY : -WiimoteRawValuesY) >= 30f & (WiimoteRawValuesX > 0 ? WiimoteRawValuesX : -WiimoteRawValuesX) >= 30f);
                                rightbumper = WiimoteButtonStatePlus | WiimoteButtonStateUp;
                                leftbumper = WiimoteButtonStateMinus | WiimoteButtonStateUp;
                                B = WiimoteButtonStateDown;
                                Y = WiimoteButtonStateLeft | WiimoteButtonStateRight;
                                righttrigger = WiimoteButtonStateB;
                                valchanged(0, WiimoteButtonStateA);
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
                                if (X | Y | rightbumper | leftbumper | rightstick | leftstick | back | start)
                                {
                                    getstate[0] = false;
                                }
                                lefttrigger = getstate[0];
                                if (irx >= 0f & irx <= 1024f)
                                    mousex = Scale(irx * irx * irx / 1024f / 1024f * viewpower3x + irx * irx / 1024f * viewpower2x + irx * viewpower1x, 0f, 1024f, dzx / 100f * 1024f, 1024f);
                                if (irx <= 0f & irx >= -1024f)
                                    mousex = Scale(-(-irx * -irx * -irx) / 1024f / 1024f * viewpower3x - (-irx * -irx) / 1024f * viewpower2x - (-irx) * viewpower1x, -1024f, 0f, -1024f, -(dzx / 100f) * 1024f);
                                if (iry >= 0f & iry <= 1024f)
                                    mousey = Scale(iry * iry * iry / 1024f / 1024f * viewpower3y + iry * iry / 1024f * viewpower2y + iry * viewpower1y, 0f, 1024f, dzy / 100f * 1024f, 1024f);
                                if (iry <= 0f & iry >= -1024f)
                                    mousey = Scale(-(-iry * -iry * -iry) / 1024f / 1024f * viewpower3y - (-iry * -iry) / 1024f * viewpower2y - (-iry) * viewpower1y, -1024f, 0f, -1024f, -(dzy / 100f) * 1024f);
                                rightstickx = (short)(-mousex / 1024f * 32767f);
                                rightsticky = (short)(-mousey / 1024f * 32767f);
                                if (JoyconLeftStickX > 0.35f)
                                    leftstickx = 32767;
                                if (JoyconLeftStickX < -0.35f)
                                    leftstickx = -32767;
                                if (JoyconLeftStickX <= 0.35f & JoyconLeftStickX >= -0.35f)
                                    leftstickx = 0;
                                if (JoyconLeftStickY > 0.35f)
                                    leftsticky = 32767;
                                if (JoyconLeftStickY < -0.35f)
                                    leftsticky = -32767;
                                if (JoyconLeftStickY <= 0.35f & JoyconLeftStickY >= -0.35f)
                                    leftsticky = 0;
                                ScpBus.SetController(back, start, A, B, X, Y, up, left, down, right, leftstick, rightstick, leftbumper, rightbumper, lefttrigger, righttrigger, leftstickx, leftsticky, rightstickx, rightsticky, 0, 0, false);
                                Thread.Sleep(sleeptime);
                            }
                        }
                        public void Close()
                        {
                            try
                            {
                                running = false;
                                Thread.Sleep(100);
                                ScpBus.UnLoadController();
                                mStream.Close();
                                Lhid_close(handleLeft);
                                handle.Close();
                                handleLeft.Close();
                                wiimotedisconnect();
                                joyconleftdisconnect();
                            }
                            catch { }
                        }
                        private static double Scale(double value, double min, double max, double minScale, double maxScale)
                        {
                            double scaled = minScale + (double)(value - min) / (max - min) * (maxScale - minScale);
                            return scaled;
                        }
                        private static void taskD()
                        {
                            while (running)
                            {
                                try
                                {
                                    mStream.Read(aBuffer, 0, 22);
                                }
                                catch { }
                            }
                        }
                        private static void taskDLeft()
                        {
                            while (running)
                            {
                                try
                                {
                                    Lhid_read_timeout(handleLeft, report_bufLeft, (UIntPtr)report_lenLeft);
                                }
                                catch { }
                            }
                        }
                        private const string vendor_id = ""57e"", vendor_id_ = ""057e"", product_r1 = ""0330"", product_r2 = ""0306"", product_l = ""2006"";
                        private enum EFileAttributes : uint
                        {
                            Overlapped = 0x40000000,
                            Normal = 0x80
                        };
                        struct SP_DEVICE_INTERFACE_DATA
                        {
                            public int cbSize;
                            public Guid InterfaceClassGuid;
                            public int Flags;
                            public IntPtr RESERVED;
                        }
                        struct SP_DEVICE_INTERFACE_DETAIL_DATA
                        {
                            public UInt32 cbSize;
                            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 256)]
                            public string DevicePath;
                        }
                        [DllImport(""MotionInputPairing.dll"", EntryPoint = ""wiimotejoyconleftconnect"")]
                        public static extern bool wiimotejoyconleftconnect();
                        [DllImport(""MotionInputPairing.dll"", EntryPoint = ""joyconleftdisconnect"")]
                        public static extern bool joyconleftdisconnect();
                        [DllImport(""MotionInputPairing.dll"", EntryPoint = ""wiimotedisconnect"")]
                        public static extern bool wiimotedisconnect();
                        private static bool ScanScanLeft()
                        {
                            ISWIIMOTE = false;
                            ISLEFT = false;
                            int index = 0;
                            System.Guid guid;
                            HidD_GetHidGuid(out guid);
                            System.IntPtr hDevInfo = SetupDiGetClassDevs(ref guid, null, new System.IntPtr(), 0x00000010);
                            SP_DEVICE_INTERFACE_DATA diData = new SP_DEVICE_INTERFACE_DATA();
                            diData.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(diData);
                            while (SetupDiEnumDeviceInterfaces(hDevInfo, new System.IntPtr(), ref guid, index, ref diData))
                            {
                                System.UInt32 size;
                                SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, new System.IntPtr(), 0, out size, new System.IntPtr());
                                SP_DEVICE_INTERFACE_DETAIL_DATA diDetail = new SP_DEVICE_INTERFACE_DETAIL_DATA();
                                diDetail.cbSize = 5;
                                if (SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, ref diDetail, size, out size, new System.IntPtr()))
                                {
                                    if ((diDetail.DevicePath.Contains(vendor_id) | diDetail.DevicePath.Contains(vendor_id_)) & (diDetail.DevicePath.Contains(product_r1) | diDetail.DevicePath.Contains(product_r2)))
                                    {
                                        ISWIIMOTE = true;
                                        WiimoteFound(diDetail.DevicePath);
                                    }
                                    if ((diDetail.DevicePath.Contains(vendor_id) | diDetail.DevicePath.Contains(vendor_id_)) & diDetail.DevicePath.Contains(product_l))
                                    {
                                        ISLEFT = true;
                                        AttachJoyLeft(diDetail.DevicePath);
                                    }
                                    if (ISWIIMOTE & ISLEFT)
                                        return true;
                                }
                                index++;
                            }
                            return false;
                        }
                        private static void WiimoteFound(string path)
                        {
                            do
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
                            }
                            while (handle.IsInvalid);
                            mStream = new FileStream(handle, FileAccess.ReadWrite, 22, true);
                        }
                        private static void ReadData(SafeFileHandle _hFile, int address, short size)
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
                        private static void WriteData(SafeFileHandle _hFile, byte mbuff, int address, byte[] buff, short size)
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
                        private static double JoyconLeftStickX, JoyconLeftStickY;
                        private static double[] stickLeft = { 0, 0 };
                        private static double[] stickCenterLeft = { 0, 0 };
                        private static byte[] stick_rawLeft = { 0, 0, 0 };
                        private static SafeFileHandle handleLeft;
                        private const uint report_lenLeft = 49;
                        private static bool JoyconLeftButtonSHOULDER_1, JoyconLeftButtonSHOULDER_2, JoyconLeftButtonSR, JoyconLeftButtonSL, JoyconLeftButtonDPAD_DOWN, JoyconLeftButtonDPAD_RIGHT, JoyconLeftButtonDPAD_UP, JoyconLeftButtonDPAD_LEFT, JoyconLeftButtonMINUS, JoyconLeftButtonSTICK, JoyconLeftButtonCAPTURE, ISLEFT;
                        private static byte[] report_bufLeft = new byte[report_lenLeft];
                        private static float acc_gcalibrationLeftY;
                        private static double JoyconLeftAccelY;
                        private static void AttachJoyLeft(string path)
                        {
                            do
                            {
                                IntPtr handle = CreateFile(path, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite, new System.IntPtr(), System.IO.FileMode.Open, EFileAttributes.Normal, new System.IntPtr());
                                handleLeft = Lhid_open_path(handle);
                                SubcommandLeft(0x40, new byte[] { 0x1 }, 1);
                                SubcommandLeft(0x3, new byte[] { 0x30 }, 1);
                            }
                            while (handleLeft.IsInvalid);
                        }
                        private static void SubcommandLeft(byte sc, byte[] buf, uint len)
                        {
                            byte[] buf_Left = new byte[report_lenLeft];
                            Array.Copy(buf, 0, buf_Left, 11, len);
                            buf_Left[10] = sc;
                            buf_Left[1] = 0;
                            buf_Left[0] = 0x1;
                            Lhid_write(handleLeft, buf_Left, (UIntPtr)(len + 11));
                            Lhid_read_timeout(handleLeft, buf_Left, (UIntPtr)report_lenLeft);
                        }
                    }
                }";
        private void Form1_Load(object sender, EventArgs e)
        {
            parameters = new System.CodeDom.Compiler.CompilerParameters();
            parameters.GenerateExecutable = false;
            parameters.GenerateInMemory = true;
            parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            parameters.ReferencedAssemblies.Add("System.Drawing.dll");
            parameters.ReferencedAssemblies.Add("controllers.dll");
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
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            program.InvokeMember("Close", BindingFlags.Default | BindingFlags.InvokeMethod, null, obj, new object[] { });
        }
    }
}