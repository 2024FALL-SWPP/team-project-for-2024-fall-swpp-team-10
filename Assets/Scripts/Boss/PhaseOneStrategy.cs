using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseOneStrategy : IMeteoriteDropStrategy
{
    private List<Vector3[]> patterns;

    public PhaseOneStrategy(GridCell[,] gridCells)
    {
        patterns = new List<Vector3[]>();

        // 그리드 셀의 위치를 가져와서 패턴 정의
        for (int x = 0; x < 3; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                patterns.Add(new Vector3[] { gridCells[x, z].transform.position });
            }
        }
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
        bossAttackPattern.HighlightGridCells(selectedPattern, Color.red);
        yield return new WaitForSeconds(bossAttackPattern.warningDuration);
        bossAttackPattern.ResetGridCells(selectedPattern);

        foreach (Vector3 pos in selectedPattern)
        {
            bossAttackPattern.StartCoroutine(bossAttackPattern.ExecuteAttack(pos));
        }

        yield return new WaitForSeconds(2f);
    }
}