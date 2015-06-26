using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using UFIDA.U9.SCM.PM.ReceivementUIModel;
using UFIDA.U9.Cust.HBH.Common.CommonLibary;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInUI
{
    public class RcvUIExtend : UFSoft.UBF.UI.Custom.ExtendedPartBase
    {
        public UFSoft.UBF.UI.IView.IPart part;
        private UFIDA.U9.SCM.PM.ReceivementUIModel.ReceivementMainUIFormWebPart _strongPart;

        public override void BeforeRender(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.BeforeRender(Part, args);

            part = Part;

            _strongPart = Part as ReceivementMainUIFormWebPart;
        }

        public override void AfterRender(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.AfterRender(Part, args);

            //object lnk = this._strongPart.NameValues["lnk"];
            //string url = "Cust_Rcv";
            //if (lnk != null
            //    && lnk.ToString() == url
            //    )
            {
                //string orgID = "1001411156753998";
                string targetOrgID = PubClass.GetString(this._strongPart.NameValues["TargetOrg"]);
                string curOId = PubClass.GetString(this._strongPart.NameValues["__curOId"]);
                string urlID = PubClass.GetString(this._strongPart.NameValues["lnk"]);
                if (
                    !PubClass.IsNull(targetOrgID)
                    && targetOrgID != curOId
                    )
                {
                    NameValueCollection nvs = new NameValueCollection();
                    nvs.Add("__curOId", targetOrgID);
                    nvs.Add("RCV_Type", PubClass.GetString(this._strongPart.NameValues["RCV_Type"]));
                    //// 有上面菜单栏，但是组织无法切换
                    //this._strongPart.NavigatePage("Cust_Rcv", nvs);
                    // 无上面菜单栏，但组织可以切换；
                    this._strongPart.NavigateForm(urlID, nvs);
                }
            }
        }
    }
}
