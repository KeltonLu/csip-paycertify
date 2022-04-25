//******************************************************************
//*  作    者：余洋
//*  功能說明：查詢已開立結清證明
//*  創建日期：2009/10/26
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//* Ares Stanley    2022/02/14    20210058-CSIP作業服務平台現代化II    調整webconfig取參數方式
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
using Framework.Common.Logging;

public partial class Page_P040302010001 : PageBase
{
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #region Event
    public static Hashtable htMailType = new Hashtable();
    /// <summary>
    /// 畫面裝載時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = BaseHelper.GetShowText("04_03020100_000");

        if (!Page.IsPostBack)
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(""));
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.gvpbSetPayCertify.DataSource = null;
            this.gvpbSetPayCertify.DataBind();
            txtCertifyNo_From.Focus();
        }

        DataTable dtMailType = new DataTable();
        
        if (BRM_PROPERTY_CODE.GetCommonProperty("MAILMETHOD2",ref dtMailType))
        {
            htMailType.Clear();
            for (int i = 0; i <= dtMailType.Rows.Count - 1; i++)
            {
                htMailType.Add(dtMailType.Rows[i]["PROPERTY_CODE"].ToString(), dtMailType.Rows[i]["PROPERTY_NAME"].ToString());
            }
        }
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
    }

    /// <summary>
    /// 點選【清空】按鈕時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear_Click(object sender, EventArgs e)
    {
        this.txtCertifyNo_From.Text = "";
        this.txtCertifyNo_To.Text = "";
        this.txtID_No.Text = "";
        this.txtID_Name.Text = "";
        this.dtpSelfGetDate.Text = "";
        this.txtCardNo.Text = "";
        this.gpList.Visible = false;
        this.gpList.RecordCount = 0;
        this.gvpbSetPayCertify.DataSource = null;
        this.gvpbSetPayCertify.DataBind();
    }

    /// <summary>
    /// 點選【查詢】Button時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string strMsg="";
        if (!CheckCondition(ref strMsg))
        {
            //* 檢核不成功時，提示不符訊息
            MessageHelper.ShowMessage(this.Page, strMsg);
            return;
        }
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtID_No.Text;
        log.Account_Nbr = this.txtCardNo.Text.Trim();
        BRL_AP_LOG.Add(log);
        //-----------------------------------------------------
        //* 欄位輸入檢核成功，進行查詢
        this.gpList.Visible = true;
        this.gpList.CurrentPageIndex = 1;
        this.ViewState["StartSerialNo"] = this.txtCertifyNo_From.Text.Trim();
        this.ViewState["EndSerialNo"] = this.txtCertifyNo_To.Text.Trim();
        this.ViewState["UserID"] = this.txtID_No.Text.Trim();
        this.ViewState["MailDay"] = this.dtpSelfGetDate.Text.Trim().Replace("/","");
        this.ViewState["CardNo"] = this.txtCardNo.Text.Trim();
        this.BindGridView();
    }

    /// <summary>
    /// GridView行綁定
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvpbSetPayCertify_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable dtblSearchResult = (DataTable)gvpbSetPayCertify.DataSource;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //* 郵寄方式
            if (dtblSearchResult.Rows[e.Row.RowIndex]["IsMail"].ToString().Trim().ToUpper() == "N")
            {
                e.Row.Cells[5].Text = BaseHelper.GetShowText("04_03020100_019");
            }
            else if (dtblSearchResult.Rows[e.Row.RowIndex]["IsMail"].ToString().Trim().ToUpper() == "Y")
            {     
                string strMailType = dtblSearchResult.Rows[e.Row.RowIndex]["MailType"].ToString();
                if (htMailType.Contains(strMailType))
                {
                    e.Row.Cells[5].Text = htMailType[strMailType].ToString() ;
                }
                else
                {
                    e.Row.Cells[5].Text = strMailType;
                }

            }

            //* 貸款金額
            if (e.Row.Cells[2].Text.Trim() != "")
                e.Row.Cells[2].Text = Convert.ToDecimal(e.Row.Cells[2].Text.Trim()).ToString("N0");
        }
    }

    /// <summary>
    ///分頁顯示
    /// </summary>
    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }
    #endregion Event


    #region function
    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    private void BindGridView()
    {
        string strMsgID = "";
        int intTotalCount = 0;
        txtID_Name.Text = "";
        DataTable dtblSearchResult = null;
        try
        {
            if (!BRSet_Pay_Certify.SearchResult_03020100(this.ViewState["StartSerialNo"].ToString(),
                        this.ViewState["EndSerialNo"].ToString(), this.ViewState["UserID"].ToString(),
                        this.ViewState["MailDay"].ToString(), this.ViewState["CardNo"].ToString(),
                        this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotalCount, ref strMsgID, ref dtblSearchResult))
            {
                //* 查詢不成功
                this.gpList.RecordCount = 0;
                this.gvpbSetPayCertify.DataSource = null;
                this.gvpbSetPayCertify.DataBind();
                this.gpList.Visible = false;
                jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(strMsgID));
                return;
            }
            else
            {
                this.gpList.Visible = true;
                this.gpList.RecordCount = intTotalCount;
                this.gvpbSetPayCertify.DataSource = dtblSearchResult;
                this.gvpbSetPayCertify.DataBind();

                //* 如果查詢結果記錄大於0筆
                if (intTotalCount > 0 )
                {
                    this.txtID_No.Text = "";
                    this.txtID_Name.Text = "";
                    if (this.ViewState["UserID"].ToString() != "")
                    {
                        this.txtID_No.Text = dtblSearchResult.Rows[0]["UserID"].ToString();
                        this.txtID_Name.Text = dtblSearchResult.Rows[0]["UserName"].ToString();
                    }
                }
            }

            //* 查詢成功
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_03020100_008"));
        }
        catch(Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_03020100_007"));
        }
    }
    /// <summary>
    /// 顯示窗體文字
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.gvpbSetPayCertify.Columns[0].HeaderText = BaseHelper.GetShowText("04_03020100_008");
        this.gvpbSetPayCertify.Columns[1].HeaderText = BaseHelper.GetShowText("04_03020100_009");
        this.gvpbSetPayCertify.Columns[2].HeaderText = BaseHelper.GetShowText("04_03020100_010");
        this.gvpbSetPayCertify.Columns[3].HeaderText = BaseHelper.GetShowText("04_03020100_011");
        this.gvpbSetPayCertify.Columns[4].HeaderText = BaseHelper.GetShowText("04_03020100_012");
        this.gvpbSetPayCertify.Columns[5].HeaderText = BaseHelper.GetShowText("04_03020100_013");
        this.gvpbSetPayCertify.Columns[6].HeaderText = BaseHelper.GetShowText("04_03020100_014");
        this.gvpbSetPayCertify.Columns[7].HeaderText = BaseHelper.GetShowText("04_03020100_015");
        this.gvpbSetPayCertify.Columns[8].HeaderText = BaseHelper.GetShowText("04_03020100_016");
        this.gvpbSetPayCertify.Columns[9].HeaderText = BaseHelper.GetShowText("04_03020100_017");
        this.gvpbSetPayCertify.Columns[10].HeaderText = BaseHelper.GetShowText("04_03020100_018");
        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvpbSetPayCertify.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }

    /// <summary>
    /// 查詢時，檢核欄位的合法性
    /// </summary>
    /// <param name="strMsg"></param>
    /// <returns></returns>
    protected bool CheckCondition(ref string strMsgID)
    {
        bool blHadInputed = false;
        //* 證明編號起欄位檢核
        if (this.txtCertifyNo_From.Text.Trim() != "")
        {
            blHadInputed = true;
            if (!Regex.IsMatch(this.txtCertifyNo_From.Text.Trim(), BaseHelper.GetShowText("04_00000000_001")))
            {
                strMsgID = "04_03020100_001";
                return false;
            }
        }

        //* 證明編號迄欄位檢核
        if (this.txtCertifyNo_To.Text.Trim() != "")
        {
            blHadInputed=true;
            if (!Regex.IsMatch(this.txtCertifyNo_To.Text.Trim(), BaseHelper.GetShowText("04_00000000_001")))
            {
                strMsgID = "04_03020100_002";
                return false;
            }
        }

        //* 身分證字號欄位檢核
        if (this.txtID_No.Text.Trim() != "")
        {
            blHadInputed=true;
            if (!Regex.IsMatch(this.txtID_No.Text.Trim(), BaseHelper.GetShowText("04_00000000_000")))
            {
                strMsgID = "04_03020100_003";
                return false;
            }
        }

        //* 自取日期欄位格式檢核
        if (this.dtpSelfGetDate.Text.Trim()!="")
        {
            blHadInputed=true;
            DateTime dtmOut = DateTime.Parse("1900-01-01");
            if (!Function.IsDateTime(this.dtpSelfGetDate.Text.Trim(), out dtmOut))
            {
                strMsgID = "04_03020100_004";
                return false;
            }
        }

        //* 卡號欄位格式檢核
        if (this.txtCardNo.Text.Trim() != "")
        {
            blHadInputed = true;
            if (!Regex.IsMatch(this.txtCardNo.Text.Trim(), BaseHelper.GetShowText("04_00000000_001")))
            {
                strMsgID = "04_03020100_005";
                return false;
            }
        }

        //* 一項也沒有輸入
        if (!blHadInputed)
        {
            strMsgID = "04_03020100_006";
            return false;
        }

        return true;
    }
    #endregion
}
