using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingObject : ObjectManager // enemy, obstacle
{
    protected int score;
    protected bool isDestroyable;

    [Header("Sound System")]
    [SerializeField] public AudioClip enemyCollisionSound;
    [SerializeField][Range(0f, 1f)] public float volume = 0.5f;


    [Header("Particle System")]
    public ParticleSystem[] hitOnInvincibleParticle; // 무적 상태에서 장애물이나 적을 파괴했을 때
    public ParticleSystem damagedParticle; // 장애물이나 적에 부딪혔을 때

    protected override void OnPlayerCollision(GameObject player)
    {
        if (playerControl.GetIsInvincible() || playerControl.GetIsBlinking())
        {
            Instantiate(hitOnInvincibleParticle[(int)GameManager.inst.GetCharacter()], playerControl.centerPosition, new Quaternion(0, 0, 0, 0));
            if (playerControl.GetIsInvincible())
                GameManager.inst.AddScore(score);
            CollisionSoundPlay();
            Destroy(gameObject);
            return;
        }

        isDestroyable = false; // 플레이어가 해당 Object에 의해 타격을 입은 경우, 해당 Object를 파괴할 수는 없다.
        Instantiate(damagedParticle, playerControl.centerPosition, new Quaternion(0, 0, 0, 0));
        GameManager.inst.AddScore(score * -1);
        GameManager.inst.RemoveLife();

        CollisionSoundPlay();
        StartCoroutine(playerControl.Blink());
    }

    protected virtual void CollisionSoundPlay()
    {
        if (enemyCollisionSound != null)
        {
            AudioSource.PlayClipAtPoint(enemyCollisionSound, transform.position, volume);
        }
    }

    public bool IsDestroyable()
    {
        return isDestroyable;
    }
}
