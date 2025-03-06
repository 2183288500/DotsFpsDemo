// using Unity.Entities;
// using Unity.Transforms;
// using Unity.Burst;
//
// // 通过 Burst 编译器加速该系统的执行
// [BurstCompile]
// public partial struct SpawnerSystem : ISystem
// {
//     // 系统创建时调用
//     public void OnCreate(ref SystemState state) { }
//
//     // 系统销毁时调用
//     public void OnDestroy(ref SystemState state) { }
//
//     // 在每帧更新时调用
//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {
//         // 查询所有带有 Spawner 组件的实体。
//         // 使用 RefRW 类型，因为系统需要读写这个组件。如果只是只读访问，可以使用 RefRO。
//         foreach (RefRW<Spawner> spawner in SystemAPI.Query<RefRW<Spawner>>())
//         {
//             ProcessSpawner(ref state, spawner);
//         }
//     }
//
//     // 处理每个 Spawner 组件的生成逻辑
//     private void ProcessSpawner(ref SystemState state, RefRW<Spawner> spawner)
//     {
//         // 如果下一个生成时间已经到达或超过当前系统时间
//         if (spawner.ValueRO.NextSpawnTime < SystemAPI.Time.ElapsedTime)
//         {
//             // 生成一个新的实体，并设置它的 Transform。
//             // 使用 EntityManager.Instantiate() 创建一个新的实体副本。
//             Entity newEntity = state.EntityManager.Instantiate(spawner.ValueRO.Prefab);
//
//             // 设置新生成实体的位置为 spawner 指定的位置
//             // LocalTransform.FromPosition 用给定位置初始化 Transform 组件
//             state.EntityManager.SetComponentData(newEntity, LocalTransform.FromPosition(spawner.ValueRO.SpawnPosition));
//
//             // 重置下一次生成时间，间隔为 SpawnRate
//             spawner.ValueRW.NextSpawnTime = (float)SystemAPI.Time.ElapsedTime + spawner.ValueRO.SpawnRate;
//         }
//     }
// }

using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;

// 使用 Burst 编译器加速该系统的执行
[BurstCompile]
public partial struct OptimizedSpawnerSystem : ISystem
{
    // 系统创建时调用（目前没有实现具体功能）
    public void OnCreate(ref SystemState state) { }

    // 系统销毁时调用（目前没有实现具体功能）
    public void OnDestroy(ref SystemState state) { }

    // 每帧更新时调用
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // 获取并创建一个并行写入的 EntityCommandBuffer（ECB）
        EntityCommandBuffer.ParallelWriter ecb = GetEntityCommandBuffer(ref state);

        // 创建并调度一个处理生成逻辑的并行作业
        new ProcessSpawnerJob
        {
            ElapsedTime = SystemAPI.Time.ElapsedTime,  // 传递当前已过去的时间
            Ecb = ecb  // 传递并行写入的 ECB
        }.ScheduleParallel();  // 调度作业并行执行
        
        // 添加随机数生成器
        var random = new Random((uint)SystemAPI.Time.ElapsedTime + 1);
        
        new ProcessSpawnerJob
        {
            Ecb = ecb,
            ElapsedTime = SystemAPI.Time.ElapsedTime,
            RandomSeed = random.NextUInt() // 传递随机种子
        }.ScheduleParallel();
        
    }

    // 获取并返回 EntityCommandBuffer 的并行写入器
    private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        return ecb.AsParallelWriter();  // 返回并行写入的 ECB
    }
    
}

// 修改生成作业
[BurstCompile]
public partial struct ProcessSpawnerJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;
    public double ElapsedTime;
    public uint RandomSeed;

    private void Execute([ChunkIndexInQuery] int chunkIndex, ref Spawner spawner)
    {
        if (spawner.NextSpawnTime >= ElapsedTime) return;

        Entity newEntity = Ecb.Instantiate(chunkIndex, spawner.Prefab);
        
        // 添加随机移动组件
        Ecb.AddComponent(chunkIndex, newEntity, new RandomMovement 
        {
            Speed = 2f,
            Direction = float3.zero,
            ChangeInterval = 3f,
            Timer = 0f,
            Seed = RandomSeed ^ (uint)newEntity.Index // 生成唯一种子
        });

        // 设置初始位置
        Ecb.SetComponent(chunkIndex, newEntity, 
            LocalTransform.FromPosition(spawner.SpawnPosition));

        spawner.NextSpawnTime = (float)ElapsedTime + spawner.SpawnRate;
    }
}

// 新增移动处理系统
[BurstCompile]
public partial struct RandomMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        new RandomMovementJob
        {
            DeltaTime = deltaTime
        }.ScheduleParallel();
    }
}

// 移动处理作业
[BurstCompile]
public partial struct RandomMovementJob : IJobEntity
{
    public float DeltaTime;

    void Execute(ref LocalTransform transform, ref RandomMovement movement)
    {
        // 更新计时器
        movement.Timer += DeltaTime;

        // 到达间隔时间时生成新方向
        if(movement.Timer >= movement.ChangeInterval)
        {
            var random = new Random(movement.Seed + (uint)(movement.Timer * 1000));
            
            movement.Direction = random.NextFloat3Direction();
            movement.Timer = 0f;
            movement.Seed = random.NextUInt(); // 更新种子保证随机性
        }

        // 计算位移
        float3 displacement = movement.Direction * movement.Speed * DeltaTime;
        transform.Position += displacement;
    }
}