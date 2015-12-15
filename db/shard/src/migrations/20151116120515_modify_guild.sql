
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `guild`
  ADD COLUMN `CooperationPoint` smallint unsigned NOT NULL AFTER `AveragePlayerLevel`,
  ADD COLUMN `CooperationPointUpdatedAt` int unsigned NOT NULL AFTER `CooperationPoint`,
  ADD COLUMN `CooperationPointConsumedNum` smallint unsigned NOT NULL AFTER `CooperationPointUpdatedAt`;

ALTER TABLE `guild_raid_boss`
  ADD COLUMN `EntryData` text NOT NULL AFTER `BattleData`;

ALTER TABLE `guild_activity_daily_log`
  ADD COLUMN `CooperationPoint` smallint unsigned NOT NULL AFTER `DefeatRaidBossNum`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `guild`
  DROP COLUMN `CooperationPoint`,
  DROP COLUMN `CooperationPointUpdatedAt`,
  DROP COLUMN `CooperationPointConsumedNum`;

ALTER TABLE `guild_raid_boss`
  DROP COLUMN `EntryData`;

ALTER TABLE `guild_activity_daily_log`
  DROP COLUMN `CooperationPoint`;

