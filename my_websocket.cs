using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace WebRemokon
{
    public class my_websocket
    {
        HttpListener listener;
        List<ws_clients> ws;

        public bool ws_loop;
        Task ListenerTask;

        public EventHandler NewClient;
        int port;

        public my_websocket(int port)
        {
            this.port = port;
            StartServer();
            ws = new List<ws_clients>();
            ListenerTask = WS_LisenerLoop();
        }
        private void StartServer()
        {
            listener = new HttpListener();
            //listener.Prefixes.Add("http://+:80/Temporary_Listen_Addresses/ws/");
            listener.Prefixes.Add(string.Format("http://+:{0}/ws/",port));
            listener.Prefixes.Add(string.Format("http://+:{0}/page/", port));
            listener.Start();
        }

        private async Task Run()
        {
            var hc = await listener.GetContextAsync();

            if (hc.Request.IsWebSocketRequest)
            {
                //WebSocketでレスポンスを返却
                var wsc = await hc.AcceptWebSocketAsync(null);
                var myws = new ws_clients(wsc.WebSocket);
                ws.Add(myws);
                var new_arg = new NewClientArg();
                new_arg.ws = myws;
                NewClient?.Invoke(this, new_arg);

                List<int> delWS = ws.Select((value, index) => new { x = value, i = index }).Where(y => (y.x.ws.State == WebSocketState.Closed)|| (y.x.ws.State == WebSocketState.Aborted)).Select(x=>x.i).ToList();
                delWS.ForEach(x =>
                {
                    ws.RemoveAt(x);
                });
            }
            else {

                WebResponse(hc);
                hc.Response.Close();
                return;
            }
        }

        public async Task WS_LisenerLoop()
        {
            ws_loop = true;

            while (ws_loop)
            {
                await Run();
            }
        }

        /// <summary>
        ///すべてのソケットをクローズ
        /// </summary>
        public void all_close()
        {
            ws.ForEach(async x => await x.Close());
        }

        public void all_send(string message)
        {
            ws.ForEach(async x => await x.sendMes(message));
        }

        public void WebResponse(HttpListenerContext context)
        {
            string url = context.Request.RawUrl;
            string path = url.Replace("/", "\\").Remove(0,1);
            string[] paths = path.Split('?');
            path = paths[0];
            if (File.Exists(path))
            {
                byte[] content = File.ReadAllBytes(path);
                context.Response.OutputStream.Write(content, 0, content.Length);
            }
            else
            {
                context.Response.StatusCode = 404;
            }
        }
    }

    class NewClientArg : EventArgs
    {
        public ws_clients ws;
    }
}
