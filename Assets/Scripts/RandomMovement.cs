using Unity.Entities;
using Unity.Mathematics;

public struct RandomMovement : IComponentData
{
    public float Speed;          // 移动速度
    public float3 Direction;     // 当前移动方向
    public float ChangeInterval; // 方向变化间隔
    public float Timer;          // 方向计时器
    public uint Seed;            // 随机种子
}