using System;
using System.Linq;
using Device.Net;
using Hid.Net.Windows;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Numerics;

namespace DualShock4API
{
    public class DualShock4
    {
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern uint TimeBeginPeriod(uint ms);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        private static extern uint TimeEndPeriod(uint ms);
        [DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
        private static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
        private static uint CurrentResolution = 0;
        private byte miscByte;
        private byte btnBlock1, btnBlock2, btnBlock3;
        private byte[] ds4data = new byte[37];
        public IDevice trezorDevice;
        public bool PS4ControllerButtonCrossPressed;
        public bool PS4ControllerButtonCirclePressed;
        public bool PS4ControllerButtonSquarePressed;
        public bool PS4ControllerButtonTrianglePressed;
        public bool PS4ControllerButtonDPadUpPressed;
        public bool PS4ControllerButtonDPadRightPressed;
        public bool PS4ControllerButtonDPadDownPressed;
        public bool PS4ControllerButtonDPadLeftPressed;
        public bool PS4ControllerButtonL1Pressed;
        public bool PS4ControllerButtonR1Pressed;
        public bool PS4ControllerButtonL2Pressed;
        public bool PS4ControllerButtonR2Pressed;
        public bool PS4ControllerButtonL3Pressed;
        public bool PS4ControllerButtonR3Pressed;
        public bool PS4ControllerButtonCreatePressed;
        public bool PS4ControllerButtonMenuPressed;
        public bool PS4ControllerButtonLogoPressed;
        public bool PS4ControllerButtonTouchpadPressed;
        public bool PS4ControllerButtonMicPressed;
        public bool PS4ControllerTouchOn;
        public double PS4ControllerLeftStickX, PS4ControllerLeftStickY, PS4ControllerRightStickX, PS4ControllerRightStickY, PS4ControllerRightTriggerPosition, PS4ControllerLeftTriggerPosition, PS4ControllerTouchX, PS4ControllerTouchY;
        public bool PS4ControllerAccelCenter;
        public double PS4ControllerAccelX, PS4ControllerAccelY, PS4ControllerGyroX, PS4ControllerGyroY;
        public Vector3 gyr_gPS4 = new Vector3();
        public Vector3 acc_gPS4 = new Vector3();
        public Vector3 InitDirectAnglesPS4, DirectAnglesPS4;
        public bool running;
        public DualShock4()
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
            running = true;
        }
        public void Close()
        {
            running = false;
        }
        public async void EnumerateControllers(string vendor_id, string product_id, string label_id)
        {
            var hidFactory = new FilterDeviceDefinition((uint)int.Parse(vendor_id, System.Globalization.NumberStyles.HexNumber), (uint)int.Parse(product_id, System.Globalization.NumberStyles.HexNumber), label: label_id).CreateWindowsHidDeviceFactory();
            var factories = hidFactory;
            var deviceDefinitions = (await factories.GetConnectedDeviceDefinitionsAsync().ConfigureAwait(false)).ToList();
            if (deviceDefinitions.Count == 0)
            {
                return;
            }
            trezorDevice = await hidFactory.GetDeviceAsync(deviceDefinitions.First()).ConfigureAwait(false);
            await trezorDevice.InitializeAsync().ConfigureAwait(false);
        }
        public void ProcessStateLogic()
        {
            LeftAnalogStick = ReadAnalogStick(ds4data[0], ds4data[1]);
            RightAnalogStick = ReadAnalogStick(ds4data[2], ds4data[3]);
            L2 = GetModeSwitch(ds4data, 7).ToUnsignedFloat();
            R2 = GetModeSwitch(ds4data, 8).ToUnsignedFloat();
            btnBlock1 = GetModeSwitch(ds4data, 4);
            btnBlock2 = GetModeSwitch(ds4data, 5);
            btnBlock3 = GetModeSwitch(ds4data, 6);
            SquareButton = btnBlock1.HasFlag(0x10);
            CrossButton = btnBlock1.HasFlag(0x20);
            CircleButton = btnBlock1.HasFlag(0x40);
            TriangleButton = btnBlock1.HasFlag(0x80);
            DPadUpButton = ReadDPadButton(btnBlock1, 0, 1, 7);
            DPadRightButton = ReadDPadButton(btnBlock1, 1, 2, 3);
            DPadDownButton = ReadDPadButton(btnBlock1, 3, 4, 5);
            DPadLeftButton = ReadDPadButton(btnBlock1, 5, 6, 7);
            L1Button = btnBlock2.HasFlag(0x01);
            R1Button = btnBlock2.HasFlag(0x02);
            L2Button = btnBlock2.HasFlag(0x04);
            R2Button = btnBlock2.HasFlag(0x08);
            CreateButton = btnBlock2.HasFlag(0x10);
            MenuButton = btnBlock2.HasFlag(0x20);
            L3Button = btnBlock2.HasFlag(0x40);
            R3Button = btnBlock2.HasFlag(0x80);
            LogoButton = btnBlock3.HasFlag(0x01);
            TouchpadButton = btnBlock3.HasFlag(0x02);
            MicButton = GetModeSwitch(ds4data, 9).HasFlag(0x04);
            Touchpad1 = ReadTouchpad(GetModeSwitch(ds4data, 34, 4));
            Touchpad2 = ReadTouchpad(GetModeSwitch(ds4data, 36, 4));
            Gyro = -ReadAccelAxes(
                GetModeSwitch(ds4data, 12, 2),
                GetModeSwitch(ds4data, 14, 2),
                GetModeSwitch(ds4data, 16, 2)
            );
            Accelerometer = ReadAccelAxes(
                GetModeSwitch(ds4data, 18, 2),
                GetModeSwitch(ds4data, 20, 2),
                GetModeSwitch(ds4data, 22, 2)
            );
            miscByte = GetModeSwitch(ds4data, 29);
            IsHeadphoneConnected = miscByte.HasFlag(0x01);
            PS4ControllerLeftStickX = LeftAnalogStick.X;
            PS4ControllerLeftStickY = LeftAnalogStick.Y;
            PS4ControllerRightStickX = -RightAnalogStick.X;
            PS4ControllerRightStickY = -RightAnalogStick.Y;
            PS4ControllerLeftTriggerPosition = L2;
            PS4ControllerRightTriggerPosition = R2;
            PS4ControllerTouchX = Touchpad1.X;
            PS4ControllerTouchY = Touchpad1.Y;
            PS4ControllerTouchOn = Touchpad1.IsDown;
            gyr_gPS4.X = Gyro.Z;
            gyr_gPS4.Y = -Gyro.X;
            gyr_gPS4.Z = -Gyro.Y;
            PS4ControllerGyroX = gyr_gPS4.Z;
            PS4ControllerGyroY = gyr_gPS4.Y;
            acc_gPS4 = new Vector3(Accelerometer.X, Accelerometer.Z, Accelerometer.Y);
            PS4ControllerAccelCenter = MenuButton;
            DirectAnglesPS4 = acc_gPS4 - InitDirectAnglesPS4;
            PS4ControllerAccelX = -(DirectAnglesPS4.Y + DirectAnglesPS4.Z) / 6f;
            PS4ControllerAccelY = DirectAnglesPS4.X / 6f;
            PS4ControllerButtonCrossPressed = CrossButton;
            PS4ControllerButtonCirclePressed = CircleButton;
            PS4ControllerButtonSquarePressed = SquareButton;
            PS4ControllerButtonTrianglePressed = TriangleButton;
            PS4ControllerButtonDPadUpPressed = DPadUpButton;
            PS4ControllerButtonDPadRightPressed = DPadRightButton;
            PS4ControllerButtonDPadDownPressed = DPadDownButton;
            PS4ControllerButtonDPadLeftPressed = DPadLeftButton;
            PS4ControllerButtonL1Pressed = L1Button;
            PS4ControllerButtonR1Pressed = R1Button;
            PS4ControllerButtonL2Pressed = L2Button;
            PS4ControllerButtonR2Pressed = R2Button;
            PS4ControllerButtonL3Pressed = L3Button;
            PS4ControllerButtonR3Pressed = R3Button;
            PS4ControllerButtonCreatePressed = CreateButton;
            PS4ControllerButtonMenuPressed = MenuButton;
            PS4ControllerButtonLogoPressed = LogoButton;
            PS4ControllerButtonTouchpadPressed = TouchpadButton;
            PS4ControllerButtonMicPressed = MicButton;
        }
        public void InitDualShock4Accel()
        {
            InitDirectAnglesPS4 = acc_gPS4;
        }
        private async void taskD()
        {
            for (; ; )
            {
                if (!running)
                    break;
                var readBuffer = trezorDevice.WriteAndReadAsync(GetOutputDataBytes());
                readBuffer.Wait();
                ds4data = (await readBuffer).Data.Skip(1).ToArray();
            }
        }
        public void BeginPolling()
        {
            Task.Run(() => taskD());
        }
        private static byte[] GetOutputDataBytes()
        {
            byte[] bytes = new byte[32];
            bytes[0] = 0x05;
            return bytes;
        }
        private static byte GetModeSwitch(byte[] ds4data, int indexIfUsb)
        {
            return indexIfUsb >= 0 ? ds4data[indexIfUsb] : (byte)0;
        }
        private static byte[] GetModeSwitch(byte[] data, int startIndexIfUsb, int size)
        {
            return startIndexIfUsb >= 0 ? data.Skip(startIndexIfUsb).Take(size).ToArray() : new byte[size];
        }
        private static Vec2 ReadAnalogStick(byte x, byte y)
        {
            float x1 = x.ToSignedFloat();
            float y1 = -y.ToSignedFloat();
            return new Vec2
            {
                X = Math.Abs(x1) >= 0f ? x1 : 0,
                Y = Math.Abs(y1) >= 0f ? y1 : 0
            };
        }
        private static bool ReadDPadButton(byte b, int v1, int v2, int v3)
        {
            int val = b & 0x0F;
            return val == v1 || val == v2 || val == v3;
        }
        private static DualShock4Touch ReadTouchpad(byte[] bytes)
        {
            if (!BitConverter.IsLittleEndian)
            {
                bytes = bytes.Reverse().ToArray();
            }
            uint raw = BitConverter.ToUInt32(bytes, 0);
            return new DualShock4Touch
            {
                X = (raw & 0x000FFF00) >> 8,
                Y = (raw & 0xFFF00000) >> 20,
                IsDown = (raw & 128) == 0,
                Id = bytes[0]
            };
        }
        private static Vec3 ReadAccelAxes(byte[] x, byte[] y, byte[] z)
        {
            if (!BitConverter.IsLittleEndian)
            {
                x = x.Reverse().ToArray();
                y = y.Reverse().ToArray();
                z = z.Reverse().ToArray();
            }
            return new Vec3
            {
                X = -BitConverter.ToInt16(x, 0),
                Y = BitConverter.ToInt16(y, 0),
                Z = BitConverter.ToInt16(z, 0)
            };
        }
        public static Vec2 LeftAnalogStick { get; private set; }
        public static Vec2 RightAnalogStick { get; private set; }
        public static float L2 { get; private set; }
        public static float R2 { get; private set; }
        public static bool SquareButton { get; private set; }
        public static bool CrossButton { get; private set; }
        public static bool CircleButton { get; private set; }
        public static bool TriangleButton { get; private set; }
        public static bool DPadUpButton { get; private set; }
        public static bool DPadRightButton { get; private set; }
        public static bool DPadDownButton { get; private set; }
        public static bool DPadLeftButton { get; private set; }
        public static bool L1Button { get; private set; }
        public static bool R1Button { get; private set; }
        public static bool L2Button { get; private set; }
        public static bool R2Button { get; private set; }
        public static bool CreateButton { get; private set; }
        public static bool MenuButton { get; private set; }
        public static bool L3Button { get; private set; }
        public static bool R3Button { get; private set; }
        public static bool LogoButton { get; private set; }
        public static bool TouchpadButton { get; private set; }
        public static bool MicButton { get; private set; }
        public static DualShock4Touch Touchpad1 { get; private set; }
        public static DualShock4Touch Touchpad2 { get; private set; }
        public static Vec3 Gyro { get; private set; }
        public static Vec3 Accelerometer { get; private set; }
        public static bool IsHeadphoneConnected { get; private set; }
    }
    internal static class DualShock4ByteConverterExtensions
    {
        public static float ToSignedFloat(this byte b)
        {
            return (b / 255.0f - 0.5f) * 2.0f;
        }
        public static float ToUnsignedFloat(this byte b)
        {
            return b / 255.0f;
        }
        public static bool HasFlag(this byte b, byte flag)
        {
            return (b & flag) == flag;
        }
    }
    public struct DualShock4Touch
    {
        public uint X;
        public uint Y;
        public bool IsDown;
        public byte Id;
    }
    public struct Vec2
    {
        public float X, Y;

        public float Magnitude()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }

        public Vec2 Normalize()
        {
            float m = Magnitude();
            return new Vec2 { X = X / m, Y = Y / m };
        }

        public static Vec2 operator -(Vec2 v)
        {
            return new Vec2 { X = -v.X, Y = -v.Y };
        }
    }
    public struct Vec3
    {
        public float X, Y, Z;
        public float Magnitude()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }
        public Vec3 Normalize()
        {
            float m = Magnitude();
            return new Vec3 { X = X / m, Y = Y / m, Z = Z / m };
        }
        public static Vec3 operator -(Vec3 v)
        {
            return new Vec3 { X = -v.X, Y = -v.Y, Z = -v.Z };
        }
    }
}