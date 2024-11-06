using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialScreenManager : MonoBehaviour
{
    public TextMeshProUGUI[] tutorialTexts;  // Text ��� TextMeshProUGUI �迭�� ����
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
        // ��� ������ �ؽ�Ʈ ��Ȱ��ȭ
        for (int i = 0; i < tutorialTexts.Length; i++)
        {
            tutorialTexts[i].gameObject.SetActive(i == currentPage);
        }

        // ���� �������� �´� �ؽ�Ʈ�� Ȱ��ȭ
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
