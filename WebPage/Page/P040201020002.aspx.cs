//******************************************************************
//*  作    者：余洋
//*  功能說明：債清證明查詢明細頁面

//*  創建日期：2009/11/30
//*  修改記錄：//Ares Stanley 2021/12/24 20210058-CSIP作業服務平台現代化II 調整端末訊息註冊位置

//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;

using Framework.Data.OM.Collections;
using Framework.Data.OM;
using Framework.WebControls;
using Framework.Common.Utility;
using Framework.Common.Message;
using Framework.Common.JavaScript;
using Framework.Data.OM.Transaction;

using CSIPPayCertify.BusinessRules;
using CSIPPayCertify.EntityLayer;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;


public partial class Page_P040201020002 : PageBase
{
    #region Page Properties
    private string strSerialNo = "";
    private string strType = "";

    #endregion

    #region  event
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            radlGetType.Attributes.Add("onclick", "selectedGetType();");
            if (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "SerialNo")) && !string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "Type")) && !string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "UserID")))
            {
                //*傳遞歸來的明細ID
                strSerialNo = RedirectHelper.GetDecryptString(this.Page, "SerialNo");
                ViewState["SerialNo"] = strSerialNo;
                DataTable dtblProperty = new DataTable();
                //* 呼叫共通方法，取郵寄方式
                if (!BRM_PROPERTY_CODE.GetCommonProperty("MAILMETHOD2", ref dtblProperty))
                {
                    jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_02010200_024"));
                    return;
                }


                //* 郵寄方式
                dropMailType.DataSource = dtblProperty;
                dropMailType.DataValueField = "PROPERTY_CODE";
                dropMailType.DataTextField = "PROPERTY_NAME";
                dropMailType.DataBind();

                //* 取得方式
                if (!BRM_PROPERTY_CODE.GetCommonProperty("ISMAIL", ref dtblProperty))
                {
                    jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_02010200_025"));
                    return;
                }

                radlGetType.DataTextField = "PROPERTY_NAME";
                radlGetType.DataValueField = "PROPERTY_CODE";
                radlGetType.DataSource = dtblProperty;
                radlGetType.DataBind();
                radlGetType.SelectByValue("Y");

                SetPageControl();
            }
            else
            {
                MessageHelper.ShowMessage(UpdatePanel1, "04_02010200_014");
                //*如果沒有傳遞明細ID直接跳轉
                Response.Redirect("P040201020001.aspx");
            }
        }
    }


    /// <summary>
    /// 當地址一下拉選單改變時,改變郵遞區號
    /// </summary>
    protected void CustAdd1_ChangeValues()
    {
        txtZip.Text = PageBase.StringLeft(this.CustAdd1.strZip,3);
    }

    /// <summary>
    /// 存檔事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        string strMsg = "";
        strType = ViewState["Type"].ToString();
        strSerialNo = ViewState["SerialNo"].ToString();

        //* 查詢時，畫面輸入欄位正確性檢查
        if (!CheckCondition(ref strMsg))
        {
            //* 檢核不成功時，提示不符訊息
            MessageHelper.ShowMessage(UpdatePanel1, strMsg);
            return;
        }

        Save();
    }

    /// <summary>
    /// 點選返回按鈕,返回到前一畫面(查詢明細頁面)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReturn_Click(object sender, EventArgs e)
    {
        Response.Redirect("P040201020001.aspx?UserID=" + Request["UserID"].ToString() + "&Type=" + Request["Type"].ToString()); ;
    }
    #endregion

    #region Method
    /// <summary>
    /// 對當前網頁上的控件預設值
    /// </summary>
    /// <param name="strSerialNo"></param>
    protected void SetPageControl()
    {
        string strMsgID = "";
        DataTable dtblResult = null;

        try
        {
            #region 根據ID查到明細數據,顯示在頁面上.
            if (!BRPay_SV.GetDetail(strSerialNo, ref strMsgID, ref dtblResult))
            {
                jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow(strMsgID));
                return;
            }

            strType = dtblResult.Rows[0]["Type"].ToString();
            ViewState["Type"] = strType;

            //* 客戶ID
            txtUserID.Text = dtblResult.Rows[0]["UserID"].ToString();

            //* 用戶姓名
            txtUserName.Text = dtblResult.Rows[0]["UserName"].ToString();

            //* 郵遞區號
            txtZip.Text = PageBase.StringLeft(dtblResult.Rows[0]["Zip"].ToString(),3);
            //* 地址一
            this.CustAdd1.InitalAdd1(dtblResult.Rows[0]["Add1"].ToString());
            //* 地址二
            txtAdd2.Text = dtblResult.Rows[0]["Add2"].ToString();
            //* 地址三
            txtAdd3.Text = dtblResult.Rows[0]["Add3"].ToString();

            //* 收件人姓名
            txtRecievName.Text = dtblResult.Rows[0]["Consignee"].ToString();

            //* 寄送/自取日期
            dpMailDay.Text = Function.InsertTimeSeparator(dtblResult.Rows[0]["MailDay"].ToString());
            //*  掛號號碼
            txtMailNo.Text = dtblResult.Rows[0]["MailNo"].ToString();

            //* 備註
            txtNote.Text = dtblResult.Rows[0]["Note"].ToString();

            //* 補寄日期
            if (dtblResult.Rows[0]["MakeUp"].ToString() == "Y")
            {
                dpMakeUpDate.Text = Function.InsertTimeSeparator(dtblResult.Rows[0]["MakeUpDate"].ToString());
            }

            dropMailType.SelectByText(dtblResult.Rows[0]["mailMethod"].ToString().Trim());

            //* 最近繳款日
            dpEndDate.Text = Function.InsertTimeSeparator(dtblResult.Rows[0]["EndDate"].ToString());

            //* 退件日期
            dpReturnDate.Text = Function.InsertTimeSeparator(dtblResult.Rows[0]["ReturnDay"].ToString());
            //* 退件原因
            txtReturnCause.Text = dtblResult.Rows[0]["ReturnReason"].ToString();

            //* 顯示附卡資料
            if (dtblResult.Rows[0]["showExtra"].ToString() == "Y ")
            {
                this.chkShowExtra.Checked = true;
            }
            //* 是否補寄
            if (dtblResult.Rows[0]["getFee"].ToString() == "Y")
            {
                this.chkGetFee.Checked = true;
            }
            //* 郵寄類型
            if (dtblResult.Rows[0]["IsMail"].ToString() == "Y")
            {
                radlGetType.SelectedIndex = 0;
            }
            else
            {
                radlGetType.SelectedIndex = 1;
            }
             #endregion
        }
        catch(Exception ex)
        {
            Framework.Common.Logging.Logging.Log( ex, Framework.Common.Logging.LogLayer.UI);
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_02010200_013"));
            return;
        }
    }


    /// <summary>
    /// 綁定郵寄方式欄位
    /// </summary>
    private void BindType()
    {
        DataTable dtblProperty = new DataTable();
        if (BRM_PROPERTY_CODE.GetCommonProperty("MAILMETHOD2", ref dtblProperty))
        {
            if (dtblProperty.Rows.Count > 0)
            {
                dropMailType.DataTextField = "PROPERTY_NAME";
                dropMailType.DataValueField = "PROPERTY_CODE";
                dropMailType.DataSource = dtblProperty;
                dropMailType.DataBind();
                dropMailType.SelectedValue = "MailMethod_Id";
            }
        }


        //* 取得方式
        if (BRM_PROPERTY_CODE.GetCommonProperty("ISMAIL", ref dtblProperty))
        {
            this.radlGetType.DataTextField = "PROPERTY_NAME";
            this.radlGetType.DataValueField = "PROPERTY_CODE";
            this.radlGetType.DataSource = dtblProperty;
            this.radlGetType.DataBind();
            this.radlGetType.SelectByValue("Y");
        }

    }

    /// <summary>
    ///保存資料到DB
    /// </summary>
    /// <param name="strSerialNo">證明編號</param>
    /// <returns></returns>
    private void Save()
    {
        string strMsgID = "";
        try
        {
            if (!CheckCondition(ref strMsgID))
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
                return ;
            }
            string strIsMail = "";
            string strGetFee = "";
            string strShowExtra = "";
            //* ”郵寄則strIsMail:Y 否則strIsMail:N
            if (radlGetType.Items[0].Selected)
            {
                strIsMail = "Y";
            }
            else
            {
                strIsMail = "N";
            }
            //* 是補寄strGetFee:Y 否則 strGetFee:N
            if (chkGetFee.Checked)
            {
                strGetFee = "Y";
            }
            else
            {
                strGetFee = "N";
            }
            //* 顯示副卡strShowExtra:Y  否則strShowExtra:N
            if (chkShowExtra.Checked)
            {
                strShowExtra = "Y";
            }
            else
            {
                strShowExtra = "N";
            }

            EntityPay_SV ePaySv = new EntityPay_SV();
            ePaySv.UserID = txtUserID.Text.Trim();  //* 客戶ID
            ePaySv.UserName = txtUserName.Text.Trim();  //* 客戶姓名
            ePaySv.Zip = txtZip.Text.Trim();  //* 郵遞區號
            ePaySv.Add1 = CustAdd1.strAddress;  //* 地址一(1) 下拉選單的文字+ 地址一(2)下拉選單的文字

            ePaySv.Add2 = txtAdd2.Text.Trim();  //* 地址二
            ePaySv.Add3 = txtAdd3.Text.Trim();  //* 地址三

            ePaySv.MailDay = dpMailDay.Text.Trim().Replace("/", "");  //* 寄送/自取日期
            ePaySv.MailNo = txtMailNo.Text;  //* 掛號號碼
            ePaySv.Note = txtNote.Text;  //* 備註
            ePaySv.Consignee = txtRecievName.Text;  //* 收件人姓名
            ePaySv.IsMail = strIsMail;  //* 是否郵寄
            ePaySv.GetFee = strGetFee;  //* 是否補寄
            ePaySv.@void = "";
            ePaySv.ShowExtra = strShowExtra;  //* 是否顯示副卡
            ePaySv.Enddate = dpEndDate.Text.Trim().Replace("/", "");  //* 最近繳款日
            if (dpMakeUpDate.Text!="")
            {
                ePaySv.MakeUp = "Y";
                ePaySv.MakeUpDate = dpMakeUpDate.Text.Trim().Replace("/", "");
            }
            else
            {
                ePaySv.MakeUp = "N";
                ePaySv.MakeUpDate = string.Empty;
            }
            ePaySv.MailMethod = dropMailType.SelectedItem.Text;

            SqlHelper Sql = new SqlHelper();
            //條件是SerialNo= SerialNo
            Sql.AddCondition(EntityPay_SV.M_SerialNo, Operator.Equal, DataTypeUtils.String, strSerialNo);

            using (OMTransactionScope ts = new OMTransactionScope())
            {
                if (BRPay_SV.UpdatePay(ePaySv, Sql.GetFilterCondition()))
                {
                    string strTransAction = "";

                    if (strType == "G")
                    {
                        strTransAction = BaseHelper.GetShowText("04_02010200_045");
                    }
                    else if (strType == "T")
                    {
                        strTransAction = BaseHelper.GetShowText("04_02010200_046");
                    }
                    else if (strType == "C")
                    {
                        strTransAction = BaseHelper.GetShowText("04_02010200_047");
                    }
                    else if (strType == "M")
                    {
                        strTransAction = BaseHelper.GetShowText("04_02010200_048");
                    }
                    else if (strType == "O")
                    {
                        strTransAction = BaseHelper.GetShowText("04_02010200_049");
                    }
                    else if (strType == "B")
                    {
                        strTransAction = BaseHelper.GetShowText("04_02010200_050");
                    }
                    else if (strType == "N")
                    {
                        strTransAction = BaseHelper.GetShowText("04_02010200_051");
                    }

                    EntitySystem_log eSystemlog = new EntitySystem_log();

                    eSystemlog.Log_date = DateTime.Now.ToString("yyyyMMdd");
                    eSystemlog.Log_Time = DateTime.Now.ToString("hhmm");
                    eSystemlog.CustomerID = ePaySv.UserID;
                    eSystemlog.Log_Action = strTransAction;
                    eSystemlog.Serial_No = strSerialNo;
                    eSystemlog.User_Name = ((EntityAGENT_INFO)this.Session["Agent"]).agent_name;
                    eSystemlog.User_ID = ((EntityAGENT_INFO)this.Session["Agent"]).agent_id;
                    eSystemlog.System_Type = "SV";

                    if (eSystemlog.DB_InsertEntity())
                    {
                        strMsgID = "04_02010200_019";
                        ts.Complete();
                    }
                    else
                    {
                        strMsgID = "04_02010200_023";
                    }
                }
                else
                {
                    strMsgID = "04_02010200_018";
                }
            }
            jsBuilder.RegScript(this.Page,BaseHelper.ClientMsgShow(strMsgID));
        }
        catch (Exception ex)
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_02010200_019"));
            Framework.Common.Logging.Logging.Log( ex, Framework.Common.Logging.LogLayer.UI);
            return ;
        }
    }

    /// <summary>
    /// 存檔時，檢核欄位的合法性
    /// </summary>
    /// <param name="strMsg"></param>
    /// <returns></returns>
    protected bool CheckCondition(ref string strMsgID)
    {
        //* 收件人姓名必須輸入
        if (string.IsNullOrEmpty(txtUserName.Text))
        {
            strMsgID = "04_02010200_002";
            txtUserName.Focus();
            return false;
        }

        //* 郵遞區號必須輸入
        if (string.IsNullOrEmpty(txtZip.Text))
        {
            strMsgID = "04_02010200_003";
            txtZip.Focus();
            return false;
        }

        //* 地址二必須輸入
        if (string.IsNullOrEmpty(txtAdd2.Text))
        {
            strMsgID = "04_02010200_005";
            txtAdd2.Focus();
            return false;
        }
        //* 地址二欄位中英文數字總長度超過28個字元
        if (BaseHelper.GetByteLength(this.txtAdd2.Text.Trim()) > 28)
        {
            strMsgID = "04_01010100_032";
            this.txtAdd2.Focus();
            return false;
        }
        //* 地址三欄位中英文數字總長度超過28個字元
        if (BaseHelper.GetByteLength(this.txtAdd3.Text.Trim()) > 28)
        {
            strMsgID = "04_01010100_033";
            this.txtAdd3.Focus();
            return false;
        }
        //* 收件人姓名
        if (string.IsNullOrEmpty(txtRecievName.Text))
        {
            strMsgID = "04_02010200_006";
            txtRecievName.Focus();
            return false;
        }

        //* 郵寄/自取日期必須輸入
        if (string.IsNullOrEmpty(dpMailDay.Text))
        {
            strMsgID = "04_02010200_007";
            dpMailDay.Focus();
            return false;
        }

        //* 取得方式為'郵寄'時
        if (this.radlGetType.Items[0].Selected)
        {
            //* 郵寄方式沒有選擇，提示‘證明取得方式為郵寄，郵寄方式欄位必須輸入’
            if (string.IsNullOrEmpty(dropMailType.SelectedValue))
            {
                strMsgID = "04_02010200_008";
                dropMailType.Focus();
                return false;
            }

            //* 如果郵寄方式為掛號的話則此欄位為 必輸入欄位請輸入掛號號碼
            if (dropMailType.SelectedItem.Text == BaseHelper.GetShowText("04_02010200_039"))
            {
                //* 掛號號碼為空時，
                if (string.IsNullOrEmpty(txtMailNo.Text))
                {
                    strMsgID = "04_02010200_009";
                    txtMailNo.Focus();
                    return false;
                }
            }
        }
        return true;
    }
    #endregion

}
