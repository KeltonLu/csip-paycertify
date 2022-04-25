//******************************************************************
//*  作    者：偉林
//*  功能說明：列印報表
//*  創建日期：2009/10/07
//*  修改記錄：
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Configuration;
using CrystalDecisions.CrystalReports.Engine;
using Framework.Data.OM;
using Framework.Data.OM.Collections;
using Framework.Data.OM.Transaction;
using Framework.Common.Message;
using Framework.Common.Logging;
using CSIPPayCertify.EntityLayer;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;
using CSIPCommonModel.BusinessRules;


namespace CSIPPayCertify.BusinessRules
{
    public class BRReport : BRBase<EntityReport>
    {
        #region sql語句
        #region sql語句 ReportPaySV

        public const string SEL_PAY_SV_BY_SerialNo = @"SELECT ISNULL(SerialNo,'') AS SerialNo, ISNULL(UserID,'') AS UserID,ISNULL(UserName,'') AS UserName,ISNULL(Add1,'') AS Add1, ISNULL(Zip,'') AS Zip, ISNULL(Add2,'') AS Add2, ISNULL(Add3,'') AS Add3, ISNULL(MailDay,'') AS MailDay, ISNULL(MailNo,'') AS MailNo, ISNULL(Note,'') AS Note, ISNULL(GetFee,'') AS GetFee, ISNULL(isMail,'') AS isMail, ISNULL([Type],'') AS [Type], ISNULL(CardType,'') AS CardType, ISNULL(Void,'') AS Void, ISNULL(KeyinDay,'') AS KeyinDay, ISNULL(ShowExtra,'') AS ShowExtra, ISNULL(Consignee,'') AS Consignee, ISNULL(EndDate,'') AS EndDate,ISNULL(ClearDate,'') AS ClearDate, ISNULL(MakeUp,'') AS MakeUp, ISNULL(MakeUpDate,'') AS MakeUpDate FROM Pay_SV WHERE SerialNo IN ({0}) ORDER BY [SerialNo] ASC";


        #endregion

        #region sql語句 Report02020300_2

        public const string SEL_PAY_SV_BY_SETTLE = @"SELECT ISNULL(SerialNo,'') AS SerialNo, ISNULL(UserID,'') AS UserID,ISNULL(UserName,'') AS UserName,ISNULL(Add1,'') AS Add1, ISNULL(Zip,'') AS Zip, ISNULL(Add2,'') AS Add2, ISNULL(Add3,'') AS Add3, ISNULL(MailDay,'') AS MailDay, ISNULL(MailNo,'') AS MailNo, ISNULL(Note,'') AS Note, ISNULL(GetFee,'') AS GetFee, ISNULL(isMail,'') AS isMail, ISNULL([Type],'') AS [Type], ISNULL(CardType,'') AS CardType, ISNULL(Void,'') AS Void, ISNULL(KeyinDay,'') AS KeyinDay, ISNULL(ShowExtra,'') AS ShowExtra, ISNULL(Consignee,'') AS Consignee, ISNULL(EndDate,'') AS EndDate, ISNULL(MakeUp,'') AS MakeUp, ISNULL(MakeUpDate,'') AS MakeUpDate FROM Pay_SV WHERE  ISNULL(KEYINDAY,'') <> '' AND keyinDay>=@STARTDATE AND keyinday<=@ENDDATE ";


        public const string SEL_PAY_SV_BY_SETTLE_NUll = @"SELECT ISNULL(SerialNo,'') AS SerialNo, ISNULL(UserID,'') AS UserID,ISNULL(UserName,'') AS UserName,ISNULL(Add1,'') AS Add1, ISNULL(Zip,'') AS Zip, ISNULL(Add2,'') AS Add2, ISNULL(Add3,'') AS Add3, ISNULL(MailDay,'') AS MailDay, ISNULL(MailNo,'') AS MailNo, ISNULL(Note,'') AS Note, ISNULL(GetFee,'') AS GetFee, ISNULL(isMail,'') AS isMail, ISNULL([Type],'') AS [Type], ISNULL(CardType,'') AS CardType, ISNULL(Void,'') AS Void, ISNULL(KeyinDay,'') AS KeyinDay, ISNULL(ShowExtra,'') AS ShowExtra, ISNULL(Consignee,'') AS Consignee, ISNULL(EndDate,'') AS EndDate, ISNULL(MakeUp,'') AS MakeUp, ISNULL(MakeUpDate,'') AS MakeUpDate FROM Pay_SV WHERE ISNULL(KEYINDAY,'') = '' AND keyinDay>=@STARTDATE AND keyinday<=@ENDDATE ORDER BY [SerialNo] ASC";


        #endregion

        #region sql語句 Report01020100
        //* 總件數
        public const string SEL_SUM =
 @"SELECT Void, IsMail, SerialNo, ShowExtra, Note FROM PAY_CERTIFY WHERE KeyinDay >= @STARTDATE AND KeyinDay <= @ENDDATE ORDER BY SERIALNO ASC";
        //* 免手續費件數 = 非註銷的免手續費件數
        public const string SEL_IsFreeCount =
@"SELECT counts=count(1) FROM Pay_Certify WHERE keyinDay >= @STARTDATE AND keyinday <= @ENDDATE AND VOID <> 'Y' AND IsFree = 'Y'";
        //* 收手續費件數 = 非註銷 + 非免手續費件數
        public const string SEL_NotFreeCount =
@"SELECT counts=count(1) FROM Pay_Certify WHERE keyinDay >= @STARTDATE AND keyinday <= @ENDDATE AND VOID <> 'Y' AND IsFree <> 'Y'";
        //* 退件
        public const string SEL_ReutrnCount =
 @"SELECT counts=count(1) FROM Pay_Return WHERE returnDay >= @STARTDATE AND returnDay<=@ENDDATE";
        //* 註銷
        public const string SEL_CancelCount =
@"SELECT counts=count(1) FROM Pay_Certify WHERE keyinDay >= @STARTDATE AND keyinday <= @ENDDATE AND VOID = 'Y'";
        //* 補寄(已廢棄)
//        public const string SEL_MakeUpCount =
//@"SELECT counts=count(1) FROM Pay_Certify WHERE keyinDay >= @STARTDATE AND keyinday <= @ENDDATE AND makeUp = 'Y'";        

        public const string SEL_Pay_Certify =
                                            @"SELECT
                                            ISNULL(SerialNo,'') AS SerialNo, 
                                            ISNULL(UserID,'') AS UserID,
                                            ISNULL(UserName,'') AS UserName,
                                            ISNULL(Add1,'') AS Add1, 
                                            ISNULL(Zip,'') AS Zip, 
                                            ISNULL(Add2,'') AS Add2, 
                                            ISNULL(Add3,'') AS Add3, 
                                            ISNULL(MailDay,'') AS MailDay, 
                                            ISNULL(MailNo,'') AS MailNo, 
                                            ISNULL(Note,'') AS Note, 
                                            ISNULL(PayName,'') AS PayName, 
                                            ISNULL(Owe,'') AS Owe, 
                                            ISNULL(Pay,'') AS Pay, 
                                            ISNULL(PayDay,'') AS PayDay, 
                                            ISNULL(GetFee,'') AS GetFee, 
                                            ISNULL(isMail,'') AS isMail, 
                                            ISNULL([Type],'') AS Type, 
                                            ISNULL(CardType,'') AS CardType, 
                                            ISNULL(Void,'') AS Void, 
                                            ISNULL(KeyinDay,'') AS KeyinDay, 
                                            ISNULL(ShowExtra,'') AS ShowExtra, 
                                            ISNULL(OpenDay,'') AS OpenDay,
                                            ISNULL(CloseDay,'') AS CloseDay, 
                                            ISNULL(Consignee,'') AS Consignee, 
                                            ISNULL(EndDate,'') AS EndDate, 
                                            ISNULL(MakeUp,'') AS MakeUp, 
                                            ISNULL(MakeUpDate,'') AS MakeUpDate, 
                                            ISNULL(MainCard4E,'') AS MainCard4E 
                                            FROM Pay_Certify
                                            WHERE 
                                            KeyinDay >= @STARTDATE 
                                            AND KeyinDay <= @ENDDATE
                                            AND VOID <> 'Y' AND ISMAIL = 'Y'
                                            ORDER BY SERIALNO ASC
                                            ";

        #endregion

        #region sql語句 Report02020100

        public const string SEL_SerialNo =
 @"SELECT DISTINCT SerialNo FROM PAY_SV WHERE keyinDay>=@STARTDATE AND keyinday<=@ENDDATE ORDER BY serialNo ASC";

        public const string SEL_ReutrnCountSV =
 @"SELECT counts=count(1) FROM Pay_ReturnSV WHERE returnDay >= @STARTDATE AND returnDay<=@ENDDATE";

        public const string SEL_MakeUpCountSV =
@"SELECT counts=count(1) FROM Pay_SV WHERE keyinDay>=@STARTDATE AND keyinday<=@ENDDATE  AND makeUp='Y'";

        public const string SEL_VoidCountSV =
@"SELECT counts=count(1) FROM Pay_SV WHERE keyinDay>=@STARTDATE  AND keyinday<=@ENDDATE  AND Void='Y'";


        public const string SEL_Temp =
                            @"SELECT
                            A.UserID,
                            A.UserName,
                            A.SerialNo,
                            A.Zip,
                            A.Add1,
                            A.Add2,
                            A.Add3,
                            A.Consignee,
                            A.EndDate,
                            A.[Type],
                            B.BLK,
                            A.ApplyDate,
                            A.Status,
                            A.ClearDate,
                            A.ApplyUser,
                            A.AgreeUser,
                            A.IsPaidOffAlone,
                            B.CardNo,
                            B.CUSID, B.CUSNAME, B.BLK,
                            B.TypeCard
                            From PAY_SV AS A
                            LEFT OUTER JOIN PAY_CARDSV AS B ON A.SerialNo = B.SerialNo
                            WHERE A.SerialNo = @SERIALNO
                            ORDER BY A.SerialNo,B.TypeCard desc
                            ";

        public const string SEL_Dayth_SV =
                            @"SELECT
                            A.UserID,
                            A.UserName,
                            A.SerialNo,
                            A.Zip,
                            A.Add1,
                            A.Add2,
                            A.Add3,
                            A.Consignee,
                            A.EndDate,
                            A.[Type],
                            B.BLK,
                            A.ApplyDate,
                            A.Status,
                            A.ClearDate,
                            A.ApplyUser,
                            A.AgreeUser,
                            A.IsPaidOffAlone,
                            B.CardNo,
                            B.CUSID, B.CUSNAME, B.BLK,
                            B.TypeCard
                            From PAY_SV AS A
                            LEFT OUTER JOIN PAY_CARDSV AS B ON A.SerialNo = B.SerialNo
                            WHERE A.keyinDay>=@STARTDATE AND A.keyinday<=@ENDDATE
                            ORDER BY A.SerialNo,B.TypeCard desc
                            ";
        #endregion
        #endregion
        /// <summary>
        /// 新增清償證明報表
        /// </summary>
        /// <param name="strCustID">傳入的客戶ID</param>
        /// <param name="strMsgID">錯誤信息ID</param>
        /// <param name="rptReort">得到的報表</param>
        /// <returns>true---報表打出成功,false---報表打出失敗</returns>
        public static bool Report01010100(string strCustID, ref string strMsgID, ref  DataSet dstResult)
        {
            try
            {
                SqlHelper sql = new SqlHelper();
                //*添加查詢條件strCustID等于客戶ID
                sql.AddCondition(EntityPay_Macro.M_M_CUST_ID, Operator.Equal, DataTypeUtils.String, strCustID);

                //*創建一個DataTable
                DataTable dtblPayMacro = new DataTable("Pay_Macro");
                dtblPayMacro.Columns.Add(new DataColumn("M_CUST_ID", Type.GetType("System.String")));
                dtblPayMacro.Columns.Add(new DataColumn("M_CUST_NAME", Type.GetType("System.String")));
                dtblPayMacro.Columns.Add(new DataColumn("M_CARD_NO", Type.GetType("System.String")));
                dtblPayMacro.Columns.Add(new DataColumn("M_CLOSE_REP", Type.GetType("System.String")));
                dtblPayMacro.Columns.Add(new DataColumn("M_BALANCE", Type.GetType("System.Decimal")));
                dtblPayMacro.Columns.Add(new DataColumn("M_DEBT_DATE", Type.GetType("System.String")));
                dtblPayMacro.Columns.Add(new DataColumn("M_DEBT_AMT", Type.GetType("System.String")));
                dtblPayMacro.Columns.Add(new DataColumn("M_DEBT_BALANCE", Type.GetType("System.Decimal")));
                dtblPayMacro.Columns.Add(new DataColumn("M_LAST_PAY_DATE", Type.GetType("System.String")));
                dtblPayMacro.Columns.Add(new DataColumn("M_TOTAL_PAYMENT", Type.GetType("System.Decimal")));
                dtblPayMacro.Columns.Add(new DataColumn("M_CLOSE_DATE", Type.GetType("System.String")));
                dtblPayMacro.Columns.Add(new DataColumn("M_CLOSE_REASON", Type.GetType("System.String")));


                //*查詢數據
                EntityPay_MacroSet esPayMacro = new EntityPay_MacroSet();

                try
                {
                    esPayMacro.FillEntitySet(sql.GetFilterCondition());
                }
                catch(Exception exp)
                {
                    BRReport.SaveLog(exp);
                    strMsgID = "00_00000000_000";
                    return false;
                }


                for (int intLoop = 0; intLoop < esPayMacro.TotalCount; intLoop++)
                {
                    DataRow drowPayMacro = dtblPayMacro.NewRow();
                    drowPayMacro["M_CUST_ID"] = esPayMacro.GetEntity(intLoop).M_CUST_ID;
                    drowPayMacro["M_CUST_NAME"] = esPayMacro.GetEntity(intLoop).M_CUST_NAME;
                    drowPayMacro["M_CARD_NO"] = esPayMacro.GetEntity(intLoop).M_CARD_NO;
                    drowPayMacro["M_CLOSE_REP"] = esPayMacro.GetEntity(intLoop).M_CLOSE_REP;
                    drowPayMacro["M_BALANCE"] = Convert.ToDecimal(esPayMacro.GetEntity(intLoop).M_BALANCE);
                    drowPayMacro["M_DEBT_DATE"] = esPayMacro.GetEntity(intLoop).M_DEBT_DATE;
                    drowPayMacro["M_DEBT_AMT"] = esPayMacro.GetEntity(intLoop).M_DEBT_AMT;
                    drowPayMacro["M_DEBT_BALANCE"] = Convert.ToDecimal(esPayMacro.GetEntity(intLoop).M_DEBT_BALANCE);
                    drowPayMacro["M_LAST_PAY_DATE"] = esPayMacro.GetEntity(intLoop).M_LAST_PAY_DATE;
                    drowPayMacro["M_TOTAL_PAYMENT"] = Convert.ToDecimal(esPayMacro.GetEntity(intLoop).M_TOTAL_PAYMENT);
                    drowPayMacro["M_CLOSE_DATE"] = esPayMacro.GetEntity(intLoop).M_CLOSE_DATE;
                    drowPayMacro["M_CLOSE_REASON"] = esPayMacro.GetEntity(intLoop).M_CLOSE_REASON;
                    dtblPayMacro.Rows.Add(drowPayMacro);
                }

                dstResult.Tables.Add(dtblPayMacro);

                return true;
            }
            catch (Exception exp)
            {
                strMsgID = "04_00000000_003";
                BRReport.SaveLog(exp);

                return false;
            }
        }
        /// <summary>
        /// 大宗掛號報表(清償證明)
        /// </summary>
        /// <param name="strMailDay">傳入的郵寄日期</param>
        /// <param name="strMailType">傳入的郵寄方式</param>
        /// <param name="strMsgID">錯誤信息ID</param>
        /// <param name="rptResult">返回報表對象</param>
        /// <returns></returns>
        public static bool Report01020200(string strMailDay, string strMailType, ref string strMsgID, ref  DataSet dstResult)
        {
            try
            {
                SqlHelper sql = new SqlHelper();
                //*添加條件mailDay=郵寄時間
                sql.AddCondition(EntityPay_Certify.M_mailDay, Operator.Equal, DataTypeUtils.String, strMailDay);
                //*添加條件mailMethod=郵寄方式
                sql.AddCondition(EntityPay_Certify.M_mailMethod, Operator.Equal, DataTypeUtils.String, strMailType);
                //*添加order條件
                sql.AddOrderCondition(EntityPay_Certify.M_keyinDay, ESortType.ASC);

                //*創建datatable
                DataTable dtblPay_Certify = new DataTable("Certify");
                dtblPay_Certify.Columns.Add(new DataColumn("Consignee", Type.GetType("System.String")));
                dtblPay_Certify.Columns.Add(new DataColumn("mailNo", Type.GetType("System.String")));
                dtblPay_Certify.Columns.Add(new DataColumn("zip", Type.GetType("System.String")));
                dtblPay_Certify.Columns.Add(new DataColumn("add1", Type.GetType("System.String")));
                dtblPay_Certify.Columns.Add(new DataColumn("add2", Type.GetType("System.String")));
                dtblPay_Certify.Columns.Add(new DataColumn("add3", Type.GetType("System.String")));
                dtblPay_Certify.Columns.Add(new DataColumn("serialNo", Type.GetType("System.String")));
                //*查詢數據
                EntityPay_CertifySet esPay_Certify = new EntityPay_CertifySet();
                try
                {
                    esPay_Certify.FillEntitySet(sql.GetFilterCondition());
                }
                catch(Exception exp)
                {
                    BRReport.SaveLog(exp);
                    strMsgID = "00_00000000_000";
                    return false;
                }


                //if (esPay_Certify.Count == 0)
                //{
                //    strMsgID = "00_00000000_037";
                //    return true;
                //}

                //*添加查詢數據到DataTable
                if (esPay_Certify.TotalCount > 0)
                {
                    for (int intLoop = 0; intLoop < esPay_Certify.TotalCount; intLoop++)
                    {
                        DataRow drowesPayCertify = dtblPay_Certify.NewRow();
                        //*綁定收件人姓名欄位
                        if (string.IsNullOrEmpty((((EntityPay_Certify)esPay_Certify.GetEntity(intLoop)).consignee + "").Trim()))
                        {
                            drowesPayCertify["Consignee"] = ((EntityPay_Certify)esPay_Certify.GetEntity(intLoop)).userName;
                        }
                        else
                        {                           
                            drowesPayCertify["Consignee"] = ((EntityPay_Certify)esPay_Certify.GetEntity(intLoop)).consignee;                   
                        }
                        //drowNewInsert["mailday"] = Function.InsertTimeSpan(((EntityPay_Certify)esReport01020200.GetEntity(intLoop)).mailDay);
                        drowesPayCertify["zip"] = ((EntityPay_Certify)esPay_Certify.GetEntity(intLoop)).zip;
                        drowesPayCertify["add1"] = ((EntityPay_Certify)esPay_Certify.GetEntity(intLoop)).add1;
                        drowesPayCertify["add2"] = ((EntityPay_Certify)esPay_Certify.GetEntity(intLoop)).add2;
                        drowesPayCertify["add3"] = ((EntityPay_Certify)esPay_Certify.GetEntity(intLoop)).add3;
                        drowesPayCertify["mailNo"] = ((EntityPay_Certify)esPay_Certify.GetEntity(intLoop)).mailNo;
                        drowesPayCertify["serialNo"] = ((EntityPay_Certify)esPay_Certify.GetEntity(intLoop)).serialNo;
                        dtblPay_Certify.Rows.Add(drowesPayCertify);
                    }
                }

                dstResult.Tables.Add(dtblPay_Certify);
                return true;
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp);
                strMsgID = "04_01020100_002";
                return false;
            }
        }

        /// <summary>
        /// 剔退記錄查詢報表
        /// </summary>
        /// <param name="strRebackDateStart">傳入的剔退日期(起)</param>
        /// <param name="strRebackDateEnd">傳入的剔退日期(迄)</param>
        /// <param name="strAppDateStart">傳入的申請日期(起)</param>
        /// <param name="strAppDateEnd">傳入的申請日期(迄)</param>
        /// <param name="strUserID">傳入的用戶名</param>
        /// <param name="strMsgID">錯誤信息ID</param>
        /// <param name="rptResult">返回報表對象</param>
        /// <returns></returns>
        public static bool Report02020600(string strRebackDateStart, string strRebackDateEnd, string strApplyDateStart, string strApplyDateEnd,
                                           string strUserID, ref string strMsgID, ref  DataSet dstResult)
        {
            try
            {

                DataTable dtblResult = null;
                BRPay_SV_Feedback.FeedBackQuery(strRebackDateStart, strRebackDateEnd, strApplyDateStart, strApplyDateEnd, strUserID, ref strMsgID, ref dtblResult);
                dtblResult.TableName = "PAY_SV_FEEDBACK";
                dstResult.Tables.Add(dtblResult.DefaultView.ToTable());
                return true;
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp);
                strMsgID = "04_02020600_011";
                return false;
            }

        }


        /// <summary>
        /// 自取簽收總表(結清證明)
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strUserID">身分證字號</param>
        /// <param name="strMailDay">自取日期</param>
        /// <param name="strCardNo">卡號</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="rptResult">返回報表對象</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0302020002(string strStartSerialNo, string strEndSerialNo, string strUserID,
                    string strMailDay, string strCardNo, ref string strMsgID, ref  DataSet dstResult)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_isMail, Operator.Equal, DataTypeUtils.String, "N");
            //* 添加查詢條件Type等於0 - 結清, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "0");

            //* 如果有輸入證明編號
            if (strStartSerialNo != "" || strEndSerialNo != "")
            {
                if (strStartSerialNo == "")
                {
                    strStartSerialNo = strEndSerialNo;
                }
                if (strEndSerialNo == "")
                {
                    strEndSerialNo = strStartSerialNo;
                }
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
            }
            if (strUserID != "")
            {
                //* 如果有輸入[身份證字號],則添加查詢條件UserID LIKE [身份證字號]%
                Sql.AddCondition(EntitySet_Pay_Certify.M_userID, Operator.LikeLeft, DataTypeUtils.String, strUserID);
            }
            if (strMailDay != "")
            {
                //* 如果有輸入[自取日期],則添加查詢條件MailDay = [自取日期]
                Sql.AddCondition(EntitySet_Pay_Certify.M_mailDay, Operator.Equal, DataTypeUtils.String, strMailDay);
            }
            if (strCardNo != "")
            {
                //* 如果有輸入[卡號],則添加查詢條件CardNo LIKE [卡號]%
                Sql.AddCondition(EntitySet_Pay_Certify.M_CardNo, Operator.LikeLeft, DataTypeUtils.String, strCardNo );
            }

            //* 創建DataTable
            DataTable dtblReport0302020002 = new DataTable();
            dtblReport0302020002.Columns.Add(new DataColumn("SerialNo", Type.GetType("System.String")));
            dtblReport0302020002.Columns.Add(new DataColumn("UserID", Type.GetType("System.String")));
            dtblReport0302020002.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
            dtblReport0302020002.Columns.Add(new DataColumn("CardNo", Type.GetType("System.String")));
            dtblReport0302020002.Columns.Add(new DataColumn("LoanDay", Type.GetType("System.String")));
            dtblReport0302020002.Columns.Add(new DataColumn("SettleAmt", Type.GetType("System.Int32")));
            dtblReport0302020002.Columns.Add(new DataColumn("SettleDay", Type.GetType("System.String")));
            dtblReport0302020002.Columns.Add(new DataColumn("SelfDay", Type.GetType("System.String")));
            dtblReport0302020002.Columns.Add(new DataColumn("OtherDay", Type.GetType("System.String")));
            dtblReport0302020002.Columns.Add(new DataColumn("sNote", Type.GetType("System.String")));

            try
            {
                //* 按證明編號起、證明編號迄、身分證字號、自取日期、卡號等查詢數據 
                EntitySet<EntitySet_Pay_Certify> esReport0302020002 = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(Sql.GetFilterCondition());
                if (esReport0302020002.TotalCount > 0)
                {
                    for (int intLoop = 0; intLoop < esReport0302020002.TotalCount; intLoop++)
                    {
                        DataRow drowNewInsert = dtblReport0302020002.NewRow();
                        drowNewInsert["SerialNo"] = ((EntitySet_Pay_Certify)esReport0302020002.GetEntity(intLoop)).serialNo;
                        drowNewInsert["UserID"] = ((EntitySet_Pay_Certify)esReport0302020002.GetEntity(intLoop)).userID;
                        drowNewInsert["UserName"] = ((EntitySet_Pay_Certify)esReport0302020002.GetEntity(intLoop)).userName;
                        drowNewInsert["CardNo"] = ((EntitySet_Pay_Certify)esReport0302020002.GetEntity(intLoop)).CardNo;
                        drowNewInsert["LoanDay"] = Function.InsertTimeSpan(((EntitySet_Pay_Certify)esReport0302020002.GetEntity(intLoop)).LoanDate);
                        drowNewInsert["SettleAmt"] = ((EntitySet_Pay_Certify)esReport0302020002.GetEntity(intLoop)).PayOffAmt;
                        drowNewInsert["SettleDay"] = Function.InsertTimeSpan(((EntitySet_Pay_Certify)esReport0302020002.GetEntity(intLoop)).PayOffDate);
                        drowNewInsert["SelfDay"] = Function.InsertTimeSpan(((EntitySet_Pay_Certify)esReport0302020002.GetEntity(intLoop)).mailDay);
                        drowNewInsert["OtherDay"] = "";
                        drowNewInsert["sNote"] = ((EntitySet_Pay_Certify)esReport0302020002.GetEntity(intLoop)).note;
                        dtblReport0302020002.Rows.Add(drowNewInsert);
                    }
                }

                dtblReport0302020002.TableName = "SelfOther";
                dstResult.Tables.Add(dtblReport0302020002.DefaultView.ToTable());
                return true;
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp);
                strMsgID = "04_03020200_009";
                return false;
            }
        }

        /// <summary>
        /// 自取簽收表(結清證明)
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strUserID">身分證字號</param>
        /// <param name="strMailDay">自取日期</param>
        /// <param name="strCardNo">卡號</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="rptResult">返回報表對象</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0302020003(string strStartSerialNo, string strEndSerialNo, string strUserID,
                    string strMailDay, string strCardNo, ref string strMsgID, ref  DataSet dstResult)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_isMail, Operator.Equal, DataTypeUtils.String, "N");
            //* 添加查詢條件Type等於0 - 結清, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "0");

            //* 如果有輸入證明編號
            if (strStartSerialNo != "" || strEndSerialNo != "")
            {
                if (strStartSerialNo == "")
                {
                    strStartSerialNo = strEndSerialNo;
                }
                if (strEndSerialNo == "")
                {
                    strEndSerialNo = strStartSerialNo;
                }
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
            }
            if (strUserID != "")
            {
                //* 如果有輸入[身份證字號],則添加查詢條件UserID LIKE [身份證字號]%
                Sql.AddCondition(EntitySet_Pay_Certify.M_userID, Operator.LikeLeft, DataTypeUtils.String, strUserID);
            }
            if (strMailDay != "")
            {
                //* 如果有輸入[自取日期],則添加查詢條件MailDay = [自取日期]
                Sql.AddCondition(EntitySet_Pay_Certify.M_mailDay, Operator.Equal, DataTypeUtils.String, strMailDay);
            }
            if (strCardNo != "")
            {
                //* 如果有輸入[卡號],則添加查詢條件CardNo LIKE [卡號]%
                Sql.AddCondition(EntitySet_Pay_Certify.M_CardNo, Operator.LikeLeft, DataTypeUtils.String, strCardNo);
            }

            //* 創建DataTable
            DataTable dtblReport0302020003 = new DataTable();
            dtblReport0302020003.Columns.Add(new DataColumn("SerialNo", Type.GetType("System.String")));
            dtblReport0302020003.Columns.Add(new DataColumn("UserID", Type.GetType("System.String")));
            dtblReport0302020003.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
            dtblReport0302020003.Columns.Add(new DataColumn("CardNo", Type.GetType("System.String")));
            dtblReport0302020003.Columns.Add(new DataColumn("LoanDay", Type.GetType("System.String")));
            dtblReport0302020003.Columns.Add(new DataColumn("SettleAmt", Type.GetType("System.Int32")));
            dtblReport0302020003.Columns.Add(new DataColumn("SettleDay", Type.GetType("System.String")));
            dtblReport0302020003.Columns.Add(new DataColumn("SelfDay", Type.GetType("System.String")));
            dtblReport0302020003.Columns.Add(new DataColumn("OtherDay", Type.GetType("System.String")));
            dtblReport0302020003.Columns.Add(new DataColumn("sNote", Type.GetType("System.String")));

            try
            {
                //* 按證明編號起、證明編號迄、身分證字號、自取日期、卡號等查詢數據 
                EntitySet<EntitySet_Pay_Certify> esReport0302020003 = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(Sql.GetFilterCondition());
                if (esReport0302020003.TotalCount > 0)
                {
                    for (int intLoop = 0; intLoop < esReport0302020003.TotalCount; intLoop++)
                    {
                        DataRow drowNewInsert = dtblReport0302020003.NewRow();
                        drowNewInsert["SerialNo"] = ((EntitySet_Pay_Certify)esReport0302020003.GetEntity(intLoop)).serialNo;
                        drowNewInsert["UserID"] = ((EntitySet_Pay_Certify)esReport0302020003.GetEntity(intLoop)).userID;
                        drowNewInsert["UserName"] = ((EntitySet_Pay_Certify)esReport0302020003.GetEntity(intLoop)).userName;
                        drowNewInsert["CardNo"] = ((EntitySet_Pay_Certify)esReport0302020003.GetEntity(intLoop)).CardNo;
                        drowNewInsert["LoanDay"] = Function.InsertTimeSpan(((EntitySet_Pay_Certify)esReport0302020003.GetEntity(intLoop)).LoanDate);
                        drowNewInsert["SettleAmt"] = ((EntitySet_Pay_Certify)esReport0302020003.GetEntity(intLoop)).PayOffAmt;
                        drowNewInsert["SettleDay"] = Function.InsertTimeSpan(((EntitySet_Pay_Certify)esReport0302020003.GetEntity(intLoop)).PayOffDate);
                        drowNewInsert["SelfDay"] = Function.InsertTimeSpan(((EntitySet_Pay_Certify)esReport0302020003.GetEntity(intLoop)).mailDay);
                        drowNewInsert["OtherDay"] = "";
                        drowNewInsert["sNote"] = ((EntitySet_Pay_Certify)esReport0302020003.GetEntity(intLoop)).note;
                        dtblReport0302020003.Rows.Add(drowNewInsert);
                    }
                }

                dtblReport0302020003.TableName = "SelfOther";
                dstResult.Tables.Add(dtblReport0302020003.DefaultView.ToTable());
                return true;
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp);
                strMsgID = "04_03020200_011";
                return false;
            }
        }

        /// <summary>
        /// 自取簽收總表(代償證明)
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strUserID">身分證字號</param>
        /// <param name="strMailDay">自取日期</param>
        /// <param name="strCardNo">卡號</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="rptResult">返回報表對象</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0402020002(string strStartSerialNo, string strEndSerialNo, string strUserID,
                    string strMailDay, string strCardNo, ref string strMsgID, ref  DataSet dstResult)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_isMail, Operator.Equal, DataTypeUtils.String, "N");
            //* 添加查詢條件Type等於0 - 結清, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "1");

            //* 如果有輸入證明編號
            if (strStartSerialNo != "" || strEndSerialNo != "")
            {
                if (strStartSerialNo == "")
                {
                    strStartSerialNo = strEndSerialNo;
                }
                if (strEndSerialNo == "")
                {
                    strEndSerialNo = strStartSerialNo;
                }
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
            }
            if (strUserID != "")
            {
                //* 如果有輸入[身份證字號],則添加查詢條件UserID LIKE [身份證字號]%
                Sql.AddCondition(EntitySet_Pay_Certify.M_userID, Operator.LikeLeft, DataTypeUtils.String, strUserID);
            }
            if (strMailDay != "")
            {
                //* 如果有輸入[自取日期],則添加查詢條件MailDay = [自取日期]
                Sql.AddCondition(EntitySet_Pay_Certify.M_mailDay, Operator.Equal, DataTypeUtils.String, strMailDay);
            }
            if (strCardNo != "")
            {
                //* 如果有輸入[卡號],則添加查詢條件CardNo LIKE [卡號]%
                Sql.AddCondition(EntitySet_Pay_Certify.M_CardNo, Operator.LikeLeft, DataTypeUtils.String, strCardNo);
            }

            //* 創建DataTable
            DataTable dtblReport0402020002 = new DataTable();
            dtblReport0402020002.Columns.Add(new DataColumn("SerialNo", Type.GetType("System.String")));
            dtblReport0402020002.Columns.Add(new DataColumn("UserID", Type.GetType("System.String")));
            dtblReport0402020002.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
            dtblReport0402020002.Columns.Add(new DataColumn("CardNo", Type.GetType("System.String")));
            dtblReport0402020002.Columns.Add(new DataColumn("LoanDay", Type.GetType("System.String")));
            dtblReport0402020002.Columns.Add(new DataColumn("SettleAmt", Type.GetType("System.Int32")));
            dtblReport0402020002.Columns.Add(new DataColumn("SettleDay", Type.GetType("System.String")));
            dtblReport0402020002.Columns.Add(new DataColumn("SelfDay", Type.GetType("System.String")));
            dtblReport0402020002.Columns.Add(new DataColumn("OtherDay", Type.GetType("System.String")));
            dtblReport0402020002.Columns.Add(new DataColumn("sNote", Type.GetType("System.String")));

            try
            {
                //* 按證明編號起、證明編號迄、身分證字號、自取日期、卡號等查詢數據 
                EntitySet<EntitySet_Pay_Certify> esReport0402020002 = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(Sql.GetFilterCondition());
                if (esReport0402020002.TotalCount > 0)
                {
                    for (int intLoop = 0; intLoop < esReport0402020002.TotalCount; intLoop++)
                    {
                        DataRow drowNewInsert = dtblReport0402020002.NewRow();
                        drowNewInsert["SerialNo"] = ((EntitySet_Pay_Certify)esReport0402020002.GetEntity(intLoop)).serialNo;
                        drowNewInsert["UserID"] = ((EntitySet_Pay_Certify)esReport0402020002.GetEntity(intLoop)).userID;
                        drowNewInsert["UserName"] = ((EntitySet_Pay_Certify)esReport0402020002.GetEntity(intLoop)).userName;
                        drowNewInsert["CardNo"] = ((EntitySet_Pay_Certify)esReport0402020002.GetEntity(intLoop)).CardNo;
                        drowNewInsert["LoanDay"] = Function.InsertTimeSpan(((EntitySet_Pay_Certify)esReport0402020002.GetEntity(intLoop)).LoanDate);
                        drowNewInsert["SettleAmt"] = ((EntitySet_Pay_Certify)esReport0402020002.GetEntity(intLoop)).PayOffAmt;
                        drowNewInsert["SettleDay"] = Function.InsertTimeSpan(((EntitySet_Pay_Certify)esReport0402020002.GetEntity(intLoop)).PayOffDate);
                        drowNewInsert["SelfDay"] = Function.InsertTimeSpan(((EntitySet_Pay_Certify)esReport0402020002.GetEntity(intLoop)).mailDay);
                        drowNewInsert["OtherDay"] = "";
                        drowNewInsert["sNote"] = ((EntitySet_Pay_Certify)esReport0402020002.GetEntity(intLoop)).note;
                        dtblReport0402020002.Rows.Add(drowNewInsert);
                    }
                }

                dtblReport0402020002.TableName = "SelfOther";
                dstResult.Tables.Add(dtblReport0402020002.DefaultView.ToTable());
                return true;
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp);
                strMsgID = "04_04020200_009";
                return false;
            }
        }

        /// <summary>
        /// 自取簽收表(代償證明)
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strUserID">身分證字號</param>
        /// <param name="strMailDay">自取日期</param>
        /// <param name="strCardNo">卡號</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="rptResult">返回報表對象</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0402020003(string strStartSerialNo, string strEndSerialNo, string strUserID,
                    string strMailDay, string strCardNo, ref string strMsgID, ref  DataSet dstResult)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_isMail, Operator.Equal, DataTypeUtils.String, "N");
            //* 添加查詢條件Type等於0 - 結清, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "1");

            //* 如果有輸入證明編號
            if (strStartSerialNo != "" || strEndSerialNo != "")
            {
                if (strStartSerialNo == "")
                {
                    strStartSerialNo = strEndSerialNo;
                }
                if (strEndSerialNo == "")
                {
                    strEndSerialNo = strStartSerialNo;
                }
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
            }
            if (strUserID != "")
            {
                //* 如果有輸入[身份證字號],則添加查詢條件UserID LIKE [身份證字號]%
                Sql.AddCondition(EntitySet_Pay_Certify.M_userID, Operator.LikeLeft, DataTypeUtils.String, strUserID);
            }
            if (strMailDay != "")
            {
                //* 如果有輸入[自取日期],則添加查詢條件MailDay = [自取日期]
                Sql.AddCondition(EntitySet_Pay_Certify.M_mailDay, Operator.Equal, DataTypeUtils.String, strMailDay);
            }
            if (strCardNo != "")
            {
                //* 如果有輸入[卡號],則添加查詢條件CardNo LIKE [卡號]%
                Sql.AddCondition(EntitySet_Pay_Certify.M_CardNo, Operator.LikeLeft, DataTypeUtils.String, strCardNo);
            }

            //* 創建DataTable
            DataTable dtblReport0402020003 = new DataTable();
            dtblReport0402020003.Columns.Add(new DataColumn("SerialNo", Type.GetType("System.String")));
            dtblReport0402020003.Columns.Add(new DataColumn("UserID", Type.GetType("System.String")));
            dtblReport0402020003.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
            dtblReport0402020003.Columns.Add(new DataColumn("CardNo", Type.GetType("System.String")));
            dtblReport0402020003.Columns.Add(new DataColumn("LoanDay", Type.GetType("System.String")));
            dtblReport0402020003.Columns.Add(new DataColumn("SettleAmt", Type.GetType("System.Int32")));
            dtblReport0402020003.Columns.Add(new DataColumn("SettleDay", Type.GetType("System.String")));
            dtblReport0402020003.Columns.Add(new DataColumn("SelfDay", Type.GetType("System.String")));
            dtblReport0402020003.Columns.Add(new DataColumn("OtherDay", Type.GetType("System.String")));
            dtblReport0402020003.Columns.Add(new DataColumn("sNote", Type.GetType("System.String")));

            try
            {
                //* 按證明編號起、證明編號迄、身分證字號、自取日期、卡號等查詢數據 
                EntitySet<EntitySet_Pay_Certify> esReport0402020003 = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(Sql.GetFilterCondition());
                if (esReport0402020003.TotalCount > 0)
                {
                    for (int intLoop = 0; intLoop < esReport0402020003.TotalCount; intLoop++)
                    {
                        DataRow drowNewInsert = dtblReport0402020003.NewRow();
                        drowNewInsert["SerialNo"] = ((EntitySet_Pay_Certify)esReport0402020003.GetEntity(intLoop)).serialNo;
                        drowNewInsert["UserID"] = ((EntitySet_Pay_Certify)esReport0402020003.GetEntity(intLoop)).userID;
                        drowNewInsert["UserName"] = ((EntitySet_Pay_Certify)esReport0402020003.GetEntity(intLoop)).userName;
                        drowNewInsert["CardNo"] = ((EntitySet_Pay_Certify)esReport0402020003.GetEntity(intLoop)).CardNo;
                        drowNewInsert["LoanDay"] = Function.InsertTimeSpan(((EntitySet_Pay_Certify)esReport0402020003.GetEntity(intLoop)).LoanDate);
                        drowNewInsert["SettleAmt"] = ((EntitySet_Pay_Certify)esReport0402020003.GetEntity(intLoop)).PayOffAmt;
                        drowNewInsert["SettleDay"] = Function.InsertTimeSpan(((EntitySet_Pay_Certify)esReport0402020003.GetEntity(intLoop)).PayOffDate);
                        drowNewInsert["SelfDay"] = Function.InsertTimeSpan(((EntitySet_Pay_Certify)esReport0402020003.GetEntity(intLoop)).mailDay);
                        drowNewInsert["OtherDay"] = "";
                        drowNewInsert["sNote"] = ((EntitySet_Pay_Certify)esReport0402020003.GetEntity(intLoop)).note;
                        dtblReport0402020003.Rows.Add(drowNewInsert);
                    }
                }

                dtblReport0402020003.TableName = "SelfOther";
                dstResult.Tables.Add(dtblReport0402020003.DefaultView.ToTable());
                return true;
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp);
                strMsgID = "04_04020200_011";
                return false;
            }
        }
        /// <summary>
        /// 列印結清證明書
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strUserID">身分證字號</param>
        /// <param name="strMailDay">自取日期</param>
        /// <param name="strCardNo">卡號</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="rptResult">返回報表對象</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0302040002(string strStartSerialNo, string strEndSerialNo, string strUserID,
                    string strMailDay, string strCardNo, ref string strMsgID, ref  DataSet dstResult)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
            //* 添加查詢條件Type等於0 - 結清, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "0");

            //* 如果有輸入證明編號
            if (strStartSerialNo != "" || strEndSerialNo != "")
            {
                if (strStartSerialNo == "")
                {
                    strStartSerialNo = strEndSerialNo;
                }
                if (strEndSerialNo == "")
                {
                    strEndSerialNo = strStartSerialNo;
                }
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
            }
            if (strUserID != "")
            {
                //* 如果有輸入[身份證字號],則添加查詢條件UserID LIKE [身份證字號]%
                Sql.AddCondition(EntitySet_Pay_Certify.M_userID, Operator.LikeLeft, DataTypeUtils.String, strUserID);
            }
            if (strMailDay != "")
            {
                //* 如果有輸入[自取日期],則添加查詢條件MailDay = [自取日期]
                Sql.AddCondition(EntitySet_Pay_Certify.M_mailDay, Operator.Equal, DataTypeUtils.String, strMailDay);
            }
            if (strCardNo != "")
            {
                //* 如果有輸入[卡號],則添加查詢條件CardNo LIKE [卡號]%
                Sql.AddCondition(EntitySet_Pay_Certify.M_CardNo, Operator.LikeLeft, DataTypeUtils.String, strCardNo);
            }
            Sql.AddOrderCondition(EntitySet_Pay_Certify.M_serialNo, ESortType.ASC);
            //* 創建DataTable
            DataTable dtblReport = new DataTable();
            dtblReport.Columns.Add(new DataColumn("SerialNo", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("UserID", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("CardNo", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("Zip", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("add1", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("add2", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("add3", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("type", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("keyinDay", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("RecvName", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("LoanDate", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("PayOffDate", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("PayOffAmt", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("payName", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("pay", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("payDay", Type.GetType("System.String")));

            try
            {
                //* 按證明編號起、證明編號迄、身分證字號、自取日期、卡號等查詢數據 
                EntitySet<EntitySet_Pay_Certify> esReport = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(Sql.GetFilterCondition());
                if (esReport.TotalCount > 0)
                {
                    for (int intLoop = 0; intLoop < esReport.TotalCount; intLoop++)
                    {
                        string strDate = esReport.GetEntity(intLoop).keyinDay.PadRight(8);
                        strDate = (Convert.ToDouble(strDate.Substring(0, 4)) - 1911).ToString() + "年" + strDate.Substring(4, 2) + "月" + strDate.Substring(6, 2) + "日";
                        DataRow drowNewInsert = dtblReport.NewRow();
                        drowNewInsert["SerialNo"] = esReport.GetEntity(intLoop).serialNo;
                        drowNewInsert["UserID"] = esReport.GetEntity(intLoop).userID;
                        drowNewInsert["UserName"] = esReport.GetEntity(intLoop).userName;
                        drowNewInsert["CardNo"] = esReport.GetEntity(intLoop).CardNo;
                        drowNewInsert["Zip"] = esReport.GetEntity(intLoop).zip;
                        drowNewInsert["add1"] = esReport.GetEntity(intLoop).add1;
                        drowNewInsert["add2"] = esReport.GetEntity(intLoop).add2;
                        drowNewInsert["add3"] = esReport.GetEntity(intLoop).add3;
                        drowNewInsert["type"] = "0";
                        drowNewInsert["keyinDay"] = strDate;
                        drowNewInsert["RecvName"] = esReport.GetEntity(intLoop).RecvName;
                        drowNewInsert["LoanDate"] = Function.InsertTimeSpan(esReport.GetEntity(intLoop).LoanDate);
                        drowNewInsert["PayOffDate"] = Function.InsertTimeSpan(esReport.GetEntity(intLoop).PayOffDate);
                        drowNewInsert["PayOffAmt"] = TransChr1(esReport.GetEntity(intLoop).PayOffAmt.ToString());
                        drowNewInsert["payName"] = "";
                        drowNewInsert["pay"] = "0";
                        drowNewInsert["payDay"] = "";
                        dtblReport.Rows.Add(drowNewInsert);
                    }
                }

                dtblReport.TableName = "Certify";
                dstResult.Tables.Add(dtblReport.DefaultView.ToTable());
                return true;
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp);
                strMsgID = "04_03020200_009";
                return false;
            }
        }
        /// <summary>
        /// 列印代償證明書
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strUserID">身分證字號</param>
        /// <param name="strMailDay">自取日期</param>
        /// <param name="strCardNo">卡號</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="rptResult">返回報表對象</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0402040002(string strStartSerialNo, string strEndSerialNo, string strUserID,
                    string strMailDay, string strCardNo, ref string strMsgID, ref  DataSet dstResult)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
            //* 添加查詢條件Type等於0 - 結清, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "1");

            //* 如果有輸入證明編號
            if (strStartSerialNo != "" || strEndSerialNo != "")
            {
                if (strStartSerialNo == "")
                {
                    strStartSerialNo = strEndSerialNo;
                }
                if (strEndSerialNo == "")
                {
                    strEndSerialNo = strStartSerialNo;
                }
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
            }
            if (strUserID != "")
            {
                //* 如果有輸入[身份證字號],則添加查詢條件UserID LIKE [身份證字號]%
                Sql.AddCondition(EntitySet_Pay_Certify.M_userID, Operator.LikeLeft, DataTypeUtils.String, strUserID);
            }
            if (strMailDay != "")
            {
                //* 如果有輸入[自取日期],則添加查詢條件MailDay = [自取日期]
                Sql.AddCondition(EntitySet_Pay_Certify.M_mailDay, Operator.Equal, DataTypeUtils.String, strMailDay);
            }
            if (strCardNo != "")
            {
                //* 如果有輸入[卡號],則添加查詢條件CardNo LIKE [卡號]%
                Sql.AddCondition(EntitySet_Pay_Certify.M_CardNo, Operator.LikeLeft, DataTypeUtils.String, strCardNo);
            }
            Sql.AddOrderCondition(EntitySet_Pay_Certify.M_serialNo, ESortType.ASC);
            //* 創建DataTable
            DataTable dtblReport = new DataTable();
            dtblReport.Columns.Add(new DataColumn("SerialNo", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("UserID", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("CardNo", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("Zip", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("add1", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("add2", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("add3", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("type", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("keyinDay", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("RecvName", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("LoanDate", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("PayOffDate", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("PayOffAmt", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("payName", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("pay", Type.GetType("System.String")));
            dtblReport.Columns.Add(new DataColumn("payDay", Type.GetType("System.String")));

            try
            {
                //* 按證明編號起、證明編號迄、身分證字號、自取日期、卡號等查詢數據 
                EntitySet<EntitySet_Pay_Certify> esReport = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(Sql.GetFilterCondition());
                if (esReport.TotalCount > 0)
                {
                    for (int intLoop = 0; intLoop < esReport.TotalCount; intLoop++)
                    {
                        string strDate = esReport.GetEntity(intLoop).keyinDay.PadRight(8);
                        strDate = (Convert.ToDouble(strDate.Substring(0, 4)) - 1911).ToString() + "年" + strDate.Substring(4, 2) + "月" + strDate.Substring(6, 2) + "日";
                        DataRow drowNewInsert = dtblReport.NewRow();
                        drowNewInsert["SerialNo"] = esReport.GetEntity(intLoop).serialNo;
                        drowNewInsert["UserID"] = esReport.GetEntity(intLoop).userID;
                        drowNewInsert["UserName"] = esReport.GetEntity(intLoop).userName;
                        drowNewInsert["CardNo"] = esReport.GetEntity(intLoop).CardNo;
                        drowNewInsert["Zip"] = esReport.GetEntity(intLoop).zip;
                        drowNewInsert["add1"] = esReport.GetEntity(intLoop).add1;
                        drowNewInsert["add2"] = esReport.GetEntity(intLoop).add2;
                        drowNewInsert["add3"] = esReport.GetEntity(intLoop).add3;
                        drowNewInsert["type"] = "1";
                        drowNewInsert["keyinDay"] = strDate;
                        drowNewInsert["RecvName"] = esReport.GetEntity(intLoop).RecvName;
                        drowNewInsert["LoanDate"] = Function.InsertTimeSpan(esReport.GetEntity(intLoop).LoanDate);
                        drowNewInsert["PayOffDate"] = Function.InsertTimeSpan(esReport.GetEntity(intLoop).PayOffDate);
                        drowNewInsert["PayOffAmt"] = TransChr1(esReport.GetEntity(intLoop).PayOffAmt.ToString());
                        drowNewInsert["payName"] = esReport.GetEntity(intLoop).payName;
                        drowNewInsert["pay"] = TransChr1(esReport.GetEntity(intLoop).pay.ToString());
                        drowNewInsert["payDay"] = Function.InsertTimeSpan(esReport.GetEntity(intLoop).payDay);
                        dtblReport.Rows.Add(drowNewInsert);
                    }

                }

                dtblReport.TableName = "Certify";
                dstResult.Tables.Add(dtblReport.DefaultView.ToTable());
                return true;
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp);
                strMsgID = "04_03020200_009";
                return false;
            }
        }

        /// <summary>
        /// 列印大宗掛號函存根聯(結清證明)
        /// </summary>
        /// <param name="strMailDay">郵寄日期</param>
        /// <param name="strMailType">郵寄方式</param>
        /// <param name="strLogonUserName">登入者姓名</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="rptResult">返回報表對象</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0302060002(string strMailDay, string strMailType, string strLogonUserName,
                    ref string strMsgID, ref  DataSet dstResult)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
            //* 添加查詢條件Type等於0 - 結清, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "0");
            //添加查詢條件isMail等於Y
            Sql.AddCondition(EntitySet_Pay_Certify.M_isMail, Operator.Equal, DataTypeUtils.String, "Y");

            //* 如果有輸入郵寄日期，MailDay = [郵寄日期]
            if (strMailDay != "")
            {
                Sql.AddCondition(EntitySet_Pay_Certify.M_mailDay, Operator.Equal, DataTypeUtils.String, strMailDay);
            }

            //* 郵寄方式
            if (strMailType != "0")
            {
                //* 掛號或限時掛號
                Sql.AddCondition(EntitySet_Pay_Certify.M_MailType, Operator.Equal, DataTypeUtils.String, strMailType);
            }
            else
            {
                //* 全部
                Sql.AddCondition(EntitySet_Pay_Certify.M_MailType, Operator.In, DataTypeUtils.String, "2,3");
            }

            //* 創建DataTable
            DataTable dtblReport0302060002 = new DataTable();
            dtblReport0302060002.Columns.Add(new DataColumn("SerialNo", Type.GetType("System.String")));
            dtblReport0302060002.Columns.Add(new DataColumn("MailNo", Type.GetType("System.String")));
            dtblReport0302060002.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
            dtblReport0302060002.Columns.Add(new DataColumn("Addr", Type.GetType("System.String")));
            dtblReport0302060002.Columns.Add(new DataColumn("sNote", Type.GetType("System.String")));
            dtblReport0302060002.Columns.Add(new DataColumn("MailDay", Type.GetType("System.String")));

            try
            {
                //* 郵寄日期和郵寄方式
                EntitySet<EntitySet_Pay_Certify> esReport0302060002 = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(Sql.GetFilterCondition());
                if (esReport0302060002.TotalCount > 0)
                {
                    for (int intLoop = 0; intLoop < esReport0302060002.TotalCount; intLoop++)
                    {
                        DataRow drowNewInsert = dtblReport0302060002.NewRow();
                        drowNewInsert["SerialNo"] = ((EntitySet_Pay_Certify)esReport0302060002.GetEntity(intLoop)).serialNo;
                        drowNewInsert["MailNo"] = ((EntitySet_Pay_Certify)esReport0302060002.GetEntity(intLoop)).mailNo;
                        drowNewInsert["UserName"] = ((EntitySet_Pay_Certify)esReport0302060002.GetEntity(intLoop)).userName;
                        drowNewInsert["Addr"] = ((EntitySet_Pay_Certify)esReport0302060002.GetEntity(intLoop)).zip.Trim() + ((EntitySet_Pay_Certify)esReport0302060002.GetEntity(intLoop)).add1.Trim() +
                                                ((EntitySet_Pay_Certify)esReport0302060002.GetEntity(intLoop)).add2.Trim() + ((EntitySet_Pay_Certify)esReport0302060002.GetEntity(intLoop)).add3.Trim();
                        drowNewInsert["sNote"] = ((EntitySet_Pay_Certify)esReport0302060002.GetEntity(intLoop)).note;
                        drowNewInsert["MailDay"] = Function.InsertTimeSpan(((EntitySet_Pay_Certify)esReport0302060002.GetEntity(intLoop)).mailDay);
                        dtblReport0302060002.Rows.Add(drowNewInsert);
                    }
                }

                dtblReport0302060002.TableName = "Mail";
                dstResult.Tables.Add(dtblReport0302060002.DefaultView.ToTable());
                return true;
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp);
                strMsgID = "04_03020600_004";
                return false;
            }
        }

        /// <summary>
        /// 列印大宗掛號函存根聯(代償證明)
        /// </summary>
        /// <param name="strMailDay">郵寄日期</param>
        /// <param name="strMailType">郵寄方式</param>
        /// <param name="strLogonUserName">登入者姓名</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="rptResult">返回報表對象</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0402060002(string strMailDay, string strMailType, string strLogonUserName,
                    ref string strMsgID, ref  DataSet dstResult)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
            //* 添加查詢條件Type等於0 - 結清, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "1");
            //添加查詢條件isMail等於Y
            Sql.AddCondition(EntitySet_Pay_Certify.M_isMail, Operator.Equal, DataTypeUtils.String, "Y");

            //* 如果有輸入郵寄日期，MailDay = [郵寄日期]
            if (strMailDay != "")
            {
                Sql.AddCondition(EntitySet_Pay_Certify.M_mailDay, Operator.Equal, DataTypeUtils.String, strMailDay);
            }

            //* 郵寄方式
            if (strMailType != "0")
            {
                //* 掛號或限時掛號
                Sql.AddCondition(EntitySet_Pay_Certify.M_MailType, Operator.Equal, DataTypeUtils.String, strMailType);
            }
            else
            {
                //* 全部
                Sql.AddCondition(EntitySet_Pay_Certify.M_MailType, Operator.In, DataTypeUtils.String, "2,3");
            }

            //* 創建DataTable
            DataTable dtblReport0402060002 = new DataTable();
            dtblReport0402060002.Columns.Add(new DataColumn("SerialNo", Type.GetType("System.String")));
            dtblReport0402060002.Columns.Add(new DataColumn("MailNo", Type.GetType("System.String")));
            dtblReport0402060002.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
            dtblReport0402060002.Columns.Add(new DataColumn("Addr", Type.GetType("System.String")));
            dtblReport0402060002.Columns.Add(new DataColumn("sNote", Type.GetType("System.String")));
            dtblReport0402060002.Columns.Add(new DataColumn("MailDay", Type.GetType("System.String")));

            try
            {
                //* 郵寄日期和郵寄方式
                EntitySet<EntitySet_Pay_Certify> esReport0402060002 = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(Sql.GetFilterCondition());
                if (esReport0402060002.TotalCount > 0)
                {
                    for (int intLoop = 0; intLoop < esReport0402060002.TotalCount; intLoop++)
                    {
                        DataRow drowNewInsert = dtblReport0402060002.NewRow();
                        drowNewInsert["SerialNo"] = ((EntitySet_Pay_Certify)esReport0402060002.GetEntity(intLoop)).serialNo;
                        drowNewInsert["MailNo"] = ((EntitySet_Pay_Certify)esReport0402060002.GetEntity(intLoop)).mailNo;
                        drowNewInsert["UserName"] = ((EntitySet_Pay_Certify)esReport0402060002.GetEntity(intLoop)).userName;
                        drowNewInsert["Addr"] = ((EntitySet_Pay_Certify)esReport0402060002.GetEntity(intLoop)).zip.Trim() + ((EntitySet_Pay_Certify)esReport0402060002.GetEntity(intLoop)).add1.Trim() +
                                                ((EntitySet_Pay_Certify)esReport0402060002.GetEntity(intLoop)).add2.Trim() + ((EntitySet_Pay_Certify)esReport0402060002.GetEntity(intLoop)).add3.Trim();
                        drowNewInsert["sNote"] = ((EntitySet_Pay_Certify)esReport0402060002.GetEntity(intLoop)).note;
                        drowNewInsert["MailDay"] = Function.InsertTimeSpan(((EntitySet_Pay_Certify)esReport0402060002.GetEntity(intLoop)).mailDay);
                        dtblReport0402060002.Rows.Add(drowNewInsert);
                    }
                }

                dtblReport0402060002.TableName = "Mail";
                dstResult.Tables.Add(dtblReport0402060002.DefaultView.ToTable());
                return true;
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp);
                strMsgID = "04_04020600_004";
                return false;
            }
        }

        /// <summary>
        /// 列印ML結清證明開立登記表(結清證明)
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strUserID">身分證字號</param>
        /// <param name="strMailDay">自取日期</param>
        /// <param name="strCardNo">卡號</param>
        /// <param name="strhidMailDay">MailDay</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="rptResult">返回報表對象</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0302030002(string strStartSerialNo, string strEndSerialNo, string strUserID,
                    string strMailDay, string strCardNo, string strhidMailDay,
                    ref string strMsgID, ref  DataSet dstResult)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
            //* 添加查詢條件Type等於0 - 結清, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.In, DataTypeUtils.String, "0");

            //* 如果有輸入證明編號
            if (strStartSerialNo != "" || strEndSerialNo != "")
            {
                if (strStartSerialNo == "")
                {
                    strStartSerialNo = strEndSerialNo;
                }
                if (strEndSerialNo == "")
                {
                    strEndSerialNo = strStartSerialNo;
                }
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
            }
            if (strUserID != "")
            {
                //* 如果有輸入[身份證字號],則添加查詢條件UserID LIKE [身份證字號]%
                Sql.AddCondition(EntitySet_Pay_Certify.M_userID, Operator.LikeLeft, DataTypeUtils.String, strUserID);
            }
            if (strMailDay != "")
            {
                //* 如果有輸入[自取日期],則添加查詢條件MailDay = [自取日期]
                Sql.AddCondition(EntitySet_Pay_Certify.M_mailDay, Operator.Equal, DataTypeUtils.String, strMailDay);
            }
            if (strCardNo != "")
            {
                //* 如果有輸入[卡號],則添加查詢條件CardNo LIKE [卡號]%
                Sql.AddCondition(EntitySet_Pay_Certify.M_CardNo, Operator.LikeLeft, DataTypeUtils.String, strCardNo);
            }

            Sql.AddOrderCondition(EntitySet_Pay_Certify.M_serialNo, ESortType.ASC);
            //* 創建DataTable
            DataTable dtblReport0302030002 = new DataTable();
            dtblReport0302030002.Columns.Add(new DataColumn("SerialNo", Type.GetType("System.String")));
            dtblReport0302030002.Columns.Add(new DataColumn("UserID", Type.GetType("System.String")));
            dtblReport0302030002.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
            dtblReport0302030002.Columns.Add(new DataColumn("CardNo", Type.GetType("System.String")));
            dtblReport0302030002.Columns.Add(new DataColumn("LoanDay", Type.GetType("System.String")));
            dtblReport0302030002.Columns.Add(new DataColumn("SettleDay", Type.GetType("System.String")));
            dtblReport0302030002.Columns.Add(new DataColumn("LoanAmt", Type.GetType("System.Decimal")));
            dtblReport0302030002.Columns.Add(new DataColumn("Letter", Type.GetType("System.String")));
            dtblReport0302030002.Columns.Add(new DataColumn("Fast", Type.GetType("System.String")));
            dtblReport0302030002.Columns.Add(new DataColumn("Reg", Type.GetType("System.String")));
            dtblReport0302030002.Columns.Add(new DataColumn("LReg", Type.GetType("System.String")));
            dtblReport0302030002.Columns.Add(new DataColumn("Self", Type.GetType("System.String")));
            dtblReport0302030002.Columns.Add(new DataColumn("Addr", Type.GetType("System.String")));
            dtblReport0302030002.Columns.Add(new DataColumn("MailNo", Type.GetType("System.String")));
            dtblReport0302030002.Columns.Add(new DataColumn("BLKCODE", Type.GetType("System.String")));
            dtblReport0302030002.Columns.Add(new DataColumn("KeyinDay", Type.GetType("System.String")));
            dtblReport0302030002.Columns.Add(new DataColumn("MailDay", Type.GetType("System.String")));
            dtblReport0302030002.Columns.Add(new DataColumn("PayDay", Type.GetType("System.String")));
            dtblReport0302030002.Columns.Add(new DataColumn("PayName", Type.GetType("System.String")));
            dtblReport0302030002.Columns.Add(new DataColumn("PayAmt", Type.GetType("System.String")));

            string strKeyInDay = Function.InsertTimeSpan(strStartSerialNo);
            if (strStartSerialNo != strEndSerialNo)
                strKeyInDay += " - " + Function.InsertTimeSpan(strEndSerialNo);
            string strMailDayHid = Function.InsertTimeSpan(strhidMailDay);

            try
            {
                //* 按證明編號起、證明編號迄、身分證字號、自取日期、卡號等查詢數據 
                EntitySet<EntitySet_Pay_Certify> esReport0302030002 = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(Sql.GetFilterCondition());
                if (esReport0302030002.TotalCount > 0)
                {
                    for (int intLoop = 0; intLoop < esReport0302030002.TotalCount; intLoop++)
                    {
                        DataRow drowNewInsert = dtblReport0302030002.NewRow();
                        drowNewInsert["SerialNo"] = ((EntitySet_Pay_Certify)esReport0302030002.GetEntity(intLoop)).serialNo;
                        drowNewInsert["UserID"] = ((EntitySet_Pay_Certify)esReport0302030002.GetEntity(intLoop)).userID;
                        drowNewInsert["UserName"] = ((EntitySet_Pay_Certify)esReport0302030002.GetEntity(intLoop)).userName;
                        drowNewInsert["CardNo"] = ((EntitySet_Pay_Certify)esReport0302030002.GetEntity(intLoop)).CardNo;
                        drowNewInsert["LoanDay"] = Function.InsertTimeSpan(((EntitySet_Pay_Certify)esReport0302030002.GetEntity(intLoop)).LoanDate);
                        drowNewInsert["SettleDay"] = Function.InsertTimeSpan(((EntitySet_Pay_Certify)esReport0302030002.GetEntity(intLoop)).PayOffDate);
                        drowNewInsert["LoanAmt"] = ((EntitySet_Pay_Certify)esReport0302030002.GetEntity(intLoop)).PayOffAmt;
                        if (((EntitySet_Pay_Certify)esReport0302030002.GetEntity(intLoop)).isMail == "N")
                        {
                            drowNewInsert["Letter"] = "";
                            drowNewInsert["Fast"] = "";
                            drowNewInsert["Reg"] = "";
                            drowNewInsert["LReg"] = "";
                            drowNewInsert["Self"] = "V";
                        }
                        else if (((EntitySet_Pay_Certify)esReport0302030002.GetEntity(intLoop)).isMail == "Y")
                        {
                            switch (((EntitySet_Pay_Certify)esReport0302030002.GetEntity(intLoop)).MailType)
                            {
                                case "0":
                                    drowNewInsert["Letter"] = "V";
                                    drowNewInsert["Fast"] = "";
                                    drowNewInsert["Reg"] = "";
                                    drowNewInsert["LReg"] = "";
                                    drowNewInsert["Self"] = "";
                                    break;
                                case "1":
                                    drowNewInsert["Letter"] = "";
                                    drowNewInsert["Fast"] = "V";
                                    drowNewInsert["Reg"] = "";
                                    drowNewInsert["LReg"] = "";
                                    drowNewInsert["Self"] = "";
                                    break;
                                case "2":
                                    drowNewInsert["Letter"] = "";
                                    drowNewInsert["Fast"] = "";
                                    drowNewInsert["Reg"] = "V";
                                    drowNewInsert["LReg"] = "";
                                    drowNewInsert["Self"] = "";
                                    break;
                                case "3":
                                    drowNewInsert["Letter"] = "";
                                    drowNewInsert["Fast"] = "";
                                    drowNewInsert["Reg"] = "";
                                    drowNewInsert["LReg"] = "V";
                                    drowNewInsert["Self"] = "";
                                    break;
                            }
                        }
                        drowNewInsert["Addr"] = ((EntitySet_Pay_Certify)esReport0302030002.GetEntity(intLoop)).zip.Trim() + ((EntitySet_Pay_Certify)esReport0302030002.GetEntity(intLoop)).add1.Trim() +
                                                ((EntitySet_Pay_Certify)esReport0302030002.GetEntity(intLoop)).add2.Trim() + ((EntitySet_Pay_Certify)esReport0302030002.GetEntity(intLoop)).add3.Trim();
                        drowNewInsert["MailNo"] = ((EntitySet_Pay_Certify)esReport0302030002.GetEntity(intLoop)).mailNo;
                        drowNewInsert["BLKCODE"] = ((EntitySet_Pay_Certify)esReport0302030002.GetEntity(intLoop)).BLK_Code;
                        drowNewInsert["KeyinDay"] = strKeyInDay;
                        drowNewInsert["MailDay"] = strMailDayHid;
                        drowNewInsert["PayDay"] = "";
                        drowNewInsert["PayName"] = "";
                        drowNewInsert["PayAmt"] = "";
                        dtblReport0302030002.Rows.Add(drowNewInsert);
                    }
                }

                dtblReport0302030002.TableName = "KeyinList";
                dstResult.Tables.Add(dtblReport0302030002.DefaultView.ToTable());
                return true;
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp);
                strMsgID = "04_03020300_009";
                return false;
            }
        }

        /// <summary>
        /// 列印ML代償證明開立登記表(代償證明)
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strUserID">身分證字號</param>
        /// <param name="strMailDay">自取日期</param>
        /// <param name="strCardNo">卡號</param>
        /// <param name="strhidMailDay">MailDay</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="rptResult">返回報表對象</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0402030002(string strStartSerialNo, string strEndSerialNo, string strUserID,
                    string strMailDay, string strCardNo, string strhidMailDay,
                    ref string strMsgID, ref  DataSet dstResult)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
            //* 添加查詢條件Type等於0 - 結清, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "1");

            //* 如果有輸入證明編號
            if (strStartSerialNo != "" || strEndSerialNo != "")
            {
                if (strStartSerialNo == "")
                {
                    strStartSerialNo = strEndSerialNo;
                }
                if (strEndSerialNo == "")
                {
                    strEndSerialNo = strStartSerialNo;
                }
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
            }
            if (strUserID != "")
            {
                //* 如果有輸入[身份證字號],則添加查詢條件UserID LIKE [身份證字號]%
                Sql.AddCondition(EntitySet_Pay_Certify.M_userID, Operator.LikeLeft, DataTypeUtils.String, strUserID);
            }
            if (strMailDay != "")
            {
                //* 如果有輸入[自取日期],則添加查詢條件MailDay = [自取日期]
                Sql.AddCondition(EntitySet_Pay_Certify.M_mailDay, Operator.Equal, DataTypeUtils.String, strMailDay);
            }
            if (strCardNo != "")
            {
                //* 如果有輸入[卡號],則添加查詢條件CardNo LIKE [卡號]%
                Sql.AddCondition(EntitySet_Pay_Certify.M_CardNo, Operator.LikeLeft, DataTypeUtils.String, strCardNo);
            }
            Sql.AddOrderCondition(EntitySet_Pay_Certify.M_serialNo, ESortType.ASC);
            //* 創建DataTable
            DataTable dtblReport0402030002 = new DataTable();
            dtblReport0402030002.Columns.Add(new DataColumn("SerialNo", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("UserID", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("CardNo", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("LoanDay", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("SettleDay", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("LoanAmt", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("Letter", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("Fast", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("Reg", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("LReg", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("Self", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("Addr", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("MailNo", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("BLKCODE", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("KeyinDay", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("MailDay", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("PayDay", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("PayName", Type.GetType("System.String")));
            dtblReport0402030002.Columns.Add(new DataColumn("PayAmt", Type.GetType("System.Int32")));

            string strKeyInDay = Function.InsertTimeSpan(strStartSerialNo);
            if (strStartSerialNo != strEndSerialNo)
                strKeyInDay += " - " + Function.InsertTimeSpan(strEndSerialNo);
            string strMailDayHid = Function.InsertTimeSpan(strhidMailDay);

            try
            {
                //* 按證明編號起、證明編號迄、身分證字號、自取日期、卡號等查詢數據 
                EntitySet<EntitySet_Pay_Certify> esReport0402030002 = (EntitySet<EntitySet_Pay_Certify>)BRSet_Pay_Certify.Search(Sql.GetFilterCondition());
                if (esReport0402030002.TotalCount > 0)
                {
                    for (int intLoop = 0; intLoop < esReport0402030002.TotalCount; intLoop++)
                    {
                        DataRow drowNewInsert = dtblReport0402030002.NewRow();
                        drowNewInsert["SerialNo"] = ((EntitySet_Pay_Certify)esReport0402030002.GetEntity(intLoop)).serialNo;
                        drowNewInsert["UserID"] = ((EntitySet_Pay_Certify)esReport0402030002.GetEntity(intLoop)).userID;
                        drowNewInsert["UserName"] = ((EntitySet_Pay_Certify)esReport0402030002.GetEntity(intLoop)).userName;
                        drowNewInsert["CardNo"] = ((EntitySet_Pay_Certify)esReport0402030002.GetEntity(intLoop)).CardNo;
                        drowNewInsert["LoanDay"] = Function.InsertTimeSpan(((EntitySet_Pay_Certify)esReport0402030002.GetEntity(intLoop)).LoanDate);
                        drowNewInsert["SettleDay"] = "";
                        drowNewInsert["LoanAmt"] = "0";
                        if (((EntitySet_Pay_Certify)esReport0402030002.GetEntity(intLoop)).isMail == "N")
                        {
                            drowNewInsert["Letter"] = "";
                            drowNewInsert["Fast"] = "";
                            drowNewInsert["Reg"] = "";
                            drowNewInsert["LReg"] = "";
                            drowNewInsert["Self"] = "V";
                        }
                        else if (((EntitySet_Pay_Certify)esReport0402030002.GetEntity(intLoop)).isMail == "Y")
                        {
                            switch (((EntitySet_Pay_Certify)esReport0402030002.GetEntity(intLoop)).MailType)
                            {
                                case "0":
                                    drowNewInsert["Letter"] = "V";
                                    drowNewInsert["Fast"] = "";
                                    drowNewInsert["Reg"] = "";
                                    drowNewInsert["LReg"] = "";
                                    drowNewInsert["Self"] = "";
                                    break;
                                case "1":
                                    drowNewInsert["Letter"] = "";
                                    drowNewInsert["Fast"] = "V";
                                    drowNewInsert["Reg"] = "";
                                    drowNewInsert["LReg"] = "";
                                    drowNewInsert["Self"] = "";
                                    break;
                                case "2":
                                    drowNewInsert["Letter"] = "";
                                    drowNewInsert["Fast"] = "";
                                    drowNewInsert["Reg"] = "V";
                                    drowNewInsert["LReg"] = "";
                                    drowNewInsert["Self"] = "";
                                    break;
                                case "3":
                                    drowNewInsert["Letter"] = "";
                                    drowNewInsert["Fast"] = "";
                                    drowNewInsert["Reg"] = "";
                                    drowNewInsert["LReg"] = "V";
                                    drowNewInsert["Self"] = "";
                                    break;
                            }
                        }
                        drowNewInsert["Addr"] = ((EntitySet_Pay_Certify)esReport0402030002.GetEntity(intLoop)).zip.Trim() + ((EntitySet_Pay_Certify)esReport0402030002.GetEntity(intLoop)).add1.Trim() +
                                                ((EntitySet_Pay_Certify)esReport0402030002.GetEntity(intLoop)).add2.Trim() + ((EntitySet_Pay_Certify)esReport0402030002.GetEntity(intLoop)).add3.Trim();
                        drowNewInsert["MailNo"] = ((EntitySet_Pay_Certify)esReport0402030002.GetEntity(intLoop)).mailNo;
                        drowNewInsert["BLKCODE"] = ((EntitySet_Pay_Certify)esReport0402030002.GetEntity(intLoop)).BLK_Code;
                        drowNewInsert["KeyinDay"] = strKeyInDay;
                        drowNewInsert["MailDay"] = strMailDayHid;
                        drowNewInsert["PayDay"] = Function.InsertTimeSpan(((EntitySet_Pay_Certify)esReport0402030002.GetEntity(intLoop)).payDay);
                        drowNewInsert["PayName"] = ((EntitySet_Pay_Certify)esReport0402030002.GetEntity(intLoop)).payName;
                        if (esReport0402030002.GetEntity(intLoop).pay != null)
                        drowNewInsert["PayAmt"] = esReport0402030002.GetEntity(intLoop).pay;
                        dtblReport0402030002.Rows.Add(drowNewInsert);
                    }
                }

                dtblReport0402030002.TableName = "KeyinList";
                dstResult.Tables.Add(dtblReport0402030002.DefaultView.ToTable());
                return true;
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp);
                strMsgID = "04_04020300_009";
                return false;
            }
        }

        /// <summary>
        /// 列印開立證明
        /// </summary>
        /// <param name="strSerialNoList">證明編號</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="dstResult">返回結果集</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report02010100(string strSerialNoList, ref string strMsgID, ref DataSet dstResult)
        {
            return ReportPaySV(strSerialNoList, ref strMsgID, ref dstResult);
        }

        /// <summary>
        /// 結清證明書公共報表
        /// </summary>
        /// <param name="strSerialNoList">證明編號</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="dstResult">返回結果集</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool ReportPaySV(string strSerialNoList, ref string strMsgID, ref DataSet dstResult)
        {
            string strCorML = "";
            string strCardData = "";
            string strExtra = "";
            string strCardNo = "";
            string strCusID = "";
            string strCusName = "";
            string strTypeCard = "";
            int intNum = 0;
            string strSqlCmd = String.Format(BRReport.SEL_PAY_SV_BY_SerialNo, strSerialNoList);

            DataSet dstPaySV = null;
            DataTable dtblPaySV = null;

            dstPaySV = BRPay_SV.SearchOnDataSet(strSqlCmd);

            if (dstPaySV == null)
            {
                strMsgID = "00_00000000_000";
                return false;
            }

            if (dstPaySV.Tables[0].Rows.Count == 0)
            {
                strMsgID = "00_00000000_037";
                return true;
            }

            EntityPay_CardSVSet esPayCardSV = new EntityPay_CardSVSet();

            dtblPaySV = dstPaySV.Tables[0];

            //* 創建DataTable
            DataTable dtblPayCertificate = new DataTable("PayCertificate");

            dtblPayCertificate.Columns.Add(new DataColumn("Receiver", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("Zip", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("Add1", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("Add2", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("Add3", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("Title", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("Context", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("AsFollow", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("Memo", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("Receiver2", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("KeyInDay", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("SerialNo", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("NoDetail", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("Remark", Type.GetType("System.String")));

            DataTable dbtlCard = new DataTable("Card");
            dbtlCard.Columns.Add(new DataColumn("serialNo", Type.GetType("System.String")));
            dbtlCard.Columns.Add(new DataColumn("cardData1", Type.GetType("System.String")));
            dbtlCard.Columns.Add(new DataColumn("cardData2", Type.GetType("System.String")));
            dbtlCard.Columns.Add(new DataColumn("cardData3", Type.GetType("System.String")));
            dbtlCard.Columns.Add(new DataColumn("cardData4", Type.GetType("System.String")));
            dbtlCard.Columns.Add(new DataColumn("cardData5", Type.GetType("System.String")));
            dbtlCard.Columns.Add(new DataColumn("cardData6", Type.GetType("System.String")));

            try
            {
                for (int i = 0; i < dtblPaySV.Rows.Count; i++)
                {
                    if (dtblPaySV.Rows[i]["Consignee"].ToString().Trim() == "")
                    {
                        dtblPaySV.Rows[i]["Consignee"] = dtblPaySV.Rows[i]["UserName"].ToString().Trim().Replace("　","");
                    }

                    if (!string.IsNullOrEmpty(dtblPaySV.Rows[i]["KeyinDay"].ToString()))
                    {
                        dtblPaySV.Rows[i]["KeyinDay"] = Function.InsertTimeUnits(Convert.ToString(int.Parse(dtblPaySV.Rows[i]["KeyinDay"].ToString().Trim()) - 19110000));
                    }
                    if (!string.IsNullOrEmpty(dtblPaySV.Rows[i]["ClearDate"].ToString()))
                    {
                        dtblPaySV.Rows[i]["ClearDate"] = MessageHelper.GetMessage("04_00000000_011") + Function.InsertTimeUnits(Convert.ToString(int.Parse(dtblPaySV.Rows[i]["ClearDate"].ToString().Trim()) - 19110000));
                    }

                    DataRow drowPayCertificate = dtblPayCertificate.NewRow();

                    drowPayCertificate["Receiver"] = dtblPaySV.Rows[i]["Consignee"].ToString().Trim().Replace("　", "");
                    drowPayCertificate["Zip"] = dtblPaySV.Rows[i]["Zip"].ToString();
                    drowPayCertificate["Add1"] = dtblPaySV.Rows[i]["Add1"].ToString();
                    drowPayCertificate["Add2"] = dtblPaySV.Rows[i]["Add2"].ToString();
                    drowPayCertificate["Add3"] = dtblPaySV.Rows[i]["Add3"].ToString();
                    drowPayCertificate["Receiver2"] = dtblPaySV.Rows[i]["UserName"].ToString().Trim().Replace("　", "");
                    drowPayCertificate["KeyInDay"] = dtblPaySV.Rows[i]["KeyinDay"].ToString();
                    drowPayCertificate["SerialNo"] = dtblPaySV.Rows[i]["SerialNo"].ToString();

                    string strType = dtblPaySV.Rows[i]["Type"].ToString();
                    if (strType == "C" || strType == "O")
                    {
                        //* [清償證明-卡]=C;[GCB清償證明-卡]=0;
                        drowPayCertificate["Title"] = MessageHelper.GetMessage("04_00000000_025");
                        drowPayCertificate["AsFollow"] = MessageHelper.GetMessage("04_00000000_027");
                        drowPayCertificate["Context"] = String.Format(MessageHelper.GetMessage("04_00000000_013"), dtblPaySV.Rows[i]["UserName"].ToString().Trim().Replace("　", ""), dtblPaySV.Rows[i]["UserID"].ToString(), dtblPaySV.Rows[i]["ClearDate"].ToString());
                        drowPayCertificate["Memo"] = "";
                        drowPayCertificate["NoDetail"] = String.Format(MessageHelper.GetMessage("04_00000000_049"), dtblPaySV.Rows[i]["SerialNo"].ToString());
                        strCorML = "C";
                        if (strType == "C")
                        {
                            //* [清償證明-卡]=C;[清償證明-ML&卡]=M;[清償證明-ML Only]=N;時才顯示
                            drowPayCertificate["Remark"] = MessageHelper.GetMessage("04_00000000_051");
                        }
                    }
                    else if (strType == "M")
                    {
                        //* [清償證明-ML&卡]=M;
                        drowPayCertificate["Title"] = MessageHelper.GetMessage("04_00000000_025");
                        drowPayCertificate["AsFollow"] = MessageHelper.GetMessage("04_00000000_028");
                        drowPayCertificate["Context"] = String.Format(MessageHelper.GetMessage("04_00000000_014"), dtblPaySV.Rows[i]["UserName"].ToString().Trim().Replace("　", ""), dtblPaySV.Rows[i]["UserID"].ToString(), dtblPaySV.Rows[i]["ClearDate"].ToString());
                        drowPayCertificate["Memo"] = "";
                        drowPayCertificate["NoDetail"] = String.Format(MessageHelper.GetMessage("04_00000000_049"), dtblPaySV.Rows[i]["SerialNo"].ToString());
                        strCorML = "M";
                        //* [清償證明-卡]=C;[清償證明-ML&卡]=M;[清償證明-ML Only]=N;時才顯示
                        drowPayCertificate["Remark"] = MessageHelper.GetMessage("04_00000000_051");
                    }
                    else if (strType == "N")
                    {
                        //* [清償證明-ML Only]=N;
                        drowPayCertificate["Title"] = MessageHelper.GetMessage("04_00000000_025");
                        drowPayCertificate["AsFollow"] = MessageHelper.GetMessage("04_00000000_029");
                        drowPayCertificate["NoDetail"] = String.Format(MessageHelper.GetMessage("04_00000000_049"), dtblPaySV.Rows[i]["SerialNo"].ToString());
                        drowPayCertificate["Context"] = String.Format(MessageHelper.GetMessage("04_00000000_015"), dtblPaySV.Rows[i]["UserName"].ToString().Trim().Replace("　", ""), dtblPaySV.Rows[i]["UserID"].ToString(), dtblPaySV.Rows[i]["ClearDate"].ToString());
                        drowPayCertificate["Memo"] = "";
                        strCorML = "N";
                        //* [清償證明-卡]=C;[清償證明-ML&卡]=M;[清償證明-ML Only]=N;時才顯示
                        drowPayCertificate["Remark"] = MessageHelper.GetMessage("04_00000000_051");
                    }
                    else if (strType == "G" || strType == "B")
                    {
                        //* [代償證明-卡]=G;[GCB代償證明]=B;
                        drowPayCertificate["Receiver"] = dtblPaySV.Rows[i]["PayName"].ToString().Trim().Replace("　", "");
                        drowPayCertificate["Title"] = MessageHelper.GetMessage("04_00000000_026");
                        drowPayCertificate["AsFollow"] = MessageHelper.GetMessage("04_00000000_027");
                        drowPayCertificate["NoDetail"] = String.Format(MessageHelper.GetMessage("04_00000000_050"), dtblPaySV.Rows[i]["SerialNo"].ToString());                        
                        drowPayCertificate["Context"] = String.Format(MessageHelper.GetMessage("04_00000000_052"),
                                                            dtblPaySV.Rows[i]["UserName"].ToString().Trim().Replace("　", ""),
                                                            dtblPaySV.Rows[i]["UserID"].ToString(),
                                                            dtblPaySV.Rows[i]["PayDay"].ToString(),
                                                            dtblPaySV.Rows[i]["PayName"].ToString(),
                                                            TransChr1(dtblPaySV.Rows[i]["Pay"].ToString()));
                        drowPayCertificate["Receiver2"] = dtblPaySV.Rows[i]["PayName"].ToString().Trim().Replace("　", "");
                        drowPayCertificate["Memo"] = MessageHelper.GetMessage("04_00000000_012");
                        strCorML = "C";
                    }
                    else if (strType == "T")
                    {
                        //* [代償證明-ML]=T
                        drowPayCertificate["Receiver"] = dtblPaySV.Rows[i]["PayName"].ToString().Trim().Replace("　", "");
                        drowPayCertificate["Title"] = MessageHelper.GetMessage("04_00000000_026");
                        drowPayCertificate["AsFollow"] = MessageHelper.GetMessage("04_00000000_028");
                        drowPayCertificate["NoDetail"] = String.Format(MessageHelper.GetMessage("04_00000000_050"), dtblPaySV.Rows[i]["SerialNo"].ToString());
                        drowPayCertificate["Context"] = String.Format(MessageHelper.GetMessage("04_00000000_017"), dtblPaySV.Rows[i]["UserName"].ToString().Trim().Replace("　", ""), dtblPaySV.Rows[i]["UserID"].ToString(), dtblPaySV.Rows[i]["PayDay"].ToString(), dtblPaySV.Rows[i]["PayName"].ToString().Trim().Replace("　", ""), TransChr1(dtblPaySV.Rows[i]["Pay"].ToString()));
                        drowPayCertificate["Memo"] = MessageHelper.GetMessage("04_00000000_012");
                        strCorML = "M";
                        drowPayCertificate["Receiver2"] = dtblPaySV.Rows[i]["PayName"].ToString().Trim().Replace("　", "");
                    }

                    dtblPayCertificate.Rows.Add(drowPayCertificate);

                    esPayCardSV.Clear();

                    SqlHelper sql = new SqlHelper();

                    sql.AddCondition(EntityPay_CardSV.M_SERIALNO, Operator.Equal, DataTypeUtils.String, dtblPaySV.Rows[i]["SerialNo"].ToString());

                    esPayCardSV.FillEntitySet(sql.GetFilterCondition());

                    strCardData = "";
                    strExtra = "";
                    intNum = 0;

                    //* CardType    , 0 - 主卡  , 1 - 附卡  , 2 - 主卡+附卡
                    //* strCorML    , C - 卡    , M - 卡&ML , N - ML
                    //* strTypeCard , M - 主卡  , L - ML    , E - 附卡

                    for (int j = 0; j < esPayCardSV.Count; j++)
                    {
                        strCardNo = esPayCardSV.GetEntity(j).CARDNO;        //* 卡號
                        strCusID = esPayCardSV.GetEntity(j).CUSID;          //* 卡人ID
                        strCusName = esPayCardSV.GetEntity(j).CUSNAME.Trim().Replace("　", "");      //* 卡人姓名
                        strTypeCard = esPayCardSV.GetEntity(j).TYPECARD;    //* 卡片類型,   M - 卡, L - ML, E - 附卡
                        if (dtblPaySV.Rows[i]["ShowExtra"].ToString().Trim() != "Y")
                        {
                            #region 不顯示明細資料 (ShowExtra != Y)
                            //* ShowExtra = Y顯示明細,!=Y不勾選明細
                            if (dtblPaySV.Rows[i]["CardType"].ToString() == "0")
                            {
                                #region 只開立主卡 (CardType = 0)
                                //* 0-只開立主卡,1-只開立附卡,2-主卡+附卡,3,他人附卡
                                if (strCorML == "C")
                                {
                                    #region 證明種類 - 卡 (strCorML = C)
                                    //* 如[證明種類] = 卡, 且卡片的類型是卡則顯示
                                    if (strTypeCard == "M")
                                    {
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            //* 一列顯示4個卡號
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }
                                    #endregion 
                                }
                                else if (strCorML == "M")
                                {
                                    #region 證明種類 - 卡&ML (strCorML = M)
                                    
                                    //* 如果是開立卡+ML,則卡片類型爲卡或者ML的都顯示
                                    if (strTypeCard == "M" || strTypeCard == "L")
                                    {
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }
                                    #endregion
                                }
                                else if (strCorML == "N")
                                {
                                    #region 證明種類 - MLOnly (strCorML = N)
                                    //* 如果開立ML Only, 則只顯示類型是ML的
                                    if (strTypeCard == "L" && Function.IsML(strCardNo) == 1)
                                    {
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            else if (dtblPaySV.Rows[i]["CardType"].ToString() == "1")
                            {
                                #region 只開立附卡 (CardType = 1)
                                //* 0-只開立主卡,1-只開立附卡,2-主卡+附卡,3,他人附卡
                                if (strTypeCard == "E")
                                {
                                    intNum++;
                                    if (intNum == 4)
                                    {
                                        intNum = 0;
                                        strCardData = strCardData + strCardNo + "<br>";
                                    }
                                    else
                                    {
                                        strCardData = strCardData + strCardNo + " ";
                                    }
                                }
                                #endregion
                            }
                            else if (dtblPaySV.Rows[i]["CardType"].ToString() == "2")
                            {
                                #region 主卡&附卡 (CardType = 2) , 他人附卡其正卡ID(CardType = 3)
                                //* 0-只開立主卡,1-只開立附卡,2-主卡+附卡,3,他人附卡
                                if (strCorML == "C")
                                {
                                    #region 證明種類 - 卡 (strCorML = C)
                                    //* 不為ML的都顯示
                                    if (strTypeCard != "L")
                                    {
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }                                    
                                    #endregion
                                }
                                else if (strCorML == "M")
                                {
                                    #region 證明種類 - 卡&ML (strCorML = M)
                                    intNum++;
                                    if (intNum == 4)
                                    {
                                        intNum = 0;
                                        strCardData = strCardData + strCardNo + "<br>";
                                    }
                                    else
                                    {
                                        strCardData = strCardData + strCardNo + " ";
                                    }
                                    #endregion
                                }
                                else if (strCorML == "N")
                                {
                                    #region 證明種類 - ML ONLY (strCorML = N)
                                    if (strTypeCard == "L")
                                    {
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region 顯示明細資料 (ShowExtra = Y)
                            //* ShowExtra = Y顯示明細
                            if (dtblPaySV.Rows[i]["CardType"].ToString() == "0")
                            {
                                #region 只開立主卡(CardType = 0)
                                //* 0-只開立主卡,1-只開立附卡,2-主卡+附卡,3,他人附卡
                                if (strCorML == "C")
                                {
                                    #region 證明種類 - 卡 (strCorML = C)
                                    if (strTypeCard == "M")
                                    {
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }
                                    #endregion
                                }
                                else if (strCorML == "M")
                                {
                                    #region 證明種類 - 卡&ML (strCorML = M)
                                    if (strTypeCard == "M" || strTypeCard == "L")
                                    {
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }
                                    #endregion
                                }
                                else if (strCorML == "N")
                                {
                                    #region 證明種類 - ML ONLY (strCorML = N)
                                    if (strTypeCard == "L" && Function.IsML(strCardNo) == 1)
                                    {
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            else if (dtblPaySV.Rows[i]["CardType"].ToString() == "1")
                            {
                                #region 只開立附卡(CardType = 1)
                                //* 0-只開立主卡,1-只開立附卡,2-主卡+附卡,3,他人附卡
                                if (strTypeCard == "E")
                                {
                                    strExtra = String.Format(MessageHelper.GetMessage("04_00000000_030"), strExtra, strCusName, strCusID, strCardNo);
                                }
                                #endregion
                            }
                            else
                            {
                                #region 主卡&附卡 (CardType = 2) , 他人附卡其正卡ID(CardType = 3)
                                //* 0-只開立主卡,1-只開立附卡,2-主卡+附卡,3,他人附卡
                                if (strCorML == "C")
                                {
                                    #region 證明種類 - 卡 (strCorML = C)
                                    if (strTypeCard != "L")
                                    {
                                        //* 不爲ML
                                        if (strTypeCard == "M")
                                        {
                                            //* 如果是主卡只顯示卡號
                                            intNum++;
                                            if (intNum == 4)
                                            {
                                                intNum = 0;
                                                strCardData = strCardData + strCardNo + "<br>";
                                            }
                                            else
                                            {
                                                strCardData = strCardData + strCardNo + " ";
                                            }
                                        }
                                        else if (strTypeCard == "E")
                                        {
                                            //* 如果是附卡顯示明細訊息
                                            strExtra = String.Format(MessageHelper.GetMessage("04_00000000_030"), strExtra, strCusName, strCusID, strCardNo);
                                        }
                                    }
                                    #endregion
                                }
                                else if (strCorML == "M")
                                {
                                    #region 證明種類 - 卡&ML (strCorML = M)
                                    if (strTypeCard != "E" && strTypeCard != "S")
                                    {
                                        //* 不爲附卡就
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }
                                    else
                                    {
                                        strExtra = String.Format(MessageHelper.GetMessage("04_00000000_030"), strExtra, strCusName, strCusID, strCardNo);
                                    }
                                    #endregion
                                }
                                else if (strCorML == "N")
                                {
                                    #region 證明種類 - ML ONLY (strCorML = N)
                                    //* 不爲主卡
                                    if (strTypeCard == "L")
                                    {
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }
                                    else
                                    {
                                        //strExtra = String.Format(MessageHelper.GetMessage("04_00000000_030"), strExtra, strCusName, strCusID, strCardNo);
                                        strExtra = "";
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                        }
                    }

                    if (!string.IsNullOrEmpty(strCardData) && !string.IsNullOrEmpty(strExtra))
                    {
                        strCardData = strCardData + "<br>" + strExtra;
                    }
                    else
                    {
                        strCardData = strCardData + strExtra;
                    }
                    DataRow drowCard = dbtlCard.NewRow();

                    drowCard["serialNo"] = dtblPaySV.Rows[i]["SerialNo"].ToString();
                    drowCard["cardData1"] = strCardData;
                    drowCard["cardData2"] = "";
                    drowCard["cardData3"] = "";
                    drowCard["cardData4"] = "";
                    drowCard["cardData5"] = "";
                    drowCard["cardData6"] = "";

                    dbtlCard.Rows.Add(drowCard);
                }

                dstResult.Tables.Add(dtblPayCertificate);

                dstResult.Tables.Add(dbtlCard);
                return true;
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp);
                strMsgID = "04_00000000_003";
                return false;
            }
        }

        /// <summary>
        /// 單個列印債清證明
        /// </summary>
        /// <param name="strSerialNoList">證明編號</param>
        /// <param name="strMailMethod">郵寄方式</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="dstResult">返回結果集</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report02020300(string strSerialNoList, ref string strMsgID, ref DataSet dstResult)
        {
            return ReportPaySV(strSerialNoList, ref strMsgID, ref dstResult);
        }

        /// <summary>
        /// 全部列印債清證明
        /// </summary>
        /// <param name="strUserID">使用者ID</param>
        /// <param name="strBeforeDate">開立日期起</param>
        /// <param name="strEndDate">開立日期迄</param>
        /// <param name="blIsKeyIn">是否開立</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="dstResult">返回結果集</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report02020300_2(string strUserID, string strBeforeDate, string strEndDate, bool blIsKeyIn, ref string strMsgID, ref DataSet dstResult)
        {
            string strSerialNoList = "";
            string strSearchSql = "";
            DataSet dstPaySV = null;
            DataTable dtblPaySV = null;

            SqlCommand sqlcmdSearchResult = new SqlCommand();
            sqlcmdSearchResult.CommandType = CommandType.Text;

            if (blIsKeyIn)
            {

                strSearchSql = SEL_PAY_SV_BY_SETTLE;

            }
            else
            {

                strSearchSql = SEL_PAY_SV_BY_SETTLE_NUll;
            }


            if (strBeforeDate == "")
            {
                strBeforeDate = "00000000";
            }

            if (strEndDate == "")
            {
                strEndDate = "99999999";
            }

            SqlParameter paramBeforeDate = new SqlParameter("@STARTDATE", strBeforeDate);
            sqlcmdSearchResult.Parameters.Add(paramBeforeDate);

            SqlParameter paramEndDate = new SqlParameter("@ENDDATE", strEndDate);
            sqlcmdSearchResult.Parameters.Add(paramEndDate);

            if (strUserID != "")
            {

                strSearchSql += " AND UserID = @UserID  ORDER BY [SerialNo] ASC";
                SqlParameter parmType = new SqlParameter("@USERID", strUserID);
                sqlcmdSearchResult.Parameters.Add(parmType);
            }
            else
            {
                strSearchSql += " ORDER BY [SerialNo] ASC";
            }
            sqlcmdSearchResult.CommandText = strSearchSql;

            dstPaySV = BRPay_SV.SearchOnDataSet(sqlcmdSearchResult);

            if (dstPaySV == null || dstPaySV.Tables[0].Rows.Count == 0)
            {
                strMsgID = "00_00000000_037";
                return false;
            }

            dtblPaySV = dstPaySV.Tables[0];

            for (int i = 0; i < dtblPaySV.Rows.Count; i++)
            {
                strSerialNoList = strSerialNoList + dtblPaySV.Rows[i]["SerialNo"].ToString() + ",";
            }

            strSerialNoList = strSerialNoList.Substring(0, strSerialNoList.Length - 1);

            return ReportPaySV(strSerialNoList, ref strMsgID, ref dstResult);
        }


        /// <summary>
        /// 全部列印清償證明
        /// </summary>
        /// <param name="strUserID">使用者ID</param>
        /// <param name="strType">類型</param>
        /// <param name="strStartDate">開始日期</param>
        /// <param name="strEndDate">結束日期</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="dstResult">返回結果集</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report01020300_2(string strUserID, string strType, string strStartDate, string strEndDate, ref string strMsgID, ref DataSet dstResult)
        {
            SqlHelper sql = new SqlHelper();

            if (strType != "ALL")
            {
                sql.AddCondition(EntityPay_Certify.M_type, Operator.Equal, DataTypeUtils.String, strType);
            }
            if(!String.IsNullOrEmpty(strUserID))
            {
                sql.AddCondition(EntityPay_Certify.M_userID, Operator.Equal, DataTypeUtils.String, strUserID);
            }
            if (strStartDate != "" && strEndDate != "")
            {
                sql.AddCondition(EntityPay_Certify.M_keyinDay, Operator.LessThanEqual, DataTypeUtils.String, strEndDate);
                sql.AddCondition(EntityPay_Certify.M_keyinDay, Operator.GreaterThanEqual, DataTypeUtils.String, strStartDate);
            }
            sql.AddOrderCondition(EntityPay_Certify.M_serialNo, ESortType.ASC);
            return ReportPayCertify(sql, ref strMsgID, ref dstResult);
        }

        /// <summary>
        /// 單個列印清償證明
        /// </summary>
        /// <param name="strStartSerialNo">證明編號</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="dstResult">返回結果集</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report01020300(string strStartSerialNo, ref string strMsgID, ref DataSet dstResult)
        {
            SqlHelper sql = new SqlHelper();

            sql.AddCondition(EntityPay_Certify.M_serialNo, Operator.Equal, DataTypeUtils.String, strStartSerialNo);

            return ReportPayCertify(sql, ref strMsgID, ref dstResult);
        }

        /// <summary>
        /// 清償證明書公共報表
        /// </summary>
        /// <param name="sql">查詢條件</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="dstResult">返回結果集</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool ReportPayCertify(SqlHelper sqlCondition, ref string strMsgID, ref DataSet dstResult)
        {
            sqlCondition.AddOrderCondition(EntityPay_Certify.M_keyinDay, ESortType.ASC);

            string strCorML = "";
            string strCardData = "";
            string strExtra = "";
            string strCardNo = "";
            string strCusID = "";
            string strCusName = "";
            string strTypeCard = "";
            int intNum = 0;

            EntityPay_CertifySet esPayCertify = new EntityPay_CertifySet();

            try
            {
                esPayCertify.FillEntitySet(sqlCondition.GetFilterCondition());
            }
            catch(Exception exp)
            {
                BRReport.SaveLog(exp);
                strMsgID = "00_00000000_000";
                return false;
            }


            if (esPayCertify.Count == 0)
            {
                strMsgID = "00_00000000_037";
                return true;
            }


            EntityPay_CardSet esPayCard = new EntityPay_CardSet();

            //* 創建DataTable
            DataTable dtblPayCertificate = new DataTable("PayCertificate");

            dtblPayCertificate.Columns.Add(new DataColumn("Receiver", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("Zip", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("Add1", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("Add2", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("Add3", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("Title", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("Context", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("AsFollow", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("Memo", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("Receiver2", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("KeyInDay", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("SerialNo", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("NoDetail", Type.GetType("System.String")));
            dtblPayCertificate.Columns.Add(new DataColumn("Remark", Type.GetType("System.String")));

            DataTable dbtlCard = new DataTable("Card");

            dbtlCard.Columns.Add(new DataColumn("serialNo", Type.GetType("System.String")));
            dbtlCard.Columns.Add(new DataColumn("cardData1", Type.GetType("System.String")));
            dbtlCard.Columns.Add(new DataColumn("cardData2", Type.GetType("System.String")));
            dbtlCard.Columns.Add(new DataColumn("cardData3", Type.GetType("System.String")));
            dbtlCard.Columns.Add(new DataColumn("cardData4", Type.GetType("System.String")));
            dbtlCard.Columns.Add(new DataColumn("cardData5", Type.GetType("System.String")));
            dbtlCard.Columns.Add(new DataColumn("cardData6", Type.GetType("System.String")));

            try
            {
                for (int i = 0; i < esPayCertify.Count; i++)
                {
                    if (esPayCertify.GetEntity(i).consignee.Trim() == "")
                    {
                        esPayCertify.GetEntity(i).consignee = esPayCertify.GetEntity(i).userName.Trim().Replace("　", "");
                    }

                    if (esPayCertify.GetEntity(i).keyinDay != null && esPayCertify.GetEntity(i).keyinDay.Trim() != "")
                        esPayCertify.GetEntity(i).keyinDay = Function.InsertTimeUnits(Convert.ToString(int.Parse(esPayCertify.GetEntity(i).keyinDay.Trim()) - 19110000));
                    if (esPayCertify.GetEntity(i).payDay != null && esPayCertify.GetEntity(i).payDay.Trim() != "" )
                        esPayCertify.GetEntity(i).payDay = Function.InsertTimeUnits(Convert.ToString(int.Parse(esPayCertify.GetEntity(i).payDay.Trim()) - 19110000));
                    if (esPayCertify.GetEntity(i).openDay != null && esPayCertify.GetEntity(i).openDay.Trim() != "")
                        esPayCertify.GetEntity(i).openDay = Function.InsertTimeUnits(esPayCertify.GetEntity(i).openDay.Trim());
                    if (esPayCertify.GetEntity(i).closeDay != null && esPayCertify.GetEntity(i).closeDay.Trim() != "")
                        esPayCertify.GetEntity(i).closeDay = Function.InsertTimeUnits(esPayCertify.GetEntity(i).closeDay.Trim());

                    if (!string.IsNullOrEmpty(esPayCertify.GetEntity(i).enddate.Trim()))
                    {
                        esPayCertify.GetEntity(i).enddate = MessageHelper.GetMessage("04_00000000_011") + Function.InsertTimeUnits(Convert.ToString(int.Parse(esPayCertify.GetEntity(i).enddate.Trim()) - 19110000));
                    }

                    DataRow drowPayCertificate = dtblPayCertificate.NewRow();

                    drowPayCertificate["Receiver"] = esPayCertify.GetEntity(i).consignee.Trim().Replace("　", "");
                    drowPayCertificate["Zip"] = esPayCertify.GetEntity(i).zip;
                    drowPayCertificate["Add1"] = esPayCertify.GetEntity(i).add1;
                    drowPayCertificate["Add2"] = esPayCertify.GetEntity(i).add2;
                    drowPayCertificate["Add3"] = esPayCertify.GetEntity(i).add3;
                    drowPayCertificate["Receiver2"] = esPayCertify.GetEntity(i).userName.Trim().Replace("　", "");
                    drowPayCertificate["KeyInDay"] = esPayCertify.GetEntity(i).keyinDay;
                    drowPayCertificate["SerialNo"] = esPayCertify.GetEntity(i).serialNo;

                    string strType = esPayCertify.GetEntity(i).type;
                    if (strType == "C" || strType == "O")
                    {
                        //* 證明種類    [清償證明-卡]=C;[GCB清償證明-卡]=0;
                        drowPayCertificate["Title"] = MessageHelper.GetMessage("04_00000000_025");
                        drowPayCertificate["AsFollow"] = MessageHelper.GetMessage("04_00000000_027");
                        drowPayCertificate["Context"] = String.Format(MessageHelper.GetMessage("04_00000000_013"), esPayCertify.GetEntity(i).userName.Trim().Replace("　", ""), esPayCertify.GetEntity(i).userID, esPayCertify.GetEntity(i).enddate);
                        drowPayCertificate["Memo"] = "";
                        drowPayCertificate["NoDetail"] = String.Format(MessageHelper.GetMessage("04_00000000_049"), esPayCertify.GetEntity(i).serialNo);
                        strCorML = "C";
                        if (strType == "C")
                        {
                            //* [清償證明-卡]=C;[清償證明-ML&卡]=M;[清償證明-ML Only]=N;時才顯示
                            drowPayCertificate["Remark"] = MessageHelper.GetMessage("04_00000000_051");
                        }
                    }
                    else if (strType == "M")
                    {
                        //* 證明種類    [清償證明-ML&卡]=M;
                        drowPayCertificate["Title"] = MessageHelper.GetMessage("04_00000000_025") ;
                        drowPayCertificate["AsFollow"] = MessageHelper.GetMessage("04_00000000_028");
                        drowPayCertificate["Context"] = String.Format(MessageHelper.GetMessage("04_00000000_014"), esPayCertify.GetEntity(i).userName.Trim().Replace("　", ""), esPayCertify.GetEntity(i).userID, esPayCertify.GetEntity(i).enddate);
                        drowPayCertificate["Memo"] = "";
                        drowPayCertificate["NoDetail"] = String.Format(MessageHelper.GetMessage("04_00000000_049"), esPayCertify.GetEntity(i).serialNo);
                        strCorML = "M";
                        //* [清償證明-卡]=C;[清償證明-ML&卡]=M;[清償證明-ML Only]=N;時才顯示
                        drowPayCertificate["Remark"] = MessageHelper.GetMessage("04_00000000_051");
                    }
                    else if (strType == "N")
                    {
                        //* 證明種類    [清償證明-ML Only]=N;
                        drowPayCertificate["Title"] = MessageHelper.GetMessage("04_00000000_025") ;
                        drowPayCertificate["AsFollow"] = MessageHelper.GetMessage("04_00000000_029");
                        drowPayCertificate["Context"] = String.Format(MessageHelper.GetMessage("04_00000000_015"), esPayCertify.GetEntity(i).userName.Trim().Replace("　", ""), esPayCertify.GetEntity(i).userID, esPayCertify.GetEntity(i).enddate);
                        drowPayCertificate["Memo"] = "";
                        drowPayCertificate["NoDetail"] = String.Format(MessageHelper.GetMessage("04_00000000_049"), esPayCertify.GetEntity(i).serialNo); 
                        strCorML = "N";
                        //* [清償證明-卡]=C;[清償證明-ML&卡]=M;[清償證明-ML Only]=N;時才顯示
                        drowPayCertificate["Remark"] = MessageHelper.GetMessage("04_00000000_051");
                    }
                    else if (strType == "G" || strType == "B")
                    {
                        //* 證明種類    [代償證明-卡]=G;[GCB代償證明]=B;
                        drowPayCertificate["Receiver"] = esPayCertify.GetEntity(i).payName.Trim().Replace("　", "");
                        drowPayCertificate["Title"] = MessageHelper.GetMessage("04_00000000_026");
                        drowPayCertificate["AsFollow"] = MessageHelper.GetMessage("04_00000000_027");
                        drowPayCertificate["Context"] = String.Format(MessageHelper.GetMessage("04_00000000_052"),
                                                            esPayCertify.GetEntity(i).userName.Trim().Replace("　", ""),
                                                            esPayCertify.GetEntity(i).userID,
                                                            esPayCertify.GetEntity(i).payDay,
                                                            esPayCertify.GetEntity(i).payName,
                                                            ChangeEMoney2CMoney(esPayCertify.GetEntity(i).pay == null ? "0" : esPayCertify.GetEntity(i).pay.ToString()));
                        drowPayCertificate["Receiver2"] = esPayCertify.GetEntity(i).payName.Trim().Replace("　", "");
                        drowPayCertificate["Memo"] = MessageHelper.GetMessage("04_00000000_012");
                        drowPayCertificate["NoDetail"] = String.Format(MessageHelper.GetMessage("04_00000000_050"), esPayCertify.GetEntity(i).serialNo);
                        strCorML = "C";
                    }
                    else if (strType == "T")
                    {
                        //* 證明種類    [代償證明-ML]=T
                        drowPayCertificate["Receiver"] = esPayCertify.GetEntity(i).payName.Trim().Replace("　", "");
                        drowPayCertificate["Title"] = MessageHelper.GetMessage("04_00000000_026") ;
                        drowPayCertificate["AsFollow"] = MessageHelper.GetMessage("04_00000000_028");
                        drowPayCertificate["NoDetail"] = String.Format(MessageHelper.GetMessage("04_00000000_050"), esPayCertify.GetEntity(i).serialNo);
                        drowPayCertificate["Context"] = String.Format(MessageHelper.GetMessage("04_00000000_017"), esPayCertify.GetEntity(i).userName.Trim().Replace("　", ""), esPayCertify.GetEntity(i).userID, esPayCertify.GetEntity(i).payDay, esPayCertify.GetEntity(i).payName.Trim().Replace("　", ""), ChangeEMoney2CMoney(esPayCertify.GetEntity(i).pay == null ? "0" : esPayCertify.GetEntity(i).pay.ToString()));
                        drowPayCertificate["Memo"] = MessageHelper.GetMessage("04_00000000_012");
                        strCorML = "M";
                        drowPayCertificate["Receiver2"] = esPayCertify.GetEntity(i).payName.Trim().Replace("　", "");
                    }

                    dtblPayCertificate.Rows.Add(drowPayCertificate);

                    esPayCard.Clear();

                    SqlHelper sql = new SqlHelper();

                    sql.AddCondition(EntityPay_Card.M_SERIALNO, Operator.Equal, DataTypeUtils.String, esPayCertify.GetEntity(i).serialNo);

                    esPayCard.FillEntitySet(sql.GetFilterCondition());

                    strCardData = "";
                    strExtra = "";
                    intNum = 0;

                    //* CardType    , 0 - 主卡  , 1 - 附卡  , 2 - 主卡+附卡
                    //* strCorML    , C - 卡    , M - 卡&ML , N - ML
                    //* strTypeCard , M - 主卡  , L - ML    , E - 附卡      , S - 他人附卡

                    for (int j = 0; j < esPayCard.Count; j++)
                    {

                        strCardNo = esPayCard.GetEntity(j).CARDNO;      //* 卡號
                        strCusID = esPayCard.GetEntity(j).CUSID;        //* 卡人ID
                        strCusName = esPayCard.GetEntity(j).CUSNAME;    //* 卡人姓名
                        strTypeCard = esPayCard.GetEntity(j).TYPECARD;  //* 卡片類型,   M - 卡, L - ML, E - 附卡, S - 他人附卡

                        if (esPayCertify.GetEntity(i).showExtra.Trim() != "Y")
                        {
                            #region 不顯示明細資料 (showExtra != Y)
                            if (esPayCertify.GetEntity(i).cardType == "0")
                            {
                                #region 只開立主卡 (CardType = 0)
                                if (strCorML == "C")
                                {
                                    #region 證明種類 - 卡 (strCorML = C)
                                    if (strTypeCard == "M")
                                    {
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            //* 一列4個卡號
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }
                                    #endregion
                                }
                                else if (strCorML == "M")
                                {
                                    #region 證明種類 - 卡&ML (strCorML = M)
                                    if (strTypeCard == "M" || strTypeCard == "L")
                                    {
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }
                                    #endregion
                                }
                                else if (strCorML == "N")
                                {
                                    #region 證明種類 - ML ONLY (strCorML = N)
                                    if (strTypeCard == "L" && Function.IsML(strCardNo) == 1)
                                    {
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            else if (esPayCertify.GetEntity(i).cardType == "1")
                            {
                                #region 只開立附卡 (CardType = 1)
                                if (strTypeCard == "E" || strTypeCard == "S")
                                {
                                    intNum++;
                                    if (intNum == 4)
                                    {
                                        intNum = 0;
                                        strCardData = strCardData + strCardNo + "<br>";
                                    }
                                    else
                                    {
                                        strCardData = strCardData + strCardNo + " ";
                                    }
                                }
                                #endregion
                            }
                            else if (esPayCertify.GetEntity(i).cardType == "2")
                            {
                                #region 主卡+附卡 (CardType = 2)
                                if (strCorML == "C")
                                {
                                    #region 證明種類 - 卡 (strCorML = C)
                                    if (strTypeCard != "L")
                                    {
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }
                                    #endregion
                                }
                                else if (strCorML == "M")
                                {
                                    #region 證明種類 - 卡&ML (strCorML = M)
                                    intNum++;
                                    if (intNum == 4)
                                    {
                                        intNum = 0;
                                        strCardData = strCardData + strCardNo + "<br>";
                                    }
                                    else
                                    {
                                        strCardData = strCardData + strCardNo + " ";
                                    }
                                    #endregion
                                }
                                else if (strCorML == "N")
                                {
                                    #region 證明種類 - ML ONLY (strCorML = N)
                                    if (strTypeCard == "L")
                                    {
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            else if (esPayCertify.GetEntity(i).cardType == "3")
                            {
                                #region 只開立他人附卡 (CardType = 3)
                                if (strTypeCard == "S")
                                {
                                    intNum++;
                                    if (intNum == 4)
                                    {
                                        intNum = 0;
                                        strCardData = strCardData + strCardNo + "<br>";
                                    }
                                    else
                                    {
                                        strCardData = strCardData + strCardNo + " ";
                                    }
                                }
                                #endregion
                            }
                           #endregion
                        }
                        else
                        {
                            #region 顯示明細資料 (showExtra = Y)
                            if (esPayCertify.GetEntity(i).cardType == "0")
                            {
                                #region 只開立主卡 (CardType = 0)
                                if (strCorML == "C")
                                {
                                    #region 證明種類 - 卡 (strCorML = C)
                                    if (strTypeCard == "M")
                                    {
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }
                                    #endregion
                                }
                                else if (strCorML == "M")
                                {
                                    #region 證明種類 - 卡&ML (strCorML = M)
                                    if (strTypeCard == "M" || strTypeCard == "L")
                                    {
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }
                                    #endregion
                                }
                                else if (strCorML == "N")
                                {
                                    #region 證明種類 - ML (strCorML = N)
                                    if (strTypeCard == "L" && Function.IsML(strCardNo) == 1)
                                    {                                        
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            else if (esPayCertify.GetEntity(i).cardType == "1")
                            {
                                #region 只開立附卡 (CardType = 1)
                                if (strTypeCard == "E" || strTypeCard == "S")
                                {
                                    strExtra = String.Format(MessageHelper.GetMessage("04_00000000_030"), strExtra, strCusName, strCusID, strCardNo);
                                }
                                #endregion
                            }
                            else if (esPayCertify.GetEntity(i).cardType == "2")
                            {
                                #region 主卡+附卡 (CardType = 2)
                                if (strCorML == "C")
                                {
                                    #region 證明種類 - 卡 (strCorML = C)
                                    if (strTypeCard != "L")
                                    {
                                        if (strTypeCard == "M")
                                        {
                                            intNum++;
                                            if (intNum == 4)
                                            {
                                                intNum = 0;
                                                strCardData = strCardData + strCardNo + "<br>";
                                            }
                                            else
                                            {
                                                strCardData = strCardData + strCardNo + " ";
                                            }
                                        }
                                        else if (strTypeCard == "E")
                                        {
                                            strExtra = String.Format(MessageHelper.GetMessage("04_00000000_030"), strExtra, strCusName, strCusID, strCardNo);
                                        }
                                    }
                                    #endregion
                                }
                                else if (strCorML == "M")
                                {
                                    #region 證明種類 - 卡&ML (strCorML = M)
                                    if (strTypeCard != "E" && strTypeCard != "S")
                                    {
                                        intNum++;
                                        if (intNum == 4)
                                        {
                                            intNum = 0;
                                            strCardData = strCardData + strCardNo + "<br>";
                                        }
                                        else
                                        {
                                            strCardData = strCardData + strCardNo + " ";
                                        }
                                    }
                                    else
                                    {
                                        strExtra = String.Format(MessageHelper.GetMessage("04_00000000_030"), strExtra, strCusName, strCusID, strCardNo);
                                    }
                                    #endregion
                                }
                                else if (strCorML == "N")
                                {
                                    #region 證明種類 - ML (strCorML = N)
                                    if (strTypeCard != "M")
                                    {
                                        if (strTypeCard == "L")
                                        {
                                            intNum++;
                                            if (intNum == 4)
                                            {
                                                intNum = 0;
                                                strCardData = strCardData + strCardNo + "<br>";
                                            }
                                            else
                                            {
                                                strCardData = strCardData + strCardNo + " ";
                                            }
                                        }
                                        else
                                        {
                                            strExtra = String.Format(MessageHelper.GetMessage("04_00000000_030"), strExtra, strCusName, strCusID, strCardNo);
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            else if (esPayCertify.GetEntity(i).cardType == "3")
                            {
                                #region 只開立他人附卡 (CardType = 3)
                                if (strTypeCard == "S")
                                {
                                    strExtra = String.Format(MessageHelper.GetMessage("04_00000000_030"), strExtra, strCusName, strCusID, strCardNo);
                                }
                                #endregion
                            }
                            #endregion
                        }
                    }

                    if (!string.IsNullOrEmpty(strCardData) && !string.IsNullOrEmpty(strExtra))
                    {
                        strCardData = strCardData + "<br>" + strExtra;
                    }
                    else
                    {
                        strCardData = strCardData + strExtra;
                    }
                    
                    DataRow drowCard = dbtlCard.NewRow();

                    drowCard["serialNo"] = esPayCertify.GetEntity(i).serialNo;
                    drowCard["cardData1"] = strCardData;
                    drowCard["cardData2"] = "";
                    drowCard["cardData3"] = "";
                    drowCard["cardData4"] = "";
                    drowCard["cardData5"] = "";
                    drowCard["cardData6"] = "";

                    dbtlCard.Rows.Add(drowCard);
                }

                dstResult.Tables.Add(dtblPayCertificate);

                dstResult.Tables.Add(dbtlCard);
                return true;
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp);
                strMsgID = "04_00000000_003";
                return false;
            }
        }

        #region 列印結清證明附件
        /// <summary>
        /// 列印結清證明附件
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strSearchMonth">查詢年月</param>
        /// <param name="strFilePath">存儲路徑</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0302050002(string strStartSerialNo, string strEndSerialNo, string strSearchMonth, ref string strFilePath)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
            //* 添加查詢條件Type等於0 - 結清, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "0");

            //* 如果有輸入證明編號
            if (strStartSerialNo != "")
            {
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
            }
            Sql.AddOrderCondition(EntitySet_Pay_Certify.M_serialNo, ESortType.ASC);
            try
            {
                //* 按證明編號起、證明編號迄、身分證字號、自取日期、卡號等查詢數據 
                EntitySet_Pay_CertifySet esySetPayCertify = new EntitySet_Pay_CertifySet();
                esySetPayCertify.FillEntitySet(Sql.GetFilterCondition());
                if (esySetPayCertify.TotalCount > 0)
                {
                    StringBuilder sbResult = new StringBuilder();
                    for (int intloop = 0; intloop < esySetPayCertify.TotalCount; intloop++)
                    {
                        sbResult.Append("SendDataKey(\"" + esySetPayCertify.GetEntity(intloop).userID + "\")" + "\r\n");
                        sbResult.Append("SendFuncKey(\"ENTER\")" + "\r\n");
                        sbResult.Append("WaitTimes(1)" + "\r\n");
                        sbResult.Append("SendFuncKey(\"PRINT\")" + "\r\n");


                    }
                    Function.CheckDirectory(ref strFilePath);

                    strFilePath = strFilePath + @"\" + DateTime.Now.ToString("yyyyMMdd") + "_1331.MAC";
                    if (System.IO.File.Exists(strFilePath))
                    {
                        System.IO.File.Delete(strFilePath);
                    }
                    System.IO.FileStream fsResult = System.IO.File.Create(strFilePath);
                    byte[] bcontent = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(sbResult.ToString());
                    fsResult.Write(bcontent, 0, bcontent.Length);
                    fsResult.Close();
                    fsResult = null;
                }

                return true;
            }
            catch (Exception ex)
            {
                BRReport.SaveLog(ex);
                return false;
            }
        }
        /// <summary>
        /// 列印結清證明附件
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strSearchMonth">查詢年月</param>
        /// <param name="strFilePath">存儲路徑</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0302050003(string strStartSerialNo, string strEndSerialNo, string strSearchMonth, ref string strFilePath)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
            //* 添加查詢條件Type等於0 - 結清, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "0");

            //* 如果有輸入證明編號
            if (strStartSerialNo != "")
            {
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
            }
            Sql.AddOrderCondition(EntitySet_Pay_Certify.M_serialNo, ESortType.ASC);

            try
            {
                //* 按證明編號起、證明編號迄、身分證字號、自取日期、卡號等查詢數據 
                EntitySet_Pay_CertifySet esySetPayCertify = new EntitySet_Pay_CertifySet();
                esySetPayCertify.FillEntitySet(Sql.GetFilterCondition());
                if (esySetPayCertify.TotalCount > 0)
                {
                    StringBuilder sbResult = new StringBuilder();
                    for (int intloop = 0; intloop < esySetPayCertify.TotalCount; intloop++)
                    {
                        sbResult.Append("Wait(-1)" + "\r\n");
                        sbResult.Append("SendDataKey(\"" + esySetPayCertify.GetEntity(intloop).userID + "\")" + "\r\n");
                        sbResult.Append("SendFuncKey(\"ENTER\")" + "\r\n");
                        sbResult.Append("WaitTimes(1)" + "\r\n");
                        sbResult.Append("SendFuncKey(\"PRINT\")" + "\r\n");
                        sbResult.Append("SendFuncKey(\"CLEAR\")" + "\r\n");

                    }
                    Function.CheckDirectory(ref strFilePath);

                    strFilePath = strFilePath + @"\" + DateTime.Now.ToString("yyyyMMdd") + "_PCIK.MAC";
                    if (System.IO.File.Exists(strFilePath))
                    {
                        System.IO.File.Delete(strFilePath);
                    }
                    System.IO.FileStream fsResult = System.IO.File.Create(strFilePath);
                    byte[] bcontent = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(sbResult.ToString());
                    fsResult.Write(bcontent, 0, bcontent.Length);
                    fsResult.Close();
                    fsResult = null;
                }

                return true;
            }
            catch (Exception ex)
            {
                BRReport.SaveLog(ex); 
                return false;
            }
        }
        /// <summary>
        /// 列印結清證明附件
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strSearchMonth">查詢年月</param>
        /// <param name="strFilePath">存儲路徑</param>
        /// <param name="strHostMsg">主機信息</param>
        /// <param name="strAlertMsg">提示信息</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0302050004(string strStartSerialNo, string strEndSerialNo, string strSearchMonth, ref string strFilePath, ref string strHostMsg, ref string strAlertMsg,EntityAGENT_INFO eAgentInfo)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
            //* 添加查詢條件Type等於0 - 結清, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "0");

            //* 如果有輸入證明編號
            if (strStartSerialNo != "")
            {
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
            }
            Sql.AddOrderCondition(EntitySet_Pay_Certify.M_serialNo, ESortType.ASC);
            EntitySet_Pay_CertifySet esSetPayCertify = new EntitySet_Pay_CertifySet();

            try
            {
                esSetPayCertify.FillEntitySet(Sql.GetFilterCondition());
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp); 
                strAlertMsg = MessageHelper.GetMessage("04_00000000_041");
                return false;
            }


            //* 按證明編號起、證明編號迄、身分證字號、自取日期、卡號等查詢數據 
            if (esSetPayCertify.TotalCount > 0)
            {
                try
                {
                    int intLineCNT = 0;
                    string strUserID = "";

                    Hashtable htInput = new Hashtable();
                    DataTable dtblOutput = new DataTable();
                    StringBuilder sbResult = new StringBuilder();

                    //* 要傳回的欄位集合
                    for (int intloop = 0; intloop < esSetPayCertify.TotalCount; intloop++)
                    {
                        strUserID = esSetPayCertify.GetEntity(intloop).userID;
                        //*得到主機傳回信息
                        htInput.Add("FUNCTION_CODE", "1");//*ID查詢
                        htInput.Add("LINE_CNT", "0000");//*LINE_CNT
                        htInput.Add("ACCT_NBR", strUserID);

                        dtblOutput = MainFrameInfo.GetMainframeDataByPage("P4_JCAB", htInput, false, "1", eAgentInfo);

                        if (dtblOutput.Rows[0]["HtgErrMsg"].ToString() != "") //*主機返回失敗
                        {
                            strHostMsg = dtblOutput.Rows[0]["HtgErrMsg"].ToString();

                            if (dtblOutput.Rows[0]["MESSAGE_TYPE"].ToString() == "8888")
                            {
                                strAlertMsg = MessageHelper.GetMessage("04_01010100_009");
                            }
                            else
                            {
                                strAlertMsg = MessageHelper.GetMessage("04_00000000_041");
                                return false;
                            }
                        }
                        else
                        {
                            strAlertMsg = MessageHelper.GetMessage("04_00000000_038");
                            strHostMsg = dtblOutput.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_038");
 
                            intLineCNT = int.Parse(dtblOutput.Rows[0]["LINE_CNT"].ToString());
                            intLineCNT = ((intLineCNT - 1) / 7) ;
                            sbResult.Append("SendDataKey(\"" + strUserID + "\")" + "\r\n");
                            sbResult.Append("SendFuncKey(\"ENTER\")" + "\r\n");
                            sbResult.Append("WaitTimes(1)" + "\r\n");
                            sbResult.Append("SendFuncKey(\"PRINT\")" + "\r\n");
                            for (int i = 0; i <= intLineCNT - 1; i++)
                            {
                                sbResult.Append("SendFuncKey(\"PF8\")" + "\r\n");
                                sbResult.Append("WaitTimes(1)" + "\r\n");
                                sbResult.Append("SendFuncKey(\"PRINT\")" + "\r\n");
                            }
                            
                        }
                        htInput.Clear();
                        dtblOutput.Clear();
                        intLineCNT = 0;
                    }
                    if (sbResult.Length <= 5)
                    {
                        sbResult.Append(" ");
                    }

                    Function.CheckDirectory(ref strFilePath);
                    
                    strFilePath = strFilePath + @"\" + DateTime.Now.ToString("yyyyMMdd") + "_1131.MAC";
                    if (System.IO.File.Exists(strFilePath))
                    {
                        System.IO.File.Delete(strFilePath);
                    }
                    System.IO.FileStream fsResult = System.IO.File.Create(strFilePath);
                    byte[] bcontent = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(sbResult.ToString());
                    fsResult.Write(bcontent, 0, bcontent.Length);
                    fsResult.Close();
                    fsResult = null;
                }
                catch (Exception ex)
                {
                    BRReport.SaveLog(ex);
                    strAlertMsg = MessageHelper.GetMessage("04_00000000_041") + " : " + ex;
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 列印結清證明附件
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strSearchMonth">查詢年月</param>
        /// <param name="strFilePath">存儲路徑</param>
        /// <param name="strHostMsg">主機信息</param>
        /// <param name="strAlertMsg">提示信息</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0302050005(string strStartSerialNo, string strEndSerialNo, string strSearchMonth, ref  string strFilePath, ref string strHostMsg, ref string strAlertMsg, EntityAGENT_INFO eAgentInfo)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
            //* 添加查詢條件Type等於0 - 結清, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "0");


            //* 如果有輸入證明編號
            if (strStartSerialNo != "")
            {
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
            }
            Sql.AddOrderCondition(EntitySet_Pay_Certify.M_serialNo, ESortType.ASC);
            //* 按證明編號起、證明編號迄、身分證字號、自取日期、卡號等查詢數據 
            EntitySet_Pay_CertifySet esSetPayCertify = new EntitySet_Pay_CertifySet();

            try
            {
                esSetPayCertify.FillEntitySet(Sql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                BRReport.SaveLog(ex); 
                strAlertMsg = MessageHelper.GetMessage("04_00000000_043");
                return false;
            }


            if (esSetPayCertify.TotalCount > 0)
            {
                try
                {
                    int intLineCNT = 0;
                    string strUserID = "";
                    StringBuilder sbResult = new StringBuilder();
                    Hashtable htInput = new Hashtable();
                    DataTable dtblOutput = new DataTable();

                    //* 要傳回的欄位集合

                    for (int intloop = 0; intloop < esSetPayCertify.TotalCount; intloop++)
                    {

                        strUserID = esSetPayCertify.GetEntity(intloop).userID;
                        //*得到主機傳回信息
                        htInput.Add("FUNCTION_CODE", "1");//*ID查詢
                        htInput.Add("LINE_CNT", "0000");//*LINE_CNT
                        htInput.Add("ACCT_NBR", strUserID);
                        htInput.Add("ACCT_DATE", strSearchMonth);

                        dtblOutput = MainFrameInfo.GetMainframeDataByPage("P4_JCAC", htInput, false, "1", eAgentInfo);

                        if (dtblOutput.Rows[0]["HtgErrMsg"].ToString() != "") //*主機返回失敗
                        {
                            strHostMsg = dtblOutput.Rows[0]["HtgErrMsg"].ToString();

                            if (dtblOutput.Rows[0]["MESSAGE_TYPE"].ToString() == "8888")
                            {
                                strAlertMsg = MessageHelper.GetMessage("04_01010100_009");
                            }
                            else
                            {
                                strAlertMsg = MessageHelper.GetMessage("04_00000000_043");
                                return false;
                            }
                        }
                        else
                        {
                            strAlertMsg = MessageHelper.GetMessage("04_00000000_040");
                            strHostMsg = dtblOutput.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_040");

                            intLineCNT = int.Parse(dtblOutput.Rows[0]["LINE_CNT"].ToString());
                            string strDateTemp = strSearchMonth.Substring(4, 2) + strSearchMonth.Substring(2, 2);

                            sbResult.Append("SendDataKey(\"" + strDateTemp + "\")" + "\r\n");
                            sbResult.Append("SendDataKey(\"" + strUserID + "\")" + "\r\n");
                            sbResult.Append("SendFuncKey(\"ENTER\")" + "\r\n");
                            sbResult.Append("WaitTimes(1)" + "\r\n");
                            sbResult.Append("SendFuncKey(\"PRINT\")" + "\r\n");
                            intLineCNT = ((intLineCNT - 1) / 9) ;
                            for (int i = 0; i <= intLineCNT - 1; i++)
                            {
                                sbResult.Append("SendFuncKey(\"PF8\")" + "\r\n");
                                sbResult.Append("WaitTimes(1)" + "\r\n");
                                sbResult.Append("SendFuncKey(\"PRINT\")" + "\r\n");
                            }
                            
                        }
                        htInput.Clear();
                        dtblOutput.Clear();
                        intLineCNT = 0;
                    }
                    if (sbResult.Length <= 5)
                    {
                        sbResult.Append(" ");
                    }

                    Function.CheckDirectory(ref strFilePath);

                    strFilePath = strFilePath + @"\" + DateTime.Now.ToString("yyyyMMdd") + "_1132.MAC";
                    if (System.IO.File.Exists(strFilePath))
                    {
                        System.IO.File.Delete(strFilePath);
                    }
                    
                    System.IO.FileStream fsResult = System.IO.File.Create(strFilePath);
                    byte[] bcontent = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(sbResult.ToString());
                    fsResult.Write(bcontent, 0, bcontent.Length);
                    fsResult.Close();
                    fsResult = null;
                }
                catch (Exception ex)
                {
                    BRReport.SaveLog(ex);
                    strAlertMsg = MessageHelper.GetMessage("04_00000000_043") + " : " + ex;
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 列印結清證明附件
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strSearchMonth">查詢年月</param>
        /// <param name="strFilePath">存儲路徑</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0302050006(string strStartSerialNo, string strEndSerialNo, string strSearchMonth, ref string strFilePath)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
            //* 添加查詢條件Type等於0 - 結清, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "0");

            //* 如果有輸入證明編號
            if (strStartSerialNo != "")
            {
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
            }
            Sql.AddOrderCondition(EntitySet_Pay_Certify.M_serialNo, ESortType.ASC);
            try
            {

                //* 按證明編號起、證明編號迄、身分證字號、自取日期、卡號等查詢數據 
                EntitySet_Pay_CertifySet esSetPayCertify = new EntitySet_Pay_CertifySet();

                esSetPayCertify.FillEntitySet(Sql.GetFilterCondition());

                if (esSetPayCertify.TotalCount > 0)
                {
                    StringBuilder sbResult = new StringBuilder();
                    for (int intloop = 0; intloop < esSetPayCertify.TotalCount; intloop++)
                    {

                        sbResult.Append("SendDataKey(\"" + esSetPayCertify.GetEntity(intloop).CardNo + "\")" + "\r\n");
                        sbResult.Append("SendFuncKey(\"ENTER\")" + "\r\n");
                        sbResult.Append("WaitTimes(1)" + "\r\n");
                        sbResult.Append("SendFuncKey(\"PRINT\")" + "\r\n");


                    }
                    Function.CheckDirectory(ref strFilePath);

                    strFilePath = strFilePath + @"\" + DateTime.Now.ToString("yyyyMMdd") + "_1572.MAC";
                    if (System.IO.File.Exists(strFilePath))
                    {
                        System.IO.File.Delete(strFilePath);
                    }
                    System.IO.FileStream fsResult = System.IO.File.Create(strFilePath);
                    byte[] bcontent = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(sbResult.ToString());
                    fsResult.Write(bcontent, 0, bcontent.Length);
                    fsResult.Close();
                    fsResult = null;
                }

                return true;
            }
            catch (Exception ex)
            {
                BRReport.SaveLog(ex); 
                return false;
            }
        }
        #endregion

        #region 列印代償證明附件
        /// <summary>
        /// 列印代償證明附件
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strSearchMonth">查詢年月</param>
        /// <param name="strFilePath">存儲路徑</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0402050002(string strStartSerialNo, string strEndSerialNo, string strSearchMonth, ref string strFilePath)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
            //* 添加查詢條件Type等於0 - 代償, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "1");

            //* 如果有輸入證明編號
            if (strStartSerialNo != "")
            {
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
            }
            Sql.AddOrderCondition(EntitySet_Pay_Certify.M_serialNo, ESortType.ASC);
            try
            {
                //* 按證明編號起、證明編號迄、身分證字號、自取日期、卡號等查詢數據 
                EntitySet_Pay_CertifySet esSetPayCertify = new EntitySet_Pay_CertifySet();
                esSetPayCertify.FillEntitySet(Sql.GetFilterCondition());
                if (esSetPayCertify.TotalCount > 0)
                {
                    StringBuilder sbResult = new StringBuilder();
                    for (int intloop = 0; intloop < esSetPayCertify.TotalCount; intloop++)
                    {

                        sbResult.Append("SendDataKey(\"" + esSetPayCertify.GetEntity(intloop).userID + "\")" + "\r\n");
                        sbResult.Append("SendFuncKey(\"ENTER\")" + "\r\n");
                        sbResult.Append("WaitTimes(1)" + "\r\n");
                        sbResult.Append("SendFuncKey(\"PRINT\")" + "\r\n");

                    }
                    Function.CheckDirectory(ref strFilePath);

                    strFilePath = strFilePath + @"\" + DateTime.Now.ToString("yyyyMMdd") + "_1331.MAC";
                    if (System.IO.File.Exists(strFilePath))
                    {
                        System.IO.File.Delete(strFilePath);
                    }
                    System.IO.FileStream fsResult = System.IO.File.Create(strFilePath);
                    byte[] bcontent = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(sbResult.ToString());
                    fsResult.Write(bcontent, 0, bcontent.Length);
                    fsResult.Close();
                    fsResult = null;
                }

                return true;
            }
            catch (Exception ex)
            {
                BRReport.SaveLog(ex); 
                return false;
            }
        }
        /// <summary>
        /// 列印代償證明附件
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strSearchMonth">查詢年月</param>
        /// <param name="strFilePath">存儲路徑</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0402050003(string strStartSerialNo, string strEndSerialNo, string strSearchMonth, ref string strFilePath)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
            //* 添加查詢條件Type等於0 - 代償, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "1");

            //* 如果有輸入證明編號
            if (strStartSerialNo != "")
            {
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
            }
            Sql.AddOrderCondition(EntitySet_Pay_Certify.M_serialNo, ESortType.ASC);
            try
            {

                EntitySet_Pay_CertifySet esSetPayCertify = new EntitySet_Pay_CertifySet();
                esSetPayCertify.FillEntitySet(Sql.GetFilterCondition());
                if (esSetPayCertify.TotalCount > 0)
                {
                    StringBuilder sbResult = new StringBuilder();
                    for (int intloop = 0; intloop < esSetPayCertify.TotalCount; intloop++)
                    {
                        sbResult.Append("Wait(-1)" + "\r\n");
                        sbResult.Append("SendDataKey(\"" + esSetPayCertify.GetEntity(intloop).userID + "\")" + "\r\n");
                        sbResult.Append("SendFuncKey(\"ENTER\")" + "\r\n");
                        sbResult.Append("WaitTimes(1)" + "\r\n");
                        sbResult.Append("SendFuncKey(\"PRINT\")" + "\r\n");
                        sbResult.Append("SendFuncKey(\"CLEAR\")" + "\r\n");

                    }
                    Function.CheckDirectory(ref strFilePath);

                    strFilePath = strFilePath + @"\" + DateTime.Now.ToString("yyyyMMdd") + "_PCIK.MAC";
                    if (System.IO.File.Exists(strFilePath))
                    {
                        System.IO.File.Delete(strFilePath);
                    }
                    System.IO.FileStream fsResult = System.IO.File.Create(strFilePath);
                    byte[] bcontent = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(sbResult.ToString());
                    fsResult.Write(bcontent, 0, bcontent.Length);
                    fsResult.Close();
                    fsResult = null;
                }

                return true;
            }
            catch (Exception ex)
            {
                BRReport.SaveLog(ex); 
                return false;
            }
        }
        /// <summary>
        /// 列印代償證明附件
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strSearchMonth">查詢年月</param>
        /// <param name="strFilePath">存儲路徑</param>
        /// <param name="strHostMsg">主機信息</param>
        /// <param name="strAlertMsg">提示信息</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0402050004(string strStartSerialNo, string strEndSerialNo, string strSearchMonth, ref string strFilePath, ref string strHostMsg, ref string strAlertMsg, EntityAGENT_INFO eAgentInfo)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
            //* 添加查詢條件Type等於0 - 代償, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "1");

            //* 如果有輸入證明編號
            if (strStartSerialNo != "")
            {
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
            }
            Sql.AddOrderCondition(EntitySet_Pay_Certify.M_serialNo, ESortType.ASC);
            //* 按證明編號起、證明編號迄、身分證字號、自取日期、卡號等查詢數據 
            EntitySet_Pay_CertifySet esSetPayCertify = new EntitySet_Pay_CertifySet();

            try
            {
                esSetPayCertify.FillEntitySet(Sql.GetFilterCondition());
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp); 
                strAlertMsg = MessageHelper.GetMessage("04_00000000_041");
                return false;
            }

            if (esSetPayCertify.TotalCount > 0)
            {
                try
                {
                    int intLineCNT = 0;
                    string strUserID = "";

                    StringBuilder sbResult = new StringBuilder();
                    Hashtable htInput = new Hashtable();
                    DataTable dtblOutput = new DataTable();
                    //* 要傳回的欄位集合
                    for (int intloop = 0; intloop < esSetPayCertify.TotalCount; intloop++)
                    {
                        strUserID = esSetPayCertify.GetEntity(intloop).userID;
                        //*得到主機傳回信息
                        htInput.Add("FUNCTION_CODE", "1");//*ID查詢
                        htInput.Add("LINE_CNT", "0000");//*LINE_CNT
                        htInput.Add("ACCT_NBR", strUserID);

                        dtblOutput = MainFrameInfo.GetMainframeDataByPage("P4_JCAB", htInput, false, "1", eAgentInfo);

                        if (dtblOutput.Rows[0]["HtgErrMsg"].ToString() != "") //*主機返回失敗
                        {
                            strHostMsg = dtblOutput.Rows[0]["HtgErrMsg"].ToString();

                            if (dtblOutput.Rows[0]["MESSAGE_TYPE"].ToString() == "8888")
                            {
                                strAlertMsg = MessageHelper.GetMessage("04_01010100_009");
                            }
                            else
                            {
                                strAlertMsg = MessageHelper.GetMessage("04_00000000_041");
                                return false;
                            }
                        }
                        else
                        {
                            strAlertMsg = MessageHelper.GetMessage("04_00000000_038");
                            strHostMsg = dtblOutput.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_038");
                            intLineCNT = int.Parse(dtblOutput.Rows[0]["LINE_CNT"].ToString());
                            sbResult.Append("SendDataKey(\"" + esSetPayCertify.GetEntity(intloop).userID + "\")" + "\r\n");
                            sbResult.Append("SendFuncKey(\"ENTER\")" + "\r\n");
                            sbResult.Append("WaitTimes(1)" + "\r\n");
                            sbResult.Append("SendFuncKey(\"PRINT\")" + "\r\n");
                            intLineCNT = ((intLineCNT - 1) / 7);
                            for (int i = 0; i <= intLineCNT - 1; i++)
                            {
                                sbResult.Append("SendFuncKey(\"PF8\")" + "\r\n");
                                sbResult.Append("WaitTimes(1)" + "\r\n");
                                sbResult.Append("SendFuncKey(\"PRINT\")" + "\r\n");
                            }
                            
                        }
                        htInput.Clear();
                        dtblOutput.Clear();
                        intLineCNT = 0;
                    }

                    if (sbResult.Length <= 5)
                    {
                        sbResult.Append(" ");
                    }
                    

                    Function.CheckDirectory(ref strFilePath);
                    
                    strFilePath = strFilePath + @"\" + DateTime.Now.ToString("yyyyMMdd") + "_1131.MAC";
                    if (System.IO.File.Exists(strFilePath))
                    {
                        System.IO.File.Delete(strFilePath);
                    }
                    System.IO.FileStream fsResult = System.IO.File.Create(strFilePath);
                    byte[] bcontent = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(sbResult.ToString());
                    fsResult.Write(bcontent, 0, bcontent.Length);
                    fsResult.Close();
                    fsResult = null;
                }
                catch (Exception ex)
                {
                    BRReport.SaveLog(ex);
                    strAlertMsg = MessageHelper.GetMessage("04_00000000_041") + " : " + ex;
                    return false;
                }
            }
            return true;

        }
        /// <summary>
        /// 列印代償證明附件
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strSearchMonth">查詢年月</param>
        /// <param name="strFilePath">存儲路徑</param>
        /// <param name="strHostMsg">主機信息</param>
        /// <param name="strAlertMsg">提示信息</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0402050005(string strStartSerialNo, string strEndSerialNo, string strSearchMonth, ref string strFilePath, ref string strHostMsg, ref string strAlertMsg, EntityAGENT_INFO eAgentInfo)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
            //* 添加查詢條件Type等於0 - 代償, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "1");


            //* 如果有輸入證明編號
            if (strStartSerialNo != "")
            {
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
            }
            Sql.AddOrderCondition(EntitySet_Pay_Certify.M_serialNo, ESortType.ASC);
            //* 按證明編號起、證明編號迄、身分證字號、自取日期、卡號等查詢數據 
            EntitySet_Pay_CertifySet esSetPayCertify = new EntitySet_Pay_CertifySet();

            try
            {
                esSetPayCertify.FillEntitySet(Sql.GetFilterCondition());
            }
            catch (Exception ex)
            {
                BRReport.SaveLog(ex); 
                strAlertMsg = MessageHelper.GetMessage("04_00000000_043");
                return false;
            }


            //* 按證明編號起、證明編號迄、身分證字號、自取日期、卡號等查詢數據 
            if (esSetPayCertify.TotalCount > 0)
            {
                try
                {
                    string strUserID = "";
                    int intLineCNT = 0;
                    StringBuilder sbResult = new StringBuilder();
                    Hashtable htInput = new Hashtable();
                    DataTable dtblOutput = new DataTable();

                    //* 要傳回的欄位集合

                    for (int intloop = 0; intloop < esSetPayCertify.TotalCount; intloop++)
                    {
                        strUserID = esSetPayCertify.GetEntity(intloop).userID;
                        //*得到主機傳回信息
                        htInput.Add("FUNCTION_CODE", "1");//*ID查詢
                        htInput.Add("LINE_CNT", "0000");//*LINE_CNT
                        htInput.Add("ACCT_NBR", strUserID);
                        htInput.Add("ACCT_DATE", strSearchMonth);

                        dtblOutput = MainFrameInfo.GetMainframeDataByPage("P4_JCAC", htInput, false, "1", eAgentInfo);

                        if (dtblOutput.Rows[0]["HtgErrMsg"].ToString() != "") //*主機返回失敗
                        {
                            strHostMsg = dtblOutput.Rows[0]["HtgErrMsg"].ToString();

                            if (dtblOutput.Rows[0]["MESSAGE_TYPE"].ToString() == "8888")
                            {
                                strAlertMsg = MessageHelper.GetMessage("04_01010100_009");
                            }
                            else
                            {
                                strAlertMsg = MessageHelper.GetMessage("04_00000000_043");
                                return false;
                            }
                        }
                        else
                        {
                            strAlertMsg = MessageHelper.GetMessage("04_00000000_040");
                            strHostMsg = dtblOutput.Rows[0]["MESSAGE_TYPE"].ToString() + " " + MessageHelper.GetMessage("04_00000000_040");

                            intLineCNT = int.Parse(dtblOutput.Rows[0]["LINE_CNT"].ToString());
                            string strDateTemp = strSearchMonth.Substring(4, 2) + strSearchMonth.Substring(2, 2);
                            sbResult.Append("SendDataKey(\"" + strDateTemp + "\")" + "\r\n");
                            sbResult.Append("SendDataKey(\"" + esSetPayCertify.GetEntity(intloop).userID + "\")" + "\r\n");
                            sbResult.Append("SendFuncKey(\"ENTER\")" + "\r\n");
                            sbResult.Append("WaitTimes(1)" + "\r\n");
                            sbResult.Append("SendFuncKey(\"PRINT\")" + "\r\n");
                            intLineCNT = ((intLineCNT - 1) / 9);
                            for (int i = 0; i <= intLineCNT - 1; i++)
                            {
                                sbResult.Append("SendFuncKey(\"PF8\")" + "\r\n");
                                sbResult.Append("WaitTimes(1)" + "\r\n");
                                sbResult.Append("SendFuncKey(\"PRINT\")" + "\r\n");
                            }
                            
                        }
                        htInput.Clear();
                        dtblOutput.Clear();
                        intLineCNT = 0;

                    }
                    if (sbResult.Length <= 5)
                    {
                        sbResult.Append(" ");
                    }
                    

                    Function.CheckDirectory(ref strFilePath);                    
                    strFilePath = strFilePath + @"\" + DateTime.Now.ToString("yyyyMMdd") + "_1132.MAC";
                    if (System.IO.File.Exists(strFilePath))
                    {
                        System.IO.File.Delete(strFilePath);
                    }
                    System.IO.FileStream fsResult = System.IO.File.Create(strFilePath);
                    byte[] bcontent = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(sbResult.ToString());
                    fsResult.Write(bcontent, 0, bcontent.Length);
                    fsResult.Close();
                    fsResult = null;
                }
                catch (Exception ex)
                {
                    BRReport.SaveLog(ex);
                    strAlertMsg = MessageHelper.GetMessage("04_00000000_043") + " : " + ex;
                    return false;
                }
            }
            return true;

        }
        /// <summary>
        /// 列印代償證明附件
        /// </summary>
        /// <param name="strStartSerialNo">證明編號(起)</param>
        /// <param name="strEndSerialNo">證明編號(迄)</param>
        /// <param name="strSearchMonth">查詢年月</param>
        /// <param name="strFilePath">存儲路徑</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report0402050006(string strStartSerialNo, string strEndSerialNo, string strSearchMonth, ref string strFilePath)
        {
            SqlHelper Sql = new SqlHelper();
            //* 添加查詢條件IsMail等於N
            Sql.AddCondition(EntitySet_Pay_Certify.M_void, Operator.NotEqual, DataTypeUtils.String, "Y");
            //* 添加查詢條件Type等於0 - 代償, 1 - 代償
            Sql.AddCondition(EntitySet_Pay_Certify.M_type, Operator.Equal, DataTypeUtils.String, "1");

            //* 如果有輸入證明編號
            if (strStartSerialNo != "")
            {
                //* 如果有輸入 證明編號起,則添加查詢條件 SerialNo >= [證明編號起]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.GreaterThanEqual, DataTypeUtils.String, strStartSerialNo);
                //* 如果有輸入 證明編號迄,則添加查詢條件 SerialNo <= [證明編號迄]
                Sql.AddCondition(EntitySet_Pay_Certify.M_serialNo, Operator.LessThanEqual, DataTypeUtils.String, strEndSerialNo);
            }
            Sql.AddOrderCondition(EntitySet_Pay_Certify.M_serialNo, ESortType.ASC);
            try
            {
                //* 按證明編號起、證明編號迄、身分證字號、自取日期、卡號等查詢數據 
                EntitySet_Pay_CertifySet esSetPayCertify = new EntitySet_Pay_CertifySet();


                esSetPayCertify.FillEntitySet(Sql.GetFilterCondition());

                if (esSetPayCertify.TotalCount > 0)
                {
                    StringBuilder sbResult = new StringBuilder();
                    for (int intloop = 0; intloop < esSetPayCertify.TotalCount; intloop++)
                    {

                        sbResult.Append("SendDataKey(\"" + esSetPayCertify.GetEntity(intloop).CardNo + "\")" + "\r\n");
                        sbResult.Append("SendFuncKey(\"ENTER\")" + "\r\n");
                        sbResult.Append("WaitTimes(1)" + "\r\n");
                        sbResult.Append("SendFuncKey(\"PRINT\")" + "\r\n");


                    }
                    Function.CheckDirectory(ref strFilePath);

                    strFilePath = strFilePath + @"\" + DateTime.Now.ToString("yyyyMMdd") + "_1572.MAC";
                    if (System.IO.File.Exists(strFilePath))
                    {
                        System.IO.File.Delete(strFilePath);
                    }
                    System.IO.FileStream fsResult = System.IO.File.Create(strFilePath);
                    byte[] bcontent = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(sbResult.ToString());
                    fsResult.Write(bcontent, 0, bcontent.Length);
                    fsResult.Close();
                    fsResult = null;
                }

                return true;
            }
            catch (Exception ex)
            {
                BRReport.SaveLog(ex); 
                return false;
            }
        }


        /// <summary>
        /// 列印清償證明統計表
        /// </summary>
        /// <param name="strRptBeforeDate">開立日期(起)</param>
        /// <param name="strRptEndDate">開立日期(迄)</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="rptResult">返回報表對象</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report01020100(string strBeforeDate, string strEndDate, ref string strMsgID, ref DataSet dstResult)
        {
            try
            {

                #region 明細資料
                DataTable dtblCertify = new DataTable("Certify");
                dtblCertify.Columns.Add(new DataColumn("SerialNo", Type.GetType("System.String")));
                dtblCertify.Columns.Add(new DataColumn("userID", Type.GetType("System.String")));
                dtblCertify.Columns.Add(new DataColumn("add1", Type.GetType("System.String")));
                dtblCertify.Columns.Add(new DataColumn("zip", Type.GetType("System.String")));
                dtblCertify.Columns.Add(new DataColumn("add2", Type.GetType("System.String")));
                dtblCertify.Columns.Add(new DataColumn("add3", Type.GetType("System.String")));
                dtblCertify.Columns.Add(new DataColumn("mailNo", Type.GetType("System.String")));
                dtblCertify.Columns.Add(new DataColumn("Consignee", Type.GetType("System.String")));

                //* 報表明細內容
                SqlCommand sqlcmdCertify = new SqlCommand(SEL_Pay_Certify);
                sqlcmdCertify.Parameters.Add(new SqlParameter("@STARTDATE", strBeforeDate));
                sqlcmdCertify.Parameters.Add(new SqlParameter("@ENDDATE", strEndDate));
                DataTable dtblPayCertify = BRReport.SearchOnDataSet(sqlcmdCertify).Tables[0];
                for (int i = 0; i < dtblPayCertify.Rows.Count; i++)
                {
                    if (dtblPayCertify.Rows[i]["Consignee"].ToString().Trim() == "")
                    {
                        dtblPayCertify.Rows[i]["Consignee"] = dtblPayCertify.Rows[i]["UserName"].ToString();
                    }
                    DataRow drowPayCertificate = dtblCertify.NewRow();
                    drowPayCertificate["SerialNo"] = dtblPayCertify.Rows[i]["SerialNo"].ToString();
                    drowPayCertificate["userID"] = dtblPayCertify.Rows[i]["userID"].ToString();
                    drowPayCertificate["zip"] = dtblPayCertify.Rows[i]["zip"].ToString();
                    drowPayCertificate["add1"] = dtblPayCertify.Rows[i]["add1"].ToString();
                    drowPayCertificate["add2"] = dtblPayCertify.Rows[i]["add2"].ToString();
                    drowPayCertificate["add3"] = dtblPayCertify.Rows[i]["add3"].ToString();
                    drowPayCertificate["mailNo"] = dtblPayCertify.Rows[i]["mailNo"].ToString();
                    drowPayCertificate["Consignee"] = dtblPayCertify.Rows[i]["Consignee"].ToString();
                    dtblCertify.Rows.Add(drowPayCertificate);
                }
                #endregion


                #region 統計信息
                //* 創建報表結果DataTable
                DataTable dtblExtraData = new DataTable("ExtraData");
                dtblExtraData.Columns.Add(new DataColumn("Char1", Type.GetType("System.String")));
                dtblExtraData.Columns.Add(new DataColumn("Char2", Type.GetType("System.String")));
                dtblExtraData.Columns.Add(new DataColumn("Int1", Type.GetType("System.String")));
                dtblExtraData.Columns.Add(new DataColumn("Int2", Type.GetType("System.String")));
                dtblExtraData.Columns.Add(new DataColumn("CountMakeUp", Type.GetType("System.String")));
                dtblExtraData.Columns.Add(new DataColumn("Char3", Type.GetType("System.String")));

                int iTotalCount = 0;    //* 總件數
                int iFreeCount = 0;     //* 免費手續費件數(不含註銷)
                int iNoFreeCount = 0;   //* 收手續費件數(不含註銷)
                int iReutrnCount = 0;   //* 退回筆數
                int iCancelCount = 0;   //* 註銷件數

                //* 總件數
                SqlCommand cmdTotal = new SqlCommand(SEL_SUM);
                cmdTotal.Parameters.Add(new SqlParameter("@STARTDATE", strBeforeDate));
                cmdTotal.Parameters.Add(new SqlParameter("@ENDDATE", strEndDate));
                iTotalCount = BRReport.SearchOnDataSet(cmdTotal).Tables[0].Rows.Count;   //* 總件數

                //* 免費手續費件數(不含註銷)
                SqlCommand cmdFree = new SqlCommand(SEL_IsFreeCount);
                cmdFree.Parameters.Add(new SqlParameter("@STARTDATE", strBeforeDate));
                cmdFree.Parameters.Add(new SqlParameter("@ENDDATE", strEndDate));
                iFreeCount = int.Parse(BRReport.SearchOnDataSet(cmdFree).Tables[0].Rows[0]["counts"].ToString());

                //* 收手續費件數(不含註銷)
                SqlCommand cmdNotFree = new SqlCommand(SEL_NotFreeCount);
                cmdNotFree.Parameters.Add(new SqlParameter("@STARTDATE", strBeforeDate));
                cmdNotFree.Parameters.Add(new SqlParameter("@ENDDATE", strEndDate));
                iNoFreeCount = int.Parse(BRReport.SearchOnDataSet(cmdNotFree).Tables[0].Rows[0]["counts"].ToString());

                //* 退回筆數
                SqlCommand cmdReutrn = new SqlCommand(SEL_ReutrnCount);
                cmdReutrn.Parameters.Add(new SqlParameter("@STARTDATE", strBeforeDate));
                cmdReutrn.Parameters.Add(new SqlParameter("@ENDDATE", strEndDate));
                iReutrnCount = int.Parse(BRReport.SearchOnDataSet(cmdReutrn).Tables[0].Rows[0]["counts"].ToString());

                //* 註銷件數
                SqlCommand cmdCacnel = new SqlCommand(SEL_CancelCount);
                cmdCacnel.Parameters.Add(new SqlParameter("@STARTDATE", strBeforeDate));
                cmdCacnel.Parameters.Add(new SqlParameter("@ENDDATE", strEndDate));
                iCancelCount = int.Parse(BRReport.SearchOnDataSet(cmdCacnel).Tables[0].Rows[0]["counts"].ToString());

                DataRow drowExtraData = dtblExtraData.NewRow();
                drowExtraData["Char1"] = iTotalCount;   //* 總計件數
                drowExtraData["Char2"] = iFreeCount;    //* 免收手續費件數
                drowExtraData["Int1"] = iNoFreeCount;   //* 收手續費件數
                drowExtraData["Int2"] = iReutrnCount;   //* 退件件數
                drowExtraData["CountMakeUp"] = 0;       //* 補發件數
                drowExtraData["Char3"] = iCancelCount;  //* 註銷件數
                dtblExtraData.Rows.Add(drowExtraData);
                #endregion

                dstResult.Tables.Add(dtblExtraData);
                dstResult.Tables.Add(dtblCertify);


                return true;
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp);
                strMsgID = "04_01020100_003";
                return false;
            }
        }

        /// <summary>
        /// 債清證明統計表
        /// </summary>
        /// <param name="strRptBeforeDate">開立日期(起)</param>
        /// <param name="strRptEndDate">開立日期(迄)</param>
        /// <param name="strMsgID">傳回的消息ID號</param>
        /// <param name="rptResult">返回報表對象</param>
        /// <returns>返回執行標識：True---執行成功；False----執行失敗</returns>
        public static bool Report02020100(string strBeforeDate, string strEndDate, ref string strMsgID, ref DataSet dstResult)
        {
            int iTotal = 0;       //免收手續費件數
            int iVoidCounts = 0;   //註銷筆數 
            int iReutrnCount = 0; //退回筆數
            int iMakeUpCount = 0; //補發筆數
            //string strCardList = "";
            //string strIDList = "";
            //string strNameList = "";
            //string strTypeCard = "";
            //string strCUSID = "";
            //string strCUSNAME = "";
            //string strBLK = "";


            string strSQLSUM = SEL_Dayth_SV;
            SqlCommand sqlcmdsum = new SqlCommand();
            SqlParameter parmBefore = new SqlParameter("@STARTDATE", strBeforeDate);
            sqlcmdsum.Parameters.Add(parmBefore);
            SqlParameter parmEnd = new SqlParameter("@ENDDATE", strEndDate);
            sqlcmdsum.Parameters.Add(parmEnd);
            sqlcmdsum.CommandText = strSQLSUM;
            sqlcmdsum.CommandType = CommandType.Text;
            DataTable dtblSum = null;


            string strSQLReutrnCountSV = SEL_ReutrnCountSV;
            SqlCommand sqlcmdReutrnCountSV = new SqlCommand();
            SqlParameter parmBeforeReutrnCountSV = new SqlParameter("@STARTDATE", strBeforeDate);
            sqlcmdReutrnCountSV.Parameters.Add(parmBeforeReutrnCountSV);
            SqlParameter parmEndReutrnCountSV = new SqlParameter("@ENDDATE", strEndDate);
            sqlcmdReutrnCountSV.Parameters.Add(parmEndReutrnCountSV);
            sqlcmdReutrnCountSV.CommandText = strSQLReutrnCountSV;
            sqlcmdReutrnCountSV.CommandType = CommandType.Text;
            DataTable dtblReutrnCountSV = null;


            string strSQLMakeUpCountSV = SEL_MakeUpCountSV;
            SqlCommand sqlcmdMakeUpCountSV = new SqlCommand();
            SqlParameter parmBeforeMakeUpCountSV = new SqlParameter("@STARTDATE", strBeforeDate);
            sqlcmdMakeUpCountSV.Parameters.Add(parmBeforeMakeUpCountSV);
            SqlParameter parmEndMakeUpCountSV = new SqlParameter("@ENDDATE", strEndDate);
            sqlcmdMakeUpCountSV.Parameters.Add(parmEndMakeUpCountSV);
            sqlcmdMakeUpCountSV.CommandText = strSQLMakeUpCountSV;
            sqlcmdMakeUpCountSV.CommandType = CommandType.Text;
            DataTable dtblMakeUpCountSV = null;


            string strSQLVoidCountSV = SEL_VoidCountSV;
            SqlCommand sqlcmdVoidCountSV = new SqlCommand();
            SqlParameter parmBeforeVoidCountSV = new SqlParameter("@STARTDATE", strBeforeDate);
            sqlcmdVoidCountSV.Parameters.Add(parmBeforeVoidCountSV);
            SqlParameter parmEndVoidCountSV = new SqlParameter("@ENDDATE", strEndDate); 
            sqlcmdVoidCountSV.Parameters.Add(parmEndVoidCountSV);
            sqlcmdVoidCountSV.CommandText = strSQLVoidCountSV;
            sqlcmdVoidCountSV.CommandType = CommandType.Text;
            DataTable dtblVoidCountSV = null;

            ////* 創建DataTable

            DataTable dtblExtraData = new DataTable("ExtraData");
            dtblExtraData.Columns.Add(new DataColumn("Char1", Type.GetType("System.String")));
            dtblExtraData.Columns.Add(new DataColumn("Char2", Type.GetType("System.String")));
            dtblExtraData.Columns.Add(new DataColumn("Int1", Type.GetType("System.String")));
            dtblExtraData.Columns.Add(new DataColumn("Int2", Type.GetType("System.String")));
            dtblExtraData.Columns.Add(new DataColumn("CountMakeUp", Type.GetType("System.String")));

            DataTable dtblSV = new DataTable("Pay_SV");
            dtblSV.Columns.Add(new DataColumn("No", Type.GetType("System.String")));
            dtblSV.Columns.Add(new DataColumn("userID", Type.GetType("System.String")));
            dtblSV.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
            dtblSV.Columns.Add(new DataColumn("SerialNo", Type.GetType("System.String")));
            dtblSV.Columns.Add(new DataColumn("Address", Type.GetType("System.String")));
            dtblSV.Columns.Add(new DataColumn("Consignee", Type.GetType("System.String")));
            dtblSV.Columns.Add(new DataColumn("EndDate", Type.GetType("System.String")));
            dtblSV.Columns.Add(new DataColumn("Type", Type.GetType("System.String")));
            dtblSV.Columns.Add(new DataColumn("CardNo", Type.GetType("System.String")));
            dtblSV.Columns.Add(new DataColumn("BLKCode", Type.GetType("System.String")));
            dtblSV.Columns.Add(new DataColumn("Status", Type.GetType("System.String")));
            dtblSV.Columns.Add(new DataColumn("ClearDate", Type.GetType("System.String")));
            dtblSV.Columns.Add(new DataColumn("ApplyUser", Type.GetType("System.String")));
            dtblSV.Columns.Add(new DataColumn("AgreeUser", Type.GetType("System.String")));
            dtblSV.Columns.Add(new DataColumn("TypeCard", Type.GetType("System.String")));
            dtblSV.Columns.Add(new DataColumn("CUSID", Type.GetType("System.String")));
            dtblSV.Columns.Add(new DataColumn("CUSNAME", Type.GetType("System.String")));
            dtblSV.Columns.Add(new DataColumn("IsPaidOffAlone", Type.GetType("System.String")));

            try
            {
                //* 取得所有資料
                dtblSum = BRReport.SearchOnDataSet(sqlcmdsum).Tables[0];
                string strSerialNo = "";
                //iTotal = dtblSum.Rows.Count;
                iTotal = 0;
                for (int i = 0; i < dtblSum.Rows.Count; i++)
                {
                    #region 廢棄
                    //strCardList = "";
                    //strIDList = "";
                    //strNameList = "";
                    //strTypeCard = "";
                    //strCUSID = "";
                    //strCUSNAME = "";
                    //strBLK = "";

                    //string strSQLTemp = SEL_Temp;
                    //SqlCommand sqlcmdTemp = new SqlCommand();
                    //SqlParameter parmSERIALNO = new SqlParameter("@SERIALNO", dtblSum.Rows[i]["SerialNo"].ToString());
                    //sqlcmdTemp.Parameters.Add(parmSERIALNO);
                    //sqlcmdTemp.CommandText = strSQLTemp;
                    //sqlcmdTemp.CommandType = CommandType.Text;

                    //DataTable dtblTemp = BRReport.SearchOnDataSet(sqlcmdTemp).Tables[0];
                    //DataTable dtblTempCopy = dtblTemp.Copy();

                    //for (int k = 0; k < dtblTempCopy.Rows.Count; k++)
                    //{
                    //    //* 卡號
                    //    if (strCardList == "")
                    //    {
                    //        strCardList = dtblTempCopy.Rows[k]["CardNo"].ToString();
                    //    }
                    //    else
                    //    {
                    //        strCardList = strCardList + "<br>" + dtblTempCopy.Rows[k]["CardNo"].ToString();
                    //    }
                    //    //* BLKCode
                    //    strBLK = strBLK + dtblTempCopy.Rows[k]["BLK"].ToString() + "<br>";
                    //    //* TypeCard,CustID,CustName
                    //    if (dtblTempCopy.Rows[k]["TypeCard"].ToString() == "M")
                    //    {
                    //        strTypeCard = strTypeCard + "0" + "<br>";
                    //        strCUSID = strCUSID + "&nbsp;<br>";
                    //        strCUSNAME = strCUSNAME + "&nbsp;<br>";
                    //    }
                    //    else if (dtblTemp.Rows[k]["TypeCard"].ToString() == "E")
                    //    {
                    //        strTypeCard = strTypeCard + "1" + "<br>";
                    //        strCUSID = strCUSID + " " + dtblTempCopy.Rows[k]["CUSID"].ToString() + "<br>";
                    //        strCUSNAME = strCUSNAME + dtblTempCopy.Rows[k]["CUSNAME"].ToString() + "<br>";
                    //    }
                    //    else
                    //    {
                    //        strTypeCard = strTypeCard + "1" + "<br>";
                    //        strCUSID = strCUSID + " " + dtblTempCopy.Rows[k]["CUSID"].ToString() + "<br>";
                    //        strCUSNAME = strCUSNAME + dtblTempCopy.Rows[k]["CUSNAME"].ToString() + "<br>";
                    //    }
                    //}

                    //DataRow drowPaySV = dtblSV.NewRow();
                    //drowPaySV["userID"] = dtblTemp.Rows[0]["UserID"].ToString();
                    //drowPaySV["UserName"] = dtblTemp.Rows[0]["UserName"].ToString();
                    //drowPaySV["SerialNo"] = dtblTemp.Rows[0]["SerialNo"].ToString();
                    //drowPaySV["Address"] = dtblTemp.Rows[0]["Zip"].ToString() + dtblTemp.Rows[0]["Add1"].ToString() + dtblTemp.Rows[0]["Add2"].ToString() + dtblTemp.Rows[0]["Add3"].ToString();
                    //drowPaySV["Consignee"] = dtblTemp.Rows[0]["Consignee"].ToString();
                    //drowPaySV["EndDate"] = dtblTemp.Rows[0]["EndDate"].ToString();
                    //drowPaySV["Type"] = dtblTemp.Rows[0]["Type"].ToString();
                    //drowPaySV["CardNo"] = strCardList;
                    //drowPaySV["BLKCode"] = strBLK;
                    //drowPaySV["Status"] = dtblTemp.Rows[0]["Status"].ToString();
                    //drowPaySV["ClearDate"] = dtblTemp.Rows[0]["ClearDate"].ToString();
                    //drowPaySV["ApplyUser"] = dtblTemp.Rows[0]["ApplyUser"].ToString();
                    //drowPaySV["AgreeUser"] = dtblTemp.Rows[0]["AgreeUser"].ToString();
                    //drowPaySV["TypeCard"] = strTypeCard;
                    //drowPaySV["CUSID"] = strCUSID;
                    //drowPaySV["CUSNAME"] = strCUSNAME;
                    //drowPaySV["IsPaidOffAlone"] = dtblTemp.Rows[0]["IsPaidOffAlone"].ToString();

                    //dtblSV.Rows.Add(drowPaySV);
                    #endregion
                    string strNo = "";
                    string strDBSerialNo = "";
                    string strUserID = "";
                    string strUserName = "";
                    string strAddress = "";
                    string strConsignee = "";
                    string strDBEndDate = "";
                    string strType = "";
                    string strCardNo = "";
                    string strBLKCode = "";
                    string strStatus = "";
                    string strClearDate = "";
                    string strApplyUser = "";
                    string strAgreeUser = "";
                    string strTypeCard = "";
                    string strCUSID = "";
                    string strCUSNAME = "";
                    string IsPaidOffAlone = "";
                    if (dtblSum.Rows[i]["SerialNo"].ToString().Trim() != strSerialNo)
                    {
                        iTotal = iTotal + 1;
                        strNo = iTotal.ToString();
                        strUserID = dtblSum.Rows[i]["UserID"].ToString().Trim();
                        strUserName = dtblSum.Rows[i]["UserName"].ToString().Trim();                        
                        strAddress = dtblSum.Rows[i]["Zip"].ToString().Trim() + 
                                    dtblSum.Rows[i]["Add1"].ToString().Trim() + 
                                    dtblSum.Rows[i]["Add2"].ToString().Trim() + 
                                    dtblSum.Rows[i]["Add3"].ToString().Trim();
                        strConsignee = dtblSum.Rows[i]["Consignee"].ToString().Trim();
                        strDBEndDate = dtblSum.Rows[i]["EndDate"].ToString().Trim();
                        strType = dtblSum.Rows[i]["Type"].ToString().Trim();
                        strStatus = dtblSum.Rows[i]["Status"].ToString().Trim();
                        strClearDate = dtblSum.Rows[i]["ClearDate"].ToString().Trim();
                        strApplyUser = dtblSum.Rows[i]["ApplyUser"].ToString().Trim();
                        strAgreeUser = dtblSum.Rows[i]["AgreeUser"].ToString().Trim();
                        IsPaidOffAlone = dtblSum.Rows[i]["IsPaidOffAlone"].ToString().Trim();
                        strSerialNo = dtblSum.Rows[i]["SerialNo"].ToString().Trim();
                        strDBSerialNo = strSerialNo;
                    }
                    strCardNo = dtblSum.Rows[i]["CardNo"].ToString().Trim();
                    strBLKCode = dtblSum.Rows[i]["BLK"].ToString().Trim();
                    strTypeCard = dtblSum.Rows[i]["TypeCard"].ToString().Trim();
                    strCUSID = dtblSum.Rows[i]["CUSID"].ToString().Trim();
                    strCUSNAME = dtblSum.Rows[i]["CUSNAME"].ToString().Trim();

                    if (dtblSum.Rows[i]["TypeCard"].ToString().Trim() == "M")
                    {
                        strTypeCard = "0";
                        strCUSID = "";
                        strCUSNAME = "";
                    }
                    else if (dtblSum.Rows[i]["TypeCard"].ToString().Trim() == "E")
                    {
                        strTypeCard = "1";
                        strCUSID = dtblSum.Rows[i]["CUSID"].ToString().Trim();
                        strCUSNAME = dtblSum.Rows[i]["CUSNAME"].ToString().Trim();
                    }
                    else
                    {
                        strTypeCard = "1";
                        strCUSID = dtblSum.Rows[i]["CUSID"].ToString().Trim();
                        strCUSNAME = dtblSum.Rows[i]["CUSNAME"].ToString().Trim();
                    }

                    DataRow drowPaySV = dtblSV.NewRow();
                    drowPaySV["No"] = strNo;
                    drowPaySV["userID"] = strUserID;
                    drowPaySV["UserName"] = strUserName;
                    drowPaySV["SerialNo"] = strDBSerialNo;
                    drowPaySV["Address"] = strAddress;
                    drowPaySV["Consignee"] = strConsignee;
                    drowPaySV["EndDate"] = strDBEndDate;
                    drowPaySV["Type"] = strType;
                    drowPaySV["CardNo"] = strCardNo;
                    drowPaySV["BLKCode"] = strBLKCode;
                    drowPaySV["Status"] = strStatus;
                    drowPaySV["ClearDate"] = strClearDate;
                    drowPaySV["ApplyUser"] = strApplyUser;
                    drowPaySV["AgreeUser"] = strAgreeUser;
                    drowPaySV["TypeCard"] = strTypeCard;
                    drowPaySV["CUSID"] = strCUSID;
                    drowPaySV["CUSNAME"] = strCUSNAME;
                    drowPaySV["IsPaidOffAlone"] = IsPaidOffAlone;
                    dtblSV.Rows.Add(drowPaySV);

                }

                dtblReutrnCountSV = BRReport.SearchOnDataSet(sqlcmdReutrnCountSV).Tables[0];
                iReutrnCount = int.Parse(dtblReutrnCountSV.Rows[0]["counts"].ToString());   //* 退件數
                dtblMakeUpCountSV = BRReport.SearchOnDataSet(sqlcmdMakeUpCountSV).Tables[0];
                iMakeUpCount = int.Parse(dtblMakeUpCountSV.Rows[0]["counts"].ToString());   //* 補發件數
                dtblVoidCountSV = BRReport.SearchOnDataSet(sqlcmdVoidCountSV).Tables[0];
                iVoidCounts = int.Parse(dtblVoidCountSV.Rows[0]["counts"].ToString());      //* 銷件數

                DataRow drowExtraData = dtblExtraData.NewRow();
                drowExtraData["Char1"] = iTotal;                    //* 總計件數
                drowExtraData["Char2"] = iTotal - iVoidCounts;      //* 免手續費件數
                drowExtraData["Int1"] = iVoidCounts;                //* 銷件數
                drowExtraData["Int2"] = iReutrnCount;               //* 退件數
                drowExtraData["CountMakeUp"] = iMakeUpCount;        //* 補發件數
                dtblExtraData.Rows.Add(drowExtraData);

                dstResult.Tables.Add(dtblExtraData);

                dtblSV.TableName = "Pay_SV";
                dstResult.Tables.Add(dtblSV.DefaultView.ToTable());
                return true;
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp);
                strMsgID = "04_02020100_003";
                return false;
            }
        }



        #endregion


        #region 公用方法
        /// <summary>
        /// 檢核輸入的日期
        /// </summary>
        /// <param name="dtmBeforeData">起始日期</param>
        /// <param name="dtmEndData">結束日期</param>
        /// <returns></returns>
        public static bool CheckDataTime(DateTime dtmBeforeData, DateTime dtmEndData)
        {
            try
            {
                System.TimeSpan diff1 = dtmBeforeData.Subtract(dtmEndData);
                int iDay = diff1.Days;
                if (iDay > 0)
                {
                    return false;
                }
                return true;

            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp);
                return false;
            }


        }

        /// <summary>
        /// 檢核是否為日期格式
        /// </summary>
        /// <param name="dtmBeforeData">起始日期</param>
        /// <returns></returns>
        public static bool CheckData(string strDate)
        {
            try
            {
                DateTime dtmOut = DateTime.Parse("1900/01/01");
                if (!Function.IsDateTime(strDate, out dtmOut))
                {
                    return false;
                }
                return true;
            }
            catch (Exception exp)
            {
                BRReport.SaveLog(exp);
                return false;
            }
        }

        #region 阿拉伯數字轉換為中文數字
        /// <summary>
        /// 金額轉換
        /// </summary>
        /// <param name="strMoney"></param>
        /// <returns></returns>
        public static string ChangeEMoney2CMoney(string strMoney)
        {
            int HasNum = 0;

            string ChinaString = null;


            if (strMoney == "")
            {
                ChinaString = "零";
            }

            string[] ChinaNumWord = new string[] { "零", "壹", "貳", "參", "肆", "伍", "陸", "柒", "捌", "玖" };
            string[] ChinaWord = new string[] { "拾", "佰", "仟", "萬", "億", "兆" };




            for (int iNumPosition = 1; iNumPosition <= strMoney.Length; iNumPosition++)
            {
                switch (iNumPosition % 4)
                {
                    case 1:
                        switch (iNumPosition / 4)
                        {
                            case 1:
                                ChinaString = ChinaWord[3] + ChinaString; //萬
                                break;
                            case 2:
                                ChinaString = ChinaWord[4] + ChinaString; //億
                                break;
                            case 3:
                                ChinaString = ChinaWord[5] + ChinaString; //兆
                                break;
                        }
                        break;
                    case 2:
                        ChinaString = ChinaWord[0] + ChinaString; //拾
                        break;
                    case 3:
                        ChinaString = ChinaWord[1] + ChinaString; //佰
                        break;
                    case 0:
                        ChinaString = ChinaWord[2] + ChinaString; //仟
                        break;
                }
                ChinaString = ChinaNumWord[System.Convert.ToInt32(strMoney.Substring(strMoney.Length - iNumPosition, 1))] + ChinaString;
            }


            ChinaString = ProZero(ChinaString);

            HasNum = (ChinaString.IndexOf("億", 0) + 1);
            if (HasNum != 0 && ChinaString.Substring(HasNum, 1) == "萬")
            {
                return  ChinaString.Substring(0, HasNum) + ChinaString.Substring(ChinaString.Length - (ChinaString.Length - HasNum - 1));
            }
            else
            {
                return ChinaString;
            }
            
        }

        private static string ProZero(string HaveZero)
        {
            string tempProZero = null;
            int PosHere = 0;
            //string BerforeString = null;
            string AfterString = null;
            PosHere = (HaveZero.IndexOf("零", 0) + 1);
            if (PosHere != 0 & HaveZero.Length != PosHere)
            {
                if (HaveZero.Substring(PosHere + 2 - 1, 1) == "零")
                {
                    AfterString = HaveZero.Substring(0, PosHere - 1);
                    if (HaveZero.Substring(PosHere, 1) == "兆" || HaveZero.Substring(PosHere, 1) == "億" || HaveZero.Substring(PosHere, 1) == "萬")
                    {
                        tempProZero = AfterString + HaveZero.Substring(PosHere, 1) + ProZero(HaveZero.Substring(PosHere + 2 - 1));
                    }
                    else
                    {
                        tempProZero = AfterString + ProZero(HaveZero.Substring(PosHere + 2 - 1));
                    }
                }
                else
                {
                   if (HaveZero.Substring(PosHere, 1) == "兆" || HaveZero.Substring(PosHere, 1) == "億" || HaveZero.Substring(PosHere, 1) == "萬")
                    {
                        AfterString = HaveZero.Substring(0, PosHere - 1);
                        tempProZero = AfterString + HaveZero.Substring(PosHere, 1) + ProZero(HaveZero.Substring(PosHere + 2 - 1));
                    }
                    else
                    {
                        AfterString = HaveZero.Substring(0, PosHere);
                        tempProZero = AfterString + ProZero(HaveZero.Substring(PosHere + 2 - 1));
                    }
                }
            }
            else
            {
                if (PosHere != 0)
                {
                    tempProZero = HaveZero.Substring(0, PosHere - 1);
                }
                else
                {
                    tempProZero = HaveZero;
                }
            }
           return tempProZero;
       }
        #endregion 阿拉伯數字轉為中文數字



       #region 阿拉伯數字轉換為中文數字
       /// <summary>
       /// 金額轉換
       /// </summary>
       /// <param name="Source"></param>
       /// <returns></returns>
        public static string TransChr1(string Source)
        {
            string tempTransChr1 = null;
            int I = 0;
            string ch = null;


            for (I = 1; I <= Source.Length; I++)
            {
                ch = Source.Substring(I - 1, 1);
                switch (ch)
                {
                    case "1":
                        if (I == 1 && (Source.Length == 2 || Source.Length == 6 || Source.Length == 10))
                        {

                        }
                        else
                        {
                            tempTransChr1 = tempTransChr1 + "一";
                        }
                        break;
                    case "2":
                        tempTransChr1 = tempTransChr1 + "二";
                        break;
                    case "3":
                        tempTransChr1 = tempTransChr1 + "三";
                        break;
                    case "4":
                        tempTransChr1 = tempTransChr1 + "四";
                        break;
                    case "5":
                        tempTransChr1 = tempTransChr1 + "五";
                        break;
                    case "6":
                        tempTransChr1 = tempTransChr1 + "六";
                        break;
                    case "7":
                        tempTransChr1 = tempTransChr1 + "七";
                        break;
                    case "8":
                        tempTransChr1 = tempTransChr1 + "八";
                        break;
                    case "9":
                        tempTransChr1 = tempTransChr1 + "九";
                        break;
                }

                switch (Source.Length - I + 1)
                {
                    case 9: 
                        tempTransChr1 = tempTransChr1 + "億";
                        break;
                    case 5: 
                        tempTransChr1 = tempTransChr1 + "萬";
                        break;
                    case 8:
                    case 4: 
                        if (ch != "0")
                        {
                            tempTransChr1 = tempTransChr1 + "千";
                        }
                        else if (Source.Substring(I, 1) != "0")
                        {
                            tempTransChr1 = tempTransChr1 + "零";
                        }
                        break;
                    case 11:
                    case 7:
                    case 3:
                        if (ch != "0")
                        {
                            tempTransChr1 = tempTransChr1 + "百";
                        }
                        else if (Source.Substring(I, 1) != "0")
                        {
                            tempTransChr1 = tempTransChr1 + "零";
                        }
                        break;
                    case 10:
                    case 6:
                    case 2: 
                        if (ch != "0")
                        {
                            tempTransChr1 = tempTransChr1 + "十";
                        }
                        else if (Source.Substring(I, 1) != "0")
                        {
                            tempTransChr1 = tempTransChr1 + "零";
                        }
                        break;
                }
            }
            return tempTransChr1;
        }
       #endregion
       
#endregion
    }


}
