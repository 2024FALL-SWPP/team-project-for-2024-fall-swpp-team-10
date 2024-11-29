using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionMusicManager : MusicManager
{
    public AudioClip[] backgroundMusics;

    protected override void SetMusic()
    {
        musicToPlay = backgroundMusics[GameManager.inst.GetStage() - 1];
    }
}
