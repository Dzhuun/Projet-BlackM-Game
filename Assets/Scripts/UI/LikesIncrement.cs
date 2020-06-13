using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class LikesIncrement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public bool positiveIncrement;
    
    private float _heldDelay = 0.5f;
    private float _speed = 1;
    private float _clickTime = 0;
    private float _incrementValue = 0;
    private bool _hover = false;

    private void Awake()
    {
        _incrementValue = positiveIncrement ? 1 : -1;
    }

    private void Update()
    {
        if(_hover)
        {
            if (Input.GetMouseButton(0))
            {
                if(_clickTime == 0)
                {
                    _clickTime = Time.realtimeSinceStartup;
                }
            }
            else
            {
                _clickTime = 0;
            }

            // Check if the press duration is over the threshold
            if (Input.GetMouseButton(0) && Time.realtimeSinceStartup - _clickTime > _heldDelay)
            {
                IncrementNote();

                // Reduce the increment speed by delaying the clickTime value
                _clickTime += 0.1f * _speed;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IncrementNote();
    }

    /// <summary>
    /// Increments or decrements the note value.
    /// </summary>
    private void IncrementNote()
    {
        LikesIncrementHandler.IncrementNote(_incrementValue); 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _clickTime = 0;

        _hover = false;
    }
}
