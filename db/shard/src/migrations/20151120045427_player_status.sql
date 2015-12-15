
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
ALTER TABLE `player_status` 
ADD COLUMN `LastGoldRecoveryNum` INT UNSIGNED NOT NULL AFTER `LastStaminaRecoveredAt`,
ADD COLUMN `LastGoldRecoveredAt` INT UNSIGNED NOT NULL AFTER `LastGoldRecoveryNum`;


-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
ALTER TABLE `player_status` 
DROP COLUMN `LastGoldRecoveryNum`,
DROP COLUMN `LastGoldRecoveredAt`;

