//******************************************************************
//*  作    者：宋戈(Ge.Song)
//*  功能說明：修改清償證明

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
using System.Text.RegularExpressions;
using CSIPCommonModel.BaseItem;
using Framework.Common.Logging;
using Framework.Common.JavaScript;
using Framework.WebControls;
using Framework.Common.Message;

using CSIPPayCertify.BusinessRules;
using CSIPPayCertify.EntityLayer;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Utility;
using Framework.Common;

public partial class Page_P040101020001 : PageBase
{
    private string strUserID = "";
    private string strType = "";
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #region  event
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {
            //* 設置文字顯示
            ShowControlsText();
            //* 設置證明種類
            BindTypeProperty();
            gpList.Visible = false;
            if (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "Type")) && !string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "UserID")))
            {
                strUserID = RedirectHelper.GetDecryptString(this.Page, "UserID");

                strType = RedirectHelper.GetDecryptString(this.Page, "Type");
                radlType.SelectedValue = strType;
                txtUserID.Text = strUserID;
                ViewState["UserID"] = strUserID;
                ViewState["Type"] = strType;
                BindGridView();
            }
            else
            {
                jsBuilder.RegScript(UpdatePanel1, BaseHelper.ClientMsgShow(""));
            }

            //* 焦點預設第一個欄位
            txtUserID.Focus();
        }
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
    }

    /// <summary>
    ///分頁顯示
    /// </summary>
    protected void gpList_PageChanged(object src, PageChangedEventArgs e)
    {
        gpList.CurrentPageIndex = e.NewPageIndex;
        BindGridView();
    }

    /// <summary>
    /// 查詢點選
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string strMsgID = "";
        if (!CheckCondition(ref strMsgID))
        {
            //* 檢核不成功時，提示不符訊息
            MessageHelper.ShowMessage(this.Page, strMsgID);
            return;
        }
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtUserID.Text;
        BRL_AP_LOG.Add(log);
      
        //------------------------------------------------------
        this.gpList.CurrentPageIndex = 1;
        ViewState["UserID"] = txtUserID.Text.Trim();
        ViewState["Type"] = radlType.SelectedValue.Trim();
        BindGridView();
    }

    /// <summary>
    /// 明細點選
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void lkbDetail_Click(object sender, EventArgs e)
    {
        LinkButton lbtnDetailed = (LinkButton)sender;
        string strSerialNo = lbtnDetailed.CommandName;

        Response.Redirect("P040101020002.aspx?Type=" + RedirectHelper.GetEncryptParam(ViewState["Type"].ToString()) + "&UserID=" + RedirectHelper.GetEncryptParam(ViewState["UserID"].ToString()) + "&SerialNo=" + RedirectHelper.GetEncryptParam(strSerialNo));
    }
    #endregion

    #region  Method
    //* 加載證明種類到radiobuttonList
    protected void BindTypeProperty()
    {
        DataTable dtblTypeProperty = new DataTable();
        if (BRM_PROPERTY_CODE.GetCommonProperty("TYPE", ref dtblTypeProperty))
        {
            //* 設置類型為全選的radiobutton
            radlType.Items.Add(new ListItem(BaseHelper.GetShowText("04_01010200_037"), "ALL"));
            //* 設置證明種類 
            for (int i = 0; i < dtblTypeProperty.Rows.Count; i++)
            {
                ListItem liTypeItem = new ListItem(dtblTypeProperty.Rows[i]["PROPERTY_NAME"].ToString(), dtblTypeProperty.Rows[i]["PROPERTY_CODE"].ToString());
                radlType.Items.Add(liTypeItem);
            }
            radlType.Items[0].Selected = true;
        }
    }

    //* 設置文字顯示
    protected void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        grvPaySV.Columns[0].HeaderText = BaseHelper.GetShowText("04_01010200_005");
        grvPaySV.Columns[1].HeaderText = BaseHelper.GetShowText("04_01010200_006");
        grvPaySV.Columns[2].HeaderText = BaseHelper.GetShowText("04_01010200_007");
        grvPaySV.Columns[3].HeaderText = BaseHelper.GetShowText("04_01010200_008");
        grvPaySV.Columns[4].HeaderText = BaseHelper.GetShowText("04_01010200_009");
        grvPaySV.Columns[5].HeaderText = BaseHelper.GetShowText("04_01010200_010");
        grvPaySV.Columns[6].HeaderText = BaseHelper.GetShowText("04_01010200_011");
        grvPaySV.Columns[7].HeaderText = BaseHelper.GetShowText("04_01010200_012");
        grvPaySV.Columns[8].HeaderText = BaseHelper.GetShowText("04_01010200_013");
        grvPaySV.Columns[9].HeaderText = BaseHelper.GetShowText("04_01010200_014");
        grvPaySV.Columns[10].HeaderText = BaseHelper.GetShowText("04_01010200_015");
        grvPaySV.PageSize = int.Parse(Configure.PageSize);
        gpList.PageSize = int.Parse(Configure.PageSize);
    }

    /// <summary>
    /// 點查詢按鈕時,驗證合法性
    /// </summary>
    /// <param name="strMsg">ref string</param>
    /// <returns>boolType</returns>
    protected bool CheckCondition(ref string strMsgID)
    {
        strUserID = txtUserID.Text;
        //* 客戶ID欄位檢核
        if (!string.IsNullOrEmpty(strUserID))
        {
            if (!Regex.IsMatch(strUserID, BaseHelper.GetShowText("04_00000000_000")))
            {
                strMsgID = "04_01010200_001";
                return false;
            }
        }
        else
        {
            strMsgID = "04_01010200_000";
            return false;
        }
        return true;
    }

    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    /// <returns>void</returns>
    private void BindGridView()
    {
        string strMsgID = "";
        int iTotalCount = 0;

        DataTable dtblResult = new DataTable();
        EntityPay_Certify ePayCert = new EntityPay_Certify();


        ePayCert.userID = ViewState["UserID"].ToString();
        ePayCert.type = ViewState["Type"].ToString();
        try
        {
            gpList.Visible = false;
            //20220223_Ares_Jack_取得參數判斷是S：查詢或P：列印, 取得顯示頁名稱
            if (BRPay_Certify.SearchResult(ePayCert,
                        this.gpList.CurrentPageIndex, this.grvPaySV.PageSize, ref iTotalCount,
                        ref strMsgID, ref dtblResult, "S", "清償證明 修改清償證明"))
            {
                gpList.Visible = true;
                gpList.RecordCount = iTotalCount;
            }
            grvPaySV.DataSource = dtblResult;
            grvPaySV.DataBind();
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(strMsgID));
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_01010200_012"));
            return;
        }
    }
    #endregion
}
