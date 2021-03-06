﻿<%@ Page Async="true" Title="Reset Password" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="ResetPassword.aspx.cs" Inherits="Account_ResetPassword" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="MainContent">
    <div style="text-align: center">
        <div class="col-md-4 col-md-offset-4">
            <div class="login-panel panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Reset Password</h3>
                </div>
                <div class="panel-body">
                    <form runat="server">
                        <fieldset>
                            <div class="form-horizontal">
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
                                <div class="form-horizontal">
                                    <asp:ValidationSummary runat="server" CssClass="text-danger" />
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="Email" CssClass="col-md-2 control-label">Email</asp:Label>
                                        <div class="col-md-10">
                                            <asp:TextBox runat="server" ID="Email" TextMode="Email" CssClass="form-control" />
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Email"
                                                CssClass="text-danger" ErrorMessage="The email field is required." />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="Password" CssClass="col-md-2 control-label">Password</asp:Label>
                                        <div class="col-md-10">
                                            <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Password"
                                                CssClass="text-danger" ErrorMessage="The password field is required." />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="PasswordConfirmation" CssClass="col-md-2 control-label">Confirm password</asp:Label>
                                        <div class="col-md-10">
                                            <asp:TextBox runat="server" ID="PasswordConfirmation" TextMode="Password" CssClass="form-control" />
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="PasswordConfirmation"
                                                CssClass="text-danger" Display="Dynamic" ErrorMessage="The confirm password field is required." />
                                            <asp:CompareValidator runat="server" ControlToCompare="Password" ControlToValidate="PasswordConfirmation"
                                                CssClass="text-danger" Display="Dynamic" ErrorMessage="The password and confirmation password do not match." />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="col-md-offset-2 col-md-10">
                                            <asp:Button ID="Button2" runat="server" OnClick="ChangePassword" Text="Change Password" CssClass="btn btn-primary" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </fieldset>
                    </form>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

