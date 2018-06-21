<%@ Page Title="Mon petit prono - sphere" Language="C#" AutoEventWireup="true" CodeFile="mpp.aspx.cs" Inherits="Mpp" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8"/>
    <meta name="description" content="Financial Stock Exchange for sport competition">
    <meta name="keywords" content="Sport, bet, finance,fun">
    <meta name="author" content="Xavier Desoindre">
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <meta http-equiv="Refresh" content="180">
    <script src="Scripts/jquery-2.2.3.min.js"></script>
    <style type="text/css">
        .table {
            width: 100%;
            height: 100%;
            max-width: 100%;
            background-color: transparent;
        }

        .table th, .table td {
            padding: 0.75rem;
            border-top: 1px solid #dee2e6;
        }

        .table thead th {
            vertical-align: bottom;
            border-bottom: 2px solid #dee2e6;
        }

        .table tbody + tbody { border-top: 2px solid #dee2e6; }

        .table .table { background-color: #fff; }

        .table-sm th, .table-sm td { padding: 0.3rem; }

        .table-bordered { border: 1px solid #dee2e6; }

        .table-bordered th, .table-bordered td { border: 1px solid #dee2e6; }

        .table-bordered thead th, .table-bordered thead td { border-bottom-width: 2px; }

        .table-borderless th, .table-borderless td, .table-borderless thead th, .table-borderless tbody + tbody { border: 0; }

        .table-striped tbody tr:nth-of-type(odd) { background-color: rgba(0, 0, 0, 0.05); }

        .table-hover tbody tr:hover { background-color: rgba(0, 0, 0, 0.075); }

        .table-primary, .table-primary > th, .table-primary > td { background-color: #cacff1; }

        .table-hover .table-primary:hover { background-color: #b6bdec; }

        .table-hover .table-primary:hover > td, .table-hover .table-primary:hover > th { background-color: #b6bdec; }

        .table-secondary, .table-secondary > th, .table-secondary > td { background-color: #d6d8db; }

        .table-hover .table-secondary:hover { background-color: #c8cbcf; }

        .table-hover .table-secondary:hover > td, .table-hover .table-secondary:hover > th { background-color: #c8cbcf; }

        .table-success, .table-success > th, .table-success > td { background-color: #cbf0cb; }

        .table-hover .table-success:hover { background-color: #b7eab7; }

        .table-hover .table-success:hover > td, .table-hover .table-success:hover > th { background-color: #b7eab7; }

        .table-info, .table-info > th, .table-info > td { background-color: #d4d8f3; }

        .table-hover .table-info:hover { background-color: #c0c6ed; }

        .table-hover .table-info:hover > td, .table-hover .table-info:hover > th { background-color: #c0c6ed; }

        .table-warning, .table-warning > th, .table-warning > td { background-color: #ffeeba; }

        .table-hover .table-warning:hover { background-color: #ffe8a1; }

        .table-hover .table-warning:hover > td, .table-hover .table-warning:hover > th { background-color: #ffe8a1; }

        .table-danger, .table-danger > th, .table-danger > td { background-color: #f9cdca; }

        .table-hover .table-danger:hover { background-color: #f6b7b3; }

        .table-hover .table-danger:hover > td, .table-hover .table-danger:hover > th { background-color: #f6b7b3; }

        .table-light, .table-light > th, .table-light > td { background-color: #fcfcfd; }

        .table-hover .table-light:hover { background-color: #ededf3; }

        .table-hover .table-light:hover > td, .table-hover .table-light:hover > th { background-color: #ededf3; }

        .table-dark, .table-dark > th, .table-dark > td { background-color: #c6c6c6; }

        .table-hover .table-dark:hover { background-color: #b9b9b9; }

        .table-hover .table-dark:hover > td, .table-hover .table-dark:hover > th { background-color: #b9b9b9; }

        .table-active, .table-active > th, .table-active > td { background-color: rgba(0, 0, 0, 0.075); }

        .table-hover .table-active:hover { background-color: rgba(0, 0, 0, 0.075); }

        .table-hover .table-active:hover > td, .table-hover .table-active:hover > th { background-color: rgba(0, 0, 0, 0.075); }

        .table .thead-dark th {
            color: #fff;
            background-color: #212529;
            border-color: #32383e;
        }

        .table .thead-light th {
            color: #495057;
            background-color: #e9ecef;
            border-color: #dee2e6;
        }

        .table-dark {
            color: #fff;
            background-color: #212529;
        }

        .table-dark th, .table-dark td, .table-dark thead th { border-color: #32383e; }

        .table-dark.table-bordered { border: 0; }

        .table-dark.table-striped tbody tr:nth-of-type(odd) { background-color: rgba(255, 255, 255, 0.05); }

        .table-dark.table-hover tbody tr:hover { background-color: rgba(255, 255, 255, 0.075); }
    </style>
</head>
<body>
<h1 style="height: 10%; text-align: center;">Sphere - Mon petit prono !</h1>
<div style="position: absolute; left: 0; width: 40%; height: 85%">
    <table class="table text-center">
        <thead>
        <tr>
            <th>Name</th>
            <th>Points</th>
            <th>Bons pronos</th>
            <th>Exacts</th>
        </tr>
        </thead>
        <%
            foreach (var user in LoadRanking())
            { %>
            <tr>
                <th><%= user.firstname.ToString() + " " + user.lastname.ToString() %></th>
                <th><%= user.totalScore.ToString() %></th>
                <th><%= user.goodForecasts.ToString() %></th>
                <th><%= user.exactForecasts.ToString() %></th>
            </tr>
        <% }
        %>
    </table>
</div>
<div style="position: absolute; right: 0; width: 40%; height: 85%">
    <% LoadForecast(); %>
    <table class="table text-center">
        <thead>
        <tr>
            <th>
                <% if (LastGame != null)
                   {
                       if (LastGame.past.Value)
                       { %>
                        <span style="height: 15px; width: 15px; background-color: #f45342; border-radius: 50%; display: inline-block;"></span>
                    <% }
                       else
                       { %> <span style="height: 15px; width: 15px; background-color: #19d31c; border-radius: 50%; display: inline-block;"></span>
                    <% } %>
                    <%= Teams[LastGame.home.ToString()] %>
                </th>
            <th><%= LastGame.score.home %></th>
            <th><%= LastGame.score.away %></th>
            <th><%= Teams[LastGame.away.ToString()] %></th>
        </tr>
        </thead>
        <% foreach (var prono in LoadProno())
           {
        %>
            <tr>
                <th><%= prono.Key %></th>
                <th><%= prono.Value.home.Value == null ? "X" : prono.Value.home.ToString() %></th>
                <th><%= prono.Value.away.Value == null ? "X" : prono.Value.away.ToString() %></th>
                <th>
                    <%= prono.Value.away.Value == null ? "X" :
                            prono.Value.away.Value > prono.Value.home.Value ? Teams[LastGame.away.ToString()] + " (" + LastGame.quotation.away + ")" :
                                prono.Value.away.Value < prono.Value.home.Value ? Teams[LastGame.home.ToString()] + " (" + LastGame.quotation.home + ")" :
                                    "Nul" + " (" + LastGame.quotation.N + ")"
    %>
                </th>
            </tr>
        <%
           }
                   } %>
    </table>
</div>
</body>
</html>