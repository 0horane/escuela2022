﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace TextEditor
{
    public static class AConsole
    {
        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern bool ReadConsoleInput(IntPtr hConsoleInput, [Out] INPUT_RECORD[] lpBuffer, int  nLength, out int lpNumberOfEventsRead);


        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/attributes/how-to-create-a-c-cpp-union-by-using-attributes
        //https://github.com/SiTox/07-SK-K-PM/blob/master/PacMan/KeyPressed.cs
        //Data structure for inputs
        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT_RECORD
        {
            public ushort EventType;
            public EVENT_RECORD Event;

        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEY_EVENT_RECORD
        {
            public bool bKeyDown;
            public ushort wRepeatCount;
            public ushort wVirtualKeyCode;
            public ushort wVirtualScanCode;
            public char UnicodeChar;
            public uint dwControlKeyState;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSE_EVENT_RECORD
        {
            public COORD dwMousePosition;
            public ushort dwButtonState;
            public ushort dwControlKeyState;
            public ushort dwEventFlags;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOW_BUFFER_SIZE_RECORD
        {
            public COORD dwSize;
        }
        private struct MENU_EVENT_RECORD
        {
            public uint dwSize;
        }
        private struct FOCUS_EVENT_RECORD
        {
            public bool dwSize;
        }


        [StructLayout(LayoutKind.Sequential)]
        private struct COORD
        {
            public short X;
            public short Y;
        }


        [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
        struct EVENT_RECORD
        {
            [System.Runtime.InteropServices.FieldOffset(0)]
            public KEY_EVENT_RECORD KeyEvent;
            [System.Runtime.InteropServices.FieldOffset(0)]
            public MOUSE_EVENT_RECORD MouseEvent;
            [System.Runtime.InteropServices.FieldOffset(0)]
            public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
            [System.Runtime.InteropServices.FieldOffset(0)]
            public MENU_EVENT_RECORD MenuEvent;
            [System.Runtime.InteropServices.FieldOffset(0)]
            public FOCUS_EVENT_RECORD FocusEvent;
        }


        private static IntPtr stdOut;
        private static IntPtr stdIn;

        static AConsole()
        {
            //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))

            stdOut = GetStdHandle(-11); //-11=stdout
            stdIn = GetStdHandle(-10); //-10=stdin

            GetConsoleMode(stdOut, out var outConsoleMode);
            //4=ENABLE_VIRTUAL_TERMINAL_PROCESSING 
            //16=ENABLE_MOUSE_INPUT 128=ENABLE_EXTENDED_FLAGS  64=ENABLE_QUICK_EDIT_MODE 
            // !(!p || q) inverted material conditional
            SetConsoleMode(stdOut,  outConsoleMode | 4 );
            SetConsoleMode(stdIn, ~(~(outConsoleMode | 128 | 16) | 64));


            //Environment.GetEnvironmentVariable("NO_COLOR")

        }

        public static void WriteLine(string istr)
        {
            Console.WriteLine(istr);
        }
        public static void Write(string istr)
        {
            Console.Write(istr);
        }



        public static void ReadInputs(uint howmany)
        {
            WriteLine("\u001b[34;45;9m\nsegjkhlsfjklgjkdfhskjsdglkuhdfskldgkhdfghuksdkulbsnilbhgvhblj,gsb\u001b[0m");
            int nRead = 0;
            INPUT_RECORD[] iRecord = new INPUT_RECORD[howmany];
            if (ReadConsoleInput(stdIn, iRecord, (int)howmany, out nRead))
            {
                foreach (INPUT_RECORD record in iRecord)
                {
                    switch (record.EventType)
                    {
                        case 0:
                            break;
                        case 1:
                            Write($"key {record.Event.KeyEvent.bKeyDown} {record.Event.KeyEvent.wRepeatCount} {record.Event.KeyEvent.wVirtualScanCode} {record.Event.KeyEvent.wVirtualKeyCode} {record.Event.KeyEvent.UnicodeChar} {record.Event.KeyEvent.dwControlKeyState}");
                            break;
                        case 2:
                            Write("Mouse");
                            break;
                        case 4:
                            Write("Window");
                            break;
                        case 16:
                            Write("focus");
                            break;
                        default:
                            break;
                    }
                    
                }
            } 
            
        }
    }
}

