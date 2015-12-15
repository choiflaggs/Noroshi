
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `guild_ranking_reference` RENAME `ranking_reference`;

DROP TABLE `guild_ranking_1`;
CREATE TABLE `guild_ranking_1_a` (
  `GuildID` int unsigned NOT NULL,
  `UniqueRank` int unsigned NOT NULL,
  `Rank` int unsigned NOT NULL,
  `Value` int NOT NULL,
  PRIMARY KEY (`GuildID`),
  UNIQUE KEY `i1` (`UniqueRank`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
DROP TABLE `guild_ranking_2`;
CREATE TABLE `guild_ranking_1_b` (
  `GuildID` int unsigned NOT NULL,
  `UniqueRank` int unsigned NOT NULL,
  `Rank` int unsigned NOT NULL,
  `Value` int NOT NULL,
  PRIMARY KEY (`GuildID`),
  UNIQUE KEY `i1` (`UniqueRank`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `ranking_reference` RENAME `guild_ranking_reference`;

DROP TABLE `guild_ranking_1_a`;
CREATE TABLE `guild_ranking_1` (
  `GuildID` int unsigned NOT NULL,
  `UniqueRank` int unsigned NOT NULL,
  `Rank` int unsigned NOT NULL,
  `Value` int unsigned NOT NULL,
  PRIMARY KEY (`GuildID`),
  UNIQUE KEY `i1` (`UniqueRank`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
DROP TABLE `guild_ranking_1_b`;
CREATE TABLE `guild_ranking_2` (
  `GuildID` int unsigned NOT NULL,
  `UniqueRank` int unsigned NOT NULL,
  `Rank` int unsigned NOT NULL,
  `Value` int unsigned NOT NULL,
  PRIMARY KEY (`GuildID`),
  UNIQUE KEY `i1` (`UniqueRank`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
