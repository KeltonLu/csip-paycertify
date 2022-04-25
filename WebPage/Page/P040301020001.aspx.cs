//******************************************************************
//*  作    者：余洋
//*  功能說明：修改結清證明
//*  創建日期：2009/10/29
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

public partial class Page_P040301020001 : PageBase
{
    private string strUserID = "";
    private string strCardNO = "";

    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #region Event
    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (!Page.IsPostBack)
        {
            ShowControlsText();
            this.gpList.Visible = false;
            if ((Request["UserID"] != null) && (Request["CardNo"] != null))
            {
                strUserID = RedirectHelper.GetDecryptString(this.Page, "UserID");

                strCardNO = RedirectHelper.GetDecryptString(this.Page, "CardNo");

                txtCardNO.Text = strCardNO;
                txtUserID.Text = strUserID;
                ViewState["UserID"] = strUserID;
                ViewState["CardNo"] = strCardNO;
                BindGridView(true);
            }
            else
            {
                jsBuilder.RegScript(UpdatePanel1, BaseHelper.ClientMsgShow(""));
            }
            txtUserID.Focus();
            
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
        this.txtUserID.Text = "";
        this.txtUserName.Text = "";
        this.txtCardNO.Text = "";

        this.gpList.Visible = false;

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
        string strMsgID = "";
        if (!CheckCondition(ref strMsgID))
        {
            //* 檢核不成功時，提示不符訊息


            MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
            return;
        }
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = strUserID;
        log.Account_Nbr = strCardNO;
        BRL_AP_LOG.Add(log);
        //-----------------------------------------------------
        //* 欄位輸入檢核成功，進行查詢
        this.gpList.CurrentPageIndex = 1;

        this.ViewState["UserID"] = strUserID;
        this.ViewState["CardNo"] = strCardNO;
        this.txtUserName.Text = "";

        this.BindGridView(true);
    }
    /// <summary>
    /// 點選明細時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lkbDetail_Click(object sender, EventArgs e)
    {
        LinkButton lbtnDetailed = (LinkButton)sender;
        string strSerialNo = lbtnDetailed.CommandName;

        Response.Redirect("P040301020002.aspx?CardNo=" + RedirectHelper.GetEncryptParam(ViewState["CardNo"].ToString()) + "&UserID=" + RedirectHelper.GetEncryptParam(ViewState["UserID"].ToString()) + "&SerialNo=" + RedirectHelper.GetEncryptParam(strSerialNo));
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
        Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "0");


        strUserID = this.ViewState["UserID"].ToString();

        if (strUserID != "")
        {
            //* 如果有輸入[身份證字號],則添加查詢條件UserID LIKE [身份證字號]%
            Sql.AddCondition(EntitySet_Pay_Certify.M_userID, Operator.LikeLeft, DataTypeUtils.String, strUserID);
        }

        strCardNO = this.ViewState["CardNo"].ToString();
        if (strCardNO != "")
        {
            //* 如果有輸入[卡號],則添加查詢條件CardNo LIKE [卡號]%
            Sql.AddCondition(EntitySet_Pay_Certify.M_CardNo, Operator.LikeLeft, DataTypeUtils.String, strCardNO);
        }
        return Sql.GetFilterCondition();

    }

    /// <summary>
    /// 綁定GridView數據源
    /// <param name="blShow">是否重新顯示身分證字號等欄位訊息</param>
    /// </summary>
    private void BindGridView(bool blShow)
    {

        try
        {
            EntitySet<EntitySet_Pay_Certify> esResult = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(GetCondition(), this.gpList.CurrentPageIndex, this.gpList.PageSize);

            
            this.gpList.RecordCount = esResult.TotalCount;
            this.grvSetPayCertify.DataSource = esResult;
            this.grvSetPayCertify.DataBind();
            this.gpList.Visible = true;
            //* 如果查詢結果記錄大於0筆


            if (esResult.TotalCount > 0 && blShow)
            {
                this.txtUserID.Text = esResult.GetEntity(0).userID;
                this.txtUserName.Text = esResult.GetEntity(0).userName;
            }

            //* 查詢成功
            //Client Msg註冊UpdatePanel改註冊至Page by Ares Dennis 20211220
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_03010200_002"));
        }
        catch (Exception ex)
        {
            gpList.Visible = false;
            this.grvSetPayCertify.DataSource = null;
            this.grvSetPayCertify.DataBind();
            //Client Msg註冊UpdatePanel改註冊至Page by Ares Dennis 20211220
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_03010200_001"));
            Framework.Common.Logging.Logging.Log( ex, Framework.Common.Logging.LogLayer.UI);
        }
    }
    /// <summary>
    /// 顯示窗體文字
    /// </summary>
    private void ShowControlsText()
    {
       
        //* 設置查詢結果GridView的列頭標題

        this.grvSetPayCertify.Columns[0].HeaderText = BaseHelper.GetShowText("04_03010200_002");
        this.grvSetPayCertify.Columns[1].HeaderText = BaseHelper.GetShowText("04_03010200_005");
        this.grvSetPayCertify.Columns[2].HeaderText = BaseHelper.GetShowText("04_03010200_008");


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
        //* 身分證字號欄位檢核
        strUserID = txtUserID.Text.Trim();
        if (strUserID != "")
        {
            if (!Regex.IsMatch(strUserID, BaseHelper.GetShowText("04_00000000_000")))
            {
                strMsgID = "04_03020100_003";
                return false;
            }
        }


        //* 卡號欄位格式檢核
        strCardNO = txtCardNO.Text.Trim();
        if (strCardNO != "")
        {
            if (!Regex.IsMatch(strCardNO, BaseHelper.GetShowText("04_00000000_001")))
            {
                strMsgID = "04_03020100_005";
                return false;
            }
        }

        //* 一項也沒有輸入
        if (strUserID == "" && strCardNO == "")
        {
            strMsgID = "04_03020100_006";
            return false;
        }

        return true;
    }
    #endregion
}
