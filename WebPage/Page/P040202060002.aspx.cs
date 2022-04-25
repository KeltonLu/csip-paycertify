//******************************************************************
//*  作    者：余洋
//*  功能說明：剔退記錄查詢報表

//*  創建日期：2009/11/11
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
// JaJa暫時移除
// using CrystalDecisions.CrystalReports.Engine;

public partial class Page_P040202060002 : PageBase
{
    //*宣告對象
    // JaJa暫時移除
    // private ReportDocument rptResult =new ReportDocument(); 

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Page.Title = BaseHelper.GetShowText("04_02020600_011");
            string strRebackDateStart = RedirectHelper.GetDecryptString(this.Page, "RebackBeforeDate");
            string strRebackDateEnd = RedirectHelper.GetDecryptString(this.Page, "RebackEndDate");
            string strApplyDateStart = RedirectHelper.GetDecryptString(this.Page, "ApplyBeforeDate");
            string strApplyDateEnd = RedirectHelper.GetDecryptString(this.Page, "ApplyEndDate");
            string strUserID = RedirectHelper.GetDecryptString(this.Page, "UserID");
          
                string strMsgID = "";
                DataSet dstResult = new DataSet();
                //*生成報表
                bool blnReport = BRReport.Report02020600(strRebackDateStart, strRebackDateEnd, strApplyDateStart, strApplyDateEnd, strUserID, ref strMsgID, ref dstResult);

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
                string strRPTPathFile = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["ReportTemplate"] + "PaySVFeedBackList.rpt";
                // JaJa暫時移除
                /*rptResult.Load(@strRPTPathFile);
                rptResult.SetDataSource(dstResult);*/





                //*報表欄位補充
                //if (strRebackDateStart == "")
                //{
                //    strRebackDateStart = "00000000";
                //}
                //if (strApplyDateStart == "")
                //{
                //    strApplyDateStart = "00000000";
                //}
                //if (strRebackDateEnd == "")
                //{
                //    strRebackDateEnd = "99999999";
                //}
                //if (strApplyDateEnd == "")
                //{
                //    strApplyDateEnd = "99999999";
                //}





                // JaJa暫時移除
                /*if (strRebackDateStart != "" || strRebackDateEnd != "")
                    rptResult.DataDefinition.FormulaFields["RebackDate"].Text = "' " + Function.InsertTimeSeparator(strRebackDateStart) + "~" + Function.InsertTimeSeparator(strRebackDateEnd) + "' ";
                if (strApplyDateStart != "" || strApplyDateEnd != "")
                rptResult.DataDefinition.FormulaFields["ApplyDate"].Text = "' " + Function.InsertTimeSeparator(strApplyDateStart) + "~" + Function.InsertTimeSeparator(strApplyDateEnd) + "' ";
                rptResult.DataDefinition.FormulaFields["UserID"].Text = "' " + strUserID + "' ";
                rptResult.DataDefinition.FormulaFields["PrintDate"].Text = "' " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "' ";
                rptResult.Refresh();
                this.crvReport.ReportSource = rptResult;*/
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
        //建立完页面时，释放报表文档资源
        // JaJa暫時移除
        /*if (rptResult != null)
        {
            rptResult.Close();
            rptResult.Dispose();
            rptResult = null;
        }*/

        this.Dispose();
        this.ClearChildState();
        System.GC.Collect(0);

    }
}
