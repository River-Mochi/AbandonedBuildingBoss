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
            LogManager.GetLogger("AbandonedBuildingBoss").SetShowsErrorsInUI(false);

        public static Setting m_Settings { get; private set; } = null!;

        public void OnLoad(UpdateSystem updateSystem)
        {
            Log.Info($"{Name} v{Version} - OnLoad");

            // Executable asset path (useful for diagnostics)
            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out ExecutableAsset execAsset))
            {
                // Settings instance and persisted values (defaults provided via template pattern)
                m_Settings = new Setting(this);
            }

            AssetDatabase.global.LoadSettings(nameof(AbandonedBuildingBoss), m_Settings, new Setting(this));

            // Locale registration so Options UI shows labels (support both "en-US" and plain "en")
            LocalizationManager? lm = GameManager.instance?.localizationManager;
            if (lm != null)
            {
                Log.Info($"[Locale] ACTIVE at LOAD: {lm.activeLocaleId}");
                lm.AddSource("en-US", new LocaleEN(m_Settings));
                lm.AddSource("en", new LocaleEN(m_Settings));
            }

            // Expose Options UI
            m_Settings.RegisterInOptionsUI();

            // Simulation system scheduling (work gated by the Enabled toggle)
            updateSystem.UpdateAfter<AbandonedBuildingBossSystem>(SystemUpdatePhase.GameSimulation);
        }

        public void OnDispose()
        {
            Log.Info(nameof(OnDispose));
            if (m_Settings != null)
            {
                m_Settings.UnregisterInOptionsUI();
                m_Settings = null!;
            }
        }
    }
}
