
/*
select *
from HBH_SPParamRecord
order by CreatedOn desc
*/

if exists(select * from sys.objects where name='HBH_BASP_JianLiYuan_EfficiencyCostWarning')
-- ���������ɾ��
	drop proc HBH_BASP_JianLiYuan_EfficiencyCostWarning
go
-- �����˾�Ч���˹��ɱ�Ԥ����
-- �����洢����
create proc HBH_BASP_JianLiYuan_EfficiencyCostWarning  (
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
	declare @Debugger bit = (select top 1 Debugger from HBH_Debug_Param where ProcName = 'HBH_BASP_JianLiYuan_EfficiencyCostWarning' or ProcName is null or ProcName = '' order by ProcName desc)
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
		select 'HBH_BASP_JianLiYuan_EfficiencyCostWarning','@��ѡ���������',IsNull(@��ѡ���������,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_EfficiencyCostWarning','@��ѡ�����',IsNull(@��ѡ�����,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_EfficiencyCostWarning','@��ѡ������',IsNull(@��ѡ������,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_EfficiencyCostWarning','@��ѡ����',IsNull(@��ѡ����,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_EfficiencyCostWarning','@��ѡ��ʼ����',IsNull(@��ѡ��ʼ����,'null'),GETDATE()
		union select 'HBH_BASP_JianLiYuan_EfficiencyCostWarning','@��ѡ���������',IsNull(@��ѡ���������,'null'),GETDATE()

		union select 'HBH_BASP_JianLiYuan_EfficiencyCostWarning','ProcSql','exec HBH_BASP_JianLiYuan_EfficiencyCostWarning '
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

-- ��ʵ��Fact_U9_EfficiencyCostWarning
-- ɾ��
truncate table Fact_U9_EfficiencyCostWarning
-- ����
insert into Fact_U9_EfficiencyCostWarning
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
	
--	-- �����ڼ�
--	,StatisticsPeriod varchar(125)
--	-- ����
--	,WarningDate varchar(125)
--	-- �ʹ�
--	,MealTime varchar(200) 
--	-- Ԥ�ƾͲ�����
--	,EstimatedQty decimal(24,9)
--	-- �ͱ�
--	,MealStandard decimal(24,9)
--	-- �Ͳ�����,������ϼƹ�ʽ�ȽϺ��ʣ�������֪��BA֧�ֲ�;   =  ���Ԥ�ƾͲ�����*��Ͳͱ�+�в�Ԥ�ƾͲ�����*�вͲͱ�+���Ԥ�ƾͲ�����*��Ͳͱ�+ҹ��Ԥ�ƾͲ�����*ҹ�Ͳͱ�
--	,DiningIncome decimal(24,9)

--	-- �ճ���Сʱ��
--	,AttendanceTime decimal(24,9)
--	-- �������� ????  = �ճ���Сʱ��/8
--	,TranslatedNumber decimal(24,9)

--	-- �����˹�����
--	,Wage decimal(24,9)
--	-- ���ۺ�ë��
--	,GrossProfit decimal(24,9)
	
--	-- �˾�Ч��Ԥ��
--	-- �˾�Ч�� = if����������=0,0���Ͳ�����/����������
--	,PerEfficiency decimal(24,9)
--	-- ��˾���տ��Ʊ�׼(Ч��) = ���̶�ֵ500
--	,EfficiencyHolidayStandards decimal(24,9) default 500
--	-- ����(Ч��) = ��˾���տ��Ʊ�׼-�˾�Ч��
--	,EfficiencyDiffer decimal(24,9)
--	-- ������(Ч��) = if(����>=0,"���","�˾�Ч�ʵ��ڹ�˾Ҫ�������")
--	,EfficiencyStandardConditions varchar(125)

--	-- �˹��ɱ�Ԥ��
--	-- �˹��ɱ� = IF(�����˹�����=0,"0",�����˹�����/�Ͳ�����)
--	,PerCost decimal(24,9)
--	-- ��˾���տ��Ʊ�׼(�ɱ�) = ���̶�ֵ20%
--	,CostHolidayStandards decimal(24,9) default 0.2
--	-- ����(�ɱ�) = ��˾���տ��Ʊ�׼-�˾�Ч��
--	,CostDiffer decimal(24,9)
--	-- ������(�ɱ�) = IF(����>=0,"�˹��ɱ����꣬�����","���")
--	,CostStandardConditions varchar(125)

--	-- ��ע
--	,Memo varchar(125)
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
	
	-- �����ڼ�
	,StatisticsPeriod
	-- ����
	,WarningDate
	-- ����
	,CheckInDate
	-- �ʹ�	(�����ɺϲ��İ�)
	,MealTime = -1
	-- Ԥ�ƾͲ�����
	,EstimatedQty = EstimatedQty
	-- �ͱ�
	,MealStandard = MealStandard
	-- �Ͳ�����,������ϼƹ�ʽ�ȽϺ��ʣ�������֪��BA֧�ֲ�;   =  ���Ԥ�ƾͲ�����*��Ͳͱ�+�в�Ԥ�ƾͲ�����*�вͲͱ�+���Ԥ�ƾͲ�����*��Ͳͱ�+ҹ��Ԥ�ƾͲ�����*ҹ�Ͳͱ�
	,DiningIncome = DiningIncome

	-- �ճ���Сʱ��
	,AttendanceTime = AttendanceTime
	-- �������� ????  = �ճ���Сʱ��/8
	,TranslatedNumber = TranslatedNumber

	-- �����˹�����
	,Wage = Wage
	-- ���ۺ�ë��
	,GrossProfit = GrossProfit
	
	-- �˾�Ч��Ԥ��
	-- �˾�Ч�� = if����������=0,0���Ͳ�����/����������
	,PerEfficiency = case when IsNull(TranslatedNumber,0) = 0 then 0
						else IsNull(DiningIncome,0) / IsNull(TranslatedNumber,0) end

	-- ��˾���տ��Ʊ�׼(Ч��) = ���̶�ֵ500
	,EfficiencyHolidayStandards = 500
	-- ����(Ч��) = ��˾���տ��Ʊ�׼-�˾�Ч��
	,EfficiencyDiffer = case when IsNull(TranslatedNumber,0) = 0 then 0
							else IsNull(DiningIncome,0) / IsNull(TranslatedNumber,0) end
						- 500
	-- ������(Ч��) = if(����>=0,"���","�˾�Ч�ʵ��ڹ�˾Ҫ�������")
	,EfficiencyStandardConditions =  case when (case when IsNull(TranslatedNumber,0) = 0 then 0
											else IsNull(DiningIncome,0) / IsNull(TranslatedNumber,0) end
										- 500) > 0
									then '���'
									else '�˾�Ч�ʵ��ڹ�˾Ҫ�������' end

	-- �˹��ɱ�Ԥ��
	-- �˹��ɱ� = IF(�����˹�����=0,"0",�����˹�����/�Ͳ�����)
	,PerCost = case when IsNull(DiningIncome,0) = 0 then 0
						else IsNull(Wage,0) / IsNull(DiningIncome,0) end
	-- ��˾���տ��Ʊ�׼(�ɱ�) = ���̶�ֵ20%
	,CostHolidayStandards = 0.2
	-- ����(�ɱ�) = ��˾���տ��Ʊ�׼-�˾�Ч��
	,CostDiffer = case when IsNull(DiningIncome,0) = 0 then 0
					else IsNull(Wage,0) / IsNull(DiningIncome,0) end
				- 0.2
	-- ������(�ɱ�) = IF(����>=0,"�˹��ɱ����꣬�����","���")
	,CostStandardConditions = case when (case when IsNull(DiningIncome,0) = 0 then 0
										else IsNull(Wage,0) / IsNull(DiningIncome,0) end
									- 0.2) <= 0
									then '���'
									else '�˹��ɱ����꣬�����' end

	-- ��ע
	,Memo = ''
	
	-- Ԥ�ƾͲ�����
	,MorningEstimatedQty
	,NoonEstimatedQty
	,AfternoonEstimatedQty
	,NightEstimatedQty
	
	-- �ͱ�
	,MorningMealStandard
	,NoonMealStandard
	,AfternoonMealStandard
	,NightMealStandard
from (
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
	
		-- �����ڼ�
		,StatisticsPeriod = Right('0000' + DateName(year,warningLine.Date),4) + '��' + Right('00' + DateName(month,warningLine.Date),2) + '��'
		-- ����
		,WarningDate = Right('00' + DateName(Month,warningLine.Date),2) + '.' + Right('00' + DateName(day,warningLine.Date),2)
		-- ����
		,CheckInDate = warningLine.Date
		---- �ʹ�
		--,MealTime = MealTime
		-- Ԥ�ƾͲ�����
		,EstimatedQty = sum(IsNull(EstimatedQty,0))
		-- �ͱ�
		--,MealStandard = max(IsNull(MealStandard,0))
		,MealStandard = @DefaultZero
		-- �Ͳ�����,������ϼƹ�ʽ�ȽϺ��ʣ�������֪��BA֧�ֲ�;   =  ���Ԥ�ƾͲ�����*��Ͳͱ�+�в�Ԥ�ƾͲ�����*�вͲͱ�+���Ԥ�ƾͲ�����*��Ͳͱ�+ҹ��Ԥ�ƾͲ�����*ҹ�Ͳͱ�
		,DiningIncome = sum(IsNull(EstimatedQty,0) * IsNull(MealStandard,0))

		-- �ճ���Сʱ��
		,AttendanceTime = sum(IsNull(AttendanceTime,0))
		-- �������� ????  = �ճ���Сʱ��/8
		,TranslatedNumber = sum(IsNull(AttendanceTime,0) / 8)

		-- �����˹�����
		,Wage = sum(IsNull(Wage,0))
		-- ���ۺ�ë��
		,GrossProfit = sum(IsNull(GrossProfit,0))
	
		---- �˾�Ч��Ԥ��
		---- �˾�Ч�� = if����������=0,0���Ͳ�����/����������
		--,PerEfficiency = case when sum(IsNull(AttendanceTime,0)) = 0 then 0
		--					else sum(IsNull(DiningIncome,0)) / (sum(IsNull(AttendanceTime,0))/8) end

		---- ��˾���տ��Ʊ�׼(Ч��) = ���̶�ֵ500
		--,EfficiencyHolidayStandards = 500
		---- ����(Ч��) = ��˾���տ��Ʊ�׼-�˾�Ч��
		--,EfficiencyDiffer = 500 - case when sum(IsNull(AttendanceTime,0)) = 0 then 0
		--							else sum(IsNull(DiningIncome,0)) / (sum(IsNull(AttendanceTime,0))/8) end
		---- ������(Ч��) = if(����>=0,"���","�˾�Ч�ʵ��ڹ�˾Ҫ�������")
		---- ,EfficiencyStandardConditions = sum(IsNull(,0))

		---- �˹��ɱ�Ԥ��
		---- �˹��ɱ� = IF(�����˹�����=0,"0",�����˹�����/�Ͳ�����)
		--,PerCost = case when sum(IsNull(DiningIncome,0)) = 0 then 0
		--					else sum(IsNull(Wage,0)) / sum(IsNull(DiningIncome,0)) end
		---- ��˾���տ��Ʊ�׼(�ɱ�) = ���̶�ֵ20%
		--,CostHolidayStandards = 0.2
		---- ����(�ɱ�) = ��˾���տ��Ʊ�׼-�˾�Ч��
		--,CostDiffer = 0.2 - case when sum(IsNull(DiningIncome,0)) = 0 then 0
		--						else sum(IsNull(Wage,0)) / sum(IsNull(DiningIncome,0)) end
		---- ������(�ɱ�) = IF(����>=0,"�˹��ɱ����꣬�����","���")
		----,CostStandardConditions = 

		---- ��ע
		--,Memo = ''
	
		/*	MealTime
		Morning	��	0
		Noon	��	1
		Afternoon	��	2
		Night	ҹ	3
		*/
		-- Ԥ�ƾͲ�����
		,MorningEstimatedQty = Sum(case when MealTime = 0 then IsNull(EstimatedQty,0)
									else @DefaultZero end
									)
		,NoonEstimatedQty = Sum(case when MealTime = 1 then IsNull(EstimatedQty,0)
									else @DefaultZero end
									)
		,AfternoonEstimatedQty = Sum(case when MealTime = 2 then IsNull(EstimatedQty,0)
									else @DefaultZero end
									)
		,NightEstimatedQty = Sum(case when MealTime = 3 then IsNull(EstimatedQty,0)
									else @DefaultZero end
									)
	
		-- �ͱ�
		,MorningMealStandard = Max(case when MealTime = 0 then IsNull(MealStandard,0)
									else @DefaultZero end
									)
		,NoonMealStandard = Max(case when MealTime = 1 then IsNull(MealStandard,0)
									else @DefaultZero end
									)
		,AfternoonMealStandard = Max(case when MealTime = 2 then IsNull(MealStandard,0)
									else @DefaultZero end
									)
		,NightMealStandard = Max(case when MealTime = 3 then IsNull(MealStandard,0)
									else @DefaultZero end
									)
	from 
		[10.28.76.125].U9.dbo.Cust_CostWarning warning
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

		and (@��ѡ��ʼ���� is null or @��ѡ��ʼ���� = ''
			or warningLine.Date >= (select max(dateStart.DayDate) from Dim_U9_Date_Filter dateStart where dateStart.DayName = @��ѡ��ʼ����)
			)
		and (@��ѡ��������� is null or @��ѡ��������� = ''
			or warningLine.Date <= (select max(dateEnd.DayDate) from Dim_U9_Date_Filter dateEnd where dateEnd.DayName = @��ѡ���������)
			)

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
		,IsNull(dept.ID,-1)
		,IsNull(dept.Code,'')
		,IsNull(deptTrl.Name,'')
	
		-- �����ڼ�
		,Right('0000' + DateName(year,warningLine.Date),4) + '��' + Right('00' + DateName(month,warningLine.Date),2) + '��'
		-- ����
		,Right('00' + DateName(Month,warningLine.Date),2) + '.' + Right('00' + DateName(day,warningLine.Date),2)
		,warningLine.Date
		---- �ʹ�
		--,MealTime = MealTime
	) warningDetail




select *
from Fact_U9_EfficiencyCostWarning
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




