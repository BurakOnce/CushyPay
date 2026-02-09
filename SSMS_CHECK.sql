-- SSMS'de çalıştırın: CushyPay veritabanını seçip F5 ile çalıştırın

-- 1. Hangi veritabanındayız?
SELECT DB_NAME() AS CurrentDatabase;
GO

-- 2. Tüm tabloları listele
SELECT 
    TABLE_SCHEMA AS [Schema],
    TABLE_NAME AS [Table Name],
    TABLE_TYPE AS [Type]
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
GO

-- 3. Tablo sayısını kontrol et
SELECT COUNT(*) AS TableCount
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE';
GO

-- 4. Users tablosunun yapısını kontrol et
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users'
ORDER BY ORDINAL_POSITION;
GO

