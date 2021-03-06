ALTER   procedure [dbo].[GetExportForBatch]
-- TODO: add params
as

set nocount on;

declare @Symba table
(
    Id int not null,
    Birthdate datetime null,
    Claws varchar(10) null,
    Eyes varchar(10) null,
    Nose int not null,
    Teeth int null 
);

declare @Nallah table
(
    LionId int not null,
    BestFriend nvarchar(50) null,
    Anniversary datetime null
);

insert into @Symba(Id, Birthdate, Claws, Eyes, Nose, Teeth)
values(1, '1993-05-07', 'Sharp', 'Brown', 1, 32),
(2, null, null, null, 0, null);

select Id, Birthdate, Claws, Eyes, Nose
from @Symba;

select Id, Teeth, 'QuestionTag1~Tag2' as RecordTypeKey
from @Symba;

insert into @Nallah(LionId, BestFriend, Anniversary)
values(1, 'Symba', '1996-04-14'),
(2, 'Zazu', null),
(3, 'Rafiki', '1968-08-07');

select LionId, BestFriend, Anniversary
from @Nallah;

