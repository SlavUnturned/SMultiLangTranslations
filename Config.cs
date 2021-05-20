using Rocket.API;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UP = Rocket.Unturned.Player.UnturnedPlayer;
using static SMultiLangTranslations.Utils;
using SDG.Unturned;

namespace SMultiLangTranslations
{
    public class LangMapper
    {
        public List<LangMap> Mappings = new List<LangMap>();
        /// <summary>
        /// Gets all mappings with key that equals <paramref name="code" />
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public IEnumerable<LangMap> this[string code]
        {
            get => Mappings.Where(x => x.Key.Equals(code, DefaultStrComparison));
        }
        /// <summary>
        /// Adds new mapping
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Map(string key, string value) => Mappings.Add(new LangMap(key, value));
        /// <summary>
        /// Tries to get alternative lang-code for <paramref name="code" />
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Key of first mapping where Value equals <paramref name="code" /> or <see cref="Config.DefaultLangCode"/> from configuration else <see cref="Config.DefaultNullLangCode"/></returns>
        public string Alt(string code) => (
            Mappings.FirstOrDefault(x => x.Value.Equals(code, DefaultStrComparison)) ?? 
            new LangMap(conf?.DefaultLangCode ?? Config.DefaultNullLangCode, null)
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
        public string Language = Config.DefaultNullLangCode;
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
        public const string DefaultNullLangCode = "00";
        public const string EnglishLangCode = "en";
        public string GetLanguage(CSteamID SteamID)
        {
            string GetLang(CSteamID steamID) => (Preferences.FirstOrDefault(x => x.SteamID == SteamID) ?? new PlayerPreferences()).Language;
            var p = PlayerTool.getPlayer(SteamID);
            var lang = GetLang(SteamID);
            if (p)
            {
                var up = UP.FromPlayer(p);
                return lang == DefaultNullLangCode ? up.SteamPlayer().language : lang;
            }
            return lang;
        }
        public void SetLanguage(CSteamID SteamID, string language)
        {
            language = language.ToLower();
            var prefId = Preferences.FindIndex(x => x.SteamID == SteamID);
            if (prefId == -1) 
                Preferences.Add(new PlayerPreferences(SteamID, language));
            else 
                Preferences[prefId].Language = language;
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
