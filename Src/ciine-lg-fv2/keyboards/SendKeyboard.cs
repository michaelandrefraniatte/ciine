﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace keyboards
{
    public class SendKeyboard
    {
        [DllImport("keyboard.dll", EntryPoint = "SendKey", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendKey(UInt16 bVk, UInt16 bScan);
        [DllImport("keyboard.dll", EntryPoint = "SendKeyF", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendKeyF(UInt16 bVk, UInt16 bScan);
        [DllImport("keyboard.dll", EntryPoint = "SendKeyArrows", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendKeyArrows(UInt16 bVk, UInt16 bScan);
        [DllImport("keyboard.dll", EntryPoint = "SendKeyArrowsF", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendKeyArrowsF(UInt16 bVk, UInt16 bScan);
        [DllImport("keyboard.dll", EntryPoint = "SendMouseEventButtonLeft", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendMouseEventButtonLeft();
        [DllImport("keyboard.dll", EntryPoint = "SendMouseEventButtonLeftF", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendMouseEventButtonLeftF();
        [DllImport("keyboard.dll", EntryPoint = "SendMouseEventButtonRight", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendMouseEventButtonRight();
        [DllImport("keyboard.dll", EntryPoint = "SendMouseEventButtonRightF", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendMouseEventButtonRightF();
        [DllImport("keyboard.dll", EntryPoint = "SendMouseEventButtonMiddle", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendMouseEventButtonMiddle();
        [DllImport("keyboard.dll", EntryPoint = "SendMouseEventButtonMiddleF", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendMouseEventButtonMiddleF();
        [DllImport("keyboard.dll", EntryPoint = "SendMouseEventButtonWheelUp", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendMouseEventButtonWheelUp();
        [DllImport("keyboard.dll", EntryPoint = "SendMouseEventButtonWheelDown", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendMouseEventButtonWheelDown();
        [DllImport("keyboard.dll", EntryPoint = "SimulateKeyDown", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SimulateKeyDown(UInt16 keyCode, UInt16 bScan);
        [DllImport("keyboard.dll", EntryPoint = "SimulateKeyUp", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SimulateKeyUp(UInt16 keyCode, UInt16 bScan);
        [DllImport("keyboard.dll", EntryPoint = "SimulateKeyDownArrows", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SimulateKeyDownArrows(UInt16 keyCode, UInt16 bScan);
        [DllImport("keyboard.dll", EntryPoint = "SimulateKeyUpArrows", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SimulateKeyUpArrows(UInt16 keyCode, UInt16 bScan);
        [DllImport("keyboard.dll", EntryPoint = "LeftClick", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LeftClick();
        [DllImport("keyboard.dll", EntryPoint = "LeftClickF", CallingConvention = CallingConvention.Cdecl)]
        public static extern void LeftClickF();
        [DllImport("keyboard.dll", EntryPoint = "RightClick", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RightClick();
        [DllImport("keyboard.dll", EntryPoint = "RightClickF", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RightClickF();
        [DllImport("keyboard.dll", EntryPoint = "MiddleClick", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MiddleClick();
        [DllImport("keyboard.dll", EntryPoint = "MiddleClickF", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MiddleClickF();
        [DllImport("keyboard.dll", EntryPoint = "WheelDownF", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WheelDownF();
        [DllImport("keyboard.dll", EntryPoint = "WheelUpF", CallingConvention = CallingConvention.Cdecl)]
        public static extern void WheelUpF();
        public const ushort VK_LBUTTON = (ushort)0x01;
        public const ushort VK_RBUTTON = (ushort)0x02;
        public const ushort VK_CANCEL = (ushort)0x03;
        public const ushort VK_MBUTTON = (ushort)0x04;
        public const ushort VK_XBUTTON1 = (ushort)0x05;
        public const ushort VK_XBUTTON2 = (ushort)0x06;
        public const ushort VK_BACK = (ushort)0x08;
        public const ushort VK_Tab = (ushort)0x09;
        public const ushort VK_CLEAR = (ushort)0x0C;
        public const ushort VK_Return = (ushort)0x0D;
        public const ushort VK_SHIFT = (ushort)0x10;
        public const ushort VK_CONTROL = (ushort)0x11;
        public const ushort VK_MENU = (ushort)0x12;
        public const ushort VK_PAUSE = (ushort)0x13;
        public const ushort VK_CAPITAL = (ushort)0x14;
        public const ushort VK_KANA = (ushort)0x15;
        public const ushort VK_HANGEUL = (ushort)0x15;
        public const ushort VK_HANGUL = (ushort)0x15;
        public const ushort VK_JUNJA = (ushort)0x17;
        public const ushort VK_FINAL = (ushort)0x18;
        public const ushort VK_HANJA = (ushort)0x19;
        public const ushort VK_KANJI = (ushort)0x19;
        public const ushort VK_Escape = (ushort)0x1B;
        public const ushort VK_CONVERT = (ushort)0x1C;
        public const ushort VK_NONCONVERT = (ushort)0x1D;
        public const ushort VK_ACCEPT = (ushort)0x1E;
        public const ushort VK_MODECHANGE = (ushort)0x1F;
        public const ushort VK_Space = (ushort)0x20;
        public const ushort VK_PRIOR = (ushort)0x21;
        public const ushort VK_NEXT = (ushort)0x22;
        public const ushort VK_END = (ushort)0x23;
        public const ushort VK_HOME = (ushort)0x24;
        public const ushort VK_LEFT = (ushort)0x25;
        public const ushort VK_UP = (ushort)0x26;
        public const ushort VK_RIGHT = (ushort)0x27;
        public const ushort VK_DOWN = (ushort)0x28;
        public const ushort VK_SELECT = (ushort)0x29;
        public const ushort VK_PRINT = (ushort)0x2A;
        public const ushort VK_EXECUTE = (ushort)0x2B;
        public const ushort VK_SNAPSHOT = (ushort)0x2C;
        public const ushort VK_INSERT = (ushort)0x2D;
        public const ushort VK_DELETE = (ushort)0x2E;
        public const ushort VK_HELP = (ushort)0x2F;
        public const ushort VK_APOSTROPHE = (ushort)0xDE;
        public const ushort VK_0 = (ushort)0x30;
        public const ushort VK_1 = (ushort)0x31;
        public const ushort VK_2 = (ushort)0x32;
        public const ushort VK_3 = (ushort)0x33;
        public const ushort VK_4 = (ushort)0x34;
        public const ushort VK_5 = (ushort)0x35;
        public const ushort VK_6 = (ushort)0x36;
        public const ushort VK_7 = (ushort)0x37;
        public const ushort VK_8 = (ushort)0x38;
        public const ushort VK_9 = (ushort)0x39;
        public const ushort VK_A = (ushort)0x41;
        public const ushort VK_B = (ushort)0x42;
        public const ushort VK_C = (ushort)0x43;
        public const ushort VK_D = (ushort)0x44;
        public const ushort VK_E = (ushort)0x45;
        public const ushort VK_F = (ushort)0x46;
        public const ushort VK_G = (ushort)0x47;
        public const ushort VK_H = (ushort)0x48;
        public const ushort VK_I = (ushort)0x49;
        public const ushort VK_J = (ushort)0x4A;
        public const ushort VK_K = (ushort)0x4B;
        public const ushort VK_L = (ushort)0x4C;
        public const ushort VK_M = (ushort)0x4D;
        public const ushort VK_N = (ushort)0x4E;
        public const ushort VK_O = (ushort)0x4F;
        public const ushort VK_P = (ushort)0x50;
        public const ushort VK_Q = (ushort)0x51;
        public const ushort VK_R = (ushort)0x52;
        public const ushort VK_S = (ushort)0x53;
        public const ushort VK_T = (ushort)0x54;
        public const ushort VK_U = (ushort)0x55;
        public const ushort VK_V = (ushort)0x56;
        public const ushort VK_W = (ushort)0x57;
        public const ushort VK_X = (ushort)0x58;
        public const ushort VK_Y = (ushort)0x59;
        public const ushort VK_Z = (ushort)0x5A;
        public const ushort VK_LWIN = (ushort)0x5B;
        public const ushort VK_RWIN = (ushort)0x5C;
        public const ushort VK_APPS = (ushort)0x5D;
        public const ushort VK_SLEEP = (ushort)0x5F;
        public const ushort VK_NUMPAD0 = (ushort)0x60;
        public const ushort VK_NUMPAD1 = (ushort)0x61;
        public const ushort VK_NUMPAD2 = (ushort)0x62;
        public const ushort VK_NUMPAD3 = (ushort)0x63;
        public const ushort VK_NUMPAD4 = (ushort)0x64;
        public const ushort VK_NUMPAD5 = (ushort)0x65;
        public const ushort VK_NUMPAD6 = (ushort)0x66;
        public const ushort VK_NUMPAD7 = (ushort)0x67;
        public const ushort VK_NUMPAD8 = (ushort)0x68;
        public const ushort VK_NUMPAD9 = (ushort)0x69;
        public const ushort VK_MULTIPLY = (ushort)0x6A;
        public const ushort VK_ADD = (ushort)0x6B;
        public const ushort VK_SEPARATOR = (ushort)0x6C;
        public const ushort VK_SUBTRACT = (ushort)0x6D;
        public const ushort VK_DECIMAL = (ushort)0x6E;
        public const ushort VK_DIVIDE = (ushort)0x6F;
        public const ushort VK_F1 = (ushort)0x70;
        public const ushort VK_F2 = (ushort)0x71;
        public const ushort VK_F3 = (ushort)0x72;
        public const ushort VK_F4 = (ushort)0x73;
        public const ushort VK_F5 = (ushort)0x74;
        public const ushort VK_F6 = (ushort)0x75;
        public const ushort VK_F7 = (ushort)0x76;
        public const ushort VK_F8 = (ushort)0x77;
        public const ushort VK_F9 = (ushort)0x78;
        public const ushort VK_F10 = (ushort)0x79;
        public const ushort VK_F11 = (ushort)0x7A;
        public const ushort VK_F12 = (ushort)0x7B;
        public const ushort VK_F13 = (ushort)0x7C;
        public const ushort VK_F14 = (ushort)0x7D;
        public const ushort VK_F15 = (ushort)0x7E;
        public const ushort VK_F16 = (ushort)0x7F;
        public const ushort VK_F17 = (ushort)0x80;
        public const ushort VK_F18 = (ushort)0x81;
        public const ushort VK_F19 = (ushort)0x82;
        public const ushort VK_F20 = (ushort)0x83;
        public const ushort VK_F21 = (ushort)0x84;
        public const ushort VK_F22 = (ushort)0x85;
        public const ushort VK_F23 = (ushort)0x86;
        public const ushort VK_F24 = (ushort)0x87;
        public const ushort VK_NUMLOCK = (ushort)0x90;
        public const ushort VK_SCROLL = (ushort)0x91;
        public const ushort VK_LeftShift = (ushort)0xA0;
        public const ushort VK_RightShift = (ushort)0xA1;
        public const ushort VK_LeftControl = (ushort)0xA2;
        public const ushort VK_RightControl = (ushort)0xA3;
        public const ushort VK_LMENU = (ushort)0xA4;
        public const ushort VK_RMENU = (ushort)0xA5;
        public const ushort VK_BROWSER_BACK = (ushort)0xA6;
        public const ushort VK_BROWSER_FORWARD = (ushort)0xA7;
        public const ushort VK_BROWSER_REFRESH = (ushort)0xA8;
        public const ushort VK_BROWSER_STOP = (ushort)0xA9;
        public const ushort VK_BROWSER_SEARCH = (ushort)0xAA;
        public const ushort VK_BROWSER_FAVORITES = (ushort)0xAB;
        public const ushort VK_BROWSER_HOME = (ushort)0xAC;
        public const ushort VK_VOLUME_MUTE = (ushort)0xAD;
        public const ushort VK_VOLUME_DOWN = (ushort)0xAE;
        public const ushort VK_VOLUME_UP = (ushort)0xAF;
        public const ushort VK_MEDIA_NEXT_TRACK = (ushort)0xB0;
        public const ushort VK_MEDIA_PREV_TRACK = (ushort)0xB1;
        public const ushort VK_MEDIA_STOP = (ushort)0xB2;
        public const ushort VK_MEDIA_PLAY_PAUSE = (ushort)0xB3;
        public const ushort VK_LAUNCH_MAIL = (ushort)0xB4;
        public const ushort VK_LAUNCH_MEDIA_SELECT = (ushort)0xB5;
        public const ushort VK_LAUNCH_APP1 = (ushort)0xB6;
        public const ushort VK_LAUNCH_APP2 = (ushort)0xB7;
        public const ushort VK_OEM_1 = (ushort)0xBA;
        public const ushort VK_OEM_PLUS = (ushort)0xBB;
        public const ushort VK_OEM_COMMA = (ushort)0xBC;
        public const ushort VK_OEM_MINUS = (ushort)0xBD;
        public const ushort VK_OEM_PERIOD = (ushort)0xBE;
        public const ushort VK_OEM_2 = (ushort)0xBF;
        public const ushort VK_OEM_3 = (ushort)0xC0;
        public const ushort VK_OEM_4 = (ushort)0xDB;
        public const ushort VK_OEM_5 = (ushort)0xDC;
        public const ushort VK_OEM_6 = (ushort)0xDD;
        public const ushort VK_OEM_7 = (ushort)0xDE;
        public const ushort VK_OEM_8 = (ushort)0xDF;
        public const ushort VK_OEM_102 = (ushort)0xE2;
        public const ushort VK_PROCESSKEY = (ushort)0xE5;
        public const ushort VK_PACKET = (ushort)0xE7;
        public const ushort VK_ATTN = (ushort)0xF6;
        public const ushort VK_CRSEL = (ushort)0xF7;
        public const ushort VK_EXSEL = (ushort)0xF8;
        public const ushort VK_EREOF = (ushort)0xF9;
        public const ushort VK_PLAY = (ushort)0xFA;
        public const ushort VK_ZOOM = (ushort)0xFB;
        public const ushort VK_NONAME = (ushort)0xFC;
        public const ushort VK_PA1 = (ushort)0xFD;
        public const ushort VK_OEM_CLEAR = (ushort)0xFE;
        public const ushort S_LBUTTON = (ushort)0x0;
        public const ushort S_RBUTTON = 0;
        public const ushort S_CANCEL = 70;
        public const ushort S_MBUTTON = 0;
        public const ushort S_XBUTTON1 = 0;
        public const ushort S_XBUTTON2 = 0;
        public const ushort S_BACK = 14;
        public const ushort S_Tab = 15;
        public const ushort S_CLEAR = 76;
        public const ushort S_Return = 28;
        public const ushort S_SHIFT = 42;
        public const ushort S_CONTROL = 29;
        public const ushort S_MENU = 56;
        public const ushort S_PAUSE = 0;
        public const ushort S_CAPITAL = 58;
        public const ushort S_KANA = 0;
        public const ushort S_HANGEUL = 0;
        public const ushort S_HANGUL = 0;
        public const ushort S_JUNJA = 0;
        public const ushort S_FINAL = 0;
        public const ushort S_HANJA = 0;
        public const ushort S_KANJI = 0;
        public const ushort S_Escape = 1;
        public const ushort S_CONVERT = 0;
        public const ushort S_NONCONVERT = 0;
        public const ushort S_ACCEPT = 0;
        public const ushort S_MODECHANGE = 0;
        public const ushort S_Space = 57;
        public const ushort S_PRIOR = 73;
        public const ushort S_NEXT = 81;
        public const ushort S_END = 79;
        public const ushort S_HOME = 71;
        public const ushort S_LEFT = 75;
        public const ushort S_UP = 72;
        public const ushort S_RIGHT = 77;
        public const ushort S_DOWN = 80;
        public const ushort S_SELECT = 0;
        public const ushort S_PRINT = 0;
        public const ushort S_EXECUTE = 0;
        public const ushort S_SNAPSHOT = 84;
        public const ushort S_INSERT = 82;
        public const ushort S_DELETE = 83;
        public const ushort S_HELP = 99;
        public const ushort S_APOSTROPHE = 41;
        public const ushort S_0 = 11;
        public const ushort S_1 = 2;
        public const ushort S_2 = 3;
        public const ushort S_3 = 4;
        public const ushort S_4 = 5;
        public const ushort S_5 = 6;
        public const ushort S_6 = 7;
        public const ushort S_7 = 8;
        public const ushort S_8 = 9;
        public const ushort S_9 = 10;
        public const ushort S_A = 16;
        public const ushort S_B = 48;
        public const ushort S_C = 46;
        public const ushort S_D = 32;
        public const ushort S_E = 18;
        public const ushort S_F = 33;
        public const ushort S_G = 34;
        public const ushort S_H = 35;
        public const ushort S_I = 23;
        public const ushort S_J = 36;
        public const ushort S_K = 37;
        public const ushort S_L = 38;
        public const ushort S_M = 39;
        public const ushort S_N = 49;
        public const ushort S_O = 24;
        public const ushort S_P = 25;
        public const ushort S_Q = 30;
        public const ushort S_R = 19;
        public const ushort S_S = 31;
        public const ushort S_T = 20;
        public const ushort S_U = 22;
        public const ushort S_V = 47;
        public const ushort S_W = 44;
        public const ushort S_X = 45;
        public const ushort S_Y = 21;
        public const ushort S_Z = 17;
        public const ushort S_LWIN = 91;
        public const ushort S_RWIN = 92;
        public const ushort S_APPS = 93;
        public const ushort S_SLEEP = 95;
        public const ushort S_NUMPAD0 = 82;
        public const ushort S_NUMPAD1 = 79;
        public const ushort S_NUMPAD2 = 80;
        public const ushort S_NUMPAD3 = 81;
        public const ushort S_NUMPAD4 = 75;
        public const ushort S_NUMPAD5 = 76;
        public const ushort S_NUMPAD6 = 77;
        public const ushort S_NUMPAD7 = 71;
        public const ushort S_NUMPAD8 = 72;
        public const ushort S_NUMPAD9 = 73;
        public const ushort S_MULTIPLY = 55;
        public const ushort S_ADD = 78;
        public const ushort S_SEPARATOR = 0;
        public const ushort S_SUBTRACT = 74;
        public const ushort S_DECIMAL = 83;
        public const ushort S_DIVIDE = 53;
        public const ushort S_F1 = 59;
        public const ushort S_F2 = 60;
        public const ushort S_F3 = 61;
        public const ushort S_F4 = 62;
        public const ushort S_F5 = 63;
        public const ushort S_F6 = 64;
        public const ushort S_F7 = 65;
        public const ushort S_F8 = 66;
        public const ushort S_F9 = 67;
        public const ushort S_F10 = 68;
        public const ushort S_F11 = 87;
        public const ushort S_F12 = 88;
        public const ushort S_F13 = 100;
        public const ushort S_F14 = 101;
        public const ushort S_F15 = 102;
        public const ushort S_F16 = 103;
        public const ushort S_F17 = 104;
        public const ushort S_F18 = 105;
        public const ushort S_F19 = 106;
        public const ushort S_F20 = 107;
        public const ushort S_F21 = 108;
        public const ushort S_F22 = 109;
        public const ushort S_F23 = 110;
        public const ushort S_F24 = 118;
        public const ushort S_NUMLOCK = 69;
        public const ushort S_SCROLL = 70;
        public const ushort S_LeftShift = 42;
        public const ushort S_RightShift = 54;
        public const ushort S_LeftControl = 29;
        public const ushort S_RightControl = 29;
        public const ushort S_LMENU = 56;
        public const ushort S_RMENU = 56;
        public const ushort S_BROWSER_BACK = 106;
        public const ushort S_BROWSER_FORWARD = 105;
        public const ushort S_BROWSER_REFRESH = 103;
        public const ushort S_BROWSER_STOP = 104;
        public const ushort S_BROWSER_SEARCH = 101;
        public const ushort S_BROWSER_FAVORITES = 102;
        public const ushort S_BROWSER_HOME = 50;
        public const ushort S_VOLUME_MUTE = 32;
        public const ushort S_VOLUME_DOWN = 46;
        public const ushort S_VOLUME_UP = 48;
        public const ushort S_MEDIA_NEXT_TRACK = 25;
        public const ushort S_MEDIA_PREV_TRACK = 16;
        public const ushort S_MEDIA_STOP = 36;
        public const ushort S_MEDIA_PLAY_PAUSE = 34;
        public const ushort S_LAUNCH_MAIL = 108;
        public const ushort S_LAUNCH_MEDIA_SELECT = 109;
        public const ushort S_LAUNCH_APP1 = 107;
        public const ushort S_LAUNCH_APP2 = 33;
        public const ushort S_OEM_1 = 27;
        public const ushort S_OEM_PLUS = 13;
        public const ushort S_OEM_COMMA = 50;
        public const ushort S_OEM_MINUS = 0;
        public const ushort S_OEM_PERIOD = 51;
        public const ushort S_OEM_2 = 52;
        public const ushort S_OEM_3 = 40;
        public const ushort S_OEM_4 = 12;
        public const ushort S_OEM_5 = 43;
        public const ushort S_OEM_6 = 26;
        public const ushort S_OEM_7 = 41;
        public const ushort S_OEM_8 = 53;
        public const ushort S_OEM_102 = 86;
        public const ushort S_PROCESSKEY = 0;
        public const ushort S_PACKET = 0;
        public const ushort S_ATTN = 0;
        public const ushort S_CRSEL = 0;
        public const ushort S_EXSEL = 0;
        public const ushort S_EREOF = 93;
        public const ushort S_PLAY = 0;
        public const ushort S_ZOOM = 98;
        public const ushort S_NONAME = 0;
        public const ushort S_PA1 = 0;
        public const ushort S_OEM_CLEAR = 0;
        public static string drivertype;
        public static int[] wd = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
        public static int[] wu = { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
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
        public static void UnLoadKM()
        {
            SetKM("kmevent", false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
            SetKM("sendinput", false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false);
        }
        public static void SetKM(string KeyboardMouseDriverType, bool SendLeftClick, bool SendRightClick, bool SendMiddleClick, bool SendWheelUp, bool SendWheelDown, bool SendLeft, bool SendRight, bool SendUp, bool SendDown, bool SendLButton, bool SendRButton, bool SendCancel, bool SendMBUTTON, bool SendXBUTTON1, bool SendXBUTTON2, bool SendBack, bool SendTab, bool SendClear, bool SendReturn, bool SendSHIFT, bool SendCONTROL, bool SendMENU, bool SendPAUSE, bool SendCAPITAL, bool SendKANA, bool SendHANGEUL, bool SendHANGUL, bool SendJUNJA, bool SendFINAL, bool SendHANJA, bool SendKANJI, bool SendEscape, bool SendCONVERT, bool SendNONCONVERT, bool SendACCEPT, bool SendMODECHANGE, bool SendSpace, bool SendPRIOR, bool SendNEXT, bool SendEND, bool SendHOME, bool SendLEFT, bool SendUP, bool SendRIGHT, bool SendDOWN, bool SendSELECT, bool SendPRINT, bool SendEXECUTE, bool SendSNAPSHOT, bool SendINSERT, bool SendDELETE, bool SendHELP, bool SendAPOSTROPHE, bool Send0, bool Send1, bool Send2, bool Send3, bool Send4, bool Send5, bool Send6, bool Send7, bool Send8, bool Send9, bool SendA, bool SendB, bool SendC, bool SendD, bool SendE, bool SendF, bool SendG, bool SendH, bool SendI, bool SendJ, bool SendK, bool SendL, bool SendM, bool SendN, bool SendO, bool SendP, bool SendQ, bool SendR, bool SendS, bool SendT, bool SendU, bool SendV, bool SendW, bool SendX, bool SendY, bool SendZ, bool SendLWIN, bool SendRWIN, bool SendAPPS, bool SendSLEEP, bool SendNUMPAD0, bool SendNUMPAD1, bool SendNUMPAD2, bool SendNUMPAD3, bool SendNUMPAD4, bool SendNUMPAD5, bool SendNUMPAD6, bool SendNUMPAD7, bool SendNUMPAD8, bool SendNUMPAD9, bool SendMULTIPLY, bool SendADD, bool SendSEPARATOR, bool SendSUBTRACT, bool SendDECIMAL, bool SendDIVIDE, bool SendF1, bool SendF2, bool SendF3, bool SendF4, bool SendF5, bool SendF6, bool SendF7, bool SendF8, bool SendF9, bool SendF10, bool SendF11, bool SendF12, bool SendF13, bool SendF14, bool SendF15, bool SendF16, bool SendF17, bool SendF18, bool SendF19, bool SendF20, bool SendF21, bool SendF22, bool SendF23, bool SendF24, bool SendNUMLOCK, bool SendSCROLL, bool SendLeftShift, bool SendRightShift, bool SendLeftControl, bool SendRightControl, bool SendLMENU, bool SendRMENU)
        {
            drivertype = KeyboardMouseDriverType;
            valchanged(1, SendLeftClick);
            if (wd[1] == 1)
                mouseclickleft();
            if (wu[1] == 1)
                mouseclickleftF();
            valchanged(2, SendRightClick);
            if (wd[2] == 1)
                mouseclickright();
            if (wu[2] == 1)
                mouseclickrightF();
            valchanged(3, SendMiddleClick);
            if (wd[3] == 1)
                mouseclickmiddle();
            if (wu[3] == 1)
                mouseclickmiddleF();
            valchanged(4, SendWheelUp);
            if (wd[4] == 1)
                mousewheelup();
            valchanged(5, SendWheelDown);
            if (wd[5] == 1)
                mousewheeldown();
            valchanged(6, SendLeft);
            if (wd[6] == 1)
                keyboardArrows(VK_LEFT, S_LEFT);
            if (wu[6] == 1)
                keyboardArrowsF(VK_LEFT, S_LEFT);
            valchanged(7, SendRight);
            if (wd[7] == 1)
                keyboardArrows(VK_RIGHT, S_RIGHT);
            if (wu[7] == 1)
                keyboardArrowsF(VK_RIGHT, S_RIGHT);
            valchanged(8, SendUp);
            if (wd[8] == 1)
                keyboardArrows(VK_UP, S_UP);
            if (wu[8] == 1)
                keyboardArrowsF(VK_UP, S_UP);
            valchanged(9, SendDown);
            if (wd[9] == 1)
                keyboardArrows(VK_DOWN, S_DOWN);
            if (wu[9] == 1)
                keyboardArrowsF(VK_DOWN, S_DOWN);
            valchanged(10, SendLButton);
            if (wd[10] == 1)
                keyboard(VK_LBUTTON, S_LBUTTON);
            if (wu[10] == 1)
                keyboardF(VK_LBUTTON, S_LBUTTON);
            valchanged(11, SendRButton);
            if (wd[11] == 1)
                keyboard(VK_RBUTTON, S_RBUTTON);
            if (wu[11] == 1)
                keyboardF(VK_RBUTTON, S_RBUTTON);
            valchanged(12, SendCancel);
            if (wd[12] == 1)
                keyboard(VK_CANCEL, S_CANCEL);
            if (wu[12] == 1)
                keyboardF(VK_CANCEL, S_CANCEL);
            valchanged(13, SendMBUTTON);
            if (wd[13] == 1)
                keyboard(VK_MBUTTON, S_MBUTTON);
            if (wu[13] == 1)
                keyboardF(VK_MBUTTON, S_MBUTTON);
            valchanged(14, SendXBUTTON1);
            if (wd[14] == 1)
                keyboard(VK_XBUTTON1, S_XBUTTON1);
            if (wu[14] == 1)
                keyboardF(VK_XBUTTON1, S_XBUTTON1);
            valchanged(15, SendXBUTTON2);
            if (wd[15] == 1)
                keyboard(VK_XBUTTON2, S_XBUTTON2);
            if (wu[15] == 1)
                keyboardF(VK_XBUTTON2, S_XBUTTON2);
            valchanged(16, SendBack);
            if (wd[16] == 1)
                keyboard(VK_BACK, S_BACK);
            if (wu[16] == 1)
                keyboardF(VK_BACK, S_BACK);
            valchanged(17, SendTab);
            if (wd[17] == 1)
                keyboard(VK_Tab, S_Tab);
            if (wu[17] == 1)
                keyboardF(VK_Tab, S_Tab);
            valchanged(18, SendClear);
            if (wd[18] == 1)
                keyboard(VK_CLEAR, S_CLEAR);
            if (wu[18] == 1)
                keyboardF(VK_CLEAR, S_CLEAR);
            valchanged(19, SendReturn);
            if (wd[19] == 1)
                keyboard(VK_Return, S_Return);
            if (wu[19] == 1)
                keyboardF(VK_Return, S_Return);
            valchanged(20, SendSHIFT);
            if (wd[20] == 1)
                keyboard(VK_SHIFT, S_SHIFT);
            if (wu[20] == 1)
                keyboardF(VK_SHIFT, S_SHIFT);
            valchanged(21, SendCONTROL);
            if (wd[21] == 1)
                keyboard(VK_CONTROL, S_CONTROL);
            if (wu[21] == 1)
                keyboardF(VK_CONTROL, S_CONTROL);
            valchanged(22, SendMENU);
            if (wd[22] == 1)
                keyboard(VK_MENU, S_MENU);
            if (wu[22] == 1)
                keyboardF(VK_MENU, S_MENU);
            valchanged(23, SendPAUSE);
            if (wd[23] == 1)
                keyboard(VK_PAUSE, S_PAUSE);
            if (wu[23] == 1)
                keyboardF(VK_PAUSE, S_PAUSE);
            valchanged(24, SendCAPITAL);
            if (wd[24] == 1)
                keyboard(VK_CAPITAL, S_CAPITAL);
            if (wu[24] == 1)
                keyboardF(VK_CAPITAL, S_CAPITAL);
            valchanged(25, SendKANA);
            if (wd[25] == 1)
                keyboard(VK_KANA, S_KANA);
            if (wu[25] == 1)
                keyboardF(VK_KANA, S_KANA);
            valchanged(26, SendHANGEUL);
            if (wd[26] == 1)
                keyboard(VK_HANGEUL, S_HANGEUL);
            if (wu[26] == 1)
                keyboardF(VK_HANGEUL, S_HANGEUL);
            valchanged(27, SendHANGUL);
            if (wd[27] == 1)
                keyboard(VK_HANGUL, S_HANGUL);
            if (wu[27] == 1)
                keyboardF(VK_HANGUL, S_HANGUL);
            valchanged(28, SendJUNJA);
            if (wd[28] == 1)
                keyboard(VK_JUNJA, S_JUNJA);
            if (wu[28] == 1)
                keyboardF(VK_JUNJA, S_JUNJA);
            valchanged(29, SendFINAL);
            if (wd[29] == 1)
                keyboard(VK_FINAL, S_FINAL);
            if (wu[29] == 1)
                keyboardF(VK_FINAL, S_FINAL);
            valchanged(30, SendHANJA);
            if (wd[30] == 1)
                keyboard(VK_HANJA, S_HANJA);
            if (wu[30] == 1)
                keyboardF(VK_HANJA, S_HANJA);
            valchanged(31, SendKANJI);
            if (wd[31] == 1)
                keyboard(VK_KANJI, S_KANJI);
            if (wu[31] == 1)
                keyboardF(VK_KANJI, S_KANJI);
            valchanged(32, SendEscape);
            if (wd[32] == 1)
                keyboard(VK_Escape, S_Escape);
            if (wu[32] == 1)
                keyboardF(VK_Escape, S_Escape);
            valchanged(33, SendCONVERT);
            if (wd[33] == 1)
                keyboard(VK_CONVERT, S_CONVERT);
            if (wu[33] == 1)
                keyboardF(VK_CONVERT, S_CONVERT);
            valchanged(34, SendNONCONVERT);
            if (wd[34] == 1)
                keyboard(VK_NONCONVERT, S_NONCONVERT);
            if (wu[34] == 1)
                keyboardF(VK_NONCONVERT, S_NONCONVERT);
            valchanged(35, SendACCEPT);
            if (wd[35] == 1)
                keyboard(VK_ACCEPT, S_ACCEPT);
            if (wu[35] == 1)
                keyboardF(VK_ACCEPT, S_ACCEPT);
            valchanged(36, SendMODECHANGE);
            if (wd[36] == 1)
                keyboard(VK_MODECHANGE, S_MODECHANGE);
            if (wu[36] == 1)
                keyboardF(VK_MODECHANGE, S_MODECHANGE);
            valchanged(37, SendSpace);
            if (wd[37] == 1)
                keyboard(VK_Space, S_Space);
            if (wu[37] == 1)
                keyboardF(VK_Space, S_Space);
            valchanged(38, SendPRIOR);
            if (wd[38] == 1)
                keyboard(VK_PRIOR, S_PRIOR);
            if (wu[38] == 1)
                keyboardF(VK_PRIOR, S_PRIOR);
            valchanged(39, SendNEXT);
            if (wd[39] == 1)
                keyboard(VK_NEXT, S_NEXT);
            if (wu[39] == 1)
                keyboardF(VK_NEXT, S_NEXT);
            valchanged(40, SendEND);
            if (wd[40] == 1)
                keyboard(VK_END, S_END);
            if (wu[40] == 1)
                keyboardF(VK_END, S_END);
            valchanged(41, SendHOME);
            if (wd[41] == 1)
                keyboard(VK_HOME, S_HOME);
            if (wu[41] == 1)
                keyboardF(VK_HOME, S_HOME);
            valchanged(42, SendLEFT);
            if (wd[42] == 1)
                keyboard(VK_LEFT, S_LEFT);
            if (wu[42] == 1)
                keyboardF(VK_LEFT, S_LEFT);
            valchanged(43, SendUP);
            if (wd[43] == 1)
                keyboard(VK_UP, S_UP);
            if (wu[43] == 1)
                keyboardF(VK_UP, S_UP);
            valchanged(44, SendRIGHT);
            if (wd[44] == 1)
                keyboard(VK_RIGHT, S_RIGHT);
            if (wu[44] == 1)
                keyboardF(VK_RIGHT, S_RIGHT);
            valchanged(45, SendDOWN);
            if (wd[45] == 1)
                keyboard(VK_DOWN, S_DOWN);
            if (wu[45] == 1)
                keyboardF(VK_DOWN, S_DOWN);
            valchanged(46, SendSELECT);
            if (wd[46] == 1)
                keyboard(VK_SELECT, S_SELECT);
            if (wu[46] == 1)
                keyboardF(VK_SELECT, S_SELECT);
            valchanged(47, SendPRINT);
            if (wd[47] == 1)
                keyboard(VK_PRINT, S_PRINT);
            if (wu[47] == 1)
                keyboardF(VK_PRINT, S_PRINT);
            valchanged(48, SendEXECUTE);
            if (wd[48] == 1)
                keyboard(VK_EXECUTE, S_EXECUTE);
            if (wu[48] == 1)
                keyboardF(VK_EXECUTE, S_EXECUTE);
            valchanged(49, SendSNAPSHOT);
            if (wd[49] == 1)
                keyboard(VK_SNAPSHOT, S_SNAPSHOT);
            if (wu[49] == 1)
                keyboardF(VK_SNAPSHOT, S_SNAPSHOT);
            valchanged(50, SendINSERT);
            if (wd[50] == 1)
                keyboard(VK_INSERT, S_INSERT);
            if (wu[50] == 1)
                keyboardF(VK_INSERT, S_INSERT);
            valchanged(51, SendDELETE);
            if (wd[51] == 1)
                keyboard(VK_DELETE, S_DELETE);
            if (wu[51] == 1)
                keyboardF(VK_DELETE, S_DELETE);
            valchanged(52, SendHELP);
            if (wd[52] == 1)
                keyboard(VK_HELP, S_HELP);
            if (wu[52] == 1)
                keyboardF(VK_HELP, S_HELP);
            valchanged(53, SendAPOSTROPHE);
            if (wd[53] == 1)
                keyboard(VK_APOSTROPHE, S_APOSTROPHE);
            if (wu[53] == 1)
                keyboardF(VK_APOSTROPHE, S_APOSTROPHE);
            valchanged(54, Send0);
            if (wd[54] == 1)
                keyboard(VK_0, S_0);
            if (wu[54] == 1)
                keyboardF(VK_0, S_0);
            valchanged(55, Send1);
            if (wd[55] == 1)
                keyboard(VK_1, S_1);
            if (wu[55] == 1)
                keyboardF(VK_1, S_1);
            valchanged(56, Send2);
            if (wd[56] == 1)
                keyboard(VK_2, S_2);
            if (wu[56] == 1)
                keyboardF(VK_2, S_2);
            valchanged(57, Send3);
            if (wd[57] == 1)
                keyboard(VK_3, S_3);
            if (wu[57] == 1)
                keyboardF(VK_3, S_3);
            valchanged(58, Send4);
            if (wd[58] == 1)
                keyboard(VK_4, S_4);
            if (wu[58] == 1)
                keyboardF(VK_4, S_4);
            valchanged(59, Send5);
            if (wd[59] == 1)
                keyboard(VK_5, S_5);
            if (wu[59] == 1)
                keyboardF(VK_5, S_5);
            valchanged(60, Send6);
            if (wd[60] == 1)
                keyboard(VK_6, S_6);
            if (wu[60] == 1)
                keyboardF(VK_6, S_6);
            valchanged(61, Send7);
            if (wd[61] == 1)
                keyboard(VK_7, S_7);
            if (wu[61] == 1)
                keyboardF(VK_7, S_7);
            valchanged(62, Send8);
            if (wd[62] == 1)
                keyboard(VK_8, S_8);
            if (wu[62] == 1)
                keyboardF(VK_8, S_8);
            valchanged(63, Send9);
            if (wd[63] == 1)
                keyboard(VK_9, S_9);
            if (wu[63] == 1)
                keyboardF(VK_9, S_9);
            valchanged(64, SendA);
            if (wd[64] == 1)
                keyboard(VK_A, S_A);
            if (wu[64] == 1)
                keyboardF(VK_A, S_A);
            valchanged(65, SendB);
            if (wd[65] == 1)
                keyboard(VK_B, S_B);
            if (wu[65] == 1)
                keyboardF(VK_B, S_B);
            valchanged(66, SendC);
            if (wd[66] == 1)
                keyboard(VK_C, S_C);
            if (wu[66] == 1)
                keyboardF(VK_C, S_C);
            valchanged(67, SendD);
            if (wd[67] == 1)
                keyboard(VK_D, S_D);
            if (wu[67] == 1)
                keyboardF(VK_D, S_D);
            valchanged(68, SendE);
            if (wd[68] == 1)
                keyboard(VK_E, S_E);
            if (wu[68] == 1)
                keyboardF(VK_E, S_E);
            valchanged(69, SendF);
            if (wd[69] == 1)
                keyboard(VK_F, S_F);
            if (wu[69] == 1)
                keyboardF(VK_F, S_F);
            valchanged(70, SendG);
            if (wd[70] == 1)
                keyboard(VK_G, S_G);
            if (wu[70] == 1)
                keyboardF(VK_G, S_G);
            valchanged(71, SendH);
            if (wd[71] == 1)
                keyboard(VK_H, S_H);
            if (wu[71] == 1)
                keyboardF(VK_H, S_H);
            valchanged(72, SendI);
            if (wd[72] == 1)
                keyboard(VK_I, S_I);
            if (wu[72] == 1)
                keyboardF(VK_I, S_I);
            valchanged(73, SendJ);
            if (wd[73] == 1)
                keyboard(VK_J, S_J);
            if (wu[73] == 1)
                keyboardF(VK_J, S_J);
            valchanged(74, SendK);
            if (wd[74] == 1)
                keyboard(VK_K, S_K);
            if (wu[74] == 1)
                keyboardF(VK_K, S_K);
            valchanged(75, SendL);
            if (wd[75] == 1)
                keyboard(VK_L, S_L);
            if (wu[75] == 1)
                keyboardF(VK_L, S_L);
            valchanged(76, SendM);
            if (wd[76] == 1)
                keyboard(VK_M, S_M);
            if (wu[76] == 1)
                keyboardF(VK_M, S_M);
            valchanged(77, SendN);
            if (wd[77] == 1)
                keyboard(VK_N, S_N);
            if (wu[77] == 1)
                keyboardF(VK_N, S_N);
            valchanged(78, SendO);
            if (wd[78] == 1)
                keyboard(VK_O, S_O);
            if (wu[78] == 1)
                keyboardF(VK_O, S_O);
            valchanged(79, SendP);
            if (wd[79] == 1)
                keyboard(VK_P, S_P);
            if (wu[79] == 1)
                keyboardF(VK_P, S_P);
            valchanged(80, SendQ);
            if (wd[80] == 1)
                keyboard(VK_Q, S_Q);
            if (wu[80] == 1)
                keyboardF(VK_Q, S_Q);
            valchanged(81, SendR);
            if (wd[81] == 1)
                keyboard(VK_R, S_R);
            if (wu[81] == 1)
                keyboardF(VK_R, S_R);
            valchanged(82, SendS);
            if (wd[82] == 1)
                keyboard(VK_S, S_S);
            if (wu[82] == 1)
                keyboardF(VK_S, S_S);
            valchanged(83, SendT);
            if (wd[83] == 1)
                keyboard(VK_T, S_T);
            if (wu[83] == 1)
                keyboardF(VK_T, S_T);
            valchanged(84, SendU);
            if (wd[84] == 1)
                keyboard(VK_U, S_U);
            if (wu[84] == 1)
                keyboardF(VK_U, S_U);
            valchanged(85, SendV);
            if (wd[85] == 1)
                keyboard(VK_V, S_V);
            if (wu[85] == 1)
                keyboardF(VK_V, S_V);
            valchanged(86, SendW);
            if (wd[86] == 1)
                keyboard(VK_W, S_W);
            if (wu[86] == 1)
                keyboardF(VK_W, S_W);
            valchanged(87, SendX);
            if (wd[87] == 1)
                keyboard(VK_X, S_X);
            if (wu[87] == 1)
                keyboardF(VK_X, S_X);
            valchanged(88, SendY);
            if (wd[88] == 1)
                keyboard(VK_Y, S_Y);
            if (wu[88] == 1)
                keyboardF(VK_Y, S_Y);
            valchanged(89, SendZ);
            if (wd[89] == 1)
                keyboard(VK_Z, S_Z);
            if (wu[89] == 1)
                keyboardF(VK_Z, S_Z);
            valchanged(90, SendLWIN);
            if (wd[90] == 1)
                keyboard(VK_LWIN, S_LWIN);
            if (wu[90] == 1)
                keyboardF(VK_LWIN, S_LWIN);
            valchanged(91, SendRWIN);
            if (wd[91] == 1)
                keyboard(VK_RWIN, S_RWIN);
            if (wu[91] == 1)
                keyboardF(VK_RWIN, S_RWIN);
            valchanged(92, SendAPPS);
            if (wd[92] == 1)
                keyboard(VK_APPS, S_APPS);
            if (wu[92] == 1)
                keyboardF(VK_APPS, S_APPS);
            valchanged(93, SendSLEEP);
            if (wd[93] == 1)
                keyboard(VK_SLEEP, S_SLEEP);
            if (wu[93] == 1)
                keyboardF(VK_SLEEP, S_SLEEP);
            valchanged(94, SendNUMPAD0);
            if (wd[94] == 1)
                keyboard(VK_NUMPAD0, S_NUMPAD0);
            if (wu[94] == 1)
                keyboardF(VK_NUMPAD0, S_NUMPAD0);
            valchanged(95, SendNUMPAD1);
            if (wd[95] == 1)
                keyboard(VK_NUMPAD1, S_NUMPAD1);
            if (wu[95] == 1)
                keyboardF(VK_NUMPAD1, S_NUMPAD1);
            valchanged(96, SendNUMPAD2);
            if (wd[96] == 1)
                keyboard(VK_NUMPAD2, S_NUMPAD2);
            if (wu[96] == 1)
                keyboardF(VK_NUMPAD2, S_NUMPAD2);
            valchanged(97, SendNUMPAD3);
            if (wd[97] == 1)
                keyboard(VK_NUMPAD3, S_NUMPAD3);
            if (wu[97] == 1)
                keyboardF(VK_NUMPAD3, S_NUMPAD3);
            valchanged(98, SendNUMPAD4);
            if (wd[98] == 1)
                keyboard(VK_NUMPAD4, S_NUMPAD4);
            if (wu[98] == 1)
                keyboardF(VK_NUMPAD4, S_NUMPAD4);
            valchanged(99, SendNUMPAD5);
            if (wd[99] == 1)
                keyboard(VK_NUMPAD5, S_NUMPAD5);
            if (wu[99] == 1)
                keyboardF(VK_NUMPAD5, S_NUMPAD5);
            valchanged(100, SendNUMPAD6);
            if (wd[100] == 1)
                keyboard(VK_NUMPAD6, S_NUMPAD6);
            if (wu[100] == 1)
                keyboardF(VK_NUMPAD6, S_NUMPAD6);
            valchanged(101, SendNUMPAD7);
            if (wd[101] == 1)
                keyboard(VK_NUMPAD7, S_NUMPAD7);
            if (wu[101] == 1)
                keyboardF(VK_NUMPAD7, S_NUMPAD7);
            valchanged(102, SendNUMPAD8);
            if (wd[102] == 1)
                keyboard(VK_NUMPAD8, S_NUMPAD8);
            if (wu[102] == 1)
                keyboardF(VK_NUMPAD8, S_NUMPAD8);
            valchanged(103, SendNUMPAD9);
            if (wd[103] == 1)
                keyboard(VK_NUMPAD9, S_NUMPAD9);
            if (wu[103] == 1)
                keyboardF(VK_NUMPAD9, S_NUMPAD9);
            valchanged(104, SendMULTIPLY);
            if (wd[104] == 1)
                keyboard(VK_MULTIPLY, S_MULTIPLY);
            if (wu[104] == 1)
                keyboardF(VK_MULTIPLY, S_MULTIPLY);
            valchanged(105, SendADD);
            if (wd[105] == 1)
                keyboard(VK_ADD, S_ADD);
            if (wu[105] == 1)
                keyboardF(VK_ADD, S_ADD);
            valchanged(106, SendSEPARATOR);
            if (wd[106] == 1)
                keyboard(VK_SEPARATOR, S_SEPARATOR);
            if (wu[106] == 1)
                keyboardF(VK_SEPARATOR, S_SEPARATOR);
            valchanged(107, SendSUBTRACT);
            if (wd[107] == 1)
                keyboard(VK_SUBTRACT, S_SUBTRACT);
            if (wu[107] == 1)
                keyboardF(VK_SUBTRACT, S_SUBTRACT);
            valchanged(108, SendDECIMAL);
            if (wd[108] == 1)
                keyboard(VK_DECIMAL, S_DECIMAL);
            if (wu[108] == 1)
                keyboardF(VK_DECIMAL, S_DECIMAL);
            valchanged(109, SendDIVIDE);
            if (wd[109] == 1)
                keyboard(VK_DIVIDE, S_DIVIDE);
            if (wu[109] == 1)
                keyboardF(VK_DIVIDE, S_DIVIDE);
            valchanged(110, SendF1);
            if (wd[110] == 1)
                keyboard(VK_F1, S_F1);
            if (wu[110] == 1)
                keyboardF(VK_F1, S_F1);
            valchanged(111, SendF2);
            if (wd[111] == 1)
                keyboard(VK_F2, S_F2);
            if (wu[111] == 1)
                keyboardF(VK_F2, S_F2);
            valchanged(112, SendF3);
            if (wd[112] == 1)
                keyboard(VK_F3, S_F3);
            if (wu[112] == 1)
                keyboardF(VK_F3, S_F3);
            valchanged(113, SendF4);
            if (wd[113] == 1)
                keyboard(VK_F4, S_F4);
            if (wu[113] == 1)
                keyboardF(VK_F4, S_F4);
            valchanged(114, SendF5);
            if (wd[114] == 1)
                keyboard(VK_F5, S_F5);
            if (wu[114] == 1)
                keyboardF(VK_F5, S_F5);
            valchanged(115, SendF6);
            if (wd[115] == 1)
                keyboard(VK_F6, S_F6);
            if (wu[115] == 1)
                keyboardF(VK_F6, S_F6);
            valchanged(116, SendF7);
            if (wd[116] == 1)
                keyboard(VK_F7, S_F7);
            if (wu[116] == 1)
                keyboardF(VK_F7, S_F7);
            valchanged(117, SendF8);
            if (wd[117] == 1)
                keyboard(VK_F8, S_F8);
            if (wu[117] == 1)
                keyboardF(VK_F8, S_F8);
            valchanged(118, SendF9);
            if (wd[118] == 1)
                keyboard(VK_F9, S_F9);
            if (wu[118] == 1)
                keyboardF(VK_F9, S_F9);
            valchanged(119, SendF10);
            if (wd[119] == 1)
                keyboard(VK_F10, S_F10);
            if (wu[119] == 1)
                keyboardF(VK_F10, S_F10);
            valchanged(120, SendF11);
            if (wd[120] == 1)
                keyboard(VK_F11, S_F11);
            if (wu[120] == 1)
                keyboardF(VK_F11, S_F11);
            valchanged(121, SendF12);
            if (wd[121] == 1)
                keyboard(VK_F12, S_F12);
            if (wu[121] == 1)
                keyboardF(VK_F12, S_F12);
            valchanged(122, SendF13);
            if (wd[122] == 1)
                keyboard(VK_F13, S_F13);
            if (wu[122] == 1)
                keyboardF(VK_F13, S_F13);
            valchanged(123, SendF14);
            if (wd[123] == 1)
                keyboard(VK_F14, S_F14);
            if (wu[123] == 1)
                keyboardF(VK_F14, S_F14);
            valchanged(124, SendF15);
            if (wd[124] == 1)
                keyboard(VK_F15, S_F15);
            if (wu[124] == 1)
                keyboardF(VK_F15, S_F15);
            valchanged(125, SendF16);
            if (wd[125] == 1)
                keyboard(VK_F16, S_F16);
            if (wu[125] == 1)
                keyboardF(VK_F16, S_F16);
            valchanged(126, SendF17);
            if (wd[126] == 1)
                keyboard(VK_F17, S_F17);
            if (wu[126] == 1)
                keyboardF(VK_F17, S_F17);
            valchanged(127, SendF18);
            if (wd[127] == 1)
                keyboard(VK_F18, S_F18);
            if (wu[127] == 1)
                keyboardF(VK_F18, S_F18);
            valchanged(128, SendF19);
            if (wd[128] == 1)
                keyboard(VK_F19, S_F19);
            if (wu[128] == 1)
                keyboardF(VK_F19, S_F19);
            valchanged(129, SendF20);
            if (wd[129] == 1)
                keyboard(VK_F20, S_F20);
            if (wu[129] == 1)
                keyboardF(VK_F20, S_F20);
            valchanged(130, SendF21);
            if (wd[130] == 1)
                keyboard(VK_F21, S_F21);
            if (wu[130] == 1)
                keyboardF(VK_F21, S_F21);
            valchanged(131, SendF22);
            if (wd[131] == 1)
                keyboard(VK_F22, S_F22);
            if (wu[131] == 1)
                keyboardF(VK_F22, S_F22);
            valchanged(132, SendF23);
            if (wd[132] == 1)
                keyboard(VK_F23, S_F23);
            if (wu[132] == 1)
                keyboardF(VK_F23, S_F23);
            valchanged(133, SendF24);
            if (wd[133] == 1)
                keyboard(VK_F24, S_F24);
            if (wu[133] == 1)
                keyboardF(VK_F24, S_F24);
            valchanged(134, SendNUMLOCK);
            if (wd[134] == 1)
                keyboard(VK_NUMLOCK, S_NUMLOCK);
            if (wu[134] == 1)
                keyboardF(VK_NUMLOCK, S_NUMLOCK);
            valchanged(135, SendSCROLL);
            if (wd[135] == 1)
                keyboard(VK_SCROLL, S_SCROLL);
            if (wu[135] == 1)
                keyboardF(VK_SCROLL, S_SCROLL);
            valchanged(136, SendLeftShift);
            if (wd[136] == 1)
                keyboard(VK_LeftShift, S_LeftShift);
            if (wu[136] == 1)
                keyboardF(VK_LeftShift, S_LeftShift);
            valchanged(137, SendRightShift);
            if (wd[137] == 1)
                keyboard(VK_RightShift, S_RightShift);
            if (wu[137] == 1)
                keyboardF(VK_RightShift, S_RightShift);
            valchanged(138, SendLeftControl);
            if (wd[138] == 1)
                keyboard(VK_LeftControl, S_LeftControl);
            if (wu[138] == 1)
                keyboardF(VK_LeftControl, S_LeftControl);
            valchanged(139, SendRightControl);
            if (wd[139] == 1)
                keyboard(VK_RightControl, S_RightControl);
            if (wu[139] == 1)
                keyboardF(VK_RightControl, S_RightControl);
            valchanged(140, SendLMENU);
            if (wd[140] == 1)
                keyboard(VK_LMENU, S_LMENU);
            if (wu[140] == 1)
                keyboardF(VK_LMENU, S_LMENU);
            valchanged(141, SendRMENU);
            if (wd[141] == 1)
                keyboard(VK_RMENU, S_RMENU);
            if (wu[141] == 1)
                keyboardF(VK_RMENU, S_RMENU);
        }
        public static void mouseclickleft()
        {
            if (drivertype == "sendinput")
                Task.Run(() => SendMouseEventButtonLeft());
            else
                Task.Run(() => LeftClick());
        }
        public static void mouseclickleftF()
        {
            if (drivertype == "sendinput")
                Task.Run(() => SendMouseEventButtonLeftF());
            else
                Task.Run(() => LeftClickF());
        }
        public static void mouseclickright()
        {
            if (drivertype == "sendinput")
                Task.Run(() => SendMouseEventButtonRight());
            else
                Task.Run(() => RightClick());
        }
        public static void mouseclickrightF()
        {
            if (drivertype == "sendinput")
                Task.Run(() => SendMouseEventButtonRightF());
            else
                Task.Run(() => RightClickF());
        }
        public static void mouseclickmiddle()
        {
            if (drivertype == "sendinput")
                Task.Run(() => SendMouseEventButtonMiddle());
            else
                Task.Run(() => MiddleClick());
        }
        public static void mouseclickmiddleF()
        {
            if (drivertype == "sendinput")
                Task.Run(() => SendMouseEventButtonMiddleF());
            else
                Task.Run(() => MiddleClickF());
        }
        public static void mousewheelup()
        {
            if (drivertype == "sendinput")
                Task.Run(() => SendMouseEventButtonWheelUp());
            else
                Task.Run(() => WheelUpF());
        }
        public static void mousewheeldown()
        {
            if (drivertype == "sendinput")
                Task.Run(() => SendMouseEventButtonWheelDown());
            else
                Task.Run(() => WheelDownF());
        }
        public static void keyboard(UInt16 bVk, UInt16 bScan)
        {
            if (drivertype == "sendinput")
                Task.Run(() => SendKey(bVk, bScan));
            else
                Task.Run(() => SimulateKeyDown(bVk, bScan));
        }
        public static void keyboardF(UInt16 bVk, UInt16 bScan)
        {
            if (drivertype == "sendinput")
                Task.Run(() => SendKeyF(bVk, bScan));
            else
                Task.Run(() => SimulateKeyUp(bVk, bScan));
        }
        public static void keyboardArrows(UInt16 bVk, UInt16 bScan)
        {
            if (drivertype == "sendinput")
                Task.Run(() => SendKeyArrows(bVk, bScan));
            else
                Task.Run(() => SimulateKeyDownArrows(bVk, bScan));
        }
        public static void keyboardArrowsF(UInt16 bVk, UInt16 bScan)
        {
            if (drivertype == "sendinput")
                Task.Run(() => SendKeyArrowsF(bVk, bScan));
            else
                Task.Run(() => SimulateKeyUpArrows(bVk, bScan));
        }
    }
}
