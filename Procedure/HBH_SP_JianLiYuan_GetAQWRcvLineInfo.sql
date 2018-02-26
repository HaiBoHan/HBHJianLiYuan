
if exists(select * from sys.objects where name='HBH_SP_JianLiYuan_GetAQWRcvLineInfo')
-- 如果存在则删掉
	drop proc HBH_SP_JianLiYuan_GetAQWRcvLineInfo
go
-- 创建存储过程
create proc HBH_SP_JianLiYuan_GetAQWRcvLineInfo  (
 @HeadIDs varchar(max)
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
			-- drop table HBH_SPParamRecord
			create table HBH_SPParamRecord
			(ProcName varchar(501)
			,ParamName varchar(501)
			,ParamValue varchar(max)
			,CreatedOn DateTime
			,Memo varchar(max)	-- 备注
			--,ParamValueXml xml
			)
		end

		insert into HBH_SPParamRecord
		(ProcName,ParamName,ParamValue,CreatedOn)
		select 'HBH_SP_JianLiYuan_GetAQWRcvLineInfo','@HeadIDs',IsNull(cast(@HeadIDs as varchar(max)),'null'),GETDATE()
		--union  select 'HBH_SP_JianLiYuan_GetAQWRcvLineInfo','@StartDate',IsNull(Convert(varchar,@StartDate,120),'null'),GETDATE()
		-- union select 'HBH_SP_JianLiYuan_GetAQWRcvLineInfo','@EndDate',IsNull(Convert(varchar,@EndDate,120),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_GetAQWRcvLineInfo','ProcSql','exec HBH_SP_JianLiYuan_GetAQWRcvLineInfo '
				+ IsNull('''' + cast(@HeadIDs as varchar(max)) + '''','null') 
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
	


	
-- 1、获得传入物料ID集合
If OBJECT_ID('tempdb..#hbh_tmp_ObjectList') is not null
	Drop Table #hbh_tmp_ObjectList

--declare @split char(1) = ','
DECLARE @xmlIDs XML

--SET @xmlIDs = CONVERT(XML,'<items><item id="' + REPLACE(@HeadIDs, @split, '"/><item id="') + '"/></items>')
set @xmlIDs = @HeadIDs

-- select *
-- into #hbh_tmp_ObjectList
-- from CBO_ItemMaster
-- where ID in (
-- 		SELECT x.item.value('@id[1]', 'bigint') as ItemID
-- 		FROM @xmlIDs.nodes('//items/item') AS x(item)
-- 		)

SELECT x.item.value('ID[1]', 'varchar(125)') as ID
into #hbh_tmp_ObjectList
FROM @xmlIDs.nodes('//HeadIDs') AS x(item)



select 
	rcvhead.*

	,wh.sno
	,wh.ldname

	,dept.shopcode
	,dept.shopname

from lgt_dispatchin rcvhead

	left join lgt_depot wh
	on rcvhead.ldid = wh.ldid

	left join sls_shop dept
	on rcvhead.lsid = dept.[sid]

where 1=1
	and rcvhead.ldiid in (select tmp.ID
				from #hbh_tmp_ObjectList tmp
				)



select 
	rcvline.*
	,lgcode = item.sno
	,lgname = item.name
from lgt_dispatchin_item rcvline
	left join lgt_good item
	on rcvline.lgid = item.lgid

where 1=1
	and ldiid in (select ID
				from #hbh_tmp_ObjectList
				)


/*
select *
from #hbh_tmp_ObjectList
*/



