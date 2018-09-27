
/*
select *
from HBH_SPParamRecord
order by CreatedOn desc
*/

if exists(select * from sys.objects where name='HBH_BASP_JianLiYuan_DayCheckInLedUse')
-- 如果存在则删掉
	drop proc HBH_BASP_JianLiYuan_DayCheckInLedUse
go
-- 创建存储过程
create proc HBH_BASP_JianLiYuan_DayCheckInLedUse  (
-- @StartDate datetime = null
-- ,@EndDate datetime = null

@请选择过滤年月 varchar(125) = ''
,@请选择大区 varchar(125) = ''
,@请选择区域 varchar(125) = ''
,@请选择部门 varchar(125) = ''
,@请选择开始日期 varchar(125) = ''
,@请选择结束日期 varchar(125) = ''
,@领导用表 varchar(125) = '是'
)
as

    exec HBH_BASP_JianLiYuan_DayCheckIn @请选择过滤年月,@请选择大区,@请选择区域,@请选择部门,@请选择开始日期,@请选择结束日期,@领导用表
