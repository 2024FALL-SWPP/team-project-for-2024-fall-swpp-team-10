// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    public GameObject[] objects;

    public float startSpawning = 0f; // 소환 시작 시간
    public float spawnInterval = 2.0f; // 소환 간격
    public float reduceGap = 0.01f; // 소환 간격이 줄어드는 간격
    public float minSpawnInterval = 0.3f; // 최소 소환 간격

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(startSpawning);
        while (true)
        {
            int spawnRandom = Random.Range(0, objects.Length);
            int xRandom = Random.Range(0, 3) - 1;
            int yRandom = Random.Range(0, 3) - 1;

            Vector3 spawnPosition = new Vector3(xRandom, yRandom, 0);
            GameObject spawnedObject = objects[spawnRandom];

            Instantiate(spawnedObject, spawnedObject.transform.position + transform.position + spawnPosition, spawnedObject.transform.rotation);
            yield return new WaitForSeconds(spawnInterval);
            if (spawnInterval >= minSpawnInterval)
            {
                spawnInterval -= reduceGap;
            }
        }
    }
}
