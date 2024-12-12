using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainIntroManager : MonoBehaviour
{
    public Vector3 FinalCameraPos = new Vector3(0, 4, -10);
    public Vector3 InitialCameraPos = new Vector3(0, 1, -4);

    public Vector3 InitialBossPos = new Vector3(0, 0, -2);
    public Vector3 FinalBossPos = new Vector3(0, 6, -2);

    public Color finalBossColor;
    public MainStageManager mainStageManager;
    public GameObject gameUI;
    public GameObject boss;
    public GameObject[] fires;

    float transformationDuration = 2f;

    public GameObject[] GuideUI;


    // Start is called before the first frame update
    void Start()
    {
        mainStageManager.isPausable = false;
        Time.timeScale = 0;
        Camera.main.transform.position = InitialCameraPos;
        boss.transform.position = InitialBossPos;
        boss.transform.localScale = Vector3.one * 0.4f;
        ChangeColorHelper(boss.transform, finalBossColor);
        StartCoroutine(changeScale());
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    // Recursively change color of all children
    public void ChangeColorHelper(Transform transform, Color bossColor)
    {
        foreach (Transform childTransform in transform)
        {
            GameObject child = childTransform.gameObject;
            SkinnedMeshRenderer smr = child.GetComponent<SkinnedMeshRenderer>();
            if (smr != null) StartCoroutine(gradualColorChange(smr, Color.white, bossColor));
            MeshRenderer mr = child.GetComponent<MeshRenderer>();
            if (mr != null) StartCoroutine(gradualColorChange(mr, Color.white, bossColor));

            if (childTransform.childCount > 0) ChangeColorHelper(childTransform, bossColor);
        }
    }

    IEnumerator gradualColorChange(Renderer sr, Color startCol, Color endCol)
    {
        float elapsedTime = 0f;
        float duration = transformationDuration;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            if (sr != null)
                sr.material.color = Color.Lerp(startCol, endCol, elapsedTime / duration);


            yield return null;
        }

        if (sr != null) sr.material.color = endCol;
    }

    IEnumerator changeScale()
    {
        yield return new WaitForSecondsRealtime(1f);

        float elapsedTime = 0f;
        float duration = transformationDuration;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float t = elapsedTime / duration;

            // Perform spherical interpolation on scales
            Vector3 newScale = Vector3.Slerp(Vector3.one * 0.4f, Vector3.one, t);
            boss.transform.localScale = newScale;
            if (elapsedTime > (duration / 3))
            {
                //float t2 = (elapsedTime - (duration / 3))
                Vector3 newPosition = Vector3.Slerp(InitialCameraPos, FinalCameraPos, t * 3 - 1);
                Camera.main.transform.position = newPosition;
            }

            yield return null;
        }

        boss.transform.localScale = Vector3.one;
        Camera.main.transform.position = FinalCameraPos;


        foreach (GameObject fire in fires)
            fire.SetActive(true);

        yield return new WaitForSecondsRealtime(1f);

        //foreach (GameObject fire in fires)
        //    fire.SetActive(false);

        elapsedTime = 0f;
        duration = 1f;


        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float t = elapsedTime / duration;

            Vector3 posDiff = (FinalBossPos - InitialBossPos) * t;

            // Perform spherical interpolation on scales
            Vector3 newPosition = InitialBossPos + posDiff;
            boss.transform.position = newPosition;

            yield return null;
        }

        Time.timeScale = 1;
        GuideUI[0].SetActive(true);
        StartCoroutine(fadeTextOut());

        boss.transform.position = FinalBossPos;

        foreach (GameObject fire in fires)
            fire.SetActive(true);

        boss.SetActive(false);
        gameUI.SetActive(true);

    }

    IEnumerator fadeTextOut()
    {
        yield return new WaitForSeconds(1f);
        float elapsedTime = 0f;
        float duration = 1f;

        TextMeshProUGUI text = GuideUI[0].GetComponent<TextMeshProUGUI>();
        Image image1 = GuideUI[1].GetComponent<Image>();
        Image image2 = GuideUI[2].GetComponent<Image>();

        float alphaDiff = 1 / duration;
        float currAlpha = 1;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            currAlpha -= alphaDiff * Time.deltaTime;

            Color newCol = new(1, 1, 1, currAlpha);

            text.color = newCol;
            image1.color = newCol;
            image2.color = newCol;

            yield return null;
        }

        foreach (GameObject UIObj in GuideUI)
        {
            Destroy(UIObj);
        }

        mainStageManager.isPausable = true;
    }
}
