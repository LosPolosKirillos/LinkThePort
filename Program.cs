using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Policy;
using System.Threading;

namespace LinkThePort
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            Console.CursorVisible = false;
            ConsoleColor defaultBackColor = Console.BackgroundColor;
            int[,] ports;
            char[,] map = ReadMap("world_map.txt", out ports);

            int startPortIndex = random.Next(0, ports.GetLength(0));
            int shipX = ports[startPortIndex, 0];
            int shipY = ports[startPortIndex, 1];

            while (true)
            {
                Console.SetCursorPosition(0, 0);
                Console.Clear();
                DrawMap(map);

                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.SetCursorPosition(shipX, shipY);
                Console.Write('S');
                Console.BackgroundColor = defaultBackColor;

                Thread.Sleep(1000);
            }
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
