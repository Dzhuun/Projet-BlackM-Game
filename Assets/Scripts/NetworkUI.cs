using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkUI : MonoBehaviourPunCallbacks
{
  public Text roomInfos;
  public InputField nicknameField;
  public Button connectButton;

  ICAN.NetworkManager networkManager;

  void Awake()
  {
    networkManager = FindObjectOfType<ICAN.NetworkManager>();
  }

  void Start()
  {
    nicknameField.text = networkManager.nickname;
  }

  void Update()
  {
    roomInfos.text = "";
    UpdateRoomUI();
  }

  void UpdateRoomUI()
  {
    if(PhotonNetwork.InRoom)
    {
      string s = "<b>PLAYERS IN ROOM :</b>\n";
      foreach(var player in PhotonNetwork.CurrentRoom.Players)
      {
        s += "#"+player.Value.NickName + "\n";
      }
      roomInfos.text += s;
    }
  }

  public void OnNicknameUpdated(string newNickname)
  {
    Debug.LogFormat("nickname updated to {0}", newNickname);
    networkManager.nickname = newNickname;

    //// TO CHANGE FOR HANDLING OTHER PLAYERS
    FindObjectOfType<PlayerCursor>().UpdateName(networkManager.nickname);
  }

  public void OnConnectButton()
  {
    networkManager.Connect();
  }








  ////////// NetworkManager
  public override void OnConnected()
  {
    // hide connect button and input field
    connectButton.gameObject.SetActive(false);
    GameObject.Find("Nickname").SetActive(false);
  }
}
