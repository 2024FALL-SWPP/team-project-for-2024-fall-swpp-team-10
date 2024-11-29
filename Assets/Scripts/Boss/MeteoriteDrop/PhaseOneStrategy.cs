using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseOneStrategy : MeteoriteDropStrategy
{
    public PhaseOneStrategy(GridCell[,] gridCells)
    {
        patterns = new List<Vector3[]>();
        gridCellWarningColor = new Color(1f, 0.416f, 0f); //��Ȳ��

        // �׸��� ���� ��ġ�� �����ͼ� ���� ����
        for (int x = 0; x < 3; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                patterns.Add(new Vector3[] { gridCells[x, z].transform.position });
            }
        }
    }
}