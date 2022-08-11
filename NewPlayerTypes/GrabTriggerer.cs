using System;
using NewPlayerTypes.Helpers;
using UnityEngine;

namespace NewPlayerTypes;

public class GrabTriggerer : MonoBehaviour
{
    private KeyCode _grabKey;
    private GrabHandler _grabHandler;
    private bool _isLocalUser;
    
    private void Start()
    {
        _grabHandler = gameObject.GetComponent<GrabHandler>();
        
        var key = Plugin.ConfigGrabKeybind.Value.ToEnum<KeyCode>();
        _grabKey = key != KeyCode.None ? key : KeyCode.Tab; // grabKey defaults to Tab if enum parsing fails

        if (!MatchmakingHandler.Instance.IsInsideLobby ||
            gameObject.GetComponent<NetworkPlayer>().NetworkSpawnID ==
            GameManager.Instance.mMultiplayerManager.LocalPlayerIndex)
        
            _isLocalUser = true;
    }

    private void Update()
    {
        if (_isLocalUser && Input.GetKeyDown(_grabKey)) _grabHandler.StartNewGrab();
    }
}