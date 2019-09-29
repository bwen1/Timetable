

USE timetable_app;

CREATE TABLE IF NOT EXISTS `users` (
			`ID` int(10) auto_increment NOT NULL,
            `Name` varchar(30),
            `passwordHash` char(64),
            primary key (`ID`));
            
 drop table friends;           
create TABLE IF NOT EXISTS friends (
	`ID1` int(10) NOT NULL,
    `ID2` int(10) NOT NULL,
    `status` enum('NO', 'PENDING_TO', 'PENDING_FROM', 'BLOCKED_TO', 'BLOCKED_FROM', 'YES'),
    primary key (`ID1`, `ID2`),
    foreign key (`ID1`) references users (`ID`));


CREATE TABLE IF NOT EXISTS `event` (
	`EventID` int(10) auto_increment NOT NULL,
    `ID` int(10) NOT NULL,
    `shared` bool,
    `EventName` varchar(120),
	`notes` varchar(120),
    `Location` varchar(50),
    `TimeStart` int(4),
    `TimeEnd` int(4),
    `Day` enum('Mon', 'Tue', 'Wed', 'Thur', 'Fri', 'Sat', 'Sun'),
    primary key (`EventID`),
    foreign key (`ID`) references users (`ID`));
    

#drop table if exists `friends`;



    
insert into `users` (`Name`, `passwordHash`) values ( 'John', 'CDEC15F02A7D1045D58F40EBC6DD5433F52EEADCDB27942451CD70750D044FC2');
insert into `users` (`Name`, `passwordHash`) values ( 'Emily', '789DF7F8F252858F5E96422333E2430DCB6371FB8A0472AA31511B4DDE5CB873');
insert into `users` (`Name`, `passwordHash`) values ( 'Bob', '789DF7F8F252858F5E96422333E2430DCB6371FB8A0472AA31511B4DDE5CB873');
insert into `users` (`Name`, `passwordHash`) values ( 'Jane', '65E84BE33532FB784C48129675F9EFF3A682B27168C0EA744B2CF58EE02337C5');

insert into `event` (`EventID`, `ID`, `shared`,`EventName`, `notes`, `Location`, `TimeStart`, `TimeEnd`, `Day`) values (100, 1,true,'IAB330', 'Mobile aplication development', 'S-514', 1500, 1700, 'Wed');
insert into `event` (`ID`, `shared`,`EventName`, `notes`, `Location`, `TimeStart`, `TimeEnd`, `Day`) values ( 1,true,'IAB130', 'Mobile and ubiqudus computing', 'P-508', 1700, 1900, 'Thur');
insert into `event` (`ID`, `shared`,`EventName`, `notes`, `Location`, `TimeStart`, `TimeEnd`, `Day`) values ( 1,true,'IAB130', 'Mobile and ubiqudus computing', 'Z-402', 1100, 1300, 'Mon');

insert into `event` (`ID`, `shared`,`EventName`, `notes`, `Location`, `TimeStart`, `TimeEnd`, `Day`) values ( 2,true,'IAB330', 'Mobile aplication development', 'S-516', 0900, 1100, 'Tue');
insert into `event` (`ID`, `shared`,`EventName`, `notes`, `Location`, `TimeStart`, `TimeEnd`, `Day`) values ( 2,false,'IFB103', 'Raspberry PI stuff', 'Z-620', 1600, 1800, 'Wed');
insert into `event` (`ID`, `shared`,`EventName`, `notes`, `Location`, `TimeStart`, `TimeEnd`, `Day`) values ( 2,true,'IAB130', 'Mobile and ubiqudus computing', 'Z-402', 1100, 1300, 'Mon');

insert into `event` (`ID`, `shared`,`EventName`, `notes`, `Location`, `TimeStart`, `TimeEnd`, `Day`) values ( 3,true,'IAB330', 'Mobile aplication development', 'S-514', 1500, 1700, 'Wed');
insert into `event` (`ID`, `shared`,`EventName`, `notes`, `Location`, `TimeStart`, `TimeEnd`, `Day`) values ( 3,true,'IFB130', 'Databases!', 'Q-426', 1400, 1500, 'Fri');
insert into `event` (`ID`, `shared`,`EventName`, `notes`, `Location`, `TimeStart`, `TimeEnd`, `Day`) values ( 3,false,'IAB130', 'Mobile and ubiqudus computing', 'Z-501', 1100, 1300, 'Tue');

insert into friends values (1,2,'YES');
insert into friends values (3,2,'YES');
#insert into friends values (2,3,'pending');
insert into friends values (3,1,'PENDING_TO');

UPDATE `users` SET passwordHash = 'value' WHERE `ID` = 1;

#select `ID` FROM `users` WHERE `Name` = 'Emily' AND `passwordHash` = '789DF7F8F252858F5E96422333E2430DCB6371FB8A0472AA31511B4DDE5CB873'; # login query

#select * FROM `event` WHERE `ID` = 3 OR (`ID` = ((select `ID2` FROM friends WHERE `ID1` = 3 AND `status` = 'friend') OR (select `ID1` FROM friends WHERE `ID2` = 3 AND `status` = 'friend'))); # select your and friends events

use timetable_app;
#this is the problematic code \/
SELECT `Name`, `ID`, `status` AS `!status` FROM (SELECT `Name`, `ID` FROM `users` WHERE `ID` IN (SELECT `ID1` FROM `friends` WHERE `ID2` = 7)) AS T
INNER JOIN `friends` ON `ID` = `ID1` ;group by `Name`;

SELECT `Name`, `ID`, `status`  FROM (SELECT `Name`, `ID` FROM `users` WHERE `ID` IN (SELECT `ID2` FROM `friends` WHERE `ID1` = 4)) AS T
INNER JOIN `friends` ON `ID` = `ID2` group by `Name`;


SELECT `Name` `ID` FROM `users` WHERE `ID` IN (SELECT `ID1` FROM `friends` WHERE `ID2` = 1);
SELECT `Name` `ID` FROM `users` WHERE `ID` = (SELECT `ID1` FROM `friends` WHERE `ID2` = 2);

SELECT * FROM `event` WHERE `ID` IN (2,3) AND `shared` = true;

UPDATE `events` SET `shared` = true, EventName = 'john ballet', notes = 'john as a thing for ballet', Location = 'not near john, thats where!', TimeStart = 100, TimeEnd = 2359, `Day` = '

select * FROM `users` WHERE `Name` = 'Jane' LIMIT 1;
#select * from `users`;
select * from `friends`;
#SELECT EXISTS(select * FROM `users` WHERE `Name` = '" + username + " AND `passwordHash' = '"+psh+"') AS `Exists`

SELECT EXISTS(select * FROM `users` WHERE `Name` = 'Jane' AND `passwordHash` = '65E84BE33532FB784C48129675F9EFF3A682B27168C0EA744B2CF58EE02337C5') AS `Exists`; # check to see if a username is avalible