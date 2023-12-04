using System;
using System.Linq;
using Device.Net;
using Hid.Net.Windows;
using System.Reactive.Linq;

namespace DualShock4API
{
    public class DualShock4
    {
        private static byte miscByte;
        private static byte btnBlock1, btnBlock2, btnBlock3;
        private static byte[] ds4data = new byte[37];
        public static IDevice trezorDevice;
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
        }
        public async void BeginPolling()
        {
            var readBuffer = trezorDevice.WriteAndReadAsync(GetOutputDataBytes());
            readBuffer.Wait();
            ds4data = (await readBuffer).Data.Skip(1).ToArray();
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