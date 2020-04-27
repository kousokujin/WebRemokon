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
        public MainWindow()
        {
            MainCore = new MainCoreClass();
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
                this.Dispatcher.Invoke((Action)(() =>
                {
                    ActiveWindowLabel.Content = windowname;
                }));
            }
        }

        private void AddAppButton_Click(object sender, RoutedEventArgs e)
        {
            AppData d = new AppData
            {
                WindowName = NewWindowTextBox.Text,
                url = JumpUrlTexBox.Text
            };

            MainCore.AppList.Add(d);
            NewWindowTextBox.Text = "";
            JumpUrlTexBox.Text = "";
            WindowList.Items.Refresh();
            MainCore.SaveConfig();
        }
    }
}
