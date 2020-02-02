# FastMapper
根据需要进行实体间转换

将实体进行转换，采用EMIT;
将实体转换分成几类，可以实体间公共属性转换，字段间转换，字段与属性转换。
样例：
最简单的属性间转换  
  var r = FastMap.Mapper<Users>(new UserInfo() { UserName = "ww", Age = 12, Id = "23", psw = 345 });

设置字段间转换：  
 var r = FastMap.Mapper<Users>(new UserInfo() { UserName = "ww", Age = 12, Id = "23", psw = 345 }, MapType.FTF);
  
  增加转换后需要处理的方法：  
    var r=  FastMap.Mapper<UserInfo, Users>(new UserInfo() { UserName = "ww", Age = 12, Id = "23", psw = 345 }, MapType.PTP, new Action<UserInfo, Users>((k, V) => {
                V.Name = k.UserName;
                    }));
