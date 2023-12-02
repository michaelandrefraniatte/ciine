using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Numerics;
using System.Linq;
using Device.Net;
using Hid.Net.Windows;
using System.Reactive.Linq;
using controllers;

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
        private static bool controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_xbox;
        private static double controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition;
        private double statex = 0f, statey = 0f, mousex = 0f, mousey = 0f, mousestatex = 0f, mousestatey = 0f, dzx = 0.0f, dzy = 0.0f, viewpower1x = 0f, viewpower2x = 1f, viewpower3x = 0f, viewpower1y = 0.25f, viewpower2y = 0.75f, viewpower3y = 0f;
        public static bool PS4ControllerButtonCrossPressed;
        public static bool PS4ControllerButtonCirclePressed;
        public static bool PS4ControllerButtonSquarePressed;
        public static bool PS4ControllerButtonTrianglePressed;
        public static bool PS4ControllerButtonDPadUpPressed;
        public static bool PS4ControllerButtonDPadRightPressed;
        public static bool PS4ControllerButtonDPadDownPressed;
        public static bool PS4ControllerButtonDPadLeftPressed;
        public static bool PS4ControllerButtonL1Pressed;
        public static bool PS4ControllerButtonR1Pressed;
        public static bool PS4ControllerButtonL2Pressed;
        public static bool PS4ControllerButtonR2Pressed;
        public static bool PS4ControllerButtonL3Pressed;
        public static bool PS4ControllerButtonR3Pressed;
        public static bool PS4ControllerButtonCreatePressed;
        public static bool PS4ControllerButtonMenuPressed;
        public static bool PS4ControllerButtonLogoPressed;
        public static bool PS4ControllerButtonTouchpadPressed;
        public static bool PS4ControllerButtonMicPressed;
        public static bool PS4ControllerTouchOn;
        private static double PS4ControllerLeftStickX, PS4ControllerLeftStickY, PS4ControllerRightStickX, PS4ControllerRightStickY, PS4ControllerRightTriggerPosition, PS4ControllerLeftTriggerPosition, PS4ControllerTouchX, PS4ControllerTouchY;
        public static bool PS4ControllerAccelCenter;
        public static double PS4ControllerAccelX, PS4ControllerAccelY, PS4ControllerGyroX, PS4ControllerGyroY;
        public static Vector3 gyr_gPS4 = new Vector3();
        public static Vector3 acc_gPS4 = new Vector3();
        public static Vector3 InitDirectAnglesPS4, DirectAnglesPS4;
        private static byte miscByte;
        private static byte btnBlock1, btnBlock2, btnBlock3;
        private static byte[] ds4data = new byte[37];
        private static bool running;
        private int sleeptime = 1;
        private void Form1_Load(object sender, EventArgs e)
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
            Task.Run(() => Start());
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                running = false;
                Thread.Sleep(100);
                ScpBus.UnLoadController();
            }
            catch { }
        }
        private async void Start()
        {
            running = true;
            Task.Run(() => taskD());
            ScpBus.LoadController();
            Thread.Sleep(1000);
            Task.Run(() => taskX());
        }
        private async void taskX()
        {
            for (; ; )
            {
                if (!running)
                    break;
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
                if (PS4ControllerAccelCenter)
                    InitDirectAnglesPS4 = acc_gPS4;
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
                statex = PS4ControllerGyroX * 15f;
                statey = PS4ControllerGyroY * 15f;
                if (statex > 0f)
                    mousestatex = Scale(statex, 0f, 32767f, dzx / 100f * 32767f, 32767f);
                if (statex < 0f)
                    mousestatex = Scale(statex, -32767f, 0f, -32767f, -(dzx / 100f) * 32767f);
                if (statey > 0f)
                    mousestatey = Scale(statey, 0f, 32767f, dzy / 100f * 32767f, 32767f);
                if (statey < 0f)
                    mousestatey = Scale(statey, -32767f, 0f, -32767f, -(dzy / 100f) * 32767f);
                mousex = mousestatex + PS4ControllerRightStickX * 32767f;
                mousey = mousestatey + PS4ControllerRightStickY * 32767f;
                statex = Math.Abs(mousex) <= 32767f ? mousex : Math.Sign(mousex) * 32767f;
                statey = Math.Abs(mousey) <= 32767f ? mousey : Math.Sign(mousey) * 32767f;
                controller1_send_rightstickx = -statex;
                controller1_send_rightsticky = -statey;
                mousex = -PS4ControllerLeftStickX * 1024f;
                mousey = -PS4ControllerLeftStickY * 1024f;
                controller1_send_leftstickx = Math.Abs(-mousex * 32767f / 1024f) <= 32767f ? -mousex * 32767f / 1024f : Math.Sign(-mousex) * 32767f;
                controller1_send_leftsticky = Math.Abs(-mousey * 32767f / 1024f) <= 32767f ? -mousey * 32767f / 1024f : Math.Sign(-mousey) * 32767f;
                controller1_send_down = PS4ControllerButtonDPadDownPressed;
                controller1_send_left = PS4ControllerButtonDPadLeftPressed;
                controller1_send_right = PS4ControllerButtonDPadRightPressed;
                controller1_send_up = PS4ControllerButtonDPadUpPressed;
                controller1_send_leftstick = PS4ControllerButtonL3Pressed;
                controller1_send_rightstick = PS4ControllerButtonR3Pressed;
                controller1_send_B = PS4ControllerButtonCrossPressed;
                controller1_send_A = PS4ControllerButtonCirclePressed;
                controller1_send_Y = PS4ControllerButtonSquarePressed;
                controller1_send_X = PS4ControllerButtonTrianglePressed;
                controller1_send_lefttriggerposition = PS4ControllerLeftTriggerPosition * 255;
                controller1_send_righttriggerposition = PS4ControllerRightTriggerPosition * 255;
                controller1_send_leftbumper = PS4ControllerButtonL1Pressed;
                controller1_send_rightbumper = PS4ControllerButtonR1Pressed;
                controller1_send_back = PS4ControllerButtonLogoPressed;
                controller1_send_start = PS4ControllerButtonTouchpadPressed;
                ScpBus.SetController(controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition, controller1_send_xbox);
                Thread.Sleep(sleeptime);
            }
        }
        private static double Scale(double value, double min, double max, double minScale, double maxScale)
        {
            double scaled = minScale + (double)(value - min) / (max - min) * (maxScale - minScale);
            return scaled;
        }
        private async void taskD()
        {
            string vendor_id = "54C";
            string product_id = "9CC";
            string label_id = "Wireless Controller";
            var hidFactory = new FilterDeviceDefinition((uint)int.Parse(vendor_id, System.Globalization.NumberStyles.HexNumber), (uint)int.Parse(product_id, System.Globalization.NumberStyles.HexNumber), label: label_id).CreateWindowsHidDeviceFactory();
            var factories = hidFactory;
            var deviceDefinitions = (await factories.GetConnectedDeviceDefinitionsAsync().ConfigureAwait(false)).ToList();
            if (deviceDefinitions.Count == 0)
            {
                return;
            }
            var trezorDevice = await hidFactory.GetDeviceAsync(deviceDefinitions.First()).ConfigureAwait(false);
            await trezorDevice.InitializeAsync().ConfigureAwait(false);
            for (; ; )
            {
                if (!running)
                    break;
                var readBuffer = trezorDevice.WriteAndReadAsync(GetOutputDataBytes());
                readBuffer.Wait();
                ds4data = ((TransferResult)await readBuffer).Data.Skip(1).ToArray();
            }
        }
        private byte[] GetOutputDataBytes()
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