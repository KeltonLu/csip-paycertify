//******************************************************************
//*  作    者：余洋(rosicky)
//*  功能說明：

//*  創建日期：2009/10/27
//*  修改記錄：

//*<author>            <time>            <TaskID>                <desc>
//* Ares Stanley    2022/02/14    20210058-CSIP作業服務平台現代化II    調整webconfig取參數方式
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using CSIPCommonModel.BusinessRules;
using CSIPPayCertify.EntityLayer;
using Framework.Data.OM;
using System.Configuration;
using CSIPCommonModel.BaseItem;
using Framework.Common.Utility;

namespace CSIPPayCertify.BusinessRules
{
    public class BRWebService : BRBase<EntityPay_Macro>
    {
        /// <summary>
        /// 連接WebSerivice到催收平台獲得資料到Pay_Macro表

        /// </summary>
        /// <param name="strUserID">客戶ID</param>
        /// <param name="strMsgID">錯誤信息ID</param>
        /// <returns>True---查詢成功；False----查詢失敗</returns>
        public static bool GetMarcoDataByCustID(string strUserID, ref string strMsgID)
        {
            string strMacroLog = "";
            try
            {
                string strIsOnLine = UtilHelper.GetAppSettings("WebServiceOnLine");
                if (strIsOnLine.Trim().ToUpper() == "TRUE")
                {
                    WS_PayOff.WS_PayOff ws_payoff = new CSIPPayCertify.BusinessRules.WS_PayOff.WS_PayOff();

                    ws_payoff.Url = UtilHelper.GetAppSettings("WebserviceURL");

                    DataTable dtblPayMacro = ws_payoff.GetPayOffDataSet("1", strUserID).Tables[0];
                    EntityPay_Macro ePayMacro = new EntityPay_Macro();

                    ePayMacro.M_CUST_ID = strUserID;
                    SqlHelper sql = new SqlHelper();

                    sql.AddCondition(EntityPay_Macro.M_M_CUST_ID, Operator.Equal, DataTypeUtils.String, strUserID);

                    if (!BRPay_Macro.DeleteEntityByCondition(ePayMacro, sql.GetFilterCondition()))
                    {
                        strMsgID = "04_02010100_037";
                        return false;
                    }

                    for (int i = 0; i < dtblPayMacro.Rows.Count; i++)
                    {
                        ePayMacro.M_CUST_NAME = dtblPayMacro.Rows[i]["CustName"].ToString().Trim().Replace("　","");
                        ePayMacro.M_CARD_NO = dtblPayMacro.Rows[i]["CardNo"].ToString().Trim();
                        ePayMacro.M_BALANCE = Decimal.Parse(dtblPayMacro.Rows[i]["Balance"].ToString().Trim());
                        ePayMacro.M_DEBT_DATE = dtblPayMacro.Rows[i]["DebtDate"].ToString().Trim();
                        ePayMacro.M_DEBT_AMT = Decimal.Parse(dtblPayMacro.Rows[i]["DebtAmt"].ToString().Trim());
                        ePayMacro.M_DEBT_BALANCE = Decimal.Parse(dtblPayMacro.Rows[i]["DebtBalance"].ToString().Trim());
                        ePayMacro.M_LAST_PAY_DATE = dtblPayMacro.Rows[i]["LastPayDate"].ToString().Trim();
                        ePayMacro.M_TOTAL_PAYMENT = Decimal.Parse(dtblPayMacro.Rows[i]["TotalPayment"].ToString().Trim());
                        ePayMacro.M_CLOSE_DATE = dtblPayMacro.Rows[i]["CloseDate"].ToString().Trim();
                        ePayMacro.M_CLOSE_REASON = dtblPayMacro.Rows[i]["CloseReason"].ToString().Trim();
                        ePayMacro.M_CLOSE_REP = dtblPayMacro.Rows[i]["Rep"].ToString().Trim().Replace("　", "");

                        strMacroLog = ePayMacro.M_CUST_NAME + "|" +
                                        ePayMacro.M_CARD_NO + "|" +
                                        ePayMacro.M_BALANCE + "|" +
                                        ePayMacro.M_DEBT_DATE + "|" +
                                        ePayMacro.M_DEBT_AMT + "|" +
                                        ePayMacro.M_DEBT_BALANCE + "|" +
                                        ePayMacro.M_LAST_PAY_DATE + "|" +
                                        ePayMacro.M_TOTAL_PAYMENT + "|" +
                                        ePayMacro.M_CLOSE_DATE + "|" +
                                        ePayMacro.M_CLOSE_REASON + "|" +
                                        ePayMacro.M_CLOSE_REP + "|";
                        // JaJa暫時移除
                        // Framework.Common.Logging.Logging.SaveLog(Framework.Common.Logging.ELogLayer.BusinessRule, strMacroLog, Framework.Common.Logging.ELogType.Info);
                        if (!BRPay_Macro.Add(ePayMacro, ref strMsgID))
                        {
                            SaveLog(strMacroLog);
                            strMsgID = "04_02010100_029";
                            return false;
                        }
                    }
                }

            }
            catch(Exception ex)
            {
                SaveLog(strMacroLog);
                strMsgID = "00_00000000_000";
                BRWebService.SaveLog(ex);
                return false;
            }

           
            return true;
        }
    }
}
