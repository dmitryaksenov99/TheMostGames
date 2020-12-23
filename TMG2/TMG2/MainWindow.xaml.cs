using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Windows;

namespace TMG2
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
        /// <summary>
        /// Класс объектов / Objects class
        /// </summary>
        public class Entry
        {
            public string Name { get; set; }//Значение имени (строка) / Name value (string)
            public List<string> Values { get; set; }//List с текстовыми значениями Values / List of values-strings
        }
        /// <summary>
        /// Метод сериализации списка из классов Entry / Serialization method of list of Entry-classes
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        private static string EntrySerializer(List<Entry> len)
        {
            var dict = new Dictionary<string, dynamic> { };//Создаём новый Dictionary для сериализации / Create new dictionary for serialization
            foreach (var x in len)//Перебор значений списка из классов Entry / Enumeration of list of Entry-classes
            {
                try
                {
                    //Используем метод ContainsKey() для проверки наличия значения имени в Dictionary
                    //Use ContainsKey() for check does the Name exists
                    if (dict.ContainsKey(x.Name))
                    {
                        throw new Exception("Значение имени уже присутствует в Dictionary / Name-value is already exists in Dictionary");
                    }
                    //Если Values является пустым
                    //If Values is null
                    if (x.Values == null)
                    {
                        dict.Add(x.Name, null);//Добавляем пару ключ-значение в Dictionary, а значение списка Values записывается как пустое / Add key-value pair to dictionary, and value of Values-list write as null
                    }
                    else
                    {
                        //Если количество элементов списка Values равно нулю
                        //If count of Values-list equals zero
                        if (x.Values.Count == 0)
                        {
                            dict.Add(x.Name, null);//Добавляем пару ключ-значение в Dictionary, а значение списка Values записывается как пустое / Add key-value pair to dictionary, and value of Values-list write as null
                        }
                        //Если количество элементов в списке Values равно одному
                        //If count of elements in Values-list equals one
                        else if (x.Values.Count == 1)
                        {
                            //Добавляем пару ключ-значение в Dictionary, а единственное значение Values записываем как строку
                            //Add key-value pair to dictionary, and the only first value write as string
                            dict.Add(x.Name, x.Values[0]);
                        }
                        else//Если Values это массив / If Values-list is a array
                        {
                            dict.Add(x.Name, x.Values);//Добавляем пару ключ-значение в Dictionary / Add key-value pair to dictionary
                        }
                    }
                }
                //Если при добавлении значений в Dictionary произошла ошибка
                //If while adding values to Dictionary an error has been occured
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);//Выводим сообщение об ошибке в консоль / Write error message to console
                }
            }
            //После цикла foreach сериализируем Dictionary / Serialize Dictionary after foreach cycle
            string json = new JavaScriptSerializer().Serialize(dict);
            return json;
        }
        /// <summary>
        /// Метод для симуляции входных данных / Method for input-values simulation
        /// </summary>
        /// <returns></returns>
        private List<Entry> SimulateInputValues()
        {
            var len = new List<Entry> { };//Создаём новый List, состоящий из экземпляров класса Entry / Create List of Entry-classes
            var rnd = new Random();//Объявлем экземпляр класса Random / Create Random class instance
            for (int i = 0; i < 10; i++)//Цикл for с произвольным количеством итераций / "for" cycle with arbitrary number of iterations
            {
                var values = new List<string> { };//Создаём новый List, состоящий из строк / Create List of strings
                for (int j = 0; j < 10; j++)//Цикл for с произвольным количеством итераций / "for" cycle with arbitrary number of iterations
                {
                    values.Add("Value" + rnd.Next(0, Int32.MaxValue));//Заполняем Values рандомными значениями / Fill list by random values
                }
                var en = new Entry//Создаём экземпляр класса Entry / Create Entry-class instance
                {
                    Name = "Name" + rnd.Next(0, Int32.MaxValue),//Присваиваем полю Name рандомное значение / Assign random value to Name
                    Values = values//Присваиваем Values массив values / Assign values to Values
                };

                //Добавляем экземпляр класса Entry в List, состоящий из экземпляров класса Entry
                //Add Entry instance to List of Entry-classes
                len.Add(en);
            }

            return len;//Возвращаем List<Entry> как результат выполнения функции / Return List of Entry-classes as function result
        }
        /// <summary>
        /// Метод для поверки тестовых значений / Method for check test-values
        /// </summary>
        /// <returns></returns>
        private List<Entry> ListForCheck()
        {
            var len = new List<Entry> { };//Создаём новый List, состоящий из экземпляров класса Entry / Create List of Entry-classes

            var en1 = new Entry//Создаём экземпляр класса Entry / Create Entry-class instance
            {
                Name = "Name1",//Присваиваем тестовое значение имени / Assign test-value for Name
                Values = new List<string> { "Value1", "Value2", "Value3" }//Массив строк в Values / Array of strings in Values
            };
            var en2 = new Entry//Создаём экземпляр класса Entry / Create Entry-class instance
            {
                Name = "Name2",//Присваиваем тестовое значение имени / Assign test-value for Name
                Values = new List<string> { "Value" }//Одна строка в Values / One string in Values
            };
            var en3 = new Entry//Создаём экземпляр класса Entry / Create Entry-class instance
            {
                Name = "Name3",//Присваиваем тестовое значение имени / Assign test-value for Name
                Values = new List<string> { }//Нет элементов в List<string> / Nothing in List<string>
            };
            var en4 = new Entry//Создаём экземпляр класса Entry / Create Entry-class instance
            {
                Name = "Name4",//Присваиваем тестовое значение имени / Assign test-value for Name
                Values = null//Значение Values равняется null / Values equals null
            };
            //Добавляем экземпляры класса Entry в List, состоящий из экземпляров класса Entry
            //Add Entry instances to List of Entry-classes
            len.Add(en1);
            len.Add(en2);
            len.Add(en3);
            len.Add(en4);
            return len;
        }
        private void jsonOutput_Click(object sender, RoutedEventArgs e)
        {
            //var len = SimulateInputValues();//Вызов функции симулирования входных данных / Execute simulation of input-values
            var len = ListForCheck();//Вызов функции создания входных данных с тестовыми значениями / Execute function of create test-values for checking
            string result = EntrySerializer(len);//Производим сериализацию входных данных / Execute input-values serialization
            jsonOutput.Text = result;//Вывод json в TextBox / Write json-answer to TextBox
        }
    }
}
