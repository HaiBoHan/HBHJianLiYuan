
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


-- 901，部门二；
declare @TableCode varchar(125) = '901'
-- 001，公共；
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




use U9BAMeta

-- select * from ETL_RealTime_Tables
-- select * from BusinessModelList

declare @ReportID varchar(125) = '290b6a33-3117-4cdf-a97d-84f1339000c9'
declare @ModelID varchar(125) = '1D61EF22-2ADF-42DD-A3D2-DE5775B160C7'
declare @TableName varchar(125) = 'Fact_U9_DayCheckIn'
declare @SPName varchar(125) = 'HBH_BASP_JianLiYuan_DayCheckIn'
declare @Parameters varchar(125) = '@请选择过滤年月,@请选择大区,@请选择部门'


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
		,@ReportID		-- 报表--属性--ID
		,@ModelID
end


set @ReportID = 'c6093628-faf4-42d6-94ee-b93c050778d0'
set @ModelID = '1D61EF22-2ADF-42DD-A3D2-DE5775B160C7'
set @TableName = 'Fact_U9_DayCheckIn'
set @SPName = 'HBH_BASP_JianLiYuan_DayCheckIn'
set @Parameters = '@请选择过滤年月,@请选择大区,@请选择部门'

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
		,@ReportID		-- 报表--属性--ID
		,@ModelID
end


set @ReportID = 'd4d091f4-5e93-4742-bfd8-f60b5e7e578f'
set @ModelID = '1D61EF22-2ADF-42DD-A3D2-DE5775B160C7'
set @TableName = 'Fact_U9_HolidayAttendance'
set @SPName = 'HBH_BASP_JianLiYuan_HolidayAttendance'
set @Parameters = '@请选择过滤年月,@请选择区域,@请选择大区,@请选择部门'

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
		,@ReportID		-- 报表--属性--ID
		,@ModelID
end

