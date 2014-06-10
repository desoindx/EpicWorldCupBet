<%@ Page Title="About" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="About.aspx.cs" Inherits="About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <h3>Rules :</h3>
    <p>Every participant start the contest with 10000 euros</p>
    <p>During all the world cup you can buy or sell teams to the other participant</p>
    <p>When a team is eliminated you cannot trade it anymore</p>
    <p>At the end of the competition you receive/pay the following amount</p>
    <p>1000 euros for the winner</p>
    <p>750 euros for the second</p>
    <p>500 euros for the third</p>
    <p>450 euros for the fourth</p>
    <p>250 euros if the team was eliminated in 1/4th finals</p>
    <p>100 euros if the team was eliminated in 1/8th finals</p>
    <p>For example you decide to buy from John 5 Brazil at 65 euros</p>
    <p>John directly received 65 euros</p>
    <p>At the end of the world cup</p>
    <p>If the Brazil won the world cup John pay you 100 euros (you made a +35 euros PnL)</p>
    <p>If the Brazil is eliminated in quarter finals John pay you 25 euros (you made a -40 euros PnL)</p>
    <p>For the final payoff i'll use the following calculator (depending on the number of participant)</p>
    <a href="http://www.diypokertour.com/pokertour.aspx?tabindex=6&tabid=136">http://www.diypokertour.com/pokertour.aspx?tabindex=6&tabid=136</a>
</asp:Content>
