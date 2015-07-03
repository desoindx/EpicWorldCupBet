<%@ Page Title="Join Universe" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Create.aspx.cs" Inherits="Universe_Create" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <% if (Context.User.Identity.IsAuthenticated)
       { %>
    <div style="text-align: center">
        <p>
            <label class="h3">Not available yet, please contact the administrator of the site.</label>
        </p>
    </div>
    <% }%>
</asp:Content>

