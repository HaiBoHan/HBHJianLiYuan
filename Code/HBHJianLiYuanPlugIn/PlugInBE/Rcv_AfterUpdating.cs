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
using UFIDA.U9.CBO.SCM.Enums;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    public class Rcv_AfterUpdating : UFSoft.UBF.Eventing.IEventSubscriber
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

            // 采购收货才更新；
            if (entity.ReceivementType == ReceivementTypeEnum.RCV)
            {
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

                // 弃审，要先弃审下游 出货单、再删除出货单、再弃审收货单；否则报负库存；
                // （要先删出货、后弃审），所以改到了 AfterUpdating 中做；
                if (isUnApproveAction)
                {
                    RcvToShipSVProxy toShipProxy = new RcvToShipSVProxy();

                    toShipProxy.IsRemove = true;
                    toShipProxy.RcvIDs = new List<long>();
                    toShipProxy.RcvIDs.Add(entity.ID);

                    toShipProxy.Do();
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
        }
    }
}
