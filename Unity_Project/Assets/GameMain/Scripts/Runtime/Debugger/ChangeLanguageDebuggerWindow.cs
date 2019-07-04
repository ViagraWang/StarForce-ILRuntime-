using GameFramework.Debugger;
using GameFramework.Localization;
using UnityEngine;
using UnityGameFrame.Runtime;

namespace Game.Runtime {	
	public class ChangeLanguageDebuggerWindow : IDebuggerWindow
	{
	    private Vector2 m_ScrollPosition = Vector2.zero;
	    private bool m_NeedRestart = false; //是否需要重启的标志位
	
	    public void Initialize(params object[] args)
	    {
	
	    }
	
	    public void OnDraw()
	    {
	        m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition);
	        {
	            DrawSectionChangeLanguage();
	        }
	        GUILayout.EndScrollView();
	    }
	
	    public void OnEnter()
	    {
	
	    }
	
	    public void OnLeave()
	    {
	
	    }
	
	    public void OnUpdate(float elapseSeconds, float readElapseSeconds)
	    {
	        if (m_NeedRestart)
	        {
	            m_NeedRestart = false;
	            UnityGameFrame.Runtime.GameEntry.Shutdown(ShutdownType.Restart);
	        }
	    }
	
	    public void Shutdown()
	    {
	
	    }
	
	    private void DrawSectionChangeLanguage()
	    {
	        GUILayout.Label("<b>Change Language</b>");
	        GUILayout.BeginHorizontal("box");
	        {
	            if(GUILayout.Button("Chinese Simplified", GUILayout.Height(30)))
	            {
	                GameEntry.Localization.Language = Language.ChineseSimplified;
	                SaveLanguage();
	            }
	            if (GUILayout.Button("Chinese Traditional", GUILayout.Height(30)))
	            {
	                GameEntry.Localization.Language = Language.ChineseTraditional;
	                SaveLanguage();
	            }
	            if (GUILayout.Button("English", GUILayout.Height(30)))
	            {
	                GameEntry.Localization.Language = Language.English;
	                SaveLanguage();
	            }
	        }
	        GUILayout.EndHorizontal();
	    }
	
	    //保存语言
	    private void SaveLanguage()
	    {
	        GameEntry.Setting.SetString(RuntimeConstant.Setting.Language, GameEntry.Localization.Language.ToString());
	        GameEntry.Setting.Save();
	        m_NeedRestart = true;
	    }
	
	}
}
