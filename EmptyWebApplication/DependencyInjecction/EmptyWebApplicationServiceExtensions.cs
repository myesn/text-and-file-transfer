using EmptyWebApplication;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EmptyWebApplicationServiceExtensions
    {
        public static IServiceCollection AddEmptyWebApplicationServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<IMessageCacheManager, MemoryMessageCacheManager>()
                .AddSingleton<IFormHandler, DefaultFormHandler>()
                .AddSingleton<IFormEventHandler, DefaultFormEventHandler>();
        }
    }
}
