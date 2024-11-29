using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseThreeStrategy : IMeteoriteDropStrategy
{
    private List<Vector3[]> patterns;

    public PhaseThreeStrategy(GridCell[,] gridCells)
    {
        patterns = new List<Vector3[]>();

        // ���� 1: �� �𼭸� ��� �׸��� 4�� ����
        patterns.Add(new Vector3[]
        {
            gridCells[0,1].transform.position,
            gridCells[1,0].transform.position,
            gridCells[1,2].transform.position,
            gridCells[2,1].transform.position
        });

        // ���� 2: �� �밢���� �������� �Ǵ� �׸��� 5�� ����
        patterns.Add(new Vector3[]
        {
            gridCells[0,0].transform.position,
            gridCells[1,1].transform.position,
            gridCells[2,2].transform.position,
            gridCells[0,2].transform.position,
            gridCells[2,0].transform.position
        });

        // ���� 3: L�� ����� 5�� �׸��� ����
        patterns.Add(new Vector3[]
        {
            gridCells[0,0].transform.position,
            gridCells[0,1].transform.position,
            gridCells[0,2].transform.position,
            gridCells[1,2].transform.position,
            gridCells[2,2].transform.position
        });

        // ���� 4: ù��°�� ����° ����/���� �� �׸��� 6�� ����
        patterns.Add(new Vector3[]
        {
            gridCells[0,0].transform.position,
            gridCells[0,1].transform.position,
            gridCells[0,2].transform.position,
            gridCells[2,0].transform.position,
            gridCells[2,1].transform.position,
            gridCells[2,2].transform.position
        });

        // �߰� ���ϵ�
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
        // �÷��̾� ��ġ�� ���Ե� ���� ���͸�
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

        // ���� ���� ����
        int randomIndex = Random.Range(0, availablePatterns.Count);
        Vector3[] selectedPattern = availablePatterns[randomIndex];

        // ���� ����
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