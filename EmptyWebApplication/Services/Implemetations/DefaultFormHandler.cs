namespace EmptyWebApplication
{
    public class DefaultFormHandler : IFormHandler
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IFormEventHandler _eventHandler;

        public DefaultFormHandler(IWebHostEnvironment environment, IFormEventHandler eventHandler)
        {
            ArgumentNullException.ThrowIfNull(environment, nameof(environment));
            ArgumentNullException.ThrowIfNull(eventHandler, nameof(eventHandler));

            _environment = environment;
            _eventHandler = eventHandler;
        }

        public async Task HandleAsync(FormCollectionHandlerOptions options)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(options));

            // handle form submit
            var formCollection = await options.HttpRequest.ReadFormAsync();
            var assetsAbsoluteFolder = Constants.GetDefaultAssetsAbsoluteFolder(_environment);
            var textInputMessage = await HandleTextInputAsync(formCollection);
            var fileUploadMessage = await HandleUploadedFilesAsync(options.HttpRequest.Form.Files, assetsAbsoluteFolder);

            // emit "form submit completed" event
            var maybeNullMessages = new List<MessageDto?>() { textInputMessage, fileUploadMessage };
            var requiredMessages = maybeNullMessages.Where(x => x != null).OfType<MessageDto>();
            await _eventHandler.OnCompletedAsync(new FormEventDto(requiredMessages));
        }


        private Task<MessageDto?> HandleTextInputAsync(IFormCollection formCollection, string inputKey = "input")
        {
            ArgumentNullException.ThrowIfNull(formCollection, nameof(formCollection));

            if (!formCollection.TryGetValue(inputKey, out var input))
            {
                return Task.FromResult<MessageDto?>(default);
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                return Task.FromResult<MessageDto?>(default);
            }

            return Task.FromResult<MessageDto?>(new MessageDto(Guid.NewGuid(), MessageDtoKind.TextInput, input.ToString(), DateTime.Now));
        }
        private async Task<MessageDto?> HandleUploadedFilesAsync(IFormFileCollection files, string assetsAbsoluteFolder)
        {
            ArgumentNullException.ThrowIfNull(files, nameof(files));
            ArgumentNullException.ThrowIfNullOrEmpty(assetsAbsoluteFolder, nameof(assetsAbsoluteFolder));

            if (!files.Any()) return default;

            var fileNames = new List<string>();
            foreach (var file in files)
            {
                var fileName = file.FileName;
                var filePath = Path.Combine(assetsAbsoluteFolder, fileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);

                await file.CopyToAsync(fileStream);
                fileNames.Add(fileName);
            }

            var combinedFileName = string.Join("、", fileNames);
            return new MessageDto(Guid.NewGuid(), MessageDtoKind.FileUpload, combinedFileName, DateTime.Now);
        }

    }
}
