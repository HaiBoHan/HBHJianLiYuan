
if exists(select * from sys.objects where name='HBH_SP_JianLiYuan_GetAllCheckIn')
-- 如果存在则删掉
	drop proc HBH_SP_JianLiYuan_GetAllCheckIn
go
-- 创建存储过程
create proc HBH_SP_JianLiYuan_GetAllCheckIn  (
-- @StartDate datetime = null
-- ,@EndDate datetime = null

@PayrollDoc bigint = -1

,@PayrollCalculate bigint = -1
--,@PayrollResults varchar(max) = ''

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

	
declare @SysMlFlag varchar(11) = 'zh-CN'


if exists(select name from sys.objects where name = 'HBH_Debug_Param')
begin
	declare @Debugger bit = (select top 1 Debugger from HBH_Debug_Param where ProcName = 'HBH_SP_JianLiYuan_GetAllCheckIn' or ProcName is null or ProcName = '' order by ProcName desc)
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
		select 'HBH_SP_JianLiYuan_GetAllCheckIn','@PayrollDoc',IsNull(cast(@PayrollDoc as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_GetAllCheckIn','@PayrollCalculate',IsNull(cast(@PayrollCalculate as varchar(max)),'null'),GETDATE()
		-- select 'HBH_SP_JianLiYuan_GetAllCheckIn','@StartDate',IsNull(Convert(varchar,@StartDate,120),'null'),GETDATE()
		-- union select 'HBH_SP_JianLiYuan_GetAllCheckIn','@EndDate',IsNull(Convert(varchar,@EndDate,120),'null'),GETDATE()
		--union select 'HBH_SP_JianLiYuan_GetAllCheckIn','@IsCalcAll',IsNull(cast(@IsCalcAll as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_GetAllCheckIn','ProcSql','exec HBH_SP_JianLiYuan_GetAllCheckIn '
				+ IsNull('''' + cast(@PayrollDoc as varchar(501)) + '''' ,'null')
				+ ',' + IsNull('''' + cast(@PayrollCalculate as varchar(501)) + '''' ,'null')
				-- + IsNull('''' + Convert(varchar,@StartDate,120) + '''' ,'null')
				-- + ',' + IsNull('''' + Convert(varchar,@EndDate,120) + '''' ,'null')
				--+ IsNull(cast(@IsCalcAll as varchar(501)),'null') 
			   ,GETDATE()
	end
end



	declare @SalePriceListCode varchar(125) = '001'
	declare @SysLineNo int = 10
	declare @Now datetime = GetDate();
	declare @CurDate datetime = Convert(date,@Now)
	declare @SheetName varchar(125)
	declare @StartID bigint = -1
	declare @TotalIDCount int = 0
	declare @TotalLineCount int = 0
	declare @DetailLineCount int = 0
	declare @DefaultZero decimal(24,9) = 0
	
	--select @SysLineNo=cast(isnull(b.Value,a.DefaultValue) as int)
	--from Base_Profile a
	--left join Base_ProfileValue b on b.Profile=a.ID 
	--where Code='SysLineNo'

	--select @CurDate = CheckInDate
	--from [Cust_DayCheckIn]
	--where ID = @PayrollDoc


/*

select 
	checkIn.Department as Department
	-- ,checkIn.CheckInDate as CheckInDate
	,checkInLine.EmployeeArchive as EmployeeArchive
	,checkInLine.CheckType as CheckType
	
	,sum(checkInLine.FullTimeDay) as FullTimeDay
	,sum(checkInLine.PartTimeDay) as PartTimeDay
	,sum(checkInLine.HourlyDay) as HourlyDay

	,min(checkIn.CheckInDate) as CheckInDate

from Pay_PayrollCalculate payCalc

	left join PAY_PayrollDoc payHead
	on payCalc.ID = payHead.PayrollCaculate
	left join PAY_EmpPayroll as payLine	--发薪明细
	on payHead.ID = payLine.PayrollDoc

	left join PAY_PayrollResult payResult
	on payCalc.ID = payResult.PayrollCaculate
	
	left join PAY_PlanPeriod monthPeriod
	on payCalc.PlanPeriodByMonth = monthPeriod.ID
	left join PAY_PlanPeriod fourWeekPeriod
	on payCalc.PlanPeriodByFourWeek = fourWeekPeriod.ID
	left join PAY_PlanPeriod twoWeekPeriod
	on payCalc.PlanPeriodByTwoWeek = twoWeekPeriod.ID
	left join PAY_PlanPeriod weekPeriod
	on payCalc.PlanPeriodByWeek = weekPeriod.ID
	left join PAY_PlanPeriod dayPeriod
	on payCalc.PlanPeriodByDay = dayPeriod.ID

	inner join Cust_DayCheckIn checkIn
	/* Approved	已审核	2
	Approving	审核中	1
	Closed	已关闭	3
	Opened	开立	0
	*/
	on checkIn.Status in (2)
		and (
			(monthPeriod.ID is not null and checkIn.CheckInDate between monthPeriod.StartDate and monthPeriod.EndDate)
			or (fourWeekPeriod.ID is not null and checkIn.CheckInDate between fourWeekPeriod.StartDate and fourWeekPeriod.EndDate)
			or (twoWeekPeriod.ID is not null and checkIn.CheckInDate between twoWeekPeriod.StartDate and twoWeekPeriod.EndDate)
			or (weekPeriod.ID is not null and checkIn.CheckInDate between weekPeriod.StartDate and weekPeriod.EndDate)
			or (dayPeriod.ID is not null and checkIn.CheckInDate between dayPeriod.StartDate and dayPeriod.EndDate)
			)
		
	inner join Cust_DayCheckInLine checkInLine
	on checkIn.ID = checkInLine.DayCheckIn
		and ( checkInLine.EmployeeArchive = payLine.Employee
			 or checkInLine.EmployeeArchive = payResult.Employee
			 )

where
	payCalc.ID = @PayrollCalculate
	or payHead.ID = @PayrollDoc
	--or payCalc.ID in (select payHead.PayrollCaculate from PAY_PayrollDoc payHead
	--					where payHead.ID = @PayrollDoc
	--					)
group by
	checkIn.Department
	-- ,checkIn.CheckInDate
	,checkInLine.EmployeeArchive
	,checkInLine.CheckType
	
order by
	min(checkIn.CheckInDate) asc

*/



select 
	checkIn.Department as Department
	-- ,checkIn.CheckInDate as CheckInDate
	,checkInLine.EmployeeArchive as EmployeeArchive
	,checkInLine.CheckType as CheckType
	
	,sum(IsNull(checkInLine.FullTimeDay,@DefaultZero)) as FullTimeDay
	,sum(IsNull(checkInLine.PartTimeDay,@DefaultZero)) as PartTimeDay
	,sum(IsNull(checkInLine.HourlyDay,@DefaultZero)) as HourlyDay


	--,[Days] = sum(checkInLine.FullTimeDay + checkInLine.PartTimeDay)
	,[Days] = sum(case checkInLine.CheckType 
					when 0 then checkInLine.FullTimeDay
					when 1 then checkInLine.PartTimeDay
					else @DefaultZero end
					)

	,min(Convert(varchar(10),checkIn.CheckInDate,120)) as CheckInDate
	
	,arch.Name
from Pay_PayrollCalculate payCalc

	--left join PAY_PayrollDoc payHead
	--on payCalc.ID = payHead.PayrollCaculate
	--left join PAY_EmpPayroll as payLine	--发薪明细
	--on payHead.ID = payLine.PayrollDoc

	left join PAY_PayrollResult payResult
	on payCalc.ID = payResult.PayrollCaculate
	
	left join PAY_PlanPeriod monthPeriod
	on payCalc.PlanPeriodByMonth = monthPeriod.ID
	left join PAY_PlanPeriod fourWeekPeriod
	on payCalc.PlanPeriodByFourWeek = fourWeekPeriod.ID
	left join PAY_PlanPeriod twoWeekPeriod
	on payCalc.PlanPeriodByTwoWeek = twoWeekPeriod.ID
	left join PAY_PlanPeriod weekPeriod
	on payCalc.PlanPeriodByWeek = weekPeriod.ID
	left join PAY_PlanPeriod dayPeriod
	on payCalc.PlanPeriodByDay = dayPeriod.ID

	inner join Cust_DayCheckIn checkIn
	/* Approved	已审核	2
	Approving	审核中	1
	Closed	已关闭	3
	Opened	开立	0
	*/
	on checkIn.Status in (2)
		and (
			(monthPeriod.ID is not null and Convert(varchar(10),checkIn.CheckInDate,120) between monthPeriod.StartDate and monthPeriod.EndDate)
			or (fourWeekPeriod.ID is not null and Convert(varchar(10),checkIn.CheckInDate,120) between fourWeekPeriod.StartDate and fourWeekPeriod.EndDate)
			or (twoWeekPeriod.ID is not null and Convert(varchar(10),checkIn.CheckInDate,120) between twoWeekPeriod.StartDate and twoWeekPeriod.EndDate)
			or (weekPeriod.ID is not null and Convert(varchar(10),checkIn.CheckInDate,120) between weekPeriod.StartDate and weekPeriod.EndDate)
			or (dayPeriod.ID is not null and Convert(varchar(10),checkIn.CheckInDate,120) between dayPeriod.StartDate and dayPeriod.EndDate)
			)
		
	inner join Cust_DayCheckInLine checkInLine
	on checkIn.ID = checkInLine.DayCheckIn
		and ( checkInLine.EmployeeArchive = payResult.Employee
			-- or checkInLine.EmployeeArchive = payLine.Employee
			 )

	left join CBO_EmployeeArchive arch
	on checkInLine.EmployeeArchive = arch.ID
where
	payCalc.ID = @PayrollCalculate
	-- or payHead.ID = @PayrollDoc
	--or payCalc.ID in (select payHead.PayrollCaculate from PAY_PayrollDoc payHead
	--					where payHead.ID = @PayrollDoc
	--					)
group by
	checkIn.Department
	-- ,checkIn.CheckInDate
	,checkInLine.EmployeeArchive
	,checkInLine.CheckType
	,arch.Name

order by
	min(Convert(varchar(10),checkIn.CheckInDate,120)) asc

