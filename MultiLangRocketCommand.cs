using Rocket.API;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Core.Utils;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using IRP = Rocket.API.IRocketPlayer;
using Logger = Rocket.Core.Logging.Logger;
using UP = Rocket.Unturned.Player.UnturnedPlayer;

namespace SMultiLangTranslations
{
    public abstract class MultiLangRocketCommand : IRocketCommand, IMultiLangTranslator
    {
        public abstract AllowedCaller AllowedCaller { get; }

        public abstract string Name { get; }

        public abstract string Help { get; }

        public abstract string Syntax { get; }

        public abstract List<string> Aliases { get; }

        public abstract List<string> Permissions { get; }

        MultiLangTranslator Translator;
        CSteamID steamID;

        /// <summary>
        /// To use this method - make sure that your plugin is <see langword="MultiLangRocketPlugin"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="placeholder"></param>
        /// <returns></returns>
        public string Translate(string key, params object[] placeholder) => Translator?.Translate(steamID, key, placeholder);
        public string Translate(string code, string key, params object[] placeholder) => Translator?.Translate(code, key, placeholder);
        public string Translate(CSteamID steamID, string key, params object[] placeholder) => Translator?.Translate(steamID, key, placeholder);

        public bool IsConsole(CSteamID steamID) => steamID.m_SteamID == defaultConsoleID;
        const ulong defaultConsoleID = 1;

        public void Say(string msg, Color color = default(Color), bool rich = false) => Say(steamID, msg, color, rich);
        public void Say(IRP irp, string msg, Color color = default(Color), bool rich = false) => Say(new CSteamID(irp.Id == new ConsolePlayer().Id ? defaultConsoleID : ulong.Parse(irp.Id)), msg, color, rich);
        public void Say(CSteamID steamID, string msg, Color color = default(Color), bool rich = false)
        {
            var isConsole = IsConsole(steamID);
            if (color == default)
            {
                if (isConsole)
                    color = Color.gray;
                else color = Color.green;
            }
            if (isConsole)
            {
                var cmdColor = Utils.FromColor(color);
                var savColor = Console.ForegroundColor;
                Console.ForegroundColor = cmdColor;
                Logger.ExternalLog(msg, cmdColor);
                Console.WriteLine(msg);
                Console.ForegroundColor = savColor;
            }
            else
                ChatManager.serverSendMessage(msg, color, toPlayer: UP.FromCSteamID(steamID)?.SteamPlayer(), mode: EChatMode.SAY, useRichTextFormatting: rich);
        }

        public void Execute(IRP caller, string[] command)
        {
            var rocketPlugin = R.Plugins.GetPlugin(GetType().Assembly);
            Translator = rocketPlugin
                .GetType().GetField(nameof(MultiLangRocketPlugin._Translator), BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(rocketPlugin) as MultiLangTranslator;
            if (caller is ConsolePlayer)
                steamID = new CSteamID(defaultConsoleID);
            else steamID = (caller as UP).CSteamID;
            OnExecute(steamID, caller, command);
        }
        public abstract void OnExecute(CSteamID CSteamID, IRP caller, string[] command);
    }
}
