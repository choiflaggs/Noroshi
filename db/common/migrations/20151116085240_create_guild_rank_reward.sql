
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

CREATE TABLE `guild_rank_reward` (
  `GuildRank` tinyint unsigned NOT NULL,
  `No` tinyint unsigned NOT NULL,
  `PossessionCategory` tinyint unsigned NOT NULL,
  `PossessionID` int unsigned NOT NULL,
  `PossessionNum` int unsigned NOT NULL,
  PRIMARY KEY (`GuildRank`,`No`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

ALTER TABLE `cpu_character`
  ADD COLUMN `FixedMaxHP` int unsigned NOT NULL AFTER `EvolutionLevel`,
  ADD COLUMN `InitialEnergy` smallint unsigned NOT NULL AFTER `FixedMaxHP`;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `guild_rank_reward`;

ALTER TABLE `cpu_character`
  DROP COLUMN `FixedMaxHP`,
  DROP COLUMN `InitialEnergy`;

