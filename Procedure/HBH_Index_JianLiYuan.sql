

-- 2018-03-08 wf �����ⵥ�ţ�д���ջ����ϣ���ѯ̫��
if not exists(select name from sys.indexes where name = 'CustIndex_RcvLine_DescFlexSegments_PrivateDescSeg10')
begin
	CREATE INDEX CustIndex_RcvLine_DescFlexSegments_PrivateDescSeg10
	ON [PM_RcvLine] 
	(
		[DescFlexSegments_PrivateDescSeg10] ASC
	) 
end




