using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public abstract class StageManager : MonoBehaviour
{
    public GameObject[] characters;
    public GameObject[] characterUI;
    protected GameObject activeCharacter;
    public GameObject pauseMenu;
    public GameObject gameOverScreen;
    public GameObject score;
    private TextMeshProUGUI scoreText;
    protected bool isGameOver = false;
    protected MusicManager musicManager;
    protected StageTransitionManager transitionManager;

    // Variables related to Hearts UI
    public GameObject[] hearts;
    protected int maxLife;
    protected bool isStageComplete = false;

    // Variables for adding score based on lives
    public AudioClip heartDeactivateSound;
    public float soundVolume = 0.7f;

    public virtual void Awake()
    {
        if (scoreText != null)
            scoreText = score.GetComponent<TextMeshProUGUI>();
        if (musicManager != null)
            musicManager = GameObject.Find("MusicManager").GetComponent<MusicManager>();
    }

    public virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            PauseGame();
        }

        // Update score
        if (scoreText != null)
            scoreText.text = "score\n" + GameManager.inst.GetScore().ToString();

        // Update Hearts UI
        if (!isStageComplete)
            UpdateHeartsUI();

        // Check for game over
        if (GameManager.inst != null)
            if (GameManager.inst.GetLife() <= 0 && !isGameOver)
            {
                HandleGameOver();
            }
    }

    public virtual void UpdateHeartsUI()
    {
        if (hearts != null)
        {
            for (int i = 0; i < maxLife; i++)
                hearts[i].SetActive(i < GameManager.inst.GetLife());
        }
    }

    public virtual void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public virtual void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int characterIndex = (int)GameManager.inst.GetCharacter();
        characters[characterIndex].SetActive(true);
        characterUI[characterIndex].SetActive(true);
        activeCharacter = characters[characterIndex];
        Time.timeScale = 1;
        isGameOver = false;
    }
    public virtual GameObject ActiveCharacter()
    {
        return activeCharacter;
    }
    public virtual void PauseGame()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(true);
        Time.timeScale = 0;
        musicManager?.PauseMusic();
    }

    public virtual void ResumeGame()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
        Time.timeScale = 1;
        musicManager?.ResumeMusic();
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    // Handle game over
    public virtual void HandleGameOver()
    {
        Time.timeScale = 0;
        gameOverScreen.SetActive(true);
        isGameOver = true;
        musicManager?.StopMusic();
    }

    // Add score based on remaining lives
    public virtual IEnumerator AddScoreBasedOnLives()
    {
        if (hearts != null)
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                if (hearts[i].activeSelf) // Ȱ��ȭ�� Life�� ó��
                {
                    // Life ��Ȱ��ȭ
                    hearts[i].SetActive(false);
                    // ���� �߰�
                    GameManager.inst.AddScore(5000);
                    // ȿ���� ���
                    if (heartDeactivateSound != null)
                    {
                        AudioSource.PlayClipAtPoint(heartDeactivateSound, Camera.main.transform.position, soundVolume);
                    }
                    // 0.5�� ��� �� ���� Life ó��
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }
}
