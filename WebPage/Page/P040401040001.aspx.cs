//******************************************************************
//*  作    者：余洋
//*  功能說明：退件代償證明
//*  創建日期：2009/11/06
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

public partial class Page_P040401040001 : PageBase
{
    /// Session變數集合
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #region Event
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = BaseHelper.GetShowText("04_03020400_000");
        if (!Page.IsPostBack)
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(""));
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvSetPayCertify.DataSource = null;
            this.grvSetPayCertify.DataBind();
            txtID.Focus();
            if (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "UserID")) || !string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "CardNo")))
            {
                this.ViewState["UserID"] = RedirectHelper.GetDecryptString(this.Page, "UserID");
                this.ViewState["CardNo"] = RedirectHelper.GetDecryptString(this.Page, "CardNo");
                txtID.Text = ViewState["UserID"].ToString();
                txtcardNO.Text = ViewState["CardNo"].ToString();
                BindGridView(true);
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
        this.txtID.Text = "";
        this.txtID_Name.Text = "";
        this.txtcardNO.Text = "";

        this.gpList.Visible = false;
        this.gpList.RecordCount = 0;
        this.grvSetPayCertify.DataSource = null;
        this.grvSetPayCertify.DataBind();
        
    }
    /// <summary>
    /// 點選【查詢】Button時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string strMsg = "";
        if (!CheckCondition(ref strMsg))
        {
            //* 檢核不成功時，提示不符訊息

            MessageHelper.ShowMessage(this.Page, strMsg);
            return;
        }

        //* 欄位輸入檢核成功，進行查詢
        this.gpList.Visible = true;
        this.gpList.CurrentPageIndex = 1;
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtID.Text;
        log.Account_Nbr = this.txtcardNO.Text.Trim();
        BRL_AP_LOG.Add(log);
        //-----------------------------------------------------
        this.ViewState["UserID"] = this.txtID.Text.Trim();

        this.ViewState["CardNo"] = this.txtcardNO.Text.Trim();
        this.txtID_Name.Text = "";
        this.BindGridView(true);
    }
    /// <summary>
    /// 點選明細時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvSetPayCertify_RowEditing(object sender, GridViewEditEventArgs e)
    {
        if (e.NewEditIndex < 0)
        {
            return;
        }
        Response.Redirect("P040401040002.aspx?SerialNo=" + RedirectHelper.GetEncryptParam(this.grvSetPayCertify.Rows[e.NewEditIndex].Cells[0].Text)
                            + "&UserID=" + RedirectHelper.GetEncryptParam(ViewState["UserID"].ToString())
                            + "&CardNo=" + RedirectHelper.GetEncryptParam(ViewState["CardNo"].ToString()));

    }
    /// <summary>
    /// GridView綁定事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvSetPayCertify_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CustLinkButton lkbDetail = (CustLinkButton)e.Row.FindControl("lkbDetail");
            lkbDetail.Text = BaseHelper.GetShowText("04_03010300_008");
        }
    }
    /// <summary>
    ///分頁顯示
    /// </summary>
    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        this.gpList.CurrentPageIndex = e.NewPageIndex;
        this.BindGridView(false);
    }
    #endregion
    #region function
    /// <summary>
    ///取得SQL查詢條件
    /// </summary>
    private string GetCondition()
    {
        SqlHelper Sql = new SqlHelper();
        //* 添加查詢條件Void不等於Y
        Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
        //* 添加查詢條件Type等於0 - 結清, 1 - 代償
        Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "1");

        if (this.ViewState["UserID"].ToString() != "")
        {
            //* 如果有輸入[身份證字號],則添加查詢條件UserID LIKE [身份證字號]%
            Sql.AddCondition(EntitySet_Pay_Certify.M_userID, Operator.LikeLeft, DataTypeUtils.String, this.ViewState["UserID"].ToString());
        }

        if (this.ViewState["CardNo"].ToString() != "")
        {
            //* 如果有輸入[卡號],則添加查詢條件CardNo LIKE [卡號]%
            Sql.AddCondition(EntitySet_Pay_Certify.M_CardNo, Operator.LikeLeft, DataTypeUtils.String, this.ViewState["CardNo"].ToString());
        }
        return Sql.GetFilterCondition();

    }

    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    /// <param name="blShow">是否重新顯示身分證字號等欄位訊息</param>
    private void BindGridView(bool blShow)
    {

        try
        {
            EntitySet<EntitySet_Pay_Certify> esResult = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(GetCondition(), this.gpList.CurrentPageIndex, this.gpList.PageSize);

            this.gpList.Visible = true;
            this.gpList.RecordCount = esResult.TotalCount;
            this.grvSetPayCertify.DataSource = esResult;
            this.grvSetPayCertify.DataBind();

            //* 如果查詢結果記錄大於0筆


            if (esResult.TotalCount > 0 && blShow)
            {
                this.txtID.Text = esResult.GetEntity(0).userID;
                this.txtID_Name.Text = esResult.GetEntity(0).userName;
                this.grvSetPayCertify.Visible = true;
            }

            //* 查詢成功
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_04010400_002"));
        }
        catch (Exception ex)
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_04010400_001"));
            Framework.Common.Logging.Logging.Log( ex, Framework.Common.Logging.LogLayer.UI);
        }
    }
    /// <summary>
    /// 顯示窗體文字
    /// </summary>
    private void ShowControlsText()
    {
        
        //* 設置查詢結果GridView的列頭標題

        this.grvSetPayCertify.Columns[0].HeaderText = BaseHelper.GetShowText("04_03010300_002");
        this.grvSetPayCertify.Columns[1].HeaderText = BaseHelper.GetShowText("04_03010300_005");
        this.grvSetPayCertify.Columns[2].HeaderText = BaseHelper.GetShowText("04_03010300_008");


        //* 設置一頁顯示最大筆數

        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.grvSetPayCertify.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
    }

    /// <summary>
    /// 查詢時，檢核欄位的合法性

    /// </summary>
    /// <param name="strMsg"></param>
    /// <returns></returns>
    protected bool CheckCondition(ref string strMsgID)
    {
        bool blHadInputed = false;


        //* 身分證字號欄位檢核

        if (this.txtID.Text.Trim() != "")
        {
            blHadInputed = true;
            if (!Regex.IsMatch(this.txtID.Text.Trim(), BaseHelper.GetShowText("04_00000000_000")))
            {
                strMsgID = "04_03020100_003";
                return false;
            }
        }


        //* 卡號欄位格式檢核
        if (this.txtcardNO.Text.Trim() != "")
        {
            blHadInputed = true;
            if (!Regex.IsMatch(this.txtcardNO.Text.Trim(), BaseHelper.GetShowText("04_00000000_001")))
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
