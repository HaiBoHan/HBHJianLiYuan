
/*
select *
from HBH_SPParamRecord
order by CreatedOn desc
*/

if exists(select * from sys.objects where name='HBH_BASP_JianLiYuan_DayCheckInLedUse')
-- ���������ɾ��
	drop proc HBH_BASP_JianLiYuan_DayCheckInLedUse
go
-- �����洢����
create proc HBH_BASP_JianLiYuan_DayCheckInLedUse  (
-- @StartDate datetime = null
-- ,@EndDate datetime = null

@��ѡ��������� varchar(125) = ''
,@��ѡ����� varchar(125) = ''
,@��ѡ������ varchar(125) = ''
,@��ѡ���� varchar(125) = ''
,@��ѡ��ʼ���� varchar(125) = ''
,@��ѡ��������� varchar(125) = ''
,@�쵼�ñ� varchar(125) = '��'
)
as

    exec HBH_BASP_JianLiYuan_DayCheckIn @��ѡ���������,@��ѡ�����,@��ѡ������,@��ѡ����,@��ѡ��ʼ����,@��ѡ���������,@�쵼�ñ�
