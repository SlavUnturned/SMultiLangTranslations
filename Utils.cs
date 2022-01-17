using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Steamworks;
using System;
using UnityEngine;

namespace SMultiLangTranslations
{
    public static class Utils
    {
        internal static MultiLangManager inst => MultiLangManager.Instance;
        internal static Config conf => MultiLangManager.Instance?.Configuration?.Instance;
        const StringComparison DefaultStringComparison = StringComparison.InvariantCultureIgnoreCase;

        public static bool Compare(this string a, string b) => a.Equals(b, DefaultStringComparison);

        public static void Say(this IRocketPlayer to, string msg, Color? color = null, bool rich = false)
        {
            color ??= Color.white;
            if (to is ConsolePlayer)
                Log(msg, ToConsoleColor(color.Value));
            else UnturnedChat.Say(to, msg, color.Value, rich);
        }

        public static CSteamID GetId(this IRocketPlayer player)
        {
            if (ulong.TryParse(player.Id, out var id))
                id = 0;
            return (CSteamID)id;
        }
        public static string GetLanguageCode(this IRocketPlayer player) => conf.GetLanguage(player.GetId());

        public static ConsoleColor ToConsoleColor(this Color32 c)
        {
            int index = c.a > 128 ? 8 : 0; // Bright bit
            index |= (c.r > 64) ? 4 : 0; // Red bit
            index |= (c.g > 64) ? 2 : 0; // Green bit
            index |= (c.b > 64) ? 1 : 0; // Blue bit
            return (ConsoleColor)index;
        }

        public static void Log(string message, ConsoleColor color = ConsoleColor.White, ELogType type = ELogType.Info, bool log = true, bool rcon = true)
        {
            var savColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = savColor;
            if (log)
                AsyncLoggerQueue.Current.Enqueue(new LogEntry
                {
                    Severity = type,
                    Message = message,
                    RCON = rcon
                });
        }
    }
}