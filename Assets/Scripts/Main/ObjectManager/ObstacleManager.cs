using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : DamagingObject
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
            GameManager.inst.AddScore(500); // 무적 상태에서 장애물 부딪하면 500점 추가
        }
        base.OnCollisionEnter(other);
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.inst.AddScore(-500);
        }
    }
}
