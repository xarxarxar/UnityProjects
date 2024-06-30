
require("InitClass")--导入准备好的类别名
print("准备就绪")
require("ItemData")--初始化道具表信息

--玩家信息
--1.单机游戏从本地读取
--PlayerPrefs、 Json、 或者2进制
--2.网络游戏从服务器读取
require("PlayerData")
PlayerData:Init()