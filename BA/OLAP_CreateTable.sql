

-- �������ݿ⣺U9OLAP				(�˲���U9ODS_Template)


-- ά�ȣ���Ա
-- drop table Dim_U9_Person
create table Dim_U9_Person
(
	ID bigint
	,Code varchar(200)
	,Name varchar(200)	
)


-- ����2��5λ��������
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


-- ����3��7λ��������
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







-- ��ʵ���տ���
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
	-- �ճ����˴� = ��ȫ����Ա�����ںϼ�/4 + �ӵ㹤Ա�����ںϼ�/8 + ȫ����Ա�����ںϼ� 
	,PersonTime decimal(24,9)
	
	-- ����
	,Income decimal(24,9)
	-- �Ͳ���Ŀ��
	,LaborYieldTarget decimal(24,9)
	-- �˹��ɱ�Ŀ��
	,LaborCostTarget decimal(24,9)
	-- ����
	,Region bigint
	,RegionCode varchar(200)
	,RegionName varchar(200)
	-- ����
	,Salary decimal(24,9)

	-- �Ͳ��� = 
	-- ,LaborYield decimal(24,9)
	-- �˹��ɱ�����
	-- 
)





-- ��ʵ��Excel����		�Ͷ��������˹��ɱ�ͳ�Ʊ�
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

		

-- ��ʵ������ʵ�ʳ���
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


	-- Ԥ��.����������
	,ForecastHolidayDayIncome decimal(24,9)
	-- Ԥ��.���ڳ�������
	,ForecastAttendance decimal(24,9)
	-- Ԥ��.�����˹�����
	,ForecastWage decimal(24,9)
	-- Ԥ��.���˾�Ч�� = ���� / ��������
	,ForecastEfficiency decimal(24,9)

	-- ʵ��.����������
	,FactHolidayDayIncome decimal(24,9)
	-- ʵ��.���ڳ�������
	-- �ճ����˴� = ��ȫ����Ա�����ںϼ�/4 + �ӵ㹤Ա�����ںϼ�/8 + ȫ����Ա�����ںϼ� 
	,FactAttendance decimal(24,9)
	-- ʵ��.���˾�Ч�� =  ���� / ��������
	,FactEfficiency decimal(24,9)
	
	-- ȫ����
	,FullTimeDay decimal(24,9)
	-- ��ȫ����
	,PartTimeDay decimal(24,9)
	-- �ӵ㹤
	,HourlyDay decimal(24,9)
	
)



