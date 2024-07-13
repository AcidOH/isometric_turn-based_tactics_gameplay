public enum StatModType //could technically declare in its own script
{ 
    Flat = 100, //makes adding a custom order more easy
    PercentAdd = 200, 
    PercentMult = 300, 
} 
public class StatModifier 
{ 
    public readonly float Value; 
    public readonly StatModType Type; 
    public readonly int Order; 
    public readonly object Source; //object can be any type (int, float, gameObject, class, etc.)
    public StatModifier(float value, StatModType type, int order, object source) 
    { 
        Value = value; 
        Type = type; 
        Order = order; 
        Source = source; 
    } 
    public StatModifier(float value, StatModType type) : this (value, type, (int)type, null) {} //contructor requires only the value and type input and not the order, it automatically calls the other constructor passing in the supplied value and type, as well as the (int)type (enum value) which acts as a default order
    public StatModifier(float value, StatModType type, int order) : this (value, type, order, null) {} 
    public StatModifier(float value, StatModType type, object source) : this (value, type, (int)type, source) {} 
}