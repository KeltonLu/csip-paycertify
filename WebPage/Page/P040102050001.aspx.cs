//******************************************************************
//*  作    者：chaoma
//*  功能說明：記錄查詢("Certify")

//*  創建日期：2009/12/01
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

public partial class Page_P040102050001 : PageBase
{
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = BaseHelper.GetShowText("04_01020500_000");
        if (!Page.IsPostBack)
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(""));
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.gvpbCust.DataSource = null;
            this.gvpbCust.DataBind();
            lblCust_ID.Focus();
        }
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
    }
  
    /// <summary>
    /// 點選【查詢】Button時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnQuery_Click(object sender, EventArgs e)
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
        log.Customer_Id = this.txtCust_ID.Text;
        BRL_AP_LOG.Add(log);
        log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtUser_ID.Text;
        BRL_AP_LOG.Add(log);
        //------------------------------------------------------

        //* 欄位輸入檢核成功，進行查詢
        this.gpList.Visible = true;
        this.gpList.CurrentPageIndex = 1;
        this.ViewState["SearchCust_ID"] = this.txtCust_ID.Text.Trim();
        this.ViewState["SearchUser_ID"] = this.txtUser_ID.Text.Trim();
        this.ViewState["dpBeforeDate"] = this.dpBeforeDate.Text.Trim().Replace("/","");
        this.ViewState["dpEndDate"] = this.dpEndDate.Text.Trim().Replace("/", "");
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

    #endregion

    #region method
    /// <summary>
    /// 從Show.xml取漢字，設置畫面控件的Text
    /// </summary>
    private void ShowControlsText()
    {
        //*CustGridView gvpRole列的Title設置
        this.gvpbCust.Columns[0].HeaderText = BaseHelper.GetShowText("04_01020500_001");
        this.gvpbCust.Columns[1].HeaderText = BaseHelper.GetShowText("04_01020500_005");
        this.gvpbCust.Columns[2].HeaderText = BaseHelper.GetShowText("04_01020500_006");
        this.gvpbCust.Columns[3].HeaderText = BaseHelper.GetShowText("04_01020500_007");
        this.gvpbCust.Columns[4].HeaderText = BaseHelper.GetShowText("04_01020500_008");
        this.gvpbCust.Columns[5].HeaderText = BaseHelper.GetShowText("04_01020500_002");
        this.gvpbCust.Columns[6].HeaderText = BaseHelper.GetShowText("04_01020500_009");
        //* 設置一頁顯示最大筆數
        this.gpList.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());
        this.gvpbCust.PageSize = int.Parse(UtilHelper.GetAppSettings("PageSize").ToString());

    }
    /// <summary>
    /// 點查詢按鈕時,驗證合法性
    /// </summary>
    /// <param name="strMsg">錯誤信息ID</param>
    /// <returns></returns>
    protected bool CheckCondition(ref string strMsg)
    {
        bool blHadInput = false;
        if (this.txtCust_ID.Text.Trim() != "")
        {
            blHadInput = true;
            //*證明客戶ID的輸入合法性
            if (!Regex.IsMatch(this.txtCust_ID.Text.Trim(), BaseHelper.GetShowText("04_00000000_000")))
            {
                strMsg = "04_01020500_001";
                return false;
            }
        }

        //*證明UserID的輸入合法性
        if (this.txtUser_ID.Text.Trim() != "")
        {
            blHadInput = true;
            if (!Regex.IsMatch(this.txtUser_ID.Text.Trim(), BaseHelper.GetShowText("04_00000000_000")))
            {
                strMsg = "04_01020500_002";
                return false;
            }
        }

        if (this.dpBeforeDate.Text.Trim() != "" || this.dpEndDate.Text.Trim() != "")
        {
            blHadInput = true;
            DateTime dtmBeforeOut = DateTime.Parse("1900-01-01");
            DateTime dtmEndOut = DateTime.Parse("1900-01-01");
            //* 異動日期(起)欄位格式檢核
            if (this.dpBeforeDate.Text.Trim() != "")
            {
                if (!Function.IsDateTime(this.dpBeforeDate.Text.Trim(), out dtmBeforeOut))
                {
                    strMsg = "04_01020500_003";
                    return false;
                }
            }

            //* 異動日期(迄)欄位格式檢核
            if (this.dpEndDate.Text.Trim() != "")
            {
                if (!Function.IsDateTime(this.dpEndDate.Text.Trim(), out dtmEndOut))
                {
                    strMsg = "04_01020500_004";
                    return false;
                }
            }

            if (this.dpBeforeDate.Text.Trim() != "" && this.dpEndDate.Text.Trim() != "")
            {
                if (dtmBeforeOut > dtmEndOut)
                {
                    strMsg = "04_01020500_008";
                    return false;
                }
            }
        }
        //*一項也沒有輸入
        if (!blHadInput)
        {
            strMsg = "04_01020500_005";
            return false;
        }
        return true;
        
    }
    /// <summary>
    /// 綁定GridView
    /// </summary>
    /// <param name="blShowUser"></param>
    private void BindGridView()
    {
        string strError_ID = "";
        EntitySet<EntitySystem_log> EntitySetResult = null;
        try
        {
            if (!BRSystem_Log.SearchResult(this.ViewState["SearchCust_ID"].ToString(), this.ViewState["SearchUser_ID"].ToString(),
                this.ViewState["dpBeforeDate"].ToString(),this.ViewState["dpEndDate"].ToString(), "Certify", ref strError_ID, ref EntitySetResult, this.gpList.CurrentPageIndex, this.gpList.PageSize))
            {
                //*查詢不成功
                this.gpList.RecordCount = 0;
                this.gvpbCust.DataSource = null;
                this.gvpbCust.DataBind();
                this.gpList.Visible = false;
                jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(strError_ID));
                return;
            }
            else
            {
                this.gpList.Visible = true;
                this.gpList.RecordCount = EntitySetResult.TotalCount;
                this.gvpbCust.DataSource = EntitySetResult;
                this.gvpbCust.DataBind();                
            }

            //* 查詢成功
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_01020500_007"));
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_01020500_006"));
        }
    }

    #endregion

}
