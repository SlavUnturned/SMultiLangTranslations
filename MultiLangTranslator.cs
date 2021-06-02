using Rocket.API.Collections;
using Rocket.Core.Assets;
using Rocket.Core.Plugins;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static SMultiLangTranslations.Utils;

namespace SMultiLangTranslations
{
    public interface IMultiLangTranslator
    {
        string Translate(string code, string key, params object[] placeholder);

        string Translate(string key, params object[] placeholder);

        string Translate(CSteamID steamID, string key, params object[] placeholder);
    }

    public sealed class MultiLangTranslator : IMultiLangTranslator, IDisposable
    {
        internal MultiLangTranslator(string directory, string prefix)
        {
            Dir = directory;
            Prefix = prefix;
            LoadTranslations();
            (Watcher = new FileSystemWatcher(Dir) { EnableRaisingEvents = true, IncludeSubdirectories = false }).Changed += (sender, e) => LoadTranslations();
        }

        internal MultiLangTranslator(RocketPlugin plugin) : this(plugin.Directory, plugin.Name) { }

        ~MultiLangTranslator()
        {
            Dispose();
        }

        public void Dispose()
        {
            Watcher.EnableRaisingEvents = false;
            Watcher.Dispose();
            SaveTranslations();
        }

        internal FileSystemWatcher Watcher;
        private string FilePattern => $"{Prefix}.*.translation.xml";

        public void ForAllFiles(Action<string, string> action)
        {
            foreach (var filePath in Directory.GetFiles(Dir, FilePattern))
            {
                var fileName = Path.GetFileName(filePath);
                var code = fileName.Substring(Prefix.Length + 1, 2).ToLower();
                if (!code.All(x => char.IsLetter(x)))
                {
                    var text = $"Wrong translation file with code: '{code}' for {Prefix}";
                    Console.WriteLine(text);
                    continue;
                }
                action(filePath, code);
            }
        }

        public void LoadTranslations()
        {
            ForAllFiles((filePath, code) =>
            {
                try
                {
                    var file = new XMLFileAsset<TranslationList>(filePath);
                    Translations[code.ToLower()] = file.Instance;
                }
                catch { }
            });
        }

        public void SaveTranslations()
        {
            foreach (var t in Translations.ToList())
            {
                try
                {
                    var filePath = FilePattern.Remove(Prefix.Length + 1, 1).Insert(Prefix.Length + 1, t.Key.ToLower());
                    var file = new XMLFileAsset<TranslationList>(filePath);
                    file.Instance = t.Value;
                    file.Save();
                }
                catch { }
            }
        }

        public readonly string Dir, Prefix;
        public readonly Dictionary<string, TranslationList> Translations = new Dictionary<string, TranslationList>();

        public string Translate(string code, string key, params object[] placeholder)
        {
            code = code.ToLower();
            if (Translations.Count == 0)
                return null;
            if (Translations.Count == 1)
                code = Translations.Keys.First();

            if (!Translations.ContainsKey(code) &&
                !Translations.ContainsKey(code = Utils.conf?.Mappings?.Alt(code)))
                return null;

            return Translations[code].Translate(key, placeholder);
        }

        public string Translate(string key, params object[] placeholder) => Translate(conf?.DefaultLangCode ?? Config.EnglishLangCode, key, placeholder);

        public string Translate(CSteamID steamID, string key, params object[] placeholder) => Translate(conf?.GetLanguage(steamID).Substring(0, 2) ?? Config.EnglishLangCode, key, placeholder);
    }
}