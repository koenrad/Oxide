﻿using System;
using System.Linq;

namespace Oxide.Core.ServerConsole
{
    public class ServerConsole
    {
        private readonly ConsoleWindow _console = new ConsoleWindow();
        private readonly ConsoleInput _input = new ConsoleInput();
        private bool _init;
        private float _nextUpdate;
        private float _nextTitleUpdate;

        public event Action<string> Input;

        public Func<string> Title;

        public Func<string> Status1Left;
        public Func<string> Status1Right;
        public Func<string> Status2Left;
        public Func<string> Status2Right;
        public Func<string> Status3Left;
        public Func<string> Status3Right;

        public Func<string, string[]> Completion
        {
            get { return _input.Completion; }
            set { _input.Completion = value; }
        }

        public ConsoleColor Status1LeftColor
        {
            get { return _input.StatusTextLeftColor[1]; }
            set { _input.StatusTextLeftColor[1] = value; }
        }
        public ConsoleColor Status1RightColor
        {
            get { return _input.StatusTextRightColor[1]; }
            set { _input.StatusTextRightColor[1] = value; }
        }
        public ConsoleColor Status2LeftColor
        {
            get { return _input.StatusTextLeftColor[2]; }
            set { _input.StatusTextLeftColor[2] = value; }
        }
        public ConsoleColor Status2RightColor
        {
            get { return _input.StatusTextRightColor[2]; }
            set { _input.StatusTextRightColor[2] = value; }
        }
        public ConsoleColor Status3RightColor
        {
            get { return _input.StatusTextRightColor[3]; }
            set { _input.StatusTextRightColor[3] = value; }
        }
        public ConsoleColor Status3LeftColor
        {
            get { return _input.StatusTextLeftColor[3]; }
            set { _input.StatusTextLeftColor[3] = value; }
        }

        private string title => Title != null ? Title() : null;

        private string status1Left => GetStatusValue(Status1Left);
        private string status1Right => GetStatusValue(Status1Right).PadLeft(_input.LineWidth - 1);
        private string status2Left => GetStatusValue(Status2Left);
        private string status2Right => GetStatusValue(Status2Right).PadLeft(_input.LineWidth - 1);
        private string status3Left => GetStatusValue(Status3Left);
        private string status3Right => GetStatusValue(Status3Right).PadLeft(_input.LineWidth - 1);

        private static string GetStatusValue(Func<string> status)
        {
            return status != null ? status() ?? string.Empty : "empty";
        }

        private static string GetStatusRight(int leftLength, string right)
        {
            return leftLength >= right.Length ? string.Empty : right.Substring(leftLength);
        }

        public void AddMessage(string message, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            _input.ClearLine(_input.StatusTextLeft.Length + message.Count(c => c == '\n'));
            Console.WriteLine(message);
            _input.RedrawInputLine();
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void OnDisable()
        {
            if (!_init) return;
            _input.OnInputText -= OnInputText;
            _console.Shutdown();
        }

        public void OnEnable()
        {
            if (!_console.Initialize()) return;
            _init = true;
            _input.OnInputText += OnInputText;
            _input.ClearLine(1);
            _input.ClearLine(Console.WindowHeight);
            for (var i = 0; i < Console.WindowHeight; i++)
            {
                Console.WriteLine();
            }
        }

        private void OnInputText(string obj)
        {
            if (Input != null) Input(obj);
        }

        public static void PrintColoured(params object[] objects)
        {
            if (Interface.Oxide.ServerConsole == null) return;
            Interface.Oxide.ServerConsole._input.ClearLine(Interface.Oxide.ServerConsole._input.StatusTextLeft.Length);
            for (var i = 0; i < objects.Length; i++)
            {
                if (i%2 != 0)
                    Console.Write((string) objects[i]);
                else
                    Console.ForegroundColor = (ConsoleColor) ((int) objects[i]);
            }
            if (Console.CursorLeft != 0)
                Console.CursorTop = Console.CursorTop + 1;
            Interface.Oxide.ServerConsole._input.RedrawInputLine();
        }

        public void Update()
        {
            UpdateStatus();
            _input.Update();
            if (_nextTitleUpdate > Interface.Oxide.Now) return;
            _nextTitleUpdate = Interface.Oxide.Now + 1f;
            _console.SetTitle(title);
        }

        private void UpdateStatus()
        {
            if (_nextUpdate > Interface.Oxide.Now) return;
            _nextUpdate = Interface.Oxide.Now + 0.66f;
            if (!_input.Valid) return;
            string left1 = status1Left, left2 = status2Left, left3 = status3Left;
            _input.StatusTextLeft[0] = string.Empty;
            _input.StatusTextLeft[1] = left1;
            _input.StatusTextLeft[2] = left2;
            _input.StatusTextLeft[3] = left3;
            _input.StatusTextRight[0] = string.Empty;
            _input.StatusTextRight[1] = GetStatusRight(left1.Length, status1Right);
            _input.StatusTextRight[2] = GetStatusRight(left2.Length, status2Right);
            _input.StatusTextRight[3] = GetStatusRight(left3.Length, status3Right);
        }
    }
}
