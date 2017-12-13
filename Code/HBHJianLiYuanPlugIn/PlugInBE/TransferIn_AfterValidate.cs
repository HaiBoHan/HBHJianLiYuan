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
using UFIDA.U9.CBO.HR.Department;
using UFIDA.U9.InvDoc.TransferIn;
using UFIDA.U9.Lot;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    public class TransferIn_AfterValidate : UFSoft.UBF.Eventing.IEventSubscriber
    {
        public void Notify(params object[] args)
        {
            if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
                return;
            BusinessEntity.EntityKey key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;

            if (key == null)
                return;

            TransferIn entity = key.GetEntity() as TransferIn;//请购单实体
            if (entity == null)
                return;

            // 提交或审核时，校验
            if (entity.Status == TransInStatus.Approving
                || entity.OriginalData.Status == TransInStatus.Approving
                )
            {
                /*
01	北京三源
02	青岛健力源
03	烟台分公司
04	济南健力源
05	管理中心
06	采购中心
07	北京健力源
08	沈阳健力源
99	商超系统
CPOrg	客户门户组织
VPOrg	供应商门户组织
                 */

                // 当调入单的单据类型为“员工领用”(003)时，“发放时间”这个字段不能为空
                // 调入单行，发放时间  =  私有段2
                if (entity.Org != null
                    && entity.TransInDocType != null
                    )
                {
                    if (entity.Org.Code == "05")
                    {
                        if (entity.TransInDocType.Code == "003")
                        {
                            StringBuilder sbError = new StringBuilder();
                            foreach (var line in entity.TransInLines)
                            {
                                if (line != null
                                    && line.DescFlexSegments != null
                                    )
                                {
                                    if (line.DescFlexSegments.PrivateDescSeg2.IsNull())
                                    {
                                        sbError.Append(line.DocLineNo).Append(",");
                                    }
                                }
                            }

                            if (sbError.Length > 0)
                            {
                                throw new BusinessException(string.Format("调入单[{0}]行[{1}]发放时间不可为空!"
                                    , entity.DocNo
                                    , sbError.GetStringRemoveLastSplit()
                                    ));
                            }
                        }
                    }
                }
            }
        }
        
    }
}
