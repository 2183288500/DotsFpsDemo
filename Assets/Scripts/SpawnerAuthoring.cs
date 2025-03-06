using Unity.Entities;
using UnityEngine;

// 这是一个作者组件类，用于在编辑器中为 GameObject 设置属性
// 这个组件在 Unity 编辑器中附加到 GameObject 上
class SpawnerAuthoring : MonoBehaviour
{
    public GameObject Prefab;        // 用于指定生成的实体的预制体
    public float SpawnRate;          // 设置生成实体的时间间隔
}

// 这个类负责将 SpawnerAuthoring 转换为实体组件（ECS的实体）
class SpawnerBaker : Baker<SpawnerAuthoring>
{
    // Bake 方法会在编辑器中调用，用于将 SpawnerAuthoring 组件转换为一个 Spawner 实体组件
    public override void Bake(SpawnerAuthoring authoring)
    {
        // 创建一个新的实体，该实体与当前的 GameObject 绑定
        var entity = GetEntity(TransformUsageFlags.None);

        // 为该实体添加 Spawner 组件，初始化其字段值
        AddComponent(entity, new Spawner
        {
            // 将预制体转化为一个实体，并绑定 TransformUsageFlags.Dynamic（动态 Transform 变换）
            Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
            
            // 获取当前 GameObject 的位置作为生成位置
            SpawnPosition = authoring.transform.position,
            
            // 初始化下一次生成时间，默认值为 0
            NextSpawnTime = 0.0f,
            
            // 从 authoring 组件中获取生成间隔 SpawnRate
            SpawnRate = authoring.SpawnRate
        });
        AddComponent(entity,new RandomMovement {
            Speed = 3.5f,          // 移动速度
            ChangeInterval = 2f,   // 每2秒改变方向
            Seed = 12345           // 初始种子（会自动更新）
        });
    }
}