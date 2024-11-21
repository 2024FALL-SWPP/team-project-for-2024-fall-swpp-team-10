using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoardManager : MonoBehaviour
{
    [Header("Stage Info")]
    public GameObject stageText;
    private TextMeshProUGUI currentStage; //현재 스테이지
    Stage stage = (Stage)GameManager.inst.GetStage();


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
        currentStage = stageText.GetComponent<TextMeshProUGUI>();
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
            PlayerPrefs.DeleteKey(stage + "_1stScore");
            PlayerPrefs.DeleteKey(stage + "_1stID");
            PlayerPrefs.DeleteKey(stage + "_2ndScore");
            PlayerPrefs.DeleteKey(stage + "_2ndID");
            PlayerPrefs.DeleteKey(stage + "_3rdScore");
            PlayerPrefs.DeleteKey(stage + "_3rdID");
            PlayerPrefs.DeleteKey(stage + "_4thScore");
            PlayerPrefs.DeleteKey(stage + "_4thID");
            PlayerPrefs.DeleteKey(stage + "_5thScore");
            PlayerPrefs.DeleteKey(stage + "_5thID");
        }
    }

    void ImportRank()   //랭킹 점수 불러오기
    {
        savedRankScore[0] = PlayerPrefs.GetInt(stage + "_1stScore");
        savedRankID[0] = PlayerPrefs.GetString(stage + "_1stID");
        savedRankScore[1] = PlayerPrefs.GetInt(stage + "_2ndScore");
        savedRankID[1] = PlayerPrefs.GetString(stage + "_2ndID");
        savedRankScore[2] = PlayerPrefs.GetInt(stage + "_3rdScore");
        savedRankID[2] = PlayerPrefs.GetString(stage + "_3rdID");
        savedRankScore[3] = PlayerPrefs.GetInt(stage + "_4thScore");
        savedRankID[3] = PlayerPrefs.GetString(stage + "_4thID");
        savedRankScore[4] = PlayerPrefs.GetInt(stage + "_5thScore");
        savedRankID[4] = PlayerPrefs.GetString(stage + "_5thID");
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
        currentStage.text = stage.ToString() + " Ranking";
        for (int i = 0; i < rankScore.Length; i++)
        {
            rankID[i].text = savedRankID[i];
            rankScore[i].text = savedRankScore[i].ToString();
        }
    }

    void RankSave()   //랭킹 점수 저장
    {
        PlayerPrefs.SetInt(stage + "_1stScore", savedRankScore[0]);
        PlayerPrefs.SetString(stage + "_1stID", savedRankID[0]);
        PlayerPrefs.SetInt(stage + "_2ndScore", savedRankScore[1]);
        PlayerPrefs.SetString(stage + "_2ndID", savedRankID[1]);
        PlayerPrefs.SetInt(stage + "_3rdScore", savedRankScore[2]);
        PlayerPrefs.SetString(stage + "_3rdID", savedRankID[2]);
        PlayerPrefs.SetInt(stage + "_4thScore", savedRankScore[3]);
        PlayerPrefs.SetString(stage + "_4thID", savedRankID[3]);
        PlayerPrefs.SetInt(stage + "_5thScore", savedRankScore[4]);
        PlayerPrefs.SetString(stage + "_5thID", savedRankID[4]);
    }
}
