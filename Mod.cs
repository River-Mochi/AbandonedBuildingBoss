// Mod.cs
namespace AbandonedBuildingBoss
{
    using Colossal.IO.AssetDatabase;
    using Colossal.Localization;
    using Colossal.Logging;
    using Game;
    using Game.Modding;
    using Game.SceneFlow;

    public sealed class Mod : IMod
    {
        public const string Name = "Abandoned Building Boss";
        public const string Version = "0.1.0";

        public static readonly ILog Log =
            LogManager.GetLogger($"{nameof(AbandonedBuildingBoss)}.{nameof(Mod)}")
                      .SetShowsErrorsInUI(false);

        public static Setting? m_Settings { get; private set; }

        public void OnLoad(UpdateSystem updateSystem)
        {
            Log.Info($"{Name} v{Version} - OnLoad");

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out ExecutableAsset execAsset))
                Log.Info($"Current mod asset at {execAsset.path}");

            // Settings page
            m_Settings = new Setting(this);

            // Register English strings so Options UI shows labels instead of raw keys
            LocalizationManager? lm = GameManager.instance?.localizationManager;
            if (lm != null)
            {
                Log.Info($"[Locale] ACTIVE at LOAD: {lm.activeLocaleId}");  // One-time info at load
                lm.AddSource("en-US", new LocaleEN(m_Settings));
                lm.AddSource("en", new LocaleEN(m_Settings));
            }

            // Load saved settings
            AssetDatabase.global.LoadSettings("AbandonedBuildingBoss", m_Settings, new Setting(this));
            // Expose Options UI
            m_Settings.RegisterInOptionsUI();

            // System registration (run before deletion system)
            updateSystem.UpdateAfter<AbandonedBuildingBossSystem>(SystemUpdatePhase.GameSimulation);
        }

        public void OnDispose()
        {
            Log.Info(nameof(OnDispose));
            if (m_Settings != null)
            {
                m_Settings.UnregisterInOptionsUI();
                m_Settings = null;
            }
        }
    }
}
