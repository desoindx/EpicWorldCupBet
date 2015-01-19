﻿<%@ Page Title="Cache" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Caches.aspx.cs" Inherits="Caches" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script src="JavaScript/SignalRCaches.js"></script>

    <div style="height: 600px;">
        <table>
            <tbody style="margin-left: 50px">
                <% foreach (var cache in Enum.GetNames(typeof (SignalR.SQL.Caches)))
                   { %>
                <tr>
                    <td>
                        <input style="margin-left: 10px" type="button" value=<%: cache %>  onclick="{$.connection.Bet.server.clearCache('<%: cache %>');}"/>
                    </td>
                </tr>
                <% } %>
            </tbody>
        </table>
    </div>
</asp:Content>
