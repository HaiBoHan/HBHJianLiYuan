
/*
	exec HBH_SP_JianLiYuan_GetAQWTableFromAWQ null,null,0
	-- exec HBH_SP_JianLiYuan_GetAQWTableFromAWQ null,null,1
*/
if exists(select * from sys.objects where name='HBH_SP_JianLiYuan_GetAQWTableFromAWQ')
-- ���������ɾ��
	drop proc HBH_SP_JianLiYuan_GetAQWTableFromAWQ
go
-- �����洢����
create proc HBH_SP_JianLiYuan_GetAQWTableFromAWQ (
 @StartDate datetime = null
,@EndDate datetime = null
,@IsDropTable bit = 0
)
with encryption
as
	--SET NOCOUNT ON;

	
declare @SysMlFlag varchar(11) = 'zh-CN'


if exists(select name from sys.objects where name = 'HBH_Debug_Param')
begin
	declare @Debugger bit = (select top 1 Debugger from HBH_Debug_Param where ProcName = 'HBH_SP_JianLiYuan_GetAQWTableFromAWQ' or ProcName is null or ProcName = '' order by ProcName desc)
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
		 select 'HBH_SP_JianLiYuan_GetAQWTableFromAWQ','@StartDate',IsNull(Convert(varchar,@StartDate,120),'null'),GETDATE()
		 union select 'HBH_SP_JianLiYuan_GetAQWTableFromAWQ','@EndDate',IsNull(Convert(varchar,@EndDate,120),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_GetAQWTableFromAWQ','@IsDropTable',IsNull(cast(@IsDropTable as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_GetAQWTableFromAWQ','ProcSql','exec HBH_SP_JianLiYuan_GetAQWTableFromAWQ '
				+ IsNull('''' + Convert(varchar,@StartDate,120) + '''' ,'null')
				+ ',' + IsNull('''' + Convert(varchar,@EndDate,120) + '''' ,'null')
				+ ',' + IsNull(cast(@IsDropTable as varchar(501)),'null') 
			 ,GETDATE()
	end
end



	declare @SalePriceListCode varchar(125) = '001'
	declare @SysLineNo int = 10
	declare @Now datetime = GetDate();
	declare @CurDate datetime = Convert(date,@Now)
	declare @SheetName varchar(125)
	declare @StartID bigint = -1
	declare @TotalIDCount int = 0
	declare @TotalLineCount int = 0
	declare @DetailLineCount int = 0
	declare @DefaultZero decimal(24,9) = 0
	


	--select OBJECT_Name('lgt_good') 
	
	if(@IsDropTable is not null and @IsDropTable = 1)
	begin

		If OBJECT_ID('lgt_good') is not null
				drop table lgt_good

		If OBJECT_ID('sls_shop') is not null
				drop table sls_shop

		If OBJECT_ID('lgt_depot') is not null
				drop table lgt_depot

		If OBJECT_ID('lgt_dispatchin') is not null
				drop table lgt_dispatchin
				
		If OBJECT_ID('lgt_dispatchin_item') is not null
				drop table lgt_dispatchin_item

	end

	-- ��Ʒ��
	If OBJECT_ID('lgt_good') is null
	begin
		create table lgt_good (
			lgid bigint 
			,sno varchar(125)
			,name varchar(125)
			,std varchar(125)
			,valid varchar(125)
			,ctime datetime
			,utime datetime
			--,lgtid bigint
			--,cuid bigint
			--,applylguid bigint
			--,entrylguid	bigint
			--,lguid	bigint
			--,costlguid bigint
			--,applycode varchar(125)
			--,entrycode varchar(125)
			--,costcode varchar(125)
			--,galias varchar(125)
			--,price decimal(24,9)
			--,incrementcode varchar(125)
			--,pricecircle varchar(125)
			--,shelflife varchar(125)
			--,applymultiple varchar(125)
			--,purchasemultiple varchar(125)
			--,ordercycle varchar(125)
			--,takecycle varchar(125)
			--,maxamount varchar(125)
			--,lfsid varchar(125)
			--,goodseq varchar(125)
			--,processratio varchar(125)
			--,bceil varchar(125)
			--,ifapply varchar(125)
			--,safetyfactor varchar(125)
			--,Safetybin varchar(125)
			--,Batchproduction varchar(125)
			--,processingcycle varchar(125)
			--,tax decimal(24,9)
			--,originalprice decimal(24,9)
			--,brandname varchar(125)
			--,type int
			--,applanbatch varchar(125)
			--,incrementmoney varchar(125)
			--,applycycle varchar(125)
			--,applysafe varchar(125)
			--,barcode varchar(125)
			--,bcheckremainitem varchar(125)
			--,outfixedprice varchar(125)
			--,warningdate varchar(125)
			--,processlguid varchar(125)
			--,processcode varchar(125)
			--,logisticstype varchar(125)
			--,minamount varchar(125)
			--,thousanduse varchar(125)
			--,goodsapplication varchar(125)
			--,xytype varchar(125) 
			--,picname varchar(125)
			--,badddisprice varchar(125)
			--,materatio varchar(125)
			--,pricewarning varchar(125)
			--,bcheckremain varchar(125)
			--,bswdc varchar(125)
			--,ifmaterial varchar(125)
			--,assistlguid varchar(125)
			--,storeenvironm varchar(125)
			--,netcontent varchar(125)
			--,floatrate varchar(125)
			--,estimateldtid varchar(125)
			--,ifpregood varchar(125)
			--,materialtype varchar(125)
			--,checklguid varchar(125)
			--,checkcode varchar(125)
			--,pricingtype varchar(125)
			--,deductrate varchar(125)
 )
	end

	insert into lgt_good
	select *
	from openquery([MYSQL-AWQ],'SELECT 
			lgid 
			,rtrim(sno) as sno
			,name
			,std
			,valid
			,ctime
			,utime
		FROM lgt_good ; ') as tmp
	where
		lgid not in (select lgid
					from lgt_good
					)


	
	-- �ŵ��
	If OBJECT_ID('sls_shop') is null
	begin
		create table sls_shop (
			sid bigint 
			,shopcode varchar(125)
			,shopname varchar(125)
			/*
�ŵ�����ͣ�
3�� ��������
4:  ֱӪ��
5:  ���˵�
6:  �����ͻ�  
			*/
			,stype int
			/*
�ŵ��״̬��
1-��Ч Ĭ��  
0-��Ч
			*/
			,status int

			,utime datetime
		)
	end
	else
	
	insert into sls_shop
	select *
	from openquery([MYSQL-AWQ],'SELECT 
			sid
			,shopcode
			,shopname 
			,stype
			,status 
			,utime 
								FROM sls_shop 
								; ') as tmp
	where
		sid not in (select sid
					from sls_shop
					)


	
	-- �ֿ��
	If OBJECT_ID('lgt_depot') is null
	begin
		create table lgt_depot (
			ldid bigint 
			,sno varchar(125)
			,ldname varchar(125)
			-- �����ŵ�
			,lsid varchar(125)
			/*
�ֿ��״̬��
1-��Ч Ĭ��  
0-��Ч
			*/
			,depotstatus int
			,ctime datetime
			,utime datetime
		)
	end
	
	insert into lgt_depot
	select *
	from openquery([MYSQL-AWQ],'SELECT 
			ldid
			,rtrim(sno) as sno
			,ldname
			,lsid
			,depotstatus 
			,ctime 
			,utime 
								FROM lgt_depot 
								; ') as tmp
	where
		ldid not in (select ldid
					from lgt_depot
					)



	-- ������ⵥ
	If OBJECT_ID('lgt_dispatchin') is null
	begin
		create table lgt_dispatchin (
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

		)
	end
	
	insert into lgt_dispatchin
	select *
	from openquery([MYSQL-AWQ],'SELECT 
			ldiid  
			,code 
			,lsid 
			,ldid 
			,status 
			,ctime 
			,utime 

			,ldoid
			,cuid
			,auid
			,atime
			,ouid
			,comment
			,bsetcheck
			,lrsid
			,nccode
			,arrivetime
			,bcheck
			,buid
			,btime
			,pdatetag
			,ordertype
			
			,confirmuid
			,confirmtime
			,rlrsid
			,returnuid
			,cast(returntime as datetime) as returntime
			,bgenerateorder

								FROM lgt_dispatchin 
								where status = 2
								; ') as tmp
	where
		ldiid not in (select ldiid
					from lgt_dispatchin
					)


	
	
	-- ������ⵥ�������
	If OBJECT_ID('lgt_dispatchin_item') is null
	begin
		create table lgt_dispatchin_item (

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
			
			--,ctime datetime
			--,utime datetime

		)
	end
	
	insert into lgt_dispatchin_item
	select *
	from openquery([MYSQL-AWQ],'SELECT 
			ldiiid
			,ldiid
			,lgid
			,amount
			,damount
			,uprice
			,total
			,icomment
			,iutime
			,batch
			,disprice
			,depotintime
			,originalprice
			,lrsid
			,bcheck
			,checkamount
			,qualifiedamount
			,movebatch
			,salestax
			,salesmode
			,disoriginalprice
			,breturn


								FROM lgt_dispatchin_item 
								; ') as tmp
	where
		ldiiid not in (select ldiiid
					from lgt_dispatchin_item
					)
		and ldiid in (select ldiid
					from lgt_dispatchin
					)




