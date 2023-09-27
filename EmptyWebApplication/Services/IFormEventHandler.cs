namespace EmptyWebApplication
{
    public interface IFormEventHandler
    {
        Task OnCompletedAsync(FormEventDto @event);
    }
}
