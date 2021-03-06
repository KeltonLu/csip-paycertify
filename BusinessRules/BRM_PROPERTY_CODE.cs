//******************************************************************
//*  作    者：艾高
//*  功能說明：屬性
//*  創建日期：2009/10/28
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Framework.Data.OM;
using CSIPCommonModel.EntityLayer;
using System.Data;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using System.Data.SqlClient;
using Framework.Common.Message;

namespace CSIPPayCertify.BusinessRules
{
    public class BRM_PROPERTY_CODE : CSIPCommonModel.BusinessRules.BRBase<EntityM_PROPERTY_CODE>
    {
        #region sql語句
        /// <summary>
        /// 查詢公共屬性
        /// </summary>
        private const string SEL_MAILTYPE = @"SELECT PROPERTY_CODE,PROPERTY_NAME FROM [M_PROPERTY_CODE] WHERE 
                                    FUNCTION_KEY = '04' AND PROPERTY_KEY = @PROPERTY_KEY
                                    ORDER BY SEQUENCE ";

        /// 作者:Ares_Jack
        /// 創建日期:20220224
        /// 修改日期:
        /// <summary>
        /// 查詢總行資料
        /// </summary>
        private const string SEL_HEAD_OFFICE = @"SELECT PROPERTY_NAME FROM [M_PROPERTY_CODE] 
                                    WHERE FUNCTION_KEY = '04' AND PROPERTY_KEY = 'headOffice' ";
        #endregion


        /// <summary>
        /// 查詢公共屬性
        /// </summary>
        /// <param name="strPropertyKey"></param>
        /// <param name="dtblResult"></param>
        public static bool GetCommonProperty(string strPropertyKey, ref DataTable dtblResult)
        {
            SqlCommand sqlcmd = new SqlCommand();

            sqlcmd.CommandText = SEL_MAILTYPE;

            sqlcmd.CommandType = CommandType.Text;

            SqlParameter parmFunctionKey = new SqlParameter("@PROPERTY_KEY", strPropertyKey);

            sqlcmd.Parameters.Add(parmFunctionKey);


            try
            {
                dtblResult = BRM_PROPERTY_CODE.SearchOnDataSet(sqlcmd, "Connection_CSIP").Tables[0];
                return true;
            }
            catch (Exception exp)
            {
                BRM_PROPERTY_CODE.SaveLog(exp);
                return false;

            }
        }

        /// 作者:Ares_Jack
        /// 創建日期:20220224
        /// 修改日期:
        /// <summary>
        /// 查詢總行資料
        /// </summary>
        /// <param name="dtblResult"></param>
        public static bool GetHeadOffice(ref DataTable dtblResult)
        {
            SqlCommand sqlcmd = new SqlCommand();

            sqlcmd.CommandText = SEL_HEAD_OFFICE;

            sqlcmd.CommandType = CommandType.Text;


            try
            {
                dtblResult = BRM_PROPERTY_CODE.SearchOnDataSet(sqlcmd, "Connection_CSIP").Tables[0];
                return true;
            }
            catch (Exception exp)
            {
                BRM_PROPERTY_CODE.SaveLog(exp);
                return false;

            }
        }
    }

       
        
}
