using System.Text.Json;

namespace EmptyWebApplication
{
    public static class Constants
    {
        public static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true,
        };

        public const string AssetsFolderName = "assets";
        public static string GetDefaultAssetsAbsoluteFolder(IWebHostEnvironment environment)
        {
            ArgumentNullException.ThrowIfNull(environment, nameof(environment));
            ArgumentNullException.ThrowIfNullOrEmpty(AssetsFolderName, nameof(AssetsFolderName));

            return Path.Combine(environment.WebRootPath, AssetsFolderName);
        }
    }
}
