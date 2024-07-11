using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character_State 
{
    #region StateNode
    class StateNode 
    {
        public IState State { get; }
        public HashSet<ITransition> Transitions { get; }
        public StateNode(IState _state) 
        {
            State = _state;
            Transitions = new HashSet<ITransition>();
        }

        public void AddTransition(IState to, IPredicate condition) 
        {
            Transitions.Add(new Transition(to, condition));
        }
    }
    #endregion
    [SerializeField] private Character_Base _base;
    [SerializeField] Dictionary<Type, StateNode> nodes = new Dictionary<Type, StateNode>(); 
    HashSet<ITransition> anyTransition = new HashSet<ITransition>();
    StateNode current;
    public IState nextState;
    public string CurrentStateString;
    public Character_State(Character_Base newbase) 
    {
        _base = newbase;
    }
    public void Update()
    {
        var transition = GetTransition();
        if (transition != null) 
        {
            ChangeState(transition.To);
        }
        current.State?.OnUpdate();
        nextState = current.State;
    }

    public void SetState(IState state) 
    {
        current = nodes[state.GetType()];
        current.State?.OnEnter();
        CurrentStateString = current.State.ToString();
    }
    public void ChangeState(IState state) 
    {
        if (state == current.State) 
        {
            return;
        }
        else
        {
            var previousState = current.State;
            var newState = nodes[state.GetType()].State;
            previousState?.OnExit();
            newState?.OnEnter();
            current = nodes[state.GetType()];
            CurrentStateString = newState.ToString();
        }
    }
    public ITransition GetTransition() 
    {
        foreach (var transition in anyTransition) 
        {
            if (transition.Condition.Evaluate()) 
            { return transition; }
        }
        foreach (var transition in current.Transitions)
        {
            if (transition.Condition.Evaluate()) 
            { return transition; }
        }
        return null;
    }

    public void AddTransition(IState from, IState to, IPredicate condition) 
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
    }
    public void AddAnyTransition(IState to, IPredicate condition)
    {
        anyTransition.Add(new Transition(GetOrAddNode(to).State,condition));
    }
    StateNode GetOrAddNode(IState state) 
    {
        var node = nodes.GetValueOrDefault(state.GetType());
        if (node == null) 
        {
            node = new StateNode(state);
            nodes.Add(state.GetType(), node);
        }
        return node;
    }
    public void FixedUpdate()
    {
        current.State?.OnFixedUpdate();
    }
    private void Start()
    {
       
    }
}
