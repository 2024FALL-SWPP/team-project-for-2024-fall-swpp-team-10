using UnityEngine;
using System.Collections;

public abstract class PlayerBase : MonoBehaviour
{
    protected Renderer[] childRenderers; //Renderer of characters
    protected Color[,] originColors; // Origin color of characters
    protected int blinkCount = 3; // �ǰ� �� �����̴� Ƚ��
    protected bool isInvincible = false; // ���� ���������� Ȯ��
    protected bool isBlinking = false; // �����̴������� Ȯ��
    public float invincibleLength; // ���� ���� �ð�

    [Header("Audio Settings")]
    public AudioClip laserFireSound;
    [Range(0f, 1f)] public float laserVolume = 0.7f;

    protected virtual void Awake()
    {
        childRenderers = GetComponentsInChildren<Renderer>();

        int maxSharedMaterialsLength = 0;
        for (int i = 0; i < childRenderers.Length; i++)
        {
            maxSharedMaterialsLength = Mathf.Max(maxSharedMaterialsLength, childRenderers[i].sharedMaterials.Length);
        }
        originColors = new Color[childRenderers.Length, maxSharedMaterialsLength];

        for (int i = 0; i < childRenderers.Length; i++)
        {
            for (int j = 0; j < childRenderers[i].sharedMaterials.Length; j++)
            {
                if (childRenderers[i].sharedMaterials[j].HasProperty("_Color"))
                    originColors[i, j] = childRenderers[i].sharedMaterials[j].color;
            }
        }

        if (GameManager.inst.originColorSave == null)
            GameManager.inst.originColorSave = originColors;
    }

    public void ChangeColor(Color color)
    {
        foreach (Renderer renderer in childRenderers)
        {
            foreach (Material material in renderer.sharedMaterials)
                material.color = color;
        }
    }

    public void ChangeColorOriginal()
    {
        for (int i = 0; i < childRenderers.Length; i++)
        {
            for (int j = 0; j < childRenderers[i].sharedMaterials.Length; j++)
            {
                childRenderers[i].sharedMaterials[j].color = GameManager.inst.originColorSave[i, j];
            }
        }
    }

    public IEnumerator Blink()
    {
        isBlinking = true;
        isInvincible = true;
        for (int i = 0; i < blinkCount; i++)
        {
            ChangeColor(Color.red);
            yield return new WaitForSeconds(0.2f);

            ChangeColorOriginal();
            yield return new WaitForSeconds(0.2f);
        }
        isBlinking = false;
        isInvincible = false;
    }

    public void SetIsInvincible(bool _isInvincible)
    {
        isInvincible = _isInvincible;
    }
    public bool GetIsInvincible()
    {
        return isInvincible;
    }

    public bool GetIsBlinking()
    {
        return isBlinking;
    }

    protected abstract void FireLaser();

    protected virtual void OnCollisionEnter(Collision other)
    {
    }
}
