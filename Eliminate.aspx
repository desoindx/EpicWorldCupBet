<%@ Page Title="Eliminate" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Eliminate.aspx.cs" Inherits="Eliminate" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="Content/slick.grid.css" type="text/css" />
    <script src="JavaScript/SignalRPricer.js"></script>

    <div style="height: 600px;">
        <table>
            <tbody style="margin-left: 50px">
                <tr>
                    <td>
                    <tr>
                        <td>
                            <label>Password</label>
                        </td>
                        <td>
                            <input style="margin-left: 35px" type="password" id='Password' value='1'/>
                        </td>
                        <td>
                            <label>Price</label>
                        </td>
                    </tr>
                <tr>
                    <td>
                        <label>Eliminate</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='text' id='Team' value='team'/>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='number' id='Value' value='value'/>
                    </td>
                </tr>
            </tbody>
        </table>
        <p>
            <input style="margin-left: 10px" type="button" id="Eliminate" value="Eliminate" />
        </p>
    </div>
</asp:Content>
