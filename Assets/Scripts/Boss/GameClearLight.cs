using UnityEngine;
using System.Collections;

public class GameClearLight : MonoBehaviour
{
    private Transform player; // �÷��̾� Transform
    public Light spotlight; // Spotlight ������Ʈ
    public ParticleSystem glowEffect; // �ı� ȿ�� Particle System
    private float spotlightHeight = 5f; // Spotlight�� ����
    private float spotlightIntensity = 50f; // Spotlight ���
    private float transitionSpeed = 2f; // Spotlight ��ȭ �ӵ�

    private bool isActive = false;

    void Start()
    {
        if (spotlight == null)
        {
            Debug.LogError("Spotlight�� �������� �ʾҽ��ϴ�.");
        }

        if (glowEffect != null)
        {
            glowEffect.Stop(); // �ʱ⿡�� ��Ȱ��ȭ
        }

        spotlight.intensity = 0f;
        spotlight.enabled = false;
    }

    void Update()
    {
        if (isActive)
        {
            FollowPlayer(); // Spotlight�� �ı� ȿ���� �÷��̾�� ����ȭ
        }
    }

    public void ActivateLight( Transform _player)
    {
        if (!isActive)
        {
            player = _player;   
            isActive = true;
            spotlight.enabled = true;

            // �ı� ȿ�� Ȱ��ȭ
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
            // Spotlight ��ġ�� �÷��̾� ���� �̵�
            Vector3 targetPosition = player.position + Vector3.up * spotlightHeight;
            spotlight.transform.position = Vector3.Lerp(spotlight.transform.position, targetPosition, Time.deltaTime * transitionSpeed);

            // Spotlight�� �׻� �÷��̾ �ٶ󺸵��� ����
            spotlight.transform.LookAt(player);

            // Particle System ��ġ�� �÷��̾�� ����ȭ
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
