// BossAttackPattern.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackPattern : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject meteoritePrefab; // � ������
    public GameObject gridCellPrefab; // �׸��� �� ������

    [Header("Player Area")]
    //TODO: BossStagePlayer ��ũ��Ʈ�� constant�� �Ͽ�ȭ�ϱ�(����!: �ܼ��� �Ͽ�ȭ�ϸ� ���� ������ �αٿ� �� ���� �� cell �ȿ� �ִ� ������ �νĵ��� ����)
    public Vector3 areaMin = new Vector3(-10f, 1.5f, -10f); // �÷��̾� �̵� ���� �ּҰ�
    public Vector3 areaMax = new Vector3(10f, 1.5f, 10f);   // �÷��̾� �̵� ���� �ִ밪
    private Vector3 offset = new Vector3(0.2f,0, 0.2f);

    [Header("Attack Settings")]
    public float warningDuration = 1f; // ��� �ð�
    public float meteoriteHeight = 15f; // ��� �������� ����

    [Header("Player")]
    public Transform playerTransform; // �÷��̾��� Transform

    private List<List<Vector3[]>> attackPatternsPerPhase; // Phase�� ���� ����
    private bool isAttacking = false;

    private GridCell[,] gridCells = new GridCell[3, 3]; // 3x3 �׸��� �� �迭
    private float cellSizeX;
    private float cellSizeZ;
    private float meteoriteSize; // � ũ��

    BossStageManager bossStageManager;

    void Start()
    {
        bossStageManager = GameObject.Find("BossStageManager").GetComponent<BossStageManager>();
        areaMin -= offset;
        areaMax += offset;
        
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
                cell.transform.localScale = new Vector3(cellSizeX, 0.1f  ,cellSizeZ); // �� ũ�� ����

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
        meteoriteSize = Mathf.Min(cellSizeX, cellSizeZ) * 0.05f;
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

        // Phase 1 ����: �÷��̾ �� �ִ� �׸��� ����
        List<Vector3[]> phase1Patterns = new List<Vector3[]> {
            new Vector3[] { gridPositions[0, 0] },
            new Vector3[] { gridPositions[0, 1] },
            new Vector3[] { gridPositions[0, 2] },
            new Vector3[] { gridPositions[1, 0] },
            new Vector3[] { gridPositions[1, 1] },
            new Vector3[] { gridPositions[1, 2] },
            new Vector3[] { gridPositions[2, 0] },
            new Vector3[] { gridPositions[2, 1] },
            new Vector3[] { gridPositions[2, 2] }
        };
        attackPatternsPerPhase.Add(phase1Patterns);

        // Phase 2 ����: �÷��̾ �� �ִ� ����/���� �ٰ� �밢�� ������ ������ ���� -> ���� 1: ����/���� �� ��ü ���� / ���� 2: �밢�� ����
        List<Vector3[]> phase2Patterns = new List<Vector3[]> {
            new Vector3[] { gridPositions[0, 0], gridPositions[0, 1], gridPositions[0, 2] },
            new Vector3[] { gridPositions[1, 0], gridPositions[1, 1], gridPositions[1, 2] },
            new Vector3[] { gridPositions[2, 0], gridPositions[2, 1], gridPositions[2, 2] },
            new Vector3[] { gridPositions[0, 0], gridPositions[1, 0], gridPositions[2, 0] },
            new Vector3[] { gridPositions[0, 1], gridPositions[1, 1], gridPositions[2, 1] },
            new Vector3[] { gridPositions[0, 2], gridPositions[1, 2], gridPositions[2, 2] },
            new Vector3[] { gridPositions[0, 0], gridPositions[1, 1], gridPositions[2, 2] },
            new Vector3[] { gridPositions[0, 2], gridPositions[1, 1], gridPositions[2, 0] }
        };

        attackPatternsPerPhase.Add(phase2Patterns);

        /* Phase 3 ����: �� ���� ���� �� �÷��̾ �� �ִ� ������ �����ϴ� ������ ���� ���� 
        => ���� 1: �� �𼭸� ��� �׸��� 4�� ���� / ���� 2: �� �밢���� �������� �Ǵ� �׸��� 5�� ���� / ���� 3: L�� ����� 5�� �׸��� ���� / ���� 4: ù��°�� ����° ����/���� �� �׸��� 6�� ����*/
        List<Vector3[]> phase3Patterns = new List<Vector3[]>{
        new Vector3[] { gridPositions[0, 1], gridPositions[1, 0], gridPositions[1, 2], gridPositions[2, 1] },
        new Vector3[] { gridPositions[0, 0], gridPositions[1, 1], gridPositions[2, 2], gridPositions[0, 2], gridPositions[2, 0] },
        new Vector3[] { gridPositions[0, 0], gridPositions[0, 1], gridPositions[0, 2], gridPositions[1, 2], gridPositions[2, 2] },
        new Vector3[] { gridPositions[0, 0], gridPositions[1, 0], gridPositions[2, 0], gridPositions[2, 1], gridPositions[2, 2] },
        new Vector3[] { gridPositions[0, 0], gridPositions[0, 1], gridPositions[0, 2], gridPositions[2, 0], gridPositions[2, 1], gridPositions[2, 2] },
        new Vector3[] { gridPositions[0, 0], gridPositions[1, 0], gridPositions[2, 0], gridPositions[0, 2], gridPositions[1, 2], gridPositions[2, 2] }
        };


        attackPatternsPerPhase.Add(phase3Patterns);

        if (attackPatternsPerPhase.Count != bossStageManager.GetBossMaxLife())
        {
            Debug.LogWarning("Count of attack patterns per phase must be equal to boss max life!");
        }
    }

    // ���� ������ ����
    IEnumerator AttackSequence()
    {
        while (bossStageManager.GetBossLife() > 0)
        {
            if (!isAttacking)
            {
                isAttacking = true;

                // ���� Phase�� ���� ����Ʈ ��������
                List<Vector3[]> patterns = attackPatternsPerPhase[bossStageManager.GetPhase()];

                // �÷��̾� ��ġ�� ���Ե� ���� index üũ
                List<int> availableIndex = new List<int>();
                for (int i = 0; i < patterns.Count; i++)
                {
                    if (IsPlayerInPattern(patterns[i])){ availableIndex.Add(i); }
                }

                // ���� �����ϰ� ����
                Vector3[] selectedPattern = null;
                int randomIndex = Random.Range(0, availableIndex.Count);
                selectedPattern = patterns[availableIndex[randomIndex]];

                if (selectedPattern != null) 
                { 
                    // ������ �׸��� �� ���� (������)
                    HighlightGridCells(selectedPattern, Color.red);
                    // ��� �ð� ���
                    yield return new WaitForSeconds(warningDuration);

                    // ���� ���� �� � ���� ����
                    ResetGridCells(selectedPattern);

                    foreach (Vector3 attackPos in selectedPattern)
                    {
                        StartCoroutine(ExecuteAttack(attackPos));
                    }

                    // ���� ���ݱ��� ��� (���� ����)
                    yield return new WaitForSeconds(2f); // ��: 2�� ���
                    isAttacking = false;
                }


                
            }

            yield return null;
        }

        // ������ ������� �� ó�� (�ʿ� �� �߰�)
    }

    // �÷��̾ ���Ͽ� ���ԵǴ��� Ȯ���ϴ� �Լ�
    bool IsPlayerInPattern(Vector3[] pattern)
    {
        foreach (Vector3 cellPosition in pattern)
        {
            GridCell cell = GetGridCellByPosition(cellPosition);
            if (cell != null && IsPlayerInCell(cell))
            {
                return true;
            }
        }
        return false;
    }


    // �÷��̾ Ư�� �׸��� ���� �ִ��� Ȯ���ϴ� �Լ�
    bool IsPlayerInCell(GridCell cell)
    {
        // �׸��� ���� ��� ���
        Vector3 cellPosition = cell.transform.position;
        Vector3 cellScale = cell.transform.localScale;

        float halfSizeX = cellScale.x / 2f;
        float halfSizeZ = cellScale.z / 2f;

        float minX = cellPosition.x - halfSizeX;
        float maxX = cellPosition.x + halfSizeX;
        float minZ = cellPosition.z - halfSizeZ;
        float maxZ = cellPosition.z + halfSizeZ;

        Vector3 playerPos = playerTransform.position;

        // �÷��̾��� ��ġ�� �׸��� ���� ��� �ȿ� �ִ��� Ȯ��
        if (playerPos.x >=minX && playerPos.x <= maxX && playerPos.z >= minZ && playerPos.z <= maxZ)
        {
            return true;
        }
        return false;
    }

    // �׸��� �� ���� ��� �Լ�
    void HighlightGridCells(Vector3[] positions, Color color)
    {
        foreach (Vector3 pos in positions)
        {
            GridCell cell = GetGridCellByPosition(pos);
            if (cell != null)
            {
                cell.Highlight(color);
            }
        }
    }

    // �׸��� �� ���� ���� �Լ�
    void ResetGridCells(Vector3[] positions)
    {
        foreach (Vector3 pos in positions)
        {
            GridCell cell = GetGridCellByPosition(pos);
            if (cell != null)
            {
                cell.ResetColor();
            }
        }
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
        Vector3 spawnPosition = new Vector3(position.x, areaMin.y + meteoriteHeight, position.z - 2f);
        GameObject meteorite = Instantiate(meteoritePrefab, spawnPosition, Quaternion.identity);
        meteorite.transform.localScale = Vector3.one * meteoriteSize;

        yield return null;
    }
}
