using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace WebRemokon
{
    class AcrtiveWindow
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "GetWindowText", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);


        public string ActiveWindowName;
        public EventHandler ChangeActiveWindow;
        private Task ObsActiveWindow;
        
        public AcrtiveWindow()
        {
            ObsActiveWindow = GetActiveWindows();
        }

        async Task GetActiveWindows()
        {
            StringBuilder sb = new StringBuilder(65535);
            await Task.Run(() =>
            {
                while (true)
                {
                    IntPtr hWnd = GetForegroundWindow();
                    int id;
                    GetWindowThreadProcessId(hWnd, out id);
                    string prosessname = Process.GetProcessById(id).ProcessName;

                    if (ActiveWindowName != prosessname)
                    {
                        ChangeActiveWindowsEvent chgEvent = new ChangeActiveWindowsEvent();
                        chgEvent.windowsname = prosessname;
                        ChangeActiveWindow?.Invoke(this, chgEvent);
                        ActiveWindowName = prosessname;
                        Console.WriteLine("Window:{0}", ActiveWindowName);
                    }
                    Thread.Sleep(100);
                    
                }
            });
        }
    }


    class ChangeActiveWindowsEvent : EventArgs
    {
        public string windowsname;
    }
}
