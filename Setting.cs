// Setting.cs
namespace AbandonedBuildingBoss
{
    using Colossal.IO.AssetDatabase;
    using Game.Modding;
    using Game.Settings;
    using UnityEngine;      // Application.OpenURL

    [FileLocation("ModsSettings/AbandonedBuildingBoss/AbandonedBuildingBoss")]
    [SettingsUITabOrder(ActionsTab, AboutTab)]
    [SettingsUIGroupOrder(ActionsGroup, AboutGroup, LinksGroup)]
    [SettingsUIShowGroupName(ActionsGroup, AboutGroup, LinksGroup)]
    public sealed class Setting : ModSetting
    {
        // Tabs
        public const string ActionsTab = "Actions";
        public const string AboutTab = "About";

        // Groups
        public const string ActionsGroup = "Main";
        public const string AboutGroup = "Info";
        public const string LinksGroup = "Links";

        // URLs
        private const string UrlDiscord = "https://discord.gg/HTav7ARPs2";
        private const string UrlParadoxMods = "https://mods.paradoxplaza.com/authors/kimosabe1?orderBy=desc&sortBy=best&time=quarter"; // update when live

        public Setting(IMod mod) : base(mod) { }

        public override void SetDefaults()
        {
            Enabled = true;
        }

        // Actions (single toggle)
        [SettingsUISection(ActionsTab, ActionsGroup)]
        public bool Enabled { get; set; } = true;

        // About (name + version)
        [SettingsUISection(AboutTab, AboutGroup)]
        public string NameDisplay => Mod.Name;

        [SettingsUISection(AboutTab, AboutGroup)]
        public string VersionDisplay => Mod.Version;

        // Links (same group → same row)
        [SettingsUIButtonGroup(LinksGroup)]
        [SettingsUIButton]
        [SettingsUISection(AboutTab, LinksGroup)]
        public bool OpenParadoxMods { set { Application.OpenURL(UrlParadoxMods); } }

        [SettingsUIButtonGroup(LinksGroup)]
        [SettingsUIButton]
        [SettingsUISection(AboutTab, LinksGroup)]
        public bool OpenDiscord { set { Application.OpenURL(UrlDiscord); } }
    }

}
