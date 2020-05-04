using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkPointerInteractable))]
public class NetworkHighlightable : MonoBehaviour
{
  public Color highlightColor = Color.grey;
  Color startColor;
  SpriteRenderer spriteRenderer;

  void Start()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();
    startColor = spriteRenderer.color;
  }


  void OnPointerEnter(PlayerCursor pointer)
  {
    spriteRenderer.color = highlightColor;
  }

  void OnPointerOver(PlayerCursor pointer)
  {
    spriteRenderer.color = highlightColor;
  }

  void OnPointerExit(PlayerCursor pointer)
  {
    spriteRenderer.color = startColor;
  }
}
