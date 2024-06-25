using ActCreator.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;
using Telegram.Bot;
using ActCreator.Properties;
using System.Security.Cryptography.X509Certificates;


namespace ActCreator
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        bool blockAdd;
        public static string CarModel;
        public static string StateNumber;
        public static string VIN;
        public static string Year;
        private readonly string botToken = "6361510741:AAFen61LwkEVxat-UIxrK7lG0qjsJe4bIEo";
        private readonly long chatId = 667637277;
        public static int ActNumber;
        public static string CountWork;

        public MainPage()
        {
            InitializeComponent();
            WorkDate_DTP.SelectedDate=DateTime.Now;
            CarName_CMB.ItemsSource=AppConnect.modelodb.Cars.ToList();
            Works_DT.ItemsSource = AppConnect.modelodb.TempColumnWorks.ToList();
            int t = int.Parse(Settings.Default["ActNumberP"].ToString());
            ActNumber_TXB.Text = (t + 1).ToString();
            ActNumber=int.Parse(ActNumber_TXB.Text);
        }

        private void Finish_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (DelAfterClose_CB.IsChecked == false)
            {
                DelTempWorks();
                Works_DT.ItemsSource = AppConnect.modelodb.TempColumnWorks.ToList();
            }
            CountWork = CountWork_LB.Content.ToString();

            CarModel = CarName_CMB.Text.ToUpper();
            StateNumber = StateNumber_TXB.Text.ToUpper();
            VIN = VIN_TXB.Text.ToUpper();
            Year = Year_TXB.Text;

            ExcelCode();
            TelegramBotClient botClient = new TelegramBotClient(botToken);
            botClient.SendTextMessageAsync(chatId, $"Номер акта: {ActNumber_TXB.Text}, задолженность: {AllPrice_LB.Content.ToString()}₽");
            if (OpenCopyText_CB.IsChecked == true)
            {
                CopyTextWindow w = new CopyTextWindow();
                w.Show();
            }
            UpdateInfo();
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        public static void DelTempWorks()
        {
            try
            {
                var works = AppConnect.modelodb.TempColumnWorks;
                AppConnect.modelodb.TempColumnWorks.RemoveRange(works);
                AppConnect.modelodb.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка " + ex.Message);
            }
        }

        private void AddCarName_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (CarName_CMB.Visibility== Visibility.Visible)
            {
                CarName_CMB.Visibility = Visibility.Collapsed;
                CarName_TXB.Visibility = Visibility.Visible;
            }
            else
            {
                try
                {
                    if (CarName_TXB.Text == "")
                    {
                        CarName_CMB.Visibility = Visibility.Visible;
                        CarName_TXB.Visibility = Visibility.Collapsed;
                        return;
                    }

                    Cars car = new Cars
                    {
                        CarName = CarName_TXB.Text
                    };
                    AppConnect.modelodb.Cars.Add(car);
                    AppConnect.modelodb.SaveChanges();

                    CarName_CMB.ItemsSource = AppConnect.modelodb.Cars.ToList();

                    CarName_CMB.Visibility = Visibility.Visible;
                    CarName_TXB.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        private void WorkPrice_TXB_LostFocus(object sender, RoutedEventArgs e)
        {
            if (AppConnect.modelodb.TempColumnWorks.Count() == 12)
            {
                MessageBox.Show("Данная работа не добавлена. Больше добавлять нельзя.");
                blockAdd=true;
            }
            else if (AppConnect.modelodb.TempColumnWorks.Count() < 12)
                blockAdd = false;

            if (blockAdd)
                return;
            else
            {
                if (AddWork_CB.IsChecked == true)
                    AddWork();
            }
        }

        void AddWork()
        {
            try
            {
                if (blockAdd)
                    return;

                TempColumnWorks tmw = new TempColumnWorks
                {
                    WorkName = WorkName_TXB.Text,
                    WorkPrice = Convert.ToDouble(WorkPrice_TXB.Text)
                };

                AppConnect.modelodb.TempColumnWorks.Add(tmw);
                AppConnect.modelodb.SaveChanges();
                Works_DT.ItemsSource = AppConnect.modelodb.TempColumnWorks.ToList();
                WorkName_TXB.Clear();
                WorkPrice_TXB.Clear();
                UpdateInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
        void UpdateInfo()
        {
            try
            {
                var sum = AppConnect.modelodb.TempColumnWorks.Sum(x => x.WorkPrice);
                CountWork_LB.Content = AppConnect.modelodb.TempColumnWorks.Count().ToString();
                AllPrice_LB.Content = sum.ToString();
            }
            catch
            {
                CountWork_LB.Content = "";
                AllPrice_LB.Content = "";
            }
           
        }

        private void DeleteWork_BTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var res = MessageBox.Show("Вы действительно хотите удалить данные?", "Удаление работ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.Yes)
                {
                    var selectedRow = Works_DT.SelectedItems.Cast<TempColumnWorks>().ToList();
                    AppConnect.modelodb.TempColumnWorks.RemoveRange(selectedRow);
                    AppConnect.modelodb.SaveChanges();
                Works_DT.ItemsSource = AppConnect.modelodb.TempColumnWorks.ToList();
                    UpdateInfo();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка " + ex.Message);
            }
        }

        void ExcelCode()
        {
            Excel.Application app = new Excel.Application
            {
                Visible = true,
                SheetsInNewWorkbook = 2
            };
     /*       app.Workbooks.Open(@"C:\Users\Артем\Desktop\ForWork\Artem\NUM2TEXT.xla",
                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing);*/
            app.Workbooks.Open(PathToFilesClass.PathToNum2Text,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing);
            

           /* app.Workbooks.Open($@"C:\Users\Артем\Desktop\ForWork\Artem\ИП ЭЙНАТЯН {CountWork_LB.Content.ToString()} шт (Тинькофф).xlsx",
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing);*/
            app.Workbooks.Open(PathToFilesClass.PathToExcel,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing);

            app.DisplayAlerts = true;

            Excel.Worksheet sheet = (Excel.Worksheet)app.Worksheets.get_Item(1);
            var _date = WorkDate_DTP.Text;
            var _dateLong = WorkDate_DTP;
            
            _dateLong.SelectedDateFormat = DatePickerFormat.Long;
            
            sheet.Range["A1"].Value = $"Акт №{ActNumber_TXB.Text} от {_dateLong.Text}";

            //Включить отображение окон с сообщениями
            app.DisplayAlerts = true;
            //Получаем первый лист документа (счет начинается с 1)

            //Пример заполнения ячеек
            sheet.Range["B5"].Value = $"ДОГОВОР НА РЕМОНТ А/М {CarModel} ГОС. НОМЕР {StateNumber} VIN {VIN} ГОД ВЫПУСКА {Year}";
            int temp = 7;
            var WorkN = AppConnect.modelodb.TempColumnWorks.Select(a => a.WorkName).ToList();
            var AllPrice = AppConnect.modelodb.TempColumnWorks.Select(a => a.WorkPrice).ToList();
            int i = 0;
            foreach (var item in WorkN)
            {
                sheet.Range[$"B{temp}"].Value = $"{item.ToString().ToUpper()}";
                sheet.Range[$"G{temp}"].Value = $"{AllPrice[i].ToString()}";

                i++;
                temp++;
            }
           
            Excel.Worksheet sheet2 = (Excel.Worksheet)app.Worksheets.get_Item(2);

            sheet2.Range["A16"].Value = $"Счет №{ActNumber_TXB.Text} от {_date}";
        }

        private void ActNumber_TXB_TextChanged(object sender, TextChangedEventArgs e)
        {
            ActNumber = int.Parse(ActNumber_TXB.Text);
        }
    }
}
