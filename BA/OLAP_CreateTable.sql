

-- �������ݿ⣺U9OLAP				(�˲���U9ODS_Template)


-- ά�ȣ���Ա
-- drop table Dim_U9_Person
create table Dim_U9_Person
(
	ID bigint
	,Code varchar(200)
	,Name varchar(200)	
)






-- ��ʵ���տ���
-- drop table Fact_U9_DayCheckIn
create table Fact_U9_DayCheckIn
(
	Department bigint
	,Status int
	,CurrentOperator bigint

	,Staff bigint
	,StaffCode varchar(200)
	,StaffName varchar(200)
	
	,FullTimeDay decimal(24,9)
	,PartTimeDay decimal(24,9)
	,HourlyDay decimal(24,9)
	
)

	


