using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace accountStatement
{
    internal static class Program
    {
        /*
         * confiigg
         */
        private static readonly bool IsNeedToSort = false;
        private static readonly bool DisplayDebugInfo = false;
        private static readonly string Filename = "success.txt";

        private static Dictionary<string, int> _balanceHistory;
        private static int _totalBalance;

        public static void Main()
        {
            while (true)
            {
                Console.Clear();

                string[] result = GetFileContent(Filename);
                LogFile(result);
                HandleHistory(result);
                Thread.Sleep(3000);
            }
        }

        private static void LogFile(string[] file)
        {
            if (!DisplayDebugInfo) return;
            foreach (string s in file)
            {
                Console.WriteLine(s);
            }
        }

        private static void HandleHistory(string[] history)
        {
            _balanceHistory = new Dictionary<string, int>();
            int balance = int.Parse(history[0]);
            for (int i = 1; i < history.Length; i++)
            {
                string[] rowInfo = GetRowInfo(history[i]);

                if (rowInfo.Length == 3)
                {
                    switch (rowInfo[2])
                    {
                        case "in":
                            balance += int.Parse(rowInfo[1]);
                            break;

                        case "out":
                            balance -= int.Parse(rowInfo[1]);
                            break;
                    }
                }
                else
                {
                    string[] previousRowInfo = GetRowInfo(history[i - 1]);
                    if (previousRowInfo[2] == "in")
                    {
                        balance -= int.Parse(previousRowInfo[1]);
                    }
                    else
                    {
                        balance += int.Parse(previousRowInfo[1]);
                    }
                }

                _balanceHistory[rowInfo[0].Trim()] = balance;
            }

            if (balance < 0)
            {
                Console.WriteLine("Расход превысил остаток по карте");
                return;
            }

            _totalBalance = balance;
            HandleUserInput();
        }

        private static void HandleUserInput()
        {
            while (true)
            {
                Console.Write("Введите дату и время в формате гггг-мм-дд чч:мм или нажмите [Enter], чтобы отобразить итоговый баланс: ");
                string userInput = Console.ReadLine();

                if (userInput is null or "")
                {
                    Console.WriteLine($"Остаток в конце: {_totalBalance}");
                    return;
                }

                userInput = userInput.Trim();
                bool isDateValid = Regex.IsMatch(userInput, @"^\d{4}-\d{2}-\d{2} \d{2}:\d{2}$");
                if (!isDateValid)
                {
                    Console.WriteLine("Неверный формат даты!");
                    continue;
                }

                int balance = _balanceHistory[userInput.Replace(" ", "")];
                Console.WriteLine($"Баланс на момент {userInput} равен {balance}");
                break;
            }
        }

        private static string[] GetRowInfo(string row)
        {
            return row.Replace(" ", "").Split('|');
        }

        private static string[] GetFileContent(string filename)
        {
            return IsNeedToSort ? File
                .ReadAllText(filename)
                .Split('\n')
                .Select(x => x.Split('|'))
                .OrderBy(x => x[0])
                .Select(x => string.Join("|", x)).ToArray() : File.ReadAllText(filename).Split('\n');
        }
    }
}