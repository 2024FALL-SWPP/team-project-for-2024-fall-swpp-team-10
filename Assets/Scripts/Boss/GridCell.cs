using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    private Renderer cellRenderer;
    private Color originalColor;

    void Awake()
    {
        cellRenderer = GetComponent<Renderer>();
        originalColor = cellRenderer.material.color;
    }

    // ��� ǥ�ø� ���� ���� ����
    public void Highlight(Color color)
    {
        cellRenderer.material.color = color;
    }

    // ���� �������� ����
    public void ResetColor()
    {
        cellRenderer.material.color = originalColor;
    }
}

