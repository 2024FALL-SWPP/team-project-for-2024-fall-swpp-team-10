using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StageSelectionManager : MonoBehaviour
{
    public Button buttonRight;
    public Button buttonLeft;

    public GameObject[] stages;
    private int stageNum = 1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        buttonLeft.gameObject.SetActive(stageNum > 1);
        buttonRight.gameObject.SetActive(stageNum < stages.Length);
    }

    public void ButtonRight()
    {
        stages[stageNum - 1].SetActive(false);
        stageNum++;
        stages[stageNum - 1].SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ButtonLeft()
    {
        stages[stageNum - 1].SetActive(false);
        stageNum--;
        stages[stageNum - 1].SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
    }
}
