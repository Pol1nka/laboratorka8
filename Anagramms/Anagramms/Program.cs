using System;
using System.Collections.Generic;
using System.Linq;

namespace Anagramms
{
    internal static class Program
    {
        public static void Main()
        {
            string[] array = { "code" , "doce", "ecod", "framer", "frame" };
            Console.WriteLine($"Массив до: {string.Join(", ", array)}");
            Console.WriteLine($"Массив после: {string.Join(", ", RemoveAnagrams(array.ToList()))}");
        }

        private static string[] RemoveAnagrams(List<string> list)
        {
            while (true)
            {
                int indexToRemove = -1;
                for (int i = 0; i < list.Count; i++)
                {
                    if (indexToRemove >= 0) break;
                    
                    for (int j = i; j > 0; j--)
                        if (IsAnagram(list[j-1], list[i])) indexToRemove = i;
                }

                if (indexToRemove == -1) return list.ToArray();

                list.RemoveAt(indexToRemove);
            }
        }
        private static bool IsAnagram(string item1, string item2)
        {
            char[] splitItem1 = item1.ToCharArray();
            Array.Sort(splitItem1);
            char[] splitItem2 = item2.ToCharArray();
            Array.Sort(splitItem2);
            
            return splitItem1.SequenceEqual(splitItem2);
        }
    }
}