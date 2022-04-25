//******************************************************************
//*  作    者：Ares Stanley
//*  功能說明：報表查詢、產出
//*  創建日期：2021/11/08
//*  修改記錄：2021/12/20
//*<author>            <time>            <TaskID>                <desc>
//*******************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using Framework.Common.Logging;
using Framework.Common.Utility;
using System.Data.SqlClient;
using Framework.Data;
using CSIPCommonModel.EntityLayer;
using CSIPCommonModel.BaseItem;
using Framework.Common.Message;

namespace CSIPPayCertify.BusinessRules
{
    public class BR_Excel_File
    {

        #region Report01020200 清償證明-清償證明統計表
        /// <summary>
        /// 專案代號:20210058-CSIP作業服務平台現代化II
        /// 功能說明:清償證明-清償證明統計表
        /// 作    者:Ares Stanley
        /// 創建時間:2021/12/20
        /// 修改紀錄:2022/03/14_Ares_Jack_詳細地址資料來源取 M_PROPERTY_CODE.PROPERTY_NAME
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="strRptBeforeDate"></param>
        /// <param name="strRptEndDate"></param>
        /// <param name="strPathFile"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Report01020100(DataSet ds, string strRptBeforeDate, string strRptEndDate, ref string strPathFile, ref string strMsgID)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "dayth.xls";

                DataTable dtExtra = ds.Tables[0];
                DataTable dtCertify = ds.Tables[1];
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                //取得樣式
                HSSFCellStyle contentFormat = getDefaultContentFormat(wb);

                #region 表頭
                strRptBeforeDate = DateHelper.ConvertToMingGuo(strRptBeforeDate);
                strRptEndDate = DateHelper.ConvertToMingGuo(strRptEndDate);
                sheet.GetRow(4).GetCell(0).SetCellValue($"開立日期{strRptBeforeDate.Substring(0, 3) + "/" + strRptBeforeDate.Substring(3, 2) + "/" + strRptBeforeDate.Substring(5, 2) + " ~ " + strRptEndDate.Substring(0, 3) + "/" + strRptEndDate.Substring(3, 2) + "/" + strRptEndDate.Substring(5, 2)}");//開立日期
                string strSenderAddress = string.Empty;
                strSenderAddress = MessageHelper.GetMessage("04_00000000_060");
                sheet.GetRow(5).GetCell(4).SetCellValue($"詳細地址：{strSenderAddress}");//詳細地址
                #endregion

                HSSFCellStyle cs = getDefaultContentFormat(wb);
                #region 表身

                //資料去空白
                removeBlank(ref dtCertify);
                removeBlank(ref dtExtra);

                for (int row = 0; row < dtCertify.Rows.Count; row++)
                {
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col = 0; col < 5; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col);
                        sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = cs;
                    }
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue((row + 1).ToString()); //項次
                    sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dtCertify.Rows[row]["userID"].ToString()); //身分證字號
                    sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(dtCertify.Rows[row]["SerialNo"].ToString()); //清證序號
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(dtCertify.Rows[row]["mailNo"].ToString()); //掛號號碼
                }
                #region 表尾資料格式
                //統計、主管經辦格式
                HSSFCellStyle lastRowFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
                lastRowFormat.VerticalAlignment = VerticalAlignment.Top; //垂直置中
                lastRowFormat.Alignment = HorizontalAlignment.Left; //水平置中
                lastRowFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字
                lastRowFormat.BorderBottom = BorderStyle.None; // 儲存格框線
                lastRowFormat.BorderLeft = BorderStyle.None;
                lastRowFormat.BorderTop = BorderStyle.None;
                lastRowFormat.BorderRight = BorderStyle.None;

                HSSFFont lastRowFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                lastRowFont.FontHeightInPoints = 10; //字體大小
                lastRowFont.FontName = "新細明體"; //字型
                lastRowFormat.SetFont(lastRowFont); //設定儲存格的文字樣式

                //ps列格式
                HSSFCellStyle psRowFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
                psRowFormat.VerticalAlignment = VerticalAlignment.Top; //垂直置中
                psRowFormat.Alignment = HorizontalAlignment.Left; //水平置中
                psRowFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字
                psRowFormat.BorderBottom = BorderStyle.None; // 儲存格框線
                psRowFormat.BorderLeft = BorderStyle.None;
                psRowFormat.BorderTop = BorderStyle.None;
                psRowFormat.BorderRight = BorderStyle.None;

                HSSFFont psRowFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                psRowFont.FontHeightInPoints = 10; //字體大小
                psRowFont.FontName = "新細明體"; //字型
                psRowFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;//粗體
                psRowFormat.SetFont(psRowFont); //設定儲存格的文字樣式
                #endregion

                //空白列
                sheet.CreateRow(sheet.LastRowNum + 1);

                //統計列
                sheet.CreateRow(sheet.LastRowNum + 1);
                sheet.GetRow(sheet.LastRowNum).CreateCell(0);
                sheet.GetRow(sheet.LastRowNum).GetCell(0).CellStyle = lastRowFormat;
                string lastRowData = $"總計件數：{dtExtra.Rows[0]["Char1"]} 收手續費件數：{dtExtra.Rows[0]["Int1"]} 免收手續費件數：{dtExtra.Rows[0]["Char2"]} 退件件數：{dtExtra.Rows[0]["Int2"]} 補發件數：{dtExtra.Rows[0]["CountMakeUp"]} 註銷件數：{dtExtra.Rows[0]["Char3"]}";
                sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(lastRowData);

                //主管經辦列
                sheet.CreateRow(sheet.LastRowNum + 1);
                for (int col = 0; col < 5; col++)
                {
                    sheet.GetRow(sheet.LastRowNum).CreateCell(col);
                    sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = lastRowFormat;
                }
                sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("主管");
                sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue("經辦");

                //ps列
                sheet.CreateRow(sheet.LastRowNum + 1);
                sheet.GetRow(sheet.LastRowNum).CreateCell(0);
                sheet.GetRow(sheet.LastRowNum).GetCell(0).CellStyle = psRowFormat;
                sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("PS.退件已於當日銷毀");
                #endregion

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report01020100" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }
        #endregion

        #region Report01020200 清償證明-大宗掛號報表
        /// <summary>
        /// 專案代號:20210058-CSIP作業服務平台現代化II
        /// 功能說明:清償證明-大宗掛號報表
        /// 作    者:Ares Stanley
        /// 創建時間:2021/12/20
        /// 修改紀錄:2022/03/14_Ares_Jack_詳細地址資料來源取 M_PROPERTY_CODE.PROPERTY_NAME
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="strMAILDAY"></param>
        /// <param name="strMAILTYPE"></param>
        /// <param name="strPathFile"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Report01020200(DataSet ds, string strMAILDAY, string strMAILTYPE, ref string strPathFile, ref string strMsgID)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "register.xls";

                DataTable dt = ds.Tables[0];
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                //取得樣式
                HSSFCellStyle contentFormat = getDefaultContentFormat(wb);

                #region 表頭
                string mailDateRegister = $"寄送日期：{strMAILDAY}";
                sheet.GetRow(4).GetCell(0).SetCellValue(mailDateRegister); //寄送日期
                string strAgentID = ((EntityAGENT_INFO)System.Web.HttpContext.Current.Session["Agent"]).agent_name.ToString();
                sheet.GetRow(4).GetCell(3).SetCellValue($"寄件人：{strAgentID}");
                sheet.GetRow(4).GetCell(7).SetCellValue($"郵寄方式：{strMAILTYPE}");
                string strSenderAddress = string.Empty;
                strSenderAddress = MessageHelper.GetMessage("04_00000000_060");
                sheet.GetRow(5).GetCell(7).SetCellValue($"詳細地址：{strSenderAddress}");//詳細地址
                #endregion

                HSSFCellStyle cs = getDefaultContentFormat(wb);

                #region 表身
                //資料去空白
                removeBlank(ref dt);

                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col = 0; col < 8; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col);
                        sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = cs;
                    }
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue((row + 1).ToString()); //項次
                    sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dt.Rows[row]["mailNo"].ToString()); //掛號號碼
                    sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(dt.Rows[row]["Consignee"].ToString()); //姓名
                    //詳細地址
                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 3, 6));
                    string fullAddress = $"{dt.Rows[row]["zip"]}{dt.Rows[row]["add1"]}{dt.Rows[row]["add2"]}{dt.Rows[row]["add3"]}";
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(fullAddress); //詳細地址
                    sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(""); //備註

                }
                #endregion

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report01020200" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }
        #endregion

        #region Report01020300 清償證明-列印清償證明
        /// 修改紀錄:20220224_Ares_Jack_總行資料取 M_PROPERTY_CODE.PROPERTY_NAME
        public static bool CreateExcelFile_Report01020300_2(DataSet ds, ref string strPathFile)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "PayCertificate.xls";

                DataTable dtPayCertificate = ds.Tables[0];
                DataTable dtCard = ds.Tables[1];

                //資料去空白
                removeBlank(ref dtPayCertificate);
                removeBlank(ref dtCard);

                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                string receiver = string.Empty;
                string zip = string.Empty;
                string add1 = string.Empty;
                string add2 = string.Empty;
                string add3 = string.Empty;
                string title = string.Empty;
                string context = string.Empty;
                string asFollow = string.Empty;
                string memo = string.Empty;
                string receiver2 = string.Empty;
                string keyinday = string.Empty;
                string nodetail = string.Empty;
                string cardData1 = string.Empty;
                string curSerialNo = string.Empty;
                string headOffice = string.Empty;//總行資料


                foreach (DataRow dr in dtPayCertificate.Rows)
                {
                    Int32 rowIndex = dtPayCertificate.Rows.IndexOf(dr);
                    Int32 pageIndex = 33 * rowIndex;
                    curSerialNo = dr["SerialNo"].ToString();

                    for (int x = 0; x < dtCard.Rows.Count; x++)
                    {
                        if (dtCard.Rows[x]["serialNo"].ToString() == curSerialNo)
                        {
                            cardData1 = dtCard.Rows[x]["cardData1"].ToString();
                            //去<br>標籤
                            cardData1 = cardData1.Replace("<br>", "\n");
                            break;
                        }
                    }

                    receiver = dr["Receiver"].ToString();
                    zip = dr["Zip"].ToString();
                    add1 = dr["Add1"].ToString();
                    add2 = dr["Add2"].ToString();
                    add3 = dr["Add3"].ToString();
                    title = dr["Title"].ToString();
                    context = dr["Context"].ToString();
                    asFollow = dr["AsFollow"].ToString();
                    memo = dr["memo"].ToString();
                    receiver2 = dr["Receiver2"].ToString();
                    keyinday = dr["KeyInDay"].ToString();
                    nodetail = dr["NoDetail"].ToString();
                    headOffice = dr["HeadOffice"].ToString();//總行資料

                    sheet.GetRow(pageIndex + 1).GetCell(4).SetCellValue(receiver);
                    sheet.GetRow(pageIndex + 3).GetCell(4).SetCellValue(zip);
                    sheet.GetRow(pageIndex + 3).GetCell(5).SetCellValue(add1);
                    sheet.GetRow(pageIndex + 4).GetCell(4).SetCellValue(add2);
                    sheet.GetRow(pageIndex + 5).GetCell(4).SetCellValue(add3);
                    sheet.GetRow(pageIndex + 7).GetCell(9).SetCellValue(title);
                    sheet.GetRow(pageIndex + 9).GetCell(1).SetCellValue(context);
                    sheet.GetRow(pageIndex + 13).GetCell(1).SetCellValue(asFollow);
                    sheet.GetRow(pageIndex + 14).GetCell(2).SetCellValue(cardData1);
                    sheet.GetRow(pageIndex + 16).GetCell(1).SetCellValue(memo);
                    sheet.GetRow(pageIndex + 19).GetCell(4).SetCellValue(receiver2);
                    sheet.GetRow(pageIndex + 29).GetCell(16).SetCellValue(keyinday);
                    sheet.GetRow(pageIndex + 31).GetCell(16).SetCellValue(nodetail);
                    sheet.GetRow(pageIndex + 23).GetCell(1).SetCellValue(headOffice);//總行資料

                    if (rowIndex + 1 == dtPayCertificate.Rows.Count)
                    {
                        break;
                    }
                    #region CopyNewPage
                    for (int r = 0; r < 33; r++)
                    {
                        sheet.CreateRow(sheet.LastRowNum + 1);
                        sheet.GetRow(sheet.LastRowNum).Height = sheet.GetRow(r).Height;
                        switch (r)
                        {
                            case 1:
                                for (int c = 4; c < 7; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 4, 6));

                                for (int c = 7; c < 11; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 7, 10));
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(sheet.GetRow(r).GetCell(7).StringCellValue.ToString());
                                break;
                            case 3:
                                for (int c = 4; c < 6; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                break;
                            case 4:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(4);
                                sheet.GetRow(sheet.LastRowNum).GetCell(4).CellStyle = sheet.GetRow(r).GetCell(4).CellStyle;
                                break;
                            case 5:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(4);
                                sheet.GetRow(sheet.LastRowNum).GetCell(4).CellStyle = sheet.GetRow(r).GetCell(4).CellStyle;
                                break;
                            case 7:
                                for (int c = 9; c < 15; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 9, 14));
                                break;
                            case 9:
                            case 10:
                            case 11:
                                for (int c = 1; c < 17; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                if (r == 11)
                                {
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 2, sheet.LastRowNum, 1, 16));
                                }
                                break;
                            case 13:
                                for (int c = 1; c < 17; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 1, 16));
                                break;
                            case 14:
                            case 15:
                                for (int c = 2; c < 17; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                if (r == 15)
                                {
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 2, 16));
                                }
                                break;
                            case 16:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(1);
                                sheet.GetRow(sheet.LastRowNum).GetCell(1).CellStyle = sheet.GetRow(r).GetCell(1).CellStyle;
                                break;
                            case 17:
                                for (int c = 1; c < 3; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 1, 2));
                                sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(sheet.GetRow(r).GetCell(1).StringCellValue.ToString());
                                break;
                            case 19:
                                for (int c = 4; c < 7; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 4, 6));
                                break;
                            case 21:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(7);
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).CellStyle = sheet.GetRow(r).GetCell(7).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(sheet.GetRow(r).GetCell(7).StringCellValue.ToString());
                                break;
                            case 23:
                            case 24:
                            case 25:
                            case 26:
                            case 27:
                                for (int c = 1; c < 17; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                if (r == 27)
                                {
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 4, sheet.LastRowNum, 1, 11));
                                    sheet.GetRow(sheet.LastRowNum - 4).GetCell(1).SetCellValue(sheet.GetRow(r - 4).GetCell(1).StringCellValue.ToString());
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 13, 16));
                                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(13).SetCellValue(sheet.GetRow(r - 1).GetCell(13).StringCellValue.ToString());
                                }
                                break;
                            case 29:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(14);
                                sheet.GetRow(sheet.LastRowNum).GetCell(14).CellStyle = sheet.GetRow(r).GetCell(14).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(14).SetCellValue(sheet.GetRow(r).GetCell(14).StringCellValue.ToString());
                                sheet.GetRow(sheet.LastRowNum).CreateCell(16);
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).CellStyle = sheet.GetRow(r).GetCell(16).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).SetCellValue(sheet.GetRow(r).GetCell(16).StringCellValue.ToString());
                                break;
                            case 31:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(16);
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).CellStyle = sheet.GetRow(r).GetCell(16).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).SetCellValue(sheet.GetRow(r).GetCell(16).StringCellValue.ToString());
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion
                }

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report01020300_2" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }


        /// <summary>
        /// 列印個別清償證明
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="strPathFile"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Report01020300(DataSet ds, ref string strPathFile)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "PayCertificate.xls";

                DataTable dtPayCertificate = ds.Tables[0];
                DataTable dtCard = ds.Tables[1];

                //資料去空白
                removeBlank(ref dtPayCertificate);
                removeBlank(ref dtCard);

                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                string receiver = string.Empty;
                string zip = string.Empty;
                string add1 = string.Empty;
                string add2 = string.Empty;
                string add3 = string.Empty;
                string title = string.Empty;
                string context = string.Empty;
                string asFollow = string.Empty;
                string memo = string.Empty;
                string receiver2 = string.Empty;
                string keyinday = string.Empty;
                string nodetail = string.Empty;
                string cardData1 = string.Empty;
                string curSerialNo = string.Empty;
                string headOffice = string.Empty;//總行資料

                foreach (DataRow dr in dtPayCertificate.Rows)
                {
                    Int32 rowIndex = dtPayCertificate.Rows.IndexOf(dr);
                    Int32 pageIndex = 33 * rowIndex;
                    curSerialNo = dr["SerialNo"].ToString();

                    for (int x = 0; x < dtCard.Rows.Count; x++)
                    {
                        if (dtCard.Rows[x]["serialNo"].ToString() == curSerialNo)
                        {
                            cardData1 = dtCard.Rows[x]["cardData1"].ToString();
                            //去<br>標籤
                            cardData1 = cardData1.Replace("<br>", "\n");
                            break;
                        }
                    }

                    receiver = dr["Receiver"].ToString();
                    zip = dr["Zip"].ToString();
                    add1 = dr["Add1"].ToString();
                    add2 = dr["Add2"].ToString();
                    add3 = dr["Add3"].ToString();
                    title = dr["Title"].ToString();
                    context = dr["Context"].ToString();
                    asFollow = dr["AsFollow"].ToString();
                    memo = dr["memo"].ToString();
                    receiver2 = dr["Receiver2"].ToString();
                    keyinday = dr["KeyInDay"].ToString();
                    nodetail = dr["NoDetail"].ToString();
                    headOffice = dr["HeadOffice"].ToString();//總行資料

                    sheet.GetRow(pageIndex + 1).GetCell(4).SetCellValue(receiver);
                    sheet.GetRow(pageIndex + 3).GetCell(4).SetCellValue(zip);
                    sheet.GetRow(pageIndex + 3).GetCell(5).SetCellValue(add1);
                    sheet.GetRow(pageIndex + 4).GetCell(4).SetCellValue(add2);
                    sheet.GetRow(pageIndex + 5).GetCell(4).SetCellValue(add3);
                    sheet.GetRow(pageIndex + 7).GetCell(9).SetCellValue(title);
                    sheet.GetRow(pageIndex + 9).GetCell(1).SetCellValue(context);
                    sheet.GetRow(pageIndex + 13).GetCell(1).SetCellValue(asFollow);
                    sheet.GetRow(pageIndex + 14).GetCell(2).SetCellValue(cardData1);
                    sheet.GetRow(pageIndex + 16).GetCell(1).SetCellValue(memo);
                    sheet.GetRow(pageIndex + 19).GetCell(4).SetCellValue(receiver2);
                    sheet.GetRow(pageIndex + 29).GetCell(16).SetCellValue(keyinday);
                    sheet.GetRow(pageIndex + 31).GetCell(16).SetCellValue(nodetail);
                    sheet.GetRow(pageIndex + 23).GetCell(1).SetCellValue(headOffice);//總行資料
                    if (rowIndex + 1 == dtPayCertificate.Rows.Count)
                    {
                        break;
                    }
                    #region CopyNewPage
                    for (int r = 0; r < 33; r++)
                    {
                        sheet.CreateRow(sheet.LastRowNum + 1);
                        sheet.GetRow(sheet.LastRowNum).Height = sheet.GetRow(r).Height;
                        switch (r)
                        {
                            case 1:
                                for (int c = 4; c < 7; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 4, 6));

                                for (int c = 7; c < 11; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 7, 10));
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(sheet.GetRow(r).GetCell(7).StringCellValue.ToString());
                                break;
                            case 3:
                                for (int c = 4; c < 6; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                break;
                            case 4:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(4);
                                sheet.GetRow(sheet.LastRowNum).GetCell(4).CellStyle = sheet.GetRow(r).GetCell(4).CellStyle;
                                break;
                            case 5:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(4);
                                sheet.GetRow(sheet.LastRowNum).GetCell(4).CellStyle = sheet.GetRow(r).GetCell(4).CellStyle;
                                break;
                            case 7:
                                for (int c = 9; c < 15; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 9, 14));
                                break;
                            case 9:
                            case 10:
                            case 11:
                                for (int c = 1; c < 17; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                if (r == 11)
                                {
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 2, sheet.LastRowNum, 1, 16));
                                }
                                break;
                            case 13:
                                for (int c = 1; c < 17; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 1, 16));
                                break;
                            case 14:
                            case 15:
                                for (int c = 2; c < 16; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                if (r == 15)
                                {
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 2, 15));
                                }
                                break;
                            case 16:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(1);
                                sheet.GetRow(sheet.LastRowNum).GetCell(1).CellStyle = sheet.GetRow(r).GetCell(1).CellStyle;
                                break;
                            case 17:
                                for (int c = 1; c < 3; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 1, 2));
                                sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(sheet.GetRow(r).GetCell(1).StringCellValue.ToString());
                                break;
                            case 19:
                                for (int c = 4; c < 7; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 4, 6));
                                break;
                            case 21:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(7);
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).CellStyle = sheet.GetRow(r).GetCell(7).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(sheet.GetRow(r).GetCell(7).StringCellValue.ToString());
                                break;
                            case 23:
                            case 24:
                            case 25:
                            case 26:
                            case 27:
                                for (int c = 1; c < 17; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                if (r == 27)
                                {
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 4, sheet.LastRowNum, 1, 11));
                                    sheet.GetRow(sheet.LastRowNum - 4).GetCell(1).SetCellValue(sheet.GetRow(r - 4).GetCell(1).StringCellValue.ToString());
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 13, 16));
                                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(13).SetCellValue(sheet.GetRow(r - 1).GetCell(13).StringCellValue.ToString());
                                }
                                break;
                            case 29:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(14);
                                sheet.GetRow(sheet.LastRowNum).GetCell(14).CellStyle = sheet.GetRow(r).GetCell(14).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(14).SetCellValue(sheet.GetRow(r).GetCell(14).StringCellValue.ToString());
                                sheet.GetRow(sheet.LastRowNum).CreateCell(16);
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).CellStyle = sheet.GetRow(r).GetCell(16).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).SetCellValue(sheet.GetRow(r).GetCell(16).StringCellValue.ToString());
                                break;
                            case 31:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(16);
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).CellStyle = sheet.GetRow(r).GetCell(16).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).SetCellValue(sheet.GetRow(r).GetCell(16).StringCellValue.ToString());
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion
                }

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report01020300" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }

        #endregion

        #region Report02010100 債清客戶-開立債清證明
        /// <summary>
        /// 開立債清證明
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="strPathFile"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Report02010100(DataSet ds, ref string strPathFile)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "PayCertificate.xls";

                DataTable dtPayCertificate = ds.Tables[0];
                DataTable dtCard = ds.Tables[1];

                //資料去空白
                removeBlank(ref dtPayCertificate);
                removeBlank(ref dtCard);

                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                string receiver = string.Empty;
                string zip = string.Empty;
                string add1 = string.Empty;
                string add2 = string.Empty;
                string add3 = string.Empty;
                string title = string.Empty;
                string context = string.Empty;
                string asFollow = string.Empty;
                string memo = string.Empty;
                string receiver2 = string.Empty;
                string keyinday = string.Empty;
                string nodetail = string.Empty;
                string cardData1 = string.Empty;
                string curSerialNo = string.Empty;
                string headOffice = string.Empty;//總行資料

                foreach (DataRow dr in dtPayCertificate.Rows)
                {
                    Int32 rowIndex = dtPayCertificate.Rows.IndexOf(dr);
                    Int32 pageIndex = 33 * rowIndex;
                    curSerialNo = dr["SerialNo"].ToString();

                    for (int x = 0; x < dtCard.Rows.Count; x++)
                    {
                        if (dtCard.Rows[x]["serialNo"].ToString() == curSerialNo)
                        {
                            cardData1 = dtCard.Rows[x]["cardData1"].ToString();
                            //去<br>標籤
                            cardData1 = cardData1.Replace("<br>", "\n");
                            break;
                        }
                    }

                    receiver = dr["Receiver"].ToString();
                    zip = dr["Zip"].ToString();
                    add1 = dr["Add1"].ToString();
                    add2 = dr["Add2"].ToString();
                    add3 = dr["Add3"].ToString();
                    title = dr["Title"].ToString();
                    context = dr["Context"].ToString();
                    asFollow = dr["AsFollow"].ToString();
                    memo = dr["memo"].ToString();
                    receiver2 = dr["Receiver2"].ToString();
                    keyinday = dr["KeyInDay"].ToString();
                    nodetail = dr["NoDetail"].ToString();
                    headOffice = dr["HeadOffice"].ToString();//總行資料

                    sheet.GetRow(pageIndex + 1).GetCell(4).SetCellValue(receiver);
                    sheet.GetRow(pageIndex + 3).GetCell(4).SetCellValue(zip);
                    sheet.GetRow(pageIndex + 3).GetCell(5).SetCellValue(add1);
                    sheet.GetRow(pageIndex + 4).GetCell(4).SetCellValue(add2);
                    sheet.GetRow(pageIndex + 5).GetCell(4).SetCellValue(add3);
                    sheet.GetRow(pageIndex + 7).GetCell(9).SetCellValue(title);
                    sheet.GetRow(pageIndex + 9).GetCell(1).SetCellValue(context);
                    sheet.GetRow(pageIndex + 13).GetCell(1).SetCellValue(asFollow);
                    sheet.GetRow(pageIndex + 14).GetCell(2).SetCellValue(cardData1);
                    sheet.GetRow(pageIndex + 16).GetCell(1).SetCellValue(memo);
                    sheet.GetRow(pageIndex + 19).GetCell(4).SetCellValue(receiver2);
                    sheet.GetRow(pageIndex + 29).GetCell(16).SetCellValue(keyinday);
                    sheet.GetRow(pageIndex + 31).GetCell(16).SetCellValue(nodetail);
                    sheet.GetRow(pageIndex + 23).GetCell(1).SetCellValue(headOffice);//總行資料
                    if (rowIndex + 1 == dtPayCertificate.Rows.Count)
                    {
                        break;
                    }
                    #region CopyNewPage
                    for (int r = 0; r < 33; r++)
                    {
                        sheet.CreateRow(sheet.LastRowNum + 1);
                        sheet.GetRow(sheet.LastRowNum).Height = sheet.GetRow(r).Height;
                        switch (r)
                        {
                            case 1:
                                for (int c = 4; c < 7; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 4, 6));

                                for (int c = 7; c < 11; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 7, 10));
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(sheet.GetRow(r).GetCell(7).StringCellValue.ToString());
                                break;
                            case 3:
                                for (int c = 4; c < 6; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                break;
                            case 4:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(4);
                                sheet.GetRow(sheet.LastRowNum).GetCell(4).CellStyle = sheet.GetRow(r).GetCell(4).CellStyle;
                                break;
                            case 5:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(4);
                                sheet.GetRow(sheet.LastRowNum).GetCell(4).CellStyle = sheet.GetRow(r).GetCell(4).CellStyle;
                                break;
                            case 7:
                                for (int c = 9; c < 15; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 9, 14));
                                break;
                            case 9:
                            case 10:
                            case 11:
                                for (int c = 1; c < 17; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                if (r == 11)
                                {
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 2, sheet.LastRowNum, 1, 16));
                                }
                                break;
                            case 13:
                                for (int c = 1; c < 17; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 1, 16));
                                break;
                            case 14:
                            case 15:
                                for (int c = 2; c < 16; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                if (r == 15)
                                {
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 2, 15));
                                }
                                break;
                            case 16:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(1);
                                sheet.GetRow(sheet.LastRowNum).GetCell(1).CellStyle = sheet.GetRow(r).GetCell(1).CellStyle;
                                break;
                            case 17:
                                for (int c = 1; c < 3; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 1, 2));
                                sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(sheet.GetRow(r).GetCell(1).StringCellValue.ToString());
                                break;
                            case 19:
                                for (int c = 4; c < 7; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 4, 6));
                                break;
                            case 21:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(7);
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).CellStyle = sheet.GetRow(r).GetCell(7).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(sheet.GetRow(r).GetCell(7).StringCellValue.ToString());
                                break;
                            case 23:
                            case 24:
                            case 25:
                            case 26:
                            case 27:
                                for (int c = 1; c < 17; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                if (r == 27)
                                {
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 4, sheet.LastRowNum, 1, 11));
                                    sheet.GetRow(sheet.LastRowNum - 4).GetCell(1).SetCellValue(sheet.GetRow(r - 4).GetCell(1).StringCellValue.ToString());
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 13, 16));
                                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(13).SetCellValue(sheet.GetRow(r - 1).GetCell(13).StringCellValue.ToString());
                                }
                                break;
                            case 29:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(14);
                                sheet.GetRow(sheet.LastRowNum).GetCell(14).CellStyle = sheet.GetRow(r).GetCell(14).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(14).SetCellValue(sheet.GetRow(r).GetCell(14).StringCellValue.ToString());
                                sheet.GetRow(sheet.LastRowNum).CreateCell(16);
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).CellStyle = sheet.GetRow(r).GetCell(16).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).SetCellValue(sheet.GetRow(r).GetCell(16).StringCellValue.ToString());
                                break;
                            case 31:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(16);
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).CellStyle = sheet.GetRow(r).GetCell(16).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).SetCellValue(sheet.GetRow(r).GetCell(16).StringCellValue.ToString());
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion
                }

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report02010100" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }

        #endregion

        #region Report02020100 債清客戶-債清證明統計表
        /// <summary>
        /// 專案代號:20210058-CSIP作業服務平台現代化II
        /// 功能說明:債清證明-債清證明統計表
        /// 作    者:Ares Stanley
        /// 創建時間:2022/01/20
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="strRptBeforeDate"></param>
        /// <param name="strRptEndDate"></param>
        /// <param name="strPathFile"></param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Report02020100(DataSet ds, string strRptBeforeDate, string strRptEndDate, ref string strPathFile, ref string strMsgID)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "dayth_sv.xls";

                DataTable dtExtra = ds.Tables[0];
                DataTable dtPay_SV = ds.Tables[1];
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                //取得樣式
                HSSFCellStyle contentFormat = getDefaultContentFormat(wb);

                #region 表頭
                strRptBeforeDate = DateHelper.ConvertToMingGuo(strRptBeforeDate);
                strRptEndDate = DateHelper.ConvertToMingGuo(strRptEndDate);
                sheet.GetRow(3).GetCell(12).SetCellValue($"開立日期{strRptBeforeDate.Substring(0, 3) + "/" + strRptBeforeDate.Substring(3, 2) + "/" + strRptBeforeDate.Substring(5, 2) + " ~ " + strRptEndDate.Substring(0, 3) + "/" + strRptEndDate.Substring(3, 2) + "/" + strRptEndDate.Substring(5, 2)}");//開立日期
                #endregion

                HSSFCellStyle cs = getDefaultContentFormat(wb);
                HSSFFont contentFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                contentFont.FontHeightInPoints = 8; //字體大小
                contentFont.FontName = "新細明體"; //字型
                cs.SetFont(contentFont); //設定儲存格的文字樣式
                #region 表身

                //資料去空白
                removeBlank(ref dtPay_SV);
                removeBlank(ref dtExtra);

                for (int row = 0; row < dtPay_SV.Rows.Count; row++)
                {
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col = 0; col < 18; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col);
                        sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = cs;
                    }
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue((row + 1).ToString()); //項次
                    sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dtPay_SV.Rows[row]["userID"].ToString()); //身分證字號
                    sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(dtPay_SV.Rows[row]["UserName"].ToString()); //客戶姓名
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(dtPay_SV.Rows[row]["SerialNo"].ToString()); //清證序號
                    sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dtPay_SV.Rows[row]["Address"].ToString());//寄送地址
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(dtPay_SV.Rows[row]["Consignee"].ToString());//收件者
                    sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue(dtPay_SV.Rows[row]["ClearDate"].ToString());//結清日
                    sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(dtPay_SV.Rows[row]["EndDate"].ToString());//結案日
                    sheet.GetRow(sheet.LastRowNum).GetCell(8).SetCellValue(dtPay_SV.Rows[row]["Type"].ToString());//證明種類
                    sheet.GetRow(sheet.LastRowNum).GetCell(9).SetCellValue(dtPay_SV.Rows[row]["CardNo"].ToString());//卡號
                    sheet.GetRow(sheet.LastRowNum).GetCell(10).SetCellValue(dtPay_SV.Rows[row]["BLKCode"].ToString());//BLK
                    sheet.GetRow(sheet.LastRowNum).GetCell(11).SetCellValue(dtPay_SV.Rows[row]["TypeCard"].ToString());//卡別
                    sheet.GetRow(sheet.LastRowNum).GetCell(12).SetCellValue(dtPay_SV.Rows[row]["CUSID"].ToString());//附卡人ID
                    sheet.GetRow(sheet.LastRowNum).GetCell(13).SetCellValue(dtPay_SV.Rows[row]["CUSNAME"].ToString());//附卡人姓名
                    sheet.GetRow(sheet.LastRowNum).GetCell(14).SetCellValue(dtPay_SV.Rows[row]["Status"].ToString());//協商狀態
                    sheet.GetRow(sheet.LastRowNum).GetCell(15).SetCellValue(dtPay_SV.Rows[row]["IsPaidOffAlone"].ToString());//單獨受償
                    sheet.GetRow(sheet.LastRowNum).GetCell(16).SetCellValue(dtPay_SV.Rows[row]["ApplyUser"].ToString());//申請經辦
                    sheet.GetRow(sheet.LastRowNum).GetCell(17).SetCellValue(dtPay_SV.Rows[row]["AgreeUser"].ToString());//放行主管

                }
                #region 表尾資料格式
                //統計、主管經辦格式
                HSSFCellStyle lastRowFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
                lastRowFormat.VerticalAlignment = VerticalAlignment.Top; //垂直置中
                lastRowFormat.Alignment = HorizontalAlignment.Left; //水平靠左
                lastRowFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字
                lastRowFormat.BorderBottom = BorderStyle.None; // 儲存格框線
                lastRowFormat.BorderLeft = BorderStyle.None;
                lastRowFormat.BorderTop = BorderStyle.None;
                lastRowFormat.BorderRight = BorderStyle.None;

                HSSFFont lastRowFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                lastRowFont.FontHeightInPoints = 10; //字體大小
                lastRowFont.FontName = "新細明體"; //字型
                lastRowFormat.SetFont(lastRowFont); //設定儲存格的文字樣式

                //ps列格式
                HSSFCellStyle psRowFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
                psRowFormat.VerticalAlignment = VerticalAlignment.Top; //垂直置中
                psRowFormat.Alignment = HorizontalAlignment.Left; //水平靠左
                psRowFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字
                psRowFormat.BorderBottom = BorderStyle.None; // 儲存格框線
                psRowFormat.BorderLeft = BorderStyle.None;
                psRowFormat.BorderTop = BorderStyle.None;
                psRowFormat.BorderRight = BorderStyle.None;

                HSSFFont psRowFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                psRowFont.FontHeightInPoints = 10; //字體大小
                psRowFont.FontName = "新細明體"; //字型
                psRowFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;//粗體
                psRowFormat.SetFont(psRowFont); //設定儲存格的文字樣式
                #endregion

                //空白列
                sheet.CreateRow(sheet.LastRowNum + 1);

                //統計列
                sheet.CreateRow(sheet.LastRowNum + 1);
                sheet.GetRow(sheet.LastRowNum).CreateCell(0);
                sheet.GetRow(sheet.LastRowNum).GetCell(0).CellStyle = lastRowFormat;
                string lastRowData = $"總計件數：{dtExtra.Rows[0]["Char1"]} 免收手續費件數：{dtExtra.Rows[0]["Char2"]} 退件件數：{dtExtra.Rows[0]["Int2"]} 補發件數：{dtExtra.Rows[0]["CountMakeUp"]} 註銷件數：{dtExtra.Rows[0]["Int1"]}";
                sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(lastRowData);

                //主管經辦列
                sheet.CreateRow(sheet.LastRowNum + 1);
                for (int col = 0; col < 5; col++)
                {
                    sheet.GetRow(sheet.LastRowNum).CreateCell(col);
                    sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = lastRowFormat;
                }
                sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("主管");
                sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue("經辦");

                //ps列
                sheet.CreateRow(sheet.LastRowNum + 1);
                sheet.GetRow(sheet.LastRowNum).CreateCell(0);
                sheet.GetRow(sheet.LastRowNum).GetCell(0).CellStyle = psRowFormat;
                sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue("PS.退件已於當日銷毀");
                #endregion

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report02020100" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }
        #endregion

        #region Report02020300 債清客戶-列印債清證明
        /// 修改紀錄:20220224_Ares_Jack_總行資料取 M_PROPERTY_CODE.PROPERTY_NAME
        /// <summary>
        /// 列印全部債清證明
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="strPathFile"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Report02020300_2(DataSet ds, ref string strPathFile)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "PayCertificate.xls";

                DataTable dtPayCertificate = ds.Tables[0];
                DataTable dtCard = ds.Tables[1];

                //資料去空白
                removeBlank(ref dtPayCertificate);
                removeBlank(ref dtCard);

                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                string receiver = string.Empty;
                string zip = string.Empty;
                string add1 = string.Empty;
                string add2 = string.Empty;
                string add3 = string.Empty;
                string title = string.Empty;
                string context = string.Empty;
                string asFollow = string.Empty;
                string memo = string.Empty;
                string receiver2 = string.Empty;
                string keyinday = string.Empty;
                string nodetail = string.Empty;
                string cardData1 = string.Empty;
                string curSerialNo = string.Empty;
                string headOffice = string.Empty;//總行資料

                foreach (DataRow dr in dtPayCertificate.Rows)
                {
                    Int32 rowIndex = dtPayCertificate.Rows.IndexOf(dr);
                    Int32 pageIndex = 33 * rowIndex;
                    curSerialNo = dr["SerialNo"].ToString();

                    for (int x = 0; x < dtCard.Rows.Count; x++)
                    {
                        if (dtCard.Rows[x]["serialNo"].ToString() == curSerialNo)
                        {
                            cardData1 = dtCard.Rows[x]["cardData1"].ToString();
                            //去<br>標籤
                            cardData1 = cardData1.Replace("<br>", "\n");
                            break;
                        }
                    }

                    receiver = dr["Receiver"].ToString();
                    zip = dr["Zip"].ToString();
                    add1 = dr["Add1"].ToString();
                    add2 = dr["Add2"].ToString();
                    add3 = dr["Add3"].ToString();
                    title = dr["Title"].ToString();
                    context = dr["Context"].ToString();
                    asFollow = dr["AsFollow"].ToString();
                    memo = dr["memo"].ToString();
                    receiver2 = dr["Receiver2"].ToString();
                    keyinday = dr["KeyInDay"].ToString();
                    nodetail = dr["NoDetail"].ToString();
                    headOffice = dr["HeadOffice"].ToString();//總行資料

                    sheet.GetRow(pageIndex + 1).GetCell(4).SetCellValue(receiver);
                    sheet.GetRow(pageIndex + 3).GetCell(4).SetCellValue(zip);
                    sheet.GetRow(pageIndex + 3).GetCell(5).SetCellValue(add1);
                    sheet.GetRow(pageIndex + 4).GetCell(4).SetCellValue(add2);
                    sheet.GetRow(pageIndex + 5).GetCell(4).SetCellValue(add3);
                    sheet.GetRow(pageIndex + 7).GetCell(9).SetCellValue(title);
                    sheet.GetRow(pageIndex + 9).GetCell(1).SetCellValue(context);
                    sheet.GetRow(pageIndex + 13).GetCell(1).SetCellValue(asFollow);
                    sheet.GetRow(pageIndex + 14).GetCell(2).SetCellValue(cardData1);
                    sheet.GetRow(pageIndex + 16).GetCell(1).SetCellValue(memo);
                    sheet.GetRow(pageIndex + 19).GetCell(4).SetCellValue(receiver2);
                    sheet.GetRow(pageIndex + 29).GetCell(16).SetCellValue(keyinday);
                    sheet.GetRow(pageIndex + 31).GetCell(16).SetCellValue(nodetail);
                    sheet.GetRow(pageIndex + 23).GetCell(1).SetCellValue(headOffice);//總行資料

                    if (rowIndex + 1 == dtPayCertificate.Rows.Count)
                    {
                        break;
                    }
                    #region CopyNewPage
                    for (int r = 0; r < 33; r++)
                    {
                        sheet.CreateRow(sheet.LastRowNum + 1);
                        sheet.GetRow(sheet.LastRowNum).Height = sheet.GetRow(r).Height;
                        switch (r)
                        {
                            case 1:
                                for (int c = 4; c < 7; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 4, 6));

                                for (int c = 7; c < 11; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 7, 10));
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(sheet.GetRow(r).GetCell(7).StringCellValue.ToString());
                                break;
                            case 3:
                                for (int c = 4; c < 6; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                break;
                            case 4:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(4);
                                sheet.GetRow(sheet.LastRowNum).GetCell(4).CellStyle = sheet.GetRow(r).GetCell(4).CellStyle;
                                break;
                            case 5:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(4);
                                sheet.GetRow(sheet.LastRowNum).GetCell(4).CellStyle = sheet.GetRow(r).GetCell(4).CellStyle;
                                break;
                            case 7:
                                for (int c = 9; c < 15; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 9, 14));
                                break;
                            case 9:
                            case 10:
                            case 11:
                                for (int c = 1; c < 17; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                if (r == 11)
                                {
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 2, sheet.LastRowNum, 1, 16));
                                }
                                break;
                            case 13:
                                for (int c = 1; c < 17; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 1, 16));
                                break;
                            case 14:
                            case 15:
                                for (int c = 2; c < 17; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                if (r == 15)
                                {
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 2, 16));
                                }
                                break;
                            case 16:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(1);
                                sheet.GetRow(sheet.LastRowNum).GetCell(1).CellStyle = sheet.GetRow(r).GetCell(1).CellStyle;
                                break;
                            case 17:
                                for (int c = 1; c < 3; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 1, 2));
                                sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(sheet.GetRow(r).GetCell(1).StringCellValue.ToString());
                                break;
                            case 19:
                                for (int c = 4; c < 7; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 4, 6));
                                break;
                            case 21:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(7);
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).CellStyle = sheet.GetRow(r).GetCell(7).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(sheet.GetRow(r).GetCell(7).StringCellValue.ToString());
                                break;
                            case 23:
                            case 24:
                            case 25:
                            case 26:
                            case 27:
                                for (int c = 1; c < 17; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                if (r == 27)
                                {
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 4, sheet.LastRowNum, 1, 11));
                                    sheet.GetRow(sheet.LastRowNum - 4).GetCell(1).SetCellValue(sheet.GetRow(r - 4).GetCell(1).StringCellValue.ToString());
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 13, 16));
                                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(13).SetCellValue(sheet.GetRow(r - 1).GetCell(13).StringCellValue.ToString());
                                }
                                break;
                            case 29:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(14);
                                sheet.GetRow(sheet.LastRowNum).GetCell(14).CellStyle = sheet.GetRow(r).GetCell(14).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(14).SetCellValue(sheet.GetRow(r).GetCell(14).StringCellValue.ToString());
                                sheet.GetRow(sheet.LastRowNum).CreateCell(16);
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).CellStyle = sheet.GetRow(r).GetCell(16).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).SetCellValue(sheet.GetRow(r).GetCell(16).StringCellValue.ToString());
                                break;
                            case 31:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(16);
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).CellStyle = sheet.GetRow(r).GetCell(16).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).SetCellValue(sheet.GetRow(r).GetCell(16).StringCellValue.ToString());
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion
                }

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report02020300_2" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }


        /// <summary>
        /// 列印個別債清證明
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="strPathFile"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Report02020300(DataSet ds, ref string strPathFile)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "PayCertificate.xls";

                DataTable dtPayCertificate = ds.Tables[0];
                DataTable dtCard = ds.Tables[1];

                //資料去空白
                removeBlank(ref dtPayCertificate);
                removeBlank(ref dtCard);

                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                string receiver = string.Empty;
                string zip = string.Empty;
                string add1 = string.Empty;
                string add2 = string.Empty;
                string add3 = string.Empty;
                string title = string.Empty;
                string context = string.Empty;
                string asFollow = string.Empty;
                string memo = string.Empty;
                string receiver2 = string.Empty;
                string keyinday = string.Empty;
                string nodetail = string.Empty;
                string cardData1 = string.Empty;
                string curSerialNo = string.Empty;
                string headOffice = string.Empty;//總行資料

                foreach (DataRow dr in dtPayCertificate.Rows)
                {
                    Int32 rowIndex = dtPayCertificate.Rows.IndexOf(dr);
                    Int32 pageIndex = 33 * rowIndex;
                    curSerialNo = dr["SerialNo"].ToString();

                    for (int x = 0; x < dtCard.Rows.Count; x++)
                    {
                        if (dtCard.Rows[x]["serialNo"].ToString() == curSerialNo)
                        {
                            cardData1 = dtCard.Rows[x]["cardData1"].ToString();
                            //去<br>標籤
                            cardData1 = cardData1.Replace("<br>", "\n");
                            break;
                        }
                    }

                    receiver = dr["Receiver"].ToString();
                    zip = dr["Zip"].ToString();
                    add1 = dr["Add1"].ToString();
                    add2 = dr["Add2"].ToString();
                    add3 = dr["Add3"].ToString();
                    title = dr["Title"].ToString();
                    context = dr["Context"].ToString();
                    asFollow = dr["AsFollow"].ToString();
                    memo = dr["memo"].ToString();
                    receiver2 = dr["Receiver2"].ToString();
                    keyinday = dr["KeyInDay"].ToString();
                    nodetail = dr["NoDetail"].ToString();
                    headOffice = dr["HeadOffice"].ToString();//總行資料

                    sheet.GetRow(pageIndex + 1).GetCell(4).SetCellValue(receiver);
                    sheet.GetRow(pageIndex + 3).GetCell(4).SetCellValue(zip);
                    sheet.GetRow(pageIndex + 3).GetCell(5).SetCellValue(add1);
                    sheet.GetRow(pageIndex + 4).GetCell(4).SetCellValue(add2);
                    sheet.GetRow(pageIndex + 5).GetCell(4).SetCellValue(add3);
                    sheet.GetRow(pageIndex + 7).GetCell(9).SetCellValue(title);
                    sheet.GetRow(pageIndex + 9).GetCell(1).SetCellValue(context);
                    sheet.GetRow(pageIndex + 13).GetCell(1).SetCellValue(asFollow);
                    sheet.GetRow(pageIndex + 14).GetCell(2).SetCellValue(cardData1);
                    sheet.GetRow(pageIndex + 16).GetCell(1).SetCellValue(memo);
                    sheet.GetRow(pageIndex + 19).GetCell(4).SetCellValue(receiver2);
                    sheet.GetRow(pageIndex + 29).GetCell(16).SetCellValue(keyinday);
                    sheet.GetRow(pageIndex + 31).GetCell(16).SetCellValue(nodetail);
                    sheet.GetRow(pageIndex + 23).GetCell(1).SetCellValue(headOffice);//總行資料
                    if (rowIndex + 1 == dtPayCertificate.Rows.Count)
                    {
                        break;
                    }
                    #region CopyNewPage
                    for (int r = 0; r < 33; r++)
                    {
                        sheet.CreateRow(sheet.LastRowNum + 1);
                        sheet.GetRow(sheet.LastRowNum).Height = sheet.GetRow(r).Height;
                        switch (r)
                        {
                            case 1:
                                for (int c = 4; c < 7; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 4, 6));

                                for (int c = 7; c < 11; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 7, 10));
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(sheet.GetRow(r).GetCell(7).StringCellValue.ToString());
                                break;
                            case 3:
                                for (int c = 4; c < 6; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                break;
                            case 4:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(4);
                                sheet.GetRow(sheet.LastRowNum).GetCell(4).CellStyle = sheet.GetRow(r).GetCell(4).CellStyle;
                                break;
                            case 5:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(4);
                                sheet.GetRow(sheet.LastRowNum).GetCell(4).CellStyle = sheet.GetRow(r).GetCell(4).CellStyle;
                                break;
                            case 7:
                                for (int c = 9; c < 15; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 9, 14));
                                break;
                            case 9:
                            case 10:
                            case 11:
                                for (int c = 1; c < 17; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                if (r == 11)
                                {
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 2, sheet.LastRowNum, 1, 16));
                                }
                                break;
                            case 13:
                                for (int c = 1; c < 17; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 1, 16));
                                break;
                            case 14:
                            case 15:
                                for (int c = 2; c < 16; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                if (r == 15)
                                {
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 2, 15));
                                }
                                break;
                            case 16:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(1);
                                sheet.GetRow(sheet.LastRowNum).GetCell(1).CellStyle = sheet.GetRow(r).GetCell(1).CellStyle;
                                break;
                            case 17:
                                for (int c = 1; c < 3; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 1, 2));
                                sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(sheet.GetRow(r).GetCell(1).StringCellValue.ToString());
                                break;
                            case 19:
                                for (int c = 4; c < 7; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum, sheet.LastRowNum, 4, 6));
                                break;
                            case 21:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(7);
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).CellStyle = sheet.GetRow(r).GetCell(7).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(sheet.GetRow(r).GetCell(7).StringCellValue.ToString());
                                break;
                            case 23:
                            case 24:
                            case 25:
                            case 26:
                            case 27:
                                for (int c = 1; c < 17; c++)
                                {
                                    sheet.GetRow(sheet.LastRowNum).CreateCell(c);
                                    sheet.GetRow(sheet.LastRowNum).GetCell(c).CellStyle = sheet.GetRow(r).GetCell(c).CellStyle;
                                }
                                if (r == 27)
                                {
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 4, sheet.LastRowNum, 1, 11));
                                    sheet.GetRow(sheet.LastRowNum - 4).GetCell(1).SetCellValue(sheet.GetRow(r - 4).GetCell(1).StringCellValue.ToString());
                                    sheet.AddMergedRegion(new CellRangeAddress(sheet.LastRowNum - 1, sheet.LastRowNum, 13, 16));
                                    sheet.GetRow(sheet.LastRowNum - 1).GetCell(13).SetCellValue(sheet.GetRow(r - 1).GetCell(13).StringCellValue.ToString());
                                }
                                break;
                            case 29:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(14);
                                sheet.GetRow(sheet.LastRowNum).GetCell(14).CellStyle = sheet.GetRow(r).GetCell(14).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(14).SetCellValue(sheet.GetRow(r).GetCell(14).StringCellValue.ToString());
                                sheet.GetRow(sheet.LastRowNum).CreateCell(16);
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).CellStyle = sheet.GetRow(r).GetCell(16).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).SetCellValue(sheet.GetRow(r).GetCell(16).StringCellValue.ToString());
                                break;
                            case 31:
                                sheet.GetRow(sheet.LastRowNum).CreateCell(16);
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).CellStyle = sheet.GetRow(r).GetCell(16).CellStyle;
                                sheet.GetRow(sheet.LastRowNum).GetCell(16).SetCellValue(sheet.GetRow(r).GetCell(16).StringCellValue.ToString());
                                break;
                            default:
                                break;
                        }
                    }
                    #endregion
                }

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report02020300" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }

        #endregion

        #region Report02020600 債清客戶-列印剔退紀錄
        public static bool CreateExcelFile_Report02020600(DataSet ds, string strRebackDateStart, string strRebackDateEnd, string strApplyDateStart, string strApplyDateEnd, string strUserID, ref string strPathFile, ref string strMsgID)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "PaySVFeedBackList.xls";

                DataTable dtPAY_SV_FEEDBACK = ds.Tables[0];
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                //取得樣式
                HSSFCellStyle contentFormat = getDefaultContentFormat(wb);

                #region 表頭
                sheet.GetRow(2).GetCell(1).SetCellValue($"{Function.InsertTimeSeparator(strRebackDateStart)}~{Function.InsertTimeSeparator(strRebackDateEnd)}");
                sheet.GetRow(3).GetCell(1).SetCellValue($"{Function.InsertTimeSeparator(strApplyDateStart)}~{Function.InsertTimeSeparator(strApplyDateEnd)}");
                sheet.GetRow(4).GetCell(1).SetCellValue($"{strUserID}");
                sheet.GetRow(4).GetCell(5).SetCellValue($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}");
                #endregion

                HSSFCellStyle cs = getDefaultContentFormat(wb);
                HSSFFont contentFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                contentFont.FontHeightInPoints = 10.5; //字體大小
                contentFont.FontName = "新細明體"; //字型
                cs.SetFont(contentFont); //設定儲存格的文字樣式
                #region 表身

                //資料去空白
                removeBlank(ref dtPAY_SV_FEEDBACK);

                for (int row = 0; row < dtPAY_SV_FEEDBACK.Rows.Count; row++)
                {
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col = 0; col < 6; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col);
                        sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = cs;
                    }
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue((row + 1).ToString()); //項次
                    sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dtPAY_SV_FEEDBACK.Rows[row]["UserID"].ToString()); //客戶ID
                    sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(dtPAY_SV_FEEDBACK.Rows[row]["UserName"].ToString()); //客戶姓名
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(dtPAY_SV_FEEDBACK.Rows[row]["ApplyDate"].ToString()); //申請日期
                    sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dtPAY_SV_FEEDBACK.Rows[row]["CardCount"].ToString());//卡片數量
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(dtPAY_SV_FEEDBACK.Rows[row]["RebackReason"].ToString());//剔退原因

                }
                #endregion

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report02020600" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }

        #endregion

        #region Report0302040002 結清證明-結清證明書
        /// <summary>
        /// 專案代號:20210058-CSIP作業服務平台現代化II
        /// 功能說明:結清證明-結清證明書
        /// 作    者:Ares Dennis
        /// 創建時間:2021/12/27
        /// 修改紀錄:20220224_Ares_Jack_總行資料取 M_PROPERTY_CODE.PROPERTY_NAME
        /// </summary>
        /// <param name="ds">資料來源</param>        
        /// <param name="strPathFile">匯出檔案位置</param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Report0302040002(DataSet ds, ref string strPathFile, ref string strMsgID)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "settle.xls";

                DataTable dt = ds.Tables[0];
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                //取得樣式                
                HSSFCellStyle cs = getDefaultContentFormat(wb);
                HSSFCellStyle cs2 = GetDefaultContentFormat2(wb);
                cs.BorderBottom = BorderStyle.None; // 儲存格框線
                cs.BorderLeft = BorderStyle.None;
                cs.BorderTop = BorderStyle.None;
                cs.BorderRight = BorderStyle.None;

                HSSFFont contentFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                contentFont.FontHeightInPoints = 12; //字體大小
                contentFont.FontName = "標楷體"; //字型
                cs.SetFont(contentFont); //設定儲存格的文字樣式

                #region 表身
                //資料去空白
                removeBlank(ref dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sheet = wb.GetSheetAt(i);

                    sheet.GetRow(1).CreateCell(0).SetCellValue(dt.Rows[i]["RecvName"].ToString()); //收件人
                    sheet.GetRow(1).GetCell(0).CellStyle = cs2;
                    sheet.GetRow(2).CreateCell(0).SetCellValue(dt.Rows[i]["zip"].ToString()); //郵遞區號
                    sheet.GetRow(2).GetCell(0).CellStyle = cs2;
                    sheet.GetRow(2).CreateCell(2).SetCellValue(dt.Rows[i]["add1"].ToString()); //地址1
                    sheet.GetRow(2).GetCell(2).CellStyle = cs2;
                    sheet.GetRow(2).CreateCell(6).SetCellValue(dt.Rows[i]["add2"].ToString()); //地址2
                    sheet.GetRow(2).GetCell(6).CellStyle = cs2;
                    sheet.GetRow(3).CreateCell(2).SetCellValue(dt.Rows[i]["add3"].ToString()); //地址3
                    sheet.GetRow(3).GetCell(2).CellStyle = cs2;
                    sheet.GetRow(7).CreateCell(3).SetCellValue(dt.Rows[i]["userName"].ToString()); //姓名
                    sheet.GetRow(7).GetCell(3).CellStyle = cs2;
                    sheet.GetRow(7).CreateCell(11).SetCellValue(dt.Rows[i]["userID"].ToString()); //身分證字號
                    sheet.GetRow(7).GetCell(11).CellStyle = cs2;
                    sheet.GetRow(8).CreateCell(5).SetCellValue(dt.Rows[i]["CardNo"].ToString()); //貸款號碼
                    sheet.GetRow(8).GetCell(5).CellStyle = cs2;
                    sheet.GetRow(8).CreateCell(13).SetCellValue(dt.Rows[i]["LoanDate"].ToString()); //貸放日
                    sheet.GetRow(8).GetCell(13).CellStyle = cs2;
                    sheet.GetRow(9).CreateCell(0).SetCellValue(dt.Rows[i]["PayOffAmt"].ToString()); //貸款金額
                    sheet.GetRow(9).GetCell(0).CellStyle = cs2;
                    sheet.GetRow(9).CreateCell(16).SetCellValue(dt.Rows[i]["PayOffDate"].ToString()); //結清日期
                    sheet.GetRow(9).GetCell(16).CellStyle = cs2;
                    sheet.GetRow(21).CreateCell(20).SetCellValue(dt.Rows[i]["keyinDay"].ToString()); //日期
                    sheet.GetRow(21).GetCell(20).CellStyle = cs2;
                    sheet.GetRow(23).CreateCell(19).SetCellValue(dt.Rows[i]["serialNo"].ToString()); //結清證明號
                    sheet.GetRow(23).GetCell(19).CellStyle = cs2;
                    //總行資料
                    sheet.GetRow(16).CreateCell(0).SetCellValue(dt.Rows[i]["HeadOffice"].ToString()); 
                    sheet.GetRow(16).GetCell(0).CellStyle = cs2;

                    //多筆資料 複製工作表
                    if (i + 1 < dt.Rows.Count)
                    {
                        wb.CloneSheet(0);
                        wb.SetSheetName(i + 1, "工作表" + (i + 2).ToString());
                    }
                }
                #endregion

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report03020400" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }
        #endregion

        #region Report0302030002 結清證明-ML結清證明開立登記表
        /// <summary>
        /// 專案代號:20210058-CSIP作業服務平台現代化II
        /// 功能說明:結清證明-ML結清證明開立登記表
        /// 作    者:Ares Dennis
        /// 創建時間:2021/12/27
        /// </summary>
        /// <param name="ds">資料來源</param>        
        /// <param name="strMAILDAY">寄出日期</param>
        /// <param name="strPathFile">匯出檔案位置</param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Report0302030002(DataSet ds, string strMAILDAY, ref string strPathFile, ref string strMsgID)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "keyinlist.xls";

                DataTable dt = ds.Tables[0];
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                //取得樣式
                HSSFCellStyle contentFormat = getDefaultContentFormat(wb);

                #region 表頭
                string mailDateRegister = $"寄送日期：{strMAILDAY}";
                sheet.GetRow(4).GetCell(0).SetCellValue(mailDateRegister); //寄送日期
                string strPRINTDAY = DateTime.Now.ToString("yyyy/MM/dd").Replace("/", "");
                string printDate = $"開立日期：{strPRINTDAY}";
                sheet.GetRow(5).GetCell(0).SetCellValue(printDate); //開立日期
                #endregion

                HSSFCellStyle cs = getDefaultContentFormat(wb);

                #region 表身
                //資料去空白
                removeBlank(ref dt);

                int dataLastRowNum = 8;
                int dataLastColNum = 15;

                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    sheet.ShiftRows(dataLastRowNum + 1, dataLastRowNum + 2, 1);

                    sheet.CreateRow(dataLastRowNum);
                    for (int col = 0; col < dataLastColNum; col++)
                    {
                        sheet.GetRow(dataLastRowNum).CreateCell(col);
                        sheet.GetRow(dataLastRowNum).GetCell(col).CellStyle = cs;
                    }
                    sheet.GetRow(dataLastRowNum).GetCell(0).SetCellValue(dt.Rows[row]["UserID"].ToString()); //身分證字號
                    sheet.GetRow(dataLastRowNum).GetCell(1).SetCellValue(dt.Rows[row]["UserName"].ToString()); //姓名
                    sheet.GetRow(dataLastRowNum).GetCell(2).SetCellValue(dt.Rows[row]["CardNo"].ToString()); //貸放號碼
                    sheet.GetRow(dataLastRowNum).GetCell(3).SetCellValue(dt.Rows[row]["LoanDay"].ToString()); //撥貸日
                    sheet.GetRow(dataLastRowNum).GetCell(4).SetCellValue(dt.Rows[row]["SettleDay"].ToString()); //結清日
                    sheet.GetRow(dataLastRowNum).GetCell(5).SetCellValue(dt.Rows[row]["LoanAmt"].ToString()); //初貸金額
                    sheet.GetRow(dataLastRowNum).GetCell(6).SetCellValue(dt.Rows[row]["SerialNo"].ToString()); //證明編號
                    sheet.GetRow(dataLastRowNum).GetCell(7).SetCellValue(dt.Rows[row]["Letter"].ToString()); //平信
                    sheet.GetRow(dataLastRowNum).GetCell(8).SetCellValue(dt.Rows[row]["Fast"].ToString()); //限時
                    sheet.GetRow(dataLastRowNum).GetCell(9).SetCellValue(dt.Rows[row]["Reg"].ToString()); //掛號
                    sheet.GetRow(dataLastRowNum).GetCell(10).SetCellValue(dt.Rows[row]["LReg"].ToString()); //限掛
                    sheet.GetRow(dataLastRowNum).GetCell(11).SetCellValue(dt.Rows[row]["Self"].ToString()); //自取
                    sheet.GetRow(dataLastRowNum).GetCell(12).SetCellValue(dt.Rows[row]["Addr"].ToString()); //地址
                    sheet.GetRow(dataLastRowNum).GetCell(13).SetCellValue(dt.Rows[row]["MailNo"].ToString()); //掛號號碼
                    sheet.GetRow(dataLastRowNum).GetCell(14).SetCellValue(dt.Rows[row]["BLKCODE"].ToString()); //BLK CODE                                        

                    dataLastRowNum++;
                }
                #endregion

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report0302030002" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }
        #endregion

        #region Report0302020002 結清證明-自取簽收總表(結清證明)
        /// <summary>
        /// 專案代號:20210058-CSIP作業服務平台現代化II
        /// 功能說明:結清證明-自取簽收總表(結清證明)
        /// 作    者:Ares Dennis
        /// 創建時間:2021/12/28
        /// </summary>
        /// <param name="ds">資料來源</param>        
        /// <param name="strPathFile">匯出檔案位置</param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Report0302020002(DataSet ds, ref string strPathFile, ref string strMsgID)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "selflist.xls";

                DataTable dt = ds.Tables[0];
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                //取得樣式
                HSSFCellStyle cs = getDefaultContentFormat(wb);

                #region 表身
                //資料去空白
                removeBlank(ref dt);

                int dataLastColNum = 9;

                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col = 0; col < dataLastColNum; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col);
                        sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = cs;
                    }
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(dt.Rows[row]["UserID"].ToString()); //身分證字號
                    sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dt.Rows[row]["UserName"].ToString()); //姓名
                    sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(dt.Rows[row]["CardNo"].ToString()); //卡號
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(dt.Rows[row]["LoanDay"].ToString()); //貸放日期
                    sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dt.Rows[row]["SettleAmt"].ToString()); //貸款金額
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(dt.Rows[row]["SettleDay"].ToString()); //結清日期
                    sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue(dt.Rows[row]["SerialNo"].ToString()); //證明編號
                    sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(dt.Rows[row]["SelfDay"].ToString()); //自取日期
                    sheet.GetRow(sheet.LastRowNum).GetCell(8).SetCellValue(dt.Rows[row]["OtherDay"].ToString()); //代領日期                                                            
                }
                #endregion

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report0302020002" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }
        #endregion

        #region Report0302020003 結清證明-自取簽收表(結清證明)
        /// <summary>
        /// 專案代號:20210058-CSIP作業服務平台現代化II
        /// 功能說明:結清證明-自取簽收表(結清證明)
        /// 作    者:Ares Dennis
        /// 創建時間:2021/12/28
        /// </summary>
        /// <param name="ds">資料來源</param>        
        /// <param name="strPathFile">匯出檔案位置</param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Report0302020003(DataSet ds, ref string strPathFile, ref string strMsgID)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "self_sign.xls";

                DataTable dt = ds.Tables[0];
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                //取得樣式
                HSSFCellStyle cs = getDefaultContentFormat(wb);

                cs.BorderBottom = BorderStyle.None; // 儲存格框線
                cs.BorderLeft = BorderStyle.None;
                cs.BorderTop = BorderStyle.None;
                cs.BorderRight = BorderStyle.None;

                #region 表身
                //資料去空白
                removeBlank(ref dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sheet = wb.GetSheetAt(i);

                    sheet.GetRow(4).CreateCell(2).SetCellValue(dt.Rows[i]["UserID"].ToString()); //身分證字號
                    sheet.GetRow(4).GetCell(2).CellStyle = cs;
                    sheet.GetRow(4).CreateCell(5).SetCellValue(dt.Rows[i]["UserName"].ToString()); //姓名
                    sheet.GetRow(4).GetCell(5).CellStyle = cs;
                    sheet.GetRow(5).CreateCell(2).SetCellValue(dt.Rows[i]["CardNo"].ToString()); //貸款卡號
                    sheet.GetRow(5).GetCell(2).CellStyle = cs;
                    sheet.GetRow(5).CreateCell(5).SetCellValue(dt.Rows[i]["SerialNo"].ToString()); //證明編號
                    sheet.GetRow(5).GetCell(5).CellStyle = cs;
                    sheet.GetRow(6).CreateCell(2).SetCellValue(dt.Rows[i]["LoanDay"].ToString()); //貸放日
                    sheet.GetRow(6).GetCell(2).CellStyle = cs;
                    sheet.GetRow(6).CreateCell(5).SetCellValue(dt.Rows[i]["SelfDay"].ToString()); //自取日期
                    sheet.GetRow(6).GetCell(5).CellStyle = cs;
                    sheet.GetRow(7).CreateCell(2).SetCellValue(dt.Rows[i]["SettleAmt"].ToString()); //貸款金額
                    sheet.GetRow(7).GetCell(2).CellStyle = cs;
                    sheet.GetRow(7).CreateCell(5).SetCellValue(dt.Rows[i]["OtherDay"].ToString()); //代領日期
                    sheet.GetRow(7).GetCell(5).CellStyle = cs;
                    sheet.GetRow(8).CreateCell(2).SetCellValue(dt.Rows[i]["SettleDay"].ToString()); //結清日期
                    sheet.GetRow(8).GetCell(2).CellStyle = cs;
                    sheet.GetRow(8).CreateCell(5).SetCellValue(dt.Rows[i]["sNote"].ToString()); //備註
                    sheet.GetRow(8).GetCell(5).CellStyle = cs;

                    //多筆資料 複製工作表
                    if (i + 1 < dt.Rows.Count)
                    {
                        wb.CloneSheet(0);
                        wb.SetSheetName(i + 1, "工作表" + (i + 2).ToString());
                    }
                }
                #endregion

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report0302020003" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }
        #endregion

        #region Report0302060002 開立ML結清證明-大宗掛號存根聯
        /// <summary>
        /// 專案代號:20210058-CSIP作業服務平台現代化II
        /// 功能說明:清償證明-大宗掛號報表
        /// 作    者:Ares Dennis
        /// 創建時間:2022/01/03
        /// </summary>
        /// <param name="ds">資料來源</param>
        /// <param name="strMAILDAY">寄送日期</param>
        /// <param name="strMAILNAME">寄件人</param>
        /// <param name="strPathFile">檔案位置</param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Report0302060002(DataSet ds, string strMAILDAY, string strMAILNAME, ref string strPathFile, ref string strMsgID)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "mail.xls";

                DataTable dt = ds.Tables[0];
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                //取得樣式
                HSSFCellStyle contentFormat = getDefaultContentFormat(wb);

                #region 表頭
                sheet.GetRow(6).CreateCell(1).SetCellValue(strMAILDAY);//寄送日期
                sheet.GetRow(6).CreateCell(4).SetCellValue(strMAILNAME);//寄件人
                string strSenderAddress = string.Empty;
                strSenderAddress = MessageHelper.GetMessage("04_00000000_060");
                sheet.GetRow(6).GetCell(7).SetCellValue($"詳細地址：{strSenderAddress}");//詳細地址
                #endregion

                #region 頁尾
                sheet.GetRow(10).CreateCell(0).SetCellValue($"上開掛號函件共 {dt.Rows.Count} 件照收無誤");//件數
                #endregion

                HSSFCellStyle cs = getDefaultContentFormat(wb);

                #region 表身
                //資料去空白
                removeBlank(ref dt);

                int dataLastRowNum = 9;
                int dataLastColNum = 8;

                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    sheet.ShiftRows(dataLastRowNum + 1, dataLastRowNum + 2, 1);

                    sheet.CreateRow(dataLastRowNum);
                    for (int col = 0; col < dataLastColNum; col++)
                    {
                        sheet.GetRow(dataLastRowNum).CreateCell(col);
                        sheet.GetRow(dataLastRowNum).GetCell(col).CellStyle = cs;
                    }
                    sheet.GetRow(dataLastRowNum).GetCell(0).SetCellValue((row + 1).ToString()); //項次
                    sheet.GetRow(dataLastRowNum).GetCell(1).SetCellValue(dt.Rows[row]["MailNo"].ToString()); //掛號號碼
                    sheet.GetRow(dataLastRowNum).GetCell(2).SetCellValue(dt.Rows[row]["UserName"].ToString()); //姓名                    
                    sheet.GetRow(dataLastRowNum).GetCell(3).SetCellValue(dt.Rows[row]["Addr"].ToString()); //詳細地址
                    sheet.GetRow(dataLastRowNum).GetCell(7).SetCellValue(dt.Rows[row]["sNote"].ToString()); //備註

                    dataLastRowNum++;
                }
                #endregion

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report0302060002" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }
        #endregion

        #region Report0402040002 代償證明-代償證明書
        /// <summary>
        /// 專案代號:20210058-CSIP作業服務平台現代化II
        /// 功能說明:代償證明 - 代償證明書
        /// 作    者:Ares Neal
        /// 創建時間:2022/01/21
        /// 修改紀錄:20220224_Ares_Jack_總行資料取 M_PROPERTY_CODE.PROPERTY_NAME
        /// </summary>
        /// <param name="ds">資料來源</param>
        /// <param name="strMAILDAY">寄送日期</param>
        /// <param name="strPathFile">檔案位置</param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Report0402040001(DataSet ds, string strMAILDAY, ref string strPathFile, ref string strMsgID)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "commute.xls";

                DataTable dt = ds.Tables[0];
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                //取得樣式
                HSSFCellStyle cs = getDefaultContentFormat(wb);
                HSSFCellStyle cs2 = GetDefaultContentFormat2(wb);

                cs.BorderBottom = BorderStyle.None; // 儲存格框線
                cs.BorderLeft = BorderStyle.None;
                cs.BorderTop = BorderStyle.None;
                cs.BorderRight = BorderStyle.None;

                #region 表身
                //資料去空白
                removeBlank(ref dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sheet = wb.GetSheetAt(i);

                    sheet.GetRow(1).CreateCell(1).SetCellValue(dt.Rows[i]["payname"].ToString()); 
                    sheet.GetRow(1).GetCell(1).CellStyle = cs2;

                    sheet.GetRow(2).CreateCell(1).SetCellValue(dt.Rows[i]["zip"].ToString()); 
                    sheet.GetRow(2).GetCell(1).CellStyle = cs2;

                    sheet.GetRow(2).CreateCell(4).SetCellValue(dt.Rows[i]["add1"].ToString());
                    sheet.GetRow(2).GetCell(4).CellStyle = cs2;

                    sheet.GetRow(2).CreateCell(10).SetCellValue(dt.Rows[i]["add2"].ToString());
                    sheet.GetRow(2).GetCell(10).CellStyle = cs2;

                    sheet.GetRow(3).CreateCell(4).SetCellValue(dt.Rows[i]["add3"].ToString());
                    sheet.GetRow(3).GetCell(4).CellStyle = cs2;

                    sheet.GetRow(7).CreateCell(6).SetCellValue(dt.Rows[i]["userName"].ToString());
                    sheet.GetRow(7).GetCell(6).CellStyle = cs2;

                    sheet.GetRow(7).CreateCell(16).SetCellValue(dt.Rows[i]["userID"].ToString());
                    sheet.GetRow(7).GetCell(16).CellStyle = cs2;

                    sheet.GetRow(7).CreateCell(20).SetCellValue(dt.Rows[i]["LoanDate"].ToString());
                    sheet.GetRow(7).GetCell(20).CellStyle = cs2;

                    sheet.GetRow(8).CreateCell(15).SetCellValue(dt.Rows[i]["CardNo"].ToString());
                    sheet.GetRow(8).GetCell(15).CellStyle = cs2;

                    sheet.GetRow(8).CreateCell(24).SetCellValue(dt.Rows[i]["payDay"].ToString());
                    sheet.GetRow(8).GetCell(24).CellStyle = cs2;

                    sheet.GetRow(9).CreateCell(2).SetCellValue(dt.Rows[i]["payName"].ToString());
                    sheet.GetRow(9).GetCell(2).CellStyle = cs2;

                    sheet.GetRow(9).CreateCell(12).SetCellValue(dt.Rows[i]["pay"].ToString());
                    sheet.GetRow(9).GetCell(12).CellStyle = cs2;

                    sheet.GetRow(16).CreateCell(7).SetCellValue(dt.Rows[i]["payName"].ToString());
                    sheet.GetRow(16).GetCell(7).CellStyle = cs2;

                    sheet.GetRow(24).CreateCell(23).SetCellValue(dt.Rows[i]["keyinDay"].ToString());
                    sheet.GetRow(24).GetCell(23).CellStyle = cs2;

                    sheet.GetRow(26).CreateCell(22).SetCellValue(dt.Rows[i]["serialNo"].ToString());
                    sheet.GetRow(26).GetCell(22).CellStyle = cs2;

                    //總行資料
                    sheet.GetRow(18).CreateCell(2).SetCellValue(dt.Rows[i]["HeadOffice"].ToString()); 
                    sheet.GetRow(18).GetCell(2).CellStyle = cs2;

                    //多筆資料 複製工作表
                    if (i + 1 < dt.Rows.Count)
                    {
                        wb.CloneSheet(0);
                        wb.SetSheetName(i + 1, "工作表" + (i + 2).ToString());
                    }
                }
                #endregion

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report0402040001" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }
        #endregion

        #region Report0402030002 代償證明-ML代償證明開立登記表
        /// <summary>
        /// 專案代號:20210058-CSIP作業服務平台現代化II
        /// 功能說明:清償證明-大宗掛號報表
        /// 作    者:Ares Neal
        /// 創建時間:2022/01/03
        /// </summary>
        /// <param name="ds">資料來源</param>
        /// <param name="strMAILDAY">寄送日期</param>
        /// <param name="strMAILNAME">寄件人</param>
        /// <param name="strPathFile">檔案位置</param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Report0402030001(DataSet ds, string strMAILDAY, string strMAILNAME, ref string strPathFile, ref string strMsgID)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "keyinlist_pay.xls";

                DataTable dt = ds.Tables[0];
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                //取得樣式
                HSSFCellStyle contentFormat = getDefaultContentFormat(wb);

                #region 表頭
                sheet.GetRow(4).CreateCell(1).SetCellValue(strMAILDAY);//寄送日期  MailDay1
                sheet.GetRow(5).CreateCell(1).SetCellValue(strMAILNAME);//開立日期 KeyinDay1
                #endregion

                #region 頁尾
                //sheet.GetRow(10).CreateCell(0).SetCellValue($"上開掛號函件共 {dt.Rows.Count} 件照收無誤");//件數
                #endregion

                HSSFCellStyle cs = getDefaultContentFormat(wb);

                #region 表身
                //資料去空白
                //removeBlank(ref dt);

                int dataLastRowNum = 8;
                int dataLastColNum =16;

                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    sheet.ShiftRows(dataLastRowNum +1 , dataLastRowNum  +2, 1);

                    sheet.CreateRow(dataLastRowNum);
                    for (int col = 0; col < dataLastColNum; col++)
                    {
                        sheet.GetRow(dataLastRowNum).CreateCell(col);
                        sheet.GetRow(dataLastRowNum).GetCell(col).CellStyle = cs;
                    }
                    sheet.GetRow(dataLastRowNum).GetCell(0).SetCellValue(dt.Rows[row]["UserID"].ToString()); //身分證字號
                    sheet.GetRow(dataLastRowNum).GetCell(1).SetCellValue(dt.Rows[row]["UserName"].ToString()); //姓名                    
                    sheet.GetRow(dataLastRowNum).GetCell(2).SetCellValue(dt.Rows[row]["CardNo"].ToString()); //貸放號碼
                    sheet.GetRow(dataLastRowNum).GetCell(3).SetCellValue(dt.Rows[row]["LoanDay"].ToString()); //撥貸日

                    sheet.GetRow(dataLastRowNum).GetCell(4).SetCellValue(dt.Rows[row]["PayDay"].ToString()); //代償日
                    sheet.GetRow(dataLastRowNum).GetCell(5).SetCellValue(dt.Rows[row]["PayName"].ToString()); //代償人                    
                    sheet.GetRow(dataLastRowNum).GetCell(6).SetCellValue(dt.Rows[row]["PayAmt"].ToString()); //代償金額
                    sheet.GetRow(dataLastRowNum).GetCell(7).SetCellValue(dt.Rows[row]["SerialNo"].ToString()); //證明編號

                    //領回方式
                    sheet.GetRow(dataLastRowNum).GetCell(8).SetCellValue(dt.Rows[row]["Letter"].ToString()); //平信
                    sheet.GetRow(dataLastRowNum).GetCell(9).SetCellValue(dt.Rows[row]["Fast"].ToString()); //限時                    
                    sheet.GetRow(dataLastRowNum).GetCell(10).SetCellValue(dt.Rows[row]["Reg"].ToString()); //掛號
                    sheet.GetRow(dataLastRowNum).GetCell(11).SetCellValue(dt.Rows[row]["LReg"].ToString()); //限掛
                    sheet.GetRow(dataLastRowNum).GetCell(12).SetCellValue(dt.Rows[row]["Self"].ToString()); //自取

                    sheet.GetRow(dataLastRowNum).GetCell(13).SetCellValue(dt.Rows[row]["Addr"].ToString()); //地址                    
                    sheet.GetRow(dataLastRowNum).GetCell(14).SetCellValue(dt.Rows[row]["MailNo"].ToString()); //掛號號碼
                    sheet.GetRow(dataLastRowNum).GetCell(15).SetCellValue(dt.Rows[row]["BLKCODE"].ToString()); //BLK CODE

                    dataLastRowNum++;
                }
                #endregion

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report0402030001" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }
        #endregion

        #region Report040402060001 列印大宗掛號函件存根聯
        /// <summary>
        /// 專案代號:20210058-CSIP作業服務平台現代化II
        /// 功能說明:代償證明-大宗掛號報表
        /// 作    者:Ares Neal
        /// 創建時間:2022/01/07
        /// 修改紀錄:2022/03/14_Ares_Jack_詳細地址資料來源取 M_PROPERTY_CODE.PROPERTY_NAME
        /// </summary>
        /// <param name="ds">資料來源</param>
        /// <param name="strMAILDAY">寄送日期</param>
        /// <param name="strMAILNAME">寄件人</param>
        /// <param name="strPathFile">檔案位置</param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Report0402060001(DataSet ds, string strMAILDAY, string strMAILNAME, ref string strPathFile, ref string strMsgID)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "mail.xls";
                DataTable dt = ds.Tables[0];
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                //取得樣式
                HSSFCellStyle contentFormat = getDefaultContentFormat(wb);

                #region 表頭
                sheet.GetRow(6).CreateCell(1).SetCellValue(strMAILDAY);//寄送日期
                sheet.GetRow(6).CreateCell(4).SetCellValue(strMAILNAME);//寄件人
                string strSenderAddress = string.Empty;
                strSenderAddress = MessageHelper.GetMessage("04_00000000_060");
                sheet.GetRow(6).GetCell(7).SetCellValue($"詳細地址：{strSenderAddress}");//詳細地址
                #endregion

                #region 頁尾
                sheet.GetRow(10).CreateCell(0).SetCellValue($"上開掛號函件共 {dt.Rows.Count} 件照收無誤");//件數
                #endregion

                HSSFCellStyle cs = getDefaultContentFormat(wb);

                #region 表身
                //資料去空白
                removeBlank(ref dt);

                int dataLastRowNum = 9;
                int dataLastColNum = 8;

                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    sheet.ShiftRows(dataLastRowNum + 1, dataLastRowNum + 2, 1);

                    sheet.CreateRow(dataLastRowNum);
                    for (int col = 0; col < dataLastColNum; col++)
                    {
                        sheet.GetRow(dataLastRowNum).CreateCell(col);
                        sheet.GetRow(dataLastRowNum).GetCell(col).CellStyle = cs;
                    }
                    sheet.GetRow(dataLastRowNum).GetCell(0).SetCellValue((row + 1).ToString()); //項次
                    sheet.GetRow(dataLastRowNum).GetCell(1).SetCellValue(dt.Rows[row]["MailNo"].ToString()); //掛號號碼
                    sheet.GetRow(dataLastRowNum).GetCell(2).SetCellValue(dt.Rows[row]["UserName"].ToString()); //姓名                    
                    sheet.GetRow(dataLastRowNum).GetCell(3).SetCellValue(dt.Rows[row]["Addr"].ToString()); //詳細地址
                    sheet.GetRow(dataLastRowNum).GetCell(7).SetCellValue(dt.Rows[row]["sNote"].ToString()); //備註

                    dataLastRowNum++;
                }
                #endregion

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report0402060001" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }
        #endregion

        #region Report0402020002 代償證明-自取簽收總表
        /// <summary>
        /// 專案代號:20210058-CSIP作業服務平台現代化II
        /// 功能說明:代償證明-自取簽收總表
        /// 作    者:Ares Jack
        /// 創建時間:2022/01/22
        /// </summary>
        /// <param name="ds">資料來源</param>        
        /// <param name="strPathFile">匯出檔案位置</param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Report0402020002(DataSet ds, ref string strPathFile, ref string strMsgID)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "selflist.xls";

                DataTable dt = ds.Tables[0];
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                //取得樣式
                HSSFCellStyle cs = getDefaultContentFormat(wb);

                #region 表身
                //資料去空白
                removeBlank(ref dt);

                int dataLastColNum = 9;

                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    sheet.CreateRow(sheet.LastRowNum + 1);
                    for (int col = 0; col < dataLastColNum; col++)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col);
                        sheet.GetRow(sheet.LastRowNum).GetCell(col).CellStyle = cs;
                    }
                    sheet.GetRow(sheet.LastRowNum).GetCell(0).SetCellValue(dt.Rows[row]["UserID"].ToString()); //身分證字號
                    sheet.GetRow(sheet.LastRowNum).GetCell(1).SetCellValue(dt.Rows[row]["UserName"].ToString()); //姓名
                    sheet.GetRow(sheet.LastRowNum).GetCell(2).SetCellValue(dt.Rows[row]["CardNo"].ToString()); //卡號
                    sheet.GetRow(sheet.LastRowNum).GetCell(3).SetCellValue(dt.Rows[row]["LoanDay"].ToString()); //貸放日期
                    sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellValue(dt.Rows[row]["SettleAmt"].ToString()); //貸款金額
                    sheet.GetRow(sheet.LastRowNum).GetCell(5).SetCellValue(dt.Rows[row]["SettleDay"].ToString()); //結清日期
                    sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellValue(dt.Rows[row]["SerialNo"].ToString()); //證明編號
                    sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellValue(dt.Rows[row]["SelfDay"].ToString()); //自取日期
                    sheet.GetRow(sheet.LastRowNum).GetCell(8).SetCellValue(dt.Rows[row]["OtherDay"].ToString()); //代領日期                                                            
                }
                #endregion

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report0402020002" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }
        #endregion

        #region Report0402020003 代償證明-自取簽收表
        /// <summary>
        /// 專案代號:20210058-CSIP作業服務平台現代化II
        /// 功能說明:結清證明-自取簽收表(結清證明)
        /// 作    者:Ares Jack
        /// 創建時間:2022/01/22
        /// </summary>
        /// <param name="ds">資料來源</param>        
        /// <param name="strPathFile">匯出檔案位置</param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Report0402020003(DataSet ds, ref string strPathFile, ref string strMsgID)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "self_sign.xls";

                DataTable dt = ds.Tables[0];
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                //取得樣式
                HSSFCellStyle cs = getDefaultContentFormat(wb);

                cs.BorderBottom = BorderStyle.None; // 儲存格框線
                cs.BorderLeft = BorderStyle.None;
                cs.BorderTop = BorderStyle.None;
                cs.BorderRight = BorderStyle.None;

                #region 表身
                //資料去空白
                removeBlank(ref dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sheet = wb.GetSheetAt(i);

                    sheet.GetRow(4).CreateCell(2).SetCellValue(dt.Rows[i]["UserID"].ToString()); //身分證字號
                    sheet.GetRow(4).GetCell(2).CellStyle = cs;
                    sheet.GetRow(4).CreateCell(5).SetCellValue(dt.Rows[i]["UserName"].ToString()); //姓名
                    sheet.GetRow(4).GetCell(5).CellStyle = cs;
                    sheet.GetRow(5).CreateCell(2).SetCellValue(dt.Rows[i]["CardNo"].ToString()); //貸款卡號
                    sheet.GetRow(5).GetCell(2).CellStyle = cs;
                    sheet.GetRow(5).CreateCell(5).SetCellValue(dt.Rows[i]["SerialNo"].ToString()); //證明編號
                    sheet.GetRow(5).GetCell(5).CellStyle = cs;
                    sheet.GetRow(6).CreateCell(2).SetCellValue(dt.Rows[i]["LoanDay"].ToString()); //貸放日
                    sheet.GetRow(6).GetCell(2).CellStyle = cs;
                    sheet.GetRow(6).CreateCell(5).SetCellValue(dt.Rows[i]["SelfDay"].ToString()); //自取日期
                    sheet.GetRow(6).GetCell(5).CellStyle = cs;
                    sheet.GetRow(7).CreateCell(2).SetCellValue(dt.Rows[i]["SettleAmt"].ToString()); //貸款金額
                    sheet.GetRow(7).GetCell(2).CellStyle = cs;
                    sheet.GetRow(7).CreateCell(5).SetCellValue(dt.Rows[i]["OtherDay"].ToString()); //代領日期
                    sheet.GetRow(7).GetCell(5).CellStyle = cs;
                    sheet.GetRow(8).CreateCell(2).SetCellValue(dt.Rows[i]["SettleDay"].ToString()); //結清日期
                    sheet.GetRow(8).GetCell(2).CellStyle = cs;
                    sheet.GetRow(8).CreateCell(5).SetCellValue(dt.Rows[i]["sNote"].ToString()); //備註
                    sheet.GetRow(8).GetCell(5).CellStyle = cs;

                    //多筆資料 複製工作表
                    if (i + 1 < dt.Rows.Count)
                    {
                        wb.CloneSheet(0);
                        wb.SetSheetName(i + 1, "工作表" + (i + 2).ToString());
                    }
                }
                #endregion

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report0402020003" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }
        #endregion

        #region Report0101010003 清償證明-新增清償證明
        /// <summary>
        /// 專案代號:20210058-CSIP作業服務平台現代化II
        /// 功能說明:清償證明-新增清償證明
        /// 作    者:Ares Jack
        /// 創建時間:2022/04/06
        /// 修改紀錄：調整報表產出 by Ares Stanley 20220407
        /// </summary>
        /// <param name="ds">資料來源</param>        
        /// <param name="strPathFile">匯出檔案位置</param>
        /// <param name="strMsgID"></param>
        /// <returns></returns>
        public static bool CreateExcelFile_Report0101010003(DataSet ds, ref string strPathFile, ref string strMsgID)
        {
            try
            {
                // 檢查目錄，并刪除以前的文檔資料
                CheckDirectory(ref strPathFile);

                string strExcelPathFile = AppDomain.CurrentDomain.BaseDirectory + UtilHelper.GetAppSettings("ReportTemplate") + "macrolist.xls";

                DataTable dt = ds.Tables[0];
                FileStream fs = new FileStream(strExcelPathFile, FileMode.Open);
                HSSFWorkbook wb = new HSSFWorkbook(fs);
                ISheet sheet = wb.GetSheet("工作表1");

                #region 文字樣式
                HSSFCellStyle contentFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式

                contentFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
                contentFormat.Alignment = HorizontalAlignment.Center; //水平置中
                contentFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字
                contentFormat.BorderBottom = BorderStyle.Thin; // 儲存格框線
                contentFormat.BorderLeft = BorderStyle.Thin;
                contentFormat.BorderTop = BorderStyle.Thin;
                contentFormat.BorderRight = BorderStyle.Thin;

                HSSFFont contentFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                contentFont.FontHeightInPoints = 10.5; //字體大小
                contentFont.FontName = "新細明體"; //字型
                contentFormat.SetFont(contentFont); //設定儲存格的文字樣式

                HSSFCellStyle numberContentFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式

                numberContentFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
                numberContentFormat.Alignment = HorizontalAlignment.Center; //水平置中
                numberContentFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##0"); //將儲存格內容設定為文字
                numberContentFormat.BorderBottom = BorderStyle.Thin; // 儲存格框線
                numberContentFormat.BorderLeft = BorderStyle.Thin;
                numberContentFormat.BorderTop = BorderStyle.Thin;
                numberContentFormat.BorderRight = BorderStyle.Thin;

                HSSFFont numberContentFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                numberContentFont.FontHeightInPoints = 10.5; //字體大小
                numberContentFont.FontName = "新細明體"; //字型
                numberContentFormat.SetFont(numberContentFont); //設定儲存格的文字樣式

                HSSFCellStyle lastRowContentFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
                lastRowContentFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
                lastRowContentFormat.Alignment = HorizontalAlignment.Center; //水平置中
                lastRowContentFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字
                lastRowContentFormat.BorderBottom = BorderStyle.Thin; // 儲存格框線
                lastRowContentFormat.BorderLeft = BorderStyle.Thin;
                lastRowContentFormat.BorderTop = BorderStyle.Thin;
                lastRowContentFormat.BorderRight = BorderStyle.Thin;

                HSSFFont lastRowContentFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                lastRowContentFont.FontHeightInPoints = 12; //字體大小
                lastRowContentFont.FontName = "新細明體"; //字型
                lastRowContentFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                lastRowContentFormat.SetFont(lastRowContentFont); //設定儲存格的文字樣式

                HSSFCellStyle numberLastRowContentFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
                numberLastRowContentFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
                numberLastRowContentFormat.Alignment = HorizontalAlignment.Center; //水平置中
                numberLastRowContentFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##0"); //將儲存格內容設定為文字
                numberLastRowContentFormat.BorderBottom = BorderStyle.Thin; // 儲存格框線
                numberLastRowContentFormat.BorderLeft = BorderStyle.Thin;
                numberLastRowContentFormat.BorderTop = BorderStyle.Thin;
                numberLastRowContentFormat.BorderRight = BorderStyle.Thin;

                HSSFFont numberLastRowContentFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                numberLastRowContentFont.FontHeightInPoints = 12; //字體大小
                numberLastRowContentFont.FontName = "新細明體"; //字型
                numberLastRowContentFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                numberLastRowContentFormat.SetFont(numberLastRowContentFont); //設定儲存格的文字樣式
                #endregion

                #region 表頭
                string mailDateRegister = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                sheet.GetRow(4).GetCell(0).SetCellValue("日期：" + mailDateRegister); //日期
                #endregion



                #region 表身
                //資料去空白
                removeBlank(ref dt);

                for (int row = 0; row < dt.Rows.Count; row++)
                {
                    sheet.CopyRow(sheet.LastRowNum - 2, sheet.LastRowNum - 1);
                    if (row == 0)
                    {
                        for (int col = 0; col < 12; col++)
                        {
                            sheet.GetRow(sheet.LastRowNum - 2).GetCell(col).CellStyle = contentFormat;
                        }
                    }
                    sheet.GetRow(sheet.LastRowNum - 2).GetCell(0).SetCellValue(dt.Rows[row]["M_CUST_ID"].ToString()); //身分證字號
                    sheet.GetRow(sheet.LastRowNum - 2).GetCell(1).SetCellValue(dt.Rows[row]["M_CUST_NAME"].ToString()); //姓名
                    sheet.GetRow(sheet.LastRowNum - 2).GetCell(2).SetCellValue(dt.Rows[row]["M_CARD_NO"].ToString()); //卡號
                    sheet.GetRow(sheet.LastRowNum - 2).GetCell(3).SetCellValue(dt.Rows[row]["M_CLOSE_REP"].ToString()); //案件負責
                    sheet.GetRow(sheet.LastRowNum - 2).GetCell(4).SetCellValue(dt.Rows[row]["M_BALANCE"].ToString()); //目前餘額
                    sheet.GetRow(sheet.LastRowNum - 2).GetCell(5).SetCellValue(dt.Rows[row]["M_DEBT_DATE"].ToString()); //轉呆日期
                    sheet.GetRow(sheet.LastRowNum - 2).GetCell(6).SetCellValue(dt.Rows[row]["M_DEBT_AMT"].ToString()); //轉呆金額
                    sheet.GetRow(sheet.LastRowNum - 2).GetCell(7).SetCellValue(dt.Rows[row]["M_DEBT_BALANCE"].ToString()); //呆帳餘額
                    sheet.GetRow(sheet.LastRowNum - 2).GetCell(8).SetCellValue(dt.Rows[row]["M_LAST_PAY_DATE"].ToString()); //最近繳款
                    sheet.GetRow(sheet.LastRowNum - 2).GetCell(9).SetCellValue(dt.Rows[row]["M_TOTAL_PAYMENT"].ToString()); //累積入帳
                    sheet.GetRow(sheet.LastRowNum - 2).GetCell(10).SetCellValue(dt.Rows[row]["M_CLOSE_DATE"].ToString()); //結案日期
                    sheet.GetRow(sheet.LastRowNum - 2).GetCell(11).SetCellValue(dt.Rows[row]["M_CLOSE_REASON"].ToString()); //結案原因                                     

                    //dataLastRowNum++;
                }

                //移除空白列
                sheet.ShiftRows(sheet.LastRowNum, sheet.LastRowNum, -1);

                //轉換文字為數字
                for (int row = 6; row < sheet.LastRowNum; row++)
                {
                    Int64 result_BALANCE = 0;
                    Int64 result_DEBT_AMT = 0;
                    Int64 result_DEBT_BALANCE = 0;
                    Int64 result_TOTAL_PAYMENT = 0;
                    if (!Int64.TryParse(sheet.GetRow(row).GetCell(4).StringCellValue.ToString(), out result_BALANCE) ||
                        !Int64.TryParse(sheet.GetRow(row).GetCell(6).StringCellValue.ToString(), out result_DEBT_AMT) ||
                        !Int64.TryParse(sheet.GetRow(row).GetCell(7).StringCellValue.ToString(), out result_DEBT_BALANCE) ||
                        !Int64.TryParse(sheet.GetRow(row).GetCell(9).StringCellValue.ToString(), out result_TOTAL_PAYMENT))
                    {
                        Logging.Log("新增清償證明報表-0101010003：文字轉數值時發生錯誤");
                        return false;
                    }
                    else
                    {
                        sheet.GetRow(row).GetCell(4).SetCellValue(result_BALANCE);
                        sheet.GetRow(row).GetCell(6).SetCellValue(result_DEBT_AMT);
                        sheet.GetRow(row).GetCell(7).SetCellValue(result_DEBT_BALANCE);
                        sheet.GetRow(row).GetCell(9).SetCellValue(result_TOTAL_PAYMENT);
                    }
                }

                //單獨處理尾列總計
                for (int col = 4; col < 12; col++)
                {
                    sheet.GetRow(sheet.LastRowNum).RemoveCell(sheet.GetRow(sheet.LastRowNum).GetCell(col));
                    sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = lastRowContentFormat;
                    if (col == 4 || col == 6 || col == 7 || col == 9)
                    {
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).SetCellType(CellType.Formula);
                        sheet.GetRow(sheet.LastRowNum).CreateCell(col).CellStyle = numberLastRowContentFormat;
                    }
                }

                sheet.GetRow(sheet.LastRowNum).GetCell(4).SetCellFormula($"SUM(E7:E{sheet.LastRowNum})");
                sheet.GetRow(sheet.LastRowNum).GetCell(6).SetCellFormula($"SUM(G7:G{sheet.LastRowNum})");
                sheet.GetRow(sheet.LastRowNum).GetCell(7).SetCellFormula($"SUM(H7:H{sheet.LastRowNum})");
                sheet.GetRow(sheet.LastRowNum).GetCell(9).SetCellFormula($"SUM(J7:J{sheet.LastRowNum})");

                //設定千分位符號
                for (int row = 6; row < sheet.LastRowNum; row++)
                {
                    sheet.GetRow(row).GetCell(4).CellStyle = numberContentFormat;
                    sheet.GetRow(row).GetCell(6).CellStyle = numberContentFormat;
                    sheet.GetRow(row).GetCell(7).CellStyle = numberContentFormat;
                    sheet.GetRow(row).GetCell(9).CellStyle = numberContentFormat;
                }
                #endregion

                //預計算公式值
                HSSFFormulaEvaluator.EvaluateAllFormulaCells(wb);

                // 保存文件到運行目錄下
                strPathFile = strPathFile + @"\ExcelFile_Report0101010003" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                FileStream fs1 = new FileStream(strPathFile, FileMode.Create);
                wb.Write(fs1);
                fs1.Close();
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return false;
            }
        }

        #endregion

        /// <summary>
        /// 檢查路徑是否存在，存在刪除該路徑下所有的文檔資料
        /// </summary>
        /// <param name="strPath"></param>
        public static void CheckDirectory(ref string strPath)
        {
            try
            {
                string strOldPath = strPath;
                //* 判斷路徑是否存在
                strPath = strPath + "\\" + DateTime.Now.ToString("yyyyMMdd");
                if (!Directory.Exists(strPath))
                {
                    //* 如果不存在，創建路徑
                    Directory.CreateDirectory(strPath);
                }

                //* 取該路徑下所有路徑
                string[] strDirectories = Directory.GetDirectories(strOldPath);
                for (int intLoop = 0; intLoop < strDirectories.Length; intLoop++)
                {
                    if (strDirectories[intLoop].ToString() != strPath)
                    {
                        if (Directory.Exists(strDirectories[intLoop]))
                        {
                            // * 刪除目錄下的所有文檔
                            DirectoryInfo di = new DirectoryInfo(strDirectories[intLoop]);
                            FileSystemInfo[] fsi = di.GetFileSystemInfos();
                            for (int intIndex = 0; intIndex < fsi.Length; intIndex++)
                            {
                                FileInfo fi = fsi[intIndex] as FileInfo;
                                if (fi != null)
                                {
                                    fi.Delete();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Logging.Log(exp, LogLayer.BusinessRule);
                throw exp;
            }
        }

        #region 共用NPOI
        /// <summary>
        /// 專案代號:
        /// 功能說明:共用NPOI
        /// 作    者:Ares Stanley
        /// 創建時間:2021/12/20
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="wb"></param>
        /// <param name="start"></param>
        /// <param name="sheetName"></param>
        private static void ExportExcelForNPOI(DataTable dt, ref HSSFWorkbook wb, Int32 start, String sheetName)
        {
            try
            {
                HSSFCellStyle cs = (HSSFCellStyle)wb.CreateCellStyle();
                cs.BorderBottom = BorderStyle.Thin;
                cs.BorderLeft = BorderStyle.Thin;
                cs.BorderTop = BorderStyle.Thin;
                cs.BorderRight = BorderStyle.Thin;
                //啟動多行文字
                cs.WrapText = true;
                //文字置中
                cs.VerticalAlignment = VerticalAlignment.Center;
                cs.Alignment = HorizontalAlignment.Center;

                HSSFFont font1 = (HSSFFont)wb.CreateFont();
                //字體尺寸
                font1.FontHeightInPoints = 12;
                font1.FontName = "新細明體";
                cs.SetFont(font1);

                if (dt != null && dt.Rows.Count != 0)
                {
                    int count = start;
                    ISheet sheet = wb.GetSheet(sheetName);
                    int cols = dt.Columns.Count;
                    foreach (DataRow dr in dt.Rows)
                    {
                        int cell = 0;
                        IRow row = (IRow)sheet.CreateRow(count);
                        row.CreateCell(0).SetCellValue(count.ToString());
                        for (int i = 0; i < cols; i++)
                        {
                            row.CreateCell(cell).SetCellValue(dr[i].ToString());
                            row.GetCell(cell).CellStyle = cs;
                            cell++;
                        }
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                throw;
            }
        }

        /// <summary>
        /// 專案代號:
        /// 功能說明:共用NPOI(含filter)
        /// 作    者:Ares Stanley
        /// 創建時間:2021/11/02
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="wb"></param>
        /// <param name="start"></param>
        /// <param name="delColumn"></param>
        /// <param name="sheetName"></param>
        private static void ExportExcelForNPOI_filter(DataTable dt, ref HSSFWorkbook wb, Int32 start, Int32 delColumn, String sheetName)
        {
            try
            {
                HSSFCellStyle cs = (HSSFCellStyle)wb.CreateCellStyle();
                cs.BorderBottom = BorderStyle.Thin;
                cs.BorderLeft = BorderStyle.Thin;
                cs.BorderTop = BorderStyle.Thin;
                cs.BorderRight = BorderStyle.Thin;

                //啟動多行文字
                cs.WrapText = true;
                //文字置中
                cs.VerticalAlignment = VerticalAlignment.Center;
                cs.Alignment = HorizontalAlignment.Center;

                HSSFFont font1 = (HSSFFont)wb.CreateFont();
                //字體尺寸
                font1.FontHeightInPoints = 12;
                font1.FontName = "新細明體";
                cs.SetFont(font1);

                if (dt != null && dt.Rows.Count != 0)
                {
                    int count = start;
                    ISheet sheet = wb.GetSheet(sheetName);
                    int cols = dt.Columns.Count - delColumn;
                    foreach (DataRow dr in dt.Rows)
                    {
                        int cell = 0;
                        IRow row = (IRow)sheet.CreateRow(count);
                        row.CreateCell(0).SetCellValue(count.ToString());
                        for (int i = 0; i < cols; i++)
                        {
                            row.CreateCell(cell).SetCellValue(dr[i].ToString());
                            row.GetCell(cell).CellStyle = cs;
                            cell++;
                        }
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                throw;
            }
        }

        /// <summary>
        /// 專案代號:
        /// 功能說明:共用NPOI
        /// 作    者:Ares Stanley
        /// 創建時間:2021/12/20
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="wb"></param>
        /// <param name="start"></param>
        /// <param name="sheetName"></param>
        private static void ExportExcelForNPOIwithItemNumber(DataTable dt, ref HSSFWorkbook wb, Int32 rowStart, Int32 colStart, String sheetName)
        {
            try
            {
                HSSFCellStyle cs = getDefaultContentFormat(wb);

                if (dt != null && dt.Rows.Count != 0)
                {
                    int count = rowStart;
                    ISheet sheet = wb.GetSheet(sheetName);
                    int cols = dt.Columns.Count;
                    foreach (DataRow dr in dt.Rows)
                    {
                        int cell = colStart;
                        IRow row = (IRow)sheet.CreateRow(count);
                        row.CreateCell(0).SetCellValue(count.ToString());
                        for (int i = 0; i < cols; i++)
                        {
                            if (i == 0)
                            {
                                row.CreateCell(0).SetCellValue(dt.Rows.IndexOf(dr).ToString());
                                row.GetCell(0).CellStyle = cs;
                            }
                            row.CreateCell(cell).SetCellValue(dr[i].ToString());
                            row.GetCell(cell).CellStyle = cs;
                            cell++;
                        }
                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                throw;
            }
        }
        #endregion

        /// <summary>
        /// 專案代號:20210058-CSIP作業服務平台現代化II
        /// 功能說明:取得共用報表樣式
        /// 作    者:Ares Stanley
        /// 創建時間:2021/12/20
        /// </summary>
        /// <param name="wb"></param>
        /// <returns></returns>
        public static HSSFCellStyle getDefaultContentFormat(HSSFWorkbook wb)
        {
            HSSFCellStyle contentFormat = (HSSFCellStyle)wb.CreateCellStyle(); //建立文字格式
            try
            {
                contentFormat.VerticalAlignment = VerticalAlignment.Center; //垂直置中
                //contentFormat.Alignment = HorizontalAlignment.Center; //水平置中
                contentFormat.DataFormat = HSSFDataFormat.GetBuiltinFormat("@"); //將儲存格內容設定為文字
                contentFormat.BorderBottom = BorderStyle.Thin; // 儲存格框線
                contentFormat.BorderLeft = BorderStyle.Thin;
                contentFormat.BorderTop = BorderStyle.Thin;
                contentFormat.BorderRight = BorderStyle.Thin;

                HSSFFont contentFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                contentFont.FontHeightInPoints = 12; //字體大小
                contentFont.FontName = "新細明體"; //字型
                contentFormat.SetFont(contentFont); //設定儲存格的文字樣式
                return contentFormat;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return contentFormat;
            }
        }

        /// <summary>
        /// 專案代號:20210058-CSIP作業服務平台現代化II
        /// 功能說明:取得總行資料報表樣式
        /// 作    者:Ares Jack
        /// 創建時間:2022/02/24
        /// <summary>
        /// </summary>
        /// <param name="wb"></param>
        /// <returns></returns>
        public static HSSFCellStyle GetDefaultContentFormat2(HSSFWorkbook wb)
        {
            HSSFCellStyle contentFormat = (HSSFCellStyle)wb.CreateCellStyle();
            try
            {
                contentFormat.WrapText = true;
                contentFormat.BorderBottom = BorderStyle.None;
                contentFormat.BorderBottom = BorderStyle.None;
                contentFormat.BorderTop = BorderStyle.None;
                contentFormat.BorderRight = BorderStyle.None;
                contentFormat.Alignment = HorizontalAlignment.Left;
                contentFormat.VerticalAlignment = VerticalAlignment.Top;

                HSSFFont contentFont = (HSSFFont)wb.CreateFont(); //建立文字樣式
                contentFont.FontHeightInPoints = 12; //字體大小
                contentFont.FontName = "標楷體"; //字型
                contentFormat.SetFont(contentFont); //設定儲存格的文字樣式
                return contentFormat;
            }
            catch (Exception ex)
            {
                Logging.Log(ex);
                return contentFormat;
            }
        }

        /// <summary>
        /// 專案代號:20210058-CSIP作業服務平台現代化II
        /// 功能說明:移除文字資料空白
        /// 修改紀錄：調整移除空白僅針對文字資料 by Ares Stanley 20220406
        /// 作    者:Ares Stanley
        /// 創建時間:2021/12/20
        /// </summary>
        /// <param name="dt"></param>
        public static void removeBlank(ref DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (dr[i].GetType() == System.Type.GetType("System.String"))
                    {
                        dr[i] = dr[i].ToString().Trim();
                    }
                }
            }
        }

    }

}
