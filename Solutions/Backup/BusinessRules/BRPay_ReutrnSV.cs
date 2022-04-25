//******************************************************************
//*  作    者：chaoma
//*  功能說明：
//*  創建日期：2009/11/12
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
    public class BRPay_ReutrnSV : BRBase<EntityPay_ReturnSV>
    {


        /// <summary>
        /// 添加Pay_Return檔
        /// </summary>
        /// <param name="eSystemLog"></param>
        /// <returns></returns>
        public static bool AddNew(EntityPay_ReturnSV eyPayReturnSV)
        {

            try
            {
                EntitySet<EntityPay_ReturnSV> esResult = null;

                SqlHelper SQL = new SqlHelper();

                SQL.AddCondition(EntityPay_ReturnSV.M_serialNo, Operator.Equal, DataTypeUtils.String, eyPayReturnSV.serialNo);

                esResult = (EntitySet<EntityPay_ReturnSV>)BRPay_ReutrnSV.Search(SQL.GetFilterCondition());

                //查詢有資料則刪除後添加，無資料則直接添加
                if (esResult.Count > 0)
                {

                    if (eyPayReturnSV.DB_DeleteEntity(SQL.GetFilterCondition()))
                    {
                        if (BRPay_ReutrnSV.AddNewEntity(eyPayReturnSV))
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
                    if (BRPay_ReutrnSV.AddNewEntity(eyPayReturnSV))
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
                BRPay_ReutrnSV.SaveLog(exp);
                return false;
            }

        }
    }
}
