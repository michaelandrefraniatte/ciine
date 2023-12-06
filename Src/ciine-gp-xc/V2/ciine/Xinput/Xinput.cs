using SharpDX.XInput;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XInputAPI
{
    public class XInput
    {
        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern uint TimeBeginPeriod(uint ms);
        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
        private static extern uint TimeEndPeriod(uint ms);
        [DllImport("ntdll.dll", EntryPoint = "NtSetTimerResolution")]
        private static extern void NtSetTimerResolution(uint DesiredResolution, bool SetResolution, ref uint CurrentResolution);
        private static uint CurrentResolution = 0;
        private static bool running;
        public XInput()
        {
            TimeBeginPeriod(1);
            NtSetTimerResolution(1, true, ref CurrentResolution);
            running = true;
        }
        public void Close()
        {
            running = false;
        }
        public void taskD()
        {
            for (; ; )
            {
                if (!running)
                    break;
                ControllerProcess();
                System.Threading.Thread.Sleep(1);
            }
        }
        public void BeginPolling()
        {
            Task.Run(() => taskD());
        }
        private static Controller[] controller = new Controller[] { null };
        private static SharpDX.XInput.State xistate;
        private static int xinum = 0;
        public bool Controller1ButtonAPressed;
        public bool Controller1ButtonBPressed;
        public bool Controller1ButtonXPressed;
        public bool Controller1ButtonYPressed;
        public bool Controller1ButtonStartPressed;
        public bool Controller1ButtonBackPressed;
        public bool Controller1ButtonDownPressed;
        public bool Controller1ButtonUpPressed;
        public bool Controller1ButtonLeftPressed;
        public bool Controller1ButtonRightPressed;
        public bool Controller1ButtonShoulderLeftPressed;
        public bool Controller1ButtonShoulderRightPressed;
        public bool Controller1ThumbpadLeftPressed;
        public bool Controller1ThumbpadRightPressed;
        public double Controller1TriggerLeftPosition;
        public double Controller1TriggerRightPosition;
        public double Controller1ThumbLeftX;
        public double Controller1ThumbLeftY;
        public double Controller1ThumbRightX;
        public double Controller1ThumbRightY;
        public bool XInputHookConnect()
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
        private void ControllerProcess()
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
}