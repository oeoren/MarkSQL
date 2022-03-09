-- exec mq_Products 1

ALTER PROCEDURE [dbo].[mq_Products]( @lookupCategory  int = NULL)
AS

DECLARE @n varchar(10) = CHAR(10);
DECLARE @i varchar(max) = ''

DECLARE @category varchar(20) = 'All'

SELECT @category = CategoryName FROM Categories WHERE CategoryID = @lookupCategory

SELECT @i = '## Products in category ' + @category + @n

SELECT @i = @i + '|Product|Units in stock|price|' + @n;
SELECT @i = @i + '|:-----|-----:|-----:|' + @n;

SELECT @i = @i + '|' + ProductName + '| ' + str(UnitsInStock) + ' | ' + str(UnitPrice) + '|' + @n 
	FROM Products WHERE CategoryID = @lookupCategory OR @lookupCategory IS NULL

SELECT @i = @i + @n + '```json' + @n +
(
SELECT ProductName, UnitsInStock, UnitPrice
	FROM Products 
	WHERE CategoryID = @lookupCategory OR @lookupCategory IS NULL
	FOR JSON AUTO
)

SELECT @i AS MarkDown