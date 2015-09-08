displayFlood = false;
displayWind = false;

function init()
{
$(function() {
    	$("#Menu").tabs();
    	$("#RiskSummaryAccordion").tabs({
      		collapsible: true
    		});
    	$("#PoliceSummaryAccordion").tabs({
      		collapsible: true
    		});
   	   $( "#FloodButton" )
      .button()
      .click(function( event ) {
        event.preventDefault();
        if (displayWind)
    	{
    		map.removeLayer(geojsonWind);
    		displayWind = false;
        }
        if (displayFlood)
        	map.removeLayer(geojsonFlood);
        else
        	geojsonFlood.addTo(map);
        
        displayFlood = !displayFlood;
      }); 
      $("#UploadButton")
      .click(function( event ) {
        event.preventDefault();
        $('#file').trigger('click');
        });

    document.getElementById('file').addEventListener('change', readFile, false);
      $("#EQButton")
      .button()
      .click(function( event ) {
        event.preventDefault();
        });
   	   $("#WindButton")
      .button()
      .click(function( event ) {
        event.preventDefault();
        if (displayFlood)
    	{
    		map.removeLayer(geojsonFlood);
    		displayFlood = false;
        }
        if (displayWind)
        	map.removeLayer(geojsonWind);
        else
        	geojsonWind.addTo(map);
        
        displayWind = !displayWind;
      });
  	});
 } 