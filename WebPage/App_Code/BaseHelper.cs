using System;
using System.Text;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using CSIPCommonModel.BaseItem;
using Framework.Common.Utility;
using Framework.Common.Message;

/// <summary>
/// Summary description for BaseHelper
/// </summary>
public sealed class BaseHelper
{
 
    #region GetScript
    /// <summary>
    /// 用户Session丢失之后跳转到HomePage页面脚本
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public static string GetScriptForUserSessionOut(Page page)
    {
        StringBuilder sbScript = new StringBuilder();
        sbScript.Append("alert('")
        .Append(MessageHelper.GetMessage("0040"))
        .Append("');")
        .Append("window.location.href = '")
        .Append(page.ResolveUrl("~/Default.aspx")).Append("';");
        return sbScript.ToString();
    }

    /// <summary>
    /// 关闭自己并刷新父页面
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public static string GetScriptForUserSessionOut_CloseMe(Page page)
    {
        StringBuilder sbScript = new StringBuilder();
        sbScript.Append("if(window.opener != null && window.opener != undefined)")
        .Append("window.opener.location.reload();")
        .Append("window.close();");
        return sbScript.ToString();
    }
    public static string GetScriptForCloseMeAndGotoURL(Page page, string URL)
    {
        StringBuilder sbScript = new StringBuilder();
        sbScript.Append("if(window.opener != null && window.opener != undefined)")
        .Append("window.opener.location.replace('" + URL + "');")
        .Append("window.close();");
        return sbScript.ToString();
    }


    #endregion

    #region Set Control
    /// <summary>
    /// 设置Cancel按钮提示
    /// </summary>
    /// <param name="btnCancel"></param>
    public static void SetCancelBtn(Framework.WebControls.CustButton btnCancel)
    {
        btnCancel.ConfirmMsg = MessageHelper.GetMessage("0028");
    }


    /// <summary>
    /// 莉北ン陪ボ
    /// </summary>
    /// <param name="ShowID"></param>
    public static string GetShowText(string ShowID)
    {
        return WebHelper.GetShowText(ShowID);
    }

    /// <summary>
    /// 陪ボ狠ソ獺
    /// </summary>
    /// <param name="ShowID"></param>
    public static string ClientMsgShow(string strMsgID)
    {
        //return "ClientMsgShow('" + MessageHelper.GetMessage(strMsgID) + "');";
        return "window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage(strMsgID) + "' }, '*');";
    }

    /// <summary>
    /// 陪ボ狠ソ獺
    /// </summary>
    /// <param name="ShowID"></param>
    public static string ClientMsgShowByMessage(string strMessage)
    {
        //return "ClientMsgShow('" + strMessage + "');";
        return "window.parent.postMessage({ func: 'ClientMsgShow', data: '" + strMessage + "' }, '*');";
    }


    /// <summary>
    /// 陪ボ诀獺
    /// </summary>
    /// <param name="strMsgID"></param>
    public static string HostMsgShow(string strMsgID)
    {
        //return "HostMsgShow('" + MessageHelper.GetMessage(strMsgID) + "');";
        return "window.parent.postMessage({ func: 'HostMsgShow', data: '" + MessageHelper.GetMessage(strMsgID) + "' }, '*');";
    }

    /// <summary>
    /// 砞竚礘翴
    /// </summary>
    /// <param name="strControlID"></param>
    public static string SetFocus(string strControlID)
    {
        return @"setTimeout(""document.getElementById('" + strControlID + @"').focus();"",250);";
    }

    /// <summary>
    /// 陪ボ诀獺
    /// </summary>
    /// <param name="strMsgID"></param>
    public static string HostMsgShowByMessage(string strMessage)
    {
        //return "HostMsgShow('" + strMessage + "');";
        return "window.parent.postMessage({ func: 'HostMsgShow', data: '" + strMessage + "' }, '*');";
    }

    public static string GetScriptForWindowOpenURL(Page page, string URL)
    {
        StringBuilder sbScript = new StringBuilder();
        sbScript.Append("window.open('" + URL + "','','width='+(screen.availWidth-7)+',height='+(screen.availHeight-38)+',top=0,left=0,toolbar=no,menubar=no,scrollbars=yes,resizable=yes,location=no,status=no');");
        return sbScript.ToString();
    }

    public static string GetScriptForWindowOpenBackURL(Page page, string URL)
    {
        StringBuilder sbScript = new StringBuilder();
        sbScript.Append("window.open('" + URL + "','','width=500,height=600,top=0,left=0,toolbar=no,menubar=no,scrollbars=yes,resizable=yes,location=no,status=no');");
        return sbScript.ToString();
    }



    public static string GetScriptForWindowClose()
    {
        StringBuilder sbScript = new StringBuilder();
        sbScript.Append("alert('" + MessageHelper.GetMessage("00_00000000_037") + "');");
        sbScript.Append("setTimeout(closewindow,1000);");
        return sbScript.ToString();
    }

    public static string GetScriptForWindowErrorClose()
    {
        StringBuilder sbScript = new StringBuilder();
        sbScript.Append("alert('" + MessageHelper.GetMessage("00_00000000_000") + "');");
        sbScript.Append("setTimeout(closewindow,1000);");
        return sbScript.ToString();
    }


    public static string GetScriptForCloseWindow()
    {
        StringBuilder sbScript = new StringBuilder();
        sbScript.Append("window.close();");
        return sbScript.ToString();
    }

    #endregion

    /// <summary>
    /// 僥沮纐粄絪絏眔才﹃竊
    /// </summary>
    /// <param name="text">僥才﹃癸禜</param>
    /// <returns>int</returns>
    public static int GetByteLength(string text)
    {
        return System.Text.Encoding.Default.GetBytes(text).Length;
    }
}
