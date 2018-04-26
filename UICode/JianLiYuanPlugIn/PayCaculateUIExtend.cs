using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.UI.ControlModel;
using UFSoft.UBF.UI.WebControlAdapter;
using UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUIModel;
using U9.VOB.Cus.HBHJianLiYuan.Proxy;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInUI
{
    /* 
     * http://114.215.220.148/U9/erp/display.aspx?lnk=HR.PAY.PayCaculate&sId=3034nid&__curOId=1001510020000918&__dm=true
     * HR.PAY.PayCaculate
        NULL	NULL	NULL	NULL	UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUIModel.PayCaculateMainUIFormWebPart	UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUI.WebPart	ReportHeader
        UFIDA.U9.PAY.PayrollCaculate.PayrollSupply	薪资补发	Pay_PayrollSupply	UFIDA.U9.PAY.PayBE	UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUIModel.PayCaculateMainUIFormWebPart	UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUI.WebPart	PayrollSupply
        NULL	NULL	NULL	NULL	UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUIModel.PayCaculateMainUIFormWebPart	UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUI.WebPart	CalcView
        UFIDA.U9.PAY.PayrollResult.PayrollResult	薪资计算结果	PAY_PayrollResult	UFIDA.U9.PAY.PayBE	UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUIModel.PayCaculateMainUIFormWebPart	UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUI.WebPart	PayrollResult
        NULL	NULL	NULL	NULL	UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUIModel.PayCaculateMainUIFormWebPart	UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUI.WebPart	HeadFilter
        NULL	NULL	NULL	NULL	UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUIModel.PayCaculateMainUIFormWebPart	UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUI.WebPart	PayrollResultCopyItems
        NULL	NULL	NULL	NULL	UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUIModel.PayCaculateMainUIFormWebPart	UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUI.WebPart	ReportBody
     */
    public class PayCaculateUIExtend : UFSoft.UBF.UI.Custom.ExtendedPartBase
    {
        public UFSoft.UBF.UI.IView.IPart part;
        private UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUIModel.PayCaculateMainUIFormWebPart _strongPart;


        public override void AfterInit(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.AfterInit(Part, args);
            part = Part;

            _strongPart = Part as UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUIModel.PayCaculateMainUIFormWebPart;

            if (HBHHelper.PubConfig.Const_ThirdHRStage)
            {
                // Card0
                // 13
                IUFCard card0 = (IUFCard)part.GetUFControlByName(part.TopLevelContainer, "Card0");
                if (card0 != null)
                {
                    //IUFButton btnGetCheckin = new UFWebButtonAdapter();
                    //btnGetCheckin.Text = "考勤计算";
                    //btnGetCheckin.ID = "btnGetCheckin";
                    //btnGetCheckin.AutoPostBack = true;
                    //btnGetCheckin.Click += new EventHandler(btnGetCheckin_Click);

                    //card0.Controls.Add(btnGetCheckin);
                    //CommonFunctionExtend.Layout(card0, btnGetCheckin, 12, 0);

                    IUFDropDownButton ddbCalcPayroll = new UFWebDropDownButtonAdapter();
                    ddbCalcPayroll.Text = "薪资计算";
                    ddbCalcPayroll.ID = "ddbCalcPayroll";
                    ddbCalcPayroll.AutoPostBack = true;

                    card0.Controls.Add(ddbCalcPayroll);
                    HBHCommon.HBHCommonUI.UICommonHelper.Layout(card0, ddbCalcPayroll, 12, 0);


                    {
                        IUFMenu btnGetCheckin = new UFWebMenuAdapter();
                        btnGetCheckin.Text = "考勤计算";
                        btnGetCheckin.ID = "btnGetCheckin";
                        btnGetCheckin.AutoPostBack = true;
                        btnGetCheckin.ItemClick += new UFSoft.UBF.UI.WebControls.MenuItemHandle(btnGetCheckin_ItemClick);
                        ddbCalcPayroll.MenuItems.Add(btnGetCheckin);

                        // 确认对话框
                        // 报错: 控件必须位于页面的控件树中。
                        //UFIDA.U9.UI.PDHelper.PDFormMessage.ShowDelConfirmDialog(_strongPart.Page, "确认重新计算考勤？", "考勤计算", btnGetCheckin);
                    }

                    {
                        IUFMenu btnCalcAreaCash = new UFWebMenuAdapter();
                        btnCalcAreaCash.Text = "计算应兑现";
                        btnCalcAreaCash.ID = "btnCalcAreaCash";
                        btnCalcAreaCash.AutoPostBack = true;
                        btnCalcAreaCash.ItemClick += new UFSoft.UBF.UI.WebControls.MenuItemHandle(btnCalcAreaCash_ItemClick);
                        ddbCalcPayroll.MenuItems.Add(btnCalcAreaCash);

                        // 确认对话框
                        // 报错: 控件必须位于页面的控件树中。
                        //UFIDA.U9.UI.PDHelper.PDFormMessage.ShowDelConfirmDialog(_strongPart.Page, "确认计算应兑现？", "计算应兑现", btnCalcAreaCash);
                    }

                }
            }
        }


        public void btnGetCheckin_Click(object sender, EventArgs e)
        {
            this.part.Model.ClearErrorMessage();

            HeadFilterRecord focusedHead = _strongPart.Model.HeadFilter.FocusedRecord;

            if (focusedHead != null
                && focusedHead.PayrollCaculate > 0
                )
            {
                _strongPart.BtnSave_Click(sender, e);

                if (!_strongPart.Model.ErrorMessage.hasErrorMessage)
                {
                    try
                    {
                        GetCheckinDataBPProxy proxy = new GetCheckinDataBPProxy();
                        proxy.IDs = new List<long>();
                        proxy.IDs.Add(focusedHead.PayrollCaculate);
                        //proxy.IDs.Add(focusedHead.ID);

                        proxy.Do();

                        _strongPart.Action.NavigateAction.Refresh(null);
                    }
                    catch (Exception ex)
                    {
                        this.part.Model.ErrorMessage.Message = ex.Message;
                        return;
                    }
                }
            }
        }
        
        void btnGetCheckin_ItemClick(object sender, UFSoft.UBF.UI.WebControls.MenuItemClickEventArgs e)
        {
            btnGetCheckin_Click(sender, e);
        }

        void btnCalcAreaCash_ItemClick(object sender, UFSoft.UBF.UI.WebControls.MenuItemClickEventArgs e)
        {
            this.part.Model.ClearErrorMessage();

            HeadFilterRecord focusedHead = _strongPart.Model.HeadFilter.FocusedRecord;

            if (focusedHead != null
                && focusedHead.PayrollCaculate > 0
                )
            {
                _strongPart.BtnSave_Click(sender, e);

                if (!_strongPart.Model.ErrorMessage.hasErrorMessage)
                {
                    try
                    {
                        CalcPayrollAreaCashBPProxy proxy = new CalcPayrollAreaCashBPProxy();
                        proxy.IDs = new List<long>();
                        proxy.IDs.Add(focusedHead.PayrollCaculate);
                        //proxy.IDs.Add(focusedHead.ID);

                        proxy.Do();

                        _strongPart.Action.NavigateAction.Refresh(null);
                    }
                    catch (Exception ex)
                    {
                        this.part.Model.ErrorMessage.Message = ex.Message;
                        return;
                    }
                }
            }
        }

    }
}
