using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameQualityInitializer : Singleton<GameQualityInitializer>
{
    protected override void AwakeSingleton()
    {
        GameOptions.CurrentQuality = 4; // sets game quality to Ultra by default
    }
}
