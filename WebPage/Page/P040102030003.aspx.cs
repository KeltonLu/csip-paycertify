//******************************************************************
//*  作    者：chaoma
//*  功能說明：清償證明列印


//*  創建日期：2009/11/23
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
using System.Data.Odbc;
// JaJa暫時移除
// using CrystalDecisions.Shared;
using CSIPPayCertify.BusinessRules;
using Framework.Common.JavaScript;
using Framework.Common.Utility;
using Framework.Common.Message;
using Framework.Common.Logging;

public partial class Page_P040102030003 : PageBase
{
    // JaJa暫時移除
    // private ReportDocument rptResult;

    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = BaseHelper.GetShowText("04_01020300_000");

        try
        {
           
                string strSerialNo = RedirectHelper.GetDecryptString(this.Page, "SerialNo");
                string strMsgID = "";

                DataSet dstResult = new DataSet();

                if (BRReport.Report01020300(strSerialNo, ref strMsgID, ref dstResult))
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
                    jsBuilder.RegScript(this.form1, BaseHelper.ClientMsgShow(strMsgID) + BaseHelper.GetScriptForWindowErrorClose());
                    return;
                }
            

            if (dstResult != null)
            {
                // JaJa暫時移除
                /*rptResult = new ReportDocument();
                string strRPTPathFile = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["ReportTemplate"] + "PayCertificate.rpt";
                rptResult.Load(@strRPTPathFile);
                rptResult.SetDataSource(dstResult);
                rptResult.Refresh();

                this.crvReport.ReportSource = rptResult;*/
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.DB);
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
