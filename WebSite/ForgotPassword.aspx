<%@ Page Title="Forgot Password" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="~/ForgotPassword.aspx.cs" Inherits="Account_ForgotPassword" Async="true" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div class="row">
        <div class="col-md-8">
            <section id="forgottPassword">
                <div>
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
                        <asp:Label ID="Label1" runat="server" AssociatedControlID="MailAdress" CssClass="col-md-2 control-label">Your Email</asp:Label>
                        <div class="col-md-10">
                            <asp:TextBox runat="server" ID="MailAdress" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="MailAdress"
                                CssClass="text-danger" ErrorMessage="The mail adress field is required." />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-md-offset-2 col-md-10">
                            <asp:Button ID="Button1" runat="server" OnClick="ResetPassword" Text="Reset" CssClass="btn btn-default" />
                        </div>
                    </div>
                </div>
            </section>
        </div>

        <div class="col-md-4">
        </div>
    </div>
</asp:Content>
