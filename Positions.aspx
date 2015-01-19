<%@ Page Title="Position" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Positions.aspx.cs" Inherits="Positions" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <% if (Context.User.Identity.IsAuthenticated)
       { %>
    <link rel="stylesheet" href="Content/slick.grid.css" type="text/css" />
    <link rel="stylesheet" href="Content/slick-default-theme.css" type="text/css" />
    <script src="Scripts/SlickGrid/Plugins/slick.cellrangedecorator.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellrangeselector.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellnewcopymanager.js"></script>
    <script src="Scripts/SlickGrid/Plugins/slick.cellselectionmodel.js"></script>
    <script src="JavaScript/Positions.js"></script>
    <% if (Master.UniverseHasMultipleCompetition())
       {%>
    <div role="tabpanel">
        <ul class="nav nav-tabs" id="CompetitionTab" role="tablist" style="width: 600px;">
            <li role="presentation" class="dropdown active">
                <a id="CompetitionDropDownButton" class="dropdown-toggle active" data-toggle="dropdown" href="#" role="button" aria-expanded="false" aria-controls="CompetitionTabMenu">Dropdown <span class="caret"></span>
                </a>
                <ul class="dropdown-menu" role="menu" id="CompetitionTabMenu">
                    <% bool hasActive = false;
                       foreach (var competition in Master.UniverseCompetitions)
                       {%>
                    <li role="presentation" class="<%: hasActive ? "" : "active" %>">
                        <a title="<%: competition.Id %>" href="#<%: "Div-" + competition.Id %>" aria-controls="<%: "Div-" + competition.Id %>" role="tab" data-toggle="tab">
                            <%: competition.Name %>
                        </a>
                    </li>
                    <% if (!hasActive)
                       {%>
                    <script type='text/javascript'> 
                        $("#CompetitionDropDownButton")[0].innerHTML = '<%: competition.Name %>' + " <span class='caret'></span>";
                        currentCompetitionId = <%: competition.Id %>;
                    </script>
                    <%}
                       hasActive = true;
                       }%>
                </ul>
            </li>
        </ul>
        <div id="CompetitionTabContent" class="tab-content">
            <% hasActive = false;
               foreach (var competition in Master.UniverseCompetitions)
               {%>
            <div role="tabpanel" class="tab-pane <%: hasActive ? "" : "active" %>" id='<%: "Div-" + competition.Id %>'>
                <div style="width: 350px; height: 900px;" id='<%: "PositionsDiv-" + competition.Id %>'>
                </div>
                <div style="overflow: auto;margin-top: -890px;margin-left: 500px;width: 600px; height: 825px;" id='<%: "TradesDiv-" + competition.Id %>'>
                </div>
            </div>
            <%
                   hasActive = true;
               }%>
        </div>
    </div>
    <script type='text/javascript'> 
        <% foreach (var competition in Master.UniverseCompetitions)
           {%>
        drawTrades(<%:competition.Id%>,<%: GetTrades(competition.Id)%>);
        drawPositionGrid(<%:competition.Id%>,<%: GetPositions(competition.Id)%>);
        <%}%>
    </script>
    <% }
       else
       { %>
    <label class="h3" style="margin-top: 0;"><%: Master.GetUniverseCompetition() %></label>
    <div style="width: 600px; height: 900px;" id='<%: "Div-" + Master.GetCompetitionId() %>'>
        <div style="width: 350px; height: 900px;" id='<%: "PositionsDiv-" + Master.GetCompetitionId() %>'>
        </div>
        <div style="overflow: auto;margin-top: -890px;margin-left: 500px;width: 600px; height: 825px;" id='<%: "TradesDiv-" + Master.GetCompetitionId() %>'>
        </div>
    </div>
    <script type='text/javascript'>
        drawTrades(<%: Master.GetCompetitionId()%>,<%: GetTrades(Master.GetCompetitionId())%>);
        drawPositionGrid(<%: Master.GetCompetitionId()%>,<%: GetPositions(Master.GetCompetitionId())%>);
    </script>
    <% } %>
    <% } %>
</asp:Content>
