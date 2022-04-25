//******************************************************************
//*  作    者：艾高
//*  功能說明：列印結清證明書
//*  創建日期：2009/10/26
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

public partial class Page_P040302040001 : PageBase
{
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

        Page.Title = BaseHelper.GetShowText("04_03020400_000");
        if (!Page.IsPostBack)
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(""));
            ShowControlsText();
            this.gpList.Visible = false;
            this.gpList.RecordCount = 0;
            this.grvSetPayCertify.DataSource = null;
            this.grvSetPayCertify.DataBind();
            txtCertifyNo_From.Focus();
            
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
        SetViewState();
        txtID_Name.Text = "";
        this.BindGridView(true);
    }
    /// <summary>
    /// 修改紀錄：報表產出改NPOI by Ares Jack 20220207
    ///  /// </summary>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        try
        {
            string strMsg = "";
            if (!CheckCondition(ref strMsg))
            {
                //* 檢核不成功時，提示不符訊息


                MessageHelper.ShowMessage(this.Page, strMsg);
                return;
            }

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

            string strMsgID = "";
            DataSet dstResult = new DataSet();
            //* 生成報表成功
            //20220223_Ares_Jack_取得參數判斷是S：查詢或P：列印
            if (BRReport.Report0302040002(strStartSerialNo, strEndSerialNo, strUserID, strMailDay, strCardNo, ref strMsgID, ref dstResult, "P"))
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
                string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
                if (!BR_Excel_File.CreateExcelFile_Report0302040002(dstResult, ref strServerPathFile, ref strMsgID))
                {
                    jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_058")));
                    return;
                }
                //* 將服務器端生成的文檔，下載到本地。
                string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
                string strFileName = "結清證明書" + strYYYYMMDD + ".xls";

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
            MessageHelper.ShowMessage(this.Page, "04_00000000_058");
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
    ///將查詢條件放入ViewState
    /// </summary>
    private void SetViewState()
    {
        if (this.txtCertifyNo_From.Text.Trim() != "" || this.txtCertifyNo_To.Text.Trim() != "")
        {
            if (this.txtCertifyNo_From.Text.Trim() == "")
            {
                this.ViewState["StartSerialNo"] = this.txtCertifyNo_To.Text.Trim();
            }
            else
            {
                this.ViewState["StartSerialNo"] = this.txtCertifyNo_From.Text.Trim();
            }
            if (this.txtCertifyNo_To.Text.Trim() == "")
            {
                this.ViewState["EndSerialNo"] = this.txtCertifyNo_From.Text.Trim();
            }
            else
            {
                this.ViewState["EndSerialNo"] = this.txtCertifyNo_To.Text.Trim();
            }
        }
        else
        {
            this.ViewState["StartSerialNo"] = this.txtCertifyNo_From.Text.Trim();
            this.ViewState["EndSerialNo"] = this.txtCertifyNo_To.Text.Trim();
        }
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = this.txtID_No.Text;
        log.Account_Nbr = this.txtCardNo.Text.Trim();
        BRL_AP_LOG.Add(log);
        //-----------------------------------------------------
        this.ViewState["UserID"] = this.txtID_No.Text.Trim();
        this.ViewState["MailDay"] = this.dtpSelfGetDate.Text.Trim().Replace("/", "");
        this.ViewState["CardNo"] = this.txtCardNo.Text.Trim();
    }
    /// <summary>
    ///取得SQL查詢條件
    /// </summary>
    private string GetCondition()
    {
        SqlHelper Sql = new SqlHelper();
        //添加查詢條件Void不等於Y
        Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
        //添加查詢條件Type等於0 - 結清, 1 - 代償
        Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "0");
        if (this.ViewState["StartSerialNo"].ToString() != "")
        {
            //如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
            Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, this.ViewState["StartSerialNo"].ToString());
        }
        if (this.ViewState["EndSerialNo"].ToString() != "")
        {
            //如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
            Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, this.ViewState["EndSerialNo"].ToString());
        }
        if (this.ViewState["UserID"].ToString() != "")
        {
            //如果有輸入[身份證字號],則添加查詢條件UserID LIKE [身份證字號]%
            Sql.AddCondition(EntitySet_Pay_Certify.M_userID, Operator.LikeLeft, DataTypeUtils.String, this.ViewState["UserID"].ToString());
        }
        if (this.ViewState["MailDay"].ToString() != "")
        {
            //如果有輸入[自取日期],則添加查詢條件MailDay = [自取日期]
            Sql.AddCondition(EntitySet_Pay_Certify.M_mailDay, Operator.Equal, DataTypeUtils.String, this.ViewState["MailDay"].ToString());
        }
        if (this.ViewState["CardNo"].ToString() != "")
        {
            //如果有輸入[卡號],則添加查詢條件CardNo LIKE [卡號]%
            Sql.AddCondition(EntitySet_Pay_Certify.M_CardNo, Operator.LikeLeft, DataTypeUtils.String, this.ViewState["CardNo"].ToString());
        }
        Sql.AddOrderCondition(EntitySet_Pay_Certify.M_serialNo, ESortType.ASC);
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
                if (this.ViewState["UserID"].ToString() != "")
                {
                    this.txtID_No.Text = esResult.GetEntity(0).userID;
                    this.txtID_Name.Text = esResult.GetEntity(0).userName;
                }
                this.grvSetPayCertify.Visible = true;
            }

            //* 查詢成功
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_03020400_008"));
        }
        catch (Exception ex)
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_03020400_007"));
            Framework.Common.Logging.Logging.Log(ex, Framework.Common.Logging.LogLayer.UI);
        }
    }
    /// <summary>
    /// 顯示窗體文字
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置控件標題
        this.btnSearch.Text = BaseHelper.GetShowText("04_04020400_006");
        this.btnClear.Text = BaseHelper.GetShowText("04_04020400_007");
        this.btnPrint.Text = BaseHelper.GetShowText("04_04020400_008");
        //* 設置查詢結果GridView的列頭標題

        this.grvSetPayCertify.Columns[0].HeaderText = BaseHelper.GetShowText("04_04020400_002");
        this.grvSetPayCertify.Columns[1].HeaderText = BaseHelper.GetShowText("04_04020400_003");
        this.grvSetPayCertify.Columns[2].HeaderText = BaseHelper.GetShowText("04_04020400_009");
        this.grvSetPayCertify.Columns[3].HeaderText = BaseHelper.GetShowText("04_04020400_010");
        this.grvSetPayCertify.Columns[4].HeaderText = BaseHelper.GetShowText("04_04020400_011");
        this.grvSetPayCertify.Columns[5].HeaderText = BaseHelper.GetShowText("04_04020400_012");
        this.grvSetPayCertify.Columns[6].HeaderText = BaseHelper.GetShowText("04_04020400_013");
        this.grvSetPayCertify.Columns[7].HeaderText = BaseHelper.GetShowText("04_04020400_014");
        this.grvSetPayCertify.Columns[8].HeaderText = BaseHelper.GetShowText("04_04020400_015");
        this.grvSetPayCertify.Columns[9].HeaderText = BaseHelper.GetShowText("04_04020400_016");
        this.grvSetPayCertify.Columns[10].HeaderText = BaseHelper.GetShowText("04_04020400_017");
        this.grvSetPayCertify.Columns[11].HeaderText = BaseHelper.GetShowText("04_04020400_018");

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
        //* 證明編號起欄位檢核

        if (this.txtCertifyNo_From.Text.Trim() != "")
        {
            blHadInputed = true;
            if (!Regex.IsMatch(this.txtCertifyNo_From.Text.Trim(), BaseHelper.GetShowText("04_00000000_001")))
            {
                strMsgID = "04_03020100_001";
                return false;
            }
        }

        //* 證明編號迄欄位檢核

        if (this.txtCertifyNo_To.Text.Trim() != "")
        {
            blHadInputed = true;
            if (!Regex.IsMatch(this.txtCertifyNo_To.Text.Trim(), BaseHelper.GetShowText("04_00000000_001")))
            {
                strMsgID = "04_03020100_002";
                return false;
            }
        }

        //* 身分證字號欄位檢核

        if (this.txtID_No.Text.Trim() != "")
        {
            blHadInputed = true;
            if (!Regex.IsMatch(this.txtID_No.Text.Trim(), BaseHelper.GetShowText("04_00000000_000")))
            {
                strMsgID = "04_03020100_003";
                return false;
            }
        }

        //* 自取日期欄位格式檢核
        if (this.dtpSelfGetDate.Text.Trim() != "")
        {
            blHadInputed = true;
            DateTime dtmOut = DateTime.Parse("1900-01-01");
            if (!Function.IsDateTime(this.dtpSelfGetDate.Text.Trim(), out dtmOut))
            {
                strMsgID = "04_03020100_004";
                return false;
            }
        }

        //* 卡號欄位格式檢核
        if (this.txtCardNo.Text.Trim() != "")
        {
            blHadInputed = true;
            if (!Regex.IsMatch(this.txtCardNo.Text.Trim(), BaseHelper.GetShowText("04_00000000_001")))
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
