
if exists(select * from sys.objects where name='HBH_SP_JianLiYuan_GetAQWRcvLineInfo')
-- ���������ɾ��
	drop proc HBH_SP_JianLiYuan_GetAQWRcvLineInfo
go

/*

	exec HBH_SP_JianLiYuan_GetAQWRcvLineInfo '50802,50804'
*/
-- �����洢����
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
			,Memo varchar(max)	-- ��ע
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
	
-- 1����ô�������ID����
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
		-- ��������ID
		,lsid bigint
		-- �����ֿ�ID
		,ldid bigint
		/*
����״̬��
1�������
2�������
3����ȷ�ϴ����
		*/
		,status int
		-- ����ʱ��
		,ctime datetime
		-- �޸�ʱ��
		,utime datetime
			
		-- ���ͳ��ⵥID
		,ldoid varchar(125)
		-- ������ID
		,cuid varchar(125)
		-- �����ID
		,auid varchar(125)
		-- �������
		,atime datetime
		-- ������ID
		,ouid varchar(125)
		-- ��ע
		,comment varchar(max)
		-- �Ƿ����
		,bsetcheck varchar(125)
		-- ԭ��עID
		,lrsid varchar(125)
		-- �洢���ѵ���
		,nccode varchar(125)
		-- ����ʱ��
		,arrivetime datetime
		-- �Ƿ����
		,bcheck varchar(125)
		-- ������
		,buid varchar(125)
		-- ����ʱ��
		,btime datetime
		-- �̵�����tag
		,pdatetag datetime
		-- ��������
		,ordertype varchar(125)
		-- ȷ����
		,confirmuid varchar(125)
		-- ȷ��ʱ��
		,confirmtime datetime
		-- ��������Id
		,rlrsid varchar(125)
		-- ������ID
		,returnuid varchar(125)
		-- ����ʱ��
		,returntime datetime
		-- �Ƿ������ɵ������õ�
		,bgenerateorder bit
		
		-- �ֿ����
		,sno varchar(125)
		-- �ֿ�����
		,ldname varchar(125)
		-- ���ű���
		,shopcode varchar(125)
		-- ��������
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

	-- ������ⵥ������ID
	ldiiid varchar(125)
	-- ������ⵥID
	,ldiid varchar(125)
	-- ��ƷID
	,lgid varchar(125)
	-- �ͻ�����
	,amount varchar(125)
	-- ʵ�ռ�Ӧ������
	,damount varchar(125)
	---- ʵ���������	=	(Amount+damount)
	--, varchar(125)
	-- ����
	,uprice varchar(125)
	-- ���
	,total varchar(125)
	-- ��ע
	,icomment varchar(125)
	-- ����ʱ��
	,iutime datetime
	-- ����
	,batch varchar(125)
	-- ���͵���
	,disprice varchar(125)
	-- ���ʱ��
	,depotintime datetime
	-- �ɱ�����
	,originalprice varchar(125)
	-- ԭ��עID
	,lrsid varchar(125)
	-- �Ƿ�˶�
	,bcheck varchar(125)
	-- �������
	,checkamount varchar(125)
	-- �ϸ�����
	,qualifiedamount varchar(125)
	-- �½�����˻����κ�
	,movebatch varchar(125)
	-- ����˰��
	,salestax varchar(125)
	-- ����ģʽ
	,salesmode varchar(125)
	-- δ��˰����
	,disoriginalprice varchar(125)
	-- �Ƿ񲵻�
	,breturn varchar(125)
			
	-- ��Ʒ����
	,lgcode varchar(125)
	-- ��Ʒ����
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

