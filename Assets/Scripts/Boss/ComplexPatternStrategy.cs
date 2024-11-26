using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexPatternStrategy : IMeteoriteDropStrategy
{
    private Vector3[] patternPositions;

    public ComplexPatternStrategy(Vector3[] positions)
    {
        patternPositions = positions;
    }

    public IEnumerator Execute(BossAttackPattern bossAttackPattern)
    {
        // 플레이어가 포함된 패턴인지 확인
        if (!bossAttackPattern.IsPlayerInPattern(patternPositions))
            yield break;

        // 그리드 셀 강조
        bossAttackPattern.HighlightGridCells(patternPositions, Color.green);

        // 경고 시간 대기
        yield return new WaitForSeconds(bossAttackPattern.warningDuration);

        // 강조 해제 및 운석 낙하
        bossAttackPattern.ResetGridCells(patternPositions);

        foreach (Vector3 pos in patternPositions)
        {
            bossAttackPattern.StartCoroutine(bossAttackPattern.ExecuteAttack(pos));
        }

        // 다음 공격까지 대기
        yield return new WaitForSeconds(2f);
    }

    public Vector3[] GetPatternPositions()
    {
        return patternPositions;
    }
}
