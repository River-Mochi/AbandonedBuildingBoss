// AbandonedBuildingBossSystem.cs
namespace AbandonedBuildingBoss
{
    using Game;
    using Game.Areas;
    using Game.Buildings;
    using Game.Common;
    using Game.Net;
    using Game.Tools;
    using Unity.Collections;
    using Unity.Entities;

    public partial class AbandonedBuildingBossSystem : GameSystemBase
    {
        private const int kUpdateIntervalFrames = 32; // adjust to 16 for faster cleanup vs. overhead
        private EntityQuery m_AbandonedBuildingQuery;

        protected override void OnCreate()
        {
            base.OnCreate();

            // Targets Abandoned + Building; excludes Deleted and Temp to prevent double work.
            m_AbandonedBuildingQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<Abandoned>(),
                    ComponentType.ReadOnly<Building>()
                },
                None = new[]
                {
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>()
                }
            });

            RequireForUpdate(m_AbandonedBuildingQuery);
            Mod.Log.Info("AbandonedBuildingBossSystem created.");
        }

        protected override void OnUpdate()
        {
            // Skip when disabled.
            if (!Mod.m_Settings.Enabled)
                return;

            // Fast no-op: avoid allocation when query is empty.
            if (m_AbandonedBuildingQuery.IsEmptyIgnoreFilter)
                return;

            // Collect matching entities; each is processed once, then excluded by 'Deleted'.
            NativeArray<Entity> abandonedBuildings = m_AbandonedBuildingQuery.ToEntityArray(Allocator.Temp);
            EntityManager em = EntityManager;

            foreach (Entity entity in abandonedBuildings)
            {
                // Remove linked areas.
                if (em.HasBuffer<SubArea>(entity))
                {
                    DynamicBuffer<SubArea> subareas = em.GetBuffer<SubArea>(entity);
                    foreach (SubArea subArea in subareas)
                        em.AddComponent<Deleted>(subArea.m_Area);
                }

                // Remove linked nets.
                if (em.HasBuffer<SubNet>(entity))
                {
                    DynamicBuffer<SubNet> subnets = em.GetBuffer<SubNet>(entity);
                    foreach (SubNet net in subnets)
                        em.AddComponent<Deleted>(net.m_SubNet);
                }

                // Remove linked lanes.
                if (em.HasBuffer<SubLane>(entity))
                {
                    DynamicBuffer<SubLane> sublanes = em.GetBuffer<SubLane>(entity);
                    foreach (SubLane lane in sublanes)
                        em.AddComponent<Deleted>(lane.m_SubLane);
                }

                // Finally remove the building entity.
                em.AddComponent<Deleted>(entity);
            }

            // Release temp storage.
            abandonedBuildings.Dispose();
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            // Executes every ~kUpdateIntervalFrames in GameSimulation to keep overhead modest; default to 1 elsewhere.
            if (phase == SystemUpdatePhase.GameSimulation)
                return kUpdateIntervalFrames;

            return 1;
        }
    }
}
