using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkPointerInteractable))]
public class NetworkSelectable : MonoBehaviour
{
  public Color selectColor = Color.green;
  public bool selected = false;
  Color startColor;
  SpriteRenderer spriteRenderer;

  void Start()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();
    startColor = spriteRenderer.color;
  }


  void OnPointerDown(PlayerCursor pointer)
  {
    if(pointer.playerID == 0)
    {
      // Debug.LogFormat("pointer down at frame {0} from player {1}", Time.frameCount, pointer.playerID);
      selected = !selected;
      if(selected) spriteRenderer.color = selectColor;
      else spriteRenderer.color = startColor;
    }
  }

  void OnPointerUp(PlayerCursor pointer)
  {
    
  }
}
