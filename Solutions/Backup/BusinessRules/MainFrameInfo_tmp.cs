//******************************************************************
//*  �@    �̡G�E�v(rosicky)
//*  �\�໡���G�D���H���@�~
//*  �Ыؤ���G2009/07/28
//*  �ק�O���G
//*<author>            <time>            <TaskID>                <desc>
//* ����                2009/12/21      �L                  1.��z�榡
//*                                                         2.�ק�q��}���]�m
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
using Framework.Common.Logging;
using Framework.Common.Message;

/// <summary>
/// �D���ާ@��
/// </summary>
public class MainFrameInfo_tmp
{


    /// <summary>
    /// �@�� �E�v
    /// �Ыؤ���G2009/10/23
    /// ���o�q��Ѽ�
    /// </summary>
    /// <param name="strType">�q��T�|����</param>
    /// <param name="strTemp">�ǤJ�Ѽ�</param>
    /// <returns>��^���r�Ŧ�</returns>
    public static string GetStr(string strType,  string strTemp)
    {
        //* ��컡��:
        //* strType �q��ID
        //* strTemp USER_ID         -   �Τ�ID
        //*         LINE_CNT        -   �������W��
        //*         LINECNT         -   �������C���̤j�ƶq
        //*         MESSAGE_TYPE    -   ���~�T����^���W��
        //*         ISONLINE        -   �O�_�w�g�W�u
        switch (strType)
        {
            case "P4_JCAB":
                #region P4_JCAB
                switch (strTemp)
                {
                    case "USER_ID":
                        //* �Τ�ID
                        return "USER_ID";       
                    case "LINE_CNT":
                        //* �������W��
                        return "LINE_CNT";
                    case "LINECNT":
                        //* �������C���̤j�ƶq
                        return "0020";
                    case "MESSAGE_TYPE":
                        //* ���~�T����^���W��
                        return "MESSAGE_TYPE"; 
                    case "ISONLINE":
                        //* �O�_�w�g�W�u
                        return ConfigurationManager.AppSettings["P4_JCAB_IsOnLine"];
                    case "AUTHONLINE":
                        return ConfigurationManager.AppSettings["AUTH_IsOnLine"]; 
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
                    case "LINECNT":
                        return "0020";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* �O�_�w�g�W�u
                        return ConfigurationManager.AppSettings["P4_JCAC_IsOnLine"];
                    case "AUTHONLINE":
                        return ConfigurationManager.AppSettings["AUTH_IsOnLine"]; 
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
                    case "LINECNT":
                        return "0007";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* �O�_�w�g�W�u
                        return ConfigurationManager.AppSettings["P4_JCEH_IsOnLine"];
                    case "AUTHONLINE":
                        return ConfigurationManager.AppSettings["AUTH_IsOnLine"]; 
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
                    case "LINECNT":
                        return "0020";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* �O�_�w�g�W�u
                        return ConfigurationManager.AppSettings["P4_JCU9_IsOnLine"];
                    case "AUTHONLINE":
                        return ConfigurationManager.AppSettings["AUTH_IsOnLine"]; 
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
                    case "LINECNT":
                        return "0020";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* �O�_�w�g�W�u
                        return ConfigurationManager.AppSettings["P4_JCII_IsOnLine"];
                    case "AUTHONLINE":
                        return ConfigurationManager.AppSettings["AUTH_IsOnLine"]; 
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
                    case "LINECNT":
                        return "0020";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* �O�_�w�g�W�u
                        return ConfigurationManager.AppSettings["P4_JCFK_IsOnLine"];
                    case "AUTHONLINE":
                        return ConfigurationManager.AppSettings["AUTH_IsOnLine"]; 
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
                    case "LINECNT":
                        return "0030";
                    case "MESSAGE_TYPE":
                        return "MESSAGE_TYPE";
                    case "ISONLINE":
                        //* �O�_�w�g�W�u
                        return ConfigurationManager.AppSettings["P4_JCHN_IsOnLine"];
                    case "AUTHONLINE":
                        return ConfigurationManager.AppSettings["AUTH_IsOnLine"]; 
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
                        //* �O�_�w�g�W�u
                        return ConfigurationManager.AppSettings["P4_JCAS_IsOnLine"];
                    case "AUTHONLINE":
                        return ConfigurationManager.AppSettings["AUTH_IsOnLine"]; 
                }
                break;
                #endregion
        }

        return "";
    }
    
    /// <summary>
    /// �@�� �E�v
    /// �Ыؤ���G2009/10/23
    /// �W�Ǧ}���o�D����ƫH��(�t����)
    /// </summary>
    /// <param name="type">�q��T�|����</param>
    /// <param name="htInput">�ǤJ�Ѽƪ�HashTable</param>
    /// <param name="stringArray">�ݭn��������춰�X</param>
    /// <returns>�D����^��HashTable</returns>
    public static DataTable GetMainframeData(Hashtable htInput, string strType, ref string strMsg, string[] stringArray)
    {
        EntityAGENT_INFO eAgentInfo = new EntityAGENT_INFO();
        eAgentInfo = (EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"]; //*Session�ܼƶ��X
        //*�K�[�W�ǥD���H��
        htInput.Add("userId", eAgentInfo.agent_id);
        htInput.Add("passWord", eAgentInfo.agent_pwd);
        htInput.Add("racfId", eAgentInfo.agent_id_racf);
        htInput.Add("racfPassWord", eAgentInfo.agent_id_racf_pwd);
        htInput.Add(GetStr(strType, "USER_ID"), eAgentInfo.agent_id_racf);
        htInput.Add(GetStr(strType, "LINE_CNT"), "0000");

        //* �ھڶǤJ���إ�DataTable
        DataTable dtblOutput = new DataTable();        
        for (int i = 0; i < stringArray.Length; i++)
        {
            dtblOutput.Columns.Add(stringArray[i], System.Type.GetType("System.String"));
        }               

        //*�o����������D���Ǧ^�H��
        strMsg = MainFrameInfo_tmp.GetMainFramePagesInfo(ref dtblOutput, strType, htInput, false, stringArray);

        return dtblOutput;
    }
    
    /// <summary>
    /// �W�Ǧ}���o�D����ƫH��(�L����)
    /// </summary>
    /// <param name="type">�q��T�|����</param>
    /// <param name="htInput">�ǤJ�Ѽƪ�HashTable</param>
    /// <param name="blnIsClose">�O�_�����D��Session</param>
    /// <returns>�ǥX�Ѽƪ�HashTable</returns>
    public static Hashtable GetMainframeData(Hashtable htInput, string strType, ref string strMsg, bool blnIsClose, string[] stringArray)
    {
        string strIsOnLine = GetStr(strType, "ISONLINE");           //* �ӹq��O�_�W�u
        string strAuthOnLine = GetStr(strType, "AUTHONLINE");       //* HTG�n�J�n�X�O�_�W�u
        EntityAGENT_INFO eAgentInfo = new EntityAGENT_INFO();
        eAgentInfo = (EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"]; //*Session�ܼƶ��X
        #region �K�[�W�ǥD���H��
        //*�K�[�W�ǥD���H��
        htInput.Add("userId", eAgentInfo.agent_id);
        htInput.Add("passWord", eAgentInfo.agent_pwd);
        htInput.Add("racfId", eAgentInfo.agent_id_racf);
        htInput.Add("racfPassWord", eAgentInfo.agent_id_racf_pwd);
        
        Hashtable htOutput = new Hashtable();
        HTGCommunicator hc = new HTGCommunicator();
        string strFileName = Configure.HTGTempletPath + "req" + strType + ".xml";
        #endregion

        

        #region ���o�q�媺SessionId
        string SessionId = "";
        //*���o�q�媺SessionId
        if (System.Web.HttpContext.Current.Session["sessionId"] != null && System.Web.HttpContext.Current.Session["sessionId"].ToString() != "")
        {
            SessionId = System.Web.HttpContext.Current.Session["sessionId"].ToString();
        }
        //*�p�GSessionId����,�ݭn�s���D���o��q��SessionId
        if (SessionId == "")
        {
            if (!hc.LogonAuth(htInput, ref strMsg, strAuthOnLine))
            {
                Logging.SaveLog(ELogLayer.HTG, strMsg, ELogType.Fatal);
                return htOutput;
            }
            else
            {
                if (!blnIsClose)
                {
                    System.Web.HttpContext.Current.Session["sessionId"] = hc.SessionId;

                }
            }
        }
        else
        {
            hc.SessionId = SessionId;
        }

        if (htInput.Contains("sessionId"))
        {
            htInput.Remove("sessionId");
        }
        htInput.Add("sessionId", SessionId);
        #endregion

        #region �إ�reqHost����

        HTGhostgateway reqHost = new HTGhostgateway();
        try
        {
            hc.RequestHostCreator(strFileName, ref reqHost, htInput);
        }
        catch
        {
            strMsg = "req" + strType + ".xml�榡�����T�Τ�󤣦s�b";
            Logging.SaveLog(ELogLayer.HTG, strMsg, ELogType.Fatal);
            return htOutput;
        }
        #endregion


        HTGhostgateway rtnHost = new HTGhostgateway();
        try
        {
            #region ���ortnHost����
            
            strMsg = hc.QueryHTG(ConfigurationManager.AppSettings["HtgHttp"].ToString(), reqHost, ref rtnHost, htInput, strIsOnLine);
            if (strMsg != "")
            {
                Logging.SaveLog(ELogLayer.HTG, strMsg, ELogType.Fatal);
                return htOutput;
            }
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
            #endregion

            #region �P�OrtnHost�O�_���T
            if (rtnHost.body != null)
            {
                strMsg = "�D���s�u����:" + rtnHost.body.msg.Value;
                Logging.SaveLog(ELogLayer.HTG, strMsg, ELogType.Fatal);
                return htOutput;
            }
            #endregion

            #region �B�z�D�����~�T��
            //*�D�����~���@�P�_
            if (!hc.HTGMsgParser(rtnHost, ref strMsg))
            {
                Logging.SaveLog(ELogLayer.HTG, strMsg, ELogType.Fatal);
                return htOutput;
            }
            #endregion

            #region �q�LMessageType�o����~�T��
            string strMESSAGE_TYPE =GetStr(strType, "MESSAGE_TYPE");

            string strMessageType = "";
            for (int i = 0; i < rtnHost.line.Count; i++)
            {
                rtnHost.line[i].msgBody.data.QueryDataByID(strMESSAGE_TYPE, ref strMessageType);
                if (strMessageType != "")
                {
                    break;
                }
            }


            switch (strType)
            {
                //*�D��P4_JCAB�@�~
                case "P4_JCEH":
                    switch (strMessageType)
                    {
                        case "0000":
                        case "0001":
                            break;
                        case "6666":
                            strMsg = "�d����NOT FOUND";
                            return htOutput;
                        case "7777":
                            strMsg = "�d�H�d�����Y��NOT FOUND";
                            return htOutput;
                        case "8888":
                            strMsg = "�ӵ���Ƥ��s�b";
                            return htOutput;
                        case "9999":
                            strMsg = "�ɮץ��}";
                            return htOutput;
                        default:
                            strMsg = "��L���~";
                            return htOutput;
                    }
                    break;
                //*�D��P4_JCAS�@�~
                case "P4_JCAS":
                    switch (strMessageType)
                    {
                        case "0001":
                            break;
                        case "6002":
                            strMsg = "���M����������T";
                            return htOutput;
                        case "6011":
                            strMsg = "�w���M�ΥX���̫�@��";
                            return htOutput;
                        case "6012":
                            strMsg = "�պ⵲�M��A�������p����Ѥ�� ";
                            return htOutput;
                        case "6013":
                            strMsg = "����p����~";
                            return htOutput;
                        case "6021":
                            strMsg = "�w���M�A���i��REVERSE";
                            return htOutput;
                        case "6022":
                            strMsg = "�������e���M�պ���O�A���ΰ� REVERSE";
                            return htOutput;
                        case "6023":
                            strMsg = "�w�w�M�v�鬰�e�G�ӧ@�餺�A�����٭�";
                            return htOutput;
                        case "6024":
                            strMsg = "LOANP-STATUS=0 OR 5���i���ɷs����";
                            return htOutput;
                        case "8888":
                            strMsg = "NOT FOUND";
                            return htOutput;
                        case "9999":
                            strMsg = "NOT OPEN";
                            return htOutput;
                        default:
                            strMsg = "��L���~";
                            return htOutput;
                    }
                    break;
            }
            #endregion

            #region �N��ƶ�JHashTable
            for (int i = 0; i < rtnHost.line.Count; i++)
            {
                for (int j = 0; j < rtnHost.line[i].msgBody.data.Count; j++)
                {
                    htOutput.Add(rtnHost.line[i].msgBody.data[j].ID, rtnHost.line[0].msgBody.data[j].Value);
                }

            }
            #endregion

            #region �P�_�һݶǦ^���O�_���w�Ǧ^

            for (int i = 0; i < stringArray.Length; i++)
            {
                if (!htOutput.Contains(stringArray[i]))
                {
                    strMsg = "����D����ƥ���";
                    Logging.SaveLog(ELogLayer.HTG, strMsg, ELogType.Fatal);
                    return htOutput;
                }
            }
            #endregion
        }
        catch (Exception ex)
        {
            strMsg = hc.ExceptionHandler(ex, "�D���q����~:");
            Logging.SaveLog(ELogLayer.HTG, strMsg, ELogType.Fatal);
            return htOutput;
        }
        finally
        {
            //*�ھڻݨD�i�b�U��q��Z�����Ϊ̤������s��
            if (blnIsClose)
            {
                string strMessage = "";
                if (!hc.CloseSession(ref strMessage, strAuthOnLine))
                {
                    strMsg = strMsg + "  " + strMessage;
                }
                System.Web.HttpContext.Current.Session["sessionId"] = "";
            }
        }
        return htOutput;

    }


    /// <summary>
    /// �D���������@��k
    /// </summary>
    /// <param name="DataTable">�ǥX���H��</param>
    /// <param name="type">�q��T�|����</param>
    /// <param name="htInput">�ǤJ�Ѽƪ�HashTable</param>
    /// <param name="blnIsClose">�O�_�����D��Session</param>
    /// <param name="stringArray">�ݭn��������춰�X</param>
    /// <returns>�ǥX�����~�H��</returns>
    private static string GetMainFramePagesInfo(ref DataTable dtblOutput, string strType, Hashtable htInput, bool blnIsClose, string[] stringArray)
    {
        int intLineCNT = 0;
        int intOldLineCNT = 0;
        int intMaxLineCNT = int.Parse(GetStr(strType, "LINECNT"));
        string strLineCNT = "";
        string strMsg = "";
        string strLINE_CNT = GetStr(strType, "LINE_CNT");
        int intTimes = 0;
        try
        {
            do
            {
                

                //* �W���q��o�쪺line
                strLineCNT = intLineCNT.ToString();
                strLineCNT = strLineCNT.PadLeft(4, '0');
                intOldLineCNT = int.Parse(strLineCNT);             
                htInput[strLINE_CNT] = strLineCNT;
                //*�s���D���o��
                strMsg = MainFrameInfo_tmp.GetMainFrameInfo(ref dtblOutput, strType, htInput, false, stringArray, intOldLineCNT, (intTimes * intMaxLineCNT) );
                intTimes = intTimes + 1;
                if (strMsg != "")
                {
                    return strMsg;
                }

                for (int i = dtblOutput.Rows.Count-1; i >= 0; i--)
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
            strMsg="�U��q��ѪR���~";
            Logging.SaveLog(ELogLayer.HTG, strMsg, ELogType.Fatal);
            return strMsg;
        }
        return "";
    }

    /// <summary>
    /// �W�Ǧ}���o�D����ƫH��(������)
    /// </summary>
    /// <param name="DataTable">�ǥX���H��</param>
    /// <param name="type">�q��T�|����</param>
    /// <param name="htInput">�ǤJ�Ѽƪ�HashTable</param>
    /// <param name="blnIsClose">�O�_�����D��Session</param>
    /// <param name="stringArray">�ݭn��������춰�X</param>
    /// <returns>�ǥX�Ѽƪ�DataTable</returns>
    private static string GetMainFrameInfo(ref DataTable dtblOutput, string strTransactionId, Hashtable htInput, bool blnIsClose, string[] stringArray, int intOldLineCNT, int intLastGet)
    {
        HTGCommunicator hc = new HTGCommunicator();
        string strFileName = Configure.HTGTempletPath + "req" + strTransactionId + ".xml";
        string strMsg = "";
        string SessionId = "";
        string strIsOnLine = GetStr(strTransactionId, "ISONLINE");  //* �q��O�_�W�u
        string strAuthOnLine = GetStr(strTransactionId, "AUTHONLINE");       //* HTG�n�J�n�X�O�_�W�u

              
        #region �ШD��Session
        //*���o�q�媺SessionId
        if (System.Web.HttpContext.Current.Session["sessionId"] != null && System.Web.HttpContext.Current.Session["sessionId"].ToString()!="")
        {
            SessionId = System.Web.HttpContext.Current.Session["sessionId"].ToString();
        }

        //*�p�GSessionId����,�ݭn�s���D���o��q��SessionId
        if (SessionId == "")
        {
            if (!hc.LogonAuth(htInput, ref strMsg, strAuthOnLine))
            {
                Logging.SaveLog(ELogLayer.HTG, strMsg, ELogType.Fatal);
                return strMsg;
            }
            else
            {
                if (!blnIsClose)
                {
                    //* �p�G���O�ݭn�������h�Nsessionid�s�JSession��
                    System.Web.HttpContext.Current.Session["sessionId"] = hc.SessionId;

                }
            }
        }
        else
        {
            hc.SessionId = SessionId;
        }

        if (htInput.Contains("sessionId"))
        {
            htInput.Remove("sessionId");
        }
        htInput.Add("sessionId", SessionId);
        #endregion

        #region �إ�reqHost����
        //* �Ы�XML�Y



        HTGhostgateway reqHost = new HTGhostgateway();
        try
        {
            hc.RequestHostCreator(strFileName, ref reqHost, htInput);
        }
        catch
        {
            strMsg = "req" + strTransactionId + ".xml�榡�����T�Τ�󤣦s�b";
            Logging.SaveLog(ELogLayer.HTG, strMsg, ELogType.Fatal);
            return strMsg;
        }
        #endregion


        HTGhostgateway rtnHost = new HTGhostgateway();
        try
        {
            #region ���ortnHost����
            //* �s��HTG�d�߸��
            
            strMsg = hc.QueryHTG(ConfigurationManager.AppSettings["HtgHttp"].ToString(), reqHost, ref rtnHost, htInput, strIsOnLine);
            //* �p�GstrMsg��?�ťN���X��
            if (strMsg != "")
            {
                Logging.SaveLog(ELogLayer.HTG, strMsg, ELogType.Fatal);
                return strMsg;
            }

            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session["sessionId"] = hc.SessionId;
            }
            #endregion

            #region �P�OrtnHost�O�_���T
            if (rtnHost.body != null)
            {
                strMsg = "�D���s�u����:" + rtnHost.body.msg.Value;
                Logging.SaveLog(ELogLayer.HTG, strMsg, ELogType.Fatal);
                return strMsg;
            }
            #endregion

            #region �B�z�D�����~�T��
            //*�D�����~���@�P�_
            if (!hc.HTGMsgParser(rtnHost, ref strMsg))
            {
                Logging.SaveLog(ELogLayer.HTG, strMsg, ELogType.Fatal);
                return strMsg;
            }
            string strLINE_CNT = "";
            string strMESSAGE_TYPE = "";

            strLINE_CNT = GetStr(strTransactionId, "LINE_CNT");
            strMESSAGE_TYPE = GetStr(strTransactionId, "MESSAGE_TYPE");
          
            #endregion

            #region ���o����
   
            string strCount = "";
            //* ���o����
            for (int i = 0; i < rtnHost.line.Count; i++)
            {
                rtnHost.line[i].msgBody.data.QueryDataByID(strLINE_CNT, ref strCount);
                if (strCount != "")
                {
                    break;
                }
            }
            //* �p�G����?0�h��^
            if (strCount == "0" || strCount == "0000" || strCount == "")
            {
                strMsg="�ثe�q��D���W��Ƶ��Ƭ�0";
                Logging.SaveLog(ELogLayer.HTG, strTransactionId + strMsg, ELogType.Info);
                return strMsg;
            }

            //* ���o��������
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

            #region �ھڹq��,��^��MessageType���P,��^���P������
            //* �ھڹq��,��^��MessageType���P,��^���P������
            switch (strTransactionId)
            {
                //*�D��P4_JCAB�@�~
                case "P4_JCAB":
                    switch (strMessageType)
                    {
                        case "0001":
                            break;
                        case "8888":
                            strMsg = "�ӵ���Ƥ��s�b";
                            return strMsg;
                        case "9999":
                            strMsg = "�D���t�ο��~";
                            return strMsg;
                        default:
                            strMsg = "��L���~";
                            return strMsg;
                    }
                    break;
                //*�D��P4_JCAC�@�~
                case "P4_JCAC":
                    switch (strMessageType)
                    {
                        case "0001":
                            break;
                        case "8888":
                            strMsg = "�ӵ���Ƥ��s�b";
                            return strMsg;
                        case "9999":
                            strMsg = "�D���t�ο��~";
                            return strMsg;
                        default:
                            strMsg = "��L���~";
                            return strMsg;
                    }
                    break;
                //*�D��P4_JCEH�@�~
                case "P4_JCEH":
                    switch (strMessageType)
                    {
                        case "0000":
                        case "0001":
                            break;
                        case "6666":
                            strMsg = "�d����NOT FOUND";
                            return strMsg;
                        case "7777":
                            strMsg = "�d�H�d�����Y��NOT FOUND";
                            return strMsg;
                        case "8888":
                            strMsg = "�ӵ���Ƥ��s�b";
                            return strMsg;
                        case "9999":
                            strMsg = "�ɮץ��}";
                            return strMsg;
                        default:
                            strMsg = "��L���~";
                            return strMsg;
                    }
                    break;
                //*�D��P4_JCII�@�~
                case "P4_JCII":
                    switch (strMessageType)
                    {
                        case "0001":
                            break;
                        case "7001":
                            strMsg = "�q����JCVKIPOL���}";
                            return strMsg;
                        case "7002":
                            strMsg = "�q����JCVHIPOL���}";
                            return strMsg;
                        case "7003":
                            strMsg = "�S����JCVKIPMR���}";
                            return strMsg;
                        case "8001":
                            strMsg = "�d�L���q����JCVKIPOL���";
                            return strMsg;
                        case "8002":
                            strMsg = "�d�L���q����JCVHIPOL���";
                            return strMsg;
                        case "8003":
                            strMsg = "�d�L���S����JCVKIPMR���";
                            return strMsg;
                        case "9999":
                            strMsg = "�t�β��` !";
                            return strMsg;
                        default:
                            strMsg = "��L���~";
                            return strMsg;
                    }
                    break;
                //*�D��P4_JCFK�@�~
                case "P4_JCFK":
                    switch (strMessageType)
                    {
                        case "0001":
                            break;
                        case "1002":
                            strMsg = "�\��X���~";
                            return strMsg;
                        case "1003":
                            strMsg = "ID����J";
                            return strMsg;
                        case "1004":
                            strMsg = "JCVKIDRL�ɮץ��}��";
                            return strMsg;
                        case "1005":
                            strMsg = "IDRL�@START�@NOT FOUND";
                            return strMsg;
                        case "8888":
                            strMsg = "�L������ơA�Э��s��J";
                            return strMsg;
                        case "9999":
                            strMsg = "��L���~";
                            return strMsg;
                        default:
                            strMsg = "��L���~";
                            return strMsg;
                    }
                    break;
                //*�D��P4_JCU9�@�~
                case "P4_JCU9":
                    switch (strMessageType)
                    {
                        case "0000":
                        case "0001":
                            break;
                        case "8888":
                            strMsg = "�L������ơA�Э��s��J";
                            return strMsg;
                        case "9999":
                            strMsg = "��L���~";
                            return strMsg;
                        default:
                            strMsg = "��L���~";
                            return strMsg;
                    }
                    break;
                case "P4_JCHN":
                    switch (strMessageType)
                    {
                        case "0000":
                            break;
                        case "1001":
                            //strMsg = "�v�L������T";
                            break;
                        case "8001":
                            strMsg = "�k��ID ��J���~";
                            return strMsg;
                        case "8002":
                            strMsg = "�ɮץ��}��";
                            return strMsg;
                        case "8883":
                            strMsg = "ID NOT FOUND";
                            return strMsg;
                        case "8884":
                            strMsg = "�d�� NOT FOUND";
                            return strMsg;
                        case "9999":
                            strMsg = "��L���~";
                            return strMsg;
                        default:
                            strMsg = "��L���~";
                            return strMsg;
                    }
                    break;
            }
            #endregion
            
            int intIndex =0;
            string strTemp = "";

            int intCount=int.Parse(strCount);

            int intMaxCount = int.Parse(GetStr(strTransactionId, "LINECNT"));
            

            intCount = intCount - intLastGet;
            //* �p�G��^�`����>�C������
            if (intCount > intMaxCount)
            {
                intCount = intMaxCount;
                //if (intOldLineCNT == intCount)
                //{
                //    intCount = 0;
                //}
                //else
                //{
                    //if (intCount % intMaxCount == 0 || intCount % intMaxCount == 1)
                    //{
                    //    intCount = intMaxCount;
                    //}
                    //else
                    //{
                    //    intCount = intCount % intMaxCount;
                    //}
                //}
                
            }

            #region �N��ƶ�JDataTable
            for (int cnt = 1; cnt <= intCount; cnt++)
            {
                DataRow drowOutput = dtblOutput.NewRow();

                for (int j = 0; j < stringArray.Length; j++)
                {
                    for (int i = 0; i < rtnHost.line.Count; i++)
                    {
                        strTemp = stringArray[j] + cnt.ToString();

                        intIndex = rtnHost.line[i].msgBody.data.QueryDataByID(strTemp);

                        if (intIndex != -1)
                        {
                            drowOutput[stringArray[j]] = rtnHost.line[i].msgBody.data[intIndex].Value;
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
                                    drowOutput[stringArray[j]] = rtnHost.line[i].msgBody.data[intIndex].Value;
                                    break;
                                }
                                else
                                {
                                    drowOutput[stringArray[j]] = "";
                                }
                            }
                            else
                            {
                                drowOutput[stringArray[j]] = "";
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
            Logging.SaveLog(ELogLayer.HTG, strMsg, ELogType.Fatal);
            strMsg = hc.ExceptionHandler(ex, "�D���q����~:");
            return strMsg;
        }
        finally
        {
            //*�ھڻݨD�i�b�U��q��Z�����Ϊ̤������s��
            if (blnIsClose)
            {
                string strMessage = "";
                if (!hc.CloseSession(ref strMessage, strAuthOnLine))
                {
                   strMsg= strMsg + "  "+strMessage;
                }
                System.Web.HttpContext.Current.Session["sessionId"] = "";
            }
        }
        return strMsg;

    }

    /// <summary>
    /// �ˬdSession���O�_���D��SessionID,�p�G���h�o�e�q������,�åB�M�ųo��SessionID
    /// </summary>
    /// <returns></returns>
    public static bool ClearSession()
    {
        //string strMsg = "";
        //string strIsOnline = ConfigurationManager.AppSettings["AUTH_IsOnLine"];
        //try
        //{
        //    //* ���oSession���s���D��SessionID
        //    string strSessionID = (System.Web.HttpContext.Current.Session["sessionId"] + "").Trim();
        //    if (!string.IsNullOrEmpty(strSessionID))
        //    {
        //        //* �p�G��?��.�o�e�D���q��,����
        //        HTGCommunicator hc = new HTGCommunicator();
        //        hc.SessionId = strSessionID;
        //        hc.CloseSession(ref strMsg, strIsOnline);
        //        //* �M��Session�����D��SessionID
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
}

   