using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using EnumManager;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    private string playerName;

    private int life;
    private int maxLife = 3;
    private int stage;
    private Character character;
    private int score = 0;


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

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void LoadStageSelection()
    {
        playerName = GameObject.FindWithTag("PlayerName").GetComponent<TextMeshProUGUI>().text;
        SceneManager.LoadScene("StageSelectionScene");
        // Debug.Log("Load Scene with Player Name: " + playerName + "\n(First Undo Comment in GameManager > loadStageSelection() )");
    }

    public void LoadCharacterSelection()
    {
        SceneManager.LoadScene("CharacterSelectionScene");
    }

    public void LoadMainStage()
    {
        SceneManager.LoadScene("MainStage" + stage + "Scene");
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
    public void AddLife()
    {
        if (life < maxLife)
            life++;
    }
}
