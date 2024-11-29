using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowColumnPatternStrategy : IMeteoriteDropStrategy
{
    private Vector3[] patternPositions;

    public RowColumnPatternStrategy(Vector3[] positions)
    {
        patternPositions = positions;
    }

    public IEnumerator Execute(BossAttackPattern bossAttackPattern)
    {
        // �÷��̾ ���Ե� �������� Ȯ��
        if (!bossAttackPattern.IsPlayerInPattern(patternPositions))
            yield break;

        // �׸��� �� ����
        bossAttackPattern.HighlightGridCells(patternPositions, Color.blue);

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