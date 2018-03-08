

-- 2018-03-08 wf 奥琦玮单号，写到收货行上，查询太慢
if not exists(select name from sys.indexes where name = 'CustIndex_RcvLine_DescFlexSegments_PrivateDescSeg10')
begin
	CREATE INDEX CustIndex_RcvLine_DescFlexSegments_PrivateDescSeg10
	ON [PM_RcvLine] 
	(
		[DescFlexSegments_PrivateDescSeg10] ASC
	) 
end




