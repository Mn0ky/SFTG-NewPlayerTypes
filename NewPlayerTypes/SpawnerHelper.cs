using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace NewPlayerTypes;

public static class SpawnerHelper
{
    public static GameObject GetPlayerObject(byte playerType, GameObject playerPrefab)
    {
        if (playerType == 0)
        {
            playerPrefab = MultiplayerManagerAssets.Instance.PlayerPrefab;
            return playerPrefab;
        }

        var hoards = CharacterSwitcherMenu.Hoards;
        Debug.Log("Checking player type.");
        
        if (hoards[0] != null && hoards[1] != null)
        {
            playerPrefab = hoards[playerType - 1].character.gameObject;
            return playerPrefab;
        }    
        
        foreach (var hoard in Resources.FindObjectsOfTypeAll<HoardHandler>())
        {
            if (hoard.name == "AI spawner (1)") hoards[0] = hoard; // Bolt
            if (hoard.name == "AI spawner (2)") hoards[1] = hoard; // Zombie
        }
            
        Debug.Log("Trying to find child objs!!");
            
        var predictionSyncCubeTest = playerPrefab.GetComponentsInChildren<MeshFilter>(true)
            .First(child => child.gameObject.name is "PredictionSyncCubeTest(Clone)" or "PredictionSyncCubeTest")
            .gameObject;
        
        var chat =  playerPrefab.GetComponentInChildren<ChatManager>().gameObject;
        var gameCanvas = playerPrefab.GetComponentInChildren<BossHealth>().gameObject.transform.parent.gameObject;
        var damageParticleObj = playerPrefab.GetComponentInChildren<BlockParticle>().transform.GetChild(0).GetComponent<ParticleSystem>();

        Debug.Log("All objs found");
    
        foreach (var hoard in hoards)
        {
            var character = hoard.character;
            var characterTransform = hoard.character.transform;
            
            var newCubeTest = Object.Instantiate(predictionSyncCubeTest, characterTransform, true);
            Object.Instantiate(chat, characterTransform, false);
            Object.Instantiate(gameCanvas, characterTransform, true);
            Object.Instantiate(damageParticleObj, characterTransform.GetComponentInChildren<BlockParticle>().transform, true);

            Traverse.Create(character.FetchComponent<NetworkPlayer>())
                .Field("mHelpPredictionSphere")
                .SetValue(newCubeTest.transform);
            
            // Adds component if missing so map switching coroutine wont have null references
            character.FetchComponent<SetMovementAbility>();
            
            // Gives special characters normal player jump sounds to prevent null references
            character.GetComponent<Movement>().jumpClips = playerPrefab.GetComponent<Movement>().jumpClips;
            
            foreach (var particleSys in character.GetComponentsInChildren<ParticleSystem>())
            {
                if (particleSys.name != "punchPartilce") continue;
                
                var particleSysMain = particleSys.main;
                particleSysMain.startColor = (Color) new Color32(45, 45, 45, 255);
                break;
            }
        }
        
        Debug.Log("Changed player prefab!!");
        playerPrefab = hoards[playerType - 1].character.gameObject;
        return playerPrefab;
    }
}