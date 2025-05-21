using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_TestReplayAnimation : MonoBehaviour
{
    public Animator _myanim;
    public string currentanimName;
    // Update is called once per frame
    private void Start()
    {
        currentanimName = $"{_myanim.GetCurrentAnimatorClipInfo(0)[0].clip.name}";

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) 
        {
            ReplayAnimation();
        }
    }
    void ReplayAnimation() 
    {
        _myanim.Play(Animator.StringToHash(currentanimName),0,0);
    }
}
