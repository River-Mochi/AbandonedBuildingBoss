// File: AbandonedBuildingBossSystem.cs

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
        private EntityQuery _abandonedBuildingQuery;

        protected override void OnCreate()
        {
            base.OnCreate();

            _abandonedBuildingQuery = GetEntityQuery(new EntityQueryDesc
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

            RequireForUpdate(_abandonedBuildingQuery);
            Mod.Log.Info("AbandonedBuildingBossSystem created (non-Burst path).");
        }

        protected override void OnUpdate()
        {
            // Gate on the settings toggle (default ON)
            if (Mod.m_Settings?.Enabled != true)
                return;

            // Pull the matching entities this frame; cost only when new Abandoned appear.
            NativeArray<Entity> abandonedBuildings = _abandonedBuildingQuery.ToEntityArray(Allocator.Temp);
            EntityManager em = EntityManager;

            foreach (Entity entity in abandonedBuildings)
            {
                // Clean sub-areas
                if (em.TryGetBuffer<SubArea>(entity, false, out DynamicBuffer<SubArea> subareas))
                {
                    foreach (SubArea subArea in subareas)
                        em.AddComponent<Deleted>(subArea.m_Area);
                }

                // Clean sub-nets
                if (em.TryGetBuffer<SubNet>(entity, false, out DynamicBuffer<SubNet> subnets))
                {
                    foreach (SubNet net in subnets)
                        em.AddComponent<Deleted>(net.m_SubNet);
                }

                // Clean sub-lanes
                if (em.TryGetBuffer<SubLane>(entity, false, out DynamicBuffer<SubLane> sublanes))
                {
                    foreach (SubLane lane in sublanes)
                        em.AddComponent<Deleted>(lane.m_SubLane);
                }

                // Finally delete the building entity itself
                em.AddComponent<Deleted>(entity);
            }

            abandonedBuildings.Dispose();
        }

        // Run ~every 16 frames in GameSimulation to keep low overhead.
        public override int GetUpdateInterval(SystemUpdatePhase phase) =>
            phase == SystemUpdatePhase.GameSimulation ? 16 : 1;
    }
}
