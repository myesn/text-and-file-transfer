using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace EmptyWebApplication
{
    public class DefaultFormEventHandler : IFormEventHandler
    {
        private readonly IMessageCacheManager _messageCacheManager;
        private readonly IHubContext<MessageHub> _messageHubContext;

        public DefaultFormEventHandler(IMessageCacheManager messageCacheManager, IHubContext<MessageHub> messageHubContext)
        {
            ArgumentNullException.ThrowIfNull(messageCacheManager, nameof(messageCacheManager));
            ArgumentNullException.ThrowIfNull(messageHubContext, nameof(messageHubContext));

            _messageCacheManager = messageCacheManager;
            _messageHubContext = messageHubContext;
        }

        public async Task OnCompletedAsync(FormEventDto @event)
        {
            ArgumentNullException.ThrowIfNull(@event, nameof(@event));

            await _messageCacheManager.PushRangeAsync(@event.Messages);
            await SendMessagesToAllClients(@event.Messages);
        }


        private async Task SendMessagesToAllClients(IEnumerable<MessageDto> messages)
        {
            ArgumentNullException.ThrowIfNull(messages, nameof(messages));

            foreach (var message in messages)
            {
                var serializedMessage = JsonSerializer.Serialize(message, Constants.JsonSerializerOptions);
                await _messageHubContext.Clients.All.SendAsync("ReceiveMessage", serializedMessage);
            }
        }
    }
}
