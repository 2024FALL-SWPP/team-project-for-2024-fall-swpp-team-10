using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibleManager : BeneficialObject
{
    private bool isInvincibleItemPlaying = false;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!isInvincibleItemPlaying)
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
        StartCoroutine(Invincible());
    }

    // 무적 아이템 효과
    IEnumerator Invincible()
    {
        playerControl.invincibleLength = 10f;
        if (playerControl.GetIsInvincible())
        {
            Destroy(gameObject);
            yield break;
        }
        isInvincibleItemPlaying = true;
        playerControl.SetIsInvincible(true);

        while (playerControl.invincibleLength > 0)
        {
            playerControl.invincibleLength -= 0.6f;
            playerControl.ChangeColor(Color.red);
            yield return new WaitForSeconds(0.1f);
            playerControl.ChangeColor(Color.yellow);
            yield return new WaitForSeconds(0.1f);
            playerControl.ChangeColor(Color.green);
            yield return new WaitForSeconds(0.1f);
            playerControl.ChangeColor(Color.blue);
            yield return new WaitForSeconds(0.1f);
            playerControl.ChangeColor(Color.magenta);
            yield return new WaitForSeconds(0.1f);
        }
        playerControl.ChangeColorOriginal();

        playerControl.SetIsInvincible(false);

        Destroy(gameObject);
    }
}
