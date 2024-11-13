using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossControl : MonoBehaviour
{
    public Animator animator;

    // Color related variables
    [SerializeField] Color bossStartColor;
    [SerializeField] Color myColorRed = new Color(203f / 255f, 83f / 255f, 83f / 255f, 1);
    Dictionary<Color, Color> myColorDict;

    bool bossDead = false;

    [SerializeField] Transform bossTransform;

    // Carrot Shooting variables
    [SerializeField] Transform carrotSpawnOffset;
    [SerializeField] GameObject carrotPf;
    [SerializeField] Transform carrotTargetPos;
    [SerializeField] float carrotDelayTime; // Time between each carrot throw
    [SerializeField] float carrotSpeed;

    // Boss movement variables
    [SerializeField] float bossHorizontalRange; // x range of boss
    [SerializeField] float bossHorizontalSpeed;
    float bossHorizontalPos;    // Position boss is to move to

    // Boss Death animation variables
    [SerializeField] ParticleSystem bossSmoke;
    [SerializeField] float bossReducedSize;
    // Initial positions and rotations for post death effect
    Dictionary<Transform, Vector3> bossComponentsInitialPositions = new Dictionary<Transform, Vector3>();
    Dictionary<Transform, Quaternion> bossComponentsInitialRotations = new Dictionary<Transform, Quaternion>();

    // Boss weak spot variables
    //[SerializeField] MeshCollider bossMeshCollider;
    [SerializeField] GameObject weakSpotPf;
    Mesh bossMesh;
    Camera mainCamera;
    MeshFilter meshFilter;
    LayerMask occlusionMask;


    // Start is called before the first frame update
    void Awake()
    {
        // Set up boss color
        myColorDict = new Dictionary<Color, Color>()
        {
            {Color.red, myColorRed},
            {Color.black, Color.gray }
        };
        ChangeColor(bossStartColor);

        // Set up for boss death animation
        foreach (Transform childTransform in bossTransform)
        {
            bossComponentsInitialPositions[childTransform] = childTransform.position;
            bossComponentsInitialRotations[childTransform] = childTransform.rotation;
        }

        // Weak spot
        //bossMeshCollider.enabled = true;
        mainCamera = Camera.main;
        meshFilter = gameObject.GetComponent<MeshFilter>();
        bossMesh = meshFilter.mesh;
        occlusionMask = gameObject.layer;

        GetWeakSpots();
    }

    // Update is called once per frame
    void Update()
    {
        if (!bossDead)
        {
            // Rotate to look at player (+ height adjustment to rotate only y axis)
            bossTransform.LookAt(carrotTargetPos.position + Vector3.up * (bossTransform.position.y - carrotTargetPos.position.y));
            // Generate new boss target position once in close enough proximity
            if (Mathf.Abs(bossHorizontalPos - bossTransform.position.x) < 0.1)
            {
                bossHorizontalPos = Random.Range(-bossHorizontalRange, bossHorizontalRange);
            }

            // Boss side to side movement
            if (bossHorizontalPos < bossTransform.position.x)
            {
                bossTransform.position -= new Vector3(bossHorizontalSpeed * Time.deltaTime, 0, 0);
            }
            if (bossHorizontalPos > bossTransform.position.x)
                bossTransform.position += new Vector3(bossHorizontalSpeed * Time.deltaTime, 0, 0);

        }
        // Finish condition for one shooting interval
        if (bossDead)
            StopShooting();
    }

    // Shoot one carrot
    public void ShootProjectile()
    {
        StartCoroutine("ShootProjectileCoroutine");
    }

    IEnumerator ShootProjectileCoroutine()
    {
        animator.SetBool("mouthOpen_b", true);

        yield return new WaitForSeconds(0.2f);

        Vector3 carrotSpawnPos = carrotSpawnOffset.position;

        Vector3 orientation = carrotTargetPos.position - carrotSpawnPos;
        GameObject projectile = Instantiate(carrotPf, carrotSpawnPos, Quaternion.Euler(orientation));
        projectile.gameObject.transform.LookAt(carrotTargetPos);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        projectileRb.velocity = orientation.normalized * carrotSpeed;

        yield return new WaitForSeconds(0.5f);

        animator.SetBool("mouthOpen_b", false);
    }

    // Stop shooting and close mouth
    public void StopShooting()
    {
        CancelInvoke("ShootProjectile");
        animator.SetBool("mouthOpen_b", false);
    }

    // Start shoot interval
    public void StartShootInterval()
    {
        InvokeRepeating("ShootProjectile", 0f, Mathf.Max(carrotDelayTime, 1f)); // Appropriate delay time based on testing: 1f ~ 1.6f
    }

    // Change color function to keep
    public void ChangeColor(Color bossColorKey)
    {
        Color bossColorValue = (myColorDict.GetValueOrDefault(bossColorKey, bossColorKey)); // If dict contains key, extract value. Otherwise, pass color as is.
        ChangeColorHelper(gameObject.transform, bossColorValue); // Actual color (value) passed as parameter
    }

    // Recursively change color of all children
    public void ChangeColorHelper(Transform transform, Color bossColor)
    {
        foreach (Transform childTransform in transform)
        {
            GameObject child = childTransform.gameObject;
            SkinnedMeshRenderer smr = child.GetComponent<SkinnedMeshRenderer>();
            if (smr != null) smr.material.color = bossColor;
            MeshRenderer mr = child.GetComponent<MeshRenderer>();
            if (mr != null) mr.material.color = bossColor;

            if (childTransform.childCount > 0) ChangeColorHelper(childTransform, bossColor);
        }
    }

    public void BossDeath()
    {
        if (!bossDead)
        {
            bossDead = true;
            //bossMeshCollider.enabled = false;

            // Fall back effect
            foreach (Transform childTransform in bossTransform)
            {
                GameObject child = childTransform.gameObject;
                Rigidbody rb = child.GetComponent<Rigidbody>();

                if (rb)
                {
                    rb.isKinematic = false;
                    rb.AddForce(Vector3.forward * 50 + Vector3.up * 10f, ForceMode.Impulse);
                }
            }

            Invoke("BossDeathHelper", 2f);
        }
    }

    public void BossDeathHelper()
    {
        // Particle effect
        bossSmoke.Play();
        Invoke("BossDeathTransform", 0.5f);
    }

    void BossDeathTransform()
    {
        // Transform boss into small white rabbit
        ChangeColor(Color.white); // Pass the color key: Color.red or Color.black. Colors that are not defined as keys will be passed as is to the helper function.
        foreach (Transform childTransform in bossTransform)
        {
            GameObject child = childTransform.gameObject;
            Rigidbody rb = child.GetComponent<Rigidbody>();

            // Restore positions
            if (rb)
            {
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
            }
            childTransform.position = bossComponentsInitialPositions[childTransform];
            childTransform.rotation = bossComponentsInitialRotations[childTransform];
        }
        bossTransform.localScale = new Vector3(bossReducedSize, bossReducedSize, bossReducedSize);
    }

    // Weak Spot
    void GetWeakSpots()
    {
        // Get all vertices on boss mesh
        List<Vector3> visibleVertices = new List<Vector3>();
        Vector3[] vertices = bossMesh.vertices;

        Bounds bounds = bossMesh.bounds;
        Vector3 center = meshFilter.transform.TransformPoint(bounds.center);

        float quarterHeight = bounds.size.y / 4;
        float lowerY = center.y - quarterHeight;
        float middleY = center.y;
        float upperY = center.y + quarterHeight;

        List<Vector3>[] regions = new List<Vector3>[8];
        for (int i = 0; i < 8; i++) regions[i] = new List<Vector3>();

        // Reduce vertices
        foreach (Vector3 vertex in vertices)
        {
            Vector3 worldPos = meshFilter.transform.TransformPoint(vertex);
            Vector3 viewportPos = mainCamera.WorldToViewportPoint(worldPos);

            // Check if vertex is within the camera’s frustum
            if (viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1 && viewportPos.z > 0)
            {
                // Perform an occlusion check
                if (!Physics.Linecast(mainCamera.transform.position, worldPos, occlusionMask))
                {
                    int index = 0;

                    // X-axis split: left or right
                    if (worldPos.x > center.x) index += 1;

                    // Y-axis split: bottom, lower middle, upper middle, or top
                    if (worldPos.y > upperY) index += 6;        // Top region
                    else if (worldPos.y > middleY) index += 4;  // Upper middle region
                    else if (worldPos.y > lowerY) index += 2;   // Lower middle region
                    // Bottom region does not need an additional offset

                    //|----|----|
                    //|  6 | 7  |
                    //|----|----| upper y
                    //| 4  |  5 |
                    //|----|----| middle y
                    //|  2 |  3 |
                    //|----|----| lower y
                    //| 0  |  1 |
                    //|----|----|

                    regions[index].Add(worldPos);
                }
            }
        }

        // Choose three random regions and select one random point from each
        List<Vector3> selectedPoints = new List<Vector3>();
        List<int> chosenIndices = new List<int>();

        while (chosenIndices.Count < 3)
        {
            int randomIndex = Random.Range(0, 8);
            if (!chosenIndices.Contains(randomIndex) && regions[randomIndex].Count > 0)
            {
                chosenIndices.Add(randomIndex);
                Vector3 randomPoint = regions[randomIndex][Random.Range(0, regions[randomIndex].Count)];
                selectedPoints.Add(randomPoint);

                Vector3 localPoint = meshFilter.transform.InverseTransformPoint(randomPoint);
                int closestVertexIndex = FindClosestVertexIndex(localPoint);

                Vector3 normal = bossMesh.normals[closestVertexIndex];
                Vector3 worldNormal = meshFilter.transform.TransformDirection(normal);

                Instantiate(weakSpotPf, randomPoint, Quaternion.LookRotation(worldNormal), gameObject.transform);
            }
        }
    }

    int FindClosestVertexIndex(Vector3 point)
    {
        Vector3[] vertices = bossMesh.vertices;
        int closestIndex = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < vertices.Length; i++)
        {
            float distance = Vector3.Distance(vertices[i], point);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
}