#region Using directives

using System;
using System.Collections;
using System.Data;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.UI.MD.Runtime.Implement;
using UFIDA.U9.UI.PDHelper;
using U9.VOB.HBHCommon.HBHCommonUI;

#endregion

namespace DayCheckInUIModel
{
    public partial class DayCheckInUIModelModel
    {
        //初始化UIMODEL之后的方法
        public override void AfterInitModel()
        {
            //this.Views[0].Fields[0].DefaultValue = thsi.co
            this.DayCheckIn.FieldBusinessDate.DefaultValue = PDContext.Current.LoginDate;
            this.DayCheckIn.FieldCheckInDate.DefaultValue = PDContext.Current.LoginDate;

            if (PDContext.Current.OrgRef != null)
            {
                this.DayCheckIn.FieldOrg.DefaultValue = PDContext.Current.OrgRef.ID;
                this.DayCheckIn.FieldOrg_Code.DefaultValue = PDContext.Current.OrgRef.CodeColumn;
                this.DayCheckIn.FieldOrg_Name.DefaultValue = PDContext.Current.OrgRef.NameColumn;
            }

            //bool isApproveFlow = false;
            //string strApproveFlow = UICommonHelper.GetValueSetCode("JLY_IsApproveFlow_DayCheckIn");
            //if (bool.TryParse(strApproveFlow, out isApproveFlow))
            //{
            //    this.DayCheckIn.FieldIsApproveFlow.DefaultValue = isApproveFlow;
            //}
        }

        //UIModel提交保存之前的校验操作.
        public override void OnValidate()
        {
            base.OnValidate();
            OnValidate_DefualtImpl();
            //your coustom code ...
        }


    }
}