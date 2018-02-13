
if exists(select * from sys.objects where name='HBH_SP_JianLiYuan_GetAQWRcvLineInfo')
-- ���������ɾ��
	drop proc HBH_SP_JianLiYuan_GetAQWRcvLineInfo
go
-- �����洢����
create proc HBH_SP_JianLiYuan_GetAQWRcvLineInfo  (
 @HeadID bigint = -1
)
with encryption
as
	SET NOCOUNT ON;

	
declare @SysMlFlag varchar(11) = 'zh-CN'


if exists(select name from sys.objects where name = 'HBH_Debug_Param')
begin
	declare @Debugger bit = (select top 1 Debugger from HBH_Debug_Param where ProcName = 'HBH_SP_JianLiYuan_GetAQWRcvLineInfo' or ProcName is null or ProcName = '' order by ProcName desc)
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
		select 'HBH_SP_JianLiYuan_GetAQWRcvLineInfo','@HeadID',IsNull(cast(@HeadID as varchar(max)),'null'),GETDATE()
		--union  select 'HBH_SP_JianLiYuan_GetAQWRcvLineInfo','@StartDate',IsNull(Convert(varchar,@StartDate,120),'null'),GETDATE()
		-- union select 'HBH_SP_JianLiYuan_GetAQWRcvLineInfo','@EndDate',IsNull(Convert(varchar,@EndDate,120),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_GetAQWRcvLineInfo','ProcSql','exec HBH_SP_JianLiYuan_GetAQWRcvLineInfo '
				+ IsNull(cast(@HeadID as varchar(501)),'null') 
				--+ ',' + IsNull('''' + Convert(varchar,@StartDate,120) + '''' ,'null')
				--+ ',' + IsNull('''' + Convert(varchar,@EndDate,120) + '''' ,'null')
			   ,GETDATE()
	end
end



	declare @SysLineNo int = 10
	declare @Now datetime = GetDate();
	declare @CurDate datetime = Convert(date,@Now)
	declare @SheetName varchar(125)
	declare @StartID bigint = -1
	declare @TotalIDCount int = 0
	declare @TotalLineCount int = 0
	declare @DetailLineCount int = 0
	declare @DefaultZero decimal(24,9) = 0
	



select 
	*
from lgt_dispatchin_item
where 1=1
	and ldiid = @HeadID





