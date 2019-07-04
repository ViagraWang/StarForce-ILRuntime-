using Game.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Hotfix
{
    /// <summary>
    /// UI界面逻辑的基类
    /// </summary>
    public abstract class UIFormBase
    {
        //UI界面数据
        public UserUIData UserUIData { get; private set; }

        public HotUIForm RuntimeUIForm { get { return UserUIData.RuntimeUIForm; } }

        /// <summary>
        /// 界面初始化
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public virtual void OnInit(object userData)
        {
            UserUIData = userData as UserUIData;
        }

        /// <summary>
        /// 界面打开
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public abstract void OnOpen(object userData);

        /// <summary>
        /// 界面关闭
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public abstract void OnClose(object userData);

        /// <summary>
        /// 界面暂停
        /// </summary>
        public virtual void OnPause() { }

        /// <summary>
        /// 界面暂停恢复
        /// </summary>
        public virtual void OnResume() { }

        /// <summary>
        /// 界面覆盖
        /// </summary>
        public virtual void OnCover() { }

        /// <summary>
        /// 界面恢复覆盖
        /// </summary>
        public virtual void OnReveal() { }

        /// <summary>
        /// 界面激活
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public virtual void OnRefocus(object userData) { }

        /// <summary>
        /// 界面轮询
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// 界面深度改变
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度</param>
        public virtual void OnDepthChanged(int uiGroupDepth, int depthInUIGroup) { }

    }
}
