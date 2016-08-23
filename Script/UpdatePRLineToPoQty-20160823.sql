
/*
select *
into tmp_PR_PRLine_20160823001
from PR_PRLine
*/



select top 1000 po.Status,po.CreatedBy,po.DocNo

	,prline.CurOrgToPOQtyTU
	,prline.CurOrgToPOQtyTUBeforeDoubleQty
	,prline.ToPOQtyTUBeforeDoubleQty
	,prline.TotalToPOQtyTU

	,poline.PurQtyTU
	-- ,*
from PM_PurchaseOrder po
	inner join PM_POLine poline
	on po.ID = poline.PurchaseOrder
	inner join PR_PRLine prline
	on prline.ID = poline.SrcDocInfo_SrcDocLine_EntityID
where 1=1
	and po.Status = 0
	and po.CreatedBy = 'ºÂ½¨Ï¼'
	-- and po.DocNo = 'PO01608230054'
order by po.CreatedOn desc



update PR_PRLine
set CurOrgToPOQtyTU = poline.PurQtyTU
	,CurOrgToPOQtyTUBeforeDoubleQty = poline.PurQtyTU
	,ToPOQtyTUBeforeDoubleQty = poline.PurQtyTU
	,TotalToPOQtyTU = poline.PurQtyTU

from PM_PurchaseOrder po
	inner join PM_POLine poline
	on po.ID = poline.PurchaseOrder
	inner join PR_PRLine prline
	on prline.ID = poline.SrcDocInfo_SrcDocLine_EntityID
where 1=1
	and po.Status = 0
	and po.CreatedBy = 'ºÂ½¨Ï¼'
	-- and po.DocNo = 'PO01608230054'



select top 1000 po.Status,po.CreatedBy,po.DocNo

	,prline.CurOrgToPOQtyTU
	,prline.CurOrgToPOQtyTUBeforeDoubleQty
	,prline.ToPOQtyTUBeforeDoubleQty
	,prline.TotalToPOQtyTU

	,poline.PurQtyTU
	-- ,*
from PM_PurchaseOrder po
	inner join PM_POLine poline
	on po.ID = poline.PurchaseOrder
	inner join PR_PRLine prline
	on prline.ID = poline.SrcDocInfo_SrcDocLine_EntityID
where 1=1
	and po.Status = 0
	and po.CreatedBy = 'ºÂ½¨Ï¼'
	-- and po.DocNo = 'PO01608230054'
order by po.CreatedOn desc

