--ֻҪ��һ���µĶ�����壩�����Ǿ��½�һ�ű�
MainPanel = {}

--���ﲻ�Ǳ���д�ģ���Ϊlua������python����Ҫ��������������������Ҫ�õĻ�����ֱ��ʹ�á�����д��Ŀ���ǵ����˿����lua����ʱ��֪���������ʲô����
MainPanel.panelObj = nil --������������
--��Ӧ�����ؼ�
MainPanel.btnRole = nil --���ǰ�ť
MainPanel.btnSkill = nil --���ܰ�ť

--�������Ҫ��ʵ����������
--Ϊ�����崦���Ӧ���߼� ���簴ť����ȵ�

--��ʼ������壬ʵ�������� �����¼�����
function MainPanel:Init( )
	if self.panelObj ~= nil then -- ���û��ʵ������������Ҫʵ��������
	return
	end
	--1.ʵ�������������ø�����
	self.panelObj= ABMgr:LoadRes("ui","MainPanel",typeof(GameObject))
	self.panelObj.transform:SetParent(Canvas,false)
	--2.�ҵ���Ӧ�ؼ�
	--�ҵ��Ӷ������ҵ����ϵ�Button���
	self.btnRole = self.panelObj.transform:Find("btnRole"):GetComponent(typeof(Button))
	print(self.btnRole.name)
	--3.Ϊ�ؼ������¼����������е���ȵȵ��߼�����
	--�������д���ᵼ��self����Ϊ�ص��������ݵ�ʱ��ʧ����MainPanel�����ã�
	--��Ϊ�á�:�����õ�ʱ�򣬻Ὣ���õĶ�����Ϊ��ʽ��self���ݽ�ȥ������������Ϊ�ص��������ݵ�ʱ�򣬴��ݵĽ����Ǻ��������ã��ᶪʧ����self������
	--self.btnRole.onClick:AddListener(self.btnClick)
	--ʹ���������������������
	self.btnRole.onClick:AddListener(function ()
		self:btnClick() --�����൱�ڵ���Ļص��������������������ֻ���������������������self:btnClick�����self�����ò�û�ж�ʧ
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

