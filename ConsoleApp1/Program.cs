using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestLib;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
          var r=  FastMap.Mapper<UserInfo, Users>(new UserInfo() { UserName = "ww", Age = 12, Id = "23", psw = 345 }, MapType.PTP, new Action<UserInfo, Users>((k, V) => {
                V.Name = k.UserName;
                    }));
            r = FastMap.Mapper<UserInfo, Users>(new UserInfo() { UserName = "ww", Age = 12, Id = "23", psw = 345 }, MapType.PTP, new Action<UserInfo, Users>((k, V) => {
                V.Name = k.UserName;
            }));

            var r = FastMap.Mapper<Users>(new UserInfo() { UserName = "ww", Age = 12, Id = "23", psw = 345 }, MapType.FTF);
        }
    }
}
