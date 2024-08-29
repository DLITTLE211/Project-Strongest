using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public class Character_HitStun : MonoBehaviour
{
    [SerializeField] private Character_Animator _cAnimator;
    public float animSpeed;
    [SerializeField] bool isFrozen;
    void Start()
    {
        isFrozen = false;
        SetStartCharacterAnimSpeed();
    }
    public void SetAnimator(Character_Animator myAnim)
    {
        _cAnimator = myAnim;
    }
    public float GetCurrentAnimSpeed() { return animSpeed; }
    public void HandleAnimatorFreeze(bool state, float speed = 0.25f)
    {
        if (state)
        {
            if (!isFrozen)
            {
                isFrozen = true;
                _cAnimator.myAnim.speed = speed;
                _cAnimator.shadowAnim.speed = speed;
                animSpeed = speed;
            }
        }
        else
        {
            if (isFrozen)
            {
                isFrozen = false;
                _cAnimator.myAnim.speed = 1;
                _cAnimator.shadowAnim.speed = 1;
                animSpeed = 1;
            }
        }
    }
    public bool IsFrozen() 
    {
        return isFrozen;
    }
    void SetStartCharacterAnimSpeed() 
    {
        animSpeed = 1;
    }
    public async void CallHitStun(float hitstunValue) 
    {
        await ApplyHitStun(hitstunValue);
    }
    public async Task ApplyHitStun(float hitstunValue)
    {
        animSpeed = 0.05f;
        float waitTime = hitstunValue;
        int delayTimeIn_MS = (int)(waitTime * 1000f);
        _cAnimator.SetCanRecover(true);
        await Task.Delay(delayTimeIn_MS);
        animSpeed = 1f;
        Messenger.Broadcast<int, string>(Events.SendReturnTime, 0, _cAnimator._base.gameObject.name);

    }

    private void Update()
    {
        if (!isFrozen)
        {
            if (_cAnimator != null)
            {
                _cAnimator.myAnim.speed = animSpeed;
                _cAnimator.shadowAnim.speed = animSpeed;
            }
        }
    }
}
