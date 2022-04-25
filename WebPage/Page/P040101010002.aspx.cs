//******************************************************************
//*  作    者：宋戈(Ge.Song)
//*  功能說明：新增明細頁面

//*  創建日期：2009/10/26
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
using Framework.Common.Message;
using Framework.Common.JavaScript;
using Framework.Common.Utility;
using CSIPPayCertify.EntityLayer;
using Framework.WebControls;
using System.Text.RegularExpressions;
using Framework.Data.OM.Collections;
using Framework.Data.OM;
using Framework.Data.OM.Transaction;
using CSIPCommonModel.EntityLayer;
using Framework.Common.Logging;

public partial class Page_P040101010002 : PageBase
{
    #region 宣告变数
    private EntityAGENT_INFO eAgentInfo;
    #endregion
    
    #region Event
    /// <summary>
    /// 進入頁面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (!Page.IsPostBack)
        {
            string strMsgID = "";

            //* 設置Grid標題
            ShowControlsText();

            ViewState["UserID"] = (RedirectHelper.GetDecryptString(this.Page, "UserID") + "").Trim();
            ViewState["Type"] = (RedirectHelper.GetDecryptString(this.Page, "Type") + "").Trim();
            ViewState["CardType"] = (RedirectHelper.GetDecryptString(this.Page, "CardType") + "").Trim();
            ViewState["No"] = (RedirectHelper.GetDecryptString(this.Page, "No") + "").Trim();
            ViewState["Other"] = (RedirectHelper.GetDecryptString(this.Page, "Other") + "").Trim();
            ViewState["OtherID"] = (RedirectHelper.GetDecryptString(this.Page, "OtherID") + "").Trim();
            ViewState["CardTmpID"] = (RedirectHelper.GetDecryptString(this.Page, "CardTmpID") + "").Trim();
            ViewState["CardOpenDay"] = (RedirectHelper.GetDecryptString(this.Page, "CardOpenDay") + "").Trim();
            ViewState["CardCloseDay"] = (RedirectHelper.GetDecryptString(this.Page, "CardCloseDay") + "").Trim();
            ViewState["ML2PLCardList"] = (RedirectHelper.GetDecryptString(this.Page, "ML2PLCardList") + "").Trim();

            string sML2PLCardList = ViewState["ML2PLCardList"].ToString();

            if (sML2PLCardList != "")
            {
                jsBuilder.RegScript(this.UpdatePanel1, jsBuilder.GetAlert("CARD NO : " + sML2PLCardList + " " + MessageHelper.GetMessage("04_00000000_053")));
                //MessageHelper.ShowMessage(this.UpdatePanel1, "CARD NO : " + sML2PLCardList + " " + MessageHelper.GetMessage("04_00000000_053"));
            }

            if (!CheckRedirect(ref strMsgID))
            {
                //* 出错
                MessageHelper.ShowMessageAndGoto(this.UpdatePanel1, "P040101010001.aspx", strMsgID);
                return;
            }

            //* 初始化資料
            //base.strClientMsg += "";
            //base.strHostMsg += "";
            eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合

            InitNewData();
            
        }

        base.strClientMsg += "";
        base.strHostMsg += "";
        eAgentInfo = (EntityAGENT_INFO)this.Session["Agent"]; //*Session變數集合
    }
    
    /// <summary>
    /// 當地址一下拉選單改變時,同時改變郵遞區號

    /// </summary>
    protected void CustAdd1_ChangeValues()
    {
        this.txtZip.Text = PageBase.StringLeft(this.CustAdd1.strZip,3);
    }

    /// <summary>
    /// 如果[取得方式]選擇非郵寄,則[郵寄方式]和[掛號號碼]欄位不可用

    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void radlGetType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.radlGetType.SelectedValue.ToString().Trim() == "Y")
        {
            //* 取得方式 = 郵寄
            this.dropMailType.Enabled = true;
            this.txtMailNo.Enabled = true;
        }
        else
        {
            //* 自取
            this.dropMailType.Enabled = false;
            this.txtMailNo.Enabled = false;
        }

    }

    /// <summary>
    /// 點擊[儲存]按鈕,儲存(新增)資料
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSave_Click(object sender, EventArgs e)
    {
        string strMsgID = "";     //* 錯誤訊息ID
        if (!CheckCondition(ref strMsgID))
        {
            //* 檢核不成功時，提示不符訊息

            MessageHelper.ShowMessage(this.UpdatePanel1, strMsgID);
            return;
        }

        if (SaveCertify())
        {
            //MessageHelper.ShowMessageAndGoto(this.UpdatePanel1, "P040101010001.aspx", "04_01010100_005");
            this.btnSave.Enabled = false;
        }
    }

    /// <summary>
    /// 點擊[返回]按鈕,返回到前一畫面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("P040101010001.aspx");
    }

    /// <summary>
    /// 點擊[發查長姓名]按鈕
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        Hashtable htInput = new Hashtable();
        DataTable dtblOutput = new DataTable();

        htInput.Add("TRAN_ID", "JC99");
        htInput.Add("PROGRAM_ID", "JCGU399");
        htInput.Add("FUNCTION_CODE", "I");
        htInput.Add("IDNO_NO", this.txtCustID.Text.Trim());// 客戶ID/身分證字號

        dtblOutput = MainFrameInfo.GetMainframeDataByPage("P4_JC99", htInput, false, "1", eAgentInfo);

        switch (dtblOutput.Rows[0]["MESSAGE_TYPE"].ToString())
        {
            case "0001":
                break;
            case "8888":
                break;
            case "9999":
                break;
        }

        BRPay_Certify.Update()
    }

    #endregion

    #region Function
    /// <summary>
    /// 初始化元件

    /// </summary>
    private void ShowControlsText()
    {
        DataTable dtblProperty = new DataTable();

        //* 取得方式
        if (BRM_PROPERTY_CODE.GetCommonProperty("ISMAIL",ref dtblProperty))
        {
            this.radlGetType.DataTextField = "PROPERTY_NAME";
            this.radlGetType.DataValueField = "PROPERTY_CODE";
            this.radlGetType.DataSource = dtblProperty;
            this.radlGetType.DataBind();
            this.radlGetType.SelectByValue("Y");
        }

                
        //* 郵寄方式
        if (BRM_PROPERTY_CODE.GetCommonProperty("MAILMETHOD", ref dtblProperty))
        {
            if (dtblProperty.Rows.Count > 0)
            {
                this.dropMailType.DataTextField = "PROPERTY_NAME";
                this.dropMailType.DataValueField = "PROPERTY_CODE";
                this.dropMailType.DataSource = dtblProperty;
                this.dropMailType.DataBind();
            }
        } 
    }

    /// <summary>
    /// 檢查跳轉需要輸入的欄位是否有輸入

    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    private bool CheckRedirect(ref string strMsgID)
    {
        string strUserID = ViewState["UserID"].ToString().Trim();
        string strType = ViewState["Type"].ToString().Trim();
        string strCardType = ViewState["CardType"].ToString().Trim();
        string strNo = ViewState["No"].ToString().Trim();
        string strOther = ViewState["Other"].ToString().Trim();
        string strOtherID = ViewState["OtherID"].ToString().Trim();
        string strCardTmpID = ViewState["CardTmpID"].ToString().Trim();
        string strCardOpenDay = ViewState["CardOpenDay"].ToString().Trim();
        string strCardCloseDay = ViewState["CardCloseDay"].ToString().Trim();

        if (String.IsNullOrEmpty(strUserID.Trim()))
        {
            //* "請輸入客戶ID";
            strMsgID = "04_01010100_000";
            return false;
        }
        else
        {
            //* 如果身分證不是合法的格式
            if (!Regex.IsMatch(strUserID.Trim(), BaseHelper.GetShowText("04_00000000_000")))
            {
                strMsgID = "04_01010100_001";
                return false;
            }
        }

        if (String.IsNullOrEmpty(strType.Trim()))
        {
            //"請選擇開立種類";
            strMsgID = "04_01010100_002";
            return false;
        }
        else
        {
            if (strType.Trim() != "C" &&
                strType.Trim() != "M" &&
                strType.Trim() != "G" &&
                strType.Trim() != "O" &&
                strType.Trim() != "B" &&
                strType.Trim() != "N" &&
                strType.Trim() != "T")
            {
                //"請選擇開立種類";
                strMsgID = "04_01010100_002";
                return false;
            }
        }

        if (String.IsNullOrEmpty(strNo.Trim()))
        {
            //"請選擇開立種類";
            strMsgID = "04_01010100_002";
            return false;
        }
        else
        {
            if (strNo.Trim() != "1" &&
                strNo.Trim() != "2" &&
                strNo.Trim() != "3" &&
                strNo.Trim() != "4")
            {
                //"請選擇開立種類";
                strMsgID = "04_01010100_002";
                return false;
            }
        }

        if (String.IsNullOrEmpty(strCardType.Trim()))
        {
            //"請選擇開立種類";
            strMsgID = "04_01010100_002";
            return false;
        }
        else
        {
            if (strCardType.Trim() != "0" &&
                strCardType.Trim() != "1" &&
                strCardType.Trim() != "2" &&
                strCardType.Trim() != "3")
            {
                //"請選擇開立類型";
                strMsgID = "04_01010100_003";
                return false;
            }
        }

        if (strCardType.Trim() == "3" && String.IsNullOrEmpty(strOtherID))
        {
            //"請輸入 他人附卡其正卡人ID!";
            strMsgID = "04_01010100_004";
            return false;
        }

        if (String.IsNullOrEmpty(strCardTmpID.Trim()))
        {
            //* CardTempID不能爲空
            strMsgID = "04_01010100_018";
            return false;
        }
        if (String.IsNullOrEmpty(strCardOpenDay.Trim()))
        {
            //* CardOpenDay不能爲空
            strMsgID = "04_01010100_019";
            return false;
        }
        if (String.IsNullOrEmpty(strCardCloseDay.Trim()))
        {
            //* CardCloseDay不能爲空
            strMsgID = "04_01010100_020";
            return false;
        }

        return true;
    }

    /// <summary>
    /// 根據主機資料和歷史資料給部分欄位設定預設值

    /// </summary>
    /// <returns></returns>
    private bool InitNewData()
    {
        #region 宣告變數
        string strMsg = "";
        string strUserName = "";        //* 客戶姓名
        string strAdd1 = "";            //* 地址1
        string strAdd2 = "";            //* 地址2
        string strAdd3 = "";            //* 地址3
        string strConsignee = "";       //* 收件人

        string strMailNo = "";          //* 快遞號碼
        string strNote = "";            //* 說明
        //edit by mel 2014/08/05 start
        string dblcurflag ="";      //雙幣卡註記
        //edit by mel 2014/08/05 end
        bool bMCode = false;            //* 是否Macro
        string strMacroLastPayDay = ""; //* Macro中最後繳款日
//        string strOpenDay = "";         //* 最早開卡日
        string strEndDay = "";        //* 最後繳款日
        string strAddress = "";         //* 地址
        EntitySet<EntityPay_Certify> esCertify = new EntitySet<EntityPay_Certify>();
        DataTable dtblMain = new DataTable();
        Hashtable htInput = new Hashtable();

        string strUserID = ViewState["UserID"].ToString().Trim();
        string strType = ViewState["Type"].ToString().Trim();
        string strCardType = ViewState["CardType"].ToString().Trim();
        string strNo = ViewState["No"].ToString().Trim();
        string strOther = ViewState["Other"].ToString().Trim();
        string strOtherID = ViewState["OtherID"].ToString().Trim();
        string strCardTmpID = ViewState["CardTmpID"].ToString().Trim();
        string strCardOpenDay = ViewState["CardOpenDay"].ToString().Trim();
        string strCardCloseDay = ViewState["CardCloseDay"].ToString().Trim();

        #endregion

        if (strNo.Trim() == "O" || strNo.Trim() == "M")
        {
            //* 是GCB(strType證明種類 = O或者M)
            #region GCB开立

            htInput.Clear();

            htInput.Add("FUNCTION_CODE", "1");//*ID查詢
            htInput.Add("IDNO", strUserID);
            htInput.Add("LINE_CNT", "0000");//*LINE_CNT
            htInput.Add("LAST_KEY", "");

            dtblMain = MainFrameInfo.GetMainframeDataByPage("P4_JCHN", htInput, false, "1", eAgentInfo);
            if (dtblMain.Rows[0]["HtgErrMsg"].ToString() != "") //*主機返回失敗
            {
                etClientMstType = eClientMstType.Select;
                if (dtblMain.Rows[0]["MESSAGE_TYPE"].ToString() == "8888")
                {
                    base.strClientMsg = MessageHelper.GetMessage("04_01010100_009");
                }
                else
                {
                    base.strClientMsg = MessageHelper.GetMessage("04_00000000_037");
                }
                base.strHostMsg = dtblMain.Rows[0]["HtgErrMsg"].ToString();
                return false;
            }
            else //*主機返回正確
            {
                base.strClientMsg = MessageHelper.GetMessage("04_00000000_036");
                base.strHostMsg = dtblMain.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_036");
                
                strUserName = dtblMain.Rows[0]["NAME"].ToString().Trim();
                //*地址 = 帳址去掉前後空格和全形空格

                strAddress = dtblMain.Rows[0]["ADDR1"].ToString().Trim().Replace("　", "");
                if (strAddress.Length < 6)
                {   
                    //* 地址不足6碼

                    strAdd1 = strAddress.PadRight(6, Convert.ToChar(" ")).Substring(0, 6);
                    strAdd2 = "";
                    strAdd3 = "";
                }
                else
                {
                    //* 地址1
                    strAdd1 = strAddress.Substring(0, 6);

                    strAddress = strAddress.Substring(6);
                    if (strAddress.Length <= 14)
                    {
                        strAdd2 = strAddress;
                        strAdd3 = "";
                    }
                    else
                    {
                        strAdd2 = strAddress.Substring(0, 14);
                        strAdd3 = strAddress.Substring(14, 14);
                    }
                }
                

                bMCode = false;
                for (int i = 0; i <= dtblMain.Rows.Count - 1; i++)
                {
                    if (dtblMain.Rows[i]["STAS"].ToString() == "3" ||
                        dtblMain.Rows[i]["STAS"].ToString() == "4" ||
                        dtblMain.Rows[i]["STAS"].ToString() == "5")
                    {
                        //* 如果<1350電文.歸戶現狀>=3或者=4或者=5
                        if (Convert.ToInt32(dtblMain.Rows[i]["BALAMT"].ToString().Trim()) <= 0)
                        {
                            //* 如果<1350電文.流水帳金額> 小於等於0
                            //* strEndDate = <1350電文.最近繳款日>
                            strEndDay = BRPay_Card_Temp.ConvertLPAYD(dtblMain.Rows[i]["LPAYD"].ToString());                            
                        }
                        else
                        {
                            bMCode = true;
                        }
                    }
                }
                string strLastPayDate = BRPay_Card_Temp.GetLasyPayDate(strCardTmpID, strType);
                if (bMCode == true)
                {
                    if (!BRPay_Macro.GetLastPayDate(strUserID.Trim(), strType, ref strMsg, ref strMacroLastPayDay))
                    {
                        //* 出错拉.
                        //* 添加LOG
                        Logging.Log("查詢Macro最後日期出錯:" + strMacroLastPayDay, LogState.Info, LogLayer.UI);
                        //* LOG END
                    }
                    if (String.IsNullOrEmpty(strMacroLastPayDay))
                    {
                        //* 取不到MACRO最后付款日则拿卡片中最后的日期为准
                        strEndDay = strLastPayDate;                        
                    }
                    else if (String.IsNullOrEmpty(strLastPayDate))
                    {
                        //* 取不到MACRO最后付款日则拿卡片中最后的日期为准
                        strEndDay = strMacroLastPayDay;                        
                    }
                    else
                    {
                        //* 如果MCode = True && 取到了Macro表(催收)的最后缴款日,则以催收的最后缴款日为准
                        strEndDay = strMacroLastPayDay;                        
                    }
                    //* 添加LOG
                    Logging.Log("是否查詢Macro:" + bMCode.ToString() + ",主機最後繳款日:" + strLastPayDate + ",Macro最後日期:" + strMacroLastPayDay, LogState.Info, LogLayer.UI);
                    //* LOG END

                }
                else
                {
                    strEndDay = strLastPayDate;                    
                }

            }
            #endregion
        }
        else
        {
            //* 开始一般开立

            #region 一般开立
            htInput.Clear();
            htInput.Add("FUNCTION_CODE", "1");//*ID查詢
            htInput.Add("ACCT_NBR", strUserID);
            htInput.Add("LINE_CNT", "0000");//*LINE_CNT
            dtblMain = MainFrameInfo.GetMainframeDataByPage("P4_JCEH", htInput, false, "1", eAgentInfo);
            if (dtblMain.Rows[0]["HtgErrMsg"].ToString() != "") //*主機返回失敗
            {
                if (dtblMain.Rows[0]["MESSAGE_TYPE"].ToString() == "8888")
                {
                    base.strClientMsg = MessageHelper.GetMessage("04_01010100_009");
                }
                else
                {
                    base.strClientMsg = MessageHelper.GetMessage("04_00000000_008");
                }
                base.strHostMsg = dtblMain.Rows[0]["HtgErrMsg"].ToString();
                return false;
            }
            else //*主機返回正確
            {
                base.strClientMsg = MessageHelper.GetMessage("04_00000000_034");
                base.strHostMsg = dtblMain.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_034");

                strUserName = dtblMain.Rows[0]["SHORT_NAME"].ToString().Trim();
                strAdd1 = dtblMain.Rows[0]["CITY"].ToString().Trim().PadRight(6, Convert.ToChar(" ")).Substring(0, 6);
                strAdd2 = dtblMain.Rows[0]["ADDR_1"].ToString().Trim();
                strAdd3 = dtblMain.Rows[0]["ADDR_2"].ToString().Trim();
                //* 设置地址一两个下拉选单,和邮编                
                this.CustAdd1.InitalAdd1(strAdd1);
                
                //edit by mel 2014/08/05 start 
                dblcurflag =dtblMain.Rows[0]["FILLER05"].ToString().Trim() ;
                //edit by mel 2014/08/05 end


                //* 最後繳款日
                for (int i = 0; i <= dtblMain.Rows.Count - 1; i++)
                {
                    if (dtblMain.Rows[i]["BLOCK"].ToString().Trim() == "M" || (dtblMain.Rows[i]["BLOCK"].ToString().Trim() == "Z" && dtblMain.Rows[i]["ALT_BLOCK"].ToString().Trim() == "M"))
                    {
                        //* 如果<1331電文.狀況碼> = M 或 (<1331電文.狀況碼>=Z AND <1331電文.前況碼>=M)
                        if (IsSameMan(strUserID.Trim(), dtblMain.Rows[i]["ACCT_CUST_ID"].ToString().Trim()))
                        {
                            //* 如果开立的ID是归户ID
                            bMCode = true;
                        }
                    }
                }

                //* 取得最後繳款日
                string strLastPayDate = BRPay_Card_Temp.GetLasyPayDate(strCardTmpID, strType);
                if (string.IsNullOrEmpty(strLastPayDate))
                {
                    //* 如果最后繳款日 = 空 則取停卡日

                    strLastPayDate = strCardCloseDay;
                }

                if (bMCode == true)
                {
                    if (!BRPay_Macro.GetLastPayDate(strUserID.Trim(), strType, ref strMsg, ref strMacroLastPayDay))
                    {
                        //* 出错拉.

                        //* 添加LOG
                        Logging.Log("查詢Macro最後日期出錯:" + strMacroLastPayDay, LogState.Info, LogLayer.UI);
                        //* LOG END

                    }
                    if (String.IsNullOrEmpty(strMacroLastPayDay))
                    {
                        //* 取不到MACRO最后付款日则拿卡片中最后的日期为准
                        strEndDay = strLastPayDate;
                    }
                    else if (string.IsNullOrEmpty(strLastPayDate))
                    {
                        //* 取得到Macro取不到主機的.以MACRO爲準
                        strEndDay = strMacroLastPayDay;
                    }
                    else
                    {
                        //* 如果MCode = True && 取到了Macro表(催收)的最后缴款日,则以催收的最后缴款日为准
                        //strEndDay = strMacroLastPayDay;

                        //* 20100322 文平:新邏輯.MACRO和主機最後繳款日比大.取大著

                        if (Convert.ToInt32(strMacroLastPayDay) > Convert.ToInt32(strLastPayDate))
                        {
                            strEndDay = strMacroLastPayDay;
                        }
                        else
                        {
                            strEndDay = strLastPayDate;
                        }
                    }
                    //* 添加LOG
                    Logging.Log("是否查詢Macro:" + bMCode.ToString() + ",主機最後繳款日:" + strLastPayDate + ",Macro最後日期:" + strMacroLastPayDay, LogState.Info, LogLayer.UI);
                    //* LOG END
                }
                else
                {
                    strEndDay = strLastPayDate;
                    Logging.Log("是否查詢Macro:" + bMCode.ToString() + ",主機最後繳款日:" + strLastPayDate + ",Macro最後日期:" + strMacroLastPayDay, LogState.Info, LogLayer.UI);
                }
            }
            #endregion
        }

        #region 取得以前的資料以填充收件人,MailNo,Note
        if (BRPay_Certify.GetDetailsByUserID(strUserID, false,ref strMsg,ref esCertify))
        {
            if (esCertify.Count > 0)
            {
                strConsignee = esCertify.GetEntity(0).consignee.ToString().Trim();
                strMailNo = esCertify.GetEntity(0).mailNo.ToString().Trim();
                strNote = esCertify.GetEntity(0).note.ToString().Trim();
            }
        }

        if (string.IsNullOrEmpty(strConsignee)) { strConsignee = strUserName.Trim(); }
        #endregion

        #region 給頁面元件賦初值

        //* 如果是代償啟用元件

        switch (strType)
        {
            case "C":                
            case "M":
            case "N":
            case "O":
                //* [清償證明-卡]=C;[清償證明-ML&卡]=M;[GCB清償證明-卡]=0;[清償證明-ML Only]=N;
                this.txtPayName.Enabled = false;
                this.txtOwe.Enabled = false;
                this.txtPay.Enabled = false;
                this.dpPayDay.Enable = false;
                // 20220425 RQ-2022-003727-000_開立清償證明系統-長姓名專案 By Kelton
                this.txtCustName_L.Enabled = false;
                this.txtCustName_Rome.Enabled = false;
                break;
            case "G":
            case "T":            
            case "B":
                //* [代償證明-卡]=G;[GCB代償證明]=B;[代償證明-ML]=T
                this.txtPayName.Enabled = true;
                this.txtOwe.Enabled = true;
                this.txtPay.Enabled = true;
                this.dpPayDay.Enable = true;
                this.dpPayDay.Text = DateTime.Today.ToString("yyyy/MM/dd"); //*代償日期
                break;
        }
        
        //*初始化元件

        this.txtCustID.Text = strUserID.Trim();
        this.txtCustName.Text = strUserName.Trim();
        this.txtAdd2.Text = strAdd2;
        this.txtAdd3.Text = strAdd3;
        if (strEndDay.Trim().Length == 8)
        {
            this.dpEndDate.Text = strEndDay.Trim().Substring(0,4) + @"/" + strEndDay.Trim().Substring(4,2) + @"/" + strEndDay.Trim().Substring(6,2);
        }
        this.txtRecievName.Text = strConsignee;
        this.txtMailNo.Text = strMailNo;
        this.txtNote.Text = strNote;


        //edit by mel 2014/08/05 start 雙幣卡需求-判斷若FILLER05值為"*"則於備註欄位加入"此為雙幣卡客戶"
        if(dblcurflag=="*")
        {
            this.txtNote.Text += "-此為雙幣卡客戶";
        }
        //edit by mel 2014/08/05 end

        this.dpMailDay.Text = DateTime.Today.AddDays(2).ToString("yyyy/MM/dd");
        #endregion

        return true;
    }

    /// <summary>
    /// 比较2个ID是否是同一人

    /// </summary>
    /// <param name="strCardID">ID1</param>
    /// <param name="strCardID2">ID2</param>
    /// <returns></returns>
    private bool IsSameMan(string strCardID, string strCardID2)
    {
        bool bResult = false;
        if (strCardID2.Trim().Length == 11)
        {
            if (strCardID.Trim() == strCardID2.Substring(1))
            {
                bResult = true;
            }
        }
        else
        {
            if (strCardID.Trim() == strCardID2.Trim())
            {
                bResult = true;
            }
        }
        return bResult;
    }

    /// <summary>
    /// 檢查輸入欄位是否正確
    /// </summary>
    /// <param name="strMsgID"></param>
    /// <returns></returns>
    private bool CheckCondition(ref string strMsgID)
    {
        //* 客戶ID必須輸入
        if (string.IsNullOrEmpty(this.txtCustID.Text))
        {
            strMsgID = "04_01010200_000";
            this.txtCustID.Focus();
            return false;
        }

        //* 客戶姓名必須輸入
        if (string.IsNullOrEmpty(this.txtCustName.Text))
        {
            strMsgID = "04_01010200_002";
            this.txtCustName.Focus();
            return false;
        }

        //* 郵遞區號必須輸入

        if (string.IsNullOrEmpty(this.txtZip.Text))
        {
            strMsgID = "04_01010200_004";
            this.txtZip.Focus();
            return false;
        }

        //* 地址一必須輸入
        if (string.IsNullOrEmpty(this.CustAdd1.strAddress))
        {
            strMsgID = "04_01010200_005";
            return false;
        }


        //* 地址二必須輸入

        if (string.IsNullOrEmpty(this.txtAdd2.Text))
        {
            strMsgID = "04_01010200_006";
            this.txtAdd2.Focus();
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

        if (string.IsNullOrEmpty(this.txtRecievName.Text))
        {
            strMsgID = "04_01010100_029";
            this.txtRecievName.Focus();
            return false;
        }

        //* 郵寄/自取日期必須輸入
        if (string.IsNullOrEmpty(this.dpMailDay.Text))
        {
            strMsgID = "04_01010200_007";
            this.dpMailDay.Focus();
            return false;
        }

        //* 備註 最多只能輸入50個字符

        if (this.txtNote.Text.Trim().Length > 50)
        {
            strMsgID = "04_01010100_028";
            this.txtNote.Focus();
            return false;
        }

        //* 取得方式為'郵寄'時

        if (this.radlGetType.SelectedValue.ToString() == "Y")
        {
            //* 郵寄方式沒有選擇，提示‘證明取得方式為郵寄，郵寄方式欄位必須輸入’

            if (string.IsNullOrEmpty(dropMailType.SelectedValue))
            {
                strMsgID = "04_01010200_008";
                dropMailType.Focus();
                return false;
            }

            //* 如果郵寄方式為掛號的話則此欄位為 必輸入欄位請輸入掛號號碼
            if (dropMailType.SelectedItem.Text.Equals("掛號"))
            {
                //* 掛號號碼為空時，
                if (string.IsNullOrEmpty(this.txtMailNo.Text))
                {
                    strMsgID = "04_01010200_009";
                    this.txtMailNo.Focus();
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 儲存
    /// </summary>
    /// <returns></returns>
    private bool SaveCertify()
    {
        try
        {
            string strMsg = "";
            string strIsMail;       //* 取得方式        Y-郵寄, N-自取
            string strShowExtra;    //* 顯示附卡信息    Y-顯示; N-不顯示

            string strGetFee;       //* 是否補寄        Y-是;   N-不是
            string strVoid;         //* 
            string strIsFree;       //* 是否免收        Y-是;   N-不是
            string strSerialNo = "";     //*
            string strCurrentNo = "";    //* 
            
            EntityPay_Certify ePayCertify;

            strVoid = "";
            //* 取得方式
            strIsMail = this.radlGetType.SelectedValue.ToString();

            //* 顯示附卡信息
            if (this.chkShowExtra.Checked == true)
            {
                strShowExtra = "Y";
            }
            else
            {
                strShowExtra = "N";
            }

            //* 是否補寄
            if (this.chkGetFee.Checked == true)
            {
                strGetFee = "Y";
            }
            else
            {
                strGetFee = "N";
            }

            //* 是否免收
            if (this.chkIsFree.Checked == true)
            {
                strIsFree = "Y";
            }
            else
            {
                strIsFree = "N";
            }
            

            if (!BRPay_Serial.GetSerialNo(ViewState["No"].ToString(), ref strSerialNo))
            {
                //* "取得序號失敗
                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("04_01010100_024"));

            }

            if (!BRPay_Serial.GetCurrentNo(ref strCurrentNo))
            {
                //* 取得流水號失敗

                jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("04_01010100_025"));
            }

            using (OMTransactionScope ts = new OMTransactionScope())
            {
                if (!BRPay_Card.InsertPayCardData(strSerialNo, ViewState["CardTmpID"].ToString(), ViewState["ML2PLCardList"].ToString(), ref strMsg))
                {
                    //* 插入Pay_Card表失敗

                    jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("04_01010100_026"));
                }
                else
                {
                    ePayCertify = new EntityPay_Certify();
                    ePayCertify.serialNo = strSerialNo;
                    ePayCertify.userID = this.txtCustID.Text.Trim();
                    ePayCertify.userName = this.txtCustName.Text.Trim();
                    ePayCertify.IsFree = strIsFree;
                    ePayCertify.add1 = this.CustAdd1.strAddress.Trim();
                    ePayCertify.add2 = this.txtAdd2.Text.Trim();
                    ePayCertify.add3 = this.txtAdd3.Text.Trim();
                    ePayCertify.zip = this.txtZip.Text.Trim();
                    ePayCertify.mailDay = this.dpMailDay.Text.Replace("/", "").Trim();
                    ePayCertify.mailNo = this.txtMailNo.Text.Trim();
                    ePayCertify.note = this.txtNote.Text.Trim();
                    ePayCertify.payName = this.txtPayName.Text.Trim();
                    if (!String.IsNullOrEmpty(this.txtOwe.Text.Trim()))
                    {
                        ePayCertify.owe = Convert.ToInt32(this.txtOwe.Text.Trim());
                    }
                    if (!String.IsNullOrEmpty(this.txtPay.Text.Trim()))
                    {
                        ePayCertify.pay = Convert.ToInt32(this.txtPay.Text.Trim());
                    }
                    ePayCertify.payDay = this.dpPayDay.Text.Replace("/", "").Trim();
                    ePayCertify.getFee = strGetFee.Trim();
                    ePayCertify.isMail = strIsMail.Trim();
                    ePayCertify.type = ViewState["Type"].ToString().Trim();
                    ePayCertify.cardType = ViewState["CardType"].ToString().Trim();
                    ePayCertify.@void = strVoid;
                    ePayCertify.keyinDay = DateTime.Today.ToString("yyyyMMdd");
                    ePayCertify.showExtra = strShowExtra;
                    ePayCertify.openDay = ViewState["CardOpenDay"].ToString().Trim();
                    ePayCertify.closeDay = ViewState["CardCloseDay"].ToString().Trim();
                    ePayCertify.consignee = this.txtRecievName.Text.Trim();
                    ePayCertify.enddate = this.dpEndDate.Text.Replace("/", "").Trim();
                    ePayCertify.makeUp = "N";
                    ePayCertify.makeUpDate = this.dpMakeUpDate.Text.Replace("/", "").Trim();
                    ePayCertify.mailMethod = this.dropMailType.SelectedItem.Text.Trim();
                    ePayCertify.mainCard4E = ViewState["OtherID"].ToString().Trim();
                    ePayCertify.MakeUpOther = ViewState["Other"].ToString().Trim();
                    string strType = ePayCertify.type;
                    if (BRPay_Certify.AddNewEntity(ePayCertify))
                    {
                        string strTransAction = "";

                        if (strType == "G")
                        {
                            strTransAction = BaseHelper.GetShowText("04_01010100_058");
                        }
                        else if (strType == "T")
                        {
                            strTransAction = BaseHelper.GetShowText("04_01010100_059");
                        }
                        else if (strType == "C")
                        {
                            strTransAction = BaseHelper.GetShowText("04_01010100_060");
                        }
                        else if (strType == "M")
                        {
                            strTransAction = BaseHelper.GetShowText("04_01010100_061");
                        }
                        else if (strType == "O")
                        {
                            strTransAction = BaseHelper.GetShowText("04_01010100_062");
                        }
                        else if (strType == "B")
                        {
                            strTransAction = BaseHelper.GetShowText("04_01010100_063");
                        }
                        else if (strType == "N")
                        {
                            strTransAction = BaseHelper.GetShowText("04_01010100_064");
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
                            ts.Complete();
                            etClientMstType = eClientMstType.Select;
                            base.strClientMsg = MessageHelper.GetMessage("04_01010100_005");
                            //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("04_01010100_005"));
                        }
                        else
                        {
                            etClientMstType = eClientMstType.Select;
                            base.strClientMsg = MessageHelper.GetMessage("04_01010100_030");
                            //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("04_01010100_030"));
                            return false;
                        }
                    }
                    else
                    {
                        //* 新增清償證明失敗
                        etClientMstType = eClientMstType.Select;
                        base.strClientMsg = MessageHelper.GetMessage("04_01010100_006");
                        //jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("04_01010100_006"));
                        return false;
                    }
                }
            }
        }
        catch (Exception Ex)
        {
            Logging.Log(Ex, LogLayer.UI);
            jsBuilder.RegScript(this.UpdatePanel1, BaseHelper.ClientMsgShow("04_01010100_006"));
            return false;
        }
        
        return true;
        
    }
    #endregion


}
