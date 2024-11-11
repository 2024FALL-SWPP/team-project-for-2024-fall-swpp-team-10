// BossAttackPattern.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackPattern : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject meteoritePrefab; // � ������
    public GameObject explosionPrefab; // ���� ��ƼŬ �ý��� ������
    public GameObject gridCellPrefab; // �׸��� �� ������

    [Header("Player Area")]
    public Vector3 areaMin = new Vector3(-10f, 0.5f, -10f); // �÷��̾� �̵� ���� �ּҰ�
    public Vector3 areaMax = new Vector3(10f, 0.5f, 10f);   // �÷��̾� �̵� ���� �ִ밪

    [Header("Attack Settings")]
    public float warningDuration = 1f; // ��� �ð�
    public float meteoriteHeight = 10f; // ��� �������� ����
    public float bossHealth = 100f; // ������ �����

    private int currentPhase = 0; // ���� Phase
    private int currentPatternIndex = 0; // ���� ���� �ε���
    private List<List<Vector3[]>> attackPatternsPerPhase; // Phase�� ���� ����
    private bool isAttacking = false;

    private GridCell[,] gridCells = new GridCell[3, 3]; // 3x3 �׸��� �� �迭
    private float cellSizeX;
    private float cellSizeZ;
    private float meteoriteSize; // � ũ��

    void Start()
    {
        // �׸��� �� ���� �� �ʱ�ȭ
        InitializeGrid();

        // ���� ���� �ʱ�ȭ
        InitializeAttackPatterns();

        // ���� ������ ����
        StartCoroutine(AttackSequence());
    }

    // �׸��� �� ����
    void InitializeGrid()
    {
        cellSizeX = (areaMax.x - areaMin.x) / 3f;
        cellSizeZ = (areaMax.z - areaMin.z) / 3f;

        for (int x = 0; x < 3; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                // �׸��� ���� �߽� ��ġ ���
                float posX = areaMin.x + cellSizeX * (x + 0.5f);
                float posZ = areaMin.z + cellSizeZ * (z + 0.5f);
                Vector3 cellPosition = new Vector3(posX, areaMin.y, posZ);

                // �׸��� �� �ν��Ͻ� ����
                GameObject cell = Instantiate(gridCellPrefab, cellPosition, Quaternion.identity);
                cell.transform.localScale = new Vector3(cellSizeX, 0.1f, cellSizeZ); // �� ũ�� ����

                // GridCell ������Ʈ �Ҵ�
                GridCell gridCell = cell.GetComponent<GridCell>();
                if (gridCell == null)
                {
                    gridCell = cell.AddComponent<GridCell>();
                }

                // �׸��� �� �迭�� ����
                gridCells[x, z] = gridCell;

                // ������� ���� �׸��� ���� ��ġ �ð�ȭ
                Debug.DrawLine(new Vector3(posX - cellSizeX / 2, areaMin.y + 0.1f, posZ - cellSizeZ / 2),
                               new Vector3(posX + cellSizeX / 2, areaMin.y + 0.1f, posZ - cellSizeZ / 2),
                               Color.green, 100f);
                Debug.DrawLine(new Vector3(posX + cellSizeX / 2, areaMin.y + 0.1f, posZ - cellSizeZ / 2),
                               new Vector3(posX + cellSizeX / 2, areaMin.y + 0.1f, posZ + cellSizeZ / 2),
                               Color.green, 100f);
                Debug.DrawLine(new Vector3(posX + cellSizeX / 2, areaMin.y + 0.1f, posZ + cellSizeZ / 2),
                               new Vector3(posX - cellSizeX / 2, areaMin.y + 0.1f, posZ + cellSizeZ / 2),
                               Color.green, 100f);
                Debug.DrawLine(new Vector3(posX - cellSizeX / 2, areaMin.y + 0.1f, posZ + cellSizeZ / 2),
                               new Vector3(posX - cellSizeX / 2, areaMin.y + 0.1f, posZ - cellSizeZ / 2),
                               Color.green, 100f);
            }
        }

        // �׸��� �� ũ�⿡ ���߾� � ũ�� ���� (�׸��� ���� �ʺ�� ���� �� ���� ���� �������� ����)
        meteoriteSize = Mathf.Min(cellSizeX, cellSizeZ) * 0.05f; // ��: 80% ũ��� ����
    }

    // ���� ���� �ʱ�ȭ
    void InitializeAttackPatterns()
    {
        attackPatternsPerPhase = new List<List<Vector3[]>>();

        // �׸��� ���� ���� ��ǥ�� ����
        Vector3[,] gridPositions = new Vector3[3, 3];
        for (int x = 0; x < 3; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                gridPositions[x, z] = gridCells[x, z].transform.position;
            }
        }

        // Phase 1 ����: �߾� �׸��� ����
        List<Vector3[]> phase1Patterns = new List<Vector3[]>();
        phase1Patterns.Add(new Vector3[] { gridPositions[1, 1] });
        attackPatternsPerPhase.Add(phase1Patterns);

        // Phase 2 ����: ��� �����ٰ� �밢�� ������ ������ ����
        List<Vector3[]> phase2Patterns = new List<Vector3[]>();
        // ���� 1: ��� ������ ��ü ����
        phase2Patterns.Add(new Vector3[] { gridPositions[1, 0], gridPositions[1, 1], gridPositions[1, 2] });
        // ���� 2: �� �밢�� ��ü ����
        phase2Patterns.Add(new Vector3[] { gridPositions[0, 0], gridPositions[1, 1], gridPositions[2, 2], gridPositions[0, 2], gridPositions[2, 0] });
        attackPatternsPerPhase.Add(phase2Patterns);

        // Phase 3 ����: �� ���� ������ ������ ����
        List<Vector3[]> phase3Patterns = new List<Vector3[]>();
        // ���� 1: �� �𼭸� ��� �׸��� 4�� ����
        phase3Patterns.Add(new Vector3[] { gridPositions[0, 1], gridPositions[1, 0], gridPositions[1, 2], gridPositions[2, 1] });
        // ���� 2: �� �밢���� �������� �Ǵ� �׸��� 5�� ����
        phase3Patterns.Add(new Vector3[] { gridPositions[0, 0], gridPositions[1, 1], gridPositions[2, 2], gridPositions[0, 2], gridPositions[2, 0] });
        // ���� 3: L�� ����� 5�� �׸��� ����
        phase3Patterns.Add(new Vector3[] { gridPositions[0, 0], gridPositions[0, 1], gridPositions[0, 2], gridPositions[1, 2], gridPositions[2, 2] });
        // ���� 4: ù��°�� ����° ������ �׸��� 6�� ����
        phase3Patterns.Add(new Vector3[] { gridPositions[0, 0], gridPositions[0, 1], gridPositions[0, 2], gridPositions[2, 0], gridPositions[2, 1], gridPositions[2, 2] });
        attackPatternsPerPhase.Add(phase3Patterns);
    }

    // ���� Phase ������Ʈ
    void UpdatePhase()
    {
        if (bossHealth >= 70f)
        {
            currentPhase = 0;
        }
        else if (bossHealth >= 40f)
        {
            currentPhase = 1;
        }
        else
        {
            currentPhase = 2;
        }
    }

    // ���� ������ ����
    IEnumerator AttackSequence()
    {
        while (bossHealth > 0)
        {
            if (!isAttacking)
            {
                isAttacking = true;

                // ���� Phase ������Ʈ
                UpdatePhase();

                // ���� Phase�� ���� ����Ʈ ��������
                List<Vector3[]> patterns = attackPatternsPerPhase[currentPhase];

                // ���� ���� �ε����� ���� ���� ����
                Vector3[] currentPattern = patterns[currentPatternIndex];

                // ���� �ε��� ������Ʈ (���� ������ ����)
                currentPatternIndex = (currentPatternIndex + 1) % patterns.Count;

                // ������ �׸��� �� ���� (������)
                foreach (Vector3 attackPos in currentPattern)
                {
                    GridCell cell = GetGridCellByPosition(attackPos);
                    if (cell != null)
                    {
                        cell.Highlight(Color.red);
                    }
                }

                // ��� �ð� ���
                yield return new WaitForSeconds(warningDuration);

                // � ���� �� ���� ����
                foreach (Vector3 attackPos in currentPattern)
                {
                    GridCell cell = GetGridCellByPosition(attackPos);
                    if (cell != null)
                    {
                        cell.ResetColor();
                        StartCoroutine(ExecuteAttack(attackPos));
                    }
                }

                // ���� ���ݱ��� ��� (���� ����)
                yield return new WaitForSeconds(2f); // ��: 2�� ���
                isAttacking = false;
            }

            yield return null;
        }

        // ������ ������� �� ó�� (�ʿ� �� �߰�)
    }

    // Ư�� ��ġ�� �׸��� �� ��������
    GridCell GetGridCellByPosition(Vector3 position)
    {
        // ��Ȯ�� ��ġ �񱳸� ���� ���� �Ÿ� ����
        foreach (GridCell cell in gridCells)
        {
            if (Vector3.Distance(cell.transform.position, position) < 0.1f)
            {
                return cell;
            }
        }
        return null;
    }

    // Ư�� ��ġ�� � ���� ����
    IEnumerator ExecuteAttack(Vector3 position)
    {
        // � ���� ��ġ: �׸��� ���� �߽ɿ��� meteoriteHeight ���� ��
        Vector3 spawnPosition = new Vector3(position.x, areaMin.y + meteoriteHeight, position.z-2.5f);
        GameObject meteorite = Instantiate(meteoritePrefab, spawnPosition, Quaternion.identity);
        meteorite.transform.localScale = Vector3.one * meteoriteSize;

        // Rigidbody�� ���ٸ� �߰��Ͽ� ���������� ���������� ����
        Rigidbody rb = meteorite.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = meteorite.AddComponent<Rigidbody>();
            rb.useGravity = true;
        }

        // ��� ���� ���� ������ ���
        while (meteorite.transform.position.y > areaMin.y + 0.5f)
        {
            yield return null;
        }

        // ���� ȿ�� ����
        TriggerExplosion(position);

        // � ����
        Destroy(meteorite);
    }

    // ���� ȿ�� ����
    void TriggerExplosion(Vector3 position)
    {
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
            Destroy(explosion, 2f); // 2�� �� ����
        }
    }

    void Update()
    {
        // ���� ���ӿ����� ������ ������� ���� �̺�Ʈ�� ���� �����ؾ� �մϴ�.
        // ���⼭�� �׽�Ʈ�� ���� �ð��� ������ ���� ���ҽ�ŵ�ϴ�.
        if (bossHealth > 0)
        {
            bossHealth -= Time.deltaTime * 5f; // �ʴ� 5�� ����
            if (bossHealth < 0f)
            {
                bossHealth = 0;
                // ���� ��� ó�� (�ʿ� �� �߰�)
            }
        }
    }
}
