using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingObject : ObjectManager // enemy, obstacle
{
    protected GameObject player;
    protected PlayerControl playerControl;
    protected int score;
    [Header("Sound System")]
    [SerializeField] public AudioClip enemyCollisionSound;
    [SerializeField][Range(0f, 1f)] public float volume = 0.5f;


    [Header("Particle System")]
    public ParticleSystem[] hitOnInvincibleParticle; // 무적 상태에서 장애물이나 적을 파괴했을 때
    public ParticleSystem damagedParticle; // 장애물이나 적에 부딪혔을 때
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
        base.Update();
    }

    protected override void OnCollisionEnter(Collision other)
    {
        base.OnCollisionEnter(other);
    }
    protected override void OnPlayerCollision(GameObject player)
    {
        if (playerControl.GetIsInvincible())
        {
            Instantiate(hitOnInvincibleParticle[(int)GameManager.inst.GetCharacter()], playerControl.centerPosition, new Quaternion(0, 0, 0, 0));
            GameManager.inst.AddScore(score);
            Destroy(gameObject);
            return;
        }
        Instantiate(damagedParticle, playerControl.centerPosition, new Quaternion(0, 0, 0, 0));
        GameManager.inst.AddScore(score * -1);
        GameManager.inst.RemoveLife();

        if (enemyCollisionSound != null)
        {
            AudioSource.PlayClipAtPoint(enemyCollisionSound, transform.position, volume);
        }

        StartCoroutine(playerControl.Blink());
    }
}
