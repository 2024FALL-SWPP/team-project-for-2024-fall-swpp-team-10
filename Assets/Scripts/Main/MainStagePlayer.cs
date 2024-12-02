using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnumManager;
using Unity.VisualScripting;
using UnityEngine;

public class MainStagePlayer : PlayerBase
{
    private float gridSpacing = 1.0f; // Distance between grid positions
    private Vector2Int gridSize = new Vector2Int(3, 3); // 3x3 grid
    public float moveSpeed = 10f; // Speed of movement between grid positions

    private Vector2Int currentGridPosition; // Current grid position (logical, not world space)
    private Vector3 initialPosition; // Initial world position of the player
    public Vector3 centerPosition; // 캐릭터 중앙 위치 보정
    private bool isMoving = false; // Flag to prevent movement while transitioning

    [Header("Invincible Settings")]
    private bool isInvincible = false; // 무적 지속중인지 확인
    public float invincibleLength; // 무적 지속 시간

    [Header("Projectile Settings")]
    private float projectileSpeed = 30.0f;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;

    [Header("Lightstick Settings")]
    public GameObject leftLightstickPrefab;   // Assign in inspector
    public GameObject rightLightstickPrefab;  // Assign in inspector
    public float lightstickOffset = 1.0f;
    public float lightstickEndTime;
    private bool isTripleShot;

    protected override void Awake()
    {
        base.Awake();

        initialPosition = transform.position - Vector3.down;
        currentGridPosition = new Vector2Int(1, 0);
        SyncCenterPosition();

        // 라이트스틱 초기화
        if (leftLightstickPrefab != null)
            leftLightstickPrefab.SetActive(false);
        if (rightLightstickPrefab != null)
            rightLightstickPrefab.SetActive(false);
    }

    void Update()
    {
        // Handle movement. Only allow new movement input if we're not currently moving
        if (!isMoving)
        {
            // Handle left movement
            if (Input.GetKeyDown(KeyCode.A) && currentGridPosition.x > 0)
            {
                currentGridPosition.x--;
                StartCoroutine(SmoothMove());
            }
            // Handle right movement
            else if (Input.GetKeyDown(KeyCode.D) && currentGridPosition.x < gridSize.x - 1)
            {
                currentGridPosition.x++;
                StartCoroutine(SmoothMove());
            }
            // Handle up movement
            else if (Input.GetKeyDown(KeyCode.W) && currentGridPosition.y < gridSize.y - 1)
            {
                currentGridPosition.y++;
                StartCoroutine(SmoothMove());
            }
            // Handle down movement
            else if (Input.GetKeyDown(KeyCode.S) && currentGridPosition.y > 0)
            {
                currentGridPosition.y--;
                StartCoroutine(SmoothMove());
            }
        }
        // Fire Laser
        if (Input.GetMouseButtonDown(0))
        {
            FireLaser();
        }

        // Handle death
        if (GameManager.inst.GetLife() <= 0)
        {
            isMoving = true;
            transform.Translate(Vector3.down * moveSpeed * 0.005f, Space.World);
            ControlLightsticks();
        }
        if (transform.position.y < 0)
        {
            gameObject.SetActive(false);
        }
    }

    void SyncCenterPosition()
    {
        centerPosition = transform.position + new Vector3(0f, 0.25f, 0.2f);
    }
    protected override void FireLaser()
    {
        if (laserFireSound != null)
        {
            AudioSource.PlayClipAtPoint(laserFireSound, centerPosition, laserVolume);
        }
        // Center shot always fires
        SpawnProjectile(projectileSpawnPoint.position);

        // Side shots if lightsticks are active
        if (isTripleShot)
        {
            if (leftLightstickPrefab != null && leftLightstickPrefab.activeSelf)
            {
                SpawnProjectile(leftLightstickPrefab.transform.position);
            }
            if (rightLightstickPrefab != null && rightLightstickPrefab.activeSelf)
            {
                SpawnProjectile(rightLightstickPrefab.transform.position);
            }
        }
    }
    void SpawnProjectile(Vector3 spawnPosition)
    {
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition + new Vector3(0, 0, 2f), Quaternion.identity);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        if (projectileRb != null)
        {
            projectileRb.velocity = Vector3.forward * projectileSpeed;
        }
    }

    IEnumerator SmoothMove()
    {
        isMoving = true;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = CalculateTargetPosition();
        float journeyLength = Vector3.Distance(startPosition, endPosition);
        float startTime = Time.time;

        // Move until we reach the target position
        while (transform.position != endPosition)
        {
            // Calculate journey progress
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            // Update player position
            Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);
            transform.position = newPosition;

            // If we're very close to the target, snap to it
            if (Vector3.Distance(transform.position, endPosition) < 0.01f)
            {
                transform.position = endPosition;
            }

            // 캐릭터 중앙 위치 수정
            SyncCenterPosition();
            yield return null;
        }

        isMoving = false;
    }
    Vector3 CalculateTargetPosition()
    {
        // Calculate target position relative to the initial position
        return initialPosition + new Vector3(
            (currentGridPosition.x - 1) * gridSpacing,
            (currentGridPosition.y - 1) * gridSpacing,
            0
        );
    }

    public void ControlLightsticks()
    {
        if (leftLightstickPrefab != null)
            leftLightstickPrefab.SetActive(isTripleShot);
        if (rightLightstickPrefab != null)
            rightLightstickPrefab.SetActive(isTripleShot);
    }

    public void SetIsInvincible(bool _isInvincible)
    {
        isInvincible = _isInvincible;
    }
    public bool GetIsInvincible()
    {
        return isInvincible;
    }

    public bool GetIsTripleShot()
    {
        return isTripleShot;
    }

    public void SetTripleShot(bool _isTripleShot)
    {
        isTripleShot = _isTripleShot;
    }

    public Vector2Int GetCurrentGridPosition()
    {
        return currentGridPosition;
    }

    public Vector2Int GetGridSize()
    {
        return gridSize;
    }
}
