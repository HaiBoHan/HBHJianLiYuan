using System;
using UFSoft.UBF.Eventing;
using UFSoft.UBF.Eventing.Configuration;
using UFSoft.UBF.Business;
using UFIDA.U9.GeneralEvents;
using U9.VOB.HBHCommon.U9CommonBE;

namespace U9.VOB.Cus.HBHJianLiYuan
{
    public partial class TotalPayrollDoc
    {
        public partial class TotalPayrollDocStateMachine
        {
			//Opened状态进入方法，开发人员在此实现Opened状态进入的逻辑
            private State OpenedEnterImp(Event ev)
            {
                // To do ...
                return CurrentState;
            }            
			//Opened状态离开方法，开发人员在此实现Opened状态离开的逻辑
            private void OpenedLeaveImp(Event ev)
            {
                // To do ...
            }
			//Approving状态进入方法，开发人员在此实现Approving状态进入的逻辑
            private State ApprovingEnterImp(Event ev)
            {
                // To do ...
                return CurrentState;
            }            
			//Approving状态离开方法，开发人员在此实现Approving状态离开的逻辑
            private void ApprovingLeaveImp(Event ev)
            {
                // To do ...
            }
			//Approved状态进入方法，开发人员在此实现Approved状态进入的逻辑
            private State ApprovedEnterImp(Event ev)
            {
                // To do ...
                return CurrentState;
            }            
			//Approved状态离开方法，开发人员在此实现Approved状态离开的逻辑
            private void ApprovedLeaveImp(Event ev)
            {
                // To do ...
            }
            
            
            
 			//SumitEvent事件驱动方法的实现，开发人员在此实现SumitEvent的逻辑
            private State Opened_SumitEventImp(U9.VOB.Cus.HBHJianLiYuan.SumitEvent ev)
            {
                // To do ...
				State toState=CurrentState;
				//TODO:实现转移条件
    			if(true)
    			{
    				toState = State.Approving;
    			}
                return toState;
            }           
			            
 			//ApproveEvent事件驱动方法的实现，开发人员在此实现ApproveEvent的逻辑
            private State Approving_ApproveEventImp(U9.VOB.Cus.HBHJianLiYuan.ApproveEvent ev)
            {
                // To do ...
				State toState=CurrentState;
				//TODO:实现转移条件
    			if(true)
    			{
    				toState = State.Approved;
    			}
                return toState;
            }           

 			//RefusedEvent事件驱动方法的实现，开发人员在此实现RefusedEvent的逻辑
            private State Approving_RefusedEventImp(U9.VOB.Cus.HBHJianLiYuan.RefusedEvent ev)
            {
                // To do ...
				State toState=CurrentState;
				//TODO:实现转移条件
    			if(true)
    			{
    				toState = State.Opened;
    			}
                return toState;
            }           
			            
 			//UnApproveEvent事件驱动方法的实现，开发人员在此实现UnApproveEvent的逻辑
            private State Approved_UnApproveEventImp(U9.VOB.Cus.HBHJianLiYuan.UnApproveEvent ev)
            {
                // To do ...
				State toState=CurrentState;
				//TODO:实现转移条件
    			if(true)
    			{
    				toState = State.Opened;
    			}
                return toState;
            }           
			
            
		    //改变实体状态，开发人员在此实现实体状态的改变
            private void ChangeEntityState()
            {
				switch (this.CurrentState)
				{
					case State.Opened:
						// To do ...
						break;
					case State.Approving:
						// To do ...
						break;
					case State.Approved:
						// To do ...
						break;
				}
            }
        }
    }

    #region 事件侦听器
    ///TODO:在状态机代码的方法使用，如果该状态机不需要侦听事件，则该类可以删除
    ///用法： ServiceOrderSubscriber subscriber = new ServiceOrderSubscriber();
    ///       subscriber.EntityKey = this.Entity.Key;//this是指状态机实例
    ///       UFSoft.UBF.Eventing.EventBroker.Subscribe("UFIDA.UBF.GeneralEvents.ApprovalResultEvent", subscriber);
    [Persistent]
    [Serializable]
    internal class ServiceOrderSubscriber_TotalPayrollDoc : IEventSubscriber
    {
        UFSoft.UBF.Business.BusinessEntity.EntityKey entityKey;
        public UFSoft.UBF.Business.BusinessEntity.EntityKey EntityKey
        {
            get
            {
                return entityKey;
            }
            set
            {
                entityKey = value;
            }
        }
        ///事件处理方法
        public void Notify(params object[] args)
        {
            if (this.EntityKey == null)
                return;
            if (args.Length == 1)
            {
                //UFIDA.UBF.GeneralEvents.ApprovalResultEvent ev = args[0] as UFIDA.UBF.GeneralEvents.ApprovalResultEvent;
                //if (ev != null && ev.EntityKey!=null&&ev.EntityKey.ID == this.EntityKey.ID)
                //{
                //    ServiceOrder entity = this.EntityKey.GetEntity() as ServiceOrder;
                //    entity.StateMachineInstance.Approved(ev);
                //}
                //TODO：参照上面实现自己的逻辑

                ApprovalResultEvent approvalResultEvent = args[0] as ApprovalResultEvent;
                if (approvalResultEvent != null && approvalResultEvent.EntityKey != null && approvalResultEvent.EntityKey.ID == this.EntityKey.ID)
                {
                    TotalPayrollDoc entity = this.EntityKey.GetEntity() as TotalPayrollDoc;
                    using (ISession session = Session.Open())
                    {
                        if (approvalResultEvent.IsTerminate)
                        {
                            entity.Status = DocStatus.Opened;
                            entity.WFCurrentState = DocStatus.Opened.Value;
                            //entity.ActivityType = ActivityTypeEnum.SrvUpdate;
                            //this.SetPRLineStatus(entity, ActivityTypeEnum.SrvUpdate);
                        }
                        else if (approvalResultEvent.IsApprovaled)
                        {
                            entity.Status = DocStatus.Approved;
                            entity.WFCurrentState = DocStatus.Approved.Value;
                            //entity.ActivityType = ActivityTypeEnum.UIUpdate;
                            //this.SetPRLineStatus(entity, ActivityTypeEnum.UIUpdate);
                        }
                        else
                        {
                            entity.Status = DocStatus.Opened;
                            entity.WFCurrentState = DocStatus.Opened.Value;
                            //entity.ActivityType = ActivityTypeEnum.SrvUpdate;
                            //this.SetPRLineStatus(entity, ActivityTypeEnum.SrvUpdate);
                        }
                        session.Commit();
                    }
                    UFIDA.U9.Approval.Util.EventHelper.UnsubscribeApprovalResultEvent(this.EntityKey, this);
                }

            }
        }
    }
    #endregion


}