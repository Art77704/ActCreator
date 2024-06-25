using ActCreator.DataBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ActCreator
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        TempColumnWorks _tempColumnWorks =new TempColumnWorks();
        bool blockAdd;
        public MainPage(TempColumnWorks tmw=null)
        {
            InitializeComponent();
            WorkDate_DTP.SelectedDate=DateTime.Now;
            CarName_CMB.ItemsSource=AppConnect.modelodb.Cars.ToList();
            Works_DT.ItemsSource = AppConnect.modelodb.TempColumnWorks.ToList();
        }




        private void Finish_BTN_Click(object sender, RoutedEventArgs e)
        {
            if (DelAfterClose_CB.IsChecked == false)
            {
                DelTempWorks();
                Works_DT.ItemsSource = AppConnect.modelodb.TempColumnWorks.ToList();
                
            }
            if (OpenCopyText_CB.IsChecked == true)
            {
                CopyTextWindow w = new CopyTextWindow();
                w.ShowDialog();
            }
            UpdateInfo();


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

        
       
    }
}
