

-- 集团数据库：U9OLAP				(账簿，U9ODS_Template)


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
	Department bigint
	,DepartmentCode varchar(200)
	,DepartmentName varchar(200)
	,CheckInDate DateTime
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
	-- 大区
	,Region bigint
	,RegionCode varchar(200)
	,RegionName varchar(200)
	-- 工资
	,Salary decimal(24,9)

	-- 劳产率 = 
	-- ,LaborYield decimal(24,9)
	-- 人工成本比例
	-- 
)





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



