using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    protected GameObject player;
    protected MainStagePlayer playerControl;
    protected Rigidbody objectRb;
    protected float speed = 22.0f;

    protected virtual void Awake()
    {
        player = GameObject.FindWithTag("Player");
        playerControl = player.GetComponent<MainStagePlayer>();
        objectRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        objectRb.velocity = Vector3.back * speed;

        if (transform.position.z < -100)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnPlayerCollision(other.gameObject);
        }
    }

    protected virtual void OnPlayerCollision(GameObject player)
    {
        throw new NotImplementedException();
    }
}
