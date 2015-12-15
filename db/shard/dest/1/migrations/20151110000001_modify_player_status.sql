-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied
ALTER TABLE `player_status` ADD COLUMN `BeginnerGuildDroppedOutOn` int unsigned DEFAULT '0' AFTER `GuildState`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back
ALTER TABLE `player_status` DROP COLUMN `BeginnerGuildDroppedOutOn`;

