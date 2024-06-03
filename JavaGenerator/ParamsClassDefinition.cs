﻿using rscconventer.JavaGenerator.Interfaces;

namespace rscconventer.JavaGenerator;

public class ParamsClassDefinition : IClassDefinition
{
    public IClassDefinition Class { get; set; }
    public string Name => Class.Name;

    public string Namespace => Class.Namespace;

    public string FullName => Class.FullName;

    public ParamsClassDefinition(IClassDefinition classDefinition)
    {
        Class = classDefinition;
    }

    public string OnImport(ClassDefinition classDefinition)
    {
        return $"{Class.OnImport(classDefinition)}...";
    }
}
