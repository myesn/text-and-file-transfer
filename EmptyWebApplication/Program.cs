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
    // ��ʹ�� app.MapPost ����Ϊ���ڲ�ʹ�� EndpointMiddleware��
    // �����Ҳ���ƥ���·�ɺ�Ὣ context.GetEndpoint()?.RequestDelegate ��������Ϊ "405 Method Not Allowed"��
    // �ᵼ�� app.UseDefaultFiles() �� DefaultFilesMiddleware �޷�ִ�У�
    // ��Ϊ���ڲ���Ҫ context.GetEndpoint()?.RequestDelegate is null �ſ�����Ч
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
