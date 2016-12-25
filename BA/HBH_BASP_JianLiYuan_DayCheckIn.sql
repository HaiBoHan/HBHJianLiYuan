
/*
select *
from HBH_SPParamRecord
order by CreatedOn desc
*/

if exists(select * from sys.objects where name='HBH_BASP_JianLiYuan_DayCheckIn')
-- ���������ɾ��
	drop proc HBH_BASP_JianLiYuan_DayCheckIn
go
-- �����洢����
create proc HBH_BASP_JianLiYuan_DayCheckIn  (
-- @StartDate datetime = null
-- ,@EndDate datetime = null

@��ѡ��������� varchar(125) = ''
,@��ѡ����� varchar(125) = ''
,@��ѡ���� varchar(125) = ''
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
			,Memo varchar(max)	-- ��ע
			)
		end

		insert into HBH_SPParamRecord
		(ProcName,ParamName,ParamValue,CreatedOn)
		select 'HBH_BASP_JianLiYuan_DayCheckIn','@��ѡ���������',IsNull(@��ѡ���������,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_DayCheckIn','@��ѡ�����',IsNull(@��ѡ�����,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_DayCheckIn','@��ѡ����',IsNull(@��ѡ����,'null'),GETDATE()

		union select 'HBH_BASP_JianLiYuan_DayCheckIn','ProcSql','exec HBH_BASP_JianLiYuan_DayCheckIn '
				+ IsNull('''' + @��ѡ��������� + '''' ,'null')
				+ ',' + IsNull('''' + @��ѡ����� + '''' ,'null')
				+ ',' + IsNull('''' + @��ѡ���� + '''' ,'null')

			   ,GETDATE()
	end
end

	declare @SysMlFlag varchar(11) = 'zh-CN'
	declare @DefaultZero decimal(24,9) = 0
	declare @Now datetime = GetDate();
	declare @CurDate datetime = GetDate()
	declare @Today datetime = convert(varchar(10), GetDate(), 120)

	

-- ���Ŷ���,ɾ�����ݣ����³�ȡ
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

-- ����
insert into Dim_U9_Department2
select dept.ID,dept.Code,deptTrl.Name,dept.Level+1 as Level,dept.Org
from [10.28.76.125].U9.dbo.CBO_Department dept 
	inner join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl 
	on dept.ID= deptTrl.ID and deptTrl.SysMLFlag='zh-CN'
where
	dept.Code != '00001'
	and Len(dept.Code) = 5
	-- �����ڵ�����
	and dept.ID not in (select region2.ID from Dim_U9_Department2 region2)

-- ����
insert into Dim_U9_Department3
select dept.ID,dept.Code,deptTrl.Name,dept.Level+1 as Level,dept.Org
from [10.28.76.125].U9.dbo.CBO_Department dept 
	inner join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl 
	on dept.ID= deptTrl.ID and deptTrl.SysMLFlag='zh-CN'
where
	Len(dept.Code) = 7
	-- �����ڵ�����
	and dept.ID not in (select region3.ID from Dim_U9_Department3 region3)




-- HRTest
-- �Ͳ��� = ���� / �����մ�
-- If((IsNull([��ѯ1].[�ճ����˴�],0) = 0),0,[��ѯ1].[����] / [��ѯ1].[�ճ����˴�])
-- �˹��ɱ����� = ���� / ����
-- If((IsNull([��ѯ1].[����],0) = 0),0,[��ѯ1].[����] / [��ѯ1].[����])

-- ��ȡ�ű�

-- ��ʵ��Fact_U9_DayCheckIn
-- ɾ��
truncate table Fact_U9_DayCheckIn
-- ����
insert into Fact_U9_DayCheckIn
--(
--	Department
--	,DepartmentCode
--	,DepartmentName
--	,CheckInDate
--	,StatisticsPeriod
--	-- ȫ����
--	,FullTimeDay
--	-- ��ȫ����
--	,PartTimeDay
--	-- �ӵ㹤
--	,HourlyDay
--	-- �ճ������� = ��ȫ����Ա�����ںϼ�/4 + �ӵ㹤Ա�����ںϼ�/8 + ȫ����Ա�����ںϼ� 
--	,PersonTime
--	-- ����
--	,Income
--  -- �Ͳ���Ŀ��
--  ,LaborYieldTarget
--  -- �˹��ɱ�Ŀ��
--  ,LaborCostTarget
--	-- ����
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
	
	-- ȫ����
	,FullTimeDay = Sum(IsNull(FullTimeDay,@DefaultZero))
	-- ��ȫ����
	,PartTimeDay = Sum(IsNull(PartTimeDay,@DefaultZero))
	-- �ӵ㹤
	,HourlyDay = Sum(IsNull(HourlyDay,@DefaultZero))
	-- �ճ������� = ��ȫ����Ա�����ںϼ�/4 + �ӵ㹤Ա�����ںϼ�/8 + ȫ����Ա�����ںϼ� 
	,PersonTime = Sum(IsNull(PartTimeDay,@DefaultZero)) / 4 + Sum(IsNull(HourlyDay,@DefaultZero)) / 8 + Sum(IsNull(FullTimeDay,@DefaultZero))
	
	-- ����
	,Income = Max(Income)
	-- �Ͳ���Ŀ��
	,LaborYieldTarget = Max(LaborYieldTarget)
	-- �˹��ɱ�Ŀ��
	,LaborCostTarget = Max(LaborCostTarget)
	-- ����
	,Region = Region
	,RegionCode
	,RegionName
	---- ��׼����
	--,StardardSalary = Sum(StardardSalary)
	
	-- Ա���չ���
	-- �չ���=��׼����/27 *ȫ����Ա������+�ӵ㹤���ʱ�׼(038)*�ӵ㹤����+��ȫ����Ա������*�ӱ�׼����		;�ӵ㹤���ʱ�׼ȡU9ά��н����Ŀ��ֵ�㹤���ʱ�׼
	-- 27 = �������� - 4
	--,Salary = Sum(StardardSalary / MonthDays * IsNull(FullTimeDay,@DefaultZero) + IsNull(PartTimeDay,@DefaultZero) * IsNull(PartSalary,@DefaultZero) + IsNull(HourlyDay,@DefaultZero) * IsNull(OvertimeSalary,@DefaultZero))
	,Salary = Sum(
		-- ȫ����Ա������
		(StardardSalary / MonthDays * IsNull(FullTimeDay,@DefaultZero) 
		-- ��ȫ����Ա������
		+ IsNull(FPartSalary,@DefaultZero) * IsNull(PartTimeDay,@DefaultZero) 
		-- ȫ���ƼӰ๤��
		+ IsNull(OvertimeSalary,@DefaultZero) * IsNull(HourlyDay,@DefaultZero) 
		-- ��ȫ���ƼӰ๤��
		+ IsNull(FOvertimeSalary,@DefaultZero) * IsNull(HourlyDay,@DefaultZero))
		)

	-- Ӧ�������� = �������� - 4
	,MonthDays
	-- �ձ���
	,DayInsurance = Sum
			(
			-- ȫ����Ա������
			(IsNull(InsuranceSalary,@DefaultZero) / MonthDays * IsNull(FullTimeDay,@DefaultZero) 
			-- ��ȫ����Ա������
			+ (IsNull(FInsuranceSalary,@DefaultZero) / MonthDays) * IsNull(PartTimeDay,@DefaultZero) )
			)

from (
	select 
		-- Ա��
		employee.ID as EmployeeArchive
		-- ����
		,Department = IsNull(dept.ID,-1)
		,DepartmentCode = IsNull(dept.Code,'')
		,DepartmentName = IsNull(deptTrl.Name,'')

		,checkin.CheckInDate
		,DisplayDate = Right('00' + DateName(Month,checkin.CheckInDate),2) + '.' + Right('00' + DateName(day,checkin.CheckInDate),2)
		,StatisticsPeriod = Right('0000' + DateName(year,checkin.CheckInDate),4) + '��' + Right('00' + DateName(month,checkin.CheckInDate),2) + '��'
		--,checkin.Status as Status
		--,checkin.CurrentOperator as CurrentOperator

		--,checkinLine.StaffMember as Staff
		--,person.PersonID as StaffCode
		--,person.Name as StaffName

	
		-- ȫ����
		,FullTimeDay = IsNull(checkinLine.FullTimeDay,@DefaultZero)
		-- ��ȫ����
		,PartTimeDay = IsNull(checkinLine.PartTimeDay,@DefaultZero)
		-- �ӵ㹤
		,HourlyDay = IsNull(checkinLine.HourlyDay,@DefaultZero)
	
		-- ����
		,Income = max(IsNull(checkin.Income,@DefaultZero))
		-- �Ͳ���Ŀ��
		,LaborYieldTarget = max(IsNull(checkin.LaborYieldTarget,@DefaultZero))
		-- �˹��ɱ�Ŀ��
		,LaborCostTarget = max(IsNull(checkin.LaborCostTarget,@DefaultZero))
		-- ����
		,Region = IsNull(region.ID,-1)
		,RegionCode = IsNull(region.Code,'')
		,RegionName = IsNull(regionTrl.Name,'')
		
		---- ��������	
		-- Ӧ�������� = �������� - 4
		,MonthDays = IsNull(Day(DateAdd(Day,-1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)) + 1,DateAdd(M,1,checkin.CheckInDate)))),27)  - 4
		
	
		-- ȫ���Ʊ�׼����=�������ʣ�01��+��ĩ�Ӱ๤�ʣ�02��+�绰������03��+��ͨ����(04)+��Ͳ�����05��+ְ������07��
		,StardardSalary = Sum(dbo.HBH_Fn_GetDecimal(
				case when salaryItem.Code in ('01','02','03','04','05','07') 
					then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
				else @DefaultZero end
					,@DefaultZero))
		
		-- �Ӱ๤��
		-- �ӵ㹤���ʱ�׼ = �ӵ㹤���ʱ�׼(06)			-- (038)
		,OvertimeSalary = Sum(dbo.HBH_Fn_GetDecimal(
				case when salaryItem.Code in ('06') 
					then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
				else @DefaultZero end
					,@DefaultZero))
		
		-- F�ӵ㹤���ʱ�׼��F01�� = �ӵ㹤���ʱ�׼(F01)			-- (F13)
		,FPartSalary = Sum(dbo.HBH_Fn_GetDecimal(
				case when salaryItem.Code in ('F01') 
					then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
				else @DefaultZero end
					,@DefaultZero))
		-- F�Ӱ๤��
		-- FJ�ӵ㹤���ʱ�׼��F06��=н����Ŀ��F���ʱ�׼��Ŀ(F06)					-- ��F56��
		,FOvertimeSalary = Sum(dbo.HBH_Fn_GetDecimal(
				case when salaryItem.Code in ('F06') 
					then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
				else @DefaultZero end
					,@DefaultZero))

		-- ��λ����
		-- ��λ���գ�12��=н����Ŀ�� ��λ����(12)					-- ��113��
		,InsuranceSalary = Sum(dbo.HBH_Fn_GetDecimal(
				case when salaryItem.Code in ('12') 
					then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
				else @DefaultZero end
					,@DefaultZero))
		-- F��λ����
		-- F��λ���գ�F04��=н����Ŀ�� F��λ����(F04)					-- ��F52��
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
		-- Ҫ������ͷ �Ĳ��ţ���ȷ�Ͽ��ڲ���
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
			-- ȫ���Ʊ�׼���� = ��׼����=�������ʣ�01��+��ĩ�Ӱ๤�ʣ�02��+�绰������03��+��ͨ����(04)+��Ͳ�����05��+ְ������07��
		-- ��ȫ���Ʊ�׼���� = �ӵ㹤���ʱ�׼(06)			-- (038)
		-- �Ӱ๤�ʱ�׼=н����Ŀ��F���ʱ�׼��Ŀ(F01)					-- ��F13��
			--(salaryItem.Code is null 
			--	or salaryItem.Code in ('01','02','03','04','05','07' ,'06','F01'))

		dept.ID is not null
	group by 
		-- Ա��
		employee.ID
		,IsNull(dept.ID,-1)
		,IsNull(dept.Code,'')
		,IsNull(deptTrl.Name,'')
		,checkin.CheckInDate
		-- ,checkinLine.EmployeeArchive
	
		-- ȫ����
		,IsNull(checkinLine.FullTimeDay,@DefaultZero)
		-- ��ȫ����
		,IsNull(checkinLine.PartTimeDay,@DefaultZero)
		-- �ӵ㹤
		,IsNull(checkinLine.HourlyDay,@DefaultZero)	
		-- ����
		,IsNull(checkin.Income,@DefaultZero)
		-- ����
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
	
	-- Ӧ�������� = �������� - 4
	,MonthDays


select *
from Fact_U9_DayCheckIn
where (@��ѡ��������� is null or @��ѡ��������� = ''
		or @��ѡ��������� = StatisticsPeriod
		)
	and (@��ѡ����� is null or @��ѡ����� = ''
		or @��ѡ����� = RegionName
		)
	and (@��ѡ���� is null or @��ѡ���� = ''
		or @��ѡ���� = DepartmentName
		)


