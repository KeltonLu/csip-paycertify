//******************************************************************
//*  作    者：chaoma
//*  功能說明：
//*  創建日期：2009/11/10
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
using Framework.Data.OM.Collections;

namespace CSIPPayCertify.BusinessRules
{
    public class BRPay_Reutrn : BRBase<EntityPay_Return>
    {
        

        /// <summary>
        /// 添加Pay_Return檔
        /// </summary>
        /// <param name="eSystemLog"></param>
        /// <returns></returns>
        public static bool AddNew(EntityPay_Return eyPayReturn)
        {

            try
            {
                EntitySet<EntityPay_Return> esResult = null;

                SqlHelper SQL = new SqlHelper();

                SQL.AddCondition(EntityPay_Return.M_serialNo, Operator.Equal, DataTypeUtils.String, eyPayReturn.serialNo);

                esResult = (EntitySet<EntityPay_Return>)BRPay_Reutrn.Search(SQL.GetFilterCondition());

                //查詢有資料則刪除後添加，無資料則直接添加
                if (esResult.Count > 0)
                {

                    if (eyPayReturn.DB_DeleteEntity(SQL.GetFilterCondition()))
                    {
                        if (BRPay_Reutrn.AddNewEntity(eyPayReturn))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (BRPay_Reutrn.AddNewEntity(eyPayReturn))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
     
            }
            catch (Exception exp)
            {
                BRPay_Reutrn.SaveLog(exp);
                return false;
            }

        }
    }
}
