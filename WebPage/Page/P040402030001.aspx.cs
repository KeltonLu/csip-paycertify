//******************************************************************
//*  作    者：chaoma
//*  功能說明：列印ML代償證明開立登記表//*  創建日期：2009/11/05
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//* Ares Stanley    2022/02/14    20210058-CSIP作業服務平台現代化II    調整webconfig取參數方式
//*******************************************************************
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.EntityLayer;
using CSIPPayCertify.BusinessRules;
using CSIPPayCertify.EntityLayer;
using Framework.Common.JavaScript;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.Common.Utility;
using Framework.Data.OM.Collections;
using Framework.WebControls;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Page_P040402030001 : PageBase
{
    /// Session變數集合
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #region Event
    /// <summary>
    /// 畫面裝載時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = BaseHelper.GetShowText("04_04020300_000");
        if (!Page.IsPostBack)
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(""));
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.gvpbSetPayCertify.DataSource = null;
            this.gvpbSetPayCertify.DataBind();
            txtCertifyNo_From.Focus();
            this.ViewState["MailDayHid"] = "";
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
            jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage(strMsg)));
            return;
        }
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtID_No.Text.Trim();
        log.Account_Nbr = this.txtCardNo.Text.Trim();
        BRL_AP_LOG.Add(log);
        //-----------------------------------------------------
        //* 欄位輸入檢核成功，進行查詢
        this.gpList.Visible = true;
        this.gpList.CurrentPageIndex = 1;
        this.ViewState["StartSerialNo"] = this.txtCertifyNo_From.Text.Trim();
        this.ViewState["EndSerialNo"] = this.txtCertifyNo_To.Text.Trim();
        this.ViewState["UserID"] = this.txtID_No.Text.Trim();
        this.ViewState["MailDay"] = this.dtpSelfGetDate.Text.Trim().Replace("/", ""); ;
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
        EntitySet<EntitySet_Pay_Certify> esSet_Pay_Certify = (EntitySet<EntitySet_Pay_Certify>)this.gvpbSetPayCertify.DataSource;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //* 貸款金額
            if (e.Row.Cells[6].Text.Trim() != "&nbsp;")
                e.Row.Cells[6].Text = Convert.ToDecimal(e.Row.Cells[6].Text.Trim()).ToString("N0");

            EntitySet_Pay_Certify eSet_Pay_Certify = esSet_Pay_Certify.GetEntity(e.Row.RowIndex);
            if (eSet_Pay_Certify.isMail == "N")
            {
                e.Row.Cells[8].Text = "";//* 平信
                e.Row.Cells[9].Text = "";//* 限時
                e.Row.Cells[10].Text = "";//* 掛號
                e.Row.Cells[11].Text = "";//* 限掛
                e.Row.Cells[12].Text = "V";//* 自取

            }
            else
            { 
                switch (eSet_Pay_Certify.MailType)
                {
                    case "0":
                        e.Row.Cells[8].Text = "V";
                        e.Row.Cells[9].Text = "";
                        e.Row.Cells[10].Text = "";
                        e.Row.Cells[11].Text = "";
                        e.Row.Cells[12].Text = "";
                        break;
                    case "1":
                        e.Row.Cells[8].Text = "";
                        e.Row.Cells[9].Text = "V";
                        e.Row.Cells[10].Text = "";
                        e.Row.Cells[11].Text = "";
                        e.Row.Cells[12].Text = "";
                        break;
                    case "2":
                        e.Row.Cells[8].Text = "";
                        e.Row.Cells[9].Text = "";
                        e.Row.Cells[10].Text = "V";
                        e.Row.Cells[11].Text = "";
                        e.Row.Cells[12].Text = "";
                        break;
                    case "3":
                        e.Row.Cells[8].Text = "";
                        e.Row.Cells[9].Text = "";
                        e.Row.Cells[10].Text = "";
                        e.Row.Cells[11].Text = "V";
                        e.Row.Cells[12].Text = "";
                        break;
                }
            }
            
            //* 地址
            e.Row.Cells[13].Text = eSet_Pay_Certify.zip.Trim() + eSet_Pay_Certify.add1.Trim() + 
                            eSet_Pay_Certify.add2.Trim() + eSet_Pay_Certify.add3.Trim();
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
    /// 列印簽收總表
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        string strMsg = "";
        if (!CheckCondition(ref strMsg))
        {
            //* 檢核不成功時，提示不符訊息
            jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage(strMsg)));
            return;
        }
        try
        {
            //* 設置窗體Title
            this.Title = BaseHelper.GetShowText("04_04020300_000");
            //* 證明編號(起)
            string strStartSerialNo = this.ViewState["StartSerialNo"].ToString();
            //* 證明編號(迄)
            string strEndSerialNo = this.ViewState["EndSerialNo"].ToString();
            //* 身分證字號
            string strUserID = this.ViewState["UserID"].ToString();
            //* 自取日期
            string strMailDay = this.ViewState["MailDay"].ToString();
            //* 卡號
            string strCardNo = this.ViewState["CardNo"].ToString();
            //* 郵寄日期
            string strMailDayHid = this.ViewState["MailDayHid"].ToString();


            string strMsgID = "";
            DataSet dstResult = new DataSet();
            //* 生成報表
            //20220223_Ares_Jack_取得參數判斷是S：查詢或P：列印
            bool blnReport = BRReport.Report0402030002(strStartSerialNo, strEndSerialNo,
                    strUserID, strMailDay, strCardNo, strMailDayHid, ref strMsgID, ref dstResult, "P");
            //* 生成報表成功
            if (blnReport)
            {
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
                jsBuilder.RegScript(this.Page, BaseHelper.GetScriptForWindowErrorClose());
            }

            if (dstResult != null)
            {
                string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());

                string strMAILNAME = "";

                if (!BR_Excel_File.CreateExcelFile_Report0402030001(dstResult, strMailDay, strMAILNAME, ref strServerPathFile, ref strMsgID))
                {
                    jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_058")));
                    return;
                }
                if (!BRReport_Data(strServerPathFile))
                    jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_04020300_007"));
            }
        }
        catch (Exception exp)
        {
            Framework.Common.Logging.Logging.Log(exp, Framework.Common.Logging.LogLayer.UI);
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_00000000_003") + BaseHelper.GetScriptForWindowErrorClose());
            return;
        }
    }
    
    #endregion Event

    #region function
    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    private void BindGridView()
    {
        string strMsgID = "";
        txtID_Name.Text = "";
        EntitySet<EntitySet_Pay_Certify> esSearchResult = null;
        try
        {
            if (!BRSet_Pay_Certify.SearchResult_04020300(this.ViewState["StartSerialNo"].ToString(),
                        this.ViewState["EndSerialNo"].ToString(), this.ViewState["UserID"].ToString(),
                        this.ViewState["MailDay"].ToString(), this.ViewState["CardNo"].ToString(),
                        this.gpList.CurrentPageIndex, this.gpList.PageSize, 
                        ref strMsgID,ref esSearchResult))
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
                this.gpList.RecordCount = esSearchResult.TotalCount;
                this.gvpbSetPayCertify.DataSource = esSearchResult;
                this.gvpbSetPayCertify.DataBind();

                //* 如果查詢結果記錄大於0筆
                if (esSearchResult.TotalCount > 0 )
                {
                    this.txtID_No.Text = "";
                    this.txtID_Name.Text = "";
                    if (this.ViewState["UserID"].ToString() != "")
                    {
                        this.txtID_No.Text = esSearchResult.GetEntity(0).userID;
                        this.txtID_Name.Text = esSearchResult.GetEntity(0).userName;
                    }
                    this.ViewState["MailDayHid"] = esSearchResult.GetEntity(0).mailDay;
                }
            }

            //* 查詢成功
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_04020300_008"));
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_04020300_007"));
        }
    }
    /// <summary>
    /// 顯示窗體文字
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置查詢結果GridView的列頭標題
        this.gvpbSetPayCertify.Columns[0].HeaderText = BaseHelper.GetShowText("04_04020300_009");
        this.gvpbSetPayCertify.Columns[1].HeaderText = BaseHelper.GetShowText("04_04020300_010");
        this.gvpbSetPayCertify.Columns[2].HeaderText = BaseHelper.GetShowText("04_04020300_011");
        this.gvpbSetPayCertify.Columns[3].HeaderText = BaseHelper.GetShowText("04_04020300_012");
        this.gvpbSetPayCertify.Columns[4].HeaderText = BaseHelper.GetShowText("04_04020300_013");
        this.gvpbSetPayCertify.Columns[5].HeaderText = BaseHelper.GetShowText("04_04020300_014");
        this.gvpbSetPayCertify.Columns[6].HeaderText = BaseHelper.GetShowText("04_04020300_015");
        this.gvpbSetPayCertify.Columns[7].HeaderText = BaseHelper.GetShowText("04_04020300_016");
        this.gvpbSetPayCertify.Columns[8].HeaderText = BaseHelper.GetShowText("04_04020300_017");
        this.gvpbSetPayCertify.Columns[9].HeaderText = BaseHelper.GetShowText("04_04020300_018");
        this.gvpbSetPayCertify.Columns[10].HeaderText = BaseHelper.GetShowText("04_04020300_019");
        this.gvpbSetPayCertify.Columns[11].HeaderText = BaseHelper.GetShowText("04_04020300_020");
        this.gvpbSetPayCertify.Columns[12].HeaderText = BaseHelper.GetShowText("04_04020300_021");
        this.gvpbSetPayCertify.Columns[13].HeaderText = BaseHelper.GetShowText("04_04020300_022");
        this.gvpbSetPayCertify.Columns[14].HeaderText = BaseHelper.GetShowText("04_04020300_023");
        this.gvpbSetPayCertify.Columns[15].HeaderText = BaseHelper.GetShowText("04_04020300_024");
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
                strMsgID = "04_04020300_001";
                return false;
            }
        }

        //* 證明編號迄欄位檢核
        if (this.txtCertifyNo_To.Text.Trim() != "")
        {
            blHadInputed=true;
            if (!Regex.IsMatch(this.txtCertifyNo_To.Text.Trim(), BaseHelper.GetShowText("04_00000000_001")))
            {
                strMsgID = "04_04020300_002";
                return false;
            }
        }

        //* 身分證字號欄位檢核
        if (this.txtID_No.Text.Trim() != "")
        {
            blHadInputed=true;
            if (!Regex.IsMatch(this.txtID_No.Text.Trim(), BaseHelper.GetShowText("04_00000000_000")))
            {
                strMsgID = "04_04020300_003";
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
                strMsgID = "04_04020300_004";
                return false;
            }
        }
        //* 卡號欄位格式檢核
        if (this.txtCardNo.Text.Trim() != "")
        {
            blHadInputed = true;
            if (!Regex.IsMatch(this.txtCardNo.Text.Trim(), BaseHelper.GetShowText("04_00000000_001")))
            {
                strMsgID = "04_04020300_005";
                return false;
            }
        }
        //* 一項也沒有輸入
        if (!blHadInputed)
        {
            strMsgID = "04_04020300_006";
            return false;
        }
        return true;
    }

    /// <summary>
    /// 專案代號:20210058-CSIP作業服務平台現代化II
    /// 列印ML代償證明開立登記表_報表
    /// 作    者:Ares Neal
    /// 創建時間:2022/01/03
    /// </summary>
    protected bool BRReport_Data(String strServerPathFile)
    {
        try
        {
            //* 將服務器端生成的文檔，下載到本地。
            string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
            string strFileName = "ML代償證明開立登記表" + strYYYYMMDD + ".xls";

            //* 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = strServerPathFile;
            this.Session["ClientFile"] = strFileName;
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("04_00000000_057") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            jsBuilder.RegScript(this.Page, urlString);     
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            return false;
        }
        return true;
    }
    #endregion

}
