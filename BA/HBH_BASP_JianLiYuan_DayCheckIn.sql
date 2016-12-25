
/*
select *
from HBH_SPParamRecord
order by CreatedOn desc
*/

if exists(select * from sys.objects where name='HBH_BASP_JianLiYuan_DayCheckIn')
-- 如果存在则删掉
	drop proc HBH_BASP_JianLiYuan_DayCheckIn
go
-- 创建存储过程
create proc HBH_BASP_JianLiYuan_DayCheckIn  (
-- @StartDate datetime = null
-- ,@EndDate datetime = null

@请选择过滤年月 varchar(125) = ''
,@请选择大区 varchar(125) = ''
,@请选择部门 varchar(125) = ''
)
with encryption
as
	SET NOCOUNT ON;
	

if exists(select name from sys.objects where name = 'HBH_Debug_Param')
begin
	declare @Debugger bit = (select top 1 Debugger from HBH_Debug_Param where ProcName = 'HBH_BASP_JianLiYuan_DayCheckIn' or ProcName is null or ProcName = '' order by ProcName desc)
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
		select 'HBH_BASP_JianLiYuan_DayCheckIn','@请选择过滤年月',IsNull(@请选择过滤年月,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_DayCheckIn','@请选择大区',IsNull(@请选择大区,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_DayCheckIn','@请选择部门',IsNull(@请选择部门,'null'),GETDATE()

		union select 'HBH_BASP_JianLiYuan_DayCheckIn','ProcSql','exec HBH_BASP_JianLiYuan_DayCheckIn '
				+ IsNull('''' + @请选择过滤年月 + '''' ,'null')
				+ ',' + IsNull('''' + @请选择大区 + '''' ,'null')
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
	

truncate table Dim_U9_Department3

insert into Dim_U9_Department3
select dept.ID,dept.Code,deptTrl.Name,dept.Level+1 as Level,dept.Org
from [10.28.76.125].U9.dbo.CBO_Department dept 
	inner join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl 
	on dept.ID= deptTrl.ID and deptTrl.SysMLFlag='zh-CN'
where
	Len(dept.Code) = 7
*/

-- 大区
insert into Dim_U9_Department2
select dept.ID,dept.Code,deptTrl.Name,dept.Level+1 as Level,dept.Org
from [10.28.76.125].U9.dbo.CBO_Department dept 
	inner join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl 
	on dept.ID= deptTrl.ID and deptTrl.SysMLFlag='zh-CN'
where
	dept.Code != '00001'
	and Len(dept.Code) = 5
	-- 不存在的新增
	and dept.ID not in (select region2.ID from Dim_U9_Department2 region2)

-- 区域
insert into Dim_U9_Department3
select dept.ID,dept.Code,deptTrl.Name,dept.Level+1 as Level,dept.Org
from [10.28.76.125].U9.dbo.CBO_Department dept 
	inner join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl 
	on dept.ID= deptTrl.ID and deptTrl.SysMLFlag='zh-CN'
where
	Len(dept.Code) = 7
	-- 不存在的新增
	and dept.ID not in (select region3.ID from Dim_U9_Department3 region3)




-- HRTest
-- 劳产率 = 收入 / 出勤日次
-- If((IsNull([查询1].[日出勤人次],0) = 0),0,[查询1].[收入] / [查询1].[日出勤人次])
-- 人工成本比例 = 工资 / 收入
-- If((IsNull([查询1].[收入],0) = 0),0,[查询1].[工资] / [查询1].[收入])

-- 抽取脚本

-- 事实表，Fact_U9_DayCheckIn
-- 删除
truncate table Fact_U9_DayCheckIn
-- 新增
insert into Fact_U9_DayCheckIn
--(
--	Department
--	,DepartmentCode
--	,DepartmentName
--	,CheckInDate
--	,StatisticsPeriod
--	-- 全日制
--	,FullTimeDay
--	-- 非全日制
--	,PartTimeDay
--	-- 钟点工
--	,HourlyDay
--	-- 日出勤人数 = 非全日制员工出勤合计/4 + 钟点工员工出勤合计/8 + 全日制员工出勤合计 
--	,PersonTime
--	-- 收入
--	,Income
--  -- 劳产率目标
--  ,LaborYieldTarget
--  -- 人工成本目标
--  ,LaborCostTarget
--	-- 区域
--	,Region
--	,RegionCode
--	,RegionName
--	,Salary
--)

select 
	Department
	,DepartmentCode
	,DepartmentName
	,CheckInDate
	,DisplayDate
	,StatisticsPeriod
	--,checkin.Status as Status
	--,checkin.CurrentOperator as CurrentOperator

	--,checkinLine.StaffMember as Staff
	--,person.PersonID as StaffCode
	--,person.Name as StaffName
	
	-- 全日制
	,FullTimeDay = Sum(IsNull(FullTimeDay,@DefaultZero))
	-- 非全日制
	,PartTimeDay = Sum(IsNull(PartTimeDay,@DefaultZero))
	-- 钟点工
	,HourlyDay = Sum(IsNull(HourlyDay,@DefaultZero))
	-- 日出勤人数 = 非全日制员工出勤合计/4 + 钟点工员工出勤合计/8 + 全日制员工出勤合计 
	,PersonTime = Sum(IsNull(PartTimeDay,@DefaultZero)) / 4 + Sum(IsNull(HourlyDay,@DefaultZero)) / 8 + Sum(IsNull(FullTimeDay,@DefaultZero))
	
	-- 收入
	,Income = Max(Income)
	-- 劳产率目标
	,LaborYieldTarget = Max(LaborYieldTarget)
	-- 人工成本目标
	,LaborCostTarget = Max(LaborCostTarget)
	-- 区域
	,Region = Region
	,RegionCode
	,RegionName
	---- 标准工资
	--,StardardSalary = Sum(StardardSalary)
	
	-- 员工日工资
	-- 日工资=标准工资/27 *全日制员工出勤+钟点工工资标准(038)*钟点工出勤+非全日制员工出勤*钟标准工资		;钟点工工资标准取U9维护薪资项目数值点工工资标准
	-- 27 = 当月天数 - 4
	--,Salary = Sum(StardardSalary / MonthDays * IsNull(FullTimeDay,@DefaultZero) + IsNull(PartTimeDay,@DefaultZero) * IsNull(PartSalary,@DefaultZero) + IsNull(HourlyDay,@DefaultZero) * IsNull(OvertimeSalary,@DefaultZero))
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
	,DayInsurance = Sum
			(
			-- 全日制员工保险
			(IsNull(InsuranceSalary,@DefaultZero) / MonthDays * IsNull(FullTimeDay,@DefaultZero) 
			-- 非全日制员工保险
			+ (IsNull(FInsuranceSalary,@DefaultZero) / MonthDays) * IsNull(PartTimeDay,@DefaultZero) )
			)

from (
	select 
		-- 员工
		employee.ID as EmployeeArchive
		-- 部门
		,Department = IsNull(dept.ID,-1)
		,DepartmentCode = IsNull(dept.Code,'')
		,DepartmentName = IsNull(deptTrl.Name,'')

		,checkin.CheckInDate
		,DisplayDate = Right('00' + DateName(Month,checkin.CheckInDate),2) + '.' + Right('00' + DateName(day,checkin.CheckInDate),2)
		,StatisticsPeriod = Right('0000' + DateName(year,checkin.CheckInDate),4) + '年' + Right('00' + DateName(month,checkin.CheckInDate),2) + '月'
		--,checkin.Status as Status
		--,checkin.CurrentOperator as CurrentOperator

		--,checkinLine.StaffMember as Staff
		--,person.PersonID as StaffCode
		--,person.Name as StaffName

	
		-- 全日制
		,FullTimeDay = IsNull(checkinLine.FullTimeDay,@DefaultZero)
		-- 非全日制
		,PartTimeDay = IsNull(checkinLine.PartTimeDay,@DefaultZero)
		-- 钟点工
		,HourlyDay = IsNull(checkinLine.HourlyDay,@DefaultZero)
	
		-- 收入
		,Income = max(IsNull(checkin.Income,@DefaultZero))
		-- 劳产率目标
		,LaborYieldTarget = max(IsNull(checkin.LaborYieldTarget,@DefaultZero))
		-- 人工成本目标
		,LaborCostTarget = max(IsNull(checkin.LaborCostTarget,@DefaultZero))
		-- 区域
		,Region = IsNull(region.ID,-1)
		,RegionCode = IsNull(region.Code,'')
		,RegionName = IsNull(regionTrl.Name,'')
		
		---- 本月天数	
		-- 应出勤天数 = 当月天数 - 4
		,MonthDays = IsNull(Day(DateAdd(Day,-1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)) + 1,DateAdd(M,1,checkin.CheckInDate)))),27)  - 4
		
	
		-- 全日制标准工资=基本工资（01）+周末加班工资（02）+电话补贴（03）+交通补贴(04)+午餐补贴（05）+职务补贴（07）
		,StardardSalary = Sum(dbo.HBH_Fn_GetDecimal(
				case when salaryItem.Code in ('01','02','03','04','05','07') 
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
		-- FJ钟点工工资标准（F06）=薪资项目中F工资标准项目(F06)					-- （F56）
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
	from 		
		[10.28.76.125].U9.dbo.Cust_DayCheckIn checkin
		inner join [10.28.76.125].U9.dbo.Cust_DayCheckInLine checkinLine
		on checkin.ID = checkinLine.DayCheckIn

		inner join [10.28.76.125].U9.dbo.CBO_EmployeeArchive employee
		on checkinLine.EmployeeArchive = employee.ID	
		inner join [10.28.76.125].U9.dbo.CBO_Department dept
		-- 要按考勤头 的部门，来确认考勤部门
		on checkin.Department = dept.ID
		-- on employee.Dept = dept.ID
		inner join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl
		on deptTrl.ID = dept.ID
			and deptTrl.SysMLFlag = 'zh-CN'
	 
		left join [10.28.76.125].U9.dbo.CBO_Department region
		on SubString(dept.Code,1,5) = region.Code
		left join [10.28.76.125].U9.dbo.CBO_Department_Trl regionTrl
		on regionTrl.ID = region.ID
			and regionTrl.SysMLFlag = 'zh-CN'

		--left join CBO_Person person
		--on checkinLine.StaffMember = person.ID
		left join [10.28.76.125].U9.dbo.CBO_EmployeeSalaryFile salary
		on salary.Employee = employee.ID
		left join [10.28.76.125].U9.dbo.CBO_PublicSalaryItem salaryItem
		on salary.SalaryItem = salaryItem.ID
		left join [10.28.76.125].U9.dbo.CBO_PublicSalaryItem_Trl salaryItemTrl
		on salaryItemTrl.ID = salaryItem.ID 
			and salaryItemTrl.SysMLFlag = 'zh-CN'
	where 
			-- 全日制标准工资 = 标准工资=基本工资（01）+周末加班工资（02）+电话补贴（03）+交通补贴(04)+午餐补贴（05）+职务补贴（07）
		-- 非全日制标准工资 = 钟点工工资标准(06)			-- (038)
		-- 加班工资标准=薪资项目中F工资标准项目(F01)					-- （F13）
			--(salaryItem.Code is null 
			--	or salaryItem.Code in ('01','02','03','04','05','07' ,'06','F01'))

		dept.ID is not null
	group by 
		-- 员工
		employee.ID
		,IsNull(dept.ID,-1)
		,IsNull(dept.Code,'')
		,IsNull(deptTrl.Name,'')
		,checkin.CheckInDate
		-- ,checkinLine.EmployeeArchive
	
		-- 全日制
		,IsNull(checkinLine.FullTimeDay,@DefaultZero)
		-- 非全日制
		,IsNull(checkinLine.PartTimeDay,@DefaultZero)
		-- 钟点工
		,IsNull(checkinLine.HourlyDay,@DefaultZero)	
		-- 收入
		,IsNull(checkin.Income,@DefaultZero)
		-- 区域
		,IsNull(region.ID,-1)
		,IsNull(region.Code,'')
		,IsNull(regionTrl.Name,'')
	) as checkinSummary
--where 1=1
group by 
	Department
	,DepartmentCode
	,DepartmentName
	,CheckInDate
	,DisplayDate
	,StatisticsPeriod
	,Region
	,RegionCode
	,RegionName
	
	-- 应出勤天数 = 当月天数 - 4
	,MonthDays


select *
from Fact_U9_DayCheckIn
where (@请选择过滤年月 is null or @请选择过滤年月 = ''
		or @请选择过滤年月 = StatisticsPeriod
		)
	and (@请选择大区 is null or @请选择大区 = ''
		or @请选择大区 = RegionName
		)
	and (@请选择部门 is null or @请选择部门 = ''
		or @请选择部门 = DepartmentName
		)


