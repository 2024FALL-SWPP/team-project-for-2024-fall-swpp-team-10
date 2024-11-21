using UnityEngine;
using System.Collections;

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
    [SerializeField] public AudioClip enemyCollisionSound;
    [SerializeField][Range(0f, 1f)] public float collisionVolume = 0.5f;

    // On Collision variables
    private bool isInvincible = false;
    private int blinkCount = 3;
    private Renderer[] childRenderers; //Renderer of characters
    private Color[,] originColors; // Origin color of characters


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        childRenderers = GetComponentsInChildren<Renderer>();

        int maxSharedMaterialsLength = 0;
        for (int i = 0; i < childRenderers.Length; i++)
        {
            maxSharedMaterialsLength = Mathf.Max(maxSharedMaterialsLength, childRenderers[i].sharedMaterials.Length);
        }
        originColors = new Color[childRenderers.Length, maxSharedMaterialsLength];

        for (int i = 0; i < childRenderers.Length; i++)
        {
            for (int j = 0; j < childRenderers[i].sharedMaterials.Length; j++)
            {
                if (childRenderers[i].sharedMaterials[j].HasProperty("_Color"))
                    originColors[i, j] = childRenderers[i].sharedMaterials[j].color;
            }
        }

        if (GameManager.inst.originColorSave == null)
            GameManager.inst.originColorSave = originColors;
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
        }
            
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition + Vector3.forward, Quaternion.identity);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        if (projectileRb != null)
        {
            projectileRb.velocity = direction * projectileSpeed;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            Destroy(other.gameObject);
            if (enemyCollisionSound != null)
            {
                AudioSource.PlayClipAtPoint(enemyCollisionSound, gameObject.transform.position, collisionVolume);
            }
            if (isInvincible) return;
            GameManager.inst.RemoveLife();
            if (GameManager.inst.GetLife() > 0)
                StartCoroutine(Blink());
        }
    }

    // Blink on damage
    IEnumerator Blink()
    {
        isInvincible = true;
        for (int i = 0; i < blinkCount; i++)
        {
            ChangeColor(Color.red);
            yield return new WaitForSeconds(0.2f);

            ChangeColorOriginal();
            yield return new WaitForSeconds(0.2f);
        }
        isInvincible = false;
    }

    // 캐릭터 색 전체 변환
    private void ChangeColor(Color _color)
    {
        foreach (Renderer renderer in childRenderers)
        {
            foreach (Material material in renderer.sharedMaterials)
                material.color = _color;
        }
    }

    // 캐릭터 색 원래 색으로
    public void ChangeColorOriginal()
    {
        for (int i = 0; i < childRenderers.Length; i++)
        {
            for (int j = 0; j < childRenderers[i].sharedMaterials.Length; j++)
            {
                childRenderers[i].sharedMaterials[j].color = GameManager.inst.originColorSave[i, j];
            }
        }
    }
}
