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
    public GameObject[] rankIDText = new GameObject[5];
    private TextMeshProUGUI[] rankID = new TextMeshProUGUI[5]; //보이는 ID
    public GameObject[] rankScoreText = new GameObject[5];
    private TextMeshProUGUI[] rankScore = new TextMeshProUGUI[5]; //보이는 점수

    private int[] savedRankScore = new int[7]; //저장된 점수
    private string[] savedRankID = new string[7]; //저장된 ID

    // Start is called before the first frame update
    void Awake()
    {
        Rank();
        myScore = myScoreText.GetComponent<TextMeshProUGUI>();
        myID = myIDText.GetComponent<TextMeshProUGUI>();
        for (int i = 0; i < rankIDText.Length; i++)
        {
            rankID[i] = rankIDText[i].GetComponent<TextMeshProUGUI>();
            rankScore[i] = rankScoreText[i].GetComponent<TextMeshProUGUI>();
        }
        RankSet();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void RankSet()  //랭킹 점수 불러오기
    {
        myScore.text = GameManager.inst.GetScore().ToString();
        myID.text = GameManager.inst.GetPlayerName();
        // Score_first_i = PlayerPrefs.GetInt("1st");
        // Score_second_i = PlayerPrefs.GetInt("2nd");
        // Score_third_i = PlayerPrefs.GetInt("3rd");
        // Score_now_i = PlayerPrefs.GetInt("Score");
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
        PlayerPrefs.SetInt("1st", savedRankScore[0]);
        PlayerPrefs.SetString("1st", savedRankID[0]);
        PlayerPrefs.SetInt("2nd", savedRankScore[0]);
        PlayerPrefs.SetString("2nd", savedRankID[0]);
        PlayerPrefs.SetInt("3rd", savedRankScore[0]);
        PlayerPrefs.SetString("3rd", savedRankID[0]);
        PlayerPrefs.SetInt("4th", savedRankScore[0]);
        PlayerPrefs.SetString("4th", savedRankID[0]);
        PlayerPrefs.SetInt("5th", savedRankScore[0]);
        PlayerPrefs.SetString("5th", savedRankID[0]);
    }
}
