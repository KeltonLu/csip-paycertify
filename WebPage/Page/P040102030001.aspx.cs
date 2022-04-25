//******************************************************************
//*  作    者：chaoma
//*  功能說明：清償證明列印


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
using CSIPPayCertify.EntityLayer;
using CSIPCommonModel.BaseItem;
using Framework.WebControls;
using Framework.Common.Utility;
using Framework.Common.Message;
using Framework.Common.JavaScript;
using CSIPPayCertify.BusinessRules;
using System.Text.RegularExpressions;
using CSIPCommonModel.EntityLayer;
using Framework.Common;

public partial class Page_P040102030001 : PageBase
{
    private string strUserID = "";
    private string strBeginKeyInDay = "";
    private string strEndKeyInDay = "";
    private string strType = "";
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息

    #region Event
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Show();

            if (Request["BeginKeyInDay"] != null || 
                Request["EndKeyInDay"] != null || 
                !string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "Type")) || 
                !string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "UserID")))
            {
                strUserID = RedirectHelper.GetDecryptString(this.Page, "UserID");
                strBeginKeyInDay = RedirectHelper.GetDecryptString(this.Page, "BeginKeyInDay");
                strEndKeyInDay = RedirectHelper.GetDecryptString(this.Page, "EndKeyInDay");
                strType = RedirectHelper.GetDecryptString(this.Page, "Type");

                txtUserID.Text = strUserID;

                radlType.SelectedValue = strType;

                dpBeforeDate.Text = CSIPCommonModel.BaseItem.Function.InsertTimeSeparator(strBeginKeyInDay);
                dpEndDate.Text = CSIPCommonModel.BaseItem.Function.InsertTimeSeparator(strEndKeyInDay);

                ViewState["UserID"] = strUserID;
                ViewState["BeginKeyInDay"] = strBeginKeyInDay;
                ViewState["EndKeyInDay"] = strEndKeyInDay;
                ViewState["Type"] = strType;


                BindGridView();
            }
        }
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
    }


    protected void btnQuery_Click(object sender, EventArgs e)
    {
        strUserID = txtUserID.Text.Trim();
        strBeginKeyInDay = dpBeforeDate.Text.Replace("/", "");
        strEndKeyInDay = dpEndDate.Text.Replace("/", "");
        strType = radlType.SelectedValue;
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = strUserID;
        BRL_AP_LOG.Add(log); 
        //------------------------------------------------------

        if (strUserID == "" && strBeginKeyInDay == "" && strEndKeyInDay == "")
        {

            MessageHelper.ShowMessage(this.Page, "04_01020300_007");
            return;
        }

        if (!string.IsNullOrEmpty(strUserID))
        {
            if (!Regex.IsMatch(this.txtUserID.Text.Trim(), BaseHelper.GetShowText("04_00000000_000")))
            {
                MessageHelper.ShowMessage(this.Page, "04_01020300_005");
                return ;
            }
        }
        

        if ((strBeginKeyInDay != "" && strEndKeyInDay == "") || (strBeginKeyInDay == "" && strEndKeyInDay != ""))
        {
            MessageHelper.ShowMessage(this.Page, "04_01020300_006");
            return;
        }

        if (dpBeforeDate.Text != "" && dpEndDate.Text != "")
        {
            DateTime dtmBeforeData = Convert.ToDateTime(dpBeforeDate.Text.Trim());
            DateTime dtmEndData = Convert.ToDateTime(dpEndDate.Text.Trim());
            if (!BRReport.CheckDataTime(dtmBeforeData, dtmEndData))
            {
                MessageHelper.ShowMessage(this.Page, "04_01020100_000");
                return;
            }
        }
      
        ViewState["UserID"] = strUserID;
        ViewState["BeginKeyInDay"] = strBeginKeyInDay;
        ViewState["EndKeyInDay"] = strEndKeyInDay;
        ViewState["Type"] = strType;


        BindGridView();
    }

    protected void lbtnValidate_Click(object sender, EventArgs e)
    {
        LinkButton lbtnValidate = (LinkButton)sender;
        string strSerialNo = lbtnValidate.CommandName;

        Response.Redirect("P040102030002.aspx?SerialNo=" + RedirectHelper.GetEncryptParam(strSerialNo) + "&UserID=" + RedirectHelper.GetEncryptParam(ViewState["UserID"].ToString()) + "&BeginKeyInDay=" + RedirectHelper.GetEncryptParam(ViewState["BeginKeyInDay"].ToString()) + "&EndKeyInDay=" + RedirectHelper.GetEncryptParam(ViewState["EndKeyInDay"].ToString()) + "&Type=" + RedirectHelper.GetEncryptParam(ViewState["Type"].ToString()));
    }


    protected void btnAllprint_Click(object sender, EventArgs e)
    {
        if ((String.IsNullOrEmpty(ViewState["UserID"] + "")) && (String.IsNullOrEmpty(ViewState["BeginKeyInDay"] + "")) && (String.IsNullOrEmpty(ViewState["EndKeyInDay"] + ""))) 
        {
            MessageHelper.ShowMessage(this.Page, "04_01020300_002");            
        }
        else
        {
            string strUserID = ViewState["UserID"].ToString();
            string strType = ViewState["Type"].ToString();
            string strBeginKeyInDay = ViewState["BeginKeyInDay"].ToString();
            string strEndKeyInDay = ViewState["EndKeyInDay"].ToString();

            string strMsgID = "";

            DataSet dstResult = new DataSet();
            //20220223_Ares_Jack_取得參數判斷是S：查詢或P：列印
            if (BRReport.Report01020300_2(strUserID, strType, strBeginKeyInDay, strEndKeyInDay, ref strMsgID, ref dstResult, "P"))
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
                    //無資料
                    jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_056")));
                    return;
                }
            }
            else
            {
                //查詢失敗
                jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_055")));
                return;
            }
            string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
            if (!BR_Excel_File.CreateExcelFile_Report01020300_2(dstResult, ref strServerPathFile))
            {
                jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_058")));
                return;
            }
            //* 將服務器端生成的文檔，下載到本地。
            string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
            string strFileName = "清償證明列印清償證明" + strYYYYMMDD + ".xls";

            //* 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = strServerPathFile;
            this.Session["ClientFile"] = strFileName;
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("04_00000000_057") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            jsBuilder.RegScript(this.Page, urlString);
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


    #endregion

    #region Method
    //* 加載證明種類到radiobuttonList
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

    /// <summary>
    ///綁定GridView數據源
    /// </summary>
    private void BindGridView()
    {
        gpList.Visible = true;
        string strMsgID = "";

        DataTable dtblPayCertify = new DataTable();

        EntityPay_Certify ePayCertify = new EntityPay_Certify();

        ePayCertify.userID = ViewState["UserID"].ToString();
        ePayCertify.type = ViewState["Type"].ToString();

        int intTotalCount = 0;
        //20220223_Ares_Jack_取得參數判斷是S：查詢或P：列印, 取得顯示頁名稱
        if (!BRPay_Certify.SearchResult(ePayCertify, ViewState["BeginKeyInDay"].ToString(), ViewState["EndKeyInDay"].ToString(), this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotalCount, ref strMsgID, ref dtblPayCertify, "S", "清償證明 列印清償證明"))
        {
            this.gpList.RecordCount = 0;
            this.grvPayCertify.DataSource = null;
            this.grvPayCertify.DataBind();
            this.gpList.Visible = false;
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(strMsgID));
            return;
        }
        else
        {
            this.gpList.RecordCount = intTotalCount;

            this.grvPayCertify.DataSource = dtblPayCertify;
            this.grvPayCertify.DataBind();

            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(strMsgID));
        }
    }

    /// <summary>
    /// 顯示窗體文字
    /// </summary>
    private void Show()
    {
        BindTypeProperty();

        Page.Title = BaseHelper.GetShowText("04_01020300_000");

        grvPayCertify.Columns[0].HeaderText = BaseHelper.GetShowText("04_01020300_006");

        grvPayCertify.Columns[1].HeaderText = BaseHelper.GetShowText("04_01020300_002");

        grvPayCertify.Columns[2].HeaderText = BaseHelper.GetShowText("04_01020300_007");

        grvPayCertify.Columns[3].HeaderText = BaseHelper.GetShowText("04_01020300_008");

        grvPayCertify.Columns[4].HeaderText = BaseHelper.GetShowText("04_01020300_009");

        grvPayCertify.Columns[5].HeaderText = BaseHelper.GetShowText("04_01020300_010");

        grvPayCertify.Columns[6].HeaderText = BaseHelper.GetShowText("04_01020300_011");

        grvPayCertify.Columns[7].HeaderText = BaseHelper.GetShowText("04_01020300_012");

        grvPayCertify.Columns[8].HeaderText = BaseHelper.GetShowText("04_01020300_013");

        grvPayCertify.Columns[9].HeaderText = BaseHelper.GetShowText("04_01020300_014");

        grvPayCertify.Columns[10].HeaderText = BaseHelper.GetShowText("04_01020300_015");

        this.grvPayCertify.PageSize = int.Parse(Configure.PageSize);
        this.gpList.PageSize = int.Parse(Configure.PageSize);
    }

    #endregion


}
