using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingObject : ObjectManager // enemy, obstacle
{
    protected GameObject player;
    protected PlayerControl playerControl;
    protected int score;
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
            OnPlayerCollision(other.gameObject);
        }
    }
    protected virtual void OnPlayerCollision(GameObject player)
    {
        if (playerControl.isInvincible)
        {
            GameManager.inst.AddScore(score);
            Destroy(gameObject);
            return;
        }
        GameManager.inst.AddScore(score * -1);
        GameManager.inst.RemoveLife();
        StartCoroutine(playerControl.Blink());
    }
}
