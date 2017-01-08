

-- �������ݿ⣺U9OLAP				(�˲���U9ODS_Template)



-- �·ݹ�������
create table Dim_U9_MonthFilter
(
	MonthName varchar(125) default ''

)

-- �·ݹ�������
create table Dim_U9_Date_Filter
(
	DayName varchar(125) default ''
	,DayDate datetime
)



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
	-- �ճ����˴� = ��ȫ����Ա�����ںϼ�/4 + �ӵ㹤Ա�����ںϼ�/8 + ȫ����Ա�����ںϼ� 
	,PersonTime decimal(24,9)
	
	-- ����
	,Income decimal(24,9)
	-- �Ͳ���Ŀ��
	,LaborYieldTarget decimal(24,9)
	-- �˹��ɱ�Ŀ��
	,LaborCostTarget decimal(24,9)
	-- ����
	,Salary decimal(24,9)

	-- �Ͳ��� = 
	-- ,LaborYield decimal(24,9)
	-- �˹��ɱ�����
	-- 
	
	-- Ӧ�������� = �������� - 4
	,MonthDays decimal(24,9)
	-- �ձ���
	,DayInsurance decimal(24,9)
	
	-- �·ݵ�һ��
	,FirstDay DateTime
	-- �·ݵ�һ��  �ǵڼ���
	,FirstWeek int
	-- �·ݵ�һ��  ���ܼ�
	,FirstWeekDay int
		
	-- �·����һ��
	,LastDay DateTime
	-- �·����һ��  �ǵڼ���
	,LastWeek int
	-- �·����һ��  ���ܼ�
	,LastWeekDay int
)

/*
alter table Fact_U9_DayCheckIn
add MonthDays decimal(24,9)


alter table Fact_U9_DayCheckIn
add DayInsurance decimal(24,9)
*/


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
	-- ��ʼ����
	,DateStart varchar(125)
	-- ��������
	,DateEnd varchar(125)
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



-- �����˾�Ч���˹��ɱ�Ԥ����
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
	
	-- �����ڼ�
	,StatisticsPeriod varchar(125)
	-- ����
	,WarningDate varchar(125)
	-- ����
	,CheckInDate DateTime
	-- ��ʼ����
	,DateStart varchar(125)
	-- ��������
	,DateEnd varchar(125)
	-- �ʹ�
	,MealTime varchar(200) 
	-- Ԥ�ƾͲ�����
	,EstimatedQty decimal(24,9)
	-- �ͱ�
	,MealStandard decimal(24,9)
	-- �Ͳ�����,������ϼƹ�ʽ�ȽϺ��ʣ�������֪��BA֧�ֲ�;   =  ���Ԥ�ƾͲ�����*��Ͳͱ�+�в�Ԥ�ƾͲ�����*�вͲͱ�+���Ԥ�ƾͲ�����*��Ͳͱ�+ҹ��Ԥ�ƾͲ�����*ҹ�Ͳͱ�
	,DiningIncome decimal(24,9)

	-- �ճ���Сʱ��
	,AttendanceTime decimal(24,9)
	-- �������� ????  = �ճ���Сʱ��/8
	,TranslatedNumber decimal(24,9)

	-- �����˹�����
	,Wage decimal(24,9)
	-- ���ۺ�ë��
	,GrossProfit decimal(24,9)
	
	-- �˾�Ч��Ԥ��
	-- �˾�Ч�� = if����������=0,0���Ͳ�����/����������
	,PerEfficiency decimal(24,9)
	-- ��˾���տ��Ʊ�׼(Ч��) = ���̶�ֵ500
	,EfficiencyHolidayStandards decimal(24,9) default 500
	-- ����(Ч��) = ��˾���տ��Ʊ�׼-�˾�Ч��
	,EfficiencyDiffer decimal(24,9)
	-- ������(Ч��) = if(����>=0,"���","�˾�Ч�ʵ��ڹ�˾Ҫ�������")
	,EfficiencyStandardConditions varchar(125)

	-- �˹��ɱ�Ԥ��
	-- �˹��ɱ� = IF(�����˹�����=0,"0",�����˹�����/�Ͳ�����)
	,PerCost decimal(24,9)
	-- ��˾���տ��Ʊ�׼(�ɱ�) = ���̶�ֵ20%
	,CostHolidayStandards decimal(24,9) default 0.2
	-- ����(�ɱ�) = ��˾���տ��Ʊ�׼-�˾�Ч��
	,CostDiffer decimal(24,9)
	-- ������(�ɱ�) = IF(����>=0,"�˹��ɱ����꣬�����","���")
	,CostStandardConditions varchar(125)

	-- ��ע
	,Memo varchar(125)

	
	-- Ԥ�ƾͲ�����(��)
	,MorningEstimatedQty decimal(24,9)
	-- Ԥ�ƾͲ�����(��)
	,NoonEstimatedQty decimal(24,9)
	-- Ԥ�ƾͲ�����(��)
	,AfternoonEstimatedQty decimal(24,9)
	-- Ԥ�ƾͲ�����(ҹ)
	,NightEstimatedQty decimal(24,9)
	
	-- �ͱ�(��)
	,MorningMealStandard decimal(24,9)
	-- �ͱ�(��)
	,NoonMealStandard decimal(24,9)
	-- �ͱ�(��)
	,AfternoonMealStandard decimal(24,9)
	-- �ͱ�(ҹ)
	,NightMealStandard decimal(24,9)
)

