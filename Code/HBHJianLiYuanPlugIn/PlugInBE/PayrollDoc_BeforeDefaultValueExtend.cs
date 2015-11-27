using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.PAY.PayrollDoc;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBE
{
    public class PayrollDoc_BeforeDefaultValueExtend : UFSoft.UBF.Eventing.IEventSubscriber
    {
        public void Notify(params object[] args)
        {
            if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
                return;
            BusinessEntity.EntityKey key = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;

            if (key == null)
                return;
            PayrollDoc entity = key.GetEntity() as PayrollDoc;//请购单实体
            if (entity == null)
                return;



        }
    }
}
