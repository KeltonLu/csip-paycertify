//******************************************************************
//*  作    者：chaoma
//*  功能說明：大宗掛號函件存根聯

//*  創建日期：2009/12/04
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

public partial class Page_P040102020002 : PageBase
{
    //*宣告對象
    // JaJa暫時移除
    // private ReportDocument rptResult;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //*標題文字顯示
            Page.Title = BaseHelper.GetShowText("04_01020200_007");
            string strMAILDAY = RedirectHelper.GetDecryptString(this.Page, "MAILDAY");
            string strMAILTYPE = RedirectHelper.GetDecryptString(this.Page, "MAILTYPE");
            //*傳入變量判斷
            if (strMAILDAY == null || strMAILTYPE == null)
            {
                jsBuilder.RegScript(this.form1, BaseHelper.GetScriptForWindowClose());
            }
            if ("01".Equals(strMAILTYPE))
            {
                strMAILTYPE = BaseHelper.GetShowText("04_01020200_005");
            }
            else
            {
                strMAILTYPE = BaseHelper.GetShowText("04_01020200_006");
            }
          
                
                string strMsgID = "";
                //*生成報表
                DataSet dstResult = new DataSet();

                bool blnReport = BRReport.Report01020200(strMAILDAY, strMAILTYPE, ref strMsgID, ref dstResult);

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
                // JaJa暫時移除
                /*rptResult = new ReportDocument();
                string strRPTPathFile = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["ReportTemplate"] + "register.rpt";
                rptResult.Load(@strRPTPathFile);
                rptResult.SetDataSource(dstResult);
                string strAgintID = ((EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"]).agent_name.ToString();
                rptResult.DataDefinition.FormulaFields["Nam"].Text = "'" + strAgintID + "'";
                string strMailDate = "寄送日期:" + Function.InsertTimeSpan(strMAILDAY).ToString();
                rptResult.DataDefinition.FormulaFields["mailDateRegister"].Text = "'" + strMailDate + "'";
                rptResult.DataDefinition.FormulaFields["mailMethod"].Text = "'" + strMAILTYPE + "'";
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
