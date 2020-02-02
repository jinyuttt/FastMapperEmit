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


namespace FastMapper
{

    /// <summary>
    /// 转换类型
    /// </summary>
    public enum MapType
    {
        /// <summary>
        /// 公共属性转换
        /// </summary>
        PTP,
       /// <summary>
       ///公共属性转换，名称不区分大小写
       /// </summary>
        PTPI,
        /// <summary>
        /// 公共字段间转换
        /// </summary>
        FTF,

       /// <summary>
       /// 公共字段转换不区分大小写
       /// </summary>
        FTFI,

        /// <summary>
        /// 公共属性先转字段（字段包括非公共）
        /// 然后转公共属性
        /// </summary>
        PTF_TP,

        /// <summary>
        /// 查找所有名称相同的字段、属性转换
        /// 以属性值为主
        /// </summary>
        FPTFP,
        FPTFPI,
    }
}
