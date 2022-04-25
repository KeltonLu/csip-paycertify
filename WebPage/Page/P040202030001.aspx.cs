//******************************************************************
//*  作    者：chaoma
//*  功能說明：債清證明列印


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
using Framework.Common.JavaScript;
using Framework.Common.Message;
using CSIPPayCertify.BusinessRules;
using Framework.WebControls;
using Framework.Common.Utility;
using System.Text.RegularExpressions;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.EntityLayer;
using Framework.Common;
using Framework.Common.Logging;

public partial class Page_P040202030001 : PageBase
{
    private string strUserID = "";

    private bool blIsKeyIn = true;

    private string strBeforeDate = "";

    private string strEndDate = "";
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #region Event
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Show();

            if (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "IsKeyIn")))
            {
                if (RedirectHelper.GetDecryptString(this.Page, "IsKeyIn") == "N")
                {
                    blIsKeyIn = false;
                    radNoAdd.Checked = true;
                }
                else
                {
                    blIsKeyIn = true;
                    radIsAdd.Checked = true;
                }
                strUserID = RedirectHelper.GetDecryptString(this.Page, "UserID");
                txtUserID.Text = strUserID;
                strBeforeDate = RedirectHelper.GetDecryptString(this.Page, "BeforeDate");
                strEndDate = RedirectHelper.GetDecryptString(this.Page, "EndDate");
                dpBeforeDate.Text = string.IsNullOrEmpty(strBeforeDate) ? "" : strBeforeDate.Substring(0, 4) + "/" + strBeforeDate.Substring(4, 2) + "/" + strBeforeDate.Substring(6, 2);
                dpEndDate.Text = string.IsNullOrEmpty(strEndDate) ? "" : strEndDate.Substring(0, 4) + "/" + strEndDate.Substring(4, 2) + "/" + strEndDate.Substring(6, 2);
                ViewState["UserID"] = strUserID;
                ViewState["BeforeDate"] = strBeforeDate;
                ViewState["EndDate"] = strEndDate;
                BindGridView();
            }
        }
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
    }


    protected void btnQuery_Click(object sender, EventArgs e)
    {
        strUserID = txtUserID.Text.Trim();
        ViewState["UserID"] = strUserID;
        ViewState["BeforeDate"] = dpBeforeDate.Text.Replace("/", "");
        ViewState["EndDate"] = dpEndDate.Text.Replace("/", "");
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtUserID.Text.Trim();
        BRL_AP_LOG.Add(log);
        //------------------------------------------------------
        if (radIsAdd.Checked)
        {
            if (strUserID == "" && dpBeforeDate.Text == "" && dpEndDate.Text == "")
            {
                MessageHelper.ShowMessage(this.Page, "04_02020300_006");
                return;
            }
        }


        if (dpBeforeDate.Text != "" && dpEndDate.Text != "")
        {
            DateTime dtmBeforeData = Convert.ToDateTime(dpBeforeDate.Text.Trim());
            DateTime dtmEndData = Convert.ToDateTime(dpEndDate.Text.Trim());
            if (!BRReport.CheckDataTime(dtmBeforeData, dtmEndData))
            {
                MessageHelper.ShowMessage(this.Page, "04_02020100_000");
                return;
            }
        }
        else if (dpBeforeDate.Text != "")
        {
            if (dpEndDate.Text == "")
                MessageHelper.ShowMessage(this.Page, "04_02020100_002");
            return;
        }
        else if (dpEndDate.Text != "")
        {
            if (dpBeforeDate.Text == "")
                MessageHelper.ShowMessage(this.Page, "04_02020100_001");
            return;
        }


        if (!string.IsNullOrEmpty(strUserID))
        {
            if (!Regex.IsMatch(this.txtUserID.Text.Trim(), BaseHelper.GetShowText("04_00000000_000")))
            {
                MessageHelper.ShowMessage(this.Page, "04_02020300_005");
                return;
            }
        }

        BindGridView();
    }


    protected void lbtnValidate_Click(object sender, EventArgs e)
    {
        LinkButton lbtnValidate = (LinkButton)sender;
        string strSerialNo = lbtnValidate.CommandName;
        //string stgUserID = lbtnValidate.userid
        string strIsKeyIn = radIsAdd.Checked ? "Y" : "N";
        Response.Redirect("P040202030002.aspx?SerialNo=" + RedirectHelper.GetEncryptParam(strSerialNo) + "&UserID=" + RedirectHelper.GetEncryptParam(ViewState["UserID"].ToString()) + "&IsKeyIn=" + RedirectHelper.GetEncryptParam(strIsKeyIn) + "&BeforeDate=" + RedirectHelper.GetEncryptParam(this.dpBeforeDate.Text.Replace("/", "")) + "&EndDate=" + RedirectHelper.GetEncryptParam(this.dpEndDate.Text.Replace("/", "")));

    }

    /// <summary>
    /// 修改紀錄：報表產出改NPOI by Ares Stanley 20220120
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAllprint_Click(object sender, EventArgs e)
    {
        try
        {
            if (radIsAdd.Checked)
            {
                if (this.txtUserID.Text == "" && dpBeforeDate.Text == "" && dpEndDate.Text == "")
                {
                    MessageHelper.ShowMessage(this.Page, "04_02020300_006");
                    return;
                }
            }

            if (dpBeforeDate.Text != "" && dpEndDate.Text != "")
            {
                DateTime dtmBeforeData = Convert.ToDateTime(dpBeforeDate.Text.Trim());
                DateTime dtmEndData = Convert.ToDateTime(dpEndDate.Text.Trim());
                if (!BRReport.CheckDataTime(dtmBeforeData, dtmEndData))
                {
                    MessageHelper.ShowMessage(this.Page, "04_02020100_000");
                    return;
                }
            }
            else if (dpBeforeDate.Text != "")
            {
                if (dpEndDate.Text == "")
                    MessageHelper.ShowMessage(this.Page, "04_02020100_002");
                return;
            }
            else if (dpEndDate.Text != "")
            {
                if (dpBeforeDate.Text == "")
                    MessageHelper.ShowMessage(this.Page, "04_02020100_001");
                return;
            }

            if (!string.IsNullOrEmpty(this.txtUserID.Text))
            {
                if (!Regex.IsMatch(this.txtUserID.Text.Trim(), BaseHelper.GetShowText("04_00000000_000")))
                {
                    MessageHelper.ShowMessage(this.Page, "04_02020300_005");
                    return;
                }
            }


            string strType = radIsAdd.Checked ? "Y" : "N";
            string strUserID = ViewState["UserID"].ToString();
            //* 開立日期(起)
            string strRptBeforeDate = this.dpBeforeDate.Text.Replace("/", "");
            //* 開立日期(迄)
            string strRptEndDate = this.dpEndDate.Text.Replace("/", "");


            bool blIsKeyIn = strType == "N" ? false : true;
            string strMsgID = "";

            DataSet dstResult = new DataSet();
            //20220223_Ares_Jack_取得參數判斷是S：查詢或P：列印
            if (BRReport.Report02020300_2(strUserID, strRptBeforeDate, strRptEndDate, blIsKeyIn, ref strMsgID, ref dstResult, "P"))
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



            if (dstResult != null)
            {
                //移除多餘參數 by Ares Stanley 20220214
                string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
                if (!BR_Excel_File.CreateExcelFile_Report02020300_2(dstResult, ref strServerPathFile))
                {
                    jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_058")));
                    return;
                }
                //* 將服務器端生成的文檔，下載到本地。
                string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
                string strFileName = "債清客戶列印債清證明書" + strYYYYMMDD + ".xls";

                //* 顯示提示訊息：匯出到Excel文檔資料成功
                this.Session["ServerFile"] = strServerPathFile;
                this.Session["ClientFile"] = strFileName;
                string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("04_00000000_057") + "' }, '*');";
                urlString += @"location.href='DownLoadFile.aspx';";
                jsBuilder.RegScript(this.Page, urlString);
            }
        }
        catch (Exception ex)
        {
            Logging.Log(ex);
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



    protected void radIsAdd_CheckedChanged(object sender, EventArgs e)
    {
        if (radIsAdd.Checked)
        {
            blIsKeyIn = true;
        }
        else
        {
            blIsKeyIn = false;
        }
    }

    protected void radNoAdd_CheckedChanged1(object sender, EventArgs e)
    {
        if (radNoAdd.Checked)
        {
            blIsKeyIn = false;
        }
        else
        {
            blIsKeyIn = true;
        }
    }
    #endregion

    #region Method


    /// <summary>
    ///綁定GridView數據源
    /// </summary>
    private void BindGridView()
    {

        string strMsgID = "";

        DataTable dtblPaySV = new DataTable();

        if (radIsAdd.Checked)
        {
            blIsKeyIn = true;
        }
        else
        {
            blIsKeyIn = false;
        }


        int intTotalCount = 0;
        //20220223_Ares_Jack_取得參數判斷是S：查詢或P：列印, 取得顯示頁名稱
        if (!BRPay_SV.SearchResult(ViewState["UserID"].ToString(), blIsKeyIn, ViewState["BeforeDate"].ToString(), ViewState["EndDate"].ToString(), this.gpList.CurrentPageIndex, this.gpList.PageSize, ref intTotalCount, ref strMsgID, ref dtblPaySV, "S", "債清客戶 列印債清證明"))
        {
            gpList.Visible = false;
            this.grvPaySV.DataSource = dtblPaySV;
            this.grvPaySV.DataBind();
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(strMsgID));
            return;
        }
        else
        {
            gpList.Visible = true;
            this.gpList.RecordCount = intTotalCount;

            this.grvPaySV.DataSource = dtblPaySV;
            this.grvPaySV.DataBind();



            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(strMsgID));
        }
    }

    /// <summary>
    /// 顯示窗體文字
    /// </summary>
    private void Show()
    {
        Page.Title = BaseHelper.GetShowText("04_02020300_000");

        grvPaySV.Columns[0].HeaderText = BaseHelper.GetShowText("04_02020300_006");

        grvPaySV.Columns[1].HeaderText = BaseHelper.GetShowText("04_02020300_002");

        grvPaySV.Columns[2].HeaderText = BaseHelper.GetShowText("04_02020300_007");

        grvPaySV.Columns[3].HeaderText = BaseHelper.GetShowText("04_02020300_008");

        grvPaySV.Columns[4].HeaderText = BaseHelper.GetShowText("04_02020300_009");

        grvPaySV.Columns[5].HeaderText = BaseHelper.GetShowText("04_02020300_010");

        grvPaySV.Columns[6].HeaderText = BaseHelper.GetShowText("04_02020300_011");

        grvPaySV.Columns[7].HeaderText = BaseHelper.GetShowText("04_02020300_012");

        grvPaySV.Columns[8].HeaderText = BaseHelper.GetShowText("04_02020300_013");

        grvPaySV.Columns[9].HeaderText = BaseHelper.GetShowText("04_02020300_014");

        grvPaySV.Columns[10].HeaderText = BaseHelper.GetShowText("04_02020300_015");

        radIsAdd.Text = BaseHelper.GetShowText("04_02020300_004");

        radNoAdd.Text = BaseHelper.GetShowText("04_02020300_005");

        this.grvPaySV.PageSize = int.Parse(Configure.PageSize);
        this.gpList.PageSize = int.Parse(Configure.PageSize);
    }

    #endregion



}
