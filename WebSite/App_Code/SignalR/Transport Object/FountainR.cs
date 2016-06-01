using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Datas.Entities;

public class FountainR
{
    public double Long { get; private set; }
    public double Lat { get; private set; }
    public DateTime? Antoine { get; private set; }
    public DateTime? Camille { get; private set; }
    public DateTime? Loic { get; private set; }
    public DateTime? Xavier { get; private set; }
    public DateTime? Rafaela { get; private set; }
    public List<string> Images { get; private set; }
    public int Found { get; private set; }

	public FountainR(Fountain fountain, DateTime referenceDate)
	{
	    Long = fountain.Long;
	    Lat = fountain.Lat;
        Antoine = fountain.Antoine;
        Camille = fountain.Camille;
        Loic = fountain.Loic;
        Xavier = fountain.Xavier;
        Rafaela = fountain.Rafaela;
	    var zurichImages = "/Zurich/Images";
	    var path = HttpContext.Current.Server.MapPath("~" + zurichImages);
        Images = Directory.EnumerateFiles(path).Where(x => x.Contains(string.Format("\\Fountain-{0},{1}-", String.Format("{0:0.00000000}", Long).Replace(',', '.'), String.Format("{0:0.00000000}", Lat).Replace(',', '.')))).Select(
            x =>
            {
                var strings = x.Split('\\');
                return zurichImages+ '/' + strings.Last();
            }).ToList();
        int value = 0;
        if (Antoine.HasValue && Antoine.Value > referenceDate)
            value += 1;
        if (Camille.HasValue && Camille.Value > referenceDate)
            value += 2;
        if (Loic.HasValue && Loic.Value > referenceDate)
            value += 4;
        if (Xavier.HasValue && Xavier.Value > referenceDate)
            value += 8;
        if (Rafaela.HasValue && Rafaela.Value > referenceDate)
            value += 16;

        Found = value;
	}
}