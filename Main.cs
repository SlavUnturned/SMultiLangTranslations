using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using UP = Rocket.Unturned.Player.UnturnedPlayer;
using static SMultiLangTranslations.Utils;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;

namespace SMultiLangTranslations
{
    public class MultiLangManager : MultiLangRocketPlugin<Config>
    {
        public static MultiLangManager Instance;
        /// <summary>
        /// Creates new instance of <see cref="MultiLangTranslator" /> for <paramref name="plugin" /> <br/>
        /// You can use <see cref="MultiLangRocketPlugin" /> for easier integration
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns><see cref="MultiLangTranslator"/></returns>
        public static MultiLangTranslator RegisterTranslator(RocketPlugin plugin) => new MultiLangTranslator(plugin);
        /// <summary>
        /// Creates new instance of <see cref="MultiLangTranslator"/> for custom usage<br/>
        /// </summary>
        /// <returns><see cref="MultiLangTranslator"/></returns>
        public static MultiLangTranslator RegisterCustomTranslator(string directory, string prefix) => new MultiLangTranslator(directory, prefix);
        protected override void Unload()
        {
#if DEBUG
            U.Events.OnPlayerConnected -= playerConnected;
#endif
        }
        protected override void Load()
        {
            Instance = this;
#if DEBUG
            U.Events.OnPlayerConnected += playerConnected;
#endif
        }
#if DEBUG
        void playerConnected(UP up)
        {
            Logger.Log($"Player {up.CSteamID} with language {conf.GetLanguage(up.CSteamID)}");
            foreach(var translation in DefaultTranslations)
                UnturnedChat.Say(up, Translate(up.CSteamID, translation.Id));
        }
#endif
        public override TranslationList DefaultTranslations => new TranslationList
        {
            { LangCommand.LangChanged, "You successfully changed your language to: \"{0}\"" },
            { LangCommand.LangError, "Usage: `/lang [language]`. Example: `/lang ru`." },
        };
    }
}
