
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

,@IsDetail varchar(125) = 'false'

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



	--declare @SalePriceListCode varchar(125) = '001'
	declare @SysLineNo int = 10
	declare @Now datetime = GetDate();
	declare @CurDate datetime = GetDate()
	declare @SheetName varchar(125)
	declare @StartID bigint = -1
	declare @TotalIDCount int = 0
	declare @TotalLineCount int = 0
	declare @DetailLineCount int = 0
	
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
	checkIn.Department as Department
	,dept.Code as DepartmentCode
	,deptTrl.Name as DepartmentName
	-- FullTimeStaff	全日制出勤	0
	-- PartTimeStaff	非全日制出勤	1
	,case when checkInLine.CheckType = 0 
		then '全日制出勤'
		when checkInLine.CheckType = 1
		then '非全日制出勤'
		else cast(checkInLine.CheckType as varchar(125))
		end
	 as CheckType
	,checkInLine.EmployeeArchive as EmployeeArchive
	,employee.EmployeeCode as EmployeeCode
	,employee.Name as EmployeeName

	,Convert(varchar(10),checkIn.CheckInDate,120)  as CheckInDate
	,Month(checkIn.CheckInDate) as CheckInMouth
	,Day(checkIn.CheckInDate) as CheckInDay
	
	--,sum(checkInLine.FullTimeDay) as FullTimeDay
	--,sum(checkInLine.PartTimeDay) as PartTimeDay
	--,sum(checkInLine.HourlyDay) as HourlyDay
	--,min(checkIn.CheckInDate) as CheckInDate

	,IsNull(checkInLine.FullTimeDay,0) as FullTimeDay
	,IsNull(checkInLine.PartTimeDay,0) as PartTimeDay
	,IsNull(checkInLine.HourlyDay,0) as HourlyDay

into #hbh_tmp_rpt_DayCheckInLine
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



where 1=1
	and ( @SalaryPeriod is null or @SalaryPeriod <= 0
		or checkIn.CheckInDate between period.StartDate and period.EndDate
		)
		--or checkIn.CheckInDate between @StartDate and @EndDate
	and (@StartDate is null or @StartDate < '2000-01-01'
		or checkIn.CheckInDate >= @StartDate
		)
	and (@StartDate is null or @EndDate < '2000-01-01'
		or checkIn.CheckInDate <= @EndDate
		)
	and (@Department is null or @Department <= 0
		or @Department = checkIn.Department
		)

--group by
--	checkIn.Department
--	,checkIn.CheckInDate
--	,checkInLine.EmployeeArchive
--	,checkInLine.CheckType
	
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
		,null as CheckInMouth
		,null as CheckInDay
		
		,0 as FullTimeDay
		,0 as PartTimeDay
		,0 as HourlyDay

		,sum(FullTimeDay) as SumFullTimeDay
		,sum(PartTimeDay) as SumPartTimeDay
		,sum(HourlyDay) as SumHourlyDay

	from #hbh_tmp_rpt_DayCheckInLine
	group by
		Department
		,DepartmentCode
		,DepartmentName
		,CheckType
		,EmployeeArchive
		,EmployeeCode
		,EmployeeName

		-- ,CheckInDate
		-- ,CheckInMouth
		-- ,CheckInDay
	order by
		EmployeeCode
		,EmployeeName
		,CheckInDate
		
print('Summary')

end




