using System.Collections.Concurrent;

namespace EmptyWebApplication
{
    public class MemoryMessageCacheManager : IMessageCacheManager
    {
        private readonly ConcurrentStack<MessageDto> _messageStack = new();

        public Task<IEnumerable<MessageDto>> FetchAllAsync(MessageFetchAllQueryDto? query = null)
        {
            if (query != null && query.Direction == MessageFetchAllQueryDirection.Asc)
            {
                return Task.FromResult(_messageStack.Reverse());
            }

            return Task.FromResult(_messageStack.AsEnumerable());
        }

        public Task PushAsync(MessageDto message)
        {
            ArgumentNullException.ThrowIfNull(message, nameof(message));

            _messageStack.Push(message);
            return Task.CompletedTask;
        }

        public Task PushRangeAsync(IEnumerable<MessageDto> messages)
        {
            ArgumentNullException.ThrowIfNull(messages, nameof(messages));

            _messageStack.PushRange(messages.ToArray());
            return Task.CompletedTask;
        }
    }
}
