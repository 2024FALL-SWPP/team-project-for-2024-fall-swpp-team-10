using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    private float speed = 22.0f;
    private float rotationSpeed = 500.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, 0, -1) * speed * Time.deltaTime, Space.World);

        if (!gameObject.CompareTag("Enemy") && !gameObject.CompareTag("Obstacle")) //if it is item or coin
        {
            transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0), Space.World);
        }

        if (transform.position.z < -10)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Obstacle"))
        {
            if (transform.position.z > 10)
                Destroy(gameObject);
        }
    }
}
