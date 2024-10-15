using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Policy;
using System.Threading;
using System.Runtime.Serialization.Formatters;

namespace LinkThePort
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            ConsoleColor defaultBackColor = Console.BackgroundColor;
            ConsoleKeyInfo key = new ConsoleKeyInfo('g', ConsoleKey.G, false, false, false);
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

                Console.SetCursorPosition(deliveryPoint[0], deliveryPoint[1]);
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.Write('D');

                MoveShip(key, ref shipX, ref shipY, map, deliveryPoint);

                Console.SetCursorPosition(shipX, shipY);
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.Write('S');

                Console.BackgroundColor = defaultBackColor;

                Console.CursorVisible = false;

                if (deliveryPoint[0] == shipX && deliveryPoint[1] == shipY)
                    break;

                key = Console.ReadKey();
            }

            Console.SetCursorPosition(0, map.GetLength(1) + 1);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Success! ");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void MoveShip(ConsoleKeyInfo key, ref int shipX, ref int shipY, char[,] map, int[] dPoint)
        {
            int[] direction = GetDirection(key);

            if (map[shipX + direction[0], shipY + direction[1]] == ' ' ||
                map[shipX + direction[0], shipY + direction[1]] == 'o' ||
                (shipX + direction[0] == dPoint[0] && shipY + direction[1] == dPoint[1]))
            {
                shipX += direction[0];
                shipY += direction[1];
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
                        EnlargeList(ref cors, x, y);
                    }
                }

            return map;
        }

        private static int[,] EnlargeList(ref int[,] list, int x, int y)
        {
            int[,] tempList = new int[list.GetLength(0) + 1, 2];

            for (int i = 0; i < list.GetLength(0); i++)
                for (int j = 0; j < list.GetLength(1); j++)
                    tempList[i, j] = list[i, j];

            tempList[tempList.GetLength(0) - 1, 0] = x; tempList[tempList.GetLength(0) - 1, 1] = y;

            list = tempList;

            return list;
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
