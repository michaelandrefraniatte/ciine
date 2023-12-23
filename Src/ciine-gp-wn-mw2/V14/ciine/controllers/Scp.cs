﻿using Microsoft.Win32.SafeHandles;
using System.Globalization;
using System;
using System.Runtime.InteropServices;

namespace controllers
{
    public class Valuechanges
    {
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern uint TimeBeginPeriod(uint ms);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        private static extern uint TimeEndPeriod(uint ms);
        [DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
        private static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
        private static uint CurrentResolution = 0;
        public double[] _valuechange = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public double[] _ValueChange = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public Valuechanges()
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
        public Valuechanges ValueChange = new Valuechanges();
        private const string SCP_BUS_CLASS_GUID = "{F679F562-3164-42CE-A4DB-E7DDBE723909}";
        private SafeFileHandle _deviceHandle;
        int transferred = 0;
        byte[] outputBuffer = null;
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
            ValueChange[0] = back ? 1 : 0;
            if (ValueChange._ValueChange[0] > 0f)
                Buttons ^= X360Buttons.Back;
            if (ValueChange._ValueChange[0] < 0f)
                Buttons &= ~X360Buttons.Back;
            ValueChange[1] = start ? 1 : 0;
            if (ValueChange._ValueChange[1] > 0f)
                Buttons ^= X360Buttons.Start;
            if (ValueChange._ValueChange[1] < 0f)
                Buttons &= ~X360Buttons.Start;
            ValueChange[2] = A ? 1 : 0;
            if (ValueChange._ValueChange[2] > 0f)
                Buttons ^= X360Buttons.A;
            if (ValueChange._ValueChange[2] < 0f)
                Buttons &= ~X360Buttons.A;
            ValueChange[3] = B ? 1 : 0;
            if (ValueChange._ValueChange[3] > 0f)
                Buttons ^= X360Buttons.B;
            if (ValueChange._ValueChange[3] < 0f)
                Buttons &= ~X360Buttons.B;
            ValueChange[4] = X ? 1 : 0;
            if (ValueChange._ValueChange[4] > 0f)
                Buttons ^= X360Buttons.X;
            if (ValueChange._ValueChange[4] < 0f)
                Buttons &= ~X360Buttons.X;
            ValueChange[5] = Y ? 1 : 0;
            if (ValueChange._ValueChange[5] > 0f)
                Buttons ^= X360Buttons.Y;
            if (ValueChange._ValueChange[5] < 0f)
                Buttons &= ~X360Buttons.Y;
            ValueChange[6] = up ? 1 : 0;
            if (ValueChange._ValueChange[6] > 0f)
                Buttons ^= X360Buttons.Up;
            if (ValueChange._ValueChange[6] < 0f)
                Buttons &= ~X360Buttons.Up;
            ValueChange[7] = left ? 1 : 0;
            if (ValueChange._ValueChange[7] > 0f)
                Buttons ^= X360Buttons.Left;
            if (ValueChange._ValueChange[7] < 0f)
                Buttons &= ~X360Buttons.Left;
            ValueChange[8] = down ? 1 : 0;
            if (ValueChange._ValueChange[8] > 0f)
                Buttons ^= X360Buttons.Down;
            if (ValueChange._ValueChange[8] < 0f)
                Buttons &= ~X360Buttons.Down;
            ValueChange[9] = right ? 1 : 0;
            if (ValueChange._ValueChange[9] > 0f)
                Buttons ^= X360Buttons.Right;
            if (ValueChange._ValueChange[9] < 0f)
                Buttons &= ~X360Buttons.Right;
            ValueChange[10] = leftstick ? 1 : 0;
            if (ValueChange._ValueChange[10] > 0f)
                Buttons ^= X360Buttons.LeftStick;
            if (ValueChange._ValueChange[10] < 0f)
                Buttons &= ~X360Buttons.LeftStick;
            ValueChange[11] = rightstick ? 1 : 0;
            if (ValueChange._ValueChange[11] > 0f)
                Buttons ^= X360Buttons.RightStick;
            if (ValueChange._ValueChange[11] < 0f)
                Buttons &= ~X360Buttons.RightStick;
            ValueChange[12] = leftbumper ? 1 : 0;
            if (ValueChange._ValueChange[12] > 0f)
                Buttons ^= X360Buttons.LeftBumper;
            if (ValueChange._ValueChange[12] < 0f)
                Buttons &= ~X360Buttons.LeftBumper;
            ValueChange[13] = rightbumper ? 1 : 0;
            if (ValueChange._ValueChange[13] > 0f)
                Buttons ^= X360Buttons.RightBumper;
            if (ValueChange._ValueChange[13] < 0f)
                Buttons &= ~X360Buttons.RightBumper;
            ValueChange[14] = xbox ? 1 : 0;
            if (ValueChange._ValueChange[14] > 0f)
                Buttons ^= X360Buttons.Logo;
            if (ValueChange._ValueChange[14] < 0f)
                Buttons &= ~X360Buttons.Logo;
            LeftStickX = (short)leftstickx;
            LeftStickY = (short)leftsticky;
            RightStickX = (short)rightstickx;
            RightStickY = (short)rightsticky;
            LeftTrigger = (byte)lefttriggerposition;
            RightTrigger = (byte)righttriggerposition;
            Report(number < 2 ? 1 : 2, GetReport());
        }
        public XBoxController()
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
        }
        public bool PlugIn(int controllerNumber)
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
        public bool Unplug(int controllerNumber)
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
        public bool Report(int controllerNumber, byte[] controllerReport)
        {
            byte[] head = new byte[8];
            head[0] = 0x1C;
            head[4] = (byte)((controllerNumber) & 0xFF);
            head[5] = (byte)((controllerNumber >> 8) & 0xFF);
            head[6] = (byte)((controllerNumber >> 16) & 0xFF);
            head[7] = (byte)((controllerNumber >> 24) & 0xFF);
            byte[] fullReport = new byte[28];
            Buffer.BlockCopy(head, 0, fullReport, 0, head.Length);
            Buffer.BlockCopy(controllerReport, 0, fullReport, head.Length, controllerReport.Length);
            return DeviceIoControl(_deviceHandle, 0x2A400C, fullReport, fullReport.Length, outputBuffer, outputBuffer?.Length ?? 0, ref transferred, IntPtr.Zero) && transferred > 0;
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
        public X360Buttons Buttons { get; set; }
        public byte LeftTrigger { get; set; }
        public byte RightTrigger { get; set; }
        public short LeftStickX { get; set; }
        public short LeftStickY { get; set; }
        public short RightStickX { get; set; }
        public short RightStickY { get; set; }
        public byte[] GetReport()
        {
            byte[] bytes = new byte[20];
            bytes[0] = 0x00;                                 // Message type (input report)
            bytes[1] = 0x14;                                 // Message size (20 bytes)
            bytes[2] = (byte)((ushort)Buttons & 0xFF);       // Buttons low
            bytes[3] = (byte)((ushort)Buttons >> 8 & 0xFF);  // Buttons high
            bytes[4] = LeftTrigger;                          // Left trigger
            bytes[5] = RightTrigger;                         // Right trigger
            bytes[6] = (byte)(LeftStickX & 0xFF);            // Left stick X-axis low
            bytes[7] = (byte)(LeftStickX >> 8 & 0xFF);       // Left stick X-axis high
            bytes[8] = (byte)(LeftStickY & 0xFF);            // Left stick Y-axis low
            bytes[9] = (byte)(LeftStickY >> 8 & 0xFF);       // Left stick Y-axis high
            bytes[10] = (byte)(RightStickX & 0xFF);          // Right stick X-axis low
            bytes[11] = (byte)(RightStickX >> 8 & 0xFF);     // Right stick X-axis high
            bytes[12] = (byte)(RightStickY & 0xFF);          // Right stick Y-axis low
            bytes[13] = (byte)(RightStickY >> 8 & 0xFF);     // Right stick Y-axis high
            return bytes;
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
        public enum X360Buttons
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