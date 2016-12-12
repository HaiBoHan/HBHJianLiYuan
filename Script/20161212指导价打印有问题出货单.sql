


select 
	line.DescFlexField_PubDescSeg3
	,line.LotInfo_LotCode

	,lot.DescFlexSegments_PubDescSeg3
	,lotTrl.DescFlexSegments_CombineName

from SM_Ship ship
	inner join SM_SHipLine line
	on line.Ship = ship.ID

	inner join Lot_LotMaster lot
	on line.LotInfo_LotMaster = lot.ID
	inner join Lot_LotMaster_Trl lotTrl
	on lot.ID = lotTrl.ID

where
	ship.DocNo = 'SM1612120365'



/*


select distinct
	ship.DocNo
	,ship.BusinessDate
	,line.DocLineNo
	,line.DescFlexField_PubDescSeg3
	,line.DescFlexField_PubDescSeg4
	,line.DescFlexField_PubDescSeg5
from SM_Ship ship
	inner join SM_ShipLine line
	on ship.ID = line.Ship

	inner join Lot_LotMaster lot
	on lot.ID = line.LotInfo_LotMaster
	inner join Lot_LotMaster_Trl lotTrl
	on lot.ID = lotTrl.ID

where 
	ship.BusinessDate > '2016-12-01'

	and (line.DescFlexField_PubDescSeg3 is null
		or line.DescFlexField_PubDescSeg3 = ''
		)

	-- and line.FinallyPriceTC != lot.DescFlexSegments_PrivateDescSeg1


*/