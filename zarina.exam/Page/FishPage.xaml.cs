using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;


namespace zarina.exam.Page
{
    /// <summary>
    /// Логика взаимодействия для FishPage.xaml
    /// </summary>
    public partial class FishPage : System.Windows.Controls.Page
    {
        public FishPage()
        {
            InitializeComponent();
        }
        private void Otchet_Click(object sender, RoutedEventArgs e)
        {

            string[] TempList = Temp.Text.Split(' ');
            List<int> temperatures = TempList.Select(t => int.TryParse(t.Trim(), out int temp) ? temp : 0).ToList();
            List<string> list = new List<string>();
            TimeMaxTemp.Text = "0";
            TimeMinTemp.Text = "0";
            var violation = list;

            if (MinTemp.Text == "")
            {
                var violations = CheckTemp(temperatures, 0, Convert.ToInt32(MaxTemp.Text));
                violation = violations;
            }
            else if (MaxTemp.Text == "")
            {
                var violations = CheckTemp(temperatures, Convert.ToInt32(MinTemp.Text), 0);
                violation = violations;
            }
            else
            {
                var violations = CheckTemp(temperatures, Convert.ToInt32(MinTemp.Text), Convert.ToInt32(MaxTemp.Text));
                violation = violations;
            }


            string filePath = "C:/Users/zarina/OneDrive/Рабочий стол/отчет.txt";
        
        MessageBoxResult result = MessageBox.Show(
                    $"Отчет сформирован. Путь: {filePath}",
                    "Отчет",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

            File.AppendAllText("" + filePath, $"Вид рыбы: {NameFish.Text}" + $"\nМаксимальная температура: {MaxTemp.Text}" + $"\nМинимальная температура: {MinTemp.Text}" + $"\nДата и время старта доставки: {DateTimeStart.Text}" + $"\nТемпература: {Temp.Text}\n" + string.Join(Environment.NewLine, violation));
        }

        private List<string> CheckTemp(List<int> temperatures, int minTemp, int maxTemp)
        {
            List<string> checkMin = new List<string>();
            checkMin.Add(" ");
            List<string> checkMax = new List<string>();
            checkMax.Add(" ");
            List<string> checkGood = new List<string>
            {
                "Доставка произошла успешно"
            };



            int violationMin = 0;
            int violationMax = 0;
            DateTime startTime = Convert.ToDateTime(DateTimeStart.Text);

            for (int i = 0; i < temperatures.Count; i++)
            {
                if (temperatures[i] < minTemp)
                {
                    violationMin += 10;
                    checkMin.Add($"Время: {startTime.AddMinutes(i * 10):HH:mm} Факт: {temperatures[i]} Норма: {minTemp} Отклонение: {minTemp - temperatures[i]}");
                }
                else if (temperatures[i] > maxTemp)
                {
                    violationMax += 10;
                    checkMax.Add($"Время: {startTime.AddMinutes(i * 10):HH:mm} Факт: {temperatures[i]} Норма: {maxTemp} Отклонение: {temperatures[i] - maxTemp}");
                }
            }

            if (violationMin > Convert.ToInt32(TimeMinTemp.Text))
            {
                checkMin[0] = $"Порог минимально допустимой температуры превышен на {violationMin - Convert.ToInt32(TimeMinTemp.Text)} минут.";
                MessageBoxResult result = MessageBox.Show(
                        $"Температурный режим нарушен!",
                        "Нарушение",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                return checkMin;
            }

            else if (violationMax > Convert.ToInt32(TimeMaxTemp.Text))
            {
                checkMax[0] = $"Порог максимально допустимой температуры превышен на {violationMax - Convert.ToInt32(TimeMaxTemp.Text)} минут.";
                return checkMax;
            }
            else
            {
                return checkGood;
            }
        }

        private void Docs_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Выберите текстовый файл"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                    {
                        string fileContent = reader.ReadToEnd();
                        string[] InfoInDocs = fileContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                        DateTimeStart.Text = InfoInDocs[0];
                        Temp.Text = InfoInDocs[1];
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Text_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[a-zA-Zа-яА-ЯёЁ]+$");
        }


        private void Number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^[-0-9]+$") && !(e.Text == "-");
        }

        private void DateTime_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^[0-9 :/-]+$");
        }


    }
}

