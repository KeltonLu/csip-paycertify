//******************************************************************
//*  作    者：余洋
//*  功能說明：修改代償證明(明細)
//*  創建日期：2009/10/29
//*  修改記錄：


//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using CSIPCommonModel.BaseItem;
using CSIPPayCertify.BusinessRules;
using CSIPPayCertify.EntityLayer;
using Framework.Common.JavaScript;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.Common.Utility;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI;

public partial class Page_P040401020002 : PageBase
{
    #region Event
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {
            if (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "SerialNo"))  && (!string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "CardNo")) || !string.IsNullOrEmpty(RedirectHelper.GetDecryptString(this.Page, "UserID"))))
            {
                //* 證明編號
                string SerialNo = RedirectHelper.GetDecryptString(this.Page, "SerialNo");

                ViewState["SerialNo"] = SerialNo;
                DataTable dtblProperty = new DataTable();
                //* 呼叫共通方法，取郵寄方式
                if (!BRM_PROPERTY_CODE.GetCommonProperty("MAILMETHOD2", ref dtblProperty))
                {
                    jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_04010200_024"));
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
                    jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_04010200_025"));
                    return;
                }

                radlGetType.DataTextField = "PROPERTY_NAME";
                radlGetType.DataValueField = "PROPERTY_CODE";
                radlGetType.DataSource = dtblProperty;
                radlGetType.DataBind();
                radlGetType.SelectByValue("Y");

                BindData();
            }
            else
            {
                Response.Redirect("P040401020001.aspx");
            }

        }

    }
    /// <summary>
    /// 點選【存檔】Button時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnMark_Click(object sender, EventArgs e)
    {
        if (Save(ViewState["SerialNo"].ToString()))
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_04010200_004"));
        }
        else
        {
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_04010200_003"));
        }
    }
    /// <summary>
    /// 點選【返回上頁】Button時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("P040401020001.aspx?UserID=" + Request["UserID"].ToString() + "&CardNo=" + Request["CardNo"].ToString()); ;

    }
    /// <summary>
    /// 點選【列印】Button時的處理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        try
        {
            //* 證明編號(起)
            string strStartSerialNo = this.txtSerialNo.Text.Trim();
            //* 證明編號(迄)
            string strEndSerialNo = "";
            //* 身分證字號

            string strUserID = "";
            //* 自取日期
            string strMailDay = "";
            //* 卡號
            string strCardNo = "";

            string strMsgID = "";
            DataSet dstResult = new DataSet();
            //* 生成報表
            bool blnReport = BRReport.Report0402040002(strStartSerialNo, strEndSerialNo,
                    strUserID, strMailDay, strCardNo, ref strMsgID, ref dstResult);
            //* 生成報表成功
            if (blnReport)
            {
                if (dstResult != null && dstResult.Tables[0].Rows.Count > 0)
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
                jsBuilder.RegScript(this.form1, BaseHelper.GetScriptForWindowErrorClose());
            }

            if (dstResult != null)
            {
                string strRPTPathFile = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["ReportTemplate"] + "commute.rpt";
                string strServerPathFile = this.Server.MapPath(UtilHelper.GetAppSettings("ExportExcelFilePath").ToString());
                if (!BR_Excel_File.CreateExcelFile_Report0402040001(dstResult, strMailDay, ref strServerPathFile, ref strMsgID))
                {
                    jsBuilder.RegScript(this.Page, string.Format("AlertConfirm({{title:'{0}'}});", MessageHelper.GetMessage("04_00000000_058")));
                    return;
                }

                if (!BRReport_Data(strServerPathFile))
                    jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_00000000_003"));

            }
        }
        catch (Exception exp)
        {
            Framework.Common.Logging.Logging.Log(exp, Framework.Common.Logging.LogLayer.UI);
            jsBuilder.RegScript(this.form1, BaseHelper.ClientMsgShow("04_00000000_003") + BaseHelper.GetScriptForWindowErrorClose());
            return;
        }
    }

    /// <summary>
    /// 選擇取得方式
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void radlGetType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.radlGetType.SelectedValue == "Y")
        {
            this.txtMailNo.Enabled = true;
            this.dropMailType.Enabled = true;

        }
        else
        {
            this.txtMailNo.Enabled = false;
            this.dropMailType.Enabled = false;
            this.txtMailNo.Text = "";
            this.dropMailType.SelectedIndex = 0;
        }

    }
    #endregion
    #region Function

    /// <summary>
    /// 將查詢結果顯示到頁面上
    /// </summary>
    private void BindData()
    {
        try
        {
            BindMailType();
            EntitySet<EntitySet_Pay_Certify> esResult = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(GetCondition());
            EntitySet<EntitySet_SelfOther> esResultSelf = (EntitySet<EntitySet_SelfOther>)BRSet_SelfOther.Search(GetCondition());
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
                    dtpKeyinDay.Text = esResult.GetEntity(0).keyinDay.Substring(0, 4) + "/" + esResult.GetEntity(0).keyinDay.Substring(4, 2) + "/" + esResult.GetEntity(0).keyinDay.Substring(6, 2);
                }
                if (esResult.GetEntity(0).payDay.Length >= 8)
                {
                    dtpPayDay.Text = esResult.GetEntity(0).payDay.Substring(0, 4) + "/" + esResult.GetEntity(0).payDay.Substring(4, 2) + "/" + esResult.GetEntity(0).payDay.Substring(6, 2);
                }
                txtNote.Text = esResult.GetEntity(0).note;
                if (esResult.GetEntity(0).PayOffDate.Length >= 8)
                {
                    dtpPayOffDate.Text = esResult.GetEntity(0).PayOffDate.Substring(0, 4) + "/" + esResult.GetEntity(0).PayOffDate.Substring(4, 2) + "/" + esResult.GetEntity(0).PayOffDate.Substring(6, 2);
                }
                txtRecievName.Text = esResult.GetEntity(0).RecvName;
                txtSerialNo.Text = esResult.GetEntity(0).serialNo;
                txtZip.Text = PageBase.StringLeft(esResult.GetEntity(0).zip,3);

                if (esResult.GetEntity(0).isMail == "Y")
                {

                    this.radlGetType.SelectByValue("Y");
                    if (esResult.GetEntity(0).mailDay.Length >= 8)
                    {
                        dtpMailDay.Text = esResult.GetEntity(0).mailDay.Substring(0, 4) + "/" + esResult.GetEntity(0).mailDay.Substring(4, 2) + "/" + esResult.GetEntity(0).mailDay.Substring(6, 2);
                    }
                    txtMailNo.Text = esResult.GetEntity(0).mailNo;
                    this.dropMailType.SelectedValue = esResult.GetEntity(0).MailType;
                }
                else
                {
                    this.radlGetType.SelectByValue("N");
                    if (esResult.GetEntity(0).mailDay.Length >= 8)
                    {
                        dtpMailDay.Text = esResult.GetEntity(0).mailDay.Substring(0, 4) + "/" + esResult.GetEntity(0).mailDay.Substring(4, 2) + "/" + esResult.GetEntity(0).mailDay.Substring(6, 2);
                    }
                    txtMailNo.Text = "";
                }
                if (esResultSelf.TotalCount > 0)
                {
                    if (esResultSelf.GetEntity(0).SelfDay.Length >= 8)
                    {
                        dtpSelfDay.Text = esResultSelf.GetEntity(0).SelfDay.Substring(0, 4) + "/" + esResultSelf.GetEntity(0).SelfDay.Substring(4, 2) + "/" + esResultSelf.GetEntity(0).SelfDay.Substring(6, 2);
                    }
                    if (esResultSelf.GetEntity(0).OtherDay.Length >= 8)
                    {
                        dtpOtherDay.Text = esResultSelf.GetEntity(0).OtherDay.Substring(0, 4) + "/" + esResultSelf.GetEntity(0).OtherDay.Substring(4, 2) + "/" + esResultSelf.GetEntity(0).OtherDay.Substring(6, 2);
                    }
                    txtselfnote.Text = esResultSelf.GetEntity(0).Note;
                }
            }
        }
        catch (Exception ex)
        {
            Framework.Common.Logging.Logging.Log( ex, Framework.Common.Logging.LogLayer.UI);
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_04010200_001"));

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
            jsBuilder.RegScript(this.Page, BaseHelper.ClientMsgShow("04_04010200_001"));
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
    /// <summary>
    ///保存資料到DB
    /// </summary>
    /// <param name="strSerialNo">證明編號</param>
    /// <returns></returns>
    private bool Save(String strSerialNo)
    {
        try
        {
            string strMsgID = "";
            if (!CheckCondition(ref strMsgID))
            {
                MessageHelper.ShowMessage(this.Page, strMsgID);
                return false;
            }
            EntitySet_Pay_Certify eSetPayCertify = new EntitySet_Pay_Certify();
            EntitySet_SelfOther eSetSelfOther = new EntitySet_SelfOther();

            eSetPayCertify.zip = txtZip.Text.Trim();
            eSetPayCertify.add1 = txtAdd1.Text.Trim();
            eSetPayCertify.add2 = txtAdd2.Text.Trim();
            eSetPayCertify.add3 = txtAdd3.Text.Trim();
            eSetPayCertify.RecvName = txtRecievName.Text.Trim();
            eSetPayCertify.keyinDay = dtpKeyinDay.Text.Trim().Replace("/", "");
            eSetPayCertify.PayOffDate = dtpPayOffDate.Text.Trim().Replace("/", "");
            eSetPayCertify.note = txtNote.Text.Trim();
            eSetPayCertify.payName = txtPayName.Text.Trim();
            eSetPayCertify.pay = int.Parse(txtPay.Text.Trim());
            eSetPayCertify.payDay = dtpPayDay.Text.Trim().Replace("/", "");
            if (this.radlGetType.SelectedValue == "Y")
            {
                eSetPayCertify.isMail = "Y";
                eSetPayCertify.MailType = this.dropMailType.SelectedValue;
                eSetPayCertify.mailDay = dtpMailDay.Text.Trim().Replace("/", "");
                eSetPayCertify.mailNo = txtMailNo.Text.Trim();
            }

            if (this.radlGetType.SelectedValue == "N")
            {
                eSetPayCertify.isMail = "N";
                eSetPayCertify.MailType = "";
                eSetPayCertify.mailDay = dtpMailDay.Text.Trim().Replace("/", "");
                eSetPayCertify.mailNo = "";
            }

            eSetSelfOther.Note = txtselfnote.Text.Trim();
            eSetSelfOther.OtherDay = dtpOtherDay.Text.Trim().Replace("/", "");
            eSetSelfOther.SelfDay = dtpSelfDay.Text.Trim().Replace("/", "");
            eSetSelfOther.SerialNo = strSerialNo;
            SqlHelper Sql = new SqlHelper();
            //條件是SerialNo= SerialNo
            Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.Equal, DataTypeUtils.String, strSerialNo);
            using (OMTransactionScope ts = new OMTransactionScope())
            {
                if (!BRSet_Pay_Certify.UpDate(eSetPayCertify,Sql.GetFilterCondition()))
                {
                    return false;
                }
                if (!BRSet_SelfOther.UpDate(eSetSelfOther, Sql.GetFilterCondition()))
                {
                    return false;
                }
                ts.Complete();
                return true;
            }
        }
        catch (Exception ex)
        {
            Framework.Common.Logging.Logging.Log( ex, Framework.Common.Logging.LogLayer.UI);
            return false;
        }

    }
    /// <summary>
    /// 存檔時，檢核欄位的合法性

    /// </summary>
    /// <param name="strMsg"></param>
    /// <returns></returns>
    protected bool CheckCondition(ref string strMsgID)
    {

        //* 郵遞區號欄位檢核


        if (this.txtZip.Text.Trim() != "")
        {

            if (!Regex.IsMatch(this.txtZip.Text.Trim(), BaseHelper.GetShowText("04_00000000_001")))
            {
                strMsgID = "04_04010200_012";
                return false;
            }
        }
        else
        {
            strMsgID = "04_04010200_005";
            return false;
        }
        //* 如果郵寄方式為掛號的話則此欄位為必輸入欄位

        if (dropMailType.SelectedItem.Text == BaseHelper.GetShowText("04_04010200_029"))
        {
            if (this.txtMailNo.Text.Trim() != "")
            {

                if (!Regex.IsMatch(this.txtMailNo.Text.Trim(), BaseHelper.GetShowText("04_00000000_001")))
                {
                    strMsgID = "04_04010200_013";
                    return false;
                }
            }
            else
            {
                strMsgID = "04_04010200_011";
                return false;
            }
        }
        //* 地址一為必輸欄位

        if (txtAdd1.Text.Trim() == "")
        {
            strMsgID = "04_04010200_006";
            return false;
        }
        //* 地址二為必輸欄位
        if (txtAdd2.Text.Trim() == "")
        {
            strMsgID = "04_04010200_007";
            return false;
        }
        //* 收件人姓名為必輸欄位
        if (txtRecievName.Text.Trim() == "")
        {
            strMsgID = "04_04010200_008";
            return false;
        }
        //* 代償人姓名為必輸欄位
        if (txtPayName.Text.Trim() == "")
        {
            strMsgID = "04_04010200_019";
            return false;
        }
        //* 代償金額為必輸欄位

        if (txtPay.Text.Trim() == "")
        {
            strMsgID = "04_04010200_020";
            return false;
        }

        //* 開立日期欄位格式檢核
        if (this.dtpKeyinDay.Text.Trim() != "")
        {

            DateTime dtmOut = DateTime.Parse("1900-01-01");
            if (!Function.IsDateTime(this.dtpKeyinDay.Text.Trim(), out dtmOut))
            {
                strMsgID = "04_04010200_014";
                return false;
            }
        }
        else
        {
            strMsgID = "04_04010200_009";
            return false;
        }
        //* 郵寄/自取日期欄位格式檢核
        if (this.dtpMailDay.Text.Trim() != "")
        {

            DateTime dtmOut = DateTime.Parse("1900-01-01");
            if (!Function.IsDateTime(this.dtpMailDay.Text.Trim(), out dtmOut))
            {
                strMsgID = "04_03020100_015";
                return false;
            }
        }
        else
        {
            strMsgID = "04_04010200_010";
            return false;
        }
        //* 結清日期欄位格式檢核
        if (this.dtpPayOffDate.Text.Trim() != "")
        {

            DateTime dtmOut = DateTime.Parse("1900-01-01");
            if (!Function.IsDateTime(this.dtpPayOffDate.Text.Trim(), out dtmOut))
            {
                strMsgID = "04_03020100_016";
                return false;
            }
        }
        //* 自取日期欄位格式檢核
        if (this.dtpSelfDay.Text.Trim() != "")
        {

            DateTime dtmOut = DateTime.Parse("1900-01-01");
            if (!Function.IsDateTime(this.dtpSelfDay.Text.Trim(), out dtmOut))
            {
                strMsgID = "04_03020100_017";
                return false;
            }
        }
        //* 代領日期欄位格式檢核
        if (this.dtpOtherDay.Text.Trim() != "")
        {

            DateTime dtmOut = DateTime.Parse("1900-01-01");
            if (!Function.IsDateTime(this.dtpOtherDay.Text.Trim(), out dtmOut))
            {
                strMsgID = "04_03020100_018";
                return false;
            }
        }
        //* 代償日期欄位格式檢核
        if (this.dtpPayDay.Text.Trim() != "")
        {

            DateTime dtmOut = DateTime.Parse("1900-01-01");
            if (!Function.IsDateTime(this.dtpPayDay.Text.Trim(), out dtmOut))
            {
                strMsgID = "04_04010200_021";
                return false;
            }
        }
        else
        {
            strMsgID = "04_04010200_022";
            return false;
        }
        return true;
    }
    /// <summary>
    /// 專案代號:20210058-CSIP作業服務平台現代化II
    /// 列印修改代償證明
    /// 作    者:Ares Neal
    /// 創建時間:2022/01/03
    /// </summary>
    protected bool BRReport_Data(String strServerPathFile)
    {
        try
        {
            //* 將服務器端生成的文檔，下載到本地。
            string strYYYYMMDD = "000" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            strYYYYMMDD = strYYYYMMDD.Substring(strYYYYMMDD.Length - 8, 8);
            string strFileName = "代償證明書" + strYYYYMMDD + ".xls";

            //* 顯示提示訊息：匯出到Excel文檔資料成功
            this.Session["ServerFile"] = strServerPathFile;
            this.Session["ClientFile"] = strFileName;
            string urlString = @"window.parent.postMessage({ func: 'ClientMsgShow', data: '" + MessageHelper.GetMessage("04_00000000_057") + "' }, '*');";
            urlString += @"location.href='DownLoadFile.aspx';";
            jsBuilder.RegScript(this.Page, urlString);
        }
        catch (Exception exp)
        {
            Logging.Log(exp, LogLayer.UI);
            return false;
        }
        return true;
    }
    #endregion

}
