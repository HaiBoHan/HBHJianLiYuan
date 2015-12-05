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
    /* HR.PAY.PayCaculate
    UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUIModel.PayCaculateMainUIFormWebPart
    UFIDA.U9.HR.PAY.PayCaculate.PayCaculateUI.WebPart
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

            // Card0
            // 13
            IUFCard card0 = (IUFCard)part.GetUFControlByName(part.TopLevelContainer, "Card0");
            if (card0 != null)
            {
                IUFButton btnGetCheckin = new UFWebButtonAdapter();
                btnGetCheckin.Text = "考勤计算";
                btnGetCheckin.ID = "btnGetCheckin";
                btnGetCheckin.AutoPostBack = true;
                btnGetCheckin.Click += new EventHandler(btnGetCheckin_Click);

                card0.Controls.Add(btnGetCheckin);
                CommonFunctionExtend.Layout(card0, btnGetCheckin, 12, 0);


                // 确认对话框
                UFIDA.U9.UI.PDHelper.PDFormMessage.ShowDelConfirmDialog(_strongPart.Page, "确认重新计算考勤？", "考勤计算", btnGetCheckin);
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
