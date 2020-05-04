using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerCursor : MonoBehaviour, IPunObservable
{
  public int playerID;
  public Vector3 screenPosition;
  public Texture2D cursorTexture;

  public bool useHardwareCursor = true;

  public bool downThisFrame = false;
  public bool upThisFrame = false;
  int downUpdateFrame;
  int upUpdateFrame;

  RectTransform rectTransform;
  PhotonView photonView;


  void Awake()
  {
    photonView    = GetComponent<PhotonView>();
    rectTransform = GetComponent<RectTransform>();
    if(useHardwareCursor)
    {
      Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
      GetComponent<Image>().enabled = false;
    }else
    {
      Cursor.visible = false;
    }

    if(photonView.Owner != null)
    {
      UpdateName(photonView.Owner.NickName);
    }else
    {
      UpdateName(FindObjectOfType<ICAN.NetworkManager>().nickname);
    }
  }

  void Update()
  {
    if(downThisFrame && Time.frameCount > downUpdateFrame) downThisFrame = false;
    if(upThisFrame && Time.frameCount > upUpdateFrame) upThisFrame = false;

    if(photonView.IsMine || !PhotonNetwork.IsConnected)
    {
      // update data
      screenPosition = Input.mousePosition;

      // update UI
      rectTransform.position = screenPosition;

      if(Input.GetMouseButtonDown(0))
      {
        if(PhotonNetwork.IsConnected) photonView.RPC("PointerStateDown", RpcTarget.All);
        else PointerStateDown();
      }
      if(Input.GetMouseButtonUp(0))
      {
        if(PhotonNetwork.IsConnected) photonView.RPC("PointerStateUp", RpcTarget.All);
        else PointerStateUp();
      }
    }

    if(PhotonNetwork.IsConnected && photonView.Owner != null) UpdateName( string.Format("{0} ({1})", photonView.Owner.NickName,playerID.ToString()));
  }

  public void UpdateName(string newName)
  {
    transform.Find("PlayerName").GetComponent<Text>().text = newName;
  }

  [PunRPC]
  void PointerStateDown()
  {
    Debug.LogFormat("Player {0} pointer down at frame {1}", playerID, Time.frameCount);
    downThisFrame = true;
    downUpdateFrame = Time.frameCount;
  }

  [PunRPC]
  void PointerStateUp()
  {
    Debug.LogFormat("Player {0} pointer up at frame {1}", playerID, Time.frameCount);
    upThisFrame = true;
    upUpdateFrame = Time.frameCount;
  }



  public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
  {
    if (stream.IsWriting)
    {
        // client resolutions may be different so we have to normalize position
        Vector3 normalizedPosition = Vector3.zero;
        normalizedPosition.x = this.screenPosition.x / Screen.width;
        normalizedPosition.y = this.screenPosition.y / Screen.height;
        // Debug.LogFormat("player {0} is sending normalized pointer position {1} from position {2} with screen w:{3} h:{4}", info.Sender.NickName, normalizedPosition, this.screenPosition, Screen.width, Screen.height);

        stream.SendNext(normalizedPosition);
        stream.SendNext(this.playerID);
    }
    else
    {
      Vector3 normalizedPosition = (Vector3)stream.ReceiveNext();
      // Debug.LogFormat("Cursor received position {0} from player {1}", normalizedPosition, info.Sender.NickName);

      // retransform to local screen coordinates
      normalizedPosition.x *= Screen.width; 
      normalizedPosition.y *= Screen.height;
      this.screenPosition = normalizedPosition;

      rectTransform.position = screenPosition;

      this.playerID = (int)stream.ReceiveNext();
    }
  }
}
