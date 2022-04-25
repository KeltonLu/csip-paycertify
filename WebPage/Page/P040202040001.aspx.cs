//******************************************************************
//*  作    者：chaoma
//*  功能說明：債清證明查詢

//*  創建日期：2009/11/26
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
public partial class Page_P040202040001 : PageBase
{
    //*宣告變量
    private string strUserID = "";
    private string strType = "";
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = BaseHelper.GetShowText("04_02020400_000");
        if (!Page.IsPostBack)
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(""));
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.gvpbPay_SV.DataSource = null;
            this.gvpbPay_SV.DataBind();
            lblUserID.Focus();
            if (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "UserID")) && !string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "Type")))
            {
                strUserID = RedirectHelper.GetDecryptString(this.Page, "UserID");
                strType = RedirectHelper.GetDecryptString(this.Page, "Type");
                this.txtUserID.Text = strUserID;
                this.radlType.SelectedValue = strType;
                ViewState["SearchUserID"] = strUserID;
                ViewState["SelectradType"] = strType;
                BindGridView();
            }
        }
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
    }

   
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        string strMsg = "";
        strUserID = this.txtUserID.Text;
        strType = this.radlType.SelectedValue;
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
        ViewState["SearchUserID"] = strUserID;
        ViewState["SelectradType"] = strType;
        this.BindGridView();
    }

   
    /// <summary>
    /// 分頁設計
    /// </summary>
    /// <param name="src"></param>
    /// <param name="e"></param>
    protected void gpList_PageChanged(object src, Framework.WebControls.PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView();
    }


    /// <summary>
    /// 點選明細時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvPay_SV_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (e.NewEditIndex < 0)
        {
            return;
        }
        Response.Redirect("P040202040002.aspx?SerialNo=" + RedirectHelper.GetEncryptParam(this.gvpbPay_SV.Rows[e.NewEditIndex].Cells[0].Text)
                            + "&UserID=" + RedirectHelper.GetEncryptParam(ViewState["SearchUserID"].ToString())
                            + "&Type=" + RedirectHelper.GetEncryptParam(ViewState["SelectradType"].ToString()));

    }
    /// <summary>
    /// GridView綁定事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvPay_SV_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CustLinkButton lbtnDetail = (CustLinkButton)e.Row.FindControl("lbtnDetail");
            lbtnDetail.Text = BaseHelper.GetShowText("04_02020400_020");
        }
    }
    #endregion event

    #region method
    /// <summary>
    /// 綁定GrinView
    /// </summary>
    private void BindGridView()
    {
        string strMsgID = "";
        int intTotalCount = 0;
        DataTable dtblPay_SV = null;
        EntityPay_SV eyPay_SV = new EntityPay_SV();

        eyPay_SV.UserID =ViewState["SearchUserID"].ToString();
        eyPay_SV.Type = ViewState["SelectradType"].ToString();

        try
        {
            //20220223_Ares_Jack_取得參數判斷是S：查詢或P：列印, 取得顯示頁名稱
            if (!BRPay_SV.SearchResult(eyPay_SV, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotalCount, ref strMsgID, ref dtblPay_SV, "S", "債清客戶 債清證明查詢"))
            {
                //* 查詢不成功
                this.gpList.RecordCount = 0;
                this.gvpbPay_SV.DataSource = null;
                this.gvpbPay_SV.DataBind();
                this.gpList.Visible = false;
                jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(strMsgID));
                return;
            }
            this.gpList.Visible = true;
            this.gpList.RecordCount = intTotalCount;
            this.gvpbPay_SV.DataSource = dtblPay_SV;
            this.gvpbPay_SV.DataBind();

           

            //* 查詢成功
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_02020400_003"));
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_02020400_004"));
        }


    }
    /// <summary>
    /// 顯示窗體文字
    /// </summary>
    private void ShowControlsText()
    {
        BindTypeProperty();
        //*顯示GriView列的Title
        this.gvpbPay_SV.Columns[0].HeaderText = BaseHelper.GetShowText("04_02020400_012");
        this.gvpbPay_SV.Columns[1].HeaderText = BaseHelper.GetShowText("04_02020400_002");
        this.gvpbPay_SV.Columns[2].HeaderText = BaseHelper.GetShowText("04_02020400_013");
        this.gvpbPay_SV.Columns[3].HeaderText = BaseHelper.GetShowText("04_02020400_014");
        this.gvpbPay_SV.Columns[4].HeaderText = BaseHelper.GetShowText("04_02020400_042");
        this.gvpbPay_SV.Columns[5].HeaderText = BaseHelper.GetShowText("04_02020400_015");
        this.gvpbPay_SV.Columns[6].HeaderText = BaseHelper.GetShowText("04_02020400_016");
        this.gvpbPay_SV.Columns[7].HeaderText = BaseHelper.GetShowText("04_02020400_017");
        this.gvpbPay_SV.Columns[8].HeaderText = BaseHelper.GetShowText("04_02020400_018");
        this.gvpbPay_SV.Columns[9].HeaderText = BaseHelper.GetShowText("04_02020400_019");
        this.gvpbPay_SV.Columns[10].HeaderText = BaseHelper.GetShowText("04_02020400_020");

        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvpbPay_SV.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());

    }
   /// <summary>
    /// 輸入合法性驗證
    /// </summary>
    /// <param name="strMsg">錯誤信息ID</param>
    /// <returns></returns>
    private bool Checkcondition(ref string strMsg)
    {
        if (!string.IsNullOrEmpty(strUserID))
        {
            if (!Regex.IsMatch(this.txtUserID.Text.Trim(), BaseHelper.GetShowText("04_00000000_000")))
            {
                strMsg = "04_02020400_001";
                return false;
            }
        }
        if (string.IsNullOrEmpty(strUserID))
        {
            strMsg = "04_02020400_002";
            return false;
        }
        return true;
    }
    /// <summary>
    /// radiobuttonlist設置
    /// </summary>
    protected void BindTypeProperty()
    {
        DataTable dtblType = new DataTable();
        if (BRM_PROPERTY_CODE.GetCommonProperty("TYPE", ref dtblType))
        {
            //* 設置類型為全選的radiobutton
            radlType.Items.Add(new ListItem(BaseHelper.GetShowText("04_01020300_004"), "ALL"));
            //* 設置證明種類 
            for (int i = 0; i < dtblType.Rows.Count; i++)
            {
                ListItem liTypeItem = new ListItem(dtblType.Rows[i]["PROPERTY_NAME"].ToString(), dtblType.Rows[i]["PROPERTY_CODE"].ToString());
                radlType.Items.Add(liTypeItem);
            }
            radlType.Items[0].Selected = true;
        }
    }
    #endregion method
}
