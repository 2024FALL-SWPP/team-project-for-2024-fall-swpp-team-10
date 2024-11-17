using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoardManager : MonoBehaviour
{
    public GameObject myScoreText;
    private TextMeshProUGUI myScore;
    public GameObject myIDText;
    private TextMeshProUGUI myID;

    // Start is called before the first frame update
    void Awake()
    {
        myScore = myScoreText.GetComponent<TextMeshProUGUI>();
        myScore.text = GameManager.inst.GetScore().ToString();
        myID = myIDText.GetComponent<TextMeshProUGUI>();
        myID.text = GameManager.inst.GetPlayerName();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
