using System.Collections.Generic;
using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFrame.Runtime;

namespace Game.Runtime {	
	public class ProcedureUpdateResource : ProcedureBase
	{
	    //更新长度数据
	    private class UpdateLengthData
	    {
	        private readonly string m_Name;
	
	        public UpdateLengthData(string name)
	        {
	            m_Name = name;
	        }
	
	        public string Name { get { return m_Name; } }
	
	        public int Length { get; set; }
	    }
	
	    public override bool UseNativeDialog { get { return true; } }
	
	    private bool m_UpdateAllComplete = false;   //更新完成的标志位
	    private int m_UpdateCount = 0;  //更新的数量
	    private int m_RemoveCount = 0;  //移除的资源数量
	    private long m_UpdateTotalLength = 0L; //需要更新的资源总大小
	    private long m_UpdateTotalZipLength = 0L;   //需要更新压缩包的总大小
	    private int m_UpdateSuccessCount = 0;   //更新成功的数量
	    private List<UpdateLengthData> m_UpdateLengthData = new List<UpdateLengthData>();
	    private UpdateResourceForm m_UpdateResourceForm = null;
	
	    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
	    {
	        base.OnEnter(procedureOwner);
	
	        m_UpdateAllComplete = false;
	        m_UpdateCount = 0;
	        m_RemoveCount = 0;
	        m_UpdateTotalLength = 0L;
	        m_UpdateTotalZipLength = 0L;
	        m_UpdateSuccessCount = 0;
	        m_UpdateLengthData.Clear();
	        m_UpdateResourceForm = null;
	
	        //注册事件
	        GameEntry.Event.Subscribe(ResourceUpdateStartEventArgs.EventId, OnResourceUpdateStart);
	        GameEntry.Event.Subscribe(ResourceUpdateChangedEventArgs.EventId, OnResourceUpdateChanged);
	        GameEntry.Event.Subscribe(ResourceUpdateSuccessEventArgs.EventId, OnResourceUpdateSuccess);
	        GameEntry.Event.Subscribe(ResourceUpdateFailureEventArgs.EventId, OnResourceUpdateFailure);
	
	        GameEntry.Resource.CheckResources(OnCheckResourcesComplete); //检查资源
	    }
	
	    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
	    {
	        base.OnLeave(procedureOwner, isShutdown);
	        //删除更新资源的界面
	        if (m_UpdateResourceForm != null)
	        {
	            Object.Destroy(m_UpdateResourceForm.gameObject);
	            m_UpdateResourceForm = null;
	        }
	
	        //反注册事件
	        GameEntry.Event.Unsubscribe(ResourceUpdateStartEventArgs.EventId, OnResourceUpdateStart);
	        GameEntry.Event.Unsubscribe(ResourceUpdateChangedEventArgs.EventId, OnResourceUpdateChanged);
	        GameEntry.Event.Unsubscribe(ResourceUpdateSuccessEventArgs.EventId, OnResourceUpdateSuccess);
	        GameEntry.Event.Unsubscribe(ResourceUpdateFailureEventArgs.EventId, OnResourceUpdateFailure);
	    }
	
	    protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
	    {
	        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
	
	        if (!m_UpdateAllComplete)
	            return;
	
	        ChangeState<ProcedurePreload>(procedureOwner);  //切换到预加载流程
	    }
	
	    //资源检查完成的回调
	    private void OnCheckResourcesComplete(int removedCount, int updateCount, long updateTotalLength, long updateTotalZipLength)
	    {
	        m_RemoveCount = removedCount;
	        m_UpdateCount = updateCount;
	        m_UpdateTotalLength = updateTotalLength;
	        m_UpdateTotalZipLength = updateTotalZipLength;
	
	        Log.Info("Check resources complete, remove count is '{0}', need update count is '{1}', zip length is '{2}', unzip length is '{3}'.", m_RemoveCount, m_UpdateCount, m_UpdateTotalZipLength, m_UpdateTotalLength);
	
	        if (updateCount <= 0)   //不需要更新资源
	        {
	            ProcessUpdateResourcesComplete();
	            return;
	        }
	
	        if(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
	        {
	            //TODO:移动网络下载需要提示
	            GameEntry.UI.OpenDialog(new DialogParams
	            {
	                Mode = 2,
	                Title = "警告",
	                Message = "需要更新资源大小：" + m_UpdateTotalZipLength + "\n会耗费流量，是否进行更新？",
	                ConfirmText = "更新",
	                OnClickConfirm = StartUpdateResources,
	                CancelText = "取消",
	                OnClickCancel = delegate (object userData) { UnityGameFrame.Runtime.GameEntry.Shutdown(ShutdownType.Quit); }
	            });
	            return;
	        }
	
	        StartUpdateResources(null);
	    }
	
	    //开始更新资源
	    private void StartUpdateResources(object userData)
	    {
	        if (m_UpdateResourceForm == null)
	            m_UpdateResourceForm = Object.Instantiate(GameEntry.BuiltinData.UpdateResourceFormTemplate);
	
	        Log.Info("Start update resources...");
	        Log.Info("Start update resource group 'Base' ...");
	        GameEntry.Resource.UpdateResources("Base", OnUpdateResourcesComplete);  //先更新Base资源
	    }
	
	    //处理资源更新完成
	    private void ProcessUpdateResourcesComplete()
	    {
	        m_UpdateAllComplete = true;
	    }
	
	    //刷新更新进度
	    private void RefreshProgress()
	    {
	        long currentTotalUpdateLength = 0L;
	        for (int i = 0; i < m_UpdateLengthData.Count; i++)
	        {
	            currentTotalUpdateLength += m_UpdateLengthData[i].Length;
	        }
	
	        float progressTotal = (float)currentTotalUpdateLength / m_UpdateTotalZipLength;
	        string descriptionText = GameEntry.Localization.GetString("UpdateResource.Tips", m_UpdateSuccessCount.ToString(), m_UpdateCount.ToString(), GetLengthString(currentTotalUpdateLength), GetLengthString(m_UpdateTotalZipLength), progressTotal, GetLengthString((int)GameEntry.Download.CurrentSpeed));
	        m_UpdateResourceForm.SetProgress(progressTotal, descriptionText);
	    }
	
	    //转换长度为可读字符串
	    private string GetLengthString(long length)
	    {
	        if (length < 1024)
	            return string.Format("{0} Bytes", length.ToString());
	
	        if (length < 1024 * 1024)
	            return string.Format("{0} KB", (length / 1024f).ToString("F2"));
	
	        if (length < 1024 * 1024 * 1024)
	            return string.Format("{0} MB", (length / 1024f / 1024f).ToString("F2"));
	
	        return string.Format("{0} GB", (length / 1024f / 1024f / 1024f).ToString("F2"));
	    }
	
	    //开始更新的回调
	    private void OnResourceUpdateStart(object sender, BaseEventArgs e)
	    {
	        ResourceUpdateStartEventArgs ne = e as ResourceUpdateStartEventArgs;
	
	        for (int i = 0; i < m_UpdateLengthData.Count; i++)
	        {
	            if (m_UpdateLengthData[i].Name == ne.Name)
	            {
	                Log.Warning("Update resource '{0}' is invalid.", ne.Name);
	                m_UpdateLengthData[i].Length = 0;
	                RefreshProgress();
	                return;
	            }
	        }
	
	        m_UpdateLengthData.Add(new UpdateLengthData(ne.Name));
	    }
	
	    //更新改变的回调
	    private void OnResourceUpdateChanged(object sender, BaseEventArgs e)
	    {
	        ResourceUpdateChangedEventArgs ne = e as ResourceUpdateChangedEventArgs;
	
	        for (int i = 0; i < m_UpdateLengthData.Count; i++)
	        {
	            if (m_UpdateLengthData[i].Name == ne.Name)
	            {
	                m_UpdateLengthData[i].Length = ne.CurrentLength;
	                RefreshProgress();
	                return;
	            }
	        }
	
	        Log.Warning("Update resource '{0}' is invalid.", ne.Name);
	    }
	
	    //更新成功的回调
	    private void OnResourceUpdateSuccess(object sender, BaseEventArgs e)
	    {
	        ResourceUpdateSuccessEventArgs ne = e as ResourceUpdateSuccessEventArgs;
	        Log.Info("Update resource '{0}' success.", ne.Name);
	
	        for (int i = 0; i < m_UpdateLengthData.Count; i++)
	        {
	            if (m_UpdateLengthData[i].Name == ne.Name)
	            {
	                m_UpdateLengthData[i].Length = ne.ZipLength;
	                m_UpdateSuccessCount++;
	                RefreshProgress();
	                return;
	            }
	        }
	
	        Log.Warning("Update resource '{0}' is invalid.", ne.Name);
	    }
	
	    //更新失败的回调
	    private void OnResourceUpdateFailure(object sender, BaseEventArgs e)
	    {
	        ResourceUpdateFailureEventArgs ne = e as ResourceUpdateFailureEventArgs;
	        if (ne.RetryCount >= ne.TotalRetryCount)
	        {
	            Log.Error("Update resource '{0}' failure from '{1}' with error message '{2}', retry count '{3}'.", ne.Name, ne.DownloadUri, ne.ErrorMessage, ne.RetryCount.ToString());
	            return;
	        }
	        else
	        {
	            Log.Info("Update resource '{0}' failure from '{1}' with error message '{2}', retry count '{3}'.", ne.Name, ne.DownloadUri, ne.ErrorMessage, ne.RetryCount.ToString());
	        }
	
	        for (int i = 0; i < m_UpdateLengthData.Count; i++)
	        {
	            if (m_UpdateLengthData[i].Name == ne.Name)
	            {
	                m_UpdateLengthData.Remove(m_UpdateLengthData[i]);
	                RefreshProgress();
	                return;
	            }
	        }
	
	        Log.Warning("Update resource '{0}' is invalid.", ne.Name);
	    }
	
	    //更新资源完成的回调
	    private void OnUpdateResourcesComplete(GameFramework.Resource.IResourceGroup resourceGroup, bool result)
	    {
	        if (resourceGroup.Name.Equals("Base"))  //更新完Base资源
	        {
	            if (result)
	            {
	                Log.Info("Update resource group 'Base' complete with no errors.");
	                Log.Info("Start update resource group 'Music' ...");
	                GameEntry.Resource.UpdateResources("Music", OnUpdateResourcesComplete); //更新Music资源
	            }
	            else
	            {
	                Log.Error("Update resource group 'Base' complete with errors.");
	            }
	        }
	        else if (resourceGroup.Name.Equals("Music"))
	        {
	            if (result)
	            {
	                Log.Info("Update resource group 'Music' complete with no errors.");
	                Log.Info("Start update other resources ...");
	                GameEntry.Resource.UpdateResources(OnUpdateResourcesComplete);  //更新其他所有资源
	            }
	            else
	            {
	                Log.Error("Update resource group 'Music' complete with errors.");
	            }
	        }
	        else //resourceGroup.Name == string.Empty
	        {
	            if (result)
	            {
	                Log.Info("Update other resources complete with no errors.");
	                ProcessUpdateResourcesComplete();
	            }
	            else
	            {
	                Log.Error("Update other resources complete with errors.");
	            }
	        }
	
	    }
	
	}
}
