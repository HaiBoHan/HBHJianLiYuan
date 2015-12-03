

/*

update SM_ShipLine
set DescFlexField_PrivateDescSeg1 = dbo.HBH_Fn_GetString( dbo.HBH_Fn_GetDecimal(DescFlexField_PubDescSeg3,0) * ShipQtyTUAmount )
	,DescFlexField_PrivateDescSeg2 = dbo.HBH_Fn_GetString( dbo.HBH_Fn_GetDecimal(lot.DescFlexSegments_PubDescSeg3,0) * shipline.ShipQtyTUAmount )
from SM_ShipLine shipline
	left join Lot_LotMaster lot
	on shipline.LotInfo_LotMaster = lot.ID
where DescFlexField_PrivateDescSeg1 = ''

*/


update SM_ShipLine
set DescFlexField_PrivateDescSeg1 = dbo.HBH_Fn_GetString( dbo.HBH_Fn_GetDecimal(DescFlexField_PubDescSeg3,0) * ShipQtyTUAmount )
from SM_ShipLine shipline
	left join Lot_LotMaster lot
	on shipline.LotInfo_LotMaster = lot.ID
where DescFlexField_PrivateDescSeg1 = ''



update SM_ShipLine
set DescFlexField_PrivateDescSeg2 = dbo.HBH_Fn_GetString( dbo.HBH_Fn_GetDecimal(lot.DescFlexSegments_PubDescSeg3,0) * shipline.ShipQtyTUAmount )
from SM_ShipLine shipline
	left join Lot_LotMaster lot
	on shipline.LotInfo_LotMaster = lot.ID
where DescFlexField_PrivateDescSeg2 = ''




select 
	shipline.DescFlexField_PrivateDescSeg1
	,shipline.DescFlexField_PrivateDescSeg2
	,shipline.DescFlexField_PubDescSeg3
	,shiplineTrl.DescFlexField_CombineName
from SM_ShipLine shipline
	inner join SM_ShipLine_Trl shiplineTrl
	on shipline.ID = shiplineTrl.ID
where DescFlexField_PrivateDescSeg1 = ''


select 
	shipline.DescFlexField_PrivateDescSeg1
	,shipline.DescFlexField_PrivateDescSeg2
	,shiplineTrl.DescFlexField_CombineName
from SM_ShipLine shipline
	inner join SM_ShipLine_Trl shiplineTrl
	on shipline.ID = shiplineTrl.ID
where DescFlexField_PrivateDescSeg1 != ''

/*
select *
from SM_ShipLine shipline
where DescFlexField_PrivateDescSeg1 != ''
*/

