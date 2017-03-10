

#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using U9.VOB.HBHCommon.U9CommonBE;
using UFSoft.UBF.Business;
using UFIDA.U9.CBO.HR.Person;

#endregion

namespace U9.VOB.Cus.HBHJianLiYuan {

	public partial class DayCheckInLine{

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

            if (this.DayCheckIn != null
                && this.DayCheckIn.Status != DocStatus.Opened
                )
            {
                throw new BusinessException(string.Format("部门[{0}]考勤日[{1}]非开立状态不允许删除!"
                    , this.DayCheckIn.Department != null ? this.DayCheckIn.Department.Name : string.Empty
                    , this.DayCheckIn.CheckInDate.ToString("yyyy-MM-dd")
                    ));
            }
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
        protected override void OnValidate()
        {
            base.OnValidate();
            this.SelfEntityValidator();
            // TO DO: write your business code here...

            if (this.DayCheckIn != null)
            {
                if (this.DayCheckIn.Status == DocStatus.Approved
                    || this.DayCheckIn.Status == DocStatus.Closed
                    )
                {
                    if (this.OriginalData != null)
                    {
                        UnchangedValidate<long>("EmployeeArchive");
                        UnchangedValidate<int>("CheckType");

                        UnchangedValidate<decimal>("FullTimeDay");
                        UnchangedValidate<decimal>("PartTimeDay");
                        UnchangedValidate<decimal>("HourlyDay");
                    }
                }
            }

            if (this.CheckType == CheckTypeEnum.FullTimeStaff)
            {
                if (this.PartTimeDay > 0)
                {
                    throw new BusinessException(string.Format("行[{0}]考勤类型为 全日制出勤时，非全日制员工出勤字段不允许大于0!"
                        , this.DocLineNo
                        ));
                }
            }
            else if (this.CheckType == CheckTypeEnum.PartTimeStaff)
            {
                if (this.FullTimeDay > 0)
                {
                    throw new BusinessException(string.Format("行[{0}]考勤类型为 非全日制出勤时，全日制员工出勤字段不允许大于0!"
                        , this.DocLineNo
                        ));
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
		
		private  static DayCheckInLine CreateDefault_Extend(U9.VOB.Cus.HBHJianLiYuan.DayCheckIn parentEntity) {
			//TODO delete next code and add your custome code here
			throw new NotImplementedException () ;
		}
	    		
		#endregion 






		#region Model Methods


        private void UnchangedValidate<T>(string field)
        {
            if (DayCheckIn.IsChanged<T>(this, field))
            {
                throw new BusinessException(string.Format("考勤单据已审核(或关闭)，行[{0}]字段[{1}]不允许修改!"
                    , this.DocLineNo
                    , DayCheckInLine.EntityRes.GetResource(field)
                    ));
            }
        }


		#endregion		
	}
}
