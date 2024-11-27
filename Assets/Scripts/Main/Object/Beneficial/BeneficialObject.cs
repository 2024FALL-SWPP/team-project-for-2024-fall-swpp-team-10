using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BeneficialObject : ObjectManager //coin, item
{
    protected float rotationSpeed = 500.0f;

    protected override void Update()
    {
        base.Update();
        transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0), Space.World);
    }

    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Obstacle"))
        {
            if (transform.position.z > 98)
                Destroy(gameObject);
        }
    }

    protected void HideAndKeep()
    {
        transform.localScale = new Vector3(0, 0, 0);
        transform.position = playerControl.centerPosition - new Vector3(0, 0, -10);
    }
}
