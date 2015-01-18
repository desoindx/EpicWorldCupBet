﻿<%@ Page Title="Join Universe" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Join.aspx.cs" Inherits="Universe_Join" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div style="text-align: center">
        <p>
            <label class="h2">Join your friends universe !</label>
        </p>
        <div class="row">
            <div class="col-md-8">
                <section id="loginForm">
                    <div class="form-horizontal">
                        <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                            <p class="text-danger">
                                <asp:Literal runat="server" ID="FailureText" />
                            </p>
                        </asp:PlaceHolder>
                        <div class="form-group">
                            <asp:Label ID="Label1" runat="server" AssociatedControlID="UniverseId" CssClass="col-md-2 control-label">Universe Id</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="UniverseId" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="UniverseId"
                                    CssClass="text-danger" ErrorMessage="The universe id field is required." />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="Label2" runat="server" AssociatedControlID="Password" CssClass="col-md-2 control-label">Password</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Password" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Password" CssClass="text-danger" ErrorMessage="The password field is required." />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-md-offset-2 col-md-10">
                                <asp:Button ID="Button1" runat="server" OnClick="JoinUniverse" Text="Join Universe" CssClass="btn btn-default" />
                            </div>
                        </div>
                    </div>
                </section>
            </div>
        </div>
    </div>
</asp:Content>

