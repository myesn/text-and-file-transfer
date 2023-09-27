using EmptyWebApplication;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddEmptyWebApplicationServices();
services.AddSignalR();
services.AddDirectoryBrowser();

var app = builder.Build();
var defaultEntryRelativePath = "/index.html";
var environment = app.Services.GetRequiredService<IWebHostEnvironment>();
var formCollectionHandler = app.Services.GetRequiredService<IFormHandler>();
var assetsAbsoluteFolder = Constants.GetDefaultAssetsAbsoluteFolder(environment);

// ensure exists assets folder
Directory.CreateDirectory(assetsAbsoluteFolder);

app.UseDirectoryBrowser(new DirectoryBrowserOptions()
{
    FileProvider = new PhysicalFileProvider(assetsAbsoluteFolder),
    RequestPath = PathString.FromUriComponent($"/{Constants.AssetsFolderName}"),
});
app.UseDefaultFiles();
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    // 不使用 app.MapPost 是因为，内部使用 EndpointMiddleware，
    // 其在找不到匹配的路由后会将 context.GetEndpoint()?.RequestDelegate 属性设置为 "405 Method Not Allowed"，
    // 会导致 app.UseDefaultFiles() 的 DefaultFilesMiddleware 无法执行，
    // 因为其内部需要 context.GetEndpoint()?.RequestDelegate is null 才可以生效
    var request = context.Request;
    var isPostMethod = request.Method.Equals(HttpMethods.Post, StringComparison.OrdinalIgnoreCase);
    var isMatchedPath = request.Path.Equals("/");
    if (!isPostMethod || !isMatchedPath)
    {
        await next(context);
        return;
    }

    await formCollectionHandler.HandleAsync(new FormCollectionHandlerOptions(request));
    context.Response.Redirect(defaultEntryRelativePath);
});

app.UseWebSockets();
app.MapHub<MessageHub>("/message-hub");

await app.RunAsync();
