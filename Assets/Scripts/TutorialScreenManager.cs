using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScreenManager : MonoBehaviour
{
    public Image[] tutorialPages;  // �� �������� Image�� ����
    public Button previousButton;
    public Button nextButton;

    private int currentPage = 0;

    void Start()
    {
        UpdateTutorialPage();
        previousButton.onClick.AddListener(GoToPreviousPage);
        nextButton.onClick.AddListener(GoToNextPage);
    }

    void UpdateTutorialPage()
    {
        // ��� ������ ��Ȱ��ȭ
        for (int i = 0; i < tutorialPages.Length; i++)
        {
            tutorialPages[i].gameObject.SetActive(i == currentPage);
        }

        // ���� ������ Ȱ��ȭ
        previousButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < tutorialPages.Length - 1;
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
        if (currentPage < tutorialPages.Length - 1)
        {
            currentPage++;
            UpdateTutorialPage();
        }
    }
}
