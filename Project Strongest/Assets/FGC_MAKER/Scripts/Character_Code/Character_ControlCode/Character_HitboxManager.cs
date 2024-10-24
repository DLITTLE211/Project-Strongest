using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_HitboxManager : MonoBehaviour
{
    [SerializeField] private GameObject hitboxPrefab;
    public List<HitBox> totalHitBoxes;
    public Transform hitBoxHolder;
    public int hitBoxCount;
    [SerializeField] private int hitBoxIndex;
     public HitBox currentActiveHitbox;
    public void SetupHitboxes(int playerID) 
    {
        totalHitBoxes = new List<HitBox>();
        for (int i = 0; i < hitBoxCount; i++)
        {
            GameObject hitbox = (GameObject)Instantiate(hitboxPrefab,Vector3.zero, Quaternion.identity, hitBoxHolder);
            hitbox.name = $"P{playerID}_Hitbox_{i+1}";
            hitbox.SetActive(true);
            HitBox _hitBoxData = hitbox.GetComponent<HitBox>();
            _hitBoxData.HBType = HitBoxType.nullified;
            _hitBoxData.SetHitboxSize(null,0f, 0f);
            _hitBoxData.SetupHitboxDictionary();
            if (playerID == 1)
            {
                _hitBoxData.SetLayerCheck(2);
            }
            else 
            {
                _hitBoxData.SetLayerCheck(1);
            }
            totalHitBoxes.Add(_hitBoxData);
        }
        hitBoxIndex = 0;
    }
    public void IterateHitBox()
    {
        hitBoxIndex++;
        if (hitBoxIndex >= totalHitBoxes.Count)
        {
            hitBoxIndex = 0;
        }
    }
    public HitBox GetActiveHitBox() 
    {
        HitBox currentHitbox = totalHitBoxes[hitBoxIndex];
        currentActiveHitbox = currentHitbox;
        return currentActiveHitbox;
    }
    public void DisableCurrentHitbox()
    { 
        Attack_BaseProperties baseProperty = totalHitBoxes[hitBoxIndex].hitboxProperties;
        if (baseProperty != null) 
        {
            baseProperty.AttackAnims?.extendedHitBox?.SetHurtBoxSize(0f, 0f, true);
        }
        totalHitBoxes[hitBoxIndex].DestroyHitbox(totalHitBoxes[hitBoxIndex]);
    }
}
