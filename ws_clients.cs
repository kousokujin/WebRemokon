using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;

namespace WebRemokon
{
    class ws_clients
    {
        public WebSocket ws;
        public EventHandler ReceiveMessage;
        public bool receive_loop;

        Task Receive_val;


        public ws_clients(WebSocket ws)
        {
            this.ws = ws;
            start_res();
        }

        private void start_res()
        {
            Receive_val = Receive();
        }

        public async Task Receive()
        {
            receive_loop = true;

            while (receive_loop)
            {
                var buffer = new byte[1024];


                //所得情報確保用の配列を準備
                var segment = new ArraySegment<byte>(buffer);

                //サーバからのレスポンス情報を取得
                var result = await ws.ReceiveAsync(segment, CancellationToken.None);

                //エンドポイントCloseの場合、処理を中断
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK",
                      CancellationToken.None);
                    continue;
                }

                //バイナリの場合は、当処理では扱えないため、処理を中断
                if (result.MessageType == WebSocketMessageType.Binary)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.InvalidMessageType,
                      "I don't do binary", CancellationToken.None);
                    continue;
                }

                //メッセージの最後まで取得
                int count = result.Count;
                while (!result.EndOfMessage)
                {
                    if (count >= buffer.Length)
                    {
                        await ws.CloseAsync(WebSocketCloseStatus.InvalidPayloadData,
                          "That's too long", CancellationToken.None);
                    }
                    else
                    {
                        segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                        result = await ws.ReceiveAsync(segment, CancellationToken.None);

                        count += result.Count;
                    }
                }

                //メッセージを取得
                var message = Encoding.UTF8.GetString(buffer, 0, count);

                ReceiveEvent re = new ReceiveEvent();
                re.mes = message;
                re.ws = this.ws;

                ReceiveMessage?.Invoke(this, re);
            }

        }

        public async Task sendMes(string mes)
        {
            var buffer = Encoding.UTF8.GetBytes(mes);
            var segment = new ArraySegment<byte>(buffer);

            if (ws.State != WebSocketState.Closed || ws.State != WebSocketState.Aborted)
            {
                //クライアント側に文字列を送信
                try
                {
                    await ws.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                    Console.WriteLine("sendmessage:{0}", mes);
                }
                catch (System.Net.WebSockets.WebSocketException)
                {
                    await this.Close();
                }
            }
        }

        public async Task Close()
        {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);

        }


    }

    class ReceiveEvent : EventArgs
    {
        public string mes;
        public WebSocket ws;
    }
}
