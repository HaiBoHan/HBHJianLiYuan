

select
	
	rcv.DocNo
	,rcv.BusinessDate
	,line.DocLineNo
	,lot.DescFlexSegments_PrivateDescSeg1
	, dbo.HBH_Fn_GetString(line.TaxRate)
	,lot.DescFlexSegments_PrivateDescSeg2 
	,dbo.HBH_Fn_GetString(line.FinallyPriceTC)
	,lot.DescFlexSegments_PubDescSeg3 
	,line.DescFlexSegments_PubDescSeg3
	,lot.DescFlexSegments_PubDescSeg4 
	,line.DescFlexSegments_PubDescSeg4
	,lot.DescFlexSegments_PubDescSeg5
	,line.DescFlexSegments_PubDescSeg5
	
	,lot.DescFlexSegments_PubDescSeg11 
	,line.DescFlexSegments_PubDescSeg11

from PM_Receivement rcv
	inner join PM_RcvLine line
	on rcv.ID = line.Receivement
	inner join Lot_LotMaster lot
	on IsNull(line.InvLot,line.RcvLot) = lot.ID

where
	-- 关闭
	rcv.Status = 5
	-- 批号档案最终价 = 0
	and (lot.DescFlexSegments_PrivateDescSeg2  is null
		or lot.DescFlexSegments_PrivateDescSeg2  = ''
	)

order by 
	rcv.BusinessDate
	,rcv.DocNo


/*
	select *
	into tmp_Lot_LotMaster_20161212001
	from Lot_LotMaster
*/

update Lot_LotMaster
set 
	DescFlexSegments_PrivateDescSeg1 = dbo.HBH_Fn_GetString(line.TaxRate)
	,DescFlexSegments_PrivateDescSeg2  = dbo.HBH_Fn_GetString(line.FinallyPriceTC)
	,DescFlexSegments_PubDescSeg3  = line.DescFlexSegments_PubDescSeg3
	,DescFlexSegments_PubDescSeg4  = line.DescFlexSegments_PubDescSeg4
	,DescFlexSegments_PubDescSeg5 = line.DescFlexSegments_PubDescSeg5
	
	,DescFlexSegments_PubDescSeg11  = line.DescFlexSegments_PubDescSeg11

from 
	Lot_LotMaster lot
	inner join PM_RcvLine line
	on IsNull(line.InvLot,line.RcvLot) = lot.ID
	inner join PM_Receivement rcv
	on rcv.ID = line.Receivement
	 

where
	-- 关闭
	rcv.Status = 5
	-- 批号档案最终价 = 0
	and (lot.DescFlexSegments_PrivateDescSeg2  is null
		or lot.DescFlexSegments_PrivateDescSeg2  = ''
	)