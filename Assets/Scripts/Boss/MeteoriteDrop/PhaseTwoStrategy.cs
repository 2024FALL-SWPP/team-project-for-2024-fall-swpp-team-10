using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseTwoStrategy : MeteoriteDropStrategy
{
    public PhaseTwoStrategy(GridCell[,] gridCells)
    {
        patterns = new List<Vector3[]>();

        // 가로 줄 공격 패턴
        for (int x = 0; x < 3; x++)
        {
            Vector3[] rowPattern = new Vector3[3];
            for (int z = 0; z < 3; z++)
            {
                rowPattern[z] = gridCells[x, z].transform.position;
            }
            patterns.Add(rowPattern);
        }

        // 세로 줄 공격 패턴
        for (int z = 0; z < 3; z++)
        {
            Vector3[] columnPattern = new Vector3[3];
            for (int x = 0; x < 3; x++)
            {
                columnPattern[x] = gridCells[x, z].transform.position;
            }
            patterns.Add(columnPattern);
        }

        // 대각선 공격 패턴
        patterns.Add(new Vector3[] { gridCells[0, 0].transform.position, gridCells[1, 1].transform.position, gridCells[2, 2].transform.position });
        patterns.Add(new Vector3[] { gridCells[0, 2].transform.position, gridCells[1, 1].transform.position, gridCells[2, 0].transform.position });
    }

    public override IEnumerator Execute(BossAttackPattern bossAttackPattern)
    {
        return ExecuteCommon(bossAttackPattern, Color.blue);
    }
}