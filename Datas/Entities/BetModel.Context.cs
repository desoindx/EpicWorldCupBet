﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.AspNet.Identity.EntityFramework;
using WorldCupBetting;

namespace Datas.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Money> Moneys { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Trade> Trades { get; set; }
        public virtual DbSet<Competition> Competitions { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<Universe> Universes { get; set; }
        public virtual DbSet<UniverseCompetition> UniverseCompetitions { get; set; }
        public virtual DbSet<UniverseAvailable> UniverseAvailables { get; set; }
        public virtual DbSet<Results> Results { get; set; }
        public virtual DbSet<ResultOverridedValue> ResultOverridedValues { get; set; }
        public virtual DbSet<CompetitionGame> CompetitionGames { get; set; }
        public virtual DbSet<CompetitionPrize> CompetitionPrizes { get; set; }
        public virtual DbSet<CompetitionResult> CompetitionResults { get; set; }
    }
}
