

/*
delete	from Approval_DocumentType_Trl
where ID in (select ID
			from Approval_DocumentType 
			where EntityType in (@EntityName,'U9.VOB.Cus.HBHJianLiYuan.TotalPayrollDoc')
			)
delete	from Approval_DocumentType 
where EntityType in (@EntityName,'U9.VOB.Cus.HBHJianLiYuan.TotalPayrollDoc')
*/

--查出 工作流实体表 最大的ID
declare @maxID bigint
declare @curID bigint
declare @StartID bigint = -1

declare @Code varchar(125) = ''
declare @Name varchar(125) = ''
declare @EntityName varchar(125) = ''
declare @HROrgExpr varchar(125) = ''
declare @PositionExpr varchar(125) = ''
declare @Uri varchar(125) = ''
declare @DisplayNameExpr varchar(125) = ''

	
set @Code = 'DayCheckIn'
set @Name = '日考勤'
set @EntityName = 'U9.VOB.Cus.HBHJianLiYuan.DayCheckIn'
set @HROrgExpr = 'DayCheckIn.Org'
set @PositionExpr = 'GetPositionByCreateBy(DayCheckIn.CreatedBy)'
--URL则是跳转,GUID则是弹出   URL='UFIDA.U9.Cust.XR.LackSendModifyUI'   GUID='d109936a-92d0-4ac3-9bc9-7028c172af21'
set @Uri = 'eff032a7-ce78-48c1-bb3f-f992965caa0d'
set @DisplayNameExpr = 'Department.Name + '':'' + Convert(char(10),DayCheckIn.CheckInDate,120)'

--如果不存在,才创建
if not Exists( select ID 
			from Approval_DocumentType 
			where EntityType = @EntityName )
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
	,null,@EntityName,@HROrgExpr
	,@PositionExpr,@Code,null
	--URL则是跳转,GUID则是弹出   URL='UFIDA.U9.Cust.XR.LackSendModifyUI'   GUID='d109936a-92d0-4ac3-9bc9-7028c172af21'
	,@Uri
	,@DisplayNameExpr,null,null,0
	,null,90,null)
	
	
	--向 工作流实体资源表 中插入预置数据
	insert into Approval_DocumentType_Trl
	(ID,Name,SysMLFlag)
	(select @curID,@Code,'en-US'
	union select @curID,@Name,'zh-CN')

	
	
end



