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
using Framework.Common.Utility;
using Framework.Common.Logging;
using Framework.Common.Message;
using CSIPPayCertify.BusinessRules;
using CSIPCommonModel.BaseItem;

public partial class Page_P040202030002 : PageBase
{
    string strSerialNo = "";

    #region event
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "SerialNo")) && !string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "IsKeyIn")))
            {
                //* 證明編號
                strSerialNo = RedirectHelper.GetDecryptString(this.Page, "SerialNo");

                Show();
                BindData();
            }
            else
            {
                Response.Redirect("P040202030001.aspx");
            }
        }
    }

    /// <summary>
    /// 修改紀錄：報表產出改NPOI by Ares Stanley 20220120
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        try
        {
            string strSerialNo = RedirectHelper.GetDecryptString(this.Page, "SerialNo");
            string strMsgID = "";

            DataSet dstResult = new DataSet();

            if (BRReport.Report02020300(strSerialNo, ref strMsgID, ref dstResult))
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
                if (!BR_Excel_File.CreateExcelFile_Report02020300(dstResult, ref strServerPathFile))
                {
                    jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_058")));
                    return;
                }
                //* 將服務器端生成的文檔，下載到本地。
                string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
                string strFileName = "債清客戶列印個別債清證明書" + strYYYYMMDD + ".xls";

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

    protected void btnBack_Click(object sender, EventArgs e)
    {
        Response.Redirect("P040202030001.aspx?IsKeyIn=" + Request["IsKeyIn"].ToString() + "&UserID=" + Request["UserID"].ToString() + "&BeforeDate=" + Request["BeforeDate"].ToString() + "&EndDate=" + Request["EndDate"].ToString());
    }

    #endregion event

    #region method
    /// <summary>
    /// 文字顯示
    /// </summary>
    private void Show()
    {
        Page.Title = BaseHelper.GetShowText("04_02020300_000");
        //*radiobutton顯示文字
        this.radIsMial.Text = BaseHelper.GetShowText("04_02020300_031");
        this.radIsSelf.Text = BaseHelper.GetShowText("04_02020300_032");
    }
    /// <summary>
    /// 綁定各項數據
    /// </summary>
    private void BindData()
    {
        string strMsgID = "";
        DataTable dtblResult = null;
        try
        {
            if (!BRPay_SV.GetDetail(strSerialNo, ref strMsgID, ref dtblResult))
            {
                jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(strMsgID));
                return;
            }

            //*郵寄方式綁定
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
                jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_02020300_004"));
                return;
            }

            //*地址1欄位綁定
            this.CustAdd1.InitalAdd1_1(dtblResult.Rows[0]["Add1"].ToString().Substring(0, 3));
            this.CustAdd1.InitalAdd1_2(dtblResult.Rows[0]["Add1"].ToString().Substring(3));
            this.CustAdd1.Disabled = true;
            //*其他數據綁定
            txtUserID.Text = dtblResult.Rows[0]["UserID"].ToString();

            txtUserName.Text = dtblResult.Rows[0]["UserName"].ToString();

            txtZip.Text = PageBase.StringLeft(dtblResult.Rows[0]["Zip"].ToString(), 3);

            txtAdd2.Text = dtblResult.Rows[0]["Add2"].ToString();

            txtAdd3.Text = dtblResult.Rows[0]["Add3"].ToString();

            txtConsignee.Text = dtblResult.Rows[0]["Consignee"].ToString();

            txtMailDay.Text = dtblResult.Rows[0]["MailDay"].ToString();

            txtMailNo.Text = dtblResult.Rows[0]["MailNo"].ToString();

            txtNote.Text = dtblResult.Rows[0]["Note"].ToString();

            txtEndDate.Text = dtblResult.Rows[0]["EndDate"].ToString();

            txtReturnDay.Text = dtblResult.Rows[0]["ReturnDay"].ToString();

            txtReturnReason.Text = dtblResult.Rows[0]["ReturnReason"].ToString();

            this.dropMailType.SelectedValue = dtblResult.Rows[0]["MailMethod_Id"].ToString();

            if (dtblResult.Rows[0]["showExtra"].ToString() == "Y ")
            {
                this.chkshowExtra.Checked = true;
            }
            if (dtblResult.Rows[0]["getFee"].ToString() == "Y")
            {
                this.chkgetFee.Checked = true;
            }

            if (dtblResult.Rows[0]["IsMail"].ToString() == "Y")
            {
                this.radIsMial.Checked = true;
            }
            else
            {
                this.radIsSelf.Checked = true;
            }

            if (dtblResult.Rows[0]["MakeUp"].ToString() == "Y")
            {
                txtMakeUpDate.Text = dtblResult.Rows[0]["MakeUpDate"].ToString();
            }
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_02020300_003"));
        }
        catch (Exception ex)
        {
            Framework.Common.Logging.Logging.Log(ex, Framework.Common.Logging.LogLayer.UI);
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_02020300_004"));
            return;
        }
    }


    #endregion method

}
