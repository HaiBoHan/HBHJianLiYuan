
/*
select *
from HBH_SPParamRecord
order by CreatedOn desc
*/

if exists(select * from sys.objects where name='HBH_BASP_JianLiYuan_HolidayAttendance')
-- 如果存在则删掉
	drop proc HBH_BASP_JianLiYuan_HolidayAttendance
go
-- 创建存储过程
create proc HBH_BASP_JianLiYuan_HolidayAttendance  (
-- @StartDate datetime = null
-- ,@EndDate datetime = null

@请选择过滤年月 varchar(125) = ''
,@请选择大区 varchar(125) = ''
,@请选择区域 varchar(125) = ''
,@请选择部门 varchar(125) = ''
)
with encryption
as
	SET NOCOUNT ON;
	

if exists(select name from sys.objects where name = 'HBH_Debug_Param')
begin
	declare @Debugger bit = (select top 1 Debugger from HBH_Debug_Param where ProcName = 'HBH_BASP_JianLiYuan_HolidayAttendance' or ProcName is null or ProcName = '' order by ProcName desc)
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
		select 'HBH_BASP_JianLiYuan_HolidayAttendance','@请选择过滤年月',IsNull(@请选择过滤年月,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_HolidayAttendance','@请选择大区',IsNull(@请选择大区,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_HolidayAttendance','@请选择区域',IsNull(@请选择区域,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_HolidayAttendance','@请选择部门',IsNull(@请选择部门,'null'),GETDATE()

		union select 'HBH_BASP_JianLiYuan_HolidayAttendance','ProcSql','exec HBH_BASP_JianLiYuan_HolidayAttendance '
				+ IsNull('''' + @请选择过滤年月 + '''' ,'null')
				+ ',' + IsNull('''' + @请选择大区 + '''' ,'null')
				+ ',' + IsNull('''' + @请选择区域 + '''' ,'null')
				+ ',' + IsNull('''' + @请选择部门 + '''' ,'null')

			   ,GETDATE()
	end
end

	declare @SysMlFlag varchar(11) = 'zh-CN'
	declare @DefaultZero decimal(24,9) = 0
	declare @Now datetime = GetDate();
	declare @CurDate datetime = GetDate()
	declare @Today datetime = convert(varchar(10), GetDate(), 120)


-- 部门二表,删除数据，重新抽取
/*
truncate table Dim_U9_Department2

insert into Dim_U9_Department2
select dept.ID,dept.Code,deptTrl.Name,dept.Level+1 as Level,dept.Org
from [10.28.76.125].U9.dbo.CBO_Department dept 
	inner join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl 
	on dept.ID= deptTrl.ID and deptTrl.SysMLFlag='zh-CN'
where
	dept.Code != '00001'
	and Len(dept.Code) = 5
*/



-- HRTest
-- 劳产率 = 收入 / 出勤日次
-- If((IsNull([查询1].[日出勤人次],0) = 0),0,[查询1].[收入] / [查询1].[日出勤人次])
-- 人工成本比例 = 工资 / 收入
-- If((IsNull([查询1].[收入],0) = 0),0,[查询1].[工资] / [查询1].[收入])

-- 抽取脚本

-- 事实表，Fact_U9_HolidayAttendance
-- 删除
truncate table Fact_U9_HolidayAttendance
-- 新增
insert into Fact_U9_HolidayAttendance
--(
--	Region bigint
--	,RegionCode varchar(200)
--	,RegionName varchar(200)
--	,Region2 bigint
--	,Region2Code varchar(200)
--	,Region2Name varchar(200)

--	,Department bigint
--	,DepartmentCode varchar(200)
--	,DepartmentName varchar(200)
	
--	,CheckInDate DateTime
--	,DisplayDate varchar(200) 
--	,StatisticsPeriod varchar(125)


--	-- 预警.假期日收入
--	,ForecastHolidayDayIncome decimal(24,9)
--	-- 预警.假期出勤人数
--	,ForecastAttendance decimal(24,9)
--	-- 预警.日人均效率
--	,ForecastEfficiency decimal(24,9)

--	-- 实际.假期日收入
--	,FactHolidayDayIncome decimal(24,9)
--	-- 实际.假期出勤人数
--	-- 日出勤人次 = 非全日制员工出勤合计/4 + 钟点工员工出勤合计/8 + 全日制员工出勤合计 
--	,FactAttendance decimal(24,9)
--	,FullTimeDay decimal(24,9)
--	,PartTimeDay decimal(24,9)
--	,HourlyDay decimal(24,9)
--	-- 实际.日人均效率
--	,FactEfficiency decimal(24,9)
	
--)

select 
	-- 大区
	Region = IsNull(region.ID,-1)
	,RegionCode = IsNull(region.Code,'')
	,RegionName = IsNull(regionTrl.Name,'')
	-- 区域
	,Region2 = IsNull(region2.ID,-1)
	,Region2Code = IsNull(region2.Code,'')
	,Region2Name = IsNull(region2Trl.Name,'')
	-- 部门
	,Department = IsNull(dept.ID,-1)
	,DepartmentCode = IsNull(dept.Code,'')
	,DepartmentName = IsNull(deptTrl.Name,'')

	,checkinSummary.CheckInDate
	,checkinSummary.DisplayDate
	,checkinSummary.StatisticsPeriod
	--,checkin.Status as Status
	--,checkin.CurrentOperator as CurrentOperator

	--,checkinLine.StaffMember as Staff
	--,person.PersonID as StaffCode
	--,person.Name as StaffName
		
	-- 预警
	-- 收入 = 餐标 * 就餐人数
	,ForecastHolidayDayIncome = Warning.Income
	-- 日出勤小时数
	,ForecastAttendance = Warning.AttendanceTime
	-- 当日人工工资
	,ForecastWage = Warning.Wage
	-- 预警.日人均效率 = 收入 / 出勤人数
	,ForecastEfficiency = case when Warning.AttendanceTime is not null and Warning.AttendanceTime != 0
							then IsNull(Warning.Income,0) / Warning.AttendanceTime
							else 0 end
	
	-- 收入
	,FactHolidayDayIncome = (checkinSummary.Income)
	-- 日出勤人数 = 非全日制员工出勤合计/4 + 钟点工员工出勤合计/8 + 全日制员工出勤合计 
	,FactAttendance = checkinSummary.PersonTime
	-- 实际.日人均效率 =  收入 / 出勤人数
	,FactEfficiency = case when checkinSummary.PersonTime is not null and checkinSummary.PersonTime != 0
							then IsNull(checkinSummary.Income,0) / checkinSummary.PersonTime
							else 0 end

	-- 全日制
	,FullTimeDay = (IsNull(checkinSummary.FullTimeDay,@DefaultZero))
	-- 非全日制
	,PartTimeDay = (IsNull(checkinSummary.PartTimeDay,@DefaultZero))
	-- 钟点工
	,HourlyDay = (IsNull(checkinSummary.HourlyDay,@DefaultZero))
	
from (
	select 
		---- 部门
		--Department = IsNull(dept.ID,-1)
		--,DepartmentCode = IsNull(dept.Code,'')
		--,DepartmentName = IsNull(deptTrl.Name,'')
		Department = employee.Dept

		,checkin.CheckInDate
		,DisplayDate = Right('00' + DateName(Month,checkin.CheckInDate),2) + '.' + Right('00' + DateName(day,checkin.CheckInDate),2)
		,StatisticsPeriod = Right('0000' + DateName(year,checkin.CheckInDate),4) + '年' + Right('00' + DateName(month,checkin.CheckInDate),2) + '月'
			
		-- 全日制
		,FullTimeDay = Sum(IsNull(checkinLine.FullTimeDay,@DefaultZero))
		-- 非全日制
		,PartTimeDay = Sum(IsNull(checkinLine.PartTimeDay,@DefaultZero))
		-- 钟点工
		,HourlyDay = Sum(IsNull(checkinLine.HourlyDay,@DefaultZero))
	
		-- 收入
		,Income = max(IsNull(checkin.Income,@DefaultZero))
		---- 大区
		--,Region = IsNull(region.ID,-1)
		--,RegionCode = IsNull(region.Code,'')
		--,RegionName = IsNull(regionTrl.Name,'')
		---- 区域
		--,Region2 = IsNull(region2.ID,-1)
		--,Region2Code = IsNull(region2.Code,'')
		--,Region2Name = IsNull(region2Trl.Name,'')
		
		-- 日出勤人数 = 非全日制员工出勤合计/4 + 钟点工员工出勤合计/8 + 全日制员工出勤合计 
		,PersonTime = Sum(IsNull(checkinLine.PartTimeDay,@DefaultZero)) / 4 + Sum(IsNull(checkinLine.HourlyDay,@DefaultZero)) / 8 + Sum(IsNull(checkinLine.FullTimeDay,@DefaultZero))
	
	from 		
		[10.28.76.125].U9.dbo.CBO_EmployeeArchive employee	
		
		left join [10.28.76.125].U9.dbo.Cust_DayCheckInLine checkinLine
		on checkinLine.EmployeeArchive = employee.ID
		left join [10.28.76.125].U9.dbo.Cust_DayCheckIn checkin
		on checkin.ID = checkinLine.DayCheckIn
	where 
		employee.Dept is not null
	group by 
		--IsNull(dept.ID,-1)
		--,IsNull(dept.Code,'')
		--,IsNull(deptTrl.Name,'')
		employee.Dept

		,checkin.CheckInDate
		-- ,checkinLine.EmployeeArchive
		---- 区域
		--,IsNull(region.ID,-1)
		--,IsNull(region.Code,'')
		--,IsNull(regionTrl.Name,'')
		--,IsNull(region2.ID,-1)
		--,IsNull(region2.Code,'')
		--,IsNull(region2Trl.Name,'')
	) as checkinSummary

	full join (
			select 
				warning.Department
				,warningLine.Date
				-- 收入 = 餐标 * 就餐人数
				,Income = Sum(MealStandard * EstimatedQty)
				-- 日出勤小时数
				,AttendanceTime = Max(AttendanceTime)
				-- 当日人工工资
				,Wage = Max(Wage)
			from [10.28.76.125].U9.dbo.Cust_CostWarning warning
				inner join [10.28.76.125].U9.dbo.Cust_CostWarningLine warningLine
				on warning.ID = warningLine.CostWarning
			group by
				warning.Department
				,warningLine.Date
			) as Warning
	on checkinSummary.Department = Warning.Department
		and checkinSummary.CheckInDate = Warning.Date
		

	
	left join [10.28.76.125].U9.dbo.CBO_Department dept
	-- on checkin.Department = dept.ID
	on dept.ID = IsNull(checkinSummary.Department,Warning.Department)

	left join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl
	on deptTrl.ID = dept.ID
		and deptTrl.SysMLFlag = 'zh-CN'
	 
	left join [10.28.76.125].U9.dbo.CBO_Department region
	on SubString(dept.Code,1,5) = region.Code
	left join [10.28.76.125].U9.dbo.CBO_Department_Trl regionTrl
	on regionTrl.ID = region.ID
		and regionTrl.SysMLFlag = 'zh-CN'
	 
	left join [10.28.76.125].U9.dbo.CBO_Department region2
	on SubString(dept.Code,1,7) = region2.Code
	left join [10.28.76.125].U9.dbo.CBO_Department_Trl region2Trl
	on region2Trl.ID = region2.ID
		and region2Trl.SysMLFlag = 'zh-CN'




select *
from Fact_U9_HolidayAttendance
where (@请选择过滤年月 is null or @请选择过滤年月 = ''
		or @请选择过滤年月 = StatisticsPeriod
		)
	and (@请选择大区 is null or @请选择大区 = ''
		or @请选择大区 = RegionName
		)
	and (@请选择区域 is null or @请选择区域 = ''
		or @请选择区域 = Region2Name
		)
	and (@请选择部门 is null or @请选择部门 = ''
		or @请选择部门 = DepartmentName
		)


