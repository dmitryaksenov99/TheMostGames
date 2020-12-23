using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace TMG1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Execute();//execute main function
        }
        private void Execute()//main function
        {
            
            try
            {
                this.Cursor = Cursors.Wait;
                var range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);//create text range from richTextBox
                string[] raw = range.Text.Split(',').Where(x => !string.IsNullOrEmpty(x) && x != " ").ToArray();//get string from richTextBox and split it to array if element isn't equals null or "" or " "
                var set = new HashSet<int>();//create new hashset to avoid duplicate values

                richTextBox.Document.Blocks.Clear();//clear richbox for writing processed values
                dataGrid1.Items.Clear();//clear table

                int badIdsCount = 0;
                int dupCount = 0;

                foreach (string rawid in raw)//foreach in raw ids and get suitable ids
                {
                    try
                    {
                        if (int.TryParse(rawid.Replace(" ", ""), out int id))//try to parse id value from array and check if the element is in the array
                        {
                            if (set.Add(id))
                            {
                                AppendText(id.ToString() + ",", Brushes.Black, FontWeights.Normal);//append correct id to richTextBox
                            }
                            else
                            {
                                dupCount++;//if we have duplicates if hashset
                            }
                        }
                        else
                        {
                            badIdsCount++;//if we have incorrect id
                            AppendText(rawid.ToString() + ",", Brushes.Red, FontWeights.SemiBold);//add red bold text to richTextBox if id is broken
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        //skip writing id to hashset if something went wrong
                    }
                }

                if (badIdsCount > 0 || dupCount > 0)//if we have bad ids and duplicates
                {
                    if (MessageBox.Show(string.Format("Некорректных идентификаторов - {0}\nКоличество дубликатов - {1}\nИгнорировать?", badIdsCount, dupCount), "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ForeachInHashSet(set);//enumeration of ids
                    }
                    else
                    {
                        //if MessageBox result is "no" - do nothing
                    }
                }
                else
                {
                    ForeachInHashSet(set);//enumeration of ids
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Arrow;
                MessageBox.Show(ex.ToString());//display an informative message of error
            }
        }
        private void ForeachInHashSet(HashSet<int> set)//method for enumeration of true-ids
        {
            foreach (var id in set)
            {
                try
                {
                    IfIdIsCorrect(id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //skip getting data for rawid if something went wrong
                }
            }
            this.Cursor = Cursors.Arrow;
        }
        private void IfIdIsCorrect(int id)//if received id is correct
        {
            try
            {
                string json = GetAnswer(id);//get answer from server
                string text = GetTextValueFromJson(json);//get text string from json
                var kvp = GetWorldsCountAndTextVowels(text);//processing worlds count and text vowels
                AddToTable(text, kvp.Key, kvp.Value);//try to add received data to table
            }
            catch
            {
                throw;
            }
        }
        private string GetAnswer(int id)//method for getting answer from server
        {
            try
            {
                string url = string.Format("https://tmgwebtest.azurewebsites.net/api/textstrings/{0}", id);//make link from main site url, api args and string id
                var webreq = (HttpWebRequest)WebRequest.Create(url);//create new WebRequest
                if (webreq != null)//if it isn't null
                {
                    webreq.Method = "GET";//set request method type
                    webreq.Timeout = 8196;//set request timeout
                    webreq.ContentType = "application/json";//set request type of content
                    webreq.Headers.Add("TMG-Api-Key", "0J/RgNC40LLQtdGC0LjQutC4IQ==");//add api auth headers

                    var s = webreq.GetResponse().GetResponseStream();//get answer from site
                    var sr = new StreamReader(s, encoding: Encoding.UTF8);//create stream from answer
                    var answer = sr.ReadToEnd();//read data from stream
                    return answer;//return stream data
                }
                else
                {
                    throw new Exception("HttpWebRequest is null!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());//display an informative message of error
                throw;
            }
        }
        private string GetTextValueFromJson(string json)
        {
            try
            {
                var answer = new JavaScriptSerializer().Deserialize<Answer>(json);//deserialize received json
                return answer.text;//get from json text value and return it
            }
            catch
            {
                throw;
            }
        }
        private KeyValuePair<int, int> GetWorldsCountAndTextVowels(string text)//processing and getting needed data from text
        {
            try
            {
                string cleaned = RemoveSpecialCharsByRegex("[^À-žА-яA-z0-9 ]+", text);//get text withoud invalid chars (only alphabets letters and numbers)
                int wordsCount = cleaned.Split(' ').Length;//split cleaned text to string array and get count of elements
                //find count of these letters (are indicated only vowel letters)
                int vowelCount = cleaned.Count(@"aeiouyауоыиэяюёеàáâãäåæāăąèéêëēĕėęěìíîïĨĩĪīĬĭĮįİıòóôõöøōŏőœùúûüũūŭůűųýÿŷAEIOUYАУОЫИЭЯЮЁЕÀÁÂÃÄÅÆĀĂĄÈÉÊËĒĔĖĘĚÌÍÎÏĨĨĪĪĬĬĮĮİIÒÓÔÕÖØŌŎŐŒÙÚÛÜŨŪŬŮŰŲÝŸŶ".Contains);
                return new KeyValuePair<int, int>(wordsCount, vowelCount);//return two processed values in keypair
            }
            catch
            {
                throw;
            }
        }
        private void AddToTable(string txt, int wc, int vow)
        {
            try
            {
                dataGrid1.Items.Add(new DataItem//create and add DataItem to table if everything else is ok
                {
                    Text = txt,//add text item
                    WordsCount = wc.ToString(),//add wc item
                    Vowels = vow.ToString()//add vowels item
                });
            }
            catch
            {
                throw;
            }
        }
        private static string RemoveSpecialCharsByRegex(string exp, string text)
        {
            try
            {
                return Regex.Replace(text, exp, "");//setting pattern for regex and get cleaned text
            }
            catch
            {
                throw;
            }
        }
        public void AppendText(string text, Brush brush, FontWeight weights)//append text to richbox with text format select
        {
            try
            {
                var range = new TextRange(richTextBox.Document.ContentEnd, richTextBox.Document.ContentEnd);//create text range from richTextBox
                range.Text = text;//set our text to range
                range.ApplyPropertyValue(TextElement.ForegroundProperty, brush);//apply brush type to text
                range.ApplyPropertyValue(TextElement.FontWeightProperty, weights);//apply thickness type to text
            }
            catch
            {
                throw;
            }
        }
        private class DataItem//datagrid (table) row class
        {
            public string Text { get; set; }//text field
            public string WordsCount { get; set; }//words count field
            public string Vowels { get; set; }//vowels count field
        }
        private class Answer//class for json deserialization 
        {
            public string text { get; set; }//text answer from json
        }
    }
}
