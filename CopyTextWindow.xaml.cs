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

namespace ActCreator
{
    /// <summary>
    /// Логика взаимодействия для CopyTextWindow.xaml
    /// </summary>
    public partial class CopyTextWindow : Window
    {
        public CopyTextWindow()
        {
            InitializeComponent();
            var WorkN = AppConnect.modelodb.TempColumnWorks.Select(a => a.WorkName).ToList();
            var AllPrice = AppConnect.modelodb.TempColumnWorks.Select(a => a.WorkPrice).ToList();
            List<string> Works = new List<string>();
            int i = 0;
            foreach (var item in WorkN)
            {
                Works_TXB.Text += $"{i + 1}) Наименование услуги и цена:\n" + item.ToString() + "\t" + AllPrice[i].ToString() + "\n";
                i++;
            }
        }
    }
}
