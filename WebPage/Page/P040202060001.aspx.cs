//******************************************************************
//*  作    者：余洋
//*  功能說明：剔退記錄查詢

//*  創建日期：2009/11/11
//*  修改記錄：


//*<author>            <time>            <TaskID>                <desc>
//* Ares Stanley    2022/02/14    20210058-CSIP作業服務平台現代化II    移除多餘參數, 調整webconfig取參數方式
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
using Framework.Common.Logging;
using System.Text.RegularExpressions;


public partial class Page_P040202060001 : PageBase
{
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = BaseHelper.GetShowText("04_02020600_000");
        if (!Page.IsPostBack)
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(""));
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.gvpbPay_SV_FeedBack.DataSource = null;
            this.gvpbPay_SV_FeedBack.DataBind();
        }
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string strMsg = "";
        if (!Checkcondition(ref strMsg))
        {
            //* 檢核不成功時，提示不符訊息
            MessageHelper.ShowMessage(this.Page, strMsg);
            return;
        }
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtUserID.Text;
        BRL_AP_LOG.Add(log);
        //-----------------------------------------------------
        //* 欄位輸入檢核成功，進行查詢
        this.gpList.Visible = true;
        this.gpList.CurrentPageIndex = 1;
        this.ViewState["RebackBeforeDate"] = this.dpRebackBeforeDate.Text.Trim().Replace("/", "");
        this.ViewState["RebackEndDate"] = this.dpRebackEndDate.Text.Trim().Replace("/", "");
        this.ViewState["ApplyBeforeDate"] = this.dpApplyBeforeDate.Text.Trim().Replace("/", "");
        this.ViewState["ApplyEndDate"] = this.dpApplyEndDate.Text.Trim().Replace("/", "");
        this.ViewState["UserID"] = this.txtUserID.Text.Trim();
        this.BindGridView();
    }


    /// <summary>
    /// 分頁設置
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }

    /// <summary>
    /// 列印按鈕動作
    /// 修改紀錄：列印報表改NPOI by Ares Stanley 0220120
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        string strMsg = "";
        //*檢查輸入欄位合法性
        if (!Checkcondition(ref strMsg))
        {
            //* 檢核不成功時，提示不符訊息
            MessageHelper.ShowMessage(this.Page, strMsg);
            return;
        }

        string strRebackDateStart = this.ViewState["RebackBeforeDate"].ToString();
        string strRebackDateEnd = this.ViewState["RebackEndDate"].ToString();
        string strApplyDateStart = this.ViewState["ApplyBeforeDate"].ToString();
        string strApplyDateEnd = this.ViewState["ApplyEndDate"].ToString();
        string strUserID = this.ViewState["UserID"].ToString();

        string strMsgID = "";
        DataSet dstResult = new DataSet();
        //*生成報表
        //20220223_Ares_Jack_取得參數判斷是S：查詢或P：列印,
        bool blnReport = BRReport.Report02020600(strRebackDateStart, strRebackDateEnd, strApplyDateStart, strApplyDateEnd, strUserID, ref strMsgID, ref dstResult, "P");

        if (blnReport)
        {
            if (dstResult.Tables.Count <= 0)
            {
                //無資料
                jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_056")));
                return;
            }

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
            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
            if (!BR_Excel_File.CreateExcelFile_Report02020600(dstResult, strRebackDateStart, strRebackDateEnd, strApplyDateStart, strApplyDateEnd, strUserID, ref strServerPathFile, ref strMsgID))
            {
                jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_058")));
                return;
            }
            //* 將服務器端生成的文檔，下載到本地。
            string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
            string strFileName = "債清客戶剔退紀錄查詢" + strYYYYMMDD + ".xls";

            //* 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = strServerPathFile;
            this.Session["ClientFile"] = strFileName;
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("04_00000000_057") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            jsBuilder.RegScript(this.Page, urlString);


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
    /// <summary>
    /// 序號欄位綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbPay_SV_FeedBack_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CustLabel lblSerialNo = (CustLabel)e.Row.FindControl("lblSerialNo");
            lblSerialNo.Text = Convert.ToString((this.gpList.CurrentPageIndex - 1) * this.gpList.PageSize + this.gvpbPay_SV_FeedBack.Rows.Count + 1);
        }
    }
    #endregion event

    #region method
    /// <summary>
    /// 輸入驗證
    /// </summary>
    /// <param name="strMsg">錯誤信息ID</param>
    /// <returns></returns>
    private bool Checkcondition(ref string strMsg)
    {
        bool blHadInput = false;
        //*驗證剔退日期的輸入
        if (this.dpRebackBeforeDate.Text.Trim() != "" || this.dpRebackEndDate.Text.Trim() != "")
        {
            blHadInput = true;
            DateTime dtmBeforeOut = DateTime.Parse("1900-01-01");
            DateTime dtmEndOut = DateTime.Parse("1900-01-01");
            if (dpRebackBeforeDate.Text.Trim() != "")
            {
                if (!Function.IsDateTime(this.dpRebackBeforeDate.Text.Trim(), out dtmBeforeOut))
                {
                    strMsg = "04_02020600_001";
                    return false;
                }
            }
            if (dpRebackEndDate.Text.Trim() != "")
            {
                if (!Function.IsDateTime(this.dpRebackEndDate.Text.Trim(), out dtmEndOut))
                {
                    strMsg = "04_02020600_002";
                    return false;
                }
            }
            if (this.dpRebackBeforeDate.Text.Trim() != "" && this.dpRebackEndDate.Text.Trim() != "")
            {
                if (dtmBeforeOut > dtmEndOut)
                {
                    strMsg = "04_02020600_005";
                    return false;
                }
            }
        }
        //*驗證申請日期的輸入
        if (this.dpApplyBeforeDate.Text.Trim() != "" || this.dpApplyEndDate.Text.Trim() != "")
        {
            blHadInput = true;
            DateTime dtmBeforeOut = DateTime.Parse("1900-01-01");
            DateTime dtmEndOut = DateTime.Parse("1900-01-01");
            if (dpApplyBeforeDate.Text.Trim() != "")
            {
                if (!Function.IsDateTime(this.dpApplyBeforeDate.Text.Trim(), out dtmBeforeOut))
                {
                    strMsg = "04_02020600_003";
                    return false;
                }
            }
            if (dpApplyEndDate.Text.Trim() != "")
            {
                if (!Function.IsDateTime(this.dpApplyEndDate.Text.Trim(), out dtmEndOut))
                {
                    strMsg = "04_02020600_004";
                    return false;
                }
            }
            if (this.dpApplyBeforeDate.Text.Trim() != "" && this.dpApplyEndDate.Text.Trim() != "")
            {
                if (dtmBeforeOut > dtmEndOut)
                {
                    strMsg = "04_02020600_006";
                    return false;
                }
            }
        }
        //*驗證客戶ID的輸入
        if (this.txtUserID.Text.Trim() != "")
        {
            blHadInput = true;
            if (!Regex.IsMatch(this.txtUserID.Text.Trim(), BaseHelper.GetShowText("04_00000000_000")))
            {
                strMsg = "04_02020600_009";
                return false;
            }
        }
        //*一項也不輸入
        if (!blHadInput)
        {
            strMsg = "04_02020600_010";
            return false;
        }
        return true;
    }
    /// <summary>
    /// GridView綁定
    /// </summary>
    private void BindGridView()
    {
        string strMsgID = "";
        int intTotalCount = 0;
        DataTable dtblSearchResult = null;
        try
        {
            if (!BRPay_SV_Feedback.FeedBackQuery(this.ViewState["RebackBeforeDate"].ToString(), this.ViewState["RebackEndDate"].ToString(),
                                                 this.ViewState["ApplyBeforeDate"].ToString(), this.ViewState["ApplyEndDate"].ToString(),
                                                 this.ViewState["UserID"].ToString(), this.gpList.CurrentPageIndex, this.gpList.PageSize,
                                                 ref strMsgID, ref dtblSearchResult, ref intTotalCount))
            {
                //* 查詢不成功
                this.gpList.RecordCount = 0;
                this.gvpbPay_SV_FeedBack.DataSource = null;
                this.gvpbPay_SV_FeedBack.DataBind();
                this.gpList.Visible = false;
                jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(strMsgID));
                return;
            }
            this.gpList.Visible = true;
            this.gpList.RecordCount = intTotalCount;
            this.gvpbPay_SV_FeedBack.DataSource = dtblSearchResult;
            this.gvpbPay_SV_FeedBack.DataBind();

            //* 查詢成功
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_02020600_007"));
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_02020600_008"));
        }
    }
    /// <summary>
    /// 文字綁定顯示
    /// </summary>
    private void ShowControlsText()
    {
        this.gvpbPay_SV_FeedBack.Columns[0].HeaderText = BaseHelper.GetShowText("04_02020600_007");
        this.gvpbPay_SV_FeedBack.Columns[1].HeaderText = BaseHelper.GetShowText("04_02020600_004");
        this.gvpbPay_SV_FeedBack.Columns[2].HeaderText = BaseHelper.GetShowText("04_02020600_008");
        this.gvpbPay_SV_FeedBack.Columns[3].HeaderText = BaseHelper.GetShowText("04_02020600_003");
        this.gvpbPay_SV_FeedBack.Columns[4].HeaderText = BaseHelper.GetShowText("04_02020600_009");
        this.gvpbPay_SV_FeedBack.Columns[5].HeaderText = BaseHelper.GetShowText("04_02020600_010");
        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvpbPay_SV_FeedBack.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }
    #endregion method

}
