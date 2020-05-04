using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NetworkPointerInteractable : MonoBehaviour
{
  Dictionary<int, bool> lastCursorOverState = new Dictionary<int, bool>();

  Collider col;

  public delegate void OnNetworkPointerInteraction(PlayerCursor pointer);
  public event OnNetworkPointerInteraction onPointerEnter;
  public event OnNetworkPointerInteraction onPointerOver;
  public event OnNetworkPointerInteraction onPointerExit;


  public void Start()
  {
    col = GetComponent<Collider>();
  }

  void Update()
  {
    for (int i = 0; i < NetworkInputManager.playerInputs.Count; i++)
    {
      PlayerCursor cursor = NetworkInputManager.playerInputs[i];
      if(cursor == null) continue; // may happen in the first frames of joining a room
      
      bool lastOverState = false;
      if(lastCursorOverState.ContainsKey(cursor.playerID)) lastOverState = lastCursorOverState[cursor.playerID];
        
      bool currentOverState = false;
      // check if cursor is inside collider
      Ray r = Camera.main.ScreenPointToRay(cursor.screenPosition);
      RaycastHit hit;
      if(col.Raycast(r, out hit, Mathf.Infinity))
      {
        currentOverState = true;
      }

      // On Enter
      if(lastOverState == false && currentOverState == true)
      {
        // if using delegate
        if(onPointerEnter != null) onPointerEnter(cursor);
        // if using Unity Component Messages
        SendMessage("OnPointerEnter", cursor, SendMessageOptions.DontRequireReceiver);
      }
      if(lastOverState == true && currentOverState == false)
      {
        // if using delegate
        if(onPointerExit != null) onPointerExit(cursor);
        // if using Unity Component Messages
        SendMessage("OnPointerExit", cursor, SendMessageOptions.DontRequireReceiver);
      }
      if(currentOverState == true)
      {
        // if using delegate
        if(onPointerOver != null) onPointerOver(cursor);
        // if using Unity Component Messages
        SendMessage("OnPointerOver", cursor, SendMessageOptions.DontRequireReceiver);

        if(cursor.downThisFrame)
        {
          SendMessage("OnPointerDown", cursor, SendMessageOptions.DontRequireReceiver);
        }

        if(cursor.upThisFrame)
        {
          SendMessage("OnPointerUp", cursor, SendMessageOptions.DontRequireReceiver);
        }
      }
      lastCursorOverState[cursor.playerID] = currentOverState;
    }
  }
}
