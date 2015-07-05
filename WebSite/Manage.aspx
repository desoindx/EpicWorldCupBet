<%@ Page Title="Manage Account" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Manage.aspx.cs" Inherits="Account_Manage" %>

<%@ Register Src="~/OpenAuthProviders.ascx" TagPrefix="uc" TagName="OpenAuthProviders" %>

<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <form runat="server">
        <div>
            <asp:PlaceHolder runat="server" ID="successMessage" Visible="false" ViewStateMode="Disabled">
                <p class="text-success"><%: SuccessMessage %></p>
            </asp:PlaceHolder>

        </div>

        <div class="row">
            <div class="col-md-12">
                <section id="passwordForm">
                    <asp:PlaceHolder runat="server" ID="setPassword" Visible="false">
                        <p>
                            You do not have a local password for this site. Add a local
                        password so you can log in without an external login.
                        </p>
                        <div class="form-horizontal">
                            <h4>Set Password Form</h4>
                            <hr />
                            <asp:ValidationSummary ID="ValidationSummary" runat="server" ShowModelStateErrors="true" CssClass="text-danger" />
                            <div class="form-group">
                                <asp:Label ID="Label1" runat="server" AssociatedControlID="password" CssClass="col-md-2 control-label">Password</asp:Label>
                                <div class="col-md-10">
                                    <asp:TextBox runat="server" ID="password" TextMode="Password" CssClass="form-control" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="password"
                                        CssClass="text-danger" ErrorMessage="The password field is required."
                                        Display="Dynamic" ValidationGroup="SetPassword" />
                                    <asp:ModelErrorMessage ID="ModelErrorMessage1" runat="server" ModelStateKey="NewPassword" AssociatedControlID="password"
                                        CssClass="text-danger" SetFocusOnError="true" />
                                </div>
                            </div>

                            <div class="form-group">
                                <asp:Label ID="Label2" runat="server" AssociatedControlID="confirmPassword" CssClass="col-md-2 control-label">Confirm password</asp:Label>
                                <div class="col-md-10">
                                    <asp:TextBox runat="server" ID="confirmPassword" TextMode="Password" CssClass="form-control" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="confirmPassword"
                                        CssClass="text-danger" Display="Dynamic" ErrorMessage="The confirm password field is required."
                                        ValidationGroup="SetPassword" />
                                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToCompare="Password" ControlToValidate="confirmPassword"
                                        CssClass="text-danger" Display="Dynamic" ErrorMessage="The password and confirmation password do not match."
                                        ValidationGroup="SetPassword" />
                                </div>
                            </div>

                            <div class="form-group">
                                <div class="col-md-offset-2 col-md-10">
                                    <asp:Button ID="Button1" runat="server" Text="Set Password" ValidationGroup="SetPassword" OnClick="SetPassword_Click" CssClass="btn btn-default" />
                                </div>
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder runat="server" ID="changePasswordHolder" Visible="false">
                        <p></p>
                        <div class="form-horizontal">
                            <asp:ValidationSummary runat="server" ShowModelStateErrors="true" CssClass="text-danger" />
                            <div class="form-group">
                                <asp:Label runat="server" ID="CurrentPasswordLabel" AssociatedControlID="CurrentPassword" CssClass="col-md-2 control-label">Current password</asp:Label>
                                <div class="col-md-10">
                                    <asp:TextBox runat="server" ID="CurrentPassword" TextMode="Password" CssClass="form-control" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="CurrentPassword"
                                        CssClass="text-danger" ErrorMessage="The current password field is required."
                                        ValidationGroup="ChangePassword" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" ID="NewPasswordLabel" AssociatedControlID="NewPassword" CssClass="col-md-2 control-label">New password</asp:Label>
                                <div class="col-md-10">
                                    <asp:TextBox runat="server" ID="NewPassword" TextMode="Password" CssClass="form-control" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="NewPassword"
                                        CssClass="text-danger" ErrorMessage="The new password is required."
                                        ValidationGroup="ChangePassword" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" ID="ConfirmNewPasswordLabel" AssociatedControlID="ConfirmNewPassword" CssClass="col-md-2 control-label">Confirm new password</asp:Label>
                                <div class="col-md-10">
                                    <asp:TextBox runat="server" ID="ConfirmNewPassword" TextMode="Password" CssClass="form-control" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="ConfirmNewPassword"
                                        CssClass="text-danger" Display="Dynamic" ErrorMessage="Confirm new password is required."
                                        ValidationGroup="ChangePassword" />
                                    <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToCompare="NewPassword" ControlToValidate="ConfirmNewPassword"
                                        CssClass="text-danger" Display="Dynamic" ErrorMessage="The new password and confirmation password do not match."
                                        ValidationGroup="ChangePassword" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-md-offset-2 col-md-10">
                                    <asp:Button ID="Button2" runat="server" Text="Change password" OnClick="ChangePassword_Click" CssClass="btn btn-primary" ValidationGroup="ChangePassword" />
                                </div>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                </section>

            </div>
        </div>
    </form>
</asp:Content>
