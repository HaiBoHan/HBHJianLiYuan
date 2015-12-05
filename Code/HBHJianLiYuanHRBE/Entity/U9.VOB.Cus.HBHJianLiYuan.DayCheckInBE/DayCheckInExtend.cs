﻿

#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UFSoft.UBF.PL;
using UFSoft.UBF.Business;
using UFIDA.U9.Base;

#endregion

namespace U9.VOB.Cus.HBHJianLiYuan {

	public partial class DayCheckIn{

		#region Custom Constructor

		#endregion

		#region before & after CUD V 
		/*	实体提交的事件顺序示例(QHELP) 主实体A 组合 非主实体B .
		/ (新增A和B两个实例)A.OnSetDefaultValue->B.OnSetDefaultValue-> B.OnValidate ->A.OnValidate ->A.OnInserting ->B.OnInserting ->产生提交SQL ->B.OnInserted ->A.OnInserted
		/ (仅修改B,A也会被修改))A.OnSetDefaultValue->B.OnSetDefaultValue-> B.OnValidate ->A.OnValidate ->A.OnUpdating ->B.OnUpdating ->产生提交SQL ->B.OnUpdated ->A.OnUpdated
		/ (删除A,B会被级联删除))A.OnDeleting ->B.OnDeleting ->产生提交SQL ->B.OnDeleted ->A.OnDeleted
		/	产生提交SQL顺序则看其外键，增修一对多先A后B，一对一先B后A。　删除一对多先B后A，一对一先A后B .
		*/	
		/// <summary>
		/// 设置默认值
		/// </summary>
		protected override void OnSetDefaultValue()
		{
            base.OnSetDefaultValue();

            if (this.Org == null)
            {
                this.Org = Context.LoginOrg;
            }
		}
		/// <summary>
		/// before Insert
		/// </summary>
		protected override void OnInserting() {
			base.OnInserting();
			// TO DO: write your business code here...
		}

		/// <summary>
		/// after Insert
		/// </summary>
		protected override void OnInserted() {
			base.OnInserted();
			// TO DO: write your business code here...
		}

		/// <summary>
		/// before Update
		/// </summary>
		protected override void OnUpdating() {
			base.OnUpdating();
			// TO DO: write your business code here...
		}

		/// <summary>
		/// after Update
		/// </summary>
		protected override void OnUpdated() {
			base.OnUpdated();
			// TO DO: write your business code here...
		}


		/// <summary>
		/// before Delete
		/// </summary>
		protected override void OnDeleting() {
			base.OnDeleting();
			// TO DO: write your business code here...
		}

		/// <summary>
		/// after Delete
		/// </summary>
		protected override void OnDeleted() {
			base.OnDeleted();
			// TO DO: write your business code here...
		}

		/// <summary>
		/// on Validate
		/// </summary>
		protected override void OnValidate() {
			base.OnValidate();
			this.SelfEntityValidator();
			// TO DO: write your business code here...

            if (this.SysState != UFSoft.UBF.PL.Engine.ObjectState.Deleted)
            {
                if (this.DepartmentKey == null)
                    throw new BusinessException("部门不可为空!");


                DayCheckIn old = DayCheckIn.Finder.Find("Department=@Dept and ID!=@ID and Convert(char(8),CheckInDate,112)=Convert(char(8),@CheckInDate,112)"
                    , new OqlParam(this.DepartmentKey.ID)
                    , new OqlParam(this.ID)
                    , new OqlParam(this.CheckInDate)
                    );
                if (old != null)
                {
                    string strMsg = string.Format("存在相同部门[{0}],相同日期[{1}]的考勤记录!"
                            , this.Department.Name
                            , this.CheckInDate.ToString("yyyy-MM-dd")
                            );
                    throw new BusinessException(strMsg);
                }
            }
		}
		#endregion
		
		#region 异常处理，开发人员可以重新封装异常
		public override void  DealException(Exception e)
        	{
          		base.DealException(e);
          		throw e;
        	}
		#endregion

		#region  扩展属性代码区
		
		#endregion

		#region CreateDefault
		private static DayCheckIn CreateDefault_Extend() {
			//TODO delete next code and add your custome code here
			throw new NotImplementedException () ;
		}
		/// <summary>
		/// Create DefaultComponent
		/// </summary>
		/// <returns>DefaultComponent Instance</returns>
		private  static DayCheckIn CreateDefaultComponent_Extend(){
			//TODO delete next code and add your custome code here
			throw new NotImplementedException () ;
		}
		
		#endregion 






		#region Model Methods
		#endregion		
	}
}
