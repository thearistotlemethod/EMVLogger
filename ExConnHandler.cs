using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EMVLogger
{
    public class ExConnHandler
    {
        private ILogger<ExConnHandler> _logger { get; }

        private WebSocket _activeWebSocket = null;
        public ExConnHandler(ILogger<ExConnHandler> logger)
        {
            _logger = logger;
        }

        public async Task ConnectionArieved(WebSocket webSocket)
        {
            try
            {
                //_logger.LogDebug("Connected");

                //if (_activeWebSocket != null)
                //    await _activeWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "none", CancellationToken.None);
            }
            catch { }

            _activeWebSocket = webSocket;
            await Process(webSocket);
        }

        private async Task Process(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 32];
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                var req = Encoding.ASCII.GetString(buffer);
                _logger.LogDebug(req);
                //var res = await _agentApp.ProcessExternalRequest(req, PublishAsync);

                Array.Clear(buffer, 0, buffer.Length);
                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }

        public async Task<bool> PublishAsync(string Topic, string Payload, bool Retain = false)
        {
            _logger.LogDebug("External Payload -> " + Payload + " ");
            var baRes = Encoding.ASCII.GetBytes(Payload);
            await _activeWebSocket.SendAsync(new ArraySegment<byte>(baRes), WebSocketMessageType.Text, true, CancellationToken.None);
            return true;

        }

        public async Task Publish(string msg)
        {
            if (_activeWebSocket != null && _activeWebSocket.State == WebSocketState.Open)
            {
                var encoded = Encoding.UTF8.GetBytes(msg);
                var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
                await _activeWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
