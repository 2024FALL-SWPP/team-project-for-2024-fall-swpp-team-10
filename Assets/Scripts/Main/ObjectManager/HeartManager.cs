using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartManager : RotatingObject
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
        base.OnCollisionEnter(other);
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.inst.AddLife(GameManager.inst.maxLife);
            Destroy(gameObject);
        }
    }
}
