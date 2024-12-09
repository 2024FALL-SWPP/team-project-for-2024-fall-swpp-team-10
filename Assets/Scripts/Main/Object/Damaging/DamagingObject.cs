using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingObject : ObjectManager // enemy, obstacle
{
    protected int score;
    protected bool hasCollided = false; // 이미 플레이어와 충돌했는지 여부 (collision 처리가 중복되는 것을 방지하기 위함)

    [Header("Sound System")]
    [SerializeField] public AudioClip enemyCollisionSound;
    [SerializeField][Range(0f, 1f)] public float volume = 0.5f;


    [Header("Particle System")]
    public ParticleSystem[] hitOnInvincibleParticle; // 무적 상태에서 장애물이나 적을 파괴했을 때
    public ParticleSystem damagedParticle; // 장애물이나 적에 부딪혔을 때

    protected override void OnPlayerCollision(GameObject player)
    {
        if (hasCollided) return; // 이미 collision 처리가 되었다면 return

        if (playerControl.GetIsInvincible() || playerControl.GetIsBlinking())
        {
            Instantiate(hitOnInvincibleParticle[(int)GameManager.inst.GetCharacter()], playerControl.centerPosition, new Quaternion(0, 0, 0, 0));
            if (playerControl.GetIsInvincible())
            {
                GameManager.inst.AddScore(score);
            }
            CollisionSoundPlay();
            Destroy(gameObject);
            return;
        }

        hasCollided = true;
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

    public bool HasCollided()
    {
        return hasCollided;
    }
}
