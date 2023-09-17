using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class AddCommandAttribute: Attribute
{
    public string CommandName { get; private set; }

    public AddCommandAttribute(string commandName)
    {
        CommandName = commandName;
    }
}
