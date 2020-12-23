using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TMG3
{
    class Program
    {
        public static List<string> ruStrings = new List<string>();//Create new instance of List<string>, containing russian strings
        public static List<string> enStrings = new List<string>();//Create new instance of List<string>, containing english strings
        static void Main(string[] args)//Get arguments
        {
            if (args.Length > 0)//If agruments exists
            {
                try//Need for catching wrong args
                {
                    LoadTo(args[0], args[1]);//Pass args to LoadTo function
                }
                catch
                {
                    Console.WriteLine("Invalid args.");
                }
            }
            else
            {
                try
                {
                    Console.WriteLine("Load default text files from debug folder? y/n");
                    string yn = Console.ReadLine().ToLower();//Get y/n answer from console
                    if (yn == "y")//if yes
                    {
                        LoadTo("ru.txt", "en.txt");//load default files from debug folder
                    }
                    else if (yn == "n")//if no
                    {
                        Console.WriteLine("Set location (or filename if it's in this folder) for file with russian strings and for file with english strings:");
                        LoadTo(Console.ReadLine(), Console.ReadLine());//load files from the specified location
                    }
                    else
                    {
                        Console.WriteLine("File load error");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.ReadKey();
        }
        private static void LoadTo(string ruFile, string enFile)//input data is text files
        {
            try
            {
                string line = "";//temp string for writing line from txt
                var rsr = new StreamReader(ruFile, encoding: Encoding.GetEncoding(1251));//create stream with file and needed encoding
                while ((line = rsr.ReadLine()) != null)//if line in stream exists
                {
                    ruStrings.Add(line);//add line to list with russian strings
                }
                var esr = new StreamReader(enFile, encoding: Encoding.GetEncoding(1251));
                while ((line = esr.ReadLine()) != null)
                {
                    enStrings.Add(line);//add line to list with english strings
                }

                OutputValues(Find());//executing matched strings search and output it
            }
            catch
            {
                throw;
            }
        }
        private static double GetGolzmanValueWithCycle(string text)//alternative variation of getting golzman value (slower)
        {
            try
            {
                double i, g;
                for (i = 0.5, g = 0.5; i < text.Length - 1; i++, g = g + i)
                {

                }
                return g * text.Length;
            }
            catch
            {
                return 0;
            }
        }
        private static double GetGolzmanValue(string text)//getting golzman value by formula
        {
            try
            {
                return (double)text.Length * text.Length / 2 * text.Length;
            }
            catch
            {
                throw;
            }
        }
        private static string RemoveSpecialCharsByRegex(string text)
        {
            try
            {
                return Regex.Replace(text, "[^А-яA-z0-9]+", "");//setting pattern for regex and get cleaned text
            }
            catch
            {
                throw;
            }
        }

        private static Dictionary<KeyValuePair<string, double>, List<string>> Find()//create method, which returns dictionary with sorted data
        {
            var rudict = new Dictionary<KeyValuePair<string, double>, List<string>>();//create dictionary

            foreach (var x in ruStrings)//foreach russian strings
            {
                try
                {
                    string cleanedText = RemoveSpecialCharsByRegex(x);//get cleaned text
                    double golzmanOfText = GetGolzmanValue(cleanedText);//get golzman value of cleaned text
                    var kvp = new KeyValuePair<string, double>(x, golzmanOfText);//create KeyValuePair: source text and his golzman value
                    rudict.Add(kvp, null);//add KeyValuePair to main dictionary, second value is null for code speed (no time wasted for creating and filling the dictionary with lists)
                }
                catch
                {

                }
            }

            foreach (var y in enStrings)//foreach english strings
            {
                try
                {
                    var array = y.Split('|');//split main string by text and comment
                    string cleanedText = RemoveSpecialCharsByRegex(array[0]);//get cleaned text
                    double golzmanOfText = GetGolzmanValue(cleanedText);//get golzman value of cleaned text
                    string cleanedComment = RemoveSpecialCharsByRegex(array[1]);//get cleaned comment
                    double golzmanOfComment = GetGolzmanValue(cleanedComment);//get golzman value of cleaned comment
                    double golzmanSumm = golzmanOfText + golzmanOfComment;//the sum of golzman values text and comment

                    foreach (var x in rudict.ToArray())//foreach in main dictionary, execute ToArray() for avoid error
                    {
                        if (x.Key.Value == golzmanSumm)//if golzman of russian string equals the sum glozman of english string and comment
                        {
                            if (rudict[x.Key] == null)//if list not created
                            {
                                rudict[x.Key] = new List<string> { y };//create list with one matched english string
                            }
                            else//if list already created
                            {
                                rudict[x.Key].Add(y);//add to list matched english string
                            }
                        }
                    }
                }
                catch
                {

                }
            }

            return rudict;
        }
        private static void OutputValues(Dictionary<KeyValuePair<string, double>, List<string>> rudict)//output matched strings
        {
            //also the result can be serialized into json and saved to a file
            foreach (var result in rudict)
            {
                try
                {
                    if (result.Value != null && result.Value.Count > 0)//if russian string has more than zero english strings
                    {
                        Console.WriteLine("Matching russian string: " + result.Key.Key);
                        Console.WriteLine("\tEnglish strings for this russian string: ");
                        foreach (var value in result.Value)//foreach matched english strings
                        {
                            Console.WriteLine("\t\t" + value);//output english string
                        }
                    }
                }
                catch
                {

                }
            }

            ruStrings = new List<string>();//recreate list of russian strings
            enStrings = new List<string>();//recreate list of english strings
        }
    }
}