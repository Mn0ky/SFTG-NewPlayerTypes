using HarmonyLib;
using UnityEngine;

namespace NewPlayerTypes.Patches;

public class ControllerHandlerPatch
{
    public static void Patch(Harmony harmonyInstance)
    {
        var createPlayerMethod = AccessTools.Method(typeof(ControllerHandler), "CreatePlayer");
        var createPlayerMethodPrefix = new HarmonyMethod(typeof(ControllerHandlerPatch).GetMethod(nameof(CreatePlayerMethodPrefix)));
        harmonyInstance.Patch(createPlayerMethod, prefix: createPlayerMethodPrefix);
    }

    public static bool CreatePlayerMethodPrefix(ControllerHandler __instance, ref GameObject ___playerPrefab)
    {
        if (__instance.players.Count >= 4 || !CharacterSwitcherMenu.HasSelectedCharacter) return false;
        
        ___playerPrefab = SpawnerHelper.GetPlayerObject(CharacterSwitcherMenu.LocalPlayerType, ___playerPrefab);
        
        return true;
    }
}