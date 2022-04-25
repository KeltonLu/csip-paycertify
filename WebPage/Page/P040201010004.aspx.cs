//******************************************************************
//*  作    者：rosicky(yangyu)
//*  功能說明：債清與開立


//*  創建日期：2009/11/16
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
// JaJa暫時移除
// using CrystalDecisions.CrystalReports.Engine;
using CSIPPayCertify.BusinessRules;
using Framework.Common.Utility;
using Framework.Common.Message;
using Framework.Common.JavaScript;
using CSIPCommonModel.EntityLayer;

public partial class Page_P040201010004 : PageBase
{
    // JaJa暫時移除
    // private ReportDocument rptResult = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

                //*傳入客戶ID信息
                string strUserID = RedirectHelper.GetDecryptString(this.Page, "Cust_ID");
                string strMsgID = "";

                DataSet dstResult = new DataSet();

                if (BRReport.Report01010100(strUserID, ref strMsgID, ref dstResult))
                {
                    if (dstResult.Tables.Count > 0)
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
                    jsBuilder.RegScript(this.form1, BaseHelper.ClientMsgShow(strMsgID) + BaseHelper.GetScriptForWindowErrorClose());
                    return;
                }
            


            if (dstResult != null)
            {
                string strRPTPathFile = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["ReportTemplate"] + "macrolist.rpt";
                // JaJa暫時移除
                /*rptResult.Load(@strRPTPathFile);
                rptResult.SetDataSource(dstResult);
                //*在報表中加入登陸員與日期欄位
                //string strAgintID = ((EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"]).agent_name.ToString();
                //rptResult.DataDefinition.FormulaFields["Nam"].Text = "'" + strAgintID + "'";  此欄為已刪除
                rptResult.DataDefinition.FormulaFields["Today"].Text = "'" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "'";
                rptResult.Refresh();

                this.crvReport.ReportSource = rptResult;*/
            }
        }
        catch (Exception exp)
        {
            Framework.Common.Logging.Logging.Log( exp, Framework.Common.Logging.LogLayer.UI);
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
            crvReport.Dispose();
        }*/
        Dispose();
        ClearChildState();
        System.GC.Collect(0);
    }
}




