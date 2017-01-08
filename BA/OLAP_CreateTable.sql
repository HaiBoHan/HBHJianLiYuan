

-- 集团数据库：U9OLAP				(账簿，U9ODS_Template)



-- 月份过滤条件
create table Dim_U9_MonthFilter
(
	MonthName varchar(125) default ''

)

-- 月份过滤条件
create table Dim_U9_Date_Filter
(
	DayName varchar(125) default ''
	,DayDate datetime
)



-- 维度，人员
-- drop table Dim_U9_Person
create table Dim_U9_Person
(
	ID bigint
	,Code varchar(200)
	,Name varchar(200)	
)


-- 部门2（5位），大区
-- select * from Dim_U9_Department2
create table Dim_U9_Department2
(
	ID bigint
	,Code varchar(200)
	,Name varchar(200)	
	,Level int
	,Org bigint
)
/*
truncate table Dim_U9_Department2

insert into Dim_U9_Department2
select dept.ID,dept.Code,deptTrl.Name,dept.Level+1 as Level,dept.Org
from HR20161108.dbo.CBO_Department dept 
	inner join HR20161108.dbo.CBO_Department_Trl deptTrl 
	on dept.ID= deptTrl.ID and deptTrl.SysMLFlag='zh-CN'
where
	dept.Code not like '00001%'
	and Len(dept.Code) = 5
*/


-- 部门3（7位），区域
-- select * from Dim_U9_Department3
create table Dim_U9_Department3
(
	ID bigint
	,Code varchar(200)
	,Name varchar(200)	
	,Level int
	,Org bigint
)
/*
truncate table Dim_U9_Department3

insert into Dim_U9_Department3
select dept.ID,dept.Code,deptTrl.Name,dept.Level+1 as Level,dept.Org
from HR20161108.dbo.CBO_Department dept 
	inner join HR20161108.dbo.CBO_Department_Trl deptTrl 
	on dept.ID= deptTrl.ID and deptTrl.SysMLFlag='zh-CN'
where
	dept.Code not like '00001%'
	and Len(dept.Code) = 7	
*/







-- 事实，日考勤
-- drop table Fact_U9_DayCheckIn
create table Fact_U9_DayCheckIn
(
	Region bigint
	,RegionCode varchar(200)
	,RegionName varchar(200)
	,Region2 bigint
	,Region2Code varchar(200)
	,Region2Name varchar(200)
	,Department bigint
	,DepartmentCode varchar(200)
	,DepartmentName varchar(200)

	--,CheckInDate varchar(200)
	,DisplayDate varchar(200) 
	,StatisticsPeriod varchar(125)
	--,Status int
	--,CurrentOperator bigint

	--,Staff bigint
	--,StaffCode varchar(200)
	--,StaffName varchar(200)
	
	,FullTimeDay decimal(24,9)
	,PartTimeDay decimal(24,9)
	,HourlyDay decimal(24,9)
	-- 日出勤人次 = 非全日制员工出勤合计/4 + 钟点工员工出勤合计/8 + 全日制员工出勤合计 
	,PersonTime decimal(24,9)
	
	-- 收入
	,Income decimal(24,9)
	-- 劳产率目标
	,LaborYieldTarget decimal(24,9)
	-- 人工成本目标
	,LaborCostTarget decimal(24,9)
	-- 工资
	,Salary decimal(24,9)

	-- 劳产率 = 
	-- ,LaborYield decimal(24,9)
	-- 人工成本比例
	-- 
	
	-- 应出勤天数 = 当月天数 - 4
	,MonthDays decimal(24,9)
	-- 日保险
	,DayInsurance decimal(24,9)
	
	-- 月份第一天
	,FirstDay DateTime
	-- 月份第一天  是第几周
	,FirstWeek int
	-- 月份第一天  有周几
	,FirstWeekDay int
		
	-- 月份最后一天
	,LastDay DateTime
	-- 月份最后一天  是第几周
	,LastWeek int
	-- 月份最后一天  有周几
	,LastWeekDay int
)

/*
alter table Fact_U9_DayCheckIn
add MonthDays decimal(24,9)


alter table Fact_U9_DayCheckIn
add DayInsurance decimal(24,9)
*/


-- 事实，Excel导入		劳动生产率人工成本统计表
---- drop table Fact_U9_DayCheckIn
--create table Fact_U9_DayCheckIn
--(
--	Department bigint
--	,Status int
--	,CurrentOperator bigint

--	,Staff bigint
--	,StaffCode varchar(200)
--	,StaffName varchar(200)
	
--	,FullTimeDay decimal(24,9)
--	,PartTimeDay decimal(24,9)
--	,HourlyDay decimal(24,9)
	
--)

		

-- 事实表，假日实际出勤
---- drop table Fact_U9_HolidayAttendance
create table Fact_U9_HolidayAttendance 
(
	Region bigint
	,RegionCode varchar(200)
	,RegionName varchar(200)
	,Region2 bigint
	,Region2Code varchar(200)
	,Region2Name varchar(200)

	,Department bigint
	,DepartmentCode varchar(200)
	,DepartmentName varchar(200)
	
	,CheckInDate DateTime
	-- 开始日期
	,DateStart varchar(125)
	-- 结束日期
	,DateEnd varchar(125)
	,DisplayDate varchar(200) 
	,StatisticsPeriod varchar(125)


	-- 预警.假期日收入
	,ForecastHolidayDayIncome decimal(24,9)
	-- 预警.假期出勤人数
	,ForecastAttendance decimal(24,9)
	-- 预警.当日人工工资
	,ForecastWage decimal(24,9)
	-- 预警.日人均效率 = 收入 / 出勤人数
	,ForecastEfficiency decimal(24,9)

	-- 实际.假期日收入
	,FactHolidayDayIncome decimal(24,9)
	-- 实际.假期出勤人数
	-- 日出勤人次 = 非全日制员工出勤合计/4 + 钟点工员工出勤合计/8 + 全日制员工出勤合计 
	,FactAttendance decimal(24,9)
	-- 实际.日人均效率 =  收入 / 出勤人数
	,FactEfficiency decimal(24,9)
	
	-- 全日制
	,FullTimeDay decimal(24,9)
	-- 非全日制
	,PartTimeDay decimal(24,9)
	-- 钟点工
	,HourlyDay decimal(24,9)
	
)



-- 假期人均效率人工成本预警表
-- drop table Fact_U9_EfficiencyCostWarning
create table Fact_U9_EfficiencyCostWarning
(
	Region bigint
	,RegionCode varchar(200)
	,RegionName varchar(200)
	,Region2 bigint
	,Region2Code varchar(200)
	,Region2Name varchar(200)

	,Department bigint
	,DepartmentCode varchar(200)
	,DepartmentName varchar(200)
	
	-- 年月期间
	,StatisticsPeriod varchar(125)
	-- 日期
	,WarningDate varchar(125)
	-- 日期
	,CheckInDate DateTime
	-- 开始日期
	,DateStart varchar(125)
	-- 结束日期
	,DateEnd varchar(125)
	-- 餐次
	,MealTime varchar(200) 
	-- 预计就餐人数
	,EstimatedQty decimal(24,9)
	-- 餐标
	,MealStandard decimal(24,9)
	-- 就餐收入,这个做合计公式比较合适，不过不知道BA支持不;   =  早餐预计就餐人数*早餐餐标+中餐预计就餐人数*中餐餐标+晚餐预计就餐人数*晚餐餐标+夜餐预计就餐人数*夜餐餐标
	,DiningIncome decimal(24,9)

	-- 日出勤小时数
	,AttendanceTime decimal(24,9)
	-- 折算人数 ????  = 日出勤小时数/8
	,TranslatedNumber decimal(24,9)

	-- 当日人工工资
	,Wage decimal(24,9)
	-- 日综合毛利
	,GrossProfit decimal(24,9)
	
	-- 人均效率预警
	-- 人均效率 = if（折算人数=0,0，就餐收入/折算人数）
	,PerEfficiency decimal(24,9)
	-- 公司节日控制标准(效率) = ：固定值500
	,EfficiencyHolidayStandards decimal(24,9) default 500
	-- 差异(效率) = 公司节日控制标准-人均效率
	,EfficiencyDiffer decimal(24,9)
	-- 达标情况(效率) = if(差异>=0,"达标","人均效率低于公司要求，请调整")
	,EfficiencyStandardConditions varchar(125)

	-- 人工成本预警
	-- 人工成本 = IF(当日人工工资=0,"0",当日人工工资/就餐收入)
	,PerCost decimal(24,9)
	-- 公司节日控制标准(成本) = ：固定值20%
	,CostHolidayStandards decimal(24,9) default 0.2
	-- 差异(成本) = 公司节日控制标准-人均效率
	,CostDiffer decimal(24,9)
	-- 达标情况(成本) = IF(差异>=0,"人工成本超标，请调整","达标")
	,CostStandardConditions varchar(125)

	-- 备注
	,Memo varchar(125)

	
	-- 预计就餐人数(早)
	,MorningEstimatedQty decimal(24,9)
	-- 预计就餐人数(中)
	,NoonEstimatedQty decimal(24,9)
	-- 预计就餐人数(晚)
	,AfternoonEstimatedQty decimal(24,9)
	-- 预计就餐人数(夜)
	,NightEstimatedQty decimal(24,9)
	
	-- 餐标(早)
	,MorningMealStandard decimal(24,9)
	-- 餐标(中)
	,NoonMealStandard decimal(24,9)
	-- 餐标(晚)
	,AfternoonMealStandard decimal(24,9)
	-- 餐标(夜)
	,NightMealStandard decimal(24,9)
)

