using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialScreenManager : MonoBehaviour
{
    public TextMeshProUGUI[] tutorialTexts;  // Text 대신 TextMeshProUGUI 배열로 설정
    public Button previousButton;
    public Button nextButton;

    private int currentPage = 0;

    void Start()
    {
        UpdateTutorialPage();
        //previousButton.onClick.AddListener(GoToPreviousPage);
        //nextButton.onClick.AddListener(GoToNextPage);
    }

    void UpdateTutorialPage()
    {
        // 모든 페이지 텍스트 비활성화
        for (int i = 0; i < tutorialTexts.Length; i++)
        {
            tutorialTexts[i].gameObject.SetActive(i == currentPage);
        }

        // 현재 페이지에 맞는 텍스트만 활성화
        previousButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < tutorialTexts.Length - 1;
    }

    public void GoToPreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateTutorialPage();
        }
    }

    public void GoToNextPage()
    {
        if (currentPage < tutorialTexts.Length - 1)
        {
            currentPage++;
            UpdateTutorialPage();
        }
    }
}
