using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float verticalSpeed = 3f;

    [Header("视角控制")]
    [SerializeField] private bool enableMouseLook = true;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float maxVerticalAngle = 10f;

    private float _rotationX = 0f;
    private Vector3 _currentVelocity;
    private bool _isCursorLocked = true;

    private void Start()
    {
        InitializeCursor();
    }

    private void Update()
    {
        HandleCursorToggle();
        HandleMovement();
        if (enableMouseLook) HandleMouseLook();
    }

    private void InitializeCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void HandleCursorToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isCursorLocked = !_isCursorLocked;
            Cursor.lockState = _isCursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !_isCursorLocked;
        }
    }

    private void HandleMovement()
    {
        // 获取输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float ascend = Input.GetKey(KeyCode.Space) ? 1 : 0;
        float descend = Input.GetKey(KeyCode.LeftControl) ? 1 : 0;

        // 计算移动方向
        Vector3 moveDirection = transform.right * horizontal + 
                              transform.forward * vertical + 
                              Vector3.up * (ascend - descend);

        // 冲刺加速
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? 
            moveSpeed * sprintMultiplier : moveSpeed;

        // 应用平滑移动
        Vector3 targetPosition = transform.position + 
            moveDirection.normalized * (currentSpeed * Time.deltaTime);

        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref _currentVelocity, 
            0.1f
        );
    }

    private void HandleMouseLook()
    {
        if (!_isCursorLocked) return;

        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 垂直视角限制
        _rotationX -= mouseY;
        _rotationX = Mathf.Clamp(_rotationX, -maxVerticalAngle, maxVerticalAngle);

        // 应用旋转
        transform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);
        transform.parent.Rotate(Vector3.up * mouseX);
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
