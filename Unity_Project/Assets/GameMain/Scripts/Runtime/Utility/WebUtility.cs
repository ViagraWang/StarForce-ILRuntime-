using System;

namespace Game.Runtime {	
	//Web扩展工具
	public static class WebUtility
	{
	    //将字符串转换为它的转义表示形式
        //对冒号(:)、斜杠(/)、空格、中文、井号(#)都进行了编码，可对参数编码
	    //Uri.EscapeDataString("http://www.baidu.com:80/2013/123.html?id=1")  //输出 http%3A%2F%2Fwww.baidu.com%3A80%2F2013%2F123.html%3Fid%3D1     Uri转义
	    //Uri.EscapeUriString对URI的网址部分编码，用Uri.EscapeDataString对URI中传递的参数进行编码。 
	    public static string EscapeString(string stringToEscape)
	    {
	        return Uri.EscapeDataString(stringToEscape);
	    }
	
	    //将字符串转换为它的非转义表示形式
	    //Uri.UnescapeDataString("http%3A%2F%2Fwww.baidu.com%3A80%2F2013%2F123.html%3Fid%3D1") //输出 http://www.baidu.com:80/2013/123.html?id=1 ， 将Url还原成不转义的形式
	    public static string UnescapeString(string stringToUnescape)
	    {
	        return Uri.UnescapeDataString(stringToUnescape);
	    }
	}
}
