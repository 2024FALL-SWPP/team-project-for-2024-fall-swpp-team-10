using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumManager;

public class ButtonHandler : MonoBehaviour
{
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
        GameManager.inst.LoadMainMenu();  // GameManager의 메서드 호출
        GameManager.inst.ResetStats();
    }

    public void LoadStageSelection()
    {
        GameManager.inst.LoadStageSelection();
        GameManager.inst.ResetStats();
    }

    public void LoadCharacterSelection(int stage)
    {
        GameManager.inst.SetStage(stage);
        GameManager.inst.LoadCharacterSelection();
    }

    public void LoadLeaderboard()
    {
        GameManager.inst.LoadLeaderboard();
    }

    public void SetCharacter(int character)
    {
        GameManager.inst.SetCharacter((Character)character);
    }
    public void LoadMainStage()
    {
        GameManager.inst.ResetStats();
        GameManager.inst.LoadMainStage();
    }

    public void SetPlayerName()
    {
        GameManager.inst.SetPlayerName();
    }
}
