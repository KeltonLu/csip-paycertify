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
using CSIPCommonModel.BusinessRules;
using CSIPPayCertify.EntityLayer;
using System.Data;
using System.Collections;

namespace CSIPPayCertify.BusinessRules
{
    public class BRPay_CardSV_Temp : BRBase<EntityPay_CardSV_Temp>
    {
                
        #region sql語句 

        public const string SEL_MAX_CardTempID = @"SELECT ISNULL(MAX(CardTemp_ID) + 1,1) AS CardTempID FROM Pay_CardSV_Temp";

        #endregion

        /// <summary>
        /// 產生Pay_Card_Temp序號
        /// </summary>
        /// <param name="strCardTempID">Pay_Card_Temp序號</param>
        /// <param name="strMsgID">錯誤信息ID</param>
        /// <returns>True---查詢成功；False----查詢失敗</returns>
        public static bool GetCardTempID(ref String strCardTempID, ref string strMsgID)
        {
            DataSet dstResult=BRPay_CardSV_Temp.SearchOnDataSet(SEL_MAX_CardTempID);
            if (dstResult == null)
            {
                strMsgID = "04_02010100_022";
                return false;
            }
            else
            {
                strMsgID = "04_02010100_021";
                strCardTempID = dstResult.Tables[0].Rows[0]["CardTempID"].ToString();
                return true;
            }
        }

        /// <summary>
        /// 新增Pay_CardSV_Temp表信息
        /// </summary>
        /// <param name="ePayCardSVTemp">要新增的信息</param>
        /// <param name="strMsgID">錯誤信息ID</param>
        /// <returns>True---刪除成功；False----刪除失敗</returns>
        public static bool Add(EntityPay_CardSV_Temp ePayCardSVTemp , ref string strMsgID)
        {

            if (!ePayCardSVTemp.DB_InsertEntity())
            {
                strMsgID = "04_02010100_026";
                return false;
            }
            else
            {
                strMsgID = "04_02010100_025";
                return true;
            }
        }

        /// <summary>
        /// 刪除Pay_CardSV_Temp表信息
        /// </summary>
        /// <param name="ePayCardSVTemp">要刪除的信息</param>
        /// <param name="strCondition">刪除條件</param>
        /// <param name="strMsgID">錯誤信息ID</param>
        /// <returns>True---刪除成功；False----刪除失敗</returns>
        public static bool Delete(EntityPay_CardSV_Temp ePayCardSVTemp, string strCondition, ref string strMsgID)
        {

            if (!BRPay_CardSV_Temp.DeleteEntityByCondition(ePayCardSVTemp, strCondition))
            {
                strMsgID = "04_02010100_024";
                return false;
            }
            else
            {
                strMsgID = "04_02010100_023";
                return true;
            }
        }

    }
}
