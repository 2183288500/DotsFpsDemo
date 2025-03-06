using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.SceneManagement;

namespace FpsDemo
{
    [BurstCompile]
    class EnemyBaker: Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            foreach (var settingsDict in authoring.config.SettingsDict)
            {
                var entity = GetEntity(TransformUsageFlags.None);

            
                AddComponent(entity,new Enemy
                {
                    PersonEntity = GetEntity(authoring.config.GetPrefab(settingsDict.Key),TransformUsageFlags.Dynamic),
                });  
                AddComponent<EnemyTag>(entity);
            }
            
            
            // var grade = authoring.config.GetGrade(authoring.type);
            // var hp = authoring.config.GetMaxHP(authoring.type,grade);
            // var atk = authoring.config.GetATK(authoring.type,grade);
            // var speed = authoring.config.GetSpeed(authoring.type);
            // AddComponent(entity,new EnemyPersonState()
            // {
            //     TargetCount = authoring.targetCount,
            //     CurrentCount = 1,
            // });
            // AddComponent(entity, new Move()
            // {
            //     StartPosition = authoring.transform.position,
            //     EndPosition = authoring.transform.position,
            //     Speed = speed
            // });
            // AddComponent(entity,new Harm()
            // {
            //     Health = hp,
            //     Atk = atk,
            // });
            // AddComponent(entity,new Upgrade()
            // {
            //     Grade = grade,
            // });
            
            // float angle = (float)1 / 10000 * math.PI * 2;
            // float3 spawnPos = new float3(0,0,0) + new float3(math.cos(angle), 0, math.sin(angle)) * 50f;
            // SetComponent(entity,LocalTransform.FromPosition(spawnPos));
            
            
        }
    }
}