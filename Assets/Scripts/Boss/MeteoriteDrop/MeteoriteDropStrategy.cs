using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteDropStrategy
{

    protected List<Vector3[]> patterns;
    protected Color gridCellWarningColor;

    void Start()
    {
        Random.InitState(System.Environment.TickCount);
    }

    public IEnumerator Execute(MeteoriteDropManager meteoriteDropManager)
    {
        List<Vector3[]> availablePatterns = new List<Vector3[]>();
        foreach (var pattern in patterns)
        {
            if (IsPlayerInPattern(meteoriteDropManager, pattern))
            {
                availablePatterns.Add(pattern);
            }
        }

        if (availablePatterns.Count == 0)
            yield break;

        int randomIndex = Random.Range(0, availablePatterns.Count);
        Vector3[] selectedPattern = availablePatterns[randomIndex];

        meteoriteDropManager.HighlightGridCells(selectedPattern, gridCellWarningColor);
        yield return new WaitForSeconds(meteoriteDropManager.warningDuration);
        meteoriteDropManager.ResetGridCells(selectedPattern);

        foreach (Vector3 pos in selectedPattern)
        {
            meteoriteDropManager.StartCoroutine(meteoriteDropManager.ExecuteAttack(pos));
        }

        yield return new WaitForSeconds(2f);
    }


    // �÷��̾ ���Ͽ� ���ԵǴ��� Ȯ���ϴ� �Լ�
    public bool IsPlayerInPattern(MeteoriteDropManager meteoriteDropManager, Vector3[] pattern)
    {
        foreach (Vector3 cellPosition in pattern)
        {
            GridCell cell = meteoriteDropManager.GetGridCellByPosition(cellPosition);
            if (cell != null && IsPlayerInCell(meteoriteDropManager, cell))
            {
                return true;
            }
        }
        return false;
    }

    // �÷��̾ Ư�� �׸��� ���� �ִ��� Ȯ���ϴ� �Լ�
    public bool IsPlayerInCell(MeteoriteDropManager meteoriteDropManager, GridCell cell)
    {
        // �׸��� ���� ��� ���
        Vector3 cellPosition = cell.transform.position;
        Vector3 cellScale = cell.transform.localScale;

        float halfSizeX = cellScale.x / 2f;
        float halfSizeZ = cellScale.z / 2f;

        float minX = cellPosition.x - halfSizeX;
        float maxX = cellPosition.x + halfSizeX;
        float minZ = cellPosition.z - halfSizeZ;
        float maxZ = cellPosition.z + halfSizeZ;

        Vector3 playerPos = meteoriteDropManager.playerTransform.position;

        // �÷��̾��� ��ġ�� �׸��� ���� ��� �ȿ� �ִ��� Ȯ��
        if (playerPos.x >= minX && playerPos.x <= maxX && playerPos.z >= minZ && playerPos.z <= maxZ)
        {
            return true;
        }
        return false;
    }
}
