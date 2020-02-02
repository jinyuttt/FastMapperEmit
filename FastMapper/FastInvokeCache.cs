#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：TestLib
*项目描述 ：
*命名空间 ：TestLib
*文件名称 ：FastInvoke.cs
* 功能描述 ：FastInvoke
* 创建时间 ：2020
*版本号   :   2020|V1.0.0.0 
---------------------------------------------------------------------
* Copyright @ jinyu 2020. All rights reserved.
---------------------------------------------------------------------

***********************************************************************/
#endregion



using System;
using System.Collections.Concurrent;

namespace FastMapper
{

    /// <summary>
    /// 缓存委托对象
    /// </summary>
    public  class FastInvokeCache
    {

        private readonly ConcurrentDictionary<string, FastInvoke> dicPTP = null;
        private readonly ConcurrentDictionary<string, FastInvoke> dicPTPI = null;
        private readonly ConcurrentDictionary<string, FastInvoke> dicFTF = null;
        private readonly ConcurrentDictionary<string, FastInvoke> dicFTFI=null;
        private readonly ConcurrentDictionary<string, FastInvoke> dicFPTFP=null;
        private readonly ConcurrentDictionary<string, FastInvoke> dicPTF_TP=null;
        private readonly ConcurrentDictionary<string, FastInvoke> dicFPTFPI=null;
        private readonly ConcurrentDictionary<string, FastInvoke> dicOhers=null;

        private static readonly Lazy<FastInvokeCache> cache = new Lazy<FastInvokeCache>();

        public static FastInvokeCache Instance
        {
            get { return cache.Value; }
        }
        public FastInvokeCache()
        {
            dicPTP = new ConcurrentDictionary<string, FastInvoke>();
            dicPTPI = new ConcurrentDictionary<string, FastInvoke>();
            dicFTF = new ConcurrentDictionary<string, FastInvoke>();
            dicFTFI = new ConcurrentDictionary<string, FastInvoke>();
            dicFPTFP = new ConcurrentDictionary<string, FastInvoke>();
            dicPTF_TP = new ConcurrentDictionary<string, FastInvoke>();
            dicFPTFPI = new ConcurrentDictionary<string, FastInvoke>();
            dicOhers = new ConcurrentDictionary<string, FastInvoke>();
        }

        public void Add(string name,MapType mapType, FastInvoke fastInvok)
        {
           
              switch (mapType)
            {
                case MapType.PTP:
                    dicPTP[name] = fastInvok;
                    break;
                case MapType.PTPI:
                    dicPTPI[name] = fastInvok;
                    break;
                case MapType.FTF:
                    dicFTF[name] = fastInvok;
                    break;
                case MapType.FTFI:
                    dicFTFI[name] = fastInvok;
                    break;
                case MapType.PTF_TP:
                    dicPTF_TP[name] = fastInvok;
                    break;
                case MapType.FPTFP:
                    dicFPTFP[name] = fastInvok;
                    break;
                case MapType.FPTFPI:
                    dicFPTFPI[name] = fastInvok;
                    break;
                default:
                    dicOhers[name] = fastInvok;
                    break;

            }

        
        }
        
        public FastInvoke GetFastInvok(string name,MapType mapType)
        {
            FastInvoke fastInvoke = null;
            switch (mapType)
            {
                case MapType.PTP:
                    dicPTP.TryGetValue(name, out fastInvoke);
                    break;
                case MapType.PTPI:
                   
                    dicPTPI.TryGetValue(name, out fastInvoke);
                    break;
                case MapType.FTF:
                 
                    dicFTF.TryGetValue(name, out fastInvoke);
                    break;
                case MapType.FTFI:
                 
                    dicFTFI.TryGetValue(name, out fastInvoke);
                    break;
                case MapType.PTF_TP:
                 
                    dicPTF_TP.TryGetValue(name, out fastInvoke);
                    break;
                case MapType.FPTFP:
                  
                    dicFPTFP.TryGetValue(name, out fastInvoke);
                    break;
                case MapType.FPTFPI:
                 
                    dicFPTFPI.TryGetValue(name, out fastInvoke);
                    break;
                default:
                   
                    dicOhers.TryGetValue(name, out fastInvoke);
                    break;

            }
            return fastInvoke;

        }
    }
}
