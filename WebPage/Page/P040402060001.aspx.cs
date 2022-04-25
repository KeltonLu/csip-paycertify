//******************************************************************
//*  作    者：chaoma
//*  功能說明：列印大宗掛號函存根聯(代償證明)//*  創建日期：2009/11/13
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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Page_P040402060001 : PageBase
{
    #region Event
    /// <summary>
    /// 畫面裝載時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = BaseHelper.GetShowText("04_04020600_000");
        if (!Page.IsPostBack)
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(""));
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.gvpbSetPayCertify.DataSource = null;
            this.gvpbSetPayCertify.DataBind();
        }
    }

    /// <summary>
    /// 點選【清空】按鈕時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear_Click(object sender, EventArgs e)
    {
        this.dtpMailDay.Text = "";
        this.radlMailType.Items[0].Selected = true;
        this.radlMailType.Items[1].Selected = false;
        this.radlMailType.Items[2].Selected = false;
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
        string strMsg = "";
        if (!CheckCondition(ref strMsg))
        {
            //* 檢核不成功時，提示不符訊息
            //  MessageHelper.ShowMessage(this.UpdatePanel1, strMsg);

            jsBuilder.RegScript(this.Page, strMsg);
            return;
        }

        //* 郵寄日期
        this.ViewState["MailDay"] = this.dtpMailDay.Text.Replace("/", "");
        if (string.IsNullOrEmpty(ViewState["MailDay"].ToString()))
        {
            jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_04020600_006")));
           return;
        }
        //* 郵寄方式
        if (this.radlMailType.Items[0].Selected)
            this.ViewState["MailType"] = this.radlMailType.Items[0].Value.ToString();
        else if (this.radlMailType.Items[1].Selected)
            this.ViewState["MailType"] = this.radlMailType.Items[1].Value.ToString();
        else
            this.ViewState["MailType"] = this.radlMailType.Items[2].Value.ToString();
        //* 欄位輸入檢核成功，進行查詢
        this.gpList.Visible = true;
        this.gpList.CurrentPageIndex = 1;
        this.BindGridView(true);
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
            //* 項次
            e.Row.Cells[0].Text = Convert.ToString((this.gpList.CurrentPageIndex-1) * this.gpList.PageSize + e.Row.RowIndex+1);
            EntitySet_Pay_Certify eSet_Pay_Certify = esSet_Pay_Certify.GetEntity(e.Row.RowIndex);
            //* 地址
            e.Row.Cells[3].Text = eSet_Pay_Certify.zip.Trim() + eSet_Pay_Certify.add1.Trim() + 
                            eSet_Pay_Certify.add2.Trim() + eSet_Pay_Certify.add3.Trim();
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
        //* 郵寄日期
        this.ViewState["MailDay"] = this.dtpMailDay.Text.Replace("/", "");
        if (string.IsNullOrEmpty(ViewState["MailDay"].ToString()))
        {
            jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_04020600_006")));
            return;
        }
        try
        {
            //* 郵寄日期
            string strMailDay = this.ViewState["MailDay"].ToString();
            //* 郵寄方式
            string strMailType = this.ViewState["MailType"].ToString();
            //* 登入者姓名
            string strLogonUserName = ((EntityAGENT_INFO)HttpContext.Current.Session["Agent"]).agent_name;


            string strMsgID = "";
            DataSet dstResult = new DataSet();
            //* 生成報表
            //20220223_Ares_Jack_取得參數判斷是S：查詢或P：列印
            bool blnReport = BRReport.Report0402060002(strMailDay, strMailType, strLogonUserName,
                    ref strMsgID, ref dstResult, "P");
            //* 生成報表成功
            if (blnReport)
            {
                if (dstResult !=null && dstResult.Tables[0].Rows.Count > 0)
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
                if (!BR_Excel_File.CreateExcelFile_Report0402060001(dstResult, strMailDay, strLogonUserName, ref strServerPathFile, ref strMsgID))
                {
                    jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_058")));
                    return;
                }
                //* 將服務器端生成的文檔，下載到本地。
                string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
                string strFileName = "代償證明-大宗掛號函件存根聯" + strYYYYMMDD + ".xls";

                //* 顯示提示訊息：匯出到Excel文檔資料成功
                this.Session["ServerFile"] = strServerPathFile;
                this.Session["ClientFile"] = strFileName;
                string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("04_00000000_057") + "' }, '*');";
                urlString += @"location.href='DownLoadFile.aspx';";
                jsBuilder.RegScript(this.Page, urlString);
            }
        }
        catch (Exception exp)
        {
            Framework.Common.Logging.Logging.Log(exp, Framework.Common.Logging.LogLayer.UI);
            jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_003")));
            return;
        }
    }
    
    #endregion Event

    #region function
    /// <summary>
    /// 綁定GridView數據源
    /// </summary>
    /// <param name="blShowUser">是否重新顯示身分證字號等欄位訊息</param>
    private void BindGridView(bool blShowUser)
    {
        string strMsgID = "";
        EntitySet<EntitySet_Pay_Certify> esSearchResult = null;
        try
        {
            if (!BRSet_Pay_Certify.SearchResult_04020600(this.ViewState["MailDay"].ToString(),
                        this.ViewState["MailType"].ToString(),
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
            }

            //* 查詢成功
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_04020600_003"));
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_04020600_002"));
        }
    }
    /// <summary>
    /// 顯示窗體文字
    /// </summary>
    private void ShowControlsText()
    {
        //* 郵寄方式
        this.radlMailType.Items[0].Text = BaseHelper.GetShowText("04_04020600_004");
        this.radlMailType.Items[1].Text = BaseHelper.GetShowText("04_04020600_005");
        this.radlMailType.Items[2].Text = BaseHelper.GetShowText("04_04020600_006");

        //* 設置查詢結果GridView的列頭標題
        this.gvpbSetPayCertify.Columns[0].HeaderText = BaseHelper.GetShowText("04_04020600_010");
        this.gvpbSetPayCertify.Columns[1].HeaderText = BaseHelper.GetShowText("04_04020600_011");
        this.gvpbSetPayCertify.Columns[2].HeaderText = BaseHelper.GetShowText("04_04020600_012");
        this.gvpbSetPayCertify.Columns[3].HeaderText = BaseHelper.GetShowText("04_04020600_013");
        this.gvpbSetPayCertify.Columns[4].HeaderText = BaseHelper.GetShowText("04_04020600_014");
        this.gvpbSetPayCertify.Columns[5].HeaderText = BaseHelper.GetShowText("04_04020600_015");
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
        //* 郵寄日期欄位格式檢核
        if (this.dtpMailDay.Text.Trim() != "")
        {
            DateTime dtmOut = DateTime.Parse("1900-01-01");
            if (!Function.IsDateTime(this.dtpMailDay.Text.Trim(), out dtmOut))
            {
                strMsgID = "04_04020600_001";
                return false;
            }
        }

        return true;
    }
    #endregion
}
