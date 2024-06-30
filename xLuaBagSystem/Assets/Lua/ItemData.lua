--首先应该先吧json文件从ab包里面加载出来
--加载的json文件的TextAsset对象
local txt= ABMgr:LoadRes("json","ItemData",typeof(TextAsset))
print(txt.text)
--获取它的文本信息进行json解析
local itemList = Json.decode(txt.text)
--加载出来是一个像数组的数据结构
--不方便我们通过id去获取里面的内容，所以用一张新表转存一次
--而且这张新的道具表在任何地方都能被使用，所以用全局变量
--一张用来存储道具信息的表
--键值对心事，键是道具ID，值是道具表一行信息
ItemData={}
for _, value in pairs(itemList) do
    ItemData[value.id] = value
end

print(ItemData[1].tips)--打印id为1的道具的tips