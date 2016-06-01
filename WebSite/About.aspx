<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="About.aspx.cs" Inherits="About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div style="position: absolute; left: 50%; margin-left: -138px; z-index: 100;top:45px">
        <input style="margin-top: 20px; margin-bottom: 20px; width: 250px;" type="button" id="download" value="Download Complete Rules" class="btn btn-primary btn-lg" />
    </div>
    <h3>Rules :</h3>
    <p>Every participant start the contest with 100 000 euros</p>
    <p>During all the competition you can buy or sell teams to the other participant</p>
    <p>When a team is eliminated you cannot trade it anymore</p>
    <p>At the end of the competition you receive/pay the following amount</p>

    <p>Rugby World Cup 2015</p>
    <table>
        <tbody style="margin-left: 50px">
            <tr>
                <td>Winner</td>
                <td>1000$</td>
            </tr>
            <tr>
                <td>Second</td>
                <td>750$</td>
            </tr>
            <tr>
                <td>Third</td>
                <td>500$</td>
            </tr>
            <tr>
                <td>Fourth</td>
                <td>450$</td>
            </tr>
            <tr>
                <td>Quarter Finalist</td>
                <td>200$</td>
            </tr>
            <tr>
                <td>Third place in group phase</td>
                <td>50$</td>
            </tr>
            <tr>
                <td>Fourth place in group phase</td>
                <td>10$</td>
            </tr>
        </tbody>
    </table>
    <p>Champions League 2015/2016</p>
    <table>
        <tbody style="margin-left: 50px">
            <tr>
                <td>Winner</td>
                <td>1000$</td>
            </tr>
            <tr>
                <td>Second</td>
                <td>750$</td>
            </tr>
            <tr>
                <td>Semi Finalist</td>
                <td>450$</td>
            </tr>
            <tr>
                <td>Quarter Finalist</td>
                <td>250$</td>
            </tr>
            <tr>
                <td>Height Finalist</td>
                <td>100$</td>
            </tr>
            <tr>
                <td>UEFA Qualification</td>
                <td>25$</td>
            </tr>
        </tbody>
    </table>

    <p>For example you decide to buy from John 5 New-Zealand at 650 euros</p>
    <p>John directly received 3250 (650 x 5) euros</p>
    <p>At the end of the world cup</p>
    <p>If the New-Zealand won the world cup John pay you 5000 (5 x 1000) euros (you made a +1750 euros PnL)</p>
    <p>If the New-Zealand is eliminated in quarter finals John pay you 1000 (5 x 200) euros (you made a -2250 euros PnL)</p>
</asp:Content>
