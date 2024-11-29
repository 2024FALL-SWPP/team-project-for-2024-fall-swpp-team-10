using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteDropStrategy
{
    protected List<Vector3[]> patterns;

    public virtual IEnumerator Execute(BossAttackPattern bossAttackPattern)
    {
        throw new System.NotImplementedException();
    }

    protected IEnumerator ExecuteCommon(BossAttackPattern bossAttackPattern, Color highlightColor)
    {
        List<Vector3[]> availablePatterns = new List<Vector3[]>();
        foreach (var pattern in patterns)
        {
            if (IsPlayerInPattern(bossAttackPattern, pattern))
            {
                availablePatterns.Add(pattern);
            }
        }

        if (availablePatterns.Count == 0)
            yield break;

        int randomIndex = Random.Range(0, availablePatterns.Count);
        Vector3[] selectedPattern = availablePatterns[randomIndex];

        bossAttackPattern.HighlightGridCells(selectedPattern, highlightColor);
        yield return new WaitForSeconds(bossAttackPattern.warningDuration);
        bossAttackPattern.ResetGridCells(selectedPattern);

        foreach (Vector3 pos in selectedPattern)
        {
            bossAttackPattern.StartCoroutine(bossAttackPattern.ExecuteAttack(pos));
        }

        yield return new WaitForSeconds(2f);
    }


    // 플레이어가 패턴에 포함되는지 확인하는 함수
    public bool IsPlayerInPattern(BossAttackPattern bossAttackPattern, Vector3[] pattern)
    {
        foreach (Vector3 cellPosition in pattern)
        {
            GridCell cell = bossAttackPattern.GetGridCellByPosition(cellPosition);
            if (cell != null && IsPlayerInCell(bossAttackPattern, cell))
            {
                return true;
            }
        }
        return false;
    }

    // 플레이어가 특정 그리드 셀에 있는지 확인하는 함수
    public bool IsPlayerInCell(BossAttackPattern bossAttackPattern, GridCell cell)
    {
        // 그리드 셀의 경계 계산
        Vector3 cellPosition = cell.transform.position;
        Vector3 cellScale = cell.transform.localScale;

        float halfSizeX = cellScale.x / 2f;
        float halfSizeZ = cellScale.z / 2f;

        float minX = cellPosition.x - halfSizeX;
        float maxX = cellPosition.x + halfSizeX;
        float minZ = cellPosition.z - halfSizeZ;
        float maxZ = cellPosition.z + halfSizeZ;

        Vector3 playerPos = bossAttackPattern.playerTransform.position;

        // 플레이어의 위치가 그리드 셀의 경계 안에 있는지 확인
        if (playerPos.x >= minX && playerPos.x <= maxX && playerPos.z >= minZ && playerPos.z <= maxZ)
        {
            return true;
        }
        return false;
    }
}
