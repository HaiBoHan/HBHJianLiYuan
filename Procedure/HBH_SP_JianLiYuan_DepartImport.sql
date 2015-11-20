


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
as
	SET NOCOUNT ON;

	
declare @SysMlFlag varchar(11) = 'zh-CN'


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
	declare @CurDate datetime = GetDate()
	declare @SheetName varchar(125)
	declare @StartID bigint = -1
	declare @TotalIDCount int = 0
	declare @LineCount int = 0

	declare @Now datetime = GetDate();
	
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
		and checkIn.CheckInDate between Ass.AssgnBeginDate and Ass.AssgnEndDate

where -- person.PersonID = '370211198801212020'
	-- and arch.Dept = 

	checkIn.ID = @ID
	and checkin.Department > 0
	--and (arch.EntranceDate is null or arch.EntranceDate <= @CurDate)
	--and (arch.EntranceEndDate is null or arch.EntranceEndDate >= @CurDate)
	
	and checkIn.CheckInDate between Ass.AssgnBeginDate and Ass.AssgnEndDate

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

		,DayCheckIn
		,DocLineNo
		,StaffMember
		,EmployeeArchive
		,CheckType
	)select 
		(@StartID + row_number() over (order by Person) - 1)
		,1
		,head.CreatedBy
		,head.ModifiedBy
		,@Now
		,@Now

		,line.DayCheckIn
		,(row_number() over (order by Person) * 10)  as DocLineNo
		-- 人员基本信息
		,line.Person as StaffMember
		-- 员工工作记录
		,line.EmployeeArchive as EmployeeArchive
		-- 默认考勤类别 = 空 ，让用户手工必须录入
		,-1 as CheckType
	from #hbh_tmp_DayCheckInLine line
		inner join Cust_DayCheckIn head
		on head.ID = @ID

end



