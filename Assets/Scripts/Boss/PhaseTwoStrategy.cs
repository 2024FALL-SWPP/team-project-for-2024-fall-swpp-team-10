using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseTwoStrategy : IMeteoriteDropStrategy
{
    private List<Vector3[]> patterns;

    public PhaseTwoStrategy(GridCell[,] gridCells)
    {
        patterns = new List<Vector3[]>();

        // ���� �� ���� ����
        for (int x = 0; x < 3; x++)
        {
            Vector3[] rowPattern = new Vector3[3];
            for (int z = 0; z < 3; z++)
            {
                rowPattern[z] = gridCells[x, z].transform.position;
            }
            patterns.Add(rowPattern);
        }

        // ���� �� ���� ����
        for (int z = 0; z < 3; z++)
        {
            Vector3[] columnPattern = new Vector3[3];
            for (int x = 0; x < 3; x++)
            {
                columnPattern[x] = gridCells[x, z].transform.position;
            }
            patterns.Add(columnPattern);
        }

        // �밢�� ���� ����
        patterns.Add(new Vector3[] { gridCells[0, 0].transform.position, gridCells[1, 1].transform.position, gridCells[2, 2].transform.position });
        patterns.Add(new Vector3[] { gridCells[0, 2].transform.position, gridCells[1, 1].transform.position, gridCells[2, 0].transform.position });
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
        bossAttackPattern.HighlightGridCells(selectedPattern, Color.blue);
        yield return new WaitForSeconds(bossAttackPattern.warningDuration);
        bossAttackPattern.ResetGridCells(selectedPattern);

        foreach (Vector3 pos in selectedPattern)
        {
            bossAttackPattern.StartCoroutine(bossAttackPattern.ExecuteAttack(pos));
        }

        yield return new WaitForSeconds(2f);
    }
}