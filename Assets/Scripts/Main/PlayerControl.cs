using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnumManager;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private float gridSpacing = 1.0f; // Distance between grid positions
    private Vector2Int gridSize = new Vector2Int(3, 3); // 3x3 grid
    public float moveSpeed = 10f; // Speed of movement between grid positions

    private Vector2Int currentGridPosition; // Current grid position (logical, not world space)
    private Vector3 initialPosition; // Initial world position of the player
    public Vector3 centerPosition; // 캐릭터 중앙 위치 보정
    private bool isMoving = false; // Flag to prevent movement while transitioning

    [Header("Projectile Settings")]
    private float projectileSpeed = 30.0f;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    private bool hasTripleShot = false;  // Flag for triple shot power-up

    private Renderer[] childRenderers; //Renderer of characters
    private Color[,] originColors; // Origin color of characters
    private int blinkCount = 3; // 피격 시 깜빡이는 횟수
    private float invincibleLength; // 무적 지속 시간
    private bool isInvincible = false; // 무적 지속중인지 확인

    [Header("Lightstick Settings")]
    public GameObject leftLightstickPrefab;   // Assign in inspector
    public GameObject rightLightstickPrefab;  // Assign in inspector
    public float lightstickOffset = 1.0f;
    public float lightStickDuration = 5.0f;
    private float powerUpEndTime; // Track when the power-up should end
    private Coroutine tripleShotCoroutine;

    [Header("Particle System")]
    public ParticleSystem hitOnInvincibleParticle; // 무적 상태에서 장애물이나 적을 파괴했을 때
    public ParticleSystem damagedParticle; // 장애물이나 적에 부딪혔을 때

    [Header("Audio Settings")]
    [SerializeField] public AudioClip coinCollectSound;
    [SerializeField] public AudioClip laserFireSound;
    [SerializeField] public AudioClip enemyCollisionSound;
    [SerializeField][Range(0f, 1f)] public float coinVolume = 0.5f;
    [SerializeField][Range(0f, 1f)] public float laserVolume = 0.7f;

    void Awake()
    {
        // Set the initial position as the down of the grid (1,0)
        initialPosition = transform.position - Vector3.down;
        currentGridPosition = new Vector2Int(1, 0); // Start at the down logically
        SyncCenterPosition();

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

        // Initialize lightsticks in disabled state
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
            DisableLightsticks();
        }
        if (transform.position.y < 0)
        {
            gameObject.SetActive(false);
        }

        damagedParticle.transform.position = centerPosition;
        hitOnInvincibleParticle.transform.position = centerPosition;
    }

    void SyncCenterPosition()
    {
        centerPosition = transform.position + new Vector3(0f, 0.25f, 0.2f);
    }
    void FireLaser()
    {
        if (laserFireSound != null)
        {
            AudioSource.PlayClipAtPoint(laserFireSound, centerPosition, laserVolume);
        }
        // Center shot always fires
        SpawnProjectile(projectileSpawnPoint.position);

        // Side shots if lightsticks are active
        if (hasTripleShot)
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
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition + new Vector3(0, 0, 1f), Quaternion.identity);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        if (projectileRb != null)
        {
            projectileRb.velocity = Vector3.forward * projectileSpeed;
        }
    }

    private void UpdateLightstickPositions(Vector3 playerPosition)
    {
        if (!hasTripleShot) return;

        float spawnHeight = projectileSpawnPoint.position.y;

        // Update left lightstick
        if (leftLightstickPrefab != null)
        {
            bool shouldBeActive = currentGridPosition.x > 0;
            leftLightstickPrefab.SetActive(shouldBeActive);
            if (shouldBeActive)
            {
                Vector3 leftPosition = new Vector3(
                    playerPosition.x - lightstickOffset,
                    spawnHeight,  // Use spawn point height
                    playerPosition.z
                );
                leftLightstickPrefab.transform.position = leftPosition;
            }
        }

        // Update right lightstick
        if (rightLightstickPrefab != null)
        {
            bool shouldBeActive = currentGridPosition.x < gridSize.x - 1;
            rightLightstickPrefab.SetActive(shouldBeActive);
            if (shouldBeActive)
            {
                Vector3 rightPosition = new Vector3(
                    playerPosition.x + lightstickOffset,
                    spawnHeight,  // Use spawn point height
                    playerPosition.z
                );
                rightLightstickPrefab.transform.position = rightPosition;
            }
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

            // Update lightstick positions to maintain relative positioning
            UpdateLightstickPositions(transform.position);
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

    // 피격 시 깜빡임
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

    // 무적 아이템 효과
    IEnumerator Invincible()
    {
        invincibleLength = 10f;
        if (isInvincible)
            yield break;
        isInvincible = true;
        while (invincibleLength > 0)
        {
            invincibleLength -= 0.6f;
            ChangeColor(Color.red);
            yield return new WaitForSeconds(0.1f);
            ChangeColor(Color.yellow);
            yield return new WaitForSeconds(0.1f);
            ChangeColor(Color.green);
            yield return new WaitForSeconds(0.1f);
            ChangeColor(Color.blue);
            yield return new WaitForSeconds(0.1f);
            ChangeColor(Color.magenta);
            yield return new WaitForSeconds(0.1f);
        }
        ChangeColorOriginal();

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

    void DisableLightsticks()
    {
        if (leftLightstickPrefab != null)
            leftLightstickPrefab.SetActive(false);
        if (rightLightstickPrefab != null)
            rightLightstickPrefab.SetActive(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (isInvincible)
            {
                Instantiate(hitOnInvincibleParticle, centerPosition, new Quaternion(0, 0, 0, 0));
                Destroy(other.gameObject);
                GameManager.inst.AddScore(1000); // 무적 상태에서 적 부딪하면 1000점 추가
                return;
            }

            if (enemyCollisionSound != null)
            {
                AudioSource.PlayClipAtPoint(enemyCollisionSound, centerPosition, coinVolume);
            }

            Instantiate(damagedParticle, centerPosition, new Quaternion(0, 0, 0, 0));
            GameManager.inst.RemoveLife();
            GameManager.inst.AddScore(-1000);
            StartCoroutine(Blink());
        }

        if (other.gameObject.CompareTag("Obstacle"))
        {
            if (isInvincible)
            {
                Instantiate(hitOnInvincibleParticle, centerPosition, new Quaternion(0, 0, 0, 0));
                Destroy(other.gameObject);
                GameManager.inst.AddScore(500); // 무적 상태에서 장애물 부딪하면 500점 추가
                return;
            }

            if (enemyCollisionSound != null)
            {
                AudioSource.PlayClipAtPoint(enemyCollisionSound, centerPosition, coinVolume);
            }

            Instantiate(damagedParticle, centerPosition, new Quaternion(0, 0, 0, 0));
            GameManager.inst.RemoveLife();
            GameManager.inst.AddScore(-500);
            StartCoroutine(Blink());
        }

        if (other.gameObject.CompareTag("Heart"))
        {
            GameManager.inst.AddLife(GameManager.inst.maxLife);
        }

        if (other.gameObject.CompareTag("Invincible"))
        {
            StartCoroutine(Invincible());
        }

        if (other.gameObject.CompareTag("Coin"))
        {
            GameManager.inst.AddScore(200);
            if (coinCollectSound != null)
            {
                AudioSource.PlayClipAtPoint(coinCollectSound, centerPosition, coinVolume);
            }
        }

        if (other.gameObject.CompareTag("Lightstick"))
        {
            if (tripleShotCoroutine != null)
            {
                StopCoroutine(tripleShotCoroutine);
                tripleShotCoroutine = null;
            }

            hasTripleShot = true;
            powerUpEndTime = Time.time + lightStickDuration;

            UpdateLightstickPositions(transform.position);
            tripleShotCoroutine = StartCoroutine(TripleShotPowerUpTimer());

            Destroy(other.gameObject);
        }
    }

    IEnumerator TripleShotPowerUpTimer()
    {

        while (Time.time < powerUpEndTime)
        {
            float remainingTime = powerUpEndTime - Time.time;
            yield return new WaitForSeconds(0.1f);
        }
        hasTripleShot = false;
        DisableLightsticks();
        tripleShotCoroutine = null;
    }
}
