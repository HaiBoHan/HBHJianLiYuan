using System;
using UFSoft.UBF.Eventing;
using UFSoft.UBF.Eventing.Configuration;
using UFSoft.UBF.Business;

namespace U9.VOB.Cus.HBHJianLiYuan
{
    public partial class CostWarning
    {
        public partial class CostWarningStateMachine
        {
            
            

            
		    //改变实体状态，开发人员在此实现实体状态的改变
            private void ChangeEntityState()
            {
				switch (this.CurrentState)
				{
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
    internal class ServiceOrderSubscriber : IEventSubscriber
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
                
            }
        }
    }
    #endregion
}