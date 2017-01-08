
use U9ETLMeta


/*
select a.Code,a.Name,c.Code ExtractTableCode,c.Name ExtractTableName,License 
from [dbo].[ExtractSubject] a 
	inner join [dbo].[ExtractSubjectDetail] b
		on a.IID=b.SubjectId
	inner join [dbo].[ExtractTables] c on b.ExtractTableCode=c.Code
where 1=1
	-- and a.Code=@SubjectCode
	and  c.SaveRule=1
order by c.Type,b.ExtractTableCode
*/



-- �洢����������ȡ�ˣ��ֹ���ȡ���¼ӵ����ǲ�ɾ�������ݣ���֪��Ϊʲô
/*
-- 901�����Ŷ���
declare @TableCode varchar(125) = '901'
-- 001��������
declare @ApplicationCode varchar(125) = '001'


if not exists(select 1 from [dbo].[ExtractSubject] app
					inner join [dbo].[ExtractSubjectDetail] detail
						on app.IID=detail.SubjectId
				where app.Code = @ApplicationCode
					and detail.ExtractTableCode = @TableCode
				)
begin
	insert into [ExtractSubjectDetail]
	(
		SubjectID,ExtractTableCode,ExtractOrder
	)
	select 
		(select app.IID from [ExtractSubject] app where app.Code = @ApplicationCode)
		,@TableCode
		,null
end
*/



use U9BAMeta

-- ����洢���̶�Ӧ��ϵ�� select * from ETL_RealTime_Tables
-- ģ��:  select * from BusinessModelList


declare @ReportID varchar(125)
declare @ModelID varchar(125)
declare @TableName varchar(125)
declare @SPName varchar(125)
declare @Parameters varchar(125)


-- �Ͷ��������˹��ɱ�ͳ�Ʊ�
--declare @ReportID varchar(125) = '290b6a33-3117-4cdf-a97d-84f1339000c9'
--declare @ModelID varchar(125) = '1D61EF22-2ADF-42DD-A3D2-DE5775B160C7'
--declare @ReportID varchar(125) = 'e0ef8d8e-5d18-43bc-93a7-3f655033cdc7'
set @ReportID = 'c6093628-faf4-42d6-94ee-b93c050778d0'
set @ModelID = '1D61EF22-2ADF-42DD-A3D2-DE5775B160C7'
set @TableName = 'Fact_U9_DayCheckIn'
set @SPName = 'HBH_BASP_JianLiYuan_DayCheckIn'
set @Parameters = '@��ѡ���������,@��ѡ�����,@��ѡ������,@��ѡ����'


/*
delete from ETL_RealTime_Tables
				where ReportID = @ReportID
					and ModelID = @ModelID
*/
if not exists(select 1 from ETL_RealTime_Tables
				where ReportID = @ReportID
					and ModelID = @ModelID
				)
begin
	insert into ETL_RealTime_Tables
	(
		TableName,SPName,Parameters,ReportID,ModelID
	)
	select
		@TableName
		,@SPName
		,@Parameters
		,@ReportID		-- ����--����--ID
		,@ModelID
end



-- �Ͷ��������˹��ɱ�ͳ�Ʊ�(�쵼�ñ�)
set @ReportID = 'b5696c01-2b0b-4745-9844-5f3efe3fea62'
set @ModelID = '1D61EF22-2ADF-42DD-A3D2-DE5775B160C7'
set @TableName = 'Fact_U9_DayCheckIn'
set @SPName = 'HBH_BASP_JianLiYuan_DayCheckInLedUse'
set @Parameters = '@��ѡ���������,@��ѡ�����,@��ѡ����'

/*
delete from ETL_RealTime_Tables
				where ReportID = @ReportID
					and ModelID = @ModelID
*/
if not exists(select 1 from ETL_RealTime_Tables
				where ReportID = @ReportID
					and ModelID = @ModelID
				)
begin
	insert into ETL_RealTime_Tables
	(
		TableName,SPName,Parameters,ReportID,ModelID
	)
	select
		@TableName
		,@SPName
		,@Parameters
		,@ReportID		-- ����--����--ID
		,@ModelID
end

-- ���ڲ���ʵ�����ͳ�Ʊ�
--set @ReportID = 'd4d091f4-5e93-4742-bfd8-f60b5e7e578f'
set @ReportID = '905e5195-aada-4c91-8ae6-273cda3f3721'
set @ModelID = '1D61EF22-2ADF-42DD-A3D2-DE5775B160C7'
set @TableName = 'Fact_U9_HolidayAttendance'
set @SPName = 'HBH_BASP_JianLiYuan_HolidayAttendance'
set @Parameters = '@��ѡ���������,@��ѡ������,@��ѡ�����,@��ѡ����,@��ѡ��ʼ����,@��ѡ���������'

/*
delete from ETL_RealTime_Tables
				where ReportID = @ReportID
					and ModelID = @ModelID
*/
if not exists(select 1 from ETL_RealTime_Tables
				where ReportID = @ReportID
					and ModelID = @ModelID
				)
begin
	insert into ETL_RealTime_Tables
	(
		TableName,SPName,Parameters,ReportID,ModelID
	)
	select
		@TableName
		,@SPName
		,@Parameters
		,@ReportID		-- ����--����--ID
		,@ModelID
end


-- �����˾�Ч���˹��ɱ�Ԥ����
--set @ReportID = '0171b154-6278-4b62-9985-ec53a2dc5519'
--set @ReportID = '3b3b5a55-60f9-400e-8aaa-bb6d7be5bd1a'
--set @ReportID = '8c20b4eb-bb0f-4d3b-8d9a-601b4c93cf11'
set @ReportID = '2012a71a-d3ba-45ed-93ff-00161427f7ca'
set @ModelID = '1D61EF22-2ADF-42DD-A3D2-DE5775B160C7'
set @TableName = 'Fact_U9_EfficiencyCostWarning'
set @SPName = 'HBH_BASP_JianLiYuan_EfficiencyCostWarning'
set @Parameters = '@��ѡ���������,@��ѡ������,@��ѡ�����,@��ѡ����,@��ѡ��ʼ����,@��ѡ���������'

/*
delete from ETL_RealTime_Tables
				where ReportID = @ReportID
					and ModelID = @ModelID
--*/
if not exists(select 1 from ETL_RealTime_Tables
				where ReportID = @ReportID
					and ModelID = @ModelID
				)
begin
	insert into ETL_RealTime_Tables
	(
		TableName,SPName,Parameters,ReportID,ModelID
	)
	select
		@TableName
		,@SPName
		,@Parameters
		,@ReportID		-- ����--����--ID
		,@ModelID
end

