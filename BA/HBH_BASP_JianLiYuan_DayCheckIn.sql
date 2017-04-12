

/*
select *
from HBH_SPParamRecord
order by CreatedOn desc
*/

if exists(select * from sys.objects where name='HBH_BASP_JianLiYuan_DayCheckIn')
-- ���������ɾ��
	drop proc HBH_BASP_JianLiYuan_DayCheckIn
go
-- �Ͳ��ʱ���
-- �����洢����
create proc HBH_BASP_JianLiYuan_DayCheckIn  (
-- @StartDate datetime = null
-- ,@EndDate datetime = null

@��ѡ��������� varchar(125) = ''
,@��ѡ����� varchar(125) = ''
,@��ѡ������ varchar(125) = ''
,@��ѡ���� varchar(125) = ''
,@��ѡ��ʼ���� varchar(125) = ''
,@��ѡ��������� varchar(125) = ''
,@�쵼�ñ� varchar(125) = ''
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
		union select 'HBH_BASP_JianLiYuan_DayCheckIn','@��ѡ������',IsNull(@��ѡ������,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_DayCheckIn','@��ѡ����',IsNull(@��ѡ����,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_DayCheckIn','@��ѡ��ʼ����',IsNull(@��ѡ��ʼ����,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_DayCheckIn','@��ѡ���������',IsNull(@��ѡ���������,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_DayCheckIn','@�쵼�ñ�',IsNull(@�쵼�ñ�,'null'),GETDATE()

		union select 'HBH_BASP_JianLiYuan_DayCheckIn','ProcSql','exec HBH_BASP_JianLiYuan_DayCheckIn '
				+ IsNull('''' + @��ѡ��������� + '''' ,'null')
				+ ',' + IsNull('''' + @��ѡ����� + '''' ,'null')
				+ ',' + IsNull('''' + @��ѡ������ + '''' ,'null')
				+ ',' + IsNull('''' + @��ѡ���� + '''' ,'null')
				+ ',' + IsNull('''' + @��ѡ��ʼ���� + '''' ,'null')
				+ ',' + IsNull('''' + @��ѡ��������� + '''' ,'null')
				+ ',' + IsNull('''' + @�쵼�ñ� + '''' ,'null')

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
	/*
	01	������Դ
	02	�ൺ����Դ
	03	��̨�ֹ�˾
	04	���Ͻ���Դ
	05	��������
	06	�ɹ�����
	07	��������Դ
	08	��������Դ
	*/
	declare @ManageOrgCode varchar(125) = '05'
	-- ��ѡ�ָ����
	declare @Separator varchar(2) = ';'
	
	-- ����ÿ�ܵ�һ��������(��һ)
	set datefirst 1

	

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
	inner join [10.28.76.125].U9.dbo.Base_Organization org
	on dept.Org = org.ID
where
	dept.Code != '00001'
	and Len(dept.Code) = 5
	-- �����ڵ�����
	and dept.ID not in (select region2.ID from Dim_U9_Department2 region2)
	-- ������֯
	and org.Code = @ManageOrgCode

-- ����
insert into Dim_U9_Department3
select dept.ID,dept.Code,deptTrl.Name,dept.Level+1 as Level,dept.Org
from [10.28.76.125].U9.dbo.CBO_Department dept 
	inner join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl 
	on dept.ID= deptTrl.ID and deptTrl.SysMLFlag='zh-CN'
	inner join [10.28.76.125].U9.dbo.Base_Organization org
	on dept.Org = org.ID
where
	Len(dept.Code) = 7
	-- �����ڵ�����
	and dept.ID not in (select region3.ID from Dim_U9_Department3 region3)
	-- ������֯
	and org.Code = @ManageOrgCode


---- ����
--insert into Dim_Department
--select dept.ID,dept.Code,deptTrl.Name,dept.Level+1 as Level,dept.Org
--from [10.28.76.125].U9.dbo.CBO_Department dept 
--	inner join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl 
--	on dept.ID= deptTrl.ID and deptTrl.SysMLFlag='zh-CN'
--	inner join [10.28.76.125].U9.dbo.Base_Organization org
--	on dept.Org = org.ID
--where
--	Len(dept.Code) = 10
--	-- �����ڵ�����
--	and dept.ID not in (select dept.ID from Dim_Department dept)
--	-- ������֯
--	and org.Code = @ManageOrgCode



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

-- ��ʵ��Fact_U9_DayCheckIn
-- ɾ��
truncate table Fact_U9_DayCheckIn
-- ����
insert into Fact_U9_DayCheckIn
--(
--	Department
--	,DepartmentCode
--	,DepartmentName
--	,DepartmentDisplayName varchar(200)
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
	-- ����
	Region
	,RegionCode
	,RegionName -- = '(' + RegionCode + ')' + RegionName
	-- ����
	,Region2 
	,Region2Code
	,Region2Name -- = '(' + Region2Code + ')' + Region2Name
	-- ����
	,Department 
	,DepartmentCode
	,DepartmentName -- = '(' + DepartmentCode + ')' + DepartmentName

	-- �����Ͳ��ʡ��˹��ɱ�Ŀ��� ������ʾ��
	/*��Ϊ  �������� | �Ͳ���Ŀ�� | �˹��ɱ�Ŀ��
��������,Ԥ��11���ֿռ�
�Ͳ���Ŀ�꣬Ԥ��5���ֿռ�
�˹��ɱ�Ŀ�꣬Ԥ��5���ֿռ�
	*/
	-- ����ַ���������varchar,������ռ2������ռ1; ����ַ���������nvarchar,�����ĺ����ֶ�ռ2
	-- Ĭ�ϲ��������  nvarchar
	-- ,DepartmentDisplayName = Left(IsNull(DepartmentName,'') + '                      ',22 - (datalength(cast(DepartmentName as varchar(125))) - len(DepartmentName))) + '|'
	,DepartmentDisplayName = Left(IsNull(DepartmentName,'') + replicate('	',11),11 + Ceiling((cast(len(DepartmentName) * 2 as decimal(3,1)) - cast(datalength(cast(DepartmentName as varchar(125))) as decimal(3,1))) / 2)  ) + '|'
		+ Left(LaborYieldTarget + replicate('	',5),5 + Ceiling((cast(len(LaborYieldTarget) * 2 as decimal(3,1)) - cast(datalength(cast(LaborYieldTarget as varchar(125))) as decimal(3,1))) / 2)  ) + '|'
		+ Left(LaborCostTarget + replicate('	',5),5 + Ceiling((cast(len(LaborCostTarget) * 2 as decimal(3,1)) - cast(datalength(cast(LaborCostTarget as varchar(125))) as decimal(3,1))) / 2))
		
	--Department
	--,DepartmentCode
	--,DepartmentName
	---- ����
	--,Region = Region
	--,RegionCode
	--,RegionName
	
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
	--,PersonTime = Sum(IsNull(PartTimeDay,@DefaultZero)) / 4 + Sum(IsNull(HourlyDay,@DefaultZero)) / 8 + Sum(IsNull(FullTimeDay,@DefaultZero))
	,PersonTime = Sum(PersonTime)
	
	-- ����(����ǰ��ܣ���ô����ÿ��)
	--,Income = Max(Income)
	,Income = Sum(Income)
	-- �Ͳ���Ŀ��
	--,LaborYieldTarget = dbo.HBH_Fn_GetString(Round(Max(LaborYieldTarget),0,0))
	,LaborYieldTarget = LaborYieldTarget
	-- �˹��ɱ�Ŀ��
	--,LaborCostTarget = dbo.HBH_Fn_GetString(Round(Max(LaborCostTarget),4,0) * 100) + '%'
	,LaborCostTarget = LaborCostTarget
	---- ��׼����
	--,StardardSalary = Sum(StardardSalary)
	
	-- Ա���չ���
	-- �չ���=��׼����/27 *ȫ����Ա������+�ӵ㹤���ʱ�׼(038)*�ӵ㹤����+��ȫ����Ա������*�ӱ�׼����		;�ӵ㹤���ʱ�׼ȡU9ά��н����Ŀ��ֵ�㹤���ʱ�׼
	-- 27 = �������� - 4
	--,Salary = Sum(StardardSalary / MonthDays * IsNull(FullTimeDay,@DefaultZero) + IsNull(PartTimeDay,@DefaultZero) * IsNull(PartSalary,@DefaultZero) + IsNull(HourlyDay,@DefaultZero) * IsNull(OvertimeSalary,@DefaultZero))
	--,Salary = Sum(
	--	-- ȫ����Ա������
	--	(StardardSalary / MonthDays * IsNull(FullTimeDay,@DefaultZero) 
	--	-- ��ȫ����Ա������
	--	+ IsNull(FPartSalary,@DefaultZero) * IsNull(PartTimeDay,@DefaultZero) 
	--	-- ȫ���ƼӰ๤��
	--	+ IsNull(OvertimeSalary,@DefaultZero) * IsNull(HourlyDay,@DefaultZero) 
	--	-- ��ȫ���ƼӰ๤��
	--	+ IsNull(FOvertimeSalary,@DefaultZero) * IsNull(HourlyDay,@DefaultZero))
	--	)
	,Salary = Sum(Salary)

	-- Ӧ�������� = �������� - 4
	,MonthDays  -- = (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end)
	-- �ձ���
	--,DayInsurance = Sum
	--		(
	--		-- ȫ����Ա������
	--		(IsNull(InsuranceSalary,@DefaultZero) / MonthDays * IsNull(FullTimeDay,@DefaultZero) 
	--		-- ��ȫ����Ա������
	--		+ (IsNull(FInsuranceSalary,@DefaultZero) / MonthDays) * IsNull(PartTimeDay,@DefaultZero) )
	--		)
	,DayInsurance = Sum(DayInsurance)
	
	-- �ճɱ��ϼ� = �չ��� + �ձ���
	,DayCost = Sum(Salary) + Sum(DayInsurance)

	-- �·ݵ�һ��
	,FirstDay
	-- �·ݵ�һ��  �ǵڼ���
	,FirstWeek
	-- �·ݵ�һ��  ���ܼ�
	,FirstWeekDay
		
	-- �·����һ��
	,LastDay
	-- �·����һ��  �ǵڼ���
	,LastWeek
	-- �·����һ��  ���ܼ�
	,LastWeekDay

from (	
		select 
			-- ����
			Region
			,RegionCode
			,RegionName
			-- ����
			,Region2
			,Region2Code
			,Region2Name
			-- ����
			,Department
			,DepartmentCode
			,DepartmentName

			---- Ա��
			--,EmployeeArchive
			---- ����
			--,Department = IsNull(dept.ID,-1)
			--,DepartmentCode = IsNull(dept.Code,'')
			--,DepartmentName = IsNull(deptTrl.Name,'')

			--,case when IsNull(@�쵼�ñ�,'') = '��' then DATEPART(WEEK,checkin.CheckInDate) - DATEPART(WEEK, DateAdd(M,-1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)) + 1,DateAdd(M,1,checkin.CheckInDate)))) + 1
			--	else convert(varchar(10),checkin.CheckInDate,23)
			--	end as CheckInDate
			--,CheckInDate = convert(varchar(10),checkin.CheckInDate,23)
			,CheckInDate
			,StatisticsPeriod
			--,DisplayDate
			-- ��ͨ�����죻�쵼�����ܣ�
			,DisplayDate = case when IsNull(@�쵼�ñ�,'') = '��' 
					--then Right(DateName(year,CheckInDate),2) + '��' + Right('00' + DateName(month,CheckInDate),2) + '��' + '��' + cast(IsNull(CheckMonthNumber,0) as char(1)) + '��(' 
					--	-- �ܵĵ��µ�һ��(��һ�ǵ��¼���)
					--	+ case when CheckMonthNumber = FirstWeek then '1' else cast(IsNull(Day(DateAdd(d,-CheckWeekDay + 1,CheckInDate)),0) as varchar(2)) end
					--	+ '-'
					--	-- �ܵĵ������һ��
					--	+ case when CheckMonthNumber = LastWeek then cast(IsNull(Day(LastDay),0) as varchar(2)) 
					--			else cast(IsNull(Day(DateAdd(d,7 - CheckWeekDay,CheckInDate)),0) as varchar(2)) end
					--	+ ')'
					then Right(CONVERT(varchar(100),case when WeekFirstDay > @StartDate then WeekFirstDay else @StartDate end, 2),8)
						+ '-'
						+ Right(CONVERT(varchar(100),case when @EndDate > WeekLastDay then WeekLastDay else @EndDate end, 2),8)
					else DisplayDate
					end
	
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
			--,LaborYieldTarget = Max(LaborYieldTarget)
			,LaborYieldTarget = dbo.HBH_Fn_GetString(Round(Max(LaborYieldTarget),0,0))
			-- �˹��ɱ�Ŀ��
			--,LaborCostTarget = Max(LaborCostTarget)
			,LaborCostTarget = dbo.HBH_Fn_GetString(Round(Max(LaborCostTarget),4,0) * 100) + '%'
			---- ��׼����
			--,StardardSalary = Sum(StardardSalary)
		
			-- Ա���չ���
			-- �չ���=��׼����/27 *ȫ����Ա������+�ӵ㹤���ʱ�׼(038)*�ӵ㹤����+��ȫ����Ա������*�ӱ�׼����		;�ӵ㹤���ʱ�׼ȡU9ά��н����Ŀ��ֵ�㹤���ʱ�׼
			-- 27 = �������� - 4
			--,Salary = Sum(StardardSalary / MonthDays * IsNull(FullTimeDay,@DefaultZero) + IsNull(PartTimeDay,@DefaultZero) * IsNull(PartSalary,@DefaultZero) + IsNull(HourlyDay,@DefaultZero) * IsNull(OvertimeSalary,@DefaultZero))
			,Salary = Sum(
				-- ȫ����Ա������
				(StardardSalary / (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end) * IsNull(FullTimeDay,@DefaultZero) 
				-- ��ȫ����Ա������
				+ IsNull(FPartSalary,@DefaultZero) * IsNull(PartTimeDay,@DefaultZero) 
				-- ȫ���ƼӰ๤��
				+ IsNull(OvertimeSalary,@DefaultZero) * IsNull(HourlyDay,@DefaultZero) 
				-- ��ȫ���ƼӰ๤��
				+ IsNull(FOvertimeSalary,@DefaultZero) * IsNull(HourlyDay,@DefaultZero))
				)

			---- ��������	
			-- Ӧ�������� = �������� - 4
			,MonthDays = (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end)
		
	
			---- ȫ���Ʊ�׼����=�������ʣ�01��+��ĩ�Ӱ๤�ʣ�02��+�绰������03��+��ͨ����(04)+��Ͳ�����05��+ְ������07��
			--,StardardSalary
					
			---- �Ӱ๤��
			---- �ӵ㹤���ʱ�׼ = �ӵ㹤���ʱ�׼(06)			-- (038)
			--,OvertimeSalary
		
			---- F�ӵ㹤���ʱ�׼��F01�� = �ӵ㹤���ʱ�׼(F01)			-- (F13)
			--,FPartSalary
			---- F�Ӱ๤��
			---- FJ�ӵ㹤���ʱ�׼��F06��=н����Ŀ��F���ʱ�׼��Ŀ(F06)					-- ��F56��
			--,FOvertimeSalary

			---- ��λ����
			---- ��λ���գ�12��=н����Ŀ�� ��λ����(12)					-- ��113��
			--,InsuranceSalary
			---- F��λ����
			---- F��λ���գ�F04��=н����Ŀ�� F��λ����(F04)					-- ��F52��
			--,FInsuranceSalary

			
			-- �ձ���
			,DayInsurance = Sum
					(
					---- ȫ����Ա������
					--(IsNull(InsuranceSalary,@DefaultZero) / (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end) * IsNull(FullTimeDay,@DefaultZero) 
					---- ��ȫ����Ա������
					--+ (IsNull(FInsuranceSalary,@DefaultZero) / (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end)) * IsNull(PartTimeDay,@DefaultZero) )

					-- 2017-04-11 ������  ���ò�����ְ����Ա�ĵ�λ���յĺͳ���Ӧ��������������
					(IsNull(InsuranceSalary,@DefaultZero) / (case when MonthWorkDays <= 0 then MonthDays else MonthWorkDays end))
					)
			
			-- �·ݵ�һ��
			,FirstDay
			-- �·ݵ�һ��  �ǵڼ���
			,FirstWeek
			-- �·ݵ�һ��  ���ܼ�
			,FirstWeekDay
		
			-- �·����һ��
			,LastDay
			-- �·����һ��  �ǵڼ���
			,LastWeek
			-- �·����һ��  ���ܼ�
			,LastWeekDay
	from
		(
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

			-- Ա��
			,employee.ID as EmployeeArchive
			---- ����
			--,Department = IsNull(dept.ID,-1)
			--,DepartmentCode = IsNull(dept.Code,'')
			--,DepartmentName = IsNull(deptTrl.Name,'')

			--,case when IsNull(@�쵼�ñ�,'') = '��' then DATEPART(WEEK,checkin.CheckInDate) - DATEPART(WEEK, DateAdd(M,-1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)) + 1,DateAdd(M,1,checkin.CheckInDate)))) + 1
			--	else convert(varchar(10),checkin.CheckInDate,23)
			--	end as CheckInDate
			--,CheckInDate = convert(varchar(10),checkin.CheckInDate,23)
			,CheckInDate = checkin.CheckInDate
			,DisplayDate = Right('00' + DateName(Month,checkin.CheckInDate),2) + '.' + Right('00' + DateName(day,checkin.CheckInDate),2)
			,StatisticsPeriod = Right('0000' + DateName(year,checkin.CheckInDate),4) + '��' + Right('00' + DateName(month,checkin.CheckInDate),2) + '��'
		
			-- ���µڼ���
			,CheckMonthNumber = DATEPART(WEEK,checkin.CheckInDate) - DATEPART(WEEK, DateAdd(M,-1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)) + 1,DateAdd(M,1,checkin.CheckInDate)))) + 1
			-- ���µڼ���
			,CheckDayNumber = DATEPART(day,checkin.CheckInDate)
			-- ���ܼ�
			,CheckWeekDay = DatePart(weekday,checkin.CheckInDate)

			-- ���ܵ�һ��
			,WeekFirstDay = DateAdd(day,1 -  DatePart(weekday,checkin.CheckInDate),checkin.CheckInDate)
			-- �������һ��
			,WeekLastDay = DateAdd(day,7 -  DatePart(weekday,checkin.CheckInDate),checkin.CheckInDate )

			-- �·ݵ�һ��
			,FirstDay = DateAdd(M,-1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)) + 1,DateAdd(M,1,checkin.CheckInDate))) 
			--,FirstDay = case when @��ѡ��ʼ���� is null or @��ѡ��ʼ���� = '' then DateAdd(M,-1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)) + 1,DateAdd(M,1,checkin.CheckInDate))) 
			--				else StartDate.DayDate end
			-- �·ݵ�һ��  �ǵڼ���
			,FirstWeek = 1
			-- �·ݵ�һ��  ���ܼ�
			,FirstWeekDay = DatePart(weekday,DateAdd(M,-1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)) + 1,DateAdd(M,1,checkin.CheckInDate)))) 
		
			-- �·����һ��
			,LastDay = DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)),DateAdd(M,1,checkin.CheckInDate)) 
			--,LastDay =  case when @��ѡ��������� is null or @��ѡ��������� = ''
			--				then DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)),DateAdd(M,1,checkin.CheckInDate)) 
			--				else EndDate.DayDate end
			-- �·����һ��  �ǵڼ���
			,LastWeek = DATEPART(WEEK,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)),DateAdd(M,1,checkin.CheckInDate))) - DATEPART(WEEK, DateAdd(M,-1,DateAdd(d,- day(DateAdd(M,1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)),DateAdd(M,1,checkin.CheckInDate)))) + 1,DateAdd(M,1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)),DateAdd(M,1,checkin.CheckInDate)))))) + 1 
			-- �·����һ��  ���ܼ�
			,LastWeekDay = DatePart(weekday,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)),DateAdd(M,1,checkin.CheckInDate))) 

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
			-- �Ͳ���Ŀ�� , ������û���еģ����� ���Ӳ�ѯ��
			--,LaborYieldTarget = max(IsNull(checkin.LaborYieldTarget,@DefaultZero))
			,LaborYieldTarget = (select max(IsNull(checkin2.LaborYieldTarget,@DefaultZero)) from [10.28.76.125].U9.dbo.Cust_DayCheckIn checkin2 
								where checkin.Department = checkin2.Department
									-- ������ͬ
									and DatePart(year,checkin.CheckInDate)*100 + DatePart(month,checkin.CheckInDate)
										= DatePart(year,checkin2.CheckInDate)*100 + DatePart(month,checkin2.CheckInDate)
								)
			-- �˹��ɱ�Ŀ�� , ������û���еģ����� ���Ӳ�ѯ��
			--,LaborCostTarget = max(IsNull(checkin.LaborCostTarget,@DefaultZero))
			,LaborCostTarget = (select max(IsNull(checkin2.LaborCostTarget,@DefaultZero)) from [10.28.76.125].U9.dbo.Cust_DayCheckIn checkin2 
								where  checkin.Department = checkin2.Department
									-- ������ͬ
									and DatePart(year,checkin.CheckInDate)*100 + DatePart(month,checkin.CheckInDate)
										= DatePart(year,checkin2.CheckInDate)*100 + DatePart(month,checkin2.CheckInDate)
								)
			---- ����
			--,Region = IsNull(region.ID,-1)
			--,RegionCode = IsNull(region.Code,'')
			--,RegionName = IsNull(regionTrl.Name,'')
		
			---- ��������	
			-- Ӧ�������� = �������� - 4
			,MonthDays = IsNull(Day(DateAdd(Day,-1,DateAdd(d,- day(DateAdd(M,1,checkin.CheckInDate)) + 1,DateAdd(M,1,checkin.CheckInDate)))),27)  - 4
		-- ��Ϊ���տ�����¼��
			,IsNull((select max(IsNull(checkin2.MonthWorkDays,0)) 
						from [10.28.76.125].U9.dbo.Cust_DayCheckIn checkin2
						where checkin2.Department = checkIn.Department
						--group by checkin2.Department
						)
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
	 
			--left join [10.28.76.125].U9.dbo.CBO_Department region
			--on SubString(dept.Code,1,5) = region.Code
			--left join [10.28.76.125].U9.dbo.CBO_Department_Trl regionTrl
			--on regionTrl.ID = region.ID
			--	and regionTrl.SysMLFlag = 'zh-CN'

			--left join CBO_Person person
			--on checkinLine.StaffMember = person.ID
			left join [10.28.76.125].U9.dbo.CBO_EmployeeSalaryFile salary
			on salary.Employee = employee.ID
			left join [10.28.76.125].U9.dbo.CBO_PublicSalaryItem salaryItem
			on salary.SalaryItem = salaryItem.ID
			left join [10.28.76.125].U9.dbo.CBO_PublicSalaryItem_Trl salaryItemTrl
			on salaryItemTrl.ID = salaryItem.ID 
				and salaryItemTrl.SysMLFlag = 'zh-CN'
			
	
			--left join [10.28.76.125].U9.dbo.CBO_Department dept
			---- on checkin.Department = dept.ID
			--on dept.ID = checkinSummary.Department

			--left join [10.28.76.125].U9.dbo.CBO_Department_Trl deptTrl
			--on deptTrl.ID = dept.ID
			--	and deptTrl.SysMLFlag = 'zh-CN'
	 
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

			--left join (select max(dateStart.DayDate) as DayDate
			--		from Dim_U9_Date_Filter dateStart 
			--		where dateStart.DayName = @��ѡ��ʼ����) StartDate
			--on 1=1
			--left join (select max(dateStart.DayDate) as DayDate
			--		from Dim_U9_Date_Filter dateStart 
			--		where dateStart.DayName = @��ѡ���������) EndDate
			--on 1=1
		where 
				-- ȫ���Ʊ�׼���� = ��׼����=�������ʣ�01��+��ĩ�Ӱ๤�ʣ�02��+�绰������03��+��ͨ����(04)+��Ͳ�����05��+ְ������07��
			-- ��ȫ���Ʊ�׼���� = �ӵ㹤���ʱ�׼(06)			-- (038)
			-- �Ӱ๤�ʱ�׼=н����Ŀ��F���ʱ�׼��Ŀ(F01)					-- ��F13��
				--(salaryItem.Code is null 
				--	or salaryItem.Code in ('01','02','03','04','05','07' ,'06','F01'))

			dept.ID is not null

			
			/* -- ״̬
			Approved	�����	2
			Approving	�����	1
			Closed	�ѹر�	3
			Opened	����	0
			*/
			and checkin.Status = 2
			
			--and (@��ѡ����� is null or @��ѡ����� = ''
			--	or @��ѡ����� = regionTrl.Name
			--	)
			--and (@��ѡ������ is null or @��ѡ������ = ''
			--	or @��ѡ������ = region2Trl.Name
			--	)
			--and (@��ѡ���� is null or @��ѡ���� = ''
			--	or @��ѡ���� = deptTrl.Name
			--	)

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

			--and (@��ѡ��������� is null or @��ѡ��������� = ''
			--	or @��ѡ��������� = Right('0000' + DateName(year,checkin.CheckInDate),4) + '��' + Right('00' + DateName(month,checkin.CheckInDate),2) + '��'
			--	)
			--and (@��ѡ��ʼ���� is null or @��ѡ��ʼ���� = ''
			--	or checkin.CheckInDate >= StartDate.DayDate --(select max(dateStart.DayDate) from Dim_U9_Date_Filter dateStart where dateStart.DayName = @��ѡ��ʼ����)
			--	)
			--and (@��ѡ��������� is null or @��ѡ��������� = ''
			--	or checkin.CheckInDate <= EndDate.DayDate --(select max(dateEnd.DayDate) from Dim_U9_Date_Filter dateEnd where dateEnd.DayName = @��ѡ���������)
			--	)

			and checkin.CheckInDate >= @StartDate
			and checkin.CheckInDate <= @EndDate

		group by 
			-- ����
			IsNull(region.ID,-1)
			,IsNull(region.Code,'')
			,IsNull(regionTrl.Name,'')
			-- ����
			,IsNull(region2.ID,-1)
			,IsNull(region2.Code,'')
			,IsNull(region2Trl.Name,'')
			-- ����
			,checkIn.Department
			,IsNull(dept.ID,-1)
			,IsNull(dept.Code,'')
			,IsNull(deptTrl.Name,'')

			-- Ա��
			,employee.ID
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
			---- ����
			--,IsNull(region.ID,-1)
			--,IsNull(region.Code,'')
			--,IsNull(regionTrl.Name,'')
			--,StartDate.DayDate
			--,EndDate.DayDate

		) as checkinSummary
	group by 
		-- ����
		Region
		,RegionCode
		,RegionName
		-- ����
		,Region2
		,Region2Code
		,Region2Name
		-- ����
		,Department
		,DepartmentCode
		,DepartmentName

		---- Ա��
		--,employee.ID
		,CheckInDate
		-- ,checkinLine.EmployeeArchive
		,DisplayDate
		,StatisticsPeriod
	
		-- Ӧ�������� = �������� - 4
		,MonthDays
		-- ���ڱ��е�Ӧ��������
		,MonthWorkDays
			
		-- �·ݵ�һ��
		,FirstDay
		-- �·ݵ�һ��  �ǵڼ���
		,FirstWeek
		-- �·ݵ�һ��  ���ܼ�
		,FirstWeekDay

		-- ���ܵ�һ��
		,WeekFirstDay
		-- �������һ��
		,WeekLastDay 
		
		-- �·����һ��
		,LastDay
		-- �·����һ��  �ǵڼ���
		,LastWeek
		-- �·����һ��  ���ܼ�
		,LastWeekDay
			
		-- ���µڼ���
		,CheckMonthNumber
		-- ���µڼ���
		,CheckDayNumber
		-- ���ܼ�
		,CheckWeekDay

		) as checkinSummary
--where 1=1
group by 
	Department
	,DepartmentCode
	,DepartmentName
	,Region
	,RegionCode
	,RegionName
	,Region2
	,Region2Code
	,Region2Name

	,CheckInDate
	,DisplayDate
	,StatisticsPeriod
	
	-- Ӧ�������� = �������� - 4
	,MonthDays
	
	-- �·ݵ�һ��
	,FirstDay
	-- �·ݵ�һ��  �ǵڼ���
	,FirstWeek
	-- �·ݵ�һ��  ���ܼ�
	,FirstWeekDay
		
	-- �·����һ��
	,LastDay
	-- �·����һ��  �ǵڼ���
	,LastWeek
	-- �·����һ��  ���ܼ�
	,LastWeekDay
	
	,LaborYieldTarget
	,LaborCostTarget


select *
from Fact_U9_DayCheckIn
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
order by
	RegionCode
	,Region2Code
	,DepartmentCode 
	,DisplayDate

