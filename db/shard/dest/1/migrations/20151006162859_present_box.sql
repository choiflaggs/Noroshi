
-- +goose Up
-- SQL in section 'Up' is executed when this migration is applied

CREATE TABLE `present_box` (
  `ID` int unsigned NOT NULL,
  `PlayerID` int unsigned NOT NULL,
  `PossessionData` text NOT NULL,
  `TextID` text NOT NULL,
  `TextData` text NOT NULL,
  `CreatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`ID`,`CreatedAt`),
  KEY `i1` (`PlayerID`,`CreatedAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE TABLE `present_box_received_log` (
  `ID` int unsigned NOT NULL,
  `PlayerID` int unsigned NOT NULL,
  `PossessionData` text NOT NULL,
  `TextID` text NOT NULL,
  `TextData` text NOT NULL,
  `CreatedAt` int unsigned NOT NULL,
  PRIMARY KEY (`ID`,`CreatedAt`),
  KEY `i1` (`PlayerID`,`CreatedAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- +goose Down
-- SQL section 'Down' is executed when this migration is rolled back

DROP TABLE `present_box`;
DROP TABLE `present_box_received_log`;

