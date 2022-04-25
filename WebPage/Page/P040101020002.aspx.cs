//******************************************************************
//*  作    者：rosickyyu(yangyu)
//*  功能說明：清償證明修改詳細頁面

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

public partial class Page_P040101020002 : PageBase
{
    #region Page Properties
    private string strType = "";
    private string strCardType = "";
    private string strMainCard4E = "";
    private string strCardTmpID = "";
    private string strCardOpenDay ="";
    private string strCardCloseDay = "";
    private string strMakeUpOther = "N";
    private string strSerialNo = "";
    private string strKeyinDay = "";
    private string strOpenDay = "";
    private string strCloseDay = "";
    private EntityAGENT_INFO eAgentInfo;
    #endregion

    #region  event
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            radlGetType.Attributes.Add("onclick", "selectedGetType();");
            if (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "SerialNo"))  && !string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "Type")) && !string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "UserID")))
            {
                //*傳遞歸來的明細ID
                strSerialNo = RedirectHelper.GetDecryptString(this.Page, "SerialNo");
                ViewState["SerialNo"]=strSerialNo;

                DataTable dtblProperty = new DataTable();
                //* 呼叫共通方法，取郵寄方式
                if (!BRM_PROPERTY_CODE.GetCommonProperty("MAILMETHOD2", ref dtblProperty))
                {
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("04_01010200_024"));
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
                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("04_01010200_025"));
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
                //*如果沒有傳遞明細ID直接跳轉
                Response.Redirect("P040101020001.aspx");
            }
        }
        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
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
        strCardType = ViewState["CardType"].ToString();
        strMainCard4E = ViewState["mainCard4E"].ToString();
        strSerialNo=ViewState["SerialNo"].ToString();
        if (null != ViewState["MakeUpOther"] && ViewState["MakeUpOther"].ToString()=="Y")
        {
            strMakeUpOther = "Y";
        }
        strKeyinDay = ViewState["KeyinDay"].ToString();
        strOpenDay = ViewState["OpenDay"].ToString();
        strCloseDay = ViewState["CloseDay"].ToString();



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
        Response.Redirect("P040101020001.aspx?UserID=" + Request["UserID"].ToString() + "&Type=" + Request["Type"].ToString()); ;
    }
    #endregion

    #region Method
    /// <summary>
    /// 對當前網頁上的控件預設值
    /// </summary>
    /// <param name="strSerialNo"></param>
    protected void SetPageControl()
    {
        DataTable dtblResult = null;
        string strMsgID = "";

        try
        {
            #region 根據ID查到明細數據,顯示在頁面上.
            if (!BRPay_Certify.GetDetail(strSerialNo, ref strMsgID, ref dtblResult))
            {
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsgID));
                return;
            }


            strType = dtblResult.Rows[0]["Type"].ToString();
            ViewState["Type"] = strType;

            strCardType = dtblResult.Rows[0]["CardType"].ToString();
            ViewState["CardType"] = strCardType;

            strMainCard4E = dtblResult.Rows[0]["mainCard4E"].ToString();
            ViewState["mainCard4E"] = strMainCard4E;

            if ( dtblResult.Rows[0]["MakeUpOther"].ToString()=="Y")
            {
                ViewState["MakeUpOther"] = "Y";
            }

            strKeyinDay = dtblResult.Rows[0]["keyinDay"].ToString();
            ViewState["KeyinDay"] = strKeyinDay;

            strOpenDay = dtblResult.Rows[0]["openDay"].ToString();
            ViewState["OpenDay"] = strOpenDay;

            strCloseDay = dtblResult.Rows[0]["closeDay"].ToString();
            ViewState["CloseDay"] = strCloseDay;
            
            //* 客戶ID
            txtUserID.Text = dtblResult.Rows[0]["UserID"].ToString();

            //* 用戶姓名
            txtUserName.Text = dtblResult.Rows[0]["UserName"].ToString();
            
            //* 代償人姓名
            txtPayUserName.Text = dtblResult.Rows[0]["PayName"].ToString();
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
            //* 是否免收
            if (dtblResult.Rows[0]["IsFree"].ToString() == "Y")
            {
                this.chkIsFree.Checked = true;
            }
            //* 寄送/自取日期
            dpMailDay.Text = Function.InsertTimeSeparator(dtblResult.Rows[0]["MailDay"].ToString());
            //*  掛號號碼
            txtMailNo.Text = dtblResult.Rows[0]["MailNo"].ToString();

            //* 備註
            txtNote.Text = dtblResult.Rows[0]["Note"].ToString();

            //* 最近繳款日

            dpEndDate.Text = Function.InsertTimeSeparator(dtblResult.Rows[0]["EndDate"].ToString());

            //* 退件日期
            dpReturnDate.Text =  Function.InsertTimeSeparator(dtblResult.Rows[0]["ReturnDay"].ToString());
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
            //* 補寄日期
            if (dtblResult.Rows[0]["getFee"].ToString() == "Y")
            {
                dpMakeUpDate.Text = Function.InsertTimeSeparator(dtblResult.Rows[0]["MakeUpDate"].ToString());
            }

            dropMailType.SelectByText(dtblResult.Rows[0]["mailMethod"].ToString().Trim());
            //* 欠款總額
            txtArrearsCount.Text = dtblResult.Rows[0]["Owe"].ToString();
            //* 代償金額
            txtRepayNum.Text = dtblResult.Rows[0]["Pay"].ToString();
            //* 代償日期
            dpRepayDate.Text = Function.InsertTimeSeparator(dtblResult.Rows[0]["PayDay"].ToString());

            txtUserName.Focus();

            switch (strType)
            {
                case "C":
                case "M":
                case "N":
                case "O":
                    //* [清償證明-卡]=C;[清償證明-ML&卡]=M;[GCB清償證明-卡]=0;[清償證明-ML Only]=N;
                    this.txtPayUserName.Enabled = false;    //* 代償人姓名
                    this.txtArrearsCount.Enabled = false;
                    this.txtRepayNum.Enabled = false;
                    this.dpRepayDate.Enable = false;
                    break;
                case "G":
                case "T":
                case "B":
                    //* [代償證明-卡]=G;[GCB代償證明]=B;[代償證明-ML]=T
                    this.txtPayUserName.Enabled = true;
                    this.txtArrearsCount.Enabled = true;
                    this.txtRepayNum.Enabled = true;
                    this.dpRepayDate.Enable = true;
                    this.dpRepayDate.Text = DateTime.Today.ToString("yyyy/MM/dd"); //*代償日期
                    break;
            }

            #endregion
        }

        catch (Exception ex)
        {
            Framework.Common.Logging.Logging.Log(ex, Framework.Common.Logging.LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("04_01010200_012"));
            return;
        }

    }

    /// <summary>
    ///保存資料到DB
    /// </summary>
    /// <param name="strSerialNo">證明編號</param>
    /// <returns></returns>
    private void Save()
    {
        string strMsg = "";
        string strUserID = this.txtUserID.Text.Trim();

        Hashtable htInput = new Hashtable();    //* 電文上行查詢條件欄位列表
        DataTable dtblJCEH = new DataTable();   //* 1331(JCEH)電文查詢結果
        DataTable dtblJCHN = new DataTable();   //* 1350(JCHN)電文查詢結果

        try
        {
            if (!CheckCondition(ref strMsg))
            {
                MessageHelper.ShowMessage(this.UpdatePanel1, strMsg);
                return ;
            }
            string strIsMail ="";
            string strGetFee = "";
            string strShowExtra = "";
            string strIsFree = "";
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
            //* 是否免收
            if (chkIsFree.Checked)
            {
                strIsFree = "Y";
            }
            else
            {
                strIsFree = "N";
            }

            if (strType == "O" || strType == "B")
            {
                htInput.Clear();

                htInput.Add("FUNCTION_CODE", "1");//*ID查詢
                htInput.Add("IDNO", strUserID);
                htInput.Add("LINE_CNT", "0000");//*LINE_CNT
                htInput.Add("LAST_KEY", "");

                dtblJCHN = MainFrameInfo.GetMainframeDataByPage("P4_JCHN", htInput, false, "1", eAgentInfo);
                if (dtblJCHN.Rows[0]["HtgErrMsg"].ToString() != "") //*主機返回失敗
                {
                    etClientMstType = eClientMstType.Select;
                    if (dtblJCHN.Rows[0]["MESSAGE_TYPE"].ToString() == "8888")
                    {
                        base.strClientMsg = MessageHelper.GetMessage("04_01010100_009");
                    }
                    else
                    {
                        base.strClientMsg = MessageHelper.GetMessage("04_00000000_037");
                    }
                    base.strHostMsg = dtblJCHN.Rows[0]["HtgErrMsg"].ToString();
                    return;
                }
                else //*主機返回正確
                {
                    base.strClientMsg = MessageHelper.GetMessage("04_00000000_036");
                    base.strHostMsg = dtblJCHN.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_036");

                    //插入到Pay_Card_Temp表.並且得到strOpenDay和strCloseDay
                    if (!BRPay_Card_Temp.InsertPayCardTempDataByGCB(dtblJCHN, txtUserID.Text.Trim(), strType, strCardType, strMakeUpOther, strMainCard4E, ref strMsg, ref strCardTmpID, ref strCardOpenDay, ref strCardCloseDay))
                    {
                        //* 儲存卡片資料臨時表失敗
                        etClientMstType = eClientMstType.Select;
                        base.strClientMsg = MessageHelper.GetMessage(strMsg);
                        //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.HostMsgShowByMessage(strHTGMsg) + BaseHelper.ClientMsgShow(strMsgID));
                        return;
                    }
                }
            }
            else
            {
                htInput.Clear();
                htInput.Add("FUNCTION_CODE", "1");//*ID查詢
                htInput.Add("ACCT_NBR", strUserID);
                htInput.Add("LINE_CNT", "0000");//*LINE_CNT
                dtblJCEH = MainFrameInfo.GetMainframeDataByPage("P4_JCEH", htInput, false, "1", eAgentInfo);
                if (dtblJCEH.Rows[0]["HtgErrMsg"].ToString() != "") //*主機返回失敗
                {
                    if (dtblJCEH.Rows[0]["MESSAGE_TYPE"].ToString() == "8888")
                    {
                        base.strClientMsg = MessageHelper.GetMessage("04_01010100_009");
                    }
                    else
                    {
                        base.strClientMsg = MessageHelper.GetMessage("04_00000000_008");
                    }
                    base.strHostMsg = dtblJCEH.Rows[0]["HtgErrMsg"].ToString();
                    return;
                }
                else //*主機返回正確
                {
                    base.strClientMsg = MessageHelper.GetMessage("04_00000000_034");
                    base.strHostMsg = dtblJCEH.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_034");

                    //插入到Pay_Card_Temp表.並且得到strOpenDay和strCloseDay
                    if (!BRPay_Card_Temp.InsertPayCardTempDataByNormal(dtblJCEH,txtUserID.Text.Trim(), strType, strCardType, strMakeUpOther, strMainCard4E, ref strMsg, ref strCardTmpID, ref strCardOpenDay, ref strCardCloseDay))
                    {
                        etClientMstType = eClientMstType.Select;
                        base.strClientMsg = MessageHelper.GetMessage(strMsg);
                        //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.HostMsgShowByMessage(strHTGMsg) + BaseHelper.ClientMsgShow(strMsgID));
                        return;
                    }
                }
            }

            using (OMTransactionScope ts = new OMTransactionScope())
            {
                SqlHelper SqlCertify = new SqlHelper();
                SqlCertify.AddCondition(EntityPay_Certify.M_serialNo, Operator.Equal, DataTypeUtils.String, strSerialNo);
                EntityPay_Certify ePayCertify = new EntityPay_Certify();
                ePayCertify.serialNo = strSerialNo;

                SqlHelper SqlCard = new SqlHelper();
                SqlCard.AddCondition(EntityPay_Card.M_SERIALNO, Operator.Equal, DataTypeUtils.String, strSerialNo);
                EntityPay_Card ePayCard = new EntityPay_Card();
                ePayCard.SERIALNO = strSerialNo;

                //* DELETE Pay_Certify && Pay_Card中該SerialNo下的所有資料
                if (BRPay_Certify.DeleteEntityByCondition(ePayCertify, SqlCertify.GetFilterCondition()))
                {
                    if (BRPay_Card.DeleteEntityByCondition(ePayCard, SqlCard.GetFilterCondition()))
                    {
                        //將PayCardSV存入到PayCard表

                        if (BRPay_Card.InsertPayCardData(strSerialNo, strCardTmpID, "",ref strMsg))
                        {
                            #region UpdatePay Certify
                            ePayCertify.userID = txtUserID.Text.Trim();  //* 客戶ID
                            ePayCertify.userName = txtUserName.Text.Trim();  //* 客戶姓名
                            ePayCertify.zip = txtZip.Text.Trim();  //* 郵遞區號
                            ePayCertify.add1 = CustAdd1.strAddress;  //* 地址一(1) 下拉選單的文字+ 地址一(2)下拉選單的文字
                            ePayCertify.add2 = txtAdd2.Text.Trim();  //* 地址二
                            ePayCertify.add3 = txtAdd3.Text.Trim();  //* 地址三 
                            ePayCertify.mailDay = dpMailDay.Text.Trim().Replace("/", "");  //* 寄送/自取日期
                            ePayCertify.mailNo = txtMailNo.Text;  //* 掛號號碼
                            ePayCertify.note = txtNote.Text;  //* 備註
                            ePayCertify.consignee = txtRecievName.Text;  //* 收件人姓名
                            ePayCertify.IsFree = strIsFree;     //* 是否免收
                            ePayCertify.isMail = strIsMail;  //* 是否郵寄
                            ePayCertify.getFee = strGetFee;  //* 是否補寄
                            ePayCertify.@void = "";
                            ePayCertify.showExtra = strShowExtra;  //* 是否顯示副卡
                            ePayCertify.enddate = dpEndDate.Text.Trim().Replace("/", "");  //* 最近繳款日
                            if (dpMakeUpDate.Text != "")
                            {
                                ePayCertify.makeUp = "Y";
                                ePayCertify.makeUpDate = dpMakeUpDate.Text.Trim().Replace("/", "");
                            }
                            else
                            {
                                ePayCertify.makeUp = "N";
                                ePayCertify.makeUpDate = string.Empty;
                            }
                            ePayCertify.mailMethod = dropMailType.SelectedItem.Text;  //* 郵寄方式
                            ePayCertify.payName = txtPayUserName.Text.Trim();  //* 代償人姓名
                            if (txtArrearsCount.Text != "")
                            { ePayCertify.owe = Convert.ToInt32(txtArrearsCount.Text.Trim()); } //* 欠款總額
                            if (txtRepayNum.Text != "")
                            { ePayCertify.pay = Convert.ToInt32(txtRepayNum.Text.Trim()); } //* 代償金額
                            if (dpRepayDate.Text != "")
                            {
                                //* 代償日期
                                ePayCertify.payDay = dpRepayDate.Text.Trim().Replace("/", "");
                            }
                            else
                            {
                                ePayCertify.payDay = "";
                            }

                            ePayCertify.cardType = ViewState["CardType"].ToString();  //* 卡片類型
                            ePayCertify.type = ViewState["Type"].ToString(); ;  //* 卡片類型
                            ePayCertify.mainCard4E = ViewState["mainCard4E"].ToString(); 
                            ePayCertify.MakeUpOther = strMakeUpOther;
                            ePayCertify.keyinDay = ViewState["KeyinDay"].ToString();
                            ePayCertify.openDay = ViewState["OpenDay"].ToString();
                            ePayCertify.closeDay = ViewState["CloseDay"].ToString(); 

                            if (ePayCertify.DB_InsertEntity())
                            {
                                string strTransAction = "";

                                if (strType == "G")
                                {
                                    strTransAction = BaseHelper.GetShowText("04_01010200_045");
                                }
                                else if (strType == "T")
                                {
                                    strTransAction = BaseHelper.GetShowText("04_01010200_046");
                                }
                                else if (strType == "C")
                                {
                                    strTransAction = BaseHelper.GetShowText("04_01010200_047");
                                }
                                else if (strType == "M")
                                {
                                    strTransAction = BaseHelper.GetShowText("04_01010200_048");
                                }
                                else if (strType == "O")
                                {
                                    strTransAction = BaseHelper.GetShowText("04_01010200_049");
                                }
                                else if (strType == "B")
                                {
                                    strTransAction = BaseHelper.GetShowText("04_01010200_050");
                                }
                                else if (strType == "N")
                                {
                                    strTransAction = BaseHelper.GetShowText("04_01010200_051");
                                }

                                EntitySystem_log eSystemlog = new EntitySystem_log();

                                eSystemlog.Log_date = DateTime.Now.ToString("yyyyMMdd");
                                eSystemlog.Log_Time = DateTime.Now.ToString("hhmm");
                                eSystemlog.CustomerID = ePayCertify.userID;
                                eSystemlog.Log_Action = strTransAction;
                                eSystemlog.Serial_No = strSerialNo;
                                eSystemlog.User_Name = ((EntityAGENT_INFO)this.Session["Agent"]).agent_name;
                                eSystemlog.User_ID = ((EntityAGENT_INFO)this.Session["Agent"]).agent_id;
                                eSystemlog.System_Type = "Certify";

                                if (eSystemlog.DB_InsertEntity())
                                {
                                    strMsg = "04_01010200_019";
                                    ts.Complete();
                                }
                                else
                                {
                                    strMsg = "04_01010200_023";
                                }
                            }
                            else
                            {
                                strMsg = "04_01010200_018";
                            }
                            #endregion
                        }
                        else
                        {
                            strMsg = "04_01010200_020";
                        }
                    }
                    else
                    {
                        strMsg = "04_01010200_021";

                    }
                }
                else
                {
                    strMsg = "04_01010200_022";
                }
            }
            //SetPageControl();
            etClientMstType = eClientMstType.Select;
            base.strClientMsg = MessageHelper.GetMessage(strMsg);
            //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow(strMsg));
        }
        catch (Exception ex)
        {
            //jsBuilder.RegScript(this.UpdatePanel1,  BaseHelper.ClientMsgShow("04_01010200_018"));
            etClientMstType = eClientMstType.Select;
            base.strClientMsg = MessageHelper.GetMessage("04_01010200_018");
            Framework.Common.Logging.Logging.Log(ex, Framework.Common.Logging.LogLayer.UI);
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
        #region 檢查畫面欄位輸入是否完整
        //* 客戶姓名必須輸入
        if (string.IsNullOrEmpty(txtUserName.Text.Trim()))
        {
            strMsgID = "04_01010200_002";
            txtUserName.Focus();
            return false;
        }

        //* 郵遞區號必須輸入
        if (string.IsNullOrEmpty(txtZip.Text.Trim()))
        {
            strMsgID = "04_01010200_004";
            txtZip.Focus();
            return false;
        }

        //* 地址二必須輸入
        if (string.IsNullOrEmpty(txtAdd2.Text.Trim()))
        {
            strMsgID = "04_01010200_006";
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

        //* 郵寄/自取日期必須輸入
        if (string.IsNullOrEmpty(dpMailDay.Text.Trim()))
        {
            strMsgID = "04_01010200_007";
            dpMailDay.Focus();
            return false;
        }

        //* 取得方式為'郵寄'時
        if (this.radlGetType.Items[0].Selected)
        {
            //* 郵寄方式沒有選擇，提示‘證明取得方式為郵寄，郵寄方式欄位必須輸入’
            if (dropMailType.SelectedValue == "")
            {
                strMsgID = "04_01010200_008";
                dropMailType.Focus();
                return false;
            }

            //* 如果郵寄方式為掛號的話則此欄位為 必輸入欄位請輸入掛號號碼
            if (dropMailType.SelectedItem.Text == BaseHelper.GetShowText("04_01010200_043"))
            {
                //* 掛號號碼為空時，
                if (string.IsNullOrEmpty(txtMailNo.Text.Trim()))
                {
                    strMsgID = "04_01010200_009";
                    txtMailNo.Focus();
                    return false;
                }
            }
        }
        if (!string.IsNullOrEmpty(strType))
        {
            #region 若該筆為代償 則為必輸入欄位
            switch (strType)
            {
                case "G":
                case "T":
                case "B":
                    //* 代償人姓名
                    if (string.IsNullOrEmpty(txtPayUserName.Text.Trim()))
                    {
                        strMsgID = "04_01010200_003";
                        txtPayUserName.Focus();
                        return false;
                    }

                    //* 欠款總額必須輸入
                    if (string.IsNullOrEmpty(txtArrearsCount.Text.Trim()))
                    {
                        strMsgID = "04_01010200_015";
                        txtArrearsCount.Focus();
                        return false;
                    }

                    //* 代償金額必須輸入
                    if (string.IsNullOrEmpty(txtRepayNum.Text.Trim()))
                    {
                        strMsgID = "04_01010200_016";
                        txtRepayNum.Focus();
                        return false;
                    }

                    //* 代償日期必須輸入
                    if (string.IsNullOrEmpty(dpRepayDate.Text.Trim()))
                    {
                        strMsgID = "04_01010200_017";
                        dpRepayDate.Focus();
                        return false;
                    }
                    break;
            }
            #endregion
        }
        return true;
        #endregion
    }
    #endregion
}
