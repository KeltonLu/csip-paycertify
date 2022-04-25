//******************************************************************
//*  作    者：余洋
//*  功能說明：作廢代償證明(明細)
//*  創建日期：2009/11/03
//*  修改記錄：調整confirm視窗語法 by Ares Stanley 20220122


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

public partial class Page_P040401030002 : PageBase
{
    #region Event
    protected void Page_Load(object sender, EventArgs e)
    {
       
        this.btnMark.OnClientClick = string.Format("return AlertYesNo({{title:'{0}', bn: this}})", MessageHelper.GetMessage("04_04010300_005") + RedirectHelper.GetDecryptString(this.Page, "SerialNo") + MessageHelper.GetMessage("04_00000000_004"));
        if (!Page.IsPostBack)
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(""));
            if (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "SerialNo")))
            {
                //* 證明編號
                string SerialNo = RedirectHelper.GetDecryptString(this.Page, "SerialNo");
                ViewState["SerialNo"] = SerialNo;


                ShowControlsText();
                BindData();
            }

        }

    }
    /// <summary>
    /// 點選【作廢】Button時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnMark_Click(object sender, EventArgs e)
    {
        if (BRSet_Pay_Certify.MarkCancel(ViewState["SerialNo"].ToString()))
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_04010300_004"));
        }
        else
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_04010300_003"));
        }
    }
    /// <summary>
    /// 點選【返回上頁】Button時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("P040401030001.aspx?UserID=" + Request["UserID"].ToString() + "&CardNo=" + Request["CardNo"].ToString());
    }
    #endregion
    #region Function
    /// <summary>
    /// 顯示窗體文字
    /// </summary>
    private void ShowControlsText()
    {
        //* 設置頁面控件顯示文字
        this.btnMark.Text = BaseHelper.GetShowText("04_04010300_024");
        this.btnCancel.Text = BaseHelper.GetShowText("04_04010300_025");
        this.rblGetType.Items[0].Text = BaseHelper.GetShowText("04_04010300_016");
        this.rblGetType.Items[1].Text = BaseHelper.GetShowText("04_04010300_017");
        this.dropMailType.Enabled = false;
        this.rblGetType.Enabled = false;
    }
    /// <summary>
    /// 將查詢結果顯示到頁面上

    /// </summary>
    private void BindData()
    {
        try
        {

            BindMailType();
            EntitySet<EntitySet_Pay_Certify> esResult = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(GetCondition());
            if (esResult.TotalCount > 0)
            {
                txtAdd1.Text = esResult.GetEntity(0).add1;
                txtAdd2.Text = esResult.GetEntity(0).add2;
                txtAdd3.Text = esResult.GetEntity(0).add3;
                txtcardNO.Text = esResult.GetEntity(0).CardNo;
                txtID_Name.Text = esResult.GetEntity(0).userName;
                txtID_No.Text = esResult.GetEntity(0).userID;
                txtPay.Text = esResult.GetEntity(0).pay.ToString();
                txtPayName.Text = esResult.GetEntity(0).payName;
                if (esResult.GetEntity(0).keyinDay.Length >= 8)
                {
                    txtKeyinDay.Text = esResult.GetEntity(0).keyinDay.Substring(0, 4) + "/" + esResult.GetEntity(0).keyinDay.Substring(4, 2) + "/" + esResult.GetEntity(0).keyinDay.Substring(6, 2);
                }
                if (esResult.GetEntity(0).payDay.Length >= 8)
                {
                    txtPayDay.Text = esResult.GetEntity(0).payDay.Substring(0, 4) + "/" + esResult.GetEntity(0).payDay.Substring(4, 2) + "/" + esResult.GetEntity(0).payDay.Substring(6, 2);
                }
                txtNote.Text = esResult.GetEntity(0).note;
                if (esResult.GetEntity(0).PayOffDate.Length >= 8)
                {
                    txtPayOffDate.Text = esResult.GetEntity(0).PayOffDate.Substring(0, 4) + "/" + esResult.GetEntity(0).PayOffDate.Substring(4, 2) + "/" + esResult.GetEntity(0).PayOffDate.Substring(6, 2);
                }
                txtRecievName.Text = esResult.GetEntity(0).RecvName;
                txtSerialNo.Text = esResult.GetEntity(0).serialNo;
                txtZip.Text = PageBase.StringLeft(esResult.GetEntity(0).zip,3);
                if (esResult.GetEntity(0).isMail == "Y")
                {

                    this.rblGetType.Items[0].Selected = true;
                    if (esResult.GetEntity(0).mailDay.Length > 7)
                    {
                        txtMailDay.Text = esResult.GetEntity(0).mailDay.Substring(0, 4) + "/" + esResult.GetEntity(0).mailDay.Substring(4, 2) + "/" + esResult.GetEntity(0).mailDay.Substring(6, 2);
                    }
                    txtMailNo.Text = esResult.GetEntity(0).mailNo;
                    this.dropMailType.SelectedValue = esResult.GetEntity(0).MailType;
                }
                else
                {
                    this.rblGetType.Items[1].Selected = true;
                    if (esResult.GetEntity(0).mailDay.Length > 7)
                    {
                        txtMailDay.Text = esResult.GetEntity(0).mailDay.Substring(0, 4) + "/" + esResult.GetEntity(0).mailDay.Substring(4, 2) + "/" + esResult.GetEntity(0).mailDay.Substring(6, 2);
                    }
                    txtMailNo.Text = "";
                }
            }
        }
        catch (Exception ex)
        {
            Framework.Common.Logging.Logging.Log( ex, Framework.Common.Logging.LogLayer.UI);
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_04010300_001"));

        }
    }
    /// <summary>
    /// 綁定郵寄方式欄位
    /// </summary>
    private void BindMailType()
    {
        DataTable dtblMailType = new DataTable();
        if (BRM_PROPERTY_CODE.GetCommonProperty("MAILMETHOD2", ref dtblMailType))
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
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("04_04010300_001"));
        }
    }
    /// <summary>
    ///取得SQL查詢條件
    /// </summary>
    private string GetCondition()
    {
        SqlHelper Sql = new SqlHelper();
        //添加查詢條件SerialNo
        if (ViewState["SerialNo"] != null)
        {
            Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.Equal, DataTypeUtils.String, ViewState["SerialNo"].ToString());
        }
        return Sql.GetFilterCondition();

    }
    
    #endregion
}
