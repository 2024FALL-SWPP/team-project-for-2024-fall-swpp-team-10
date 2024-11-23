using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    protected float speed = 22.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        transform.Translate(new Vector3(0, 0, -1) * speed * Time.deltaTime, Space.World);

        if (transform.position.z < -10)
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnCollisionEnter(Collision other) { }
}
