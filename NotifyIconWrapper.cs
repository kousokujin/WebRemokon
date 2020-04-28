using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebRemokon
{
    public partial class NotifyIconWrapper : Component
    {
        MainCoreClass MainCore;
        MainWindow wnd;
        public NotifyIconWrapper()
        {
            InitializeComponent();
            MainCore = new MainCoreClass();
            this.DisplayShow.Click += this.DisPlayButton_Click;
            this.ExitButton.Click += this.ExitButton_Click;
        }

        public NotifyIconWrapper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private void DisPlayButton_Click(object sender, EventArgs e)
        {
            // MainWindow を生成、表示
            WindowShow();
        }

        private void WindowShow()
        {
            if (wnd == null)
            {
                wnd = new MainWindow(MainCore);
                wnd.Show();
            }
            else
            {
                if(wnd.Visibility == Visibility.Hidden)
                {
                    wnd.Visibility = Visibility.Visible;
                }
                else
                {
                    wnd.Focus();
                }
            }
        }
        private void ExitButton_Click(object sender, EventArgs e)
        {
            // 現在のアプリケーションを終了
            MainCore.SaveConfig();
            Application.Current.Shutdown();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // MainWindow を生成、表示
            WindowShow();
        }
    }
}
