using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightstickManager : BeneficialObject
{

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnPlayerCollision(GameObject player)
    {
        Debug.Log("OnPlayerCollision still not implemented in LightStickManager");
    }
}
