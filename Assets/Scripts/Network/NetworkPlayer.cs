using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

// IPunObservable : dit que ce component est compatible avec une photon view
public class NetworkPlayer : MonoBehaviourPunCallbacks
{
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static NetworkPlayer LocalPlayerInstance;

    public int PlayerID = -1; // -1 means uninitialized

    public string PlayerName = "default";

    public Character character;

    public PlayerCursor cursor;

    public float popularity = 2;

    public int likes = 0;

    public int orderIndex = 0;
  
    void Awake()
    {
        PlayerID = photonView.Owner.ActorNumber;

        GameManager.AddPlayer(this);
        
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this;

            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                {SettingsManager.KEY_PLAYER_LOADED_LEVEL, true}
            };

            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        //DontDestroyOnLoad(this.gameObject);

        PlayerName = photonView.Owner.NickName;
        gameObject.name = PlayerName;

        // Add ourself to InputManager
        //cursor = GetComponentInChildren<PlayerCursor>();
        //NetworkInputManager.playerInputs.Add(cursor);

        //if(PhotonNetwork.IsMasterClient)
        //{
        //    SendPlayerID(photonView.Owner.NickName, PhotonNetwork.CurrentRoom.PlayerCount-1);
        //}
    }

    /// <summary>
    /// Send the player's state update through network.
    /// </summary>
    /// <param name="character">The character assigned to the player.</param>
    /// <param name="playerOrder">The turn order assigned to the player.</param>
    public void SetCharacter(Character character, int playerOrder)
    {
        photonView.RPC("SetCharacter", RpcTarget.AllBuffered, character.nickname, playerOrder);
    }

    /// <summary>
    /// Stores the character linked to the player and its turn order.
    /// </summary>
    /// <param name="name">The name of the character.</param>
    /// <param name="playerOrder">The turn order of the player.</param>
    [PunRPC]
    private void SetCharacter(string name, int playerOrder)
    {
        Debug.LogError($"Set character {name} at order '{playerOrder}'");

        character = Database.characters.Find(x => x.nickname == name);
        GameManager.SetPlayerOrder(this, playerOrder);
    }
    
}
