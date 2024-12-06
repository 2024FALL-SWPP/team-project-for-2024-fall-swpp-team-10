using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakspotControl : MonoBehaviour
{
    Dictionary<int, Color> WeakSpotStatCol = new Dictionary<int, Color>();
    Color WeakSpotCols0 = new(51f / 255f, 1f, 0f, 156f / 255f);  // Initial spot color
    Color WeakSpotCols1 = new(0f, 1f, 219f / 255f, 156f / 255f); // Spot color on first hit
    Color WeakSpotCols2 = new(0f, 152f / 255f, 1f, 1f);         //      ''       second hit
    Color WeakSpotCols3 = new(1f, 1f, 1f, 100f / 255f);         //      ''       third hit = final color
    BossStageManager bossStageManager;

    [Header("Audio Settings")]
    [SerializeField] public AudioClip weakSpotSound;
    [SerializeField][Range(0f, 1f)] public float weakSpotVolume = 0.7f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        bossStageManager = GameObject.Find("BossStageManager").GetComponent<BossStageManager>();

        WeakSpotStatCol = new Dictionary<int, Color>()
        {
            { 0, WeakSpotCols0 },
            { 1, WeakSpotCols1 },
            { 2, WeakSpotCols2 },
            { 3, WeakSpotCols3 },
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        // Make sure weakspot is not buried under boss mesh
        if (collision.gameObject.CompareTag("BossMesh"))
        {
            AdjustWeakSpot();
        }
    }

    // Make sure weakspot is not buried under boss mesh (Called on collision btwn boss and weak spot)
    void AdjustWeakSpot()
    {
        gameObject.transform.localPosition += (Vector3.forward * 0.1f);
    }

    // Call to change color of weak spot on attack (collision between laser and weakspot)
    public void TransformWeakSpot()
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr == null) return;
        Color WeakSpotCol = sr.color;

        int status = -1;

        foreach (KeyValuePair<int, Color> statCol in WeakSpotStatCol)
        {
            if (statCol.Value == WeakSpotCol)
            {
                status = statCol.Key;
                break;
            }
        }

        if (status == 3 || status == -1) return;
        GameManager.inst.AddScore(3000);
        if (weakSpotSound != null)
        {
            AudioSource.PlayClipAtPoint(weakSpotSound, gameObject.transform.position, weakSpotVolume);
        }
        bossStageManager.weakspotHitCount += 1;
        if (bossStageManager.weakspotHitCount % 9 == 0)
        {
            bossStageManager.IncrementPhase();
        }
        StartCoroutine(gradualColorChange(sr, sr.color, WeakSpotStatCol[status + 1]));
    }

    // Called by TransformWeakSpot()
    IEnumerator gradualColorChange(SpriteRenderer sr, Color startCol, Color endCol)
    {
        float elapsedTime = 0f;
        float duration = 0.1f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            if (sr != null)
                sr.color = Color.Lerp(startCol, endCol, elapsedTime / duration);


            yield return null;
        }

        if (sr != null) sr.color = endCol;
    }
}
