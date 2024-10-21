using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Policy;
using System.Threading;
using System.Runtime.Serialization.Formatters;
using System.Diagnostics.Eventing.Reader;

namespace LinkThePort
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Random random = new Random();
                ConsoleColor defaultBackColor = Console.BackgroundColor;
                ConsoleColor defaultForeColor = Console.ForegroundColor;
                ConsoleKeyInfo key = new ConsoleKeyInfo('g', ConsoleKey.G, false, false, false);
                int fuel = random.Next(20, 40);
                int[] deliveryPoint;
                int[,] ports;
                char[,] map = ReadMap("world_map.txt", out ports);
                deliveryPoint = ChooseDeliveryCoordinates(map);

                int startPortIndex = random.Next(0, ports.GetLength(0));
                int shipX = ports[startPortIndex, 0];
                int shipY = ports[startPortIndex, 1];

                while (true)
                {
                    Console.BufferHeight = 1000;
                    Console.BufferWidth = 1000;

                    Console.SetCursorPosition(0, 0);
                    Console.Clear();
                    DrawMap(map);
                    DrawActivePorts(ports, defaultBackColor);

                    Console.SetCursorPosition(deliveryPoint[0], deliveryPoint[1]);
                    Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    Console.Write('D');

                    MoveShip(key, ref shipX, ref shipY, map, deliveryPoint, ref fuel);

                    Console.SetCursorPosition(shipX, shipY);
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.Write('S');

                    Console.BackgroundColor = defaultBackColor;

                    if (IsInActivePort(shipX, shipY, map, ref ports))
                        fuel += random.Next(20, 40);

                    drawDownBoard(defaultBackColor, defaultForeColor, map, fuel);

                    Console.CursorVisible = false;

                    if (deliveryPoint[0] == shipX && deliveryPoint[1] == shipY)
                        break;
                    else if (fuel == 0)
                        break;

                    key = Console.ReadKey();
                }

                if (fuel > 0)
                {
                    Console.SetCursorPosition(0, map.GetLength(1) + 1);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Success! \n");
                    Console.ForegroundColor = defaultForeColor;
                }
                else
                {
                    Console.SetCursorPosition(0, map.GetLength(1) + 1);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Fail! \n");
                    Console.ForegroundColor = defaultForeColor;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("--- Do you wanna play again?(write 'yes' if you do) ---");
                if (Console.ReadLine() == "yes")
                {
                    Console.ForegroundColor = defaultForeColor;
                    continue;
                }
                else
                {
                    Console.ForegroundColor = defaultForeColor;
                    Console.Clear();
                    break;
                }
            }
        }

        private static void drawDownBoard(ConsoleColor defaultBackColor, ConsoleColor defaultForeColor, char[,] map, int fuel)
        {
            Console.SetCursorPosition(0, map.GetLength(1));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"--- Fuel: {fuel} --- | '");
            Console.ForegroundColor = defaultForeColor;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Write("o");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = defaultBackColor;
            Console.Write("' - Active port  | '");
            Console.ForegroundColor = defaultForeColor;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write("o");
            Console.BackgroundColor = defaultBackColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("' - Visited port  | '");
            Console.ForegroundColor = defaultForeColor;
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.Write("S");
            Console.BackgroundColor = defaultBackColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("' - Your transport (ship)  | '");
            Console.ForegroundColor = defaultForeColor;
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.Write("D");
            Console.BackgroundColor = defaultBackColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("' - Delivery point");
            Console.ForegroundColor = defaultForeColor;
        }
        private static bool IsInActivePort(int x, int y, char[,] map, ref int[,] ports)
        {
            for (int i = 0; i < ports.GetLength(0); i++)
                for (int j = 0; j < ports.GetLength(1); j++)
                    if (ports[i, j] == x && ports[i, j + 1] == y)
                        if (ports[i, j] == 0 && ports[i, j + 1] == 0)
                        {
                            return false;
                        }
                        else
                        {
                            ports = ChangeWidthOfList(ports, x, y, false);
                            return true;
                        }

            return false;
        }

        private static void DrawActivePorts(int[,] ports, ConsoleColor defaultColor)
        {
            for (int i = 0; i < ports.GetLength(0); i++)
            {
                if (ports[i, 0] == 0 && ports[i, 1] == 0)
                {
                    continue;
                }
                else
                {
                    Console.SetCursorPosition(ports[i, 0], ports[i, 1]);
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    Console.Write('o');
                }
            }
        }
        private static void MoveShip(ConsoleKeyInfo key, ref int shipX, ref int shipY, char[,] map, int[] dPoint, ref int fuel)
        {
            int[] direction = GetDirection(key);

            if (map[shipX + direction[0], shipY + direction[1]] == ' ' ||
                map[shipX + direction[0], shipY + direction[1]] == 'o' ||
                (shipX + direction[0] == dPoint[0] && shipY + direction[1] == dPoint[1]))
            {
                shipX += direction[0];
                shipY += direction[1];
                fuel--;
            }
        }

        private static int[] GetDirection(ConsoleKeyInfo pressedKey)
        {
            int[] direction = new int[2];

            if (pressedKey.Key == ConsoleKey.UpArrow)
                direction[1] = -1;
            else if (pressedKey.Key == ConsoleKey.DownArrow)
                direction[1] = 1;
            else if (pressedKey.Key == ConsoleKey.LeftArrow)
                direction[0] = -1;
            else if (pressedKey.Key == ConsoleKey.RightArrow)
                direction[0] = 1;

            return direction;
        }

        private static int[] ChooseDeliveryCoordinates(char[,] map)
        {
            Random rand = new Random();
            int[] pointCoordinates = new int[2];
            while (true)
            {
                pointCoordinates[0] = rand.Next(1, map.GetLength(0) - 1);
                pointCoordinates[1] = rand.Next(1, map.GetLength(1) - 1);

                if (map[pointCoordinates[0], pointCoordinates[1]] != ' ' &&
                    map[pointCoordinates[0], pointCoordinates[1]] != 'o')
                {
                    if (map[pointCoordinates[0], pointCoordinates[1] - 1] == ' ')
                        break;
                    else if (map[pointCoordinates[0], pointCoordinates[1] + 1] == ' ')
                        break;
                    else if (map[pointCoordinates[0] - 1, pointCoordinates[1]] == ' ')
                        break;
                    else if (map[pointCoordinates[0] + 1, pointCoordinates[1]] == ' ')
                        break;
                }
            }

            return pointCoordinates;

        }

        private static void DrawMap(char[,] map)
        {
            Console.SetCursorPosition(0, 0);
            ConsoleColor defaultColor = Console.BackgroundColor;

            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    if (map[x, y] == 'o')
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                    else
                        Console.BackgroundColor = defaultColor;

                    Console.Write(map[x, y]);

                    Console.BackgroundColor = defaultColor;
                }
                Console.Write("\n");
            }
        }

        private static char[,] ReadMap(string path, out int[,] cors)
        {
            string[] file = File.ReadAllLines(path);

            char[,] map = new char[GetMaxLengthOfLine(file), file.Length];
            cors = new int[0, 0];

            for (int x = 0; x < map.GetLength(0); x++)
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    map[x, y] = file[y][x];
                    if (map[x, y] == 'o')
                    {
                        cors = ChangeWidthOfList(cors, x, y);
                    }
                }

            return map;
        }

        private static int[,] ChangeWidthOfList(int[,] list, int x, int y, bool enlargeList = true)
        {
            if (enlargeList)
            {
                int[,] tempList = new int[list.GetLength(0) + 1, 2];

                for (int i = 0; i < list.GetLength(0); i++)
                    for (int j = 0; j < list.GetLength(1); j++)
                        tempList[i, j] = list[i, j];

                tempList[tempList.GetLength(0) - 1, 0] = x; tempList[tempList.GetLength(0) - 1, 1] = y;

                list = tempList;

                return list;
            }
            else
            {
                int[,] tempList = new int[list.GetLength(0), 2];

                for (int i = 0; i < list.GetLength(0); i++)
                    for (int j = 0; j < list.GetLength(1); j++)
                        if (j != 1)
                        {
                            if (list[i, j] == x && list[i, j + 1] == y)
                            {
                                tempList[i, j] = 0;
                            }
                            else
                            {
                                tempList[i, j] = list[i, j];
                            }
                        }
                        else
                        {
                            if (list[i, j] == y && list[i, j - 1] == x)
                            {
                                tempList[i, j] = 0;
                            }
                            else
                            {
                                tempList[i, j] = list[i, j];
                            }
                        }


                list = tempList;

                return list;
            }
        }

        private static int GetMaxLengthOfLine(string[] lines)
        {
            int maxLength = lines[0].Length;

            foreach (string line in lines)
            {
                if (line.Length > maxLength)
                {
                    maxLength = line.Length;
                }
            }

            return maxLength;
        }
    }
}
