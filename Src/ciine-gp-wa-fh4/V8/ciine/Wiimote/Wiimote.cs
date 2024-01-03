﻿using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace WiiMoteAPI
{
    public class WiiMote
    {
        [DllImport("MotionInputPairing.dll", EntryPoint = "wiimoteconnect")]
        public static extern bool wiimoteconnect();
        [DllImport("MotionInputPairing.dll", EntryPoint = "wiimotedisconnect")]
        public static extern bool wiimotedisconnect();
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
        [DllImport("Kernel32.dll")]
        private static extern IntPtr CreateFile(string fileName, System.IO.FileAccess fileAccess, System.IO.FileShare fileShare, IntPtr securityAttributes, System.IO.FileMode creationDisposition, EFileAttributes flags, IntPtr template);
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern uint TimeBeginPeriod(uint ms);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        private static extern uint TimeEndPeriod(uint ms);
        [DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
        private static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
        private static uint CurrentResolution = 0;
        private const double REGISTER_IR = 0x04b00030, REGISTER_EXTENSION_INIT_1 = 0x04a400f0, REGISTER_EXTENSION_INIT_2 = 0x04a400fb, REGISTER_EXTENSION_TYPE = 0x04a400fa, REGISTER_EXTENSION_CALIBRATION = 0x04a40020, REGISTER_MOTIONPLUS_INIT = 0x04a600fe;
        public string path;
        public byte[] mBuff = new byte[22], aBuffer = new byte[22];
        private const byte Type = 0x12, IR = 0x13, WriteMemory = 0x16, ReadMemory = 0x16, IRExtensionAccel = 0x37;
        private static FileStream mStream;
        private static SafeFileHandle handle = null, handleunshared = null;
        public bool reconnectingwiimotebool;
        public double reconnectingwiimotecount;
        public bool running;
        public double irxc, iryc, irx0, iry0, irx1, iry1, irx2, iry2, irx3, iry3, irx, iry, tempirx, tempiry, WiimoteIRSensors0X, WiimoteIRSensors0Y, WiimoteIRSensors1X, WiimoteIRSensors1Y, WiimoteRawValuesX, WiimoteRawValuesY, WiimoteRawValuesZ, calibrationinit, WiimoteIRSensors0Xcam, WiimoteIRSensors0Ycam, WiimoteIRSensors1Xcam, WiimoteIRSensors1Ycam, WiimoteIRSensorsXcam, WiimoteIRSensorsYcam;
        public bool WiimoteIR0foundcam, WiimoteIR1foundcam, WiimoteIRswitch, WiimoteIR1found, WiimoteIR0found, WiimoteButtonStateA, WiimoteButtonStateB, WiimoteButtonStateMinus, WiimoteButtonStateHome, WiimoteButtonStatePlus, WiimoteButtonStateOne, WiimoteButtonStateTwo, WiimoteButtonStateUp, WiimoteButtonStateDown, WiimoteButtonStateLeft, WiimoteButtonStateRight, WiimoteNunchuckStateC, WiimoteNunchuckStateZ;
        public double WiimoteIR0notfound, stickviewxinit, stickviewyinit, WiimoteNunchuckStateRawValuesX, WiimoteNunchuckStateRawValuesY, WiimoteNunchuckStateRawValuesZ, WiimoteNunchuckStateRawJoystickX, WiimoteNunchuckStateRawJoystickY, centery = 80f;
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
        }
        public void taskD()
        {
            for (; ; )
            {
                if (!running)
                    break;
                Reconnection();
                try
                {
                    mStream.Read(aBuffer, 0, 22);
                    reconnectingwiimotebool = false;
                }
                catch { }
                ProcessStateLogic();
            }
        }
        public void Init()
        {
            calibrationinit = -aBuffer[4] + 135f;
            stickviewxinit = -aBuffer[16] + 125f;
            stickviewyinit = -aBuffer[17] + 125f;
        }
        public void ProcessStateLogic()
        {
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
        }
        public void Reconnection()
        {
            if (reconnectingwiimotecount == 0)
                reconnectingwiimotebool = true;
            reconnectingwiimotecount++;
            if (reconnectingwiimotecount >= 15f)
            {
                if (reconnectingwiimotebool)
                {
                    ReconnectionInit();
                    WiimoteFound(path);
                    reconnectingwiimotecount = -15f;
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
        public bool ScanWiimote()
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
                        WiimoteFound(path);
                        WiimoteFound(path);
                        WiimoteFound(path);
                        handleunshared = CreateFile(path, FileAccess.ReadWrite, FileShare.None, IntPtr.Zero, FileMode.Open, (uint)EFileAttributes.Overlapped, IntPtr.Zero);
                        return true;
                    }
                }
                index++;
            }
            return false;
        }
        public void WiimoteFound(string path)
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