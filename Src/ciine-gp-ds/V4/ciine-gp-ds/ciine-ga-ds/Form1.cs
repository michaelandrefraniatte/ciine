using System;
using System.Windows.Forms;
using System.Reflection;
using System.Text;

namespace ciine_ga_ds
{
    public partial class Form1 : Form
    {
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
                using System.Runtime.InteropServices;
                using System.Threading;
                using System.Threading.Tasks;
                using System.Windows;
                using System.Windows.Forms;
                using System.Reflection;
                using controllers;
                using DualSenseAPI;
                using System.Numerics;
                using System.Collections.Generic;
                using System.Runtime;
                using System.Linq;
                namespace StringToCode
                {
                    public class FooClass 
                    { 
                        [DllImport(""winmm.dll"", EntryPoint = ""timeBeginPeriod"")]
                        private static extern uint TimeBeginPeriod(uint ms);
                        [DllImport(""winmm.dll"", EntryPoint = ""timeEndPeriod"")]
                        private static extern uint TimeEndPeriod(uint ms);
                        [DllImport(""ntdll.dll"", EntryPoint = ""NtSetTimerResolution"")]
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
                        public static bool PS5ControllerButtonFnLPressed;
                        public static bool PS5ControllerButtonFnRPressed;
                        public static bool PS5ControllerButtonBLPPressed;
                        public static bool PS5ControllerButtonBRPPressed;
                        public static bool PS5ControllerButtonMicPressed;
                        public static bool PS5ControllerTouchOn;
                        private static double PS5ControllerLeftStickX, PS5ControllerLeftStickY, PS5ControllerRightStickX, PS5ControllerRightStickY, PS5ControllerRightTriggerPosition, PS5ControllerLeftTriggerPosition, PS5ControllerTouchX, PS5ControllerTouchY;
                        public static bool PS5ControllerAccelCenter;
                        public static double PS5ControllerAccelX, PS5ControllerAccelY, PS5ControllerGyroX, PS5ControllerGyroY;
                        public static Vector3 gyr_gPS5 = new Vector3();
                        public static Vector3 acc_gPS5 = new Vector3();
                        public static Vector3 InitDirectAnglesPS5, DirectAnglesPS5;
                        private static bool running;
                        private int sleeptime = 1;
                        public void Load()
                        {
                            TimeBeginPeriod(1);
                            NtSetTimerResolution(1, true, ref CurrentResolution);
                            Task.Run(() => Start());
                        }
                        private void Start()
                        {
                            running = true;
                            DualSense.EnumerateControllers(""54C"", ""CE6"", ""DualSense"");
                            Thread.Sleep(2000);
                            Task.Run(() => taskD());
                            ScpBus.LoadController();
                            Thread.Sleep(2000);
                            Task.Run(() => taskX());
                        }
                        private void taskX()
                        {
                            for (; ; )
                            {
                                if (!running)
                                    break;
                                PS5ControllerLeftStickX = DualSense.LeftAnalogStick.X;
                                PS5ControllerLeftStickY = DualSense.LeftAnalogStick.Y;
                                PS5ControllerRightStickX = -DualSense.RightAnalogStick.X;
                                PS5ControllerRightStickY = -DualSense.RightAnalogStick.Y;
                                PS5ControllerLeftTriggerPosition = DualSense.L2;
                                PS5ControllerRightTriggerPosition = DualSense.R2;
                                PS5ControllerTouchX = DualSense.Touchpad1.X;
                                PS5ControllerTouchY = DualSense.Touchpad1.Y;
                                PS5ControllerTouchOn = DualSense.Touchpad1.IsDown;
                                gyr_gPS5.X = DualSense.Gyro.Z;
                                gyr_gPS5.Y = -DualSense.Gyro.X;
                                gyr_gPS5.Z = -DualSense.Gyro.Y;
                                PS5ControllerGyroX = gyr_gPS5.Z;
                                PS5ControllerGyroY = gyr_gPS5.Y;
                                acc_gPS5 = new Vector3(DualSense.Accelerometer.X, DualSense.Accelerometer.Z, DualSense.Accelerometer.Y);
                                PS5ControllerAccelCenter = DualSense.MenuButton;
                                if (PS5ControllerAccelCenter)
                                    InitDirectAnglesPS5 = acc_gPS5;
                                DirectAnglesPS5 = acc_gPS5 - InitDirectAnglesPS5;
                                PS5ControllerAccelX = -(DirectAnglesPS5.Y + DirectAnglesPS5.Z) / 6f;
                                PS5ControllerAccelY = DirectAnglesPS5.X / 6f;
                                PS5ControllerButtonCrossPressed = DualSense.CrossButton;
                                PS5ControllerButtonCirclePressed = DualSense.CircleButton;
                                PS5ControllerButtonSquarePressed = DualSense.SquareButton;
                                PS5ControllerButtonTrianglePressed = DualSense.TriangleButton;
                                PS5ControllerButtonDPadUpPressed = DualSense.DPadUpButton;
                                PS5ControllerButtonDPadRightPressed = DualSense.DPadRightButton;
                                PS5ControllerButtonDPadDownPressed = DualSense.DPadDownButton;
                                PS5ControllerButtonDPadLeftPressed = DualSense.DPadLeftButton;
                                PS5ControllerButtonL1Pressed = DualSense.L1Button;
                                PS5ControllerButtonR1Pressed = DualSense.R1Button;
                                PS5ControllerButtonL2Pressed = DualSense.L2Button;
                                PS5ControllerButtonR2Pressed = DualSense.R2Button;
                                PS5ControllerButtonL3Pressed = DualSense.L3Button;
                                PS5ControllerButtonR3Pressed = DualSense.R3Button;
                                PS5ControllerButtonCreatePressed = DualSense.CreateButton;
                                PS5ControllerButtonMenuPressed = DualSense.MenuButton;
                                PS5ControllerButtonLogoPressed = DualSense.LogoButton;
                                PS5ControllerButtonTouchpadPressed = DualSense.TouchpadButton;
                                PS5ControllerButtonFnLPressed = DualSense.FnL;
                                PS5ControllerButtonFnRPressed = DualSense.FnR;
                                PS5ControllerButtonBLPPressed = DualSense.BLP;
                                PS5ControllerButtonBRPPressed = DualSense.BRP;
                                PS5ControllerButtonMicPressed = DualSense.MicButton;
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
                        public void Close()
                        {
                            try
                            {
                                running = false;
                                Thread.Sleep(100);
                                ScpBus.UnLoadController();
                            }
                            catch { }
                        }
                        private static double Scale(double value, double min, double max, double minScale, double maxScale)
                        {
                            double scaled = minScale + (double)(value - min) / (max - min) * (maxScale - minScale);
                            return scaled;
                        }
                        private void taskD()
                        {
                            for (; ; )
                            {
                                if (!running)
                                    break;
                                DualSense.BeginPolling();
                            }
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
            parameters.ReferencedAssemblies.Add("System.Runtime.dll");
            parameters.ReferencedAssemblies.Add("System.Numerics.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("Dualsense.dll");
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