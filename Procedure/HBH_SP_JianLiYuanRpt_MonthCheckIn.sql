
if exists(select * from sys.objects where name='HBH_SP_JianLiYuanRpt_MonthCheckIn')
-- 如果存在则删掉
	drop proc HBH_SP_JianLiYuanRpt_MonthCheckIn
go
-- 创建存储过程
create proc HBH_SP_JianLiYuanRpt_MonthCheckIn  (
-- @StartDate datetime = null
-- ,@EndDate datetime = null

@SalaryPeriod bigint = -1
,@StartDate datetime = null
,@EndDate datetime = null

,@Department bigint = -1

,@IsDetail varchar(125) = '1'

-- @PlanDate datetime = null
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

if exists(select name from sys.objects where name = 'HBH_Debug_Param')
begin
	declare @Debugger bit = (select top 1 Debugger from HBH_Debug_Param where ProcName = 'HBH_SP_JianLiYuanRpt_MonthCheckIn' or ProcName is null or ProcName = '' order by ProcName desc)
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
		select 'HBH_SP_JianLiYuan_DepartImport','@SalaryPeriod',IsNull(cast(@SalaryPeriod as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuanRpt_MonthCheckIn','@StartDate',IsNull(Convert(varchar,@StartDate,120),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuanRpt_MonthCheckIn','@EndDate',IsNull(Convert(varchar,@EndDate,120),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_DepartImport','@Department',IsNull(cast(@Department as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_DepartImport','@IsDetail',IsNull(cast(@IsDetail as varchar(max)),'null'),GETDATE()
		--union select 'HBH_SP_JianLiYuanRpt_MonthCheckIn','@IsCalcAll',IsNull(cast(@IsCalcAll as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuanRpt_MonthCheckIn','ProcSql','exec HBH_SP_JianLiYuanRpt_MonthCheckIn '
				+ IsNull('''' + cast(@SalaryPeriod as varchar(501)) + '''' ,'null')
				+ ',' + IsNull('''' + Convert(varchar,@StartDate,120) + '''' ,'null')
				+ ',' + IsNull('''' + Convert(varchar,@EndDate,120) + '''' ,'null')
				+ ',' + IsNull('''' + cast(@Department as varchar(501)) + '''' ,'null')
				--+ IsNull(cast(@IsCalcAll as varchar(501)),'null') 
				+ ',' + IsNull('''' + cast(@IsDetail as varchar(501)) + '''' ,'null')
			   ,GETDATE()
	end
end



	

	declare @SysMlFlag varchar(11) = 'zh-CN'
	declare @DefaultZero decimal(24,9) = 0
	--declare @SalePriceListCode varchar(125) = '001'
	declare @SysLineNo int = 10
	declare @Now datetime = GetDate();
	declare @CurDate datetime = GetDate()
	declare @SheetName varchar(125)
	declare @StartID bigint = -1
	declare @TotalIDCount int = 0
	declare @TotalLineCount int = 0
	declare @DetailLineCount int = 0


if(@IsDetail is null)
begin
	set @IsDetail = '1'
end

	
	--select @SysLineNo=cast(isnull(b.Value,a.DefaultValue) as int)
	--from Base_Profile a
	--left join Base_ProfileValue b on b.Profile=a.ID 
	--where Code='SysLineNo'

	--select @CurDate = CheckInDate
	--from [Cust_DayCheckIn]
	--where ID = @PayrollDoc
	
If OBJECT_ID('tempdb..#hbh_tmp_rpt_DayCheckInLine') is not null
	Drop Table #hbh_tmp_rpt_DayCheckInLine


select 
	Department
	,DepartmentCode
	,DepartmentName
	-- FullTimeStaff	全日制出勤	0
	-- PartTimeStaff	非全日制出勤	1
	-- HourlyStaff		钟点工出勤	2
	,CheckType
	,EmployeeArchive
	,EmployeeCode
	,EmployeeName

	,CheckInDate
	,CheckInMonth
	,CheckInDay

	-- 本月天数	
	,MonthDays = (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end)
	
	--,sum(checkInLine.FullTimeDay) as FullTimeDay
	--,sum(checkInLine.PartTimeDay) as PartTimeDay
	--,sum(checkInLine.HourlyDay) as HourlyDay
	--,min(checkIn.CheckInDate) as CheckInDate

	,FullTimeDay
	,PartTimeDay
	,HourlyDay

	
	-- 全日制标准工资=基本工资（01）+周末加班工资（02）+电话补贴（03）+交通补贴(04)+午餐补贴（05）+职务补贴（07）
	-- 2017-01-10 wf  现场让修改成：(基本工资（01） + 周末加班工资（02）)，改成了 标准工资.(14)
	,StardardSalary		
	-- F钟点工工资标准（F01） = 钟点工工资标准(F01)			-- (F13)
	,FPartSalary
	-- 加班工资
	-- 钟点工工资标准 = 钟点工工资标准(06)			-- (038)
	,OvertimeSalary
	-- F加班工资
	-- FJ钟点工工资标准（F06）=薪资项目中F工资标准项目(F06)					-- （F54）
	,FOvertimeSalary

	
		 --日工资=标准工资/应出勤天数 *全日制员工出勤
   --               +钟点工工资标准        *钟点工出勤
   --               +FJ钟点工工资标准     *钟点工出勤
   --               +F钟点工工资标准      *非全日制员工出勤
	,Salary = 
		-- 全日制员工工资
		(StardardSalary / (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end) * IsNull(FullTimeDay,@DefaultZero) 
		-- 非全日制员工工资
		+ IsNull(FPartSalary,@DefaultZero) * IsNull(PartTimeDay,@DefaultZero) 
		-- 全日制加班工资
		+ IsNull(OvertimeSalary,@DefaultZero) * IsNull(HourlyDay,@DefaultZero) 
		-- 非全日制加班工资
		+ IsNull(FOvertimeSalary,@DefaultZero) * IsNull(HourlyDay,@DefaultZero))

	-- 日保险
	,DayInsurance = -- Sum
			(
			-- 全日制员工保险
			(IsNull(InsuranceSalary,@DefaultZero) / (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end) * IsNull(FullTimeDay,@DefaultZero) 
			-- 非全日制员工保险
			+ (IsNull(FInsuranceSalary,@DefaultZero) / (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end)) * IsNull(PartTimeDay,@DefaultZero) )
			)

into #hbh_tmp_rpt_DayCheckInLine
from (
	select 
		checkIn.Department as Department
		,dept.Code as DepartmentCode
		,deptTrl.Name as DepartmentName
		-- FullTimeStaff	全日制出勤	0
		-- PartTimeStaff	非全日制出勤	1
		-- HourlyStaff		钟点工出勤	2
		,case when checkInLine.CheckType = 0 
			then '全日制出勤'
			when checkInLine.CheckType = 1
			then '非全日制出勤'
			when checkInLine.CheckType = 2
			then '钟点工出勤'
			else cast(checkInLine.CheckType as varchar(125))
			end
		 as CheckType
		,checkInLine.EmployeeArchive as EmployeeArchive
		,employee.EmployeeCode as EmployeeCode
		,employee.Name as EmployeeName

		,Convert(varchar(10),checkIn.CheckInDate,120)  as CheckInDate
		,Month(checkIn.CheckInDate) as CheckInMonth
		,Day(checkIn.CheckInDate) as CheckInDay

	
		--,sum(checkInLine.FullTimeDay) as FullTimeDay
		--,sum(checkInLine.PartTimeDay) as PartTimeDay
		--,sum(checkInLine.HourlyDay) as HourlyDay
		--,min(checkIn.CheckInDate) as CheckInDate

		,IsNull(checkInLine.FullTimeDay,0) as FullTimeDay
		,IsNull(checkInLine.PartTimeDay,0) as PartTimeDay
		,IsNull(checkInLine.HourlyDay,0) as HourlyDay

		---- 本月天数	
		-- 应出勤天数 = 当月天数 - 4
		-- 改为在日考勤中录入
		,MonthDays = IsNull(Day(DateAdd(Day,-1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)) + 1,DateAdd(M,1,checkin.CheckInDate)))),27)  - 4
		-- 改为在日考勤中录入
		,IsNull((select max(IsNull(checkin2.MonthWorkDays,0)) 
					from Cust_DayCheckIn checkin2
					where checkin2.Department = checkIn.Department)
				,0) as MonthWorkDays
		
		-- 全日制标准工资=基本工资（01）+周末加班工资（02）+电话补贴（03）+交通补贴(04)+午餐补贴（05）+职务补贴（07）
		-- 2017-01-10 wf  现场让修改成：(基本工资（01） + 周末加班工资（02）)，改成了 标准工资.(14)
		,StardardSalary = Sum(dbo.HBH_Fn_GetDecimal(
				--case when salaryItem.Code in ('01','02','03','04','05','07') 
				case when salaryItem.Code in ('14','03','04','05','07')
					then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
				else @DefaultZero end
					,@DefaultZero))
		
		-- 加班工资
		-- 钟点工工资标准 = 钟点工工资标准(06)			-- (038)
		,OvertimeSalary = Sum(dbo.HBH_Fn_GetDecimal(
				case when salaryItem.Code in ('06') 
					then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
				else @DefaultZero end
					,@DefaultZero))
		
		-- F钟点工工资标准（F01） = 钟点工工资标准(F01)			-- (F13)
		,FPartSalary = Sum(dbo.HBH_Fn_GetDecimal(
				case when salaryItem.Code in ('F01') 
					then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
				else @DefaultZero end
					,@DefaultZero))
		-- F加班工资
		-- FJ钟点工工资标准（F06）=薪资项目中F工资标准项目(F06)					-- （F54）
		,FOvertimeSalary = Sum(dbo.HBH_Fn_GetDecimal(
				case when salaryItem.Code in ('F06') 
					then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
				else @DefaultZero end
					,@DefaultZero))

		
		-- 单位保险
		-- 单位保险（12）=薪资项目中 单位保险(12)					-- （113）
		,InsuranceSalary = Sum(dbo.HBH_Fn_GetDecimal(
				case when salaryItem.Code in ('12') 
					then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
				else @DefaultZero end
					,@DefaultZero))
		-- F单位保险
		-- F单位保险（F04）=薪资项目中 F单位保险(F04)					-- （F52）
		,FInsuranceSalary = Sum(dbo.HBH_Fn_GetDecimal(
				case when salaryItem.Code in ('F04') 
					then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
				else @DefaultZero end
					,@DefaultZero))

	from Cust_DayCheckIn checkIn
		inner join Cust_DayCheckInLine checkInLine
		on checkIn.ID = checkInLine.DayCheckIn

		left join CBO_EmployeeArchive employee
		on checkInLine.EmployeeArchive = employee.ID

		left join PAY_PlanPeriod period
		on period.ID = @SalaryPeriod

		left join CBO_Department dept
		on checkIn.Department = dept.ID
		left join CBO_Department_Trl deptTrl
		on deptTrl.ID = dept.ID and deptTrl.SysMLFlag = @SysMlFlag

	
		--left join CBO_Person person
		--on checkinLine.StaffMember = person.ID
		left join CBO_EmployeeSalaryFile salary
		on salary.Employee = employee.ID
		left join CBO_PublicSalaryItem salaryItem
		on salary.SalaryItem = salaryItem.ID
		left join CBO_PublicSalaryItem_Trl salaryItemTrl
		on salaryItemTrl.ID = salaryItem.ID 
			and salaryItemTrl.SysMLFlag = 'zh-CN'

		--left join (select max(IsNull(checkin2.MonthWorkDays,0)) MonthWorkDays
		--				,checkin2.Department as Department
		--			from Cust_DayCheckIn checkin2
		--			where checkin2.Department = checkIn.Department
		--			) allCheckIn
		--on allCheckIn.Department = checkIn.Department

	where 1=1
		and ( @SalaryPeriod is null or @SalaryPeriod <= 0
			or checkIn.CheckInDate between period.StartDate and period.EndDate
			)
			--or checkIn.CheckInDate between @StartDate and @EndDate
		and (@StartDate is null or @StartDate < '2000-01-01'
			or checkIn.CheckInDate >= @StartDate
			)
		and (@EndDate is null or @EndDate < '2000-01-01'
			or checkIn.CheckInDate <= @EndDate
			)
		and (@Department is null or @Department <= 0
			or @Department = checkIn.Department
			)

	group by
		checkIn.Department
		,dept.Code
		,deptTrl.Name
		,checkIn.CheckInDate
		,checkInLine.EmployeeArchive
		,checkInLine.CheckType
		,employee.EmployeeCode
		,employee.Name
		,checkIn.CheckInDate
		,IsNull(checkInLine.FullTimeDay,0)
		,IsNull(checkInLine.PartTimeDay,0)
		,IsNull(checkInLine.HourlyDay,0)
	) checkData
	
	
;


--with checkSummary 
--as(		
--	select 
--		Department
--		,CheckType
--		,EmployeeArchive
--		,sum(FullTimeDay) as SumFullTimeDay
--		,sum(PartTimeDay) as SumPartTimeDay
--		,sum(HourlyDay) as SumHourlyDay
--	from #hbh_tmp_rpt_DayCheckInLine
--	group by
--		Department
--		,CheckType
--		,EmployeeArchive
--	)
--select
--	checkLine.*
--	,summary.SumFullTimeDay
--	,summary.SumPartTimeDay
--	,summary.SumHourlyDay
--from #hbh_tmp_rpt_DayCheckInLine checkLine
--	left join checkSummary summary
--	on checkLine.Department = summary.Department
--		and checkLine.CheckType = summary.CheckType
--		and checkLine.EmployeeArchive = summary.EmployeeArchive



if(@IsDetail = '1')
begin

	with checkSummary 
	as(		
		select 
			Department
			,CheckType
			,EmployeeArchive
			,sum(FullTimeDay) as SumFullTimeDay
			,sum(PartTimeDay) as SumPartTimeDay
			,sum(HourlyDay) as SumHourlyDay
			-- 员工日工资
			 --日工资=标准工资/应出勤天数 *全日制员工出勤
	   --               +钟点工工资标准        *钟点工出勤
	   --               +FJ钟点工工资标准     *钟点工出勤
	   --               +F钟点工工资标准      *非全日制员工出勤
			-- 27 = 当月天数 - 4
			,Salary = Sum(
				-- 全日制员工工资
				(StardardSalary / MonthDays * IsNull(FullTimeDay,@DefaultZero) 
				-- 非全日制员工工资
				+ IsNull(FPartSalary,@DefaultZero) * IsNull(PartTimeDay,@DefaultZero) 
				-- 全日制加班工资
				+ IsNull(OvertimeSalary,@DefaultZero) * IsNull(HourlyDay,@DefaultZero) 
				-- 非全日制加班工资
				+ IsNull(FOvertimeSalary,@DefaultZero) * IsNull(HourlyDay,@DefaultZero))
				)

		from #hbh_tmp_rpt_DayCheckInLine
		group by
			Department
			,CheckType
			,EmployeeArchive
		)
	select
		checkLine.*
		,summary.SumFullTimeDay
		,summary.SumPartTimeDay
		,summary.SumHourlyDay
	from #hbh_tmp_rpt_DayCheckInLine checkLine
		left join checkSummary summary
		on checkLine.Department = summary.Department
			and checkLine.CheckType = summary.CheckType
			and checkLine.EmployeeArchive = summary.EmployeeArchive
	
	order by
		checkLine.EmployeeCode
		,checkLine.EmployeeName
		,checkLine.CheckInDate
			
print('Detail')

end else
begin

	select 
		--Department
		--,CheckType
		--,EmployeeArchive
		--,sum(FullTimeDay) as SumFullTimeDay
		--,sum(PartTimeDay) as SumPartTimeDay
		--,sum(HourlyDay) as SumHourlyDay
		
		Department
		,DepartmentCode
		,DepartmentName
		,CheckType
		,EmployeeArchive
		,EmployeeCode
		,EmployeeName

		,null as CheckInDate
		,null as CheckInMonth
		,null as CheckInDay
		
		,0 as FullTimeDay
		,0 as PartTimeDay
		,0 as HourlyDay

		,sum(FullTimeDay) as SumFullTimeDay
		,sum(PartTimeDay) as SumPartTimeDay
		,sum(HourlyDay) as SumHourlyDay
		
		,StardardSalary
		,FPartSalary
		,OvertimeSalary
		,FOvertimeSalary
		
		-- 员工日工资
		 --日工资=标准工资/应出勤天数 *全日制员工出勤
   --               +钟点工工资标准        *钟点工出勤
   --               +FJ钟点工工资标准     *钟点工出勤
   --               +F钟点工工资标准      *非全日制员工出勤
		-- 27 = 当月天数 - 4
		,Salary = Sum(
			-- 全日制员工工资
			(StardardSalary / MonthDays * IsNull(FullTimeDay,@DefaultZero) 
			-- 非全日制员工工资
			+ IsNull(FPartSalary,@DefaultZero) * IsNull(PartTimeDay,@DefaultZero) 
			-- 全日制加班工资
			+ IsNull(OvertimeSalary,@DefaultZero) * IsNull(HourlyDay,@DefaultZero) 
			-- 非全日制加班工资
			+ IsNull(FOvertimeSalary,@DefaultZero) * IsNull(HourlyDay,@DefaultZero))
			)

		-- 应出勤天数 = 当月天数 - 4
		,MonthDays
		-- 日保险
		,DayInsurance
	from #hbh_tmp_rpt_DayCheckInLine
	group by
		Department
		,DepartmentCode
		,DepartmentName
		,CheckType
		,EmployeeArchive
		,EmployeeCode
		,EmployeeName
		,StardardSalary
		,PartSalary
		,OvertimeSalary

		-- ,CheckInDate
		-- ,CheckInMonth
		-- ,CheckInDay

		-- 应出勤天数 = 当月天数 - 4
		,MonthDays
		-- 日保险
		,DayInsurance
	order by
		EmployeeCode
		,EmployeeName
		,CheckInDate
		
print('Summary')

end




