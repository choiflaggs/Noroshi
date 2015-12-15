
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

CREATE TABLE `player_device_token` (
  `PlayerID` int unsigned NOT NULL,
  `Type` tinyint unsigned NOT NULL,
  `Status` tinyint unsigned NOT NULL,
  `Token` varchar(100) NOT NULL,
  `CreatedAt` int unsigned NOT NULL,
  `UpdatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`PlayerID`,`Type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `player_device_token`;

