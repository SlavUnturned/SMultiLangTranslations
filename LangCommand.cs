using Rocket.API;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using static SMultiLangTranslations.Utils;
using Color = UnityEngine.Color;
using IRP = Rocket.API.IRocketPlayer;

namespace SMultiLangTranslations
{
    public class LangCommand : MultiLangRocketCommand
    {
        public override AllowedCaller AllowedCaller => AllowedCaller.Both;

        public override string Name => "lang";

        public override string Help => "Changes translation language of player.";

        public override string Syntax => "";

        public override List<string> Aliases => new List<string> { "language" };

        public override List<string> Permissions => new List<string> { Name };

        public override void OnExecute(CSteamID steamID, IRP caller, string[] args)
        {
            var message = Translate(LangError);
            var color = Color.yellow;
            string lang;
            if (args.Length == 1 && (lang = args[0]).Length == 2 && lang.All(x => char.IsLetter(x)))
            {
                conf.SetLanguage(steamID, lang);
                message = Translate(key: LangChanged, lang);
                color = default;
            }
            Say(message, color);
        }

        internal const string
            LangChanged = nameof(LangChanged),
            LangError = nameof(LangError);
    }
}