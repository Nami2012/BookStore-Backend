CREATE DATABASE BookStoreDB
GO
USE BookStoreDB
GO

---========================================================Section Category And Books===============================================================---------
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
	BAuthor NVARCHAR(100) NOT NULL,
	BISBN NVARCHAR(50) NOT NULL,
	BYEAR DATE NOT NULL,
	BPrice MONEY NOT NULL,
	BDescription NVARCHAR(1000) NOT NULL,
	BPosition INT NOT NULL,
	BStatus BIT NOT NULL,
	BImage NVARCHAR(500) NOT NULL
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
(1,1,'Treasure Island','Robert Louis Stevenson','978-8172344764','2013',125,'Think of the high seas and of a buccaneer ship; of a wild seaman with a sea chest full of gold; of Long John Silver; of a buried treasure and of young Jim Hawkins, the boy with the treasure map the key to it all.',
1,1,'../../assets/BookDetails/ActionAndAdventure/TreasureIsland.jpg'),
(2,1,'The Count of Monte Cristo','Alexandre Dumas','978-1853267338','1997',250,'the victim of a miscarriage of justice, Edmund Dantes is fired by a desire for retribution and empowered by a stroke of Providence. In his campaign of vengeance, he becomes an anonymous agent of fate.',
2,1,'../../assets/BookDetails/ActionAndAdventure/CountOfMonteCristo.jpg'),
(3,1,'The Three Musketeers','Alexandre Dumas','978-8172344696','2013',270,'17th century France: Young D''Artagnan leaves his home and travels to Paris with dreams of joining The Musketeers of Guard—the glamourous and gallant group of men who guard Louis XIII, the King of France. There he meets Athos, Porthos and Aramis, the three best musketeers and inseparable friends who believe with all their heart in the words ''all for one, one for all.What the young and gallant D''Artagnan does not realise is that he has, inadvertently, landed himself in the very centre of one of the foulest conspiracies in monarchist France.',
3,1,'../../assets/BookDetails/ActionAndAdventure/TheThreeMusketeers.jpg'),
(4,1,'Twenty Thousand Leagues Under the Sea','Jules Verne','978-1853260315','1992',300,'Professor Aronnax, his faithful servant, Conseil, and the Canadian harpooner, Ned Land, begin a hazardous voyage to rid the seas of a little-known and terrifying sea monster. However, the ''monster'' turns out to be a giant submarine, commanded by the mysterious Captain Nemo, by whom they are soon held captive.',
4,1,'../../assets/BookDetails/ActionAndAdventure/Twenty Thousand Leagues Under the Sea.jpg'),
(5,1,'The Call of the Wild','Jack London','978-9380005454','2012',500,'This classic book brings out the true spirit of the Gold Rush days at the turn of the last century and portrays the brutality, kindness, love, and folly that Jack London experienced during his time in the far north.',
5,1,'../../assets/BookDetails/ActionAndAdventure/TheCallOfTheWild.jpg'),
(6,2,'Pride and Prejudice','Jane Austen','978-8172344504','1813',150,'Pride and Prejudice follows the turbulent relationship between Elizabeth Bennet, the daughter of a country gentleman, and Fitzwilliam Darcy, a rich aristocratic landowner. They must overcome the titular sins of pride and prejudice in order to fall in love and marry.',
6,1,''),
(7,2,'The Catcher in the Rye','J. D. Salinger','978-7543321724','1991',750,'The hero-narrator of The Catcher in the Rye is an ancient child of sixteen, a native New Yorker named Holden Caufield. Through circumstances that tend to preclude adult, secondhand description, he leaves his prep school in Pennsylvania and goes underground in New York City for three days.'
,7,1,''),
(8,2,'Wuthering Heights','Emily Brontë','978-8172344894','2013',200,'In this epic story of love, envy, betrayal and revenge, Heathcliff and Catherine come together in a romance that destroys them and those around them. Set in the lonely and bleak Yorkshire moors, this classic tale of thwarted passion begins when the new tenant of Thrushcross Grange, a Mr Lockwood, is forced to seek shelter for a night at Wuthering Heights. As the night passes, Lockwood learns of the tumultuous past of Wuthering Heights and of those connected with it.',
8,1,''),
(9,2,'The Great Gatsby','F. Scott Fitzgerald','978-0743273565','2004',990,'The story of the mysteriously wealthy Jay Gatsby and his love for the beautiful Daisy Buchanan, of lavish parties on Long Island at a time when The New York Times noted “gin was the national drink and sex the national obsession,” it is an exquisitely crafted tale of America in the 1920s.'
,9,1,''),
(10,2,'Nineteen Eighty Four','George Orwell','978-0141036144','2008',300,'Nineteen Eighty-Four is George Orwell''s terrifying vision of a totalitarian future in which everything and everyone is slave to a tyrannical regime.',
10,1,''),
(11,3,'The Complete MAUS','Art Spiegelman','978-0141014081','2003',686,'Maus tells the story of Vladek Spiegelman, a Jewish survivor of Hitler''s Europe, and his son, a cartoonist coming to terms with his father''s story.',
11,1,''),
(12,3,'Jimmy Corrigan: The Smartest Kid on Earth','Chris Ware','978-0224063975','2003',661,'It is the tragic autobiography of an office dogsbody in Chicago who one day meets the father who abandoned him as a child. With a subtle, complex and moving story and the drawings that are as simple and original as they are strikingly beautiful, Jimmy Corrigan is a book unlike any other and certainly not to be missed.',
12,1,''),
(13,3,'Fun Home: A Family Tragicomic','Alison Bechdel','978-0544709041','2007',1200,'An unusual memoir done in the form of a graphic novel by a cult favorite comic artist offers a darkly funny family portrait that details her relationship with her father, a historic preservation expert dedicated to restoring the family''s Victorian home, funeral home director, high-school English teacher, and closeted homosexual.'
,13,1,''),
(14,3,'Persepolis: The Story of a Childhood','Marjane Satrapi','978-0375714573','2004',1322,'In powerful black-and-white comic strip images, Satrapi tells the story of her life in Tehran from ages six to fourteen, years that saw the overthrow of the Shah’s regime, the triumph of the Islamic Revolution, and the devastating effects of war with Iraq.',
14,1,''),
(15,3,'A Contract with God – And Other Tenement Stories','Will Eisner','978-0393609189','2017',1741,'A Contract with God begins with a gripping tale that mirrors the artist’s real-life tragedy, the death of his daughter. Frimme Hersh, a devout Jew, questions his relationship with God after the loss of his own beloved child.',
15,1,''),
(16,4,'Murder on the Orient Express','Agatha Christie','9780007527502','2017',150,'Isolated and with a killer in their midst, detective Hercule Poirot must identify the murderer – in case he or she decides to strike again.',
16,1,''),
(17,4,'The Hound of the Baskervilles','Arthur Conan Doyle','978-9350362990','2013',120,' tells the story of an attempted murder inspired by the legend of a fearsome, diabolical hound of supernatural origin. Sherlock Holmes and his companion Dr. Watson investigate the case.',
17,1,''),
(18,4,'The Big Sleep','Raymond Chandler','978-0394758282','1988',880,'A dying millionaire hires private eye Philip Marlowe to handle the blackmailer of one of his two troublesome daughters, and Marlowe finds himself involved with more than extortion. Kidnapping, pornography, seduction, and murder are just a few of the complications he gets caught up in.',
18,1,''),
(19,4,'The Moonstone','Wilkie Collins','978-9350333433','2010',855,'It catches one up and unfolds its amazing story through the recountings of its several narrators, all of them enticing and singular. Wilkie Collins''s spellbinding tale of romance, theft, and murder inspired a hugely popular genre–the detective mystery.'
,19,1,''),
(20,4,'The Mysterious Affair At Styles','Agatha Christie','978-9350260425','2000',500,'Introducing Hercule Poirot, the brilliant-and eccentric- detective who, at a friend’s request, steps out of retirement- and into the shadows of a classic mystery on the outskirts of Essex.',
20,1,''),
(21,5,'The Martian','Andy Weir','978-1785031137','2015',330,'Stranded on Mars, one astronaut fights to survive',21,1,''),
(22,5,'Fahrenheit 451','Ray Bradbury','978-1451673319','2012',562,'Throughout Fahrenheit 451, Ray Bradbury warns readers about potential issues arising from increased technology, censorship, prescription medication, consumer culture, politics, war, and reliance on mass media',
22,1,''),
(23,5,'The Left Hand of Darkness','Ursula K. Le Guin','978-0441478125','2015',450,'Embracing the aspects of psychology, society, and human emotion on an alien world, The Left Hand of Darkness stands as a landmark achievement in the annals of intellectual science fiction.',
23,1,''),
(24,5,'Brave New World','Aldous Huxley','978-0060850524','2006',870,'Aldous Huxley''s profoundly important classic of world literature, Brave New World is a searching vision of an unequal, technologically-advanced future where humans are genetically bred, socially',
24,1,''),
(25,5,'The Time Machine','H. G. Wells','978-8175992955','2015',400,'A compelling science fiction, the Time Machine is a first-hand account of a Time Traveler''s journey into the future. a pull of the lever and the machine sends him to the year 802,701, when humanity has split into two bizarre races—the ethereal Eloi and the subterranean Morlocks. Here, his machine is stolen and with the help of Weena, an Eloi he saved from drowning, the traveler is able to retrieve it.'
,25,1,'')

-----------------========================================= END OF SECTION : Category And Books=====================================----------------------------


------------------============================================Section :Accounts=================================================------------------------------
GO
-----DDL Commands----

--Admin Table
CREATE TABLE Admin
(
	Username NVARCHAR(50) PRIMARY KEY,
	Password NVARCHAR(100) NOT NULL, --- setting password same as username for now.
	Email NVARCHAR(100) NOT NULL 
)
GO


--User_credentials
CREATE TABLE User_Credentials
(
	UId INT PRIMARY KEY,
	Username NVARCHAR(100) NOT NULL,----Input Email as username
	Password NVARCHAR(100) NOT NULL,--- setting password same as username for now.
)
GO


--User_details
CREATE TABLE User_Account_Info
(
	UId INT PRIMARY KEY FOREIGN KEY REFERENCES User_Credentials,
	Name NVARCHAR(100),
	PhoneNo NVARCHAR(10),
	ShippingAddress NVARCHAR(100),
	ActiveStatus BIT NOT NULL
)
GO

------DML Commands---------
--admin
insert into admin values
('admin','admin','a@d.bookStore.com')
GO
---User
insert into User_Credentials values
(1,'martin@gmail.com','martin'),
(2,'johndoe@gmail.com','johndoe')
GO
--user details
insert into User_Account_Info values
(1,'Martin','9876543210','Kochi',1),
(2,'John Doe','9876654321','Mumbai',2)

----=====================================================================END OF SECTION: Accounts==========================================----------------------------


---========================================================================Section : Cart and Wishlist =========================================-------------------
GO
---DDL Commands----
--cart
CREATE TABLE Cart 
(
	UId INT FOREIGN KEY REFERENCES User_Credentials,
	BId INT FOREIGN KEY REFERENCES Book,
	Count INT,
	CONSTRAINT Pk_Cart PRIMARY KEY(UId, BId)
)
GO
--wishlist
CREATE TABLE Wishlist
(
	UId INT FOREIGN KEY REFERENCES User_Credentials,
	BId INT FOREIGN KEY REFERENCES Book,
	CONSTRAINT Pk_Wishlist PRIMARY KEY(UId, BId)
)
GO

---======================================END OF Section:Cart And Wishlist ========================================================---------------------

------====================================================SECTION: Coupon====================================================--------------

--DDL Commands
GO
--Coupons
CREATE TABLE Coupon 
(
	CouponId NVARCHAR(10) PRIMARY KEY,
	Discount DECIMAL NOT NULL,
	Status BIT NOT NULL
)
GO

--CouponValidation
CREATE TABLE Coupon_Validation
(
	CouponId NVARCHAR(10) FOREIGN KEY REFERENCES Coupon,
	UId INT FOREIGN KEY REFERENCES User_Credentials,
	CONSTRAINT Pk_Coupon_Validation PRIMARY KEY(CouponId, UId)
)
GO
---======================================END OF Section:Cart And Wishlist ========================================================---------------------

---======================================Section:Order ========================================================---------------------
GO
--DDL Commands
--OrderItems
CREATE TABLE OrderItems
(
	OrderId NVARCHAR(10) FOREIGN KEY REFERENCES OrderInvoiceDetails,
	BId INT FOREIGN KEY REFERENCES Book,
	UId INT FOREIGN KEY REFERENCES User_Credentials,
	CONSTRAINT Pk_Order PRIMARY KEY (BId, UId,OrderId)
)
GO
--OrderInvoice
CREATE TABLE OrderInvoiceDetails
(
	OrderId NVARCHAR(10) PRIMARY KEY,
	Amount MONEY,
	ShippingAddress NVARCHAR(100)
)
GO
---======================================END OF Section:Orders========================================================---------------------

