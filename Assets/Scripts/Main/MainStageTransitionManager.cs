using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;


public class MainStageTransitionManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] Canvas transitionCanvas;
    [SerializeField] TextMeshProUGUI stageCleared;
    [SerializeField] TextMeshProUGUI countdownText;

    [Header("Character References")]
    private GameObject activeCharacter;

    [Header("Transition Settings")]
    [SerializeField] protected Animator transitionAnimator;
    [SerializeField] float cameraTransitionDuration = 2.0f;
    [SerializeField] GameObject pointLight;

    private bool isTransitioning = false;
    private Camera mainCamera;
    private Transform finalCameraAngle;

    void Awake()
    {
        mainCamera = Camera.main;
        // Hide UI elements initially
        transitionCanvas.gameObject.SetActive(false);
        pointLight?.SetActive(false);
    }

    public void SetCurrentCharacter(GameObject _character)
    {
        activeCharacter = _character;
    }

    private bool GetFinalCameraAngle()
    {
        if (activeCharacter != null)
        {
            finalCameraAngle = activeCharacter.transform.Find("FinalCameraAngle");
            return true;
        }
        return false;
    }

    public IEnumerator StartMainStageTransition()
    {
        if (isTransitioning) yield break;
        isTransitioning = true;

        pointLight?.SetActive(true);

        // Make sure time is not scaled
        Time.timeScale = 1f;

        if (!GetFinalCameraAngle())
        {
            yield break;
        }

        // Store starting camera transform
        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;
        float startTime = Time.time;

        // Move camera to final position
        while (Time.time - startTime < cameraTransitionDuration)
        {
            float t = (Time.time - startTime) / cameraTransitionDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            mainCamera.transform.position = Vector3.Lerp(startPosition, finalCameraAngle.position, smoothT);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, finalCameraAngle.rotation, smoothT);

            yield return null;
        }

        // Ensure final position
        mainCamera.transform.position = finalCameraAngle.position;
        mainCamera.transform.rotation = finalCameraAngle.rotation;

        // Now stop time for the rest of the transition
        Time.timeScale = 0f;

        // Smoothly show "Stage Cleared" text
        transitionCanvas.gameObject.SetActive(true);
        stageCleared.gameObject.SetActive(true);
        stageCleared.alpha = 0f;

        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.unscaledDeltaTime;
            stageCleared.alpha = elapsed;
            yield return null;
        }

        // Play the animation
        transitionAnimator?.Play("MainStageEnd");

        // Reset time scale before scene change
        Time.timeScale = 1;

        // Load boss scene
        GameManager.inst.LoadBossStage();
    }
}
