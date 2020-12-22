using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace SignalRChat.ConsoleApp
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var chatManager = new ChatManager("https://localhost:5001/chathub");
            Console.WriteLine("Please enter your username:");
            var username = Console.ReadLine();
            chatManager.OnReceivedMessage += (o, e) =>
            {
                Console.WriteLine($"{e.User}: {e.Message}");
            };

            chatManager.OnConnectionStateChanged += (o, e) =>
            {
                Console.WriteLine($"Connection Changed: {e}");
            };

            await chatManager.ConnectAsync();

            

            Console.WriteLine("Connected");

            var wantsToExit = false;
            while (!wantsToExit)
            {
                var text = Console.ReadLine();
                if(text == "exit")
                {
                    wantsToExit = true;
                }
                else
                {
                    await chatManager.SendMessageAsync(username, text);
                }
            }

            Console.WriteLine("Chat ended.");
        }
    }
}
