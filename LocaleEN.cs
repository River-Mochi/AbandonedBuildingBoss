// LocaleEN.cs
namespace AbandonedBuildingBoss
{
    using System.Collections.Generic;
    using Colossal;

    public sealed class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                // Tabs
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Actions" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),   "About"   },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsGroup), "Main"  },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutGroup),   "Info"  },
                { m_Setting.GetOptionGroupLocaleID(Setting.LinksGroup),   "Links" },

                // Actions
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Enabled)), "Enabled" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.Enabled)),  "Turn the mod on or off." },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NameDisplay)),    "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.NameDisplay)),     "Mod display name." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.VersionDisplay)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.VersionDisplay)),  "Installed version." },

                // Links (two buttons in same row)
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "Open the Paradox Mods page." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenDiscord)),     "Discord" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenDiscord)),      "Open the Discord invite." },

                // Settings page header
                { m_Setting.GetSettingsLocaleID(), "Abandoned Building Boss" },
            };
        }

        public void Unload()
        {
        }
    }
}
