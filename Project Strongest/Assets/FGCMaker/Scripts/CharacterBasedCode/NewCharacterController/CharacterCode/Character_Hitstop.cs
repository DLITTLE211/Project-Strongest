using System;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

public class Character_Hitstop : MonoBehaviour
{
    [SerializeField] private Character_Animator p1, p2;
    [SerializeField,Range(0,1f)] private float TimeScale;
    public static Character_Hitstop Instance;
    public bool canHS;
    private void Start()
    {
        Instance = this; 
        SetStartTimeScale();
    }
    public void SetStartTimeScale() 
    {
        TimeScale = 1;
    }
    private void Update()
    {
        Time.timeScale = TimeScale;
    }
    public void SetCharacterAnimator(int id, Character_Animator anim) 
    {
        if (id == 0) 
        {
            p1 = anim;
        }
        if(id == 1) 
        {
            p2 = anim;
        }
        if (id == -1)
        {
            if (p1 != null)
            {
                p2 = anim;
            }
            else 
            {
                p1 = anim;
            }
        }
    }
    public async Task CallHitStop(Attack_BaseProperties lastAttack, float rateOfIncrease = 0, Character_Base targetBase = null)
    {
        //Sets Total HitstopTime
        float actualWaitTime = rateOfIncrease * (1 / 60f);
        int waitTime_milli = (int)(actualWaitTime * 1000f);
        if(waitTime_milli <= 0) 
        {
            return;
        }

        p1.SetSelfFreeze();
        p2.SetSelfFreeze();
        targetBase._cAnimator.SetShake(true);
        await Task.Delay(waitTime_milli);
        p1.SetSelfUnfreeze();
        p2.SetSelfUnfreeze();
        targetBase._cForce.SendKnockBackOnHit(lastAttack);
    }
    
}
