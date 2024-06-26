﻿namespace rsc_converter.JavaGenerator.Bukkit;

public static class BukkitClass
{
    public static ClassDefinition Class { get; }
    public static MethodDefinition GetConsoleSender { get; }
    static BukkitClass()
    {
        Class = new ClassDefinition("org.bukkit", "Bukkit")
        {
            NeedGenerate = false
        };
        GetConsoleSender = new("getConsoleSender")
        {
            ReturnType = ConsoleCommandSenderClass.Class,
            IsStatic = true
        };
        Class.Methods.Add(GetConsoleSender);
    }
}
