using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace EmptyWebApplication
{
    /// <summary>
    /// The Hub class manages cnnections, groups, and messaging.
    /// <para>https://learn.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-7.0&tabs=visual-studio</para>
    /// </summary>
    public class MessageHub : Hub
    {
        private readonly IMessageCacheManager _messageCacheManager;

        public MessageHub(IMessageCacheManager messageCacheManager)
        {
            ArgumentNullException.ThrowIfNull(messageCacheManager, nameof(messageCacheManager));

            _messageCacheManager = messageCacheManager;
        }

        ///// <summary>
        ///// 任意一个已连接的 client 都可以调用本函数，将 message 发送给所有的 clients.
        ///// <para>client 监听 ReceiveMessage 事件</para>
        ///// </summary>
        ///// <param name="message">message content</param>
        ///// <returns></returns>
        //public async Task SendMessageAsync(string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", message);
        //}

        public override async Task OnConnectedAsync()
        {
            var messages = await _messageCacheManager.FetchAllAsync(
                new MessageFetchAllQueryDto(MessageFetchAllQueryDirection.Asc)
            );
            foreach (var message in messages)
            {
                var serializedMessage = JsonSerializer.Serialize(message, Constants.JsonSerializerOptions);
                await Clients.Caller.SendAsync("ReceiveMessage", serializedMessage);
            }
        }
    }
}
