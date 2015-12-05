
if exists(select * from sys.objects where name='HBH_SP_JianLiYuanRpt_MonthCheckIn')
-- 如果存在则删掉
	drop proc HBH_SP_JianLiYuanRpt_MonthCheckIn
go
-- 创建存储过程
create proc HBH_SP_JianLiYuanRpt_MonthCheckIn  (
-- @StartDate datetime = null
-- ,@EndDate datetime = null

@StartDate datetime = null
,@EndDate datetime = null


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
		-- select 'HBH_SP_JianLiYuan_DepartImport','@PayrollDoc',IsNull(cast(@PayrollDoc as varchar(max)),'null'),GETDATE()
		select 'HBH_SP_JianLiYuanRpt_MonthCheckIn','@StartDate',IsNull(Convert(varchar,@StartDate,120),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuanRpt_MonthCheckIn','@EndDate',IsNull(Convert(varchar,@EndDate,120),'null'),GETDATE()
		--union select 'HBH_SP_JianLiYuanRpt_MonthCheckIn','@IsCalcAll',IsNull(cast(@IsCalcAll as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuanRpt_MonthCheckIn','ProcSql','exec HBH_SP_JianLiYuanRpt_MonthCheckIn '
				-- + IsNull('''' + cast(@PayrollDoc as varchar(501)) + '''' ,'null')
				+ IsNull('''' + Convert(varchar,@StartDate,120) + '''' ,'null')
				+ ',' + IsNull('''' + Convert(varchar,@EndDate,120) + '''' ,'null')
				--+ IsNull(cast(@IsCalcAll as varchar(501)),'null') 
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
	declare @TotalLineCount int = 0
	declare @DetailLineCount int = 0
	
	--select @SysLineNo=cast(isnull(b.Value,a.DefaultValue) as int)
	--from Base_Profile a
	--left join Base_ProfileValue b on b.Profile=a.ID 
	--where Code='SysLineNo'

	--select @CurDate = CheckInDate
	--from [Cust_DayCheckIn]
	--where ID = @PayrollDoc


select 
	checkIn.Department as Department
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

	,checkIn.CheckInDate as CheckInDate
	,Month(checkIn.CheckInDate) as CheckInMouth
	,Day(checkIn.CheckInDate) as CheckInDay
	
	--,sum(checkInLine.FullTimeDay) as FullTimeDay
	--,sum(checkInLine.PartTimeDay) as PartTimeDay
	--,sum(checkInLine.HourlyDay) as HourlyDay
	--,min(checkIn.CheckInDate) as CheckInDate

	,checkInLine.FullTimeDay as FullTimeDay
	,checkInLine.PartTimeDay as PartTimeDay
	,checkInLine.HourlyDay as HourlyDay

from Cust_DayCheckIn checkIn
	inner join Cust_DayCheckInLine checkInLine
	on checkIn.ID = checkInLine.DayCheckIn

	left join CBO_EmployeeArchive employee
	on checkInLine.EmployeeArchive = employee.ID

where
	checkIn.CheckInDate between @StartDate and @EndDate
--group by
--	checkIn.Department
--	,checkIn.CheckInDate
--	,checkInLine.EmployeeArchive
--	,checkInLine.CheckType
	

