//******************************************************************
//*  作    者：rosicky(yangyu)
//*  功能說明：債清與開立


//*  創建日期：2009/11/16
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
using CSIPPayCertify.BusinessRules;
using CSIPPayCertify.EntityLayer;
using Framework.Data.OM.Collections;
using Framework.Common.Message;
using Framework.Data.OM;
using Framework.Common.Utility;
using System.Text.RegularExpressions;
using System.Text;
using Framework.Common.JavaScript;

public partial class Page_P040201010003 : PageBase
{
    #region 宣告變數
    private double dblBalance;
    private double dblDebtBalance;
    private double dblTtlPayment;
    #endregion

    #region Event
    /// <summary>
    /// 載入畫面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "UserID")))
            {
                jsBuilder.RegScript(this.Page,BaseHelper.GetScriptForWindowErrorClose());
                return;
            }

            //* 設置文字顯示
            Show();

            ViewState["UserID"] = RedirectHelper.GetDecryptString(this.Page, "UserID").Trim();


            //* 綁定資料列
            BindGridView();
        }

    }

    /// <summary>
    /// 綁定Gird時增加統計
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grvSetPayCertify_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            dblBalance = dblBalance + Convert.ToDouble(e.Row.Cells[4].Text.Trim());
            dblDebtBalance = dblDebtBalance + Convert.ToDouble(e.Row.Cells[7].Text.Trim());
            dblTtlPayment = dblTtlPayment + Convert.ToDouble(e.Row.Cells[9].Text.Trim());
            if (e.Row.Cells[5].Text.Trim().Length == 8)
            {
                e.Row.Cells[5].Text = e.Row.Cells[5].Text.Substring(0, 4) + @"/" + e.Row.Cells[5].Text.Substring(4, 2) + @"/" + e.Row.Cells[5].Text.Substring(6, 2);
            }
            if (e.Row.Cells[8].Text.Trim().Length == 8)
            {
                e.Row.Cells[8].Text = e.Row.Cells[8].Text.Substring(0, 4) + @"/" + e.Row.Cells[8].Text.Substring(4, 2) + @"/" + e.Row.Cells[8].Text.Substring(6, 2);
            }
            if (e.Row.Cells[10].Text.Trim().Length == 8)
            {
                e.Row.Cells[10].Text = e.Row.Cells[10].Text.Substring(0, 4) + @"/" + e.Row.Cells[10].Text.Substring(4, 2) + @"/" + e.Row.Cells[10].Text.Substring(6, 2);
            }
        }

        if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.Cells[0].Text = BaseHelper.GetShowText("04_02010100_037");
            e.Row.Cells[4].Text = dblBalance.ToString();
            e.Row.Cells[7].Text = dblDebtBalance.ToString();
            e.Row.Cells[9].Text = dblTtlPayment.ToString();
            if ((((EntitySet<EntityPay_Macro>)this.grvSetPayCertify.DataSource).Count % 2) == 0)
            {
                e.Row.CssClass = "Grid_Item";
            }
            else
            {
                e.Row.CssClass = "Grid_AlternatingItem";
            }
        }

    }

    /// <summary>
    /// 點擊[列印]按鈕開始列印
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        string strURL = "P040101010004.aspx?Cust_ID=" + RedirectHelper.GetEncryptParam(this.ViewState["UserID"].ToString());

        //* 打開列印簽收總表畫面
        jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.GetScriptForWindowOpenURL(this.Page, strURL));
    }

    /// <summary>
    /// 點擊[離開]按鈕
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.GetScriptForCloseWindow());
    }

    #endregion

    #region Function
    /// <summary>
    /// 顯示窗體文字
    /// </summary>
    /// <returns></returns>
    private void Show()
    {
        //* 列印日期 = 當天
        this.lblDate.Text = DateTime.Today.ToString("yyyy/MM/dd");
        //* Gird
        this.grvSetPayCertify.Columns[0].HeaderText = BaseHelper.GetShowText("04_02010100_025");
        this.grvSetPayCertify.Columns[1].HeaderText = BaseHelper.GetShowText("04_02010100_026");
        this.grvSetPayCertify.Columns[2].HeaderText = BaseHelper.GetShowText("04_02010100_027");
        this.grvSetPayCertify.Columns[3].HeaderText = BaseHelper.GetShowText("04_02010100_028");
        this.grvSetPayCertify.Columns[4].HeaderText = BaseHelper.GetShowText("04_02010100_029");
        this.grvSetPayCertify.Columns[5].HeaderText = BaseHelper.GetShowText("04_02010100_030");
        this.grvSetPayCertify.Columns[6].HeaderText = BaseHelper.GetShowText("04_02010100_031");
        this.grvSetPayCertify.Columns[7].HeaderText = BaseHelper.GetShowText("04_02010100_032");
        this.grvSetPayCertify.Columns[8].HeaderText = BaseHelper.GetShowText("04_02010100_033");
        this.grvSetPayCertify.Columns[9].HeaderText = BaseHelper.GetShowText("04_02010100_034");
        this.grvSetPayCertify.Columns[10].HeaderText = BaseHelper.GetShowText("04_02010100_035");
        this.grvSetPayCertify.Columns[11].HeaderText = BaseHelper.GetShowText("04_02010100_036");
    }

    /// <summary>
    /// Grid查詢綁定資料
    /// </summary>
    /// <returns></returns>
    private void BindGridView()
    {
        EntityPay_MacroSet esMacro = new EntityPay_MacroSet();
        string strMsgID = "";
        if (BRPay_Macro.GetDataByCustID(ViewState["UserID"].ToString().Trim(), ref strMsgID, ref esMacro))
        {
            this.grvSetPayCertify.DataSource = esMacro;
            this.grvSetPayCertify.DataBind();
        }
        jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgID));
    }

    #endregion


}