namespace Rystem.OpenAi
{
    public sealed class ChatModelName : ModelName
    {
        internal ChatModelName(string name) : base(name) { }
        public static readonly ChatModelName Gpt4_o = "gpt-4o";
        public static readonly ChatModelName Gpt_4o_2024_11_20 = "gpt-4o-2024-11-20";
        public static readonly ChatModelName Gpt_4o_2024_08_06 = "gpt-4o-2024-08-06";
        public static readonly ChatModelName Gpt_4o_audio_preview = "gpt-4o-audio-preview";
        public static readonly ChatModelName Gpt_4o_audio_preview_2024_10_01 = "gpt-4o-audio-preview-2024-10-01";
        public static readonly ChatModelName Gpt_4o_2024_05_13 = "gpt-4o-2024-05-13";
        public static readonly ChatModelName Gpt_4o_mini = "gpt-4o-mini";
        public static readonly ChatModelName Gpt_4o_mini_2024_07_18 = "gpt-4o-mini-2024-07-18";
        public static readonly ChatModelName O1_preview = "o1-preview";
        public static readonly ChatModelName O1_preview_2024_09_12 = "o1-preview-2024-09-12";
        public static readonly ChatModelName O1_mini = "o1-mini";
        public static readonly ChatModelName O1_mini_2024_09_12 = "o1-mini-2024-09-12";
        public static readonly ChatModelName ChatGpt_4o_latest = "chatgpt-4o-latest";
        public static readonly ChatModelName Gpt_4_turbo = "gpt-4-turbo";
        public static readonly ChatModelName Gpt_4_turbo_2024_04_09 = "gpt-4-turbo-2024-04-09";
        public static readonly ChatModelName Gpt_4 = "gpt-4";
        public static readonly ChatModelName Gpt_4_32k = "gpt-4-32k";
        public static readonly ChatModelName Gpt_4_0125_preview = "gpt-4-0125-preview";
        public static readonly ChatModelName Gpt_4_1106_preview = "gpt-4-1106-preview";
        public static readonly ChatModelName Gpt_4_vision_preview = "gpt-4-vision-preview";
        public static implicit operator string(ChatModelName name)
            => name.Name;
        public static implicit operator ChatModelName(string name)
            => new(name);
    }
    public sealed class ModerationModelName : ModelName
    {
        internal ModerationModelName(string name) : base(name) { }
        public static readonly ModerationModelName OmniLatest = "omni-moderation-latest";
        public static implicit operator string(ModerationModelName name)
            => name.Name;
        public static implicit operator ModerationModelName(string name)
            => new(name);
    }
    public sealed class EmbeddingModelName : ModelName
    {
        internal EmbeddingModelName(string name) : base(name) { }
        public static readonly EmbeddingModelName Text_embedding_3_small = "text-embedding-3-small";
        public static readonly EmbeddingModelName Text_embedding_3_large = "text-embedding-3-large";
        public static readonly EmbeddingModelName AdaV2 = "ada v2";
        public static implicit operator string(EmbeddingModelName name)
            => name.Name;
        public static implicit operator EmbeddingModelName(string name)
            => new(name);
    }
    public sealed class FineTuningModelName : ModelName
    {
        internal FineTuningModelName(string name) : base(name) { }
        public static readonly FineTuningModelName Gpt_4o_2024_08_06 = "gpt-4o-2024-08-06";
        public static readonly FineTuningModelName Gpt_4o_mini_2024_07_18 = "gpt-4o-mini-2024-07-18";
        public static readonly FineTuningModelName Gpt_35_turbo = "gpt-3.5-turbo";
        public static readonly FineTuningModelName Gpt_35_turbo_0125 = "gpt-3.5-turbo-0125";
        public static readonly FineTuningModelName Gpt_35_turbo_instruct = "gpt-3.5-turbo-instruct";
        public static readonly FineTuningModelName Gpt_35_turbo_1106 = "gpt-3.5-turbo-1106";
        public static readonly FineTuningModelName Gpt_35_turbo_0613 = "gpt-3.5-turbo-0613";
        public static readonly FineTuningModelName Gpt_35_turbo_16k_0613 = "gpt-3.5-turbo-16k-0613";
        public static readonly FineTuningModelName Gpt_35_turbo_0301 = "gpt-3.5-turbo-0301";
        public static readonly FineTuningModelName Davinci_002 = "davinci-002";
        public static readonly FineTuningModelName Babbage_002 = "babbage-002";
        public static implicit operator string(FineTuningModelName name)
            => name.Name;
        public static implicit operator FineTuningModelName(string name)
            => new(name);
    }
    public sealed class RealTimeModelName : ModelName
    {
        internal RealTimeModelName(string name) : base(name) { }
        public static readonly RealTimeModelName Gpt_4o_realtime_preview = "gpt-4o-realtime-preview";
        public static readonly RealTimeModelName Gpt_4o_realtime_preview_2024_10_01 = "gpt-4o-realtime-preview-2024-10-01";
        public static implicit operator string(RealTimeModelName name)
            => name.Name;
        public static implicit operator RealTimeModelName(string name)
            => new(name);
    }
    public sealed class ImageModelName : ModelName
    {
        internal ImageModelName(string name) : base(name) { }
        public static readonly ImageModelName Dalle3 = "DALL·E 3";
        public static readonly ImageModelName Dalle2 = "DALL·E 2";
        public static implicit operator string(ImageModelName name)
            => name.Name;
        public static implicit operator ImageModelName(string name)
            => new(name);
    }
    public sealed class AudioModelName : ModelName
    {
        public static readonly AudioModelName Whisper = "Whisper";
        internal AudioModelName(string name) : base(name) { }

        public static implicit operator string(AudioModelName name)
            => name.Name;
        public static implicit operator AudioModelName(string name)
            => new(name);
    }
    public sealed class SpeechModelName : ModelName
    {
        internal SpeechModelName(string name) : base(name) { }
        public static readonly SpeechModelName Tts = "tts-1";
        public static readonly SpeechModelName TtsHd = "tts-1-hd";
        public static implicit operator string(SpeechModelName name)
            => name.Name;
        public static implicit operator SpeechModelName(string name)
            => new(name);
    }

    public class ModelName
    {
        internal ModelName(string name)
        {
            Name = name;
        }
        public string Name { get; private set; }
        public static implicit operator string(ModelName name)
            => name.Name;
        public static implicit operator ModelName(string name)
            => new(name);
    }
}
