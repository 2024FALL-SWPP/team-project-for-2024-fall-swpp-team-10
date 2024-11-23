using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : DamagingObject
{
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
        if (playerControl.isInvincible)
        {
            GameManager.inst.AddScore(1000); // 무적 상태에서 적 부딪하면 1000점 추가
        }
        base.OnCollisionEnter(other);
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.inst.AddScore(-1000);
        }
    }
}
