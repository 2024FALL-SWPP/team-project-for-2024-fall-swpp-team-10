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

    // 경고 표시를 위한 색상 변경
    public void Highlight(Color color)
    {
        cellRenderer.material.color = color;
    }

    // 원래 색상으로 복원
    public void ResetColor()
    {
        cellRenderer.material.color = originalColor;
    }
}

