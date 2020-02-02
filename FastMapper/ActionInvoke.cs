#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：TestLib
*项目描述 ：
*命名空间 ：TestLib
*文件名称 ：ActionInvoke.cs
* 功能描述 ：ActionInvoke
* 创建时间 ：2020
*版本号   :   2020|V1.0.0.0 
---------------------------------------------------------------------
* Copyright @ jinyu 2020. All rights reserved.
---------------------------------------------------------------------

***********************************************************************/
#endregion



using System;

namespace FastMapper
{
    /// <summary>
    /// 委托代理类
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDest"></typeparam>
    public class ActionInvoke<TSource,TDest>
    {

        /// <summary>
        /// 执行的委托
        /// </summary>
        public Action<TSource,TDest> Action { get; set; }
    }
}
