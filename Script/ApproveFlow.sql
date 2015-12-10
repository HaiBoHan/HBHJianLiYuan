
/*
delete	from Approval_DocumentType 
where EntityType = 'U9.VOB.Cus.HBHJianLiYuan.DayCheckIn'
delete	from Approval_DocumentType_Trl
where Name in ('DayCheckIn','日考勤')
*/

	--查出 工作流实体表 最大的ID
	declare @maxID bigint
	declare @curID bigint


--如果不存在,才创建
if not Exists( select ID 
			from Approval_DocumentType 
			where EntityType = 'U9.VOB.Cus.HBHJianLiYuan.DayCheckIn' )
begin
	--查出 工作流实体表 最大的ID
	select @maxID = max(ID) from Approval_DocumentType 
	set @curID = @maxID + 11
	
	--向 工作流实体表 中插入预置数据
	insert into Approval_DocumentType
	(ID,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy,SysVersion
	,ParentToChildPath,EntityType,HROrgExpr
	,PositionExpr,Code,Parent
	--URL则是跳转,GUID则是弹出
	,Uri
	,DisplayNameExpr,ConditionExpr,UriExpr,Responsibility
	,Template,TimeLimite,ListUri)
	values(@curID,GETDATE(),'hbh',GETDATE(),'hbh',0
	,null,'U9.VOB.Cus.HBHJianLiYuan.DayCheckIn','DayCheckIn.Org'
	,'GetPositionByCreateBy(DayCheckIn.CreatedBy)','DayCheckIn',null
	--URL则是跳转,GUID则是弹出   URL='UFIDA.U9.Cust.XR.LackSendModifyUI'   GUID='d109936a-92d0-4ac3-9bc9-7028c172af21'
	,'eff032a7-ce78-48c1-bb3f-f992965caa0d'
	,'Department.Name + '':'' + Convert(char(10),DayCheckIn.CheckInDate,120)',null,null,0
	,null,90,null)
	
	
	--向 工作流实体资源表 中插入预置数据
	insert into Approval_DocumentType_Trl
	(ID,Name,SysMLFlag)
	(select @curID,'DayCheckIn','en-US'
	union select @curID,'日考勤','zh-CN')
	
end



--如果不存在,才创建
if not Exists( select ID 
			from Approval_DocumentType 
			where EntityType = 'U9.VOB.Cus.HBHJianLiYuan.TotalPayrollDoc' )
begin
	select @maxID = max(ID) from Approval_DocumentType 
	set @curID = @maxID + 11
	
	--向 工作流实体表 中插入预置数据
	insert into Approval_DocumentType
	(ID,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy,SysVersion
	,ParentToChildPath,EntityType,HROrgExpr
	,PositionExpr,Code,Parent
	--URL则是跳转,GUID则是弹出
	,Uri
	,DisplayNameExpr,ConditionExpr,UriExpr,Responsibility
	,Template,TimeLimite,ListUri)
	values(@curID,GETDATE(),'hbh',GETDATE(),'hbh',0
	,null,'U9.VOB.Cus.HBHJianLiYuan.TotalPayrollDoc','TotalPayrollDoc.Org'
	,'GetPositionByCreateBy(TotalPayrollDoc.CreatedBy)','TotalPayrollDoc',null
	--URL则是跳转,GUID则是弹出   URL='UFIDA.U9.Cust.XR.LackSendModifyUI'   GUID='d109936a-92d0-4ac3-9bc9-7028c172af21'
	,'f3740355-ee66-46c3-9efe-c573fd2bacf3'
	,'PayrollType.Name + '':'' + Convert(char(10),TotalPayrollDoc.PayDate,120)',null,null,0
	,null,90,null)
	
	
	--向 工作流实体资源表 中插入预置数据
	insert into Approval_DocumentType_Trl
	(ID,Name,SysMLFlag)
	(select @curID,'TotalPayrollDoc','en-US'
	union select @curID,'薪资申请汇总表','zh-CN')
	
end



