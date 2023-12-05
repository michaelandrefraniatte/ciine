using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JoyconChargingGripAPI
{
    public class JoyconChargingGrip
    {
        [DllImport("hid.dll")]
        public static extern void HidD_GetHidGuid(out Guid gHid);
        [DllImport("hid.dll")]
        public extern static bool HidD_SetOutputReport(IntPtr HidDeviceObject, byte[] lpReportBuffer, uint ReportBufferLength);
        [DllImport("setupapi.dll")]
        public static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, string Enumerator, IntPtr hwndParent, UInt32 Flags);
        [DllImport("setupapi.dll")]
        public static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInvo, ref Guid interfaceClassGuid, Int32 memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);
        [DllImport("setupapi.dll")]
        public static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, UInt32 deviceInterfaceDetailDataSize, out UInt32 requiredSize, IntPtr deviceInfoData);
        [DllImport("setupapi.dll")]
        public static extern Boolean SetupDiGetDeviceInterfaceDetail(IntPtr hDevInfo, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, ref SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData, UInt32 deviceInterfaceDetailDataSize, out UInt32 requiredSize, IntPtr deviceInfoData);
        [DllImport("Kernel32.dll")]
        public static extern SafeFileHandle CreateFile(string fileName, [MarshalAs(UnmanagedType.U4)] FileAccess fileAccess, [MarshalAs(UnmanagedType.U4)] FileShare fileShare, IntPtr securityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition, [MarshalAs(UnmanagedType.U4)] uint flags, IntPtr template);
        [DllImport("Kernel32.dll")]
        public static extern IntPtr CreateFile(string fileName, System.IO.FileAccess fileAccess, System.IO.FileShare fileShare, IntPtr securityAttributes, System.IO.FileMode creationDisposition, EFileAttributes flags, IntPtr template);
        [DllImport("lhidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Lhid_read_timeout")]
        public static extern int Lhid_read_timeout(SafeFileHandle dev, byte[] data, UIntPtr length);
        [DllImport("lhidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Lhid_write")]
        public static extern int Lhid_write(SafeFileHandle device, byte[] data, UIntPtr length);
        [DllImport("lhidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Lhid_open_path")]
        public static extern SafeFileHandle Lhid_open_path(IntPtr handle);
        [DllImport("lhidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Lhid_close")]
        public static extern void Lhid_close(SafeFileHandle device);
        [DllImport("rhidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rhid_read_timeout")]
        public static extern int Rhid_read_timeout(SafeFileHandle dev, byte[] data, UIntPtr length);
        [DllImport("rhidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rhid_write")]
        public static extern int Rhid_write(SafeFileHandle device, byte[] data, UIntPtr length);
        [DllImport("rhidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rhid_open_path")]
        public static extern SafeFileHandle Rhid_open_path(IntPtr handle);
        [DllImport("rhidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rhid_close")]
        public static extern void Rhid_close(SafeFileHandle device);
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern uint TimeBeginPeriod(uint ms);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        private static extern uint TimeEndPeriod(uint ms);
        [DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
        private static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
        private static uint CurrentResolution = 0;
        public const uint report_lenLeft = 49;
        public byte[] report_bufLeft = new byte[report_lenLeft];
        public const uint report_lenRight = 49;
        public byte[] report_bufRight = new byte[report_lenRight];
        public SafeFileHandle handleRight;
        public SafeFileHandle handleLeft;
        public bool ISLEFT, ISRIGHT, running;
        public JoyconChargingGrip()
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
            running = true;
        }
        public void Close()
        {
            running = false;
            Thread.Sleep(100);
            Subcommand3GripLeftController(0x06, new byte[] { 0x01 }, 1);
            Subcommand3GripRightController(0x06, new byte[] { 0x01 }, 1);
            Lhid_close(handleLeft);
            handleLeft.Close();
            handleLeft.Dispose();
            Rhid_close(handleRight);
            handleRight.Close();
            handleRight.Dispose();
        }
        private void taskDLeft()
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
        private void taskDRight()
        {
            while (running)
            {
                try
                {
                    Rhid_read_timeout(handleRight, report_bufRight, (UIntPtr)report_lenRight);
                }
                catch { }
            }
        }
        public void BeginAsyncPollingLeft()
        {
            Task.Run(() => taskDLeft());
        }
        public void BeginAsyncPollingRight()
        {
            Task.Run(() => taskDRight());
        }
        public const string vendor_id = "57e", vendor_id_ = "057e", product_grip = "200e";
        public enum EFileAttributes : uint
        {
            Overlapped = 0x40000000,
            Normal = 0x80
        };
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid InterfaceClassGuid;
            public int Flags;
            public IntPtr RESERVED;
        }
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public UInt32 cbSize;
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }
        public bool ScanGrip()
        {
            ISLEFT = false;
            ISRIGHT = false;
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
                    if ((diDetail.DevicePath.Contains(vendor_id) | diDetail.DevicePath.Contains(vendor_id_)) & diDetail.DevicePath.Contains(product_grip))
                    {
                        if (ISLEFT)
                        {
                            AttachGripRightController(diDetail.DevicePath);
                            ISRIGHT = true;
                        }
                        if (!ISLEFT)
                        {
                            AttachGripLeftController(diDetail.DevicePath);
                            ISLEFT = true;
                        }
                        if (ISLEFT & ISRIGHT)
                            return true;
                    }
                }
                index++;
            }
            return false;
        }
        private void AttachGripLeftController(string path)
        {
            do
            {
                IntPtr handle = CreateFile(path, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite, new System.IntPtr(), System.IO.FileMode.Open, EFileAttributes.Normal, new System.IntPtr());
                handleLeft = Lhid_open_path(handle);
                Subcommand1GripLeftController(0x06, new byte[] { 0x01 }, 1);
                Subcommand2GripLeftController(0x40, new byte[] { 0x1 }, 1);
                Subcommand2GripLeftController(0x3, new byte[] { 0x30 }, 1);
            }
            while (handleLeft.IsInvalid);
        }
        private void Subcommand1GripLeftController(byte sc, byte[] buf, uint len)
        {
            byte[] buf_Left = new byte[report_lenLeft];
            System.Array.Copy(buf, 0, buf_Left, 11, len);
            buf_Left[10] = sc;
            buf_Left[1] = 0x2;
            buf_Left[0] = 0x80;
            Lhid_write(handleLeft, buf_Left, new UIntPtr(2));
            Lhid_read_timeout(handleLeft, buf_Left, (UIntPtr)report_lenLeft);
            buf_Left[1] = 0x3;
            buf_Left[0] = 0x80;
            Lhid_write(handleLeft, buf_Left, new UIntPtr(2));
            Lhid_read_timeout(handleLeft, buf_Left, (UIntPtr)report_lenLeft);
            buf_Left[1] = 0x2;
            buf_Left[0] = 0x80;
            Lhid_write(handleLeft, buf_Left, new UIntPtr(2));
            Lhid_read_timeout(handleLeft, buf_Left, (UIntPtr)report_lenLeft);
            buf_Left[1] = 0x4;
            buf_Left[0] = 0x80;
            Lhid_write(handleLeft, buf_Left, new UIntPtr(2));
            Lhid_read_timeout(handleLeft, buf_Left, (UIntPtr)report_lenLeft);
        }
        private void Subcommand2GripLeftController(byte sc, byte[] buf, uint len)
        {
            byte[] buf_Left = new byte[report_lenLeft];
            System.Array.Copy(buf, 0, buf_Left, 11, len);
            buf_Left[10] = sc;
            buf_Left[1] = 0;
            buf_Left[0] = 0x1;
            Lhid_write(handleLeft, buf_Left, (UIntPtr)(len + 11));
            Lhid_read_timeout(handleLeft, buf_Left, (UIntPtr)report_lenLeft);
        }
        private void Subcommand3GripLeftController(byte sc, byte[] buf, uint len)
        {
            byte[] buf_Left = new byte[report_lenLeft];
            System.Array.Copy(buf, 0, buf_Left, 11, len);
            buf_Left[10] = sc;
            buf_Left[1] = 0x5;
            buf_Left[0] = 0x80;
            Lhid_write(handleLeft, buf_Left, new UIntPtr(2));
            buf_Left[1] = 0x6;
            buf_Left[0] = 0x80;
            Lhid_write(handleLeft, buf_Left, new UIntPtr(2));
        }
        private void AttachGripRightController(string path)
        {
            do
            {
                IntPtr handle = CreateFile(path, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite, new System.IntPtr(), System.IO.FileMode.Open, EFileAttributes.Normal, new System.IntPtr());
                handleRight = Rhid_open_path(handle);
                Subcommand1GripRightController(0x06, new byte[] { 0x01 }, 1);
                Subcommand2GripRightController(0x40, new byte[] { 0x1 }, 1);
                Subcommand2GripRightController(0x3, new byte[] { 0x30 }, 1);
            }
            while (handleRight.IsInvalid);
        }
        private void Subcommand1GripRightController(byte sc, byte[] buf, uint len)
        {
            byte[] buf_Right = new byte[report_lenRight];
            System.Array.Copy(buf, 0, buf_Right, 11, len);
            buf_Right[10] = sc;
            buf_Right[1] = 0x2;
            buf_Right[0] = 0x80;
            Rhid_write(handleRight, buf_Right, new UIntPtr(2));
            Rhid_read_timeout(handleRight, buf_Right, (UIntPtr)report_lenRight);
            buf_Right[1] = 0x3;
            buf_Right[0] = 0x80;
            Rhid_write(handleRight, buf_Right, new UIntPtr(2));
            Rhid_read_timeout(handleRight, buf_Right, (UIntPtr)report_lenRight);
            buf_Right[1] = 0x2;
            buf_Right[0] = 0x80;
            Rhid_write(handleRight, buf_Right, new UIntPtr(2));
            Rhid_read_timeout(handleRight, buf_Right, (UIntPtr)report_lenRight);
            buf_Right[1] = 0x4;
            buf_Right[0] = 0x80;
            Rhid_write(handleRight, buf_Right, new UIntPtr(2));
            Rhid_read_timeout(handleRight, buf_Right, (UIntPtr)report_lenRight);
        }
        private void Subcommand2GripRightController(byte sc, byte[] buf, uint len)
        {
            byte[] buf_Right = new byte[report_lenRight];
            System.Array.Copy(buf, 0, buf_Right, 11, len);
            buf_Right[10] = sc;
            buf_Right[1] = 0;
            buf_Right[0] = 0x1;
            Rhid_write(handleRight, buf_Right, (UIntPtr)(len + 11));
            Rhid_read_timeout(handleRight, buf_Right, (UIntPtr)report_lenRight);
        }
        private void Subcommand3GripRightController(byte sc, byte[] buf, uint len)
        {
            byte[] buf_Right = new byte[report_lenRight];
            System.Array.Copy(buf, 0, buf_Right, 11, len);
            buf_Right[10] = sc;
            buf_Right[1] = 0x5;
            buf_Right[0] = 0x80;
            Rhid_write(handleRight, buf_Right, new UIntPtr(2));
            buf_Right[1] = 0x6;
            buf_Right[0] = 0x80;
            Rhid_write(handleRight, buf_Right, new UIntPtr(2));
        }
    }
}