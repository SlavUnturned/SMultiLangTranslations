using Rocket.API;
using Steamworks;

namespace SMultiLangTranslations
{
    public interface IMultiLangTranslator
    {
        string Translate(string code, string key, params object[] placeholder);

        string Translate(string key, params object[] placeholder);

        string Translate(IRocketPlayer player, string key, params object[] placeholder);
    }
}