#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：TestLib
*项目描述 ：
*命名空间 ：TestLib
*文件名称 ：Class1.cs
* 功能描述 ：Class1
* 创建时间 ：2020
*版本号   :   2020|V1.0.0.0 
---------------------------------------------------------------------
* Copyright @ jinyu 2020. All rights reserved.
---------------------------------------------------------------------

***********************************************************************/
#endregion



using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace TestLib
{
    public delegate object FastInvoke(object obj);
    public class FastMap
    {
      
        private static readonly ConcurrentDictionary<string, object> dicActions = new ConcurrentDictionary<string,object>();

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="TSource">源</typeparam>
        /// <typeparam name="TDest">目标</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="mapType">转换方式</param>
        /// <param name="func">执行委托</param>
        /// <param name="key">定义缓存名称</param>
        /// <returns></returns>
        public static TDest Mapper<TSource, TDest>(TSource obj, MapType mapType = MapType.PTP, Action<TSource, TDest> func = null, string key = null)
          where TSource : class  
          where TDest : class, new()
        {
            
            string name = typeof(TSource).FullName + "To" + typeof(TDest).FullName + "_" +mapType;
            if(string.IsNullOrEmpty(key))
            {
                key = name;
            }
            FastInvoke fastInvok = FastInvokeCache.Instance.GetFastInvok(name, mapType);
            if (fastInvok!=null)
            {
                var r= (TDest)fastInvok.Invoke(obj);
                object action = null;
                if (dicActions.TryGetValue(key,out action))
                {
                   
                    ActionInvoke<TSource, TDest> actionInvoke = (ActionInvoke<TSource, TDest>) action;
                    if(actionInvoke!=null)
                    {
                        actionInvoke.Action.Invoke(obj, r);
                    }
                }
                return r;
            }
            else
            {
                switch(mapType)
                {
                    case MapType.PTP:
                        fastInvok=GetFastInvokPTP(typeof(TDest),obj);
                        break;
                    case MapType.PTPI:
                        fastInvok = GetFastInvokPTPI(typeof(TDest), obj);
                        break;
                    case MapType.FTF:
                        fastInvok = GetFastInvokFTF(typeof(TDest), obj);
                        break;
                    case MapType.FTFI:
                        fastInvok = GetFastInvokFTFI(typeof(TDest), obj);
                        break;
                    case MapType.PTF_TP:
                        fastInvok = GetFastInvokPTF_TP(typeof(TDest), obj);
                        break;
                    case MapType.FPTFP:
                        fastInvok = GetFastInvokFPTFP(typeof(TDest), obj);
                        break;
                    case MapType.FPTFPI:
                        fastInvok = GetFastInvokFPTFP(typeof(TDest), obj);
                        break;
                    default:
                        fastInvok = GetFastInvokPTP(typeof(TDest), obj);
                        break;

                }
            }
          
            TDest result=(TDest)fastInvok.Invoke(obj);
            //执行成功的缓存
            FastInvokeCache.Instance.Add(name, mapType, fastInvok);
            if (func!=null)
            {
                 func(obj, result);
                 ActionInvoke<TSource, TDest> action = new ActionInvoke<TSource, TDest>
                {
                    Action = func
                };
               
                dicActions[key] = action;
            }
            return result;
        }


        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="TDest">目标</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="mapType">转换方式</param>
        /// <param name="func">执行委托</param>
        /// <param name="key">定义缓存名称</param>
        /// <returns></returns>
        public static TDest Mapper<TDest>(object obj, MapType mapType = MapType.PTP, Action<object,TDest> func = null, string key = null)
        
         where TDest : class, new()
        {
          
            string name = obj.GetType().FullName + "To" + typeof(TDest).FullName + "_" + mapType;
            if (string.IsNullOrEmpty(key))
            {
                key = name;
            }
            FastInvoke fastInvok = FastInvokeCache.Instance.GetFastInvok(name, mapType);
            if(fastInvok!=null)
            {
                var r = (TDest)fastInvok.Invoke(obj);
                object action = null;
                if (dicActions.TryGetValue(key, out action))
                {
                   
                    ActionInvoke<object, TDest> actionInvoke = (ActionInvoke<object, TDest>)action;
                    if (actionInvoke != null)
                    {
                        actionInvoke.Action.Invoke(obj, r);
                    }
                }
                return r;
            }
            else
            {
                switch (mapType)
                {
                    case MapType.PTP:
                        fastInvok = GetFastInvokPTP(typeof(TDest), obj);
                        break;
                    case MapType.PTPI:
                        fastInvok = GetFastInvokPTPI(typeof(TDest), obj);
                        break;
                    case MapType.FTF:
                        fastInvok = GetFastInvokFTF(typeof(TDest), obj);
                        break;
                    case MapType.FTFI:
                        fastInvok = GetFastInvokFTFI(typeof(TDest), obj);
                        break;
                    case MapType.PTF_TP:
                        fastInvok = GetFastInvokPTF_TP(typeof(TDest), obj);
                        break;
                    case MapType.FPTFP:
                        fastInvok = GetFastInvokFPTFP(typeof(TDest), obj);
                        break;
                    case MapType.FPTFPI:
                        fastInvok = GetFastInvokFPTFP(typeof(TDest), obj);
                        break;
                    default:
                        fastInvok = GetFastInvokPTP(typeof(TDest), obj);
                        break;

                }
            }

            TDest result = (TDest)fastInvok.Invoke(obj);
            //执行成功的缓存
            FastInvokeCache.Instance.Add(name, mapType,fastInvok);
            if (func != null)
            {
                func(obj, result);
                ActionInvoke<object, TDest> action = new ActionInvoke<object, TDest>
                {
                    Action = func
                };
                
                dicActions[key] = action;
            }
            return result;
        }

        private static string  GetMethod(Type type)
        {
            switch(type.Name)
            {
                case "Int32":
                    return "ToInt32";
                  
                default:
                  
                    return "ToInt32";
                   
            }
        }
        private static FastInvoke GetFastInvokPTP<T>(Type type, T obj)
        {
            string name = typeof(T).FullName + "To" + type.FullName + "_" + MapType.PTP;
            DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(object), new Type[] {typeof(object) }, type.Module);
            var il = dynamicMethod.GetILGenerator();
            var src = il.DeclareLocal(obj.GetType());
            var tar = il.DeclareLocal(type);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Isinst, obj.GetType());
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Newobj, type.GetConstructor(new Type[0]));
            il.Emit(OpCodes.Stloc_1);
            var source = obj.GetType().GetProperties();
            foreach (var pty in source)
            {
                var p = type.GetProperty(pty.Name);
                if (p != null)
                {
                    if (p.PropertyType.Equals(pty.PropertyType) || !p.PropertyType.IsValueType)
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Call, pty.GetMethod);
                        il.Emit(OpCodes.Call, p.SetMethod);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Call, pty.GetMethod);

                        if (p.PropertyType.IsValueType)
                        {
                            var mth = GetMethod(p.PropertyType);
                            // Convert.ToInt32();
                            var m = typeof(Convert).GetMethod(mth, new Type[] { pty.PropertyType });
                            if (m == null && pty.PropertyType.IsClass)
                            {
                                m = typeof(Convert).GetMethod(mth, new Type[] { typeof(object) });
                            }
                            il.Emit(OpCodes.Call, m);
                        }
                        il.Emit(OpCodes.Call, p.SetMethod);

                    }
                }
            }
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);
            FastInvoke fastInvok = (FastInvoke)dynamicMethod.CreateDelegate(typeof(FastInvoke));
            return fastInvok;

        }

        private static FastInvoke GetFastInvokPTPI<T>(Type type, T obj)
        {
            string name = typeof(T).FullName + "To" + type.FullName + "_" + MapType.PTPI;
            DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(object), new Type[] { typeof(object) }, type.Module);
            var il = dynamicMethod.GetILGenerator();
            var src = il.DeclareLocal(obj.GetType());
            var tar = il.DeclareLocal(type);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Isinst, obj.GetType());
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Newobj, type.GetConstructor(new Type[0]));
            il.Emit(OpCodes.Stloc_1);
            var source = obj.GetType().GetProperties();
            foreach (var pty in source)
            {
                var p = type.GetProperty(pty.Name, BindingFlags.IgnoreCase | BindingFlags.Public);
                if (p != null)
                {
                    if (p.PropertyType.Equals(pty.PropertyType) || !p.PropertyType.IsValueType)
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Call, pty.GetMethod);
                        il.Emit(OpCodes.Call, p.SetMethod);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Call, pty.GetMethod);

                        if (p.PropertyType.IsValueType)
                        {
                            var mth = GetMethod(p.PropertyType);
                          
                            var m = typeof(Convert).GetMethod(mth, new Type[] { pty.PropertyType });
                            if (m == null && pty.PropertyType.IsClass)
                            {
                                m = typeof(Convert).GetMethod(mth, new Type[] { typeof(object) });
                            }
                            il.Emit(OpCodes.Call, m);
                        }
                        il.Emit(OpCodes.Call, p.SetMethod);

                    }
                }
            }
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
            FastInvoke fastInvok = (FastInvoke)dynamicMethod.CreateDelegate(typeof(FastInvoke));
            return fastInvok;

        }

        private static FastInvoke GetFastInvokFTF<T>(Type type, T obj)
        {
            string name = typeof(T).FullName + "To" + type.FullName + "_" + MapType.FTF;
            DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(object), new Type[] { typeof(object) }, type.Module);
            var il = dynamicMethod.GetILGenerator();
            var src = il.DeclareLocal(obj.GetType());
            var tar = il.DeclareLocal(type);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Isinst, obj.GetType());
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Newobj, type.GetConstructor(new Type[0]));
            il.Emit(OpCodes.Stloc_1);
            var source = obj.GetType().GetFields(BindingFlags.GetField|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance);
            foreach (var field in source)
            {

                if (field.Name.Contains("k__BackingField"))
                {
                    continue;
                }
                var p = type.GetField(field.Name,BindingFlags.Instance| BindingFlags.Public | BindingFlags.NonPublic);
            
              
                if (p != null)
                {
                    if (p.FieldType.Equals(field.FieldType) || !p.FieldType.IsValueType)
                    {
                        il.Emit(OpCodes.Ldloc, tar);
                        il.Emit(OpCodes.Ldloc, src);
                        il.Emit(OpCodes.Ldfld, field);
                        il.Emit(OpCodes.Stfld, p);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Ldfld, field);

                        if (p.FieldType.IsValueType)
                        {
                            var mth = GetMethod(p.FieldType);

                            var m = typeof(Convert).GetMethod(mth, new Type[] { field.FieldType });
                            if (m == null && field.FieldType.IsClass)
                            {
                                m = typeof(Convert).GetMethod(mth, new Type[] { typeof(object) });
                            }
                            il.Emit(OpCodes.Call, m);
                        }
                        il.Emit(OpCodes.Stfld, p);

                    }
                }
            }
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);
            FastInvoke fastInvok = (FastInvoke)dynamicMethod.CreateDelegate(typeof(FastInvoke));
            return fastInvok;

        }

        private static FastInvoke GetFastInvokFTFI<T>(Type type, T obj)
        {
            string name = typeof(T).FullName + "To" + type.FullName + "_" + MapType.FTFI;
            DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(object), new Type[] { typeof(object) }, type.Module);
            var il = dynamicMethod.GetILGenerator();
            var src = il.DeclareLocal(obj.GetType());
            var tar = il.DeclareLocal(type);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Isinst, obj.GetType());
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Newobj, type.GetConstructor(new Type[0]));
            il.Emit(OpCodes.Stloc_1);
            var source = obj.GetType().GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in source)
            {

                if (field.Name.Contains("k__BackingField"))
                {
                    continue;
                }
                var p = type.GetField(field.Name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);


                if (p != null)
                {
                    if (p.FieldType.Equals(field.FieldType) || !p.FieldType.IsValueType)
                    {
                        il.Emit(OpCodes.Ldloc, tar);
                        il.Emit(OpCodes.Ldloc, src);
                        il.Emit(OpCodes.Ldfld, field);
                        il.Emit(OpCodes.Stfld, p);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Ldfld, field);

                        if (p.FieldType.IsValueType)
                        {
                            var mth = GetMethod(p.FieldType);

                            var m = typeof(Convert).GetMethod(mth, new Type[] { field.FieldType });
                            if (m == null && field.FieldType.IsClass)
                            {
                                m = typeof(Convert).GetMethod(mth, new Type[] { typeof(object) });
                            }
                            il.Emit(OpCodes.Call, m);
                        }
                        il.Emit(OpCodes.Stfld, p);

                    }
                }
            }
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);
            FastInvoke fastInvok = (FastInvoke)dynamicMethod.CreateDelegate(typeof(FastInvoke));
            return fastInvok;

        }

        private static FastInvoke GetFastInvokPTF_TP<T>(Type type, T obj)
        {
            string name = typeof(T).FullName + "To" + type.FullName + "_" + MapType.PTF_TP;
            DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(object), new Type[] { typeof(object) }, type.Module);
            var il = dynamicMethod.GetILGenerator();
            var src = il.DeclareLocal(obj.GetType());
            var tar = il.DeclareLocal(type);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Isinst, obj.GetType());
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Newobj, type.GetConstructor(new Type[0]));
            il.Emit(OpCodes.Stloc_1);
            var source = obj.GetType().GetProperties();
            foreach (var pty in source)
            {

                var p = type.GetField(pty.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (p != null)
                {
                    if (p.FieldType.Equals(pty.PropertyType) || !p.FieldType.IsValueType)
                    {
                        il.Emit(OpCodes.Ldloc, tar);
                        il.Emit(OpCodes.Ldloc, src);
                        il.Emit(OpCodes.Call, pty.GetMethod);
                        il.Emit(OpCodes.Stfld, p);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Call, pty.GetMethod);

                        if (p.FieldType.IsValueType)
                        {
                            var mth = GetMethod(p.FieldType);

                            var m = typeof(Convert).GetMethod(mth, new Type[] { pty.PropertyType });
                            if (m == null && pty.PropertyType.IsClass)
                            {
                                m = typeof(Convert).GetMethod(mth, new Type[] { typeof(object) });
                            }
                            il.Emit(OpCodes.Call, m);
                        }
                        il.Emit(OpCodes.Stfld, p);

                    }
                }
                //
                var pro = type.GetProperty(pty.Name, BindingFlags.IgnoreCase | BindingFlags.Public);
                if (pro != null)
                {
                    if (pro.PropertyType.Equals(pty.PropertyType) || !pro.PropertyType.IsValueType)
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Call, pty.GetMethod);
                        il.Emit(OpCodes.Call, pro.SetMethod);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Call, pty.GetMethod);

                        if (pro.PropertyType.IsValueType)
                        {
                            var mth = GetMethod(pro.PropertyType);

                            var m = typeof(Convert).GetMethod(mth, new Type[] { pty.PropertyType });
                            if (m == null && pty.PropertyType.IsClass)
                            {
                                m = typeof(Convert).GetMethod(mth, new Type[] { typeof(object) });
                            }
                            il.Emit(OpCodes.Call, m);
                        }
                        il.Emit(OpCodes.Call, pro.SetMethod);

                    }
                }


            }
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);
            FastInvoke fastInvok = (FastInvoke)dynamicMethod.CreateDelegate(typeof(FastInvoke));
            return fastInvok;

        }

        private static FastInvoke GetFastInvokFPTFP<T>(Type type, T obj)
        {
            string name = typeof(T).FullName + "To" + type.FullName + "_" + MapType.FPTFP;
            DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(object), new Type[] { typeof(object) }, type.Module);
            var il = dynamicMethod.GetILGenerator();
            var src = il.DeclareLocal(obj.GetType());
            var tar = il.DeclareLocal(type);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Isinst, obj.GetType());
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Newobj, type.GetConstructor(new Type[0]));
            il.Emit(OpCodes.Stloc_1);
            var source = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            FieldEmit(il, type, source);
            var properties = obj.GetType().GetProperties();
            PropertyEmit(il, type, properties);

            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);
            FastInvoke fastInvok = (FastInvoke)dynamicMethod.CreateDelegate(typeof(FastInvoke));
            return fastInvok;

        }

        private static void FieldEmit(ILGenerator il, Type type,FieldInfo[] source)
        {
            foreach (var field in source)
            {
                if (field.Name.Contains("k__BackingField"))
                {
                    continue;
                }

                var p = type.GetField(field.Name, BindingFlags.IgnoreCase| BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (p != null)
                {
                    if (p.FieldType.Equals(field.FieldType) || !p.FieldType.IsValueType)
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Ldfld, field);
                        il.Emit(OpCodes.Stfld, p);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Ldfld, field);

                        if (p.FieldType.IsValueType)
                        {
                            var mth = GetMethod(p.FieldType);

                            var m = typeof(Convert).GetMethod(mth, new Type[] { field.FieldType });
                            if (m == null && field.FieldType.IsClass)
                            {
                                m = typeof(Convert).GetMethod(mth, new Type[] { typeof(object) });
                            }
                            il.Emit(OpCodes.Call, m);
                        }
                        il.Emit(OpCodes.Stfld, p);

                    }
                }
                //
                var pro = type.GetProperty(field.Name, BindingFlags.IgnoreCase | BindingFlags.Public);
                if (pro != null)
                {
                    if (pro.PropertyType.Equals(field.FieldType) || !pro.PropertyType.IsValueType)
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldfld, field);
                        il.Emit(OpCodes.Call, pro.SetMethod);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldfld, field);

                        if (pro.PropertyType.IsValueType)
                        {
                            var mth = GetMethod(pro.PropertyType);

                            var m = typeof(Convert).GetMethod(mth, new Type[] { field.FieldType });
                            if (m == null && field.FieldType.IsClass)
                            {
                                m = typeof(Convert).GetMethod(mth, new Type[] { typeof(object) });
                            }
                            il.Emit(OpCodes.Call, m);
                        }
                        il.Emit(OpCodes.Call, pro.SetMethod);

                    }
                }
            }
        }

        private static void PropertyEmit(ILGenerator il, Type type, PropertyInfo[] source)
        {
            foreach (var pty in source)
            {

                var p = type.GetField(pty.Name,BindingFlags.IgnoreCase| BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (p != null)
                {
                    if (p.FieldType.Equals(pty.PropertyType) || !p.FieldType.IsValueType)
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Call, pty.GetMethod);
                        il.Emit(OpCodes.Stfld, p);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Call, pty.GetMethod);

                        if (p.FieldType.IsValueType)
                        {
                            var mth = GetMethod(p.FieldType);

                            var m = typeof(Convert).GetMethod(mth, new Type[] { pty.PropertyType });
                            if (m == null && pty.PropertyType.IsClass)
                            {
                                m = typeof(Convert).GetMethod(mth, new Type[] { typeof(object) });
                            }
                            il.Emit(OpCodes.Call, m);
                        }
                        il.Emit(OpCodes.Stfld, p);

                    }
                }
                //
                var pro = type.GetProperty(pty.Name, BindingFlags.IgnoreCase | BindingFlags.Public);
                if (pro != null)
                {
                    if (pro.PropertyType.Equals(pty.PropertyType) || !pro.PropertyType.IsValueType)
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Call, pty.GetMethod);
                        il.Emit(OpCodes.Call, pro.SetMethod);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Call, pty.GetMethod);

                        if (pro.PropertyType.IsValueType)
                        {
                            var mth = GetMethod(pro.PropertyType);

                            var m = typeof(Convert).GetMethod(mth, new Type[] { pty.PropertyType });
                            if (m == null && pty.PropertyType.IsClass)
                            {
                                m = typeof(Convert).GetMethod(mth, new Type[] { typeof(object) });
                            }
                            il.Emit(OpCodes.Call, m);
                        }
                        il.Emit(OpCodes.Call, pro.SetMethod);

                    }
                }


            }
        }


        private static FastInvoke GetFastInvok1<T>(Type type, T obj)
        {
           // DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType.Module);
            var assemblyName = new AssemblyName("Kitty");
            var assemblyBuilder = AppDomain.CurrentDomain.
                          DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("KittyModule", "Kitty.dll");
            var typeBuilder = moduleBuilder.DefineType("HelloKittyClass", TypeAttributes.Public);
            // create method builder
            var methodBuilder = typeBuilder.DefineMethod(
              "SayHelloMethod",
              MethodAttributes.Public |  MethodAttributes.HideBySig,
              typeof(object),
              new Type[] { typeof(object) });
            //
            var il = methodBuilder.GetILGenerator();
             var tar=  il.DeclareLocal(type);
            il.Emit(OpCodes.Newobj, type.GetConstructor(new Type[0]));
            il.Emit(OpCodes.Stloc_0);
            var source=  obj.GetType().GetProperties();
            foreach(var pty in source)
            {
               var p= type.GetProperty(pty.Name);
                if(p!=null)
                {
                    if (p.PropertyType.Equals(pty.PropertyType)||!p.PropertyType.IsValueType)
                    {
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Call, pty.GetMethod);
                        il.Emit(OpCodes.Call, p.SetMethod);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Call, pty.GetMethod);
                      
                        if(p.PropertyType.IsValueType)
                        {
                           var name= GetMethod(p.PropertyType);
                           // Convert.ToInt32();
                            var m = typeof(Convert).GetMethod(name,new Type[] {pty.PropertyType});
                            if(m==null&&pty.PropertyType.IsClass)
                            {
                                m = typeof(Convert).GetMethod(name, new Type[] { typeof(object) });
                            }
                            il.Emit(OpCodes.Call, m);
                        }
                        il.Emit(OpCodes.Call, p.SetMethod);

                    }
                }
            }

            //   var sest = type.GetProperties();
            // then get the method il generator


            // then create the method function
            //il.Emit(OpCodes.Ldstr, "Hello, Kitty!");
            //il.Emit(OpCodes.Call,
            //  typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
            //il.Emit(OpCodes.Call, typeof(Console).GetMethod("ReadLine"));
            //il.Emit(OpCodes.Pop); // we just read something here, throw it.
            //il.Emit(OpCodes.Ret);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
            var helloKittyClassType = typeBuilder.CreateType();
            assemblyBuilder.Save("Kitty.dll");
            
            FastInvoke fastInvok = (FastInvoke)Delegate.CreateDelegate(typeof(FastInvoke),null ,helloKittyClassType.GetMethod("SayHelloMethod"));
            return fastInvok;
        }

        private static FastInvoke GetFastInvok2<T>(Type type, T obj)
        {
            // DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType.Module);
            var assemblyName = new AssemblyName("Kitty");
            var assemblyBuilder = AppDomain.CurrentDomain.
                          DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("KittyModule", "Kitty.dll");
            var typeBuilder = moduleBuilder.DefineType("HelloKittyClass", TypeAttributes.Public);
            // create method builder
            var methodBuilder = typeBuilder.DefineMethod(
              "SayHelloMethod",
              MethodAttributes.Public | MethodAttributes.HideBySig,
              typeof(object),
              new Type[] { typeof(object) });
            //
            var il = methodBuilder.GetILGenerator();
            var src = il.DeclareLocal(obj.GetType());
            var tar = il.DeclareLocal(type);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Isinst, obj.GetType());
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Newobj, type.GetConstructor(new Type[0]));
            il.Emit(OpCodes.Stloc_1);
         
            var source = obj.GetType().GetFields(BindingFlags.SetField|BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in source)
            {
                if (field.Name.Contains("k__BackingField"))
                {
                    continue;
                }
                var p = type.GetField(field.Name,BindingFlags.NonPublic| BindingFlags.IgnoreCase|BindingFlags.Public|BindingFlags.Instance);
                if (p != null)
                {
                    if (p.FieldType.Equals(field.FieldType) || !p.FieldType.IsValueType)
                    {
                        il.Emit(OpCodes.Ldloc,tar);
                        il.Emit(OpCodes.Ldloc,src);
                        il.Emit(OpCodes.Ldfld, field);
                        il.Emit(OpCodes.Stfld, p);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Ldfld, field);

                        if (p.FieldType.IsValueType)
                        {
                            var name = GetMethod(p.FieldType);
                            // Convert.ToInt32();
                            var m = typeof(Convert).GetMethod(name, new Type[] { field.FieldType });
                            if (m == null && field.FieldType.IsClass)
                            {
                                m = typeof(Convert).GetMethod(name, new Type[] { typeof(object) });
                            }
                            il.Emit(OpCodes.Call, m);
                        }
                        il.Emit(OpCodes.Stfld, p);

                    }
                }
            }

            il.Emit(OpCodes.Ldloc_1);
           
            il.Emit(OpCodes.Ret);
            var helloKittyClassType = typeBuilder.CreateType();
            assemblyBuilder.Save("Kitty.dll");

            FastInvoke fastInvok = (FastInvoke)Delegate.CreateDelegate(typeof(FastInvoke), null, helloKittyClassType.GetMethod("SayHelloMethod"));
            return fastInvok;
        }



    }
}
