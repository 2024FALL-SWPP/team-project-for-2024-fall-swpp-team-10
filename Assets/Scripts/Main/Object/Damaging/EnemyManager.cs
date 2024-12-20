using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumManager;

public class EnemyManager : DamagingObject
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        score = 1000;
    }

    protected override void OnPlayerCollision(GameObject player)
    {
        if (playerControl.GetIsInvincible())
        {
            GameManager.inst.AddEnemyKill();
        }
        base.OnPlayerCollision(player);
    }
}
