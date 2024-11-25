using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : BeneficialObject
{
    [Header("Audio Settings")]
    [SerializeField] public AudioClip coinCollectSound;
    [SerializeField][Range(0f, 1f)] public float coinVolume = 0.5f;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
    }

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
