create or alter procedure dbo.GetExportForBatch
-- TODO: add params
as

declare @Symba table
(
    Birthdate datetime null,
    Claws varchar(10) null,
    Eyes varchar(10) null,
    Nose int not null,
    Teeth int null 
);

insert into @Symba(Birthdate, Claws, Eyes, Nose, Teeth)
values('1993-05-07', 'Sharp', 'Brown', 1, 32),
(null, null, null, 0, null);

select Birthdate, Claws, Eyes, Nose, Teeth
from @Symba;

go
