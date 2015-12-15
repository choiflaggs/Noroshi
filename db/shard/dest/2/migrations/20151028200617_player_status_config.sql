
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
ALTER TABLE `player_status`
 ADD COLUMN `LastBPRecoveryNum` int unsigned NOT NULL AFTER `LastBPUpdatedAt`,
 ADD COLUMN `LastBPRecoveredAt` int unsigned NOT NULL AFTER `LastBPRecoveryNum`,
 ADD COLUMN `LastStaminaRecoveryNum` int unsigned NOT NULL AFTER `LastStaminaUpdatedAt`,
 ADD COLUMN `LastStaminaRecoveredAt` int unsigned NOT NULL  AFTER `LastStaminaRecoveryNum`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
ALTER TABLE `player_status` 
 DROP COLUMN `LastBPRecoveryNum`,
 DROP COLUMN `LastBPRecoveredAt`,
 DROP COLUMN `LastStaminaRecoveryNum`,
 DROP COLUMN `LastStaminaRecoveredAt`;


