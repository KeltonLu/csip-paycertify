//******************************************************************
//*  作    者：chaoma
//*  功能說明：債清證明查詢(明細)

//*  創建日期：2009/11/26
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
public partial class Page_P040202040002 : PageBase
{
    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = BaseHelper.GetShowText("04_02020400_000");
        if (!IsPostBack)
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(""));
            if (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "SerialNo"))
                && !string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "UserID"))
                && !string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "Type")))
            {
                //* 證明編號
                string SerialNo = RedirectHelper.GetDecryptString(this.Page, "SerialNo");
                ViewState["SerialNo"] = SerialNo;

                ShowControlsText();
                BindData();
            }
        }
    }
    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("P040202040001.aspx?UserID=" + Request["UserID"].ToString() + "&Type=" + Request["Type"].ToString());
    }
    #endregion event

    #region method
    /// <summary>
    /// 文字顯示
    /// </summary>
    private void ShowControlsText()
    {
        //*radiobutton顯示文字
        this.radIsMial.Text = BaseHelper.GetShowText("04_02020400_034");
        this.radIsSelf.Text = BaseHelper.GetShowText("04_02020400_035");
    }
    /// <summary>
    /// 綁定各項數據
    /// </summary>
    private void BindData()
    {
        string strMsgID = "";
        DataTable dtblPay_SV = null;
        try
        {
            //*郵寄方式綁定
            BindMailType();
            if (!BRPay_SV.GetDetail(this.ViewState["SerialNo"].ToString(), ref strMsgID, ref dtblPay_SV))
            {
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgID));
                return;
            }
            //*地址1欄位綁定
            this.CustAdd1.InitalAdd1_1(dtblPay_SV.Rows[0]["Add1"].ToString().Substring(0, 3));
            this.CustAdd1.InitalAdd1_2(dtblPay_SV.Rows[0]["Add1"].ToString().Substring(3));
            this.CustAdd1.Disabled = true;
            //*其他數據綁定
            txtUserID.Text = dtblPay_SV.Rows[0]["UserID"].ToString();
            txtUserID.Enabled = false;
            txtUserName.Text = dtblPay_SV.Rows[0]["UserName"].ToString();
            txtUserName.Enabled = false;
            txtZip.Text = PageBase.StringLeft(dtblPay_SV.Rows[0]["Zip"].ToString(),3);
            txtZip.Enabled = false;
            txtAdd2.Text = dtblPay_SV.Rows[0]["Add2"].ToString();
            txtAdd2.Enabled = false;
            txtAdd3.Text = dtblPay_SV.Rows[0]["Add3"].ToString();
            txtAdd3.Enabled = false;
            txtConsignee.Text = dtblPay_SV.Rows[0]["Consignee"].ToString();
            txtConsignee.Enabled = false;
            string strTmpMailDay = dtblPay_SV.Rows[0]["MailDay"].ToString() + "";
            if (!string.IsNullOrEmpty(strTmpMailDay.Trim()))
            txtMailDay.Text = dtblPay_SV.Rows[0]["MailDay"].ToString().Substring(0, 4) + "/" + dtblPay_SV.Rows[0]["MailDay"].ToString().Substring(4, 2) + "/" + dtblPay_SV.Rows[0]["MailDay"].ToString().Substring(6, 2);
            txtMailDay.Enabled = false;
            txtMailNo.Text = dtblPay_SV.Rows[0]["MailNo"].ToString();
            txtMailNo.Enabled = false;
            txtNote.Text = dtblPay_SV.Rows[0]["Note"].ToString();
            txtNote.Enabled = false;
            string strTmpEndDate = dtblPay_SV.Rows[0]["EndDate"].ToString() + "";
            if (!string.IsNullOrEmpty(strTmpEndDate.Trim()))
            txtEndDate.Text = dtblPay_SV.Rows[0]["EndDate"].ToString().Substring(0,4)+"/"+dtblPay_SV.Rows[0]["EndDate"].ToString().Substring(4,2)+"/"+dtblPay_SV.Rows[0]["EndDate"].ToString().Substring(6,2);
            txtEndDate.Enabled = false;
            string strTmpReturnDay = dtblPay_SV.Rows[0]["ReturnDay"].ToString() + "";
            if (!string.IsNullOrEmpty(strTmpReturnDay.Trim()))
            txtReturnDay.Text = dtblPay_SV.Rows[0]["ReturnDay"].ToString().Substring(0,4)+"/"+dtblPay_SV.Rows[0]["ReturnDay"].ToString().Substring(4,2)+"/"+dtblPay_SV.Rows[0]["ReturnDay"].ToString().Substring(6,2);
            txtReturnDay.Enabled = false;
            txtReturnReason.Text = dtblPay_SV.Rows[0]["ReturnReason"].ToString();
            txtReturnReason.Enabled = false;
            this.dropMailType.SelectedValue = dtblPay_SV.Rows[0]["MailMethod_Id"].ToString();
            dropMailType.Enabled = false;
            if (dtblPay_SV.Rows[0]["showExtra"].ToString().Trim() == "Y ")
            {
                this.chkshowExtra.Checked = true;
            }
            if (dtblPay_SV.Rows[0]["getFee"].ToString() == "Y")
            {
                this.chkgetFee.Checked = true;
            }
            chkshowExtra.Enabled = false;
            chkgetFee.Enabled = false;
            if (dtblPay_SV.Rows[0]["IsMail"].ToString() == "Y")
            {
                this.radIsMial.Checked = true;
            }
            else
            {
                this.radIsSelf.Checked = true;
            }
            radIsMial.Enabled = false;
            radIsSelf.Enabled = false;
            if (dtblPay_SV.Rows[0]["MakeUp"].ToString() == "Y")
            {
                txtMakeUpDate.Text = dtblPay_SV.Rows[0]["MakeUpDate"].ToString().Substring(0,4)+"/"+dtblPay_SV.Rows[0]["MakeUpDate"].ToString().Substring(4,2)+"/"+dtblPay_SV.Rows[0]["MakeUpDate"].ToString().Substring(6,2);
            }
            txtMakeUpDate.Enabled = false;
        }
        catch (Exception ex)
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("04_02020400_005"));
            Framework.Common.Logging.Logging.Log( ex, Framework.Common.Logging.LogLayer.UI);
            return;
        }

    }
    /// <summary>
    /// 綁定郵寄方式欄位
    /// </summary>
    private void BindMailType()
    {
        DataTable dtblMailType = new DataTable();
        if (BRM_PROPERTY_CODE.GetCommonProperty("MAILMETHOD", ref dtblMailType))
        {
            if (dtblMailType.Rows.Count > 0)
            {
                dropMailType.DataTextField = "PROPERTY_NAME";
                dropMailType.DataValueField = "PROPERTY_CODE";
                dropMailType.DataSource = dtblMailType;
                dropMailType.DataBind();
            }
        }
        else
        {
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("04_02020400_005"));
        }
    }

    #endregion method
}
