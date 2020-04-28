using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WebRemokon
{
    static class VKeyCode
    {
        [DllImport("user32.dll")]
        public static extern uint keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        public static void pushKey(List<byte> keys,int delay)
        {
            // キーの押し下げをシミュレートする。
            keys.ForEach((x) =>
            {
                keybd_event(x, 0, 0, (UIntPtr)0);
            });
            System.Threading.Thread.Sleep(delay);

            keys.ForEach((x) =>
            {
                keybd_event(x, 0, 2, (UIntPtr)0);
            });
        }

        /// <summary>
        /// 仮想キーコード(16進数)を,で区切ってkeyに代入する。
        /// "0x41,0x42,0x43,0x44"だとabcdと入力。
        /// "0xa0|0x41,0x42,0x43,0x44"だとAbcdと入力。
        /// |を使うことで同時押しを再現可能
        /// </summary>
        /// <param name="key"></param>
        public static void pushKey(string key)
        {
            string[] keysec = key.Split(',');
            foreach(string x in keysec)
            {
                string[] keypara = x.Split('|');
                List<byte> byList = new List<byte>();

                foreach (string y in keypara)
                {
                    if (y.StartsWith("0x"))
                    {
                        Console.WriteLine(y.Substring(2, 2));
                        byList.Add(Convert.ToByte(y.Substring(2,2), 16));
                    }
                    else
                    {
                        byte[] data = System.Text.Encoding.ASCII.GetBytes(y);
                        byList.Add(data[0]);
                    }
                }
                pushKey(byList, 100);
            }
        }
    }
}
