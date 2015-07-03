

#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UFSoft.UBF.Business;

#endregion

namespace U9.VOB.Cus.HBHJianLiYuan.DeptItemSupplierBE {

	public partial class DeptItemSupplier{

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
                this.Org = UFIDA.U9.Base.Context.LoginOrg;

            // 赋值行号
            int maxLineNo = 0;
            if (this.DeptItemSupplierLine != null
                && this.DeptItemSupplierLine.Count > 0
                )
            {
                foreach (DeptItemSupplierLine line in this.DeptItemSupplierLine)
                {
                    if (line != null
                        && line.DocLineNo > maxLineNo
                        )
                    {
                        maxLineNo = line.DocLineNo;
                    }
                }

                foreach (DeptItemSupplierLine line in this.DeptItemSupplierLine)
                {
                    if (line != null
                        )
                    {
                        if (line.DocLineNo <= 0)
                        {
                            maxLineNo += 10;
                            line.DocLineNo = maxLineNo;
                        }
                    }
                }
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

            // 王希提：2、现在部门料品交叉档案上，同一部门同一物料可以有多个建议供应商，需要进行控制，同一部门同一无物料只允许有一个建议供应商
            if (this.DeptItemSupplierLine != null
                && this.DeptItemSupplierLine.Count > 0
                )
            { 
                StringBuilder msg = new StringBuilder();
                Dictionary<long, string> dicItemDocLineNo = new Dictionary<long, string>();
                foreach (DeptItemSupplierLine line in this.DeptItemSupplierLine)
                {
                    if (line != null
                        && line.ItemMasterKey != null
                        )
                    {
                        long itemID = line.ItemMasterKey.ID;

                        if (itemID > 0)
                        {
                            string info = string.Format("行{0}物料{1}"
                                , line.DocLineNo.ToString()
                                , line.ItemMaster.Name
                                );
                            if (!dicItemDocLineNo.ContainsKey(itemID))
                            {
                                dicItemDocLineNo.Add(itemID, info);
                            }
                            else
                            {
                                msg.Append(string.Format("[{0}]与[{1}] 物料相同! /r/n"
                                    , dicItemDocLineNo[itemID]
                                    , info
                                    ));
                            }
                        }
                    }
                }

                if (msg.Length > 0)
                {
                    throw new BusinessException(msg.ToString());
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
		private static DeptItemSupplier CreateDefault_Extend() {
			//TODO delete next code and add your custome code here
			throw new NotImplementedException () ;
		}
		/// <summary>
		/// Create DefaultComponent
		/// </summary>
		/// <returns>DefaultComponent Instance</returns>
		private  static DeptItemSupplier CreateDefaultComponent_Extend(){
			//TODO delete next code and add your custome code here
			throw new NotImplementedException () ;
		}
		
		#endregion 






		#region Model Methods
		#endregion		
	}
}
