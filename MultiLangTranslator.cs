using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Assets;
using Rocket.Core.Plugins;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using static SMultiLangTranslations.Utils;

namespace SMultiLangTranslations
{

    public sealed class MultiLangTranslator : IMultiLangTranslator, IDisposable
    {
        internal MultiLangTranslator(string directory, string prefix)
        {
            Directory = directory;
            FilePrefix = prefix;
            LoadTranslations();
            (Watcher = new FileSystemWatcher(Directory) { EnableRaisingEvents = true, IncludeSubdirectories = false }).Changed += (sender, e) => LoadTranslations();
        }

        internal MultiLangTranslator(RocketPlugin plugin) : this(plugin.Directory, plugin.Name) { }

        ~MultiLangTranslator() { Dispose(); }

        public void Dispose()
        {
            Watcher.EnableRaisingEvents = false;
            Watcher.Dispose();
            SaveTranslations();
        }

        internal FileSystemWatcher Watcher;
        private string FilePattern => $"{FilePrefix}.*.translation.xml";

        public void ForAllFiles(Action<string, string> action)
        {
            foreach (var filePath in System.IO.Directory.GetFiles(Directory, FilePattern))
            {
                var fileName = Path.GetFileName(filePath);
                var code = fileName.Substring(FilePrefix.Length + 1, 2).ToLower();
                if (!code.All(x => char.IsLetter(x)))
                {
                    var text = $"Wrong translation file with code: '{code}' for {FilePrefix}";
                    Console.WriteLine(text);
                    continue;
                }
                action(filePath, code);
            }
        }

        static XmlSerializer serializer = new XmlSerializer(typeof(TranslationList));

        public void LoadTranslations()
        {
            ForAllFiles((filePath, code) =>
            {
                try
                {
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        var list = (TranslationList)serializer.Deserialize(fileStream);
                        Translations[code.ToLower()] = list;
                    }
                }
                catch (Exception ex) 
                {
#if DEBUG
                    Log(ex.ToString(), ConsoleColor.Red, Rocket.Core.Logging.ELogType.Exception);
#endif
                }
            });
        }

        public void SaveTranslations()
        {
            foreach (var t in Translations.ToList())
            {
                try
                {
                    var filePath = FilePattern.Remove(FilePrefix.Length + 1, 1).Insert(FilePrefix.Length + 1, t.Key.ToLower());
                    using (var fileStream = File.OpenWrite(filePath))
                    {
                        fileStream.Flush();
                        serializer.Serialize(fileStream, t.Value);
                    }
                }
                catch { }
            }
        }

        public readonly string Directory, FilePrefix;
        public readonly Dictionary<string, TranslationList> Translations = new Dictionary<string, TranslationList>();

        public string Translate(string code, string key, params object[] placeholder)
        {
            code = code.ToLower();
            if (Translations.Count <= 1)
                code = Translations.Keys.FirstOrDefault();

            if (!Translations.ContainsKey(code) &&
                !Translations.ContainsKey(code = conf?.Mappings?.Alternative(code)))
                return null;

            return Translations[code].Translate(key, placeholder);
        }

        public string Translate(string key, params object[] placeholder) => Translate(conf?.DefaultLangCode ?? Config.EnglishCode, key, placeholder);

        public string Translate(IRocketPlayer player, string key, params object[] placeholder) => Translate(conf?.GetLanguage(player.GetId()).Substring(0, 2) ?? Config.EnglishCode, key, placeholder);
    }
}