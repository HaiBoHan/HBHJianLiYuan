

select 
	DATEDIFF(Minute,ship.ApproveDate,ship.CreatedOn)
	,ship.DocNo
	, ship.Status
	, ship.DescFlexField_PrivateDescSeg1
	, ship.DescFlexField_PrivateDescSeg2
	, ship.DescFlexField_PrivateDescSeg3

	, ship.ApproveDate , ship.CreatedOn
	-- , *
from SM_Ship ship
	inner join SM_ShipDocType doctype
	on ship.DocumentType = doctype.ID

where
	doctype.Code = 'SM2'
	-- and DateAdd(ship.ApproveDate,ship.CreatedOn)

	and ( DATEDIFF(Minute,ship.ApproveDate,ship.CreatedOn) not between -5 and 5
		or ship.DocNo in ('SM2015120200') 
		or ship.DescFlexField_PrivateDescSeg3 in ('0011512030085'
												,'0011512230070'
												,'0011512240140'
												,'0011512030085'
												)
		)




