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

    // Variables related to Hearts UI
    public GameObject[] hearts;
    protected int maxLife;
    protected bool isStageComplete = false;

    // Variables for adding score based on lives
    public AudioClip heartDeactivateSound;
    public float soundVolume = 0.7f;

    protected bool isPausable = true;
    public bool IsPausable()
    {
        return isPausable;
    }
    public void SetPausable(bool _isPausable)
    {
        isPausable = _isPausable;
    }

    protected virtual void Awake()
    {
        scoreText = score.GetComponent<TextMeshProUGUI>();
        musicManager = GameObject.Find("MusicManager").GetComponent<MusicManager>();
    }

    public virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !isGameOver && isPausable)
        {
            PauseGame();
        }

        // Update score
        scoreText.text = "score\n" + GameManager.inst.GetScore().ToString();

        // Update Hearts UI
        if (!isStageComplete)
            UpdateHeartsUI();

        // Check for game over
        if (GameManager.inst.GetLife() <= 0 && !isGameOver)
        {
            HandleGameOver();
        }
    }

    protected virtual void UpdateHeartsUI()
    {
        if (hearts != null)
        {
            for (int i = 0; i < maxLife; i++)
                hearts[i].SetActive(i < GameManager.inst.GetLife());
        }
    }

    protected virtual void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected virtual void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Time.timeScale = 1;
    }

    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isPausable = true;
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
        isPausable = false;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        musicManager?.PauseMusic();
    }

    public virtual void ResumeGame()
    {
        isPausable = true;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        musicManager?.ResumeMusic();
        EventSystem.current?.SetSelectedGameObject(null);
    }

    // Handle game over
    protected virtual void HandleGameOver()
    {
        isPausable = false;
        Time.timeScale = 0;
        gameOverScreen.SetActive(true);
        isGameOver = true;
        musicManager?.StopMusic();
    }

    // Add score based on remaining lives
    protected virtual IEnumerator AddScoreBasedOnLives()
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
