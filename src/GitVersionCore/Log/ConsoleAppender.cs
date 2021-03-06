using System;
using System.Collections.Generic;

namespace GitVersion.Logging
{
    public class ConsoleAppender : ILogAppender
    {
        private readonly object _lock;
        private IDictionary<LogLevel, (ConsoleColor, ConsoleColor)> _palettes;
        public ConsoleAppender()
        {
            _lock = new object();
            _palettes = CreatePalette();
        }
        public void WriteTo(LogLevel level, string message)
        {
            lock (_lock)
            {
                try
                {
                    var (backgroundColor, foregroundColor) = _palettes[level];

                    Console.BackgroundColor = backgroundColor;
                    Console.ForegroundColor = foregroundColor;

                    if (level == LogLevel.Error)
                    {
                        Console.Error.Write(message);
                    }
                    else if (level != LogLevel.None)
                    {
                        Console.Write(message);
                    }
                }
                finally
                {
                    Console.ResetColor();
                    if (level == LogLevel.Error)
                    {
                        Console.Error.WriteLine();
                    }
                    else if (level != LogLevel.None)
                    {
                        Console.WriteLine();
                    }
                }
            }
        }

        private IDictionary<LogLevel, (ConsoleColor backgroundColor, ConsoleColor foregroundColor)> CreatePalette()
        {
            var background = Console.BackgroundColor;
            var palette = new Dictionary<LogLevel, (ConsoleColor, ConsoleColor)>
            {
                { LogLevel.Error, (ConsoleColor.DarkRed, ConsoleColor.White) },
                { LogLevel.Warn, (background, ConsoleColor.Yellow) },
                { LogLevel.Info, (background, ConsoleColor.White) },
                { LogLevel.Debug, (background, ConsoleColor.DarkGray) }
            };
            return palette;
        }
    }
}
