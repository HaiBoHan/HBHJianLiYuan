using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.UI.WebControlAdapter;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.UI.Engine.Builder;
using System.Data;
using UFSoft.UBF.Util.DataAccess;
using UFSoft.UBF.Sys.Database;
using UFSoft.UBF.ExportService;
using UFSoft.UBF.UI.ActionProcess;
using UFSoft.UBF.UI.WebControls.Association.Adapter;
using UFSoft.UBF.UI.WebControls.Association;
using UFSoft.UBF.UI.WebControls.ClientCallBack;
using System.Collections;
using System.Collections.Specialized;
using UFIDA.U9.PriceCal.PriceCalSV;
using UFSoft.UBF.UI.Controls;
using UFSoft.UBF.UI.WebControls;
using UFIDA.U9.Cust.HBH.Common.CommonLibary;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInUI
{
    public class PRPlugUIExtend : UFSoft.UBF.UI.Custom.ExtendedPartBase
    {
        public UFSoft.UBF.UI.IView.IPart part;
        private UFIDA.U9.SCM.PM.PRUIModel.PRMainUIFormWebPart _strongPart;

        IUFMenu BtnCreatBR;//下发请购
        IUFMenu BtnQueryPR;//请购追溯
        IUFButton BtnCreatPR1;//下发请购1

        IUFDataGrid DataGrid8;

        #region 插件事件
        public override void AfterInit(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.AfterInit(Part, args);
            part = Part;

            _strongPart = Part as UFIDA.U9.SCM.PM.PRUIModel.PRMainUIFormWebPart;
            DataGrid8 = (IUFDataGrid)part.GetUFControlByName(part.TopLevelContainer, "DataGrid8");
            IUFDataGrid dataGrid = (IUFDataGrid)this.part.GetUFControlByName(this.part.TopLevelContainer, "DataGrid8");
            CreateButton(_strongPart);

        }
        public override void AfterLoad(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            if (Part == null || Part.Model == null)
                return;
            part = Part;
            _strongPart = Part as UFIDA.U9.SCM.PM.PRUIModel.PRMainUIFormWebPart;
            part.Model.ClearErrorMessage();
            
            base.AfterLoad(Part, args);
        }
        public override void BeforeDataLoad(UFSoft.UBF.UI.IView.IPart Part, out bool executeDefault)
        {
            base.BeforeDataLoad(Part, out executeDefault);
            if (Part == null || Part.Model == null)
                return;
            part = Part;
            _strongPart = Part as UFIDA.U9.SCM.PM.PRUIModel.PRMainUIFormWebPart;

            part.Model.ClearErrorMessage();
            IUFFldReferenceColumn itemRef = (IUFFldReferenceColumn)DataGrid8.Columns["ItemInfo_ItemID"];
            //itemRef.CustomInParams = BaseAction.Symbol_MovePageFilter + "= 1=1";
            //itemRef.CustomInParams = BaseAction.Symbol_AddCustomFilter + "= ID =1001411280110126";
            //itemRef.CustomInParams = "ID =1001411280110126";
        }

        public override void BeforeDataBinding(UFSoft.UBF.UI.IView.IPart Part, out bool executeDefault)
        {
            base.BeforeDataBinding(Part, out executeDefault);

            part = Part;
            _strongPart = Part as UFIDA.U9.SCM.PM.PRUIModel.PRMainUIFormWebPart;

        }
        public override void AfterDataCollect(UFSoft.UBF.UI.IView.IPart Part)
        {
            base.AfterDataCollect(Part);
            //if (_strongPart.Model.Views["PR"].FocusedRecord != null)
            //{
            //    string id = _strongPart.Model.Views["PR"].FocusedRecord["ReqDepartment"].ToString();
            //    IUFFldReferenceColumn itemRef = (IUFFldReferenceColumn)DataGrid8.Columns["ItemInfo_ItemID"];
            //    //if (_strongPart.Model.Views["PR"].FocusedRecord["ReqDepartment"] == null || (long)_strongPart.Model.Views["PR"].FocusedRecord["ReqDepartment"] ==0)
            //    //_strongPart.Model.ErrorMessage.Message = "请先选择需求部门";
            //    itemRef.CustomInParams = BaseAction.Symbol_AddCustomFilter + "= ID in (select ItemMaster from U9::VOB::Cus::HBHJianLiYuan::DeptItemSupplierBE::DeptItemSupplierLine where DeptItemSupplier.Department.ID=" + _strongPart.Model.Views["PR"].FocusedRecord["ReqDepartment"] + ")";
            //}
            //IUFFldReferenceColumn itemRef = (IUFFldReferenceColumn)DataGrid8.Columns["ItemInfo_ItemID"];
            //itemRef.CustomInParams = BaseAction.Symbol_AddCustomFilter + "= ID =1001411280110126";
        }
        public override void AfterEventProcess(UFSoft.UBF.UI.IView.IPart Part, string eventName, object sender, EventArgs args)
        {
            base.AfterEventProcess(Part, eventName, sender, args);

            string strMeg = string.Empty;
            UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter webButton = sender as UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter;
            UFSoft.UBF.UI.WebControlAdapter.UFWebReferenceAdapter web = sender as UFSoft.UBF.UI.WebControlAdapter.UFWebReferenceAdapter;
            #region//重选部门
            if (web != null && web.UIField.Name == "ReqDepartment")
            {
                IUFFldReferenceColumn itemRef = (IUFFldReferenceColumn)DataGrid8.Columns["ItemInfo_ItemID"];
                ////if (_strongPart.Model.Views["PR"].FocusedRecord["ReqDepartment"] == null || (long)_strongPart.Model.Views["PR"].FocusedRecord["ReqDepartment"] ==0)
                ////_strongPart.Model.ErrorMessage.Message = "请先选择需求部门";
                //itemRef.CustomInParams = BaseAction.Symbol_AddCustomFilter + "= ID in (select ItemMaster from U9::VOB::Cus::HBHJianLiYuan::DeptItemSupplierBE::DeptItemSupplierLine where DeptItemSupplier.Department.ID=" + _strongPart.Model.Views["PR"].FocusedRecord["ReqDepartment"] + ")";
                //itemRef.CustomInParams = BaseAction.Symbol_AddCustomFilter + " = Code ='001120511-2'";
                //itemRef.AddReferenceInParameter("ItemMaster", "Code", "001120511-2");
                //itemRef.AddReferenceInParameter("ItemMaster", "ID", "0");
                //itemRef.CustomInParams += " & Code ='001120511-2'";
            }
            #endregion
        }

        public override void AfterRender(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.AfterRender(Part, args);

            if (_strongPart.Model.Views["PR"].FocusedRecord != null)
            {
                string id = _strongPart.Model.Views["PR"].FocusedRecord["ReqDepartment"].ToString();
                IUFFldReferenceColumn itemRef = (IUFFldReferenceColumn)DataGrid8.Columns["ItemInfo_ItemID"];
                //if (_strongPart.Model.Views["PR"].FocusedRecord["ReqDepartment"] == null || (long)_strongPart.Model.Views["PR"].FocusedRecord["ReqDepartment"] ==0)
                //_strongPart.Model.ErrorMessage.Message = "请先选择需求部门";
                // itemRef.CustomInParams = BaseAction.Symbol_AddCustomFilter + "= ID in (select ItemMaster from U9::VOB::Cus::HBHJianLiYuan::DeptItemSupplierBE::DeptItemSupplierLine where DeptItemSupplier.Department.ID=" + _strongPart.Model.Views["PR"].FocusedRecord["ReqDepartment"] + ")";

                string opath = "ID in (select disLine.ItemMaster from U9::VOB::Cus::HBHJianLiYuan::DeptItemSupplierBE::DeptItemSupplierLine disLine where disLine.DeptItemSupplier.Department=" + _strongPart.Model.Views["PR"].FocusedRecord["ReqDepartment"] + ")";
                //string opath = "Code = '000001'";

                string custFilter = BaseAction.Symbol_AddCustomFilter + "=";
                if (itemRef.CustomInParams != null
                    && itemRef.CustomInParams.Contains(custFilter)
                    )
                {
                    itemRef.CustomInParams = itemRef.CustomInParams.Replace(custFilter, custFilter + opath + " and ");                    
                }
                else
                {
                    itemRef.CustomInParams = custFilter + opath;
                }
            }
            //IUFFldReferenceColumn itemRef = (IUFFldReferenceColumn)DataGrid8.Columns["ItemInfo_ItemID"];
            //itemRef.CustomInParams = BaseAction.Symbol_AddCustomFilter + "= ID =1001411280110126";
        }

        #endregion

        #region 自定义方法
        /// <summary>
        /// 创建按钮
        /// </summary>
        private void CreateButton(UFIDA.U9.SCM.PM.PRUIModel.PRMainUIFormWebPart aa)
        {
            #region 在操作按钮下添加按钮
            //获取操作下拉按钮
            UFSoft.UBF.UI.ControlModel.IUFDropDownButton dpOperatePR = (UFSoft.UBF.UI.ControlModel.IUFDropDownButton)this.part.GetUFControlByName(this.part.TopLevelContainer, "DDBtnOperation");
            //下发请购 
            BtnCreatBR = new UFWebMenuAdapter();
            BtnCreatBR.Text = "下发请购";
            BtnCreatBR.ID = "BtnDPCreatPR";
            BtnCreatBR.ItemClick += new UFSoft.UBF.UI.WebControls.MenuItemHandle(BtnCreatBR_ItemClick);
            BtnCreatBR.AutoPostBack = true;


            dpOperatePR.MenuItems.Add(BtnCreatBR);

            #endregion

            #region 在查询按钮下添加按钮
            //获取操作下拉按钮
            UFSoft.UBF.UI.ControlModel.IUFDropDownButton dpCustQuery = (UFSoft.UBF.UI.ControlModel.IUFDropDownButton)this.part.GetUFControlByName(this.part.TopLevelContainer, "DDSourceQuery");
            //下发请购 
            BtnQueryPR = new UFWebMenuAdapter();
            BtnQueryPR.Text = "请购追溯";
            BtnQueryPR.ID = "BtnDDQuery";
            BtnQueryPR.ItemClick += new UFSoft.UBF.UI.WebControls.MenuItemHandle(BtnQueryPR_ItemClick);
            BtnQueryPR.AutoPostBack = true;
            dpCustQuery.MenuItems.Add(BtnQueryPR);

            #endregion

            #region 自定义按钮

            BtnCreatPR1 = new UFWebButtonAdapter();
            BtnCreatPR1.Text = "下发请购";
            BtnCreatPR1.ID = "BtnCreatPR1";
            BtnCreatPR1.AutoPostBack = true;
            BtnCreatPR1.Click += new EventHandler(BtnCreatPR1_Click);

            IUFCard card = (IUFCard)part.GetUFControlByName(part.TopLevelContainer, "Card0");
            card.Controls.Add(BtnCreatPR1);
            CommonFunctionExtend.Layout(card, BtnCreatPR1, 18, 0);

            #endregion

        }

        void BtnCreatPR1_Click(object sender, EventArgs e)
        {
            this.part.Model.ClearErrorMessage();
            if (this.part.Model.Views["PR"].FocusedRecord == null)
            {
                this.part.Model.ErrorMessage.Message = "请先保存请购单";
                return;
            }
            try
            {
                U9.VOB.Cus.HBHJianLiYuan.PrToPrSV.Proxy.PrToPrSVProxy proxy = new PrToPrSV.Proxy.PrToPrSVProxy();
                List<long> listPR = new List<long>();
                listPR.Add(long.Parse(part.Model.Views["PR"].FocusedRecord["ID"].ToString()));
                proxy.PR = listPR;
                List<UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData> resultDataList = proxy.Do();
                if (resultDataList == null || resultDataList.Count == 0)
                {
                    this.part.Model.ErrorMessage.Message = "生成请购单失败";
                    return;
                }
                else if (resultDataList[0].Name != "")
                {
                    this.part.Model.ErrorMessage.Message = resultDataList[0].Name;
                    return;
                }
            }
            catch (Exception ex)
            {
                this.part.Model.ErrorMessage.Message = ex.Message;
                return;
            }
        }
       
        #endregion

        #region 自定义按钮事件

        #region 下发请购

        //下发请购
        public void BtnCreatBR_ItemClick(object sender, UFSoft.UBF.UI.WebControls.MenuItemClickEventArgs e)
        {
            this.part.Model.ClearErrorMessage();
            if (this.part.Model.Views["PR"].FocusedRecord == null)
            {
                this.part.Model.ErrorMessage.Message = "请先保存请购单";
                return;
            }
            try
            {
                U9.VOB.Cus.HBHJianLiYuan.PrToPrSV.Proxy.PrToPrSVProxy proxy = new PrToPrSV.Proxy.PrToPrSVProxy();
                List<long> listPR = new List<long>();
                listPR.Add(long.Parse(part.Model.Views["PR"].FocusedRecord["ID"].ToString()));
                proxy.PR = listPR;
                List<UFIDA.U9.CBO.Pub.Controller.CommonArchiveDataDTOData> resultDataList = proxy.Do();
                if (resultDataList == null || resultDataList.Count == 0)
                {
                    this.part.Model.ErrorMessage.Message = "生成请购单失败";
                    return;
                }
                else if(!PubClass.IsNull( resultDataList[0].Code))
                {
                    //this.part.Model.ErrorMessage.Message = resultDataList[0].Name;

                    BtnQueryPR_ItemClick(null, null);
                    return;
                }
            }
            catch (Exception ex)
            {
                this.part.Model.ErrorMessage.Message = ex.Message;
                return;
            }
        }
     

        #endregion

        #region 弹出页面

        //请购追溯
        public void BtnQueryPR_ItemClick(object sender, UFSoft.UBF.UI.WebControls.MenuItemClickEventArgs e)
        {
            if (this.part.Model.Views["PR"].FocusedRecord != null)
            {
                part.Model.ClearErrorMessage();
                part.DataCollect();
                part.IsConsuming = false;
                part.IsDataBinding = true;
                NameValueCollection param = new NameValueCollection();
                param.Add("SrcPRID", part.Model.Views[0].FocusedRecord["ID"].ToString());

                part.ShowModalDialog("e37cea28-9138-43a4-bbe7-e747977e3db5", "已转成功请购单", "992", "504", "", param, false);
            }
            else
            {
                this.part.Model.ErrorMessage.Message = "当前请购单不能为空！";
                return;
            }
        }

        #endregion

        #endregion
    }
}
