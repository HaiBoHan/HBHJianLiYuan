
if exists(select * from sys.objects where name='HBH_SP_JianLiYuan_GetAQWRcvInfo')
-- ���������ɾ��
	drop proc HBH_SP_JianLiYuan_GetAQWRcvInfo
go
/*
--  1001709120704165	021	02101	wangenbao
	exec HBH_SP_JianLiYuan_GetAQWRcvInfo '2018-03-02',null, '',1001709120704165

*/

-- �����洢����
create proc HBH_SP_JianLiYuan_GetAQWRcvInfo  (
@OrgID bigint = -1
,@StartDate datetime = null
,@EndDate datetime = null
,@DocNo varchar(125) = ''
,@LoginUser varchar(125) = ''
)
with encryption
as

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
		select 'HBH_SP_JianLiYuan_GetAQWRcvInfo','@OrgID',IsNull(cast(@OrgID as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_GetAQWRcvInfo','@StartDate',IsNull(Convert(varchar,@StartDate,120),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_GetAQWRcvInfo','@EndDate',IsNull(Convert(varchar,@EndDate,120),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_GetAQWRcvInfo','@DocNo',IsNull(cast(@DocNo as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_GetAQWRcvInfo','@LoginUser',IsNull(cast(@LoginUser as varchar(max)),'null'),GETDATE()
		union select 'HBH_SP_JianLiYuan_GetAQWRcvInfo','ProcSql','exec HBH_SP_JianLiYuan_GetAQWRcvInfo '
				+ IsNull('''' + cast(@OrgID as varchar(501)) + '''' ,'null') 
				+ ',' + IsNull('''' + Convert(varchar,@StartDate,120) + '''' ,'null')
				+ ',' + IsNull('''' + Convert(varchar,@EndDate,120) + '''' ,'null')
				+ ',' + IsNull('''' + cast(@DocNo as varchar(501)) + '''' ,'null') 
				+ ',' + IsNull('''' + cast(@LoginUser as varchar(501)) + '''' ,'null') 
			   ,GETDATE()
	end
end

	SET NOCOUNT ON;

	
	declare @SysMlFlag varchar(11) = 'zh-CN'
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
	
	declare @AqwDeptCodeStart varchar(125) = ''
	declare @AqwDeptName varchar(125) = ''
	
	set @EndDate = DateAdd(second,-1,DateAdd(day,1,@StartDate))

	-- @LoginUser

	select 
		@AqwDeptCodeStart = dept.Code
		,@AqwDeptName = deptTrl.Name
	from Base_User usr
		inner join CBO_Operators opr
		on usr.Contact = opr.Contact
		inner join CBO_Department dept
		on opr.Dept = dept.ID
			and dept.Org = @OrgID
		inner join CBO_Department_Trl deptTrl
		on dept.ID = deptTrl.ID
			and deptTrl.SysMlFlag = @SysMlFlag
	where
		usr.ID = @LoginUser
		
	-- select @AqwDeptCodeStart

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


declare @WhereDocNo varchar(125) = ' 1=1 '
if(@DocNo is not null and @DocNo != '')
begin
	set @WhereDocNo = ' rcvhead.code like ''''%' + @DocNo + '%'''''
end

if (@AqwDeptCodeStart is null or @AqwDeptCodeStart = '')
begin
	set @Sql = Replace(@Sql,'#OPath#',' and 1=0 '  )
end
else
begin
	set @Sql = Replace(@Sql,'#OPath#'
							--,' and dept.shopname like ''''%' + @AqwDeptCodeStart + '''''' + '
							,' and dept.shopname like ''''%' + @AqwDeptName + '''''' + '
							and rcvhead.arrivetime between ''''' + Convert(varchar(10),@StartDate,120) + ''''' 
							and ''''' + Convert(varchar(19),@EndDate,120) + '''''' + ' 
							and ' + @WhereDocNo
					)
end
	

  print (@Sql)
  exec (@Sql)




If OBJECT_ID('tempdb..#tmp_hbh_PageHead') is not null
	Drop Table #tmp_hbh_PageHead


select top 500
	--rcvhead.*
	
	rcvhead.ldiid  
	,rcvhead.code 
	,rcvhead.lsid 
	,rcvhead.ldid 
	/* ����״̬��
1�������
2�������
3����ȷ�ϴ����
	*/
	,status = case rcvhead.status 
				when '1' then '�����'
				when '2' then '�����'
				when '3' then '��ȷ�ϴ����'
				else cast(rcvhead.status as varchar(125))
				end
	,rcvhead.ctime 
	,rcvhead.utime 

	,rcvhead.ldoid
	,rcvhead.cuid
	,rcvhead.auid
	,rcvhead.atime
	,rcvhead.ouid
	,rcvhead.comment
	/*
�Ƿ����:
  0����  Ĭ��
  1����
	*/
	,bsetcheck = case rcvhead.bsetcheck 
					when '0' then '��'
					when '1' then '��'
					else cast(rcvhead.bsetcheck as varchar(125))
					end
	,rcvhead.lrsid
	,rcvhead.nccode
	,rcvhead.arrivetime
	/*
�Ƿ���飺 
0���� Ĭ��
1����
	*/
	--,rcvhead.bcheck
	,bcheck = case rcvhead.bcheck 
					when '0' then '��'
					when '1' then '��'
					else cast(rcvhead.bcheck as varchar(125))
					end
	,rcvhead.buid
	,rcvhead.btime
	,rcvhead.pdatetag
	/*
��������:
1.	�������  Ĭ��
2.	��������
3.	�˻��ŵ����
4.	����˲���
	*/
	--,rcvhead.ordertype
	,ordertype = case rcvhead.ordertype 
					when '1' then '�������'
					when '2' then '��������'
					when '3' then '�˻��ŵ����'
					when '4' then '����˲���'
					else cast(rcvhead.ordertype as varchar(125))
					end
			
	,rcvhead.confirmuid
	,rcvhead.confirmtime
	,rcvhead.rlrsid
	,rcvhead.returnuid
	,rcvhead.returntime
	/*
�Ƿ������ɵ������õ���
0����   Ĭ��
1����
	*/
	--,rcvhead.bgenerateorder
	,bgenerateorder = case rcvhead.bgenerateorder 
					when '0' then '��'
					when '1' then '��'
					else cast(rcvhead.bgenerateorder as varchar(125))
					end

	
	--,wh.sno
	--,wh.ldname

	--,dept.shopcode
	--,dept.shopname

	,rcvhead.sno
	,rcvhead.ldname
	,rcvhead.shopcode
	,rcvhead.shopname

into #tmp_hbh_PageHead
--from lgt_dispatchin as rcvhead

--	left join lgt_depot wh
--	on rcvhead.ldid = wh.ldid

--	left join sls_shop dept
--	--on rcvhead.lsid = dept.[sid]
--	on wh.lsid = dept.[sid]

from #tmp_hbh_dispatchin rcvhead

where 1=1
/*
1�������
2�������
3����ȷ�ϴ����
*/
	and rcvhead.status = ('2')
	
	---- ��ʼ����
	--and (@StartDate is null 
	--	or @StartDate <= '2010-01-01'
	--	or @StartDate <= Convert(varchar(10),rcvhead.arrivetime,120)
	--	)
	---- ��������
	--and (@EndDate is null 
	--	or @EndDate <= '2010-01-01'
	--	or @EndDate >= Convert(varchar(10),rcvhead.arrivetime,120)
	--	)

	--����
	and (@DocNo is null or @DocNo = ''
		or rcvhead.Code like '%' + @DocNo + '%'
		)

	-- ����������
	--and rcvhead.Code not in (select u9Doc.DescFlexField_PrivateDescSeg2
	--				from PM_Receivement u9Doc
	--				where u9Doc.DescFlexField_PrivateDescSeg2 is not null
	--					and u9Doc.DescFlexField_PrivateDescSeg2 != ''
	--				)
	and rcvhead.Code not in (select distinct u9Doc.DescFlexSegments_PrivateDescSeg10
					from PM_RcvLine u9Doc
					where u9Doc.DescFlexSegments_PrivateDescSeg10 is not null
						and u9Doc.DescFlexSegments_PrivateDescSeg10 != ''
					)


order by 
	rcvhead.arrivetime desc




select *
from #tmp_hbh_PageHead




