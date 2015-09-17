using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.SM.SO;
using UFSoft.UBF.PL;
using UFIDA.U9.SM.Enums;
using UFIDA.U9.PR.PurchaseRequest;
using U9.VOB.Cus.HBHJianLiYuan.HBHHelper;
using UFIDA.U9.Cust.HBH.Common.CommonLibary;
using UFIDA.U9.CBO.SCM.Supplier;
using UFIDA.U9.Base;
using UFIDA.U9.PR.DemandInterface;
using UFIDA.U9.PR.DemandInterface.Proxy;
using U9.VOB.Cus.HBHJianLiYuan.Proxy;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    public class PR_AfterUpdatedExtend : UFSoft.UBF.Eventing.IEventSubscriber
    {
        public void Notify(params object[] args)
        {
            if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
                return;
            BusinessEntity.EntityKey entitykey = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;

            if (entitykey == null)
                return;
            PR entity = entitykey.GetEntity() as UFIDA.U9.PR.PurchaseRequest.PR;//请购单实体
            if (entity == null)
                return;

            // 审核后，将需求接口表转到采购订单；
            bool isApprove = false;
            if (entity.OriginalData.Status == PRStatusEnum.Approving 
                && entity.Status == PRStatusEnum.Approved
                )
            {
                isApprove = true;
            }
            if (isApprove)
            {
                // UFIDA.U9.PR.DemandInterface.DemandInterface
                // UFIDA.U9.PR.DemandInterface.DemandInterfaceBListUIModel.DemandInterfaceBListUIFormWebPart ; UFIDA.U9.SCM.PM.DIUI.WebPart
                // BtnToPurchase
                // UFIDA.U9.PR.DemandInterface.DemandInterfaceBListUIModel.DemandInterfaceBListUIModelAction
                // private void ToPRPO(bool isAll)
                // DIToFollowingDocBPProxy dIToFollowingDocBPProxy = new DIToFollowingDocBPProxy();

                string opath = string.Format("SrcDocInfo.SrcDoc.EntityID = @SrcDocID");
                DemandInterface.EntityList lstDemand = DemandInterface.Finder.FindAll(opath,new OqlParam(entity.ID));

                if (lstDemand != null
                    && lstDemand.Count > 0
                    )
                {
                //    long OrgID = -1;
                //    DIToFollowingDocBPProxy dIToFollowingDocBPProxy = new DIToFollowingDocBPProxy();

                //    dIToFollowingDocBPProxy.Dikeys = new List<DIKeyDTOData>();

                //    foreach (DemandInterface demand in lstDemand)
                //    {
                //        if (demand != null)
                //        {
                //            if (OrgID <= 0
                //                && demand.CurrentOrgKey != null
                //                )
                //            {
                //                OrgID = demand.CurrentOrgKey.ID;
                //            }

                //            DIKeyDTOData keyDTO = new DIKeyDTOData();

                //            keyDTO.DIID = demand.ID;
                //            keyDTO.Version = (int)demand.SysVersion;
                //            keyDTO.TransferQty = new UFIDA.U9.CBO.DTOs.DoubleQuantityData();
                //            keyDTO.TransferQty.Amount1 = demand.PurQtyTU;
                //            keyDTO.TransferQty.Amount2 = demand.PurQtyTBU;
                //            //keyDTO.TransferQty.UOM1 = new UFIDA.U9.CBO.DTOs.UOMInfoDTOData(demand.TradeUOMKey.ID, demand.TradeUOM.BaseUOMKey.ID, demand.TradeUOM.RatioToBase);
                //            keyDTO.TransferQty.UOM1 = demand.TradeUOM.GetUomData();
                //            keyDTO.TransferQty.UOM2 = demand.TradeBaseUOM.GetUomData();

                //            dIToFollowingDocBPProxy.Dikeys.Add(keyDTO);
                //        }
                //    }

                //    // BP不能跨组织，是个问题
                //    DIToFlowResultDTOData dIToFlowResultDTOData = dIToFollowingDocBPProxy.Do();
                
                    long OrgID = -1;
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
                        }
                    }

                    if (OrgID > 0)
                    {
                        DemandToPOSVProxy proxy = new DemandToPOSVProxy();
                        proxy.PRList = new List<long>();
                        proxy.PRList.Add(entity.ID);

                        proxy.Do(OrgID);
                    }
                }

            }
        }
    }
}
