//******************************************************************
//*  作    者： 許竹香
//*  功能說明：查詢ML 轉 PL 未結清名單
//*  創建日期：2010/12/17
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using Framework.Common.HTG;
using Framework.Common.Logging;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BusinessRules;
using CSIPPayCertify.EntityLayer;
using Framework.Common.Message;

namespace CSIPPayCertify.BusinessRules
{
    public class BRML2PL : BRBase<EntityML2PL>
    {
        #region SQL
        /// <summary>
        /// 查詢ML 轉 PL 未結清名單的SQL
        /// </summary>    
        private static string SEL_ML2PL_BY_CARDNO = @" SELECT COUNT(*) FROM R_ML2PL WHERE CARD_NO=@CARD_NO";
        
 
        #endregion SQL
       
        /// 作者 許竹香
        /// 創建日期：2010/12/17
        /// <summary>
        /// 查詢卡號
        public static bool Chk_ML2PL_Exist(string strCARD_NO)
        {
            int iML2PLCount = 0;
            try
            {
                DataTable dtblCARDNo = new DataTable();
                
                SqlCommand sqlcmdSearch = new SqlCommand();
                sqlcmdSearch.CommandText = SEL_ML2PL_BY_CARDNO;
                SqlParameter parmCARD_NO = new SqlParameter("@CARD_NO", strCARD_NO);
                sqlcmdSearch.CommandType = CommandType.Text;
                sqlcmdSearch.Parameters.Add(parmCARD_NO);

                dtblCARDNo = ((DataSet)BRML2PL.SearchOnDataSet(sqlcmdSearch)).Tables[0];
                iML2PLCount = int.Parse(dtblCARDNo.Rows[0][0].ToString());

                if (iML2PLCount > 0 )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                BRML2PL.SaveLog(exp);
            }

            return false;
        }                 
   
    }
}
