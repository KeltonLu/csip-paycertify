//******************************************************************
//*  作    者：chaoma
//*  功能說明：列印結清證明附件

//*  創建日期：2009/11/10
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
using Framework.Common;

public partial class Page_P040302050001 : PageBase
{
    private string strStartSerialNo = "";
    private string strEndSerialNo = "";
    private string strSearchMonth = "";

    #region Event
    /// <summary>
    /// 畫面裝載時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {

        Page.Title = BaseHelper.GetShowText("04_03020500_000");
        if (!Page.IsPostBack)
        {
            this.txtErrorList.Text = "";
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(""));
            ShowControlsText();
            this.gpList.Visible = false;

            txtCertifyNo_From.Focus();
            
        }
    }
    /// <summary>
    /// 點選【清空】按鈕時的處理


    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear_Click(object sender, EventArgs e)
    {
        this.txtErrorList.Text = "";
        this.txtCertifyNo_From.Text = "";
        this.txtCertifyNo_To.Text = "";
        this.txtSearchMonth.Text = "";

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
        this.txtErrorList.Text = "";
        string strMsg = "";
        SetViewState();
        if (!CheckCondition(ref strMsg))
        {
            //* 檢核不成功時，提示不符訊息


            MessageHelper.ShowMessage(this.Page, strMsg);
            return;
        }

        //* 欄位輸入檢核成功，進行查詢
        this.gpList.Visible = true;
        
        this.BindGridView();
    }

    /// <summary>
    /// 點選【列印】Button時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        this.txtErrorList.Text = "";
        if (grvSetPayCertify.Rows.Count>0)
        {
            string strURL = BaseHelper.HostMsgShow("") + BaseHelper.ClientMsgShow("04_00000000_001") +
                            " $(\"#iDownLoadFrame1\").attr(\"src\",\"P040302050002.aspx?StartSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["StartSerialNo"].ToString()) +
                            "&EndSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["EndSerialNo"].ToString()) +
                            "&SearchMonth=" + RedirectHelper.GetEncryptParam(this.ViewState["SearchMonth"].ToString()) +
                            "\");" +
                            " $(\"#iDownLoadFrame2\").attr(\"src\",\"P040302050003.aspx?StartSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["StartSerialNo"].ToString()) +
                            "&EndSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["EndSerialNo"].ToString()) +
                            "&SearchMonth=" + RedirectHelper.GetEncryptParam(this.ViewState["SearchMonth"].ToString()) +
                            "\");" +
                            " $(\"#iDownLoadFrame3\").attr(\"src\",\"P040302050004.aspx?StartSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["StartSerialNo"].ToString()) +
                            "&EndSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["EndSerialNo"].ToString()) +
                            "&SearchMonth=" + RedirectHelper.GetEncryptParam(this.ViewState["SearchMonth"].ToString()) +
                            "\");" +
                            " $(\"#iDownLoadFrame4\").attr(\"src\",\"P040302050005.aspx?StartSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["StartSerialNo"].ToString()) +
                            "&EndSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["EndSerialNo"].ToString()) +
                            "&SearchMonth=" + RedirectHelper.GetEncryptParam(this.ViewState["SearchMonth"].ToString()) +
                            "\");" +
                            " $(\"#iDownLoadFrame5\").attr(\"src\",\"P040302050006.aspx?StartSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["StartSerialNo"].ToString()) +
                            "&EndSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["EndSerialNo"].ToString()) +
                            "&SearchMonth=" + RedirectHelper.GetEncryptParam(this.ViewState["SearchMonth"].ToString()) +
                            "\");";

                            //"window.open('P040302050002.aspx?StartSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["StartSerialNo"].ToString()) +
                            // "&EndSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["EndSerialNo"].ToString()) +
                            // "&SearchMonth=" + RedirectHelper.GetEncryptParam(this.ViewState["SearchMonth"].ToString()) +
                            // "','_blank','height=200,width=300,toolbar=no,location=no,directories=no,menubar=no,scrollbars=no,resizable=no,status=no,top=10000,left=0');" +
                            // "window.open('P040302050003.aspx?StartSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["StartSerialNo"].ToString()) +
                            // "&EndSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["EndSerialNo"].ToString()) +
                            // "&SearchMonth=" + RedirectHelper.GetEncryptParam(this.ViewState["SearchMonth"].ToString()) +
                            // "','_blank','height=200,width=300,toolbar=no,location=no,directories=no,menubar=no,scrollbars=no,resizable=no,status=no,top=10000,left=0');" +
                            // "window.open('P040302050004.aspx?StartSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["StartSerialNo"].ToString()) +
                            // "&EndSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["EndSerialNo"].ToString()) +
                            // "&SearchMonth=" + RedirectHelper.GetEncryptParam(this.ViewState["SearchMonth"].ToString()) +
                            // "','_blank','height=200,width=300,toolbar=no,location=no,directories=no,menubar=no,scrollbars=no,resizable=no,status=no,top=10000,left=0');" +
                            // "window.open('P040302050005.aspx?StartSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["StartSerialNo"].ToString()) +
                            // "&EndSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["EndSerialNo"].ToString()) +
                            // "&SearchMonth=" + RedirectHelper.GetEncryptParam(this.ViewState["SearchMonth"].ToString()) +
                            // "','_blank','height=200,width=300,toolbar=no,location=no,directories=no,menubar=no,scrollbars=no,resizable=no,status=no,top=10000,left=0');" +
                            // "window.open('P040302050006.aspx?StartSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["StartSerialNo"].ToString()) +
                            // "&EndSerialNo=" + RedirectHelper.GetEncryptParam(this.ViewState["EndSerialNo"].ToString()) +
                            // "&SearchMonth=" + RedirectHelper.GetEncryptParam(this.ViewState["SearchMonth"].ToString()) +
                            // "','_blank','height=200,width=300,toolbar=no,location=no,directories=no,menubar=no,scrollbars=no,resizable=no,status=no,top=10000,left=0');";


            //* 顯示報表頁面
            jsBuilder.RegScript(this.Page, strURL);
        }
        else
        {
            MessageHelper.ShowMessage(this.Page, "04_03020500_003");
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
    #region function
    /// <summary>
    ///將查詢條件放入ViewState
    /// </summary>
    private void SetViewState()
    {
        strStartSerialNo = txtCertifyNo_From.Text.Trim();
        strEndSerialNo = txtCertifyNo_To.Text.Trim();

        if (strStartSerialNo == "" && strEndSerialNo != "")
        {
            strStartSerialNo = strEndSerialNo;
        }
        else if (strStartSerialNo != "" && strEndSerialNo == "")
        {
            strEndSerialNo = strStartSerialNo;
        }

        strSearchMonth = txtSearchMonth.Text.Trim();
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
        if (ViewState["StartSerialNo"].ToString() != "")
        {
            //如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
            Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, ViewState["StartSerialNo"].ToString());

            //如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
            Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, ViewState["EndSerialNo"].ToString());
        }

        return Sql.GetFilterCondition();

    }
    /// <summary>
    /// 綁定GridView數據源


    /// </summary>
    /// <param name="blShowUser">是否重新顯示身分證字號等欄位訊息</param>
    private void BindGridView()
    {

        try
        {
            EntitySet_Pay_CertifySet esSetPayCertify = new EntitySet_Pay_CertifySet();

            esSetPayCertify.FillEntitySet(GetCondition(), this.gpList.CurrentPageIndex, this.gpList.PageSize);

            this.gpList.Visible = true;
            this.gpList.RecordCount = esSetPayCertify.TotalCount;
            this.grvSetPayCertify.DataSource = esSetPayCertify;
            this.grvSetPayCertify.DataBind();

            //* 查詢成功
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_03020500_005"));
        }
        catch (Exception ex)
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_03020500_006"));
            Logging.Log(ex, LogLayer.UI);
        }
    }
    /// <summary>
    /// 顯示窗體文字
    /// </summary>
    private void ShowControlsText()
    {

        //* 設置查詢結果GridView的列頭標題
        this.grvSetPayCertify.Columns[0].HeaderText = BaseHelper.GetShowText("04_03020500_004");
        this.grvSetPayCertify.Columns[1].HeaderText = BaseHelper.GetShowText("04_03020500_005");
        this.grvSetPayCertify.Columns[2].HeaderText = BaseHelper.GetShowText("04_03020500_006");


        //* 設置一頁顯示最大筆數
        this.grvSetPayCertify.PageSize = int.Parse(Configure.PageSize);
        this.gpList.PageSize = int.Parse(Configure.PageSize);
    }

    /// <summary>
    /// 查詢時，檢核欄位的合法性
    /// </summary>
    /// <param name="strMsg"></param>
    /// <returns></returns>
    protected bool CheckCondition(ref string strMsgID)
    {

        //* 證明編號起欄位檢核
        if (strStartSerialNo != "")
        {
            if (!Regex.IsMatch(strStartSerialNo, BaseHelper.GetShowText("04_00000000_001")))
            {
                strMsgID = "04_03020500_001";
                return false;
            }
        }

        //* 證明編號迄欄位檢核
        if (strEndSerialNo != "")
        {

            if (!Regex.IsMatch(strEndSerialNo, BaseHelper.GetShowText("04_00000000_001")))
            {
                strMsgID = "04_03020500_002";
                return false;
            }
        }

        //*查詢年月欄位格式檢核
        if (this.txtSearchMonth.Text.Trim() != "")
        {

            if (!Function.IsYearMonth(this.txtSearchMonth.Text.Trim()))
            {
                strMsgID = "04_03020500_009";
                return false;
            }
        }
        else
        {
            strMsgID = "04_03020500_004";
            return false;
        }
        ViewState["StartSerialNo"] = strStartSerialNo;
        ViewState["EndSerialNo"] = strEndSerialNo;
        ViewState["SearchMonth"] = strSearchMonth;
        return true;
    }
    #endregion
}
