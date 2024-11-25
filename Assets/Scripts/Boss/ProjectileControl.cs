using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileControl : MonoBehaviour
{
    Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        Physics.IgnoreLayerCollision(7, 8);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsObjectInView(this.transform.position))
        {
            Destroy(gameObject);
        }
    }

    bool IsObjectInView(Vector3 objectPosition)
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(objectPosition);

        return viewportPosition.x >= 0 && viewportPosition.x <= 1 &&
               viewportPosition.y >= 0 && viewportPosition.y <= 1 &&
               viewportPosition.z > 0;
    }
}
