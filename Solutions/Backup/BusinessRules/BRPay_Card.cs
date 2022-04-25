//******************************************************************
//*  作    者：宋戈
//*  功能說明：
//*  創建日期：2009/11/09
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using CSIPPayCertify.EntityLayer;
using CSIPCommonModel.BusinessRules;
using Framework.Data.OM.Collections;
using Framework.Data.OM;
using Framework.Data.OM.Transaction;

namespace CSIPPayCertify.BusinessRules
{
    public class BRPay_Card : BRBase<EntityPay_Card>
    {
        /// <summary>
        /// 將Pay_Card_Temp中臨時表資料儲存到正式表
        /// </summary>
        /// <param name="strSerialNo">儲存時想要顯示的SerialNo</param>
        /// <param name="strCardTmpID">抓取臨時表的依據CardTempID</param>
        /// <param name="strMsgID">出錯ID</param>
        /// <returns>True   -   成功;Flase  -   失敗</returns>
        public static bool InsertPayCardData(string strSerialNo, string strCardTmpID, string strML2PLCardList, ref string strMsgID)
        {
            try
            {
                string strCurrentNo = "";
                SqlHelper objSqlHelper = new SqlHelper();
                EntitySet<EntityPay_Card_Temp> esPayCardTemp = new EntitySet<EntityPay_Card_Temp>();
                EntityPay_Card ePayCard = new EntityPay_Card();

                using (OMTransactionScope ts = new OMTransactionScope())
                {
                    //* 1.通過BR得到Pay_Card_Temp表中CARDTEMP_ID= strCardTmpID的esCardTmp
                    objSqlHelper.AddCondition(EntityPay_Card_Temp.M_CARDTEMP_ID, Operator.Equal, DataTypeUtils.String, strCardTmpID);
                    objSqlHelper.AddCondition(EntityPay_Card_Temp.M_CARDNO, Operator.NotIn, DataTypeUtils.String, strML2PLCardList);
                    esPayCardTemp = (EntitySet<EntityPay_Card_Temp>)BRPay_Card_Temp.Search(objSqlHelper.GetFilterCondition());

                    //* 2.取得CurrentNo
                    if (!BRPay_Serial.GetCurrentNo(ref strCurrentNo))
                    {
                        //* 出錯
                        return false;
                    }

                    //* 3.update Pay_SerialNo SET currentNo = strCurrentNo +esCardTmp.Rows
                    if (!BRPay_Serial.UpdCurrentNo(esPayCardTemp.Count))
                    {
                        //* 出錯
                        return false;
                    }

                    //* 開始添加
                    for (int i = 0; i < esPayCardTemp.Count; i++)
                    {
                        ePayCard = new EntityPay_Card();
                        ePayCard.SERIALNO = strSerialNo;
                        ePayCard.NO = Convert.ToInt32(strCurrentNo) + i + 1;
                        ePayCard.CARDNO = esPayCardTemp.GetEntity(i).CARDNO;
                        ePayCard.CUSID = esPayCardTemp.GetEntity(i).CUSID;
                        ePayCard.CUSNAME = esPayCardTemp.GetEntity(i).CUSNAME;
                        ePayCard.OPENDAY = esPayCardTemp.GetEntity(i).OPENDAY;
                        ePayCard.CODE1 = esPayCardTemp.GetEntity(i).CODE1;
                        ePayCard.CLOSEDAY1 = esPayCardTemp.GetEntity(i).CLOSEDAY1;
                        ePayCard.CODE2 = esPayCardTemp.GetEntity(i).CODE2;
                        ePayCard.CLOSEDAY2 = esPayCardTemp.GetEntity(i).CLOSEDAY2;
                        ePayCard.TYPECARD = esPayCardTemp.GetEntity(i).TYPECARD;
                        ePayCard.CardTempId = esPayCardTemp.GetEntity(i).CARDTEMP_ID.ToString(); ;
                        BRPay_Card.AddNewEntity(ePayCard);
                    }
                    ts.Complete();
                }
            }
            catch (Exception Ex)
            {
                BRPay_Card.SaveLog(Ex);
                return false;
            }
            return true;
        }
    }
}
