using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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

namespace WebRemokon
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        MainCoreClass MainCore;
        public MainWindow(MainCoreClass MainCore)
        {
            //MainCore = new MainCoreClass();
            this.MainCore = MainCore;
            MainCore.window.ChangeActiveWindow += ChgWindows;
            InitializeComponent();

            //DataContext = MainCore;
            WindowList.ItemsSource = MainCore.AppList;
            //ActiveWindowLabel.Content = MainCore.window.ActiveWindowName;
        }

        private void ChgWindows(object sender,EventArgs e)
        {
            if (e is ChangeActiveWindowsEvent)
            {
                string windowname = (e as ChangeActiveWindowsEvent).windowsname;

                if (windowname != "WebRemokon")
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        NewWindowTextBox.Text = windowname;
                    }));
                }
            }
        }

        private void AddAppButton_Click(object sender, RoutedEventArgs e)
        {
            bool isExist = false;
            MainCore.AppList.ForEach(x =>
            {
                if (x.WindowName == NewWindowTextBox.Text)
                {
                    isExist = true;
                }
            });

            if (isExist == false)
            {
                AppData d = new AppData
                {
                    WindowName = NewWindowTextBox.Text,
                    url = JumpUrlTexBox.Text
                };

                MainCore.AppList.Add(d);
            }
            else
            {
                foreach(AppData x in MainCore.AppList)
                {
                    if(x.WindowName == NewWindowTextBox.Text)
                    {
                        x.url = JumpUrlTexBox.Text;
                    }
                }
            }
            NewWindowTextBox.Text = "";
            JumpUrlTexBox.Text = "";
            WindowList.Items.Refresh();
            MainCore.SaveConfig();
        }

        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            AppData name = ((sender as Button).Tag) as AppData;

            /*
            int i = 0;
            int del_i = 0;
            foreach(AppData d in MainCore.AppList)
            {
                if(d.WindowName == name.WindowName && d.url == name.url)
                {
                    del_i = i;
                }
                i++;
            }
            */
            if (name.WindowName != "$default$")
            {
                MainCore.AppList.Remove(name);
                WindowList.Items.Refresh();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            WindowsBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 122, 204));
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            WindowsBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 161,161,161));
        }

        /*
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
        */
    }
}
