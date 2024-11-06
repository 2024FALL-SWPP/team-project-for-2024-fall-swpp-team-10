using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    string playerName;

    public GameObject[] heart = new GameObject[3];
    private int characterint;
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
        // playerName = GameObject.FindWithTag("PlayerName").GetComponent<TextMeshProUGUI>().text;
        SceneManager.LoadScene("StageSelectionScene");
        // Debug.Log("Load Scene with Player Name: " + playerName + "\n(First Undo Comment in GameManager > loadStageSelection() )");
    }

    public void LoadCharacterSelection(int stage)
    {
        SceneManager.LoadScene("Stage" + stage + "CharacterSelectionScene");
    }

    public void LoadMainStage(int stage)
    {
        SceneManager.LoadScene("MainStage" + stage + "Scene");
    }

    public int GetCharacterInt()
    {
        return characterint;
    }
    public void SetCharacterInt(int character)
    {
        characterint = character;
    }
}
