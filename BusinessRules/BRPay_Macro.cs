//******************************************************************
//*  作    者：余洋(rosicky)
//*  功能說明：
//*  創建日期：2009/10/27
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using CSIPCommonModel.BusinessRules;
using CSIPPayCertify.EntityLayer;
using Framework.Data.OM;
using System.Data.SqlClient;
using Framework.Data.OM.Collections;

namespace CSIPPayCertify.BusinessRules
{
    public class BRPay_Macro : BRBase<EntityPay_Macro>
    {
        #region SQL
        public const string SEL_MAX_LAST_PAY_DATE = @"SELECT ISNULL(MAX(M_LAST_PAY_DATE),'') AS M_LAST_PAY_DATE FROM Pay_Macro WHERE M_CUST_ID=@CustID";
        #endregion
        /// <summary>
        /// 新增實體
        /// </summary>
        /// <param name="ePayMacro">要新增的實體</param>
        /// <param name="strMsgID">錯誤信息ID</param>
        /// <returns>True---新增成功；False----新增失敗</returns>
        public static bool Add(EntityPay_Macro ePayMacro, ref string strMsgID)
        {

            if (!ePayMacro.DB_InsertEntity())
            {
                strMsgID = "04_02010100_029";
                return false;
            }
            else
            {
                strMsgID = "04_02010100_028";
                return true;
            }
        }

        /// <summary>
        /// 取得最後繳款日
        /// </summary>
        /// <param name="strUserID"></param>
        /// <param name="strType"></param>
        /// <param name="strMsgID"></param>
        /// <param name="strMacroLastPayDate"></param>
        /// <returns></returns>
        public static bool GetLastPayDate(string strUserID, string strType, ref string strMsgID, ref string strMacroLastPayDate)
        {
            try
            {
                string strCommand = SEL_MAX_LAST_PAY_DATE;
                DataTable dtblMacro = new DataTable();
                SqlCommand cmd = new SqlCommand();
                SqlParameter parmCustID = new SqlParameter("@CustID", strUserID);

                if (strType == "")
                {
                    //* 如果strtType = M(清償證明-ML&卡)代表搜索全部,直接執行下面SQL
                }
                if (strType == "C" || strType == "G" || strType == "O" || strType == "B")
                {
                    //* 如果strType = C(清償證明-卡), G(代償證明-卡)., O(GCB清償證明-卡),B( GCB代償證明),代表搜索卡的,必須在SQL上加上區間 卡號前7碼NOT BETWEEN 1234048 AND 1234079
                    strCommand = strCommand + " AND SUBSTRING(M_CARD_NO,1,7) NOT BETWEEN '1234048' AND '1234079'";
                }
                if (strType == "N" || strType == "T" )
                {
                    //* 如果strType= N(清償證明-ML Only), T(代償證明-ML), 代表搜索ML,必須在SQL上加上區間 卡號前7碼BETWEEN 1234048 AND 1234079
                    strCommand = strCommand + " AND SUBSTRING(M_CARD_NO,1,7) BETWEEN '1234048' AND '1234079'";
                }
                cmd.CommandText = strCommand;
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(parmCustID);
                dtblMacro = BRPay_Serial.SearchOnDataSet(cmd).Tables[0];
                //* 得到该ID下最大的的值(最后缴款日)
                strMacroLastPayDate = dtblMacro.Rows[0]["M_LAST_PAY_DATE"].ToString();
            }
            catch (Exception Ex)
            {
                SaveLog(Ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 通過客戶ID取得該ID的資料列表
        /// </summary>
        /// <param name="strCustID"></param>
        /// <param name="strMsgID"></param>
        /// <param name="esResult"></param>
        /// <returns></returns>
        public static bool GetDataByCustID(string strCustID, ref string strMsgID, ref EntityPay_MacroSet esResult)
        {
            try
            {                
                SqlHelper sql = new SqlHelper();
                EntityPay_MacroSet esMacro = new EntityPay_MacroSet();
                sql.AddCondition(EntityPay_Macro.M_M_CUST_ID, Operator.Equal, DataTypeUtils.String, strCustID.Trim());
                esResult.FillEntitySet(sql.GetFilterCondition());
            }
            catch(Exception exp) 
            {
                BRPay_Macro.SaveLog(exp);
                strMsgID = "04_02010100_039";
                return false;
            }
            strMsgID = "04_02010100_038";
            return true;
        }
    }
}
