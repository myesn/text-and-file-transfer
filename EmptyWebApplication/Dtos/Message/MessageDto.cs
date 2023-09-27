namespace EmptyWebApplication
{
    public record class MessageDto(Guid Id, MessageDtoKind Kind, string Message, DateTime DateTime);

    public enum MessageDtoKind
    {
        TextInput = 1,
        FileUpload
    }
}
