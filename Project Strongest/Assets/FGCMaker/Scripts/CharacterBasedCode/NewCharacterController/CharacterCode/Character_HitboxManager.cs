using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_HitboxManager : MonoBehaviour
{
    [SerializeField] private GameObject hitboxPrefab;
    public List<HitBox> totalHitBoxes;
    public Transform hitBoxHolder;
    public int hitBoxCount;
    private int hitBoxIndex;
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

    public HitBox GetCurrentHitbox() 
    {
        if(hitBoxIndex >= totalHitBoxes.Count) 
        {
            hitBoxIndex = 0;
        }
        HitBox currentHitbox = totalHitBoxes[hitBoxIndex];
        hitBoxIndex++;
        return currentHitbox;
    }
    public void DisableCurrentHitbox() 
    {
        totalHitBoxes[hitBoxIndex].DestroyHitbox(totalHitBoxes[hitBoxIndex]);
        totalHitBoxes[hitBoxIndex].hitboxProperties = null;
    }
}
