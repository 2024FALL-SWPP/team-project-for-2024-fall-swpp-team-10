using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartManager : BeneficialObject
{
    [Header("Sound System")]
    [SerializeField] public AudioClip healSound;
    [SerializeField][Range(0f, 1f)] public float volume = 0.5f;
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
        if (healSound != null)
        {
            AudioSource.PlayClipAtPoint(healSound, transform.position, volume);
        }
        GameManager.inst.AddLife(GameManager.inst.maxLife);
        Destroy(gameObject);
    }
}
