using System.Management.Automation;
using System.Reflection;
using System.Reflection.Emit;

namespace PowershellJSONTest;

public static class Extensions
{
    static Extensions()
    {
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("ModuleBuilder"),
            AssemblyBuilderAccess.RunAndCollect);
        ModuleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
    }

    public static object CreateObject(this PSObject obj)
    {
        var properties = obj.Properties
            .Where(x => Type.GetType(x.TypeNameOfValue) != null)
            .ToDictionary(
                o => o.Name,
                o => (propName: o.Name, propType: Type.GetType(o.TypeNameOfValue), propValue: GetPropValue(o)));

        var objType = CreateClass(properties);

        var finalObj = Activator.CreateInstance(objType);
        
        foreach (var prop in objType.GetProperties())
            prop.SetValue(finalObj, properties[prop.Name].propValue);

        return finalObj;
    }

    private static object GetPropValue(PSMemberInfo prop)
    {
        var type = Type.GetType(prop.TypeNameOfValue);

        if (type == typeof(string)) return prop.Value;
        if (type == typeof(int)) return prop.Value;
        if (type == typeof(bool)) return prop.Value;
        
        return Convert.ChangeType(prop.Value.ToString(), type);
    }

    private const MethodAttributes MethodAttributes = System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.SpecialName | System.Reflection.MethodAttributes.HideBySig;

    private static readonly ModuleBuilder ModuleBuilder;

    private static Type CreateClass(IDictionary<string, (string propName, Type type, object)> parameters)
    {
        var typeBuilder = ModuleBuilder.DefineType(Guid.NewGuid().ToString(),
            TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass |
            TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout, null);
        typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName |
                                             MethodAttributes.RTSpecialName);
        foreach (var parameter in parameters)
            CreateProperty(typeBuilder, parameter.Key, parameter.Value.type);
        var type = typeBuilder.CreateTypeInfo().AsType();
        return type;
    }

    private static PropertyBuilder CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
    {
        var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

        var propBuilder =
            typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
        propBuilder.SetGetMethod(DefineGet(typeBuilder, fieldBuilder, propBuilder));
        propBuilder.SetSetMethod(DefineSet(typeBuilder, fieldBuilder, propBuilder));

        return propBuilder;
    }

    private static MethodBuilder DefineSet(TypeBuilder typeBuilder, FieldBuilder fieldBuilder,
        PropertyBuilder propBuilder)
        => DefineMethod(typeBuilder, $"set_{propBuilder.Name}", null, new[] {propBuilder.PropertyType}, il =>
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fieldBuilder);
            il.Emit(OpCodes.Ret);
        });

    private static MethodBuilder DefineGet(TypeBuilder typeBuilder, FieldBuilder fieldBuilder,
        PropertyBuilder propBuilder)
        => DefineMethod(typeBuilder, $"get_{propBuilder.Name}", propBuilder.PropertyType, Type.EmptyTypes, il =>
        {
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fieldBuilder);
            il.Emit(OpCodes.Ret);
        });

    private static MethodBuilder DefineMethod(TypeBuilder typeBuilder, string methodName, Type propertyType,
        Type[] parameterTypes, Action<ILGenerator> bodyWriter)
    {
        var methodBuilder = typeBuilder.DefineMethod(methodName, MethodAttributes, propertyType, parameterTypes);
        bodyWriter(methodBuilder.GetILGenerator());
        return methodBuilder;
    }
}