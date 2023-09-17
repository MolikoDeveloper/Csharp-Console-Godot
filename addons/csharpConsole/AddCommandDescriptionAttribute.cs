using System;
using Godot;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AddCommandDescriptionAttribute : Attribute
{
    public string Description { get; private set; }

    public AddCommandDescriptionAttribute(string description)
    {
        this.Description = description;
    }
}
