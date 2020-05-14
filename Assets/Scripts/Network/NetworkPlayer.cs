using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

// IPunObservable : dit que ce component est compatible avec une photon view
public class NetworkPlayer : MonoBehaviour, IPunObservable, IOnEventCallback
{
    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static NetworkPlayer LocalPlayerInstance;

    public int PlayerID = -1; // -1 means uninitialized

    public string PlayerName = "default";

    public Character character;

    public PlayerCursor cursor;

    private PhotonView photonView;
  
    void Awake()
    {
        GameManager.AddPlayer(this);

        photonView = GetComponent<PhotonView>();

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

        Debug.LogFormat("Instanciated player {0}", photonView.Owner.NickName);

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
        photonView.RPC("SetCharacter", RpcTarget.All, character.nickname, playerOrder);
    }

    /// <summary>
    /// Stores the character linked to the player and its turn order.
    /// </summary>
    /// <param name="name">The name of the character.</param>
    /// <param name="playerOrder">The turn order of the player.</param>
    [PunRPC]
    private void SetCharacter(string name, int playerOrder)
    {
        character = Database.characters.Find(x => x.nickname == name);
        GameManager.SetPlayerOrder(this, playerOrder);
    }





    int currentPlayer;
    public int[] scores = new int[10];

    void OnButtonNextPlayer()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            currentPlayer++;
            SendNextPlayerID();
        }
    }

    void OnButtonLike()
    {
        SendLike(1);
    }

    void OnButtonDislike()
    {
        SendLike(-1);
    }

    void SendLike(int value)
    {
        byte evCode = 3; // Custom Event 1: Used as "MoveUnitsToTargetPosition" event
        object[] content = new object[] { value }; // Array contains the target position and the IDs of the selected units

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        //raiseEventOptions.CachingOption = EventCaching.AddToRoomCacheGlobal;
        raiseEventOptions.Receivers = ReceiverGroup.All;
        SendOptions sendOptions = new SendOptions();
        sendOptions.DeliveryMode = DeliveryMode.Reliable;
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }

    void SendNextPlayerID()
    {
        byte evCode = 2; // Custom Event 1: Used as "MoveUnitsToTargetPosition" event
        object[] content = new object[] { currentPlayer }; // Array contains the target position and the IDs of the selected units

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        //raiseEventOptions.CachingOption = EventCaching.AddToRoomCacheGlobal;
        raiseEventOptions.Receivers = ReceiverGroup.Others;
        SendOptions sendOptions = new SendOptions();
        sendOptions.DeliveryMode = DeliveryMode.Reliable;
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }


    void SendPlayerID(string PlayerName, int ID)
    {
        Debug.LogFormat("master client sending event to players : {0} will be player {1}", PlayerName, ID);
        byte evCode = 1; // Custom Event 1: Used as "MoveUnitsToTargetPosition" event
        object[] content = new object[] { PlayerName, ID }; // Array contains the target position and the IDs of the selected units

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        raiseEventOptions.CachingOption = EventCaching.AddToRoomCacheGlobal;
        raiseEventOptions.Receivers = ReceiverGroup.All;
        SendOptions sendOptions = new SendOptions();
        sendOptions.DeliveryMode = DeliveryMode.Reliable;
        PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
    }


    public void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }


    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == 1)
        {
            Debug.LogFormat("client {0} received event {1}", photonView.Owner.NickName, photonEvent.Code);
            object[] data = (object[])photonEvent.CustomData;

            string PlayerName = (string)data[0];
            int playerID = (int)data[1];
      
            // get all player instances and set the correct ID
            NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();
            for (int i = 0; i < players.Length; i++)
            {
                if(players[i].photonView.Owner.NickName == PlayerName)
                {
                    players[i].PlayerID = playerID;
                    players[i].transform.GetComponentInChildren<PlayerCursor>().playerID = playerID;
                    break;
                }
            }
        }


        if (eventCode == 2) // next player
        {
            Debug.LogFormat("client {0} received event {1}", photonView.Owner.NickName, photonEvent.Code);
            object[] data = (object[])photonEvent.CustomData;
            currentPlayer = (int)data[0];
        }


        if (eventCode == 3) // like/dislike
        {
            Debug.LogFormat("client {0} received event {1}", photonView.Owner.NickName, photonEvent.Code);
            object[] data = (object[])photonEvent.CustomData;
            int val = (int)data[0];
            scores[currentPlayer] += val;
        }
    }

    // IPunObservable function
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
