

-- 集团数据库：U9OLAP				(账簿，U9ODS_Template)


-- 维度，人员
-- drop table Dim_U9_Person
create table Dim_U9_Person
(
	ID bigint
	,Code varchar(200)
	,Name varchar(200)	
)


-- 部门2
create table Dim_U9_Department2
(
	ID bigint
	,Code varchar(200)
	,Name varchar(200)	
	,Level int
	,Org bigint
)







-- 事实，日考勤
-- drop table Fact_U9_DayCheckIn
create table Fact_U9_DayCheckIn
(
	Department bigint
	,DepartmentCode varchar(200)
	,DepartmentName varchar(200)
	,CheckInDate varchar(200) 
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

		


