using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebRemokon
{
    class MainCoreClass
    {

        public my_websocket server;
        public AcrtiveWindow window;

        public List<AppData> AppList { get; set; }

        private string nowWindow;

        public MainCoreClass()
        {
            server = new my_websocket(12255);
            window = new AcrtiveWindow();
            AppList = new List<AppData>();
            server.NewClient += new_client;
            window.ChangeActiveWindow += changeWindow;

            AppList.Add(new AppData
            {
                WindowName = "$default$",
                url = "default.html"
            });

            nowWindow = "";
        }

        void new_client(object sender, EventArgs ev)
        {
            if (ev is NewClientArg)
            {
                NewClientArg client = ev as NewClientArg;
                client.ws.ReceiveMessage += receive;
            }
        }

        void receive(object sender, EventArgs ev)
        {
            if (ev is ReceiveEvent)
            {
                string mes = (ev as ReceiveEvent).mes;
                Console.WriteLine(mes);
                SendKeys.SendWait(mes);
            }
        }

        void changeWindow(object sender, EventArgs e)
        {
            if (e is ChangeActiveWindowsEvent)
            {
                ChangeActiveWindowsEvent ev = e as ChangeActiveWindowsEvent;
                bool isWindow = false;

                AppList.ForEach(x => {
                    if (x.WindowName == ev.windowsname)
                    {
                        if (nowWindow != x.WindowName)
                        {
                            server.all_send(x.url);
                            isWindow = true;
                            nowWindow = x.WindowName;
                        }
                    }
                });

                if (isWindow == false && nowWindow != "default.html")
                {
                    AppList.ForEach(x =>
                    {
                        if (x.WindowName == "$default$")
                        {
                            server.all_send(x.url);
                        }
                        nowWindow = "default.html";
                    });
                }
            }
            else
            {
                return;
            }

        }
        
    }

    class AppData
    {
        public string WindowName{get; set;}
        public string url { get; set; }
    }
}
