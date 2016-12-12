



select  
	newLot.ID									[批号ID]
	,newLot.LotCode								[批号]
	,newLot.DescFlexSegments_PrivateDescSeg1	[税率(全局段1)收货单税率]
	-- , dbo.HBH_Fn_GetString(line.TaxRate)
	,newLot.DescFlexSegments_PrivateDescSeg2	[指导价(全局段2)收货单最终价]
	-- ,dbo.HBH_Fn_GetString(line.FinallyPriceTC)
	,newLot.DescFlexSegments_PubDescSeg3		[指导单价(公共段3)收货单指导单价]
	-- ,line.DescFlexSegments_PubDescSeg3
	,newLot.DescFlexSegments_PubDescSeg4		[折扣率(公共段4)收货单折扣率]
	-- ,line.DescFlexSegments_PubDescSeg4
	,newLot.DescFlexSegments_PubDescSeg5		[折扣额(公共段5)收货单折扣额]
	-- ,line.DescFlexSegments_PubDescSeg5
	
	,newLot.DescFlexSegments_PubDescSeg11		[批号小灶物料名称(公共段11)]
	-- ,line.DescFlexSegments_PubDescSeg11

	,rcv.DocNo									入库单号
	,rcv.BusinessDate							入库日期

from PM_Receivement rcv
	inner join PM_RcvLine line
	on rcv.ID = line.Receivement
	inner join tmp_Lot_LotMaster_20161212001 lot
	on IsNull(line.InvLot,line.RcvLot) = lot.ID

	inner join Lot_LotMaster newLot
	on lot.ID = newLot.ID
	inner join Lot_LotMaster_Trl newLotTrl
	on newLotTrl.ID = newLot.ID

where
	-- 关闭
	rcv.Status = 5
	-- 批号档案最终价 = 0
	and (lot.DescFlexSegments_PrivateDescSeg2  is null
		or lot.DescFlexSegments_PrivateDescSeg2  = ''
	)

	and newLotTrl.DescFlexSegments_CombineName = '#@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@#'

--order by 
--	rcv.BusinessDate
--	,rcv.DocNo



