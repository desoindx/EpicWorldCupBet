<%@ Page Title="Join Universe" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Join.aspx.cs" Inherits="Universe_Join" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <% if (Context.User.Identity.IsAuthenticated)
       { %>
    <div style="text-align: center">
        <div class="col-md-4 col-md-offset-4">
            <div class="login-panel panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Join Universe</h3>
                </div>
                <div class="panel-body">
                    <form runat="server">
                        <fieldset>
                            <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                                <p class="text-danger">
                                    <asp:Literal runat="server" ID="FailureText" />
                                </p>
                            </asp:PlaceHolder>
                            <div class="form-group">
                                <asp:TextBox runat="server" ID="UniverseId" CssClass="form-control" />
                            </div>
                            <div class="form-group">
                                <asp:TextBox runat="server" ID="Password" CssClass="form-control" />
                            </div>
                            <!-- Change this to a button or input when using this as a form -->
                            <asp:Button runat="server" OnClick="JoinUniverse" Text="Join" CssClass="btn btn-lg btn-success btn-block" />
                        </fieldset>
                    </form>
                </div>
            </div>
        </div>
    </div>
    <% }%>
</asp:Content>

