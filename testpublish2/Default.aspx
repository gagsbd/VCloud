<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>LiveIT Cloud Provisioning</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.min.css" integrity="sha384-MCw98/SFnGE8fJT3GXwEOngsV7Zt27NXFoaoApmYm81iuXoPkFOJwJ8ERdknLPMO" crossorigin="anonymous"/>
   
    
</head>
<body class="align-content-md-center">
       <form id="form1" runat="server">
        
        <div class="container "

        
        
                   <br />
                   <br />
                   <asp:Table ID="Table1" runat="server" CssClass="table table-dark" Height="28px">
               <asp:TableRow>
                   <asp:TableCell>
                       Company Name:  <asp:TextBox ID="txtCompanyFullName" runat="server"></asp:TextBox>
                   </asp:TableCell>
                   <asp:TableCell>
                       Company Short Name:  <asp:TextBox ID="txtCompanyShortName" runat="server"></asp:TextBox>
                   </asp:TableCell>
                   <asp:TableCell>

                   </asp:TableCell>
               </asp:TableRow>
               </asp:Table>
                      
        </div>
        
        
                    
       <br />
        <br />
        Your Name&nbsp;
        <asp:TextBox ID="txtOrgName" runat="server"></asp:TextBox><br />
        Email Address&nbsp;
        <asp:TextBox ID="txtOrgEmailAddress" runat="server"></asp:TextBox><br />
        Password&nbsp;
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" ToolTip="Password must be at least 6 characters long"></asp:TextBox><br />
        Reenter Password&nbsp;
        <asp:TextBox ID="txtPasswordCheck" runat="server" TextMode="Password" ToolTip="Password must be at least 6 characters long"></asp:TextBox><br />
        Credit Card Info<br />
        First name&nbsp;&nbsp;
        <asp:TextBox ID="txtCCFirstName" runat="server"></asp:TextBox><br />
        Last Name&nbsp;
        <asp:TextBox ID="txtCCLastName" runat="server"></asp:TextBox><br />
        Email Address&nbsp;
        <asp:TextBox ID="txtCCEmailAddress" runat="server"></asp:TextBox><br />
        Company Name&nbsp;
        <asp:TextBox ID="txtCCCompanyName" runat="server"></asp:TextBox><br />
        Card Number&nbsp;
        <asp:TextBox ID="txtCCNumber" runat="server"></asp:TextBox><br />
        Card CVC&nbsp;
        <asp:TextBox ID="txtCCCVC" runat="server"></asp:TextBox><br />
        Billing Address&nbsp;
        <asp:TextBox ID="txtCCBillingAddress" runat="server"></asp:TextBox><br />
        2 Digit Month&nbsp;
        <asp:TextBox ID="txtCCMonth" runat="server"></asp:TextBox><br />
        4 Digit Year&nbsp;
        <asp:TextBox ID="txtCCYear" runat="server"></asp:TextBox><br />
        City&nbsp;
        <asp:TextBox ID="txtCCCity" runat="server"></asp:TextBox><br />
        State&nbsp;
        <asp:TextBox ID="txtCCState" runat="server"></asp:TextBox><br />
        Zip&nbsp;
        <asp:TextBox ID="txtCCZip" runat="server"></asp:TextBox>&nbsp;
        <br />
        <br />
        <asp:Button ID="Button1" runat="server" Text="Provision" />
        <br />
        <p>
            &nbsp;</p>
           <p>
               &nbsp;</p>
  
    <div class="container">
  <div class="row">
    <div class="col align-self-start">
      One of three columns
    </div>
    <div class="col align-self-center">
      One of three columns
    </div>
    <div class="col align-self-end">
      One of three columns
    </div>
  </div>
</div>

 <script src="https://code.jquery.com/jquery-3.3.1.slim.min.js" integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo" crossorigin="anonymous"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js" integrity="sha384-ZMP7rVo3mIykV+2+9J3UJ46jBk0WLaUAdn689aCwoqbBJiSnjAK/l8WvCWPIPm49" crossorigin="anonymous"></script>
<script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/js/bootstrap.min.js" integrity="sha384-ChfqqxuZUCnJSK3+MXmPNIyE6ZbWh2IMqE241rYiqJxyMiZ6OW/JmZQ5stwEULTy" crossorigin="anonymous"></script>   
          
    </form>
    </body>
</html>
