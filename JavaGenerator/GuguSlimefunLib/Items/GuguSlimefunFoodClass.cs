﻿namespace rscconventer.JavaGenerator.GuguSlimefunLib.Items;

public static class GuguSlimefunFoodClass
{
    public static ClassDefinition Class { get; }
    static GuguSlimefunFoodClass()
    {
        Class = new("me.ddggdd135.guguslimefunlib.items", "GuguSlimefunFood")
        {
            NeedGenerate = false,
            Super = GuguSlimefunItemClass.Class
        };
    }
}