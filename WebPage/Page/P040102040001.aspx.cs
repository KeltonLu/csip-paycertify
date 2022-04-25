//******************************************************************
//*  作    者：chaoma
//*  功能說明：清償證明查詢

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
public partial class Page_P040102040001 : PageBase
{
    //*宣告變量
    private string strUserID = "";
    private string strType = "";
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息

    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = BaseHelper.GetShowText("04_01020400_000");
        if (!Page.IsPostBack)
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(""));
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.gvpbSetCertify.DataSource = null;
            this.gvpbSetCertify.DataBind();
            lblUserID.Focus();
            if (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page,"UserID")) &&!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page,"Type")))
            {
                strUserID= RedirectHelper.GetDecryptString(this.Page, "UserID");
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

  
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        string strMsg="";
        strUserID = txtUserID.Text.Trim();
        strType = radlType.SelectedValue;
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = strUserID;
        BRL_AP_LOG.Add(log);
        //------------------------------------------------------

        if (!Checkcondition(ref strMsg))
        {
            //* 檢核不成功時，提示不符訊息
            MessageHelper.ShowMessage(this.Page, strMsg);
            return;
        }
        //* 欄位輸入檢核成功，進行查詢
        this.gpList.Visible = true;
        this.gpList.CurrentPageIndex = 1;
        ViewState["SearchUserID"] = strUserID;
        ViewState["SelectradType"] = strType;
        this.BindGridView();
    }
    /// <summary>
    /// 點選明細時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvSetCertify_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (e.NewEditIndex < 0)
        {
            return;
        }


        Response.Redirect("P040102040002.aspx?SerialNo=" + RedirectHelper.GetEncryptParam(this.gvpbSetCertify.Rows[e.NewEditIndex].Cells[0].Text)
                            + "&UserID=" + RedirectHelper.GetEncryptParam(ViewState["SearchUserID"].ToString())
                            + "&Type=" + RedirectHelper.GetEncryptParam(ViewState["SelectradType"].ToString()));

    }
    /// <summary>
    /// GridView綁定事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvSetCertify_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CustLinkButton lbtnDetail = (CustLinkButton)e.Row.FindControl("lbtnDetail");
            lbtnDetail.Text = BaseHelper.GetShowText("04_01020400_020");
        }
    }
  
    #endregion event

    #region method
    /// <summary>
    /// 顯示窗體文字
    /// </summary>
    private void ShowControlsText()
    {
        BindTypeProperty();
       
        //*顯示GriView列的Title
        this.gvpbSetCertify.Columns[0].HeaderText = BaseHelper.GetShowText("04_01020400_012");
        this.gvpbSetCertify.Columns[1].HeaderText = BaseHelper.GetShowText("04_01020400_002");
        this.gvpbSetCertify.Columns[2].HeaderText = BaseHelper.GetShowText("04_01020400_013");
        this.gvpbSetCertify.Columns[3].HeaderText = BaseHelper.GetShowText("04_01020400_014");
        this.gvpbSetCertify.Columns[4].HeaderText = BaseHelper.GetShowText("04_01020400_042");
        this.gvpbSetCertify.Columns[5].HeaderText = BaseHelper.GetShowText("04_01020400_015");
        this.gvpbSetCertify.Columns[6].HeaderText = BaseHelper.GetShowText("04_01020400_016");
        this.gvpbSetCertify.Columns[7].HeaderText = BaseHelper.GetShowText("04_01020400_017");
        this.gvpbSetCertify.Columns[8].HeaderText = BaseHelper.GetShowText("04_01020400_018");
        this.gvpbSetCertify.Columns[9].HeaderText = BaseHelper.GetShowText("04_01020400_019");
        this.gvpbSetCertify.Columns[10].HeaderText = BaseHelper.GetShowText("04_01020400_020");
       
        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvpbSetCertify.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        
    }
    /// <summary>
    /// 綁定GrinView
    /// </summary>
    private void BindGridView()
    {
        string strMsgID = "";
        int intTotalCount = 0;
        DataTable dtblPayCertify = null;
        EntityPay_Certify eyPay_Certify = new EntityPay_Certify();
        eyPay_Certify.userID = ViewState["SearchUserID"].ToString();
        eyPay_Certify.type = ViewState["SelectradType"] .ToString();
        try
        {
            //20220223_Ares_Jack_取得參數判斷是S：查詢或P：列印, 取得顯示頁名稱
            if (!BRPay_Certify.SearchResult(eyPay_Certify, this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotalCount, ref strMsgID, ref dtblPayCertify, "S", "清償證明 清償證明查詢"))
            {
                //* 查詢不成功
                this.gpList.RecordCount = 0;
                this.gvpbSetCertify.DataSource = null;
                this.gvpbSetCertify.DataBind();
                this.gpList.Visible = false;
                jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(strMsgID));
                return;
            }
            this.gpList.Visible = true;
            this.gpList.RecordCount = intTotalCount;
            this.gvpbSetCertify.DataSource = dtblPayCertify;
            this.gvpbSetCertify.DataBind();

            
        
            //* 查詢成功
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_01020400_003"));
            
        }
        catch(Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_01020400_004"));
        }
        

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
                strMsg = "04_01020400_001";
                return false;
            }
        }
        if (string.IsNullOrEmpty(strUserID))
        {
            strMsg = "04_01020400_002";
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
            radlType.Items.Add(new ListItem(BaseHelper.GetShowText("04_01020400_004"), "ALL"));
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
