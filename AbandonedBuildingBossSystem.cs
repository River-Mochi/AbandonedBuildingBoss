// AbandonedBuildingBossSystem.cs
namespace AbandonedBuildingBoss
{
    using Colossal.Entities;
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
            // Skip when disabled
            if (!Mod.m_Settings.Enabled)
                return;

            // Fast no-op: avoids NativeArray allocation when there are no matches this tick.
            if (m_AbandonedBuildingQuery.IsEmptyIgnoreFilter)
                return;

            // Collect matching entities; each is processed once, then excluded by 'Deleted'.
            NativeArray<Entity> abandonedBuildings = m_AbandonedBuildingQuery.ToEntityArray(Allocator.Temp);
            EntityManager em = EntityManager;

            foreach (Entity entity in abandonedBuildings)
            {
                // Remove linked areas.
                if (em.TryGetBuffer<SubArea>(entity, false, out DynamicBuffer<SubArea> subareas))
                {
                    foreach (SubArea subArea in subareas)
                        em.AddComponent<Deleted>(subArea.m_Area);
                }

                // Remove linked nets.
                if (em.TryGetBuffer<SubNet>(entity, false, out DynamicBuffer<SubNet> subnets))
                {
                    foreach (SubNet net in subnets)
                        em.AddComponent<Deleted>(net.m_SubNet);
                }

                // Remove linked lanes.
                if (em.TryGetBuffer<SubLane>(entity, false, out DynamicBuffer<SubLane> sublanes))
                {
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
            // Executes every ~32 frames in-game to keep low overhead; default to 1 elsewhere.
            if (phase == SystemUpdatePhase.GameSimulation)
                return 32;  // change to 16 for more aggressive cleanup
            return 1;
        }
    }
}
