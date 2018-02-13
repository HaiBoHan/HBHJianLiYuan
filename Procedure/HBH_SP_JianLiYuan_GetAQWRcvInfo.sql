
if exists(select * from sys.objects where name='HBH_SP_JianLiYuan_GetAQWRcvInfo')
-- ���������ɾ��
	drop proc HBH_SP_JianLiYuan_GetAQWRcvInfo
go
-- �����洢����
create proc HBH_SP_JianLiYuan_GetAQWRcvInfo  (
 @StartDate datetime = null
,@EndDate datetime = null
,@DocNo varchar(125) = ''
)
with encryption
as
	SET NOCOUNT ON;

	
declare @SysMlFlag varchar(11) = 'zh-CN'


if exists(select name from sys.objects where name = 'HBH_Debug_Param')
begin
	declare @Debugger bit = (select top 1 Debugger from HBH_Debug_Param where ProcName = 'HBH_SP_JianLiYuan_GetAQWRcvInfo' or ProcName is null or ProcName = '' order by ProcName desc)
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
		 select 'HBH_SP_JianLiYuan_GetAQWRcvInfo','@StartDate',IsNull(Convert(varchar,@StartDate,120),'null'),GETDATE()
		 union select 'HBH_SP_JianLiYuan_GetAQWRcvInfo','@EndDate',IsNull(Convert(varchar,@EndDate,120),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_GetAQWRcvInfo','@DocNo',IsNull(cast(@DocNo as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_GetAQWRcvInfo','ProcSql','exec HBH_SP_JianLiYuan_GetAQWRcvInfo '
				+ IsNull('''' + Convert(varchar,@StartDate,120) + '''' ,'null')
				+ ',' + IsNull('''' + Convert(varchar,@EndDate,120) + '''' ,'null')
				+ ',' + IsNull(cast(@DocNo as varchar(501)),'null') 
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
	



If OBJECT_ID('tempdb..#tmp_hbh_PageHead') is not null
	Drop Table #tmp_hbh_PageHead


select top 500
	head.*
into #tmp_hbh_PageHead
from lgt_dispatchin as head
where 1=1
/*
1�������
2�������
3����ȷ�ϴ����
*/
	and head.status = ('2')
	
	-- ��ʼ����
	and (@StartDate is null 
		or @StartDate <= '2010-01-01'
		or @StartDate <= Convert(varchar(10),head.Sndate,120)
		)
	-- ��������
	and (@EndDate is null 
		or @EndDate <= '2010-01-01'
		or @EndDate >= Convert(varchar(10),head.Sndate,120)
		)

	--����
	and (@DocNo is null or @DocNo = ''
		or head.Code like @DocNo
		)

	-- ����������
	and head.Code not in (select u9Doc.DescFlexField_PrivateDescSeg1
					from PM_Receivement u9Doc
					where u9Doc.DescFlexField_PrivateDescSeg1 is not null
						and u9Doc.DescFlexField_PrivateDescSeg1 != ''
					)


order by 
	head.arrivetime desc




