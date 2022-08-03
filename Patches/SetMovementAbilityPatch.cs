using HarmonyLib;

namespace NewPlayerTypes.Patches;

public class SetMovementAbilityPatch
{
    public static void Patch(Harmony harmonyInstance)
    {
        var startMethod = AccessTools.Method(typeof(SetMovementAbility), "Start");
        var startMethodPrefix = new HarmonyMethod(typeof(SetMovementAbilityPatch).GetMethod(nameof(StartMethodPrefix)));
        harmonyInstance.Patch(startMethod, prefix: startMethodPrefix);
        
        var resetMethod = AccessTools.Method(typeof(SetMovementAbility), "Reset");
        var resetMethodPrefix = new HarmonyMethod(typeof(SetMovementAbilityPatch).GetMethod(nameof(ResetMethodPrefix)));
        harmonyInstance.Patch(resetMethod, prefix: resetMethodPrefix);
    }
    
    public static bool StartMethodPrefix(SetMovementAbility __instance) 
        => __instance.transform.root.name is not ("ZombieCharacter(Clone)" or "ZombieCharacterArms(Clone)");

    public static bool ResetMethodPrefix(SetMovementAbility __instance) 
        => __instance.transform.root.name is not ("ZombieCharacter(Clone)" or "ZombieCharacterArms(Clone)");
}