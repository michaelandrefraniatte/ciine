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
        public static bool PS5ControllerButtonCrossPressed;
        public static bool PS5ControllerButtonCirclePressed;
        public static bool PS5ControllerButtonSquarePressed;
        public static bool PS5ControllerButtonTrianglePressed;
        public static bool PS5ControllerButtonDPadUpPressed;
        public static bool PS5ControllerButtonDPadRightPressed;
        public static bool PS5ControllerButtonDPadDownPressed;
        public static bool PS5ControllerButtonDPadLeftPressed;
        public static bool PS5ControllerButtonL1Pressed;
        public static bool PS5ControllerButtonR1Pressed;
        public static bool PS5ControllerButtonL2Pressed;
        public static bool PS5ControllerButtonR2Pressed;
        public static bool PS5ControllerButtonL3Pressed;
        public static bool PS5ControllerButtonR3Pressed;
        public static bool PS5ControllerButtonCreatePressed;
        public static bool PS5ControllerButtonMenuPressed;
        public static bool PS5ControllerButtonLogoPressed;
        public static bool PS5ControllerButtonTouchpadPressed;
        public static bool PS5ControllerButtonMicPressed;
        public static bool PS5ControllerTouchOn;
        private static double PS5ControllerLeftStickX, PS5ControllerLeftStickY, PS5ControllerRightStickX, PS5ControllerRightStickY, PS5ControllerRightTriggerPosition, PS5ControllerLeftTriggerPosition, PS5ControllerTouchX, PS5ControllerTouchY;
        public static bool PS5ControllerAccelCenter;
        public static double PS5ControllerAccelX, PS5ControllerAccelY, PS5ControllerGyroX, PS5ControllerGyroY;
        public static Vector3 gyr_gPS5 = new Vector3();
        public static Vector3 acc_gPS5 = new Vector3();
        public static Vector3 InitDirectAnglesPS5, DirectAnglesPS5;
        private static byte miscByte;
        private static byte btnBlock1, btnBlock2, btnBlock3;
        private static byte[] dsdata = new byte[54];
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
            Thread.Sleep(2000);
            Task.Run(() => taskX());
        }
        private async void taskX()
        {
            for (; ; )
            {
                if (!running)
                    break;
                LeftAnalogStick = ReadAnalogStick(dsdata[0], dsdata[1]);
                RightAnalogStick = ReadAnalogStick(dsdata[2], dsdata[3]);
                L2 = GetModeSwitch(dsdata, 4).ToUnsignedFloat();
                R2 = GetModeSwitch(dsdata, 5).ToUnsignedFloat();
                btnBlock1 = GetModeSwitch(dsdata, 7);
                btnBlock2 = GetModeSwitch(dsdata, 8);
                btnBlock3 = GetModeSwitch(dsdata, 9);
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
                FnL = btnBlock3.HasFlag(1 << 4);
                FnR = btnBlock3.HasFlag(1 << 5);
                BLP = btnBlock3.HasFlag(1 << 6);
                BRP = btnBlock3.HasFlag(1 << 7);
                MicButton = GetModeSwitch(dsdata, 9).HasFlag(0x04);
                Touchpad1 = ReadTouchpad(GetModeSwitch(dsdata, 32, 4));
                Touchpad2 = ReadTouchpad(GetModeSwitch(dsdata, 36, 4));
                Gyro = -ReadAccelAxes(
                    GetModeSwitch(dsdata, 15, 2),
                    GetModeSwitch(dsdata, 17, 2),
                    GetModeSwitch(dsdata, 19, 2)
                );
                Accelerometer = ReadAccelAxes(
                    GetModeSwitch(dsdata, 21, 2),
                    GetModeSwitch(dsdata, 23, 2),
                    GetModeSwitch(dsdata, 25, 2)
                );
                miscByte = GetModeSwitch(dsdata, 53);
                IsHeadphoneConnected = miscByte.HasFlag(0x01);
                PS5ControllerLeftStickX = LeftAnalogStick.X;
                PS5ControllerLeftStickY = LeftAnalogStick.Y;
                PS5ControllerRightStickX = -RightAnalogStick.X;
                PS5ControllerRightStickY = -RightAnalogStick.Y;
                PS5ControllerLeftTriggerPosition = L2;
                PS5ControllerRightTriggerPosition = R2;
                PS5ControllerTouchX = Touchpad1.X;
                PS5ControllerTouchY = Touchpad1.Y;
                PS5ControllerTouchOn = Touchpad1.IsDown;
                gyr_gPS5.X = Gyro.Z;
                gyr_gPS5.Y = -Gyro.X;
                gyr_gPS5.Z = -Gyro.Y;
                PS5ControllerGyroX = gyr_gPS5.Z;
                PS5ControllerGyroY = gyr_gPS5.Y;
                acc_gPS5 = new Vector3(Accelerometer.X, Accelerometer.Z, Accelerometer.Y);
                PS5ControllerAccelCenter = MenuButton;
                if (PS5ControllerAccelCenter)
                    InitDirectAnglesPS5 = acc_gPS5;
                DirectAnglesPS5 = acc_gPS5 - InitDirectAnglesPS5;
                PS5ControllerAccelX = -(DirectAnglesPS5.Y + DirectAnglesPS5.Z) / 6f;
                PS5ControllerAccelY = DirectAnglesPS5.X / 6f;
                PS5ControllerButtonCrossPressed = CrossButton;
                PS5ControllerButtonCirclePressed = CircleButton;
                PS5ControllerButtonSquarePressed = SquareButton;
                PS5ControllerButtonTrianglePressed = TriangleButton;
                PS5ControllerButtonDPadUpPressed = DPadUpButton;
                PS5ControllerButtonDPadRightPressed = DPadRightButton;
                PS5ControllerButtonDPadDownPressed = DPadDownButton;
                PS5ControllerButtonDPadLeftPressed = DPadLeftButton;
                PS5ControllerButtonL1Pressed = L1Button;
                PS5ControllerButtonR1Pressed = R1Button;
                PS5ControllerButtonL2Pressed = L2Button;
                PS5ControllerButtonR2Pressed = R2Button;
                PS5ControllerButtonL3Pressed = L3Button;
                PS5ControllerButtonR3Pressed = R3Button;
                PS5ControllerButtonCreatePressed = CreateButton;
                PS5ControllerButtonMenuPressed = MenuButton;
                PS5ControllerButtonLogoPressed = LogoButton;
                PS5ControllerButtonTouchpadPressed = TouchpadButton;
                PS5ControllerButtonMicPressed = MicButton;
                statex = PS5ControllerGyroX * 15f;
                statey = PS5ControllerGyroY * 15f;
                if (statex > 0f)
                    mousestatex = Scale(statex, 0f, 32767f, dzx / 100f * 32767f, 32767f);
                if (statex < 0f)
                    mousestatex = Scale(statex, -32767f, 0f, -32767f, -(dzx / 100f) * 32767f);
                if (statey > 0f)
                    mousestatey = Scale(statey, 0f, 32767f, dzy / 100f * 32767f, 32767f);
                if (statey < 0f)
                    mousestatey = Scale(statey, -32767f, 0f, -32767f, -(dzy / 100f) * 32767f);
                mousex = mousestatex + PS5ControllerRightStickX * 32767f;
                mousey = mousestatey + PS5ControllerRightStickY * 32767f;
                statex = Math.Abs(mousex) <= 32767f ? mousex : Math.Sign(mousex) * 32767f;
                statey = Math.Abs(mousey) <= 32767f ? mousey : Math.Sign(mousey) * 32767f;
                controller1_send_rightstickx = -statex;
                controller1_send_rightsticky = -statey;
                mousex = -PS5ControllerLeftStickX * 1024f;
                mousey = -PS5ControllerLeftStickY * 1024f;
                controller1_send_leftstickx = Math.Abs(-mousex * 32767f / 1024f) <= 32767f ? -mousex * 32767f / 1024f : Math.Sign(-mousex) * 32767f;
                controller1_send_leftsticky = Math.Abs(-mousey * 32767f / 1024f) <= 32767f ? -mousey * 32767f / 1024f : Math.Sign(-mousey) * 32767f;
                controller1_send_down = PS5ControllerButtonDPadDownPressed;
                controller1_send_left = PS5ControllerButtonDPadLeftPressed;
                controller1_send_right = PS5ControllerButtonDPadRightPressed;
                controller1_send_up = PS5ControllerButtonDPadUpPressed;
                controller1_send_leftstick = PS5ControllerButtonL3Pressed;
                controller1_send_rightstick = PS5ControllerButtonR3Pressed;
                controller1_send_B = PS5ControllerButtonCrossPressed;
                controller1_send_A = PS5ControllerButtonCirclePressed;
                controller1_send_Y = PS5ControllerButtonSquarePressed;
                controller1_send_X = PS5ControllerButtonTrianglePressed;
                controller1_send_lefttriggerposition = PS5ControllerLeftTriggerPosition * 255;
                controller1_send_righttriggerposition = PS5ControllerRightTriggerPosition * 255;
                controller1_send_leftbumper = PS5ControllerButtonL1Pressed;
                controller1_send_rightbumper = PS5ControllerButtonR1Pressed;
                controller1_send_back = PS5ControllerButtonLogoPressed;
                controller1_send_start = PS5ControllerButtonTouchpadPressed;
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
            // This is Windows specific code. You can replace this with your platform of choice or put this part in the composition root of your app
            string vendor_id = "54C";
            string product_id = "CE6";
            string label_id = "DualSense";
            //Register the factory for creating Hid devices. 
            var hidFactory = new FilterDeviceDefinition((uint)int.Parse(vendor_id, System.Globalization.NumberStyles.HexNumber), (uint)int.Parse(product_id, System.Globalization.NumberStyles.HexNumber), label: label_id).CreateWindowsHidDeviceFactory();
            //Join the factories together so that it picks up either the Hid or USB device
            var factories = hidFactory;
            //Get connected device definitions
            var deviceDefinitions = (await factories.GetConnectedDeviceDefinitionsAsync().ConfigureAwait(false)).ToList();
            if (deviceDefinitions.Count == 0)
            {
                //No devices were found
                return;
            }
            //Get the device from its definition
            var trezorDevice = await hidFactory.GetDeviceAsync(deviceDefinitions.First()).ConfigureAwait(false);
            //Initialize the device
            await trezorDevice.InitializeAsync().ConfigureAwait(false);
            for (; ; )
            {
                if (!running)
                    break;
                var readBuffer = trezorDevice.WriteAndReadAsync(GetOutputDataBytes());
                readBuffer.Wait();
                dsdata = ((TransferResult)await readBuffer).Data.Skip(1).ToArray();
            }
        }

        /// <summary>
        /// Builds the output byte array that will be sent to the controller.
        /// </summary>
        /// <returns>An array of bytes to send to the controller</returns>
        private byte[] GetOutputDataBytes()
        {
            byte[] bytes = new byte[48];
            bytes[0] = 0x02;
            return bytes;
        }
        private static byte GetModeSwitch(byte[] dsdata, int indexIfUsb)
        {
            return indexIfUsb >= 0 ? dsdata[indexIfUsb] : (byte)0;
        }
        private static byte[] GetModeSwitch(byte[] dsdata, int startIndexIfUsb, int size)
        {
            return startIndexIfUsb >= 0 ? dsdata.Skip(startIndexIfUsb).Take(size).ToArray() : new byte[size];
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
            // force everything into the right byte order; input bytes are LSB-first
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
            // force everything into the right byte order; assuming that input bytes is little-endian
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

        /// <summary>
        /// The left analog stick. Values are from -1 to 1. Positive X is right, positive Y is up.
        /// </summary>
        public static Vec2 LeftAnalogStick { get; private set; }

        /// <summary>
        /// The right analog stick. Values are from -1 to 1. Positive X is right, positive Y is up.
        /// </summary>
        public static Vec2 RightAnalogStick { get; private set; }

        /// <summary>
        /// L2's analog value, from 0 to 1.
        /// </summary>
        public static float L2 { get; private set; }

        /// <summary>
        /// R2's analog value, from 0 to 1.
        /// </summary>
        public static float R2 { get; private set; }

        /// <summary>
        /// The status of the square button.
        /// </summary>
        public static bool SquareButton { get; private set; }

        /// <summary>
        /// The status of the cross button.
        /// </summary>
        public static bool CrossButton { get; private set; }

        /// <summary>
        /// The status of the circle button.
        /// </summary>
        public static bool CircleButton { get; private set; }

        /// <summary>
        /// The status of the triangle button.
        /// </summary>
        public static bool TriangleButton { get; private set; }

        /// <summary>
        /// The status of the D-pad up button.
        /// </summary>
        public static bool DPadUpButton { get; private set; }

        /// <summary>
        /// The status of the D-pad right button.
        /// </summary>
        public static bool DPadRightButton { get; private set; }

        /// <summary>
        /// The status of the D-pad down button.
        /// </summary>
        public static bool DPadDownButton { get; private set; }

        /// <summary>
        /// The status of the D-pad left button.
        /// </summary>
        public static bool DPadLeftButton { get; private set; }

        /// <summary>
        /// The status of the L1 button.
        /// </summary>
        public static bool L1Button { get; private set; }

        /// <summary>
        /// The status of the R1 button.
        /// </summary>
        public static bool R1Button { get; private set; }

        /// <summary>
        /// The status of the L2 button.
        /// </summary>
        public static bool L2Button { get; private set; }

        /// <summary>
        /// The status of the R2 button.
        /// </summary>
        public static bool R2Button { get; private set; }

        /// <summary>
        /// The status of the create button.
        /// </summary>
        public static bool CreateButton { get; private set; }

        /// <summary>
        /// The status of the menu button.
        /// </summary>
        public static bool MenuButton { get; private set; }

        /// <summary>
        /// The status of the L3 button.
        /// </summary>
        public static bool L3Button { get; private set; }

        /// <summary>
        /// The status of the R3 button.
        /// </summary>
        public static bool R3Button { get; private set; }

        /// <summary>
        /// The status of the PlayStation logo button.
        /// </summary>
        public static bool LogoButton { get; private set; }

        /// <summary>
        /// The status of the touchpad button.
        /// </summary>
        public static bool TouchpadButton { get; private set; }

        /// <summary>
        /// The status of the FnL button.
        /// </summary>
        public static bool FnL { get; private set; }

        /// <summary>
        /// The status of the FnR button.
        /// </summary>
        public static bool FnR { get; private set; }

        /// <summary>
        /// The status of the BLP button.
        /// </summary>
        public static bool BLP { get; private set; }

        /// <summary>
        /// The status of the BRP button.
        /// </summary>
        public static bool BRP { get; private set; }

        /// <summary>
        /// The status of the mic button.
        /// </summary>
        public static bool MicButton { get; private set; }

        /// <summary>
        /// The first touch point.
        /// </summary>
        public static DualShock4Touch Touchpad1 { get; private set; }

        /// <summary>
        /// The second touch point.
        /// </summary>
        public static DualShock4Touch Touchpad2 { get; private set; }

        /// <summary>
        /// The accelerometer's rotational axes. The directions of the axes have been slightly adjusted from the controller's original values
        /// to make them behave nicer with standard Newtonian physics. The signs follow normal right-hand rule with respect to
        /// <see cref="Accelerometer"/>'s axes, e.g. +X rotation means counterclockwise around the +X axis and so on. Unit is unclear, but
        /// magnitude while stationary is about 0.
        /// </summary>
        public static Vec3 Gyro { get; private set; }

        /// <summary>
        /// The accelerometer's linear axes. The directions of the axes have been slightly adjusted from the controller's original values
        /// to make them behave nicer with standard Newtonian physics. +X is to the right. +Y is behind the controller (roughly straight down
        /// if the controller is flat on the table). +Z is at the top of the controller (where the USB port is). Unit is unclear, but magnitude
        /// while stationary (e.g. just gravity) is about 8000 +- 100.
        /// </summary>
        public static Vec3 Accelerometer { get; private set; }

        /// <summary>
        /// Whether or not headphones are connected to the controller.
        /// </summary>
        public static bool IsHeadphoneConnected { get; private set; }
    }
    /// <summary>
    /// Extension logic to help conversion between bytes and more useful formats.
    /// </summary>
    internal static class DualShock4ByteConverterExtensions
    {
        /// <summary>
        /// Converts a byte to the corresponding signed float.
        /// </summary>
        /// <param name="b">The byte value</param>
        /// <returns>The byte, scaled and translated to floating point value between -1 and 1.</returns>
        public static float ToSignedFloat(this byte b)
        {
            return (b / 255.0f - 0.5f) * 2.0f;
        }

        /// <summary>
        /// Converts a byte to the corresponding unsigned float.
        /// </summary>
        /// <param name="b">The byte value</param>
        /// <returns>The byte, scaled to a floating point value between 0 and 1.</returns>
        public static float ToUnsignedFloat(this byte b)
        {
            return b / 255.0f;
        }

        /// <summary>
        /// Checks whether the provided flag's bits are set on this byte. Similar to <see cref="Enum.HasFlag(Enum)"/>.
        /// </summary>
        /// <param name="b">The byte value</param>
        /// <param name="flag">The flag to check</param>
        /// <returns>Whether all the bits of the flag are set on the byte.</returns>
        public static bool HasFlag(this byte b, byte flag)
        {
            return (b & flag) == flag;
        }
    }
    /// <summary>
    /// One of the DualShock4's 2 touch points. The touchpad is 1920x1080, 0-indexed.
    /// </summary>
    public struct DualShock4Touch
    {
        /// <summary>
        /// The X position of the touchpoint. 0 is the leftmost edge. If the touch point is currently pressed,
        /// this is the current position. If the touch point is released, it was the last position before it
        /// was released.
        /// </summary>
        public uint X;

        /// <summary>
        /// The Y position of the touchpoint. 0 is the topmost edge. If the touch point is currently pressed,
        /// this is the current position. If the touch point is released, it was the last position before it
        /// was released.
        /// </summary>
        public uint Y;

        /// <summary>
        /// Whether the touch point is currently pressed.
        /// </summary>
        public bool IsDown;

        /// <summary>
        /// The touch id. This is a counter that changes whenever a touch is pressed or released.
        /// </summary>
        public byte Id;
    }
    /// <summary>
    /// A 2D vector
    /// </summary>
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
    /// <summary>
    /// A 3D vector
    /// </summary>
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