using UnityEngine;

public class BossStagePlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 10f;
    public float rotationSpeed = 50f;  // ȸ�� �ӵ��� ������ ����

    [Header("Altitude Settings")]
    public float altitudeLimit = 5f;

    [Header("Movement Constraints")]
    public Vector3 horizontalAreaMin = new Vector3(-25f, 0.5f, -5f);
    public Vector3 horizontalAreaMax = new Vector3(25f, 0.5f, 5f);
    public Vector3 verticalAreaMin = new Vector3(-5f, 0.5f, -25f);
    public Vector3 verticalAreaMax = new Vector3(5f, 0.5f, 25f);

    private Rigidbody rb;
    private float currentYaw = 0f; // Y�� ȸ�� ����
    private const float initialPitch = 20f; // �ʱ� X�� ȸ�� ����

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // �÷��̾��� �ʱ� Y�� ȸ�� ���� ����
        currentYaw = transform.eulerAngles.y;
    }

    private void Update()
    {
        // �Է� ó��
        HandleRotationInput();
    }

    private void FixedUpdate()
    {
        // ���� ��� �̵� �� ȸ�� ó��
        MovePlayer();
        RotatePlayer();
    }

    private void HandleRotationInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); // �¿� ȸ�� �Է�

        if (moveHorizontal != 0)
        {
            float rotationAmount = moveHorizontal * rotationSpeed * Time.deltaTime;
            currentYaw += rotationAmount;
        }
    }

    private void MovePlayer()
    {
        float moveVertical = Input.GetAxis("Vertical");     // ���� �̵� �Է�

        // ���� �̵� ���� ��� (Y�� �̵� ����)
        Vector3 forwardDirection = transform.forward;
        forwardDirection.y = 0f;
        forwardDirection.Normalize();

        Vector3 movement = forwardDirection * moveVertical * speed * Time.fixedDeltaTime;

        Vector3 newPosition = rb.position + movement;

        // Y��ǥ�� �����Ͽ� ���� ��ȭ ����
        newPosition.y = rb.position.y;

        // ������ ���� �������� �̵� ���
        if (IsInsideAllowedArea(newPosition))
        {
            rb.MovePosition(newPosition);
        }
    }

    private void RotatePlayer()
    {
        // �÷��̾��� ȸ���� X�� -20�� ����� ���� Y�� ȸ�� ������ ����
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
