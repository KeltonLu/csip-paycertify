//******************************************************************
//*  作    者：chaoma
//*  功能說明：清償證明統計表

//*  創建日期：2009/11/18
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

public partial class Page_P040102010002 : PageBase
{
    // JaJa暫時移除
    // private ReportDocument rptResult;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //* 設置窗體Title
            this.Title = BaseHelper.GetShowText("04_01020100_000");
            //* 開立日期(起)
            string strRptBeforeDate = RedirectHelper.GetDecryptString(this.Page, "BeforeDate");
            //* 開立日期(迄)
            string strRptEndDate = RedirectHelper.GetDecryptString(this.Page, "EndDate");

 
                string strMsgID = "";

                DataSet dstResult = new DataSet();

                //* 生成報表
                bool blnReport = BRReport.Report01020100(strRptBeforeDate, strRptEndDate, ref strMsgID, ref dstResult);
                //* 生成報表成功
                if (blnReport)
                {
                    if (dstResult.Tables[1].Rows.Count > 0 )
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
                // JaJa暫時移除
                /*rptResult = new ReportDocument();
                string strRPTPathFile = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["ReportTemplate"] + "dayth.rpt";
                rptResult.Load(@strRPTPathFile);
                rptResult.SetDataSource(dstResult);
                string strBeforeDate = Function.MinGuoDate7length(strRptBeforeDate);
                string strEndDate = Function.MinGuoDate7length(strRptEndDate);
                rptResult.DataDefinition.FormulaFields["mailDate"].Text = "'" + strBeforeDate.Substring(0, 3) + "/" + strBeforeDate.Substring(3, 2) + "/" + strBeforeDate.Substring(5, 2) + " ~ " + strEndDate.Substring(0, 3) + "/" + strEndDate.Substring(3, 2) + "/" + strEndDate.Substring(5, 2) + "'";
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
        }*/

        this.Dispose();
        this.ClearChildState();
        System.GC.Collect(0);

    }
}
