﻿using ActCreator.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ActCreator
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            MainPage.DelTempWorks();
            if (MainPage.ChangeAct == false)
            {
                Settings.Default["ActNumberP"] = MainPage.ActNumber;
                Settings.Default.Save();
            }
        }
    }
}
