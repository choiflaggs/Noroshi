
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
ALTER TABLE `player_status` 
ADD COLUMN `LastActionLevelPoint` TINYINT UNSIGNED NOT NULL AFTER `LastGoldRecoveredAt`,
ADD COLUMN `LastActionLevelPointUpdatedAt` INT UNSIGNED NOT NULL AFTER `LastActionLevelPoint`,
ADD COLUMN `LastActionLevelPointRecoveryNum` INT UNSIGNED NOT NULL AFTER `LastActionLevelPointUpdatedAt`,
ADD COLUMN `LastActionLevelPointRecoveredAt` INT UNSIGNED NOT NULL AFTER `LastActionLevelPointRecoveryNum`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
ALTER TABLE `player_status` 
DROP COLUMN `LastActionLevelPoint`,
DROP COLUMN `LastActionLevelPointUpdatedAt`,
DROP COLUMN `LastActionLevelPointRecoveryNum`,
DROP COLUMN `LastActionLevelPointRecoveredAt`;

