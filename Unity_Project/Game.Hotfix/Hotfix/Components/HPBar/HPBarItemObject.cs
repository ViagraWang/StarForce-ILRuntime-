using UnityEngine;

namespace Game.Hotfix
{	
	//血条对象
	public class HPBarItemObject : ObjectBase
	{
	    public HPBarItemObject(object target) : base(target)
        {
            
        }

        //释放
        protected internal override void Release(bool isShutdown)
	    {
	        HPBarItem hpBarItem = Target as HPBarItem;
	        if (hpBarItem == null)
	            return;
	
	        Object.Destroy(hpBarItem.ReferenceCollector.CachedGameObject);
	    }
	
	}
}
