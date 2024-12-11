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
    private float introAnimationDuration;
    private System.Action onIntroEnd;
    private GameObject[] fires;
    private AudioSource[] fireAudioSources;
    private float cameraOrbitDuration = 3f; // 카메라 연출 시간

    public void Initialize(
        BossStageCamera cameraScript,
        Animator fadeImageAnimator,
        BossStageTransitionManager transitionManager,
        BossControl bossControl,
        Transform playerTransform,
        Transform gameplayCameraTransform,
        float introAnimationDuration,
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
        this.introAnimationDuration = introAnimationDuration;
        this.onIntroEnd = onIntroEnd;
        this.fires = fires;
        this.fireAudioSources = fireAudioSources;
    }

    public IEnumerator RunIntroSequence()
    {
        // 경기장 보여주기
        cameraScript.transform.position = new Vector3(15, 15, 0);
        cameraScript.transform.LookAt(new Vector3(0, 1, 0));

        yield return new WaitForSecondsRealtime(1f);

        // 불기둥 연출
        yield return StartCoroutine(ActivateFiresInPhases());

        // 페이드 아웃
        fadeImageAnimator?.SetTrigger("FadeOut");
        yield return new WaitForSecondsRealtime(1f);

        // 보스 보여주기
        fadeImageAnimator?.SetTrigger("FadeIn");
        yield return StartCoroutine(DynamicCameraMovement(bossControl.transform, 15, 4, Vector3.zero));

        // 페이드 아웃
        fadeImageAnimator?.SetTrigger("FadeOut");
        yield return new WaitForSecondsRealtime(1f);

        // 캐릭터 보여주기
        fadeImageAnimator?.SetTrigger("FadeIn");
        yield return StartCoroutine(DynamicCameraMovement(playerTransform, 1, 1, new Vector3(0, 0, -5)));

        // 페이드 아웃
        fadeImageAnimator?.SetTrigger("FadeOut");
        yield return new WaitForSecondsRealtime(1f);

        // 게임 플레이 시점으로 복귀
        cameraScript.transform.position = gameplayCameraTransform.position;
        cameraScript.transform.rotation = gameplayCameraTransform.rotation;

        // 페이드 인 (게임 시작)
        fadeImageAnimator?.SetTrigger("FadeIn");
        cameraScript.SetCameraFixed(false);
        yield return new WaitForSecondsRealtime(0.5f);

        // 카운트다운
        StartCoroutine(WaitForIntro());
        StartCoroutine(transitionManager.Countdown());
    }

    private IEnumerator WaitForIntro()
    {
        if (introAnimationDuration < transitionManager.countdownDuration)
            Debug.LogError("Countdown finishes before animation");
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
        // BossStageManager에서 모든 불기둥 비활성화를 마친 상태라고 가정.
        // 필요하다면 BossStageManager에서 public 메소드로 불기둥 배열을 주입받을 수도 있음.
        BossStageManager stageManager = FindObjectOfType<BossStageManager>();
        if (stageManager == null)
        {
            Debug.LogError("BossStageManager not found for ActivateFiresInPhases");
            yield break;
        }

        // 첫 번째 절반 활성화
        for (int i = 0; i < fires.Length; i += 2)
        {
            fires[i].SetActive(true);
            AudioSource.PlayClipAtPoint(fireAudioSources[i].clip, cameraScript.transform.position, 0.03f);
            yield return null;
        }
        yield return new WaitForSecondsRealtime(1f); // 첫 번째 단계 대기

        // 두 번째 절반 활성화
        for (int i = 1; i < fires.Length; i += 2)
        {
            fires[i].SetActive(true);
            AudioSource.PlayClipAtPoint(fireAudioSources[i].clip, cameraScript.transform.position, 0.03f);
            yield return null;
        }
        yield return new WaitForSecondsRealtime(1f); // 두 번째 단계 대기
    }
}
