






select 
	rcv.ID				�ջ���ID
	,line.ID			�ջ�����ID
	,poline.DescFlexSegments_PubDescSeg3	[����ָ����(������˽�ж�5)]
	,org.Code			��֯����
	,orgTrl.Name		��֯����
	,rcv.BusinessDate		�ջ�������
	,rcv.DocNo		�ջ�����
	,line.DocLineNo	�ջ����к�
	,line.DescFlexSegments_PrivateDescSeg5	[�ջ����ж���ָ����(�о�����)]
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







