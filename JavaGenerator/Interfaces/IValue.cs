﻿using rsc_converter.JavaGenerator.Actions;

namespace rsc_converter.JavaGenerator.Interfaces;

public interface IValue
{
    string BuildContent(ClassDefinition classDefinition);
}

public static class IValueExtension
{
    public static ObjectInvokeAction Invoke(this IValue value, MethodDefinition method, params IValue[] parameters)
    {
        return new ObjectInvokeAction(value, method, parameters);
    }
}
