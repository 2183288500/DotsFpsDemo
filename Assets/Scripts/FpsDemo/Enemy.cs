using Unity.Entities;
using Unity.Mathematics;

namespace FpsDemo
{
    // public struct EnemySpawnConfig : IComponentData
    // {
    //     public int SpawnCount;
    //     public Entity EnemyPrefab;
    //     public Entity PersonEntity;
    // }
    public struct EnemyTag : IComponentData
    {

    }

    public struct Enemy : IComponentData
    {
        public Entity PersonEntity;
        public Entity GoblinEntity;
        public Entity OrcEntity;
        public Entity DragonEntity;
    }

    public struct EnemyPersonState : IComponentData
    {
        public int TargetCount;
        public int CurrentCount;
    }

    public struct Move : IComponentData
    {
        public float3 StartPosition;
        public float3 EndPosition;
        public float Speed;
    }

    public struct Harm : IComponentData
    {
        public int Health;
        public int Atk;
    }

    public struct Upgrade : IComponentData
    {
        public int Grade;
    }
    
    
}