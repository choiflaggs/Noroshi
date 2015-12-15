
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
ALTER TABLE `player_vip_level_bonus` 
DROP COLUMN `MaxBPRecoveryNum`;


-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
ALTER TABLE `player_vip_level_bonus` 
ADD COLUMN `MaxBPRecoveryNum` SMALLINT UNSIGNED NOT NULL AFTER `MaxStaminaRecoveryNum`;

