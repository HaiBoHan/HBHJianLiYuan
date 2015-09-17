namespace U9.VOB.Cus.HBHJianLiYuan
{
	using System;
	using System.Collections.Generic;
	using System.Text; 
	using UFSoft.UBF.AopFrame;	
	using UFSoft.UBF.Util.Context;
    using UFIDA.U9.PR.DemandInterface;
    using UFSoft.UBF.PL;
    using UFIDA.U9.PR.DemandInterface.Proxy;
    using UFIDA.U9.PR.PurchaseRequest;
    using UFIDA.U9.PM.PO.Proxy;
    using UFIDA.U9.PM.PO;
    using UFIDA.U9.PM.Enums;
    using UFSoft.UBF.Business;

	/// <summary>
	/// DemandToPOSV partial 
	/// </summary>	
	public partial class DemandToPOSV 
	{	
		internal BaseStrategy Select()
		{
			return new DemandToPOSVImpementStrategy();	
		}		
	}
	
    //#region  implement strategy	
	/// <summary>
	/// Impement Implement
	/// 
	/// </summary>	
	internal partial class DemandToPOSVImpementStrategy : BaseStrategy
	{
		public DemandToPOSVImpementStrategy() { }

		public override object Do(object obj)
		{						
			DemandToPOSV bpObj = (DemandToPOSV)obj;
			
			//get business operation context is as follows
			//IContext context = ContextManager.Context	
			
			//auto generating code end,underside is user custom code
			//and if you Implement replace this Exception Code...
            //throw new NotImplementedException();

            if (bpObj != null
                && bpObj.PRList != null
                && bpObj.PRList.Count > 0
                )
            {
                foreach (long prID in bpObj.PRList)
                {
                    PR entity = PR.Finder.FindByID(prID);

                    if (entity != null)
                    {
                        string opath = string.Format("SrcDocInfo.SrcDoc.EntityID = @SrcDocID");
                        DemandInterface.EntityList lstDemand = DemandInterface.Finder.FindAll(opath, new OqlParam(entity.ID));

                        if (lstDemand != null
                            && lstDemand.Count > 0
                            )
                        {
                            long OrgID = -1;
                            DIToFollowingDocBPProxy dIToFollowingDocBPProxy = new DIToFollowingDocBPProxy();

                            dIToFollowingDocBPProxy.Dikeys = new List<DIKeyDTOData>();

                            foreach (DemandInterface demand in lstDemand)
                            {
                                if (demand != null)
                                {
                                    if (OrgID <= 0
                                        && demand.CurrentOrgKey != null
                                        )
                                    {
                                        OrgID = demand.CurrentOrgKey.ID;
                                    }

                                    DIKeyDTOData keyDTO = new DIKeyDTOData();

                                    keyDTO.DIID = demand.ID;
                                    keyDTO.Version = (int)demand.SysVersion;
                                    keyDTO.TransferQty = new UFIDA.U9.CBO.DTOs.DoubleQuantityData();
                                    keyDTO.TransferQty.Amount1 = demand.PurQtyTU - demand.TotalToPOQtyTU;
                                    keyDTO.TransferQty.Amount2 = demand.PurQtyTBU - demand.TotalToPOQtyTBU;
                                    //keyDTO.TransferQty.UOM1 = new UFIDA.U9.CBO.DTOs.UOMInfoDTOData(demand.TradeUOMKey.ID, demand.TradeUOM.BaseUOMKey.ID, demand.TradeUOM.RatioToBase);
                                    keyDTO.TransferQty.UOM1 = demand.TradeUOM.GetUomData();
                                    keyDTO.TransferQty.UOM2 = demand.TradeBaseUOM.GetUomData();

                                    dIToFollowingDocBPProxy.Dikeys.Add(keyDTO);
                                }
                            }

                            // BP不能跨组织，是个问题
                            DIToFlowResultDTOData dIToFlowResultDTOData = dIToFollowingDocBPProxy.Do();

                            if (dIToFlowResultDTOData != null
                                && dIToFlowResultDTOData.LineKeyDTOList != null
                                && dIToFlowResultDTOData.LineKeyDTOList.Count > 0
                                )
                            {
                                foreach (LineKeyDTOData lineKey in dIToFlowResultDTOData.LineKeyDTOList)
                                { 
                                    UFIDA.U9.PM.PO.PurchaseOrder po = PurchaseOrder.Finder.FindByID(lineKey.ID);
                                    if (po != null)
                                    {
                                        using (ISession session = Session.Open())
                                        {
                                            po.Status = PODOCStatusEnum.Approving;
                                            po.ActionType = ActivateTypeEnum.ApprovingAct;
                                            po.DGNeedDecompose = false;

                                            session.Commit();
                                        }

                                        po = PurchaseOrder.Finder.FindByID(lineKey.ID);
                                        using (ISession session = Session.Open())
                                        {
                                            po.Status = PODOCStatusEnum.Approved;
                                            po.ActionType = ActivateTypeEnum.ApprovingAct;
                                            po.DGNeedDecompose = false;

                                            session.Commit();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
		}		
	}

    //#endregion
	
	
}