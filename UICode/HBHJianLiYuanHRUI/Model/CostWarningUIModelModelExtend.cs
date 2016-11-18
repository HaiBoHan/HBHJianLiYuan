#region Using directives

using System;
using System.Collections;
using System.Data;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.UI.MD.Runtime.Implement;
using UFIDA.U9.UI.PDHelper;

#endregion

namespace CostWarningUIModel
{	public partial class CostWarningUIModelModel 
	{
        //初始化UIMODEL之后的方法
        public override  void AfterInitModel()
        {
            //this.Views[0].Fields[0].DefaultValue = thsi.co

            this.CostWarning.FieldImportDate.DefaultValue = PDContext.Current.LoginDate;

            this.CostWarning_CostWarningLine.FieldDate.DefaultValue = PDContext.Current.LoginDate;

            // 早中晚夜  (0123)
            this.CostWarning_CostWarningLine.FieldMealTime.DefaultValue = 0;
        }
        
        //UIModel提交保存之前的校验操作.
        public override void OnValidate()
        {
        		base.OnValidate() ;
            OnValidate_DefualtImpl();
            //your coustom code ...
        }
	}
}