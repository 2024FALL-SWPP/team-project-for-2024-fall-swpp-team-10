using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    private float speed = 20.0f;
    public int numbersOfMapPrefab;
    public float standardZPosition;

    private MainManager mainManager;

    // Start is called before the first frame update
    void Awake()
    {
        mainManager = FindObjectOfType<MainManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mainManager.IsStageComplete())
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
            if (transform.position.z < -standardZPosition)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, standardZPosition * (numbersOfMapPrefab - 1));
            }
        }
    }
}
