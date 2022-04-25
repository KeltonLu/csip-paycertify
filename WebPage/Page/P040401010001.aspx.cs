//******************************************************************
//*  作    者：占偉林
//*  功能說明：新增代償證明
//*  創建日期：2009/11/05
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
using System.IO;
using System.Text;
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

public partial class Page_P040401010001 : PageBase
{

    #region 變數區
    /// Session變數集合
    private EntityAGENT_INFO eAgentInfo;
    private structPageInfo sPageInfo;//*記錄網頁訊息
    #endregion

    #region event
    /// <summary>
    /// 初始化元件
    /// </summary>
    private void ShowControlsText()
    {


    }
    /// <summary>
    /// 畫面加載時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.radlGetType.Attributes.Add("onclick", "selectedGetType();");
        if (!IsPostBack)
        {
            DataTable dtblProperty = new DataTable();
            //* 呼叫共通方法，取郵寄方式
            if (!BRM_PROPERTY_CODE.GetCommonProperty("MAILMETHOD2", ref dtblProperty))
            {
                jsBuilder.RegScript(this.UpdatePanel1, "displayDetail('none');" + BaseHelper.HostMsgShow("") + BaseHelper.ClientMsgShow("04_04010100_023"));
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
                jsBuilder.RegScript(this.UpdatePanel1, "displayDetail('none');" + BaseHelper.HostMsgShow("") + BaseHelper.ClientMsgShow("04_04010100_033"));
                return;
            }

            radlGetType.DataTextField = "PROPERTY_NAME";
            radlGetType.DataValueField = "PROPERTY_CODE";
            radlGetType.DataSource = dtblProperty;
            radlGetType.DataBind();
            radlGetType.SelectByValue("Y");

            //* 設置焦點
            this.txtUserIDQuery.Focus();
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
        sPageInfo = (structPageInfo)this.Session["PageInfo"];
    }

    /// <summary>
    /// 點選畫面【查詢】時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        //* 查詢時，畫面輸入欄位正確性檢查
        this.hidTag1.Value = "N";
        this.hidTag2.Value = "N";
        string strCardNo = "";
        string sUserID = txtUserIDQuery.Text.Trim();
        //------------------------------------------------------
        //AuditLog to SOC
        CSIPCommonModel.EntityLayer_new.EntityL_AP_LOG log = BRL_AP_LOG.getDefaultValue(eAgentInfo, sPageInfo.strPageCode);
        log.Customer_Id = txtUserIDQuery.Text.Trim();
       
        BRL_AP_LOG.Add(log);
        //-----------------------------------------------------
        //* 設置上傳HashTable
        Hashtable htInput = new Hashtable();
        Hashtable htCurrBal = new Hashtable();
        DataTable dtblOutput = new DataTable();

        htInput.Add("FUNCTION_CODE", "1");//*ID查詢
        htInput.Add("ACCT_NBR", sUserID);
        htInput.Add("LINE_CNT", "0000");//*LINE_CNT

        try
        {
            //* 取數據

            dtblOutput = MainFrameInfo.GetMainframeDataByPage("P4_JCEH", htInput, false, "1", eAgentInfo);

            if (dtblOutput.Rows[0]["HtgErrMsg"].ToString() != "") //*主機返回失敗
            {
                etMstType = eMstType.Select;
                base.strClientMsg = MessageHelper.GetMessage("04_00000000_008");
                base.strHostMsg = dtblOutput.Rows[0]["HtgErrMsg"].ToString();
            }
            else //*主機返回正確
            {
                base.strClientMsg = MessageHelper.GetMessage("04_00000000_034");
                base.strHostMsg = dtblOutput.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_034");

                txtUserID.Text = txtUserIDQuery.Text;
                this.txtUserName.Text = dtblOutput.Rows[0]["SHORT_NAME"].ToString();//* 客戶姓名
                this.dropCardNo.Items.Clear();

                //* 向畫面卡號DropDownList中添加卡號
                this.dropCardNo.Items.Clear();
                for (int intLoop = 0; intLoop < dtblOutput.Rows.Count; intLoop++)
                {
                    if (dtblOutput.Rows[intLoop]["CARDHOLDER"].ToString().Length >= 6)
                    {
                        strCardNo = dtblOutput.Rows[intLoop]["CARDHOLDER"].ToString();
                        if (Function.IsML(strCardNo) == 1)
                        {
                            dropCardNo.Items.Add(new ListItem(strCardNo));
                            htCurrBal.Add(strCardNo, Convert.ToInt32(dtblOutput.Rows[intLoop]["CURR_BAL"]));
                        }
                    }
                }

                if (dropCardNo.Items.Count <= 0) //無ML卡
                {
                    etClientMstType = eClientMstType.Select;
                    base.strClientMsg = MessageHelper.GetMessage("04_04010100_034");
                    base.sbRegScript.Append(BaseHelper.SetFocus("txtUserIDQuery"));
                }
                else
                {
                    //* 目前餘額
                    ViewState["CurrBal"] = htCurrBal;
                    //* 郵遞區號

                    txtZip.Text = PageBase.StringLeft(dtblOutput.Rows[0]["ZIP"].ToString(), 3);
                    //* 地址一
                    txtAdd1.Text = dtblOutput.Rows[0]["CITY"].ToString();
                    //* 地址二

                    txtAdd2.Text = dtblOutput.Rows[0]["ADDR_1"].ToString();
                    //* 地址三

                    txtAdd3.Text = dtblOutput.Rows[0]["ADDR_2"].ToString();
                    //* 代償人姓名

                    txtPayName.Text = "";
                    //* 代償金額
                    txtPay.Text = "";
                    //* 收件人姓名

                    txtRecvName.Text = "";
                    //* 取得方式
                    radlGetType.SelectedIndex = 0;
                    //* 掛號號碼
                    txtMailNo.Enabled = true;
                    txtMailNo.Text = "";
                    //* 備注
                    txtNote.Text = "";

                    dpPayDay.Text = DateTime.Now.ToString("yyyy/MM/dd");
                    dpLoanDay.Text = DateTime.Now.ToString("yyyy/MM/dd");
                    dpMailDay.Text = DateTime.Now.AddDays(1).ToString("yyyy/MM/dd");

                    base.sbRegScript.Append(BaseHelper.SetFocus("dropCardNo"));
                    //* 顯示明細
                    jsBuilder.RegScript(this.UpdatePanel1, "displayDetail('');");
                }
            }
        }
        catch(Exception ex)
        {
            Logging.Log(ex, LogLayer.UI);
            etClientMstType = eClientMstType.Select;
            base.sbRegScript.Append(BaseHelper.SetFocus("txtUserIDQuery"));
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
        }
    }

    /// <summary>
    /// 點選畫面【儲存】時的處理    /// 修改紀錄：調整Confirm語法 by Ares Stanley 20220124
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        StringBuilder sbRegScript = new StringBuilder("");

        sbRegScript.Append("displayDetail('');selectedGetType();");

        //2010.12.17 檢查該ID/CARDNO 是否存在為 ML轉PL 的未結清戶
        if (BRML2PL.Chk_ML2PL_Exist(this.dropCardNo.Text.ToString()))
        {
            etClientMstType = eClientMstType.Select;
            base.strClientMsg = MessageHelper.GetMessage("04_00000000_053");
            jsBuilder.RegScript(this.UpdatePanel1, sbRegScript.ToString());
            return;
        }
        //

        SqlHelper Sql = new SqlHelper();
        //* 添加查詢條件Void不等於Y
        Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
        //* 添加查詢條件Type等於1 (代償)
        Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "1");
        //* 如果有輸入[身份證字號],則添加查詢條件UserID 等於 [身份證字號]
        Sql.AddCondition(EntitySet_Pay_Certify.M_userID, Operator.Equal, DataTypeUtils.String, this.txtUserID.Text.ToUpper());
        //* 如果有輸入[卡號],則添加查詢條件CardNo 等於 [卡號]
        Sql.AddCondition(EntitySet_Pay_Certify.M_CardNo, Operator.Equal, DataTypeUtils.String, this.dropCardNo.Text);
        //* 撈取符合條件的記錄
        EntitySet_Pay_CertifySet esSetPayCertify = new EntitySet_Pay_CertifySet();
        esSetPayCertify.FillEntitySet(Sql.GetFilterCondition());

        string strHide1 = hidTag1.Value;
        if (esSetPayCertify.TotalCount > 0 && strHide1 != "Y")
        {
            //* esResult.筆數> 0且[隱藏欄位1] != "Y" 
            //20220223_Ares_Neal_修改_彈跳視窗
            //調整swal語法 by Ares Stanley 20220225
            string msg = MessageHelper.GetMessage("04_04010100_019");
            sbRegScript.Append(@"
            Swal.fire({
                title: '@msg',
                showDenyButton: true,
                //showCancelButton: true,
                confirmButtonText: '是',
                denyButtonText: '否',
            }).then(function(result) {
                if (result.isConfirmed) {
                document.getElementById('hidTag1').value='Y';
                $('#btnHiden').click();
                } else if (result.isDenied) {
                document.getElementById('hidTag1').value='N';
                document.getElementById('hidTag2').value='N';
                }
            });").Replace("@msg", msg);
            jsBuilder.RegScript(this.UpdatePanel1, sbRegScript.ToString());
            return;
        }
        else
        {
            CheckJCAS();
        }

    }

    protected void btnHiden_Click(object sender, EventArgs e)
    {
        CheckJCAS();
    }

    protected void btnHiden1_Click(object sender, EventArgs e)
    {
        SavePay_Certify();
    }

    /// <summary>
    /// 點選畫面【清除】按鈕時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnClear_Click(object sender, EventArgs e)
    {
        this.hidTag1.Value = "N";
        this.hidTag2.Value = "N";
        this.ViewState["CurrBal"] = "";
        this.txtUserIDQuery.Text = "";
        StringBuilder sbRegScript = new StringBuilder("");
        sbRegScript.Append("displayDetail('none');");
        sbRegScript.Append("HostMsgShow('');");
        sbRegScript.Append(BaseHelper.ClientMsgShow(""));
        jsBuilder.RegScript(this.UpdatePanel1, sbRegScript.ToString());
    }

    #endregion event

    #region function

    protected void SavePay_Certify()
    {
        string strSeriNo_New = "";
        try
        {
            //* 重新取序列號
            strSeriNo_New = BRSet_Pay_Certify.GetSerialNo();
            //* 如果strSerialNo = ""
            if (strSeriNo_New == "")
            {
                hidTag1.Value = "N";
                hidTag2.Value = "N";

                etClientMstType = eClientMstType.Select;
                base.strClientMsg = MessageHelper.GetMessage("04_04010100_020");
                base.sbRegScript.Append("displayDetail('');selectedGetType();");
                return;
            }

        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            etClientMstType = eClientMstType.Select;
            base.strClientMsg = MessageHelper.GetMessage("04_04010100_020");
            base.sbRegScript.Append("displayDetail('');selectedGetType();");
            return;
        }
        Hashtable htCondition = new Hashtable();

        try
        {
            htCondition = (Hashtable)ViewState["htCondition"];

            //* 檢核成功，保存畫面訊息。
            EntitySet_Pay_Certify eSetPayCertify = new EntitySet_Pay_Certify();
            eSetPayCertify.userID = this.txtUserID.Text;
            eSetPayCertify.userName = this.txtUserName.Text;
            eSetPayCertify.CardNo = this.dropCardNo.Text;
            eSetPayCertify.add1 = this.txtAdd1.Text;
            eSetPayCertify.add2 = this.txtAdd2.Text;
            eSetPayCertify.zip = this.txtZip.Text;
            eSetPayCertify.add3 = this.txtAdd3.Text;
            eSetPayCertify.pay = Convert.ToInt32(this.txtPay.Text);
            eSetPayCertify.payName = this.txtPayName.Text;
            eSetPayCertify.payDay = this.dpPayDay.Text.Replace("/", "");
            eSetPayCertify.note = this.txtNote.Text;
            eSetPayCertify.owe = (int)((Hashtable)ViewState["CurrBal"])[this.dropCardNo.Text];
            //* 取得方式為"郵寄"
            if (this.radlGetType.SelectedIndex == 0)
            {
                eSetPayCertify.isMail = "Y";
                eSetPayCertify.MailType = this.dropMailType.SelectedValue;
                eSetPayCertify.mailDay = this.dpMailDay.Text.Replace("/", "");
                eSetPayCertify.mailNo = this.txtMailNo.Text;
            }
            else
            {
                eSetPayCertify.isMail = "N";
                eSetPayCertify.MailType = "";
                eSetPayCertify.mailDay = this.dpMailDay.Text.Replace("/", "");
                eSetPayCertify.mailNo = "";
            }
            eSetPayCertify.keyinDay = this.dpLoanDay.Text.Replace("/", "");
            eSetPayCertify.RecvName = this.txtRecvName.Text;

            eSetPayCertify.serialNo = strSeriNo_New;
            eSetPayCertify.type = "1"; //* 代償
            eSetPayCertify.keyinUser = ((EntityAGENT_INFO)Session["Agent"]).agent_id;
            eSetPayCertify.@void = "N";
            eSetPayCertify.voidDay = "";
            string strTmpDate = (htCondition["DATE"] + "").ToString().Trim();
            if (strTmpDate == "" || strTmpDate == "00000000")
            {
                eSetPayCertify.LoanDate = "";
            }
            else
            {
                eSetPayCertify.LoanDate = Function.InsertTimeSeparator(strTmpDate).Replace("/", "");
            }
            strTmpDate = (htCondition["SETTLE_DATE"] + "").ToString().Trim();
            if (strTmpDate == "" || strTmpDate == "00000000")
            {
                eSetPayCertify.PayOffDate = "";
            }
            else
            {
                eSetPayCertify.PayOffDate = Function.InsertTimeSeparator(strTmpDate).Replace("/", "");
            }
            eSetPayCertify.PayOffAmt = Convert.ToInt32((htCondition["AMT"] + "").ToString() == "" ? "0" : htCondition["AMT"].ToString());
            eSetPayCertify.BLK_Code = (htCondition["BLOCK_CODE"] + "").ToString(); ;
            eSetPayCertify.Back_Date = "";
            eSetPayCertify.BackNote = "";
            if (BRSet_Pay_Certify.AddNewEntity(eSetPayCertify))
            {
                hidTag1.Value = "N";
                hidTag2.Value = "N";
                ViewState["CurrBal"] = "";
                txtUserIDQuery.Text = "";
                // 調整eClientMstType為Select以顯示alert, 調整alert訊息"結清證明新增存儲成功">"代償證明新增存儲成功" by Ares Stanley 20220224
                etClientMstType = eClientMstType.Select;
                base.strClientMsg = MessageHelper.GetMessage("04_04010100_022");
                base.sbRegScript.Append("displayDetail('none');");
            }
            else
            {
                etClientMstType = eClientMstType.Select;
                base.strClientMsg = MessageHelper.GetMessage("04_04010100_016");
                base.sbRegScript.Append("displayDetail('');selectedGetType();");
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            etClientMstType = eClientMstType.Select;
            base.strClientMsg = MessageHelper.GetMessage("04_04010100_016");
            base.sbRegScript.Append("displayDetail('');selectedGetType();");
        }
    }

    /// <summary>
    /// 修改紀錄：調整Confirm語法 by Ares Stanley 20220124
    /// </summary>
    protected void CheckJCAS()
    {

        StringBuilder sbRegScript = new StringBuilder();
        base.sbRegScript.Append("displayDetail('');selectedGetType();");

        Hashtable htInput = new Hashtable();

        htInput.Add("FUNCTION_CODE", "1");//*ID查詢
        htInput.Add("ACCT_NBR", this.dropCardNo.Text);
        htInput.Add("LINE_CNT", "0000");//*LINE_CNT
        try
        {
            Hashtable htReturn = MainFrameInfo.GetMainframeData("P4_JCAS", htInput, false, "1", eAgentInfo);

            string strHide2 = hidTag2.Value;

            if (htReturn.Contains("HtgMsg")) //*主機返回失敗
            {
                hidTag1.Value = "N";
                hidTag2.Value = "N";

                etMstType = eMstType.Select;
                if (htReturn["HtgMsgFlag"].ToString() == "0")//*若主機訊息標識為"0",顯示到主機訊息,否則主機訊息標識為"1",則顯示到端末訊息
                {
                    base.strClientMsg = MessageHelper.GetMessage("04_00000000_009") + MessageHelper.GetMessage("04_04010100_016");
                    base.strHostMsg = htReturn["HtgMsg"].ToString();
                }
                else
                {
                    base.strClientMsg = htReturn["HtgMsg"].ToString() + MessageHelper.GetMessage("04_04010100_016");
                }                

                return;
            }
            else
            {
                base.strHostMsg = htReturn["HtgSuccess"].ToString();//*主機返回成功訊息     

                ViewState["htCondition"] = htReturn;
                //* 如果strPayOffDate = "" (空) 且[隱藏欄位2] != "Y"
                if (strHide2 != "Y" && (htReturn["SETTLE_DATE"].ToString().Trim() == "" || htReturn["SETTLE_DATE"].ToString().Trim() == "00000000"))
                {
                    string msg = MessageHelper.GetMessage("04_04010100_021");
                    //20220223_Ares_Neal_修改_彈跳視窗
                    //調整swal語法 by Ares Stanley 20220225
                    sbRegScript.Append(@"
                    Swal.fire({
                             title: '@msg',
                             showDenyButton: true,
                            // showCancelButton: true,
                             confirmButtonText: '是',
                             denyButtonText: '否',
                           }).then(functioon(result) {
                             if (result.isConfirmed) {
                               document.getElementById('hidTag2').value='Y';
                               $('#btnHiden1').click();
                             } else if (result.isDenied) {
                               document.getElementById('hidTag1').value='N';
                               document.getElementById('hidTag2').value='N';
                             }
                           });").Replace("@msg", msg);
                    jsBuilder.RegScript(this.UpdatePanel1, sbRegScript.ToString());
                    return;
                }
                else
                {
                    SavePay_Certify();
                }
            }
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            etClientMstType = eClientMstType.Select;
            base.strClientMsg = MessageHelper.GetMessage("00_00000000_000");
            return;
        }
    }


    #endregion function
}
