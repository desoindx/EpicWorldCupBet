//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Datas.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Message
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Message1 { get; set; }
        public System.DateTime Received { get; set; }
        public Nullable<System.DateTime> Read { get; set; }
    }
}