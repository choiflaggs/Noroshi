
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

DROP TABLE `character_exp`;
CREATE TABLE `character_level` (
  `Level` smallint unsigned NOT NULL,
  `Exp` int unsigned NOT NULL,
  PRIMARY KEY (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE `player_exp`;
CREATE TABLE `player_level` (
  `Level` smallint unsigned NOT NULL,
  `Exp` int unsigned NOT NULL,
  PRIMARY KEY (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE `player_vip_exp`;
CREATE TABLE `player_vip_level` (
  `Level` smallint unsigned NOT NULL,
  `Exp` int unsigned NOT NULL,
  PRIMARY KEY (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `character_level`;
CREATE TABLE `character_exp` (
  `Level` int(10) unsigned NOT NULL,
  `Exp` int(10) unsigned NOT NULL,
  PRIMARY KEY (`Level`),
  UNIQUE KEY `Level_UNIQUE` (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE `player_level`;
CREATE TABLE `player_exp` (
  `Level` int(10) unsigned NOT NULL,
  `Exp` int(10) unsigned NOT NULL,
  PRIMARY KEY (`Level`),
  UNIQUE KEY `level_UNIQUE` (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE `player_vip_level`;
CREATE TABLE `player_vip_exp` (
  `Level` int(10) unsigned NOT NULL,
  `Exp` int(10) unsigned NOT NULL,
  PRIMARY KEY (`Level`),
  UNIQUE KEY `level_UNIQUE` (`Level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

