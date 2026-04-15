-- Migration: Add ProviderId FK to User table for Provider-User linkage
-- Run this on the CareSchedule database before deploying the corresponding code changes.

IF NOT EXISTS (
    SELECT 1 FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND name = 'ProviderID'
)
BEGIN
    ALTER TABLE [dbo].[User] ADD ProviderID INT NULL;

    ALTER TABLE [dbo].[User] ADD CONSTRAINT FK__User__ProviderID
        FOREIGN KEY (ProviderID) REFERENCES [dbo].[Provider](ProviderID)
        ON DELETE SET NULL;
END
GO
