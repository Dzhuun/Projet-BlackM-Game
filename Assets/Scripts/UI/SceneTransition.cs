using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public Animator transitionAnimator;

    public Lobby lobbyManager;

    public Image blurEffect;
    
    public float currentStrength = 0f; // Value set in the fade animation clip
    public bool isAnimationRunning = false; // Value set in the fade animation clip

    private Material blurMat; // Cached blur material

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Initializes the blur effect
        blurMat = blurEffect.material;
        blurMat.SetFloat("_Radius", 0);
    }
    
    void Update()
    {
        if (isAnimationRunning)
        {
            blurMat.SetFloat("_Radius", currentStrength);
        }
    }

    public void OnTransitionEnded()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);      
    }

    public void FadeIn()
    {
        transitionAnimator.gameObject.SetActive(true);
        transitionAnimator.SetTrigger("FadeIn");
    }

    public void FadeOut()
    {
        transitionAnimator.SetTrigger("FadeOut");
    }
}
