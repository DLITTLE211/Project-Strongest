using System;

public interface IPredicate 
{
    bool Evaluate();
}
public class Predicate: IPredicate 
{
    readonly Func<bool> function;

    public Predicate(Func<bool> _function) 
    {
        this.function = _function;
    }
    public bool Evaluate() => function.Invoke();
}
