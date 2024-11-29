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
        // �÷��̾ ���Ե� �������� Ȯ��
        if (!bossAttackPattern.IsPlayerInPattern(patternPositions))
            yield break;

        // �׸��� �� ����
        bossAttackPattern.HighlightGridCells(patternPositions, Color.green);

        // ��� �ð� ���
        yield return new WaitForSeconds(bossAttackPattern.warningDuration);

        // ���� ���� �� � ����
        bossAttackPattern.ResetGridCells(patternPositions);

        foreach (Vector3 pos in patternPositions)
        {
            bossAttackPattern.StartCoroutine(bossAttackPattern.ExecuteAttack(pos));
        }

        // ���� ���ݱ��� ���
        yield return new WaitForSeconds(2f);
    }

    public Vector3[] GetPatternPositions()
    {
        return patternPositions;
    }
}
