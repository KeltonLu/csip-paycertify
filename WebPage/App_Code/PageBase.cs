//******************************************************************
//*  作    者：占偉林(James)
//*  功能說明：系統角色維護

//*  創建日期：2009/07/10
//*  修改記錄：調整Alert語法 by Ares Stanley 20220122


//*<author>            <time>            <TaskID>                <desc>
//* Ares Stanley    2022/02/14    20210058-CSIP作業服務平台現代化II    調整webconfig取參數方式
//*******************************************************************

using System;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Text;
using System.Reflection;
using CSIPPayCertify.BusinessRules;
using CSIPCommonModel.BusinessRules;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;
using Framework.Common.Utility;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using Framework.Common.JavaScript;
using System.IO;
using Framework.Common;
using PageAction = Framework.Common.PageAction;

/// <summary>
/// 要做權限判斷的頁面基礎類別

/// </summary>
public class PageBase : System.Web.UI.Page
{
    protected long ProgramBeginRunTime;
    protected long programRunTime;

    protected StringBuilder sbRegScript;
    protected string strClientMsg = "";
    protected string strHostMsg = "";
    protected string strAlertMsg = "";
    protected string strIsShow = "";

    protected eMstType etMstType = eMstType.NoAlert;
    protected eClientMstType etClientMstType = eClientMstType.NoAlert;
    protected enum eMstType
    {
        //* 無需做pop動作
        NoAlert = 1,
        //* 查詢操作
        Select = 2,
        //* 新增、異動、刪除操作
        Control = 3,

    }
    /*增加記錄網頁訊息的struct Add by 陳靜嫻2009-09-21 Start */
    public struct structPageInfo
    {
        public string strPageCode;//*網頁FunctionID
        public string strPageName;//*網頁名稱
    }
    /*增加記錄網頁訊息的struct Add by 陳靜嫻2009-09-21 End */
    protected enum eClientMstType
    {
        //* 無需做pop動作
        NoAlert = 1,
        //* 查詢操作
        Select = 2,
        //* 新增、異動、刪除操作
        Control = 3,

    }

    public PageBase()
    {
        sbRegScript = new StringBuilder();
        this.ProgramBeginRunTime = System.Environment.TickCount; //*程序开始运行时间

    }

    /// <summary>
    /// 填充页面上显示程序运行时间的文本控件
    /// </summary>
    /// <param name="literal">显示程序运行时间的文本控件</param>
    private void ProgramRunTime()
    {        
        long ProgramEndRunTime = System.Environment.TickCount;
        programRunTime = ProgramEndRunTime - this.ProgramBeginRunTime;
        // sbRegScript.Append("var local = window.parent.location!=window.location?window.parent:window.opener?window.opener.parent:window;local.document.all.runtime.innerText='" + programRunTime.ToString() + " 毫秒';");
        //sbRegScript.Append("var local = window.parent.location!=window.location?window.parent:window.opener?window.opener.parent:window;local.document.getElementById('runtime').innerText='" + programRunTime.ToString() + " 毫秒';");
        sbRegScript.Append("window.parent.postMessage({ func: 'ProgramRunTime', data: '" + programRunTime.ToString() + " 毫秒' }, '*');");        
    }

    /// <summary>
    /// 修改紀錄：調整swal方式避免alert失效 by Ares Stanley 20220224
    /// </summary>
    /// <param name="writer"></param>
    protected override void Render(System.Web.UI.HtmlTextWriter writer)
    {
        //* 如果Session中含有主機的SessionID,則發送電文關閉之,并清空網站中存的主機SessionID
        MainFrameInfo.ClearSession();
        //* 顯示頁面執行時間
        this.ProgramRunTime();
        if (this.strAlertMsg != "")
        {
            //sbRegScript.Append("alert('" + strAlertMsg + "');");
            sbRegScript.Append(string.Format(@"setTimeout(function(){{Swal.fire('{0}');}}, 100);", strAlertMsg));
        }

        if (strIsShow != "")
        {
            sbRegScript.Append(strIsShow);
        }
        else
        {

            //sbRegScript.Append(@"ClientMsgShow('" + strClientMsg + "');" + "HostMsgShow(\"" + strHostMsg + "\");");
            sbRegScript.Append(@"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + strClientMsg + "' }, '*');" + "window.parent.postMessage({ func: 'HostMsgShow', data: '" + strHostMsg + "' }, '*');");            
            if (etMstType != eMstType.NoAlert && "" != strHostMsg.Trim())
            {
                //sbRegScript.Append(@"alert('" + strHostMsg + "');");
                sbRegScript.Append(string.Format(@"setTimeout(function(){{Swal.fire('{0}');}}, 100);", strHostMsg));
            }
            if (etClientMstType != eClientMstType.NoAlert && "" != strClientMsg.Trim())
            {
                //sbRegScript.Append(@"alert('" + strClientMsg + "');");
                sbRegScript.Append(string.Format("setTimeout(function(){{Swal.fire('{0}');}}, 100);", strClientMsg));
            }

        }
        jsBuilder.RegScript(this.Page, sbRegScript.ToString());
        base.Render(writer);
    }

    /// <summary>
    /// 验证用户是否登陆或登陆超时，重载二

    /// </summary>
    /// <param name="session"></param>
    public void ValidateLogin(object session)
    {
        if (session == null)
        {
            Response.Write("<script language='javascript'>window.location.href='/Error.aspx?errorId=1'</script>");
            Response.End();
        }
    }


    /// <summary>
    /// 頁面的Function_ID
    /// </summary>
    private String _Function_ID;


    public string M_Function_ID
    {
        get { return this._Function_ID; }
        set
        {
            //this._Function_ID = value; 
        }
    }

    /// <summary>
    /// 頁面的Function_Name
    /// </summary>
    private String _Function_Name;


    public string M_Function_Name
    {
        get { return this._Function_Name; }
        set
        {
            //this._Function_Name = value; 
        }
    }

    /// <summary>
    /// 黨頁面加載時
    /// </summary>
    /// <param name="e">事件參數</param>
    protected override void OnLoad(EventArgs e)
    {
        /*增加記錄網頁訊息的struct Add by 陳靜嫻2009-09-21 Start */
        structPageInfo sPageInfo = new structPageInfo();
        /*增加記錄網頁訊息的struct Add by 陳靜嫻2009-09-21 End */

        string strUrlError = UtilHelper.GetAppSettings("Error").ToString();
        string strMsg = "";
        #region 判斷Session是否存在及重新取Session值


        //* 判斷Session是否存在
        if (Session == null || HttpContext.Current.Session == null || this.Session["Agent"] == null)
        {
            //* Session不存在時，判斷TicketID是否存在
            if (string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "TicketID")))
            {
                //* TicketID不存在，顯示重新登入訊息，轉向重新登入畫面

                //jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsg) + "');var local = window.parent?window.parent:window;local.location.href='" + strUrlLogon + "';");
                //return;

                strMsg = "00_00000000_035";
                //*TicketID不存在，顯示重新登入訊息，轉向重新登入畫面

                Response.Redirect(strUrlError + "?MsgID=" + RedirectHelper.GetEncryptParam(strMsg));
            }
            else
            {
                //* TicketID存在時，
                //* 取TicketID
                string strTicketID = RedirectHelper.GetDecryptString(this.Page, "TicketID");
                //* 以TicketID到DB中取Session資料。


                if (!getSessionFromDB(strTicketID, ref strMsg))
                {
                    //jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsg) + "');var local = window.parent?window.parent:window;local.location.href='" + strUrlLogon + "';");
                    //return;
                    Response.Redirect(strUrlError + "?MsgID=" + RedirectHelper.GetEncryptParam(strMsg));

                }
            }
        }
        #endregion 判斷Session是否存在及重新取Session值

        #region 判斷用戶是否有使用該頁面的權限

        //* 取頁面的功能ID號(Function_ID)
        this._Function_ID = "88888888";
        string strPath = this.Server.MapPath(this.Request.Url.AbsolutePath).ToUpper();
        if (strPath.IndexOf("DEFAULT.ASPX") == -1)
        {
            PageAction pgaNow = PopedomManager.MainPopedomManager.PageSettings[strPath];
            this._Function_ID = pgaNow.FunctionID;   //* 頁面的功能ID號
                                                     /*Session中增加記錄網頁訊息的struct Add by 陳靜嫻2009-09-21 Start */
            sPageInfo.strPageCode = pgaNow.FunctionID;
            this.Session["PageInfo"] = sPageInfo;
            /*Session中增加記錄網頁訊息的struct Add by 陳靜嫻2009-09-21 End */

            strMsg = "00_00000000_021";
            string strUrlLogon = UtilHelper.GetAppSettings("LOGOUT").ToString();
                        
            if (this.Session["Agent"] == null || ((EntityAGENT_INFO)this.Session["Agent"]).dtfunction == null ||
                ((DataTable)((EntityAGENT_INFO)this.Session["Agent"]).dtfunction).Rows.Count <= 0)
            {
                //jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsg) + "');var local = window.parent?window.parent:window;local.location.href='" + strUrlLogon + "';");
                jsBuilder.RegScript(this.Page, string.Format(@"swal.fire('{0}');", MessageHelper.GetMessage(strMsg)) + "var local = window.parent?window.parent:window;local.location.href='" + strUrlLogon + "';");
                return;
            }
            else
            {
                bool blCanUseAction = false;
                //* 檢查用戶的權限列表中是否存在當前頁面的Funcion_ID;
                for (int intLoop = 0; intLoop < ((DataTable)((EntityAGENT_INFO)this.Session["Agent"]).dtfunction).Rows.Count; intLoop++)
                {
                    if (((DataTable)((EntityAGENT_INFO)this.Session["Agent"]).dtfunction).Rows[intLoop]["Function_ID"].ToString() == this._Function_ID)
                    {
                        this._Function_Name = ((DataTable)((EntityAGENT_INFO)this.Session["Agent"]).dtfunction).Rows[intLoop]["Function_Name"].ToString();
                        blCanUseAction = true;
                        break;
                    }
                }

                //* 沒有權限使用該功能ID
                if (!blCanUseAction)
                {
                    //jsBuilder.RegScript(this.Page, "alert('" + MessageHelper.GetMessage(strMsg) + "');var local = window.parent?window.parent:window;local.location.href='" + strUrlLogon + "';");
                    strMsg = "00_00000000_025";
                    Response.Redirect(strUrlError + "?MsgID=" + RedirectHelper.GetEncryptParam(strMsg));

                    return;
                }
            }
        }
        #endregion

        base.OnLoad(e);
    }

    /// <summary>
    /// 以TicketID到DB中取Session資料。

    /// </summary>
    /// <param name="strTicketID"></param>
    private bool getSessionFromDB(String strTicketID, ref string strMsg)
    {
        EntityAGENT_INFO eAgentInfo = new EntityAGENT_INFO();

        EntitySESSION_INFO eSessionInfo = new EntitySESSION_INFO();

        eSessionInfo.TICKET_ID = strTicketID;

        //* 取Session訊息
        if (!BRSESSION_INFO.Search(eSessionInfo, ref eAgentInfo, ref strMsg))
        {
            return false;
        }

        //* 重新回覆當前Session的訊息

        this.Session["Agent"] = eAgentInfo;

        //* 刪除DB中的TicketID對應的Session訊息
        if (!BRSESSION_INFO.Delete(eSessionInfo, ref strMsg))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 取得左邊多少多少位(因為.NET Substring 對於不足的取有缺陷

    /// </summary>
    /// <param name="strString">需要Left的字符串</param>
    /// <param name="iCount">需要的位數</param>
    /// <returns></returns>
    public static string StringLeft(string strString, int iCount)
    {
        string strTmp = strString.Trim().PadLeft(iCount, Convert.ToChar(" "));
        return strTmp.Substring(0, iCount).Trim();
    }
}