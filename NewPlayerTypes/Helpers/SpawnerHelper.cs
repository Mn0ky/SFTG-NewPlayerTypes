using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace NewPlayerTypes.Helpers;

public static class SpawnerHelper
{
    private static readonly Color SpecialCharacterParticleColor = new Color32(45, 45, 45, 255);

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
        
        // Instantiate the following
        var chat =  playerPrefab.GetComponentInChildren<ChatManager>().gameObject;
        var gameCanvas = playerPrefab.GetComponentInChildren<BossHealth>().gameObject.transform.parent.gameObject;
        var damageParticleObj = playerPrefab.GetComponentInChildren<BlockParticle>().transform.GetChild(0).GetComponent<ParticleSystem>();

        var playerAudioMixerGroup = playerPrefab.GetComponentInChildren<AudioSource>().outputAudioMixerGroup;

        Debug.Log("All objs found");    
    
        foreach (var hoard in hoards)
        {
            var character = hoard.character;
            var characterTransform = hoard.character.transform;

            character.GetComponentInChildren<AudioSource>().outputAudioMixerGroup = playerAudioMixerGroup;
            
            var newCubeTest = Object.Instantiate(predictionSyncCubeTest, characterTransform, true);
            Object.Instantiate(chat, characterTransform, false);
            Object.Instantiate(gameCanvas, characterTransform, true);
            Object.Instantiate(damageParticleObj, characterTransform.GetComponentInChildren<BlockParticle>().transform, true);

            Traverse.Create(character.FetchComponent<NetworkPlayer>())
                .Field("mHelpPredictionSphere")
                .SetValue(newCubeTest.transform);
            
            // Adds SetMovementAbility component if missing so map switching coroutine wont have null references
            character.FetchComponent<SetMovementAbility>();
            
            // Gives special characters normal player jump sounds to prevent null references
            character.GetComponent<Movement>().jumpClips = playerPrefab.GetComponent<Movement>().jumpClips;
            
            // Chat messages need to follow the correct head transform (not the player prefab's)
            character.GetComponentInChildren<ChatManager>().GetComponent<FollowTransform>().target = 
                character.GetComponentInChildren<Head>().transform;
            
            // Make sure all particles are special character black 
            foreach (var particleSys in character.GetComponentsInChildren<ParticleSystem>())
            {
                if (particleSys.name != "punchPartilce") continue;
                
                var particleSysMain = particleSys.main;
                particleSysMain.startColor = SpecialCharacterParticleColor;
                break;
            }

            if (character.name == "ZombieCharacterArms")
            {
                character.AddComponent<GrabTriggerer>(); // Let zombie be able to grab for balancing
                character.GetComponent<GrabHandler>().grabClips = playerPrefab.GetComponent<GrabHandler>().grabClips;

                character.GetComponent<Fighting>().punchForce = 12000; // Roundabout way of fixing buggy flight when punching
            }
        }
        
        Debug.Log("Changed player prefab!!");
        playerPrefab = hoards[playerType - 1].character.gameObject;
        return playerPrefab;
    }
}