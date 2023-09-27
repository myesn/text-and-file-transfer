namespace EmptyWebApplication
{
    public interface IMessageCacheManager
    {
        Task<IEnumerable<MessageDto>> FetchAllAsync(MessageFetchAllQueryDto? query = null);
        Task PushAsync(MessageDto message);
        Task PushRangeAsync(IEnumerable<MessageDto> messages);
    }
}
