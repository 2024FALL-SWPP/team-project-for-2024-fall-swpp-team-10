using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    private float speed = 20.0f;
    public int numbersOfMapPrefab;
    public float standardZPosition;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
        if (transform.position.z < -standardZPosition)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, standardZPosition * (numbersOfMapPrefab - 1));
        }
    }
}
