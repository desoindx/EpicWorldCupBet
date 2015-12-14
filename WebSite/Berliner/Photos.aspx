<%@ Page Title="Photos" Language="C#" MasterPageFile="~/Berliner/Site.Master" AutoEventWireup="true" CodeFile="Photos.aspx.cs" Inherits="Photos" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h1><%:GetTitle() %></h1>
    <div id="blueimp-gallery" class="blueimp-gallery">
        <!-- The container for the modal slides -->
        <div class="slides"></div>
        <!-- Controls for the borderless lightbox -->
        <h3 class="title"></h3>
        <a class="prev">‹</a>
        <a class="next">›</a>
        <a class="close">×</a>
        <a class="play-pause"></a>
        <ol class="indicator"></ol>
        <!-- The modal dialog, which will be used to wrap the lightbox content -->
        <div class="modal fade">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" aria-hidden="true">&times;</button>
                        <h4 class="modal-title"></h4>
                    </div>
                    <div class="modal-body next"></div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default pull-left prev">
                            <i class="glyphicon glyphicon-chevron-left"></i>
                            Previous
                        </button>
                        <button type="button" class="btn btn-primary next">
                            Next
                        <i class="glyphicon glyphicon-chevron-right"></i>
                        </button>
                        <h4 class="modal-footer-title" style="text-align: center"></h4>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="links" style="margin-top: 20px">
        <% foreach (var photo in GetPhotos())
           {
        %>
        <a href="<%: photo.Path%>" title="<%: photo.Title%>" data-text="<%: photo.Comment%>" data-gallery>
            <img src="<%: photo.Thumbnail%>" alt="<%: photo.Title%>">
        </a>
        <% } %>
    </div>
    <script>
        window.onresize = function() {
            var w = $("#page-wrapper").width();
            if (w > 1300) {
                $("#links").width(1300);
            }
            else if (w > 850) {
                $("#links").width(940);
            }
            else if (w > 500) {
                $("#links").width(510);
            }
        }

        var w = $("#page-wrapper").width();
        if (w > 1300) {
            $("#links").width(1300);
        }
        else if (w > 850) {
            $("#links").width(940);
        }
        else if (w > 500) {
            $("#links").width(510);
        }
    </script>
</asp:Content>
