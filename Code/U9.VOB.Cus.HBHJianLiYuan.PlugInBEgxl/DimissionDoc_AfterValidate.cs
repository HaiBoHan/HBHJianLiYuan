using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.Business;
using UFIDA.U9.HI.Dimission;
using UFIDA.U9.HI.Enums;
using UFIDA.U9.InvTrans.WhQoh;
using UFSoft.UBF.PL;
using UFIDA.U9.Base;
using UFIDA.U9.InvDoc.TransferIn;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInBEgxl
{
    public class DimissionDoc_AfterValidate : UFSoft.UBF.Eventing.IEventSubscriber
    {
        public void Notify(params object[] args)
        {
            if (args == null || args.Length == 0 || !(args[0] is UFSoft.UBF.Business.EntityEvent))
                return;
            BusinessEntity.EntityKey entitykey = ((UFSoft.UBF.Business.EntityEvent)args[0]).EntityKey;

            if (entitykey == null)
                return;
            DimissionDoc entity = entitykey.GetEntity() as DimissionDoc;
            if (entity == null)
                return;

            if (entity.OriginalData.DocStatus == DocStatusEnum.Opening
                && entity.DocStatus == DocStatusEnum.Approving)
            {
                string employeeCode = string.Empty;  //员工编码

                if (entity.Employee != null)
                {
                    employeeCode = entity.Employee.EmployeeCode;
                }

                if (employeeCode != null && employeeCode.Length > 0)
                {
                    // 2018-05-25 wf 修改为 员工编码 与 库位编码 末尾匹配，客户那边有人建立库位档案不规范（有的前两位加01、00，这次问题是库位前面没有加编码，跟员工编码完全匹配了）
                    //string oql = "ItemOwnOrg=@Org and SqlLen(BinInfo.Code)>2 and Substr(BinInfo.Code,3,SqlLen(BinInfo.Code)-2)=@EmployeeCode and StoreQty>0";
                    string oql = "ItemOwnOrg=@Org and BinInfo.Code like '%' + @EmployeeCode and StoreQty>0";
                    WhQoh whQoh = WhQoh.Finder.Find(oql
                        , new OqlParam(Context.LoginOrg.ID)
                        , new OqlParam(employeeCode)
                        );
                    
                    if (whQoh != null)
                    {
                        throw new BusinessException("工作服未交回");
                    }
                }
            }

            if (entity.OriginalData.DocStatus == DocStatusEnum.Opening
                && entity.DocStatus == DocStatusEnum.Approving)
            {

                string employeeCode = string.Empty;  //员工编码

                if (entity.Employee != null)
                {
                    employeeCode = entity.Employee.EmployeeCode;
                }

                //TransInBin bin = new TransInBin();
                //bin.TransInSubLine.TransInLine.TransferIn.ApprovedOn

                TransInBin.EntityList binList = TransInBin.Finder.FindAll("Org=@Org and SqlLen(BinInfo.Code)>2 and Substr(BinInfo.Code,3,SqlLen(BinInfo.Code)-2)=@EmployeeCode and TransInSubLine is not null and TransInSubLine > 0 order by TransInSubLine.TransInLine.TransferIn.ApprovedOn desc"
                    , new OqlParam(Context.LoginOrg.ID)
                    , new OqlParam(employeeCode)
                    );

                if (binList != null && binList.Count > 0)
                {
                    if (binList[0] != null
                        && binList[0].TransInSubLine != null
                        && binList[0].TransInSubLine.TransInLine != null
                        && binList[0].TransInSubLine.TransInLine.TransferIn != null
                        && binList[0].TransInSubLine.TransInLine.TransferIn.DocType != null
                        )
                    {
                        if (binList[0].TransInSubLine.TransInLine.TransferIn.DocType.Code == "007")
                        {
                            if (entity.DescFlexField == null)
                            {
                                entity.DescFlexField = new UFIDA.U9.Base.FlexField.DescFlexField.DescFlexSegments();
                            }
                            entity.DescFlexField.PrivateDescSeg4 = "离职未退回";
                        }
                    }
                }
            }
        }
    }
}
