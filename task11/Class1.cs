namespace task11;

using System;
using System.Reflection;
using System.Reflection.Emit;

public static class DynamicClassGenerator
{
    public static Type GenerateCalculatorType()
    {
        
        var assemblyName = new AssemblyName("DynamicCalculatorAssembly");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicCalculatorModule");

        
        var typeBuilder = moduleBuilder.DefineType(
            "Calculator",
            TypeAttributes.Public | TypeAttributes.Class);

        
        AddMethod(typeBuilder, "Add", typeof(int), typeof(int), typeof(int), (a, b) => a + b);
        AddMethod(typeBuilder, "Minus", typeof(int), typeof(int), typeof(int), (a, b) => a - b);
        AddMethod(typeBuilder, "Mul", typeof(int), typeof(int), typeof(int), (a, b) => a * b);
        AddMethod(typeBuilder, "Div", typeof(int), typeof(int), typeof(int), (a, b) => a / b);

        
        return typeBuilder.CreateType();
    }

    private static void AddMethod(
        TypeBuilder typeBuilder,
        string methodName,
        Type returnType,
        Type paramType1,
        Type paramType2,
        Func<int, int, int> operation)
    {
        
        var methodBuilder = typeBuilder.DefineMethod(
            methodName,
            MethodAttributes.Public,
            returnType,
            new[] { paramType1, paramType2 });

        
        var ilGenerator = methodBuilder.GetILGenerator();
        
        
        ilGenerator.Emit(OpCodes.Ldarg_1);
        
        ilGenerator.Emit(OpCodes.Ldarg_2);
        
        
        switch (methodName)
        {
            case "Add":
                ilGenerator.Emit(OpCodes.Add);
                break;
            case "Minus":
                ilGenerator.Emit(OpCodes.Sub);
                break;
            case "Mul":
                ilGenerator.Emit(OpCodes.Mul);
                break;
            case "Div":
                ilGenerator.Emit(OpCodes.Div);
                break;
        }
        
        
        ilGenerator.Emit(OpCodes.Ret);
    }
}