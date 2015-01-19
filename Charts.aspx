<%@ Page Title="Histo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Charts.aspx.cs" Inherits="Charts" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <link rel="stylesheet" type="text/css" href="Scripts/Chart/css/jquery.jqChart.css" />
    <link rel="stylesheet" type="text/css" href="Scripts/Chart/css/jquery.jqRangeSlider.css" />
    <link rel="stylesheet" type="text/css" href="Scripts/Chart/themes/smoothness/jquery-ui-1.10.4.css" />
    <script src="Scripts/Chart/js/jquery.mousewheel.js" type="text/javascript"></script>
    <script src="Scripts/Chart/js/jquery.jqChart.min.js" type="text/javascript"></script>
    <script src="Scripts/Chart/js/jquery.jqRangeSlider.min.js" type="text/javascript"></script>

    <link rel="stylesheet" href="Content/slick.grid.css" type="text/css" />
    <link href="Scripts/listbox.css" rel="stylesheet">
    <script src="Scripts/listbox.js"></script>
    <script src="JavaScript/Charts.js"></script>
    <script src="JavaScript/SignalRCharts.js"></script>

    <div id="divCountry">
        <select id="country">
            <option>Algeria</option>
            <option>Argentina</option>
            <option>Australia</option>
            <option>Belgium</option>
            <option>Bosnia And Herzgovina</option>
            <option>Brazil</option>
            <option>Cameroon</option>
            <option>Chile</option>
            <option>Colombia</option>
            <option>Costa Rica</option>
            <option>Croatia</option>
            <option>Ecuador</option>
            <option>England</option>
            <option>France</option>
            <option>Germany</option>
            <option>Ghana</option>
            <option>Greece</option>
            <option>Honduras</option>
            <option>Iran</option>
            <option>Italy</option>
            <option>Ivory Coast</option>
            <option>Japan</option>
            <option>Mexico</option>
            <option>Netherlands</option>
            <option>Nigeria</option>
            <option>Portugal</option>
            <option>Russia</option>
            <option>South Korea</option>
            <option>Spain</option>
            <option>Switzerland</option>
            <option>United States</option>
            <option>Uruguay</option>
        </select>
    </div>

    <h3>Price Index<span style="float: right;
            font-weight: bolder; width: 302px; margin-left: 1px;"
            id="date"></span>
        <b style="margin-left:50px">Total traded volume:</b><span id="totalVolume" style="display: inline-block; width: 45px"> </span></h3>
    <div style="margin-left: 10px">
        <b>Open:</b><span id="open" style="display: inline-block; width: 45px;"> </span>
        <b>High:</b><span id="high" style="display: inline-block; width: 45px;"> </span>
        <b>Low:</b><span id="low" style="display: inline-block; width: 45px;"> </span><b>Close:</b><span
            id="close" style="display: inline-block; width: 45px;"></span><b>Volume: </b>
        <span id="volume" style="display: inline-block; width: 45px;"></span>
    </div>
    <div>
        <div>
            <div id="jqChart" style="width: 1086px; height: 500px;">
            </div>
        </div>
        <div>
            <div id="jqChartVolume" style="width: 1085px; height: 500px;">
            </div>
        </div>
    </div>
</asp:Content>
