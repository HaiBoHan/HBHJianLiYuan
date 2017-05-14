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
using UFIDA.U9.PPR.PurPriceListUI;
using HBH.DoNet.DevPlatform.EntityMapping;
using UFSoft.UBF.UI.MD.Runtime;
using HBH.DoNet.DevPlatform.U9Mapping;


namespace U9.VOB.Cus.HBHJianLiYuan.PlugInUI
{
    public class PurPriceListUIExtend : UFSoft.UBF.UI.Custom.ExtendedPartBase
    {
        public UFSoft.UBF.UI.IView.IPart part;
        IUFButton btnDepartment = new UFWebButtonAdapter();//实例化按钮“部门子表”
        PurPriceListMainUIFormWebPart _strongPart;
        IUFButton btnRefresh;

        public override void AfterInit(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.AfterInit(Part, args);

            if (Part == null || Part.Model == null)
                return;
            part = Part;
            _strongPart = Part as PurPriceListMainUIFormWebPart;

            CreateButton(part);

            // Excel导入测试
            //// Card0  6
            //IUFCard Card0 = (IUFCard)part.GetUFControlByName(part.TopLevelContainer, "Card0");
            //if (Card0 != null)
            //{
            //    btnRefresh = new UFWebButtonAdapter();
            //    btnRefresh.Text = "刷新";
            //    btnRefresh.ID = "btnRefresh";
            //    btnRefresh.AutoPostBack = true;
            //    btnRefresh.Visible = false;
            //    btnRefresh.Click += new EventHandler(btnRefresh_Click);

            //    Card0.Controls.Add(btnRefresh);
            //    HBHCommon.HBHCommonUI.UICommonHelper.Layout(Card0, btnRefresh, 7, 0);


            //    // 5
            //    IUFButton btnExcelImport = new UFWebButtonAdapter();
            //    btnExcelImport.Text = "Excel导入";
            //    btnExcelImport.ID = "btnExcelImport";
            //    btnExcelImport.AutoPostBack = true;
            //    btnExcelImport.Click += new EventHandler(btnExcelImport_Click);

            //    Card0.Controls.Add(btnExcelImport);
            //    HBHCommon.HBHCommonUI.UICommonHelper.Layout(Card0, btnExcelImport, 7, 0);

            //    // 确认对话框
            //    UFIDA.U9.UI.PDHelper.PDFormMessage.ShowConfirmDialog(_strongPart.Page, "确认导入新数据？", "导入新数据", btnExcelImport);
            //}
        }

        public override void AfterRender(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.AfterRender(Part, args);

            //ShowDepartment();
        }

        private const string SaveClick = "SaveClick";
        private const string FindClick = "FindClick";
        private const string UndoApproveClick = "UndoApproveClick";

        public override void AfterEventProcess(UFSoft.UBF.UI.IView.IPart Part, string eventName, object sender, EventArgs args)
        {
            base.AfterEventProcess(Part, eventName, sender, args);

            UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter webButton = sender as UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter;

            if (webButton != null)
            {
                if (webButton.Action == SaveClick
                    || webButton.Action == FindClick
                    || webButton.Action == UndoApproveClick
                    )
                {
                    ShowDepartment();
                }
            }
        }

        private void ShowDepartment()
        {
            PurPriceListRecord head = _strongPart.Model.PurPriceList.FocusedRecord;

            if (head != null
                && head.ID > 0
                )
            {
                /*  UFIDA.U9.PPR.Enums.Status
                Approved	已核准	2
                Approving	核准中	1
                Opened	开立	0
                 */
                if (head.Status == 0)
                {
                    btnDepartment_Click(null, null);
                }
            }
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
                param.Add("SrcPurpriceID", PubClass.GetString(part.Model.Views[0].FocusedRecord["ID"].ToString()));
                param.Add("SrcPurpriceCode", PubClass.GetString(part.Model.Views[0].FocusedRecord["Code"].ToString()));
                param.Add("SrcPurpriceName", PubClass.GetString(part.Model.Views[0].FocusedRecord["Name"]));
                part.ShowModalDialog("b6217510-0ac9-4905-a3a2-6730a9179884", "部门子表", "504", "504", "", param, false);
            }
        }


        public void btnExcelImport_Click(object sender, EventArgs e)
        {
            this.part.Model.ClearErrorMessage();


            System.Collections.Specialized.NameValueCollection nvs = U9.VOB.HBHCommon.HBHCommonUI.ExcelDataHelper.GetHBHCommonExcelNameValues_Line();
            //nvs.Add(ExcelImportUIFormWebPart.Const_ReturnStateFlag, Const_StockupToolImportData);

            this._strongPart.CurrentState[U9.VOB.HBHCommon.ExcelImportUIFormWebPart.Const_ExcelParams] = U9.VOB.HBHCommon.HBHCommonUI.ExcelDataHelper.GetHBHCommonExcelNameValues_FirstColumnName();


            this._strongPart.ShowAtlasModalDialog(this.btnRefresh, "19b4d677-1955-4c11-b808-d0bd65bde31b", "导入数据", "510", "110", this._strongPart.TaskId.ToString(), nvs, true, false, false);
        }


        public void btnRefresh_Click(object sender, EventArgs e)
        {

            //IUIView headView = this._strongPart.Model.SupplierEngineer;
            IUIView lineView = this._strongPart.Model.PurPriceList_PurPriceLines;
            DataSet ds = this._strongPart.CurrentState[U9.VOB.HBHCommon.ExcelImportUIFormWebPart.Const_DefaultStateName] as DataSet;
            DataTable tbLine = ds.GetTableOfFirst();

            if (tbLine != null
                && tbLine.Rows != null
                && tbLine.Rows.Count > 0
                )
            {
                string procName = "HBH_SP_SunRa_ResetCustSupplierEngineerInfo";

                List<ParamDTO> lstParam = new List<ParamDTO>();
                {
                    ParamDTO param = new ParamDTO();
                    param.ParamName = "LineData";
                    param.ParamDirection = ParameterDirection.Input;
                    param.ParamType = DbType.Xml;
                    param.ParamValue = EntitySerialization.EntitySerial(tbLine, SerialType.XML);

                    lstParam.Add(param);
                }

                DataTable processedTable = new DataTable();
                HBH.DoNet.DevPlatform.U9Mapping.U9Helper.GetResultByProcess(procName, out processedTable, lstParam.ToArray());

                if (processedTable != null
                    && processedTable.Rows != null
                    && processedTable.Rows.Count > 0
                    )
                {
                    //ds.Tables.RemoveAt(0);
                    ds.Tables.Clear();
                    ds.Tables.Add(processedTable);

                    U9.VOB.HBHCommon.HBHCommonUI.ExcelDataHelper.ResetViewByExcelTable(null, lineView, ds);

                }
            }
        }
    }
}
