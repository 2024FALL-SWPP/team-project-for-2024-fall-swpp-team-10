using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropAttackStrategy
{

    protected List<Vector3[]> patterns;
    protected Color gridCellWarningColor;

    void Start()
    {
        Random.InitState(System.Environment.TickCount);
    }

    public IEnumerator Execute(DropAttackManager dropAttackManager)
    {
        List<Vector3[]> availablePatterns = new List<Vector3[]>();
        foreach (var pattern in patterns)
        {
            if (IsPlayerInPattern(dropAttackManager, pattern))
            {
                availablePatterns.Add(pattern);
            }
        }

        if (availablePatterns.Count == 0)
            yield break;

        int randomIndex = Random.Range(0, availablePatterns.Count);
        Vector3[] selectedPattern = availablePatterns[randomIndex];

        dropAttackManager.HighlightGridCells(selectedPattern, gridCellWarningColor);
        yield return new WaitForSeconds(dropAttackManager.warningDuration);
        dropAttackManager.ResetGridCells(selectedPattern);

        foreach (Vector3 pos in selectedPattern)
        {
            dropAttackManager.StartCoroutine(dropAttackManager.ExecuteAttack(pos));
        }

        yield return new WaitForSeconds(2f);
    }


    // �÷��̾ ���Ͽ� ���ԵǴ��� Ȯ���ϴ� �Լ�
    public bool IsPlayerInPattern(DropAttackManager dropAttackManager, Vector3[] pattern)
    {
        foreach (Vector3 cellPosition in pattern)
        {
            GridCell cell = dropAttackManager.GetGridCellByPosition(cellPosition);
            if (cell != null && IsPlayerInCell(dropAttackManager, cell))
            {
                return true;
            }
        }
        return false;
    }

    // �÷��̾ Ư�� �׸��� ���� �ִ��� Ȯ���ϴ� �Լ�
    public bool IsPlayerInCell(DropAttackManager dropAttackManager, GridCell cell)
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

        Vector3 playerPos = dropAttackManager.GetPlayerPosition();

        // �÷��̾��� ��ġ�� �׸��� ���� ��� �ȿ� �ִ��� Ȯ��
        if (playerPos.x >= minX && playerPos.x <= maxX && playerPos.z >= minZ && playerPos.z <= maxZ)
        {
            return true;
        }
        return false;
    }
}
