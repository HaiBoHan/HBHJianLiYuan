
/*
select *
from HBH_SPParamRecord
order by CreatedOn desc
*/

if exists(select * from sys.objects where name='HBH_BASP_JianLiYuan_HolidayAttendance')
-- ���������ɾ��
	drop proc HBH_BASP_JianLiYuan_HolidayAttendance
go
-- ����ʵ��Ч��ͳ�Ʊ�
-- �����洢����
create proc HBH_BASP_JianLiYuan_HolidayAttendance  (
-- @StartDate datetime = null
-- ,@EndDate datetime = null

@��ѡ��������� varchar(125) = ''
,@��ѡ����� varchar(125) = ''
,@��ѡ������ varchar(125) = ''
,@��ѡ���� varchar(125) = ''
,@��ѡ��ʼ���� varchar(125) = ''
,@��ѡ��������� varchar(125) = ''
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
		union select 'HBH_BASP_JianLiYuan_HolidayAttendance','@��ѡ��ʼ����',IsNull(@��ѡ��ʼ����,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_HolidayAttendance','@��ѡ���������',IsNull(@��ѡ���������,'null'),GETDATE()

		union select 'HBH_BASP_JianLiYuan_HolidayAttendance','ProcSql','exec HBH_BASP_JianLiYuan_HolidayAttendance '
				+ IsNull('''' + @��ѡ��������� + '''' ,'null')
				+ ',' + IsNull('''' + @��ѡ����� + '''' ,'null')
				+ ',' + IsNull('''' + @��ѡ������ + '''' ,'null')
				+ ',' + IsNull('''' + @��ѡ���� + '''' ,'null')
				+ ',' + IsNull('''' + @��ѡ��ʼ���� + '''' ,'null')
				+ ',' + IsNull('''' + @��ѡ��������� + '''' ,'null')

			   ,GETDATE()
	end
end

	declare @SysMlFlag varchar(11) = 'zh-CN'
	declare @DefaultZero decimal(24,9) = 0
	declare @Now datetime = GetDate();
	declare @CurDate datetime = GetDate()
	declare @Today datetime = convert(varchar(10), GetDate(), 120)
	declare @StartDate datetime
	declare @EndDate datetime
	-- ��ѡ�ָ����
	declare @Separator varchar(2) = ';'


if(@��ѡ��������� is null or @��ѡ��������� = '')
begin
	select @StartDate=max(dateStart.DayDate)
	from Dim_U9_Date_Filter dateStart 
	where dateStart.DayName = @��ѡ��ʼ����

	select @EndDate = max(dateStart.DayDate)
	from Dim_U9_Date_Filter dateStart 
	where dateStart.DayName = @��ѡ���������

end
else
begin

	set @StartDate = cast((replace(replace(@��ѡ���������,'��','-'),'��','-') + '01') as datetime)
	-- �¸���1�ż�һ��
	set @EndDate = DateAdd(day,-1,DateAdd(Month,1,@StartDate))
	
end



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


-- ��ѡ���š�����

-- ����
If OBJECT_ID('tempdb..#hbh_tmp_Department') is not null
	Drop Table #hbh_tmp_Department

select Item
into #hbh_tmp_Department
from dbo.HBH_Fn_StrSplitToTable(@��ѡ����,@Separator,0)

-- ����
If OBJECT_ID('tempdb..#hbh_tmp_Region') is not null
	Drop Table #hbh_tmp_Region

select Item
into #hbh_tmp_Region
from dbo.HBH_Fn_StrSplitToTable(@��ѡ������,@Separator,0)

-- ����
If OBJECT_ID('tempdb..#hbh_tmp_BigRegion') is not null
	Drop Table #hbh_tmp_BigRegion

select Item
into #hbh_tmp_BigRegion
from dbo.HBH_Fn_StrSplitToTable(@��ѡ�����,@Separator,0)



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
--insert into Fact_U9_HolidayAttendance
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



/*
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

*/


;
with tmp_Cust_CostWarning -- (Department,Date,Income,AttendanceTime,Wage)
as 
(	select 
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
		
		left join [10.28.76.125].U9.dbo.CBO_Department dept
		-- on checkin.Department = dept.ID
		on dept.ID = warning.Department

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

	where 1=1
		--and (@��ѡ��ʼ���� is null or @��ѡ��ʼ���� = ''
		--	or warningLine.Date >= (select max(dateStart.DayDate) from Dim_U9_Date_Filter dateStart where dateStart.DayName = @��ѡ��ʼ����)
		--	)
		--and (@��ѡ��������� is null or @��ѡ��������� = ''
		--	or warningLine.Date <= (select max(dateEnd.DayDate) from Dim_U9_Date_Filter dateEnd where dateEnd.DayName = @��ѡ���������)
		--	)

		and warningLine.Date >= @StartDate
		and warningLine.Date <= @EndDate

		and (@��ѡ����� is null or @��ѡ����� = ''
			-- or @��ѡ����� = 
			or regionTrl.Name in (select Item from #hbh_tmp_BigRegion )
			)
		and (@��ѡ������ is null or @��ѡ������ = ''
			--or @��ѡ������ = region2Trl.Name
			or region2Trl.Name in (select Item from #hbh_tmp_Region )
			)
		and (@��ѡ���� is null or @��ѡ���� = ''
			--or @��ѡ���� = deptTrl.Name
			or deptTrl.Name in (select Item from #hbh_tmp_Department )
			)
	group by
		warning.Department
		,warningLine.Date
)
insert into Fact_U9_HolidayAttendance

select 
	-- ����
	Region = IsNull(region.ID,-1)
	,RegionCode = IsNull(region.Code,'')
	,RegionName = IsNull(regionTrl.Name,'')
	--,RegionName = '(' + IsNull(region.Code,'') + ')' + IsNull(regionTrl.Name,'')
	-- ����
	,Region2 = IsNull(region2.ID,-1)
	,Region2Code = IsNull(region2.Code,'')
	,Region2Name = IsNull(region2Trl.Name,'')
	--,RegionName = '(' + IsNull(region.Code,'') + ')' + IsNull(regionTrl.Name,'')
	-- ����
	,Department = IsNull(dept.ID,-1)
	,DepartmentCode = IsNull(dept.Code,'')
	,DepartmentName = IsNull(deptTrl.Name,'')
	--,DepartmentName = '(' + IsNull(dept.Code,'') + ')' + IsNull(deptTrl.Name,'')

	,IsNull(Warning.Date,checkinSummary.CheckInDate) as CheckInDate
	--,checkinSummary.DisplayDate
	--,checkinSummary.StatisticsPeriod
	,DisplayDate = Right('00' + DateName(Month,IsNull(Warning.Date,checkinSummary.CheckInDate)),2) + '.' + Right('00' + DateName(day,IsNull(Warning.Date,checkinSummary.CheckInDate)),2)
	,StatisticsPeriod = Right('0000' + DateName(year,IsNull(Warning.Date,checkinSummary.CheckInDate)),4) + '��' + Right('00' + DateName(month,IsNull(Warning.Date,checkinSummary.CheckInDate)),2) + '��'
			
	--,checkin.Status as Status
	--,checkin.CurrentOperator as CurrentOperator

	--,checkinLine.StaffMember as Staff
	--,person.PersonID as StaffCode
	--,person.Name as StaffName
		
	-- Ԥ��
	-- ���� = �ͱ� * �Ͳ�����
	,ForecastHolidayDayIncome = Warning.Income
	-- ���ճ������� = �ճ���Сʱ�� / 8 
	,ForecastAttendance = Warning.AttendanceTime / 8
	-- �����˹�����
	,ForecastWage = Warning.Wage
	-- Ԥ��.���˾�Ч�� = ���� / ��������
	,ForecastEfficiency = case when Warning.AttendanceTime is not null and Warning.AttendanceTime != 0
							then IsNull(Warning.Income,0) * 8 / Warning.AttendanceTime
							else 0 end
	
	-- ����
	,FactHolidayDayIncome = IsNull(checkinSummary.Income,@DefaultZero)
	-- �ճ������� = ��ȫ����Ա�����ںϼ�/4 + �ӵ㹤Ա�����ںϼ�/8 + ȫ����Ա�����ںϼ� 
	,FactAttendance = IsNull(checkinSummary.PersonTime,@DefaultZero)
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
	
from 
	tmp_Cust_CostWarning  as Warning

	left join (
			select 
				---- ����
				--Department = IsNull(dept.ID,-1)
				--,DepartmentCode = IsNull(dept.Code,'')
				--,DepartmentName = IsNull(deptTrl.Name,'')
				-- Department = employee.Dept
				Department = IsNull(checkin.Department,-1)

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
				[10.28.76.125].U9.dbo.Cust_DayCheckIn checkin
				inner join [10.28.76.125].U9.dbo.Cust_DayCheckInLine checkinLine
				on checkin.ID = checkinLine.DayCheckIn

				inner join [10.28.76.125].U9.dbo.CBO_EmployeeArchive employee			
				on checkinLine.EmployeeArchive = employee.ID
			where 
				-- checkin.Department in (select warn.Department from tmp_Cust_CostWarning warn)
				exists( select 1
						from tmp_Cust_CostWarning warn
						where warn.Department = checkin.Department
							and checkin.CheckInDate = warn.Date
						 )
				
				/* -- ״̬
				Approved	�����	2
				Approving	�����	1
				Closed	�ѹر�	3
				Opened	����	0
				*/
				and checkin.Status = 2
			group by 
				--IsNull(dept.ID,-1)
				--,IsNull(dept.Code,'')
				--,IsNull(deptTrl.Name,'')
				-- employee.Dept
				checkin.Department

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
	on checkinSummary.Department = Warning.Department
		and checkinSummary.CheckInDate = Warning.Date		

	
	left join [10.28.76.125].U9.dbo.CBO_Department dept
	-- on checkin.Department = dept.ID
	on dept.ID = IsNull(Warning.Department,checkinSummary.Department)

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
from Fact_U9_HolidayAttendance holiday
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
	and (@��ѡ��ʼ���� is null or @��ѡ��ʼ���� = ''
		or CheckInDate >= (select max(dateStart.DayDate) from Dim_U9_Date_Filter dateStart where dateStart.DayName = @��ѡ��ʼ����)
		)
	and (@��ѡ��������� is null or @��ѡ��������� = ''
		or CheckInDate <= (select max(dateEnd.DayDate) from Dim_U9_Date_Filter dateEnd where dateEnd.DayName = @��ѡ���������)
		)
