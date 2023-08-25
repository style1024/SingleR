using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SingleR.Models;
using static SingleR.Models.MessageInfo;

namespace CoreMVC_SignalR_Chat.Hubs
{
    public class ChatHub : Hub
    {
        public static Dictionary<string, string> UserMap = new Dictionary<string, string>();
        private static List<string> UserNames = new List<string> { "Tom", "Jerry", "Sam", "Amy" };

        public override async Task OnConnectedAsync()
        {
            // Get available user names by excluding names that are already in use
            var availableNames = UserNames.Except(UserMap.Values).ToList();

            if (availableNames.Any())
            {
                // Assign the first available name to the new user
                string assignedName = availableNames.First();
                UserMap[Context.ConnectionId] = assignedName;

                // Convert user list to JSON and notify all clients to update their user list
                string jsonString = JsonConvert.SerializeObject(UserMap.Values.ToList());
                await Clients.All.SendAsync("UpdList", jsonString);

                // Notify the connecting client of their assigned name
                await Clients.Client(Context.ConnectionId).SendAsync("UpdSelfID", assignedName);
            }
            else
            {
                // Notify the connecting client that the chat room is full and close their connection
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", "The chat room is full. Please try again later.");
                Context.Abort();
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            // If the disconnecting user is in the UserMap, remove them
            if (UserMap.ContainsKey(Context.ConnectionId))
            {
                UserMap.Remove(Context.ConnectionId);
            }

            // Convert user list to JSON and notify all clients to update their user list
            string jsonString = JsonConvert.SerializeObject(UserMap.Values.ToList());
            await Clients.All.SendAsync("UpdList", jsonString);

            await base.OnDisconnectedAsync(ex);
        }

        /// <summary>
        /// 傳送訊息
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        public async Task<MessageOutput> SendMessageToRoom(MessageOutput output)
        {
            // 訊息處裡等等 
            if (!String.IsNullOrEmpty(output.Message))
                await Clients.Group(output.DocumentSEQ).SendAsync("ReceiveMessage", $"{output.UID}: {output.Message}");

            foreach (var item in output.FileName)
            {
                await Clients.Group(output.DocumentSEQ).SendAsync("ReceiveMessage", $"{output.UID}: {item}");
            }

            // 模擬回傳
            return output;
        }

        /// <summary>
        /// 進入聊天室
        /// </summary>
        /// <param name="roomName"></param>
        /// <returns></returns>
        public async Task JoinRoom(string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        }

        /// <summary>
        /// 離開聊天室
        /// </summary>
        /// <param name="roomName"></param>
        /// <returns></returns>
        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        }
    }
}
