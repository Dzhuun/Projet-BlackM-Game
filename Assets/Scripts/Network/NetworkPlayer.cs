using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

// IPunObservable : dit que ce component est compatible avec une photon view
public class NetworkPlayer : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// The local player instance. Use this to know if the local player is represented in the Scene.
    /// </summary>
    public static NetworkPlayer LocalPlayerInstance;

    /// <summary>
    /// The ID of the player.
    /// </summary>
    public int PlayerID = -1; // -1 means uninitialized

    /// <summary>
    /// The pseudonyme of the player.
    /// </summary>
    public string PlayerName;

    /// <summary>
    /// The character linked to the player.
    /// </summary>
    public Character character;

    /// <summary>
    /// The popularity of the player. Starts at 2, value between 0 and 5.
    /// </summary>
    public float popularity = 2;

    /// <summary>
    /// The amount of likes owned by the player.
    /// </summary>
    public int likes = 0;

    /// <summary>
    /// The mental health value of the player. Starts at 100, value between 0 and 100.
    /// </summary>
    public int mentalHealth = 100;

    /// <summary>
    /// The turn order index of the player.
    /// </summary>
    public int orderIndex = 0;

    //public GameManager GameManager
    //{
    //    get
    //    {
    //        if(_gameManager == null)
    //        {
    //            _gameManager = FindObjectOfType<GameManager>();
    //        }

    //        return _gameManager;
    //    }
    //}

    //private GameManager _gameManager;
  
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
    
    /// <summary>
    /// Gets the current sanity level of the player according to its mental health.
    /// </summary>
    /// <returns>The mental health level, from 0 to 4.</returns>
    public int GetMentalHealthLevel()
    {
        return mentalHealth % 20;
    }

    /// <summary>
    /// Resets the active state of the traits to true at the beginning of each turn.
    /// </summary>
    public void ResetActiveTraits()
    {
        foreach(CharacterTrait trait in character.traits)
        {
            trait.isActive = true;
        }
    }
}
