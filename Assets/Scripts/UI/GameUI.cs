using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Core")]
    public Animator animatorUI;

    [Header("Start")]
    public GameObject charactersDisplay;
    public List<Image> charactersImage;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayCharacters()
    {
        animatorUI.SetTrigger("DisplayCharacter");
    }
}
