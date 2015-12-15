
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `player_status`
  ADD COLUMN `ArenaPoint` int unsigned NOT NULL AFTER `ChargeGem`,
  ADD COLUMN `GuildPoint` int unsigned NOT NULL AFTER `ArenaPoint`,
  ADD COLUMN `ExpeditionPoint` int unsigned NOT NULL AFTER `GuildPoint`,
  ADD COLUMN `SoulPoint` int unsigned NOT NULL AFTER `ExpeditionPoint`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `player_status`
  DROP COLUMN `ArenaPoint`,
  DROP COLUMN `GuildPoint`,
  DROP COLUMN `ExpeditionPoint`,
  DROP COLUMN `SoulPoint`;

