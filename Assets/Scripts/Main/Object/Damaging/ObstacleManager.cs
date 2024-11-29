using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : DamagingObject
{
    protected float rotationSpeed = 200.0f;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        score = 500;
    }

    protected override void Update()
    {
        base.Update();
        transform.Rotate(new Vector3(-rotationSpeed * Time.deltaTime, 0, 0), Space.World);
    }
}
