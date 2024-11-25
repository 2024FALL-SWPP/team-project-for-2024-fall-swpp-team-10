using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    protected GameObject player;
    protected PlayerControl playerControl;
    protected float speed = 22.0f;

    protected virtual void Awake()
    {
        player = GameObject.FindWithTag("Player");
        playerControl = player.GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        transform.Translate(new Vector3(0, 0, -1) * speed * Time.deltaTime, Space.World);

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

    protected virtual void OnPlayerCollision(GameObject player) {
        throw new NotImplementedException();
    }
}