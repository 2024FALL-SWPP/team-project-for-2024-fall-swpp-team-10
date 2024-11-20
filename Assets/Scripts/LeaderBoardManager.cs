using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoardManager : MonoBehaviour
{
    [Header("My Info")]
    public GameObject myScoreText;
    private TextMeshProUGUI myScore; //현재 점수
    public GameObject myIDText;
    private TextMeshProUGUI myID; //현재 ID

    [Header("Rank Info")]
    public GameObject[] rankText = new GameObject[5];
    private TextMeshProUGUI[] rankID = new TextMeshProUGUI[5]; //보이는 ID
    private TextMeshProUGUI[] rankScore = new TextMeshProUGUI[5]; //보이는 점수

    private int[] savedRankScore = new int[7]; //저장된 점수
    private string[] savedRankID = new string[7]; //저장된 ID

    // Start is called before the first frame update
    void Awake()
    {
        Rank();
        myScore = myScoreText.GetComponent<TextMeshProUGUI>();
        myScore.text = GameManager.inst.GetScore().ToString();
        myID = myIDText.GetComponent<TextMeshProUGUI>();
        myID.text = GameManager.inst.GetPlayerName();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void RankSet()  //랭킹 점수 불러오기
    {

    }

    void Rank()  //현재점수와 랭킹점수 비교 및 배치 함수
    {
        for (int i = 0; i < 5; i++)
        {
            if (int.Parse(myScore.text) > savedRankScore[i]) //i+1등보다 점수가 높으면
            {
                savedRankID[i + 2] = savedRankID[i + 1];
                savedRankScore[i + 2] = savedRankScore[i + 1];
                savedRankID[i + 1] = savedRankID[i];
                savedRankScore[i + 1] = savedRankScore[i];
                savedRankID[i] = myID.text;
                savedRankScore[i] = int.Parse(myScore.text);
            }
        }
    }

    void RankSave()   //종료전 랭킹 점수 저장
    {
        // PlayerPrefs.SetInt("1st", Score_first_i);
        // PlayerPrefs.SetInt("2nd", Score_second_i);
        // PlayerPrefs.SetInt("3rd", Score_third_i);
    }
}
