using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace fidelizPlus_back
{
	public class NiceWebSocket
	{
		private Task<WebSocket> WebSocket { get; }

		public NiceWebSocket(HttpContext context) =>
			WebSocket = context.WebSockets.IsWebSocketRequest
				? context.WebSockets.AcceptWebSocketAsync()
				: new Break("Inappropriate use of NiceWebSocket", BreakCode.ErrNiceWS).Throw<Task<WebSocket>>();

		public async Task<string> Read()
		{
			byte[] buffer = new byte[0x100];
			WebSocketReceiveResult read = await (await WebSocket).ReceiveAsync(
				new ArraySegment<byte>(buffer),
				CancellationToken.None
			);
			return read.EndOfMessage
				? buffer.BytesToString()
				: new Break("Too long", BreakCode.BigString).Throw<string>();
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
