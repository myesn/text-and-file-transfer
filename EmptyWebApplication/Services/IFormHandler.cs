namespace EmptyWebApplication
{
    public interface IFormHandler
    {
        Task HandleAsync(FormCollectionHandlerOptions options);
    }
}
