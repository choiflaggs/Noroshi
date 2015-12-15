
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

ALTER TABLE `player_status`
  ADD COLUMN `GuildState` tinyint unsigned NOT NULL AFTER `GuildRole`;

ALTER TABLE `guild`
  ADD COLUMN `RequestNum` tinyint unsigned NOT NULL AFTER `MemberNum`;

CREATE TABLE `guild_request` (
  `PlayerID` int unsigned NOT NULL,
  `GuildID` int unsigned NOT NULL,
  `CreatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`),
  KEY `i1` (`GuildID`,`CreatedAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

ALTER TABLE `player_status`
  DROP COLUMN `GuildState`;

ALTER TABLE `guild`
  DROP COLUMN `RequestNum`;

DROP TABLE `guild_request`;

