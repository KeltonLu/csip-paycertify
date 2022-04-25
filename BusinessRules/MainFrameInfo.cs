//******************************************************************
//*  作    者：余洋(rosicky)
//*  功能說明：主機信息作業

//*  創建日期：2009/07/28
//*  修改記錄：

//*<author>            <time>            <TaskID>                <desc>
//* 宋戈                2009/12/21      無                  1.整理格式
//*                                                         2.修改電文開關設置
//* Ares Stanley    2021/12/24  20210058-CSIP作業服務平台現代化II    調整HTG範本路徑取用方式, Log紀錄方式
//* Ares Stanley    2022/02/14    20210058-CSIP作業服務平台現代化II    調整webconfig取參數方式
//*******************************************************************
using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Xml;
using CSIPCommonModel.EntityLayer;
using Framework.Common.HTG;
using Framework.Common.JavaScript;
using CSIPCommonModel.BaseItem;
using System.IO;
using Framework.Common;
using Framework.Common.Logging;
using Framework.Common.Message;
using Framework.Common.Utility;

/// <summary>
/// 主機操作類

/// </summary>
public class MainFrameInfo
{

    /// <summary>
    /// 上傳并取得主機資料信息(無分頁)
    /// </summary>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="htInput">傳入參數的HashTable</param>
    /// <param name="blnIsClose">是否關閉主機Session</param>
    /// <returns>傳出參數的HashTable</returns>
    public static Hashtable GetMainframeData(string strTransactionId, Hashtable htInput, bool blnIsClose, string strType, EntityAGENT_INFO eAgentInfo)
    {
        MainFrameInfo.AddSession(htInput, eAgentInfo, strTransactionId);

        string strHtgMessage = "";
        string strErrorMessage = strTransactionId;


        string strIsOnLine = GetStr(strTransactionId, "ISONLINE");//*是否上線

        ArrayList arrRet = new ArrayList();

        Hashtable htOutput = new Hashtable();

        HTGCommunicator hc = new HTGCommunicator();

        string strFileName = "";

        strFileName = Configure.HTGTempletPath + "req" + strTransactionId + ".xml";
        string strMsg = "";
        string SessionId = "";

        //*取得電文的SessionId
        if (htInput.Contains("sessionId"))
        {
            SessionId = htInput["sessionId"].ToString();
        }

        //*如果SessionId為空,需要連接主機得到電文SessionId
        if (SessionId == "")
        {
            if (!hc.LogonAuth(htInput, ref strMsg, strIsOnLine))
            {
                htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");

                if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
                {
                    htOutput.Add("HtgMsgFlag", "2");
                }
                else
                {
                    htOutput.Add("HtgMsgFlag", "0");//*顯示端末訊息標識
                }
                return htOutput;
            }
            else
            {
                htOutput.Add("sessionId", hc.SessionId);
                htInput["sessionId"] = hc.SessionId;
            }
        }
        else
        {
            hc.SessionId = SessionId;
        }

        if (HttpContext.Current != null)
        {
            HttpContext.Current.Session["sessionId"] = hc.SessionId;
        }

        #region 建立reqHost物件

        HTGhostgateway reqHost = new HTGhostgateway();
        try
        {
            hc.RequestHostCreator(strFileName, ref reqHost, htInput);
        }
        catch
        {
            strMsg = "req" + strTransactionId + ".xml格式不正確或文件不存在";
            Logging.Log(strErrorMessage + ":" + strMsg, LogState.Fatal, LogLayer.HTG);
            htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");
            if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
            {
                htOutput.Add("HtgMsgFlag", "2");
            }
            else
            {
                htOutput.Add("HtgMsgFlag", "1");//*顯示端末訊息標識
            }
            return htOutput;
        }
        #endregion

        HTGhostgateway rtnHost = new HTGhostgateway();
        try
        {
            #region 取得rtnHost物件
            strMsg = hc.QueryHTG(UtilHelper.GetAppSettings("HtgHttp").ToString(), reqHost, ref rtnHost, htInput, strIsOnLine);
            if (htOutput.Contains("sessionId"))
            {
                htOutput["sessionId"] = hc.SessionId;
            }
            else
            {
                htOutput.Add("sessionId", hc.SessionId);
            }

            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session["sessionId"] = hc.SessionId;
            }

            if (strMsg != "")
            {
                //*"Session超時,..."
                htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");
                if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
                {
                    htOutput.Add("HtgMsgFlag", "2");
                }
                else
                {
                    htOutput.Add("HtgMsgFlag", "0");//*顯示端末訊息標識
                }
                return htOutput;
            }

            #endregion

            #region 判別rtnHost是否正確
            if (rtnHost.body != null)
            {
                strMsg = "主機連線失敗:" + rtnHost.body.msg.Value;
                Logging.Log(strErrorMessage + ":" + strMsg, LogState.Fatal, LogLayer.HTG);
                htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");
                if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
                {
                    htOutput.Add("HtgMsgFlag", "2");
                }
                else
                {
                    htOutput.Add("HtgMsgFlag", "0");//*顯示端末訊息標識
                }
                return htOutput;
            }
            #endregion
            #region 處理主機錯誤訊息
            //*主機錯誤公共判斷
            if (!hc.HTGMsgParser(rtnHost, ref strMsg))
            {
                //*"下行電文為空"
                htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");
                if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
                {
                    htOutput.Add("HtgMsgFlag", "2");
                }
                else
                {
                    htOutput.Add("HtgMsgFlag", "0");//*顯示端末訊息標識
                }
                Logging.Log(strErrorMessage + ":" + strMsg, LogState.Fatal, LogLayer.HTG);
                return htOutput;
            }

            #region 將資料塞入HashTable
            for (int i = 0; i < rtnHost.line.Count; i++)
            {
                if (rtnHost.line[i].msgBody.data != null)
                {
                    for (int j = 0; j < rtnHost.line[i].msgBody.data.Count; j++)
                    {
                        htOutput.Add(rtnHost.line[i].msgBody.data[j].ID.Trim(), rtnHost.line[0].msgBody.data[j].Value);
                    }
                }
            }
            #endregion

            string strLog = "";//*記錄錯誤日志變數

            arrRet = InitHTGOutputFields(strTransactionId, strType);
            //*檢核主機欄位
            if (!CheckHtgColumn(ref htOutput, arrRet, strErrorMessage))
            {
                return htOutput;
            }

            switch (strTransactionId)
            {
                case "P4_JCEH":
                    if (htOutput["MESSAGE_TYPE"].ToString() != "0000" && htOutput["MESSAGE_TYPE"].ToString() != "0001")
                    {
                        strHtgMessage = GetMessageType(strTransactionId, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(strTransactionId, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;
                case "P4_JCAS":
                    if (htOutput["MESSAGE_TYPE"].ToString() != "0000" && htOutput["MESSAGE_TYPE"].ToString() != "0001")
                    {
                        strHtgMessage = GetMessageType(strTransactionId, htOutput["MESSAGE_TYPE"].ToString());
                        htOutput.Add("HtgMsg", strHtgMessage);
                        htOutput.Add("HtgMsgFlag", "0");//*顯示主機訊息標識
                        strLog = htOutput["MESSAGE_TYPE"].ToString() + " " + strHtgMessage;
                    }
                    else
                    {
                        htOutput.Add("HtgSuccess", htOutput["MESSAGE_TYPE"].ToString() + " " + GetMessageType(strTransactionId, htOutput["MESSAGE_TYPE"].ToString()));
                    }
                    break;
            }

            if (htOutput.Contains("HtgMsg"))
            {

                htOutput["HtgMsg"] = strTransactionId + ":" + strLog;

                Logging.Log(strErrorMessage + ":" + strLog, LogState.Fatal, LogLayer.HTG);
                return htOutput;
            }
            else
            {

                htOutput["HtgSuccess"] = strTransactionId + ":" + htOutput["HtgSuccess"].ToString();
            }
            #endregion
        }
        catch (Exception ex)
        {
            strMsg = hc.ExceptionHandler(ex, "主機電文錯誤:");
            htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");

            if (strType == "100" || strType == "200")//*判斷批次作業錯誤類型
            {
                if (!htOutput.Contains("HtgMsgFlag"))
                {
                    htOutput.Add("HtgMsgFlag", "2");//*顯示主機訊息標識
                }
                else
                {
                    htOutput["HtgMsgFlag"] = "2";//*顯示主機訊息標識
                }
            }
            else
            {
                if (!htOutput.Contains("HtgMsgFlag"))
                {
                    htOutput.Add("HtgMsgFlag", "1");//*顯示主機訊息標識
                }
                else
                {
                    htOutput["HtgMsgFlag"] = "1";//*顯示主機訊息標識
                }
            }
            Logging.Log(strErrorMessage + ":" + strMsg, LogState.Fatal, LogLayer.HTG);
            return htOutput;
        }
        finally
        {
            //*根據需求可在下行電文后關閉或者不關閉連接
            if (blnIsClose)
            {
                if (!hc.CloseSession(ref strMsg))
                {
                    if (htOutput.Contains("HtgMsg"))
                    {
                        htOutput["HtgMsg"] = htOutput["HtgMsg"].ToString() + "  " + strMsg;
                    }
                    else
                    {
                        htOutput.Add("HtgMsg", strMsg);
                    }
                }
                else
                {
                    if (htOutput.Contains("sessionId"))
                    {
                        htOutput["sessionId"] = "";
                    }
                    else
                    {
                        htOutput.Add("sessionId", "");
                    }
                }
            }
        }
        return htOutput;

    }

    /// <summary>
    /// 作者 竹香
    /// 創建日期：2009/10/23
    /// 上傳并取得主機資料信息(含分頁)
    /// </summary>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="htInput">傳入參數的HashTable</param>
    /// <param name="stringArray">需要采集的欄位集合</param>
    /// <returns>主機返回的HashTable</returns>
    public static DataTable GetMainframeDataByPage(string strTransactionId, Hashtable htInput, bool blnIsClose, string strType, EntityAGENT_INFO eAgentInfo)
    {
        int intLineCNT = 0;
        int intOldLineCNT = 0;
        int intMaxLineCNT = int.Parse(GetStr(strTransactionId, "PAGECNT"));
        string strLineCNT = "";
        string strLINE_CNT = GetStr(strTransactionId, "LINE_CNT");
        int intTimes = 0;
        string strMsg = "";

        ArrayList arrRet = new ArrayList();

        MainFrameInfo.AddSession(htInput, eAgentInfo, strTransactionId);

        //* 根據傳入欄位建立DataTable
        DataTable dtblOutput = new DataTable();

        arrRet = InitHTGOutputFields(strTransactionId, strType);

        for (int i = 0; i < arrRet.Count; i++)
        {
            dtblOutput.Columns.Add(arrRet[i].ToString(), System.Type.GetType("System.String"));
        }
        dtblOutput.Columns.Add("HtgSuccess", System.Type.GetType("System.String"));
        dtblOutput.Columns.Add("HtgErrMsg", System.Type.GetType("System.String"));
        // 增加MESSAGE_TYPE欄位 by Ares Stanley 20220210
        //dtblOutput.Columns.Add("MESSAGE_TYPE", System.Type.GetType("System.String"));

        try
        {
            do
            {
                //* 上次電文得到的line
                strLineCNT = intLineCNT.ToString();
                strLineCNT = strLineCNT.PadLeft(4, '0');
                intOldLineCNT = int.Parse(strLineCNT);
                htInput[strLINE_CNT] = strLineCNT;
                //*連接主機得到
                strMsg = MainFrameInfo.GetMainFrameInfo(ref dtblOutput, strTransactionId, htInput, false, arrRet, intOldLineCNT, (intTimes * intMaxLineCNT));
                intTimes = intTimes + 1;
                if (strMsg != "")
                {
                    if (dtblOutput.Rows.Count <= 0)
                    {
                        DataRow drowOutput = dtblOutput.NewRow();
                        drowOutput["HtgErrMsg"] = strMsg;
                        dtblOutput.Rows.Add(drowOutput);
                    }
                    return dtblOutput;
                }

                for (int i = dtblOutput.Rows.Count - 1; i >= 0; i--)
                {
                    if (dtblOutput.Rows[i][strLINE_CNT].ToString().Trim() != "")
                    {
                        intLineCNT = int.Parse(dtblOutput.Rows[i][strLINE_CNT].ToString().Trim());
                        break;
                    }
                }

            } while (intLineCNT > (intTimes * intMaxLineCNT));
        }
        catch
        {
            strMsg = strTransactionId + "下行電文解析錯誤";
            DataRow drowOutput = dtblOutput.NewRow();
            drowOutput["HtgErrMsg"] = strMsg;
            dtblOutput.Rows.Add(drowOutput);
            Logging.Log(strMsg, LogState.Fatal, LogLayer.HTG);
        }
        return dtblOutput;
    }



    /// <summary>
    /// 上傳并取得主機資料信息(分頁用)
    /// </summary>
    /// <param name="DataTable">傳出的信息</param>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="htInput">傳入參數的HashTable</param>
    /// <param name="blnIsClose">是否關閉主機Session</param>
    /// <param name="stringArray">需要采集的欄位集合</param>
    /// <returns>傳出參數的DataTable</returns>
    private static string GetMainFrameInfo(ref DataTable dtblOutput, string strTransactionId, Hashtable htInput, bool blnIsClose, ArrayList arrRet, int intOldLineCNT, int intLastGet)
    {
        string strHtgMessage = "";
        string strErrorMessage = strTransactionId;


        string strIsOnLine = GetStr(strTransactionId, "ISONLINE");//*是否上線

        Hashtable htOutput = new Hashtable();

        HTGCommunicator hc = new HTGCommunicator();

        string strFileName = "";

        strFileName = Configure.HTGTempletPath + "req" + strTransactionId + ".xml";
        string strMsg = "";
        string SessionId = "";

        //*取得電文的SessionId
        if (htInput.Contains("sessionId"))
        {
            SessionId = htInput["sessionId"].ToString();
        }

        //*如果SessionId為空,需要連接主機得到電文SessionId
        if (SessionId == "")
        {
            if (!hc.LogonAuth(htInput, ref strMsg, strIsOnLine))
            {
                Logging.Log(strMsg, LogState.Fatal, LogLayer.HTG);
                return strTransactionId + ":" + strMsg + " ";
            }
            else
            {
                htInput["sessionId"] = hc.SessionId;
            }
        }
        else
        {
            hc.SessionId = SessionId;
        }

        if (HttpContext.Current != null)
        {
            HttpContext.Current.Session["sessionId"] = hc.SessionId;
        }




        #region 建立reqHost物件
        //* 創建XML頭
        HTGhostgateway reqHost = new HTGhostgateway();
        try
        {
            hc.RequestHostCreator(strFileName, ref reqHost, htInput);
        }
        catch
        {
            strMsg = "req" + strTransactionId + ".xml格式不正確或文件不存在";
            Logging.Log(strErrorMessage + ":" + strMsg, LogState.Fatal, LogLayer.HTG);
            return strTransactionId + ":" + strMsg + " ";
        }
        #endregion


        HTGhostgateway rtnHost = new HTGhostgateway();
        try
        {
            #region 取得rtnHost物件
            //* 連接HTG查詢資料

            strMsg = hc.QueryHTG(UtilHelper.GetAppSettings("HtgHttp").ToString(), reqHost, ref rtnHost, htInput, strIsOnLine);
            //* 如果strMsg不?空代表出錯

            if (strMsg != "")
            {
                Logging.Log(strMsg, LogState.Fatal, LogLayer.HTG);
                return strTransactionId + ":" + strMsg + " ";
            }

            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session["sessionId"] = hc.SessionId;
            }
            #endregion

            #region 判別rtnHost是否正確
            if (rtnHost.body != null)
            {
                strMsg = "主機連線失敗:" + rtnHost.body.msg.Value;
                Logging.Log(strErrorMessage + ":" + strMsg, LogState.Fatal, LogLayer.HTG);
                return strTransactionId + ":" + strMsg + " ";
            }
            #endregion

            #region 處理主機錯誤訊息
            //*主機錯誤公共判斷
            if (!hc.HTGMsgParser(rtnHost, ref strMsg))
            {
                Logging.Log(strErrorMessage + ":" + strMsg, LogState.Fatal, LogLayer.HTG);
                return strTransactionId + ":" + strMsg + " ";
            }
            string strLINE_CNT = "";
            string strMESSAGE_TYPE = "";

            strLINE_CNT = GetStr(strTransactionId, "LINE_CNT");
            strMESSAGE_TYPE = GetStr(strTransactionId, "MESSAGE_TYPE");

            #endregion

            #region 取得筆數
            //* 取得消息類型
            string strMessageType = "";
            for (int i = 0; i < rtnHost.line.Count; i++)
            {
                rtnHost.line[i].msgBody.data.QueryDataByID(strMESSAGE_TYPE, ref strMessageType);
                if (strMessageType != "")
                {
                    break;
                }
            }
            #endregion

            #region 根據電文,返回的MessageType不同,返回不同的消息

            //* 根據電文,返回的MessageType不同,返回不同的消息
            if (strMessageType != "0000" && strMessageType != "0001")
            {
                strMsg = strTransactionId + ":" + " " + strMessageType + " " + GetMessageType(strTransactionId, strMessageType);
                DataRow drowOutput = dtblOutput.NewRow();
                drowOutput[strMESSAGE_TYPE] = strMessageType;
                drowOutput["HtgErrMsg"] = strMsg;
                dtblOutput.Rows.Add(drowOutput);
                return strMsg;
            }


            string strCount = "";
            //* 取得筆數
            for (int i = 0; i < rtnHost.line.Count; i++)
            {
                rtnHost.line[i].msgBody.data.QueryDataByID(strLINE_CNT, ref strCount);
                if (strCount != "")
                {
                    break;
                }
            }

            #endregion

            int intIndex = 0;
            string strTemp = "";

            int intCount = int.Parse(strCount);

            int intMaxCount = int.Parse(GetStr(strTransactionId, "PAGECNT"));


            intCount = intCount - intLastGet;
            //* 如果返回總筆數>每頁筆數
            if (intCount > intMaxCount)
            {
                intCount = intMaxCount;
            }

            #region 將資料塞入DataTable
            for (int cnt = 1; cnt <= intCount; cnt++)
            {
                DataRow drowOutput = dtblOutput.NewRow();

                for (int j = 0; j < arrRet.Count; j++)
                {
                    for (int i = 0; i < rtnHost.line.Count; i++)
                    {
                        strTemp = arrRet[j] + cnt.ToString();

                        intIndex = rtnHost.line[i].msgBody.data.QueryDataByID(strTemp);

                        if (intIndex != -1)
                        {
                            drowOutput[arrRet[j].ToString()] = rtnHost.line[i].msgBody.data[intIndex].Value;
                            break;
                        }
                        else
                        {
                            if (cnt == 1)
                            {
                                strTemp = strTemp.Substring(0, strTemp.Length - 1);
                                intIndex = rtnHost.line[i].msgBody.data.QueryDataByID(strTemp);
                                if (intIndex != -1)
                                {
                                    drowOutput[arrRet[j].ToString()] = rtnHost.line[i].msgBody.data[intIndex].Value;
                                    break;
                                }
                                else
                                {
                                    drowOutput[arrRet[j].ToString()] = "";
                                }
                            }
                            else
                            {
                                drowOutput[arrRet[j].ToString()] = "";
                            }
                        }
                    }

                }
                dtblOutput.Rows.Add(drowOutput);
            }
            #endregion
        }
        catch (Exception ex)
        {
            Logging.Log(strMsg, LogState.Fatal, LogLayer.HTG);
            strMsg = hc.ExceptionHandler(ex, "主機電文錯誤:");
            return strMsg;
        }
        finally
        {
            //*根據需求可在下行電文后關閉或者不關閉連接
            if (blnIsClose)
            {
                string strMessage = "";
                if (!hc.CloseSession(ref strMessage, strIsOnLine))
                {
                    strMsg = strMsg + "  " + strMessage;
                }
                System.Web.HttpContext.Current.Session["sessionId"] = "";
            }
        }
        return strMsg;

    }

    /// <summary>
    /// 檢查Session中是否有主機SessionID,如果有則發送電文關閉,並且清空這個SessionID
    /// </summary>
    /// <returns></returns>
    public static bool ClearSession()
    {
        //string strMsg = "";
        //string strIsOnline = ConfigurationManager.AppSettings["AUTH_IsOnLine"];
        //try
        //{
        //    //* 取得Session中存的主機SessionID
        //    string strSessionID = (System.Web.HttpContext.Current.Session["sessionId"] + "").Trim();
        //    if (!string.IsNullOrEmpty(strSessionID))
        //    {
        //        //* 如果不?空.發送主機電文,關掉
        //        HTGCommunicator hc = new HTGCommunicator();
        //        hc.SessionId = strSessionID;
        //        hc.CloseSession(ref strMsg, strIsOnline);
        //        //* 清空Session中的主機SessionID
        //        System.Web.HttpContext.Current.Session["sessionId"] = "";
        //    }
        //}
        //catch
        //{
        //    Logging.SaveLog(ELogLayer.HTG, strMsg, ELogType.Fatal);
        //    return false;
        //}
        return true;
    }
    /// <summary>
    /// 作者 余洋
    /// 創建日期：2009/10/23
    /// 取得電文參數
    /// </summary>
    /// <param name="strType">電文枚舉類型</param>
    /// <param name="strTemp">傳入參數</param>
    /// <returns>返回的字符串</returns>
    public static string GetStr(string strType, string strTemp)
    {
        //* 欄位說明:
        //* strType 電文ID
        //* strTemp USER_ID         -   用戶ID
        //*         LINE_CNT        -   筆數欄位名稱
        //*         LINECNT         -   筆數欄位每頁最大數量

        //*         MESSAGE_TYPE    -   錯誤訊息返回欄位名稱
        //*         ISONLINE        -   是否已經上線
        switch (strType)
        {
            case "P4_JCAB":
                #region P4_JCAB
                switch (strTemp)
                {
                    case "USER_ID":
                        //* 用戶ID
                        return "USER_ID";
                    case "LINE_CNT":
                        //* 筆數欄位名稱
                        return "LINE_CNT";
                    case "PAGECNT":
                        //* 筆數欄位每頁最大數量

                        return "0020";
                    case "MESSAGE_TYPE":
                        //* 錯誤訊息返回欄位名稱
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCAB_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion
            case "P4_JCAC":
                #region P4_JCAC
                switch (strTemp)
                {
                    case "USER_ID":
                        return "USER_ID";
                    case "LINE_CNT":
                        return "LINE_CNT";
                    case "PAGECNT":
                        return "0020";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCAC_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion
            case "P4_JCEH":
                #region P4_JCEH
                switch (strTemp)
                {
                    case "USER_ID":
                        return "USER_ID";
                    case "LINE_CNT":
                        return "LINE_CNT";
                    case "PAGECNT":
                        return "0007";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCEH_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion
            case "P4_JCU9":
                #region P4_JCU9
                switch (strTemp)
                {
                    case "USER_ID":
                        return "USER_ID";
                    case "LINE_CNT":
                        return "LINE_CNT";
                    case "PAGECNT":
                        return "0020";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCU9_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion
            case "P4_JCII":
                #region P4_JCII
                switch (strTemp)
                {
                    case "USER_ID":
                        return "USER_ID";
                    case "LINE_CNT":
                        return "LINE_CNT";
                    case "PAGECNT":
                        return "0020";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCII_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion
            case "P4_JCFK":
                #region P4_JCFK
                switch (strTemp)
                {
                    case "USER_ID":
                        return "USER_ID";
                    case "LINE_CNT":
                        return "LINE_CNT";
                    case "PAGECNT":
                        return "0020";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCFK_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion
            case "P4_JCHN":
                #region P4_JCHN
                switch (strTemp)
                {
                    case "USER_ID":
                        return "USER_ID";
                    case "LINE_CNT":
                        return "LINE_CNT";
                    case "PAGECNT":
                        return "0030";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCHN_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion
            case "P4_JCAS":
                #region P4_JCAS
                switch (strTemp)
                {
                    case "USER_ID":
                        return "USER_ID";
                    case "LINE_CNT":
                        return "LINE_CNT";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* 是否已經上線
                        return UtilHelper.GetAppSettings("P4_JCAS_IsOnLine");
                    case "AUTHONLINE":
                        return UtilHelper.GetAppSettings("AUTH_IsOnLine");
                }
                break;
                #endregion
        }

        return "";
    }

    //*作者 趙呂梁
    //*創建日期：2009/12/22
    //*修改日期：2009/12/22
    /// <summary>
    /// 獲取信息說明中文內容
    /// </summary>
    /// <param name="type">電文枚舉類型</param>
    /// <param name="strMessageCode">代碼</param>
    /// <returns>中文說明</returns>
    public static string GetMessageType(string strTransactionId, string strMessageCode)
    {
        string strMsg = "";
        if (strMessageCode == "")
        {
            strMsg = "主機回覆碼為空，請確認主機回傳是否正確 ";
            return strMsg;
        }

        switch (strTransactionId)
        {
            case "P4_JCAB":
                switch (strMessageCode)
                {
                    case "0001":
                        break;
                    case "8888":
                        strMsg = "該筆資料不存在";
                        break;
                    case "9999":
                        strMsg = "主機系統錯誤";
                        break;
                    default:
                        strMsg = "其他錯誤";
                        break;
                }
                break;

            case "P4_JCAC":
                switch (strMessageCode)
                {
                    case "0001":
                        break;
                    case "8888":
                        strMsg = "該筆資料不存在";
                        break;
                    case "9999":
                        strMsg = "主機系統錯誤";
                        break;
                    default:
                        strMsg = "其他錯誤";
                        break;
                }
                break;

            case "P4_JCAS":
                switch (strMessageCode)
                {
                    case "0001":
                        strMsg = "成功. ";
                        break;
                    case "0000":
                        strMsg = "成功. ";
                        break;
                    case "6002":
                        strMsg = "結清日期必須正確";
                        break;
                    case "6011":
                        strMsg = "已結清或出完最後一期";
                        break;
                    case "6012":
                        strMsg = "試算結清日，必須不小於當天日期 ";
                        break;
                    case "6013":
                        strMsg = "日期計算錯誤";
                        break;
                    case "6021":
                        strMsg = "已結清，不可做REVERSE";
                        break;
                    case "6022":
                        strMsg = "未做提前結清試算註記，不用做 REVERSE";
                        break;
                    case "6023":
                        strMsg = "預定清償日為前二個作日內，不能還原";
                        break;
                    case "6024":
                        strMsg = "LOANP-STATUS=0 OR 5不可做借新還舊";
                        break;
                    case "8888":
                        strMsg = "NOT FOUND";
                        break;
                    case "9999":
                        strMsg = "NOT OPEN";
                        break;
                    default:
                        strMsg = "其他錯誤";
                        break;
                }
                break;

            case "P4_JCII":
                switch (strMessageCode)
                {
                    case "0001":
                        break;
                    case "7001":
                        strMsg = "訂單檔JCVKIPOL未開";
                        break;
                    case "7002":
                        strMsg = "訂單檔JCVHIPOL未開";
                        break;
                    case "7003":
                        strMsg = "特店檔JCVKIPMR未開";
                        break;
                    case "8001":
                        strMsg = "查無此訂單檔JCVKIPOL資料";
                        break;
                    case "8002":
                        strMsg = "查無此訂單檔JCVHIPOL資料";
                        break;
                    case "8003":
                        strMsg = "查無此特店檔JCVKIPMR資料";
                        break;
                    case "9999":
                        strMsg = "系統異常 !";
                        break;
                    default:
                        strMsg = "其他錯誤";
                        break;
                }
                break;

            case "P4_JCEH":
                switch (strMessageCode)
                {
                    case "0001":
                        strMsg = "成功. ";
                        break;
                    case "0000":
                        strMsg = "成功. ";
                        break;
                    case "6666":
                        strMsg = "卡片檔NOT FOUND";
                        break;
                    case "7777":
                        strMsg = "卡人卡片關係檔NOT FOUND";
                        break;
                    case "8888":
                        strMsg = "該筆資料不存在";
                        break;
                    case "9999":
                        strMsg = "檔案未開";
                        break;
                    default:
                        strMsg = "其他錯誤";
                        break;
                }
                break;

            case "P4_JCFK":
                switch (strMessageCode)
                {
                    case "0001":
                        break;
                    case "1002":
                        strMsg = "功能碼有誤";
                        break;
                    case "1003":
                        strMsg = "ID未輸入";
                        break;
                    case "1004":
                        strMsg = "JCVKIDRL檔案未開啟";
                        break;
                    case "1005":
                        strMsg = "IDRL　START　NOT FOUND";
                        break;
                    case "8888":
                        strMsg = "無此筆資料，請重新輸入";
                        break;
                    case "9999":
                        strMsg = "其他錯誤";
                        break;
                    default:
                        strMsg = "其他錯誤";
                        break;
                }
                break;

            case "P4_JCU9":
                switch (strMessageCode)
                {
                    case "0000":
                    case "0001":
                        break;
                    case "8888":
                        strMsg = "無此筆資料，請重新輸入";
                        break;
                    case "9999":
                        strMsg = "其他錯誤";
                        break;
                    default:
                        strMsg = "其他錯誤";
                        break;
                }
                break;
            case "P4_JCHN":
                switch (strMessageCode)
                {
                    case "0000":
                        break;
                    case "1001":
                        //strMsg = "己無次頁資訊";
                        break;
                    case "8001":
                        strMsg = "歸戶ID 輸入錯誤";
                        break;
                    case "8002":
                        strMsg = "檔案未開啟";
                        break;
                    case "8883":
                        strMsg = "ID NOT FOUND";
                        break;
                    case "8888":
                        strMsg = "ID NOT FOUND";
                        break;
                    case "8884":
                        strMsg = "卡片 NOT FOUND";
                        break;
                    case "9999":
                        strMsg = "其他錯誤";
                        break;
                    default:
                        strMsg = "其他錯誤";
                        break;
                }
                break;

            default:
                switch (strMessageCode)
                {
                    case "0001":
                    case "0000":
                        strMsg = "成功. ";
                        break;
                    case "9999":
                        strMsg = "系統異常 ! ";
                        break;
                    default:
                        strMsg = MessageHelper.GetMessage("01_00000000_030");
                        break;
                }
                break;
        }
        return strMsg;
    }

     /// <summary>
    /// 檢核主機回傳欄位
    /// </summary>
    /// <param name="htOutput">主機回傳Hashtbale</param>
    /// <param name="arrRet">欄位集合</param>
    /// <param name="strErrorMessage">錯誤提示信息</param>
    /// <returns>true成功，false失敗</returns>
    public static bool CheckHtgColumn(ref Hashtable htOutput, ArrayList arrRet, string strErrorMessage)
    {
        foreach (string strTemp in arrRet)
        {
            if (!htOutput.ContainsKey(strTemp))
            {
                string strMsg = "主機電文錯誤";
                htOutput.Add("HtgMsg", strErrorMessage + ":" + strMsg + " ");
                if (!htOutput.Contains("HtgMsgFlag"))
                {
                    htOutput.Add("HtgMsgFlag", "1");//*顯示主機訊息標識
                }
                else
                {
                    htOutput["HtgMsgFlag"] = "1";//*顯示主機訊息標識
                }
                Logging.Log(strErrorMessage + ":" + strMsg, LogState.Fatal, LogLayer.HTG);
                return false;
            }
        }
        return true;
    }

        /// <summary>
    /// 添加上傳主機SESSION信息
    /// </summary>
    /// <param name="htInput">上傳主機HashTable</param>
    /// <param name="eAgentInfo">Session變數集合</param>
    /// <param name="type">電文類型</param>
    public static void AddSession(Hashtable htInput, EntityAGENT_INFO eAgentInfo, string strTransactionId)
    {
        if (htInput.ContainsKey("userId"))
        {
            htInput["userId"] = eAgentInfo.agent_id;
        }
        else
        {
            htInput.Add("userId", eAgentInfo.agent_id);
        }

        if (htInput.ContainsKey("passWord"))
        {
            htInput["passWord"] = eAgentInfo.agent_pwd;
        }
        else
        {
            htInput.Add("passWord", eAgentInfo.agent_pwd);
        }


        if (htInput.ContainsKey("racfId"))
        {
            htInput["racfId"] = eAgentInfo.agent_id_racf;
        }
        else
        {
            htInput.Add("racfId", eAgentInfo.agent_id_racf);
        }

        if (htInput.ContainsKey("racfPassWord"))
        {
            htInput["racfPassWord"] = eAgentInfo.agent_id_racf_pwd;
        }
        else
        {
            htInput.Add("racfPassWord", eAgentInfo.agent_id_racf_pwd);
        }

        if (htInput.ContainsKey("USER_ID"))
        {
            htInput["USER_ID"] = eAgentInfo.agent_id_racf;
        }
        else
        {
            htInput.Add(GetStr(strTransactionId, "USER_ID"), eAgentInfo.agent_id_racf);
        }


        if (HttpContext.Current != null)//*判斷是JOB還是網頁中上傳主機
        {
            if (HttpContext.Current.Session["sessionId"] != null)
            {
                if (htInput.ContainsKey("sessionId"))
                {
                    htInput["sessionId"] = HttpContext.Current.Session["sessionId"].ToString().Trim();
                }
                else
                {
                    htInput.Add("sessionId", HttpContext.Current.Session["sessionId"].ToString().Trim());
                }
            }
            else
            {
                if (htInput.ContainsKey("sessionId"))
                {
                    htInput["sessionId"] = "";
                }
                else
                {
                    htInput.Add("sessionId", "");

                }
            }
        }
        else
        {
            if (!htInput.ContainsKey("sessionId"))
            {
                htInput.Add("sessionId", "");
            }
        }
    }

    private static ArrayList InitHTGOutputFields(string strTransactionId, string strType)
    {
        ArrayList arrRet = new ArrayList();
        switch (strTransactionId)
        {
            case "P4_JCEH":
                if (strType == "1") //ID查詢
                {
                    arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", "LINE_CNT", "ACCT_NBR", "CUST_ID", 
                                            "WGHT", "ADDR_DATE", "SINCE", "SHORT_NAME", "CUST_CODE", "FILLER05", "CUST_VIP_CODE", "FILLER06", 
                                            "DTE_BIRTH", "DELQ", "SPACES1", "EMPLOYER", "SPACES2", "POSITION", "OFFICE_PHONE", "HOME-PHONE", "ZIP", 
                                            "CITY", "ADDR_1", "ADDR_2", "CRG", "ARG", "CYCLE", "CO_OWNER", "DD_MAINT_DATE", "GRADUATE_YYMM", "POSSWORD", "SERV_GRADE", 
                                            "CARD_TYPE","CARDHOLDER","SUNIT","SALES_NAME","OPENED","EXPIR_DTE","CUSTID","CARDHOLDER_NAME","EMBOSSER_NAME_1",
                                            "ACCT_CUST_ID","STATUS","BLOCK","BLOCK_DTE","ALT_BLOCK","ALT_BLOCK_DTE","REGION_CODE","CATEGORY","INDICATOR",
                                            "USER_CODE","CARD_CYCLE","OASM_M","NMBR_OUTST_AUTH","AMNT_OUTST_AUTH","CHIP","CM_CRLIMIT","CURR_BAL","CLINE_EXCEED",
                                            "APP_FLAG","SAVE_ACCT","COLLATERAL_CODE","AMC_FLAG","DTE_LST_PYMT"});
                }
                break;

            case "P4_JCAS":
                if (strType == "1") //ID查詢
                {
                    arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", "LINE_CNT", "ACCT_NBR", 
                                            "SET_DATE", "CARD_NO", "NEW_CARD_NO", "AMT", "DATE", "PERIOD_AMT", "PERIOD_NO", "PAID_CNT", 
                                            "FREE_INT_NO", "PERIOD_NO_DIS", "CURR_BALANCE", "RTL_BALANCE", "TOT_BALANCE", "LATE_CHARGE", 
                                            "CASH_BALANCE", "BREAK_AMT", "SHORT_NAME", "RATE", "MATURITY_DATE", "LAST_PAY_DATE", "LAST_TXN_DATE", 
                                            "BREAK_EXP_DATE", "BREAK_RATE", "CUST_ID", "PERIOD_AMT_DIS", "RATE_DIS", "INT_DAY", "NEST_PAY_DATE", 
                                            "SETTLE_DATE", "SETTLE_MAIN_DATE", "PRODUCT", "PROJ_CODE", "BLOCK_CODE", "BLOCK_DATE", "FEE", "FEE_RATE", 
                                            "PRE_SETTLE_DATE", "TOT_AMT", "RATE_DIF", "PP_FEE_RATE", "PP_FLAG", "RC_CL_INT", "PERIOD_NO_NEW", 
                                            "UNPAID_CNT", "PERIOD_NO_NEW", "LNBGEDT", "LNBGNO", "LNPCFEE", "PRMRATE", "PRMAMT", "ACCT", "NGFLAG", 
                                            "NGNO_FLAG" });
                }
                break;

            case "P4_JCAB":
                if (strType == "1") 
                {
                    arrRet = new ArrayList(new object[] { "LINE_CNT" });
                }
                break;

            case "P4_JCAC":
                if (strType == "1")
                {
                    arrRet = new ArrayList(new object[] { "LINE_CNT" });
                }
                break;

            case "P4_JCII":
                if (strType == "1")
                {
                    arrRet = new ArrayList(new object[] { "TRAN_ID","PROGRAM_ID","USER_ID","MESSAGE_TYPE","FUNCTION_CODE","LINE_CNT","ACCT_NBR","ORDER_NO","NO",
                                                        "CARD_NO","ACCT_ID","MERCH_NO","MERCH_NAME","AUTH_CODE","INSTALLMENT_TIMES","INSTALLMENT_AMT","ORDER_QTY",
                                                        "ORDER_AMT","TIME_ORIG","LST_DATE_BILLED","REFUND_TIMES","REFUND_AMT","REFUND_DATE","STATUS","CATEGPRY_IND",
                                                        "UNBILLED_AMT"});
                }
                break;

            case "P4_JCFK":
                if (strType == "1")
                {
                    arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", "LINE_CNT", "ACCT_NBR", "IDRL_CARD_ID", 
                                                        "IDRL_SEQ", "ACCT_FLAG", "RELATION", "CARD_NO", "CUSTOMER_IND", "CARDHOLDER_IND", "EXPIRE_DATE", "BLOCK_CODE", 
                                                        "CURRENT_CODE", "OPEN_DATE", "BLOCK_DATE", "CUST_NO", "CUST_FLAG", "1331_MEMO" });
                }
                break;

            case "P4_JCU9":
                if (strType == "1")
                {
                    arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", "LINE_CNT", "ACCT_NBR", "ORDER_NO", 
                                                        "ORDER_AMT", "ORDER_TIMES", "ORDER_BAL", "UNPAY_TIMES", "CARD_NO" });
                }
                break;

            case "P4_JCHN":
                if (strType == "1")
                {
                    arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE", "LINE_CNT", "IDNO", "LAST_KEY", "NAME", 
                                                        "ADDR1", "ADDR2", "STAS", "STASD", "BALAMT", "LPAYD", "LPAYT", "CARDNUM", "CARDDTE", "ALID", "STOPREC", 
                                                        "CUSTID" });
                }
                break;
            case "P4_JC99":
                if (strType == "1")
                {
                    arrRet = new ArrayList(new object[] { "TRAN_ID", "PROGRAM_ID", "USER_ID", "MESSAGE_TYPE", "FUNCTION_CODE",
                                                              "NAME","ROMA","PARENT_NAME","PARENT_ROMA","IN_CFLAG","IN_CHANNEL",
                                                              "IN_PFLAG","FILLER"});
                }
                if (strType == "2")
                {
                    arrRet = new ArrayList(new object[] { "MESSAGE_TYPE", "MESSAGE_CHI" });
                }
                break;
        }
        return arrRet;

    }
}


