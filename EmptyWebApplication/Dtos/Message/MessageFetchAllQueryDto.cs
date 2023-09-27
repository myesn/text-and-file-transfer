namespace EmptyWebApplication
{
    public record class MessageFetchAllQueryDto(MessageFetchAllQueryDirection Direction = MessageFetchAllQueryDirection.Desc);

    public enum MessageFetchAllQueryDirection
    {
        Asc = 1,
        Desc = 2
    }
}
