using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

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
            //Console.OutputEncoding = new UTF8Encoding();

            server = new my_websocket(12255);
            window = new AcrtiveWindow();
            AppList = new List<AppData>();
            server.NewClient += new_client;
            window.ChangeActiveWindow += changeWindow;

            bool isConfig = LoadConfig();
            if (isConfig == false)
            {
                AppList.Add(new AppData
                {
                    WindowName = "$default$",
                    url = "default.html"
                });
                SaveConfig();
            }

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
        public void SaveConfig()
        {
            string formattedJson = JsonConvert.SerializeObject(AppList, Formatting.Indented);
            Encoding enc = Encoding.GetEncoding("UTF-8");
            StreamWriter writer = new StreamWriter(@"setting.json", false, enc);
            Console.WriteLine(formattedJson);
            writer.WriteLine(formattedJson);
            writer.Close();
        }

        public bool LoadConfig()
        {
            if (File.Exists("setting.json"))
            {

                StreamReader sr = new StreamReader(@"setting.json", Encoding.GetEncoding("UTF-8"));
                string str = sr.ReadToEnd();
                sr.Close();
                AppList = JsonConvert.DeserializeObject<List<AppData>>(str);

                bool isDefault = false;
                AppList.ForEach(x =>
                {
                    if (x.WindowName == "$default$")
                    {
                        isDefault = true;
                    }
                });

                if (isDefault == false)
                {
                    AppList.Add(new AppData
                    {
                        WindowName = "$default$",
                        url = "default.html"
                    });

                    SaveConfig();
                }

                return true;
            }
            return false;
        }

    }

    class AppData
    {
        public string WindowName{get; set;}
        public string url { get; set; }
    }
}
