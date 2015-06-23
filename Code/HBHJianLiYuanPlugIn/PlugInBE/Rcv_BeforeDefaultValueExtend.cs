using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.SM.SO;
using UFSoft.UBF.PL;
using UFIDA.U9.SM.Enums;
using UFIDA.U9.PM.Rcv;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    public class Rcv_BeforeDefaultValueExtend : UFSoft.UBF.Eventing.IEventSubscriber
    {
        public void Notify(params object[] args)
        {
            if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
                return;
            BusinessEntity.EntityKey key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;

            if (key == null)
                return;
            UFIDA.U9.PM.Rcv.Receivement rcv = key.GetEntity() as UFIDA.U9.PM.Rcv.Receivement;//收货单实体
            if (rcv == null)
                return;
            bool isSubmit = false;
            if (rcv.OriginalData.Status == RcvStatusEnum.Opened && rcv.Status == RcvStatusEnum.Approving)
            {
                isSubmit = true;
            }
            foreach (RcvLine line in rcv.RcvLines)
            {
                //修改折前价格（扩展字段1），计算最终价
                if (line.DescFlexSegments != null && !String.IsNullOrEmpty(line.DescFlexSegments.PrivateDescSeg1) && line.DescFlexSegments.PrivateDescSeg1 != line.OriginalData.DescFlexSegments.PrivateDescSeg1)
                {
                    line.FinallyPriceTC = line.OriginalData.FinallyPriceTC + Convert.ToDecimal(line.DescFlexSegments.PrivateDescSeg1) - Convert.ToDecimal(line.OriginalData.DescFlexSegments.PrivateDescSeg1);
                    Session.Current.InList(line);
                }
                //提交，单价写入批号主档
                if (isSubmit && line.InvLot != null)
                {
                    using (ISession session = Session.Open())
                    {
                        UFIDA.U9.Lot.LotMaster lotMaster = UFIDA.U9.Lot.LotMaster.Finder.FindByID(line.InvLot.ID);
                        lotMaster.DescFlexSegments.PrivateDescSeg1 = line.FinallyPriceTC.ToString();
                        session.Commit();
                    }
                }
            }
        }

    }
}
