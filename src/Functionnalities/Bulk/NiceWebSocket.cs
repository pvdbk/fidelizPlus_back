using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace fidelizPlus_back
{
    public class NiceWebSocket
    {
        private Task<WebSocket> WebSocket { get; }

        public NiceWebSocket(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                throw new AppException("Inappropriate use of NiceWebSocket");
            }
            WebSocket = context.WebSockets.AcceptWebSocketAsync();
        }

        public async Task<string> Read()
        {
            byte[] buffer = new byte[0x100];
            WebSocketReceiveResult read = await (await WebSocket).ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None
            );
            if (!read.EndOfMessage)
            {
                throw new AppException("Too long");
            }
            return Encoding.UTF8.GetString(buffer);
        }

        public async Task Send(string s) => await (await WebSocket).SendAsync(
            new ArraySegment<byte>(s.ToBytes(), 0, s.Length),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );

        public async Task Close(string s) =>
            await (await WebSocket).CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                s,
                CancellationToken.None
            );

        public async Task Error(string s) =>
            await (await WebSocket).CloseAsync(
                WebSocketCloseStatus.InvalidMessageType,
                s,
                CancellationToken.None
            );
    }
}
