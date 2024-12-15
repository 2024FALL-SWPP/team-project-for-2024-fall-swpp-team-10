using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartManager : BeneficialObject
{
    [Header("Sound System")]
    [SerializeField] public AudioClip healSound;
    [SerializeField][Range(0f, 1f)] public float volume = 0.5f;

    protected override void OnPlayerCollision(GameObject player)
    {
        if (healSound != null)
        {
            AudioSource.PlayClipAtPoint(healSound, transform.position, volume);
        }
        GameManager.inst.AddLife(GameManager.inst.GetMaxLife());
        Destroy(gameObject);
    }
}
