CREATE DATABASE savedDatabase ON  
( 
    NAME = SampleDB,  -- Logical File Name
    FILENAME = 'C:\Users\jt4d\Desktop\OOHotel.ss' 
)  
AS SNAPSHOT OF SampleDB;  
GO