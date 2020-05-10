using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestingScript : MonoBehaviour
{


    [Header("Utility")]
    public float blurDuration;
    [Range(0f, 5f)] public float blurMaxStrengh;
    public Image blurEffect;

    public Animator animator;

    // Value set in the fade animation clip
    public float currentStrength = 0f;

    // Value set in the fade animation clip
    public bool isAnimationRunning = false;

    private Material blurMat;

    private void Start()
    {
        // Initializes the blur effect
        blurMat = blurEffect.material;

        blurMat.SetFloat("_Radius", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            //StartCoroutine(LoadGameScene());
            animator.SetTrigger("StartFade");
        }
        
        if(isAnimationRunning)
        {
            blurMat.SetFloat("_Radius", currentStrength);
        }
    }


    private System.Collections.IEnumerator LoadGameScene()
    {
        float ratio = blurMaxStrengh / blurDuration;
        float blurStrength = 0f;

        while(blurStrength < blurMaxStrengh)
        {
            blurStrength += Time.deltaTime * ratio;
            blurMat.SetFloat("_Radius", blurStrength);
            Debug.Log(Time.time);
            yield return null;
        }
    }
}
