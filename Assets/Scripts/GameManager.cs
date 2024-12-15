using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using EnumManager;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    private string playerName;

    private int life;
    private int maxLife = 3;
    private int stage = 1;
    private Character character;
    private int score = 0;
    public Color[,] originColorSave = null;
    private int bossStageMaxLife = 5;

    private bool selected = false;
    private int enemyKill = 0;

    private void Awake()
    {
        if (GameManager.inst == null)
        {
            GameManager.inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        life = maxLife;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public string PlayerPrefsCharacterUnlockKey(Character _character)
    {
        return _character + "Unlock";
    }
    public void LoadIntro()
    {
        SceneManager.LoadScene("IntroScene");
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void LoadStageSelection()
    {
        SceneManager.LoadScene("StageSelectionScene");
    }

    public void LoadCharacterSelection()
    {
        SceneManager.LoadScene("CharacterSelectionScene");
    }

    public void LoadMainStage()
    {
        SceneManager.LoadScene("MainStage" + stage + "Scene");
    }

    public void LoadBossStage()
    {
        SceneManager.LoadScene("BossStage" + stage + "Scene");
    }

    public void LoadLeaderboard()
    {
        SceneManager.LoadScene("LeaderBoardScene");
    }

    public void ResetStats()
    {
        life = maxLife;
        score = 0;
        enemyKill = 0;
    }

    public string GetPlayerName()
    {
        return playerName;
    }
    public void SetPlayerName()
    {
        playerName = GameObject.FindWithTag("PlayerName").GetComponent<TextMeshProUGUI>().text;
    }

    public int GetStage()
    {
        return stage;
    }
    public void SetStage(int _stage)
    {
        stage = _stage;
    }

    public Character GetCharacter()
    {
        return character;
    }
    public void SetCharacter(Character _character)
    {
        character = _character;
        originColorSave = null;
    }

    public int GetLife()
    {
        return life;
    }
    public void RemoveLife()
    {
        if (life > 0)
            life--;
    }
    public void AddLife(int _maxLife)
    {
        if (life < _maxLife)
            life++;
    }

    public void AddScore(int _score)
    {
        score += _score;
        if (score < 0) score = 0;
    }
    public int GetScore()
    {
        return score;
    }
    public int GetMaxLife()
    {
        return maxLife;
    }
    public void SetMaxLife(int _maxLife)
    {
        maxLife = _maxLife;
    }
    public int GetBossStageMaxLife()
    {
        return bossStageMaxLife;
    }
    public void SetBossStageMaxLife(int _bossStageMaxLife)
    {
        bossStageMaxLife = _bossStageMaxLife;
    }
    public int GetEnemyKill()
    {
        return enemyKill;
    }
    public void AddEnemyKill()
    {
        enemyKill++;
    }
    public void SetEnemyKillToZero()
    {
        enemyKill = 0;
    }
    public bool IsSelected()
    {
        return selected;
    }
    public void SetSelected(bool _selected)
    {
        selected = _selected;
    }

    public void CursorActive(bool toVisible)
    {
        Cursor.lockState = toVisible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = toVisible;
    }

    public void SetPlayerUnlockPrefs(Character character)  //0이나 1은 다루지 않음
    {
        PlayerPrefs.SetInt(PlayerPrefsCharacterUnlockKey(character), 0);
    }

    public bool IsUnlocked(Character character)
    {
        return PlayerPrefs.HasKey(PlayerPrefsCharacterUnlockKey(character));
    }
}
