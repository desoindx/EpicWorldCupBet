﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccesLayer
{
    using System;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.Entity.Core.EntityClient;
    using System.Data.Entity.Infrastructure;
    using System.Data.SqlClient;
    using System.Linq;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
            var ensureDLLIsCopied =
                    System.Data.Entity.SqlServer.SqlProviderServices.Instance;   
        }
    
        private static string GetConnectionString()
        {
            var uriString = ConfigurationManager.AppSettings["SQLSERVER_URI"];
            if (uriString == null)
                uriString = "sqlserver://nkffzcwhcdtslhqf:BQWXgX4RBNdKrVshFRhpFLxtaAW7bmuBNTUFK2EPCDSpcrbsWNkSkwPXrBWNb6Qq@d533926a-738d-4224-81bd-a34000eb626c.sqlserver.sequelizer.com/dbd533926a738d422481bda34000eb626c";
            var uri = new Uri(uriString);

            // Initialize the connection string builder for the
            // underlying provider.
            SqlConnectionStringBuilder sqlBuilder =
            new SqlConnectionStringBuilder();

            // Set the properties for the data source.
            sqlBuilder.DataSource = uri.Host;
            sqlBuilder.InitialCatalog = uri.AbsolutePath.Trim('/');
            sqlBuilder.UserID =uri.UserInfo.Split(':').First();
            sqlBuilder.Password = uri.UserInfo.Split(':').Last();
            sqlBuilder.PersistSecurityInfo = true;
            sqlBuilder.MultipleActiveResultSets = true;
            sqlBuilder.ApplicationName = "EntityFramework";

            // Build the SqlConnection connection string.
            string providerString = sqlBuilder.ToString();

            // Initialize the EntityConnectionStringBuilder.
            EntityConnectionStringBuilder entityBuilder =
            new EntityConnectionStringBuilder();

            //Set the provider name.
            entityBuilder.Provider = "System.Data.SqlClient";

            // Set the provider-specific connection string.
            entityBuilder.ProviderConnectionString = providerString;

            // Set the Metadata location.
            entityBuilder.Metadata = @"res://*/App_Code.DataAccesLayer.BetModel.csdl|res://*/App_Code.DataAccesLayer.BetModel.ssdl|res://*/App_Code.DataAccesLayer.BetModel.msl";
            return entityBuilder.ToString();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Trade> Trades { get; set; }
    }
}
