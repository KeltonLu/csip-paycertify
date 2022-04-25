//******************************************************************
//*  作    者：艾高
//*  功能說明：列印代償證明書
//*  創建日期：2009/10/26
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

public partial class Page_P040402040002 : PageBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //* 設置窗體Title
            this.Title = BaseHelper.GetShowText("04_04020400_000");
            //* 證明編號(起)
            string strStartSerialNo = RedirectHelper.GetDecryptString(this.Page, "StartSerialNo");
            //* 證明編號(迄)
            string strEndSerialNo = RedirectHelper.GetDecryptString(this.Page, "EndSerialNo");
            //* 身分證字號

            string strUserID = RedirectHelper.GetDecryptString(this.Page, "UserID");
            //* 自取日期
            string strMailDay = RedirectHelper.GetDecryptString(this.Page, "MailDay");
            //* 卡號
            string strCardNo = RedirectHelper.GetDecryptString(this.Page, "CardNo");

            
                string strMsgID = "";
                DataSet dstResult = new DataSet();
                //* 生成報表
                bool blnReport = BRReport.Report0402040002(strStartSerialNo, strEndSerialNo,
                        strUserID, strMailDay, strCardNo, ref strMsgID, ref dstResult);
                //* 生成報表成功
                if (blnReport)
                {
                    if (dstResult.Tables[0].Rows.Count > 0)
                    {
                    }
                    else
                    {
                        jsBuilder.RegScript(this.form1, BaseHelper.GetScriptForWindowClose());
                        return;
                    }
                }
                else
                {
                    jsBuilder.RegScript(this.form1, BaseHelper.GetScriptForWindowErrorClose());
                }
            
            if (dstResult != null)
            {
                string strRPTPathFile = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["ReportTemplate"] + "commute.rpt";
                string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
                if (!BR_Excel_File.CreateExcelFile_Report0402040002(dstResult, strMailDay, ref strServerPathFile, ref strMsgID))
                {
                    jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_058")));
                    return;
                }
                //* 將服務器端生成的文檔，下載到本地。
                string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
                string strFileName = "代償證明書" + strYYYYMMDD + ".xls";

                //* 顯示提示訊息：匯出到Excel文檔資料成功
                this.Session["ServerFile"] = strServerPathFile;
                this.Session["ClientFile"] = strFileName;
                string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("04_00000000_057") + "' }, '*');";
                urlString += @"location.href='DownLoadFile.aspx';";
                jsBuilder.RegScript(this.Page, urlString);
            }
        }
        catch (Exception exp)
        {
            Framework.Common.Logging.Logging.Log(exp, Framework.Common.Logging.LogLayer.UI);
            jsBuilder.RegScript(this.form1, BaseHelper.ClientMsgShow("04_00000000_003") + BaseHelper.GetScriptForWindowErrorClose());
            return;
        }
    }

    protected void Page_UnLoad(object sender, EventArgs e)
    {
        this.Dispose();
        this.ClearChildState();
        System.GC.Collect(0);
    }
}
