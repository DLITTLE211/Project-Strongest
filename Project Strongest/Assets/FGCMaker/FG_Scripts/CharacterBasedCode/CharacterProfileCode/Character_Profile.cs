using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using System;

[CreateAssetMenu(menuName = "Character/Character Profile")]
public class Character_Profile : ScriptableObject
{
    #region Character Identification Info
    [Header("Character Indentity Information")]
    public string CharacterName;
    public int CharacterID;
    public Sprite CharacterProfileImage;
    #endregion

    #region Character Sizing Info
    [Header("Character Sizing Information")]
    public int Mass;
    public float Height;
    public float Weight;
    public Character_HurtBoxSizing hurtboxSizing;
    public Character_CollisionSizing collisionSizing;
    #endregion

    #region Character Movement Info
    [Header("Character Movement Information")]
    public float MoveVelocity;
    public float JumpForce;
    public float InAirMoveForce;
    public float dashSpeed;
    #endregion

    #region Character Health Info
    [Header("Character Health Information")]
    //Minimum 100f
    [SerializeField, Range(0f, 250f)] public float MaxHealth;
    //Minimum 30f
    [SerializeField, Range(0f, 80f)] public float MaxStunValue;
    //Minimum 25f
    [SerializeField, Range(0f, 150f)] public float DefenseValue;
    //Minimum 3f
    [SerializeField,Range(0f,8f)] public float HealthRegenRate;
    #endregion

    #region Character Animator Info
    [Header("Character Animator Information")]
    public GameObject characterModel;
    public Animator characterAnimator;
    public Character_Animator cAnimatorScript;
    public List<AnimationClip> AllCharacterAnimations = new List<AnimationClip>();
    public GameObject CharacterModel;
    public List<HitAnimationField> properHitResponseAnimations;
    #endregion

    #region Character MoveList Info
    [Header("Character MoveList Information")]
    public Character_MoveList _characterMoveList;
    public int moveListCount;
    public List<GameObject> CharacterMoveListPrefab;
    public Character_MobilityAsset _CharacterMobility;
    #endregion

    #region Character Interaction Info
    [Header("Character Interaction Information")]
    public List<CharacterIntro> BasicCharacterInteractions = new List<CharacterIntro>();
    public List<CharacterIntro> SpecialCharacterInteractions = new List<CharacterIntro>();
    #endregion

    
    public void SetCharacterAnimator(Character_Animator animator)
    {
        characterAnimator = characterModel.GetComponent<Animator>();
        cAnimatorScript = animator;
     
    }
}

[Serializable]
public class Character_HurtBoxSizing 
{
    public float radius;
    public Vector3 hurtboxPosition;
    public Vector3 hurtboxSizing;
}

[Serializable]
public class Character_CollisionSizing
{
    public float headRadius;
    public float headHeight;
    public Vector3 headHurtboxPositioning;

    public float bodyRadius;
    public float bodyHeight;
    public Vector3 bodyHurtboxPositioning;
}
[Serializable]
public class CharacterIntro 
{
    public string targetCharacter;
    public AnimationClip introAnim;
    public DialogueOption dialogueOption;
}
[Serializable]
public class DialogueOption
{
    public string statementSaid;
    public List<DialgueAudio> statementAudio = new List<DialgueAudio>();
}
[Serializable]
public class DialgueAudio
{
    public enum languageType {English, Japanese};
    public languageType Type;
    public AudioClip statementAudio;
    public float statementAudioLength;
}
