<%@ Page Title="Pricer" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Pricer.aspx.cs" Inherits="Pricer" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <link rel="stylesheet" href="Content/slick.grid.css" type="text/css" />
    <script src="Scripts/jquery-1.7.2.min.js"></script>
    <script src="Scripts/jquery.event.drag.js"></script>
    <script src="Scripts/SlickGrid/slick.core.js"></script>
    <script src="Scripts/SlickGrid/slick.grid.js"></script>
    <script type="text/javascript" src="Scripts/jquery.signalR-2.0.3.js"></script>
    <script type="text/javascript" src="../signalr/hubs"></script>
    <script src="JavaScript/SignalRPricer.js"></script>

    <div style="height: 600px;">
        <table>
            <tbody style="margin-left: 50px">
                <tr>
                    <td>
                    <tr>
                        <td>
                            <label>Password</label>
                        </td>
                        <td>
                            <input style="margin-left: 35px" type="password" id='Password' />
                        </td>
                        <td>
                            <label>Price</label>
                        </td>
                    </tr>
                <tr>
                    <td>
                        <label>Brazil</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Brazil' />
                    </td>
                    <td>
                        <label id='BrazilPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Cameroon</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Cameroon' />
                    </td>
                    <td>
                        <label id='CameroonPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Croatia</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Croatia' />
                    </td>
                    <td>
                        <label id='CroatiaPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Mexico</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Mexico' />
                    </td>
                    <td>
                        <label id='MexicoPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Australia</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Australia' />
                    </td>
                    <td>
                        <label id='AustraliaPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Chile</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Chile' />
                    </td>
                    <td>
                        <label id='ChilePrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Netherlands</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Netherlands' />
                    </td>
                    <td>
                        <label id='NetherlandsPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Spain</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Spain' />
                    </td>
                    <td>
                        <label id='SpainPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Colombia</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Colombia' />
                    </td>
                    <td>
                        <label id='ColombiaPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Greece</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Greece' />
                    </td>
                    <td>
                        <label id='GreecePrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Ivory Coast</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='IvoryCoast' />
                    </td>
                    <td>
                        <label id='IvoryCoastPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Japan</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Japan' />
                    </td>
                    <td>
                        <label id='JapanPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Costa Rica</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='CostaRica' />
                    </td>
                    <td>
                        <label id='CostaRicaPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>England</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='England' />
                    </td>
                    <td>
                        <label id='EnglandPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Italy</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Italy' />
                    </td>
                    <td>
                        <label id='ItalyPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Uruguay</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Uruguay' />
                    </td>
                    <td>
                        <label id='UruguayPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Ecuador</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Ecuador' />
                    </td>
                    <td>
                        <label id='EcuadorPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>France</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='France' />
                    </td>
                    <td>
                        <label id='FrancePrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Honduras</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Honduras' />
                    </td>
                    <td>
                        <label id='HondurasPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Switzerland</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Switzerland' />
                    </td>
                    <td>
                        <label id='SwitzerlandPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Argentina</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Argentina' />
                    </td>
                    <td>
                        <label id='ArgentinaPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Bosnia And Herzgovina</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='BosniaAndHerzgovina' />
                    </td>
                    <td>
                        <label id='BosniaAndHerzgovinaPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Iran</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Iran' />
                    </td>
                    <td>
                        <label id='IranPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Nigeria</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Nigeria' />
                    </td>
                    <td>
                        <label id='NigeriaPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Germany</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Germany' />
                    </td>
                    <td>
                        <label id='GermanyPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Ghana</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Ghana' />
                    </td>
                    <td>
                        <label id='GhanaPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Portugal</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Portugal' />
                    </td>
                    <td>
                        <label id='PortugalPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>United States</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='UnitedStates' />
                    </td>
                    <td>
                        <label id='UnitedStatesPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Algeria</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Algeria' />
                    </td>
                    <td>
                        <label id='AlgeriaPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Belgium</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Belgium' />
                    </td>
                    <td>
                        <label id='BelgiumPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>Russia</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='Russia' />
                    </td>
                    <td>
                        <label id='RussiaPrice'></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label>South Korea</label>
                    </td>
                    <td>
                        <input style="margin-left: 35px" type='textbox' id='SouthKorea' />
                    </td>
                    <td>
                        <label id='SouthKoreaPrice'></label>
                    </td>
                </tr>
            </tbody>
        </table>
        <p>
            <input style="margin-left: 10px" type="button" id="Price" value="Price" />
        </p>
    </div>
</asp:Content>
