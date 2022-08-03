using HarmonyLib;
using UnityEngine;

namespace NewPlayerTypes.Patches;

public class TimeEventPatch
{
    public static bool IntroFinished;
    
    public static void Patch(Harmony harmonyInstance)
    {
        var stopAndHideMethod = AccessTools.Method(typeof(TimeEvent), "StopAndHide");
        var stopAndHideMethodPostfix = new HarmonyMethod(typeof(TimeEventPatch).GetMethod(nameof(StopAndHideMethodPostfix)));
        harmonyInstance.Patch(stopAndHideMethod, postfix: stopAndHideMethodPostfix);
    }

    public static void StopAndHideMethodPostfix(TimeEvent __instance)
    {
        if (__instance.transform.gameObject.name == "Intro" && !Object.FindObjectOfType<CharacterSwitcherMenu>())
        {
            new GameObject("SwitcherHandler", typeof(CharacterSwitcherMenu));
            IntroFinished = true;
        }
    }
}