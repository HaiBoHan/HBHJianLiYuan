using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using UFIDA.U9.SCM.PM.ReceivementUIModel;

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

            object lnk = this._strongPart.NameValues["lnk"];
            string url = "Cust_Rcv";
            if (lnk != null
                && lnk.ToString() == url
                )
            {
                string orgID = "1001411156753998";
                object nvOrg = this._strongPart.NameValues["__curOId"];
                if (nvOrg != null
                    && nvOrg.ToString() != orgID
                    )
                {
                    NameValueCollection nvs = new NameValueCollection();
                    nvs.Add("__curOId", orgID);
                    nvs.Add("RCV_Type", "PM6010");
                    this._strongPart.NavigatePage("Cust_Rcv", nvs);
                }
            }
        }
    }
}
