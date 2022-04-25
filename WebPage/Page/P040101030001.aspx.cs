//******************************************************************
//*  作    者：偉林
//*  功能說明：註銷清償證明
//*  創建日期：2009/11/09
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//* Ares Stanley    2021/12/24  20210058-CSIP作業服務平台現代化II    confirm改sweetAlert2, 調整js註冊位置
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
using Framework.Common.JavaScript;
using Framework.WebControls;
using System.Text.RegularExpressions;
using Framework.Common.Message;
using CSIPPayCertify.BusinessRules;
using CSIPPayCertify.EntityLayer;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Logging;
using Framework.Common.Utility;

public partial class Page_P040101030001 : PageBase
{
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #region Event
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {

            //* 清空末端訊息
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(""));
            //* 設置文字顯示
            ShowControlsText();
            //* 設置證明種類
            BindTypeProperty();
            //* 控件設置
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvSetPayCertify.DataSource = null;
            this.grvSetPayCertify.DataBind();

            this.txtID.Focus();
        }
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
    }

    /// <summary>
    /// 點擊查詢
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

        this.ViewState["UserID"] = this.txtID.Text.Trim();


        this.ViewState["TypeNo"] = radlType.SelectedValue.Trim();
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtID.Text;
        BRL_AP_LOG.Add(log);

        //------------------------------------------------------


        this.BindGridView();

        jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_01010300_002"));
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
            lkbDetail.Text = BaseHelper.GetShowText("04_01010300_022");
            string title = string.Empty;
            title = BaseHelper.GetShowText("04_01010300_023") + Server.HtmlDecode(e.Row.Cells[0].Text.Trim().Replace("'", "‘")) + BaseHelper.GetShowText("04_01010300_024");
            lkbDetail.Attributes.Add("onclick", string.Format("return AlertYesNo({{title: '{0}', bn: '', link: this}});", title));

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

    /// <summary>
    /// 點擊注銷時處理    /// 修改紀錄：變更row觸發事件避免欄位變成輸入框 by Ares Stanley 20211224
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvSetPayCertify_RowSelecting(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            LinkButton linkBtn = (LinkButton)e.CommandSource;
            GridViewRow row = linkBtn.NamingContainer as GridViewRow;
            Int32 idx = row.RowIndex;

            CSIPCommonModel.EntityLayer.EntityAGENT_INFO eAgentinfo = (CSIPCommonModel.EntityLayer.EntityAGENT_INFO)Session["Agent"];

            EntitySystem_log eSystemlog = new EntitySystem_log();

            eSystemlog.Log_date = DateTime.Now.ToString("yyyyMMdd");
            eSystemlog.Log_Time = DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0');
            eSystemlog.CustomerID = this.grvSetPayCertify.Rows[idx].Cells[1].Text.ToString();
            eSystemlog.Log_Action = "註銷";
            eSystemlog.Serial_No = this.grvSetPayCertify.Rows[idx].Cells[0].Text.ToString();
            eSystemlog.User_Name = eAgentinfo.agent_name;
            eSystemlog.User_ID = eAgentinfo.agent_id;
            eSystemlog.System_Type = "Certify";
            if (BRPay_Certify.MarkCancel(eSystemlog))
            {
                this.BindGridView();
                jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_01010300_004"));
                return;
            }
        }
        catch(Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_01010300_005"));
            return;
        }
    }

    #endregion


    #region Function
    /// <summary>
    /// 顯示窗體文字
    /// </summary>
    private void ShowControlsText()
    {
        

        //* 設置查詢結果GridView的列頭標題


        this.grvSetPayCertify.Columns[0].HeaderText = BaseHelper.GetShowText("04_01010300_013");
        this.grvSetPayCertify.Columns[1].HeaderText = BaseHelper.GetShowText("04_01010300_014");
        this.grvSetPayCertify.Columns[2].HeaderText = BaseHelper.GetShowText("04_01010300_015");
        this.grvSetPayCertify.Columns[3].HeaderText = BaseHelper.GetShowText("04_01010300_016");
        this.grvSetPayCertify.Columns[4].HeaderText = BaseHelper.GetShowText("04_01010300_017");
        this.grvSetPayCertify.Columns[5].HeaderText = BaseHelper.GetShowText("04_01010300_018");
        this.grvSetPayCertify.Columns[6].HeaderText = BaseHelper.GetShowText("04_01010300_003");
        this.grvSetPayCertify.Columns[7].HeaderText = BaseHelper.GetShowText("04_01010300_019");
        this.grvSetPayCertify.Columns[8].HeaderText = BaseHelper.GetShowText("04_01010300_020");
        this.grvSetPayCertify.Columns[9].HeaderText = BaseHelper.GetShowText("04_01010300_021");
        this.grvSetPayCertify.Columns[10].HeaderText = BaseHelper.GetShowText("04_01010300_022");


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

        //* 客戶ID欄位檢核
        if (this.txtID.Text.Trim() != "")
        {
            if (!Regex.IsMatch(this.txtID.Text.Trim(), BaseHelper.GetShowText("04_00000000_000")))
            {
                strMsgID = "04_01010300_001";
                return false;
            }
        }
        else
        {
            strMsgID = "04_01010300_000";
            return false;
        }

        return true;
    }

    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    private void BindGridView()
    {
        string strMsgID = "";

        int iTotalCount = 0;

        DataTable dtblResult = new DataTable();

        EntityPay_Certify ePayCertify = new EntityPay_Certify();

        ePayCertify.userID = this.ViewState["UserID"].ToString();

        ePayCertify.type = this.ViewState["TypeNo"].ToString();

        try
        {
            //20220223_Ares_Jack_取得參數判斷是S：查詢或P：列印, 取得顯示頁名稱
            if (!BRPay_Certify.SearchResult(ePayCertify,
                        this.gpList.CurrentPageIndex, this.gpList.PageSize,ref iTotalCount,
                        ref strMsgID, ref dtblResult, "S", "清償證明 註銷清償證明"))
            {
                //* 查詢不成功
                this.gpList.RecordCount = 0;
                this.grvSetPayCertify.DataSource = null;
                this.grvSetPayCertify.DataBind();
                this.gpList.Visible = false;
                jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(strMsgID));
                return;
            }
            else
            {
                this.gpList.Visible = true;
                this.gpList.RecordCount = iTotalCount;
                this.grvSetPayCertify.DataSource = dtblResult;
                this.grvSetPayCertify.DataBind();
               
            }

            
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.DB);
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_01010300_003"));
            return;
        }
    }

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
            radlType.Items[1].Selected = true;
        }
    }

    #endregion




   
}
