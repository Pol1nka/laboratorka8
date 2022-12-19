using System;
using System.Collections.Generic;
// using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
// using System.Runtime.CompilerServices;
using System.Threading;

namespace Subtitles
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            TableDrawer.DrawTable();
            
            string[] initialStrings = File.ReadAllLines("subs.txt");
            SubtitlesLoader[] subtitles = new SubtitlesLoader[initialStrings.Length];
            for (int i = 0; i < initialStrings.Length; i++)
            {
                subtitles[i] = SubCreator.CreateSubtitle(initialStrings[i]);
            }
            
            SubtitleOutputer shower = new SubtitleOutputer(subtitles);
            shower.BeignWork();

            Console.ReadLine();
        }
    }

    public static class TableDrawer
    {
        public static int Width = 50;
        public static int Height = 20;
        private static int timeout = 1;

        public static void DrawTable()
        {
            // Отрисовка верхней границы таблицы
            for (int i = 0; i < Width + 2; i++)
            {
                Console.Write("─");
                Thread.Sleep(timeout);
            }
            Console.WriteLine();
            

            // Отрисовка вертикальных границ таблицы
            for (int i = 0; i < Height; i++)
            {
                Console.Write("│");
                for (int j = 0; j < Width; j++)
                {
                    Console.Write(" ");
                }
                Console.WriteLine("│");
                Thread.Sleep(timeout);
            }
            
            // Отрисовка верхней границы таблицы
            for (int i = 0; i < Width + 2; i++)
            {
                Console.Write("─");
                Thread.Sleep(timeout);
            }
            
        }
    }
    
    public class SubtitlesLoader
    {
        public int TimeStart { get; }
        public int TimeEnd { get; }
        public string Position { get; }
        public string Phrase { get; }
        public string TextColor { get; }

        public SubtitlesLoader(int timeStart, int timeEnd, string position, string phrase, string textColor)
        {
            TimeStart = timeStart;
            TimeEnd = timeEnd;
            Position = position;
            Phrase = phrase;
            TextColor = textColor;
        }
    }

    class SubCreator
    {
        public static SubtitlesLoader CreateSubtitle(string initialString)
        {
            int startTime = GetTimeStart(initialString);
            int endTime = GetTimeEnd(initialString);
            string position = GetPosition(initialString);
            string phrase = GetPhrase(initialString);
            string textColor = GetColor(initialString);

            return new SubtitlesLoader(startTime, endTime, position, phrase, textColor);
        }

        public static int GetTimeStart(string initialString)
        {
            int startTime = int.Parse(initialString.Split('-')[0].Split(' ')[0].Split(':')[1]);
            return startTime;
        }

        public static int GetTimeEnd(string initialString)
        {
            int endTime = int.Parse(initialString.Split('-')[1].Split(' ')[1].Split(':')[1]);
            return endTime;
        }

        public static string GetPosition(string initialString)
        {
            string position = "";
            if (initialString.Contains("["))
                position = initialString.Split('[')[1].Split(',')[0];
            else
            {
                position = "Bottom";
            }

            return position;
        }

        public static string GetColor(string initialString)
        {
            string colors = null;
            if (initialString.Contains("]"))
                colors = initialString.Split(']')[0].Split(',')[1];

            if (colors != null && (colors.Equals("") || colors.Length == 0) || colors == null) colors = "White";


            return colors;
        }

        public static string GetPhrase(string initialString)
        {
            string text = "";
            if (initialString.Contains("[") || initialString.Contains("]"))
                text = initialString.Split(']')[1];
            else
            {
                string[] phrases = initialString.Split(' ');

                List<string> words = phrases.ToList();
                words.RemoveAt(0);
                words.RemoveAt(0);
                words.RemoveAt(0);

                text = string.Join(" ", words);
            }

            return text;
        }
    }

    public class SubtitleOutputer
    {
        private static int runTime;
        private SubtitlesLoader[] subtitles;

        public SubtitleOutputer(SubtitlesLoader[] subtitles)
        {
            this.subtitles = subtitles;
        }

        public void BeignWork()
        {
            TimerCallback timerCallback = new TimerCallback(Test);
            Timer timer = new Timer(timerCallback, subtitles, 0, 1000);
        }

        private static void Test(object obj)
        {
            SubtitlesLoader[] input = (SubtitlesLoader[])obj;
            foreach (SubtitlesLoader subtit in input)
            {
                if (subtit.TimeStart == runTime)
                    SubtitleConsole(subtit);
                else if (subtit.TimeEnd == runTime)
                    RemoveSubtitle(subtit);
            }

            runTime++;
            Console.SetCursorPosition(0, TableDrawer.Height + 3);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(runTime);
        }
        

        private static void SubtitleConsole(SubtitlesLoader subtit)
        {
            SetPosition(subtit);
                
            if (subtit.TextColor.Trim().Equals("Red")) Console.ForegroundColor = ConsoleColor.Red;
            if (subtit.TextColor.Trim() == "Blue") Console.ForegroundColor = ConsoleColor.Blue;
            if (subtit.TextColor.Trim() == "Green") Console.ForegroundColor = ConsoleColor.Green;
            if (subtit.TextColor.Trim() == "White") Console.ForegroundColor = ConsoleColor.White;
                
            Console.Write(subtit.Phrase);
        }

        private static void RemoveSubtitle(SubtitlesLoader subtit)
        {
            SetPosition(subtit);
            for (int i = 0; i < subtit.Phrase.Length; i++)
                Console.Write(" ");
        }

        private static void SetPosition(SubtitlesLoader subtit)
        {
            switch (subtit.Position)
            {
                case "Top":
                    Console.SetCursorPosition((TableDrawer.Width - subtit.Phrase.Length) / 2, 1);
                    break;
                case "Right":
                    Console.SetCursorPosition(TableDrawer.Width - subtit.Phrase.Length,
                        (TableDrawer.Height - 1) / 2 + 1);
                    break;
                case "Bottom":
                    Console.SetCursorPosition((TableDrawer.Width - subtit.Phrase.Length) / 2, TableDrawer.Height);
                    break;
                case "Left":
                    Console.SetCursorPosition(1, (TableDrawer.Height - 1) / 2 + 1);
                    break;
                default:
                    break;
            }

        }
    }
}