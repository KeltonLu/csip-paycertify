//******************************************************************
//*  作    者：chaoma
//*  功能說明：列印結清證明附件(1132產檔畫面頁面)
//*  創建日期：2009/11/10
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using CSIPPayCertify.BusinessRules;
using CSIPPayCertify.EntityLayer;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;
using Framework.Data.OM.Collections;
using Framework.Data.OM;
using Framework.WebControls;
using Framework.Common.Utility;
using Framework.Common.Message;
using Framework.Common.JavaScript;
using System.IO;
using System.Text;
using Framework.Common.Logging;
using Framework.Common;

public partial class Page_P040302050005 : System.Web.UI.Page
{
    //private string strClientMsg = "";
    private string strHostMsg = "";
    private string strAlertMsg = "";
    private EntityAGENT_INFO eAgentInfo;
    //private string strJs = "";

    //protected override void Render(System.Web.UI.HtmlTextWriter writer)
    //{
    //    string strRegScript = "";
    //    if (this.strClientMsg != "")
    //    {
    //        strRegScript = @"ClientMsgShow('" + strClientMsg + "');";
    //    }
    //    if (this.strHostMsg != "")
    //    {
    //        strRegScript = strRegScript + "HostMsgShow(\"" + strHostMsg + "\");";
    //    }
    //    if (this.strAlertMsg != "")
    //    {
    //        strRegScript = strRegScript + "alert('" + strAlertMsg + "');";
    //    }
    //    if (this.strJs != "")
    //    {
    //        strRegScript = strRegScript + strJs;
    //    }
    //    jsBuilder.RegScript(this.UpdatePanel1, strRegScript);
    //    base.Render(writer);
    //}

    protected void Page_Load(object sender, EventArgs e)
    {
        //bool blResponse = false;
        string strFileName = DateTime.Now.ToString("yyyyMMdd") + "_1132.MAC";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        try
        {
            //* 證明編號(起)
            string strStartSerialNo = RedirectHelper.GetDecryptString(this.Page, "StartSerialNo");
            //* 證明編號(迄)
            string strEndSerialNo = RedirectHelper.GetDecryptString(this.Page, "EndSerialNo");
            //* 查詢年月
            string strSearchMonth = RedirectHelper.GetDecryptString(this.Page, "SearchMonth");
            string strSeverPathFile = Configure.ExportExcelFilePath;

            if (BRReport.Report0302050005(strStartSerialNo, strEndSerialNo, strSearchMonth, ref strSeverPathFile, ref strHostMsg, ref strAlertMsg, eAgentInfo))
            {
                if (System.IO.File.Exists(strSeverPathFile))
                {
                    //blResponse = true;
                    this.Response.ContentType = "application/mac-compactpro";
                    this.Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(strFileName, Encoding.UTF8));
                    this.Response.TransmitFile(strSeverPathFile);
                }
                else
                {
                    //jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_00000000_002") + "alert('" + strFileName + MessageHelper.GetMessage("04_00000000_048") + "');setTimeout(closewindow,1000);");
                    WriteErr(strFileName);
                }
            }
            else
            {
                //strClientMsg = MessageHelper.GetMessage("04_00000000_002");
                //strJs = "setTimeout(closewindow,1000);";
                WriteErr(strFileName);
            }
        }
        catch (Exception ex)
        {
            Framework.Common.Logging.Logging.Log( ex, Framework.Common.Logging.LogLayer.UI);
            //strClientMsg = MessageHelper.GetMessage("04_00000000_002");
            //strJs = "setTimeout(closewindow,1000);";
            WriteErr(strFileName);
        }
        finally
        {
            //if (blResponse)
            //{
                Response.End();
            //}
        }
    }

    /// <summary>
    /// 顯示出錯訊息
    /// </summary>
    /// <param name="strFileName"></param>
    private void WriteErr(string strFileName)
    {
        string strJs = "<script language=\"javascript\" type=\"text/javascript\"> " +
                       " window.parent.document.getElementById(\"txtErrorList\").value += \"" + strFileName + " \"; " +
                       " window.parent.window.ClientMsgShow(window.parent.document.getElementById(\"txtErrorList\").value + '" + MessageHelper.GetMessage("04_00000000_002") + "'); " +
                       "</script>";
        this.Response.Write(strJs);
    }
}


