
if exists(select * from sys.objects where name='HBH_SP_JianLiYuan_GetAQWRcvInfo')
-- 如果存在则删掉
	drop proc HBH_SP_JianLiYuan_GetAQWRcvInfo
go
-- 创建存储过程
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
			,Memo varchar(max)	-- 备注
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
				+ ',' + IsNull('''' + cast(@DocNo as varchar(501)) + '''' ,'null') 
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
	rcvhead.*
	
	,wh.sno
	,wh.ldname

	,dept.shopcode
	,dept.shopname

into #tmp_hbh_PageHead
from lgt_dispatchin as rcvhead

	left join lgt_depot wh
	on rcvhead.ldid = wh.ldid

	left join sls_shop dept
	on rcvhead.lsid = dept.[sid]

where 1=1
/*
1：待审核
2：已审核
3：已确认待审核
*/
	and rcvhead.status = ('2')
	
	-- 开始日期
	and (@StartDate is null 
		or @StartDate <= '2010-01-01'
		or @StartDate <= Convert(varchar(10),rcvhead.arrivetime,120)
		)
	-- 结束日期
	and (@EndDate is null 
		or @EndDate <= '2010-01-01'
		or @EndDate >= Convert(varchar(10),rcvhead.arrivetime,120)
		)

	--单号
	and (@DocNo is null or @DocNo = ''
		or rcvhead.Code like @DocNo
		)

	-- 已生单过滤
	and rcvhead.Code not in (select u9Doc.DescFlexField_PrivateDescSeg2
					from PM_Receivement u9Doc
					where u9Doc.DescFlexField_PrivateDescSeg2 is not null
						and u9Doc.DescFlexField_PrivateDescSeg2 != ''
					)


order by 
	rcvhead.arrivetime desc




select *
from #tmp_hbh_PageHead




