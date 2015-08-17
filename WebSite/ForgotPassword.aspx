<%@ Page Title="Forgot Password" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="~/ForgotPassword.aspx.cs" Inherits="Account_ForgotPassword" Async="true" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div class="col-md-4 col-md-offset-4">
        <div class="login-panel panel panel-default">
            <div class="panel-heading">
                <h3 class="panel-title">Enter Your Email</h3>
            </div>
            <div class="panel-body">
                <form runat="server">
                    <fieldset>
                        <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                            <p class="text-danger">
                                <asp:Literal runat="server" ID="FailureText" />
                            </p>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder runat="server" ID="SuccessMessage" Visible="false">
                            <p class="text-success">
                                <asp:Literal runat="server" ID="SuccessText" />
                            </p>
                        </asp:PlaceHolder>
                        <div class="form-group">
                            <input class="form-control" placeholder="Email" name="Email" autofocus>
                        </div>
                        <!-- Change this to a button or input when using this as a form -->
                        <asp:Button runat="server" OnClick="ResetPassword" Text="Reset Password" CssClass="btn btn-lg btn-success btn-block" />
                    </fieldset>
                </form>
            </div>
        </div>
    </div>
</asp:Content>
