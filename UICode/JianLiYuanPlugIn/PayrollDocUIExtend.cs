using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UFSoft.UBF.UI.ControlModel;
using UFIDA.U9.HR.PAY.PayrollDoc.PayrollDocUIModel;

namespace U9.VOB.Cus.HBHJianLiYuan.PlugInUI
{
    public class PayrollDocUIExtend : UFSoft.UBF.UI.Custom.ExtendedPartBase
    {
        public UFSoft.UBF.UI.IView.IPart part;
        UFIDA.U9.HR.PAY.PayrollDoc.PayrollDocUIModel.PayrollDocMainUIFormWebPart _strongPart;

        //IUFDataGrid DataGrid10;
        public override void AfterInit(UFSoft.UBF.UI.IView.IPart Part, EventArgs args)
        {
            base.AfterInit(Part, args);

            if (Part == null || Part.Model == null)
                return;
            part = Part;

            _strongPart = Part as PayrollDocMainUIFormWebPart;
        }


        //public override void BeforeEventProcess(UFSoft.UBF.UI.IView.IPart Part, string eventName, object sender, EventArgs args, out bool executeDefault)
        //{
        //    base.BeforeEventProcess(Part, eventName, sender, args, out executeDefault);
        //    UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter webButton = sender as UFSoft.UBF.UI.WebControlAdapter.UFWebButton4ToolbarAdapter;

        //    // BtnComputePay
        //    //_strongPart.Action.ComputeRealPay
        //    if (webButton != null && webButton.Action == "ComputeRealPay")
        //    {
        //        ////拣货
        //        //if (_strongPart.Model.Ship.FocusedRecord != null && _strongPart.Model.Ship.FocusedRecord.DocNo != "")
        //        //{
        //        //    VOB.Cus.HBHJianLiYuan.ShipPickByDocBP.Proxy.ShipPickByDocProxy proxy = new ShipPickByDocBP.Proxy.ShipPickByDocProxy();
        //        //    List<String> docList = new List<string>();
        //        //    docList.Add(_strongPart.Model.Ship.FocusedRecord.DocNo);
        //        //    proxy.ShipNos = docList;
        //        //    proxy.Do();
        //        //}
        //    }
        //}

        /*      计算薪资按钮
        // UFIDA.U9.HR.PAY.PayrollDoc.PayrollDocUIModel.PayrollDocUIModelAction
private void ComputeRealPay_Extend(object sender, UFSoft.UBF.UI.ActionProcess.UIActionEventArgs e)
{
	if (this.CurrentModel.ErrorMessage.hasErrorMessage)
	{
		this.CurrentModel.ClearErrorMessage();
	}
	base.CommonAction.Save();
	long iD = this.CurrentModel.PayrollDoc.FocusedRecord.ID;
	CalculateActualPayBPProxy calculateActualPayBPProxy = new CalculateActualPayBPProxy();
	calculateActualPayBPProxy.set_PayrollDoc(iD);
	calculateActualPayBPProxy.Do();
	if (!this.CurrentModel.ErrorMessage.hasErrorMessage)
	{
		this.Refresh();
	}
}
         * 
         * 
        namespace UFIDA.U9.PAY.PayrollDoc
{
	/// <summary>
	/// Impement Implement
	///
	/// </summary>	
	internal class CalculateActualPayBPImpementStrategy : BaseStrategy
	{
		public override object Do(object obj)
		{
			CalculateActualPayBP calculateActualPayBP = (CalculateActualPayBP)obj;
			object result;
			if (calculateActualPayBP.PayrollDoc == null)
			{
				result = null;
			}
			else
			{
				PayrollDoc entity = calculateActualPayBP.PayrollDoc.GetEntity();
				if (entity.EmpPayrolls == null || entity.EmpPayrolls.get_Count() == 0)
				{
					throw new PaymasterNotExistException();
				}
				if (entity.PayrollDocItems == null || entity.PayrollDocItems.get_Count() == 0)
				{
					throw new PayItemNotExistCannotCalculateException();
				}
				if (entity.EmpPayrolls != null && entity.EmpPayrolls.get_Count() > 0 && entity.PayrollDocItems != null && entity.PayrollDocItems.get_Count() > 0)
				{
					using (Session session = Session.Open())
					{
						entity.UpdateSalaryValue();
						entity.UpdateSystemItemValue();
						session.Commit();
					}
				}
				result = null;
			}
			return result;
		}
	}
         */
    }
}
