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

    private int[] savedRankScore = new int[6]; //저장된 점수
    private string[] savedRankID = new string[6]; //저장된 ID

    // Start is called before the first frame update
    void Awake()
    {
        myScore = myScoreText.GetComponent<TextMeshProUGUI>();
        myID = myIDText.GetComponent<TextMeshProUGUI>();
        for (int i = 0; i < rankIDText.Length; i++)
        {
            rankID[i] = rankIDText[i].GetComponent<TextMeshProUGUI>();
            rankScore[i] = rankScoreText[i].GetComponent<TextMeshProUGUI>();
        }

        ImportRank();
        RankSort();
        ShowRank();
        RankSave();
    }

    // Update is called once per frame
    void Update()
    {
        // 디버그용
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerPrefs.DeleteKey("1stScore");
            PlayerPrefs.DeleteKey("1stID");
            PlayerPrefs.DeleteKey("2ndScore");
            PlayerPrefs.DeleteKey("2ndID");
            PlayerPrefs.DeleteKey("3rdScore");
            PlayerPrefs.DeleteKey("3rdID");
            PlayerPrefs.DeleteKey("4thScore");
            PlayerPrefs.DeleteKey("4thID");
            PlayerPrefs.DeleteKey("5thScore");
            PlayerPrefs.DeleteKey("5thID");
        }
    }

    void ImportRank()   //랭킹 점수 불러오기
    {
        savedRankScore[0] = PlayerPrefs.GetInt("1stScore");
        savedRankID[0] = PlayerPrefs.GetString("1stID");
        savedRankScore[1] = PlayerPrefs.GetInt("2ndScore");
        savedRankID[1] = PlayerPrefs.GetString("2ndID");
        savedRankScore[2] = PlayerPrefs.GetInt("3rdScore");
        savedRankID[2] = PlayerPrefs.GetString("3rdID");
        savedRankScore[3] = PlayerPrefs.GetInt("4thScore");
        savedRankID[3] = PlayerPrefs.GetString("4thID");
        savedRankScore[4] = PlayerPrefs.GetInt("5thScore");
        savedRankID[4] = PlayerPrefs.GetString("5thID");
        myScore.text = GameManager.inst.GetScore().ToString();
        myID.text = GameManager.inst.GetPlayerName();
    }

    void RankSort()   //현재점수와 랭킹점수 비교 및 정렬
    {
        for (int i = 0; i < rankScore.Length; i++)
        {
            if (int.Parse(myScore.text) > savedRankScore[i]) //i+1등보다 점수가 높으면
            {
                for (int j = rankScore.Length - 1; j >= i; j--)
                {
                    savedRankID[j + 1] = savedRankID[j];
                    savedRankScore[j + 1] = savedRankScore[j];
                }
                savedRankID[i] = myID.text;
                savedRankScore[i] = int.Parse(myScore.text);
                break;
            }
        }
    }

    void ShowRank()   //화면에 랭킹 개시
    {
        for (int i = 0; i < rankScore.Length; i++)
        {
            rankID[i].text = savedRankID[i];
            rankScore[i].text = savedRankScore[i].ToString();
        }
    }

    void RankSave()   //랭킹 점수 저장
    {
        PlayerPrefs.SetInt("1stScore", savedRankScore[0]);
        PlayerPrefs.SetString("1stID", savedRankID[0]);
        PlayerPrefs.SetInt("2ndScore", savedRankScore[1]);
        PlayerPrefs.SetString("2ndID", savedRankID[1]);
        PlayerPrefs.SetInt("3rdScore", savedRankScore[2]);
        PlayerPrefs.SetString("3rdID", savedRankID[2]);
        PlayerPrefs.SetInt("4thScore", savedRankScore[3]);
        PlayerPrefs.SetString("4thID", savedRankID[3]);
        PlayerPrefs.SetInt("5thScore", savedRankScore[4]);
        PlayerPrefs.SetString("5thID", savedRankID[4]);
    }
}
