using System;
using System.IO;
using System.Linq;
using HarmonyLib;
using Steamworks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NewPlayerTypes.Patches;

class MultiplayerManagerPatches
{
    public static void Patches(Harmony harmonyInstance)
    {
        var addClientToListMethod = AccessTools.Method(typeof(MultiplayerManager), "AddClientToList");
        var addClientToListMethodPrefix = new HarmonyMethod(typeof(MultiplayerManagerPatches)
            .GetMethod(nameof(AddClientToListMethodPrefix)));
        harmonyInstance.Patch(addClientToListMethod, prefix: addClientToListMethodPrefix);
        
        var initDataFromServerRecievedMethod = AccessTools.Method(typeof(MultiplayerManager), "InitDataFromServerRecieved");
        var initDataFromServerRecievedMethodPrefix = new HarmonyMethod(typeof(MultiplayerManagerPatches)
            .GetMethod(nameof(InitDataFromServerRecievedMethodPrefix)));
        harmonyInstance.Patch(initDataFromServerRecievedMethod, prefix: initDataFromServerRecievedMethodPrefix);
        
        var requestSpawnPlayerMethod = AccessTools.Method(typeof(MultiplayerManager), "RequestSpawnPlayer");
        var requestSpawnPlayerMethodPrefix = new HarmonyMethod(typeof(MultiplayerManagerPatches)
            .GetMethod(nameof(RequestSpawnPlayerMethodPrefix)));
        harmonyInstance.Patch(requestSpawnPlayerMethod, prefix: requestSpawnPlayerMethodPrefix);

        var onPlayerRequestingToSpawnMethod = AccessTools.Method(typeof(MultiplayerManager), 
            "OnPlayerRequestingToSpawn");
        var onPlayerRequestingToSpawnMethodPrefix = new HarmonyMethod(typeof(MultiplayerManagerPatches)
            .GetMethod(nameof(OnPlayerRequestingToSpawnPrefix)));
        harmonyInstance.Patch(onPlayerRequestingToSpawnMethod, prefix: onPlayerRequestingToSpawnMethodPrefix);
        
        var onPlayerSpawnedMethod = AccessTools.Method(typeof(MultiplayerManager), "OnPlayerSpawned");
        var onPlayerSpawnedMethodPrefix = new HarmonyMethod(typeof(MultiplayerManagerPatches)
            .GetMethod(nameof(OnPlayerSpawnedMethodPrefix)));
        harmonyInstance.Patch(onPlayerSpawnedMethod, prefix: onPlayerSpawnedMethodPrefix);
        
        var spawnPlayerDummyMethod = AccessTools.Method(typeof(MultiplayerManager), "SpawnPlayerDummy");
        var spawnPlayerDummyMethodPrefix = new HarmonyMethod(typeof(MultiplayerManagerPatches)
            .GetMethod(nameof(SpawnPlayerDummyMethodPrefix)));
        harmonyInstance.Patch(spawnPlayerDummyMethod, prefix: spawnPlayerDummyMethodPrefix);
    }

    public static bool AddClientToListMethodPrefix(MultiplayerManager __instance, ref CSteamID newClient,
        ref bool SendCallBackToClient, ref bool guest, ref CSteamID ___mUnassignedID)
    {
        byte playerID = 0;
        var clients = GameManager.Instance.mMultiplayerManager.ConnectedClients;
        while (playerID < clients.Length)
        {
            if (clients[playerID] != null && clients[playerID].ClientID == newClient && !guest)
            {
                Debug.LogError("Client: " + newClient + " Is Already In The Server!");
                return false;
            }
            playerID += 1;
        }
        
        playerID = 0;
        while (playerID < clients.Length)
        {
            if (clients[playerID] == null || clients[playerID].ClientID == ___mUnassignedID)
            {
                var array = new byte[9];
                using (var memoryStream = new MemoryStream(array))
                {
                    using (var binaryWriter = new BinaryWriter(memoryStream))
                    {
                        binaryWriter.Write(playerID);
                        binaryWriter.Write(newClient.m_SteamID);
                    }
                }
                
                var sendMessageToAllClientsMethod = AccessTools.Method(typeof(MultiplayerManager), "SendMessageToAllClients");
                sendMessageToAllClientsMethod.Invoke(__instance,
                    new object[]
                    {
                        array,
                        P2PPackageHandler.MsgType.ClientJoined,
                        true,
                        0UL,
                        EP2PSend.k_EP2PSendReliable,
                        0
                    });
                
                var connectedClientData = new ConnectedClientData
                {
                    Stats = new ClientStats(),
                    ClientID = newClient,
                    Ready = false
                };
                
                clients[playerID] = connectedClientData;
                
                if (SendCallBackToClient)
                {
                    var prefixData = new byte[] { 1, playerID };
                    var getStatusDataMethod = AccessTools.Method(typeof(MultiplayerManager), "GetStatusData");
                    var statusData = (byte[]) getStatusDataMethod.Invoke(__instance, new object[]{ prefixData });
                    
                    var finalData = new byte[statusData.Length + 1];
                    statusData.CopyTo(finalData, 0);
                    finalData[finalData.Length - 1] = CharacterSwitcherMenu.LocalPlayerType; // Sends host player type
                    
                    Debug.Log("sending host player type: " + CharacterSwitcherMenu.LocalPlayerType);
                        
                    P2PPackageHandler.Instance.SendP2PPacketToUser(newClient, finalData, P2PPackageHandler.MsgType.ClientInit);
                }
                
                Debug.Log(string.Concat("Added New client to list: ", clients[playerID].PlayerName, " Send Callback: ",
                    SendCallBackToClient));
                break;
            }
            playerID += 1;
        }

        return false;
    }

    public static void InitDataFromServerRecievedMethodPrefix(ref byte[] data)
    {
        CharacterSwitcherMenu.ReceivedPlayerType = data[data.Length - 1];
        Debug.Log("Received host player type: " + CharacterSwitcherMenu.ReceivedPlayerType);
    }

    public static void SpawnPlayerDummyMethodPrefix(ref byte i, ref GameObject ___m_PlayerPrefab)
    {
        Debug.Log("SpawnPlayerDummyMethod!!!!");

        var playerID = i;
        var playerType = CharacterSwitcherMenu.ReceivedPlayerType; // playerType is host playerType

        Debug.Log("Got Dummy player type: " + playerType + " and Dummy ID: " + playerID);
        ___m_PlayerPrefab = SpawnerHelper.GetPlayerObject(playerType, ___m_PlayerPrefab);
    }

    public static bool RequestSpawnPlayerMethodPrefix(MultiplayerManager __instance, ref bool isInLobby)
    {
        if (MultiplayerManager.IsServer)
        {
            var spawnPlayerMethod = AccessTools.Method(typeof(MultiplayerManager), "SpawnPlayer");
            spawnPlayerMethod.Invoke(__instance, null);
            Debug.Log("Server Called Client Function 'Request Player Spawn', Intentional'? Spawning Player Immediately instead...");
            return false;
        }
        
        if (GameManager.Instance.mMultiplayerManager.LocalPlayerIndex < 0)
            throw new Exception("Attempting to request when no index has been set, wait for server response!");

        var vector = !isInLobby ? new Vector3(0f, 12f, 0f) : new Vector3(0f, 0f, 0f);
        var eulerAngles = Quaternion.identity.eulerAngles;
        byte playerID = 0;

        var clients = GameManager.Instance.mMultiplayerManager.ConnectedClients;
        while (playerID < clients.Length)
        {
            if (clients[playerID] != null && clients[playerID].ControlledLocally)
            {
                var array = new byte[26]; // One extra byte for local playerType
                using (var memoryStream = new MemoryStream(array))
                {
                    using (var binaryWriter = new BinaryWriter(memoryStream))
                    {
                        binaryWriter.Write(playerID);
                        binaryWriter.Write(vector.x);
                        binaryWriter.Write(vector.y);
                        binaryWriter.Write(vector.z);
                        binaryWriter.Write(eulerAngles.x);
                        binaryWriter.Write(eulerAngles.y);
                        binaryWriter.Write(eulerAngles.z);
                        binaryWriter.Write(CharacterSwitcherMenu.LocalPlayerType);
                    }
                }
                
                Debug.Log($"Sending Request to spawn to the server at pos: {vector} Rotation: {eulerAngles}");
                Debug.Log("Sending our local player type: " + CharacterSwitcherMenu.LocalPlayerType);
                P2PPackageHandler.Instance.SendP2PPacketToUser(MatchmakingHandler.Instance.LobbyOwner,
                    array,
                    P2PPackageHandler.MsgType.ClientRequestingToSpawn);
            }
            playerID += 1;
        }

        return false;
    }
    
    public static bool OnPlayerRequestingToSpawnPrefix(MultiplayerManager __instance, ref byte[] data)
    {
        Debug.Log("Running ONPLAYERREQUESTINGTOSPAWN!!!");
        
        var playerType = data[data.Length - 1];
        
        var fullData = new byte[data.Length + 1]; // Keep +1 as we already have an extra byte from playerType before
        var isMoreThan1Player = !GameManager.Instance.IsInLobby() && __instance.GetPlayersInLobby(true) > 1;

        using (var memoryStream = new MemoryStream(fullData))
        {
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(data.Take(data.Length - 1).ToArray()); // Remove playerType byte so it's at back
                binaryWriter.Write(isMoreThan1Player);
                binaryWriter.Write(playerType); // Re-add the playerType as the last byte
            }
        }

        var sendMessageToAllClientsMethod = AccessTools.Method(typeof(MultiplayerManager), "SendMessageToAllClients");
        
        sendMessageToAllClientsMethod.Invoke(__instance, 
            new object[]
            {
                fullData,
                P2PPackageHandler.MsgType.ClientSpawned,
                false,
                0UL,
                EP2PSend.k_EP2PSendReliable,
                0
            });
    
        return false;
    }

    public static void OnPlayerSpawnedMethodPrefix(ref byte[] data, ref GameObject ___m_PlayerPrefab)
    {
        Debug.Log("RUNNING OnPlayerSpawned NOW!!!!!");

        var playerID = data[0];
        // If not local user then playerType is last byte in array
        var playerType = playerID == GameManager.Instance.mMultiplayerManager.LocalPlayerIndex
            ? CharacterSwitcherMenu.LocalPlayerType
            : data[data.Length - 1];
        
        Debug.Log("Got player type: " + playerType + " and ID: " + playerID);
        ___m_PlayerPrefab = SpawnerHelper.GetPlayerObject(playerType, ___m_PlayerPrefab);

        ___m_PlayerPrefab.GetComponentInChildren<ChatManager>().GetComponent<FollowTransform>().target =
            ___m_PlayerPrefab.GetComponentInChildren<Head>().transform;
    }
}