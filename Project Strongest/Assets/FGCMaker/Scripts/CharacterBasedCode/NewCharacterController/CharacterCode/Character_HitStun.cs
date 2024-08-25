using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public class Character_HitStun : MonoBehaviour
{
    [SerializeField] private Character_Animator _cAnimator;
    [Range(0f,1f)]public float animSpeed;
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
    public void HandleAnimatorFreeze(bool state)
    {
        if (state)
        {
            if (!isFrozen)
            {
                isFrozen = true;
                _cAnimator.myAnim.speed = 0.25f;
            }
        }
        else
        {
            if (isFrozen)
            {
                isFrozen = false;
                _cAnimator.myAnim.speed = animSpeed;
            }
        }
    }
    void SetStartCharacterAnimSpeed() 
    {
        animSpeed = 1;
    }
    public async Task ApplyHitStun(float hitstunValue)
    {
        animSpeed = 0.05f;
        float waitTime = hitstunValue * (1 / 60f);
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
            }
        }
    }
}
