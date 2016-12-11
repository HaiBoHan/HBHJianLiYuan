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
using HBH.DoNet.DevPlatform.EntityMapping;
using UFIDA.U9.CBO.SCM.Supplier;
using UFIDA.U9.Base;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    public class PR_AfterValidateExtend : UFSoft.UBF.Eventing.IEventSubscriber
    {
        public void Notify(params object[] args)
        {
            if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
                return;
            BusinessEntity.EntityKey key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;

            if (key == null)
                return;
            PR pr = key.GetEntity() as UFIDA.U9.PR.PurchaseRequest.PR;//请购单实体
            if (pr == null)
                return;

            // 提交时校验，价格为空不允许提交，否则提交了到采购订单也没有价格；
            bool isSubmit = false;
            if (pr.OriginalData.Status == PRStatusEnum.OpenOpen 
                && pr.Status == PRStatusEnum.Approving
                )
            {
                isSubmit = true;
            }
            if (isSubmit)
            {
                StringBuilder sbMsg = new StringBuilder();
                foreach (PRLine line in pr.PRLineList)
                {
                    if (line != null
                        && line.SuggestedPrice <= 0
                        )
                    {
                        sbMsg.Append(string.Format("行[{0}]物料[{1}]价格不能为空. \r\n "
                            , line.DocLineNo
                            , line.ItemInfo != null ? line.ItemInfo.ItemName : string.Empty
                            ));
                    }
                }

                if (sbMsg.Length > 0)
                {
                    throw new BusinessException(sbMsg.ToString());
                }
            }
        }
    }
}
