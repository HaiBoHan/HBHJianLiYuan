


if exists(select * from sys.objects where name='HBH_SP_JianLiYuan_DepartImport')
-- 如果存在则删掉
	drop proc HBH_SP_JianLiYuan_DepartImport
go
-- 创建存储过程
create proc HBH_SP_JianLiYuan_DepartImport  (
@ID bigint = -1
--,@ShipLineID bigint =-1
--,@LotCode varchar(125) = ''
--,@ItemSpec varchar(125) = ''
--,@SalesmanCode varchar(125) = ''
----,@IsAllSalesman smallint = 0
--,@IsFuzzySalesman smallint = 0
--,@IsContainBranchWh smallint = 0
--,@InvCategory bigint = -1
--,@ItemCode varchar(125) = ''
--,@ItemName varchar(125) = ''
----,@IsForceSalesman smallint = 0
--,@IsShowZeroQty smallint = 0
--,@Branch bigint = -1
)
with encryption
as
	SET NOCOUNT ON;

	
declare @SysMlFlag varchar(11) = 'zh-CN'
declare @ContractTypeDefCode varchar(125) = 'ht'


if exists(select name from sys.objects where name = 'HBH_Debug_Param')
begin
	declare @Debugger bit = (select top 1 Debugger from HBH_Debug_Param where ProcName = 'HBH_SP_JianLiYuan_DepartImport' or ProcName is null or ProcName = '' order by ProcName desc)
	if(@Debugger=1)
	begin	
		if not exists(select name from sys.objects where name = 'HBH_SPParamRecord')
		begin
			create table HBH_SPParamRecord
			(ProcName varchar(501)
			,ParamName varchar(501)
			,ParamValue varchar(max)
			,CreatedOn DateTime
			,Memo varchar(max)	-- 备注
			)
		end

		insert into HBH_SPParamRecord
		(ProcName,ParamName,ParamValue,CreatedOn)
		select 'HBH_SP_JianLiYuan_DepartImport','@ID',IsNull(cast(@ID as varchar(max)),'null'),GETDATE()
		--union select 'HBH_SP_JianLiYuan_DepartImport','@IsCalcAll',IsNull(cast(@IsCalcAll as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_DepartImport','ProcSql','exec HBH_SP_JianLiYuan_DepartImport '
				+ IsNull('''' + cast(@ID as varchar(501)) + '''' ,'null')
				--+ ',' + IsNull(cast(@IsCalcAll as varchar(501)),'null') 
			   ,GETDATE()
	end
end



	declare @SalePriceListCode varchar(125) = '001'
	declare @SysLineNo int = 10
	declare @Now datetime = GetDate();
	declare @CurDate datetime = GetDate()
	declare @SheetName varchar(125)
	declare @StartID bigint = -1
	declare @TotalIDCount int = 0
	declare @LineCount int = 0

	
	select @SysLineNo=cast(isnull(b.Value,a.DefaultValue) as int)
	from Base_Profile a
	left join Base_ProfileValue b on b.Profile=a.ID 
	where Code='SysLineNo'

	select @CurDate = CheckInDate
	from [Cust_DayCheckIn]
	where ID = @ID

-- 清空所有 明细行
delete from [Cust_DayCheckInLine]
where [DayCheckIn] = @ID


-- 4、	根据人员基本信息中部门信息分别生成考勤日报表样表，即根据不同部门自动生成上图样表，职员代码、考勤类别、职员姓名、职务、部门从人员信息自动取数，非全日制员工出勤、钟点工考勤、全日制员工出勤手工录入，小计=非全日制员工出勤+钟点工出勤+全日制员工出勤
-- CBO.HR.PersonInfo
-- CBO_Person

If OBJECT_ID('tempdb..#hbh_tmp_DayCheckInLine') is not null
	Drop Table #hbh_tmp_DayCheckInLine


select 
	--person.PersonID
	--,deptTrl.Name
	--,dept.Code
	--,arch.*
	--,person.*
	checkIn.ID as DayCheckIn
	,person.ID as Person
	,arch.ID as EmployeeArchive
	-- 劳动合同类型
	-- 最新劳动合同类型取值方式：取劳动合同类型3的值，如果劳动合同类型3为空，取劳动合同类型2的值，如果劳动合同类型2的值为空，取劳动合同类型1的值。
	,case when person.DescFlexField_PrivateDescSeg19 is not null and person.DescFlexField_PrivateDescSeg19 != ''
			then person.DescFlexField_PrivateDescSeg19
		when person.DescFlexField_PrivateDescSeg16 is not null and person.DescFlexField_PrivateDescSeg16 != ''
			then person.DescFlexField_PrivateDescSeg16
		when person.DescFlexField_PrivateDescSeg13 is not null and person.DescFlexField_PrivateDescSeg13 != ''
			then person.DescFlexField_PrivateDescSeg13
		end as ContractType
into #hbh_tmp_DayCheckInLine
from CBO_Person person
	inner join CBO_EmployeeArchive arch
	on person.ID = arch.Person
	--left join CBO_Department dept
	--on dept.ID = arch.Dept
	--left join CBO_Department_Trl deptTrl
	--on dept.ID = deptTrl.ID
	inner join Cust_DayCheckIn checkIn
	on arch.Dept = checkin.Department
	
	inner join CBO_EmployeeAssignment Ass
	on arch.ID = Ass.Employee
		and checkIn.CheckInDate between IsNull(Ass.AssgnBeginDate,'2000-12-31') and IsNull(Ass.AssgnEndDate,'9999-12-31')

where -- person.PersonID = '370211198801212020'
	-- and arch.Dept = 

	checkIn.ID = @ID
	and checkin.Department > 0
	--and (arch.EntranceDate is null or arch.EntranceDate <= @CurDate)
	--and (arch.EntranceEndDate is null or arch.EntranceEndDate >= @CurDate)
	
	and checkIn.CheckInDate between IsNull(Ass.AssgnBeginDate,'2000-12-31') and IsNull(Ass.AssgnEndDate,'9999-12-31')

--order by
--	person.PersonID


select @LineCount = count(*)  from #hbh_tmp_DayCheckInLine

	-- 计算ID
	set @TotalIDCount = @LineCount
	

if(@TotalIDCount > 0)
begin

	-- select @StartID=0,@Count=@Count+1
	execute AllocSerials @TotalIDCount,@StartID output		

	insert into Cust_DayCheckInLine
	(
		ID
		,SysVersion
		,CreatedBy
		,ModifiedBy
		,CreatedOn
		,ModifiedOn
		--,Status

		,DayCheckIn
		,DocLineNo
		,StaffMember
		,EmployeeArchive
		,CheckType

		,FullTimeDay
		,PartTimeDay
		,HourlyDay
	)select 
		(@StartID + row_number() over (order by Person) - 1)
		,1
		,head.CreatedBy
		,head.ModifiedBy
		,@Now
		,@Now
		---- Approved	已审核	2	Approving	审核中	1	Closed	已关闭	3	Opened	开立	0
		--,0

		,line.DayCheckIn
		,(row_number() over (order by Person) * 10)  as DocLineNo
		-- 人员基本信息
		,line.Person as StaffMember
		-- 员工工作记录
		,line.EmployeeArchive as EmployeeArchive
		-- 默认考勤类别 = 空 ，让用户手工必须录入
		-- 全日制出勤 = 0  ，   非全日制出勤 = 1
		--,-1 as CheckType
		-- 如果劳动合同名称含有“非全日制”，对应考勤类型为：非全日制出勤，其他劳动合同类型统一对应全日制出勤。
		,case when defValueTrl.Name is not null and defValueTrl.Name like '%非全日制%'
			then 1
			else 0
			end

		,0
		,0
		,0
	from #hbh_tmp_DayCheckInLine line
		inner join Cust_DayCheckIn head
		on head.ID = @ID

		left join Base_ValueSetDef def
		on def.Code in (@ContractTypeDefCode)
		left join Base_DefineValue defValue
		on defValue.Code = line.ContractType
		left join Base_DefineValue_Trl defValueTrl
		on defValue.ID = defValueTrl.ID


		/*
		select defValue.Code,defValueTrl.Name,defValue.*,defValueTrl.*
from Base_DefineValue defValue
	inner join Base_DefineValue_Trl defValueTrl
	on defValue.ID = defValueTrl.ID

	left join Base_ValueSetDef def
	on defValue.ValueSetDef = def.ID
where def.Code in ('ht')
		*/




end



