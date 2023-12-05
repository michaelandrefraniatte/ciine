﻿using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Vector3 = System.Numerics.Vector3;

namespace SwitchProControllerAPI
{
    public class SwitchProController
    {
        [DllImport("hid.dll")]
        public static extern void HidD_GetHidGuid(out Guid gHid);
        [DllImport("hid.dll")]
        public static extern bool HidD_SetOutputReport(IntPtr HidDeviceObject, byte[] lpReportBuffer, uint ReportBufferLength);
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
        [DllImport("prohidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Prohid_read_timeout")]
        public static extern int Prohid_read_timeout(SafeFileHandle dev, byte[] data, UIntPtr length);
        [DllImport("prohidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Prohid_write")]
        public static extern int Prohid_write(SafeFileHandle device, byte[] data, UIntPtr length);
        [DllImport("prohidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Prohid_open_path")]
        public static extern SafeFileHandle Prohid_open_path(IntPtr handle);
        [DllImport("prohidread.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Prohid_close")]
        public static extern void Prohid_close(SafeFileHandle device);
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern uint TimeBeginPeriod(uint ms);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        private static extern uint TimeEndPeriod(uint ms);
        [DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
        private static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
        private static uint CurrentResolution = 0;
        public const uint report_lenPro = 49;
        public byte[] report_bufPro = new byte[report_lenPro];
        public SafeFileHandle handlePro; 
        public bool ISPRO = true;
        public bool ProControllerButtonACC, ProControllerRollLeft, ProControllerRollRight;
        public double ProControllerLeftStickX, ProControllerLeftStickY, ProControllerRightStickX, ProControllerRightStickY;
        public System.Collections.Generic.List<double> ProValListX = new System.Collections.Generic.List<double>(), ProValListY = new System.Collections.Generic.List<double>();
        public bool ProControllerAccelCenter, ProControllerStickCenter;
        public double ProControllerAccelX, ProControllerAccelY, ProControllerGyroX, ProControllerGyroY;
        private double[] stickleftPro = { 0, 0 };
        private double[] stickCenterleftPro = { 0, 0 };
        private byte[] stick_rawleftPro = { 0, 0, 0 };
        private double[] stickrightPro = { 0, 0 };
        private double[] stickCenterrightPro = { 0, 0 };
        private byte[] stick_rawrightPro = { 0, 0, 0 };
        public Vector3 acc_gPro = new Vector3();
        public Vector3 gyr_gPro = new Vector3();
        public Vector3 InitDirectAnglesPro, DirectAnglesPro;
        public bool ProControllerButtonSHOULDER_Left_1, ProControllerButtonSHOULDER_Left_2, ProControllerButtonSHOULDER_Right_1, ProControllerButtonSHOULDER_Right_2, ProControllerButtonDPAD_DOWN, ProControllerButtonDPAD_RIGHT, ProControllerButtonDPAD_UP, ProControllerButtonDPAD_LEFT, ProControllerButtonA, ProControllerButtonB, ProControllerButtonX, ProControllerButtonY, ProControllerButtonMINUS, ProControllerButtonPLUS, ProControllerButtonSTICK_Left, ProControllerButtonSTICK_Right, ProControllerButtonCAPTURE, ProControllerButtonHOME;
        public float acc_gcalibrationProX, acc_gcalibrationProY, acc_gcalibrationProZ;
        public bool running;
        public SwitchProController()
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
            running = true;
        }
        public void Close()
        {
            running = false;
            Thread.Sleep(100);
            Subcommand3ProController(0x06, new byte[] { 0x01 }, 1);
            Prohid_close(handlePro);
            handlePro.Close();
            handlePro.Dispose();
        }
        private void taskDPro()
        {
            while (running)
            {
                try
                {
                    Prohid_read_timeout(handlePro, report_bufPro, (UIntPtr)report_lenPro);
                }
                catch { }
            }
        }
        public void BeginAsyncPolling()
        {
            Task.Run(() => taskDPro());
        }
        public void InitProController()
        {
            try
            {
                stick_rawleftPro[0] = report_bufPro[6 + (ISPRO ? 0 : 3)];
                stick_rawleftPro[1] = report_bufPro[7 + (ISPRO ? 0 : 3)];
                stick_rawleftPro[2] = report_bufPro[8 + (ISPRO ? 0 : 3)];
                stickCenterleftPro[0] = (UInt16)(stick_rawleftPro[0] | ((stick_rawleftPro[1] & 0xf) << 8));
                stickCenterleftPro[1] = (UInt16)((stick_rawleftPro[1] >> 4) | (stick_rawleftPro[2] << 4));
                stick_rawrightPro[0] = report_bufPro[6 + (!ISPRO ? 0 : 3)];
                stick_rawrightPro[1] = report_bufPro[7 + (!ISPRO ? 0 : 3)];
                stick_rawrightPro[2] = report_bufPro[8 + (!ISPRO ? 0 : 3)];
                stickCenterrightPro[0] = (UInt16)(stick_rawrightPro[0] | ((stick_rawrightPro[1] & 0xf) << 8));
                stickCenterrightPro[1] = (UInt16)((stick_rawrightPro[1] >> 4) | (stick_rawrightPro[2] << 4));
                acc_gcalibrationProX = (Int16)(report_bufPro[13 + 0 * 12] | ((report_bufPro[14 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufPro[13 + 1 * 12] | ((report_bufPro[14 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufPro[13 + 2 * 12] | ((report_bufPro[14 + 2 * 12] << 8) & 0xff00));
                acc_gcalibrationProY = (Int16)(report_bufPro[15 + 0 * 12] | ((report_bufPro[16 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufPro[15 + 1 * 12] | ((report_bufPro[16 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufPro[15 + 2 * 12] | ((report_bufPro[16 + 2 * 12] << 8) & 0xff00));
                acc_gcalibrationProZ = (Int16)(report_bufPro[17 + 0 * 12] | ((report_bufPro[18 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufPro[17 + 1 * 12] | ((report_bufPro[18 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufPro[17 + 2 * 12] | ((report_bufPro[18 + 2 * 12] << 8) & 0xff00));
            }
            catch { }
        }
        public void ProcessButtonsAndSticksPro()
        {
            try
            {
                stick_rawleftPro[0] = report_bufPro[6 + (ISPRO ? 0 : 3)];
                stick_rawleftPro[1] = report_bufPro[7 + (ISPRO ? 0 : 3)];
                stick_rawleftPro[2] = report_bufPro[8 + (ISPRO ? 0 : 3)];
                stickleftPro[0] = ((UInt16)(stick_rawleftPro[0] | ((stick_rawleftPro[1] & 0xf) << 8)) - stickCenterleftPro[0]) / 1440f;
                stickleftPro[1] = ((UInt16)((stick_rawleftPro[1] >> 4) | (stick_rawleftPro[2] << 4)) - stickCenterleftPro[1]) / 1440f;
                ProControllerLeftStickX = stickleftPro[0];
                ProControllerLeftStickY = stickleftPro[1];
                stick_rawrightPro[0] = report_bufPro[6 + (!ISPRO ? 0 : 3)];
                stick_rawrightPro[1] = report_bufPro[7 + (!ISPRO ? 0 : 3)];
                stick_rawrightPro[2] = report_bufPro[8 + (!ISPRO ? 0 : 3)];
                stickrightPro[0] = ((UInt16)(stick_rawrightPro[0] | ((stick_rawrightPro[1] & 0xf) << 8)) - stickCenterrightPro[0]) / 1440f;
                stickrightPro[1] = ((UInt16)((stick_rawrightPro[1] >> 4) | (stick_rawrightPro[2] << 4)) - stickCenterrightPro[1]) / 1440f;
                ProControllerRightStickX = -stickrightPro[0];
                ProControllerRightStickY = -stickrightPro[1];
                acc_gPro.X = ((Int16)(report_bufPro[13 + 0 * 12] | ((report_bufPro[14 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufPro[13 + 1 * 12] | ((report_bufPro[14 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufPro[13 + 2 * 12] | ((report_bufPro[14 + 2 * 12] << 8) & 0xff00)) - acc_gcalibrationProX) * (1.0f / 12000f);
                acc_gPro.Y = -((Int16)(report_bufPro[15 + 0 * 12] | ((report_bufPro[16 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufPro[15 + 1 * 12] | ((report_bufPro[16 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufPro[15 + 2 * 12] | ((report_bufPro[16 + 2 * 12] << 8) & 0xff00)) - acc_gcalibrationProY) * (1.0f / 12000f);
                acc_gPro.Z = -((Int16)(report_bufPro[17 + 0 * 12] | ((report_bufPro[18 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufPro[17 + 1 * 12] | ((report_bufPro[18 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufPro[17 + 2 * 12] | ((report_bufPro[18 + 2 * 12] << 8) & 0xff00)) - acc_gcalibrationProZ) * (1.0f / 12000f);
                ProControllerButtonSHOULDER_Left_1 = (report_bufPro[3 + (ISPRO ? 2 : 0)] & 0x40) != 0;
                ProControllerButtonSHOULDER_Left_2 = (report_bufPro[3 + (ISPRO ? 2 : 0)] & 0x80) != 0;
                ProControllerButtonDPAD_DOWN = (report_bufPro[3 + (ISPRO ? 2 : 0)] & (ISPRO ? 0x01 : 0x04)) != 0;
                ProControllerButtonDPAD_RIGHT = (report_bufPro[3 + (ISPRO ? 2 : 0)] & (ISPRO ? 0x04 : 0x08)) != 0;
                ProControllerButtonDPAD_UP = (report_bufPro[3 + (ISPRO ? 2 : 0)] & (ISPRO ? 0x02 : 0x02)) != 0;
                ProControllerButtonDPAD_LEFT = (report_bufPro[3 + (ISPRO ? 2 : 0)] & (ISPRO ? 0x08 : 0x01)) != 0;
                ProControllerButtonMINUS = (report_bufPro[4] & 0x01) != 0;
                ProControllerButtonCAPTURE = (report_bufPro[4] & 0x20) != 0;
                ProControllerButtonSTICK_Left = (report_bufPro[4] & (ISPRO ? 0x08 : 0x04)) != 0;
                ProControllerButtonACC = acc_gPro.X <= -1.13;
                ProControllerButtonSHOULDER_Right_1 = (report_bufPro[3 + (!ISPRO ? 2 : 0)] & 0x40) != 0;
                ProControllerButtonSHOULDER_Right_2 = (report_bufPro[3 + (!ISPRO ? 2 : 0)] & 0x80) != 0;
                ProControllerButtonA = (report_bufPro[3 + (!ISPRO ? 2 : 0)] & (!ISPRO ? 0x04 : 0x08)) != 0;
                ProControllerButtonB = (report_bufPro[3 + (!ISPRO ? 2 : 0)] & (!ISPRO ? 0x01 : 0x04)) != 0;
                ProControllerButtonX = (report_bufPro[3 + (!ISPRO ? 2 : 0)] & (!ISPRO ? 0x02 : 0x02)) != 0;
                ProControllerButtonY = (report_bufPro[3 + (!ISPRO ? 2 : 0)] & (!ISPRO ? 0x08 : 0x01)) != 0;
                ProControllerButtonPLUS = (report_bufPro[4] & 0x02) != 0;
                ProControllerButtonHOME = (report_bufPro[4] & 0x10) != 0;
                ProControllerButtonSTICK_Right = ((report_bufPro[4] & (!ISPRO ? 0x08 : 0x04)) != 0);
                if (ProValListY.Count >= 50)
                {
                    ProValListY.RemoveAt(0);
                    ProValListY.Add(acc_gPro.Y);
                }
                else
                    ProValListY.Add(acc_gPro.Y);
                DirectAnglesPro = acc_gPro - InitDirectAnglesPro;
                ProControllerAccelX = DirectAnglesPro.X * 1350f;
                ProControllerAccelY = -DirectAnglesPro.Y * 1350f;
                gyr_gPro.X = ((Int16)(report_bufPro[19 + 0 * 12] | ((report_bufPro[20 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufPro[19 + 1 * 12] | ((report_bufPro[20 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufPro[19 + 2 * 12] | ((report_bufPro[20 + 2 * 12] << 8) & 0xff00)));
                gyr_gPro.Y = ((Int16)(report_bufPro[21 + 0 * 12] | ((report_bufPro[22 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufPro[21 + 1 * 12] | ((report_bufPro[22 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufPro[21 + 2 * 12] | ((report_bufPro[22 + 2 * 12] << 8) & 0xff00)));
                gyr_gPro.Z = ((Int16)(report_bufPro[23 + 0 * 12] | ((report_bufPro[24 + 0 * 12] << 8) & 0xff00)) + (Int16)(report_bufPro[23 + 1 * 12] | ((report_bufPro[24 + 1 * 12] << 8) & 0xff00)) + (Int16)(report_bufPro[23 + 2 * 12] | ((report_bufPro[24 + 2 * 12] << 8) & 0xff00)));
                ProControllerGyroX = gyr_gPro.Z;
                ProControllerGyroY = gyr_gPro.Y;
            }
            catch { }
        }
        public void InitProControllerAccel()
        {
            InitDirectAnglesPro = acc_gPro;
        }
        public void InitProControllerStick()
        {
            stick_rawleftPro[0] = report_bufPro[6 + (ISPRO ? 0 : 3)];
            stick_rawleftPro[1] = report_bufPro[7 + (ISPRO ? 0 : 3)];
            stick_rawleftPro[2] = report_bufPro[8 + (ISPRO ? 0 : 3)];
            stickCenterleftPro[0] = (UInt16)(stick_rawleftPro[0] | ((stick_rawleftPro[1] & 0xf) << 8));
            stickCenterleftPro[1] = (UInt16)((stick_rawleftPro[1] >> 4) | (stick_rawleftPro[2] << 4));
            stick_rawrightPro[0] = report_bufPro[6 + (!ISPRO ? 0 : 3)];
            stick_rawrightPro[1] = report_bufPro[7 + (!ISPRO ? 0 : 3)];
            stick_rawrightPro[2] = report_bufPro[8 + (!ISPRO ? 0 : 3)];
            stickCenterrightPro[0] = (UInt16)(stick_rawrightPro[0] | ((stick_rawrightPro[1] & 0xf) << 8));
            stickCenterrightPro[1] = (UInt16)((stick_rawrightPro[1] >> 4) | (stick_rawrightPro[2] << 4));
        }
        public const string vendor_id = "57e", vendor_id_ = "057e", product_pro = "2009";
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
        public bool ScanPro()
        {
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
                    if ((diDetail.DevicePath.Contains(vendor_id) | diDetail.DevicePath.Contains(vendor_id_)) & diDetail.DevicePath.Contains(product_pro))
                    {
                        AttachProController(diDetail.DevicePath);
                        return true;
                    }
                }
                index++;
            }
            return false;
        }
        private void AttachProController(string path)
        {
            do
            {
                IntPtr handle = CreateFile(path, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite, new System.IntPtr(), System.IO.FileMode.Open, EFileAttributes.Normal, new System.IntPtr());
                handlePro = Prohid_open_path(handle);
                Subcommand1ProController(0x06, new byte[] { 0x01 }, 1);
                Subcommand2ProController(0x40, new byte[] { 0x1 }, 1);
                Subcommand2ProController(0x3, new byte[] { 0x30 }, 1);
                Subcommand2ProController(0x40, new byte[] { 0x1 }, 1);
                Subcommand2ProController(0x3, new byte[] { 0x30 }, 1);
            }
            while (handlePro.IsInvalid);
        }
        private void Subcommand1ProController(byte sc, byte[] buf, uint len)
        {
            byte[] buf_Pro = new byte[report_lenPro];
            System.Array.Copy(buf, 0, buf_Pro, 11, len);
            buf_Pro[10] = sc;
            buf_Pro[1] = 0x2;
            buf_Pro[0] = 0x80;
            Prohid_write(handlePro, buf_Pro, new UIntPtr(2));
            Prohid_read_timeout(handlePro, buf_Pro, (UIntPtr)report_lenPro);
            buf_Pro[1] = 0x3;
            buf_Pro[0] = 0x80;
            Prohid_write(handlePro, buf_Pro, new UIntPtr(2));
            Prohid_read_timeout(handlePro, buf_Pro, (UIntPtr)report_lenPro);
            buf_Pro[1] = 0x2;
            buf_Pro[0] = 0x80;
            Prohid_write(handlePro, buf_Pro, new UIntPtr(2));
            Prohid_read_timeout(handlePro, buf_Pro, (UIntPtr)report_lenPro);
            buf_Pro[1] = 0x4;
            buf_Pro[0] = 0x80;
            Prohid_write(handlePro, buf_Pro, new UIntPtr(2));
            Prohid_read_timeout(handlePro, buf_Pro, (UIntPtr)report_lenPro);
        }
        private void Subcommand2ProController(byte sc, byte[] buf, uint len)
        {
            byte[] buf_Pro = new byte[report_lenPro];
            System.Array.Copy(buf, 0, buf_Pro, 11, len);
            buf_Pro[10] = sc;
            buf_Pro[1] = 0;
            buf_Pro[0] = 0x1;
            Prohid_write(handlePro, buf_Pro, (UIntPtr)(len + 11));
            Prohid_read_timeout(handlePro, buf_Pro, (UIntPtr)report_lenPro);
        }
        private void Subcommand3ProController(byte sc, byte[] buf, uint len)
        {
            byte[] buf_Pro = new byte[report_lenPro];
            System.Array.Copy(buf, 0, buf_Pro, 11, len);
            buf_Pro[10] = sc;
            buf_Pro[1] = 0x5;
            buf_Pro[0] = 0x80;
            Prohid_write(handlePro, buf_Pro, new UIntPtr(2));
            buf_Pro[1] = 0x6;
            buf_Pro[0] = 0x80;
            Prohid_write(handlePro, buf_Pro, new UIntPtr(2));
        }
    }
}