using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.SM.Ship;
using U9.VOB.Cus.HBHJianLiYuan.HBHHelper;
using UFIDA.U9.PM.Rcv;
using UFIDA.U9.PM.PO;
using UFIDA.U9.PM.Enums;
using U9.VOB.Cus.HBHJianLiYuan.Proxy;
using UFIDA.U9.ISV.PO.Proxy;
using UFIDA.U9.ISV.PO;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    public class Rcv_AfterUpdated : UFSoft.UBF.Eventing.IEventSubscriber
    {
        public void Notify(params object[] args)
        {
            if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
                return;
            BusinessEntity.EntityKey key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;

            if (key == null)
                return;
            Receivement entity = key.GetEntity() as Receivement;//出货单实体
            if (entity == null)
                return;
            bool isApproveAction = false;
            bool isUnApproveAction = false;
            // 审核操作
            if (entity.OriginalData.Status != RcvStatusEnum.Closed
                && entity.Status == RcvStatusEnum.Closed
                )
            {
                isApproveAction = true;
            }
            // 弃审操作
            if (entity.OriginalData.Status == RcvStatusEnum.Closed
                && entity.Status == RcvStatusEnum.Opened
                )
            {
                isUnApproveAction = true;
            }
            //提交，单价写入批号主档
            using (ISession session = Session.Open())
            {
                bool isUpdated = false;
                foreach (RcvLine line in entity.RcvLines)
                {
                    if (isApproveAction)
                    {
                        if (line.RcvLot != null)
                        {
                            UFIDA.U9.Lot.LotMaster lotMaster = line.RcvLot;
                            UpdateLotPrice(lotMaster, line);

                            isUpdated = true;
                        }

                        // ShangLuo用的是InvLot，估计这里也是
                        if (line.InvLot != null && line.InvLot != null)
                        {
                            // UFIDA.U9.Lot.LotMaster lotMaster = UFIDA.U9.Lot.LotMaster.Finder.FindByID(line.LotInfo.LotMaster.ID);
                            // lotMaster.DescFlexSegments.PrivateDescSeg1 = line.FinallyPriceTC.ToString();
                            UFIDA.U9.Lot.LotMaster lotMaster = line.InvLot;
                            UpdateLotPrice(lotMaster, line);

                            isUpdated = true;
                        }
                    }
                }

                // 甚至都不需要开Session,后面可以试试
                // Updated ,应该需要开Session
                if (isUpdated)
                {
                    session.Commit();
                }

                // 更新来源订单为关闭状态
                // UFIDA.U9.PM.PurchaseOrderUIModel.PurchaseOrderMainUIFormWebPart       MenuClose
                // MenuClose_Click_Extend
                long srcDocOrg = -1;
                List<long> lstPO = new List<long>();
                foreach (RcvLine line in entity.RcvLines)
                {
                    if (line != null
                        && line.SrcDoc != null
                        && line.SrcDoc.SrcDoc != null
                        && line.SrcDoc.SrcDoc.EntityID > 0
                        )
                    {
                        long srcPOID = line.SrcDoc.SrcDoc.EntityID;

                        if (!lstPO.Contains(srcPOID))
                        {
                            lstPO.Add(srcPOID);
                        }

                        if(srcDocOrg <= 0
                            && line.SrcDoc.SrcDocOrgKey != null 
                            )
                        {
                            srcDocOrg = line.SrcDoc.SrcDocOrgKey.ID;
                        }
                    }
                }

                if (lstPO.Count > 0)
                {
                    if (PubConfig.Const_TwoStage)
                    {
                        // 审核操作，整单关闭来源订单
                        if (isApproveAction)
                        {
                            //bool isClose = false;
                            ClosePOLineSRVProxy closeProxy = new ClosePOLineSRVProxy();

                            closeProxy.POLineKeyDTOs = new List<UFIDA.U9.ISV.PO.POLineKeyDTOData>();

                            foreach (long srcPOID in lstPO)
                            {
                                PurchaseOrder po = PurchaseOrder.Finder.FindByID(srcPOID);

                                // 关闭服务里，没有设置动作
                                // // UFIDA.U9.PM.PO.SinglePoShipLineBPImpementStrategy
                                // 已审核，则关闭
                                if (po != null
                                    && po.Status == PODOCStatusEnum.Approved
                                    )
                                {
                                    // po.Status = PODOCStatusEnum.Closed;
                                    //po.ActionType = ActivateTypeEnum.CloseAct;

                                    foreach (POLine line in po.POLines)
                                    {
                                        //if (line.Status == PODOCStatusEnum.Approved)
                                        //{
                                        //    line.Status = PODOCStatusEnum.ClosedShort;
                                        //}
                                        // line.TotalRecievedQtyTU
                                        foreach (POShipLine subline in line.POShiplines)
                                        {
                                            if (subline.Status == PODOCStatusEnum.Approved
                                                && line.Status == PODOCStatusEnum.Approved
                                                )
                                            {
                                                //subline.Status = PODOCStatusEnum.ClosedShort;

                                                POLineKeyDTOData lineDTO = new POLineKeyDTOData();
                                                lineDTO.LineKeyDTO = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                                                lineDTO.LineKeyDTO.ID = line.ID;

                                                closeProxy.POLineKeyDTOs.Add(lineDTO);

                                                break;
                                            }
                                            // subline.TotalArriveQtyTU
                                            // subline.TotalRecievedQtyTU
                                        }
                                    }

                                    //isUpdated = true;
                                }
                            }

                            if (closeProxy.POLineKeyDTOs.Count > 0)
                            {   // 2017-05-05 wf  暂时先取消掉这个整单关闭功能吧；  现场收货单审批流审核报错，终审失败..."订单子行不是已核准状态，不能执行[关闭]操作"
                                closeProxy.Do(srcDocOrg);
                            }
                        }
                        // 弃审操作，整单打开来源订单
                        else if (isUnApproveAction)
                        {
                            OpenPOLineSRVProxy openProxy = new OpenPOLineSRVProxy();

                            openProxy.POLineKeyDTOs = new List<UFIDA.U9.ISV.PO.POLineKeyDTOData>();

                            foreach (long srcPOID in lstPO)
                            {
                                PurchaseOrder po = PurchaseOrder.Finder.FindByID(srcPOID);

                                // 短缺关闭，则打开
                                if (po != null
                                    //&& po.Status == PODOCStatusEnum.Approved
                                    )
                                {
                                    // 有行是短缺关闭的,才执行打开
                                    if (IsCanOpen(po))
                                    {
                                        //po.Status = PODOCStatusEnum.Approved;
                                        //po.ActionType = ActivateTypeEnum.OpenAct;

                                        foreach (POLine line in po.POLines)
                                        {
                                            //if (line.Status == PODOCStatusEnum.ClosedShort)
                                            //{
                                            //    line.Status = PODOCStatusEnum.Approved;
                                            //}

                                            foreach (POShipLine subline in line.POShiplines)
                                            {
                                                if (subline.Status == PODOCStatusEnum.ClosedShort)
                                                {
                                                    //subline.Status = PODOCStatusEnum.Approved;

                                                    POLineKeyDTOData lineDTO = new POLineKeyDTOData();
                                                    lineDTO.LineKeyDTO = new UFIDA.U9.Base.DTOs.IDCodeNameDTOData();
                                                    lineDTO.LineKeyDTO.ID = line.ID;

                                                    openProxy.POLineKeyDTOs.Add(lineDTO);

                                                    break;
                                                }
                                            }
                                        }

                                        //isUpdated = true;
                                    }
                                }
                            }


                            if (openProxy.POLineKeyDTOs.Count > 0)
                            {
                                openProxy.Do(srcDocOrg);
                            }
                        }
                    }
                }

                // 审核，则自动生成领料单
                if (isApproveAction)
                {
                    RcvToShipSVProxy toShipProxy = new RcvToShipSVProxy();

                    toShipProxy.RcvIDs = new List<long>();
                    toShipProxy.RcvIDs.Add(entity.ID);

                    if (entity.OrgKey != null)
                    {
                        toShipProxy.Do(entity.OrgKey.ID);
                    }
                }
                // 弃审，要先弃审下游 出货单、再删除出货单、再弃审收货单；否则报负库存；
                // （要先删出货、后弃审），所以改到了 AfterUpdating 中做；
                else if (isUnApproveAction)
                {
                    //RcvToShipSVProxy toShipProxy = new RcvToShipSVProxy();

                    //toShipProxy.IsRemove = true;
                    //toShipProxy.RcvIDs = new List<long>();
                    //toShipProxy.RcvIDs.Add(entity.ID);

                    //toShipProxy.Do();
                }
            }
        }

        /// <summary>
        /// 有行是短缺关闭的
        /// </summary>
        /// <param name="po"></param>
        /// <returns></returns>
        private bool IsCanOpen(PurchaseOrder po)
        {
            // 有行是短缺关闭的
            if (po != null
                && po.POLines != null
                )
            {
                foreach (POLine line in po.POLines)
                {
                    if (line.Status == PODOCStatusEnum.ClosedShort)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void UpdateLotPrice(UFIDA.U9.Lot.LotMaster lotMaster, RcvLine line)
        {
            LotMasterHelper.SetTaxRate(lotMaster.DescFlexSegments, line.TaxRate);
            LotMasterHelper.SetFinallyPrice(lotMaster.DescFlexSegments, line.FinallyPriceTC);
            DescFlexFieldHelper.SetPreDiscountPrice(lotMaster.DescFlexSegments, line.DescFlexSegments);
            DescFlexFieldHelper.SetDiscountRate(lotMaster.DescFlexSegments, line.DescFlexSegments);
            DescFlexFieldHelper.SetDiscountLimit(lotMaster.DescFlexSegments, line.DescFlexSegments);

            // 小灶物料名称 = 公共段11
            DescFlexFieldHelper.SetOnceItemNameField(lotMaster.DescFlexSegments, line.DescFlexSegments);

        }
    }
}
