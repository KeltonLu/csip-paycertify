//******************************************************************
//*  作    者：艾高
//*  功能說明：修改結清，代償證明
//*  創建日期：2009/10/14
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BusinessRules;
using CSIPPayCertify.EntityLayer;

namespace CSIPPayCertify.BusinessRules
{
    public class BRSet_SelfOther : CSIPCommonModel.BusinessRules.BRBase<EntitySet_SelfOther>
    {
        /// <summary>
        /// 修改
        /// </summary>
        public static bool UpDate(EntitySet_SelfOther eSetSelfOther, string strCondition)
        {
            try
            {
                BRSet_SelfOther.DeleteEntityByCondition(eSetSelfOther, strCondition);

                if (!BRSet_SelfOther.AddNewEntity(eSetSelfOther))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                BRSet_SelfOther.SaveLog(ex);
                return false;
            }
        }
    }
}
