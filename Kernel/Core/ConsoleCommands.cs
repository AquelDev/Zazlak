using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zazlak.Kernel.Core
{
    public class ConsoleCommands
    {
        public static bool Logged = false;

        public static void Parse(string Input)
        {
            string[] Params = Input.Split(' ');
            Params[0] = Decoders.MD5Encrypt(Params[0]);

            switch (Params[0])
            {
                case "99dea78007133396a7b8ed70578ac6ae":
                    Out.WriteLine("Please enter Secret Key", ConsoleColor.DarkYellow, "", "Zazlak.¿?");
                    if (Decoders.MD5Encrypt(Console.ReadLine()) == "ec8956637a99787bd197eacd77acce5e")
                    {
                        Out.WriteLine("Welcome, enter commands", ConsoleColor.DarkYellow, "", "Zazlak.¿?");
                        Logged = true;
                    }
                    break;

                default:
                    Out.WriteLine("Error 404", ConsoleColor.DarkRed, "", "Azure");
                    break;
            }
        }

        public static void LOL(string Input)
        {
            try
            {
                string[] Params = Input.Split(':');
                Params[0] = Decoders.MD5Encrypt(Params[0]);
                Params[1] = Decoders.MD5Encrypt(Params[1]);

                switch (Params[0])
                {
                    case "dc30bc0c7914db5918da4263fce93ad2": //Clear
                        if (Params[1] == "822fd6a434f68f3127bcdf5fd696e413") // 3x13
                            Console.Clear();
                        break;

                    case "42c1b4656d363422315489b4611066ea": // ChangeColor
                        if (Params[1] == "c5330905d40da957ee71b763aeca7099") //0x23
                        {
                            try
                            {
                                if (Params[2] == "15")
                                    Console.ForegroundColor = ConsoleColor.Black;
                                else if (Params[2] == "14")
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                else if (Params[2] == "13")
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                else if (Params[2] == "12")
                                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                                else if (Params[2] == "11")
                                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                                else if (Params[2] == "10")
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                else if (Params[2] == "9")
                                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                                else if (Params[2] == "8")
                                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                else if (Params[2] == "7")
                                    Console.ForegroundColor = ConsoleColor.DarkRed;
                                else if (Params[2] == "6")
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                else if (Params[2] == "5")
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                else if (Params[2] == "4")
                                    Console.ForegroundColor = ConsoleColor.Green;
                                else if (Params[2] == "3")
                                    Console.ForegroundColor = ConsoleColor.Magenta;
                                else if (Params[2] == "2")
                                    Console.ForegroundColor = ConsoleColor.Red;
                                else if (Params[2] == "1")
                                    Console.ForegroundColor = ConsoleColor.White;
                                else if (Params[2] == "0")
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                            }
                            catch { Out.WriteLine("Error 505", ConsoleColor.DarkRed, "", "Zazlak.¿?");  }
                        }
                        break;
                    case "a0dbea706936984e02547bdfb2118320": //ReloadCatalog
                        if (Params[1] == "9b9948d2f39e39900eb5e9610a2911eb") //6x0
                        {
                            Habbo.Cache.Catalog.InitCatalogPages();
                            Habbo.Cache.Items.InitCatalogItems();
                        }
                        break;

                    default:
                        Out.WriteLine("Error 404", ConsoleColor.DarkRed, "", "Zazlak.¿?");
                        break;
                }
            }
            catch
            { Out.WriteLine("Error 404", ConsoleColor.DarkRed, "", "Zazlak.¿?"); }
        }

        public static string MergeParams(string[] Params, int Start)
        {
            StringBuilder MergedParams = new StringBuilder();

            for (int i = 0; i < Params.Length; i++)
            {
                if (i < Start)
                {
                    continue;
                }

                if (i > Start)
                {
                    MergedParams.Append(" ");
                }

                MergedParams.Append(Params[i]);
            }

            return MergedParams.ToString();
        }
    }
}
