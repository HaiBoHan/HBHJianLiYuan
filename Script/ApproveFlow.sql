
/*
delete	from Approval_DocumentType 
where EntityType = 'U9.VOB.Cus.HBHJianLiYuan.DayCheckIn'
delete	from Approval_DocumentType_Trl
where Name in ('DayCheckIn','�տ���')
*/

	--��� ������ʵ��� ����ID
	declare @maxID bigint
	declare @curID bigint


--���������,�Ŵ���
if not Exists( select ID 
			from Approval_DocumentType 
			where EntityType = 'U9.VOB.Cus.HBHJianLiYuan.DayCheckIn' )
begin
	--��� ������ʵ��� ����ID
	select @maxID = max(ID) from Approval_DocumentType 
	set @curID = @maxID + 11
	
	--�� ������ʵ��� �в���Ԥ������
	insert into Approval_DocumentType
	(ID,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy,SysVersion
	,ParentToChildPath,EntityType,HROrgExpr
	,PositionExpr,Code,Parent
	--URL������ת,GUID���ǵ���
	,Uri
	,DisplayNameExpr,ConditionExpr,UriExpr,Responsibility
	,Template,TimeLimite,ListUri)
	values(@curID,GETDATE(),'hbh',GETDATE(),'hbh',0
	,null,'U9.VOB.Cus.HBHJianLiYuan.DayCheckIn','DayCheckIn.Org'
	,'GetPositionByCreateBy(DayCheckIn.CreatedBy)','DayCheckIn',null
	--URL������ת,GUID���ǵ���   URL='UFIDA.U9.Cust.XR.LackSendModifyUI'   GUID='d109936a-92d0-4ac3-9bc9-7028c172af21'
	,'eff032a7-ce78-48c1-bb3f-f992965caa0d'
	,'Department.Name + '':'' + Convert(char(10),DayCheckIn.CheckInDate,120)',null,null,0
	,null,90,null)
	
	
	--�� ������ʵ����Դ�� �в���Ԥ������
	insert into Approval_DocumentType_Trl
	(ID,Name,SysMLFlag)
	(select @curID,'DayCheckIn','en-US'
	union select @curID,'�տ���','zh-CN')
	
end



--���������,�Ŵ���
if not Exists( select ID 
			from Approval_DocumentType 
			where EntityType = 'U9.VOB.Cus.HBHJianLiYuan.TotalPayrollDoc' )
begin
	select @maxID = max(ID) from Approval_DocumentType 
	set @curID = @maxID + 11
	
	--�� ������ʵ��� �в���Ԥ������
	insert into Approval_DocumentType
	(ID,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy,SysVersion
	,ParentToChildPath,EntityType,HROrgExpr
	,PositionExpr,Code,Parent
	--URL������ת,GUID���ǵ���
	,Uri
	,DisplayNameExpr,ConditionExpr,UriExpr,Responsibility
	,Template,TimeLimite,ListUri)
	values(@curID,GETDATE(),'hbh',GETDATE(),'hbh',0
	,null,'U9.VOB.Cus.HBHJianLiYuan.TotalPayrollDoc','TotalPayrollDoc.Org'
	,'GetPositionByCreateBy(TotalPayrollDoc.CreatedBy)','TotalPayrollDoc',null
	--URL������ת,GUID���ǵ���   URL='UFIDA.U9.Cust.XR.LackSendModifyUI'   GUID='d109936a-92d0-4ac3-9bc9-7028c172af21'
	,'f3740355-ee66-46c3-9efe-c573fd2bacf3'
	,'PayrollType.Name + '':'' + Convert(char(10),TotalPayrollDoc.PayDate,120)',null,null,0
	,null,90,null)
	
	
	--�� ������ʵ����Դ�� �в���Ԥ������
	insert into Approval_DocumentType_Trl
	(ID,Name,SysMLFlag)
	(select @curID,'TotalPayrollDoc','en-US'
	union select @curID,'н��������ܱ�','zh-CN')
	
end



