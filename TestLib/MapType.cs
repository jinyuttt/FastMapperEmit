#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：FastMapper
*项目描述 ：
*命名空间 ：FastMapper
*文件名称 ：MapType.cs
* 功能描述 ：MapType
* 创建时间 ：2020
*版本号   :   2020|V1.0.0.0 
---------------------------------------------------------------------
* Copyright @ jinyu 2020. All rights reserved.
---------------------------------------------------------------------

***********************************************************************/
#endregion



using System;
using System.Collections.Generic;
using System.Text;

namespace TestLib
{

   /// <summary>
   /// 转换类型
   /// </summary>
   public enum MapType
    {
        /// <summary>
        /// 公共同名属性转换
        /// </summary>
        PTP,
       /// <summary>
       ///公共属性转换，名称不区分大小写
       /// </summary>
        PTPI,
        /// <summary>
        /// 所有字段间转换
        /// </summary>
        FTF,

       /// <summary>
       /// 所有字段转换不区分大小写
       /// </summary>
        FTFI,

        /// <summary>
        /// 用公共属性先转字段（字段包括非公共）
        /// 然后再转一次公共属性
        /// </summary>
        PTF_TP,

        /// <summary>
        /// 查找所有名称相同的字段、属性转换
        /// 先找到源类型所有字段，将其同名称赋值给目标对象的字段(字段忽略大小写)和属性
        /// 再找到源类型所有属性，将其同名称赋值给目标对象的字段（字段忽略大小写）和属性
        /// </summary>
        FPTFP,

        /// <summary>
        /// （没有实现）查找所有名称相同(忽略大小写)的字段、属性转换
        /// 以属性值为主
        /// 
        /// </summary>
        FPTFPI,
    }
}
