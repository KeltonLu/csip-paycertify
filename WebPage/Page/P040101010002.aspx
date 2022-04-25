<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040101010002.aspx.cs" Inherits="Page_P040101010002" %>

<%@ Register Src="../Common/Controls/CustAddress.ascx" TagName="CustAddress" TagPrefix="uc1" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>
    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>
    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>
    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
</head>
<body class="workingArea">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="True" runat="server">
        </asp:ScriptManager>
        
        <script language="javascript" type="text/javascript">
			window.addEventListener("scroll", scroll, false);
              function   scroll()   
              {   
                $("#divProgress").css("top",290+document.documentElement.scrollTop);
              }    
		</script>

        <asp:UpdateProgress ID="updateProgress1" runat="server">
            <ProgressTemplate>
                <div id="divProgress" align="center" class="progress" style="position: absolute;
                    top: 290px; width: 100%; filter: Alpha(opacity=80); text-align: center;">
                    <div id="divProgress2" align="center" class="progress" style="background-color: White;
                        width: 50%; margin: 0px auto;">
                        <br />
                        <img alt="Please Wait..." src="../Common/images/Waiting.gif" />
                        <br />
                        <cc1:CustLabel ID="lblWaiting" runat="server" CurAlign="center" CurSymbol="£" FractionalDigit="2"
                            IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                            SetBreak="False" SetOmit="False" ShowID="00_00000000_000" StickHeight="False"></cc1:CustLabel>
                    </div>
                </div>
            </ProgressTemplate>
        </asp:UpdateProgress>
        
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                    <tr class="itemTitle">
                        <td colspan="4">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="04_01010100_001" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblCustID" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_018" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtCustID" runat="server" MaxLength="11"  InputType="LetterAndInt" Enabled="False"></cc1:CustTextBox>
                            <%-- 20220425 RQ-2022-003727-000_開立清償證明系統-長姓名專案 By Kelton Start--%>
                            <cc1:CustButton ID="btnSearch" runat="server" class="smallButton" Style="width: 70px;"
                                ShowID="04_01010100_068" OnClick="btnSearch_Click" />&nbsp;&nbsp;&nbsp;
                            <%-- 20220425 RQ-2022-003727-000_開立清償證明系統-長姓名專案 By Kelton  End--%>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblCustName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_019" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtCustName" runat="server" MaxLength="50"></cc1:CustTextBox></td>
                    </tr>
                    <%-- 20220425 RQ-2022-003727-000_開立清償證明系統-長姓名專案 By Kelton Start--%>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblCustName_L" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_066" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtCustName_L" runat="server" MaxLength="50" ReadOnly="false"></cc1:CustTextBox></td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblCustName_Rome" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_067" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtCustName_Rome" runat="server" MaxLength="50" ReadOnly="false"></cc1:CustTextBox></td>
                    </tr>
                    <%-- 20220425 RQ-2022-003727-000_開立清償證明系統-長姓名專案 By Kelton  End--%>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblPayName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_020" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtPayName" runat="server" MaxLength="5" ReadOnly="false"></cc1:CustTextBox></td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblZip" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_021" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtZip" runat="server" MaxLength="3" ReadOnly="false" InputType="Int"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblAdd1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_022" StickHeight="False"></cc1:CustLabel></td>
                        <td colspan="3">
                            <uc1:CustAddress id="CustAdd1" runat="server" OnChangeValues="CustAdd1_ChangeValues">
                            </uc1:CustAddress>
                            
                            </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblAdd2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_023" StickHeight="False"></cc1:CustLabel></td>
                        <td colspan="3">
                            <cc1:CustTextBox ID="txtAdd2" runat="server"  ReadOnly="false" MaxLength="28" Width="500px"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblAdd3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_024" StickHeight="False"></cc1:CustLabel></td>
                        <td colspan="3">
                            <cc1:CustTextBox ID="txtAdd3" runat="server" MaxLength="28" Width="500px"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%; height: 25px;">
                            <cc1:CustLabel ID="lblRecievName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_025" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%; height: 25px;">
                            <cc1:CustTextBox ID="txtRecievName" runat="server" ReadOnly="false" MaxLength="5"></cc1:CustTextBox>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblIsFree" runat="server" IsColon="True" ShowID="04_01010100_065"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustCheckBox ID="chkIsFree" runat="server" Checked="true" /></td>   
                    </tr>
                    <tr class="trOdd">
                    <td align="right" style="width: 15%; height: 25px;">
                            <cc1:CustLabel ID="lblMailDay" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_026" StickHeight="False"></cc1:CustLabel>
                        </td>
                        <td style="width: 35%; height: 25px;">
                            <cc1:DatePicker ID="dpMailDay" runat="server" Width="150"></cc1:DatePicker>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblMailNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_027" StickHeight="False"></cc1:CustLabel></td>
                        <td>
                            <cc1:CustTextBox ID="txtMailNo" runat="server" MaxLength="20" InputType="Int"></cc1:CustTextBox></td>
                        
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblNote" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_028" StickHeight="False"></cc1:CustLabel></td>
                        <td colspan="3">
                            <cc1:CustTextBox ID="txtNote" TextMode="MultiLine" Width="500px" Height="70px"
                                MaxLength="50" runat="server" InputType="Memo"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblMakeUpDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_029" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:DatePicker ID="dpMakeUpDate" runat="server" Width="150"></cc1:DatePicker>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblEndDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_030" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:DatePicker ID="dpEndDate" runat="server" Width="150"></cc1:DatePicker>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblMailType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_031" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustDropDownList ID="dropMailType" runat="server">
                            </cc1:CustDropDownList></td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblGetType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_032" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustRadioButtonList ID="radlGetType" runat="server" RepeatDirection="Horizontal" AutoPostBack="True" OnSelectedIndexChanged="radlGetType_SelectedIndexChanged">
                            </cc1:CustRadioButtonList></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblShowExtra" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_035" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustCheckBox ID="chkShowExtra" runat="server" Checked="true" /></td>
                        <td align="right" style="width: 15%">
                            &nbsp;<cc1:CustLabel ID="lblGetFee" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_01010100_036"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustCheckBox ID="chkGetFee" runat="server" /></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblOwe" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_037" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtOwe" runat="server" MaxLength="9" ReadOnly="false" InputType="Int"></cc1:CustTextBox></td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblPay" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_038" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtPay" runat="server" MaxLength="9" ReadOnly="false" InputType="Int"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblPayDay" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_01010100_039" StickHeight="False"></cc1:CustLabel></td>
                        <td colspan="3">
                            <cc1:DatePicker ID="dpPayDay" runat="server" Width="150"></cc1:DatePicker>
                        </td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="4">
                            <cc1:CustButton ID="btnSave" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="04_01010100_040" OnClick="btnSave_Click" />&nbsp;&nbsp;&nbsp;
                            <cc1:CustButton ID="btnCancel" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="04_01010100_041" OnClick="btnCancel_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
