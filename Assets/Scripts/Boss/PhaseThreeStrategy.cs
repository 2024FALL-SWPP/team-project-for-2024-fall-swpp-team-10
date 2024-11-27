using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseThreeStrategy : IMeteoriteDropStrategy
{
    private List<Vector3[]> patterns;

    public PhaseThreeStrategy(GridCell[,] gridCells)
    {
        patterns = new List<Vector3[]>();

        // 패턴 1: 각 모서리 가운데 그리드 4개 공격
        patterns.Add(new Vector3[]
        {
            gridCells[0,1].transform.position,
            gridCells[1,0].transform.position,
            gridCells[1,2].transform.position,
            gridCells[2,1].transform.position
        });

        // 패턴 2: 두 대각선의 합집합이 되는 그리드 5개 공격
        patterns.Add(new Vector3[]
        {
            gridCells[0,0].transform.position,
            gridCells[1,1].transform.position,
            gridCells[2,2].transform.position,
            gridCells[0,2].transform.position,
            gridCells[2,0].transform.position
        });

        // 패턴 3: L자 모양의 5개 그리드 공격
        patterns.Add(new Vector3[]
        {
            gridCells[0,0].transform.position,
            gridCells[0,1].transform.position,
            gridCells[0,2].transform.position,
            gridCells[1,2].transform.position,
            gridCells[2,2].transform.position
        });

        // 패턴 4: 첫번째와 세번째 가로/세로 줄 그리드 6개 공격
        patterns.Add(new Vector3[]
        {
            gridCells[0,0].transform.position,
            gridCells[0,1].transform.position,
            gridCells[0,2].transform.position,
            gridCells[2,0].transform.position,
            gridCells[2,1].transform.position,
            gridCells[2,2].transform.position
        });

        // 추가 패턴들
        patterns.Add(new Vector3[]
        {
            gridCells[0,0].transform.position,
            gridCells[1,0].transform.position,
            gridCells[2,0].transform.position,
            gridCells[2,1].transform.position,
            gridCells[2,2].transform.position
        });

        patterns.Add(new Vector3[]
        {
            gridCells[0,0].transform.position,
            gridCells[1,0].transform.position,
            gridCells[2,0].transform.position,
            gridCells[0,2].transform.position,
            gridCells[1,2].transform.position,
            gridCells[2,2].transform.position
        });
    }

    public IEnumerator Execute(BossAttackPattern bossAttackPattern)
    {
        // 플레이어 위치가 포함된 패턴 필터링
        List<Vector3[]> availablePatterns = new List<Vector3[]>();
        foreach (var pattern in patterns)
        {
            if (bossAttackPattern.IsPlayerInPattern(pattern))
            {
                availablePatterns.Add(pattern);
            }
        }

        if (availablePatterns.Count == 0)
            yield break;

        // 패턴 랜덤 선택
        int randomIndex = Random.Range(0, availablePatterns.Count);
        Vector3[] selectedPattern = availablePatterns[randomIndex];

        // 공격 실행
        bossAttackPattern.HighlightGridCells(selectedPattern, Color.green);
        yield return new WaitForSeconds(bossAttackPattern.warningDuration);
        bossAttackPattern.ResetGridCells(selectedPattern);

        foreach (Vector3 pos in selectedPattern)
        {
            bossAttackPattern.StartCoroutine(bossAttackPattern.ExecuteAttack(pos));
        }

        yield return new WaitForSeconds(2f);
    }
}