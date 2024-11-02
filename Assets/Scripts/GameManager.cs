using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    string playerName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadStageSelection()
    {
        playerName = GameObject.FindWithTag("PlayerName").GetComponent<TextMeshProUGUI>().text;
        //SceneManager.LoadScene("StageSelectionScene");
        Debug.Log("Load Scene with Player Name: " + playerName + "\n(First Undo Comment in GameManager > loadStageSelection() )");
    }
}
