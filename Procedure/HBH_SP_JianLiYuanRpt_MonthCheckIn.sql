
if exists(select * from sys.objects where name='HBH_SP_JianLiYuanRpt_MonthCheckIn')
-- ���������ɾ��
	drop proc HBH_SP_JianLiYuanRpt_MonthCheckIn
go
-- �����洢����
create proc HBH_SP_JianLiYuanRpt_MonthCheckIn  (
-- @StartDate datetime = null
-- ,@EndDate datetime = null

@SalaryPeriod bigint = -1
,@StartDate datetime = null
,@EndDate datetime = null

,@Department bigint = -1

,@IsDetail varchar(125) = '1'

-- @PlanDate datetime = null
--,@ShipLineID bigint =-1
--,@LotCode varchar(125) = ''
--,@ItemSpec varchar(125) = ''
--,@SalesmanCode varchar(125) = ''
----,@IsAllSalesman smallint = 0
--,@IsFuzzySalesman smallint = 0
--,@IsContainBranchWh smallint = 0
--,@InvCategory bigint = -1
--,@ItemCode varchar(125) = ''
--,@ItemName varchar(125) = ''
----,@IsForceSalesman smallint = 0
--,@IsShowZeroQty smallint = 0
--,@Branch bigint = -1
)
with encryption
as
	SET NOCOUNT ON;

if exists(select name from sys.objects where name = 'HBH_Debug_Param')
begin
	declare @Debugger bit = (select top 1 Debugger from HBH_Debug_Param where ProcName = 'HBH_SP_JianLiYuanRpt_MonthCheckIn' or ProcName is null or ProcName = '' order by ProcName desc)
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
		select 'HBH_SP_JianLiYuan_DepartImport','@SalaryPeriod',IsNull(cast(@SalaryPeriod as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuanRpt_MonthCheckIn','@StartDate',IsNull(Convert(varchar,@StartDate,120),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuanRpt_MonthCheckIn','@EndDate',IsNull(Convert(varchar,@EndDate,120),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_DepartImport','@Department',IsNull(cast(@Department as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_DepartImport','@IsDetail',IsNull(cast(@IsDetail as varchar(max)),'null'),GETDATE()
		--union select 'HBH_SP_JianLiYuanRpt_MonthCheckIn','@IsCalcAll',IsNull(cast(@IsCalcAll as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuanRpt_MonthCheckIn','ProcSql','exec HBH_SP_JianLiYuanRpt_MonthCheckIn '
				+ IsNull('''' + cast(@SalaryPeriod as varchar(501)) + '''' ,'null')
				+ ',' + IsNull('''' + Convert(varchar,@StartDate,120) + '''' ,'null')
				+ ',' + IsNull('''' + Convert(varchar,@EndDate,120) + '''' ,'null')
				+ ',' + IsNull('''' + cast(@Department as varchar(501)) + '''' ,'null')
				--+ IsNull(cast(@IsCalcAll as varchar(501)),'null') 
				+ ',' + IsNull('''' + cast(@IsDetail as varchar(501)) + '''' ,'null')
			   ,GETDATE()
	end
end



	

	declare @SysMlFlag varchar(11) = 'zh-CN'
	declare @DefaultZero decimal(24,9) = 0
	--declare @SalePriceListCode varchar(125) = '001'
	declare @SysLineNo int = 10
	declare @Now datetime = GetDate();
	declare @CurDate datetime = GetDate()
	declare @SheetName varchar(125)
	declare @StartID bigint = -1
	declare @TotalIDCount int = 0
	declare @TotalLineCount int = 0
	declare @DetailLineCount int = 0


if(@IsDetail is null)
begin
	set @IsDetail = '1'
end

	
	--select @SysLineNo=cast(isnull(b.Value,a.DefaultValue) as int)
	--from Base_Profile a
	--left join Base_ProfileValue b on b.Profile=a.ID 
	--where Code='SysLineNo'

	--select @CurDate = CheckInDate
	--from [Cust_DayCheckIn]
	--where ID = @PayrollDoc
	
If OBJECT_ID('tempdb..#hbh_tmp_rpt_DayCheckInLine') is not null
	Drop Table #hbh_tmp_rpt_DayCheckInLine


select 
	Department
	,DepartmentCode
	,DepartmentName
	-- FullTimeStaff	ȫ���Ƴ���	0
	-- PartTimeStaff	��ȫ���Ƴ���	1
	-- HourlyStaff		�ӵ㹤����	2
	,CheckType
	,EmployeeArchive
	,EmployeeCode
	,EmployeeName

	,CheckInDate
	,CheckInMonth
	,CheckInDay

	-- ��������	
	,MonthDays = (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end)
	
	--,sum(checkInLine.FullTimeDay) as FullTimeDay
	--,sum(checkInLine.PartTimeDay) as PartTimeDay
	--,sum(checkInLine.HourlyDay) as HourlyDay
	--,min(checkIn.CheckInDate) as CheckInDate

	,FullTimeDay
	,PartTimeDay
	,HourlyDay

	
	-- ȫ���Ʊ�׼����=�������ʣ�01��+��ĩ�Ӱ๤�ʣ�02��+�绰������03��+��ͨ����(04)+��Ͳ�����05��+ְ������07��
	-- 2017-01-10 wf  �ֳ����޸ĳɣ�(�������ʣ�01�� + ��ĩ�Ӱ๤�ʣ�02��)���ĳ��� ��׼����.(14)
	,StardardSalary		
	-- F�ӵ㹤���ʱ�׼��F01�� = �ӵ㹤���ʱ�׼(F01)			-- (F13)
	,FPartSalary
	-- �Ӱ๤��
	-- �ӵ㹤���ʱ�׼ = �ӵ㹤���ʱ�׼(06)			-- (038)
	,OvertimeSalary
	-- F�Ӱ๤��
	-- FJ�ӵ㹤���ʱ�׼��F06��=н����Ŀ��F���ʱ�׼��Ŀ(F06)					-- ��F54��
	,FOvertimeSalary

	
		 --�չ���=��׼����/Ӧ�������� *ȫ����Ա������
   --               +�ӵ㹤���ʱ�׼        *�ӵ㹤����
   --               +FJ�ӵ㹤���ʱ�׼     *�ӵ㹤����
   --               +F�ӵ㹤���ʱ�׼      *��ȫ����Ա������
	,Salary = 
		-- ȫ����Ա������
		(StardardSalary / (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end) * IsNull(FullTimeDay,@DefaultZero) 
		-- ��ȫ����Ա������
		+ IsNull(FPartSalary,@DefaultZero) * IsNull(PartTimeDay,@DefaultZero) 
		-- ȫ���ƼӰ๤��
		+ IsNull(OvertimeSalary,@DefaultZero) * IsNull(HourlyDay,@DefaultZero) 
		-- ��ȫ���ƼӰ๤��
		+ IsNull(FOvertimeSalary,@DefaultZero) * IsNull(HourlyDay,@DefaultZero))

	-- �ձ���
	,DayInsurance = -- Sum
			(
			-- ȫ����Ա������
			(IsNull(InsuranceSalary,@DefaultZero) / (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end) * IsNull(FullTimeDay,@DefaultZero) 
			-- ��ȫ����Ա������
			+ (IsNull(FInsuranceSalary,@DefaultZero) / (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end)) * IsNull(PartTimeDay,@DefaultZero) )
			)

into #hbh_tmp_rpt_DayCheckInLine
from (
	select 
		checkIn.Department as Department
		,dept.Code as DepartmentCode
		,deptTrl.Name as DepartmentName
		-- FullTimeStaff	ȫ���Ƴ���	0
		-- PartTimeStaff	��ȫ���Ƴ���	1
		-- HourlyStaff		�ӵ㹤����	2
		,case when checkInLine.CheckType = 0 
			then 'ȫ���Ƴ���'
			when checkInLine.CheckType = 1
			then '��ȫ���Ƴ���'
			when checkInLine.CheckType = 2
			then '�ӵ㹤����'
			else cast(checkInLine.CheckType as varchar(125))
			end
		 as CheckType
		,checkInLine.EmployeeArchive as EmployeeArchive
		,employee.EmployeeCode as EmployeeCode
		,employee.Name as EmployeeName

		,Convert(varchar(10),checkIn.CheckInDate,120)  as CheckInDate
		,Month(checkIn.CheckInDate) as CheckInMonth
		,Day(checkIn.CheckInDate) as CheckInDay

	
		--,sum(checkInLine.FullTimeDay) as FullTimeDay
		--,sum(checkInLine.PartTimeDay) as PartTimeDay
		--,sum(checkInLine.HourlyDay) as HourlyDay
		--,min(checkIn.CheckInDate) as CheckInDate

		,IsNull(checkInLine.FullTimeDay,0) as FullTimeDay
		,IsNull(checkInLine.PartTimeDay,0) as PartTimeDay
		,IsNull(checkInLine.HourlyDay,0) as HourlyDay

		---- ��������	
		-- Ӧ�������� = �������� - 4
		-- ��Ϊ���տ�����¼��
		,MonthDays = IsNull(Day(DateAdd(Day,-1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)) + 1,DateAdd(M,1,checkin.CheckInDate)))),27)  - 4
		-- ��Ϊ���տ�����¼��
		,IsNull((select max(IsNull(checkin2.MonthWorkDays,0)) 
					from Cust_DayCheckIn checkin2
					where checkin2.Department = checkIn.Department)
				,0) as MonthWorkDays
		
		-- ȫ���Ʊ�׼����=�������ʣ�01��+��ĩ�Ӱ๤�ʣ�02��+�绰������03��+��ͨ����(04)+��Ͳ�����05��+ְ������07��
		-- 2017-01-10 wf  �ֳ����޸ĳɣ�(�������ʣ�01�� + ��ĩ�Ӱ๤�ʣ�02��)���ĳ��� ��׼����.(14)
		,StardardSalary = Sum(dbo.HBH_Fn_GetDecimal(
				--case when salaryItem.Code in ('01','02','03','04','05','07') 
				case when salaryItem.Code in ('14','03','04','05','07')
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
		-- FJ�ӵ㹤���ʱ�׼��F06��=н����Ŀ��F���ʱ�׼��Ŀ(F06)					-- ��F54��
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

	from Cust_DayCheckIn checkIn
		inner join Cust_DayCheckInLine checkInLine
		on checkIn.ID = checkInLine.DayCheckIn

		left join CBO_EmployeeArchive employee
		on checkInLine.EmployeeArchive = employee.ID

		left join PAY_PlanPeriod period
		on period.ID = @SalaryPeriod

		left join CBO_Department dept
		on checkIn.Department = dept.ID
		left join CBO_Department_Trl deptTrl
		on deptTrl.ID = dept.ID and deptTrl.SysMLFlag = @SysMlFlag

	
		--left join CBO_Person person
		--on checkinLine.StaffMember = person.ID
		left join CBO_EmployeeSalaryFile salary
		on salary.Employee = employee.ID
		left join CBO_PublicSalaryItem salaryItem
		on salary.SalaryItem = salaryItem.ID
		left join CBO_PublicSalaryItem_Trl salaryItemTrl
		on salaryItemTrl.ID = salaryItem.ID 
			and salaryItemTrl.SysMLFlag = 'zh-CN'

		--left join (select max(IsNull(checkin2.MonthWorkDays,0)) MonthWorkDays
		--				,checkin2.Department as Department
		--			from Cust_DayCheckIn checkin2
		--			where checkin2.Department = checkIn.Department
		--			) allCheckIn
		--on allCheckIn.Department = checkIn.Department

	where 1=1
		and ( @SalaryPeriod is null or @SalaryPeriod <= 0
			or checkIn.CheckInDate between period.StartDate and period.EndDate
			)
			--or checkIn.CheckInDate between @StartDate and @EndDate
		and (@StartDate is null or @StartDate < '2000-01-01'
			or checkIn.CheckInDate >= @StartDate
			)
		and (@EndDate is null or @EndDate < '2000-01-01'
			or checkIn.CheckInDate <= @EndDate
			)
		and (@Department is null or @Department <= 0
			or @Department = checkIn.Department
			)

	group by
		checkIn.Department
		,dept.Code
		,deptTrl.Name
		,checkIn.CheckInDate
		,checkInLine.EmployeeArchive
		,checkInLine.CheckType
		,employee.EmployeeCode
		,employee.Name
		,checkIn.CheckInDate
		,IsNull(checkInLine.FullTimeDay,0)
		,IsNull(checkInLine.PartTimeDay,0)
		,IsNull(checkInLine.HourlyDay,0)
	) checkData
	
	
;


--with checkSummary 
--as(		
--	select 
--		Department
--		,CheckType
--		,EmployeeArchive
--		,sum(FullTimeDay) as SumFullTimeDay
--		,sum(PartTimeDay) as SumPartTimeDay
--		,sum(HourlyDay) as SumHourlyDay
--	from #hbh_tmp_rpt_DayCheckInLine
--	group by
--		Department
--		,CheckType
--		,EmployeeArchive
--	)
--select
--	checkLine.*
--	,summary.SumFullTimeDay
--	,summary.SumPartTimeDay
--	,summary.SumHourlyDay
--from #hbh_tmp_rpt_DayCheckInLine checkLine
--	left join checkSummary summary
--	on checkLine.Department = summary.Department
--		and checkLine.CheckType = summary.CheckType
--		and checkLine.EmployeeArchive = summary.EmployeeArchive



if(@IsDetail = '1')
begin

	with checkSummary 
	as(		
		select 
			Department
			,CheckType
			,EmployeeArchive
			,sum(FullTimeDay) as SumFullTimeDay
			,sum(PartTimeDay) as SumPartTimeDay
			,sum(HourlyDay) as SumHourlyDay
			-- Ա���չ���
			 --�չ���=��׼����/Ӧ�������� *ȫ����Ա������
	   --               +�ӵ㹤���ʱ�׼        *�ӵ㹤����
	   --               +FJ�ӵ㹤���ʱ�׼     *�ӵ㹤����
	   --               +F�ӵ㹤���ʱ�׼      *��ȫ����Ա������
			-- 27 = �������� - 4
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

		from #hbh_tmp_rpt_DayCheckInLine
		group by
			Department
			,CheckType
			,EmployeeArchive
		)
	select
		checkLine.*
		,summary.SumFullTimeDay
		,summary.SumPartTimeDay
		,summary.SumHourlyDay
	from #hbh_tmp_rpt_DayCheckInLine checkLine
		left join checkSummary summary
		on checkLine.Department = summary.Department
			and checkLine.CheckType = summary.CheckType
			and checkLine.EmployeeArchive = summary.EmployeeArchive
	
	order by
		checkLine.EmployeeCode
		,checkLine.EmployeeName
		,checkLine.CheckInDate
			
print('Detail')

end else
begin

	select 
		--Department
		--,CheckType
		--,EmployeeArchive
		--,sum(FullTimeDay) as SumFullTimeDay
		--,sum(PartTimeDay) as SumPartTimeDay
		--,sum(HourlyDay) as SumHourlyDay
		
		Department
		,DepartmentCode
		,DepartmentName
		,CheckType
		,EmployeeArchive
		,EmployeeCode
		,EmployeeName

		,null as CheckInDate
		,null as CheckInMonth
		,null as CheckInDay
		
		,0 as FullTimeDay
		,0 as PartTimeDay
		,0 as HourlyDay

		,sum(FullTimeDay) as SumFullTimeDay
		,sum(PartTimeDay) as SumPartTimeDay
		,sum(HourlyDay) as SumHourlyDay
		
		,StardardSalary
		,FPartSalary
		,OvertimeSalary
		,FOvertimeSalary
		
		-- Ա���չ���
		 --�չ���=��׼����/Ӧ�������� *ȫ����Ա������
   --               +�ӵ㹤���ʱ�׼        *�ӵ㹤����
   --               +FJ�ӵ㹤���ʱ�׼     *�ӵ㹤����
   --               +F�ӵ㹤���ʱ�׼      *��ȫ����Ա������
		-- 27 = �������� - 4
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
		,DayInsurance
	from #hbh_tmp_rpt_DayCheckInLine
	group by
		Department
		,DepartmentCode
		,DepartmentName
		,CheckType
		,EmployeeArchive
		,EmployeeCode
		,EmployeeName
		,StardardSalary
		,PartSalary
		,OvertimeSalary

		-- ,CheckInDate
		-- ,CheckInMonth
		-- ,CheckInDay

		-- Ӧ�������� = �������� - 4
		,MonthDays
		-- �ձ���
		,DayInsurance
	order by
		EmployeeCode
		,EmployeeName
		,CheckInDate
		
print('Summary')

end




