using UnityEngine;

public class BossStagePlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 10f;
    public float rotationSpeed = 50f;  // 회전 속도를 각도로 조정

    [Header("Altitude Settings")]
    public float altitudeLimit = 5f;

    [Header("Movement Constraints")]
    public Vector3 horizontalAreaMin = new Vector3(-25f, 0.5f, -5f);
    public Vector3 horizontalAreaMax = new Vector3(25f, 0.5f, 5f);
    public Vector3 verticalAreaMin = new Vector3(-5f, 0.5f, -25f);
    public Vector3 verticalAreaMax = new Vector3(5f, 0.5f, 25f);

    private Rigidbody rb;
    private float currentYaw = 0f; // Y축 회전 각도
    private const float initialPitch = 20f; // 초기 X축 회전 각도

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // 플레이어의 초기 Y축 회전 각도 저장
        currentYaw = transform.eulerAngles.y;
    }

    private void Update()
    {
        // 입력 처리
        HandleRotationInput();
    }

    private void FixedUpdate()
    {
        // 물리 기반 이동 및 회전 처리
        MovePlayer();
        RotatePlayer();
    }

    private void HandleRotationInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); // 좌우 회전 입력

        if (moveHorizontal != 0)
        {
            float rotationAmount = moveHorizontal * rotationSpeed * Time.deltaTime;
            currentYaw += rotationAmount;
        }
    }

    private void MovePlayer()
    {
        float moveVertical = Input.GetAxis("Vertical");     // 전후 이동 입력

        // 전후 이동 방향 계산 (Y축 이동 방지)
        Vector3 forwardDirection = transform.forward;
        forwardDirection.y = 0f;
        forwardDirection.Normalize();

        Vector3 movement = forwardDirection * moveVertical * speed * Time.fixedDeltaTime;

        Vector3 newPosition = rb.position + movement;

        // Y좌표를 고정하여 높이 변화 방지
        newPosition.y = rb.position.y;

        // 십자형 영역 내에서만 이동 허용
        if (IsInsideAllowedArea(newPosition))
        {
            rb.MovePosition(newPosition);
        }
    }

    private void RotatePlayer()
    {
        // 플레이어의 회전을 X축 -20도 기울기와 현재 Y축 회전 각도로 설정
        Quaternion targetRotation = Quaternion.Euler(initialPitch, currentYaw, 0f);
        rb.MoveRotation(targetRotation);
    }

    private bool IsInsideAllowedArea(Vector3 position)
    {
        return IsInsideHorizontalArea(position) || IsInsideVerticalArea(position);
    }

    private bool IsInsideHorizontalArea(Vector3 position)
    {
        return position.x >= horizontalAreaMin.x && position.x <= horizontalAreaMax.x &&
               position.z >= horizontalAreaMin.z && position.z <= horizontalAreaMax.z;
    }

    private bool IsInsideVerticalArea(Vector3 position)
    {
        return position.x >= verticalAreaMin.x && position.x <= verticalAreaMax.x &&
               position.z >= verticalAreaMin.z && position.z <= verticalAreaMax.z;
    }
}
