namespace Rystem.OpenAi
{
    public sealed class ChatModelName : ModelName
    {
        internal ChatModelName(string name) : base(name) { }
        
        // GPT-5 Series
        public static ChatModelName Gpt_5_1 { get; } = "gpt-5.1";
        public static ChatModelName Gpt_5 { get; } = "gpt-5";
        public static ChatModelName Gpt_5_mini { get; } = "gpt-5-mini";
        public static ChatModelName Gpt_5_nano { get; } = "gpt-5-nano";
        public static ChatModelName Gpt_5_pro { get; } = "gpt-5-pro";
        public static ChatModelName Gpt_5_1_chat_latest { get; } = "gpt-5.1-chat-latest";
        public static ChatModelName Gpt_5_chat_latest { get; } = "gpt-5-chat-latest";
        public static ChatModelName Gpt_5_1_codex { get; } = "gpt-5.1-codex";
        public static ChatModelName Gpt_5_codex { get; } = "gpt-5-codex";
        public static ChatModelName Gpt_5_1_codex_mini { get; } = "gpt-5.1-codex-mini";
        public static ChatModelName Gpt_5_search_api { get; } = "gpt-5-search-api";
        
        // GPT-4.5 Series
        public static ChatModelName Gpt_4_5_preview { get; } = "gpt-4.5-preview";
        public static ChatModelName Gpt_4_5_preview_2025_02_27 { get; } = "gpt-4.5-preview-2025-02-27";
        
        // GPT-4.1 Series (existing)
        public static ChatModelName Gpt_4_1 { get; } = "gpt-4.1";
        public static ChatModelName Gpt_4_1_2025_04_14 { get; } = "gpt-4.1-2025-04-14";
        public static ChatModelName Gpt_4_1_mini { get; } = "gpt-4.1-mini";
        public static ChatModelName Gpt_4_1_mini_2025_04_14 { get; } = "gpt-4.1-mini-2025-04-14";
        public static ChatModelName Gpt_4_1_nano { get; } = "gpt-4.1-nano";
        public static ChatModelName Gpt_4_1_nano_2025_04_14 { get; } = "gpt-4.1-nano-2025-04-14";
        
        // GPT-4o Series (existing)
        public static ChatModelName Gpt_4o { get; } = "gpt-4o";
        public static ChatModelName Gpt_4o_2024_11_20 { get; } = "gpt-4o-2024-11-20";
        public static ChatModelName Gpt_4o_2024_08_06 { get; } = "gpt-4o-2024-08-06";
        public static ChatModelName Gpt_4o_2024_05_13 { get; } = "gpt-4o-2024-05-13";
        public static ChatModelName Gpt_4o_mini { get; } = "gpt-4o-mini";
        public static ChatModelName Gpt_4o_mini_2024_07_18 { get; } = "gpt-4o-mini-2024-07-18";
        public static ChatModelName Gpt_4o_latest { get; } = "gpt-4o-latest";
        
        // GPT-4o Audio
        public static ChatModelName Gpt_4o_audio_preview { get; } = "gpt-4o-audio-preview";
        public static ChatModelName Gpt_4o_audio_preview_2024_10_01 { get; } = "gpt-4o-audio-preview-2024-10-01";
        public static ChatModelName Gpt_4o_audio_preview_2024_12_17 { get; } = "gpt-4o-audio-preview-2024-12-17";
        public static ChatModelName Gpt_4o_mini_audio_preview { get; } = "gpt-4o-mini-audio-preview";
        public static ChatModelName Gpt_4o_mini_audio_preview_2024_12_17 { get; } = "gpt-4o-mini-audio-preview-2024-12-17";
        
        // GPT-4o Realtime
        public static ChatModelName Gpt_4o_realtime_preview { get; } = "gpt-4o-realtime-preview";
        public static ChatModelName Gpt_4o_realtime_preview_2024_12_17 { get; } = "gpt-4o-realtime-preview-2024-12-17";
        public static ChatModelName Gpt_4o_mini_realtime_preview { get; } = "gpt-4o-mini-realtime-preview";
        public static ChatModelName Gpt_4o_mini_realtime_preview_2024_12_17 { get; } = "gpt-4o-mini-realtime-preview-2024-12-17";
        
        // GPT-4o Search
        public static ChatModelName Gpt_4o_search_preview { get; } = "gpt-4o-search-preview";
        public static ChatModelName Gpt_4o_search_preview_2025_03_11 { get; } = "gpt-4o-search-preview-2025-03-11";
        public static ChatModelName Gpt_4o_mini_search_preview { get; } = "gpt-4o-mini-search-preview";
        public static ChatModelName Gpt_4o_mini_search_preview_2025_03_11 { get; } = "gpt-4o-mini-search-preview-2025-03-11";
        
        // GPT Realtime/Audio (new generic)
        public static ChatModelName Gpt_realtime { get; } = "gpt-realtime";
        public static ChatModelName Gpt_realtime_mini { get; } = "gpt-realtime-mini";
        public static ChatModelName Gpt_audio { get; } = "gpt-audio";
        public static ChatModelName Gpt_audio_mini { get; } = "gpt-audio-mini";
        
        // GPT-4 Legacy
        public static ChatModelName Gpt_4_turbo { get; } = "gpt-4-turbo";
        public static ChatModelName Gpt_4_turbo_2024_04_09 { get; } = "gpt-4-turbo-2024-04-09";
        public static ChatModelName Gpt_4 { get; } = "gpt-4";
        public static ChatModelName Gpt_4_32k { get; } = "gpt-4-32k";
        public static ChatModelName Gpt_4_0125_preview { get; } = "gpt-4-0125-preview";
        public static ChatModelName Gpt_4_1106_preview { get; } = "gpt-4-1106-preview";
        public static ChatModelName Gpt_4_vision_preview { get; } = "gpt-4-vision-preview";
        public static ChatModelName Gpt_4_0613 { get; } = "gpt-4-0613";
        public static ChatModelName Gpt_4_0314 { get; } = "gpt-4-0314";
        
        // O-Series
        public static ChatModelName O1 { get; } = "o1";
        public static ChatModelName O1_2024_12_17 { get; } = "o1-2024-12-17";
        public static ChatModelName O1_preview { get; } = "o1-preview";
        public static ChatModelName O1_preview_2024_09_12 { get; } = "o1-preview-2024-09-12";
        public static ChatModelName O1_mini { get; } = "o1-mini";
        public static ChatModelName O1_mini_2024_09_12 { get; } = "o1-mini-2024-09-12";
        public static ChatModelName O1_pro { get; } = "o1-pro";
        public static ChatModelName O1_pro_2025_03_19 { get; } = "o1-pro-2025-03-19";
        
        public static ChatModelName O3 { get; } = "o3";
        public static ChatModelName O3_2025_04_16 { get; } = "o3-2025-04-16";
        public static ChatModelName O3_pro { get; } = "o3-pro";
        public static ChatModelName O3_deep_research { get; } = "o3-deep-research";
        public static ChatModelName O3_mini { get; } = "o3-mini";
        public static ChatModelName O3_mini_2025_01_31 { get; } = "o3-mini-2025-01-31";
        
        public static ChatModelName O4_mini { get; } = "o4-mini";
        public static ChatModelName O4_mini_2025_04_16 { get; } = "o4-mini-2025-04-16";
        public static ChatModelName O4_mini_deep_research { get; } = "o4-mini-deep-research";
        
        // Codex
        public static ChatModelName Codex_mini_latest { get; } = "codex-mini-latest";
        
        // Computer Use
        public static ChatModelName Computer_use_preview { get; } = "computer-use-preview";
        public static ChatModelName Computer_use_preview_2025_03_11 { get; } = "computer-use-preview-2025-03-11";
        
        // Image Models
        public static ChatModelName Gpt_image_1 { get; } = "gpt-image-1";
        public static ChatModelName Gpt_image_1_mini { get; } = "gpt-image-1-mini";
        
        // Transcription Models
        public static ChatModelName Gpt_4o_transcribe { get; } = "gpt-4o-transcribe";
        public static ChatModelName Gpt_4o_transcribe_diarize { get; } = "gpt-4o-transcribe-diarize";
        public static ChatModelName Gpt_4o_mini_transcribe { get; } = "gpt-4o-mini-transcribe";
        public static ChatModelName Gpt_4o_mini_tts { get; } = "gpt-4o-mini-tts";
        
        // GPT-3.5 Series (Legacy)
        public static ChatModelName Gpt_3_5_turbo { get; } = "gpt-3.5-turbo";
        public static ChatModelName Gpt_3_5_turbo_0125 { get; } = "gpt-3.5-turbo-0125";
        public static ChatModelName Gpt_3_5_turbo_1106 { get; } = "gpt-3.5-turbo-1106";
        public static ChatModelName Gpt_3_5_turbo_0613 { get; } = "gpt-3.5-turbo-0613";
        public static ChatModelName Gpt_3_5_0301 { get; } = "gpt-3.5-0301";
        public static ChatModelName Gpt_3_5_turbo_instruct { get; } = "gpt-3.5-turbo-instruct";
        public static ChatModelName Gpt_3_5_turbo_16k_0613 { get; } = "gpt-3.5-turbo-16k-0613";
        
        // ChatGPT
        public static ChatModelName Chatgpt_4o_latest { get; } = "chatgpt-4o-latest";
        
        // Base Models (Legacy)
        public static ChatModelName Davinci_002 { get; } = "davinci-002";
        public static ChatModelName Babbage_002 { get; } = "babbage-002";

        // Implicit conversions
        public static implicit operator string(ChatModelName name)
            => name.Name;
        public static implicit operator ChatModelName(string name)
            => new(name);
    }
}
