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

    private MeteoriteDropStrategy currentStrategy; // 현재 Phase의 Strategy
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
        UpdateNewStrategy();

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
    void UpdateNewStrategy()
    {
        int currentPhase = bossStageManager.GetPhase(); // Phase indexing: 0-base
        switch (currentPhase)
        {
            case 0:
                currentStrategy = new PhaseOneStrategy(gridCells);
                break;
            case 1:
                currentStrategy = new PhaseTwoStrategy(gridCells);
                break;
            case 2:
                currentStrategy = new PhaseThreeStrategy(gridCells);
                break;
            default:
                Debug.LogWarning("Invalid Phase");
                break;
        }
    }


    // 공격 시퀀스 실행
    IEnumerator AttackSequence()
    {
        int lastPhase = bossStageManager.GetPhase(); // 초기 Phase 저장

        while (bossStageManager.GetBossLife() > 0)
        {
            if (!isAttacking)
            {
                isAttacking = true;

                // 현재 Phase 확인
                int currentPhase = bossStageManager.GetPhase();

                // Phase가 변경되었는지 확인
                if (currentPhase != lastPhase)
                {
                    UpdateNewStrategy(); // 새로운 전략 갱신
                    lastPhase = currentPhase; // 마지막 Phase 업데이트
                }

                // 현재 Phase의 전략 실행
                yield return StartCoroutine(currentStrategy.Execute(this));

                isAttacking = false;
            }

            yield return null;
        }


        // 보스가 사망했을 때 처리 (필요 시 추가)
    }

    // 그리드 셀 강조 기능 함수
    public void HighlightGridCells(Vector3[] positions, Color color)
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
    public void ResetGridCells(Vector3[] positions)
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
    public GridCell GetGridCellByPosition(Vector3 position)
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
    public IEnumerator ExecuteAttack(Vector3 position)
    {
        // 운석 생성 위치: 그리드 셀의 중심에서 meteoriteHeight 높이 위
        Vector3 spawnPosition = new Vector3(position.x, areaMin.y + meteoriteHeight, position.z - 2.5f);
        GameObject meteorite = Instantiate(meteoritePrefab, spawnPosition, Quaternion.identity);
        meteorite.transform.localScale = Vector3.one * meteoriteSize;

        yield return null;
    }
}
