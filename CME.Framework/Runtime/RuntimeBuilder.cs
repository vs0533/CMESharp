using CME.Framework.Model;
using CME.Framework.Utils;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace CME.Framework.Runtime
{
    public class RuntimeBuilder
    {
        private static ModuleBuilder moduleBuilder;
        private static AssemblyName assemName = new AssemblyName("WSRC.Runtime");
        static RuntimeBuilder()
        {
            moduleBuilder = AssemblyBuilder.DefineDynamicAssembly(assemName, AssemblyBuilderAccess.RunAndCollect).DefineDynamicModule("WSCME.Runtime");
        }

        public static Type Builder(Entity entity,bool redDefine = false)
        {
            var define = moduleBuilder.Assembly.DefinedTypes.FirstOrDefault(c => c.Name == entity.TypeName);
            if (define != null)
            {
                if (redDefine)
                {
                    moduleBuilder = AssemblyBuilder.DefineDynamicAssembly(assemName, AssemblyBuilderAccess.RunAndCollect).DefineDynamicModule("WSCME.Runtime");
                }
                else
                {
                    return define.UnderlyingSystemType;
                }
            }
            
            TypeBuilder builder = moduleBuilder.DefineType(entity.TypeName, TypeAttributes.Public);
            CustomAttributeBuilder tableAttributeBuilder = new CustomAttributeBuilder(
                typeof(TableAttribute).GetConstructor(new Type[1] { typeof(string) }),
                new object[] { entity.TypeName }
            );
            builder.SetParent(entity.BaseType);
            builder.SetCustomAttribute(tableAttributeBuilder);

            foreach (var item in entity.Propertys)
            {
                AddProperty(item, builder, entity.BaseType);
            }
            return builder.CreateTypeInfo().UnderlyingSystemType;
        }
        private static void AddProperty(EntityProperty property, TypeBuilder builder, Type baseType)
        {
            PropertyBuilder propertyBuilder = builder.DefineProperty(
                property.PropertyName,
                PropertyAttributes.None,
                property.PropertyType,
                null
            );
            #region SetAttributeforProperty
            foreach (var item in property.Attributes)
            {
                if (item.ConstructorArgTypes == null)
                {
                    item.ConstructorArgTypes = new Type[0];
                    item.ConstructorArgValues = new Type[0];
                }
                ConstructorInfo cInfo = item.AttributeType.GetConstructor(item.ConstructorArgTypes);
                PropertyInfo[] pInfos = item.Properties.Select(c => item.AttributeType.GetProperty(c)).ToArray();
                CustomAttributeBuilder cattrBuilder = new CustomAttributeBuilder(cInfo, item.ConstructorArgValues, pInfos, item.PropertyValues);
                propertyBuilder.SetCustomAttribute(cattrBuilder);
            }
            #endregion

            MethodAttributes attributes = MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Public;
            MethodBuilder getMethodBuilder = builder.DefineMethod(
                "get_" + property.PropertyName,
                attributes,
                property.PropertyType,
                Type.EmptyTypes
            );

            ILGenerator iLGenerator = getMethodBuilder.GetILGenerator();
            MethodInfo getMethod = baseType.GetMethod("GetValue").MakeGenericMethod(
                new Type[] { property.PropertyType }
            );
            iLGenerator.DeclareLocal(property.PropertyType);
            iLGenerator.Emit(OpCodes.Nop);
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldstr, property.PropertyName);
            iLGenerator.EmitCall(OpCodes.Call, getMethod, null);
            iLGenerator.Emit(OpCodes.Stloc_0);
            iLGenerator.Emit(OpCodes.Ldloc_0);
            iLGenerator.Emit(OpCodes.Ret);

            MethodInfo setMethod = baseType.GetMethod("SetValue").MakeGenericMethod(
                new Type[] { property.PropertyType }
            );
            MethodBuilder setMethodBuilder = builder.DefineMethod(
                "set_" + property.PropertyName,
                attributes,
                null,
                new Type[] { property.PropertyType }
            );
            ILGenerator generator2 = setMethodBuilder.GetILGenerator();
            generator2.Emit(OpCodes.Nop);
            generator2.Emit(OpCodes.Ldarg_0);
            generator2.Emit(OpCodes.Ldstr, property.PropertyName);
            generator2.Emit(OpCodes.Ldarg_1);
            generator2.EmitCall(OpCodes.Call, setMethod, null);
            generator2.Emit(OpCodes.Nop);
            generator2.Emit(OpCodes.Ret);
            propertyBuilder.SetGetMethod(getMethodBuilder);
            propertyBuilder.SetSetMethod(setMethodBuilder);
        }
    }
}
