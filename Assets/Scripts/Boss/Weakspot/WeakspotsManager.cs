using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakspotsManager : MonoBehaviour
{
    [Header("Boss weak spot variables")]
    [SerializeField] GameObject weakSpotPf;
    [SerializeField] Mesh bossMesh;
    Camera mainCamera;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] LayerMask occlusionMask;

    private void Awake()
    {
        // Weak spot
        mainCamera = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        GetWeakSpots();
    }

    // Create weak spots : Call at the start of each phase
    void GetWeakSpots()
    {
        // Get all vertices on boss mesh
        Vector3[] vertices = bossMesh.vertices;

        // Divide target region into 8 areas
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
            if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1 || viewportPos.z <= 0) continue;

            // Perform an occlusion check
            if (Physics.Linecast(mainCamera.transform.position, worldPos, occlusionMask)) continue;

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

        // Choose three random regions and select one random point from each
        List<int> chosenIndices = new List<int>();

        int count = 0;

        while (chosenIndices.Count < 3)
        {
            int randomIndex = Random.Range(0, 8);

            // 각각 다른 구역의 weakspot 생성을 16번 시도하고, 16번의 시도 끝에도 3개의 weakspot이 생성되지 않았다면 같은 구역의 weakspot도 허용한다.
            if ((!chosenIndices.Contains(randomIndex) || count > 16) && regions[randomIndex].Count > 0)
            {
                chosenIndices.Add(randomIndex);
                Vector3 randomPoint = regions[randomIndex][Random.Range(0, regions[randomIndex].Count)];

                // Make weak spot orientation match boss mesh
                Vector3 localPoint = meshFilter.transform.InverseTransformPoint(randomPoint);
                int closestVertexIndex = FindClosestVertexIndex(localPoint);

                Vector3 normal = bossMesh.normals[closestVertexIndex];
                Vector3 worldNormal = meshFilter.transform.TransformDirection(normal);

                Instantiate(weakSpotPf, randomPoint, Quaternion.LookRotation(worldNormal), gameObject.transform);
            }
            count += 1;
        }
    }

    // To find normal (Called by GetWeakSpots())
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

    // Called on boss death, call on phase change
    public void RemoveAllWeakSpots()
    {
        GameObject[] wss = GameObject.FindGameObjectsWithTag("WeakSpot");
        foreach (GameObject ws in wss) Destroy(ws);
    }


    // Removes old and produces new weakspots.
    public void NewWeakSpots()
    {
        Invoke("RemoveAllWeakSpots", 0.2f);
        Invoke("GetWeakSpots", 0.2f);
    }
}
