#region   文件版本注释
/************************************************************************
*CLR版本  ：4.0.30319.42000
*项目名称 ：TestLib
*项目描述 ：
*命名空间 ：TestLib
*文件名称 ：Test.cs
* 功能描述 ：Test
* 创建时间 ：2020
*版本号   :   2020|V1.0.0.0 
---------------------------------------------------------------------
* Copyright @ jinyu 2020. All rights reserved.
---------------------------------------------------------------------

***********************************************************************/
#endregion



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLib
{
  public  class Test
    {
        public  object GetUsers(object obj)
        {
            UserInfo info =obj as UserInfo;
            Users users = new Users();
            //users.Age = Convert.ToInt32(info.Age);
          
            //users.Name = info.UserName;
            users.Psw = info.psw;
          //  users.Age = Convert.ChangeType(info.UserName, typeof(Int32));
           
            return users;
        }
    }
}
