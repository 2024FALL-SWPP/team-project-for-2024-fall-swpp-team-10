using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : BeneficialObject
{
    [Header("Audio Settings")]
    [SerializeField] public AudioClip coinCollectSound;
    [SerializeField][Range(0f, 1f)] public float coinVolume = 0.5f;

    protected override void OnPlayerCollision(GameObject player)
    {
        base.OnPlayerCollision(player);
        if (coinCollectSound != null)
        {
            AudioSource.PlayClipAtPoint(coinCollectSound, playerControl.centerPosition, coinVolume);
        }
        GameManager.inst.AddScore(200);
        Destroy(gameObject);
    }
}
