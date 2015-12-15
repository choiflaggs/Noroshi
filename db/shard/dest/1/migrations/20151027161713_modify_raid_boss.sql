
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `guild_raid_boss`
  ADD COLUMN `DefeatedAt` int unsigned NOT NULL AFTER `EntryData`,
  ADD KEY `i2` (`GuildID`,`State`,`DefeatedAt`);

ALTER TABLE `guild_activity_daily_log`
  ADD COLUMN `DefeatRaidBossNum` int unsigned NOT NULL AFTER `BPConsuming`;

ALTER TABLE `guild`
  DROP COLUMN `ChatID`;

CREATE TABLE `guild_ranking_reference` (
  `RankingID` int unsigned NOT NULL,
  `ReferenceID` tinyint unsigned NOT NULL,
  `UpdatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`RankingID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `guild_ranking_1` (
  `GuildID` int unsigned NOT NULL,
  `UniqueRank` int unsigned NOT NULL,
  `Rank` int unsigned NOT NULL,
  `Value` int unsigned NOT NULL,
  PRIMARY KEY (`GuildID`),
  UNIQUE KEY `i1` (`UniqueRank`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
CREATE TABLE `guild_ranking_2` (
  `GuildID` int unsigned NOT NULL,
  `UniqueRank` int unsigned NOT NULL,
  `Rank` int unsigned NOT NULL,
  `Value` int unsigned NOT NULL,
  PRIMARY KEY (`GuildID`),
  UNIQUE KEY `i1` (`UniqueRank`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `guild_raid_boss`
  DROP COLUMN `DefeatedAt`,
  DROP KEY `i2`;

ALTER TABLE `guild_activity_daily_log`
  DROP COLUMN `DefeatRaidBossNum`;

ALTER TABLE `guild`
  ADD COLUMN `ChatID` text NOT NULL AFTER `RequestNum`;

DROP TABLE `guild_ranking_reference`;
DROP TABLE `guild_ranking_1`;
DROP TABLE `guild_ranking_2`;
