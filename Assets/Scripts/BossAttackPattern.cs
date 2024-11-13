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
    //TODO: BossStagePlayer 스크립트의 constant와 일원화하기
    public Vector3 areaMin = new Vector3(-10f, 0.5f, -10f); // 플레이어 이동 영역 최소값   
    public Vector3 areaMax = new Vector3(10f, 0.5f, 10f);   // 플레이어 이동 영역 최대값

    [Header("Attack Settings")]
    public float warningDuration = 1f; // 경고 시간
    public float meteoriteHeight = 15f; // 운석이 떨어지는 높이
    public float bossHealth = 100f; // 보스의 생명력

    private int currentPhase = 0; // 현재 Phase
    private int currentPatternIndex = 0; // 현재 패턴 인덱스
    private List<List<Vector3[]>> attackPatternsPerPhase; // Phase별 공격 패턴
    private bool isAttacking = false;

    private GridCell[,] gridCells = new GridCell[3, 3]; // 3x3 그리드 셀 배열
    private float cellSizeX;
    private float cellSizeZ;
    private float meteoriteSize; // 운석 크기

    void Start()
    {
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
                cell.transform.localScale = new Vector3(cellSizeX, 0.1f, cellSizeZ); // 셀 크기 조정

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

        // Phase 1 패턴: 중앙 그리드 공격
        List<Vector3[]> phase1Patterns = new List<Vector3[]> {
            new Vector3[] { gridPositions[1, 1] }
        };
        attackPatternsPerPhase.Add(phase1Patterns);

        // Phase 2 패턴: 가운데 세로줄과 대각선 공격을 번갈아 실행 -> 패턴 1: 가운데 세로줄 전체 공격 / 패턴 2: 대각선 공격
        List<Vector3[]> phase2Patterns = new List<Vector3[]> {
            new Vector3[] { gridPositions[1, 0], gridPositions[1, 1], gridPositions[1, 2] },
            new Vector3[] { gridPositions[0, 0], gridPositions[1, 1], gridPositions[2, 2] }
        };

        attackPatternsPerPhase.Add(phase2Patterns);

        // Phase 3 패턴: 네 가지 패턴을 번갈아 실행 -> 패턴 1: 각 모서리 가운데 그리드 4개 공격 / 패턴 2: 두 대각선의 합집합이 되는 그리드 5개 공격 / 패턴 3: L자 모양의 5개 그리드 공격 / 패턴 4: 첫번째와 세번째 세로줄 그리드 6개 공격
        List<Vector3[]> phase3Patterns = new List<Vector3[]> {
            new Vector3[] { gridPositions[0, 1], gridPositions[1, 0], gridPositions[1, 2], gridPositions[2, 1] },
            new Vector3[] { gridPositions[0, 0], gridPositions[1, 1], gridPositions[2, 2], gridPositions[0, 2], gridPositions[2, 0] },
            new Vector3[] { gridPositions[0, 0], gridPositions[0, 1], gridPositions[0, 2], gridPositions[1, 2], gridPositions[2, 2] },
            new Vector3[] { gridPositions[0, 0], gridPositions[0, 1], gridPositions[0, 2], gridPositions[2, 0], gridPositions[2, 1], gridPositions[2, 2] }
        };

        attackPatternsPerPhase.Add(phase3Patterns);
    }

    // 현재 Phase 업데이트
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

    // 공격 시퀀스 실행
    IEnumerator AttackSequence()
    {
        while (bossHealth > 0)
        {
            if (!isAttacking)
            {
                isAttacking = true;

                // 현재 Phase 업데이트
                UpdatePhase();

                // 현재 Phase의 패턴 리스트 가져오기
                List<Vector3[]> patterns = attackPatternsPerPhase[currentPhase];

                // 현재 패턴 인덱스에 따른 패턴 선택
                Vector3[] currentPattern = patterns[currentPatternIndex];

                // 패턴 인덱스 업데이트 (다음 패턴을 위해)
                currentPatternIndex = (currentPatternIndex + 1) % patterns.Count;

                // 공격할 그리드 셀 강조 (빨간색)
                HighlightGridCells(currentPattern, Color.red);

                // 경고 시간 대기
                yield return new WaitForSeconds(warningDuration);

                // 강조 해제 및 운석 공격 실행
                ResetGridCells(currentPattern);

                foreach (Vector3 attackPos in currentPattern)
                {
                    StartCoroutine(ExecuteAttack(attackPos));
                }

                // 다음 공격까지 대기 (조정 가능)
                yield return new WaitForSeconds(2f); // 예: 2초 대기
                isAttacking = false;
            }

            yield return null;
        }

        // 보스가 사망했을 때 처리 (필요 시 추가)
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

    void Update()
    {
        // 실제 게임에서는 보스의 생명력이 게임 이벤트에 따라 감소해야 합니다.
        // 여기서는 테스트를 위해 시간이 지남에 따라 감소시킵니다.
        if (bossHealth > 0)
        {
            bossHealth -= Time.deltaTime * 2f; // ex)초당 2씩 감소
            if (bossHealth < 0f)
            {
                bossHealth = 0;
                // 보스 사망 처리 (필요 시 추가)
            }
        }
    }
}
