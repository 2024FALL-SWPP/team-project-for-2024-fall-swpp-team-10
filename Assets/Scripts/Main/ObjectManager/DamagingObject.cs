using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingObject : ObjectManager // enemy, obstacle
{
    protected GameObject player;
    protected PlayerControl playerControl;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindWithTag("Player");
        playerControl = player.GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
        if (other.gameObject.CompareTag("Player"))
        {
            if (playerControl.isInvincible)
            {
                Destroy(gameObject);
                return;
            }
            GameManager.inst.RemoveLife();
            StartCoroutine(playerControl.Blink());
        }
    }
}
