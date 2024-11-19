using UnityEngine;

public class BossStagePlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 10f;
    public float rotationSpeed = 5f; // ȸ�� ���� �ӵ�

    [Header("Movement Constraints")]
    public Vector3 areaMin = new Vector3(-10f, 0.8f, -10f);
    public Vector3 areaMax = new Vector3(10f, 0.8f,10f);
    public float minDistanceFromBoss = 2f; // �������κ��� �ּ� �Ÿ� ����

    [Header("Boss Settings")]
    public Transform bossTransform;

    private Rigidbody rb;
    private const float initialPitch = 20f;
    private Quaternion lastRotation;

    [Header("Projectile Settings")]
    public float projectileSpeed = 10.0f;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    private Camera mainCamera;

    [Header("Audio Settings")]
    [SerializeField] public AudioClip laserFireSound;
    [SerializeField][Range(0f, 1f)] public float laserVolume = 0.7f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // �ʱ� ȸ�� ����
        lastRotation = Quaternion.Euler(initialPitch, 0f, 0f);
        rb.rotation = lastRotation;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
    }

    private void Update()
    {
        // Fire Laser
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            FireLaser();
        }
    }

    private void MovePlayer()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); // �¿� �̵� �Է�
        float moveVertical = Input.GetAxis("Vertical");     // ���� �̵� �Է�

        // ������ ��ġ (������ ������ ���� ���)
        Vector3 bossPosition = bossTransform != null ? bossTransform.position : Vector3.zero;

        // ���� ���� ���
        Vector3 directionToBoss = bossPosition - rb.position;
        directionToBoss.y = 0f; // ���� ���� �����Ͽ� ���� ���⸸ ����

        Vector3 forwardDirection;
        Vector3 rightDirection;

        if (directionToBoss.sqrMagnitude > 0.0001f)
        {
            forwardDirection = directionToBoss.normalized;
            rightDirection = Vector3.Cross(Vector3.up, forwardDirection).normalized; // ������ ���� ���
        }
        else
        {
            // �÷��̾ ������ ���� ��ġ�� ���� ���, ������ ���� ����
            forwardDirection = Vector3.forward;
            rightDirection = Vector3.right;
        }

        // �̵� ���� ���
        Vector3 movement = (forwardDirection * moveVertical + rightDirection * moveHorizontal) * speed * Time.fixedDeltaTime;

        // ���ο� ��ġ ���
        Vector3 newPosition = rb.position + movement;

        // �̵� ���� ����
        newPosition.x = Mathf.Clamp(newPosition.x, areaMin.x, areaMax.x);
        newPosition.z = Mathf.Clamp(newPosition.z, areaMin.z, areaMax.z);
        newPosition.y = rb.position.y; // Y��ǥ ����

        // �����κ��� �ּ� �Ÿ� ����
        Vector3 offsetFromBoss = newPosition - bossPosition;
        float distanceFromBoss = offsetFromBoss.magnitude;

        if (distanceFromBoss < minDistanceFromBoss)
        {
            // �÷��̾ �ּ� �Ÿ� ���� ������ ���
            // �������� ���� �������� Ȯ��
            Vector3 attemptedMovement = newPosition - rb.position;
            Vector3 toBoss = bossPosition - rb.position;
            toBoss.y = 0f;

            float dot = Vector3.Dot(attemptedMovement.normalized, toBoss.normalized);

            if (dot > 0f && moveVertical > 0f)
            {
                // �������� ���� �������� ���ϰ� �ְ�, ���� �Է��� ���
                // �������� ��� (�̵����� ����)
                movement = Vector3.zero;
                newPosition = rb.position;
            }
            else
            {
                // �������� �������� �־����� �����̰ų�, ���� �Է��� �ƴ�
                // �ּ� �Ÿ��� �����ϵ��� ��ġ ����
                offsetFromBoss = offsetFromBoss.normalized * minDistanceFromBoss;
                newPosition = bossPosition + offsetFromBoss;
            }
        }

        rb.MovePosition(newPosition);
    }

    private void RotatePlayer()
    {
        // ������ ��ġ (������ ������ ���� ���)
        Vector3 bossPosition = bossTransform != null ? bossTransform.position : Vector3.zero;

        // �÷��̾ �׻� ������ �ٶ󺸵��� ȸ�� ���
        Vector3 directionToBoss = bossPosition - rb.position;
        directionToBoss.y = 0f; // ���� ���� �����Ͽ� ���� ȸ���� ����

        if (directionToBoss.sqrMagnitude > 0.0001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToBoss);

            // �ʱ� X�� ����(20��)�� ����
            Quaternion pitchRotation = Quaternion.Euler(initialPitch, 0f, 0f);

            Quaternion targetRotation = lookRotation * pitchRotation;

            // ȸ���� �ε巴�� �����Ͽ� �ް��� ȸ�� ��ȭ ����
            Quaternion newRotation = Quaternion.Slerp(lastRotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

            rb.MoveRotation(newRotation);

            // ������ ȸ�� ������Ʈ
            lastRotation = newRotation;
        }
        else
        {
            // �÷��̾ ������ ���� ��ġ�� ���� ���, ȸ�� ��ȭ ����
            // ���� ȸ���� ����
            rb.MoveRotation(lastRotation);
        }
    }

    // Player Attack
    void FireLaser()
    {
        if (laserFireSound != null)
        {
            AudioSource.PlayClipAtPoint(laserFireSound, transform.position, laserVolume);
        }
        // Center shot always fires
        SpawnProjectile(projectileSpawnPoint.position);
    }

    void SpawnProjectile(Vector3 spawnPosition)
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 direction;

        // Convert to a ray from the camera
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        // Perform the raycast
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            // Get the hit point on the mesh
            Vector3 targetPoint = hit.point;

            // Calculate the direction from the spawn point to the target point
            direction = (targetPoint - spawnPosition).normalized;
        }
        else
        {
            mousePosition.z = 16f; ; // Distance from camera to plane
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
            direction = (worldPoint - spawnPosition).normalized;
            Debug.Log("Raycast did not hit any target.");
        }
            
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        if (projectileRb != null)
        {
            projectileRb.velocity = direction * projectileSpeed;
            Debug.Log(direction);
        }
    }
}
