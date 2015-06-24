<%@ Page Title="Confirm Email" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="~/ConfirmEmail.aspx.cs" Inherits="Account_ConfirmEmail" Async="true" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:content runat="server" id="BodyContent" contentplaceholderid="MainContent">
    <% if (Success)
       { %>
    <p>
        Thank you for confirming your email. You can now Log In !
    </p>
    <% }
       else
       {%>
          
    <p>
        Something went wrong please try again !
    </p> 
       <%}%>
    </asp:content>
