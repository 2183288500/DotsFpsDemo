using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace FpsDemo
{
    public class EnemyDataAuthoring : EnemyAuthoring
    {
        public EnemyType type;
        public class EnemyPersonBaker : Baker<EnemyDataAuthoring>
        {
            public override void Bake(EnemyDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                
                var grade = authoring.config.GetGrade(authoring.type);
                var hp = authoring.config.GetMaxHp(authoring.type,grade);
                var atk = authoring.config.GetAtk(authoring.type,grade);
                var speed = authoring.config.GetSpeed(authoring.type);
                var targetCount = authoring.config.GetTargetCount(authoring.type);
                
                AddComponent(entity,new EnemyPersonState()
                {
                    TargetCount = targetCount,
                    CurrentCount = 1,
                });
                AddComponent(entity, new Move()
                {
                    StartPosition = authoring.transform.position,
                    EndPosition = authoring.transform.position,
                    Speed = speed
                });
                AddComponent(entity,new Harm()
                {
                    Health = hp,
                    Atk = atk,
                });
                AddComponent(entity,new Upgrade()
                {
                    Grade = grade,
                });
            }
        }
    }
    
   
}