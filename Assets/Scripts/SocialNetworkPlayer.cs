using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;

public class SocialNetworkPlayer : MonoBehaviour, IOnEventCallback
{
  int MAX_PLAYER = 4;
  int currentPlayer = 0;

  public int[] scores;
  public Text[] scoresText;

  PhotonView photonView;

  byte NET_MSG_LIKE = 3;


  void Awake()
  {
    photonView = GetComponent<PhotonView>();

    scores = new int[MAX_PLAYER];
    scoresText = new Text[MAX_PLAYER];
  }

  public void OnEnable()
  {
    PhotonNetwork.AddCallbackTarget(this);
  }

  public void OnDisable()
  {
    PhotonNetwork.RemoveCallbackTarget(this);
  }

  void Update()
  {
    if(photonView.IsMine)
    {
      // !!!!!!!!!! TROUVER LE BON ENDROIT où récuppérer les component text
      // (OnPlayerEnteredRoom ? (Photon))
      for (int i = 0; i < MAX_PLAYER; i++)
      {
        scoresText[i] = GameObject.Find("Score_"+i).GetComponent<Text>();
      }

      NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();
      for (int i = 0; i < players.Length; i++)
      {
        int id = players[i].PlayerID;
        string playerName = players[i].PlayerName;

        GameObject.Find("Player_"+id).GetComponent<Text>().text = playerName;
      }


      for (int i = 0; i < MAX_PLAYER; i++)
      {
        scoresText[i].text = scores[i].ToString();
        if(i == currentPlayer)
        {
          scoresText[i].color = Color.green;
        }else
        {
          scoresText[i].color = Color.white;
        }
      }
    }
  }


  public void OnUpVoteButton()
  {
    //scores[currentPlayer] += 1;
    SendLike(1);
  }

  public void OnDownVoteButton()
  {
    //scores[currentPlayer] -= 1;
    SendLike(-1);
  }

  public void OnNextPlayerButton()
  {
    if(PhotonNetwork.IsMasterClient)
    {
      SendNextPlayer();
    }
  }

  void SendNextPlayer()
  {
    currentPlayer += 1;
    currentPlayer = currentPlayer % MAX_PLAYER;

    byte evCode = 2;
    object[] content = new object[] { currentPlayer }; // Array contains the target position and the IDs of the selected units
    RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
    //raiseEventOptions.CachingOption = EventCaching.AddToRoomCacheGlobal;
    raiseEventOptions.Receivers = ReceiverGroup.All;
    SendOptions sendOptions = new SendOptions();
    sendOptions.DeliveryMode = DeliveryMode.Reliable;
    PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
  }


  void SendLike(int value)
  {
    byte evCode = NET_MSG_LIKE;
    object[] content = new object[] { value }; // Array contains the target position and the IDs of the selected units

    RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
    //raiseEventOptions.CachingOption = EventCaching.AddToRoomCacheGlobal;
    raiseEventOptions.Receivers = ReceiverGroup.All;
    SendOptions sendOptions = new SendOptions();
    sendOptions.DeliveryMode = DeliveryMode.Reliable;
    PhotonNetwork.RaiseEvent(evCode, content, raiseEventOptions, sendOptions);
  }



  public void OnEvent(EventData photonEvent)
  {
    byte eventCode = photonEvent.Code;


    if (eventCode == 2) // next player
    {
      Debug.LogFormat("client {0} received event {1}", photonView.Owner.NickName, photonEvent.Code);
      object[] data = (object[])photonEvent.CustomData;
      currentPlayer = (int)data[0];
    }


    if (eventCode == NET_MSG_LIKE) // like/dislike
    {
      object[] data = (object[])photonEvent.CustomData;
      int val = (int)data[0];
      Debug.LogFormat("client {0} received event {1} with data {2}", photonView.Owner.NickName, photonEvent.Code, val);
      scores[currentPlayer] += val;
    }
  }
}
