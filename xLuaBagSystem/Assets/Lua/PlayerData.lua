PlayerData = {}

--目前制作背包功能，所以只需要它的道具信息即可

PlayerData.equips={}
PlayerData.items={}
PlayerData.gems={}

--为玩家数据写了一个初始化方法
function PlayerData:Init()
    --道具信息不管存本地还是存服务器，都只需要存储道具的ID和数量

    --目前为了测试，就写死道具数据作为玩家信息
    table.insert(self.equips,{id = 1, num =1})
    table.insert(self.equips,{id = 12, num =1})

    table.insert(self.items,{id = 3, num =50})
    table.insert(self.items,{id = 4, num =20})

    table.insert(self.gems,{id = 5, num =99})
    table.insert(self.gems,{id = 6, num =88})
end

PlayerData:Init()