using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.WebControlAdapter;
using System.Collections.Specialized;
using UFSoft.UBF.ExportService;
using UFSoft.UBF.UI.ActionProcess;
using System.Data;
using UFSoft.UBF.Util.DataAccess;
using UFIDA.U9.Cust.HBH.Common.CommonLibary;
using WhQohRefUIModel;


namespace U9.VOB.Cus.HBHJianLiYuan.PlugInUI
{
    public class ItemWhQohRefUIExtend : UFSoft.UBF.UI.Custom.ExtendedPartBase
    {
        public UFSoft.UBF.UI.IView.IPart part;
        WhQohRefBaseUIFormWebPart _strongPart;

        public override void AfterInit(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.AfterInit(Part, args);

            if (Part == null || Part.Model == null)
                return;
            part = Part;
            _strongPart = Part as WhQohRefBaseUIFormWebPart;

        }

        public override void BeforeLoad(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.BeforeLoad(Part, args);


            if (!_strongPart.Page.IsPostBack)
            {
                string strCondName = "ItemRefCondition";

                if (_strongPart.NameValues.ContainsKey(strCondName))
                {
                    object objItemOpath = _strongPart.NameValues[strCondName];

                    if (objItemOpath != null)
                    {
                        string strItemOpath = objItemOpath.ToString();

                        if (!string.IsNullOrWhiteSpace(strItemOpath)
                            && strItemOpath.Contains("HBHJianLiYuan")
                            )
                        {
                            object objDeptName = this._strongPart.NameValues["JianLiYuanDeptName"];

                            if (objDeptName != null)
                            {
                                strItemOpath = strItemOpath.Replace("@JianLiYuanDeptName", objDeptName.ToString());
                            }
                            _strongPart.CurrentState["ItemRefCondition"] = strItemOpath;
                        }
                    }
                }
            }
        }

    }
}
