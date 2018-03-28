

declare @StartDate varchar(125) = '2018-03-24'
declare @EndDate varchar(125) = '2018-03-28'


declare @Sql varchar(max) = '
	select *
	from openquery([MYSQL-AWQ],''SELECT 
			
			rcvhead.arrivetime as ''''日期''''
			,dept.shopname as ''''部门''''

			, count(1) as ''''单据个数''''

					FROM lgt_dispatchin rcvhead

						left join lgt_depot wh
						on rcvhead.ldid = wh.ldid

						left join sls_shop dept
						on wh.lsid = dept.sid
					where rcvhead.status = 2
						and rcvhead.arrivetime >= ''''2018-03-13''''
						-- 行数量汇总大于0
						and (select sum(ifnull(rcvline.amount,0) + ifnull(rcvline.damount,0))
								from lgt_dispatchin_item rcvline
								where rcvline.ldiid = rcvhead.ldiid
								) > 0
						-- and rcvhead.
						 and ( rcvhead.arrivetime between ''''' + @StartDate + ''''' 
						and ''''' + @EndDate + ' 23:59:59''''
						and  1=1 )
						 -- and (dept.shopname like ''''%南京大唐'''')

				group by 
			rcvhead.arrivetime
			,dept.shopname

			order by
			rcvhead.arrivetime
			,dept.shopname
				
					; '') tmp';



	
  print (@Sql)
  exec (@Sql)

