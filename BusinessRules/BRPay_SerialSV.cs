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
using System.Data.SqlClient;
using Framework.Data.OM.Transaction;
using Framework.Data.OM;
using CSIPCommonModel.BaseItem;


namespace CSIPPayCertify.BusinessRules
{
    public class BRPay_SerialSV : BRBase<EntityPay_SerialSV>
    {

        /// <summary>
        /// 產生Pay_Certify的序號欄位
        /// </summary>
        /// <param name="strSerialNo">返回序號</param>
        /// <param name="strMsgID">錯誤信息ID</param>
        /// <returns>True---查詢成功；False----查詢失敗</returns>
        public static bool GetSerialSVNo(ref string strSerialNo,ref string strMsgID)
        {
            try
            {
                string strTmpID;                    //* 序號前綴
                string strCurNo;                    //* 當前Card表編號
                EntityPay_SerialSet esPaySerial;    //* 序號表結果集
                EntityPay_Serial ePaySerial;        //* 序號表單列
                SqlHelper sql;                      //* Sql語句幫手

                strSerialNo = "";
                //* 卡號類型(1碼) + 當天民國年7碼
                strTmpID = "1" + Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));

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
                    strCurNo = "0";
                }

                //* 事務處理。
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    //* 刪除舊資料失敗就返回False
                    if (!BRPay_Serial.DeleteEntityByCondition(ePaySerial, sql.GetFilterCondition())) 
                    {
                        strMsgID = "04_02010100_020";
                        return false; 
                    }

                    ePaySerial.SERIALNO = strSerialNo;
                    ePaySerial.DAY = strTmpID;
                    ePaySerial.CURRENTNO = Convert.ToDecimal(strCurNo);

                    //* 插入資料
                    if (!ePaySerial.DB_InsertEntity())
                    {
                        strMsgID = "04_02010100_020";
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
                strMsgID = "04_02010100_020";
                return false;
            }



            /* 2010/04/02 修改爲和一般清證一樣讀取Pay_Serial表
            strSerialNo = "";
            string strTempID = "1" + CSIPCommonModel.BaseItem.Function.MinGuoDate7length(DateTime.Now.ToString("yyyyMMdd"));
            EntityPay_SerialSVSet esPaySerialSV = new EntityPay_SerialSVSet();
            SqlHelper Sql = new SqlHelper();
            Sql.AddCondition(EntityPay_SerialSV.M_DAY, Operator.Equal, DataTypeUtils.String, strTempID);
            esPaySerialSV.FillEntitySet(Sql.GetFilterCondition());
            EntityPay_SerialSV ePaySerialSV = new EntityPay_SerialSV();
            try
            {
                //* 事務處理。
                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    if (esPaySerialSV.Count > 0)
                    {
                        strSerialNo = esPaySerialSV.GetEntity(0).SERIALNO;
                        strSerialNo = strTempID + Convert.ToString(int.Parse(strSerialNo.Substring(strSerialNo.Length - 3)) + 1).PadLeft(3, '0');
                    }
                    else
                    {
                        strSerialNo = strTempID + "001";
                    }
                    if (!BRPay_SerialSV.DeleteEntityByCondition(ePaySerialSV, Sql.GetFilterCondition()))
                    {
                        strMsgID = "04_02010100_020";
                        return false;
                    }
                    ePaySerialSV.SERIALNO = strSerialNo;
                    ePaySerialSV.DAY = strTempID;
                    if (!ePaySerialSV.DB_InsertEntity())
                    {
                        strMsgID = "04_02010100_020";
                        return false;
                    }
                    //* 事務提交
                    ts.Complete();
                }
            }
            catch (Exception exp)
            {
                BRPay_SerialSV.SaveLog(exp);
                strMsgID = "04_02010100_020";
                return false;
            }
            return true;
            */
        }
    }
}
