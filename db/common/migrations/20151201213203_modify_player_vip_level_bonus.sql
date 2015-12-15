
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
ALTER TABLE `player_vip_level_bonus` 
CHANGE COLUMN `GuildBossPointBonus` `GuildPointBonus` FLOAT UNSIGNED NOT NULL;


-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
ALTER TABLE `player_vip_level_bonus` 
CHANGE COLUMN `GuildPointBonus` `GuildBossPointBonus` FLOAT UNSIGNED NOT NULL;

