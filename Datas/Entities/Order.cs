//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Datas.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        public int Id { get; set; }
        public string User { get; set; }
        public System.DateTime Date { get; set; }
        public string Team { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public int Price { get; set; }
        public string Side { get; set; }
        public int IdUniverseCompetition { get; set; }
    }
}