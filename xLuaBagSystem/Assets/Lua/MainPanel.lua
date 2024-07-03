--只要是一个新的对象（面板）那我们就新建一张表
MainPanel = {}

--这里不是必须写的，因为lua类似于python不需要事先声明变量，在下面要用的话可以直接使用。这样写的目的是当别人看这个lua代码时，知道这个表有什么变量
MainPanel.panelObj = nil --关联的面板对象
--对应的面板控件
MainPanel.btnRole = nil --主角按钮
MainPanel.btnSkill = nil --技能按钮

--这个表需要做实例化面板对象
--为这个面板处理对应的逻辑 比如按钮点击等等

--初始化该面板，实例化对象 控制事件监听
function MainPanel:Init( )
	if self.panelObj ~= nil then -- 面板没有实例化过，才需要实例化处理
	return
	end
	--1.实例化面板对象，设置父对象
	self.panelObj= ABMgr:LoadRes("ui","MainPanel",typeof(GameObject))
	self.panelObj.transform:SetParent(Canvas,false)
	--2.找到对应控件
	--找到子对象，再找到身上的Button组件
	self.btnRole = self.panelObj.transform:Find("btnRole"):GetComponent(typeof(Button))
	print(self.btnRole.name)
	--3.为控件加上事件监听，进行点击等等的逻辑处理
	--如果这样写，会导致self在作为回调函数传递的时候丢失对于MainPanel的引用，
	--因为用“:”调用的时候，会将调用的对象作为隐式的self传递进去，而当函数作为回调函数传递的时候，传递的仅仅是函数的引用，会丢失对于self的引用
	--self.btnRole.onClick:AddListener(self.btnClick)
	--使用匿名函数解决上诉问题
	self.btnRole.onClick:AddListener(function ()
		self:btnClick() --这里相当于点击的回调函数是这个匿名函数，只是在匿名函数里面调用了self:btnClick，因此self的引用并没有丢失
	end)
end

function MainPanel:ShowMe( )
	-- body
	self:Init()
	self.panelObj:SetActive(true)
end

function MainPanel:HideMe()
	-- body
	self.panelObj:SetActive(false)
end

function MainPanel:btnClick()
	-- body
	print(123123)
	print(self.btnRole)
end

