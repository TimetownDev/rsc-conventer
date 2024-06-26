﻿using rsc_converter.JavaGenerator.System;

namespace rsc_converter.JavaGenerator.Bukkit;

public static class ConsoleCommandSenderClass
{
    public static ClassDefinition Class { get; }
    public static MethodDefinition SendMessage { get; }
    static ConsoleCommandSenderClass()
    {
        Class = new ClassDefinition("org.bukkit.command", "ConsoleCommandSender")
        {
            NeedGenerate = false
        };
        SendMessage = new MethodDefinition("sendMessage");
        SendMessage.ParameterTypes.Add(SystemClass.StringClass);
        Class.Methods.Add(SendMessage);
    }
}
