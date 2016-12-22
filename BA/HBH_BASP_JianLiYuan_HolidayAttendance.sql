
/*
select *
from HBH_SPParamRecord
order by CreatedOn desc
*/

if exists(select * from sys.objects where name='HBH_BASP_JianLiYuan_HolidayAttendance')
-- ���������ɾ��
	drop proc HBH_BASP_JianLiYuan_HolidayAttendance
go
-- �����洢����
create proc HBH_BASP_JianLiYuan_HolidayAttendance  (
-- @StartDate datetime = null
-- ,@EndDate datetime = null

@��ѡ��������� varchar(125) = ''
,@��ѡ����� varchar(125) = ''
,@��ѡ������ varchar(125) = ''
,@��ѡ���� varchar(125) = ''
)
with encryption
as
	SET NOCOUNT ON;
	

if exists(select name from sys.objects where name = 'HBH_Debug_Param')
begin
	declare @Debugger bit = (select top 1 Debugger from HBH_Debug_Param where ProcName = 'HBH_BASP_JianLiYuan_HolidayAttendance' or ProcName is null or ProcName = '' order by ProcName desc)
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
		select 'HBH_BASP_JianLiYuan_HolidayAttendance','@��ѡ���������',IsNull(@��ѡ���������,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_HolidayAttendance','@��ѡ�����',IsNull(@��ѡ�����,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_HolidayAttendance','@��ѡ������',IsNull(@��ѡ������,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_HolidayAttendance','@��ѡ����',IsNull(@��ѡ����,'null'),GETDATE()

		union select 'HBH_BASP_JianLiYuan_HolidayAttendance','ProcSql','exec HBH_BASP_JianLiYuan_HolidayAttendance '
				+ IsNull('''' + @��ѡ��������� + '''' ,'null')
				+ ',' + IsNull('''' + @��ѡ����� + '''' ,'null')
				+ ',' + IsNull('''' + @��ѡ������ + '''' ,'null')
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
*/



-- HRTest
-- �Ͳ��� = ���� / �����մ�
-- If((IsNull([��ѯ1].[�ճ����˴�],0) = 0),0,[��ѯ1].[����] / [��ѯ1].[�ճ����˴�])
-- �˹��ɱ����� = ���� / ����
-- If((IsNull([��ѯ1].[����],0) = 0),0,[��ѯ1].[����] / [��ѯ1].[����])

-- ��ȡ�ű�

-- ��ʵ��Fact_U9_HolidayAttendance
-- ɾ��
truncate table Fact_U9_HolidayAttendance
-- ����
insert into Fact_U9_HolidayAttendance
--(
--	Region bigint
--	,RegionCode varchar(200)
--	,RegionName varchar(200)
--	,Region2 bigint
--	,Region2Code varchar(200)
--	,Region2Name varchar(200)

--	,Department bigint
--	,DepartmentCode varchar(200)
--	,DepartmentName varchar(200)
	
--	,CheckInDate DateTime
--	,DisplayDate varchar(200) 
--	,StatisticsPeriod varchar(125)


--	-- Ԥ��.����������
--	,ForecastHolidayDayIncome decimal(24,9)
--	-- Ԥ��.���ڳ�������
--	,ForecastAttendance decimal(24,9)
--	-- Ԥ��.���˾�Ч��
--	,ForecastEfficiency decimal(24,9)

--	-- ʵ��.����������
--	,FactHolidayDayIncome decimal(24,9)
--	-- ʵ��.���ڳ�������
--	-- �ճ����˴� = ��ȫ����Ա�����ںϼ�/4 + �ӵ㹤Ա�����ںϼ�/8 + ȫ����Ա�����ںϼ� 
--	,FactAttendance decimal(24,9)
--	,FullTimeDay decimal(24,9)
--	,PartTimeDay decimal(24,9)
--	,HourlyDay decimal(24,9)
--	-- ʵ��.���˾�Ч��
--	,FactEfficiency decimal(24,9)
	
--)

select 
	-- ����
	Region = IsNull(region.ID,-1)
	,RegionCode = IsNull(region.Code,'')
	,RegionName = IsNull(regionTrl.Name,'')
	-- ����
	,Region2 = IsNull(region2.ID,-1)
	,Region2Code = IsNull(region2.Code,'')
	,Region2Name = IsNull(region2Trl.Name,'')
	-- ����
	,Department = IsNull(dept.ID,-1)
	,DepartmentCode = IsNull(dept.Code,'')
	,DepartmentName = IsNull(deptTrl.Name,'')

	,checkinSummary.CheckInDate
	,checkinSummary.DisplayDate
	,checkinSummary.StatisticsPeriod
	--,checkin.Status as Status
	--,checkin.CurrentOperator as CurrentOperator

	--,checkinLine.StaffMember as Staff
	--,person.PersonID as StaffCode
	--,person.Name as StaffName
		
	-- Ԥ��
	-- ���� = �ͱ� * �Ͳ�����
	,ForecastHolidayDayIncome = Warning.Income
	-- �ճ���Сʱ��
	,ForecastAttendance = Warning.AttendanceTime
	-- �����˹�����
	,ForecastWage = Warning.Wage
	-- Ԥ��.���˾�Ч�� = ���� / ��������
	,ForecastEfficiency = case when Warning.AttendanceTime is not null and Warning.AttendanceTime != 0
							then IsNull(Warning.Income,0) / Warning.AttendanceTime
							else 0 end
	
	-- ����
	,FactHolidayDayIncome = (checkinSummary.Income)
	-- �ճ������� = ��ȫ����Ա�����ںϼ�/4 + �ӵ㹤Ա�����ںϼ�/8 + ȫ����Ա�����ںϼ� 
	,FactAttendance = checkinSummary.PersonTime
	-- ʵ��.���˾�Ч�� =  ���� / ��������
	,FactEfficiency = case when checkinSummary.PersonTime is not null and checkinSummary.PersonTime != 0
							then IsNull(checkinSummary.Income,0) / checkinSummary.PersonTime
							else 0 end

	-- ȫ����
	,FullTimeDay = (IsNull(checkinSummary.FullTimeDay,@DefaultZero))
	-- ��ȫ����
	,PartTimeDay = (IsNull(checkinSummary.PartTimeDay,@DefaultZero))
	-- �ӵ㹤
	,HourlyDay = (IsNull(checkinSummary.HourlyDay,@DefaultZero))
	
from (
	select 
		---- ����
		--Department = IsNull(dept.ID,-1)
		--,DepartmentCode = IsNull(dept.Code,'')
		--,DepartmentName = IsNull(deptTrl.Name,'')
		Department = employee.Dept

		,checkin.CheckInDate
		,DisplayDate = Right('00' + DateName(Month,checkin.CheckInDate),2) + '.' + Right('00' + DateName(day,checkin.CheckInDate),2)
		,StatisticsPeriod = Right('0000' + DateName(year,checkin.CheckInDate),4) + '��' + Right('00' + DateName(month,checkin.CheckInDate),2) + '��'
			
		-- ȫ����
		,FullTimeDay = Sum(IsNull(checkinLine.FullTimeDay,@DefaultZero))
		-- ��ȫ����
		,PartTimeDay = Sum(IsNull(checkinLine.PartTimeDay,@DefaultZero))
		-- �ӵ㹤
		,HourlyDay = Sum(IsNull(checkinLine.HourlyDay,@DefaultZero))
	
		-- ����
		,Income = max(IsNull(checkin.Income,@DefaultZero))
		---- ����
		--,Region = IsNull(region.ID,-1)
		--,RegionCode = IsNull(region.Code,'')
		--,RegionName = IsNull(regionTrl.Name,'')
		---- ����
		--,Region2 = IsNull(region2.ID,-1)
		--,Region2Code = IsNull(region2.Code,'')
		--,Region2Name = IsNull(region2Trl.Name,'')
		
		-- �ճ������� = ��ȫ����Ա�����ںϼ�/4 + �ӵ㹤Ա�����ںϼ�/8 + ȫ����Ա�����ںϼ� 
		,PersonTime = Sum(IsNull(checkinLine.PartTimeDay,@DefaultZero)) / 4 + Sum(IsNull(checkinLine.HourlyDay,@DefaultZero)) / 8 + Sum(IsNull(checkinLine.FullTimeDay,@DefaultZero))
	
	from 		
		[10.28.76.125].U9.dbo.CBO_EmployeeArchive employee	
		
		left join [10.28.76.125].U9.dbo.Cust_DayCheckInLine checkinLine
		on checkinLine.EmployeeArchive = employee.ID
		left join [10.28.76.125].U9.dbo.Cust_DayCheckIn checkin
		on checkin.ID = checkinLine.DayCheckIn
	where 
		employee.Dept is not null
	group by 
		--IsNull(dept.ID,-1)
		--,IsNull(dept.Code,'')
		--,IsNull(deptTrl.Name,'')
		employee.Dept

		,checkin.CheckInDate
		-- ,checkinLine.EmployeeArchive
		---- ����
		--,IsNull(region.ID,-1)
		--,IsNull(region.Code,'')
		--,IsNull(regionTrl.Name,'')
		--,IsNull(region2.ID,-1)
		--,IsNull(region2.Code,'')
		--,IsNull(region2Trl.Name,'')
	) as checkinSummary

	full join (
			select 
				warning.Department
				,warningLine.Date
				-- ���� = �ͱ� * �Ͳ�����
				,Income = Sum(MealStandard * EstimatedQty)
				-- �ճ���Сʱ��
				,AttendanceTime = Max(AttendanceTime)
				-- �����˹�����
				,Wage = Max(Wage)
			from [10.28.76.125].U9.dbo.Cust_CostWarning warning
				inner join [10.28.76.125].U9.dbo.Cust_CostWarningLine warningLine
				on warning.ID = warningLine.CostWarning
			group by
				warning.Department
				,warningLine.Date
			) as Warning
	on checkinSummary.Department = Warning.Department
		and checkinSummary.CheckInDate = Warning.Date
		

	
	left join [10.28.76.125].U9.dbo.CBO_Department dept
	-- on checkin.Department = dept.ID
	on dept.ID = IsNull(checkinSummary.Department,Warning.Department)

	left join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl
	on deptTrl.ID = dept.ID
		and deptTrl.SysMLFlag = 'zh-CN'
	 
	left join [10.28.76.125].U9.dbo.CBO_Department region
	on SubString(dept.Code,1,5) = region.Code
	left join [10.28.76.125].U9.dbo.CBO_Department_Trl regionTrl
	on regionTrl.ID = region.ID
		and regionTrl.SysMLFlag = 'zh-CN'
	 
	left join [10.28.76.125].U9.dbo.CBO_Department region2
	on SubString(dept.Code,1,7) = region2.Code
	left join [10.28.76.125].U9.dbo.CBO_Department_Trl region2Trl
	on region2Trl.ID = region2.ID
		and region2Trl.SysMLFlag = 'zh-CN'




select *
from Fact_U9_HolidayAttendance
where (@��ѡ��������� is null or @��ѡ��������� = ''
		or @��ѡ��������� = StatisticsPeriod
		)
	and (@��ѡ����� is null or @��ѡ����� = ''
		or @��ѡ����� = RegionName
		)
	and (@��ѡ������ is null or @��ѡ������ = ''
		or @��ѡ������ = Region2Name
		)
	and (@��ѡ���� is null or @��ѡ���� = ''
		or @��ѡ���� = DepartmentName
		)


