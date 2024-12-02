using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightstickManager : BeneficialObject
{
    private bool isLightstickItemPlaying = false;
    // Update is called once per frame
    protected override void Update()
    {
        // Update lightstick positions to maintain relative positioning
        UpdateLightstickPositions(player.transform.position);
        if (!isLightstickItemPlaying)
        {
            base.Update();
        }
        else
        {
            HideAndKeep();
        }
    }

    protected override void OnPlayerCollision(GameObject player)
    {
        StartCoroutine(TripleShotPowerUp());
    }

    IEnumerator TripleShotPowerUp()
    {
        playerControl.lightstickEndTime = 10.0f;
        if (playerControl.GetIsTripleShot())
        {
            Destroy(gameObject);
            yield break;
        }
        isLightstickItemPlaying = true;
        playerControl.SetTripleShot(true);
        UpdateLightstickPositions(player.transform.position);
        while (1 < playerControl.lightstickEndTime)
        {
            Debug.Log($"playerControl.lightstickEndTime: {playerControl.lightstickEndTime}");
            playerControl.lightstickEndTime -= 1.0f;
            yield return new WaitForSeconds(0.9f);
        }
        Debug.Log("Exit while loop");
        playerControl.SetTripleShot(false);
        playerControl.ControlLightsticks();
        Destroy(gameObject);
    }

    public void UpdateLightstickPositions(Vector3 playerPosition)
    {
        if (!playerControl.GetIsTripleShot()) return;

        float spawnHeight = playerControl.projectileSpawnPoint.position.y;

        Vector2Int currentGridPosition = playerControl.GetCurrentGridPosition();
        Vector2Int gridSize = playerControl.GetGridSize();

        // Update left lightstick
        if (playerControl.leftLightstickPrefab != null)
        {
            bool shouldBeActive = currentGridPosition.x > 0;
            playerControl.leftLightstickPrefab.SetActive(shouldBeActive);
            if (shouldBeActive)
            {
                Vector3 leftPosition = new Vector3(
                    playerPosition.x - playerControl.lightstickOffset,
                    spawnHeight,  // Use spawn point height
                    playerPosition.z
                );
                playerControl.leftLightstickPrefab.transform.position = leftPosition;
            }
        }

        // Update right lightstick
        if (playerControl.rightLightstickPrefab != null)
        {
            bool shouldBeActive = currentGridPosition.x < gridSize.x - 1;
            playerControl.rightLightstickPrefab.SetActive(shouldBeActive);
            if (shouldBeActive)
            {
                Vector3 rightPosition = new Vector3(
                    playerPosition.x + playerControl.lightstickOffset,
                    spawnHeight,  // Use spawn point height
                    playerPosition.z
                );
                playerControl.rightLightstickPrefab.transform.position = rightPosition;
            }
        }
    }
}
