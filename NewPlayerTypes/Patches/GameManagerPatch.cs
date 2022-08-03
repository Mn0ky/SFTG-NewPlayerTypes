using System.Collections;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NewPlayerTypes.Patches;

public class GameManagerPatch
{
    public static void Patch(Harmony harmonyInstance)
    {
        var startMethod = AccessTools.Method(typeof(GameManager), "Start");
        var startMethodPostfix = new HarmonyMethod(typeof(GameManagerPatch).GetMethod(nameof(StartMethodPostfix)));
        harmonyInstance.Patch(startMethod, postfix: startMethodPostfix);
    }

    public static void StartMethodPostfix()
    {
        if (TimeEventPatch.IntroFinished && !Object.FindObjectOfType<CharacterSwitcherMenu>())
            new GameObject("SwitcherHandler", typeof(CharacterSwitcherMenu));
    }
}