create or alter procedure dbo.GetExportBatch
	@ProcessCode varchar(50),
    @BatchId int = null
as

declare @Today date = getdate();

select 1 as Id, dateadd(day, -1, @Today) as FromDate, @Today as ToDate;

go
