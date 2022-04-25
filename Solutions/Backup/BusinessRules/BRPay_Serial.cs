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
using CSIPCommonModel.BaseItem;
using Framework.Data.OM;
using Framework.Data.OM.Transaction;
using System.Data.SqlClient;
using System.Data;


namespace CSIPPayCertify.BusinessRules
{
    public class BRPay_Serial : BRBase<EntityPay_Serial>
    {

        #region sql語句
        public const string SEL_CURRENT_NO = @"SELECT ISNULL(MAX(CURRENTNO)+1,1) AS CURRENTNO FROM [Pay_Serial]";
        public const string UPD_CURRENT_NO = @"UPDATE [Pay_Serial] SET CURRENTNO = CURRENTNO + @CURRENTNO";
        #endregion


        /// <summary>
        /// 取得最新的Pay_Certify序號
        /// </summary>
        /// <param name="strNo">卡別編號</param>
        /// <param name="strSerialNo">傳出:最新的Pay_Certify序號</param>
        /// <returns>True - 成功; False - 失敗</returns>
        public static bool GetSerialNo(string strNo, ref string strSerialNo)
        {
            try
            {
                string strTmpID;                    //* 序號前綴
                string strCurNo;                    //* 當前Card表編號
                EntityPay_SerialSet esPaySerial;    //* 序號表結果集
                EntityPay_Serial ePaySerial;        //* 序號表單列
                SqlHelper sql;                      //* Sql語句幫手

                
                //* 卡號類型(1碼) + 當天民國年7碼
                strTmpID = strNo + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
                strCurNo = "0";

                //取得DB中該類型當天的序號資料
                esPaySerial = new EntityPay_SerialSet();
                sql = new SqlHelper();
                sql.AddCondition(EntityPay_Serial.M_DAY, Operator.Equal, DataTypeUtils.String, strTmpID);
                esPaySerial.FillEntitySet(sql.GetFilterCondition());

                ePaySerial = new EntityPay_Serial();

                if (esPaySerial.Count > 0)
                {
                    //* 如果資料庫里當天當類型有資料, 最大序號+1
                    strSerialNo = esPaySerial.GetEntity(0).SERIALNO.ToString();
                    strCurNo = esPaySerial.GetEntity(0).CURRENTNO.ToString();
                    strSerialNo = strTmpID + Convert.ToString(int.Parse(strSerialNo.Substring(strSerialNo.Length - 3)) + 1).PadLeft(3, '0');
                }
                else
                {
                    //* 如果資料庫里當天當類型沒有資料,從strTmpID+001開始
                    strSerialNo = strTmpID + "001";
                }
                
                //* 事務處理。
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    //* 刪除舊資料失敗就返回False
                    if (!BRPay_Serial.DeleteEntityByCondition(ePaySerial,sql.GetFilterCondition())) { return false; }

                    ePaySerial.SERIALNO = strSerialNo;
                    ePaySerial.DAY = strTmpID;
                    ePaySerial.CURRENTNO = Convert.ToDecimal(strCurNo);
                    
                    //* 插入資料
                    if (!ePaySerial.DB_InsertEntity())
                    {
                        return false;
                    }
                    //* 事務提交
                    ts.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                SaveLog(ex);
                return false;
            }

            //return true;
        }   //* End GetSerialNo()

        /// <summary>
        /// 取得最新Pay_Card表流水號
        /// </summary>
        /// <param name="strCurrentNo">傳出:最新Pay_Card表流水號</param>
        /// <returns>True - 成功;False - 失敗</returns>
        public static bool GetCurrentNo(ref string strCurrentNo)
        {
            
            try
            {
                DataTable dtblPaySerial = new DataTable();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = SEL_CURRENT_NO;
                cmd.CommandType = CommandType.Text;
                dtblPaySerial = BRPay_Serial.SearchOnDataSet(cmd).Tables[0];
                strCurrentNo = dtblPaySerial.Rows[0]["CURRENTNO"].ToString();


                //cmd.CommandText = UPD_CURRENT_NO;
                //cmd.CommandType = CommandType.Text;
                //SqlParameter parmCurrNo = new SqlParameter("@CURRENTNO", strCurrentNo);
                //cmd.Parameters.Add(parmCurrNo);

                //BRPay_Serial.Update(cmd);
                
            }
            catch (Exception ex)
            {
                SaveLog(ex);
                return false;
            }
            return true;
        }   //* End GetCurrentNo()

        /// <summary>
        /// Update最新的CurrentNo
        /// </summary>
        /// <param name="iCount">在原來的基礎上加上的個數</param>
        /// <returns>True - 成功;False - 失敗</returns>
        public static bool UpdCurrentNo(int iCount)
        {
            try
            {
                DataTable dtblPaySerial = new DataTable();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = UPD_CURRENT_NO;
                cmd.CommandType = CommandType.Text;
                SqlParameter parmCurrNo = new SqlParameter("@CURRENTNO", iCount);
                cmd.Parameters.Add(parmCurrNo);
                if (BRPay_Serial.Update(cmd))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                SaveLog(ex);
                return false;
            }

            //return true;
        }
    }
}
