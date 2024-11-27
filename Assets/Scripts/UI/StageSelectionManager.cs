using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectionManager : MonoBehaviour
{
    public Button buttonRight;
    public Button buttonLeft;

    public GameObject[] stages;
    private int stageNum = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (stageNum == 0)
        {
            buttonLeft.gameObject.SetActive(false);
        }
        else
        {
            buttonLeft.gameObject.SetActive(true);
        }
        if (stageNum == stages.Length - 1)
        {
            buttonRight.gameObject.SetActive(false);
        }
        else
        {
            buttonRight.gameObject.SetActive(true);
        }
    }

    public void ButtonRight()
    {
        stages[stageNum].SetActive(false);
        stageNum++;
        stages[stageNum].SetActive(true);
    }

    public void ButtonLeft()
    {
        stages[stageNum].SetActive(false);
        stageNum--;
        stages[stageNum].SetActive(true);
    }
}
