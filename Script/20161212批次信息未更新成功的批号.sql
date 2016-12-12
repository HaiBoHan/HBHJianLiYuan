



select  
	newLot.ID									[����ID]
	,newLot.LotCode								[����]
	,newLot.DescFlexSegments_PrivateDescSeg1	[˰��(ȫ�ֶ�1)�ջ���˰��]
	-- , dbo.HBH_Fn_GetString(line.TaxRate)
	,newLot.DescFlexSegments_PrivateDescSeg2	[ָ����(ȫ�ֶ�2)�ջ������ռ�]
	-- ,dbo.HBH_Fn_GetString(line.FinallyPriceTC)
	,newLot.DescFlexSegments_PubDescSeg3		[ָ������(������3)�ջ���ָ������]
	-- ,line.DescFlexSegments_PubDescSeg3
	,newLot.DescFlexSegments_PubDescSeg4		[�ۿ���(������4)�ջ����ۿ���]
	-- ,line.DescFlexSegments_PubDescSeg4
	,newLot.DescFlexSegments_PubDescSeg5		[�ۿ۶�(������5)�ջ����ۿ۶�]
	-- ,line.DescFlexSegments_PubDescSeg5
	
	,newLot.DescFlexSegments_PubDescSeg11		[����С����������(������11)]
	-- ,line.DescFlexSegments_PubDescSeg11

	,rcv.DocNo									��ⵥ��
	,rcv.BusinessDate							�������

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
	-- �ر�
	rcv.Status = 5
	-- ���ŵ������ռ� = 0
	and (lot.DescFlexSegments_PrivateDescSeg2  is null
		or lot.DescFlexSegments_PrivateDescSeg2  = ''
	)

	and newLotTrl.DescFlexSegments_CombineName = '#@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@##@#'

--order by 
--	rcv.BusinessDate
--	,rcv.DocNo



