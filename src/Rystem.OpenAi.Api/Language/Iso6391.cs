using System.Collections.Generic;

namespace Rystem.OpenAi
{
    public enum Language
    {
        Abkhazian,
        Afar,
        Afrikaans,
        Akan,
        Albanian,
        Amharic,
        Arabic,
        Aragonese,
        Armenian,
        Assamese,
        Avaric,
        Avestan,
        Aymara,
        Azerbaijani,
        Bambara,
        Bashkir,
        Basque,
        Belarusian,
        Bengali,
        Bislama,
        Bosnian,
        Breton,
        Bulgarian,
        Burmese,
        Catalan,
        Chamorro,
        Chechen,
        Chichewa,
        Chinese,
        ChurchSlavonic,
        Chuvash,
        Cornish,
        Corsican,
        Cree,
        Croatian,
        Czech,
        Danish,
        Divehi,
        Dutch,
        Dzongkha,
        English,
        Esperanto,
        Estonian,
        Ewe,
        Faroese,
        Fijian,
        Finnish,
        French,
        WesternFrisian,
        Fulah,
        Gaelic,
        Galician,
        Ganda,
        Georgian,
        German,
        ModernGreek,
        Kalaallisut,
        Guarani,
        Gujarati,
        Haitian,
        Hausa,
        Hebrew,
        Herero,
        Hindi,
        HiriMotu,
        Hungarian,
        Icelandic,
        Ido,
        Igbo,
        Indonesian,
        Interlingua,
        InterlinguaOccidental,
        Inuktitut,
        Inupiaq,
        Irish,
        Italian,
        Japanese,
        Javanese,
        Kannada,
        Kanuri,
        Kashmiri,
        Kazakh,
        CentralKhmer,
        Kikuyu,
        Kinyarwanda,
        Kirghiz,
        Komi,
        Kongo,
        Korean,
        Kuanyama,
        Kurdish,
        Lao,
        Latin,
        Latvian,
        Limburgan,
        Lingala,
        Lithuanian,
        LubaKatanga,
        Luxembourgish,
        Macedonian,
        Malagasy,
        Malay,
        Malayalam,
        Maltese,
        Manx,
        Maori,
        Marathi,
        Marshallese,
        Mongolian,
        Nauru,
        NavajoNavaho,
        NorthNdebele,
        SouthNdebele,
        Ndonga,
        Nepali,
        Norwegian,
        NorwegianBokmål,
        NorwegianNynorsk,
        SichuanYiNuosu,
        Occitan,
        Ojibwa,
        Oriya,
        Oromo,
        Ossetian,
        Pali,
        Pashto,
        Persian,
        Polish,
        Portuguese,
        Punjabi,
        Quechua,
        Romanian,
        Romansh,
        Rundi,
        Russian,
        NorthernSami,
        Samoan,
        Sango,
        Sanskrit,
        Sardinian,
        Serbian,
        Shona,
        Sindhi,
        Sinhala,
        Slovak,
        Slovenian,
        Somali,
        SouthernSotho,
        Spanish,
        Sundanese,
        Swahili,
        Swati,
        Swedish,
        Tagalog,
        Tahitian,
        Tajik,
        Tamil,
        Tatar,
        Telugu,
        Thai,
        Tibetan,
        Tigrinya,
        Tonga,
        Tsonga,
        Tswana,
        Turkish,
        Turkmen,
        Twi,
        Uighur,
        Ukrainian,
        Urdu,
        Uzbek,
        Venda,
        Vietnamese,
        Volapük,
        Walloon,
        Welsh,
        Wolof,
        Xhosa,
        Yiddish,
        Yoruba,
        Zhuang,
        Zulu
    }
    public static class LanguageExtensions
    {
        private static readonly Dictionary<Language, string> s_languages = LoadLanguages();
        private static Dictionary<Language, string> LoadLanguages()
        {
            var languages = new Dictionary<Language, string>();
            languages.Add(Language.Abkhazian, "ab");
            languages.Add(Language.Afar, "aa");
            languages.Add(Language.Afrikaans, "af");
            languages.Add(Language.Akan, "ak");
            languages.Add(Language.Albanian, "sq");
            languages.Add(Language.Amharic, "am");
            languages.Add(Language.Arabic, "ar");
            languages.Add(Language.Aragonese, "an");
            languages.Add(Language.Armenian, "hy");
            languages.Add(Language.Assamese, "as");
            languages.Add(Language.Avaric, "av");
            languages.Add(Language.Avestan, "ae");
            languages.Add(Language.Aymara, "ay");
            languages.Add(Language.Azerbaijani, "az");
            languages.Add(Language.Bambara, "bm");
            languages.Add(Language.Bashkir, "ba");
            languages.Add(Language.Basque, "eu");
            languages.Add(Language.Belarusian, "be");
            languages.Add(Language.Bengali, "bn");
            languages.Add(Language.Bislama, "bi");
            languages.Add(Language.Bosnian, "bs");
            languages.Add(Language.Breton, "br");
            languages.Add(Language.Bulgarian, "bg");
            languages.Add(Language.Burmese, "my");
            languages.Add(Language.Catalan, "ca");
            languages.Add(Language.Chamorro, "ch");
            languages.Add(Language.Chechen, "ce");
            languages.Add(Language.Chichewa, "ny");
            languages.Add(Language.Chinese, "zh");
            languages.Add(Language.ChurchSlavonic, "cu");
            languages.Add(Language.Chuvash, "cv");
            languages.Add(Language.Cornish, "kw");
            languages.Add(Language.Corsican, "co");
            languages.Add(Language.Cree, "cr");
            languages.Add(Language.Croatian, "hr");
            languages.Add(Language.Czech, "cs");
            languages.Add(Language.Danish, "da");
            languages.Add(Language.Divehi, "dv");
            languages.Add(Language.Dutch, "nl");
            languages.Add(Language.Dzongkha, "dz");
            languages.Add(Language.English, "en");
            languages.Add(Language.Esperanto, "eo");
            languages.Add(Language.Estonian, "et");
            languages.Add(Language.Ewe, "ee");
            languages.Add(Language.Faroese, "fo");
            languages.Add(Language.Fijian, "fj");
            languages.Add(Language.Finnish, "fi");
            languages.Add(Language.French, "fr");
            languages.Add(Language.WesternFrisian, "fy");
            languages.Add(Language.Fulah, "ff");
            languages.Add(Language.Gaelic, "gd");
            languages.Add(Language.Galician, "gl");
            languages.Add(Language.Ganda, "lg");
            languages.Add(Language.Georgian, "ka");
            languages.Add(Language.German, "de");
            languages.Add(Language.ModernGreek, "el");
            languages.Add(Language.Kalaallisut, "kl");
            languages.Add(Language.Guarani, "gn");
            languages.Add(Language.Gujarati, "gu");
            languages.Add(Language.Haitian, "ht");
            languages.Add(Language.Hausa, "ha");
            languages.Add(Language.Hebrew, "he");
            languages.Add(Language.Herero, "hz");
            languages.Add(Language.Hindi, "hi");
            languages.Add(Language.HiriMotu, "ho");
            languages.Add(Language.Hungarian, "hu");
            languages.Add(Language.Icelandic, "is");
            languages.Add(Language.Ido, "io");
            languages.Add(Language.Igbo, "ig");
            languages.Add(Language.Indonesian, "id");
            languages.Add(Language.Interlingua, "ia");
            languages.Add(Language.InterlinguaOccidental, "ie");
            languages.Add(Language.Inuktitut, "iu");
            languages.Add(Language.Inupiaq, "ik");
            languages.Add(Language.Irish, "ga");
            languages.Add(Language.Italian, "it");
            languages.Add(Language.Japanese, "ja");
            languages.Add(Language.Javanese, "jv");
            languages.Add(Language.Kannada, "kn");
            languages.Add(Language.Kanuri, "kr");
            languages.Add(Language.Kashmiri, "ks");
            languages.Add(Language.Kazakh, "kk");
            languages.Add(Language.CentralKhmer, "km");
            languages.Add(Language.Kikuyu, "ki");
            languages.Add(Language.Kinyarwanda, "rw");
            languages.Add(Language.Kirghiz, "ky");
            languages.Add(Language.Komi, "kv");
            languages.Add(Language.Kongo, "kg");
            languages.Add(Language.Korean, "ko");
            languages.Add(Language.Kuanyama, "kj");
            languages.Add(Language.Kurdish, "ku");
            languages.Add(Language.Lao, "lo");
            languages.Add(Language.Latin, "la");
            languages.Add(Language.Latvian, "lv");
            languages.Add(Language.Limburgan, "li");
            languages.Add(Language.Lingala, "ln");
            languages.Add(Language.Lithuanian, "lt");
            languages.Add(Language.LubaKatanga, "lu");
            languages.Add(Language.Luxembourgish, "lb");
            languages.Add(Language.Macedonian, "mk");
            languages.Add(Language.Malagasy, "mg");
            languages.Add(Language.Malay, "ms");
            languages.Add(Language.Malayalam, "ml");
            languages.Add(Language.Maltese, "mt");
            languages.Add(Language.Manx, "gv");
            languages.Add(Language.Maori, "mi");
            languages.Add(Language.Marathi, "mr");
            languages.Add(Language.Marshallese, "mh");
            languages.Add(Language.Mongolian, "mn");
            languages.Add(Language.Nauru, "na");
            languages.Add(Language.NavajoNavaho, "nv");
            languages.Add(Language.NorthNdebele, "nd");
            languages.Add(Language.SouthNdebele, "nr");
            languages.Add(Language.Ndonga, "ng");
            languages.Add(Language.Nepali, "ne");
            languages.Add(Language.Norwegian, "no");
            languages.Add(Language.NorwegianBokmål, "nb");
            languages.Add(Language.NorwegianNynorsk, "nn");
            languages.Add(Language.SichuanYiNuosu, "ii");
            languages.Add(Language.Occitan, "oc");
            languages.Add(Language.Ojibwa, "oj");
            languages.Add(Language.Oriya, "or");
            languages.Add(Language.Oromo, "om");
            languages.Add(Language.Ossetian, "os");
            languages.Add(Language.Pali, "pi");
            languages.Add(Language.Pashto, "ps");
            languages.Add(Language.Persian, "fa");
            languages.Add(Language.Polish, "pl");
            languages.Add(Language.Portuguese, "pt");
            languages.Add(Language.Punjabi, "pa");
            languages.Add(Language.Quechua, "qu");
            languages.Add(Language.Romanian, "ro");
            languages.Add(Language.Romansh, "rm");
            languages.Add(Language.Rundi, "rn");
            languages.Add(Language.Russian, "ru");
            languages.Add(Language.NorthernSami, "se");
            languages.Add(Language.Samoan, "sm");
            languages.Add(Language.Sango, "sg");
            languages.Add(Language.Sanskrit, "sa");
            languages.Add(Language.Sardinian, "sc");
            languages.Add(Language.Serbian, "sr");
            languages.Add(Language.Shona, "sn");
            languages.Add(Language.Sindhi, "sd");
            languages.Add(Language.Sinhala, "si");
            languages.Add(Language.Slovak, "sk");
            languages.Add(Language.Slovenian, "sl");
            languages.Add(Language.Somali, "so");
            languages.Add(Language.SouthernSotho, "st");
            languages.Add(Language.Spanish, "es");
            languages.Add(Language.Sundanese, "su");
            languages.Add(Language.Swahili, "sw");
            languages.Add(Language.Swati, "ss");
            languages.Add(Language.Swedish, "sv");
            languages.Add(Language.Tagalog, "tl");
            languages.Add(Language.Tahitian, "ty");
            languages.Add(Language.Tajik, "tg");
            languages.Add(Language.Tamil, "ta");
            languages.Add(Language.Tatar, "tt");
            languages.Add(Language.Telugu, "te");
            languages.Add(Language.Thai, "th");
            languages.Add(Language.Tibetan, "bo");
            languages.Add(Language.Tigrinya, "ti");
            languages.Add(Language.Tonga, "to");
            languages.Add(Language.Tsonga, "ts");
            languages.Add(Language.Tswana, "tn");
            languages.Add(Language.Turkish, "tr");
            languages.Add(Language.Turkmen, "tk");
            languages.Add(Language.Twi, "tw");
            languages.Add(Language.Uighur, "ug");
            languages.Add(Language.Ukrainian, "uk");
            languages.Add(Language.Urdu, "ur");
            languages.Add(Language.Uzbek, "uz");
            languages.Add(Language.Venda, "ve");
            languages.Add(Language.Vietnamese, "vi");
            languages.Add(Language.Volapük, "vo");
            languages.Add(Language.Walloon, "wa");
            languages.Add(Language.Welsh, "cy");
            languages.Add(Language.Wolof, "wo");
            languages.Add(Language.Xhosa, "xh");
            languages.Add(Language.Yiddish, "yi");
            languages.Add(Language.Yoruba, "yo");
            languages.Add(Language.Zhuang, "za");
            languages.Add(Language.Zulu, "zu");
            return languages;
        }
        public static string ToIso639_1(this Language language)
        {
            if (s_languages.ContainsKey(language))
                return s_languages[language];
            return s_languages[Language.English];
        }
    }
}
