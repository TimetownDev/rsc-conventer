﻿using rsc_converter.Classes.Interfaces;
using rsc_converter.Classes.Utils;
using rsc_converter.Classes.Yaml;
using rsc_converter.JavaGenerator;
using rsc_converter.JavaGenerator.Actions;
using rsc_converter.JavaGenerator.Bukkit;
using rsc_converter.JavaGenerator.GuguSlimefunLib.Items;
using rsc_converter.JavaGenerator.GuguSlimefunLib.Script;
using rsc_converter.JavaGenerator.Interfaces;
using rsc_converter.JavaGenerator.Slimefun;
using rsc_converter.JavaGenerator.Values;
using YamlDotNet.RepresentationModel;

namespace rsc_converter.Classes.Generators;

public class GeneratorsGenerator : IClassGenerator
{
    public IList<ClassDefinition>? OnGenerate(BuildSession session)
    {
        ClassDefinition generated = new($"me.ddggdd135.{session.Name}.generators", $"{char.ToUpper(session.Name[0])}{session.Name[1..]}Generators");

        YamlStream stream = [];
        stream.Load(new StringReader(File.ReadAllText(Path.Combine(session.Directory.FullName, "generators.yml"))));
        YamlMappingNode generators = (YamlMappingNode)stream.Documents[0].RootNode;

        IList<ClassDefinition> itemClasses = [];
        ClassDefinition itemGroupClass = session.GetClassDefinition($"{char.ToUpper(session.Name[0])}{session.Name[1..]}ItemGroups")!;
        ClassDefinition recipeTypeClass = session.GetClassDefinition($"{char.ToUpper(session.Name[0])}{session.Name[1..]}RecipeTypes")!;
        ClassDefinition itemsClass = session.GetClassDefinition($"{char.ToUpper(session.Name[0])}{session.Name[1..]}Items")!;
        ClassDefinition menusClass = session.GetClassDefinition($"{char.ToUpper(session.Name[0])}{session.Name[1..]}Menus")!;

        MethodDefinition onSetup = new("onSetup")
        {
            ParameterTypes = [SlimefunAddonClass.Class],
            IsStatic = true
        };
        generated.Methods.Add(onSetup);

        if (generators is not YamlMappingNode mappingNode) return null;
        foreach (KeyValuePair<YamlNode, YamlNode> pair in mappingNode)
        {
            YamlNode key = pair.Key;
            if (key is not YamlScalarNode scalarNode) continue;
            string? stringKey = scalarNode.Value;
            if (stringKey == null) continue;

            YamlNode value = pair.Value;

            ClassDefinition itemClass = new($"me.ddggdd135.{session.Name}.generators.implementations", $"{stringKey.ToUpper()}GeneratorImplementation")
            {
                Super = GuguGeneratorClass.Class,
            };
            CtorMethodDefinition ctor = new(ItemGroupClass.Class, SlimefunItemStackClass.Class, RecipeTypeClass.Class, new ArrayClassDefinition(ItemStackClass.Class), MachineMenuClass.Class, ScriptEvalClass.Class);
            ctor.Block.Actions.Add(new SuperInvokeAction(new ParameterValue(0), new ParameterValue(1), new ParameterValue(2), new ParameterValue(3), new ParameterValue(4), new ParameterValue(5)));
            itemClass.Ctors.Add(ctor);

            MethodDefinition preRegister = new("preRegister");

            RawValue slimefunItemStackValue = new($"{itemsClass.Name}.{stringKey.ToUpper()}");
            slimefunItemStackValue.ImportList.Import(itemsClass);

            IValue itemGroup = value.ReadItemGroup(itemGroupClass);
            IValue recipeType = value.ReadRecipeType(recipeTypeClass);
            IValue[] recipe = value.ReadRecipe(session.Directory, itemsClass);

            bool hasScript = value.Contains("script");
            IValue? scriptEval = null;
            if (hasScript)
            {
                string scriptFileName = value.GetString("script")!;
                string script = File.ReadAllText(Path.Combine(session.Directory.FullName, "scripts", scriptFileName + ".js"));
                scriptEval = new NewInstanceAction(JavaScriptEvalClass.Class, new StringValue(script));
            }
            scriptEval ??= new NullValue();

            IList<int>? inputSlot = value.GetIntList("input");
            IList<int>? outputSlot = value.GetIntList("output");

            if (inputSlot != null)
            {
                MethodDefinition getInputSlots = new("getInputSlots")
                {
                    ReturnType = new ArrayClassDefinition(new RawClassDefinition("int"))
                };
                getInputSlots.Block.Actions.Add(new ReturnAction(new ArrayValue(new RawClassDefinition("int"), inputSlot.Select<int, IValue>(x => new NumberValue<int>(x)).ToList())));
                itemClass.Methods.Add(getInputSlots);
            }
            if (outputSlot != null)
            {
                MethodDefinition getOutputSlots = new("getOutputSlots")
                {
                    ReturnType = new ArrayClassDefinition(new RawClassDefinition("int"))
                };
                getOutputSlots.Block.Actions.Add(new ReturnAction(new ArrayValue(new RawClassDefinition("int"), outputSlot.Select<int, IValue>(x => new NumberValue<int>(x)).ToList())));
                itemClass.Methods.Add(getOutputSlots);
            }

            IValue machineMenu = new NullValue();
            if (menusClass.FieldList.FindField(stringKey.ToUpper()) != null)
            {
                machineMenu = new RawValue($"{char.ToUpper(session.Name[0])}{session.Name[1..]}Menus.{stringKey.ToUpper()}");
                ((RawValue)machineMenu).ImportList.Import(menusClass);
            }

            int capacity = value.GetInt("capacity");
            int production = value.GetInt("production");

            preRegister.Block.Actions.Add(new ThisValue().Invoke(GuguGeneratorClass.SetCapacity, new NumberValue<int>(capacity)));
            preRegister.Block.Actions.Add(new ThisValue().Invoke(GuguGeneratorClass.SetEnergyProduction, new NumberValue<int>(production)));
            foreach(IValue fuel in value.ReadFuels(session.Directory, itemsClass))
            {
                preRegister.Block.Actions.Add(new ThisValue().Invoke(GuguGeneratorClass.RegisterFuel, fuel));
            }

            itemClass.Methods.Add(preRegister);

            itemClasses.Add(itemClass);
            onSetup.Block.Actions.Add(new NewInstanceAction(itemClass, itemGroup, slimefunItemStackValue, recipeType, new ArrayValue(ItemStackClass.Class, recipe), machineMenu, scriptEval).Invoke(GuguSlimefunItemClass.Register, new ParameterValue(0)));
        }

        itemClasses.Add(generated);

        return itemClasses;
    }
}
