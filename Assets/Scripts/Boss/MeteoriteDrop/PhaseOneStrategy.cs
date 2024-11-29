using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseOneStrategy : MeteoriteDropStrategy
{
    public PhaseOneStrategy(GridCell[,] gridCells)
    {
        patterns = new List<Vector3[]>();
        gridCellWarningColor = new Color(1f, 0.416f, 0f); //주황색

        // 그리드 셀의 위치를 가져와서 패턴 정의
        for (int x = 0; x < 3; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                patterns.Add(new Vector3[] { gridCells[x, z].transform.position });
            }
        }
    }
}