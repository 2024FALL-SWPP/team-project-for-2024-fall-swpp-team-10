using UnityEngine;
using System.Collections;

public class GameClearLight : MonoBehaviour
{
    private Transform player; // 플레이어 Transform
    public Light spotlight; // Spotlight 컴포넌트
    public ParticleSystem glowEffect; // 후광 효과 Particle System
    private float spotlightHeight = 5f; // Spotlight의 높이
    private float spotlightIntensity = 50f; // Spotlight 밝기
    private float transitionSpeed = 2f; // Spotlight 변화 속도

    private bool isActive = false;

    void Start()
    {
        if (spotlight == null)
        {
            Debug.LogError("Spotlight가 설정되지 않았습니다.");
        }

        if (glowEffect != null)
        {
            glowEffect.Stop(); // 초기에는 비활성화
        }

        spotlight.intensity = 0f;
        spotlight.enabled = false;
    }

    void Update()
    {
        if (isActive)
        {
            FollowPlayer(); // Spotlight와 후광 효과를 플레이어와 동기화
        }
    }

    public void ActivateLight( Transform _player)
    {
        if (!isActive)
        {
            player = _player;   
            isActive = true;
            spotlight.enabled = true;

            // 후광 효과 활성화
            if (glowEffect != null)
            {
                glowEffect.Play();
            }

            StartCoroutine(IncreaseLightIntensity());
        }
    }

    private void FollowPlayer()
    {
        if (player != null)
        {
            // Spotlight 위치를 플레이어 위로 이동
            Vector3 targetPosition = player.position + Vector3.up * spotlightHeight;
            spotlight.transform.position = Vector3.Lerp(spotlight.transform.position, targetPosition, Time.deltaTime * transitionSpeed);

            // Spotlight가 항상 플레이어를 바라보도록 설정
            spotlight.transform.LookAt(player);

            // Particle System 위치도 플레이어와 동기화
            glowEffect.transform.position = player.position;
        }
    }

    private IEnumerator IncreaseLightIntensity()
    {
        float currentIntensity = spotlight.intensity;

        while (currentIntensity < spotlightIntensity)
        {
            currentIntensity += Time.deltaTime * transitionSpeed;
            spotlight.intensity = currentIntensity;
            yield return null;
        }

        spotlight.intensity = spotlightIntensity;
    }
}
