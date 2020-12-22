using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRChat.ConsoleApp
{
    public class ChatManager
    {
        private HubConnection _hubConnection;

        public event EventHandler<ChatMessage> OnReceivedMessage;
        public event EventHandler<ConnectionState> OnConnectionStateChanged;
        public bool IsConnected { get; private set; }

        private ConnectionState _ConnectionState = ConnectionState.Disconnected;
        public ConnectionState ConnectionState
        {
            get => _ConnectionState;
            set
            {
                _ConnectionState = value;
                OnConnectionStateChanged?.Invoke(this, _ConnectionState);
            }
        }

        public ChatManager(string url)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                OnReceivedMessage?.Invoke(this, new ChatMessage { User = user, Message = message });
            });

            _hubConnection.Closed += hubConnection_Closed;
            _hubConnection.Reconnecting += hubConnection_Reconnecting;
            _hubConnection.Reconnected += hubConnection_Reconnected;
        }

        private Task hubConnection_Reconnected(string arg)
        {
            IsConnected = true;
            ConnectionState = ConnectionState.Connected;
            return Task.FromResult(0);
        }

        private Task hubConnection_Reconnecting(Exception arg)
        {
            IsConnected = false;
            ConnectionState = ConnectionState.Reconnecting;
            return Task.FromResult(0);
        }

        private Task hubConnection_Closed(Exception arg)
        {
            IsConnected = false;
            ConnectionState = ConnectionState.Disconnected;
            return Task.FromResult(0);
        }

        public async Task ConnectAsync()
        {
            if (IsConnected)
                return;

            ConnectionState = ConnectionState.Connecting;

            await _hubConnection.StartAsync();
            IsConnected = true;
            ConnectionState = ConnectionState.Connected;
        }

        public async Task SendMessageAsync(string username, string message)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected");

            await _hubConnection.InvokeAsync("SendMessage", username, message);
        }

    }
}
