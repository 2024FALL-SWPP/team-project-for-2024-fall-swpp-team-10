using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIntroManager : MonoBehaviour
{
    private BossStageCamera cameraScript;
    private Animator fadeImageAnimator;
    private BossStageTransitionManager transitionManager;
    private BossControl bossControl;
    private Transform playerTransform;
    private Transform gameplayCameraTransform;
    private System.Action onIntroEnd;
    private GameObject[] fires;
    private AudioSource[] fireAudioSources;
    private float cameraOrbitDuration = 3f; // ī�޶� ���� �ð�

    public void Initialize(
        BossStageCamera cameraScript,
        Animator fadeImageAnimator,
        BossStageTransitionManager transitionManager,
        BossControl bossControl,
        Transform playerTransform,
        Transform gameplayCameraTransform,
        System.Action onIntroEnd,
        GameObject[] fires,
        AudioSource[] fireAudioSources
    )
    {
        this.cameraScript = cameraScript;
        this.fadeImageAnimator = fadeImageAnimator;
        this.transitionManager = transitionManager;
        this.bossControl = bossControl;
        this.playerTransform = playerTransform;
        this.gameplayCameraTransform = gameplayCameraTransform;
        this.onIntroEnd = onIntroEnd;
        this.fires = fires;
        this.fireAudioSources = fireAudioSources;
    }

    public IEnumerator RunIntroSequence()
    {
        // ����� �����ֱ�
        cameraScript.transform.position = new Vector3(15, 15, 0);
        cameraScript.transform.LookAt(new Vector3(0, 1, 0));

        yield return new WaitForSecondsRealtime(1f);

        // �ұ�� ����
        yield return StartCoroutine(ActivateFiresInPhases());

        // ���̵� �ƿ�
        fadeImageAnimator?.SetTrigger("FadeOut");
        yield return new WaitForSecondsRealtime(1f);

        // ���� �����ֱ�
        fadeImageAnimator?.SetTrigger("FadeIn");
        yield return StartCoroutine(DynamicCameraMovement(bossControl.transform, 15, 4, Vector3.zero));

        // ���̵� �ƿ�
        fadeImageAnimator?.SetTrigger("FadeOut");
        yield return new WaitForSecondsRealtime(1f);

        // ĳ���� �����ֱ�
        fadeImageAnimator?.SetTrigger("FadeIn");
        yield return StartCoroutine(DynamicCameraMovement(playerTransform, 1, 1, new Vector3(0, 0, -5)));

        // ���̵� �ƿ�
        fadeImageAnimator?.SetTrigger("FadeOut");
        yield return new WaitForSecondsRealtime(1f);

        // ���� �÷��� �������� ����
        cameraScript.transform.position = gameplayCameraTransform.position;
        cameraScript.transform.rotation = gameplayCameraTransform.rotation;

        // ���̵� �� (���� ����)
        fadeImageAnimator?.SetTrigger("FadeIn");
        cameraScript.SetCameraFixed(false);
        yield return new WaitForSecondsRealtime(0.5f);

        // ī��Ʈ�ٿ�
        StartCoroutine(WaitForIntro());
        StartCoroutine(transitionManager.Countdown());
    }

    private IEnumerator WaitForIntro()
    {
        yield return new WaitForSecondsRealtime(transitionManager.countdownDuration);

        onIntroEnd?.Invoke();
    }

    private IEnumerator DynamicCameraMovement(Transform target, int h, int radius, Vector3 offset)
    {
        float duration = cameraOrbitDuration;
        float elapsed = 0f;

        float startX = -radius;
        float endX = radius;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float currentX = Mathf.Lerp(startX, endX, t);

            Vector3 linearPosition = target.position + offset + new Vector3(currentX, h, -radius);

            cameraScript.transform.position = linearPosition;
            cameraScript.transform.LookAt(target.position + new Vector3(0, 1, 0));

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private IEnumerator ActivateFiresInPhases()
    {
        // BossStageManager���� ��� �ұ�� ��Ȱ��ȭ�� ��ģ ���¶�� ����.
        // �ʿ��ϴٸ� BossStageManager���� public �޼ҵ�� �ұ�� �迭�� ���Թ��� ���� ����.
        BossStageManager stageManager = FindObjectOfType<BossStageManager>();
        if (stageManager == null)
        {
            Debug.LogError("BossStageManager not found for ActivateFiresInPhases");
            yield break;
        }

        // ù ��° ���� Ȱ��ȭ
        for (int i = 0; i < fires.Length; i += 2)
        {
            fires[i].SetActive(true);
            AudioSource.PlayClipAtPoint(fireAudioSources[i].clip, cameraScript.transform.position, 0.03f);
            yield return null;
        }
        yield return new WaitForSecondsRealtime(1f); // ù ��° �ܰ� ���

        // �� ��° ���� Ȱ��ȭ
        for (int i = 1; i < fires.Length; i += 2)
        {
            fires[i].SetActive(true);
            AudioSource.PlayClipAtPoint(fireAudioSources[i].clip, cameraScript.transform.position, 0.03f);
            yield return null;
        }
        yield return new WaitForSecondsRealtime(1f); // �� ��° �ܰ� ���
    }
}
