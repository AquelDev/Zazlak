using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Zazlak.Habbo.Extensions.Pathfinding;
using Zazlak.Storage;

namespace Zazlak.Habbo.Cache
{
    class Models
    {
        internal string Model;
        internal int DoorX;
        internal int DoorY;
        internal int DoorZ;
        internal int DoorDir;
        internal string HeightMap;
        internal int MapSizeX
        {
            get
            {
                return Lines[0].Length;
            }
        }
        internal int MapSizeY
        {
            get
            {
                return Lines.Count;
            }
        }
        internal string Items;
        internal bool ClubOnly;
        internal string Map;
        internal TileState[,] DefaultTiles;
        internal double[,] DefaultHeightMap;
        internal List<string> Lines = new List<string>();
        public static List<Models> RoomModels;
        public static Dictionary<string, Models> RoomModelByName;

        public static void InitModels()
        {
            try
            {
                RoomModels = new List<Models>();
                RoomModelByName = new Dictionary<string, Models>();

                MySQL Data = new MySQL(Init.ServerMySQL);
                Data.Query("SELECT * FROM rooms_models");

                foreach (DataRow Row in Data.Fetch_Array())
                {
                    Models M = new Models();
                    M.Model = (string)Row["model_name"];
                    M.DoorX = (int)Row["door_x"] - 1;
                    M.DoorY = (int)Row["door_y"];
                    M.DoorZ = (int)Row["door_z"];
                    M.DoorDir = (int)Row["door_dir"];

                    M.HeightMap = (string)Row["heightmap"];
                    M.Map = M.HeightMap;

                    M.Items = (string)Row["items"];
                    M.ClubOnly = Decoders.ConvertEnumToBool(Row["club_only"].ToString());
                    M.GenerateLines();

                    RoomModels.Add(M);
                    RoomModelByName.Add(M.Model, M);
                }
            }
            catch (Exception Error)
            {
                Out.WriteLine(Error.Message, ConsoleColor.DarkRed, "   ", "Habbo.Rooms.Models");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        private void GenerateLines()
        {
            try
            {
                Map.Replace(Convert.ToChar(10).ToString(), "").Split('\r').ToList().ForEach(Lines.Add);

                GetPremairParams();
                GetSecondairParams();
            }
            catch (Exception Error)
            {
                Out.WriteLine(Error.Message, ConsoleColor.DarkRed, "   ", "Habbo.Rooms.Models");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        public string GetPremairParams()
        {
            StringBuilder Builder = new StringBuilder();
            for (short y = 0; y < MapSizeY; y++)
            {
                string Line = Lines[y];

                Builder.AppendLine(Line);
            }

            return Builder.ToString().Replace(Convert.ToChar(10).ToString(), "");
        }

        public string GetSecondairParams()
        {
            DefaultTiles = new TileState[MapSizeX, MapSizeY];
            DefaultHeightMap = new double[MapSizeX, MapSizeY];

            StringBuilder Builder = new StringBuilder();

            for (short y = 0; y < MapSizeY; y++)
            {
                string FixedLine = string.Empty;

                for (short x = 0; x < MapSizeX; x++)
                {
                    string Character = Lines[y][x].ToString().Trim().ToLower();

                    double HeightMapChar = 0.0;

                    double.TryParse(Character, out HeightMapChar);

                    DefaultHeightMap[x, y] = HeightMapChar;

                    if (x == DoorX && y == DoorY)
                    {
                        DefaultTiles[x, y] = TileState.Walkable_laststep;

                        DefaultHeightMap[x, y] = DoorZ;

                        FixedLine += DoorZ;
                    }
                    else
                    {
                        if (Character == "x")
                        {
                            DefaultTiles[x, y] = TileState.Blocked;
                        }
                        else
                        {
                            DefaultTiles[x, y] = TileState.Walkable;
                        }

                        FixedLine += Character;
                    }
                }

                Builder.AppendLine(FixedLine);
            }

            return Builder.ToString().Replace(Convert.ToChar(10).ToString(), "");
        }

        private static bool isNumeric(string cadena)
        {
            try
            {
                int.Parse(cadena);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
