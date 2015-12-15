
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

DROP TABLE `player_trial`;
CREATE TABLE `player_trial` (
  `PlayerID` int unsigned NOT NULL,
  `TrialID` int unsigned NOT NULL,
  `ClearLevel` tinyint unsigned NOT NULL,
  `LastBattleNum` tinyint unsigned NOT NULL,
  `LastBattledAt` int unsigned NOT NULL,
  `ReopenedAt` int unsigned NOT NULL,
  `CreatedAt` int unsigned NOT NULL,
  `UpdatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`TrialID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `player_trial`;
CREATE TABLE `player_trial` (
  `PlayerID` int(10) unsigned NOT NULL,
  `TrialID` int(10) unsigned NOT NULL,
  `PlayCount` int(10) unsigned NOT NULL,
  `PlayMaxCount` int(10) unsigned NOT NULL,
  `CoolTime` int(10) unsigned NOT NULL,
  `ResetAt` int(10) unsigned NOT NULL,
  `CreatedAt` int(10) unsigned NOT NULL,
  `UpdatedAt` int(10) unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`TrialID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

