#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：FastMapper
*项目描述 ：
*命名空间 ：FastMapper
*文件名称 ：FastMap.cs
* 功能描述 ：FastMap
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

namespace FastMapper
{
    public delegate object FastInvoke(object obj);


    /// <summary>
    /// 转换类
    /// </summary>
    public class FastMap
    {

        /// <summary>
        /// 保存转换补充的委托
        /// </summary>
        private static readonly ConcurrentDictionary<string, object> dicActions = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 生成对应转换
        /// </summary>
        /// <typeparam name="TSource">源</typeparam>
        /// <typeparam name="TDest">目标</typeparam>
        /// <param name="mapType">转换方式</param>
        /// <param name="func">独立委托</param>
        /// <param name="key">单独命名独立委托缓存</param>
        public static  void Bind<TSource,TDest>(MapType mapType = MapType.PTP, Action<TSource, TDest> func = null, string key = null)
        {
            string name = typeof(TSource).FullName + "To" + typeof(TDest).FullName + "_" + mapType;
            if (string.IsNullOrEmpty(key))
            {
                key = name;
            }
            FastInvoke fastInvok = FastInvokeCache.Instance.GetFastInvok(name, mapType);
            if (fastInvok != null)
            {
              
                return ;
            }
            else
            {
                switch (mapType)
                {
                    case MapType.PTP:
                        fastInvok = GetFastInvokPTP(typeof(TDest), typeof(TSource));
                        break;
                    case MapType.PTPI:
                        fastInvok = GetFastInvokPTPI(typeof(TDest), typeof(TSource));
                        break;
                    case MapType.FTF:
                        fastInvok = GetFastInvokFTF(typeof(TDest), typeof(TSource));
                        break;
                    case MapType.FTFI:
                        fastInvok = GetFastInvokFTFI(typeof(TDest), typeof(TSource));
                        break;
                    case MapType.PTF_TP:
                        fastInvok = GetFastInvokPTF_TP(typeof(TDest), typeof(TSource));
                        break;
                    case MapType.FPTFP:
                        fastInvok = GetFastInvokFPTFP(typeof(TDest), typeof(TSource));
                        break;
                    case MapType.FPTFPI:
                        fastInvok = GetFastInvokFPTFP(typeof(TDest), typeof(TSource));
                        break;
                    default:
                        fastInvok = GetFastInvokPTP(typeof(TDest), typeof(TSource));
                        break;

                }
            }

         
            //执行成功的缓存
            FastInvokeCache.Instance.Add(name, mapType, fastInvok);
            if (func != null)
            {
               
                ActionInvoke<TSource, TDest> action = new ActionInvoke<TSource, TDest>
                {
                    Action = func
                };

                dicActions[key] = action;
            }
        
        }

        /// <summary>
        /// 生成对应转换
        /// </summary>
        /// <param name="TSource"></param>
        /// <param name="TDest"></param>
        /// <param name="mapType"></param>
        public static void Bind(Type TSource,Type TDest, MapType mapType = MapType.PTP)
        {
            string name = TSource.FullName + "To" + TDest.FullName + "_" + mapType;
           
            FastInvoke fastInvok = FastInvokeCache.Instance.GetFastInvok(name, mapType);
            if (fastInvok != null)
            {

                return;
            }
            else
            {
                switch (mapType)
                {
                    case MapType.PTP:
                        fastInvok = GetFastInvokPTP(TDest, TSource);
                        break;
                    case MapType.PTPI:
                        fastInvok = GetFastInvokPTPI(TDest, TSource);
                        break;
                    case MapType.FTF:
                        fastInvok = GetFastInvokFTF(TDest, TSource);
                        break;
                    case MapType.FTFI:
                        fastInvok = GetFastInvokFTFI(TDest, TSource);
                        break;
                    case MapType.PTF_TP:
                        fastInvok = GetFastInvokPTF_TP(TDest, TSource);
                        break;
                    case MapType.FPTFP:
                        fastInvok = GetFastInvokFPTFP(TDest, TSource);
                        break;
                    case MapType.FPTFPI:
                        fastInvok = GetFastInvokFPTFP(TDest, TSource);
                        break;
                    default:
                        fastInvok = GetFastInvokPTP(TDest, TSource);
                        break;

                }
            }
        }


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

            string name = typeof(TSource).FullName + "To" + typeof(TDest).FullName + "_" + mapType;
            if (string.IsNullOrEmpty(key))
            {
                key = name;
            }
            FastInvoke fastInvok = FastInvokeCache.Instance.GetFastInvok(name, mapType);
            if (fastInvok != null)
            {
                var r = (TDest)fastInvok.Invoke(obj);
                object action = null;
                if (dicActions.TryGetValue(key, out action))
                {

                    ActionInvoke<TSource, TDest> actionInvoke = (ActionInvoke<TSource, TDest>)action;
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
                        fastInvok = GetFastInvokPTP(typeof(TDest),obj.GetType());
                        break;
                    case MapType.PTPI:
                        fastInvok = GetFastInvokPTPI(typeof(TDest), obj.GetType());
                        break;
                    case MapType.FTF:
                        fastInvok = GetFastInvokFTF(typeof(TDest), obj.GetType());
                        break;
                    case MapType.FTFI:
                        fastInvok = GetFastInvokFTFI(typeof(TDest), obj.GetType());
                        break;
                    case MapType.PTF_TP:
                        fastInvok = GetFastInvokPTF_TP(typeof(TDest), obj.GetType());
                        break;
                    case MapType.FPTFP:
                        fastInvok = GetFastInvokFPTFP(typeof(TDest), obj.GetType());
                        break;
                    case MapType.FPTFPI:
                        fastInvok = GetFastInvokFPTFP(typeof(TDest), obj.GetType());
                        break;
                    default:
                        fastInvok = GetFastInvokPTP(typeof(TDest), obj.GetType());
                        break;

                }
            }

            TDest result = (TDest)fastInvok.Invoke(obj);
            //执行成功的缓存
            FastInvokeCache.Instance.Add(name, mapType, fastInvok);
            if (func != null)
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
        public static TDest Mapper<TDest>(object obj, MapType mapType = MapType.PTP, Action<object, TDest> func = null, string key = null)

         where TDest : class, new()
        {

            string name = obj.GetType().FullName + "To" + typeof(TDest).FullName + "_" + mapType;
            if (string.IsNullOrEmpty(key))
            {
                key = name;
            }
            FastInvoke fastInvok = FastInvokeCache.Instance.GetFastInvok(name, mapType);
            if (fastInvok != null)
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
                        fastInvok = GetFastInvokPTP(typeof(TDest), obj.GetType());
                        break;
                    case MapType.PTPI:
                        fastInvok = GetFastInvokPTPI(typeof(TDest), obj.GetType());
                        break;
                    case MapType.FTF:
                        fastInvok = GetFastInvokFTF(typeof(TDest), obj.GetType());
                        break;
                    case MapType.FTFI:
                        fastInvok = GetFastInvokFTFI(typeof(TDest), obj.GetType());
                        break;
                    case MapType.PTF_TP:
                        fastInvok = GetFastInvokPTF_TP(typeof(TDest), obj.GetType());
                        break;
                    case MapType.FPTFP:
                        fastInvok = GetFastInvokFPTFP(typeof(TDest), obj.GetType());
                        break;
                    case MapType.FPTFPI:
                        fastInvok = GetFastInvokFPTFP(typeof(TDest), obj.GetType());
                        break;
                    default:
                        fastInvok = GetFastInvokPTP(typeof(TDest), obj.GetType());
                        break;

                }
            }

            TDest result = (TDest)fastInvok.Invoke(obj);
            //执行成功的缓存
            FastInvokeCache.Instance.Add(name, mapType, fastInvok);
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

       
        /// <summary>
        /// 获取转换方法
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetMethod(Type type)
        {

            
            switch (type.Name)
            {
                case "Int32":
                    return "ToInt32";
                case "Boolean ":
                    return "ToBoolean";
                case "DateTime":
                    return "ToDateTime";
                case "Decimal":
                    return "ToDecimal";
                case "Double":
                    return "ToDouble";
                case "Int16":
                    return "ToInt16";
                case "Int64":
                    return "ToInt64";
                case "Single":
                    return "ToSingle";
                case "String":
                    return "ToString";
                case "UInt16":
                    return "ToUInt16";
                case "UInt32":
                    return "ToUInt32";
                case "UInt64":
                    return "ToUInt64";
                default:

                    return "ToInt32";

            }
        }
       
        private static FastInvoke GetFastInvokPTP(Type dest, Type sourceType)
        {
            string name = sourceType.FullName + "To" + dest.FullName + "_" + MapType.PTP;
            DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(object), new Type[] { typeof(object) }, dest.Module);
            var il = dynamicMethod.GetILGenerator();
            var src = il.DeclareLocal(sourceType);
            var tar = il.DeclareLocal(dest);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Isinst, sourceType);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Newobj, dest.GetConstructor(new Type[0]));
            il.Emit(OpCodes.Stloc_1);
            var source = sourceType.GetProperties();
            foreach (var pty in source)
            {
                var p = dest.GetProperty(pty.Name);
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

        private static FastInvoke GetFastInvokPTPI(Type dest, Type sourceType)
        {
            string name = sourceType.FullName + "To" + dest.FullName + "_" + MapType.PTPI;
            DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(object), new Type[] { typeof(object) }, dest.Module);
            var il = dynamicMethod.GetILGenerator();
            var src = il.DeclareLocal(sourceType);
            var tar = il.DeclareLocal(dest);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Isinst, sourceType);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Newobj, dest.GetConstructor(new Type[0]));
            il.Emit(OpCodes.Stloc_1);
            var source = sourceType.GetProperties();
            foreach (var pty in source)
            {
                var p = dest.GetProperty(pty.Name, BindingFlags.IgnoreCase | BindingFlags.Public);
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

        private static FastInvoke GetFastInvokFTF(Type dest, Type sourceType)
        {
            string name = sourceType.FullName + "To" + dest.FullName + "_" + MapType.FTF;
            DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(object), new Type[] { typeof(object) }, dest.Module);
            var il = dynamicMethod.GetILGenerator();
            var src = il.DeclareLocal(sourceType);
            var tar = il.DeclareLocal(dest);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Isinst, sourceType);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Newobj, dest.GetConstructor(new Type[0]));
            il.Emit(OpCodes.Stloc_1);
            var source = sourceType.GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in source)
            {

                if (field.Name.Contains("k__BackingField"))
                {
                    continue;
                }
                var p = dest.GetField(field.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);


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

        private static FastInvoke GetFastInvokFTFI(Type dest, Type sourceType)
        {
            string name = sourceType.FullName + "To" + dest.FullName + "_" + MapType.FTFI;
            DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(object), new Type[] { typeof(object) }, dest.Module);
            var il = dynamicMethod.GetILGenerator();
            var src = il.DeclareLocal(sourceType);
            var tar = il.DeclareLocal(dest);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Isinst, sourceType);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Newobj, dest.GetConstructor(new Type[0]));
            il.Emit(OpCodes.Stloc_1);
            var source = sourceType.GetFields(BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in source)
            {

                if (field.Name.Contains("k__BackingField"))
                {
                    continue;
                }
                var p = dest.GetField(field.Name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);


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

        private static FastInvoke GetFastInvokPTF_TP(Type dest,Type sourceType)
        {
            string name = sourceType.FullName + "To" + dest.FullName + "_" + MapType.PTF_TP;
            DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(object), new Type[] { typeof(object) }, dest.Module);
            var il = dynamicMethod.GetILGenerator();
            var src = il.DeclareLocal(sourceType);
            var tar = il.DeclareLocal(dest);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Isinst, sourceType);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Newobj, dest.GetConstructor(new Type[0]));
            il.Emit(OpCodes.Stloc_1);
            var source = sourceType.GetProperties();
            foreach (var pty in source)
            {

                var p = dest.GetField(pty.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

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
                var pro = dest.GetProperty(pty.Name, BindingFlags.IgnoreCase | BindingFlags.Public);
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

        private static FastInvoke GetFastInvokFPTFP(Type dest,Type sourceType)
        {
            string name = sourceType.FullName + "To" + dest.FullName + "_" + MapType.FPTFP;
            DynamicMethod dynamicMethod = new DynamicMethod(name, typeof(object), new Type[] { typeof(object) }, dest.Module);
            var il = dynamicMethod.GetILGenerator();
            var src = il.DeclareLocal(sourceType);
            var tar = il.DeclareLocal(dest);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Isinst, sourceType);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Newobj, dest.GetConstructor(new Type[0]));
            il.Emit(OpCodes.Stloc_1);
            var source = sourceType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            FieldEmit(il, dest, source);
            var properties = sourceType.GetProperties();
            PropertyEmit(il, dest, properties);

            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Ret);
            FastInvoke fastInvok = (FastInvoke)dynamicMethod.CreateDelegate(typeof(FastInvoke));
            return fastInvok;

        }

        private static void FieldEmit(ILGenerator il, Type type, FieldInfo[] source)
        {
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

                var p = type.GetField(pty.Name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

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

    }
}
