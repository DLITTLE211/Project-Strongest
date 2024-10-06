using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;

[Serializable]
public class Round_CharacterDialogueState : Round_BaseState
{
    public Round_CharacterDialogueState(MainGame_RoundSystemController rSystem) : base(rSystem){}
    
    public override void OnEnter()
    {
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
    public async Task SayCharacterDialogue(/*DialogueSet dialogueSet*/) 
    {
        await Task.Delay(1000);
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
