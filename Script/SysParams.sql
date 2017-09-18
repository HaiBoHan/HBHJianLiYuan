

/*
EnterpriseCode	企业编码
EnterpriseName	企业名称
Org	组织
OrgCode	组织编码
OrgName	组织名称
User	用户
UserCode	用户编码
UserName	用户名称

*/

/*
delete from Base_Profile
-- where ID in (2015091700100001,2015091700100002,2015091700100003,2015091700100004,2015091700100005)
where ID between 9009201709160002 and 
9009201709160002
delete from Base_Profile_Trl
-- where ID in (2015091700100001,2015091700100002,2015091700100003,2015091700100004,2015091700100005)
where ID between 9009201709160002 and 9009201709160002
*/

/*
select *
from Base_Profile
order by ID desc
*/

declare @ID bigint
declare @Application bigint
declare @Code varchar(125)
declare @Name varchar(125)
declare @Group varchar(125)
declare @Type varchar(125)
declare @DefaultValue varchar(125)
declare @ProfileValueType int
declare @Today datetime = convert(varchar(10), GetDate(), 120);


/*  -- 增加了实体字段，而不是通过参数；

set @ID = 9009201512100001
--	CBO		；  JLYCheck	301101
set @Application = 3000
set @Code = 'JLY_IsApproveFlow_DayCheckIn'
set @Name = '日考勤--默认审批流作业'
set @Group = '审批参数'
set @Type = 'bool'
set @DefaultValue = 'false'

if not exists(select 1 from Base_Profile where ID = @ID)
begin
	insert into Base_Profile
	(ID,SysVersion,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy
	,Code,ShortName,ProfileValueType,SubTypeName,DefaultValue
	,Application,ControlScope,SensitiveType,Sort
	,ValidateSV,CanBeUpdatedSV,UpdatedProcessSV,ReferenceID,Hidden,ShowPecent,IsSend,IsModify
	)values(
	@ID,1,'2015-09-17','hbh','2015-09-17','hbh'
	,@Code,@Name,0,@Type,@DefaultValue
	,@Application,1,0,0
	,null,null,null,null,0,0,0,0
	)

	insert into Base_Profile_Trl
	(ID,SysMLFlag,ProfileGroup,Name,Description
	)values(
	@ID,'zh-CN',@Group,@Name,@Name
	)
end ;



set @ID = 9009201512100002
--	CBO		
set @Application = 3000
set @Code = 'JLY_IsApproveFlow_TotalPayrollDoc'
set @Name = '发薪申请汇总--默认审批流作业'
set @Group = '审批参数'
set @Type = 'bool'
set @DefaultValue = 'false'

if not exists(select 1 from Base_Profile where ID = @ID)
begin
	insert into Base_Profile
	(ID,SysVersion,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy
	,Code,ShortName,ProfileValueType,SubTypeName,DefaultValue
	,Application,ControlScope,SensitiveType,Sort
	,ValidateSV,CanBeUpdatedSV,UpdatedProcessSV,ReferenceID,Hidden,ShowPecent,IsSend,IsModify
	)values(
	@ID,1,'2015-09-17','hbh','2015-09-17','hbh'
	,@Code,@Name,0,@Type,@DefaultValue
	,@Application,1,0,0
	,null,null,null,null,0,0,0,0
	)

	insert into Base_Profile_Trl
	(ID,SysMLFlag,ProfileGroup,Name,Description
	)values(
	@ID,'zh-CN',@Group,@Name,@Name
	)
end ;


*/



-- 系统参数： 米面油是否可无来源收货
set @ID = 9009201709160002
--	CBO		
set @Application = 3000
set @Code = 'HBHJianLiYuan_IsControlPlanPrice'
set @Name = '健力源参数--米面油是否计划价控制'
set @Group = '健力源参数'
set @Type = 'bool'
-- String:0	;	bool:3	;	
set @ProfileValueType = 3
set @DefaultValue = 'false'

if not exists(select 1 from Base_Profile where ID = @ID)
begin
	insert into Base_Profile
	(ID,SysVersion,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy
	,Code,ShortName,ProfileValueType,SubTypeName,DefaultValue
	,Application,ControlScope,SensitiveType,Sort
	,ValidateSV,CanBeUpdatedSV,UpdatedProcessSV,ReferenceID,Hidden,ShowPecent,IsSend,IsModify
	)values(
	@ID,1,@Today,'hbh',@Today,'hbh'
	,@Code,@Name,@ProfileValueType,@Type,@DefaultValue
	,@Application,1,0,0
	,null,null,null,null,0,0,0,0
	)

	insert into Base_Profile_Trl
	(ID,SysMLFlag,ProfileGroup,Name,Description
	)values(
	@ID,'zh-CN',@Group,@Name,@Name
	)
end ;
