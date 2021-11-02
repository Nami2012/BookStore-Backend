CREATE DATABASE BookStoreDB
GO
USE BookStoreDB
GO
-- Category Table
CREATE TABLE Category
(
	CId INT PRIMARY KEY,
	CName NVARCHAR(50) NOT NULL,
	CDescription NVARCHAR(500),
	CImage NVARCHAR(500) NOT NULL,
	CStatus BIT NOT NULL,
	CPosition INT NOT NULL,
	CCreatedAt DATETIME NOT NULL
)
GO
-- Book Table
CREATE TABLE Book
(
	BId INT PRIMARY KEY,
	CId INT REFERENCES Category,
	BTitle NVARCHAR(500) NOT NULL,
	BISBN NVARCHAR(50) NOT NULL,
	BYEAR DATE NOT NULL,
	BPrice MONEY NOT NULL,
	BDescription NVARCHAR(500) NOT NULL,
	BPosition INT NOT NULL,
	BStatus BIT NOT NULL,
	BImage NVARCHAR(500) NOT NULL --- add created at
)
------- Insertion Commands -------
----insert into category-----
Go
insert into Category values
( 1,'Action and Adventure',
'Books that constantly have you on the edge of your seat with excitement',
'../../assets/images/CategoryImages/ActionAndAdventure.jpg',1,5,'01-11-2021'),
( 2,'Classics',
'Books that have continued to impact generations',
'../../assets/images/CategoryImages/Classics.jpg',1,4,'11-10-2021'),
( 3,'Graphic Novel',
'For those who like having  the dialogue presented in the tell-tale word balloons next to the respective characters.',
'../../assets/images/CategoryImages/GraphicNovel.jpg',1,3,'11-09-2021'),
( 4,'"Detective and Mystery',
'The plot always revolves around a crime of sorts that must be solved—or foiled—by the protagonists.',
'../../assets/images/CategoryImages/DetectiveAndMystery.jpg',1,2,'11-08-2021'),
( 5,'Science Fiction (Sci-Fi)',
'Heavy Themes of technology and future science',
'../../assets/images/CategoryImages/ScienceFiction.jpg',1,1,'11-07-2021')

Go
-------Insert into books------------
insert into Book values
(1,1,'Treasure Island','978-8172344764','2013',125,'Think of the high seas and of a buccaneer ship; of a wild seaman with a sea chest full of gold; of Long John Silver; of a buried treasure and of young Jim Hawkins, the boy with the treasure map the key to it all.',
1,1,'../../assets/BookDetails/ActionAndAdventure/TreasureIsland.jpg'),
(2,1,'The Count of Monte Cristo','978-1853267338','1997',250,'the victim of a miscarriage of justice, Edmund Dantes is fired by a desire for retribution and empowered by a stroke of Providence. In his campaign of vengeance, he becomes an anonymous agent of fate.',
2,1,'../../assets/BookDetails/ActionAndAdventure/CountOfMonteCristo.jpg'),
(3,1,'The Three Musketeers','978-8172344696','2013',270,'17th century France: Young D''Artagnan leaves his home and travels to Paris with dreams of joining The Musketeers of Guard—the glamourous and gallant group of men who guard Louis XIII, the King of France. There he meets Athos, Porthos and Aramis, the three best musketeers and inseparable friends who believe with all their heart in the words ''all for one, one for all.What the young and gallant D''Artagnan does not realise is that he has, inadvertently, landed himself in the very centre of one of the foulest conspiracies in monarchist France.',
3,1,'../../assets/BookDetails/ActionAndAdventure/TheThreeMusketeers.jpg'),
(4,1,'Twenty Thousand Leagues Under the Sea','978-1853260315','1992',300,'Professor Aronnax, his faithful servant, Conseil, and the Canadian harpooner, Ned Land, begin a hazardous voyage to rid the seas of a little-known and terrifying sea monster. However, the ''monster'' turns out to be a giant submarine, commanded by the mysterious Captain Nemo, by whom they are soon held captive.',
4,1,'../../assets/BookDetails/ActionAndAdventure/Twenty Thousand Leagues Under the Sea.jpg'),
(5,1,'The Call of the Wild','978-9380005454',2012,500,'This classic book brings out the true spirit of the Gold Rush days at the turn of the last century and portrays the brutality, kindness, love, and folly that Jack London experienced during his time in the far north.',
5,1,'../../assets/BookDetails/ActionAndAdventure/TheCallOfTheWild.jpg'),
(6,2,'Pride and Prejudice','978-8172344504','1813',150,'Pride and Prejudice follows the turbulent relationship between Elizabeth Bennet, the daughter of a country gentleman, and Fitzwilliam Darcy, a rich aristocratic landowner. They must overcome the titular sins of pride and prejudice in order to fall in love and marry.',
6,1,''),
(7,2,'The Catcher in the Rye','978-7543321724','1991',750,'The hero-narrator of The Catcher in the Rye is an ancient child of sixteen, a native New Yorker named Holden Caufield. Through circumstances that tend to preclude adult, secondhand description, he leaves his prep school in Pennsylvania and goes underground in New York City for three days.'
,7,1,''),
(8,2,'Wuthering Heights','978-8172344894','2013',200,'In this epic story of love, envy, betrayal and revenge, Heathcliff and Catherine come together in a romance that destroys them and those around them. Set in the lonely and bleak Yorkshire moors, this classic tale of thwarted passion begins when the new tenant of Thrushcross Grange, a Mr Lockwood, is forced to seek shelter for a night at Wuthering Heights. As the night passes, Lockwood learns of the tumultuous past of Wuthering Heights and of those connected with it.',
8,1,''),
(9,2,'The Great Gatsby','978-0743273565','2004',990,'The story of the mysteriously wealthy Jay Gatsby and his love for the beautiful Daisy Buchanan, of lavish parties on Long Island at a time when The New York Times noted “gin was the national drink and sex the national obsession,” it is an exquisitely crafted tale of America in the 1920s.'
,9,1,''),
(10,2,'Nineteen Eighty Four','978-0141036144','2008',300,'Nineteen Eighty-Four is George Orwell''s terrifying vision of a totalitarian future in which everything and everyone is slave to a tyrannical regime.',
10,1,'')

