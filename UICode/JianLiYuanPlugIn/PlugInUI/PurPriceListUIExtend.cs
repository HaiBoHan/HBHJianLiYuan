using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.WebControlAdapter;
using System.Collections.Specialized;
using UFIDA.U9.Base.BaseBE;
using UFSoft.UBF.ExportService;
using UFSoft.UBF.UI.ActionProcess;
using System.Data;
using UFSoft.UBF.Util.DataAccess;


namespace U9.VOB.Cus.HBHJianLiYuan.PlugInUI
{
    public class PurPriceListUIExtend : UFSoft.UBF.UI.Custom.ExtendedPartBase
    {
        public UFSoft.UBF.UI.IView.IPart part;
        IUFButton btnDepartment = new UFWebButtonAdapter();//实例化按钮“部门子表”
        public override void AfterInit(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.AfterInit(Part, args);

            if (Part == null || Part.Model == null)
                return;
            part = Part;

            CreateButton(part);
        }
        public void CreateButton(UFSoft.UBF.UI.IView.IPart ipart)
        {
            //按钮属性
            btnDepartment.Text = "部门子表";
            btnDepartment.ID = "btnPPLDepartment";
            btnDepartment.Visible = true;
            btnDepartment.AutoPostBack = true;

            //设定按钮位置
            IUFCard card = (IUFCard)part.GetUFControlByName(ipart.TopLevelContainer, "Card0");
            card.Controls.Add(btnDepartment);
            CommonFunctionExtend.Layout(card, btnDepartment, 6, 0);
            //按钮事件
            btnDepartment.Click += new EventHandler(btnDepartment_Click);
        }
        /// <summary>
        /// 业务员分配
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnDepartment_Click(object sender, EventArgs e)
        {
            if (part.Model.Views[0].FocusedRecord != null && part.Model.Views[0].FocusedRecord.DataRecordState != DataRowState.Added)
            {
                NameValueCollection param = new NameValueCollection();
                param.Add("SrcPurpriceID", part.Model.Views[0].FocusedRecord["ID"].ToString());
                param.Add("SrcPurpriceCode", part.Model.Views[0].FocusedRecord["Code"].ToString());
                param.Add("SrcPurpriceName", part.Model.Views[0].FocusedRecord["Name"].ToString());
                part.ShowModalDialog("b6217510-0ac9-4905-a3a2-6730a9179884", "部门子表", "504", "504", "", param, false);
            }
        }
    }
}
