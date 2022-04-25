<%@ Page Language="C#" AutoEventWireup="true" CodeFile="P040301010001.aspx.cs" Inherits="Page_P040301010001" %>

<%@ Register Assembly="Framework.WebControls" Namespace="Framework.WebControls" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript" src="../Common/Script/JavaScript.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-1.3.2.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/jquery-ui-1.7.min.js"></script>

    <script type="text/javascript" src="../Common/Script/JQuery/WINF_JQuery.js"></script>

    <link href="../App_Themes/Default/global.css" type="text/css" rel="stylesheet" />
        <style type="text/css" >

   .btnHiden

    {display:none; }

    </style>
    <script language="javascript" type="text/javascript">	
    <!--
        //* 顯示或隱藏明細部分
        function displayDetail( disType )
        {
            document.getElementById("TableDetail").style.display=disType;
        }
        
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
               
        function checkInputText()
        {
              var obj =  document.getElementById("txtUserIDQuery");
               if(obj.value == "")
              {
                   alert("請輸入身份證字號! ");
                   obj.focus();
                   return false;
              }        
              return true;
        }

        function CheckConditionSave(){
            var strMsg = "";
            //* 郵遞區號必須輸入
            if (document.getElementById("txtZip").value.Trim() == "")
            {
                if(strMsg==""){
                    document.getElementById("txtZip").focus();
                }
                strMsg = strMsg + "郵遞區號不可為空白\n\n";
            }

            //* 地址一必須輸入
            if (document.getElementById("txtAdd1").value.Trim() == "")
            {
                if(strMsg==""){
                    document.getElementById("txtAdd1").focus();
                }
                strMsg = strMsg + "地址一不可為空白\n\n";
            }

            //* 地址二必須輸入
            if (document.getElementById("txtAdd2").value.Trim() == "")
            {
                if(strMsg==""){
                    document.getElementById("txtAdd2").focus();
                }
                strMsg = strMsg + "地址二不可為空白\n\n";
            }
            
             //* 收件人姓名必須輸入
            if (document.getElementById("txtRecvName").value.Trim() == "")
            {
                if(strMsg==""){
                    document.getElementById("txtRecvName").focus();
                }
                strMsg = strMsg + "收件人姓名不可為空白\n\n";
            }

            //* 取得方式為'郵寄'時
            if (document.getElementById("radlGetType_0").checked)
            {
                var obj =  document.getElementById("dropMailType");                                //* 郵寄方式沒有選擇，提示‘證明取得方式為郵寄，郵寄方式欄位必須輸入’                var strMailType = obj.options[obj.selectedIndex].innerText;
                if (strMailType == "")
                {
                    if(strMsg==""){
                        obj.focus();
                    }
                    strMsg = strMsg + "證明取得方式為郵寄，郵寄方式欄位必須輸入\n\n";
                }

                //* 如果郵寄方式為掛號的話則此欄位為 必輸入欄位請輸入掛號號碼
                if (strMailType=="掛號")
                {
                    //* 掛號號碼為空時，
                    if (document.getElementById("txtMailNo").value.Trim() == "")
                    {
                        if(strMsg==""){
                            document.getElementById("txtMailNo").focus();
                        }
                        strMsg = strMsg + "郵寄方式為掛號，掛號號碼必須輸入\n\n";
                    }
                }
            }
            
            //* 開立日期必須輸入
            if (document.getElementById("dpLoanDay_foo").value.Trim() == "")
            {
                if(strMsg==""){
                    document.getElementById("dpLoanDay_foo").focus();
                }
                strMsg = strMsg + "開立日期不可為空白\n\n";
            }

            //* 郵寄/自取日期必須輸入
            if (document.getElementById("dpMailDay_foo").value.Trim() == "")
            {
                if(strMsg==""){
                    document.getElementById("dpMailDay_foo").focus();
                }
                strMsg = strMsg + "郵寄/自取日期不可為空白\n\n";
            }     
            
            if(strMsg==""){ 
                return true;       
            }else{
                alert(strMsg);
                return false;       
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
                        <td colspan="4">
                            <li>
                                <cc1:CustLabel ID="lblTitle" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                    IsColon="False" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                    SetBreak="False" SetOmit="False" ShowID="04_03010100_001" StickHeight="False"></cc1:CustLabel></li>
                        </td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblUserIDQuery" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010100_002"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td colspan="3" style="width: 80%">
                            <cc1:CustTextBox ID="txtUserIDQuery" runat="server" MaxLength="11" InputType="LetterAndInt"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr class="itemTitle">
                        <td colspan="4" align="center">
                            <cc1:CustButton ID="btnSearch" class="smallButton" runat="server" ShowID="04_03010100_018"
                                Style="width: 50px" OnClick="btnSearch_Click"  OnClientClick="return checkInputText();"></cc1:CustButton>
                        </td>
                    </tr>
                </table>
                <table width="100%" border="0" cellpadding="0" cellspacing="1" id="TableDetail" style="display: none">
                    <tr class="trOdd">
                        <td align="right" style="width: 15%; height: 25px;">
                            <cc1:CustLabel ID="lblUserID" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010100_002"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%; height: 25px;">
                            <cc1:CustTextBox ID="txtUserID" runat="server" MaxLength="11"  Enabled="False" InputType="LetterAndInt"></cc1:CustTextBox></td>
                        <td align="right" style="width: 15%; height: 25px;">
                            <cc1:CustLabel ID="lblUserName" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010100_003"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%; height: 25px;">
                            <cc1:CustTextBox ID="txtUserName" runat="server" MaxLength="10"  Enabled="False"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblCardNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_03010100_004" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustDropDownList ID="dropCardNo" runat="server">
                            </cc1:CustDropDownList></td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblZip" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_03010100_005" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtZip" runat="server" MaxLength="3" InputType="Int"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblAdd1" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_03010100_006" StickHeight="False"></cc1:CustLabel></td>
                        <td colspan="3" style="width: 80%">
                            <cc1:CustTextBox ID="txtAdd1" runat="server" MaxLength="25" Width="448px" ></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 20%; height: 25px;">
                            <cc1:CustLabel ID="lblAdd2" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_03010100_007" StickHeight="False"></cc1:CustLabel></td>
                        <td colspan="3" style="width: 80%; height: 25px;">
                            <cc1:CustTextBox ID="txtAdd2" runat="server" MaxLength="30" Width="448px" ></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%">
                            <cc1:CustLabel ID="lblAdd3" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_03010100_008" StickHeight="False"></cc1:CustLabel></td>
                        <td colspan="3" style="width: 80%">
                            <cc1:CustTextBox ID="txtAdd3" runat="server" MaxLength="50" Width="448px"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblRecvName" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010100_009"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtRecvName" runat="server" MaxLength="10" Style="text-align: left"></cc1:CustTextBox></td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblGetType" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010100_010"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustRadioButtonList ID="radlGetType" runat="server" RepeatDirection="Horizontal">
                            </cc1:CustRadioButtonList></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblLoanDate" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010100_013"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:DatePicker ID="dpLoanDay" Text="" MaxLength="10" runat="server">
                            </cc1:DatePicker>
                        </td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblMailDate" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010100_014"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:DatePicker ID="dpMailDay" Text="" MaxLength="10" runat="server">
                            </cc1:DatePicker>
                        </td>
                    </tr>
                    <tr class="trEven">
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblMailType" runat="server" CurAlign="left" CurSymbol="£"
                                FractionalDigit="2" IsColon="True" IsCurrency="False" NeedDateFormat="False"
                                NumBreak="0" NumOmit="0" SetBreak="False" SetOmit="False" ShowID="04_03010100_015"
                                StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustDropDownList ID="dropMailType" runat="server">
                            </cc1:CustDropDownList></td>
                        <td align="right" style="width: 15%">
                            <cc1:CustLabel ID="lblMailNo" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_03010100_016" StickHeight="False"></cc1:CustLabel></td>
                        <td style="width: 35%">
                            <cc1:CustTextBox ID="txtMailNo" runat="server" MaxLength="10" InputType="Int"></cc1:CustTextBox></td>
                    </tr>
                    <tr class="trOdd">
                        <td align="right" style="width: 20%; height: 34px;">
                            <cc1:CustLabel ID="lblNote" runat="server" CurAlign="left" CurSymbol="£" FractionalDigit="2"
                                IsColon="True" IsCurrency="False" NeedDateFormat="False" NumBreak="0" NumOmit="0"
                                SetBreak="False" SetOmit="False" ShowID="04_03010100_017" StickHeight="False"></cc1:CustLabel></td>
                        <td colspan="3" style="width: 80%; height: 34px;">
                            <cc1:CustTextBox ID="txtNote" runat="server" Width="448px" MaxLength="100" InputType="Memo"
                                Height="112px" TextMode="MultiLine" ReadOnly="false"></cc1:CustTextBox>
                        </td>
                    </tr>
                    <tr align="center" class="itemTitle">
                        <td colspan="4">
                            <cc1:CustButton ID="btnSave" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="04_03010100_019"  DisabledWhenSubmit="False"  OnClick="btnSave_Click"  OnClientClick="return CheckConditionSave();"/>&nbsp;&nbsp;
                            <cc1:CustButton ID="btnClear" runat="server" class="smallButton" Style="width: 50px;"
                                ShowID="04_03010100_020" OnClick="btnClear_Click" />
                        </td>
                    </tr>
                </table>
                <cc1:CustHiddenField ID="hidTag1" runat="server"></cc1:CustHiddenField>
                <cc1:CustHiddenField ID="hidTag2" runat="server"></cc1:CustHiddenField>
            </ContentTemplate>
        </asp:UpdatePanel>
                  <cc1:CustButton ID="btnHiden" runat="server"  OnClick="btnHiden_Click" CssClass="btnHiden" />
          <cc1:CustButton ID="btnHiden1" runat="server"  OnClick="btnHiden1_Click" CssClass="btnHiden" />
    </form>
</body>
</html>
