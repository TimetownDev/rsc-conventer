﻿using rscconventer.JavaGenerator.Exceptions;
using rscconventer.JavaGenerator.Interfaces;
using rscconventer.JavaGenerator.Utils;
using System.Text;

namespace rscconventer.JavaGenerator;

public class StaticInvokeAction : IAction, IValue
{
    public ClassDefinition ClassDefinition { get; set; }
    public MethodDefinition MethodDefinition { get; set; }
    public IList<IValue> Parameters { get; set; } = [];

    public string BuildContent(ClassDefinition classDefinition)
    {
        StringBuilder sb = new();
        classDefinition.ImportList.Import(ClassDefinition);
        sb.Append(classDefinition.ImportList.GetUsing(ClassDefinition));
        sb.Append('.');
        sb.Append(MethodDefinition.Name);
        sb.Append('(');

        int x = 0;
        foreach (IValue parameter in Parameters)
        {
            sb.Append(parameter.BuildContent(classDefinition));
            if (x + 1 !=  Parameters.Count)
                sb.Append(", ");
            x++;
        }
        sb.Append(')');

        return sb.ToString();
    }
    public StaticInvokeAction(ClassDefinition classDefinition, MethodDefinition methodDefinition, params IValue[] parameters)
    {
        if (!classDefinition.Methods.Contains(methodDefinition))
            throw new NoSuchMethodException(methodDefinition);
        if (!methodDefinition.IsStatic)
            throw new InvalidOperationException("方法不是静态");
        ClassDefinition = classDefinition;
        MethodDefinition = methodDefinition;
        Parameters = parameters;
    }
}
