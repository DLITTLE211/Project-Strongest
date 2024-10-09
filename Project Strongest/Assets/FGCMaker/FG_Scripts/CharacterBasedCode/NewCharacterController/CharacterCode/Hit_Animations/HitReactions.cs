using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Hit Reactions", menuName = "Hit Response Animations")]
public class HitReactions : ScriptableObject
{
    public List<ResponseAnim_Base> ground_hitAnims;
    public List<ResponseAnim_Base> getUp_Anims;
    public List<ResponseAnim_Base> air_HitAnims;
    public List<ResponseAnim_Base> air_RecoverAnims;
    public List<ResponseAnim_Base> standingblock_Anims;
    public List<ResponseAnim_Base> crouchingblock_Anims;
}
