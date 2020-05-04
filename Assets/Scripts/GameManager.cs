using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
  public GameObject playerPrefab;

  


  void Start()
  {
    if(NetworkPlayer.LocalPlayerInstance == null)
    {
      GameObject playerGO = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
      playerGO.name = playerPrefab.name + "_" + PhotonNetwork.NickName;
    }
  }

  void Update()
  {
    UpdateGameState();  
  }



  enum GameState
  {
    Start,
    PlayerTurn
  };
  GameState state = GameState.Start;


  public void UpdateGameState()
  {
    switch(state)
    {
      case GameState.Start:
      // wait for player one to click on start button

      break;

      case GameState.PlayerTurn:
      break;
    }
  }
}
