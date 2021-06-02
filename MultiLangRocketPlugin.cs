using Rocket.API;
using Rocket.Core.Plugins;
using Steamworks;

namespace SMultiLangTranslations
{
    public abstract class MultiLangRocketPlugin : RocketPlugin, IMultiLangTranslator
    {
        public MultiLangRocketPlugin()
        {
            _Translator = MultiLangManager.RegisterTranslator(this);
        }

        internal MultiLangTranslator _Translator;
        protected MultiLangTranslator Translator => _Translator;

        public string Translate(string code, string key, params object[] placeholder) => Translator.Translate(code, key, placeholder);

        public new string Translate(string key, params object[] placeholder) => Translator.Translate(key, placeholder);

        public string Translate(CSteamID steamID, string key, params object[] placeholder) => Translator.Translate(steamID, key, placeholder);
    }

    public abstract class MultiLangRocketPlugin<PluginConfiguration> : RocketPlugin<PluginConfiguration>, IMultiLangTranslator
        where PluginConfiguration : class, IRocketPluginConfiguration
    {
        public MultiLangRocketPlugin()
        {
            _Translator = MultiLangManager.RegisterTranslator(this);
        }

        internal MultiLangTranslator _Translator;
        protected MultiLangTranslator Translator => _Translator;

        public string Translate(string code, string key, params object[] placeholder) => Translator.Translate(code, key, placeholder);

        public new string Translate(string key, params object[] placeholder) => Translator.Translate(key, placeholder);

        public string Translate(CSteamID steamID, string key, params object[] placeholder) => Translator.Translate(steamID, key, placeholder);
    }
}