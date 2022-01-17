using Rocket.API;
using Rocket.Core;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using IRP = Rocket.API.IRocketPlayer;
using UP = Rocket.Unturned.Player.UnturnedPlayer;

namespace SMultiLangTranslations
{
    public abstract class MultiLangRocketCommand : IRocketCommand, IMultiLangTranslator
    {
        public MultiLangRocketCommand(IMultiLangTranslator translator = null)
        {
            _Translator = translator;
        }
        public virtual AllowedCaller AllowedCaller { get; } = AllowedCaller.Both;

        public abstract string Name { get; }

        public virtual string Help { get; }

        public virtual string Syntax => $"/{Name}";

        public virtual List<string> Aliases { get; } = new();

        public virtual List<string> Permissions => new() { Name };

        IMultiLangTranslator _Translator;
        public virtual IMultiLangTranslator Translator => _Translator ?? (_Translator = (R.Plugins.GetPlugin(GetType().Assembly) as IMultiLangTranslator));

        /// <summary>
        /// To use this method - make sure that your plugin is <see cref="IMultiLangTranslator"/> or <see cref="MultiLangRocketPlugin"/>
        /// </summary>
        /// <returns>Translated and formated text</returns>
        public virtual string Translate(string key, params object[] placeholder) => Translator?.Translate(key, placeholder);

        /// <summary>
        /// To use this method - make sure that your plugin is <see cref="IMultiLangTranslator"/> or <see cref="MultiLangRocketPlugin"/>
        /// </summary>
        /// <returns>Translated and formated text</returns>
        public virtual string Translate(string code, string key, params object[] placeholder) => Translator?.Translate(code, key, placeholder);

        /// <summary>
        /// To use this method - make sure that your plugin is <see cref="IMultiLangTranslator"/> or <see cref="MultiLangRocketPlugin"/>
        /// </summary>
        /// <returns>Translated and formated text</returns>
        public virtual string Translate(IRP player, string key, params object[] placeholder) => Translator?.Translate(player, key, placeholder);

        public virtual void Execute(IRP caller, string[] command) => Execute(caller.GetId(), command);
        public virtual void Execute(CSteamID steamId, string[] command) { }
    }
}