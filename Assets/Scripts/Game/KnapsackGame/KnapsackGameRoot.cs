using System.Collections;
using System.Collections.Generic;
using SthGame;
using UnityEngine;

public class KnapsackGameRoot : GameRoot
{
    protected override void OnStartGame()
    {
        GUIManager.Instance.Open<KnapsackGameController>();
    }
}
