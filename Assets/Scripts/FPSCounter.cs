using UnityEngine;
using UnityEngine.UI;
using System.Text;

[RequireComponent(typeof(Text))]
public class FPSCounter : MonoBehaviour
{
    [Header("显示设置")]
    [SerializeField] private float updateInterval = 0.5f;  // 更新频率（秒）
    [SerializeField] private Color goodColor = Color.green;  // 60+ FPS
    [SerializeField] private Color warnColor = Color.yellow; // 30-60 FPS
    [SerializeField] private Color badColor = Color.red;     // <30 FPS

    [Header("高级设置")]
    [SerializeField] private bool showMilliseconds = true;
    [SerializeField] private bool persistBetweenScenes = true;

    private Text _fpsText;
    private float _accumulatedTime;
    private int _frameCount;
    private StringBuilder _stringBuilder = new StringBuilder(16);

    private void Awake()
    {
        if (persistBetweenScenes)
        {
            DontDestroyOnLoad(gameObject);
        }

        _fpsText = GetComponent<Text>();
        _fpsText.raycastTarget = false;  // 禁用射线检测提升性能
    }

    private void Update()
    {
        // 累计时间和帧数
        _accumulatedTime += Time.unscaledDeltaTime;
        _frameCount++;

        if (_accumulatedTime >= updateInterval)
        {
            // 计算帧率
            float fps = _frameCount / _accumulatedTime;
            float ms = 1000f / fps;

            // 构建显示文本
            _stringBuilder.Clear();
            _stringBuilder.AppendFormat(fps >= 100 ? "{0:F0}" : "{0:F1}", fps);
            _stringBuilder.Append(" FPS");

            if (showMilliseconds)
            {
                _stringBuilder.AppendFormat("\n{0:F1} ms", ms);
            }

            // 更新颜色
            _fpsText.color = fps switch
            {
                >= 60 => goodColor,
                >= 30 => warnColor,
                _ => badColor
            };

            // 更新UI
            _fpsText.text = _stringBuilder.ToString();

            // 重置计数器
            _accumulatedTime = 0f;
            _frameCount = 0;
        }
    }

    // 添加编辑器预览功能
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying && _fpsText != null)
        {
            _fpsText.text = "FPS Counter (Preview)";
        }
    }
    #endif
    
}
