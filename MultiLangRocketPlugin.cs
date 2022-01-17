using Rocket.API;
using Rocket.Core.Plugins;
using Steamworks;

namespace SMultiLangTranslations
{
    public abstract class MultiLangRocketPlugin : RocketPlugin, IMultiLangTranslator
    {
        public MultiLangRocketPlugin()
        {
            Translator = MultiLangManager.RegisterTranslator(this);
        }

        public virtual IMultiLangTranslator Translator { get; }

        public string Translate(string code, string key, params object[] placeholder) => Translator.Translate(code, key, placeholder);

        public new string Translate(string key, params object[] placeholder) => Translator.Translate(key, placeholder);

        public string Translate(IRocketPlayer player, string key, params object[] placeholder) => Translator.Translate(player, key, placeholder);
    }

    public abstract class MultiLangRocketPlugin<PluginConfiguration> : RocketPlugin<PluginConfiguration>, IMultiLangTranslator
        where PluginConfiguration : class, IRocketPluginConfiguration
    {
        public MultiLangRocketPlugin()
        {
            Translator = MultiLangManager.RegisterTranslator(this);
        }

        public virtual IMultiLangTranslator Translator { get; }

        public string Translate(string code, string key, params object[] placeholder) => Translator.Translate(code, key, placeholder);

        public new string Translate(string key, params object[] placeholder) => Translator.Translate(key, placeholder);

        public string Translate(IRocketPlayer player, string key, params object[] placeholder) => Translator.Translate(player, key, placeholder);
    }
}