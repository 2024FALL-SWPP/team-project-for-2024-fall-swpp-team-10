using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnumManager;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private float gridSpacing = 1.0f; // Distance between grid positions
    private Vector2Int gridSize = new Vector2Int(3, 3); // 3x3 grid
    public float moveSpeed = 10f; // Speed of movement between grid positions

    private Vector2Int currentGridPosition; // Current grid position (logical, not world space)
    private Vector3 initialPosition; // Initial world position of the player
    private Vector3 centerPosition; // 캐릭터 중앙 위치 보정
    private bool isMoving = false; // Flag to prevent movement while transitioning

    public float projectileSpeed = 10.0f; // Speed of laser
    public GameObject projectilePrefab; // Laser prefab
    public Transform projectileSpawnPoint; // Laser is instantiated at this point

    private Renderer[] childRenderers; //Renderer of characters
    private Color[,] originColors; // Origin color of characters
    private int blinkCount = 3; // 피격 시 깜빡이는 횟수
    private float invincibleLength; // 무적 지속 시간
    private bool isInvincible = false; // 무적 지속중인지 확인
    private float magnetDuration; // 자석 지속 시간
    private bool isMagnet = false; // 자석 지속중인지 확인
    private GameObject magnetEffect; // 자석 아이템 적용 시 UI

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

        childRenderers = GetComponentsInChildren<Renderer>();
        originColors = new Color[childRenderers.Length, 2];

        for (int i = 0; i < childRenderers.Length; i++)
        {
            for (int j = 0; j < childRenderers[i].sharedMaterials.Length; j++)
            {
                originColors[i, j] = childRenderers[i].sharedMaterials[j].color;
            }
        }

        magnetEffect = GameObject.FindWithTag("MagnetEffect");
    }

    void Update()
    {
        // Only allow new movement input if we're not currently moving
        if (!isMoving)
        {
            // Handle left movement
            if (Input.GetKeyDown(KeyCode.LeftArrow) && currentGridPosition.x > 0)
            {
                currentGridPosition.x--;
                StartCoroutine(SmoothMove());
            }
            // Handle right movement
            else if (Input.GetKeyDown(KeyCode.RightArrow) && currentGridPosition.x < gridSize.x - 1)
            {
                currentGridPosition.x++;
                StartCoroutine(SmoothMove());
            }
            // Handle up movement
            else if (Input.GetKeyDown(KeyCode.UpArrow) && currentGridPosition.y < gridSize.y - 1)
            {
                currentGridPosition.y++;
                StartCoroutine(SmoothMove());
            }
            // Handle down movement
            else if (Input.GetKeyDown(KeyCode.DownArrow) && currentGridPosition.y > 0)
            {
                currentGridPosition.y--;
                StartCoroutine(SmoothMove());
            }
        }

        // Fire Laser
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireLaser();
        }

        if (GameManager.inst.GetLife() <= 0)
        {
            isMoving = true;
            transform.Translate(Vector3.down * moveSpeed * 0.005f, Space.World);
        }
        if (transform.position.y < 0)
        {
            gameObject.SetActive(false);
        }

        magnetEffect.SetActive(isMagnet);
    }

    void FireLaser()
    {
        if (laserFireSound != null)
        {
            AudioSource.PlayClipAtPoint(laserFireSound, transform.position, laserVolume);
        }
        // Create the projectile at the preassigned point
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position + new Vector3(0, 0, 1f), Quaternion.identity);
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

            // Move the player using lerp
            transform.position = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);

            // If we're very close to the target, snap to it
            if (Vector3.Distance(transform.position, endPosition) < 0.01f)
            {
                transform.position = endPosition;
            }

            yield return null;
        }
        centerPosition = transform.position + new Vector3(0f, 0.25f, 0.2f);

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
    private void ChangeColorOriginal()
    {
        for (int i = 0; i < childRenderers.Length; i++)
        {
            for (int j = 0; j < childRenderers[i].sharedMaterials.Length; j++)
            {
                childRenderers[i].sharedMaterials[j].color = originColors[i, j];
            }
        }
    }

    // 자석 아이템 효과
    IEnumerator Magnet()
    {
        float coinSpeed = 40f;
        float distance = 1.4f;
        magnetDuration = 5f;

        if (isMagnet)
            yield break;

        isMagnet = true;

        while (magnetDuration > 0)
        {
            magnetDuration -= Time.deltaTime;
            GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");

            foreach (GameObject coin in coins)
            {
                if (coin != null)
                {
                    if (Vector3.Distance(coin.transform.position, centerPosition) < distance)
                        coin.transform.position = Vector3.MoveTowards(coin.transform.position, centerPosition, coinSpeed * Time.deltaTime);
                }
            }

            yield return null;
        }
        isMagnet = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (isInvincible)
            {
                Destroy(other.gameObject);
                GameManager.inst.AddScore(1000); // 무적 상태에서 적 부딪하면 1000점 추가
                return;
            }
            GameManager.inst.RemoveLife();
            GameManager.inst.AddScore(-1000);
            StartCoroutine(Blink());
        }

        if (other.gameObject.CompareTag("Obstacle"))
        {
            if (isInvincible)
            {
                Destroy(other.gameObject);
                GameManager.inst.AddScore(500); // 무적 상태에서 장애물 부딪하면 500점 추가
                return;
            }
            if (enemyCollisionSound != null)
            {
                AudioSource.PlayClipAtPoint(enemyCollisionSound, transform.position, coinVolume);
            }
            GameManager.inst.RemoveLife();
            GameManager.inst.AddScore(-500);
            StartCoroutine(Blink());
        }

        if (other.gameObject.CompareTag("Heart"))
        {
            GameManager.inst.AddLife();
        }

        if (other.gameObject.CompareTag("Invincible"))
        {
            StartCoroutine(Invincible());
        }

        if (other.gameObject.CompareTag("Magnet"))
        {
            StartCoroutine(Magnet());
        }

        if (other.gameObject.CompareTag("Coin"))
        {
            GameManager.inst.AddScore(200);
            if (coinCollectSound != null)
            {
                AudioSource.PlayClipAtPoint(coinCollectSound, transform.position, coinVolume);
            }
        }

        if (!other.gameObject.CompareTag("Enemy") && !other.gameObject.CompareTag("Obstacle")) //if it is item or coin
        {
            Destroy(other.gameObject);
        }
    }
}
