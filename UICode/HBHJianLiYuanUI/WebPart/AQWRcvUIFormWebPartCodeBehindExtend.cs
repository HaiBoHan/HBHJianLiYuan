using System;
using System.Text;
using System.Collections;
using System.Xml;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Threading;

using Telerik.WebControls;
using UFSoft.UBF.UI.WebControls;
using UFSoft.UBF.UI.Controls;
using UFSoft.UBF.Util.Log;
using UFSoft.UBF.Util.Globalization;

using UFSoft.UBF.UI.IView;
using UFSoft.UBF.UI.Engine;
using UFSoft.UBF.UI.MD.Runtime;
using UFSoft.UBF.UI.ActionProcess;
using UFSoft.UBF.UI.WebControls.ClientCallBack;
using U9.VOB.HBHCommon.HBHCommonUI;
using System.Collections.Generic;
using HBH.DoNet.DevPlatform.U9Mapping;
using HBH.DoNet.DevPlatform.EntityMapping;
using U9.VOB.Cus.HBHJianLiYuan.Proxy;



/***********************************************************************************************
 * Form ID: 
 * UIFactory Auto Generator 
 ***********************************************************************************************/
namespace AQWRcvUIModel
{
    public partial class AQWRcvUIFormWebPart
    {
        #region Custome eventBind
	
		 
				//BtnOk_Click...
        private void BtnOk_Click_Extend(object sender, EventArgs e)
        {
            //调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            //BtnOk_Click_DefaultImpl(sender,e);

            this.Model.ClearErrorMessage();

            IUIRecordCollection selectedRecords = this.Model.DispatchinView.SelectRecords;

            if (selectedRecords != null
                && selectedRecords.Count > 0
                )
            {
                AQWTransToRcvSVProxy proxy = new AQWTransToRcvSVProxy();

                proxy.HeadIDs = new List<string>();
                foreach (DispatchinViewRecord record in selectedRecords)
                {
                    proxy.HeadIDs.Add(record.ldiid);
                }

                proxy.Do();
            }
            else
            {
                UICommonHelper.ShowErrorInfo(this, "请选择收货单!");
            }
        }
		 
				//BtnClose_Click...
		private void BtnClose_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
			
		
			BtnClose_Click_DefaultImpl(sender,e);
		}	
		 
				//BtnSearch_Click...
		private void BtnSearch_Click_Extend(object sender, EventArgs  e)
		{
			//调用模版提供的默认实现.--默认实现可能会调用相应的Action.
            //BtnSearch_Click_DefaultImpl(sender,e);

            this.Model.ClearErrorMessage();

            FilterViewRecord filterRecord = this.Model.FilterView.FocusedRecord;

            if (filterRecord == null)
            {
                UICommonHelper.ShowErrorInfo(this, "过滤条件为空,请检查模型!");
                return;
            }

            string strProcName = "HBH_SP_JianLiYuan_GetAQWRcvInfo";
            List<ParamDTO> lstParam = new List<ParamDTO>();
            {
                ParamDTO paramDTO = new ParamDTO();
                paramDTO.ParamName = "StartDate";
                paramDTO.ParamDirection = ParameterDirection.Input;
                paramDTO.ParamType = DbType.DateTime;
                paramDTO.ParamValue = filterRecord.StartDate;

                lstParam.Add(paramDTO);
            }
            {
                ParamDTO paramDTO = new ParamDTO();
                paramDTO.ParamName = "EndDate";
                paramDTO.ParamDirection = ParameterDirection.Input;
                paramDTO.ParamType = DbType.DateTime;
                paramDTO.ParamValue = filterRecord.EndDate;

                lstParam.Add(paramDTO);
            }
            {
                ParamDTO paramDTO = new ParamDTO();
                paramDTO.ParamName = "DocNo";
                paramDTO.ParamDirection = ParameterDirection.Input;
                paramDTO.ParamType = DbType.String;
                paramDTO.ParamValue = filterRecord.Code;

                lstParam.Add(paramDTO);
            }

            DataTable dt;
            EntityResult result = U9Helper.GetResultByProcess(strProcName, out dt, lstParam.ToArray());

            if (result != null)
            {
                if (result.Sucessfull)
                {
                    this.Model.DispatchinView.Clear();

                    if (dt != null
                        && dt.Rows != null
                        && dt.Rows.Count > 0
                        )
                    {
                        int i = 1;
                        foreach (DataRow row in dt.Rows)
                        {
                            if (row != null)
                            { 
                                //this.Model.DispatchinView
                                DispatchinViewRecord record = this.Model.DispatchinView.AddNewUIRecord();

                                record.LineNum = i++;
                                record.ldiid = row["ldiid"].GetString();
                                record.code = row["code"].GetString();
                                record.lsid = row["lsid"].GetString();
                                record.ldid = row["ldid"].GetString();
                                record.status = row["status"].GetString();
                                record.ctime = row["ctime"].GetString();
                                record.utime = row["utime"].GetString();
                                record.ldoid = row["ldoid"].GetString();
                                record.cuid = row["cuid"].GetString();
                                record.auid = row["auid"].GetString();
                                record.atime = row["atime"].GetString();
                                record.ouid = row["ouid"].GetString();
                                record.comment = row["comment"].GetString();
                                record.bsetcheck = row["bsetcheck"].GetString();
                                record.lrsid = row["lrsid"].GetString();
                                record.nccode = row["nccode"].GetString();
                                record.arrivetime = row["arrivetime"].GetString();
                                record.bcheck = row["bcheck"].GetString();
                                record.buid = row["buid"].GetString();
                                record.btime = row["btime"].GetString();
                                record.pdatetag = row["pdatetag"].GetString();
                                record.ordertype = row["ordertype"].GetString();
                                record.confirmuid = row["confirmuid"].GetString();
                                record.confirmtime = row["confirmtime"].GetString();
                                record.rlrsid = row["rlrsid"].GetString();
                                record.returnuid = row["returnuid"].GetString();
                                record.returntime = row["returntime"].GetString();
                                record.bgenerateorder = row["bgenerateorder"].GetString();
                            }
                        }
                    }
                }
                else
                {
                    UICommonHelper.ShowErrorInfo(this, result.Message);
                }
            }
		}


        #endregion
        

        #region 自定义数据初始化加载和数据收集

        private void OnLoadData_Extend(object sender)
		{	
            //OnLoadData_DefaultImpl(sender);
		}

		private void OnDataCollect_Extend(object sender)
		{	
			OnDataCollect_DefaultImpl(sender);
		}

		#endregion  

        #region 自己扩展 Extended Event handler 

		public void AfterOnLoad()
		{
            UICommonHelper.AddFocusedRecord(this.Model.FilterView);
		}

        public void AfterCreateChildControls()
        {
            // 实现个性化
            UFIDA.U9.UI.PDHelper.PersonalizationHelper.SetPersonalizationEnable(this, true);	
        }
        
        public void AfterEventBind()
        {
        }
        
		public void BeforeUIModelBinding()
		{

		}

		public void AfterUIModelBinding()
		{


		}


        #endregion
    }
}