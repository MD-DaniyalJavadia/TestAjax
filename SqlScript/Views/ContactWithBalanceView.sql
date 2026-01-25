USE [TestWebApp]
GO

CREATE OR ALTER VIEW dbo.ContactWithBalance
AS
SELECT 
    c.Id AS ContactId,
    c.Name,
    c.PhoneNumber,
    COALESCE(c.PhoneNumber, '-') AS DisplayPhone,
    c.ContactType,
    c.Email,
    c.Address,
    c.Notes,
    c.CreatedAt,
    CONVERT(varchar(10), c.CreatedAt, 105) AS CreatedDateFormatted,
    c.UpdatedAt,
    c.CreatedBy,
    c.IsActive,
    COALESCE(
        SUM(
            CASE 
                WHEN t.Type = 'Received' THEN t.Amount
                WHEN t.Type = 'Given' THEN -t.Amount
                ELSE 0
            END
        ), 
        0.00
    ) AS Balance,
    COUNT(t.Id) AS TotalTransactions,
    MAX(t.TransactionDate) AS LastTransactionDate,
    CONVERT(varchar(10), MAX(t.TransactionDate), 105) AS LastTransactionFormatted,
    CASE 
        WHEN COALESCE(SUM(CASE WHEN t.Type = 'Received' THEN t.Amount ELSE -t.Amount END), 0) > 0 THEN 'ToReceive'
        WHEN COALESCE(SUM(CASE WHEN t.Type = 'Received' THEN t.Amount ELSE -t.Amount END), 0) < 0 THEN 'ToPay'
        ELSE 'Settled'
    END AS BalanceStatus
FROM dbo.Contacts c
LEFT JOIN dbo.Transactions t 
    ON c.Id = t.ContactId
WHERE c.IsActive = 1
GROUP BY 
    c.Id,
    c.Name,
    c.PhoneNumber,
    c.ContactType,
    c.Email,
    c.Address,
    c.Notes,
    c.CreatedAt,
    c.UpdatedAt,
    c.CreatedBy,
    c.IsActive;
GO
