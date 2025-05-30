using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;

[Serializable]
public class Round_CharacterDialogueState : Round_BaseState
{
    public List<Character_Base> players;
    [SerializeField] private GameObject overlayCanvas;
    public Round_CharacterDialogueState(MainGame_RoundSystemController rSystem) : base(rSystem){}
    
    public override void OnEnter()
    {
        overlayCanvas.SetActive(false);
        GameManager.instance.winningCharacter = null;
        GameManager.instance.settingsController._pauseMenu.SetActive(false);
        _rSystem.p1_Signifiers.DisableRoundObjects();
        _rSystem.p2_Signifiers.DisableRoundObjects();
        _rSystem.p1_Signifiers.SetRoundSignifier(CharacterSelect_LoadArena._roundInfo.winningRoundCount);
        _rSystem.p2_Signifiers.SetRoundSignifier(CharacterSelect_LoadArena._roundInfo.winningRoundCount);
        TriggerDialogue();
    }
    public async void TriggerDialogue(/*DialogueSet dialogueSet*/)
    {
        await SayCharacterDialogue(/*dialogueSet*/);
    }
    async Task SayCharacterDialogue(/*DialogueSet dialogueSet*/) 
    {
        for (int i = 0; i < players.Count; i++) 
        {
            players[i]._cSubStateController.PlayIntroAnimation();
            while (!players[i]._cSubStateController.introAnimationComplete) 
            {
                await Task.Yield();
            }
        }
        //TODO Character Dialogue Function
        /*
         * for(int i = 0; i < dialogueSet.Count; i++)
         * {
         *      dialogueSet[i].Speak();
         *      await Task.Delay(dialogueSet[i].AudioLength);
         * }
         */
        _rSystem.StateMachine.CallInitialTimerState();
    }
    public override void OnExit()
    {
        
    }
}
