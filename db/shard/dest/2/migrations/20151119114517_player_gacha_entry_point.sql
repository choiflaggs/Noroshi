
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

CREATE TABLE `player_gacha_entry_point` (
  `PlayerID` int unsigned NOT NULL,
  `GachaEntryPointID` int unsigned NOT NULL,
  `TotalLotNum` int unsigned NOT NULL,
  `FreeReopenedAt` int unsigned NOT NULL,
  `LastFreeLotNum` tinyint unsigned NOT NULL,
  `LastFreeLottedAt` int unsigned NOT NULL,
  `CreatedAt` int unsigned NOT NULL,
  `UpdatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`GachaEntryPointID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `player_gacha` (
  `PlayerID` int unsigned NOT NULL,
  `GachaID` int unsigned NOT NULL,
  `HitNum` int unsigned NOT NULL,
  `MissLotNum` int unsigned NOT NULL,
  `CreatedAt` int unsigned NOT NULL,
  `UpdatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`GachaID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `player_gacha_entry_point`;
DROP TABLE `player_gacha`;
