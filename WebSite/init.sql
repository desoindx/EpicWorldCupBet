insert into Competitions values ('Cup', '2015-01-01', '2015-02-01', 'Football World Cup 2014')
insert into Competitions values ('Cup', '2015-01-01', '2015-02-01', 'Football World Cup 2015')
insert into Universes values ('Test', 'Xavier', '123456')
insert into Universes values ('Compet De Ouf!', 'Bebeto69', '123456')
insert into UniverseAvailables values (1, 'Xavier')
insert into UniverseAvailables values (2, 'Xavier')
insert into UniverseAvailables values (2, 'Bebeto69')
insert into UniverseCompetitions values (1,1)
insert into UniverseCompetitions values (1,2)
insert into Teams (Name, IdCompetition) values ('Brazil', 1),
            ('Croatia', 1),
            ('Mexico', 1),
            ('Cameroon', 1),
            ('Australia', 1),
            ('Chile', 1),
            ('Netherlands', 1),
            ('Spain', 1),
            ('Colombia', 1),
            ('Greece', 1),
            ('Ivory Coast', 1),
            ('Japan', 1),
            ('Costa Rica', 1),
            ('England', 1),
            ('Italy', 1),
            ('Uruguay', 1),
            ('Ecuador', 1),
            ('France', 1),
            ('Honduras', 1),
            ('Switzerland', 1),
            ('Argentina', 1),
            ('Bosnia And Herzgovina', 1),
            ('Iran', 1),
            ('Nigeria', 1),
            ('Germany', 1),
            ('Ghana', 1),
            ('Portugal', 1),
            ('United States', 1),
            ('Algeria', 1),
            ('Belgium', 1),
            ('Russia', 1),
            ('South Korea', 1);
insert into Teams (Name, IdCompetition) values ('NewBrazil', 2),
            ('NewCroatia', 2),
            ('NewMexico', 2),
            ('NewCameroon', 2),
            ('NewAustralia', 2),
            ('NewChile', 2),
            ('NewNetherlands', 2),
            ('NewSpain', 2),
            ('NewColombia', 2),
            ('NewGreece', 2),
            ('NewIvory Coast', 2),
            ('NewJapan', 2),
            ('NewCosta Rica', 2),
            ('NewEngland', 2),
            ('NewItaly', 2),
            ('NewUruguay', 2),
            ('NewEcuador', 2),
            ('NewFrance', 2),
            ('NewHonduras', 2),
            ('NewSwitzerland', 2),
            ('NewArgentina', 2),
            ('NewBosnia And Herzgovina', 2),
            ('NewIran', 2),
            ('NewNigeria', 2),
            ('NewGermany', 2),
            ('NewGhana', 2),
            ('NewPortugal', 2),
            ('NewUnited States', 2),
            ('NewAlgeria', 2),
            ('NewBelgium', 2),
            ('NewRussia', 2),
            ('NewSouth Korea', 2);
insert into Orders ([User], Date, Team, Quantity, Status, Price, Side, IdUniverseCompetition) select [User], o.Date, t.Id, o.Quantity, o.Status, o.Price, o.Side, 1 from [dbo].[Order] o, Teams t where o.Team = t.Name;
insert into Trades (Buyer, Seller, Date, Team, Quantity, Price, IdUniverseCompetition) select Buyer, Seller, o.Date, t.Id, o.Quantity, o.Price, 1 from [dbo].[Trade] o, Teams t where o.Team = t.Name;
insert into Results values ('Winner', 1000),('Finalist', 750),('Third Place', 500), ('Semi Finalist', 450), ('Quarter Finalist', 250), ('Eight Finalist', 100), ('Sixteenth Finalist', 0)
INSERT INTO [dbo].[AspNetUsers] ([Id], [UserName], [PasswordHash], [SecurityStamp], [Email], [EmailConfirmed], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount]) VALUES (N'656ca35d-4002-4cbb-96f7-5c327ea2ed4f', N'Xavier', N'APh1j6ishLBkzYmoeWX3C+lXTtvMGFlrGGG1E+qxC8q6CwmCf+KCHntIl0jqN3wV6g==', N'e108308a-761d-4a84-9ee7-0a491f1861a7', NULL, 0, NULL, 0, 0, NULL, 0, 0)
INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'de25f5cb-a425-4718-b4d8-e4e88dcf9f23', N'Admin')
INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'656ca35d-4002-4cbb-96f7-5c327ea2ed4f', N'de25f5cb-a425-4718-b4d8-e4e88dcf9f23')
