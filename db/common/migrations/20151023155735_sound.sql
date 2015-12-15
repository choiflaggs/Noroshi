
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

CREATE TABLE `sound` (
  `ID` int unsigned NOT NULL,
  `Path` text NOT NULL,
  `ChannelNum` tinyint unsigned NOT NULL,
  `PlayType` tinyint unsigned NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `sound`;

