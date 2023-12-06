using System;
using System.Windows.Forms;
using System.Reflection;
using System.Text;

namespace ciine
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
                using System.Numerics;
                using System.Runtime.InteropServices;
                using System.Threading;
                using System.Threading.Tasks;
                using System.Windows;
                using System.Windows.Forms;
                using System.Reflection;
                using SharpDX.XInput;
                using SharpDX;
                using controllers;
                using Valuechanges;
                using System.Diagnostics;
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
                        private static bool running;
                        private static bool controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_lefttrigger, controller1_send_righttrigger, controller1_send_xbox;
                        private static double controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition;
                        private static double statex = 0f, statey = 0f, mousex = 0f, mousey = 0f, mousestatex = 0f, mousestatey = 0f, dzx = 0.0f, dzy = 0.0f, viewpower1x = 0f, viewpower2x = 1f, viewpower3x = 0f, viewpower1y = 0.25f, viewpower2y = 0.75f, viewpower3y = 0f;
                        private static bool[] getstate = new bool[12];
                        private static int[] pollcount = new int[12];
                        private static int[] keys12345 = new int[12];
                        private static int[] keys54321 = new int[12];
                        private static double[] mousexp = new double[12];
                        private static double[] mouseyp = new double[12];
                        private static int sleeptime = 1;
                        public static Valuechange ValueChange = new Valuechange();
                        private static int[] wd = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
                        private static int[] wu = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
                        public static void valchanged(int n, bool val)
                        {
                            if (val)
                            {
                                if (wd[n] <= 1)
                                {
                                    wd[n] = wd[n] + 1;
                                }
                                wu[n] = 0;
                            }
                            else
                            {
                                if (wu[n] <= 1)
                                {
                                    wu[n] = wu[n] + 1;
                                }
                                wd[n] = 0;
                            }
                        }
                        public static void Main()
                        {
                            TimeBeginPeriod(1);
                            NtSetTimerResolution(1, true, ref CurrentResolution);
                            SetProcessPriority();
                            Task.Run(() => Start());
                        }
                        private static void SetProcessPriority()
                        {
                            using (Process p = Process.GetCurrentProcess())
                            {
                                p.PriorityClass = ProcessPriorityClass.RealTime;
                            }
                        }
                        private static void Start()
                        {
                            running = true;
                            XInputHookConnect();
                            ScpBus.LoadController();
                            Task.Run(() => taskX());
                        }
                        private static void taskX()
                        {
                            for (; ; )
                            {
                                if (!running)
                                    break;
                                ControllerProcess();
                                mousex                        = Controller1ThumbRightX;
                                mousey                        = Controller1ThumbRightY;
                                statex                        = Math.Abs(mousex) <= 32767f ? mousex : Math.Sign(mousex) * 32767f;
                                statey                        = Math.Abs(mousey) <= 32767f ? mousey : Math.Sign(mousey) * 32767f;
                                controller1_send_rightstickx   = statex;
                                controller1_send_rightsticky   = statey;
                                mousex                        = Controller1ThumbLeftX;
                                mousey                        = Controller1ThumbLeftY;
                                controller1_send_leftstickx  = Math.Abs(mousex) <= 32767f ? mousex : Math.Sign(mousex) * 32767f;
                                controller1_send_leftsticky  = Math.Abs(mousey) <= 32767f ? mousey : Math.Sign(mousey) * 32767f;
                                controller1_send_up           = Controller1ButtonUpPressed;
                                controller1_send_left         = Controller1ButtonLeftPressed;
                                controller1_send_down         = Controller1ButtonDownPressed;
                                controller1_send_right        = Controller1ButtonRightPressed;
                                controller1_send_back         = Controller1ButtonBackPressed;
                                controller1_send_start        = Controller1ButtonStartPressed;
                                controller1_send_leftstick    = Controller1ThumbpadLeftPressed;
                                controller1_send_leftbumper   = Controller1ButtonShoulderLeftPressed;
                                controller1_send_rightbumper  = Controller1ButtonShoulderRightPressed;
                                controller1_send_A            = Controller1ButtonAPressed;
                                controller1_send_B            = Controller1ButtonBPressed;
                                controller1_send_X            = Controller1ButtonXPressed;
                                controller1_send_Y            = Controller1ButtonYPressed;
                                controller1_send_rightstick   = Controller1ThumbpadRightPressed;
                                controller1_send_lefttrigger  = Controller1TriggerLeftPosition >= 100 ? true : false;
                                controller1_send_righttrigger = Controller1TriggerRightPosition >= 100 ? true : false;
                                ScpBus.SetController(controller1_send_back, controller1_send_start, controller1_send_A, controller1_send_B, controller1_send_X, controller1_send_Y, controller1_send_up, controller1_send_left, controller1_send_down, controller1_send_right, controller1_send_leftstick, controller1_send_rightstick, controller1_send_leftbumper, controller1_send_rightbumper, controller1_send_lefttrigger, controller1_send_righttrigger, controller1_send_leftstickx, controller1_send_leftsticky, controller1_send_rightstickx, controller1_send_rightsticky, controller1_send_lefttriggerposition, controller1_send_righttriggerposition, controller1_send_xbox);
                                Thread.Sleep(sleeptime);
                            }
                        }
                        public static void Close()
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
                        private static Controller[] controller = new Controller[] { null };
                        private static SharpDX.XInput.State xistate;
                        private static int xinum = 0;
                        public static bool Controller1ButtonAPressed;
                        public static bool Controller1ButtonBPressed;
                        public static bool Controller1ButtonXPressed;
                        public static bool Controller1ButtonYPressed;
                        public static bool Controller1ButtonStartPressed;
                        public static bool Controller1ButtonBackPressed;
                        public static bool Controller1ButtonDownPressed;
                        public static bool Controller1ButtonUpPressed;
                        public static bool Controller1ButtonLeftPressed;
                        public static bool Controller1ButtonRightPressed;
                        public static bool Controller1ButtonShoulderLeftPressed;
                        public static bool Controller1ButtonShoulderRightPressed;
                        public static bool Controller1ThumbpadLeftPressed;
                        public static bool Controller1ThumbpadRightPressed;
                        public static double Controller1TriggerLeftPosition;
                        public static double Controller1TriggerRightPosition;
                        public static double Controller1ThumbLeftX;
                        public static double Controller1ThumbLeftY;
                        public static double Controller1ThumbRightX;
                        public static double Controller1ThumbRightY;
                        public static bool XInputHookConnect()
                        {
                            try
                            {
                                controller = new Controller[] { null };
                                xinum = 0;
                                var controllers = new[] { new Controller(UserIndex.One) };
                                foreach (var selectControler in controllers)
                                {
                                    if (selectControler.IsConnected)
                                    {
                                        controller[xinum] = selectControler;
                                        xinum++;
                                        if (xinum >= 1)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                            catch { }
                            if (controller[0] == null)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        private static void ControllerProcess()
                        {
                            for (int inc = 0; inc < xinum; inc++)
                            {
                                xistate = controller[inc].GetState();
                                if (inc == 0)
                                {
                                    if (xistate.Gamepad.Buttons.HasFlag(GamepadButtonFlags.A))
                                        Controller1ButtonAPressed = true;
                                    else
                                        Controller1ButtonAPressed = false;
                                    if (xistate.Gamepad.Buttons.HasFlag(GamepadButtonFlags.B))
                                        Controller1ButtonBPressed = true;
                                    else
                                        Controller1ButtonBPressed = false;
                                    if (xistate.Gamepad.Buttons.HasFlag(GamepadButtonFlags.X))
                                        Controller1ButtonXPressed = true;
                                    else
                                        Controller1ButtonXPressed = false;
                                    if (xistate.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Y))
                                        Controller1ButtonYPressed = true;
                                    else
                                        Controller1ButtonYPressed = false;
                                    if (xistate.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Start))
                                        Controller1ButtonStartPressed = true;
                                    else
                                        Controller1ButtonStartPressed = false;
                                    if (xistate.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Back))
                                        Controller1ButtonBackPressed = true;
                                    else
                                        Controller1ButtonBackPressed = false;
                                    if (xistate.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown))
                                        Controller1ButtonDownPressed = true;
                                    else
                                        Controller1ButtonDownPressed = false;
                                    if (xistate.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp))
                                        Controller1ButtonUpPressed = true;
                                    else
                                        Controller1ButtonUpPressed = false;
                                    if (xistate.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft))
                                        Controller1ButtonLeftPressed = true;
                                    else
                                        Controller1ButtonLeftPressed = false;
                                    if (xistate.Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadRight))
                                        Controller1ButtonRightPressed = true;
                                    else
                                        Controller1ButtonRightPressed = false;
                                    if (xistate.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder))
                                        Controller1ButtonShoulderLeftPressed = true;
                                    else
                                        Controller1ButtonShoulderLeftPressed = false;
                                    if (xistate.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder))
                                        Controller1ButtonShoulderRightPressed = true;
                                    else
                                        Controller1ButtonShoulderRightPressed = false;
                                    if (xistate.Gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb))
                                        Controller1ThumbpadLeftPressed = true;
                                    else
                                        Controller1ThumbpadLeftPressed = false;
                                    if (xistate.Gamepad.Buttons.HasFlag(GamepadButtonFlags.RightThumb))
                                        Controller1ThumbpadRightPressed = true;
                                    else
                                        Controller1ThumbpadRightPressed = false;
                                    Controller1TriggerLeftPosition = xistate.Gamepad.LeftTrigger;
                                    Controller1TriggerRightPosition = xistate.Gamepad.RightTrigger;
                                    Controller1ThumbLeftX = xistate.Gamepad.LeftThumbX;
                                    Controller1ThumbLeftY = xistate.Gamepad.LeftThumbY;
                                    Controller1ThumbRightX = xistate.Gamepad.RightThumbX;
                                    Controller1ThumbRightY = xistate.Gamepad.RightThumbY;
                                }
                            }
                        }
                    }
                }";
        private void Form1_Load(object sender, EventArgs e)
        {
            parameters = new System.CodeDom.Compiler.CompilerParameters();
            parameters.GenerateExecutable = true;
            parameters.GenerateInMemory = false;
            parameters.IncludeDebugInformation = false;
            parameters.CompilerOptions = "/optimize";
            parameters.ReferencedAssemblies.Add(Application.StartupPath + @"\System.dll");
            parameters.ReferencedAssemblies.Add(Application.StartupPath + @"\System.Windows.Forms.dll");
            parameters.ReferencedAssemblies.Add(Application.StartupPath + @"\System.Drawing.dll");
            parameters.ReferencedAssemblies.Add(Application.StartupPath + @"\System.Runtime.dll");
            parameters.ReferencedAssemblies.Add(Application.StartupPath + @"\SharpDX.XInput.dll");
            parameters.ReferencedAssemblies.Add(Application.StartupPath + @"\SharpDX.dll");
            parameters.ReferencedAssemblies.Add(Application.StartupPath + @"\controllers.dll");
            parameters.ReferencedAssemblies.Add(Application.StartupPath + @"\Valuechanges.dll");
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
            program.InvokeMember("Main", BindingFlags.IgnoreReturn | BindingFlags.InvokeMethod, null, obj, new object[] { });
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            program.InvokeMember("Close", BindingFlags.IgnoreReturn | BindingFlags.InvokeMethod, null, obj, new object[] { });
        }
    }
}