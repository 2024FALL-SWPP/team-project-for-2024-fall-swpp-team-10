using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : DamagingObject
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        score = 1000;
        isDestroyable = true;
    }
}
