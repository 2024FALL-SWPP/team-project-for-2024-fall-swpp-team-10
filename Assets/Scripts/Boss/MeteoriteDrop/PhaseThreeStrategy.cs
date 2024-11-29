using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseThreeStrategy : MeteoriteDropStrategy
{
    public PhaseThreeStrategy(GridCell[,] gridCells)
    {
        patterns = new List<Vector3[]>();

        // 패턴 1: 각 모서리 가운데 그리드 4개 공격
        patterns.Add(new Vector3[]
        {
            gridCells[0,1].transform.position,
            gridCells[1,0].transform.position,
            gridCells[1,2].transform.position,
            gridCells[2,1].transform.position
        });

        // 패턴 2: 두 대각선의 합집합이 되는 그리드 5개 공격
        patterns.Add(new Vector3[]
        {
            gridCells[0,0].transform.position,
            gridCells[1,1].transform.position,
            gridCells[2,2].transform.position,
            gridCells[0,2].transform.position,
            gridCells[2,0].transform.position
        });

        // 패턴 3: L자 모양의 5개 그리드 공격
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

        // 패턴 4: 첫번째와 세번째 가로/세로 줄 그리드 6개 공격
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