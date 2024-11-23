using Microsoft.Extensions.DependencyInjection;
using Rystem.OpenAi.Chat;

namespace Rystem.OpenAi
{
    internal sealed class OpenAi : IOpenAi, IServiceForFactory
    {
        private readonly IFactory<OpenAiConfiguration> _configurationFactory;
        private readonly IFactory<IOpenAiAudio> _openAiAudioFactory;
        private readonly IFactory<IOpenAiChat> _openAiChatFactory;
        private readonly IFactory<IOpenAiEmbedding> _openAiEmbeddingFactory;
        private readonly IFactory<IOpenAiFineTune> _openAiFineTuneFactory;
        private readonly IFactory<IOpenAiFile> _openAiFileFactory;
        private readonly IFactory<IOpenAiImage> _openAiImageFactory;
        private readonly IFactory<IOpenAiModel> _openAiModelFactory;
        private readonly IFactory<IOpenAiModeration> _openAiModerationFactory;
        private readonly IFactory<IOpenAiManagement> _openAiManagementFactory;
        public OpenAi(
            IFactory<OpenAiConfiguration> configurationFactory,
            IFactory<IOpenAiAudio> openAiAudioFactory,
            IFactory<IOpenAiChat> openAiChatFactory,
            IFactory<IOpenAiEmbedding> openAiEmbeddingFactory,
            IFactory<IOpenAiFineTune> openAiFineTuneFactory,
            IFactory<IOpenAiFile> openAiFileFactory,
            IFactory<IOpenAiImage> openAiImageFactory,
            IFactory<IOpenAiModel> openAiModelFactory,
            IFactory<IOpenAiModeration> openAiModerationFactory,
            IFactory<IOpenAiManagement> openAiManagementFactory)
        {
            _configurationFactory = configurationFactory;
            _openAiAudioFactory = openAiAudioFactory;
            _openAiChatFactory = openAiChatFactory;
            _openAiEmbeddingFactory = openAiEmbeddingFactory;
            _openAiFineTuneFactory = openAiFineTuneFactory;
            _openAiFileFactory = openAiFileFactory;
            _openAiImageFactory = openAiImageFactory;
            _openAiModelFactory = openAiModelFactory;
            _openAiModerationFactory = openAiModerationFactory;
            _openAiManagementFactory = openAiManagementFactory;
        }
        public IOpenAiAudio Audio => field ??= _openAiAudioFactory.Create(_factoryName)!;
        public IOpenAiChat Chat => field ??= _openAiChatFactory.Create(_factoryName)!;
        public IOpenAiEmbedding Embeddings => field ??= _openAiEmbeddingFactory.Create(_factoryName)!;
        public IOpenAiFineTune FineTune => field ??= _openAiFineTuneFactory.Create(_factoryName)!;
        public IOpenAiFile File => field ??= _openAiFileFactory.Create(_factoryName)!;
        public IOpenAiImage Image => field ??= _openAiImageFactory.Create(_factoryName)!;
        public IOpenAiModel Model => field ??= _openAiModelFactory.Create(_factoryName)!;
        public IOpenAiModeration Moderation => field ??= _openAiModerationFactory.Create(_factoryName)!;
        public IOpenAiManagement Management => field ??= _openAiManagementFactory.Create(_factoryName)!;
        public OpenAiConfiguration Configuration => field ??= _configurationFactory.Create(_factoryName)!;
        private string? _factoryName;
        public void SetFactoryName(string name)
        {
            _factoryName = name;
            Configuration.Settings.RequestConfiguration?.Invoke(this);
        }
    }
}
