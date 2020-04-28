using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace WebRemokon
{
    public class MainCoreClass
    {

        public my_websocket server;
        public AcrtiveWindow window;

        public List<AppData> AppList { get; set; }

        private string nowWindow;
        public bool isRun;

        public MainCoreClass()
        {
            //Console.OutputEncoding = new UTF8Encoding();

            try
            {
                server = new my_websocket(12255);
            }
            catch (System.Net.HttpListenerException)
            {
                MessageBox.Show("Webサーバを開始できまん。管理者権限で\n netsh http add urlacl url=http://+:12255/ user=Everyone を実行してください。",
                "Webリモコン",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                isRun = false;
                return;
            }
            isRun = true;
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
                if (mes.StartsWith("<HEX>") == true)
                {
                    string s = Regex.Replace(mes, "<HEX>", "");
                    VKeyCode.pushKey(s);
                }
                else
                {
                    Console.WriteLine(mes);
                    SendKeys.SendWait(mes);
                }
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

    public class AppData
    {
        public string WindowName{get; set;}
        public string url { get; set; }
    }
}
