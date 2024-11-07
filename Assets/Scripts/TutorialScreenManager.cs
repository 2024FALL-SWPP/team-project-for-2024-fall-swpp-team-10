using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScreenManager : MonoBehaviour
{
    public GameObject[] tutorialPageGameObjects;  // �� �������� Image�� ����
    public Button previousButton;
    public Button nextButton;
    public GameObject backgroundGameObject;

    private int currentPage = 0;

    void Start()
    {
        SetBackgroundImage();
        SetTutorialImages();

        UpdateTutorialPage();
        previousButton.onClick.AddListener(GoToPreviousPage);
        nextButton.onClick.AddListener(GoToNextPage);
    }

    void SetBackgroundImage()
    {
        Image image = backgroundGameObject.GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>("Stage" + GameManager.inst.GetStage() + "Background");
    }

    void SetTutorialImages()
    {
        for (int i = 0; i < tutorialPageGameObjects.Length; i++)
        {
            // example of tutorial file name: Stage1Tutorial2.png ("second image of Stage 1 Tutorial")
            string tutorialFilename = "Stage" + GameManager.inst.GetStage() + "Tutorial" + (i + 1);
            tutorialPageGameObjects[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(tutorialFilename);
        }
    }

    void UpdateTutorialPage()
    {
        // ��� ������ ��Ȱ��ȭ
        for (int i = 0; i < tutorialPageGameObjects.Length; i++)
        {
            tutorialPageGameObjects[i].SetActive(i == currentPage);
        }

        // ���� ������ Ȱ��ȭ
        previousButton.interactable = currentPage > 0;
        nextButton.interactable = currentPage < tutorialPageGameObjects.Length - 1;
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
        if (currentPage < tutorialPageGameObjects.Length - 1)
        {
            currentPage++;
            UpdateTutorialPage();
        }
    }
}
