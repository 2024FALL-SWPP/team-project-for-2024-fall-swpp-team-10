using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetManager : RotatingObject
{
    private GameObject player;
    protected PlayerControl playerControl;
    private bool isMagnet = false; // 자석 지속중인지 확인

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        player = GameObject.FindWithTag("Player");
        playerControl = player.GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!isMagnet)
            base.Update();
        else
        {
            transform.localScale = new Vector3(0, 0, 0);
            transform.position = playerControl.centerPosition;
        }
    }

    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);

        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Magnet());
        }
    }

    // 자석 아이템 효과
    IEnumerator Magnet()
    {
        float coinSpeed = 50f;

        if (isMagnet)
            yield break;

        isMagnet = true;

        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");

        while (true)
        {
            // 모든 GameObject가 null인지 확인
            bool allDestroyed = true;
            foreach (GameObject coin in coins)
            {
                if (coin != null) // 아직 Destroy되지 않은 객체가 있다면
                {
                    allDestroyed = false;
                    break;
                }
            }

            if (allDestroyed)
            {
                break;
            }

            foreach (GameObject coin in coins)
            {
                if (coin != null && coin.transform.position.z - playerControl.centerPosition.z < 80)
                {
                    coin.transform.position = Vector3.MoveTowards(coin.transform.position, playerControl.centerPosition, coinSpeed * Time.deltaTime);
                }
            }

            yield return null;
        }
        isMagnet = false;
        Destroy(gameObject);
    }
}
