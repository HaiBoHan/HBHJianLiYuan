
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
from HR20161108.dbo.CBO_Department dept 
	inner join HR20161108.dbo.CBO_Department_Trl deptTrl 
	on dept.ID= deptTrl.ID and deptTrl.SysMLFlag='zh-CN'
where
	dept.Code != '00001'
	and Len(dept.Code) = 5
*/



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
	,Income = Sum(Income)
	-- �Ͳ���Ŀ��
	,Max(LaborYieldTarget)
	-- �˹��ɱ�Ŀ��
	,Max(LaborCostTarget)
	-- ����
	,Region = Region
	,RegionCode
	,RegionName
	---- ��׼����
	--,StardardSalary = Sum(StardardSalary)
	
	-- Ա���չ���
	-- �չ���=��׼����/27 *ȫ����Ա������+�ӵ㹤���ʱ�׼(038)*�ӵ㹤����+��ȫ����Ա������*�ӱ�׼����		;�ӵ㹤���ʱ�׼ȡU9ά��н����Ŀ��ֵ�㹤���ʱ�׼
	-- 27 = �������� - 4
	,Salary = Sum(StardardSalary / MonthDays * IsNull(FullTimeDay,@DefaultZero) + IsNull(PartTimeDay,@DefaultZero) * IsNull(PartSalary,@DefaultZero) + IsNull(HourlyDay,@DefaultZero) * IsNull(OvertimeSalary,@DefaultZero))

from (
	select 
		-- Ա��
		employee.ID as EmployeeArchive
		-- ����
		,Department = IsNull(dept.ID,-1)
		,DepartmentCode = IsNull(dept.Code,'')
		,DepartmentName = IsNull(deptTrl.Name,'')

		-- ,checkin.CheckInDate
		,CheckInDate = DateName(Month,checkin.CheckInDate) + '.' + Right('00' + DateName(day,checkin.CheckInDate),2)
		,StatisticsPeriod = DateName(year,checkin.CheckInDate) + '��' + Right('00' + DateName(month,checkin.CheckInDate),2) + '��'
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
		,Income = IsNull(checkin.Income,@DefaultZero)
		-- �Ͳ���Ŀ��
		,LaborYieldTarget = max(IsNull(checkin.LaborYieldTarget,0))
		-- �˹��ɱ�Ŀ��
		,LaborCostTarget = max(IsNull(checkin.LaborCostTarget,0))
		-- ����
		,Region = IsNull(region.ID,-1)
		,RegionCode = IsNull(region.Code,'')
		,RegionName = IsNull(regionTrl.Name,'')
		-- ȫ���Ʊ�׼����=�������ʣ�01��+��ĩ�Ӱ๤�ʣ�02��+�绰������03��+��ͨ����(04)+��Ͳ�����05��+ְ������07��
		,StardardSalary = Sum(dbo.HBH_Fn_GetDecimal(
				case when salaryItem.Code in ('01','02','03','04','05','07') 
					then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
				else @DefaultZero end
					,@DefaultZero))
		
		-- ��ȫ���Ʊ�׼���� = �ӵ㹤���ʱ�׼(06)			-- (038)
		,PartSalary = Sum(dbo.HBH_Fn_GetDecimal(
				case when salaryItem.Code in ('06') 
					then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
				else @DefaultZero end
					,@DefaultZero))
		-- �Ӱ๤��
		-- �Ӱ๤�ʱ�׼=н����Ŀ��F���ʱ�׼��Ŀ(F01)					-- ��F13��
		,OvertimeSalary = Sum(dbo.HBH_Fn_GetDecimal(
				case when salaryItem.Code in ('F01') 
					then IsNull(salary.SalaryItemVlaue,@DefaultZero) 
				else @DefaultZero end
					,@DefaultZero))

		-- ��������	
		,MonthDays = IsNull(Day(DateAdd(Day,-1,DateAdd(d,- day(checkin.CheckInDate) + 1,checkin.CheckInDate))),27)

	from 		
		HR20161108.dbo.CBO_EmployeeArchive employee	
		left join HR20161108.dbo.CBO_Department dept
		-- on checkin.Department = dept.ID
		on employee.Dept = dept.ID
		left join HR20161108.dbo.CBO_Department_Trl deptTrl
		on deptTrl.ID = dept.ID
			and deptTrl.SysMLFlag = 'zh-CN'
		
		left join HR20161108.dbo.Cust_DayCheckInLine checkinLine
		on checkinLine.EmployeeArchive = employee.ID
		left join HR20161108.dbo.Cust_DayCheckIn checkin
		on checkin.ID = checkinLine.DayCheckIn
	 
		left join HR20161108.dbo.CBO_Department region
		on SubString(dept.Code,1,5) = region.Code
		left join HR20161108.dbo.CBO_Department_Trl regionTrl
		on regionTrl.ID = region.ID
			and regionTrl.SysMLFlag = 'zh-CN'

		--left join CBO_Person person
		--on checkinLine.StaffMember = person.ID
		left join HR20161108.dbo.CBO_EmployeeSalaryFile salary
		on salary.Employee = employee.ID
		left join HR20161108.dbo.CBO_PublicSalaryItem salaryItem
		on salary.SalaryItem = salaryItem.ID
		left join HR20161108.dbo.CBO_PublicSalaryItem_Trl salaryItemTrl
		on salaryItemTrl.ID = salaryItem.ID 
			and salaryItemTrl.SysMLFlag = 'zh-CN'
	where 
			-- ȫ���Ʊ�׼���� = ��׼����=�������ʣ�01��+��ĩ�Ӱ๤�ʣ�02��+�绰������03��+��ͨ����(04)+��Ͳ�����05��+ְ������07��
		-- ��ȫ���Ʊ�׼���� = �ӵ㹤���ʱ�׼(06)			-- (038)
		-- �Ӱ๤�ʱ�׼=н����Ŀ��F���ʱ�׼��Ŀ(F01)					-- ��F13��
			(salaryItem.Code is null 
				or salaryItem.Code in ('01','02','03','04','05','07' ,'06','F01'))
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
	,StatisticsPeriod
	,Region
	,RegionCode
	,RegionName



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


