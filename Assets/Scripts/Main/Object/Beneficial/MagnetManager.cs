using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetManager : BeneficialObject
{
    private bool isMagnet = false; // 자석 지속중인지 확인
    private float coinSpeed = 50f;

    private float positionZ(GameObject gameObject)
    {
        return gameObject.transform.position.z;
    }

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!isMagnet)
            base.Update();
        else // 아이템이 파괴되면 coroutine이 시작하지 않기 때문에 플레이어 안에 작게 숨겨두게 변경
        {
            HideAndKeep();
        }
    }

    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
    }

    protected override void OnPlayerCollision(GameObject player)
    {
        base.OnPlayerCollision(player);
        StartCoroutine(Magnet());
    }

    // 자석 아이템 효과
    IEnumerator Magnet()
    {
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
                if (coin != null && -6 < positionZ(coin) && positionZ(coin) < 80)
                {
                    coin.transform.position = Vector3.MoveTowards(coin.transform.position, playerControl.centerPosition, coinSpeed * Time.deltaTime);
                }
            }

            yield return null;
        }
        Destroy(gameObject);
    }
}
