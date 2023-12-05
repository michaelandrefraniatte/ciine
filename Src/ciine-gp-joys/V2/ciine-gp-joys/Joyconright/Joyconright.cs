using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vector3 = System.Numerics.Vector3;

namespace JoyconRightAPI
{
    public class JoyconRight
    {
        [DllImport("MotionInputPairing.dll", EntryPoint = "joyconrightconnect")]
        public static extern bool joyconrightconnect();
        [DllImport("MotionInputPairing.dll", EntryPoint = "joyconrightdisconnect")]
        public static extern bool joyconrightdisconnect();
        [DllImport("rhidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rhid_read_timeout")]
        public static extern int Rhid_read_timeout(SafeFileHandle dev, byte[] data, UIntPtr length);
        [DllImport("rhidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rhid_write")]
        public static extern int Rhid_write(SafeFileHandle device, byte[] data, UIntPtr length);
        [DllImport("rhidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rhid_open_path")]
        public static extern SafeFileHandle Rhid_open_path(IntPtr handle);
        [DllImport("rhidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Rhid_close")]
        public static extern void Rhid_close(SafeFileHandle device);
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
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern uint TimeBeginPeriod(uint ms);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        private static extern uint TimeEndPeriod(uint ms);
        [DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
        private static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
        private static uint CurrentResolution = 0;
        public const uint report_lenRight = 49;
        public byte[] report_bufRight = new byte[report_lenRight];
        public SafeFileHandle handleRight;
        public bool JoyconRightButtonSPA, JoyconRightButtonACC, JoyconRightRollLeft, JoyconRightRollRight;
        public double JoyconRightStickX, JoyconRightStickY;
        public System.Collections.Generic.List<double> RightValListX = new System.Collections.Generic.List<double>(), RightValListY = new System.Collections.Generic.List<double>();
        public bool JoyconRightAccelCenter, JoyconRightStickCenter;
        public double JoyconRightAccelX, JoyconRightAccelY, JoyconRightGyroX, JoyconRightGyroY;
        private double[] stickRight = { 0, 0 };
        private double[] stickCenterRight = { 0, 0 };
        private byte[] stick_rawRight = { 0, 0, 0 };
        public Vector3 acc_gRight = new Vector3();
        public Vector3 gyr_gRight = new Vector3();
        public Vector3 InitDirectAnglesRight, DirectAnglesRight;
        public bool JoyconRightButtonSHOULDER_1, JoyconRightButtonSHOULDER_2, JoyconRightButtonSR, JoyconRightButtonSL, JoyconRightButtonDPAD_DOWN, JoyconRightButtonDPAD_RIGHT, JoyconRightButtonDPAD_UP, JoyconRightButtonDPAD_LEFT, JoyconRightButtonPLUS, JoyconRightButtonSTICK, JoyconRightButtonHOME;
        public float acc_gcalibrationRightX, acc_gcalibrationRightY, acc_gcalibrationRightZ;
        public bool ISRIGHT = true;
        private bool running;
        public JoyconRight()
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
            running = true;
        }
        public void Close()
        {
            running = false;
            Thread.Sleep(100);
            Rhid_close(handleRight);
            handleRight.Close();
            handleRight.Dispose();
            joyconrightdisconnect();
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
        public void BeginAsyncPolling()
        {
            Task.Run(() => taskDRight());
        }
        public void InitRightJoycon()
        {
            try
            {
                stick_rawRight[0] = report_bufRight[6 + (!ISRIGHT ? 0 : 3)];
                stick_rawRight[1] = report_bufRight[7 + (!ISRIGHT ? 0 : 3)];
                stick_rawRight[2] = report_bufRight[8 + (!ISRIGHT ? 0 : 3)];
                stickCenterRight[0] = (UInt16)(stick_rawRight[0] | ((stick_rawRight[1] & 0xf) << 8));
                stickCenterRight[1] = (UInt16)((stick_rawRight[1] >> 4) | (stick_rawRight[2] << 4));
                acc_gcalibrationRightX = (Int16)(report_bufRight[13 + 0 * 12] | ((report_bufRight[14 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufRight[13 + 1 * 12] | ((report_bufRight[14 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufRight[13 + 2 * 12] | ((report_bufRight[14 + 2 * 12] << 8) & 0xff00));
                acc_gcalibrationRightY = (Int16)(report_bufRight[15 + 0 * 12] | ((report_bufRight[16 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufRight[15 + 1 * 12] | ((report_bufRight[16 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufRight[15 + 2 * 12] | ((report_bufRight[16 + 2 * 12] << 8) & 0xff00));
                acc_gcalibrationRightZ = (Int16)(report_bufRight[17 + 0 * 12] | ((report_bufRight[18 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufRight[17 + 1 * 12] | ((report_bufRight[18 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufRight[17 + 2 * 12] | ((report_bufRight[18 + 2 * 12] << 8) & 0xff00));
            }
            catch { }
        }
        public void ProcessButtonsRightJoycon()
        {
            try
            {
                stick_rawRight[0] = report_bufRight[6 + (!ISRIGHT ? 0 : 3)];
                stick_rawRight[1] = report_bufRight[7 + (!ISRIGHT ? 0 : 3)];
                stick_rawRight[2] = report_bufRight[8 + (!ISRIGHT ? 0 : 3)];
                stickRight[0] = ((UInt16)(stick_rawRight[0] | ((stick_rawRight[1] & 0xf) << 8)) - stickCenterRight[0]) / 1440f;
                stickRight[1] = ((UInt16)((stick_rawRight[1] >> 4) | (stick_rawRight[2] << 4)) - stickCenterRight[1]) / 1440f;
                JoyconRightStickX = -stickRight[0];
                JoyconRightStickY = -stickRight[1];
                acc_gRight.X = ((Int16)(report_bufRight[13 + 0 * 12] | ((report_bufRight[14 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufRight[13 + 1 * 12] | ((report_bufRight[14 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufRight[13 + 2 * 12] | ((report_bufRight[14 + 2 * 12] << 8) & 0xff00)) - acc_gcalibrationRightX) * (1.0f / 12000f);
                acc_gRight.Y = -((Int16)(report_bufRight[15 + 0 * 12] | ((report_bufRight[16 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufRight[15 + 1 * 12] | ((report_bufRight[16 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufRight[15 + 2 * 12] | ((report_bufRight[16 + 2 * 12] << 8) & 0xff00)) - acc_gcalibrationRightY) * (1.0f / 12000f);
                acc_gRight.Z = -((Int16)(report_bufRight[17 + 0 * 12] | ((report_bufRight[18 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufRight[17 + 1 * 12] | ((report_bufRight[18 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufRight[17 + 2 * 12] | ((report_bufRight[18 + 2 * 12] << 8) & 0xff00)) - acc_gcalibrationRightZ) * (1.0f / 12000f);
                JoyconRightButtonSHOULDER_1 = (report_bufRight[3 + (!ISRIGHT ? 2 : 0)] & 0x40) != 0;
                JoyconRightButtonSHOULDER_2 = (report_bufRight[3 + (!ISRIGHT ? 2 : 0)] & 0x80) != 0;
                JoyconRightButtonSR = (report_bufRight[3 + (!ISRIGHT ? 2 : 0)] & 0x10) != 0;
                JoyconRightButtonSL = (report_bufRight[3 + (!ISRIGHT ? 2 : 0)] & 0x20) != 0;
                JoyconRightButtonDPAD_DOWN = (report_bufRight[3 + (!ISRIGHT ? 2 : 0)] & (!ISRIGHT ? 0x01 : 0x04)) != 0;
                JoyconRightButtonDPAD_RIGHT = (report_bufRight[3 + (!ISRIGHT ? 2 : 0)] & (!ISRIGHT ? 0x04 : 0x08)) != 0;
                JoyconRightButtonDPAD_UP = (report_bufRight[3 + (!ISRIGHT ? 2 : 0)] & (!ISRIGHT ? 0x02 : 0x02)) != 0;
                JoyconRightButtonDPAD_LEFT = (report_bufRight[3 + (!ISRIGHT ? 2 : 0)] & (!ISRIGHT ? 0x08 : 0x01)) != 0;
                JoyconRightButtonPLUS = ((report_bufRight[4] & 0x02) != 0);
                JoyconRightButtonHOME = ((report_bufRight[4] & 0x10) != 0);
                JoyconRightButtonSTICK = ((report_bufRight[4] & (!ISRIGHT ? 0x08 : 0x04)) != 0);
                JoyconRightButtonACC = acc_gRight.X <= -1.13;
                JoyconRightButtonSPA = JoyconRightButtonSL | JoyconRightButtonSR | JoyconRightButtonPLUS | JoyconRightButtonACC;
                DirectAnglesRight = acc_gRight - InitDirectAnglesRight;
                JoyconRightAccelX = DirectAnglesRight.X * 1350f;
                JoyconRightAccelY = -DirectAnglesRight.Y * 1350f;
                gyr_gRight.X = ((Int16)(report_bufRight[19 + 0 * 12] | ((report_bufRight[20 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufRight[19 + 1 * 12] | ((report_bufRight[20 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufRight[19 + 2 * 12] | ((report_bufRight[20 + 2 * 12] << 8) & 0xff00)));
                gyr_gRight.Y = ((Int16)(report_bufRight[21 + 0 * 12] | ((report_bufRight[22 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufRight[21 + 1 * 12] | ((report_bufRight[22 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufRight[21 + 2 * 12] | ((report_bufRight[22 + 2 * 12] << 8) & 0xff00)));
                gyr_gRight.Z = ((Int16)(report_bufRight[23 + 0 * 12] | ((report_bufRight[24 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufRight[23 + 1 * 12] | ((report_bufRight[24 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufRight[23 + 2 * 12] | ((report_bufRight[24 + 2 * 12] << 8) & 0xff00)));
                JoyconRightGyroX = gyr_gRight.Z;
                JoyconRightGyroY = gyr_gRight.Y;
            }
            catch { }
        }
        public void InitRightJoyconAccel()
        {
            InitDirectAnglesRight = acc_gRight;
        }
        public void InitRightJoyconStick()
        {
            stick_rawRight[0] = report_bufRight[6 + (!ISRIGHT ? 0 : 3)];
            stick_rawRight[1] = report_bufRight[7 + (!ISRIGHT ? 0 : 3)];
            stick_rawRight[2] = report_bufRight[8 + (!ISRIGHT ? 0 : 3)];
            stickCenterRight[0] = (UInt16)(stick_rawRight[0] | ((stick_rawRight[1] & 0xf) << 8));
            stickCenterRight[1] = (UInt16)((stick_rawRight[1] >> 4) | (stick_rawRight[2] << 4));
        }
        public const string vendor_id = "57e", vendor_id_ = "057e", product_l = "2006", product_r = "2007";
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
        public bool ScanRight()
        {
            joyconrightconnect();
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
                    if ((diDetail.DevicePath.Contains(vendor_id) | diDetail.DevicePath.Contains(vendor_id_)) & diDetail.DevicePath.Contains(product_r))
                    {
                        AttachJoyRight(diDetail.DevicePath);
                        return true;
                    }
                }
                index++;
            }
            return false;
        }
        public void AttachJoyRight(string path)
        {
            do
            {
                IntPtr handle = CreateFile(path, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite, new System.IntPtr(), System.IO.FileMode.Open, EFileAttributes.Normal, new System.IntPtr());
                handleRight = Rhid_open_path(handle);
                SubcommandRight(0x40, new byte[] { 0x1 }, 1);
                SubcommandRight(0x3, new byte[] { 0x30 }, 1);
            }
            while (handleRight.IsInvalid);
        }
        public void SubcommandRight(byte sc, byte[] buf, uint len)
        {
            byte[] buf_Right = new byte[report_lenRight];
            System.Array.Copy(buf, 0, buf_Right, 11, len);
            buf_Right[10] = sc;
            buf_Right[1] = 0;
            buf_Right[0] = 0x1;
            Rhid_write(handleRight, buf_Right, (UIntPtr)(len + 11));
            Rhid_read_timeout(handleRight, buf_Right, (UIntPtr)report_lenRight);
        }
    }
}