// BossAttackPattern.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackPattern : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject meteoritePrefab; // 운석 프리팹
    public GameObject gridCellPrefab; // 그리드 셀 프리팹

    [Header("Player Area")]
    //TODO: BossStagePlayer 스크립트의 constant와 일원화하기(주의!: 단순히 일원화하면 영역 꼭짓점 부근에 서 있을 때 cell 안에 있는 것으로 인식되지 않음)
    public Vector3 areaMin = new Vector3(-10f, 1.5f, -10f); // 플레이어 이동 영역 최소값
    public Vector3 areaMax = new Vector3(10f, 1.5f, 10f);   // 플레이어 이동 영역 최대값
    private Vector3 offset = new Vector3(0.2f,0, 0.2f);

    [Header("Attack Settings")]
    public float warningDuration = 1f; // 경고 시간
    public float meteoriteHeight = 15f; // 운석이 떨어지는 높이

    [Header("Player")]
    public Transform playerTransform; // 플레이어의 Transform

    private List<List<Vector3[]>> attackPatternsPerPhase; // Phase별 공격 패턴
    private bool isAttacking = false;

    private GridCell[,] gridCells = new GridCell[3, 3]; // 3x3 �׸��� �� �迭
    private float cellSizeX;
    private float cellSizeZ;
    private float meteoriteSize; // 운석 크기

    BossStageManager bossStageManager;

    void Start()
    {
        bossStageManager = GameObject.Find("BossStageManager").GetComponent<BossStageManager>();
        areaMin -= offset;
        areaMax += offset;
        // 그리드 셀 생성 및 초기화
        InitializeGrid();

        // 공격 패턴 초기화
        InitializeAttackPatterns();

        // 공격 시퀀스 시작
        StartCoroutine(AttackSequence());
    }



    // 그리드 셀 생성
    void InitializeGrid()
    {
        cellSizeX = (areaMax.x - areaMin.x) / 3f;
        cellSizeZ = (areaMax.z - areaMin.z) / 3f;

        for (int x = 0; x < 3; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                // 그리드 셀의 중심 위치 계산
                float posX = areaMin.x + cellSizeX * (x + 0.5f);
                float posZ = areaMin.z + cellSizeZ * (z + 0.5f);
                Vector3 cellPosition = new Vector3(posX, areaMin.y, posZ);

                // 그리드 셀 인스턴스 생성
                GameObject cell = Instantiate(gridCellPrefab, cellPosition, Quaternion.identity);
                cell.transform.localScale = new Vector3(cellSizeX, 0.1f  ,cellSizeZ); // 셀 크기 조정

                // GridCell 컴포넌트 할당
                GridCell gridCell = cell.GetComponent<GridCell>();
                if (gridCell == null)
                {
                    gridCell = cell.AddComponent<GridCell>();
                }

                // 그리드 셀 배열에 저장
                gridCells[x, z] = gridCell;

                // 디버깅을 위해 그리드 셀의 위치 시각화
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

        // 그리드 셀 크기에 맞추어 운석 크기 설정 (그리드 셀의 너비와 깊이 중 작은 값을 기준으로 설정)
        meteoriteSize = Mathf.Min(cellSizeX, cellSizeZ) * 0.05f;
    }
    // 공격 패턴 초기화
    void InitializeAttackPatterns()
    {
        attackPatternsPerPhase = new List<List<Vector3[]>>();

        // 그리드 셀의 월드 좌표를 저장
        Vector3[,] gridPositions = new Vector3[3, 3];
        for (int x = 0; x < 3; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                gridPositions[x, z] = gridCells[x, z].transform.position;
            }
        }

        // Phase 1 패턴: 플레이어가 서 있는 그리드 공격
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

        // Phase 2 패턴: 플레이어가 서 있는 가로/세로 줄과 대각선 공격을 번갈아 실행 -> 패턴 1: 가로/세로 줄 전체 공격 / 패턴 2: 대각선 공격
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

        /* Phase 3 패턴: 네 가지 패턴 중 플레이어가 서 있는 영역을 포함하는 패턴을 랜덤 실행 
        => 패턴 1: 각 모서리 가운데 그리드 4개 공격 / 패턴 2: 두 대각선의 합집합이 되는 그리드 5개 공격 / 패턴 3: L자 모양의 5개 그리드 공격 / 패턴 4: 첫번째와 세번째 가로/세로 줄 그리드 6개 공격*/
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

    // 공격 시퀀스 실행
    IEnumerator AttackSequence()
    {
        while (bossStageManager.GetBossLife() > 0)
        {
            if (!isAttacking)
            {
                isAttacking = true;

                // 현재 Phase의 패턴 리스트 가져오기
                List<Vector3[]> patterns = attackPatternsPerPhase[bossStageManager.GetPhase()];

                // 플레이어 위치가 포함된 패턴 index 체크
                List<int> availableIndex = new List<int>();
                for (int i = 0; i < patterns.Count; i++)
                {
                    if (IsPlayerInPattern(patterns[i])){ availableIndex.Add(i); }
                }

                // 패턴 랜덤하게 선택
                Vector3[] selectedPattern = null;
                int randomIndex = Random.Range(0, availableIndex.Count);
                selectedPattern = patterns[availableIndex[randomIndex]];

                if (selectedPattern != null) 
                { 
                    // 공격할 그리드 셀 강조 (빨간색)
                    HighlightGridCells(selectedPattern, Color.red);
                    // 경고 시간 대기
                    yield return new WaitForSeconds(warningDuration);

                    // 강조 해제 및 운석 공격 실행
                    ResetGridCells(selectedPattern);

                    foreach (Vector3 attackPos in selectedPattern)
                    {
                        StartCoroutine(ExecuteAttack(attackPos));
                    }

                    // 다음 공격까지 대기 (조정 가능)
                    yield return new WaitForSeconds(2f); // 예: 2초 대기
                    isAttacking = false;
                }


                
            }

            yield return null;
        }

        // 보스가 사망했을 때 처리 (필요 시 추가)
    }

    // 플레이어가 패턴에 포함되는지 확인하는 함수
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


    // 플레이어가 특정 그리드 셀에 있는지 확인하는 함수
    bool IsPlayerInCell(GridCell cell)
    {
        // 그리드 셀의 경계 계산
        Vector3 cellPosition = cell.transform.position;
        Vector3 cellScale = cell.transform.localScale;

        float halfSizeX = cellScale.x / 2f;
        float halfSizeZ = cellScale.z / 2f;

        float minX = cellPosition.x - halfSizeX;
        float maxX = cellPosition.x + halfSizeX;
        float minZ = cellPosition.z - halfSizeZ;
        float maxZ = cellPosition.z + halfSizeZ;

        Vector3 playerPos = playerTransform.position;

        // 플레이어의 위치가 그리드 셀의 경계 안에 있는지 확인
        if (playerPos.x >=minX && playerPos.x <= maxX && playerPos.z >= minZ && playerPos.z <= maxZ)
        {
            return true;
        }
        return false;
    }

    // 그리드 셀 강조 기능 함수
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

    // 그리드 셀 강조 해제 함수
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

    // 특정 위치의 그리드 셀 가져오기
    GridCell GetGridCellByPosition(Vector3 position)
    {
        // 정확한 위치 비교를 위해 작은 거리 기준
        foreach (GridCell cell in gridCells)
        {
            if (Vector3.Distance(cell.transform.position, position) < 0.1f)
            {
                return cell;
            }
        }
        return null;
    }

    // 특정 위치에 운석 공격 실행
    IEnumerator ExecuteAttack(Vector3 position)
    {
        // 운석 생성 위치: 그리드 셀의 중심에서 meteoriteHeight 높이 위
        Vector3 spawnPosition = new Vector3(position.x, areaMin.y + meteoriteHeight, position.z - 2.5f);
        GameObject meteorite = Instantiate(meteoritePrefab, spawnPosition, Quaternion.identity);
        meteorite.transform.localScale = Vector3.one * meteoriteSize;

        yield return null;
    }
}
