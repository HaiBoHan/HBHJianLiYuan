

-- �������ݿ⣺U9OLAP				(�˲���U9ODS_Template)


-- ά�ȣ���Ա
-- drop table Dim_U9_Person
create table Dim_U9_Person
(
	ID bigint
	,Code varchar(200)
	,Name varchar(200)	
)


-- ����2
create table Dim_U9_Department2
(
	ID bigint
	,Code varchar(200)
	,Name varchar(200)	
	,Level int
	,Org bigint
)







-- ��ʵ���տ���
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

		


