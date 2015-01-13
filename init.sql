insert into Competitions values ('Cup', '2015-01-01', '2015-02-01', 'Football World Cup 2014') 
insert into Universes values ('Test', 'Xavier')
insert into UniverseAvailables values (1, 'Xavier')
insert into UniverseCompetitions values (1,1)
insert into Orders ([User], Date, Team, Quantity, Status, Price, Side, IdUniverseCompetition) select [User], Date, Team, Quantity, Status, Price, Side, 1 from [dbo].[Order];
insert into Trades (Buyer, Seller, Date, Team, Quantity, Price, IdUniverseCompetition) select Buyer, Seller, Date, Team, Quantity, Price, 1 from [dbo].[Trade];
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