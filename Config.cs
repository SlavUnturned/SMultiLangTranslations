using Rocket.API;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using static SMultiLangTranslations.Utils;
using UP = Rocket.Unturned.Player.UnturnedPlayer;

namespace SMultiLangTranslations
{
    public class LangMapper
    {
        public List<LangMap> Mappings = new List<LangMap>();

        /// <param name="code">Language-code</param>
        /// <returns>All mappings with key that equals <paramref name="code" /></returns>
        public IEnumerable<LangMap> this[string code]
        {
            get => Mappings.Where(x => x.Key.Equals(code, DefaultStrComparison));
        }

        /// <summary>
        /// Adds new mapping
        /// </summary>
        public void Map(string key, string value) => Mappings.Add(new LangMap(key, value));

        /// <summary>
        /// Tries to get alternative lang-code for specified <paramref name="code" />
        /// </summary>
        /// <returns>Key of first mapping where Value equals <paramref name="code" /> or <see cref="Config.DefaultLangCode"/> from configuration else <see cref="Config.EnglishLangCode" /></returns>
        public string Alt(string code) => (
            Mappings.FirstOrDefault(x => x.Value.Equals(code, DefaultStrComparison)) ??
            new LangMap(conf?.DefaultLangCode ?? Config.EnglishLangCode, null)
            ).Key;
    }

    public class LangMap
    {
        [XmlAttribute]
        public string Key, Value;

        public LangMap() { }

        public LangMap(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }

    public class PlayerPreferences
    {
        [XmlAttribute("SteamID")]
        public ulong steamID;

        [XmlIgnore]
        public CSteamID SteamID { get => new CSteamID(steamID); private set => steamID = value.m_SteamID; }

        [XmlAttribute]
        public string Language;

        public PlayerPreferences() { }

        public PlayerPreferences(CSteamID steamID, string language)
        {
            SteamID = steamID;
            Language = language;
        }
    }

    public class Config : IRocketPluginConfiguration
    {
        public string DefaultLangCode = EnglishLangCode;
        public LangMapper Mappings = new LangMapper();
        public List<PlayerPreferences> Preferences = new List<PlayerPreferences>();
        public const string EnglishLangCode = "en";

        public string GetLanguage(CSteamID SteamID)
        {
            string GetLang(CSteamID steamID) => (Preferences.FirstOrDefault(x => x.SteamID == SteamID) ?? new PlayerPreferences()).Language;
            var p = PlayerTool.getPlayer(SteamID);
            var lang = GetLang(SteamID);
            lang = lang ?? p?.channel.owner.language ?? EnglishLangCode;
            return lang;
        }

        public void SetLanguage(CSteamID SteamID, string language)
        {
            language = language.ToLower();
            var prefId = Preferences.FindIndex(x => x.SteamID == SteamID);
            if (prefId == -1)
                Preferences.Add(new PlayerPreferences(SteamID, language));
            else Preferences[prefId].Language = language;
            inst.Configuration.Save();
        }

        public void LoadDefaults()
        {
            var en = EnglishLangCode;
            Mappings.Map(en, "gb");
            Mappings.Map(en, "uk");
        }
    }
}