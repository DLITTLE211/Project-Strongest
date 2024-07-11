using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITransition 
{
    IState To { get; }
    IPredicate Condition { get; }
}

public class Transition : ITransition 
{
    public IState To { get; }
    public IPredicate Condition { get; }
    public Transition(IState _to, IPredicate _condition) 
    {
        To = _to;
        Condition = _condition;
    }
}
