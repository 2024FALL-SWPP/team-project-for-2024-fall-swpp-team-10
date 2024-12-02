using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRepositionWeakspot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        // Make sure weakspot is not buried under boss mesh
        if (collision.gameObject.CompareTag("WeakSpot"))
        {
            AdjustWeakSpot(collision.gameObject);
        }
    }

    // Make sure weakspot is not buried under boss mesh (Called on collision btwn boss and weak spot)
    void AdjustWeakSpot(GameObject weakSpot)
    {
        weakSpot.transform.localPosition += (Vector3.forward * 0.1f);
    }
}
