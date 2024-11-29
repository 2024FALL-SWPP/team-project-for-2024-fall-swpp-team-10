using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseThreeStrategy : MeteoriteDropStrategy
{
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
            gridCells[2,0].transform.position,
            gridCells[1,0].transform.position,
            gridCells[0,0].transform.position,
            gridCells[0,1].transform.position,
            gridCells[0,2].transform.position
        });
        patterns.Add(new Vector3[]
        {
            gridCells[2,0].transform.position,
            gridCells[2,1].transform.position,
            gridCells[2,2].transform.position,
            gridCells[1,2].transform.position,
            gridCells[0,2].transform.position
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

    public override IEnumerator Execute(BossAttackPattern bossAttackPattern)
    {
        return ExecuteCommon(bossAttackPattern, Color.green);
    }
}