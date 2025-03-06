using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FpsDemo
{

    [BurstCompile]
    public partial struct EnemyPersonSystem : ISystem
    {
        private EntityQuery _entityQuery;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EnemyPersonState>();
            state.RequireForUpdate<Enemy>();

            _entityQuery = state.GetEntityQuery(ComponentType.ReadWrite<EnemyTag>());
        }
        public void OnDestroy(ref SystemState state)
        {}
        
        public void OnUpdate(ref SystemState state)
        {
            var entity = SystemAPI.GetSingleton<Enemy>();

            var spawner = SystemAPI.GetSingleton<EnemyPersonState>();

            spawner.CurrentCount = _entityQuery.CalculateEntityCount();
            
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            
            int needToSpawn = math.min(spawner.TargetCount - spawner.CurrentCount, 500);
            
            var random = Random.CreateFromIndex((uint)state.WorldUnmanaged.Time.ElapsedTime * 1000);
            
            if (needToSpawn<=0)
            {
                return;
            }
            
            for (int i = 0; i < needToSpawn; i++)
            {
                Entity enemy = ecb.Instantiate(entity.PersonEntity);
                
                float angle = random.NextFloat(0, math.PI * 2);
                float3 pos = new float3(
                    math.cos(angle) * 50,
                    0,
                    math.sin(angle) * 50
                );
                ecb.SetComponent(enemy, LocalTransform.FromPosition(pos));
                
                ecb.AddComponent<EnemyTag>(enemy);
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    [BurstCompile]
    public partial struct CreateEnemySpawnerJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;

        private void Execute([ChunkIndexInQuery] int chunkIndex, ref Spawner spawner)
        {
            
        }
    }
}
