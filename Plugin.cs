using System;
using BepInEx;
using HarmonyLib;
using NewPlayerTypes.Patches;

namespace NewPlayerTypes
{
    [BepInPlugin(Guid, Name, Version)]
    [BepInProcess("StickFight.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private const string Guid = "monky.plugins.NewPlayerClasses";
        private const string Name = "NewPlayerClasses";
        private const string Version = "1.0.0";
        
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {Guid} is loaded!");
            try
            {
                var harmony = new Harmony(Guid);
                
                Logger.LogInfo("Applying MultiplayerManager patches...");
                MultiplayerManagerPatches.Patches(harmony);
                Logger.LogInfo("Applying GameManager patch...");
                GameManagerPatch.Patch(harmony);
                Logger.LogInfo("Applying SetMovementAbility patch...");
                SetMovementAbilityPatch.Patch(harmony);
                Logger.LogInfo("Applying ControllerHandler patch...");
                ControllerHandlerPatch.Patch(harmony);
                Logger.LogInfo("Applying DisableIfPlayed patch...");
                TimeEventPatch.Patch(harmony);
            }
            catch (Exception e)
            {
                Logger.LogInfo(e);
            }
        }
    }
}
