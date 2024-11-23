namespace Rystem.OpenAi
{
    public sealed class OpenAiModelName
    {
        public static class Chat
        {
            public static readonly OpenAiModelName Gpt4_o = "gpt-4o";
            public static readonly OpenAiModelName Gpt_4o_2024_11_20 = "gpt-4o-2024-11-20";
            public static readonly OpenAiModelName Gpt_4o_2024_08_06 = "gpt-4o-2024-08-06";
            public static readonly OpenAiModelName Gpt_4o_audio_preview = "gpt-4o-audio-preview";
            public static readonly OpenAiModelName Gpt_4o_audio_preview_2024_10_01 = "gpt-4o-audio-preview-2024-10-01";
            public static readonly OpenAiModelName Gpt_4o_2024_05_13 = "gpt-4o-2024-05-13";
            public static readonly OpenAiModelName Gpt_4o_mini = "gpt-4o-mini";
            public static readonly OpenAiModelName Gpt_4o_mini_2024_07_18 = "gpt-4o-mini-2024-07-18";
            public static readonly OpenAiModelName O1_preview = "o1-preview";
            public static readonly OpenAiModelName O1_preview_2024_09_12 = "o1-preview-2024-09-12";
            public static readonly OpenAiModelName O1_mini = "o1-mini";
            public static readonly OpenAiModelName O1_mini_2024_09_12 = "o1-mini-2024-09-12";
            public static readonly OpenAiModelName ChatGpt_4o_latest = "chatgpt-4o-latest";
            public static readonly OpenAiModelName Gpt_4_turbo = "gpt-4-turbo";
            public static readonly OpenAiModelName Gpt_4_turbo_2024_04_09 = "gpt-4-turbo-2024-04-09";
            public static readonly OpenAiModelName Gpt_4 = "gpt-4";
            public static readonly OpenAiModelName Gpt_4_32k = "gpt-4-32k";
            public static readonly OpenAiModelName Gpt_4_0125_preview = "gpt-4-0125-preview";
            public static readonly OpenAiModelName Gpt_4_1106_preview = "gpt-4-1106-preview";
            public static readonly OpenAiModelName Gpt_4_vision_preview = "gpt-4-vision-preview";
        }

        public static class Embedding
        {
            public static readonly OpenAiModelName Text_embedding_3_small = "text-embedding-3-small";
            public static readonly OpenAiModelName Text_embedding_3_large = "text-embedding-3-large";
            public static readonly OpenAiModelName AdaV2 = "ada v2";
        }

        public static class FineTuning
        {
            public static readonly OpenAiModelName Gpt_4o_2024_08_06 = "gpt-4o-2024-08-06";
            public static readonly OpenAiModelName Gpt_4o_mini_2024_07_18 = "gpt-4o-mini-2024-07-18";
            public static readonly OpenAiModelName Gpt_35_turbo = "gpt-3.5-turbo";
            public static readonly OpenAiModelName Gpt_35_turbo_0125 = "gpt-3.5-turbo-0125";
            public static readonly OpenAiModelName Gpt_35_turbo_instruct = "gpt-3.5-turbo-instruct";
            public static readonly OpenAiModelName Gpt_35_turbo_1106 = "gpt-3.5-turbo-1106";
            public static readonly OpenAiModelName Gpt_35_turbo_0613 = "gpt-3.5-turbo-0613";
            public static readonly OpenAiModelName Gpt_35_turbo_16k_0613 = "gpt-3.5-turbo-16k-0613";
            public static readonly OpenAiModelName Gpt_35_turbo_0301 = "gpt-3.5-turbo-0301";
            public static readonly OpenAiModelName Davinci_002 = "davinci-002";
            public static readonly OpenAiModelName Babbage_002 = "babbage-002";
        }

        public static class RealTime
        {
            public static readonly OpenAiModelName Gpt_4o_realtime_preview = "gpt-4o-realtime-preview";
            public static readonly OpenAiModelName Gpt_4o_realtime_preview_2024_10_01 = "gpt-4o-realtime-preview-2024-10-01";
        }

        public static class Image
        {
            public static readonly OpenAiModelName Dalle3 = "DALL·E 3";
            public static readonly OpenAiModelName Dalle2 = "DALL·E 2";
        }

        public static class Audio
        {
            public static readonly OpenAiModelName Whisper = "Whisper";
            public static readonly OpenAiModelName Tts = "TTS";
            public static readonly OpenAiModelName TtsHd = "TTS HD";
        }

        internal OpenAiModelName(string name)
        {
            Name = name;
        }
        public string Name { get; private set; }
        public static implicit operator string(OpenAiModelName name)
            => name.Name;
        public static implicit operator OpenAiModelName(string name)
            => new(name);
    }
}
