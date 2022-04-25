<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040201020002.aspx.cs" Inherits="Page_P040201020002" %>
<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<%@ Register Src="../Common/Controls/CustAddress.ascx" TagName="CustAddress" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>
    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>
    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>
    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>
    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
    <script language="javascript" type="text/javascript">	
    <!--      
        //* 當取得方式選擇‘郵寄’或‘自取’時，
        function selectedGetType()
        {
            //* 郵寄時‘郵寄方式’和‘掛號號碼’兩個欄位可以輸入
            if (document.getElementById("radlGetType_0").checked)
            {
                document.getElementById("dropMailType").disabled = false;
                document.getElementById("txtMailNo").disabled = false;
            }else
            {
            //* 自取時‘郵寄方式’和‘掛號號碼’兩個欄位不可以輸入
                document.getElementById("dropMailType").disabled = true;
                document.getElementById("txtMailNo").disabled = true;
                document.getElementById("txtMailNo").value = "";
            }
        }
    //-->
    </script>
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
                        <td colspan="4"><li>
                                    <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="04_02010200_001" StickHeight="False"></cc1:CustLabel>
                        </li></td>
                    </tr>
                    <tr class="trOdd">
                        <td style="text-align: right; width: 15%; height: 25px;">
                                <cc1:CustLabel ID="lblUser_ID" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="04_02010200_016" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel>
                            </td>
                        <td style="text-align: left; width: 30%; height: 25px;">
                            <cc1:CustTextBox ID="txtUserID" MaxLength="11" runat="server" Enabled="False"></cc1:CustTextBox>
                            </td>
                            
                        <td style="text-align: right; width: 15%; height: 25px;">
                                <cc1:CustLabel ID="lblUserName" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="04_02010200_017" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td style="text-align: left; width: 30%; height: 25px;">
                            <cc1:CustTextBox ID="txtUserName" MaxLength="5" runat="server"></cc1:CustTextBox>
                            </td>
                    </tr>
                    <tr class="trEven">
                        <td style="text-align: right; width: 15%; height: 25px;">
                            <cc1:CustLabel ID="lblZip" runat="server" FractionalDigit="2" IsColon="True"
                                IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0" SetBreak="False"
                                SetOmit="False" StickHeight="False" ShowID="04_02010200_018" CurAlign="left"
                                CurSymbol="£"></cc1:CustLabel></td>
                        <td colspan="3">
                            <cc1:CustTextBox ID="txtZip" MaxLength="3" runat="server" InputType="LetterAndInt"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblAdd1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_02010200_019" StickHeight="False"></cc1:CustLabel></td>
                        <td colspan="3">
                            <uc1:CustAddress id="CustAdd1" runat="server" OnChangeValues="CustAdd1_ChangeValues">
                            </uc1:CustAddress></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                                <cc1:CustLabel ID="lblAdd2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_02010200_020" StickHeight="False"></cc1:CustLabel></td>
                        <td colspan="3" >
                                <cc1:CustTextBox ID="txtAdd2" runat="server"
                                ReadOnly="false" MaxLength="28" Width="500px"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                             <cc1:CustLabel ID="lblAdd3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_02010200_021" StickHeight="False"></cc1:CustLabel></td>
                        <td colspan="3" style="height: 25px">
                            <cc1:CustTextBox ID="txtAdd3" runat="server"
                                ReadOnly="false" MaxLength="28" Width="500px"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%; height: 25px;">
                            <cc1:CustLabel ID="lblRecievName" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_02010200_022" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 30%; height: 25px">
                            <cc1:CustTextBox ID="txtRecievName" MaxLength="5" runat="server"></cc1:CustTextBox></td>
                        <td align="right" style="width: 15%; height: 25px;">
                            <cc1:CustLabel ID="lblMailDay" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_02010200_023" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 30%; height: 25px">
                             <cc1:DatePicker ID="dpMailDay" runat="server" Width="150"></cc1:DatePicker></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblMailNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_02010200_024" StickHeight="False"></cc1:CustLabel></td>
                        <td colspan="3" style="width: 85%; height: 25px">
                            <cc1:CustTextBox ID="txtMailNo" runat="server" MaxLength="20" InputType="Int"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblNote" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_02010200_025" StickHeight="False"></cc1:CustLabel></td>
                        <td colspan="3" style="width: 85%; height: 25px">
                            <cc1:CustTextBox ID="txtNote" TextMode="MultiLine" Width="500px" Height="70px"
                                MaxLength="50" runat="server" InputType="Memo"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblMakeUpDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_02010200_026" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 30%">
                            <cc1:DatePicker ID="dpMakeUpDate"  runat="server" Width="150"></cc1:DatePicker>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblEndDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_02010200_027" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 30%">
                            <cc1:DatePicker ID="dpEndDate" runat="server" Width="150"></cc1:DatePicker></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblMailType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_02010200_028" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 30%">
                            <cc1:CustDropDownList ID="dropMailType" runat="server">
                            </cc1:CustDropDownList></td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblGetType" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_02010200_040" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 30%">
                            <cc1:CustRadioButtonList ID="radlGetType" runat="server" RepeatDirection="Horizontal" Height="1px">
                            </cc1:CustRadioButtonList></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblShowExtra" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_02010200_031" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 30%; height: 25px">
                            <cc1:CustCheckBox ID="chkShowExtra" runat="server" /></td>
                        <td  align="right" style="width: 15%; height: 25px">
                             <cc1:CustLabel ID="lblGetFee" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_02010200_032"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 30%; height: 25px">
                            <cc1:CustCheckBox ID="chkGetFee" runat="server" /></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblReturnDate" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_02010200_033" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 30%">
                            <cc1:DatePicker ID="dpReturnDate" runat="server" Enable="false" Width="150" ></cc1:DatePicker></td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblReturnCause" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_02010200_034" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 100px">
                            <cc1:CustTextBox ID="txtReturnCause" runat="server" Enabled="False" ></cc1:CustTextBox></td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="4">
                             <cc1:CustButton ID="btnSave" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="04_04010200_024" OnClick="btnSave_Click"  />&nbsp;&nbsp;&nbsp;
                            <cc1:CustButton ID="btnReturn" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="04_04010200_025" OnClick="btnReturn_Click" />
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
