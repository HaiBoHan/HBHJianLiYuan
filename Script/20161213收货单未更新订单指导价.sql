






select 
	rcv.ID				收货单ID
	,line.ID			收货单行ID
	,poline.DescFlexSegments_PubDescSeg3	[订单指导价(更新行私有段5)]
	,org.Code			组织编码
	,orgTrl.Name		组织名称
	,rcv.BusinessDate		收货单日期
	,rcv.DocNo		收货单号
	,line.DocLineNo	收货单行号
	,line.DescFlexSegments_PrivateDescSeg5	[收货单行订单指导价(行旧数据)]
from PM_Receivement rcv
	inner join PM_RcvLine line
	on rcv.ID = line.Receivement
	
	inner join PM_POLine poline
	on line.SrcPO_SrcDocLine_EntityID = poline.ID
	inner join Base_Organization org
	on rcv.Org = org.ID 
	inner join Base_Organization_Trl orgTrl
	on org.ID = orgTrl.ID

where
	line.DescFlexSegments_PrivateDescSeg5 != poline.DescFlexSegments_PubDescSeg3

order by
	org.Code
	,rcv.BusinessDate
	,rcv.DocNo







