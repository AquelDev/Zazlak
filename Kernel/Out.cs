using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.IO;

namespace Zazlak
{
    public static class Out // New Out v1.5
    {
        public static void WriteLine(object logText, ConsoleColor ColorOne = ConsoleColor.Green, String InitText = "» ", String Other = "")
        {
            if (Other != "")
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(InitText + "[" + Other + "] ");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(InitText);
            }

            Console.ForegroundColor = ColorOne;
            Console.WriteLine(logText);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void Write(object logText, ConsoleColor ColorOne = ConsoleColor.Green, String InitText = "» ")
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(InitText);

            Console.ForegroundColor = ColorOne;
            Console.Write(logText);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void WritePlain(string logText, ConsoleColor ColorOne = ConsoleColor.Gray)
        {
            Console.ForegroundColor = ColorOne;
            Console.WriteLine(logText);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void WriteBlank()
        {
            Console.WriteLine();
        }
    }
}