
if exists(select * from sys.objects where name='HBH_SP_JianLiYuan_GetAQWRcvLineInfo')
-- 如果存在则删掉
	drop proc HBH_SP_JianLiYuan_GetAQWRcvLineInfo
go

/*

	exec HBH_SP_JianLiYuan_GetAQWRcvLineInfo '50802,50804'
*/
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
	


/*
	
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
	--on rcvhead.lsid = dept.[sid]
	on wh.lsid = dept.[sid]

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

*/



If OBJECT_ID('tempdb..#tmp_hbh_dispatchin') is not null
	Drop Table #tmp_hbh_dispatchin
	
	create table #tmp_hbh_dispatchin (
		ldiid bigint 
		,code varchar(125)
		-- 配送中心ID
		,lsid bigint
		-- 所属仓库ID
		,ldid bigint
		/*
单据状态：
1：待审核
2：已审核
3：已确认待审核
		*/
		,status int
		-- 创建时间
		,ctime datetime
		-- 修改时间
		,utime datetime
			
		-- 配送出库单ID
		,ldoid varchar(125)
		-- 创建人ID
		,cuid varchar(125)
		-- 审核人ID
		,auid varchar(125)
		-- 审核日期
		,atime datetime
		-- 经办人ID
		,ouid varchar(125)
		-- 备注
		,comment varchar(max)
		-- 是否结算
		,bsetcheck varchar(125)
		-- 原因备注ID
		,lrsid varchar(125)
		-- 存储用友单号
		,nccode varchar(125)
		-- 到货时间
		,arrivetime datetime
		-- 是否稽查
		,bcheck varchar(125)
		-- 稽查人
		,buid varchar(125)
		-- 稽查时间
		,btime datetime
		-- 盘点周期tag
		,pdatetag datetime
		-- 单据类型
		,ordertype varchar(125)
		-- 确认人
		,confirmuid varchar(125)
		-- 确认时间
		,confirmtime datetime
		-- 驳回理由Id
		,rlrsid varchar(125)
		-- 驳回人ID
		,returnuid varchar(125)
		-- 驳回时间
		,returntime datetime
		-- 是否已生成调拨领用单
		,bgenerateorder bit
		
		-- 仓库编码
		,sno varchar(125)
		-- 仓库名称
		,ldname varchar(125)
		-- 部门编码
		,shopcode varchar(125)
		-- 部门名称
		,shopname varchar(125)

	)


declare @Sql varchar(max) = 
	'
	insert into #tmp_hbh_dispatchin
	select *
	from openquery([MYSQL-AWQ],''SELECT 
			rcvhead.ldiid  
			,rcvhead.code 
			,rcvhead.lsid 
			,rcvhead.ldid 
			,rcvhead.status 
			,rcvhead.ctime 
			,rcvhead.utime 

			,rcvhead.ldoid
			,rcvhead.cuid
			,rcvhead.auid
			,rcvhead.atime
			,rcvhead.ouid
			,rcvhead.comment
			,rcvhead.bsetcheck
			,rcvhead.lrsid
			,rcvhead.nccode
			,rcvhead.arrivetime
			,rcvhead.bcheck
			,rcvhead.buid
			,rcvhead.btime
			,rcvhead.pdatetag
			,rcvhead.ordertype
			
			,rcvhead.confirmuid
			,rcvhead.confirmtime
			,rcvhead.rlrsid
			,rcvhead.returnuid
			,cast(rcvhead.returntime as datetime) as returntime
			,rcvhead.bgenerateorder
			
			,cast(wh.sno as char(125)) as sno
			,wh.ldname
			
			,dept.shopcode
			,dept.shopname

					FROM lgt_dispatchin rcvhead

						left join lgt_depot wh
						on rcvhead.ldid = wh.ldid

						left join sls_shop dept
						on wh.lsid = dept.sid
					where rcvhead.status = 2
						#OPath#
					; '') tmp';


declare @WhereheadIDs varchar(125) = ' 1=0 '
if(@headIDs is not null and @headIDs != '')
begin
	set @WhereheadIDs = ' rcvhead.ldiid in (' + @headIDs + ')'
end


set @Sql = Replace(@Sql,'#OPath#',' and ' + @WhereheadIDs  )
--set @Sql = Replace(@Sql,'#OPath#',' and ' + '1=0'  )
	
  -- print (@Sql)
  exec (@Sql)
  

If OBJECT_ID('tempdb..#tmp_hbh_dispatchin_item') is not null
	Drop Table #tmp_hbh_dispatchin_item
	  
create table #tmp_hbh_dispatchin_item (

	-- 配送入库单单据项ID
	ldiiid varchar(125)
	-- 配送入库单ID
	,ldiid varchar(125)
	-- 货品ID
	,lgid varchar(125)
	-- 送货数量
	,amount varchar(125)
	-- 实收减应收数量
	,damount varchar(125)
	---- 实际入库数量	=	(Amount+damount)
	--, varchar(125)
	-- 单价
	,uprice varchar(125)
	-- 金额
	,total varchar(125)
	-- 备注
	,icomment varchar(125)
	-- 更新时间
	,iutime datetime
	-- 批次
	,batch varchar(125)
	-- 配送单价
	,disprice varchar(125)
	-- 入库时间
	,depotintime datetime
	-- 成本单价
	,originalprice varchar(125)
	-- 原因备注ID
	,lrsid varchar(125)
	-- 是否核对
	,bcheck varchar(125)
	-- 抽查数量
	,checkamount varchar(125)
	-- 合格数量
	,qualifiedamount varchar(125)
	-- 月结调拨退回批次号
	,movebatch varchar(125)
	-- 销售税点
	,salestax varchar(125)
	-- 配送模式
	,salesmode varchar(125)
	-- 未含税单价
	,disoriginalprice varchar(125)
	-- 是否驳回
	,breturn varchar(125)
			
	-- 货品编码
	,lgcode varchar(125)
	-- 货品名称
	,lgname varchar(125)

)


 set @Sql = 
	'
	insert into #tmp_hbh_dispatchin_item
	select *
	from openquery([MYSQL-AWQ],''SELECT 
			rcvline.ldiiid
			,rcvline.ldiid
			,rcvline.lgid
			,rcvline.amount
			,rcvline.damount
			,rcvline.uprice
			,rcvline.total
			,rcvline.icomment
			,rcvline.iutime
			,rcvline.batch
			,rcvline.disprice
			,rcvline.depotintime
			,rcvline.originalprice
			,rcvline.lrsid
			,rcvline.bcheck
			,rcvline.checkamount
			,rcvline.qualifiedamount
			,rcvline.movebatch
			,rcvline.salestax
			,rcvline.salesmode
			,rcvline.disoriginalprice
			,rcvline.breturn
			
			,item.sno as lgcode
			,item.name as lgname
			

				from lgt_dispatchin_item rcvline
					left join lgt_good item
					on rcvline.lgid = item.lgid


				where 1=1
						#OPath#
					; '') tmp';

					
set @WhereheadIDs = ' 1=0 '

if(@headIDs is not null and @headIDs != '')
begin
	set @WhereheadIDs = ' rcvline.ldiid in (' + @headIDs + ')'
end

set @Sql = Replace(@Sql,'#OPath#',' and ' + @WhereheadIDs  )
  -- print (@Sql)
  exec (@Sql)


  


  select *
  from #tmp_hbh_dispatchin

  select *
  from #tmp_hbh_dispatchin_item

