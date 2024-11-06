using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    public void LoadStageSelection()
    {
        GameManager.inst.LoadStageSelection();
    }

    public void LoadCharacterSelection(int stage)
    {
        GameManager.inst.LoadCharacterSelection(stage);
    }

    public void LoadMainStageCharacter(int character)
    {
        GameManager.inst.SetCharacterInt(character);
    }
    public void LoadMainStageStage(int stage)
    {
        GameManager.inst.LoadMainStage(stage);
    }
}
