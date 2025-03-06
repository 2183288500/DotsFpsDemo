using UnityEngine;
using System.Collections.Generic;

// 敌人类型配置
public enum EnemyType
{
    Person,
    Goblin,
    Orc,
    Dragon,
    
}

[CreateAssetMenu(fileName = "EnemyHealthConfig", menuName = "Configs/EnemyHealthConfig")]
public class EnemyHealthConfig : ScriptableObject
{
    [System.Serializable]
    public struct HealthSetting
    {
        public EnemyType Type;
        public int BasicsHP;
        public int MaxGrade;
        public int MinGrade;
        public int BasicsATK;
        public float Speed;
        public GameObject prefab;
        public int targetCount;

        // 将计算逻辑封装在结构体内
        public int GenerateGrade() => 
            Mathf.Clamp(Random.Range(MinGrade, MaxGrade + 1), MinGrade, MaxGrade);

        public int CalculateMaxHP(int grade) => 
            BasicsHP * grade + Random.Range(0, (MaxGrade - MinGrade) / 2);

        public int CalculateATK(int grade) => 
            (BasicsATK * grade) / 2;
    }

    [SerializeField] private HealthSetting[] _settings;

    // 用字典缓存配置（避免每次遍历）
    private Dictionary<EnemyType, HealthSetting> _settingsDict;
    
    public Dictionary<EnemyType, HealthSetting> SettingsDict => _settingsDict;
    
    private void OnEnable()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        _settingsDict = new Dictionary<EnemyType, HealthSetting>();
        foreach (var setting in _settings)
        {
            _settingsDict.TryAdd(setting.Type, setting);
        }
    }

    // 统一获取配置方法
    private bool TryGetSetting(EnemyType type, out HealthSetting setting)
    {
        if (_settingsDict == null) InitializeDictionary();
        return _settingsDict.TryGetValue(type, out setting);
    }

    
    public int GetTargetCount(EnemyType type)
    { 
        if (TryGetSetting(type, out var setting))
        {
            return setting.targetCount;
        }

        return 100;

    }
    public GameObject GetPrefab(EnemyType type)
    {
        if (TryGetSetting(type, out var setting))
        {
            return setting.prefab;
        }

        return null;
    }

    public int GetGrade(EnemyType type)
    {
        if (TryGetSetting(type, out var setting))
        {
            return setting.GenerateGrade();
        }

        return 1;
    }

    public int GetMaxHp(EnemyType type, int grade)
    {
        if (TryGetSetting(type, out var setting))
        {
            return setting.CalculateMaxHP(grade);
        }

        return 100;
    }

    public int GetAtk(EnemyType type, int grade)
    {
        if (TryGetSetting(type, out var setting))
        {
            return setting.CalculateATK(grade);
        }

        return 5;
    }

    public float GetSpeed(EnemyType type)
    {
        if (TryGetSetting(type, out var setting))
        {
            return setting.Speed;
        }

        return 30f;
    }

    
    
}